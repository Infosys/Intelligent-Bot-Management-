/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.Solutions.Superbot.Infrastructure.Common
{
    public class LogHandler
    {
        public enum BusinessFunctionConstants
        {
            SystemHealthCheck = 1000,
            PlatformHealthCheck = 1001,
            MetricProcessor = 1002,
            SelfHeal = 1003
        }

        public enum Layer
        {
            WebUI=500,
            WebServiceHost = 501,
            Business = 502,
            Resource = 503,
            Infrastructure = 504,
            Job = 505,
            Facade = 506

        }

        public enum LogProvider
        {
            EnterpriseLibrary = 1
        }

        static LogProvider logProvider = ReadProviderFromConfig();
        private static string enableLogsConfig = System.Configuration.ConfigurationManager.AppSettings["EnableAllLogs"];
        private static bool enableLogs = false;
        LogWriter writer = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();
        private static LogProvider ReadProviderFromConfig()
        {
            LogProvider provider = LogProvider.EnterpriseLibrary;


            return provider;

        }

        public static Tracer LogBusinessFunction(string message, BusinessFunctionConstants businessFunction, params object[] messageArguments)
        {

            if (!string.IsNullOrEmpty(enableLogsConfig))
                enableLogs = true;
            if (enableLogs)
            {
                try
                {
                    TraceManager traceMgr = new TraceManager(EnterpriseLibraryContainer.Current.GetInstance<LogWriter>());
                    Tracer tracer = traceMgr.StartTrace("Performance");
                    string strActivityId = "";
                    LogEntry logEntry = new LogEntry();

                    logEntry.EventId = (int)businessFunction;
                    logEntry.Priority = 6;
                    logEntry.Severity = System.Diagnostics.TraceEventType.Information;
                    if (null != messageArguments)
                    {
                        logEntry.Message = string.Format(strActivityId + message, messageArguments);
                    }
                    else
                    {
                        logEntry.Message = strActivityId + message;
                    }
                    if (Microsoft.Practices.EnterpriseLibrary.Logging.Logger.ShouldLog(logEntry))
                    {
                        Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(logEntry);
                    }

                    return tracer;
                }
                catch (Exception) { }
            }
            return null;
        }

        /// <summary>
        /// Log to debug code statements in the application
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="applicationLayer">Is the architecture layer of the application in which the debug statements have to be placed</param>
        /// <param name="arguments">Optional. Arguments to assign dynamic values for the placeholders in the message</param>
        public static void LogDebug(string message, Layer applicationLayer, params object[] messageArguments)
        {
            if (logProvider == LogProvider.EnterpriseLibrary)
            {
                WriteLogUsingEnterpriseLibrary(message, 10, "General", System.Diagnostics.TraceEventType.Verbose, (int)applicationLayer, messageArguments);
            }
        }

        /// <summary>
        /// Log Information statements in the application
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="applicationLayer">Is the architecture layer of the application in which the debug statements have to be placed</param>
        /// <param name="arguments">Optional. Arguments to assign dynamic values for the placeholders in the message</param>
        public static void LogInfo(string message, Layer applicationLayer, params object[] messageArguments)
        {
            if (logProvider == LogProvider.EnterpriseLibrary)
            {
                WriteLogUsingEnterpriseLibrary(message, 10, "General", System.Diagnostics.TraceEventType.Information, (int)applicationLayer, messageArguments);
            }
        }

        /// <summary>
        /// Log warnings in the application
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="applicationLayer">Is the architecture layer of the application in which the debug statements have to be placed</param>
        /// <param name="arguments">Optional. Arguments to assign dynamic values for the placeholders in the message</param>
        public static void LogWarning(string message, Layer applicationLayer, params object[] messageArguments)
        {
            if (logProvider == LogProvider.EnterpriseLibrary)
            {
                WriteLogUsingEnterpriseLibrary(message, 3, "General", System.Diagnostics.TraceEventType.Warning, (int)applicationLayer, messageArguments);
            }
        }

        /// <summary>
        /// Log errors in the application
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="applicationLayer">Is the architecture layer of the application in which the debug statements have to be placed</param>
        /// <param name="arguments">Optional. Arguments to assign dynamic values for the placeholders in the message</param>
        public static void LogError(string message, Layer applicationLayer, params object[] messageArguments)
        {
            if (logProvider == LogProvider.EnterpriseLibrary)
            {
                WriteLogUsingEnterpriseLibrary(message, 2, "General", System.Diagnostics.TraceEventType.Error, (int)applicationLayer, messageArguments);
            }
        }

        private static void WriteLogUsingEnterpriseLibrary(string message, int priority, string category, TraceEventType traceEvent, int applicationLayer, object[] messageArguments)
        {
            if (message != null)
            {
                if (!string.IsNullOrEmpty(enableLogsConfig))
                    enableLogs = true;
                if (enableLogs)
                {
                    try
                    {
                        LogEntry logEntry = new LogEntry();
                        logEntry.EventId = (int)applicationLayer;
                        logEntry.Priority = priority;
                        logEntry.Severity = traceEvent;
                        if (null != messageArguments)
                        {
                            logEntry.Message = string.Format(message, messageArguments);
                        }
                        else
                        {
                            logEntry.Message = message;
                        }
                        logEntry.Categories.Add(category);

                        if (Microsoft.Practices.EnterpriseLibrary.Logging.Logger.ShouldLog(logEntry))
                        {
                            Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(logEntry);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        public static Tracer TraceOperations(string message, Layer applicationLayer, Guid activityId, params object[] messageArguments)
        {
            if (!string.IsNullOrEmpty(enableLogsConfig))
                enableLogs = true;
            if (enableLogs)
            {
                try
                {
                    TraceManager traceMgr = new TraceManager(EnterpriseLibraryContainer.Current.GetInstance<LogWriter>());
                    Tracer tracer;
                    if (activityId != null || activityId != System.Guid.Empty)
                    {
                        tracer = traceMgr.StartTrace("Performance", activityId);
                    }
                    else
                    {
                        tracer = traceMgr.StartTrace("Performance");
                    }

                    LogEntry logEntry = new LogEntry();
                    logEntry.EventId = (int)applicationLayer;
                    logEntry.Priority = 11;
                    logEntry.Severity = System.Diagnostics.TraceEventType.Verbose;
                    if (null != messageArguments)
                    {
                        logEntry.Message = string.Format(message, messageArguments);
                    }
                    else
                    {
                        logEntry.Message = message;
                    }
                    logEntry.Categories.Add("General");
                    if (Microsoft.Practices.EnterpriseLibrary.Logging.Logger.ShouldLog(logEntry))
                    {
                        Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(logEntry);
                    }
                    return tracer;

                }
                catch (Exception ex)
                {
                }
            }
            return null;
        }

        public static void ArchiveMessages(string message, string location)
        {
            try
            {
                LogEntry logEntry = new LogEntry();
                //6000 code for message archival
                logEntry.EventId = 6000;
                logEntry.Priority = 20;
                logEntry.Severity = System.Diagnostics.TraceEventType.Information;
                logEntry.Message = message + "$" + location;
                logEntry.Categories.Add("MessageArchive");
                if (Logger.ShouldLog(logEntry))
                {
                    Logger.Write(logEntry);
                }
            }
            catch (Exception) { }
        }
    }
}
