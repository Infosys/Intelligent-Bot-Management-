/****************************************************************
 * This file is a part of the Legacy Integration Framework.
 * This file is the Entry point for the framework and manages the service calls.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Specialized;
using Infosys.Lif.LegacyFacade;
using Infosys.Lif.LegacyParameters;
using Infosys.Lif.LegacyConfig;
using Infosys.Lif.LegacyCommon;
using System.Configuration;
//using Microsoft.Practices.EnterpriseLibrary.Configuration;     

namespace Infosys.Lif.LegacyFacade
{

    #region delegate RequestDelegate
    /// <summary>
    /// The delegate indicates what types of methods can be notified of an event.
    /// The method(s) must have the same signature as the delegate.
    /// </summary>
    /// <param name="sender">The Source of Event</param>
    /// <param name="responses">The object array containing the responses</param>
    public delegate void RequestDelegate(object sender, RequestDelegateArgs responses);
    #endregion

    #region ServiceManager Class
    /// <summary>
    /// This Class contains the methods for invoking and using the framework
    /// </summary>
    public class ServiceManager
    {

        #region Private Fields
        //Hashtable containing the request objects stored as per Region
        private const string CONFIGSECTIONAME = "LegacyFacade";
        private Hashtable regions;
        private Hashtable requestCounter;
        //Event object for Synchronous operations
        private LegacyEvent waitForAllEvent = new LegacyEvent();
        //Event Method for Asynchronous operations
        private LegacyEvent waitForNoneEvent = new LegacyEvent();
        //Configuration Information
        private LegacySettings legacySettings;
        private ServiceCollection serviceCollection;
        private Wrapper wrap;

        //ArrayLists containing processing Information
        private ArrayList totalExecuteList = new ArrayList();
        private ArrayList inputCounter = new ArrayList();
        private ArrayList outputCounter = new ArrayList();
        private ArrayList responseList = new ArrayList();
        private int count;
        private int position;

        #endregion

        #region Enumerations

        #region ProcessMode
        /// <summary>
        /// The ProcessMode indicates the type of operation
        /// WaitForAll: Synchronous
        /// WaitForNone: Asynchronous
        /// </summary>
        public enum ProcessMode
        {
            WaitForAll = 1,
            WaitForNone = 2
        }
        #endregion
        #endregion

        #region public methods
        #region Constructor
        /// <summary>
        /// This is the Constructor method
        /// </summary>
        public ServiceManager()
        {
            count = 0;
            position = 0;
            try
            {
                regions = new Hashtable();
                requestCounter = new Hashtable();
                //Fetch the Configuration Information
                legacySettings = RetrieveConfigDetails();
                serviceCollection = legacySettings.Services.ServiceCollection;
                wrap = legacySettings.Wrapper;
            }
            catch (ApplicationException ex)
            {
                throw new LegacyException("Exception in Configuration File", ex.InnerException);
            }

        }

        /// <summary>
        /// Will be used to retrieve the configuration details.
        /// </summary>
        /// <returns></returns>
        private static LegacySettings RetrieveConfigDetails()
        {
            LegacySettings legacyConfigDetails = (LegacySettings)ConfigurationManager.GetSection("LegacySettings");
            RequestHandler.isPerformanceCountersEnabled 
                = legacyConfigDetails.EnablePerformanceCounters;
            return legacyConfigDetails;
        }
        #endregion

        #region BeginBatch
        /// <summary>
        /// This method indicates the beginning of a batch operation
        /// </summary>
        public static void BeginBatch()
        {
            //Get the data From TLS
            LocalDataStoreSlot dataSlot;
            dataSlot = Thread.GetNamedDataSlot(CONFIGSECTIONAME);
            ArrayList arrayList = new ArrayList();
            arrayList = (ArrayList)Thread.GetData(dataSlot);
            if (arrayList == null)
                arrayList = new ArrayList();

            //Instantiate a new object of ServiceManager
            LegacyFacade.ServiceManager serviceManager = new ServiceManager();
            //Add the serviceManager object to Arraylist
            arrayList.Add(serviceManager);
            //Store the Arraylist back in TLS
            Thread.SetData(dataSlot, arrayList);
        }
        #endregion

        #region ExecuteBatch
        /// <summary>
        /// The ExecuteBatch method Executes a Batch operation/Transaction.
        /// It controls the Asynchronous and Synchronous modes of operation
        /// </summary>
        /// <param name="legacyEvent">The Event to be raised after completion of execution</param>
        /// <param name="processMode">The Mode of operation Asynchronous/Synchronous</param>
        public static void ExecuteBatch(LegacyEvent legacyEvent, ProcessMode processMode)
        {
            //Fetch the data from TLS
            LocalDataStoreSlot dataSlot;

            dataSlot = Thread.GetNamedDataSlot(CONFIGSECTIONAME);
            ArrayList arrayList = new ArrayList();
            arrayList = (ArrayList)Thread.GetData(dataSlot);
            //Get the serviceManager object from the Arrylist stored in TLS
            LegacyFacade.ServiceManager serviceManager = new ServiceManager();
            serviceManager = (ServiceManager)arrayList[arrayList.Count - 1];
            if (serviceManager == null)
            {
                throw new LegacyException("Error In Legacy Framework. BeginBatch Method has not been invoked");
            }


            LegacyEvent responseEvent = new LegacyEvent();
            RegionHandler RegionHandle = new RegionHandler();

            //For Asynchronous and Synchronous operations
            RegionHandle.StartProcess(serviceManager.regions, legacyEvent, processMode, serviceManager.requestCounter, serviceManager.position);

            //Reset the Class level information for next Execute Call
            serviceManager.regions = new Hashtable();
            serviceManager.requestCounter = new Hashtable();
            serviceManager.legacySettings = RetrieveConfigDetails();
            serviceManager.serviceCollection = serviceManager.legacySettings.Services.ServiceCollection;
            serviceManager.wrap = serviceManager.legacySettings.Wrapper;
            serviceManager.count = 0;
            serviceManager.position++;


        }
        #endregion

        #region Add
        /// <summary>
        /// This Method Adds a service for Execution
        /// </summary>
        /// <param name="serviceName">The Name of service to be executed</param>
        /// <param name="dataEntity">The request DataEntity</param>
        /// <param name="requestParameters">Parameter Collection containing processing information</param>
        public static void Add(string serviceName, object dataEntity, object requestParameters)
        {

            //Fetch the ServiceManager Instance from TLS
            LocalDataStoreSlot dataSlot;

            dataSlot = Thread.GetNamedDataSlot(CONFIGSECTIONAME);
            ArrayList arrayList = new ArrayList();
            arrayList = (ArrayList)Thread.GetData(dataSlot);

            LegacyFacade.ServiceManager serviceManager = new ServiceManager();
            serviceManager = (ServiceManager)arrayList[arrayList.Count - 1];

            //increment the count of services added
            serviceManager.count++;
            string regionName = string.Empty;
            Service service = new Service();
            bool IsServicePresent = false;
            //Fetch the configuration information for service and to identify the Region
            foreach (Service s in serviceManager.serviceCollection)
            {
                if (s.ServiceName == serviceName)
                {
                    regionName = s.RegionName;
                    service = s;
                    IsServicePresent = true;
                    break;
                }
            }
            if (!IsServicePresent)
                throw new LegacyException("Service" + serviceName + "Not found in Config File");

            //If Region does not exist in Hashtable
            if (!serviceManager.regions.ContainsKey(regionName))
            {
                //Create a Hashtable for the request Information
                //Changed now Hashtable request = new Hashtable();

                ArrayList request = new ArrayList();
                object[] reqobj = new object[4];
                //Add the Data Entity.
                reqobj[0] = dataEntity;
                //Add Parameter Collection
                reqobj[1] = (RequestParameters)requestParameters;
                //Add Service object containing Configuration
                reqobj[2] = service;
                //Add Wrapper Class Information
                reqobj[3] = serviceManager.wrap;
                //request.Add(serviceName, reqobj);
                request.Add(reqobj);
                //Add to the regions Hashtable
                serviceManager.regions.Add(regionName, request);

                //Set the Counter for the current service
                //This is used for identifying the response objects 
                //And to set them in order.
                ArrayList counter = new ArrayList();
                counter.Add(serviceManager.count);
                serviceManager.requestCounter.Add(regionName, counter);

            }
            else
            {
                //Fetch Hashtable for request from region Hashtable
                //Hashtable request = (Hashtable)serviceManager.regions[regionName];
                ArrayList request = (ArrayList)serviceManager.regions[regionName];
                object[] reqobj = new object[4];
                //Add the DataEntity
                reqobj[0] = dataEntity;
                //Add the Parameter Collection
                reqobj[1] = (RequestParameters)requestParameters;
                //Add Service object containing Configuration
                reqobj[2] = service;
                //Add Wrapper Class Information
                reqobj[3] = serviceManager.wrap;
                //request.Add(serviceName, reqobj);
                request.Add(reqobj);
                serviceManager.regions[regionName] = request;

                //Set the Counter for the current service
                //This is used for identifying the response objects 
                //And to set them in order.
                ArrayList counter = (ArrayList)serviceManager.requestCounter[regionName];
                counter.Add(serviceManager.count);
                serviceManager.requestCounter[regionName] = counter;
            }
            //add the service manager object into the Arraylist and store it in TLS
            arrayList[arrayList.Count - 1] = serviceManager;

            Thread.SetData(dataSlot, arrayList);
        }
        #endregion

        #region Execute
        /// <summary>
        /// The Execute Method is called for single invocations of the component.
        /// It can be used when a batch operation is not required.
        /// </summary>
        /// <param name="legacyEvent">The Event to be raised after completion of execution</param>
        /// <param name="processMode">The Mode of operation Asynchronous/Synchronous</param>
        /// <param name="serviceName">The Name of service to be executed</param>
        /// <param name="dataEntity">The request DataEntity</param>
        /// <param name="requestParameters">Parameter Collection containing processing information</param>
        public static void Execute(LegacyEvent legacyEvent, ProcessMode processMode, string serviceName, object dataEntity, object requestParameters)
        {
            ServiceManager serviceManager = new ServiceManager();

            //increment the count of services added
            serviceManager.count++;
            string regionName = string.Empty;
            Service service = new Service();
            bool IsServicePresent = false;
            //Fetch the configuration information for service and to identify the Region
            foreach (Service s in serviceManager.serviceCollection)
            {
                if (s.ServiceName == serviceName)
                {
                    regionName = s.RegionName;
                    service = s;
                    IsServicePresent = true;
                    break;
                }
            }
            if (!IsServicePresent)
                throw new LegacyException("Service" + serviceName + "Not found in Config File");

            //If Region does not exist in Hashtable
            if (!serviceManager.regions.ContainsKey(regionName))
            {
                //Create a Hashtable for the request Information
                //Changed now Hashtable request = new Hashtable();

                ArrayList request = new ArrayList();
                object[] reqobj = new object[4];
                //Add the Data Entity.
                reqobj[0] = dataEntity;
                //Add Parameter Collection
                reqobj[1] = (RequestParameters)requestParameters;
                //Add Service object containing Configuration
                reqobj[2] = service;
                //Add Wrapper Class Information
                reqobj[3] = serviceManager.wrap;
                //request.Add(serviceName, reqobj);
                request.Add(reqobj);
                //Add to the regions Hashtable
                serviceManager.regions.Add(regionName, request);

                //Set the Counter for the current service
                //This is used for identifying the response objects 
                //And to set them in order.
                ArrayList counter = new ArrayList();
                counter.Add(serviceManager.count);
                serviceManager.requestCounter.Add(regionName, counter);

            }
            else
            {
                //Fetch Hashtable for request from region Hashtable
                //Hashtable request = (Hashtable)serviceManager.regions[regionName];
                ArrayList request = (ArrayList)serviceManager.regions[regionName];
                object[] reqobj = new object[4];
                //Add the DataEntity
                reqobj[0] = dataEntity;
                //Add the Parameter Collection
                reqobj[1] = (RequestParameters)requestParameters;
                //Add Service object containing Configuration
                reqobj[2] = service;
                //Add Wrapper Class Information
                reqobj[3] = serviceManager.wrap;
                //request.Add(serviceName, reqobj);
                request.Add(reqobj);
                serviceManager.regions[regionName] = request;

                //Set the Counter for the current service
                //This is used for identifying the response objects 
                //And to set them in order.
                ArrayList counter = (ArrayList)serviceManager.requestCounter[regionName];
                counter.Add(serviceManager.count);
                serviceManager.requestCounter[regionName] = counter;
            }

            LegacyEvent responseEvent = new LegacyEvent();
            RegionHandler RegionHandle = new RegionHandler();

            //For Asynchronous and Synchronous operations
            RegionHandle.StartProcess(serviceManager.regions, legacyEvent, processMode, serviceManager.requestCounter, serviceManager.position);

            //Reset the Class level information for next Execute Call
            serviceManager.regions = new Hashtable();
            serviceManager.requestCounter = new Hashtable();
            serviceManager.legacySettings = RetrieveConfigDetails();
            serviceManager.serviceCollection = serviceManager.legacySettings.Services.ServiceCollection;
            serviceManager.wrap = serviceManager.legacySettings.Wrapper;
            serviceManager.count = 0;
            serviceManager.position++;
        }
        #endregion

        #region Add
        /// <summary>
        /// This is an overloaded add method which takes an additional parameter RegionName
        /// </summary>
        /// <param name="serviceName">The Name of service to be executed</param>
        /// <param name="dataEntity">The request DataEntity</param>
        /// <param name="requestParameters">Parameter Collection containing processing information</param>
        /// <param name="regionName">The RegionName where request is sent</param>
        public static void Add(string serviceName, object dataEntity, object requestParameters, string regionName)
        {

            //Fetch the ServiceManager Instance from TLS
            LocalDataStoreSlot dataSlot;
            dataSlot = Thread.GetNamedDataSlot(CONFIGSECTIONAME);
            ArrayList arrayList = new ArrayList();
            arrayList = (ArrayList)Thread.GetData(dataSlot);
            LegacyFacade.ServiceManager serviceManager = new ServiceManager();
            serviceManager = (ServiceManager)arrayList[arrayList.Count - 1];

            //Increment the service count
            serviceManager.count++;
            Service service = new Service();
            bool IsServicePresent = false;
            //Fetch the configuration information for service and to identify the Region
            foreach (Service s in serviceManager.serviceCollection)
            {
                if (s.ServiceName == serviceName)
                {
                    service = s;
                    IsServicePresent = true;
                    break;
                }

            }
            if (!IsServicePresent)
                throw new LegacyException("Service" + serviceName + "Not found in Config File");

            //If the region Name is not a key in the regions Hashtable
            if (!serviceManager.regions.ContainsKey(regionName))
            {
                //Create a Hashtable for the request Information
                Hashtable request = new Hashtable();
                object[] reqobj = new object[4];
                //Add the Data Entity.
                reqobj[0] = dataEntity;
                //Add Parameter Collection
                reqobj[1] = (RequestParameters)requestParameters;
                //Add Service object containing Configuration
                reqobj[2] = service;
                //Add Wrapper Class Information
                reqobj[3] = serviceManager.wrap;
                request.Add(serviceName, reqobj);
                serviceManager.regions.Add(regionName, request);

                //Set the Counter for the current service
                //This is used for identifying the response objects 
                //And to set them in order.
                ArrayList counter = new ArrayList();
                counter.Add(serviceManager.count);
                serviceManager.requestCounter.Add(regionName, counter);
            }
            else
            {
                //Fetch Hashtable for request from region Hashtable
                Hashtable request = (Hashtable)serviceManager.regions[regionName];
                object[] reqobj = new object[4];
                //Add the DataEntity
                reqobj[0] = dataEntity;
                //Add the Parameter Collection
                reqobj[1] = (RequestParameters)requestParameters;
                //Add Service object containing Configuration
                reqobj[2] = service;
                //Add Wrapper Class Information
                reqobj[3] = serviceManager.wrap;
                string key = string.Empty;

                request.Add(serviceName, reqobj);
                serviceManager.regions[regionName] = request;


                //Set the Counter for the current service
                //This is used for identifying the response objects 
                //And to set them in order.
                ArrayList counter = (ArrayList)serviceManager.requestCounter[regionName];
                counter.Add(serviceManager.count);
                serviceManager.requestCounter[regionName] = counter;


            }

            //add the service manager object into the Arraylist and store it in TLS
            arrayList[arrayList.Count - 1] = serviceManager;
            Thread.SetData(dataSlot, arrayList);
        }
        #endregion

        #region RemoveBatch

        /// <summary>
        /// This method Removes the last instance of ServiceManager from TLS
        /// </summary>
        public static void RemoveBatch()
        {
            //Fetch the data from TLS
            try
            {
                LocalDataStoreSlot dataSlot;

                dataSlot = Thread.GetNamedDataSlot(CONFIGSECTIONAME);
                ArrayList arrayList = new ArrayList();
                arrayList = (ArrayList)Thread.GetData(dataSlot);
                if (arrayList.Count == 0)
                {
                    throw new LegacyException("No transaction started");
                }
                //Remove the Last Transaction.
                arrayList.RemoveAt(arrayList.Count - 1);
                Thread.SetData(dataSlot, arrayList);
            }
            catch (LegacyException legacyException)
            {
                throw new LegacyException(legacyException.Message);
            }

        }


        #endregion


        #endregion



    }
    #endregion

    #region RequestDelegateArgs Class
    /// <summary>
    /// This Class contains Arguments for the LegacyEvent
    /// </summary>
    public class RequestDelegateArgs : System.EventArgs
    {


        #region Private Fields
        private ResponseCollection response;
        #endregion

        #region Methods

        #region Constructor
        /// <summary>
        /// This is the constructor method
        /// </summary>
        /// <param name="responseObjects">Object Array containing response objects</param>
        /// <param name="serviceNames">Object Array containing response objects</param>
        public RequestDelegateArgs(object[] responseObjects, string[] serviceNames)
        {
            this.response = new ResponseCollection();
            for (int i = 0; i < responseObjects.Length; i++)
                response.Add(serviceNames[i], responseObjects[i]);
        }
        #endregion
        #endregion

        #region Properties
        /// <summary>
        /// Property Response which contains the ResponseCollection
        /// </summary>
        public ResponseCollection Response
        {
            get
            {
                return response;
            }
        }
        #endregion

    }
    #endregion

    #region class ResponseCollection
    /// <summary>
    /// The ResponseCollection class contains the Response objects
    /// and their respective service Names.
    /// It implements NameObjectCollectionBase
    /// </summary>
    public class ResponseCollection : NameObjectCollectionBase
    {

        #region Public Methods

        #region Constructor
        /// <summary>
        /// The Constructor Method
        /// </summary>
        public ResponseCollection()
        {

        }
        #endregion

        #region Add
        /// <summary>
        /// Adds an entry to the collection.
        /// </summary>
        /// <param name="serviceName">Name of the Service which is the key</param>
        /// <param name="responseObject">response Object for the service</param>
        public void Add(String serviceName, object responseObject)
        {
            this.BaseAdd(serviceName, responseObject);
        }
        #endregion

        #region Remove(serviceName)
        /// <summary>
        /// Removes an entry with the specified key from the collection.
        /// </summary>
        /// <param name="serviceName">Name of the Service which is the Key</param>
        public void Remove(String serviceName)
        {
            this.BaseRemove(serviceName);
        }
        #endregion

        #region Remove
        /// <summary>
        /// Removes an entry in the specified index from the collection.
        /// </summary>
        /// <param name="index">position from where the object is to be removed</param>
        public void Remove(int index)
        {
            this.BaseRemoveAt(index);
        }
        #endregion

        #region Clear
        /// <summary>
        /// Clears all the elements in the collection. 
        /// </summary>
        public void Clear()
        {
            this.BaseClear();
        }
        #endregion

        #endregion


        #region Properties
        #region this
        /// <summary>
        /// Gets or sets the value associated with the specified key. 
        /// </summary>
        /// <param name="serviceName">the ServiceName</param>
        /// <returns>response object for the service</returns>
        public Object this[String serviceName]
        {
            get
            {
                return (this.BaseGet(serviceName));
            }
            set
            {
                this.BaseSet(serviceName, value);
            }
        }
        #endregion

        #region ServiceName
        /// <summary>
        /// Gets a String array that contains all the ServiceNames in the ResponseCollection. 
        /// </summary>
        public String[] ServiceName
        {
            get
            {
                return (this.BaseGetAllKeys());
            }
        }

        #endregion

        #region ResponseData
        /// <summary>
        /// Gets an Object array that contains all the response objects in the collection. 
        /// </summary>
        public object[] ResponseData
        {
            get
            {
                return (this.BaseGetAllValues());
            }
        }
        #endregion

        #region HasKeys
        /// <summary>
        /// Gets a value indicating if the collection contains keys that are not null.
        /// </summary>
        public Boolean HasKeys
        {
            get
            {
                return (this.BaseHasKeys());
            }
        }
        #endregion
        #endregion


    }
    #endregion

    #region LegacyEvent Class
    /// <summary>
    /// This is the Event Class.
    /// </summary>
    public class LegacyEvent
    {
        /// <summary>
        /// This Event is raised when the response is received from host.
        /// </summary>
        public event RequestDelegate ResponseReceived;

        #region Event Method OnResponseReceived
        /// <summary>
        /// This is the Method to Handle the event
        /// </summary>
        /// <param name="requestDelegateArgs">The Event Argument</param>
        public virtual void OnResponseReceived(RequestDelegateArgs requestDelegateArgs)
        {
            if (ResponseReceived != null)
            {
                // Invokes the delegates. 
                ResponseReceived(this, requestDelegateArgs);
            }
        }
        #endregion
    }

    #endregion
}