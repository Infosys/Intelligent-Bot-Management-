/****************************************************************
 * This file is a part of the Legacy Integration Framework.
 * This file implements IAdapter interface to use HIS as communication medium.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/

using System;

using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using Infosys.Lif.LegacyCommon;
using Infosys.Lif.LegacyIntegratorService;


namespace Infosys.Lif.TCPIP
{


    /*******************************************************
     * 
     * This class will try to build the initial conneciton message in the format:
     * 01 CLIENT-IN-DATA.
     * 05 CID-USERID                     PIC X(8).
     * 05 CID-PASSWORD                   PIC X(8).
     * 05 CID-LINK-TO-PROG               PIC X(8).
     * 05 CID-COMMAREA-LEN               PIC S9(4) COMP.
     * 05 CID-DATA-LEN                   PIC S9(8) COMP.
     * 05 CID-VERSION                    PIC X.
     * 	88 CID-VERSION-1               VALUE X'00'.
     * 	88 CID-VERSION-2               VALUE X'01'.
     * 05 CID-FLAGS                      PIC X(2).
     * 	88 CID-FLAGS-PERSISTENT-NONE   VALUE X'0001'.
     * 	88 CID-FLAGS-PERSISTENT-OPEN   VALUE X'0002'.
     * 	88 CID-FLAGS-PERSISTENT-USE    VALUE X'0004'.
     * 	88 CID-FLAGS-PERSISTENT-CLOSE  VALUE X'0008'.
     * 05 CID-FORMAT                     PIC X(1).
     * 	88 CID-FORMAT-NOTSET           VALUE X'00'.
     * 	88 CID-FORMAT-MS               VALUE X'01'.
     * 	88 CID-FORMAT-IBM              VALUE X'02'.

     ******************************************************/

    public sealed class TCPIPAdapter : IAdapter
    {


        #region Configuration parameters - Start
        /// <summary>
        /// The connection mode to be used
        /// </summary>
        private Constants.ConnectionMode connectionMode;

        /// <summary>
        /// The ID of the transaction which needs to be executed for the concurrent listener.
        /// A concurrent server is usually executed on the mainframe. It will have eben assigned 
        /// some transation id. Eg. For MS provided we use MSCS.
        /// </summary>
        private string transactionId = string.Empty;

        /// <summary>
        /// The concurrent server will pass the request to a protocol handler. This protocol handler will
        /// be a program with a program id on the mainframe. This variable will store the program id.
        /// </summary>
        private string hostProgId = string.Empty;

        /// <summary>
        /// Indicates whetther tracign should be turned on or off.
        /// </summary>
        private bool isTraceEnabled = false;

        /// <summary>
        /// Indicates whether performance traces should be logged
        /// </summary>
        private bool isPerfTraceEnabled = false;


        private IPAddress mainframeIP = IPAddress.None;

        private int portNumber = 0;

        /// <summary>
        /// Timeout in microseconds
        /// </summary>
        private int readTimeout = 0;

        private Connection connectionModel = null;


        #endregion Configuration parameters - End


        /// <summary>
        /// Socket to be used to send all the messages
        /// </summary>
        Socket tcpSocket = null;


        #region IAdapter Members
        public event ReceiveHandler Received;

        /// <summary>
        /// Is the metho
        /// </summary>
        /// <param name="adapterDetails"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public string Send(System.Collections.Specialized.ListDictionary adapterDetails, string message)
        {

            /// The Infy Mainframe requires 8 bytes extra to be sent initially.
            /// This is because in LEARNPRG we need to send across S9(4) COMP 2 times

            message  = string.Empty.PadLeft(8, ' ') + message;




            string errorInConfigFile = RetrieveConfigFileErrors(adapterDetails);

            if (!string.IsNullOrEmpty(errorInConfigFile))
            {
                throw new LegacyException("TCP/IP Configuration Exception: " + errorInConfigFile);
            }


            StringBuilder stringBuilder = new StringBuilder(40);

            stringBuilder.Append(Pad(transactionId, 4));//transaction id

            stringBuilder.Append(Constants.MessageSeperator);// seperator

            stringBuilder.Append(Pad(RetrieveFromTls(Constants.UserIdTls), 8));//user id 

            stringBuilder.Append(Pad(RetrieveFromTls(Constants.PasswordTls), 8));//password

            stringBuilder.Append(Pad(hostProgId, 8));//program name

            stringBuilder.Append(Pad(string.Empty, 11)); // Padding so that encoding results in 40 bytes


            byte[] encodedBytes = EncodeToEBCDIC(stringBuilder.ToString());
            // we are doing the encoding before setting the length. 
            // if we set the length of the commarea and then encode the string, it will encode the length to
            // some other number

            if (message.Length > ushort.MaxValue)
            {
                throw new LegacyException("TCP/IP Adapter Exception: Message length cannot be greater than " + ushort.MaxValue);
            }

            {

                // Commarea fields

                // higher order of the integer
                encodedBytes[29] = (byte)(message.Length >> 8);

                // lower order of the integer
                encodedBytes[30] = (byte)(message.Length & 0xFF);
            }

            {

                // Data area fields
                encodedBytes[31] = 0;//need to find out what data area is
                encodedBytes[32] = 0;//need to find out what data area is
                encodedBytes[33] = 0;
                encodedBytes[34] = 0;
                encodedBytes[35] = 0;
            }
            {
                // setting the CID version to 00 if standard else 1
                encodedBytes[36] = (connectionMode == Constants.ConnectionMode.Standard) ? (byte)0 : (byte)1;
            }

            {
                // setting the Persistence
                encodedBytes[38] = 0;
                encodedBytes[38] = GetPersistenceMode();
            }

            {
                //setting the CID format as NOT Set
                // Need to see what we can do about this field
                encodedBytes[39] = 0;
            }

            tcpSocket = GetConnectedTcpSocket();

            TryConnectionInitiationMessage(encodedBytes);
            // the connection has succeeded


            string receivedMessage = SendMessage(message);


            if (GetConnectionModel() == ConnectionModelType.None)
            {
                tcpSocket.Close();
            }


            return receivedMessage.Substring(8);

        }

        public void Receive(System.Collections.Specialized.ListDictionary adapterDetails)
        {
            throw new NotImplementedException();
        }

        public bool Delete(System.Collections.Specialized.ListDictionary messageDetails)
        {
            throw new NotImplementedException();
        }

        private string SendMessage(string message)
        {
            byte[] messageAsBytes = EncodeToEBCDIC(message);

            tcpSocket.Send(messageAsBytes);

            string receivedString = string.Empty;
            while (tcpSocket.Poll(readTimeout, SelectMode.SelectRead) && tcpSocket.Available > 0)
            {

                byte[] receivedMessageAsBytes = new byte[tcpSocket.Available];



                tcpSocket.Receive(receivedMessageAsBytes);

                receivedString += Encoding.GetEncoding(Constants.Ebcdic_Code_Page_Number).GetString(receivedMessageAsBytes);

            }

            if (receivedString.Length == 0)
            {
                // the read failed.
                throw new LegacyException("TCP/IP Adapter exception : Receive timed out");
            }
            return receivedString;


        }


        private void TryConnectionInitiationMessage(byte[] encodedBytes)
        {
            if (GetConnectionModel() == ConnectionModelType.None)
            {
                tcpSocket.Send(encodedBytes);

                if (tcpSocket.Poll(readTimeout, SelectMode.SelectRead))
                {

                    byte[] receivedBytes = new byte[tcpSocket.Available];

                    int lengthOfMessageReceived = tcpSocket.Receive(receivedBytes);


                    // the first 4 bytes will contain a 0 as the value. string end is indicated by character 0.
                    // so we should not convert the first 4 characters.
                    string receivedString = Encoding.GetEncoding(Constants.Ebcdic_Code_Page_Number).GetString(receivedBytes, 4, receivedBytes.Length - 4);

                    if (receivedString.StartsWith("EZY", StringComparison.InvariantCultureIgnoreCase))
                    {
                        // An error message has been sent by the mainframe
                        throw new LegacyException("TCP/IP Adapter exception : Mainframe responded incorrectly. Mainframe returned : " + receivedString);
                    }
                }
                else
                {
                    // the read failed.
                    throw new LegacyException("TCP/IP Adapter exception : Receive timed out");
                }

            }
        }

        private ConnectionModelType GetConnectionModel()
        {
            switch (connectionModel.ConnectionModel)
            {
                case "None":
                    return ConnectionModelType.None;
                case "ConnectionPool":
                    return ConnectionModelType.ConnectionPool;
            }
            return ConnectionModelType.None;
        }

        private byte GetPersistenceMode()
        {
            return 1;
        }


        private byte[] EncodeToEBCDIC(string asciiString)
        {
            System.Text.Encoding encoding = System.Text.Encoding.GetEncoding(Constants.Ebcdic_Code_Page_Number);
            byte[] stringAsBytes = encoding.GetBytes(asciiString);
            return stringAsBytes;
        }

        private string RetrieveFromTls(string tlsParameterName)
        {

            LocalDataStoreSlot dataSlot = Thread.GetNamedDataSlot(tlsParameterName);

            if (dataSlot != null)
            {
                string retrievedParameter = (string)Thread.GetData(dataSlot);

                if (retrievedParameter != null)
                {
                    return retrievedParameter;
                }
            }

            // if dataSlot is null or retrievedParamater is null a string.Empty will be returned.
            return string.Empty;

        }

        /// <summary>
        /// Method to pad the strings to a total width.
        /// </summary>
        /// <param name="stringToBePadded"></param>
        /// <param name="totalWidth"></param>
        /// <returns></returns>
        private string Pad(string stringToBePadded, int totalWidth)
        {
            stringToBePadded = stringToBePadded.PadRight(totalWidth, ' ');

            return stringToBePadded;
        }

        /// <summary>
        /// Returns a connection to the mainframe.
        /// This will be either from a pool. if ConnectionPool is being used.
        /// Will be a new connection to the pool otherwise.
        /// </summary>
        /// <returns></returns>
        private Socket GetConnectedTcpSocket()
        {
            Socket socket = new Socket(AddressFamily.Unspecified, SocketType.Stream, ProtocolType.Tcp);
            if (GetConnectionModel() == ConnectionModelType.None)
            {
                // Open a new conection with the mainframe. 
                IPEndPoint ipEndPoint = new IPEndPoint(mainframeIP, portNumber);
                socket.Connect(ipEndPoint);
            }
            else
            {
                // retrieve connection from pool.
            }

            return socket;
        }





        #endregion


        private string RetrieveConfigFileErrors(System.Collections.Specialized.ListDictionary adapterDetails)
        {


            Region regionToBeUsed = (Region)adapterDetails["Region"];
            LegacyIntegratorService.TCPIP tcpIpTransportSection = (LegacyIntegratorService.TCPIP)adapterDetails["TransportSection"];

            TCPIPDetails currentTCPIPDetail = null;

            foreach (TCPIPDetails detail in tcpIpTransportSection.TCPIPDetailsCollection)
            {
                // Cross Check the regionToBeUsed.TransportName with the detail.TransportName
                // Currently i dont have a variable named TransportName
                currentTCPIPDetail = detail;
                break;
            }




            // Mode Check Start
            if (currentTCPIPDetail.Mode == Constants.ConnectionMode.Standard.ToString())
            {
                connectionMode = Constants.ConnectionMode.Standard;
            }
            else if (currentTCPIPDetail.Mode == Constants.ConnectionMode.Enhanced.ToString())
            {
                connectionMode = Constants.ConnectionMode.Enhanced;
            }
            else
            {
                return "Invalid Mode. Options are Standard/Enhanced";
            }
            // Mode Check End

            // transaction id check - Start
            if (!string.IsNullOrEmpty(currentTCPIPDetail.TransactionID) &&
                currentTCPIPDetail.TransactionID.ToString().Length < 5)
            {
                transactionId = currentTCPIPDetail.TransactionID;
            }
            else if (connectionMode == Constants.ConnectionMode.Standard)
            {
                // if connection mode of enhanced, allow trnsaction id to be null
                return "Invalid Transaction ID. POssible values should be between 1 and 4 characters in length";
            }
            // transaction id check - End


            // cobol prog id check - Start
            if (!string.IsNullOrEmpty(currentTCPIPDetail.HostProgID) &&
                currentTCPIPDetail.HostProgID.Length < 9)
            {
                hostProgId = currentTCPIPDetail.HostProgID;
            }
            else
            {
                return "Invalid Host Program ID. Possible values should be between 1 and 8 characters in length";
            }
            // cobol prog id check - End


            if (currentTCPIPDetail.EnableTrace == Constants.TrueIndicator
                || currentTCPIPDetail.EnableTrace == Constants.FalseIndicator)
            {
                isTraceEnabled = (currentTCPIPDetail.EnableTrace == Constants.TrueIndicator);
            }
            else
            {
                return "Invalid value for EnableTrace. Possible values are " + Constants.TrueIndicator + "/" + Constants.FalseIndicator + ".";
            }


            if (currentTCPIPDetail.EnablePerformanceCounters == Constants.TrueIndicator
                || currentTCPIPDetail.EnablePerformanceCounters == Constants.FalseIndicator)
            {
                isPerfTraceEnabled = currentTCPIPDetail.EnablePerformanceCounters == Constants.TrueIndicator;
            }
            else
            {
                return "Invalid value for EnableTrace. Possible values are " + Constants.TrueIndicator + "/" + Constants.FalseIndicator + ".";
            }


            if (currentTCPIPDetail.EnablePerformanceCounters == Constants.TrueIndicator
                || currentTCPIPDetail.EnablePerformanceCounters == Constants.FalseIndicator)
            {
                isPerfTraceEnabled = (currentTCPIPDetail.EnablePerformanceCounters == Constants.TrueIndicator);
            }
            else
            {
                return "Invalid value for EnableTrace. Possible values are " + Constants.TrueIndicator + "/" + Constants.FalseIndicator + ".";
            }

            if (!IPAddress.TryParse(currentTCPIPDetail.IPAddress, out mainframeIP))
            {
                return "Invalid IP Address. Please use the IP address of the mainframe";
            }


            if (currentTCPIPDetail.Port <= 0)
            {
                return "Invalid port number of mainframe. Port number should be an integer";
            }
            else
            {
                portNumber = currentTCPIPDetail.Port;
            }
            
            
            if (currentTCPIPDetail.TimeOut <= 0)
            {
                return "Invalid timeout. timeout should be in number of milliseconds";
            }
            else
            {
                // convert to microseconds.
                readTimeout = currentTCPIPDetail.TimeOut * 1000;
            }


            connectionModel = currentTCPIPDetail.Connection;


            return null;
        }

        
    }
}
