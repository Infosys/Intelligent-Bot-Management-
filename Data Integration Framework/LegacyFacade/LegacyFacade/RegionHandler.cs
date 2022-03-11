/****************************************************************
 * This file is a part of the Legacy Integration Framework.
 * This file separates the requests based on the region types
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using Infosys.Lif.LegacyCommon;
using System.Collections.Specialized;
using System.Runtime.Remoting.Messaging;
namespace Infosys.Lif.LegacyFacade
{
	class RegionHandler
	{
		//This Delegate handles the Execution of the requests.
		private delegate ArrayList RegionDelegate(ArrayList obj, string str, int position);

		//The response object array
		private object[] response;

        private string[] serviceNames;
		//Hashtable containing the region objects
		private Hashtable regions = new Hashtable();
		//Hashtable containing a counter of objects.
		private Hashtable requestCounter = new Hashtable();
		//The Event raised when the Delegate completes execution
		LegacyEvent responseEvent = new LegacyEvent();

		#region Public Methods
		#region Constructor
		/// <summary>
		/// This is the Constructor method
		/// </summary>
		public RegionHandler()
		{
		}
		#endregion

		#region StartProcess
		/// <summary>
		/// This Method is the Entry point for the class.
		/// It controls the Execution of requests. 
		/// </summary>
		/// <param name="requestHashTable">Hashtable containg request objects</param>
		/// <param name="myEvent">Event object that will be raised after completion of Execution</param>
		/// <param name="process">ProcessMode WaitForAll or WaitForNone</param>
		/// <param name="counter">The Counter of the request order</param>
		/// <param name="position">The request Position in a Batch Call</param>
		public void StartProcess(Hashtable requestHashTable, LegacyEvent myEvent, ServiceManager.ProcessMode process, Hashtable counter, int position)
		{
			regions = requestHashTable;
			responseEvent = myEvent;
			requestCounter = counter;

			//Instantiate a new object of delegate
			RegionDelegate newDelegate = new RegionDelegate(Invoke);
			//Fetch the key collection from the requestHashTable
			ICollection keys = requestHashTable.Keys;

			//create the object for results
			object[] asyncResults = new object[requestHashTable.Count];
			int count = 0;
			int objectCount = 0;
			//Loop to start processing
			foreach (object key in keys)
			{
				//Fetch request based on kep
				ArrayList request = (ArrayList)(regions[key.ToString()]);
				if (request == null)
				{
					throw new LegacyException("Error in Legacy Framework. No Objects in request");
				}
				objectCount += request.Count;

				if (process == ServiceManager.ProcessMode.WaitForNone)
				{
					// set the callback method for Asynchronous Mode
					AsyncCallback cb = new AsyncCallback(Results);
					//Begin invocation of delegate
					asyncResults[count] = newDelegate.BeginInvoke(request, key.ToString().ToString(), position, cb, newDelegate);
				}
				else if (process == ServiceManager.ProcessMode.WaitForAll)
				{
					//Begin invocation of delegate for Synchronous call
					asyncResults[count] = newDelegate.BeginInvoke(request, key.ToString(), position, null, null);
				}
				//Increment Counter for the total number of delegates fired
				count++;
			}
			//instantiate response object array.
			response = new object[objectCount];
            serviceNames = new string[objectCount];  
			count = 0;
			//For Synchronous operations set the outputs into the response Array
			if (process == ServiceManager.ProcessMode.WaitForAll)
			{
				for (int responseCount = 0; responseCount < asyncResults.Length; responseCount++)
				{
					//Get the Invoked delegate
					RegionDelegate InvokedDelegate = (RegionDelegate)((AsyncResult)asyncResults[responseCount]).AsyncDelegate;
					//Call the EndInvoke Method on the Invoked delegate to get responses
					ArrayList outputData = InvokedDelegate.EndInvoke((IAsyncResult)asyncResults[responseCount]);
                    object[] output = (object[])outputData[0];
                    string[] outputServiceNames = (string[])outputData[1]; 
                    ArrayList requestPositions = (ArrayList)requestCounter[output[0].ToString()];
					//set the responses into the response object array
					for (int requestPositionsCount = 0; requestPositionsCount < requestPositions.Count; requestPositionsCount++)
					{
						object tempObj = output[requestPositionsCount + 2];
						response[Convert.ToInt32(requestPositions[requestPositionsCount], System.Globalization.CultureInfo.InvariantCulture) - 1] = tempObj;
                        serviceNames[Convert.ToInt32(requestPositions[requestPositionsCount], System.Globalization.CultureInfo.InvariantCulture) - 1] = outputServiceNames[requestPositionsCount];
					}



				}
			}
			//For Synchronous operations Raise the Event to signal end of processing
			if (process == ServiceManager.ProcessMode.WaitForAll)
			{
				//Set the event Arguments
				RequestDelegateArgs reqDelArgs = new RequestDelegateArgs(response,serviceNames);
				//Raise the Event
				responseEvent.OnResponseReceived(reqDelArgs);
			}
		}
		#endregion

		#region Results
		/// <summary>
		/// This method is invoked for Asynchronous operation
		/// </summary>
		/// <param name="asyncResult">the IAsyncResult object</param>
		public void Results(IAsyncResult asyncResult)
		{
			// Extract the delegate from the AsyncResult.  
			RegionDelegate fd = (RegionDelegate)asyncResult.AsyncState;

			// Obtain the result .
			ArrayList outputData = fd.EndInvoke(asyncResult);
            object[] outPutResponse = (object[])outputData[0];
            serviceNames = (string[])outputData[1]; 
            response = new object[outPutResponse.Length - 2];   
            for (int i = 2; i < outPutResponse.Length; i++)
            {
                response[i - 2] =(object)outPutResponse[i];  
            }

			//For Asynchronous operations Raise the Event to signal end of processing
			//Set the Event Arguments
			RequestDelegateArgs reqDelArgs = new RequestDelegateArgs(response,serviceNames);
			//Raise the Event
			responseEvent.OnResponseReceived(reqDelArgs);

		}
		#endregion

		#region Invoke
		/// <summary>
		/// This is the Method invoked by the Delegate RegionDelegate
		/// </summary>
        /// <param name="requestsTable">ArrayList containg the request objects</param>
		/// <param name="regionName">The Region Name on which Execution of requests is to take place</param>
		/// <param name="position">position of requests in Batch Execution</param>
		/// <returns>ArrayList Containins objects and serviceNames</returns>
		public ArrayList Invoke(ArrayList requestsTable, string regionName, int position)
		{
			//Instantiate the RequestHandler class object
			RequestHandler requestHandler = new RequestHandler();
			//Invoke the Execute method. this returns the response objects after execution
			return requestHandler.Execute(requestsTable, regionName, position);

		}
		#endregion 
		#endregion


	}


}
