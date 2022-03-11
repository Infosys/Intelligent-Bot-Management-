/****************************************************************
 * This file is a part of the Legacy Integration Framework.
 * This file provides custom connection pool.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/
#region Using
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Configuration;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using IBM.WMQ;
using Infosys.Lif.LegacyCommon;
using Infosys.Lif.LegacyIntegratorService;
//using System.Diagnostics;
using System.IO;

#endregion

namespace Infosys.Lif.IBMMQ
{
    #region MQPoolManager
    /// <summary>
    /// Manages the connection pool for IBM MQ.
    /// </summary>
    internal sealed class MQPoolManager
    {
        #region Private Variables
        static ArrayList[] connectionsQueue;
        static int[] currentPoolSize;
        static int[] currentOpenConnections;
        static bool isInitialized;
        //static int[] connectionTimeOut;
        static int[] minSize;
        static ArrayList uniqueConnectionString;
        static int numberOfPool;
        static object syncObject = new Object();
        static ArrayList timeStampCollection;
        static int activeTimeForConnection;
        //static PerformanceCounter totalConnection = new PerformanceCounter();
        //static PerformanceCounter connectionPerPool = new PerformanceCounter();
        static int numberOfPoolScans;
        static System.Timers.Timer poolCleanUpTimer = new System.Timers.Timer();
        static LogEntry logEntry = new LogEntry();
        #endregion

        #region Constants
        const string CATEGORY = "LegacyIntegrator";
        const string TOTAL_ACTIVE_CONNECTIONS = "MQ - TotalActiveConnections";
        const string ACTIVE_CONNECTIONS_PER_POOL = "MQ - ActiveConnectionsPerPool";
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
        const string DEBUG_INFO = "MQAdapterInformation";
        #endregion

        #region Methods

        #region MQPoolManager
        private MQPoolManager()
        {
        }
        #endregion

        #region Inintialize
        /// <summary>
        /// Initializes connection pool with MinSize number of conections.
        /// </summary>
        public static void Initialize(Infosys.Lif.LegacyIntegratorService.IBMMQ transportSection)
        {
            try
            {
                if (!isInitialized)  //first check
                {
                    lock (syncObject)
                    {
                        if (!isInitialized) //second check
                        {
                            isPerformanceCountersEnabled
                                = transportSection.EnablePerformanceCounters;

                            //// Initialize logEntry
                            //logEntry.Category = DEBUG_INFO;
                            //logEntry.Priority = 5;
                            //logEntry.Severity = Severity.Information;

                            //logEntry.Message = "connection pool initialized is called";
                            //Logger.Write(logEntry);

                            // Register eventhandler for appdomain unload event. In eventhandler
                            // performance counter instance can removed.
                            //AppDomain app = AppDomain.CurrentDomain;
                            //app.DomainUnload += new EventHandler(ClearConnectionPoolCounters);

                            bool isGlobalConnectionModelIsPooling = false;
                            // Create a arraylist object to contain unique connection string information.
                            uniqueConnectionString = new ArrayList();

                            // Get the default connection pool information.
                            Connection globalConnectionInformation = transportSection.Connection as Connection;
                            int globalMinSize = -1;
                            int globalMaxSize = -1;
                            int globalConnTimeOut = -1;

                            // If connection pool is required then read min size, max size and connectionTimeOut
                            // These values will be used if Connection is specified in IBMMQDetails section.
                            if (!string.IsNullOrEmpty(globalConnectionInformation.ConnectionModel))
                            {
                                if (globalConnectionInformation.ConnectionModel == ConnectionModelType.ConnectionPool.ToString())
                                {
                                    isGlobalConnectionModelIsPooling = true;
                                    Pooling globalPooling = globalConnectionInformation.Pooling as Pooling;
                                    if (globalPooling == null)
                                    {
                                        throw new LegacyException("Pooling information is not defined for IBMMQ");
                                    }
                                    globalMinSize = globalPooling.MinSize;
                                    globalMaxSize = globalPooling.MaxSize;
                                    globalConnTimeOut = globalConnectionInformation.ConnectionTimeOut;
                                }
                                else
                                {
                                    isGlobalConnectionModelIsPooling = false;
                                }
                            }

                            // Get all the region.
                            MQConnectionInformation newConnInfo;
                            IBMMQDetails individualRegion;
                            Connection localConnectionInformartion;
                            Pooling localPooling;

                            string queueManagerName;
                            string channelName;
                            string connectionName;
                            int port;

                            // Indicates whether IBMMQDetails section is Connection pool or none
                            bool isPoolingRequired = false;

                            // For all region, find the connection model information and add to 
                            // uniqueConnectionString arraylist if ConnectionModel is connection pool 
                            // and if it is not already present in uniqueConnectionString.
                            for (int index = 0; index < transportSection.IBMMQDetailsCollection.Count; index++)
                            {
                                isPoolingRequired = false;

                                individualRegion = transportSection.IBMMQDetailsCollection[index] as IBMMQDetails;

                                localConnectionInformartion = individualRegion.Connection as Connection;
                                newConnInfo = new MQConnectionInformation();

                                if (!string.IsNullOrEmpty(localConnectionInformartion.ConnectionModel))
                                {
                                    if (localConnectionInformartion.ConnectionModel == ConnectionModelType.ConnectionPool.ToString())
                                    {
                                        localPooling = localConnectionInformartion.Pooling as Pooling;
                                        if (localPooling == null)
                                        {
                                            throw new LegacyException("pooling information is not defined for IBMMQ");
                                        }

                                        newConnInfo.MinSize = localPooling.MinSize;
                                        newConnInfo.MaxSize = localPooling.MaxSize;
                                        newConnInfo.ConnectionTimeOut = localConnectionInformartion.ConnectionTimeOut;

                                        // Set the flag indicating this section uses pooling
                                        isPoolingRequired = true;
                                    }
                                    else
                                    {
                                        // Set the flag indicating this section does not uses pooling
                                        isPoolingRequired = false;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (isGlobalConnectionModelIsPooling)
                                    {
                                        newConnInfo.MinSize = globalMinSize;
                                        newConnInfo.MaxSize = globalMaxSize;
                                        newConnInfo.ConnectionTimeOut = globalConnTimeOut;

                                        // Set the flag indicating this section uses pooling
                                        isPoolingRequired = true;
                                    }
                                    else
                                    {
                                        // Set the flag indicating this section does not uses pooling
                                        isPoolingRequired = false;
                                    }
                                }

                                if (isPoolingRequired)
                                {
                                    // Read queue manager, channel and connection.
                                    queueManagerName = individualRegion.QueueManager as string;
                                    channelName = individualRegion.ChannelName as string;
                                    connectionName = individualRegion.ConnectionName as string;
                                    port = individualRegion.Port;

                                    if (string.IsNullOrEmpty(queueManagerName))
                                    {
                                        if (string.IsNullOrEmpty(channelName))
                                        {
                                            throw new LegacyException("Channel name cannot be empty when QueueManager is empty");
                                        }
                                        if (string.IsNullOrEmpty(connectionName))
                                        {
                                            throw new LegacyException("Connection name cannot be empty when QueueManager is empty");
                                        }
                                    }

                                    connectionName = connectionName + "(" + port + ")";

                                    newConnInfo.QueueManagerName = queueManagerName;
                                    newConnInfo.ChannelName = channelName;
                                    newConnInfo.ConnectionName = connectionName;

                                    // If connection string does not exist in arraylist then add it.
                                    if (!uniqueConnectionString.Contains(newConnInfo))
                                    {
                                        uniqueConnectionString.Add(newConnInfo);
                                    }
                                }
                                // count of arraylist gives number connection pool required.
                                numberOfPool = uniqueConnectionString.Count;

                                //logEntry.Message = "Number of connection pool identified " + numberOfPool;
                                //Logger.Write(logEntry);
                            }

                            // At this point uniqueConnectionString will contain individual connection string information.
                            // Create so many queues to hold connection object
                            connectionsQueue = new ArrayList[numberOfPool];
                            currentPoolSize = new int[numberOfPool];
                            currentOpenConnections = new int[numberOfPool];
                            //connectionTimeOut = new int[numberOfPool];
                            minSize = new int[numberOfPool];

                            // Create queue for holding the connection
                            for (int queueNumber = 0; queueNumber < numberOfPool; queueNumber++)
                            {
                                connectionsQueue[queueNumber] = new ArrayList();
                            }

                            if (numberOfPool != 0)
                            {
                                // Set the timer 
                                poolCleanUpTimer.Enabled = true;
                                poolCleanUpTimer.AutoReset = true;
                                poolCleanUpTimer.Interval = (double)transportSection.PoolScanFrequency;
                                poolCleanUpTimer.Elapsed += new System.Timers.ElapsedEventHandler(ReleaseConnectionsFromPool);
                                activeTimeForConnection = transportSection.ActiveTimeForConnection;
                                //logEntry.Message = "Timer is initialized";
                                //Logger.Write(logEntry);
                                // Create a arraylist which will load MQQueueManager and timestamp.
                                timeStampCollection = new ArrayList();
                            }

                            //Performance counter for total number of connectiona
                            //totalConnection.CategoryName = CATEGORY;
                            //totalConnection.CounterName = TOTAL_ACTIVE_CONNECTIONS;
                            //totalConnection.ReadOnly = false;
                            //totalConnection.InstanceName = COUNTER_INSTANCE;

                            //Performance counter for total number of connections per pool
                            //connectionPerPool.CategoryName = CATEGORY;
                            //connectionPerPool.CounterName = ACTIVE_CONNECTIONS_PER_POOL;
                            //connectionPerPool.ReadOnly = false;
                            //connectionPerPool.InstanceName = COUNTER_INSTANCE;
                            isInitialized = true;
                        }
                    }
                }
            }
            catch (MQException exception)
            {
                throw new LegacyException("PoolManager cannot be initialized", exception);
            }
            catch (Exception exception)
            {
                throw new LegacyException("PoolManager cannot be initialized", exception);
            }
        }
        #endregion

        #region GetConnection
        /// <summary>
        /// Get the connection object from the queue and returns the same.
        /// </summary>
        /// <param name="connectionString">Details required for identifing the 
        /// connection</param>
        /// <returns>MQQueueManager</returns>
        public static MQQueueManager GetConnection(MQConnectionInformation connectionString)
        {
            if (uniqueConnectionString == null)
            {
                throw new LegacyException("Pool is not initialized");
            }

            int index = uniqueConnectionString.IndexOf(connectionString);

            // If passed connection string does not exist in the connecton pool then raise the 
            // exception.
            if (index == -1)
            {
                throw new LegacyException("Wrong connection parameter");
            }

            AddConnectionsToPool(connectionString, index);

            MQQueueManager queueManager = null;
            lock (syncObject)
            {
                //logEntry.Message = "GetConnection is called";
                //Logger.Write(logEntry);

                // If connection exist in the queue then get the connection object and return
                // the same.
                if (currentPoolSize[index] > 0)
                {
                    // Get the connection object which is connected
                    while (currentPoolSize[index] > 0)
                    {
                        // Get the object from arraylist and then remove it.
                        queueManager = (MQQueueManager)connectionsQueue[index][connectionsQueue[index].Count - 1];
                        connectionsQueue[index].Remove(queueManager);

                        currentPoolSize[index] = currentPoolSize[index] - 1;

                        // update the timestamp
                        if (timeStampCollection.IndexOf(queueManager) != -1)
                        {
                            ConnectionTimeStamp connectionTimeStamp = (ConnectionTimeStamp)timeStampCollection[timeStampCollection.IndexOf(queueManager)];
                            connectionTimeStamp.IsPresentInPool = false;
                        }
                        if (queueManager.IsConnected)
                        {
                            break;
                        }
                        else
                        {
                            // If it is disconnected then remove it from TimestampCollection.
                            if (timeStampCollection.Contains(queueManager))
                            {
                                timeStampCollection.Remove(queueManager);
                            }
                        }
                    }
                }

                // If no free connection are available then create connection and return the same if
                // if number of open connection is less than MaxSize other wait till one is freed.
                if (queueManager == null || !queueManager.IsConnected)
                {
                    //when One request is waiting for a connection other request should wait until the first 
                    //request is served. So lock is needed.
                    if (currentPoolSize[index] == 0)
                    {
                        //if Connection created is less than maximum size defined in Config file create
                        // a new connection and return it.
                        if (currentOpenConnections[index] < connectionString.MaxSize)
                        {
                            if (string.IsNullOrEmpty(connectionString.QueueManagerName))
                            {
                                MQEnvironment.Channel = connectionString.ChannelName;
                                MQEnvironment.Hostname = connectionString.ConnectionName;
                                queueManager = new MQQueueManager();
                                queueManager.OpenOptions = MQC.MQCNO_HANDLE_SHARE_BLOCK;
                            }
                            else if (string.IsNullOrEmpty(connectionString.ChannelName) || string.IsNullOrEmpty(connectionString.ConnectionName))
                            {
                                queueManager = new MQQueueManager(connectionString.QueueManagerName, MQC.MQCNO_HANDLE_SHARE_BLOCK);
                            }
                            else
                            {
                                queueManager = new MQQueueManager(connectionString.QueueManagerName, MQC.MQCNO_HANDLE_SHARE_BLOCK,
                                connectionString.ChannelName, connectionString.ConnectionName);
                            }

                            //Set the timestamp for connection object and set IsPresentInPool to false
                            //since it is not added to queue.
                            ConnectionTimeStamp connectionTimeStamp = new ConnectionTimeStamp(DateTime.Now, queueManager, false);
                            timeStampCollection.Add(connectionTimeStamp);

                            // Update number of open connections
                            currentOpenConnections[index] = currentOpenConnections[index] + 1;
                            //return queueManager;
                        }
                        else if (currentOpenConnections[index] == connectionString.MaxSize)
                        {
                            int timeToWait = connectionString.ConnectionTimeOut;
                            //If coonection created reached the maximum size then the current thread 
                            //will wait for sometime and then check again if any connection is returned 
                            //by that time. If connection is returned update the pool size return the connection
                            //otherwise throw an exception.
                            System.Threading.Thread.Sleep(timeToWait);
                            if (currentPoolSize[index] > 0)
                            {
                                queueManager = (MQQueueManager)connectionsQueue[index][connectionsQueue[index].Count - 1];
                                connectionsQueue[index].Remove(queueManager);
                                currentPoolSize[index] = currentPoolSize[index] - 1;

                                // update the timestamp
                                if (timeStampCollection.IndexOf(queueManager) != -1)
                                {
                                    ConnectionTimeStamp connectionTimeStamp = (ConnectionTimeStamp)timeStampCollection[timeStampCollection.IndexOf(queueManager)];
                                    connectionTimeStamp.IsPresentInPool = false;
                                }
                            }
                            else
                            {
                                throw new LegacyException("System is busy serving other request");
                            }
                        }
                    }
                }

                //logEntry.Message = "Number of open connections in ConnectionPool " + index.ToString() + " is " + currentOpenConnections[index].ToString();
                //Logger.Write(logEntry);

                //logEntry.Message = "Number of connections in ConnectionPool " + index.ToString() + " is " + currentPoolSize[index].ToString();
                //Logger.Write(logEntry);

                //logEntry.Message = "GetConnection end";
                //Logger.Write(logEntry);
            }
            return queueManager;
        }
        #endregion

        #region AddConnectionsToPool
        /// <summary>
        /// This is called from GetConnection when number of connection is less than MinSize.
        /// This create MinSize number of connection and adds it to pool.
        /// </summary>
        /// <param name="connectionString">ConnectionString object</param>
        /// <param name="index">Index of the connection pool</param>
        private static void AddConnectionsToPool(MQConnectionInformation connectionString, int index)
        {
            MQQueueManager queueManager = null;
            // Check if the connection pool for this connection string is initialized with 
            // MinSize number of QueueManager object. If not then create so many QueueManager object
            // and add to corresponding connection pool.
            if (currentOpenConnections[index] < connectionString.MinSize)
            {
                lock (syncObject)
                {
                    if (currentOpenConnections[index] < connectionString.MinSize)
                    {
                        //logEntry.Message = "AddConnectionsToPool is called";
                        //Logger.Write(logEntry);

                        // Set minSize which will be useful for checking in ReleaseConnectionsFromPool.
                        minSize[index] = connectionString.MinSize;

                        // Create MinSize no of connection object and add it to queue.
                        for (int minConnections = currentOpenConnections[index]; minConnections < connectionString.MinSize;
                            minConnections++)
                        {
                            if (string.IsNullOrEmpty(connectionString.QueueManagerName))
                            {
                                MQEnvironment.Channel = connectionString.ChannelName;
                                MQEnvironment.Hostname = connectionString.ConnectionName;
                                queueManager = new MQQueueManager();
                                queueManager.OpenOptions = MQC.MQCNO_HANDLE_SHARE_BLOCK;
                            }
                            else if (string.IsNullOrEmpty(connectionString.ChannelName) || string.IsNullOrEmpty(connectionString.ConnectionName))
                            {
                                queueManager = new MQQueueManager(connectionString.QueueManagerName, MQC.MQCNO_HANDLE_SHARE_BLOCK);
                            }
                            else
                            {
                                queueManager = new MQQueueManager(connectionString.QueueManagerName, MQC.MQCNO_HANDLE_SHARE_BLOCK,
                                connectionString.ChannelName, connectionString.ConnectionName);
                            }

                            // Add the connection object to queue.
                            connectionsQueue[index].Add(queueManager);

                            // Set the timestamp for connection object
                            ConnectionTimeStamp connectionTimeStamp = new ConnectionTimeStamp(DateTime.Now, queueManager, true);
                            timeStampCollection.Add(connectionTimeStamp);
                        }

                        // Current no of open connection = old no of open connection + 
                        // no of newly added connection
                        currentOpenConnections[index] = currentOpenConnections[index] +
                            (connectionString.MinSize - currentOpenConnections[index]);
                        currentPoolSize[index] = connectionsQueue[index].Count;

                        //logEntry.Message = "Number of open connections in ConnectionPool " + index.ToString() + " is " + currentOpenConnections[index].ToString();
                        //Logger.Write(logEntry);

                        //logEntry.Message = "Number of connections in ConnectionPool " + index.ToString() + " is " + currentPoolSize[index].ToString();
                        //Logger.Write(logEntry);

                        //logEntry.Message = "AddConnectionsToPool end";
                        //Logger.Write(logEntry);
                    }
                }
            }
        }
        #endregion

        #region ReleaseConnectionsFromPool
        /// <summary>
        /// This disconnects queuemanager which are in pool for a time more than ActiveTimeForConnection 
        /// time. This is called after every elapse of PoolScanFrequency.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ReleaseConnectionsFromPool(object sender, System.Timers.ElapsedEventArgs e)
        {

            lock (syncObject)
            {
                //logEntry.Message = "ReleaseConnectionsFromPool is called";
                //Logger.Write(logEntry);

                poolCleanUpTimer.Enabled = false;

                bool isAnyConnectionRemoved = false;
                // For each connection created so far check time they are in pool.
                for (int index = 0; index < timeStampCollection.Count; index++)
                {
                    ConnectionTimeStamp connectionTimeStamp = (ConnectionTimeStamp)timeStampCollection[index];

                    if (connectionTimeStamp.IsPresentInPool)
                    {
                        // Calculate the time difference time the connection object became inactive to current time
                        // in milliseconds.
                        TimeSpan ts = new TimeSpan();
                        ts = DateTime.Now.Subtract(connectionTimeStamp.CurrentTime);
                        int timeDifference = (ts.Hours * 60 * 60 * 1000) +
                            (ts.Minutes * 60 * 1000) + (ts.Seconds * 1000) + ts.Milliseconds;

                        int itemIndex = -1;
                        // If queue manager timeout is over then disconnect it and reduce the 
                        // Current open connections by one.
                        if (timeDifference >= activeTimeForConnection)
                        {
                            for (int count = 0; count < connectionsQueue.Length; count++)
                            {
                                if (connectionsQueue[count].Contains(connectionTimeStamp.QueueManager))
                                {
                                    itemIndex = connectionsQueue[count].IndexOf(connectionTimeStamp.QueueManager);

                                    if ((itemIndex + 1) > minSize[count])
                                    {
                                        connectionTimeStamp.QueueManager.Disconnect();
                                        currentOpenConnections[count] = currentOpenConnections[count] - 1;
                                        // Remove connection object from pool and update the pool size.
                                        connectionsQueue[count].Remove(connectionTimeStamp.QueueManager);
                                        currentPoolSize[count] = currentPoolSize[count] - 1;
                                        isAnyConnectionRemoved = true;
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
                int numberOfConnectionsRemoved = 0;
                if (isAnyConnectionRemoved)
                {
                    int timeStampColIndex = timeStampCollection.Count - 1;
                    while (timeStampColIndex > -1)
                    {
                        ConnectionTimeStamp connectionTimeStamp = (ConnectionTimeStamp)timeStampCollection[timeStampColIndex];
                        if (connectionTimeStamp.IsPresentInPool && !connectionTimeStamp.QueueManager.IsConnected)
                        {
                            timeStampCollection.Remove(connectionTimeStamp.QueueManager);
                            numberOfConnectionsRemoved++;
                        }
                        timeStampColIndex--;
                    }
                }

                numberOfPoolScans++;

                if (isAnyConnectionRemoved)
                {
                    logEntry.Message = "Number of connections removed in this scan is " + numberOfConnectionsRemoved.ToString();
                    Logger.Write(logEntry);
                }

                // Update the counters
                ShowConnectionPoolCounter();

                // Enable the timer
                poolCleanUpTimer.Enabled = true;

                //logEntry.Message = "Number of pool scan done = " + numberOfPoolScans.ToString();
                //Logger.Write(logEntry);

                //logEntry.Message = "ReleaseConnectionsFromPool end";
                //Logger.Write(logEntry);
            }
        }
        #endregion


        #region numberOfConnections
        /// <summary>
        /// Returns the total number of connection present.
        /// </summary>
        /// <returns></returns>
        public static long FindTotalConnections()
        {
            long numberOfConnections = 0;
            if (connectionsQueue != null)
            {
                // For each pool check the number of connected queuemanager
                for (int index = 0; index < connectionsQueue.Length; index++)
                {
                    if (connectionsQueue[index] != null)
                    {
                        //logEntry.Message = "Start of Find Total COnnections";
                        //Logger.Write(logEntry);
                        MQQueueManager[] tempQueue = new MQQueueManager[connectionsQueue[index].Count];
                        try
                        {
                            connectionsQueue[index].CopyTo(tempQueue, 0);
                        }
                        catch (Exception exc)
                        {
                            logEntry.Message = "An exception occured at FindTotalConnections" + exc.ToString();
                            Logger.Write(logEntry);

                            throw exc;
                        }
                        for (int count = 0; count < tempQueue.Length; count++)
                        {
                            if (tempQueue[count] != null)
                            {
                                if (((MQQueueManager)tempQueue[count]).IsConnected)
                                {
                                    numberOfConnections++;
                                }
                            }
                        }
                        numberOfConnections = numberOfConnections + (long)(currentOpenConnections[index] - currentPoolSize[index]);
                        //logEntry.Message = "End of Find Total COnnections";
                        //Logger.Write(logEntry);
                    }
                }
            }
            return numberOfConnections;
        }
        #endregion

        #region ConnectionsPerPool
        /// <summary>
        /// Return avarage number of connections per pool
        /// </summary>
        /// <returns></returns>
        public static void ShowConnectionPoolCounter()
        {
            lock (syncObject)
            {
                long totalNumberOfConnections = FindTotalConnections();
                long numberOfconnectionsPerPool = 0;
                if (connectionsQueue != null && connectionsQueue.Length != 0)
                {
                    numberOfconnectionsPerPool = totalNumberOfConnections / (long)connectionsQueue.Length;
                }

                SetPerformanceCounter(TOTAL_ACTIVE_CONNECTIONS, totalNumberOfConnections);
                //totalConnection.RawValue = totalNumberOfConnections;
                SetPerformanceCounter(ACTIVE_CONNECTIONS_PER_POOL, numberOfconnectionsPerPool);
                //connectionPerPool.RawValue = numberOfconnectionsPerPool;

                //logEntry.Message = "Total number of connections = " + totalNumberOfConnections.ToString();
                //Logger.Write(logEntry);

                //logEntry.Message = "Average number of connections per pool = " + numberOfconnectionsPerPool.ToString();
                //Logger.Write(logEntry);
            }
        }
        #endregion

        #region ClearConnectionPoolCounters
        private static void ClearConnectionPoolCounters(object sender, EventArgs e)
        {
            DeletePerformanceCounter(TOTAL_ACTIVE_CONNECTIONS);
            //totalConnection.Close();
            DeletePerformanceCounter(ACTIVE_CONNECTIONS_PER_POOL);
            //connectionPerPool.Close();
            //totalConnection.RemoveInstance();
            //connectionPerPool.RemoveInstance();
        }
        #endregion

        #region CloseConnection
        /// <summary>
        /// Releases the connection back to the pool
        /// </summary>
        /// <param name="queueManager">queue manager object which should be added 
        /// to pool</param>
        /// <param name="connectionString">connection string which determines which pool 
        /// queue manager should be added back</param>
        public static void CloseConnection(MQQueueManager queueManager, MQConnectionInformation connectionString)
        {
            //Return the connection to the poolManager and update the poolsize.
            //While returning the connection code should be locked so that no two
            //connection is returned at the same time otherwise there may be a 
            //conflict during updation of the poolsize.
            lock (syncObject)
            {
                //logEntry.Message = "CloseConnection called";
                //Logger.Write(logEntry);

                int index = uniqueConnectionString.IndexOf(connectionString);
                if (index == -1)
                {
                    return;
                }

                // Release the connection to pool
                connectionsQueue[index].Add(queueManager);

                //logEntry.Message = "Connection is released into connectionPool " + index.ToString();
                //Logger.Write(logEntry);

                // Update current pool size
                currentPoolSize[index] = currentPoolSize[index] + 1;

                //logEntry.Message = "Number of open connections in ConnectionPool " + index.ToString() + " is " + currentOpenConnections[index].ToString();
                //Logger.Write(logEntry);

                //logEntry.Message = "Number of connections in ConnectionPool " + index.ToString() + " is " + currentPoolSize[index].ToString();
                //Logger.Write(logEntry);

                // Set IsPresentInPool to true and update the timestamp
                ConnectionTimeStamp connectionTimeStamp = (ConnectionTimeStamp)timeStampCollection[timeStampCollection.IndexOf(queueManager)];
                connectionTimeStamp.CurrentTime = DateTime.Now;
                connectionTimeStamp.IsPresentInPool = true;
                ShowConnectionPoolCounter();

                //logEntry.Message = "CloseConnection end";
                //Logger.Write(logEntry);
            }
        }

        #endregion





        static bool AppDomainUnloadRegistered = false;

        private static System.Diagnostics.PerformanceCounter CreatePerformanceCounter(string counterName)
        {
            System.Diagnostics.PerformanceCounter performanceCounter;
            if (IsPerformanceCountersEnabled)
            {
                performanceCounter =
                    new System.Diagnostics.PerformanceCounter();
                performanceCounter.CategoryName = CATEGORY;
                performanceCounter.CounterName = counterName;
                performanceCounter.ReadOnly = false;
                performanceCounter.InstanceName = COUNTER_INSTANCE;
                if (!AppDomainUnloadRegistered)
                {
                    // Register eventhandler for appdomain unload event. In eventhandler
                    // performance counter instance can removed.
                    AppDomain app = AppDomain.CurrentDomain;
                    app.DomainUnload += new EventHandler(ClearConnectionPoolCounters);
                    AppDomainUnloadRegistered = true;
                }
            }
            else
            {
                performanceCounter = null;
            }
            return performanceCounter;
        }

        private static void IncrementPerformanceCounter(string counterName, long incrementValue)
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
                // If isPerformanceCountersEnabled is YES, return true, else false
                return (isPerformanceCountersEnabled == "ON");
            }
        }

        private static void IncrementPerformanceCounter(string counterName)
        {
            IncrementPerformanceCounter(counterName, 1);
        }

        private static void SetPerformanceCounter(string counterName, long rawValue)
        {
            System.Diagnostics.PerformanceCounter performanceCounter
                = CreatePerformanceCounter(counterName);
            if (performanceCounter != null)
            {
                performanceCounter.RawValue = rawValue;
                performanceCounter.Close();
            }
        }

        private static void DeletePerformanceCounter(string performanceCounterName)
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
    #endregion

    #region ConnectionTimeStamp
    /// <summary>
    /// Objects of this class will have timestamp of the queue manager that was
    /// last accessed.
    /// </summary>
    internal class ConnectionTimeStamp
    {
        DateTime currentTime;
        MQQueueManager queueManager;
        bool isPresentInPool;

        #region CurrentTime
        /// <summary>
        /// Indicates time at this queue manager was last accessed.
        /// </summary>
        internal DateTime CurrentTime
        {
            get
            {
                return currentTime;
            }
            set
            {
                currentTime = value;
            }
        }
        #endregion

        #region QueueManager
        /// <summary>
        /// Queue manager object which will have last accessed time in CurrentTime
        /// </summary>
        internal MQQueueManager QueueManager
        {
            get
            {
                return queueManager;
            }
        }
        #endregion

        #region IsPresentInPool
        /// <summary>
        /// Indicates whether queue manager is present in the MQ Pool.
        /// </summary>
        internal bool IsPresentInPool
        {
            get
            {
                return isPresentInPool;
            }
            set
            {
                isPresentInPool = value;
            }
        }
        #endregion

        #region Methods

        #region ConnectionTimeStamp
        /// <summary>
        /// Two parameter constructor which set instance variables.
        /// </summary>
        /// <param name="currentTime">Time the queue manager was last accessed</param>
        /// <param name="isPresentInPool">Indicates whether queue manager is present in MQ pool or not</param>
        internal ConnectionTimeStamp(DateTime currentTime, MQQueueManager queueManager, bool isPresentInPool)
        {
            this.currentTime = currentTime;
            this.queueManager = queueManager;
            this.isPresentInPool = isPresentInPool;
        }
        #endregion

        #region Equals
        /// <summary>
        /// Returns true if queue manager is equal.
        /// </summary>
        /// <param name="obj">Another object of same type</param>
        /// <returns>Boolean value indicating whether they are equal</returns>
        public override bool Equals(object obj)
        {
            if (this.queueManager == (MQQueueManager)obj)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region GetHashCode
        /// <summary>
        /// Returns unique hash code for the object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (RuntimeHelpers.GetHashCode(this));
        }
        #endregion

        #endregion
    }
    #endregion

    #region MQConnectionInformation
    /// <summary>
    /// Represent connection parameter
    /// </summary>
    internal class MQConnectionInformation
    {
        #region Private members
        int minSize;
        int maxSize;
        int connectionTimeOut;
        string queueManagerName;
        string channelName;
        string connectionName;

        #endregion

        #region Properties

        #region MinSize
        /// <summary>
        /// Indicates minimum number of connection required during initialization.
        /// </summary>
        internal int MinSize
        {
            get
            {
                return minSize;
            }
            set
            {
                minSize = value;
            }
        }
        #endregion

        #region Connection>TimeOut
        /// <summary>
        /// Time it should wait for free connection
        /// </summary>
        internal int ConnectionTimeOut
        {
            get
            {
                return connectionTimeOut;
            }
            set
            {
                connectionTimeOut = value;
            }
        }
        #endregion

        #region MaxSize
        /// <summary>
        /// Indicates maximum number of connection that should be allowed.
        /// </summary>
        internal int MaxSize
        {
            get
            {
                return maxSize;
            }
            set
            {
                maxSize = value;
            }
        }
        #endregion

        #region QueueManagerName
        /// <summary>
        /// Queue manager name it should use for creating connection
        /// </summary>

        internal string QueueManagerName
        {
            get
            {
                return queueManagerName;
            }
            set
            {
                queueManagerName = value;
            }
        }
        #endregion

        #region ChannelName
        /// <summary>
        /// Channel name it should use for creating connection
        /// </summary>
        internal string ChannelName
        {
            get
            {
                return channelName;
            }
            set
            {
                channelName = value;
            }
        }
        #endregion

        #region ConnectionName
        /// <summary>
        /// Connection name it should use for creating connection
        /// </summary>
        internal string ConnectionName
        {
            get
            {
                return connectionName;
            }
            set
            {
                connectionName = value;
            }
        }
        #endregion

        #endregion

        #region methods

        #region MQConnectionInformation
        /// <summary>
        /// constructor made internal so that user cannot create object outside this assembly.
        /// </summary>
        internal MQConnectionInformation()
        {
        }
        #endregion

        #region MQConnectionInformatiom with parameters
        /// <summary>
        /// Initializes the members.
        /// </summary>
        /// <param name="minSize">minimum number of connection</param>
        /// <param name="maxSize">maximum number of connection</param>
        /// <param name="connectionTimeOut">Time it should wait for free connection</param>
        /// <param name="queueManagerName">Queue manager name</param>
        /// <param name="channelName">Channel name</param>
        /// <param name="connection">Connection name</param>
        public MQConnectionInformation(int minSize, int maxSize, int connectionTimeOut,
            string queueManagerName, string channelName, string connectionName)
        {
            this.minSize = minSize;
            this.maxSize = maxSize;
            this.connectionTimeOut = connectionTimeOut;
            this.queueManagerName = queueManagerName;
            this.channelName = channelName;
            this.connectionName = connectionName;
        }
        #endregion

        #region Equals
        /// <summary>
        /// Returns true if all members are equal.
        /// </summary>
        /// <param name="obj">Another object of same type</param>
        /// <returns>Boolean value indicating whether they are equal</returns>
        public override bool Equals(object obj)
        {
            MQConnectionInformation connectionInformation = (MQConnectionInformation)obj;
            if (this.minSize == connectionInformation.minSize &&
            this.maxSize == connectionInformation.maxSize &&
            this.connectionTimeOut == connectionInformation.connectionTimeOut &&
            this.queueManagerName == connectionInformation.queueManagerName &&
            this.channelName == connectionInformation.channelName &&
            this.connectionName == connectionInformation.connectionName)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region GetHashCode
        /// <summary>
        /// Returns unique hash code for the object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (RuntimeHelpers.GetHashCode(this));
        }
        #endregion

        #endregion
    }
    #endregion
}
