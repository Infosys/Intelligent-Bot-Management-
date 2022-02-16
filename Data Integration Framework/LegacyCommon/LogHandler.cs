/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;


namespace Infosys.Lif.LegacyCommon
{

    public class LifLogHandler
    {      

        public enum Layer
        {
            IntegrationLayer = 510
        }

        LogWriter writer = EnterpriseLibraryContainer.Current.GetInstance<LogWriter>();

        /// <summary>
        /// Log debug statements in the application
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="applicationLayer">Is the architecture layer of the application in which the debug statements have to be placed</param>
        /// <param name="messageArguments">Optional. Arguments to assign dynamic values for the placeholders in the message</param>

        public static void LogDebug(string message, Layer applicationLayer, params object[] messageArguments)
        {
            try
            {
                LogEntry logEntry = new LogEntry();
                logEntry.EventId = (int) applicationLayer;
                logEntry.Priority = 10;
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

                if (Logger.ShouldLog(logEntry))
                {
                    Logger.Write(logEntry);
                }
            }
            catch (Exception) { }
        }

        
        /// <summary>
        /// Log errors in the application
        /// </summary>
        /// <param name="message">Message to be logged</param>
        /// <param name="applicationLayer">Is the architecture layer of the application in which the debug statements have to be placed</param>
        /// <param name="messageArguments">Optional. Arguments to assign dynamic values for the placeholders in the message</param>

        public static void LogError(string message, Layer applicationLayer, params object[] messageArguments)
        {
            try
            {
                LogEntry logEntry = new LogEntry();
                logEntry.EventId = (int) applicationLayer;
                logEntry.Priority = 2;
                logEntry.Severity = System.Diagnostics.TraceEventType.Error;
                if (null != messageArguments)
                {
                    logEntry.Message = string.Format(message, messageArguments);
                }
                else
                {
                    logEntry.Message = message;
                }
                logEntry.Categories.Add("General");

                if (Logger.ShouldLog(logEntry))
                {
                    Logger.Write(logEntry);
                }
            }
            catch (Exception) { }
        }

    //    public static Tracer TraceOperations(string message, Layer applicationLayer, Guid activityId, params object[] messageArguments)
    //    {
    //        try
    //            {
    //                TraceManager traceMgr = new TraceManager(EnterpriseLibraryContainer.Current.GetInstance<LogWriter>());
    //                Tracer tracer;
    //                if (activityId != null || activityId != System.Guid.Empty)
    //                {
    //                    tracer = traceMgr.StartTrace("Performance", activityId);
    //                }
    //                else
    //                {
    //                    tracer = traceMgr.StartTrace("Performance");
    //                }

    //                LogEntry logEntry = new LogEntry();
    //                logEntry.EventId = (int)applicationLayer;
    //                logEntry.Priority = 11;
    //                logEntry.Severity = System.Diagnostics.TraceEventType.Verbose;
    //                if (null != messageArguments)
    //                {
    //                    logEntry.Message = string.Format(message, messageArguments);
    //                }
    //                else
    //                {
    //                    logEntry.Message = message;
    //                }
    //                logEntry.Categories.Add("General");
    //                if (Microsoft.Practices.EnterpriseLibrary.Logging.Logger.ShouldLog(logEntry))
    //                {
    //                    Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(logEntry);
    //                }
    //                return tracer;

    //            }
    //            catch (Exception ex)
    //            {
    //            }            
    //        return null;
    //    }
   }
}
