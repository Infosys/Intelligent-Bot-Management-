/****************************************************************
 * This file is a part of the Legacy Integration Framework.
 * This file implements IAdapter interface to use HIS as communication medium.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/
using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Specialized;
//using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging;
//using Microsoft.Practices.EnterpriseLibrary.Logging.Tracing;
using Infosys.Lif.LegacyCommon;
using Infosys.Lif.LegacyIntegratorService;
using LearningPrograms;

namespace Infosys.Lif.HIS
{
    #region HISAdapter
    public class HISAdapter : IAdapter
    {
        #region private fields

        ///Commented by Sachin - Have moved the Performance COunter creation to a single location
        ////// Create performance counter objects.
        ////PerformanceCounter totalRequest = new PerformanceCounter();
        ////PerformanceCounter totalResponse = new PerformanceCounter();
        ////PerformanceCounter averageRequest = new PerformanceCounter();
        ////PerformanceCounter averageResponse = new PerformanceCounter();
        ////PerformanceCounter requestSize = new PerformanceCounter();
        ////PerformanceCounter responseSize = new PerformanceCounter();
        ////PerformanceCounter averageTime = new PerformanceCounter();
        ////PerformanceCounter averageBase = new PerformanceCounter();
        ////PerformanceCounter numberOfExceptions = new PerformanceCounter();
        #endregion

        #region Constants
        // Region 
        private const string REGION = "Region";
        private const string TRANSPORT_SECTION = "TransportSection";
        // Trace category
        const string TRACE_CATEGORY = "LegacyIntegrator";
        #endregion

        #region Methods
        #region Send
        public string Send(ListDictionary adapterDetails, string message)
        {

            // Commented by Sachin Nayak
            // This functionality has been moved to the IncrementPerformance Counter
            // method

            // Register eventhandler for appdomain unload event. In eventhandler
            // performance counter instance can removed.

            //////Create performance counter for total number of requests
            ////totalRequest = CreatePerformanceCounter(TOTAL_REQUESTS_IN_HIS);


            //////Create performance counter for total number of responses
            ////totalResponse = CreatePerformanceCounter(TOTAL_RESPONSES_IN_HIS);

            //////Create performance counter for average requests
            ////averageRequest = CreatePerformanceCounter(AVERAGE_REQUEST_IN_HIS);

            //////Create performance counter for average responses
            ////averageResponse= CreatePerformanceCounter(AVERAGE_RESPONSE_IN_HIS);

            //////Create performance counter for request size
            ////requestSize = CreatePerformanceCounter(REQUEST_SIZE_IN_HIS);

            //////Create performance counter for response size
            ////responseSize = CreatePerformanceCounter(RESPONSE_SIZE_IN_HIS);

            //////Create performance counter for average time to process
            ////averageTime = CreatePerformanceCounter(AVERAGE_TIME_IN_HIS);

            //////Create performance counter to find average base which will used 
            ////// to find average time.
            ////averageBase = CreatePerformanceCounter(AVERAGE_BASE_IN_HIS);

            //////Create performance counter for number of exceptions
            ////numberOfExceptions= CreatePerformanceCounter(NUMBER_OF_EXCEPTIONS_IN_HIS);

            int returnCode = 0;
            int errorNumber = 0;
            string ioMessage = message;
            long interval = 0;

            try
            {
                IncrementPerformanceCounter(TOTAL_REQUESTS_IN_HIS);
                IncrementPerformanceCounter(AVERAGE_REQUEST_IN_HIS);


                long lengthOfRequest = (long)message.Length;
                SetPerformanceCounter(REQUEST_SIZE_IN_HIS, lengthOfRequest);

                LearnerProgs leg = new LearnerProgs();
                long start = DateTime.Now.Ticks;
                long end;

                DateTime beginTime = DateTime.Now;

                leg.LEARNPRG(ref returnCode, ref errorNumber, ref ioMessage);

                TimeSpan timeSpan = DateTime.Now - beginTime;
                interval = timeSpan.Hours * 12 + timeSpan.Minutes * 60 + timeSpan.Seconds * 60 + timeSpan.Milliseconds;

                end = DateTime.Now.Ticks;

                IncrementPerformanceCounter(AVERAGE_TIME_IN_HIS, end - start);
                IncrementPerformanceCounter(AVERAGE_BASE_IN_HIS);



                if (returnCode != 0)
                {
                    throw new LegacyException("Return code :" + returnCode.ToString() + "\r\n Error Code: " + errorNumber.ToString());
                }

                IncrementPerformanceCounter(TOTAL_RESPONSES_IN_HIS);
                IncrementPerformanceCounter(AVERAGE_RESPONSE_IN_HIS);


                long lengthOfResponse = (long)ioMessage.Length;
                SetPerformanceCounter(RESPONSE_SIZE_IN_HIS, lengthOfResponse);

                return ioMessage;

            }
            catch (LegacyException exception)
            {
                IncrementPerformanceCounter(NUMBER_OF_EXCEPTIONS_IN_HIS);
                throw exception;
            }
            catch (Exception exception)
            {
                IncrementPerformanceCounter(NUMBER_OF_EXCEPTIONS_IN_HIS);
                throw exception;
            }
            finally
            {
                Infosys.Lif.LegacyIntegratorService.HIS transportSection = adapterDetails[TRANSPORT_SECTION] as Infosys.Lif.LegacyIntegratorService.HIS;
                if (transportSection.EnableTrace != "ON" && transportSection.EnableTrace != "OFF")
                {
                    throw new LegacyException("Invalid EnableTrace value. Use ON or OFF");
                }
                isPerformanceCountersEnabled =transportSection.EnablePerformanceCounters;

                if (transportSection.EnableTrace == "ON")
                {
                    using (new Tracer(TRACE_CATEGORY))
                    {
                        Region region = adapterDetails[REGION] as Region;
                        StringBuilder traceMessage = new StringBuilder();
                        traceMessage.Append(Environment.NewLine);
                        traceMessage.Append("Region:\t" + region.Name);
                        traceMessage.Append(Environment.NewLine);
                        traceMessage.Append("RequestString:\t" + message);
                        traceMessage.Append(Environment.NewLine);
                        traceMessage.Append("RequestLength:\t" + message.Length.ToString());
                        traceMessage.Append(Environment.NewLine);
                        if (message == ioMessage)
                        {
                            traceMessage.Append("**************Exception occured with host connectivity");
                        }
                        else
                        {
                            traceMessage.Append("ResponseString:\t" + ioMessage);
                            traceMessage.Append(Environment.NewLine);
                            traceMessage.Append("ResponseLength:\t" + ioMessage.Length.ToString());
                            traceMessage.Append(Environment.NewLine);
                            traceMessage.Append("Interval:\t" + interval.ToString() + "ms");
                            traceMessage.Append(Environment.NewLine);
                        }

                        Logger.Write(traceMessage.ToString(), TRACE_CATEGORY);
                    }
                }


            }
        }

        public void Receive(ListDictionary adapterDetails)
        {
            throw new NotImplementedException();
        }

        public bool Delete(System.Collections.Specialized.ListDictionary messageDetails)
        {
            throw new NotImplementedException();
        }

        public event ReceiveHandler Received;
        #endregion

        #region ClearHISCounter
        protected void ClearHISCounter(object sender, EventArgs e)
        {
            //totalRequest.RawValue = 0;
            //totalResponse.RawValue = 0;
            //averageRequest.RawValue = 0;
            //averageResponse.RawValue = 0;
            //requestSize.RawValue = 0;
            //responseSize.RawValue = 0;
            //averageBase.RawValue = 0;
            //numberOfExceptions.RawValue = 0;
            DeletePerformanceCounter(TOTAL_REQUESTS_IN_HIS);

            DeletePerformanceCounter(TOTAL_REQUESTS_IN_HIS);

            DeletePerformanceCounter(TOTAL_RESPONSES_IN_HIS);

            DeletePerformanceCounter(AVERAGE_REQUEST_IN_HIS);

            DeletePerformanceCounter(AVERAGE_RESPONSE_IN_HIS);

            DeletePerformanceCounter(REQUEST_SIZE_IN_HIS);

            DeletePerformanceCounter(RESPONSE_SIZE_IN_HIS);

            DeletePerformanceCounter(AVERAGE_TIME_IN_HIS);

            DeletePerformanceCounter(AVERAGE_BASE_IN_HIS);

            DeletePerformanceCounter(NUMBER_OF_EXCEPTIONS_IN_HIS);

        }
        #endregion

        #endregion



        //Start  - Added by Sachin -- Performance counter needs to be created and
        // used only if the configuration file enables performance Counters

        // Performance related counters
        const string CATEGORY = "LegacyIntegrator";
        const string TOTAL_REQUESTS_IN_HIS = "HIS - TotalNumberofRequest";
        const string TOTAL_RESPONSES_IN_HIS = "HIS - TotalNumberofResponse";
        const string AVERAGE_REQUEST_IN_HIS = "HIS - AverageNumberOfRequest";
        const string AVERAGE_RESPONSE_IN_HIS = "HIS - AverageNumberOfResponse";
        const string AVERAGE_TIME_IN_HIS = "HIS - AverageTime";
        const string AVERAGE_BASE_IN_HIS = "HIS - AverageBase";
        const string REQUEST_SIZE_IN_HIS = "HIS - RequestLength";
        const string RESPONSE_SIZE_IN_HIS = "HIS - ResponseLength";
        const string NUMBER_OF_EXCEPTIONS_IN_HIS = "HIS - NumberOfException";
        // Counter
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
                AppDomain app = AppDomain.CurrentDomain;
                app.DomainUnload += new EventHandler(ClearHISCounter);
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
                if (isPerformanceCountersEnabled.Length == 0)
                {

                    // EnablePerformanceCounters can only be ON OFF or 
                    // the node may not have been mentioned at all.
                    if (isPerformanceCountersEnabled != "ON"
                        && isPerformanceCountersEnabled != "OFF"
                        )
                    {
                        throw new LegacyException("Invalid EnablePerformanceCounters value. Use ON or OFF");
                    }
                }

                // If isPerformanceCountersEnabled is YES, return true, else false
                // If the performance countersEnabled is not mentioend in config file
                // it will treat it as NO
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

        private void DeletePerformanceCounter(string performanceCounterName)
        {
            System.Diagnostics.PerformanceCounter performanceCounter
                = CreatePerformanceCounter(performanceCounterName);
            if (performanceCounter != null)
            {
                performanceCounter.RemoveInstance();
            }
        }
        //End    - Added by Sachin -- Performance counter needs to be created and
        // used only if the configuration file enables performance Counters

    }
    #endregion
}
