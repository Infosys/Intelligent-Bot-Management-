/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infosys.Solutions.Ainauto.Infrastructure.ProcessScheduler.Framework;
using QueueEntity = Infosys.Solutions.Superbot.Resource.Entity.Queue;
using Infosys.Solutions.Superbot.Infrastructure.Common;
using Infosys.Solutions.Ainauto.Superbot.BusinessComponent;
using BE = Infosys.Solutions.Ainauto.BusinessEntity;
using DE = Infosys.Solutions.Superbot.Resource.Entity;
using Infosys.Solutions.Ainauto.Resource.DataAccess;
using DE1 = Infosys.Solutions.Superbot.Resource.Entity.Queue;
using Infosys.Solutions.Ainauto.Resource.DataAccess.Queue;
using System.Reflection;
using BC=Infosys.Solutions.Ainauto.Superbot.BusinessComponent;
using Newtonsoft.Json;
using Infosys.Solutions.Superbot.Infrastructure.Common.PerfMon;
using System.Diagnostics;

namespace Infosys.Solutions.Ainauto.Processes
{
    public class MetricProcessor: ProcessHandlerBase<QueueEntity.MetricMessage>
    {
        private Stopwatch stopwatch = new Stopwatch();
        public override void Dump(QueueEntity.MetricMessage message)
        {
            
            //var formatString = string.Join("\n", new string[]{
            //    "Request to assemble presentation with following details:",
            //    "CompanyId : {0}",
            //    "Presentation Id: {1}",
            //    "PartitionKey: {2}",
            //    "Row Key: {3}",
            //    "Presentation File Name: {4}",
            //    "Action: {5}",
            //    "MyStuffRowKey: {6}",
            //    "Site: {7}"
            //});

            //LogHandler.LogDebug(formatString, LogHandler.Layer.WorkerHost, message.CompanyId, message.PresentationId,
            //    message.PartitionKey, message.RowKey, message.PresentationName, message.Action, message.MyStuffRowKey,
            //    message.Site);
        }

        public override bool Process(QueueEntity.MetricMessage messageList, int robotId, int runInstanceId, int robotTaskMapId)
        {
            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_Start, "Process", "MetricProcessor"), LogHandler.Layer.Business, null);
            LogHandler.LogDebug(String.Format("The Process Method of MetricProcessor class is getting executed with parameters : Metric message={0}; robotId={1};runInstanceId={2}; robotTaskMapId={3}", messageList, robotId,runInstanceId,robotTaskMapId),
                LogHandler.Layer.Business, null);

            #region avoidablePropertyList
            List<string> propertylist = new List<string>() { "SequenceNumber", "ConfigId", "PortfolioId", "ObservableId", "Count", "ResourceTypeId", "IncidentId", "IncidentTime", "Serverstate", "Description", "ServerIp" , "TransactionId" };
            #endregion
            try
            {
                if (messageList != null && messageList.MetricMessages != null && messageList.MetricMessages.Count > 0)
                {
                    foreach (var message in messageList.MetricMessages)
                    {
                        stopwatch = Stopwatch.StartNew();

                        using (LogHandler.TraceOperations("MetricProcessor:Process", LogHandler.Layer.Business, Guid.NewGuid(), null))
                        {
                            LogHandler.LogDebug("iterating through all the properties of Metric message to check for any null or empty values",
                                      LogHandler.Layer.Business, null);

                            foreach (PropertyInfo pi in message.GetType().GetProperties())
                            {
                                LogHandler.LogDebug(String.Format("checking null or empty values for the property: {0}", pi.Name),
                                    LogHandler.Layer.Business, null);
                                if (pi.PropertyType == typeof(string))
                                {
                                    string value = (string)pi.GetValue(message);
                                    if (string.IsNullOrEmpty(value) && !propertylist.Contains(pi.Name))
                                    {
                                        LogHandler.LogDebug("the value for the property :{0} is null or empty. value: {1}",
                                            LogHandler.Layer.Business, pi.Name, value);
                                        LogHandler.LogError(String.Format(ErrorMessages.Value_NullOrEmpty_Error, pi.Name), LogHandler.Layer.Business, null);

                                        //throwing exception for null or empty values in anamoly message                                
                                        SuperbotValidationException exception = new SuperbotValidationException(String.Format(ErrorMessages.Value_NullOrEmpty_Error, pi.Name));
                                        List<ValidationError> validationErrors_List = new List<ValidationError>();
                                        ValidationError validationErr = new ValidationError();
                                        validationErr.Code = "1044";
                                        validationErr.Description = string.Format(ErrorMessages.Value_NullOrEmpty_Error, pi.Name);
                                        validationErrors_List.Add(validationErr);

                                        if (validationErrors_List.Count > 0)
                                        {
                                            exception.Data.Add("ValueNullorEmptyErrors", validationErrors_List);
                                            throw exception;
                                        }

                                    }
                                }
                            }


                            BE.Metric receivedMessage = new BE.Metric();

                            EntityTranslator et = new EntityTranslator();
                            receivedMessage = et.MetricDEToBE(message);

                            Monitor mn = new Monitor();
                            LogHandler.LogDebug(String.Format("calling CheckThresholdBreach method of Monitor class with input:{0} ", receivedMessage),
                                LogHandler.Layer.Business, null);

                            if (!receivedMessage.MetricName.ToLower().Equals("environment scan"))
                            {
                                bool status = mn.CheckThresholdBreach(receivedMessage);
                                LogHandler.LogDebug(String.Format("The CheckThresholdBreach method of Monitor class returned {0} for input:{1} ", status.ToString(), receivedMessage),
                                    LogHandler.Layer.Business, null);

                                if (!status)
                                {
                                    // get anomaly messge
                                    LogHandler.LogDebug(String.Format("calling the GetOne method of ObservationDS class to get last entry in observations table"),
                                        LogHandler.Layer.Business, null);

                                    DE.anomaly_details anomalyDetails = new DE.anomaly_details();
                                    AnomalyDetailsDS anomalyDetailsDS = new AnomalyDetailsDS();
                                    var anomalyDetailsObj = anomalyDetailsDS.GetOne(anomalyDetails);

                                    if (anomalyDetailsObj != null)
                                    {
                                        BC.Action action = new BC.Action();
                                        int remediationPlanId = Helper.GetRemediationPlanId(anomalyDetailsObj.ResourceId, anomalyDetailsObj.ResourceTypeId, anomalyDetailsObj.ObservableId);                                        

                                        if (remediationPlanId != 0 && remediationPlanId != -1)
                                        {
                                            //logging to Performance metric - # Of Anomaly detected
                                            PerfMonLogger.LogIncrementValue(PerfMonLogger.SuperBotCategories.SuperBot, PerfMonLogger.SuperBotCounters.NumOfAnomalyDetected);

                                            // create remediation queue message
                                            LogHandler.LogDebug(String.Format("Creating an Anamoly message to send to the queue"),
                                                LogHandler.Layer.Business, null);

                                            QueueEntity.Anomaly anomaly = new QueueEntity.Anomaly();

                                            anomaly.ObservationId = anomalyDetailsObj.AnomalyId;
                                            anomaly.PlatformId = Convert.ToString(anomalyDetailsObj.PlatformId);
                                            anomaly.ResourceId = anomalyDetailsObj.ResourceId;
                                            anomaly.ResourceTypeId = anomalyDetailsObj.ResourceTypeId;
                                            anomaly.ObservableId = anomalyDetailsObj.ObservableId;
                                            anomaly.ObservableName = anomalyDetailsObj.ObservableName;
                                            anomaly.ObservationStatus = "Failed";
                                            anomaly.Value = Convert.ToString(anomalyDetailsObj.Value);
                                            anomaly.ThresholdExpression = "Anomaly detected in resource_type:" + anomalyDetailsObj.ResourceTypeId + "-resource:" + anomalyDetailsObj.ResourceId + " deployed on:" + anomalyDetailsObj.SourceIp + " for the monitored_metric:" + anomalyDetailsObj.ObservableName + ". Threshold breach rule:" + anomalyDetailsObj.Description;
                                            anomaly.ServerIp = anomalyDetailsObj.SourceIp;
                                            anomaly.ObservationTime = Convert.ToString(anomalyDetailsObj.ObservationTime);
                                            anomaly.EventType = "Anomaly Remediation";
                                            anomaly.Source = "Platform";
                                            if (string.IsNullOrEmpty(anomaly.Description))
                                            { anomaly.Description = "NA"; }


                                            RemediationDS rem = new RemediationDS();
                                            rem.Send(anomaly, "");
                                            //logging to Performance metric - # of Anomalies detected
                                            LogHandler.LogDebug(String.Format("The Anamoly message has been sent to the queue"),
                                                LogHandler.Layer.Business, null);
                                        }

                                        if (Helper.CheckNotificationRestriction(anomalyDetailsObj.ResourceId, anomalyDetailsObj.ObservableId, anomalyDetailsObj.PlatformId, anomalyDetailsObj.TenantId))
                                        {
                                            // create notification queue message
                                            LogHandler.LogDebug(String.Format("Creating an Notification message to send to the queue"),
                                                LogHandler.Layer.Business, null);
                                            QueueEntity.Notification notification = new QueueEntity.Notification();

                                            notification.ObservationId = anomalyDetailsObj.AnomalyId;
                                            notification.PlatformId = anomalyDetailsObj.PlatformId;
                                            notification.ResourceId = anomalyDetailsObj.ResourceId;
                                            notification.ResourceTypeId = anomalyDetailsObj.ResourceTypeId;
                                            notification.ObservableId = anomalyDetailsObj.ObservableId;
                                            notification.ObservableName = anomalyDetailsObj.ObservableName;
                                            notification.ObservationStatus = "Failed";
                                            notification.Value = Convert.ToString(anomalyDetailsObj.Value);
                                            notification.ThresholdExpression = "Anomaly detected in resource_type:" + anomalyDetailsObj.ResourceTypeId + "-resource:" + anomalyDetailsObj.ResourceId + " deployed on:" + anomalyDetailsObj.SourceIp + " for the monitored_metric:" + anomalyDetailsObj.ObservableName + ". Threshold breach rule:" + anomalyDetailsObj.Description;
                                            notification.ServerIp = anomalyDetailsObj.SourceIp;
                                            notification.ObservationTime = Convert.ToString(anomalyDetailsObj.ObservationTime);
                                            notification.EventType = "Anomaly Notifcation";
                                            notification.Source = "Platform";
                                            notification.TenantId = anomalyDetailsObj.TenantId;
                                            if (remediationPlanId != 0 && remediationPlanId != -1)
                                                notification.Type = (int)NotificationType.AnomalyDetected;
                                            else
                                            {
                                                notification.Type = (int)NotificationType.NoRemediation;
                                                //UPDATING THE REMEDIATION STATUS AS MANUAL REMEDIATION FROM NOT STARTED
                                                anomalyDetailsObj.RemediationStatus = "Manual Remediation";
                                                //if(anomalyDetailsDS.Update(anomalyDetailsObj) == null)
                                                //{
                                                //    //Log warning
                                                //}
                                            }


                                            notification.Channel = (int)NotificationChannel.Email;
                                            notification.BaseURL = "";
                                            if (string.IsNullOrEmpty(anomalyDetailsObj.Description))
                                                notification.Description = "NA";
                                            else
                                                notification.Description = anomalyDetailsObj.Description;


                                            NotificationDS not = new NotificationDS();
                                            not.Send(notification, "");
                                            LogHandler.LogDebug(String.Format("The Notification message has been sent to the queue"),
                                                LogHandler.Layer.Business, null);
                                            //Setting notified time
                                            anomalyDetailsObj.IsNotified = "Yes";
                                            anomalyDetailsObj.NotifiedTime = DateTime.UtcNow;
                                            anomalyDetailsObj.ModifiedDate = DateTime.UtcNow;
                                            anomalyDetailsObj.ModifiedBy = "admin@123";
                                            if (anomalyDetailsDS.Update(anomalyDetailsObj) == null)
                                            {
                                                LogHandler.LogWarning(String.Format(ErrorMessages.ValueUpdationUnsuccessful, "observations"), LogHandler.Layer.Business, null);
                                            }
                                        }

                                    }
                                    else
                                    {
                                        LogHandler.LogError(String.Format(ErrorMessages.RemediatioPlan_NotFound, "Data", "observations", "Latest entry"),
                                            LogHandler.Layer.Business, null);

                                        //throwing exception for null or empty values in anamoly message                                
                                        SuperbotValidationException exception = new SuperbotValidationException(String.Format(ErrorMessages.RemediatioPlan_NotFound, "Data", "observations", "Latest entry"));
                                        List<ValidationError> validationErrors_List = new List<ValidationError>();
                                        ValidationError validationErr = new ValidationError();
                                        validationErr.Code = "1042";
                                        validationErr.Description = string.Format(ErrorMessages.RemediatioPlan_NotFound, "Data", "observations", "Latest entry");
                                        validationErrors_List.Add(validationErr);

                                        if (validationErrors_List.Count > 0)
                                        {
                                            exception.Data.Add("DataNotFoundErrors", validationErrors_List);
                                            throw exception;
                                        }
                                        return false;
                                    }



                                }
                            }
                            else
                            {
                                EnvironmentScanMetricAnalyser environmentScanMetricAnalyser = new EnvironmentScanMetricAnalyser();
                                return environmentScanMetricAnalyser.CheckAnomaly(receivedMessage);
                            }



                            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_End, "Process", "MetricProcessor"), LogHandler.Layer.Business, null);                           

                        }

                        //logging to Performance metric - # of message processed
                        PerfMonLogger.LogIncrementValue(PerfMonLogger.SuperBotCategories.SuperBot, PerfMonLogger.SuperBotCounters.NumOfMsgProcessed);

                        //logging to Performance metric - # of message processed / sec
                        PerfMonLogger.LogIncrementValue(PerfMonLogger.SuperBotCategories.SuperBot, PerfMonLogger.SuperBotCounters.NumOfMsgProcessedPerSec);

                        //logging to Performance metric - Message processing time
                        stopwatch.Stop();

                        PerfMonLogger.LogRawValue(PerfMonLogger.SuperBotCategories.SuperBot, PerfMonLogger.SuperBotCounters.MsgProcessTime, stopwatch.ElapsedMilliseconds);
                    }
                }                
                
                return true;
            }
            catch (Exception exMP)
            {
                //logging to Performance metric - # of message errors detected
                PerfMonLogger.LogIncrementValue(PerfMonLogger.SuperBotCategories.SuperBot, PerfMonLogger.SuperBotCounters.NumOfErrors);

                if (exMP is SuperbotValidationException || exMP is SuperbotDataItemNotFoundException)
                {
                    string message = JsonConvert.SerializeObject(messageList);
                    LogHandler.LogError("Superbot Custome Exception {0} occured in Process method of MetricProcessor class Exception : {1} and StackTrace {2} ",
                    LogHandler.Layer.Business, exMP.GetType().ToString(), exMP.Message, exMP.StackTrace);
                    LogHandler.LogError("Failed Message : {0}",
                    LogHandler.Layer.Business, message);
                    //returning true to read next message
                    return true;
                }
                LogHandler.LogError("Exception occured in Process method of MetricProcessor class Exception : {0} and StackTrace {1} ",
                    LogHandler.Layer.Business, exMP.Message, exMP.StackTrace);
                bool failureLogged = false;

                try
                {
                    Exception ex = new Exception();
                    bool rethrow = ExceptionHandler.HandleException(exMP, ApplicationConstants.WORKER_EXCEPTION_HANDLING_POLICY, out ex);
                    failureLogged = true;
                    if (rethrow)
                    {                        
                        throw ex;
                    }
                    else
                    {
                        //Set as a succesfull operation as the message was invalid since an equivalent presentation entity was
                        //not found in the database. This could be a rogue transaction.
                        //returning a true since the message has been sent with invalid presentation id and has to be deleted
                        //to avoid further processing
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    LogHandler.LogError("Exception Occured while handling an exception, Exception : {0} and StackTrace {1} ",
                        LogHandler.Layer.Business, ex.Message, ex.StackTrace);

                    //Any messages which would have to indicate to the worker process that the transaction has failed
                    // and the messahe should be retried
                    //MetricProcessing Request  processing failed
                    if (!failureLogged)
                    {
                        LogHandler.LogDebug(String.Format("Exception Occured while handling an exception. error message: {0}", ex.Message), LogHandler.Layer.Business, null);
                    }

                    return false;
                }
            }
        }

    }
}
