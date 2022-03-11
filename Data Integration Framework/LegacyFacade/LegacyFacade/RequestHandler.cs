/****************************************************************
 * This file is a part of the Legacy Integration Framework.
 * This file handles calls to serializers/Wrapper and Legacy Integrator.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using Infosys.Lif.LegacyParameters;
using Infosys.Lif.WrapperInterface;
using System.Reflection;
using System.Runtime.Remoting;

//using System.Diagnostics;

using Infosys.Lif.LegacyConfig;
using Infosys.Lif.LegacyParser.Framework;
using Infosys.Lif.LegacyIntegrator;
using Infosys.Lif.LegacyCommon;
using Infosys.Lif.LegacyIntegratorService;
using Infosys.Lif.MessageWrapper;

// For Caching
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;


namespace Infosys.Lif.LegacyFacade
{
    class RequestHandler
    {

        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public static extern void QueryPerformanceCounter(ref long ticks);

        private static bool isAppDomainUnloadRegistered = false;

        

        #region Constants
        private const string REGIONNAME = "CONNECTSERIALIZER";
        #endregion

        #region Private fields
        //Hashtable containing request objects
        private ArrayList requests;
        //Objects Arrays to populate responses
        private object[] responseObjects;
        private object[] response;
        //String Arrays for internal processing of responses and requests
        private string[] requestArray;
        private string[] responseArray;
        private string[] serviceNames;
        //Request String to be sent to Host
        private string requestString;
        //Response String received from Host
        private string responseString;
        //The region in which execution of request will take place
        private string region;
        //Collection of requestParameters
        private RequestParameters[] requestParameters;
        //total Number of parsers invoked
        private int parserCount;
        //Position of request in batch call
        private int position;
        //The Service Level Configuration information
        private Service[] service;
        //The Wrapper level Configuration Information
        private Wrapper wrap;
        //Temporary State Maintanence for Serializers.
        private object[] serializers;

        //Base class and Interface definitions
        private IWrapper wrapper;
        private SerializerBase serializerBase;

        private string[] requestCachePosition;

        private ICacheManager lifCacheManager;
        private string[] requestCacheArrayStore;

        private int CacheCount = 0;

        #endregion

        #region Public Methods
        #region Constructor
        /// <summary>
        /// This is the Constructor Method
        /// </summary>
        public RequestHandler()
        {
        }
        #endregion






        #region Execute
        /// <summary>
        /// This Method controls the Processing.
        /// </summary>
        /// <param name="requestTable">Hashtable containing the Request objects</param>
        /// <param name="regionName">The RegionName where execution of requests will occur</param>
        /// <param name="objectPosition">The position of the objects in a Batch operation</param>
        /// <returns>Arraylist containing response objects and serviceNames</returns>
        public ArrayList Execute(ArrayList requestTable, string regionName, int objectPosition)
        {


            //PerformanceCounter ctrExecuteMethodExecutionTime = new PerformanceCounter();
            //ctrExecuteMethodExecutionTime.CounterName = "AverageTime (RequestHandler)";
            //ctrExecuteMethodExecutionTime.CategoryName = categoryName;
            //ctrExecuteMethodExecutionTime.ReadOnly = false;
            //ctrExecuteMethodExecutionTime.InstanceName = processName;

            //PerformanceCounter ctrExecuteMethodExecutionTimeBase = new PerformanceCounter();
            //ctrExecuteMethodExecutionTimeBase.CounterName = "AverageTime (RequestHandler) Base";
            //ctrExecuteMethodExecutionTimeBase.CategoryName = categoryName;
            //ctrExecuteMethodExecutionTimeBase.ReadOnly = false;
            //ctrExecuteMethodExecutionTimeBase.InstanceName = processName;

            long startExecuteTicks = 0;
            QueryPerformanceCounter(ref startExecuteTicks);


            try
            {





                requests = requestTable;
                region = regionName;
                position = objectPosition;

                #region Execution of CreateParser
                long startCreateParserTicks = 0;
                QueryPerformanceCounter(ref startCreateParserTicks);


                //Invoke the CreateParser Method
                CreateParser();


                long endCreateParserTicks = 0;
                QueryPerformanceCounter(ref endCreateParserTicks);

                //PerformanceCounter ctrCreateParser = new PerformanceCounter();
                //ctrCreateParser.CounterName = "AverageTime (CreateParser)";
                //ctrCreateParser.CategoryName = categoryName;
                //ctrCreateParser.ReadOnly = false;
                //ctrCreateParser.InstanceName = processName;

                //PerformanceCounter ctrCreateParserBase = new PerformanceCounter();
                //ctrCreateParserBase.CounterName = "AverageTime (CreateParser) Base";
                //ctrCreateParserBase.CategoryName = categoryName;
                //ctrCreateParserBase.ReadOnly = false;
                //ctrCreateParserBase.InstanceName = processName;


                IncrementPerformanceCounter("AverageTime (CreateParser)", endCreateParserTicks - startCreateParserTicks);
                //ctrCreateParser.IncrementBy(endCreateParserTicks - startCreateParserTicks);

                IncrementPerformanceCounter("AverageTime (CreateParser) Base");
                //ctrCreateParserBase.Increment();

                //ctrCreateParser.Close();
                //ctrCreateParserBase.Close();
                #endregion



                if (requestArray.Length > 0)
                {
                    //Invoke the Wrap method to create the request message 
                    if (!region.Equals(REGIONNAME))
                    {
                        #region Execution of Wrap
                        //PerformanceCounter ctrWrap = new PerformanceCounter();
                        //ctrWrap.CategoryName = categoryName;
                        //ctrWrap.CounterName = "AverageTime (Wrap)";
                        //ctrWrap.ReadOnly = false;
                        //ctrWrap.InstanceName = processName;

                        //PerformanceCounter ctrWrapBase = new PerformanceCounter();
                        //ctrWrapBase.CategoryName = categoryName;
                        //ctrWrapBase.CounterName = "AverageTime (Wrap) Base";
                        //ctrWrapBase.ReadOnly = false;
                        //ctrWrapBase.InstanceName = processName;

                        long startWrapTicks = 0;
                        QueryPerformanceCounter(ref startWrapTicks);
                        Wrap();

                        long endWrapTicks = 0;
                        QueryPerformanceCounter(ref endWrapTicks);

                        IncrementPerformanceCounter("AverageTime (Wrap)", endWrapTicks - startWrapTicks);
                        //ctrWrap.IncrementBy(endWrapTicks - startWrapTicks);
                        IncrementPerformanceCounter("AverageTime (Wrap) Base");
                        //ctrWrapBase.Increment();
                        //ctrWrap.Close();
                        //ctrWrapBase.Close();
                        #endregion

                        //Instantiate the Adapter Class
                        AdapterManager adapterManager = new AdapterManager();

                        #region Execution of adapter manager


                        //PerformanceCounter ctrAdapManager = new PerformanceCounter();
                        //ctrAdapManager.CounterName = "AverageTime (AdapterManager)";
                        //ctrAdapManager.CategoryName = categoryName;
                        //ctrAdapManager.ReadOnly = false;
                        //ctrAdapManager.InstanceName = processName;

                        //PerformanceCounter ctrAdapManagerBase = new PerformanceCounter();
                        //ctrAdapManagerBase.CounterName = "AverageTime (AdapterManager) Base";
                        //ctrAdapManagerBase.CategoryName = categoryName;
                        //ctrAdapManagerBase.ReadOnly = false;
                        //ctrAdapManagerBase.InstanceName = processName;



                        long startTicks = 0;
                        QueryPerformanceCounter(ref startTicks);


                        //invoke the Execute method to fetch the response

                        responseString = adapterManager.Execute(requestString, region);

                        long endTicks = 0;
                        QueryPerformanceCounter(ref endTicks);


                        IncrementPerformanceCounter("AverageTime (AdapterManager)", endTicks - startTicks);
                        IncrementPerformanceCounter("AverageTime (AdapterManager) Base");
                        //ctrAdapManager.IncrementBy(endTicks - startTicks);
                        //ctrAdapManagerBase.Increment();

                        //ctrAdapManager.Close();
                        //ctrAdapManagerBase.Close();
                        #endregion

                        #region Unwrap execution

                        //PerformanceCounter unWrapCtr = new PerformanceCounter();
                        //unWrapCtr.CategoryName = categoryName;
                        //unWrapCtr.CounterName = "AverageTime (UnWrap)";
                        //unWrapCtr.ReadOnly = false;
                        //unWrapCtr.InstanceName = processName;

                        //PerformanceCounter unWrapCtrBase = new PerformanceCounter();
                        //unWrapCtrBase.CategoryName = categoryName;
                        //unWrapCtrBase.CounterName = "AverageTime (UnWrap) Base";
                        //unWrapCtrBase.ReadOnly = false;
                        //unWrapCtrBase.InstanceName = processName;

                        long startUnWrapTicks = 0;
                        QueryPerformanceCounter(ref startUnWrapTicks);

                        //Invoke the UnWrap method to Split the response string into string array
                        UnWrap();

                        long endUnWrapTicks = 0;
                        QueryPerformanceCounter(ref endUnWrapTicks);

                        IncrementPerformanceCounter("AverageTime (UnWrap)", endUnWrapTicks - startUnWrapTicks);
                        //unWrapCtr.IncrementBy(endUnWrapTicks - startUnWrapTicks);
                        IncrementPerformanceCounter("AverageTime (UnWrap) Base");
                        //unWrapCtrBase.Increment();

                        //unWrapCtr.Close();
                        //unWrapCtrBase.Close();
                        #endregion
                    }
                }

                #region Execution of Extract Parser
                //PerformanceCounter ctrExtract = new PerformanceCounter();
                //ctrExtract.CounterName = "AverageTime (ExtractParser)";
                //ctrExtract.CategoryName = categoryName;
                //ctrExtract.ReadOnly = false;
                //ctrExtract.InstanceName = processName;


                //PerformanceCounter ctrExtractBase = new PerformanceCounter();
                //ctrExtractBase.CounterName = "AverageTime (ExtractParser) Base";
                //ctrExtractBase.CategoryName = categoryName;
                //ctrExtractBase.ReadOnly = false;
                //ctrExtractBase.InstanceName = processName;


                startCreateParserTicks = 0;
                QueryPerformanceCounter(ref startCreateParserTicks);

                //Invoke the ExtractParser to convert strings into objects
                ExtractParser();

                endCreateParserTicks = 0;
                QueryPerformanceCounter(ref endCreateParserTicks);

                IncrementPerformanceCounter("AverageTime (ExtractParser)", endCreateParserTicks - startCreateParserTicks);
                IncrementPerformanceCounter("AverageTime (ExtractParser) Base");
                //ctrExtract.IncrementBy(endCreateParserTicks - startCreateParserTicks);
                //ctrExtractBase.Increment();
                //ctrExtract.Close();
                //ctrExtractBase.Close();
                #endregion
            }
            catch (ApplicationException ex)
            {
                throw new LegacyException("Error in Legacy Framework", ex);
            }
            //create the response Arraylist
            ArrayList responseData = new ArrayList();
            //add the responseobjects
            responseData.Add(responseObjects);
            //add the services
            responseData.Add(serviceNames);



            long endExecuteTicks = 0;
            QueryPerformanceCounter(ref endExecuteTicks);

            IncrementPerformanceCounter("AverageTime (RequestHandler)", endExecuteTicks - startExecuteTicks);
            //ctrExecuteMethodExecutionTime.IncrementBy(endExecuteTicks - startExecuteTicks);
            IncrementPerformanceCounter("AverageTime (RequestHandler) Base");
            //ctrExecuteMethodExecutionTimeBase.Increment();
            //ctrExecuteMethodExecutionTime.Close();
            //ctrExecuteMethodExecutionTimeBase.Close();


            return responseData;
        }

        void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {

            string categoryName = "LegacyIntegrationFrameworkPerformanceMonitor";

            System.Diagnostics.PerformanceCounter ctr
                = new System.Diagnostics.PerformanceCounter();
            ctr.CounterName = "ExecutionTimeOfAdapterManager";
            ctr.CategoryName = categoryName;
            ctr.ReadOnly = false;

            string processName
                = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            ctr.InstanceName = processName;



            System.Diagnostics.PerformanceCounter ctrBase
                    = new System.Diagnostics.PerformanceCounter();
            ctrBase.CounterName = "ExecutionTimeOfAdapterManagerBase";
            ctrBase.CategoryName = categoryName;
            ctrBase.ReadOnly = false;
            ctrBase.InstanceName = processName;


            ctr.RemoveInstance();
            ctrBase.RemoveInstance();




            categoryName = "LegacyFacadePerformanceCounters";
            ctr = new System.Diagnostics.PerformanceCounter();
            ctr.CounterName = "CreateParserExecutionTime";
            ctr.CategoryName = categoryName;
            ctr.ReadOnly = false;
            ctr.InstanceName = processName;



            ctrBase = new System.Diagnostics.PerformanceCounter();
            ctrBase.CounterName = "CreateParserExecutionTimeBase";
            ctrBase.CategoryName = categoryName;
            ctrBase.ReadOnly = false;
            ctrBase.InstanceName = processName;

            ctr.RemoveInstance();
            ctrBase.RemoveInstance();

            categoryName = "LegacyFacadePerformanceCounters";
            ctr = new System.Diagnostics.PerformanceCounter();
            ctr.CounterName = "ExtractParserExecutionTime";
            ctr.CategoryName = categoryName;
            ctr.ReadOnly = false;
            ctr.InstanceName = processName;



            ctrBase = new System.Diagnostics.PerformanceCounter();
            ctrBase.CounterName = "ExtractParserExecutionTimeBase";
            ctrBase.CategoryName = categoryName;
            ctrBase.ReadOnly = false;
            ctrBase.InstanceName = processName;

            ctr.RemoveInstance();
            ctrBase.RemoveInstance();


            categoryName = "LegacyFacadePerformanceCounters";
            ctr = new System.Diagnostics.PerformanceCounter();
            ctr.CounterName = "RequestHandlerExecutionTime";
            ctr.CategoryName = categoryName;
            ctr.ReadOnly = false;
            ctr.InstanceName = processName;



            ctrBase = new System.Diagnostics.PerformanceCounter();
            ctrBase.CounterName = "RequestHandlerExecutionTimeBase";
            ctrBase.CategoryName = categoryName;
            ctrBase.ReadOnly = false;
            ctrBase.InstanceName = processName;

            ctr.RemoveInstance();
            ctrBase.RemoveInstance();

            ctr = new System.Diagnostics.PerformanceCounter();
            ctr.CounterName = "WrapExecutionTime";
            ctr.CategoryName = categoryName;
            ctr.ReadOnly = false;
            ctr.InstanceName = processName;



            ctrBase = new System.Diagnostics.PerformanceCounter();
            ctrBase.CounterName = "WrapExecutionTimeBase";
            ctrBase.CategoryName = categoryName;
            ctrBase.ReadOnly = false;
            ctrBase.InstanceName = processName;

            ctr.RemoveInstance();
            ctrBase.RemoveInstance();

            ctr = new System.Diagnostics.PerformanceCounter();
            ctr.CounterName = "UnWrapExecutionTime";
            ctr.CategoryName = categoryName;
            ctr.ReadOnly = false;
            ctr.InstanceName = processName;



            ctrBase = new System.Diagnostics.PerformanceCounter();
            ctrBase.CounterName = "UnWrapExecutionTimeBase";
            ctrBase.CategoryName = categoryName;
            ctrBase.ReadOnly = false;
            ctrBase.InstanceName = processName;

            ctr.RemoveInstance();
            ctrBase.RemoveInstance();


        }
        #endregion
        #endregion

        #region Private Methods

        #region CreateParser
        /// <summary>
        /// This Method creates the individial request strings from request objects
        /// </summary>
        private void CreateParser()
        {
            //Creating a new instance of the Cache Manager

            lifCacheManager = CacheFactory.GetCacheManager();
            
  
            
            //instantiate the response string array 
            response = new object[requests.Count];
            requestArray = new string[requests.Count];
            requestParameters = new RequestParameters[requests.Count];
            service = new Service[requests.Count];
            CacheCount = 0;
            requestCachePosition = new string[requests.Count];   
            parserCount = requests.Count;

            if (region.Equals(REGIONNAME))
            {
                serializers = new object[requests.Count];
            }

            //Fetch the keys from Hashtable to iterate.

            int count = 0;

            for (int objectCount = 0; objectCount < parserCount; objectCount++)
            {
               //fetch the object array containing information for each parser
                object[] objTemp = (object[])requests[objectCount];//[requestKey.ToString()];
                //get the position of the object in response
                response[count] = objTemp[0];

                requestParameters[count] = (RequestParameters)objTemp[1];
                //Get the Service level information
                service[count] = (Service)objTemp[2];
                //fetch the wrapper information.
                wrap = (Wrapper)objTemp[3];
                //Get the path of serializer class to be invoked
                string path = service[count].SerializerClass;
                //Get the type of serializer class to be invoked
                string type = service[count].SerializerType;

                

                requestCachePosition[count] = "Y".ToString(); 
                
                try
                {

                    
                    //Invoke the Serializer Class dynamically
                    ObjectHandle objHandle = Activator.CreateInstanceFrom(System.IO.Path.GetFullPath(path), type);
                    serializerBase = (SerializerBase)objHandle.Unwrap();
                    object temp = response[count];
                    
                    string retValue = string.Empty;

                    

                    string key = CreateCacheKey(count);

                    if (service[count].CacheEnabled == "ON")
                    {
                        if (lifCacheManager.Contains(key))
                        {
                            CacheCount++;
                            requestCachePosition[count] = count.ToString();
                            //response[objectCount] = lifCacheManager.GetData(key);
                        }
                    }
                    if (region.Equals(REGIONNAME))
                    {
                        serializers[objectCount] = serializerBase;
                    }
                    

                }
                catch (SystemException ex)
                {
                    throw new LegacyException("Parser Class" +  type + " could not be loaded", ex.InnerException);
                }
                //Set the parameters for the Serilizer class
                serializerBase.Parameters = requestParameters[count];
                //get the request string by invoking the ParseToString method on serializer class
                requestArray[count] = serializerBase.Serialize(response[count]);

                //TODO implement the Caching Feature
                count++;
            }
            string[] requestTempArray = new string[requestArray.Length-CacheCount] ;
            requestCacheArrayStore = requestArray; 
            int pos = 0;
            if (CacheCount > 0)
            {
                foreach (string strCache in requestCachePosition)
                {
                    if (strCache.Equals("Y"))
                    {
                        requestTempArray[pos] = requestArray[pos];  
                    }
                }
                requestArray = requestTempArray; 
            }
             

        }
        #endregion

        #region ExtractParser
        /// <summary>
        /// This method is invoked to get the response objects from the response string
        /// </summary>
        private void ExtractParser()
        {
            string errormessage = "";

            string[] requestTempArray = new string[requestArray.Length + CacheCount];
   
            ArrayList cacheDataList = new ArrayList();
            if (responseArray != null)
            {

                cacheDataList = new ArrayList(responseArray);
            }
            if (CacheCount > 0)
            {
                foreach (string strCache in requestCachePosition)
                {
                    if (!strCache.Equals("Y"))
                    {
                        int pos = Convert.ToInt32(strCache);
                        cacheDataList.Insert(pos, requestCacheArrayStore[pos]);   
                           
                    }
                }
                responseArray = cacheDataList.ToArray(Type.GetType("System.String")) as string[];

            }

            try
            {
                responseString = string.Empty;
                responseObjects = new object[requests.Count + 2];
                serviceNames = new string[requests.Count];
                //requestParameters = new RequestParameters[requests.Count];
                
                //set the region and position in response object array for processing.
                responseObjects[0] = region;
                responseObjects[1] = position;
                if (region.Equals(REGIONNAME))
                {
                    responseArray = new string[requests.Count];
                }


                //for caching
                string retValue = string.Empty;
                //Fill the response object array with objects placed at appropriate locations 

                for (int count = 1; count <= requests.Count; count++)
                {
                    //Get the Serializer class information

                    if (region.Equals(REGIONNAME))
                    {
                        serializerBase = (SerializerBase)serializers[count - 1];
                        responseArray[count - 1] = string.Empty;
                    }
                    else
                    {
                        string path = service[count - 1].SerializerClass;
                        string type = service[count - 1].SerializerType;
                        try
                        {
                            //Invoke the Serializer class dynamically
                            ObjectHandle objHandle = Activator.CreateInstanceFrom(System.IO.Path.GetFullPath(path), type);
                            serializerBase = (SerializerBase)objHandle.Unwrap();


                            object temp = response[count - 1];

                            //Invoke Method to return the CacheKey 
                            string key = CreateCacheKey(count - 1);
                           
                           
                            //Get the Object from serializer class and set it in the response object array
                            responseObjects[count + 1] = serializerBase.Deserialize(responseArray[count - 1]);
                            serviceNames[count - 1] = service[count - 1].ServiceName;

                            if (service[count - 1].CacheEnabled == "ON")
                            {
                                if (!lifCacheManager.Contains(key))
                                {
                                    lifCacheManager.Add(key, responseObjects[count + 1]);
                                }
                                else
                                {
                                    responseObjects[count + 1] = lifCacheManager.GetData(key);    
                                }
 
                            }


                        }
                        catch (ApplicationException ex)
                        {
                            throw new LegacyException("Serializer Class " + type + " could not be loaded", ex.InnerException);
                        }
                    }



                }

            }
            catch (SystemException  ex)
            {
                throw new LegacyException(errormessage); 
            }
            

        }
        #endregion

        #region Wrap
        /// <summary>
        /// This Method is used to add header information to the request strings
        /// </summary>
        private void Wrap()
        {            
            try
            {
                //Invoke the Wrapper class dynamically
                if (wrap.WrapperClass == null || wrap.WrapperType == null || wrap.WrapperClass.Equals(string.Empty) || wrap.WrapperType.Equals(string.Empty))
                {
                    requestString = requestArray[0];
                    return;
                }
                ObjectHandle objHandle = Activator.CreateInstanceFrom(System.IO.Path.GetFullPath(wrap.WrapperClass), wrap.WrapperType);
                wrapper = (IWrapper)objHandle.Unwrap();

            }
            catch (ApplicationException ex)
            {
                throw new LegacyException("Wrapper Class could not be loaded", ex.InnerException);
            }
            //Create the complete request string 
            requestString = wrapper.CreateRequest(requestArray, requestParameters[0]);

        }
        #endregion

        #region UnWrap
        /// <summary>
        /// This method extracts the response from response string by removing Header information
        /// </summary>
        private void UnWrap()
        {

            try
            {
                //Invoke the Wrapper class dynamically
                if (wrap.WrapperClass == null || wrap.WrapperType == null || wrap.WrapperClass.Equals(string.Empty) || wrap.WrapperType.Equals(string.Empty))
                {
                    responseArray = new string[1];
                    responseArray[0] = responseString;
                    return;
                }
                ObjectHandle objHandle = Activator.CreateInstanceFrom(System.IO.Path.GetFullPath(wrap.WrapperClass), wrap.WrapperType);
                wrapper = (IWrapper)objHandle.Unwrap();

            }
            catch (ApplicationException ex)
            {
                throw new LegacyException("Wrapper Class could not be loaded", ex.InnerException);
            }
            //Extract the response data from response string.
            if (!string.IsNullOrEmpty(responseString))
            {
                responseArray = wrapper.Extract(responseString);
            }

        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private string CreateCacheKey(int position)
        {
            string key = string.Empty;
            if (service[position].CacheEnabled == "ON")
            {
                string[] cacheIdentifiers;
                string[] keys = requestParameters[position].RequestCollection.AllKeys;
                bool keyInParameters = false;
                foreach (string cachekey in keys)
                {
                    if (cachekey.Equals("CacheIdentifier"))
                    {
                        keyInParameters = true;
                    }
                }
                if (keyInParameters)
                {
                    if (requestParameters[position].RequestCollection["CacheIdentifier"].Equals(string.Empty))
                    {
                        cacheIdentifiers = service[position].CacheIdentifier.Split(",".ToCharArray());
                    }
                    else
                    {
                        cacheIdentifiers = requestParameters[position].RequestCollection["CacheIdentifier"].Split(",".ToCharArray());
                    }
                }
                else
                {
                    cacheIdentifiers = service[position].CacheIdentifier.Split(",".ToCharArray());
                }
                key = region + "+" + service[position].ServiceName;
                
                //requestParameters[count].RequestCollection[    
                foreach (string str in cacheIdentifiers)
                {
                    if (!str.Contains("."))
                    {
                        PropertyInfo pi = response[position].GetType().GetProperty(str);
                        key = key + "+" + pi.GetValue(response[position], null).ToString();
                    }
                    else
                    {
                        string[] cacheIdTemp = str.Split(".".ToCharArray());
                        PropertyInfo pi = response[position].GetType().GetProperty(cacheIdTemp[0]);
                        ArrayList objtemp = (ArrayList)pi.GetValue(response[position], null);
                        pi = objtemp[0].GetType().GetProperty(cacheIdTemp[1]);
                        key = key + "+" + pi.GetValue(objtemp[0], null).ToString();
                    }

                }
               

               
            }
            return key;
        }

        private System.Diagnostics.PerformanceCounter CreatePerformanceCounter(string counterName)
        {
            if (IsPerformanceCountersEnabled)
            {
                string categoryName = "LegacyFacade";
                string processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;

                System.Diagnostics.PerformanceCounter performanceCounter
                    = new System.Diagnostics.PerformanceCounter();
                performanceCounter.CategoryName = categoryName;
                performanceCounter.CounterName = counterName;
                performanceCounter.ReadOnly = false;
                performanceCounter.InstanceName = processName;
                if (!isAppDomainUnloadRegistered)
                {
                    // Register eventhandler for appdomain unload event. In eventhandler
                    // performance counter instance can removed.
                    AppDomain app = AppDomain.CurrentDomain;
                    app.DomainUnload += new EventHandler(CurrentDomain_DomainUnload);
                    isAppDomainUnloadRegistered = true;
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


        internal static string isPerformanceCountersEnabled = string.Empty;
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
                // If isPerformanceCountersEnabled is YES, return true, else false
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


    }
}
