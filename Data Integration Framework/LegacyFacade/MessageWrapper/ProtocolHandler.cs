/****************************************************************
 * This file is a part of the Legacy Integration Framework.
 * This file implements IWrapper interface to implement the wrapper for CCIS message protocol.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using Infosys.Lif.LegacyParameters;
using Infosys.Lif.WrapperInterface;
using Infosys.Lif.LegacyCommon;
namespace Infosys.Lif.MessageWrapper
{
	/// <summary>
	/// This Class implements the IWrapper interface and handles APCA protocol
	/// </summary>
	public class ProtocolHandler : IWrapper
	{
		#region Private Fields

		private string[] response;
		private RequestParameters requestParams;
		private StringBuilder request;
		#endregion

		#region Constructor
		/// <summary>
		/// This is the Constructor method
		/// </summary>
		public ProtocolHandler()
		{
			request = new StringBuilder();
		}
		#endregion

		#region Public Methods
		#region CreateRequest
		/// <summary>
		/// This is the interface implementation of CreateRequest
		/// it creates a single request string to be sent to host  
		/// </summary>
		/// <param name="inputRequest">string array containing individual request strings</param>
		/// <param name="requestParameters">Parameter collection containing </param>
		/// <returns>Request String</returns>
		public string CreateRequest(string[] inputRequest, RequestParameters requestParameters)
		{
			requestParams = requestParameters;
			//Invoke PrepareAPCAHeader method to create apcaHeader
			String apcaHeader = PrepareAPCAHeader(inputRequest.Length);
			request.Append(apcaHeader);
			//Add the individual requests
			Order(inputRequest);
			//return the request string
			return request.ToString();
		}
		#endregion

		#region Extract
		/// <summary>
		/// This Method returns the string array containing individual responses
		/// </summary>
		/// <param name="hostResponse">The response string</param>
		/// <returns>response string array</returns>
		public string[] Extract(string hostResponse)
		{
            hostResponse = hostResponse.Replace('\0', ' ');
			//Split the response string on the new line character
			string[] responses1 = null;
			if (!string.IsNullOrEmpty(hostResponse))
			{
				responses1 = hostResponse.Split(Environment.NewLine.ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);
			}
			//Check if there are errors in the APCA header
			if (responses1.Length > 0)
			{
				if (!ValidateHeader(responses1[0]))
				{
					throw new LegacyException("Error in APCA header" + responses1[0]);
				}
			}
			response = new string[responses1.Length - 1];
			//Check if there are errors in each response
			for (int i = 1; i < responses1.Length; i++)
			{
				if (!ValidateParser(responses1[i]))
				{
					throw new LegacyException("Error in DAP" + responses1[i]);
				}
				else
				{
					response[i - 1] = responses1[i];
				}
			}
			//return the response string array
			return response;
		}
		#endregion
		#endregion

		#region Private Methods

		#region PrepareAPCAHeader
		/// <summary>
		/// This Method prepares the APCA header for the request string
		/// </summary>
		/// <param name="objectCount">The total number of requests</param>
		/// <returns>APCA header string</returns>
		private string PrepareAPCAHeader(int objectCount)
		{
			StringBuilder apcaHeader = new StringBuilder();

			//Preparation of APCA Header
			apcaHeader.Append("APCA    "
				+ requestParams.RequestCollection["TransactionID"].PadRight(4, ' ')
                + requestParams.RequestCollection["UserID"].PadRight(10, ' ')
				+ "  " + "   0000" + "          " + "        "
				+ "        "
                + requestParams.RequestCollection["Password"].PadRight(8, ' ')
                + objectCount.ToString(System.Globalization.CultureInfo.InvariantCulture).PadLeft(4, '0')
				+ "                                           "
				+ "                    ");
			//Check if APCA header if 132 characters in length
			if (apcaHeader.Length != 132)
				throw new LegacyException("APCA header incorrectly formed");
			return apcaHeader.ToString();
		} 
		#endregion

		#region Order
		/// <summary>
		/// This method adds the sequence number to each request
		/// </summary>
		/// <param name="inputRequest">input request string array</param>
		/// <returns>Ordered string to be appended to APCA header</returns>
		private void Order(string[] inputRequest)
		{
			//string request = "\r\n";
			request.Append(Environment.NewLine);
			for (int i = 0; i < inputRequest.Length; i++)
			{
				inputRequest[i] = inputRequest[i].Insert(8, i.ToString(System.Globalization.CultureInfo.InvariantCulture).PadLeft(4, '0'));
				inputRequest[i] = inputRequest[i].Remove(12, 4);
				request.Append(inputRequest[i]);
				request.Append(Environment.NewLine);
				//request += inputRequest[i] + "\r\n";
			}

		}
		#endregion

		#region ReOrder
		/// <summary>
		/// If the responses are not in order then the Reorder method rearranges the responses
		/// </summary>
		private static void ReOrder()
		{
		}
		#endregion

		#region ValidateHeader
		/// <summary>
		/// This Method Validates the APCA header received for any Errors
		/// </summary>
		/// <param name="responseHeader"></param>
		/// <returns>True/False</returns>
		private static bool ValidateHeader(string responseHeader)
		{
			string errorCodeAPCA = "";
			//Extract the error code from APCA header
			errorCodeAPCA = responseHeader.Substring(27, 4).Trim();
			//Check if error code is valid error condition
			if (errorCodeAPCA != "0000")
			{
				throw new LegacyException(errorCodeAPCA);
			}
			else
			{
				return true;
			}
		}
		#endregion

		#region ValidateParser
		/// <summary>
		/// This Method Validates the APCA header received for any Errors
		/// </summary>
		/// <param name="parserResponse">The DAP response string</param>
		/// <returns>true/False</returns>
		private static bool ValidateParser(string parserResponse)
		{
			//Split the response string based on newline character
			string[] dataObjects = parserResponse.Split("\r\n".ToCharArray(),
												   System.StringSplitOptions.
												   RemoveEmptyEntries);

			for (int iCount = 1; iCount < dataObjects.Length; iCount++)
			{
				//Extract the DAP error code
				string dapErrorCode = dataObjects[iCount].Substring(23, 5).Trim();
				//Extract the DAP reason code


				/****************************************************
				 * Uncomment the Below Line and Use only if Necessary 
				 * Reason Code Checking
				 //string dapReasonCode = dataObjects[iCount].Substring(28, 8);
				 ****************************************************/


				//Extract the DAP SQL code
				string sqlCode = dataObjects[iCount].Substring(36, 5).Trim();
				//Extract the rowCount
				int noOfRows = Convert.ToInt32(dataObjects[iCount]
													.Substring(18, 5).Trim(), System.Globalization.CultureInfo.InvariantCulture);

				//Only if RowCount <1 , DAP contains errors
				if (noOfRows < 1)
				{

					//DAP Error Code is given precedence and thrown even if
					//the DAP header contains both DAP error and SQL error codes.
					if (dapErrorCode != "0000" && dapErrorCode != "0100" && dapErrorCode != "9701" && dapErrorCode != "0020")
					{
						throw new LegacyException(dapErrorCode);
					}
					else if (sqlCode != "0000" && sqlCode != "0100" && sqlCode != "9701"
						&& sqlCode != "-9008")
					{

						throw new LegacyException(sqlCode);
					}
				}


			}
			return true;
		}
		#endregion
		#endregion
	}
}
