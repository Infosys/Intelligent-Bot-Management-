using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Security;
namespace LegacyCounter
{
    public class Program
    {
        [STAThread]
        static void Main()
        {
            LegacyCounter.CreateCounter();
        }
    }

    internal class LegacyCounter
    {
        // Constants for IBM MQ adapter
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


        // Constants for HIS adapter 
        const string TOTAL_REQUESTS_IN_HIS = "HIS - TotalNumberofRequest";
        const string TOTAL_RESPONSES_IN_HIS = "HIS - TotalNumberofResponse";
        const string AVERAGE_REQUEST_IN_HIS = "HIS - AverageNumberOfRequest";
        const string AVERAGE_RESPONSE_IN_HIS = "HIS - AverageNumberOfResponse";
        const string AVERAGE_TIME_IN_HIS = "HIS - AverageTime";
        const string AVERAGE_BASE_IN_HIS = "HIS - AverageBase";
        const string REQUEST_SIZE_IN_HIS = "HIS - RequestLength";
        const string RESPONSE_SIZE_IN_HIS = "HIS - ResponseLength";
        const string NUMBER_OF_EXCEPTIONS_IN_HIS = "HIS - NumberOfException";
        const string ACTIVE_CONNECTIONS_HIS = "HIS - ActiveConnections";
        const string TIME_FOR_CONNECTION_HIS = "HIS - Time for getting a connection";


        /// <summary>
        /// Creates counters for the LegacyIntegrator
        /// </summary>
        internal static void CreateCounter()
        {
            #region LegacyIntegrator Category Creation
            {
                CounterCreationDataCollection counterCollection =
                    new CounterCreationDataCollection();
                counterCollection.Add(CreateCounterCreationData("Time Taken for Initialization",
                    "Indicates average time taken for the MQAdapter to get an MQ connection.",
                    PerformanceCounterType.AverageTimer32));

                counterCollection.Add(CreateCounterCreationData("Time Taken for Initialization Base",
    "Indicates average time taken for the MQAdapter to get an MQ connection.",
    PerformanceCounterType.AverageBase));
                counterCollection.Add(CreateCounterCreationData("Time Taken for SendSync",
                    "Indicates average time taken for the MQAdapter to get an MQ connection.",
                    PerformanceCounterType.AverageTimer32));
                counterCollection.Add(CreateCounterCreationData("Time Taken for SendSync Base",
                    "Indicates average time taken for the MQAdapter to get an MQ connection.",
                    PerformanceCounterType.AverageBase));



                // IBM MQ adapter performance counter
                // Counter for time taken to get a connection 
                counterCollection.Add(CreateCounterCreationData(TIME_TO_GET_A_CONNECTION,
                    "Indicates average time taken for the MQAdapter to get an MQ connection.",
                    PerformanceCounterType.AverageTimer32));

                // IBM MQ adapter performance counter
                // Base counter for time taken to get a connection 
                counterCollection.Add(CreateCounterCreationData(TIME_TO_GET_A_CONNECTION + " Base",
                    "Is the base counter for the average time taken for the MQAdapter to get an MQ connection.",
                    PerformanceCounterType.AverageBase));

                // IBM MQ adapter performance counter
                // Counter for total number of request
                counterCollection.Add(CreateCounterCreationData(TOTAL_REQUESTS,
                    "Indicates total number of requests to IBM MQ Adapter.",
                    PerformanceCounterType.NumberOfItems64));

                //// Counter for total number of response
                counterCollection.Add(CreateCounterCreationData(TOTAL_RESPONSES,
                    "Indicates total number of responses from IBM MQ Adapter.",
                    PerformanceCounterType.NumberOfItems64));

                // Counter for average request
                counterCollection.Add(CreateCounterCreationData(AVERAGE_REQUEST,
                    "Indicates average number of requests to IBM MQ Adapter.",
                    PerformanceCounterType.RateOfCountsPerSecond32));

                // Counter for average response
                counterCollection.Add(CreateCounterCreationData(AVERAGE_RESPONSE,
                    "Indicates average number of responses from IBM MQ Adapter.",
                    PerformanceCounterType.RateOfCountsPerSecond32));

                // Counter for average time
                counterCollection.Add(CreateCounterCreationData(AVERAGE_TIME,
                    "An average time it takes to complete a request in IBM MQ Adapter." +
                    " Its displays a ratio of the total elapsed time of the sample interval to the number of requests " +
                    " completed during that time. This measures time in ticks of the system clock.",
                    PerformanceCounterType.AverageTimer32));

                // Create AverageBase counter (required when you need AverageTimer counter
                counterCollection.Add(CreateCounterCreationData(AVERAGE_BASE,
                    "Indicates number of operations which is used in calculating average time in IBM MQ Adapter.",
                    PerformanceCounterType.AverageBase));

                // Counter for request size
                counterCollection.Add(CreateCounterCreationData(REQUEST_SIZE,
                    "Indicates length of the request made to IBM MQ Adapter. It shows number of characters in a request string.",
                    PerformanceCounterType.NumberOfItems64));

                //// Counter for response size
                counterCollection.Add(CreateCounterCreationData(RESPONSE_SIZE,
                    "Indicates length of the response from IBM MQ Adapter. It shows number of characters in a response string.",
                    PerformanceCounterType.NumberOfItems64));

                // Counter for total connections
                counterCollection.Add(CreateCounterCreationData(TOTAL_ACTIVE_CONNECTIONS,
                    "Indicates total number of active connections in custom connection pool of IBM MQ Adapter.",
                    PerformanceCounterType.NumberOfItems32));

                // Counter for connections per pool
                counterCollection.Add(CreateCounterCreationData(ACTIVE_CONNECTIONS_PER_POOL,
                    "Indicates active number of connections per pool in IBM MQ Adapter.",
                    PerformanceCounterType.NumberOfItems32));

                // Counter for number of exceptions
                counterCollection.Add(CreateCounterCreationData(NUMBER_OF_EXCEPTIONS,
                    "Indicates number of exceptions occurred in IBM MQ Adapter.",
                    PerformanceCounterType.NumberOfItems32));

                // HIS adapter performance counter

                // Counter for total number of response
                counterCollection.Add(CreateCounterCreationData(TOTAL_REQUESTS_IN_HIS,
                    "Indicates total number of requests to HIS Adapter.",
                    PerformanceCounterType.NumberOfItems64));

                // Counter for total number of response
                counterCollection.Add(CreateCounterCreationData(TOTAL_RESPONSES_IN_HIS,
                    "Indicates total number of response from HIS Adapter.",
                    PerformanceCounterType.NumberOfItems64));

                // Counter for average request
                counterCollection.Add(CreateCounterCreationData(AVERAGE_REQUEST_IN_HIS,
                    "Indicates average number of requests to HIS Adapter.",
                    PerformanceCounterType.RateOfCountsPerSecond32));

                // Counter for average response
                counterCollection.Add(CreateCounterCreationData(AVERAGE_RESPONSE_IN_HIS,
                    "Indicates average number of responses from HIS Adapter.",
                    PerformanceCounterType.RateOfCountsPerSecond32));

                // Counter for average time
                counterCollection.Add(CreateCounterCreationData(AVERAGE_TIME_IN_HIS,
                    "An average time it takes to complete a request in HIS Adapter." +
                    " Its displays a ratio of the total elapsed time of the sample interval " +
                    "to the number of requests " +
                    " completed during that time. This measures time in ticks of the system clock.",
                    PerformanceCounterType.AverageTimer32));

                // Create AverageBase counter (required when you need AverageTimer counter
                counterCollection.Add(CreateCounterCreationData(AVERAGE_BASE_IN_HIS,
                    "Indicates number of operations which is used in calculating average time in HIS Adapter.",
                    PerformanceCounterType.AverageBase));

                // Counter for request size
                counterCollection.Add(CreateCounterCreationData(REQUEST_SIZE_IN_HIS,
                    "Indicates length of the request made to HIS Adapter. It shows number of characters in a request string.",
                    PerformanceCounterType.NumberOfItems64));

                // Counter for response size
                counterCollection.Add(CreateCounterCreationData(RESPONSE_SIZE_IN_HIS,
                    "Indicates length of the response from HIS Adapter. It shows number of characters in a response string.",
                    PerformanceCounterType.NumberOfItems64));

                // Counter for number of exceptions
                counterCollection.Add(CreateCounterCreationData(NUMBER_OF_EXCEPTIONS_IN_HIS,
                    "Indicates number of exceptions occurred in HIS Adapter.",
                    PerformanceCounterType.NumberOfItems32));

                // Counter for number of active connections in HIS connection pool
                counterCollection.Add(CreateCounterCreationData(ACTIVE_CONNECTIONS_HIS,
                    "Indicates the number of active (open) connections existing between the LIF server and the HIS server."
                    + " This also indicates the number of ports opened between the HIS Server and the mainframe",
                    PerformanceCounterType.NumberOfItems32));

                // Counter for average time taken to get a connection to the mainframe
                counterCollection.Add(CreateCounterCreationData(TIME_FOR_CONNECTION_HIS,
                    "Time taken in seconds to get a connection object from the connection pool manager.",
                    PerformanceCounterType.AverageTimer32));

                // Counter for average time taken to get a connection to the mainframe base
                counterCollection.Add(CreateCounterCreationData(TIME_FOR_CONNECTION_HIS + " base",
                    "This is the base counter for time to get a connecion in HIS",
                    PerformanceCounterType.AverageBase));

                string categoryName = "LegacyIntegrator";
                if (PerformanceCounterCategory.Exists(categoryName))
                {
                    PerformanceCounterCategory.Delete(categoryName);
                }

                PerformanceCounterCategory.Create(categoryName,
                    "Performance counter for legacy integrator",
                    PerformanceCounterCategoryType.MultiInstance,
                    counterCollection);
            }
            #endregion

            #region LegacyFacade Category Creation
            {
                CounterCreationDataCollection collectionOfCounters
                    = new CounterCreationDataCollection();


                /// Creation of the RequestHandler execution time counter.
                collectionOfCounters.Add(
                    CreateCounterCreationData("AverageTime (RequestHandler)",
                    "Average Time taken by Request Handler to execute",
                    PerformanceCounterType.AverageTimer32));

                /// Creation of the RequestHandler execution time base counter.
                collectionOfCounters.Add(
                    CreateCounterCreationData("AverageTime (RequestHandler) Base",
                    "Average Time taken by Request Handler to execute",
                    PerformanceCounterType.AverageBase));

                /// Creation of the AverageTime (CreateParser) counter
                collectionOfCounters.Add(
                    CreateCounterCreationData("AverageTime (CreateParser)",
                    "Average Time taken by CreateParser method to execute",
                    PerformanceCounterType.AverageTimer32));

                /// Creation of the AverageTime (CreateParser) Base counter
                collectionOfCounters.Add(
                    CreateCounterCreationData("AverageTime (CreateParser) Base",
                    "Average Time taken by CreateParser method to execute",
                    PerformanceCounterType.AverageBase));

                /// 

                /// Creation of the AverageTime (CreateParser) counter
                collectionOfCounters.Add(
                    CreateCounterCreationData("AverageTime (ExtractParser)",
                    "Average Time taken by ExtractParser method to execute",
                    PerformanceCounterType.AverageTimer32));

                /// Creation of the AverageTime (CreateParser) Base counter
                collectionOfCounters.Add(
                    CreateCounterCreationData("AverageTime (ExtractParser) Base",
                    "Average Time taken by ExtractParser method to execute",
                    PerformanceCounterType.AverageBase));




                /// Creation of the AverageTime (AdapterManager) counter
                collectionOfCounters.Add(
                    CreateCounterCreationData("AverageTime (AdapterManager)",
                    "Average Time taken by AdapterManager to execute",
                    PerformanceCounterType.AverageTimer32));

                /// Creation of the AverageTime (AdapterManager) Base counter
                collectionOfCounters.Add(
                    CreateCounterCreationData("AverageTime (AdapterManager) Base",
                    "Average Time taken by AdapterManager to execute",
                    PerformanceCounterType.AverageBase));

                /// Creation of the AverageTime (Wrap) counter
                collectionOfCounters.Add(
                    CreateCounterCreationData("AverageTime (Wrap)",
                    "Average Time taken by Wrap method to execute",
                    PerformanceCounterType.AverageTimer32));

                /// Creation of the AverageTime (Wrap) Base counter
                collectionOfCounters.Add(
                    CreateCounterCreationData("AverageTime (Wrap) Base",
                    "Average Time taken by Wrap method to execute",
                    PerformanceCounterType.AverageBase));

                /// Creation of the AverageTime (UnWrap) counter
                collectionOfCounters.Add(
                    CreateCounterCreationData("AverageTime (UnWrap)",
                    "Average Time taken by UnWrap method to execute",
                    PerformanceCounterType.AverageTimer32));

                /// Creation of the AverageTime (Wrap) Base counter
                collectionOfCounters.Add(
                    CreateCounterCreationData("AverageTime (UnWrap) Base",
                    "Average Time taken by UnWrap method to execute",
                    PerformanceCounterType.AverageBase));



                string counterCategory = "LegacyFacade";
                if (PerformanceCounterCategory.Exists(counterCategory))
                {
                    PerformanceCounterCategory.Delete(counterCategory);
                }
                PerformanceCounterCategory.Create(
                        counterCategory,
                        "Counters required to monitor the Legacy Facade",
                        PerformanceCounterCategoryType.MultiInstance,
                        collectionOfCounters
                        );

            }
            #endregion

        }


        /// <summary>
        /// This method helps in the creation of a counter
        /// It returns a CounterCreationData object which can be added to the colleciton of the counters 
        /// to be created in a category
        /// </summary>
        /// <param name="counterName">Name of the counter to be created</param>
        /// <param name="counterHelp">Help which should be shown when this counter's help is requested</param>
        /// <param name="counterType">Type of counter (AverageBase, AverageTimer32, etc.)</param>
        /// <returns>
        /// Counter creation data which can be added to the collection of the counters to be 
        /// created while creating a category.
        /// </returns>
        private static CounterCreationData CreateCounterCreationData(string counterName,
            string counterHelp, PerformanceCounterType counterType)
        {
            CounterCreationData creationData = new CounterCreationData();
            creationData.CounterName = counterName;
            creationData.CounterHelp = counterHelp;
            creationData.CounterType = counterType;
            return creationData;
        }
    }
}

