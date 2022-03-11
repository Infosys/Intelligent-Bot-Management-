/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.Solutions.Superbot.Infrastructure.Common.PerfMon
{
    public class PerfMonLogger
    {
        static readonly PerformanceCounter counter = new PerformanceCounter();
        static readonly string enablePerfMonLogsConfig = System.Configuration.ConfigurationManager.AppSettings["EnablePerformanceLog"];
        static readonly bool enablePerfMonLogs = false;
        static PerfMonLogger()
        {
            if (!string.IsNullOrEmpty(enablePerfMonLogsConfig))
                if (Boolean.TryParse(enablePerfMonLogsConfig, out enablePerfMonLogs) && enablePerfMonLogs)
                {
                    enablePerfMonLogs = true;
                    InitializeSuperBotCounters();
                }
                    

        }
        
        public static void LogRawValue(string categoryName, string counterName, long rawValue)
        {
            if (enablePerfMonLogs)
                if (PerformanceCounterCategory.Exists(categoryName) && PerformanceCounterCategory.CounterExists(counterName, categoryName))
                {
                    counter.CategoryName = categoryName;
                    counter.CounterName = counterName;
                    counter.ReadOnly = false;
                    counter.InstanceName = "_total";
                    counter.RawValue = rawValue;
                }
        }
        public static void LogIncrementValue(string categoryName, string counterName)
        {
            if(enablePerfMonLogs)
                if (PerformanceCounterCategory.Exists(categoryName) && PerformanceCounterCategory.CounterExists(counterName, categoryName))
                {
                    counter.CategoryName = categoryName;
                    counter.CounterName = counterName;
                    counter.ReadOnly = false;
                    counter.InstanceName = "_total";
                    counter.Increment();
                }            
        }
        
        public static bool InitializeSuperBotCounters()
        {
            if (enablePerfMonLogs)
            {
                try
                {
                    SuperBotCounters counters = new SuperBotCounters();
                    foreach (var field in typeof(SuperBotCounters).GetFields())
                    {
                        counter.CategoryName = SuperBotCategories.SuperBot;
                        counter.CounterName = field.GetValue(counters).ToString();
                        counter.ReadOnly = false;
                        counter.InstanceName = "_total";
                        counter.RawValue = 0;
                        //field.Name -- for name of property
                        //field.GetValue(fGPerfMonCounters) -- for value of property
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        public struct SuperBotCategories
        {
            public const string SuperBot = @"SuperBot";
            
        }
        public struct SuperBotCounters
        {
            public const string NumOfMsgProcessed = @"# of messages processed";
            public const string NumOfMsgProcessedPerSec = @"# of messages processed/Sec";
            public const string NumOfAnomalyDetected = @"# Of Anomaly detected";
            public const string NumOfErrors = @"# Of Error detected";
            public const string MsgProcessTime = @"Message processing time";
        }
    }
}
