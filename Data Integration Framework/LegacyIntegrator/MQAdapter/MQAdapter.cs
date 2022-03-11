/****************************************************************
 * This file is a part of the Legacy Integration Framework.
 * This file implements IAdapter interface to use IBM MQ as communication medium.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/


/******************************
 * Sachin Nayak - 
 * While compiling this assembly for MQSeries client version 6.0 please use preprocessor 
 * directive MQClientVersion6
 *****************************/


using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Text;
using System.Globalization;
//using System.Diagnostics;
using System.Threading;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using IBM.WMQ;
using Infosys.Lif.LegacyCommon;
using Infosys.Lif.LegacyIntegratorService;

/// Added by Sachin  --  Start
using System.Runtime.InteropServices;
/// Added by Sachin  --  End


namespace Infosys.Lif.IBMMQ
{
    #region MQAdapter
    /// <summary>
    /// Implements communication using IMB MQ.
    /// </summary>
    public class MQAdapter : IAdapter
    {
        /// Added by Sachin  --  Start
        /// This is because there is some bug with related to DateTime.Now.Ticks
        /// as told in the article http://www.codeproject.com/dotnet/perfcounter.asp
        [DllImport("Kernel32.dll")]
        public static extern void QueryPerformanceCounter(ref long ticks);
        /// Added by Sachin   --  End

        /// Added by Sachin   --  Start
        /// Prevents objects from registering the AppDOmain Unload more than once.
        static bool AppDomainUnloadRegistered = false;
        /// Added by Sachin   --  End


        #region private fields
        private MQQueueManager mqQueueManager;
        private MQQueue mqResponseQueue;
        private MQQueue mqRequestQueue;
        private MQConnectionInformation connectionString;
        private bool isConnectionPool;
        private string queueManagerName;
        private string requestQueueName;
        private string responseQueueName;
        private int timeOut;
        private string queueType;
        private string Persistence;
        private byte[] correlationID;
        private string queueNameModel;
        private string channelName;
        private string connectionName;
        private int port;
        private int expiryTime;
        private int minSize;
        private int maxSize;
        private int connectionTimeOut;
        private bool isTraceRequired;
        private string sslRequired;
        private string certificatePath;
        private string encryptionType;

        //Added for Retry
        private int maxRetry;
        private string retryAll;

        ///Sachin commented out Start -- This counter serves no purpose
        /// <summary>
        /// Counts the number of calls which the MQ has handled
        /// </summary>
        /// private static int numberOfCalls;
        /// <summary>
        /// Serves as a synchronization object 
        /// </summary>
        /// static object syncObject = new Object();
        ///Sachin commented out End -- This counter serves no purpose


        private double interval;

        /// Sachin Commented Start -- Removal of logging all entries
        /// <summary>
        /// Used to write Log entries
        /// </summary>
        /// LogEntry logEntry = new LogEntry();
        /// Sachin Commented End -- Removal of logging all entries

        ////// Create performance counter objects.
        ////PerformanceCounter totalRequest = new PerformanceCounter();
        ////PerformanceCounter averageRequest = new PerformanceCounter();
        ////PerformanceCounter requestSize = new PerformanceCounter();
        ////PerformanceCounter numberOfExceptions = new PerformanceCounter();
        ////PerformanceCounter averageTime = new PerformanceCounter();
        ////PerformanceCounter averageBase = new PerformanceCounter();
        ////PerformanceCounter totalResponse = new PerformanceCounter();
        ////PerformanceCounter averageResponse = new PerformanceCounter();
        ////PerformanceCounter responseSize = new PerformanceCounter();
        ////PerformanceCounter timeToGetAConnection = new PerformanceCounter();
        ////PerformanceCounter timeToGetAConnectionBase = new PerformanceCounter();

        #endregion

        # region CONSTANTS
        // Parameter related constants
        private const string REGION = "Region";
        private const string TRANSPORT_SECTION = "TransportSection";

        // Queue related constants
        const string DYNAMIC = "Dynamic";
        const string STATIC = "Static";

        // Unique name for a slot in TLS
        const string SLOT_NAME = "LifHostConnect";

        // Performance counter related
        const string CATEGORY = "LegacyIntegrator";
        const string TOTAL_REQUESTS = "MQ - TotalNumberofRequest";
        const string TOTAL_RESPONSES = "MQ - TotalNumberofResponse";
        const string AVERAGE_REQUEST = "MQ - AverageNumberOfRequest";
        const string AVERAGE_RESPONSE = "MQ - AverageNumberOfResponse";
        const string AVERAGE_TIME = "MQ - AverageTime";
        const string AVERAGE_BASE = "MQ - AverageBase";
        const string REQUEST_SIZE = "MQ - RequestLength";
        const string RESPONSE_SIZE = "MQ - ResponseLength";
        const string TOTAL_ACTIVE_CONNECTIONS = "MQ - TotalActiveConnections";
        const string ACTIVE_CONNECTIONS_PER_POOL = "MQ - ActiveConnectionsPerPool";
        const string NUMBER_OF_EXCEPTIONS = "MQ - NumberOfException";
        const string TIME_TO_GET_A_CONNECTION = "MQ - TimeToGetAConnection";
        static string counterInstance = string.Empty;
        static string COUNTER_INSTANCE
        {
            get
            {
                if (counterInstance.Length == 0)
                {
                    counterInstance = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                }
                return counterInstance;
            }
        }
        // Trace category
        const string TRACE_CATEGORY = "LegacyIntegrator";

        /// Sachin Commented Start -- Serves no purpose
        // Tracing for debugging
        ///const string DEBUG_INFO = "MQAdapterInformation";
        ///Sachin Commented End -- 

        #endregion

        #region Methods

        #region Initialize
        /// <summary>
        /// Sets the transport details into private variables.
        /// </summary>
        /// <param name="ibmMQDetails">Dictionary containing transport details</param>
        private void Initialize(Infosys.Lif.LegacyIntegratorService.IBMMQDetails ibmMQDetails)
        {

            //store the configuration information to the private fields
            queueManagerName = ibmMQDetails.QueueManager as string;
            requestQueueName = ibmMQDetails.RequestQueue as string;
            responseQueueName = ibmMQDetails.ResponseQueue as string;
            timeOut = Convert.ToInt32(ibmMQDetails.TimeOut, NumberFormatInfo.CurrentInfo);
            expiryTime = Convert.ToInt32(ibmMQDetails.ExpiryTime, NumberFormatInfo.CurrentInfo);
            queueNameModel = ibmMQDetails.ModelQueueName as string;
            queueType = ibmMQDetails.QueueType as string;
            Persistence = ibmMQDetails.Persistence as string;
            channelName = ibmMQDetails.ChannelName as string;
            connectionName = ibmMQDetails.ConnectionName as string;
            port = ibmMQDetails.Port;
            sslRequired = ibmMQDetails.SSLRequired as string;

            maxRetry = ibmMQDetails.RetryCount;
            retryAll = ibmMQDetails.RetryAll;  

            if (ibmMQDetails.EnableTrace != "ON" && ibmMQDetails.EnableTrace != "OFF")
            {
                throw new LegacyException("Invalid EnableTrace value. Use ON or OFF");
            }

            if (ibmMQDetails.EnableTrace == "ON")
            {
                isTraceRequired = true;
            }

            // If queuemanager is not specified then it uses default QueueManager. Set channel and 
            // connection name into environment variables.
            if (string.IsNullOrEmpty(queueManagerName))
            {
                MQEnvironment.Channel = channelName;
                MQEnvironment.Hostname = connectionName;
                MQEnvironment.Port = port;
            }
            connectionName = connectionName + "(" + port + ")";

            /************* Added by SSE team to support SSL certificates ****/
            if (sslRequired != null && sslRequired.ToUpperInvariant() == "YES")
            {
                certificatePath = ibmMQDetails.CertificatePath as string;
                encryptionType = ibmMQDetails.EncryptionType as string;
                if ((string.IsNullOrEmpty(certificatePath)) || (string.IsNullOrEmpty(encryptionType)))
                {
                    throw new LegacyException("Certificate Path or encryption type is empty.");
                }
                else
                {
                    // Set the environment variables
                    MQEnvironment.SSLCipherSpec = encryptionType;
                    MQEnvironment.SSLKeyRepository = certificatePath;
                }
            }

            /************* Added by SSE team to support SSL certificates ****/

        }
        #endregion
        Region regionToBeUsed;
        #region Send
        /// <summary>
        /// Communication type and connection pool type is determined. Based on 
        /// communication type it calls respective method for transfering the data.
        /// </summary>
        /// <param name="adapterDetails">ListDictionary containing, boolean value
        /// indication whether to use TLS or not, region details(transportName and TransportMedium) 
        /// and IBMMQ details which are set in configuration</param>
        /// <param name="message">message to be sent</param>
        /// <returns>message returned from host</returns>
        public string Send(ListDictionary adapterDetails, string message)
        {
            /// Sachin removed this Start--- Not required as this counter just increments
            //logEntry.Category = DEBUG_INFO;
            //logEntry.Priority = 5;
            //logEntry.Severity = Severity.Information;

            //lock (syncObject)
            //{
            //    numberOfCalls++;
            //}

            //logEntry.Message = "MQAdapter. Requested Number = " + numberOfCalls.ToString();
            //Logger.Write(logEntry);
            /// Sachin removed this End--- Not required as this counter just increments


            ////Create performance counter for total number of requests
            //totalRequest.CategoryName = CATEGORY;
            //totalRequest.CounterName = TOTAL_REQUESTS;
            //totalRequest.ReadOnly = false;
            //totalRequest.InstanceName = COUNTER_INSTANCE;

            ////Create performance counter for average requests
            //averageRequest.CategoryName = CATEGORY;
            //averageRequest.CounterName = AVERAGE_REQUEST;
            //averageRequest.ReadOnly = false;
            //averageRequest.InstanceName = COUNTER_INSTANCE;

            ////Create performance counter for request size
            //requestSize.CategoryName = CATEGORY;
            //requestSize.CounterName = REQUEST_SIZE;
            //requestSize.ReadOnly = false;
            //requestSize.InstanceName = COUNTER_INSTANCE;

            ////Create performance counter for number of exceptions
            //numberOfExceptions.CategoryName = CATEGORY;
            //numberOfExceptions.CounterName = NUMBER_OF_EXCEPTIONS;
            //numberOfExceptions.ReadOnly = false;
            //numberOfExceptions.InstanceName = COUNTER_INSTANCE;

            isConnectionPool = false;

            minSize = -1;
            maxSize = -1;
            connectionTimeOut = -1;
            Infosys.Lif.LegacyIntegratorService.IBMMQ transportSection = null;
            regionToBeUsed = null;
            string response = string.Empty;
            // Read the values of ListDictioary parameter into variables
            try
            {
                foreach (DictionaryEntry items in adapterDetails)
                {
                    if (items.Key.ToString() == REGION)
                    {
                        regionToBeUsed = items.Value as Region;
                    }
                    else if (items.Key.ToString() == TRANSPORT_SECTION)
                    {
                        transportSection = items.Value as Infosys.Lif.LegacyIntegratorService.IBMMQ;
                    }
                }

                // Validates whether TransportName specified in the region, exists in IBMMQDetails
                // section.
                IBMMQDetails ibmMQDetails = ValidateTransportName(transportSection, regionToBeUsed.TransportName);

                // Call Initialize to set the transport details into member variables.                
                Initialize(ibmMQDetails);

                // Find the connection model this region should use for communication.
                FindConnectionModel(ibmMQDetails, transportSection.Connection);

                // Initialize the pool manager. 
                MQPoolManager.Initialize(transportSection);

                // Call connect method to connect to mq queue manager.
                Connect();

                // Set the RequestLength counter.
                SetPerformanceCounter(REQUEST_SIZE, (long)message.Length);
                //requestSize.RawValue = (long)message.Length;

                // If syncronous communication is specified then call SendSync otherwise 
                // call SendASync.
                //string response = string.Empty;
                if (String.IsNullOrEmpty(regionToBeUsed.CommunicationType))
                {
                    throw new LegacyException("Communication type is not specified");
                }

                if (regionToBeUsed.CommunicationType.Equals(CommunicationType.Sync.ToString()))
                {
                    response = SendSync(message);
                }
                else if (regionToBeUsed.CommunicationType.Equals(CommunicationType.Async.ToString()))
                {
                    response = SendAsync(message);
                }
                else
                {
                    throw new LegacyException("Communication type specified is not correct");
                }


                return response;
            }
            catch (MQException mqException)
            {
                IncrementPerformanceCounter(NUMBER_OF_EXCEPTIONS);
                //numberOfExceptions.Increment();
                //Logic for reconnecting

                //Check if the exception reason is 2009
                if (mqException.ReasonCode == 2009)
                {
                    int retry=1;

                    //Loop untill the retrycount is not completed 
                    //if the message is successfuly sent then the loop iterations are stopped.
                    while (retry <= maxRetry)
                    {
                        try
                        {
                            //Check if the rest for all is enabled
                            if (retryAll == "ON")
                            {
                                //Make Call to reset all connections
                                //MQPoolManager.ResetAllConnections(mqQueueManager, connectionString);

                            }
                            else
                            {
                                //if resetAll if not ON then call to reset only current connection
                                //MQPoolManager.ResetConnection(mqQueueManager, connectionString);
                            }

                            //Fetch another connection from the Pool
                            Connect(); 

                            //Make the call to the MQ based on communication type
                            if (regionToBeUsed.CommunicationType.Equals(CommunicationType.Sync.ToString()))
                            {
                                response = SendSync(message);
                                break; 
                            }
                            else
                            {
                                response = SendAsync(message);
                                break;
                            }
                        }
                        //Check if the exception reoccurs. 
                        catch(MQException mqError)
                        {
                            IncrementPerformanceCounter(NUMBER_OF_EXCEPTIONS);
                            //if connections still fail after the retry limit is reached then throw exception.
                            if (retry == maxRetry)
                            {
                                throw new LegacyException("MQ Error. Please Resend Request after sometime" + mqError.CompCode.ToString() + mqError.Reason.ToString() + mqError.Message.ToString(), mqError);
                            }
                        }
                        //increment the retry counter.
                        retry++;
                    }
                }
                //End Logic for reconnecting. 
                throw new LegacyException("Error in send method" + mqException.CompCode.ToString() + mqException.Reason.ToString() + mqException.Message.ToString(), mqException);
            }
            catch (LegacyException excepion)
            {
                IncrementPerformanceCounter(NUMBER_OF_EXCEPTIONS);

                //numberOfExceptions.Increment();
                throw excepion;
            }
            catch (Exception exception)
            {
                IncrementPerformanceCounter(NUMBER_OF_EXCEPTIONS);

                //numberOfExceptions.Increment();
                throw exception;
            }
            finally
            {
                if (isConnectionPool)
                {
                    if (mqQueueManager != null)
                    {
                        MQPoolManager.CloseConnection(mqQueueManager, connectionString);
                        //totalConnection.RawValue = MQPoolManager.FindTotalConnections();
                        //connectionPerPool.RawValue = MQPoolManager.ConnectionsPerPool();
                        MQPoolManager.ShowConnectionPoolCounter();
                    }
                }
                else
                {
                    if (mqQueueManager != null)
                    {
                        mqQueueManager.Disconnect();
                    }
                }

                // Increment counter
                IncrementPerformanceCounter(TOTAL_REQUESTS);
                //totalRequest.Increment();

                IncrementPerformanceCounter(AVERAGE_REQUEST);
                //averageRequest.Increment();


                // Call the method which will return number of total active connection
                // and number connections per pool
                //totalConnection.RawValue = MQPoolManager.FindTotalConnections();
                //connectionPerPool.RawValue = MQPoolManager.ConnectionsPerPool();
                MQPoolManager.ShowConnectionPoolCounter();
                // Close all the performance counter

                //totalRequest.Close();
                //averageRequest.Close();

                //requestSize.Close();
                //totalConnection.Close();
                //connectionPerPool.Close();
                //numberOfExceptions.Close();
                                
                
                Trace(message, response);

            }

             
            
        }

        
        private void Trace(string message, string response)
        {
            if (isTraceRequired)
            {
                using (new Microsoft.Practices.EnterpriseLibrary.Logging.Tracer(TRACE_CATEGORY))
                {
                    StringBuilder traceMessage = new StringBuilder();
                    traceMessage.Append(Environment.NewLine);
                    traceMessage.Append("Region:\t" + regionToBeUsed.Name);
                    traceMessage.Append(Environment.NewLine);
                    traceMessage.Append("RequestString:\t" + message);
                    traceMessage.Append(Environment.NewLine);
                    traceMessage.Append("RequestLength:\t" + message.Length.ToString());
                    traceMessage.Append(Environment.NewLine);

                    if (response == null)
                    {
                        traceMessage.Append("************Exception Occured while waiting for response ******************");
                        traceMessage.Append(Environment.NewLine);
                    }
                    else
                    {
                        traceMessage.Append("ResponseString:\t" + response);
                        traceMessage.Append(Environment.NewLine);
                        traceMessage.Append("ResponseLength:\t" + response.Length.ToString());
                        traceMessage.Append(Environment.NewLine);
                        traceMessage.Append("Interval:\t" + interval.ToString() + "ms");
                        traceMessage.Append(Environment.NewLine);
                    }

                    Logger.Write(traceMessage.ToString(), TRACE_CATEGORY);
                }
            }
        }
        #endregion

        #region ClearMQCounter
        /// <summary>
        /// This is called when appdomain unload event is triggered.
        /// Counter instance is removed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearMQCounter(object sender, EventArgs e)
        {
            //totalConnection.RawValue = 0;
            //connectionPerPool.RawValue = 0;
            //totalRequest.RawValue = 0;
            //averageRequest.RawValue = 0;
            //requestSize.RawValue = 0;
            //numberOfExceptions.RawValue = 0;
            //averageBase.RawValue = 0;
            //totalResponse.RawValue = 0;
            //averageResponse.RawValue = 0;
            //responseSize.RawValue = 0;
            DeletePerformanceCounter(TOTAL_REQUESTS);


            //totalRequest.RemoveInstance();
            DeletePerformanceCounter(AVERAGE_REQUEST);
            //averageRequest.RemoveInstance();
            DeletePerformanceCounter(REQUEST_SIZE);
            //requestSize.RemoveInstance();

            DeletePerformanceCounter(REQUEST_SIZE);
            DeletePerformanceCounter(NUMBER_OF_EXCEPTIONS);
            //numberOfExceptions.RemoveInstance();

            DeletePerformanceCounter(REQUEST_SIZE);
            DeletePerformanceCounter(AVERAGE_TIME);
            //averageTime.RemoveInstance();

            DeletePerformanceCounter(REQUEST_SIZE);
            //averageBase.RemoveInstance();

            DeletePerformanceCounter(REQUEST_SIZE);
            //totalResponse.RemoveInstance();

            DeletePerformanceCounter(REQUEST_SIZE);
            DeletePerformanceCounter(AVERAGE_RESPONSE);
            //averageResponse.RemoveInstance();

            DeletePerformanceCounter(REQUEST_SIZE);
            DeletePerformanceCounter(RESPONSE_SIZE);
            //responseSize.RemoveInstance();

            DeletePerformanceCounter(TIME_TO_GET_A_CONNECTION);
            //timeToGetAConnection.RemoveInstance();

            DeletePerformanceCounter(TIME_TO_GET_A_CONNECTION + " Base");
            //timeToGetAConnectionBase.RemoveInstance();
        }
        #endregion

        #region FindConnectionModel
        /// <summary>
        /// Finds the connection model that a region should use for communication.
        /// </summary>
        /// <param name="ibmMQDetails">Transport details whose connection model should be identified</param>
        /// <param name="globalConnectionInformation">Default connection information</param>
        private void FindConnectionModel(Infosys.Lif.LegacyIntegratorService.IBMMQDetails ibmMQDetails, Connection globalConnectionInformation)
        {
            // Get the connection model specified for the region.
            Connection innerConnectionInformation = ibmMQDetails.Connection as Connection;

            // If connection model is specified for the region then read the value.
            if (!string.IsNullOrEmpty(innerConnectionInformation.ConnectionModel))
            {
                if (innerConnectionInformation.ConnectionModel ==
                    ConnectionModelType.ConnectionPool.ToString())
                {
                    isConnectionPool = true;
                    Pooling pooling = innerConnectionInformation.Pooling as Pooling;
                    if (pooling == null)
                    {
                        throw new LegacyException("pooling information is not defined for IBMMQ");
                    }
                    maxSize = pooling.MaxSize;
                    minSize = pooling.MinSize;
                    connectionTimeOut = innerConnectionInformation.ConnectionTimeOut;
                }
                else if (innerConnectionInformation.ConnectionModel ==
                    ConnectionModelType.None.ToString())
                {
                    isConnectionPool = false;
                }
                else
                {
                    throw new LegacyException("Invalid ConnectionModel for " + ibmMQDetails.TransportName);
                }
            }
            else if (!string.IsNullOrEmpty(globalConnectionInformation.ConnectionModel))
            {
                // If connection model is not specified for the region then read the value from
                // default connection model (one specified under IBMMQ tag and outside region information).
                if (globalConnectionInformation.ConnectionModel ==
                    ConnectionModelType.ConnectionPool.ToString())
                {
                    isConnectionPool = true;
                    Pooling pooling = globalConnectionInformation.Pooling as Pooling;
                    if (pooling == null)
                    {
                        throw new LegacyException("pooling information is not defined for IBMMQ");
                    }
                    maxSize = pooling.MaxSize;
                    minSize = pooling.MinSize;
                    connectionTimeOut = globalConnectionInformation.ConnectionTimeOut;
                }
                else if (globalConnectionInformation.ConnectionModel ==
                    ConnectionModelType.None.ToString())
                {
                    isConnectionPool = false;
                }
                else
                {
                    throw new LegacyException("Invalid ConnectionModel in Default Connection Information");
                }
            }
            else
            {
                throw new LegacyException("ConnectionModel is not specified for " + ibmMQDetails.TransportName);
            }
        }
        #endregion

        #region ValidateTransportName
        /// <summary>
        /// Validates whether TransportName specified in the region, exists in IBMMQDetails
        // section. If it found it returns corresponding IBMMQDetails object.
        /// </summary>
        /// <param name="transportSection">IBMMQ section</param>
        /// <param name="transportName">name of the transport</param>
        private IBMMQDetails ValidateTransportName(Infosys.Lif.LegacyIntegratorService.IBMMQ transportSection, string transportName)
        {
            IBMMQDetails ibmMQDetails = null;
            bool isTransportNameExists = false;
            // Find the IBMMQ region to which it should connect for sending message.
            for (int count = 0; count < transportSection.IBMMQDetailsCollection.Count; count++)
            {
                ibmMQDetails = transportSection.IBMMQDetailsCollection[count] as IBMMQDetails;
                if (ibmMQDetails.TransportName == transportName)
                {
                    isTransportNameExists = true;
                    break;
                }
            }
            // If IBMMQ region is not set in the config then throw the exception
            if (!isTransportNameExists)
            {
                throw new LegacyException(transportName + " is not defined in IBMMQDetails section");
            }
            isPerformanceCountersEnabled = transportSection.EnablePerformanceCounters;

            return ibmMQDetails;
        }
        #endregion

        #region SendSync
        /// <summary>
        /// This is called from send method if communication type is Sync.
        /// </summary>
        /// <param name="message">message that is to be sent</param>
        /// <returns>response from the host</returns>
        private string SendSync(string message)
        {

            /// Sachin Commented Start  - Not required
            //logEntry.Message = "Synchronous method is called";
            //Logger.Write(logEntry);
            /// Sachin commented end


            mqRequestQueue = mqQueueManager.AccessQueue(requestQueueName,
                                        MQC.MQOO_OUTPUT + MQC.MQOO_FAIL_IF_QUIESCING);

            MQMessage putMessage = new MQMessage();
            MQPutMessageOptions putOptions = new MQPutMessageOptions();

            // Check if Queue Type is Dynamic if is Dynamic then create queue based 
            // on model queue.
            if (queueType.Equals(DYNAMIC))
            {
                responseQueueName = responseQueueName + "*";
                mqResponseQueue = mqQueueManager.AccessQueue(queueNameModel,
                            MQC.MQOO_INPUT_AS_Q_DEF + MQC.MQOO_FAIL_IF_QUIESCING,
                                        queueManagerName, responseQueueName, null);
            }
            else if (queueType.Equals(STATIC))
            {
#if MQClientVersion6
                CreateResponseQueueForV6();
#else
                CreateResponseQueueForV5();
#endif
            }
            else
            {
                throw new LegacyException("QueueType is not Static or Dynamic");
            }
            // Create response string.
            string response = null;

            //Create performance counter for average time to process
            //averageTime.CategoryName = CATEGORY;
            //averageTime.CounterName = AVERAGE_TIME;
            //averageTime.ReadOnly = false;
            //averageTime.InstanceName = COUNTER_INSTANCE;

            //Create performance counter for average time to process
            //averageBase.CategoryName = CATEGORY;
            //averageBase.CounterName = AVERAGE_BASE;
            //averageBase.ReadOnly = false;
            //averageBase.InstanceName = COUNTER_INSTANCE;

            long startTicks = 0;
            try
            {
                // Set the properties
                putMessage.WriteString(message);
                putMessage.Expiry = expiryTime;
                putMessage.Format = MQC.MQFMT_STRING;
                putMessage.ReplyToQueueName = mqResponseQueue.Name.Trim();

                // Set persistence
                if (Persistence == MessagePersistence.Persistent.ToString())
                {
                    putMessage.Persistence = MQC.MQPER_PERSISTENT;
                }
                else if (Persistence == MessagePersistence.NonPersistent.ToString())
                {
                    putMessage.Persistence = MQC.MQPER_NOT_PERSISTENT;
                }
                else if (Persistence == MessagePersistence.Default.ToString())
                {
                    putMessage.Persistence = MQC.MQPER_PERSISTENCE_AS_Q_DEF;
                }

                putOptions.Options = MQC.MQPMO_FAIL_IF_QUIESCING + MQC.MQPMO_NO_SYNCPOINT;

                /// Commented out by Sachin - Start
                // start = DateTime.Now.Ticks;
                /// Commented out by Sachin - End
                QueryPerformanceCounter(ref startTicks);

                DateTime beginTime = DateTime.Now;
#if !MQClientVersion6
                mqQueueManager.Begin();
#endif
                // Put the message
                mqRequestQueue.Put(putMessage, putOptions);

                // Get the message Id 
                correlationID = putMessage.MessageId;

                // The Infy mainframe reuires the operations to be committed
                mqQueueManager.Commit();


                // Call Receive which will wait for the response.
                response = Receive();
                long endTicks = 0;
                QueryPerformanceCounter(ref endTicks);

                TimeSpan timeSpan = DateTime.Now - beginTime;

                /// Commented out by Sachin     -   Start
                // interval = timeSpan.Hours * 12 + timeSpan.Minutes * 60 + timeSpan.Seconds * 60 + timeSpan.Milliseconds;
                /// Commented out by Sachin     -   End
                interval = timeSpan.TotalMilliseconds;

                /// Every time a average counter is modified, the base 
                /// counter also should be incremented
                IncrementPerformanceCounter(AVERAGE_TIME, endTicks - startTicks);
                //averageTime.IncrementBy(endTicks - startTicks);
                IncrementPerformanceCounter(AVERAGE_BASE);
                //averageBase.Increment();
            }
            finally
            {
                // If the response is null. It indicates that some error occured while
                // getting the response. The Trace method should log that exception occured
                //Trace(message, response);


                // Close the counter
                //averageTime.Close();
                //averageBase.Close();

                // Close all the open resources.
                mqRequestQueue.Close();
                mqResponseQueue.Close();
            }
            /// Sachin Commented Start -- Removal of logging all entries
            /// logEntry.Message = "Synchronous end";
            /// Logger.Write(logEntry);
            /// Sachin Commented End -- Removal of logging all entries
            return response;
        }

        /// <summary>
        /// Should be called if the MQSeries client version is 5.3
        /// </summary>
        private void CreateResponseQueueForV5()
        {

            // if the QueueType is Static then instantiate a new
            // Reponse Queue object using the responseQueueName
            mqResponseQueue = mqQueueManager.AccessQueue(responseQueueName,
                                                    MQC.MQOO_INPUT_AS_Q_DEF + MQC.MQOO_FAIL_IF_QUIESCING);
        }


        /// <summary>
        /// Should be called if the MQSeries client version is 6.0
        /// During SSE project we found that different values need to be sent in case we 
        /// want to perform inquiries on the response queue. Mainly to retrieve the queue name.
        /// </summary>
        private void CreateResponseQueueForV6()
        {

            // if the QueueType is Static then instantiate a new
            // Reponse Queue object using the responseQueueName
            mqResponseQueue = mqQueueManager.AccessQueue(responseQueueName,
                                                    MQC.MQOO_INPUT_SHARED | MQC.MQOO_INQUIRE);
        }
        #endregion

        #region Receive
        /// <summary>
        /// Here it waits for the response and sends back the same to SendSync method.
        /// </summary>
        /// <returns>response from the host</returns>
        private string Receive()
        {
            MQMessage getMessage = new MQMessage();
            string response = string.Empty;
            MQGetMessageOptions getOptions = new MQGetMessageOptions();
            getOptions.Options = MQC.MQGMO_FAIL_IF_QUIESCING + MQC.MQGMO_WAIT + MQC.MQGMO_NO_SYNCPOINT;

            // set the wait interval to TimeOut value 
            // Change related to Unsat No 86
            getOptions.WaitInterval = timeOut;

            // Set the message.CorrelationId property to the 
            // correlationID byte array.
            // This enables picking the right message from the queue.
            getMessage.CorrelationId = correlationID;

            //Create performance counter for tracking total number of responses
            //totalResponse.CategoryName = CATEGORY;
            //totalResponse.CounterName = TOTAL_RESPONSES;
            //totalResponse.ReadOnly = false;
            //totalResponse.InstanceName = COUNTER_INSTANCE;

            //Create performance counter for average response
            //averageResponse.CategoryName = CATEGORY;
            //averageResponse.CounterName = AVERAGE_RESPONSE;
            //averageResponse.ReadOnly = false;
            //averageResponse.InstanceName = COUNTER_INSTANCE;

            //Create performance counter for average response
            //responseSize.CategoryName = CATEGORY;
            //responseSize.CounterName = RESPONSE_SIZE;
            //responseSize.ReadOnly = false;
            //responseSize.InstanceName = COUNTER_INSTANCE;

            try
            {
#if !MQClientVersion6
                mqQueueManager.Begin();
#endif
                mqResponseQueue.Get(getMessage, getOptions);
                mqQueueManager.Commit();

                // Increment counter
                IncrementPerformanceCounter(TOTAL_RESPONSES);
                //totalResponse.Increment();
                IncrementPerformanceCounter(AVERAGE_RESPONSE);
                //averageResponse.Increment();

                // ResponseQueue.Get(getMessage, getOptions);
                // Check if the message format is string.
                // If it if string then read the reponse string
                if (getMessage.Format.CompareTo(MQC.MQFMT_STRING) == 0)
                {
                    response = getMessage.ReadString(getMessage.MessageLength);
                }
                SetPerformanceCounter(RESPONSE_SIZE, (long)response.Length);
                //responseSize.RawValue = (long)response.Length;
            }
            finally
            {
                // Close the performance 
                //totalResponse.Close();
                //averageResponse.Close();
                //responseSize.Close();
            }

            // return the response string
            return response;
        }
        #endregion

        #region SendAsync
        /// <summary>
        /// This is called from send method if communication type is Async.
        /// </summary>
        /// <param name="message">message that is to be sent</param>
        /// <returns>response from the host</returns>
        private string SendAsync(string message)
        {
            // Sachin has changed this to null from string.empty
            string response = null;
            // If the response is null while tracing, it indicates that some
            // exception occured while waiting for the response.

            /// Sachin Commented Start -- Removal of logging all entries
            /// logEntry.Message = "Asynchronous method is called";
            /// Logger.Write(logEntry);
            /// Sachin Commented End-- Removal of logging all entries

            // instantiate a new object for RequestQueue
            mqRequestQueue = mqQueueManager.AccessQueue(requestQueueName,
                                MQC.MQOO_OUTPUT + MQC.MQOO_FAIL_IF_QUIESCING);

            //instantiate an object of MQMessage
            MQMessage putMessage = new MQMessage();

            try
            {
                //set the Request message to the put Message object
                putMessage.WriteString(message);

                //set the message format property of Message object to string
                putMessage.Format = MQC.MQFMT_STRING;

                //putMessage.ReplyToQueueName = mqResponseQueue.Name.Trim();

                // Set persistence
                if (Persistence == MessagePersistence.Persistent.ToString())
                {
                    putMessage.Persistence = MQC.MQPER_PERSISTENT;
                }
                else if (Persistence == MessagePersistence.NonPersistent.ToString())
                {
                    putMessage.Persistence = MQC.MQPER_NOT_PERSISTENT;
                }
                else if (Persistence == MessagePersistence.Default.ToString())
                {
                    putMessage.Persistence = MQC.MQPER_PERSISTENCE_AS_Q_DEF;
                }

                // set the putOptions
                MQPutMessageOptions putOptions = new MQPutMessageOptions();
                putOptions.Options = MQC.MQPMO_NO_SYNCPOINT;

                DateTime beginTime = DateTime.Now;
                mqQueueManager.Begin();
                // Put the message in the queue.
                mqRequestQueue.Put(putMessage, putOptions);
                mqQueueManager.Commit();

                TimeSpan timeSpan = DateTime.Now - beginTime;

                /// COmmented out by Sachin     -   Start
                // interval = timeSpan.Hours * 12 + timeSpan.Minutes * 60 + timeSpan.Seconds * 60 + timeSpan.Milliseconds;
                /// COmmented out by Sachin     -   End


                /// During Async method there is no method of tracing out the response time as 
                interval = timeSpan.TotalMilliseconds;


                // set the correlationID byte array to the Message.MessageId 
                // property This is done to enable that the correct message 
                // gets picked up while reading from response Queue
                correlationID = putMessage.MessageId;
            }
            finally
            {
                //Trace(message, response);
                //return the response message
                mqRequestQueue.Close();
            }
            /// Sachin Commented Start -- Removal of logging all entries
            /// logEntry.Message = "Asynchronous end";
            /// Logger.Write(logEntry);
            /// Sachin Commented End -- Removal of logging all entries

            return response;
        }
        #endregion

        #region Connect
        /// <summary>
        /// Depening on the connection pooling type, it gets the connection object.
        /// </summary>
        private void Connect()
        {
            // Check conection model required and accordingly create the connection.
            if (isConnectionPool)
            {

                //timeToGetAConnection.CategoryName = CATEGORY;
                //timeToGetAConnection.CounterName = TIME_TO_GET_A_CONNECTION;
                //timeToGetAConnection.ReadOnly = false;
                //timeToGetAConnection.InstanceName = COUNTER_INSTANCE;

                //timeToGetAConnectionBase.CategoryName = CATEGORY;
                //timeToGetAConnectionBase.CounterName = TIME_TO_GET_A_CONNECTION + " Base";
                //timeToGetAConnectionBase.ReadOnly = false;
                //timeToGetAConnectionBase.InstanceName = COUNTER_INSTANCE;

                long startTicks = 0;

                // If connection pooling is required then create a MQConnectionInformation and 
                // Call GetConnection which will return MQQueueManager object.
                connectionString = new MQConnectionInformation(minSize, maxSize,
                    connectionTimeOut, queueManagerName, channelName, connectionName);

                QueryPerformanceCounter(ref startTicks);
                mqQueueManager = MQPoolManager.GetConnection(connectionString);
                long endTicks = 0;
                QueryPerformanceCounter(ref endTicks);

                IncrementPerformanceCounter(TIME_TO_GET_A_CONNECTION, endTicks - startTicks);
                //timeToGetAConnection.IncrementBy(endTicks - startTicks);

                IncrementPerformanceCounter(TIME_TO_GET_A_CONNECTION + " Base");
                //timeToGetAConnectionBase.Increment();

                //timeToGetAConnection.Close();
                //timeToGetAConnectionBase.Close();

                //totalConnection.RawValue = MQPoolManager.FindTotalConnections();
                //connectionPerPool.RawValue = MQPoolManager.ConnectionsPerPool();
                MQPoolManager.ShowConnectionPoolCounter();

                /// Sachin Commented Start -- Removal of logging all entries
                /// logEntry.Message = "ConnectionModel Type = ConnectionPool";
                /// Sachin Commented End -- Removal of logging all entries
            }
            else
            {
                // Create a MQQueueManager object
                if (String.IsNullOrEmpty(queueManagerName))
                {
                    // Uses default queuemanager
                    mqQueueManager = new MQQueueManager();
                }
                else if (String.IsNullOrEmpty(channelName) || String.IsNullOrEmpty(connectionName))
                {
                    // Queue manager exists in local system
                    mqQueueManager = new MQQueueManager(queueManagerName);
                }
                else
                {
                    // Queuemanager exists in remote system
                    mqQueueManager = new MQQueueManager(queueManagerName,
                                        channelName, connectionName);
                }
                /// Sachin Commented Start -- Removal of logging all entries
                /// logEntry.Message = "ConnectionModel Type = None";
                /// Sachin Commented End -- Removal of logging all entries
            }
            /// Sachin Commented Start -- Removal of logging all entries
            /// Logger.Write(logEntry);
            /// Sachin Commented End -- Removal of logging all entries
        }


        private System.Diagnostics.PerformanceCounter CreatePerformanceCounter(string counterName)
        {
            if (IsPerformanceCountersEnabled)
            {

                System.Diagnostics.PerformanceCounter performanceCounter
                    = new System.Diagnostics.PerformanceCounter();
                performanceCounter.CategoryName = CATEGORY;
                performanceCounter.CounterName = counterName;
                performanceCounter.ReadOnly = false;
                performanceCounter.InstanceName = COUNTER_INSTANCE;
                if (!AppDomainUnloadRegistered)
                {
                    // Register eventhandler for appdomain unload event. In eventhandler
                    // performance counter instance can removed.
                    AppDomain app = AppDomain.CurrentDomain;
                    app.DomainUnload += new EventHandler(ClearMQCounter);
                    AppDomainUnloadRegistered = true;
                }
                return performanceCounter;
            }
            return null;
        }

        private void IncrementPerformanceCounter(string counterName, long incrementValue)
        {
            System.Diagnostics.PerformanceCounter performanceCounter
                = CreatePerformanceCounter(counterName);
            if (performanceCounter != null)
            {
                performanceCounter.IncrementBy(incrementValue);
                performanceCounter.Close();
            }
        }


        static string isPerformanceCountersEnabled = string.Empty;
        static bool IsPerformanceCountersEnabled
        {
            get
            {
                // EnablePerformanceCounters can only be ON OFF or 
                // the node may not have been mentioned at all.
                if (isPerformanceCountersEnabled != "ON"
                    && isPerformanceCountersEnabled != "OFF"
                    )
                {
                    throw new LegacyException("Invalid EnablePerformanceCounters value. Use ON or OFF");
                }
                // If isPerformanceCountersEnabled is ON, return true, else false
                return (isPerformanceCountersEnabled == "ON");
            }
        }

        private void IncrementPerformanceCounter(string counterName)
        {
            IncrementPerformanceCounter(counterName, 1);
        }

        private void SetPerformanceCounter(string counterName, long rawValue)
        {
            System.Diagnostics.PerformanceCounter performanceCounter
                = CreatePerformanceCounter(counterName);
            if (performanceCounter != null)
            {
                performanceCounter.RawValue = rawValue;
                performanceCounter.Close();
            }
        }
        private void DeletePerformanceCounter(System.Diagnostics.PerformanceCounter performanceCounter)
        {
            if (performanceCounter != null)
            {
                performanceCounter.RemoveInstance();
            }
        }

        private void DeletePerformanceCounter(string performanceCounterName)
        {
            System.Diagnostics.PerformanceCounter performanceCounter
                = CreatePerformanceCounter(performanceCounterName);
            if (performanceCounter != null)
            {
                performanceCounter.RemoveInstance();
            }
        }
        #endregion

        #endregion

        #region IAdapter Members
        public event ReceiveHandler Received;

        public void Receive(ListDictionary adapterDetails)
        {
            throw new NotImplementedException();
        }

        public bool Delete(System.Collections.Specialized.ListDictionary messageDetails)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    #endregion

}
