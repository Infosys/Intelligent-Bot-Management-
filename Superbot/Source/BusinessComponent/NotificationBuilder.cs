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
using System.Data;
using System.Data.SqlClient;
using IE = Infosys.Solutions.Ainauto.Services.Superbot.Contracts.Data;
using BE = Infosys.Solutions.Ainauto.Superbot.BusinessEntity;
using DE = Infosys.Solutions.Superbot.Resource.Entity.Queue;
using Infosys.Solutions.Ainauto.Resource.DataAccess;
using Infosys.Solutions.Superbot.Infrastructure.Common;
using Infosys.Solutions.Superbot.Resource.Entity;
using Translator = Infosys.Solutions.Ainauto.Superbot.BusinessComponent.Translator;
using System.Reflection;
using Newtonsoft.Json;
using System.Globalization;
using System.IO;

namespace Infosys.Solutions.Ainauto.Superbot.BusinessComponent
{
    public class NotificationBuilder
    {
        public BE.Notification BuildNotification(DE.Notification message)
        {
            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_Start, "BuildNotification", "NotificationBuilder"), LogHandler.Layer.Business, null);
            LogHandler.LogDebug(String.Format("The BuildNotification Method of NotificationBuilder class is getting executed with parameters : Metric message={0};", message),
                LogHandler.Layer.Business, null);

            string result = string.Empty;
            BE.Notification notification = new BE.Notification();

            LogHandler.LogDebug(String.Format("Creating the service channel inside BuildNotification method"),
                LogHandler.Layer.Business, null);

            Infrastructure.ServiceClientLibrary.SuperBot resourceHandler = new Infrastructure.ServiceClientLibrary.SuperBot();
            var channel = resourceHandler.ServiceChannel;

            LogHandler.LogDebug(String.Format("Calling the Get ResourceDetails service"),
                LogHandler.Layer.Business, null);
            //getting the resource details
            IE.ResourceDetails resourceDetails = channel.GetResourceDetails(message.ResourceId, message.TenantId, message.PlatformId);

            if (resourceDetails==null)
            {
                LogHandler.LogError(String.Format(ErrorMessages.Method_Returned_Null, "GetResourceDetails", "ResourceHandlerController", "Resource Id :"+ message.ResourceId + " ; TenantId :"+ message.TenantId +" ; Platform ID :"+ message.PlatformId),
                                        LogHandler.Layer.Business, null);
                SuperbotDataItemNotFoundException exception = new SuperbotDataItemNotFoundException(String.Format(ErrorMessages.Method_Returned_Null, "GetResourceDetails", "ResourceHandlerController", "Resource Id :" + message.ResourceId + " ; TenantId :" + message.TenantId + " ; Platform ID :" + message.PlatformId));
                List<ValidationError> validationErrors_List = new List<ValidationError>();
                ValidationError validationErr = new ValidationError();
                validationErr.Code = "1045";
                validationErr.Description = string.Format(ErrorMessages.Method_Returned_Null, "GetResourceDetails", "ResourceHandlerController", "Resource Id :" + message.ResourceId + " ; TenantId :" + message.TenantId + " ; Platform ID :" + message.PlatformId);
                validationErrors_List.Add(validationErr);

                if (validationErrors_List.Count > 0)
                {
                    exception.Data.Add("DataNotFoundErrors", validationErrors_List);
                    throw exception;
                }
            }
            
            notification.ResourceId = resourceDetails.ResourceId;
            notification.ResourceName = resourceDetails.ResourceName;
            notification.ResourceTypeId = resourceDetails.ResourceTypeId.ToString();
            notification.TenantId = resourceDetails.TenantId.ToString();
            notification.TenantName = resourceDetails.TenantName;
            notification.PlatformId = resourceDetails.PlatformId.ToString();
            notification.PlatformName = resourceDetails.PlatformName;
            notification.ResourceTypeName = resourceDetails.ResourceTypeName;
            notification.HostName = resourceDetails.HostName;

            LogHandler.LogDebug(String.Format("Calling the GetObservationsDetails service"),
                LogHandler.Layer.Business, null);
            IE.ObservationDetails observationDetails = channel.GetObservationsDetails(message.ObservationId,message.PlatformId,message.TenantId);
            if (observationDetails == null)
            {
                LogHandler.LogError(String.Format(ErrorMessages.Method_Returned_Null, "GetObservationsDetails", "ResourceHandlerController", "Observation Id :" + message.ObservationId + " ; TenantId :" + message.TenantId + " ; Platform ID :" + message.PlatformId),
                                        LogHandler.Layer.Business, null);
                SuperbotDataItemNotFoundException exception = new SuperbotDataItemNotFoundException(String.Format(ErrorMessages.Method_Returned_Null, "GetResourceDetails", "ResourceHandlerController", "Observation Id :" + message.ObservationId + " ; TenantId :" + message.TenantId + " ; Platform ID :" + message.PlatformId));
                List<ValidationError> validationErrors_List = new List<ValidationError>();
                ValidationError validationErr = new ValidationError();
                validationErr.Code = "1045";
                validationErr.Description = string.Format(ErrorMessages.Method_Returned_Null, "GetResourceDetails", "ResourceHandlerController", "Observation Id :" + message.ObservationId +" ; TenantId :" + message.TenantId + " ; Platform ID :" + message.PlatformId);
                validationErrors_List.Add(validationErr);

                if (validationErrors_List.Count > 0)
                {
                    exception.Data.Add("DataNotFoundErrors", validationErrors_List);
                    throw exception;
                }
            }
            // values from observation details
            notification.ObservationId = observationDetails.ObservationId.ToString();
            notification.ObservableId = observationDetails.ObservableId.ToString();
            notification.ObservableName = observationDetails.ObservableName;
            notification.ObservationStatus = observationDetails.ObservationStatus;
            //DateTime dt = DateTime.ParseExact(observationDetails.ObservationTime.ToString(), "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            //notification.ObservationTime = dt.ToString("dd/M/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            //notification.ObservationTime = observationDetails.ObservationTime.ToString();]
            DateTime dt = Convert.ToDateTime(observationDetails.ObservationTime);
            notification.ObservationTime = dt.ToString("dd/M/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            //notification.ThresholdExpression = observationDetails.Description;
            //notification.RemediationPlanId =observationDetails.RemediationPlanId.ToString();
            notification.RemediationPlanStatus = observationDetails.RemediationStatus;
            dt = Convert.ToDateTime(observationDetails.RemediationPlanTime);
            notification.RemediationPlanTime = dt.ToString("dd/M/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            //DateTime dt1 = DateTime.ParseExact(observationDetails.RemediationPlanTime.ToString(), "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            //notification.RemediationPlanTime = dt1.ToString("dd/M/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            //notification.RemediationPlanTime = observationDetails.RemediationPlanTime.ToString();
            notification.ServerIp = observationDetails.ServerIp;
            notification.Source = observationDetails.Source;
            dt = Convert.ToDateTime(observationDetails.AlertTime);
            //DateTime dt2 = DateTime.ParseExact(observationDetails.AlertTime.ToString(), "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            notification.AlertTime = dt.ToString("dd/M/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            //notification.AlertTime = observationDetails.AlertTime.ToString();
            String[] spearator = { "Anomaly found.", "Reference Details: " };
            Int32 count = 3;

            // using the method 
            String[] strlist = observationDetails.Description.Split(spearator, count,
                  StringSplitOptions.RemoveEmptyEntries);
            notification.Description = "The "+ notification.ObservableName + strlist[1];
            //notification.Description = observationDetails.Description;

            // values from remediationPlan details

            //notification.RemediationPlanDescription = remediationPlanDetails.RemediationPlanDescription;
            
            if (observationDetails.RemediationPlanId != 0)
            {
                LogHandler.LogDebug(String.Format("Calling the GetRemediationPlanDetails service"),
                LogHandler.Layer.Business, null);
                IE.RemediationPlanDetails remediationPlanDetails = channel.GetRemediationPlanDetails(observationDetails.RemediationPlanId, message.TenantId);
                if (remediationPlanDetails == null)
                {
                    LogHandler.LogError(String.Format(ErrorMessages.Method_Returned_Null, "GetRemediationPlanDetails", "ResourceHandlerController", "Remediation plan Id :" + observationDetails.RemediationPlanId + " ; TenantId :" + message.TenantId),
                                            LogHandler.Layer.Business, null);
                    SuperbotDataItemNotFoundException exception = new SuperbotDataItemNotFoundException(String.Format(ErrorMessages.Method_Returned_Null, "GetRemediationPlanDetails", "ResourceHandlerController", "Remediation plan Id :" + observationDetails.RemediationPlanId + " ; TenantId :" + message.TenantId));
                    List<ValidationError> validationErrors_List = new List<ValidationError>();
                    ValidationError validationErr = new ValidationError();
                    validationErr.Code = "1045";
                    validationErr.Description = string.Format(ErrorMessages.Method_Returned_Null, "GetRemediationPlanDetails", "ResourceHandlerController", "Remediation plan Id :" + observationDetails.RemediationPlanId + " ; TenantId :" + message.TenantId);
                    validationErrors_List.Add(validationErr);

                    if (validationErrors_List.Count > 0)
                    {
                        exception.Data.Add("DataNotFoundErrors", validationErrors_List);
                        throw exception;
                    }
                }
                notification.RemediationPlanName = remediationPlanDetails.RemediationPlanName;
            }

            //List<BE.ActionDetails> actionList = new List<BE.ActionDetails>();
            ////loop through actions
            //foreach(var action in remediationPlanDetails.ActionDetails)
            //{
            //    BE.ActionDetails actionDetails = new BE.ActionDetails();
            //    actionDetails.ActionId = action.ActionId;
            //    actionDetails.ActionName = action.ActionName;
            //    actionDetails.ActionSequence = action.ActionSequence;
            //    actionDetails.ActionStageId = action.ActionStageId;
            //    actionList.Add(actionDetails);
            //}
            //notification.ActionDetails = actionList;
            NotificationConfiguration nc = new NotificationConfiguration();
            notification.AnomalyReason = "";
            //notification.AnomalyReason = nc.GetAnomalyReasonIntermediate(ApplicationConstants.ReferenceTypes.ANOMALY_REASON, notification.ObservableName, Convert.ToInt32(notification.TenantId), Convert.ToInt32(notification.PlatformId)).ReferenceValue;
            notification.NotificationSender = "sender";
            //notification.AnomalyReason = "yet to be configured";

            //result = RecMethod(notification);
            //return result;
            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_End, "BuildNotification", "NotificationBuilder"), LogHandler.Layer.Business, null);
            return notification;
        }

        public BE.EnvironmentScanReportDetails BuildEnvironmentScanNotification(DE.Notification message)
        {
            Infrastructure.ServiceClientLibrary.SuperBot resourceHandler = new Infrastructure.ServiceClientLibrary.SuperBot();
            var channel = resourceHandler.ServiceChannel;
            BE.EnvironmentScanReportDetails environmentScanReportDetails = new BE.EnvironmentScanReportDetails();

            //getting the Anomaly details
            // call GetEnvironmentScanAnomalyDetails service 
            BE.EnvironmentScanReportDetails anomalyDetails = Translator.Translator_IEtoBE.EnvironmentScanAnomalyDetails_BEtoIE(channel.GetEnvironmentScanAnomalyDetails(message.ResourceId, message.ObservationId, message.PlatformId,message.TenantId));
            
            return anomalyDetails;
        }

        public BE.EnvironmentScanReportDetails GetEnvironmentScanAnomalyDetails(string resourceId, int observationId, int platformId, int tenantId)
        {
            BE.EnvironmentScanReportDetails environmentScanReportDetails = new BE.EnvironmentScanReportDetails();
            List<BE.EnvironmentScanAnomalyDetails> anomalyDetailsList = new List<BE.EnvironmentScanAnomalyDetails>();
           
            resource resourceObj = new resource { ResourceId = resourceId };            
            var resourceDetail = new ResourceDS().GetOne(resourceObj);
            resourcetype resourcetypeObj = new resourcetype { ResourceTypeId = resourceDetail.ResourceTypeId };
            var resourceTypeTable = new ResourceTypeDS().GetOne(resourcetypeObj);
            
            EnvironmentScanMetricAnomalyDetailsDS environmentScanMetricAnomalyDetailsDS = new EnvironmentScanMetricAnomalyDetailsDS();
            Environment_Scan_Metric_Anomaly_Details anomalyDetailsDE = new Environment_Scan_Metric_Anomaly_Details { ResourceId = resourceId, ObservationId = observationId, PlatformId = platformId, TenantId = tenantId };
            var res = environmentScanMetricAnomalyDetailsDS.GetAll(anomalyDetailsDE);

            foreach (var obj in res)
            {
                BE.EnvironmentScanAnomalyDetails anomalyDetailsObj = new BE.EnvironmentScanAnomalyDetails();

                anomalyDetailsObj.MetricName = obj.MetricName;
                anomalyDetailsObj.MetricId = obj.MetricId;
                anomalyDetailsObj.MetricKey = obj.MetricKey;
                anomalyDetailsObj.AttributeName = obj.AttributeName;
                anomalyDetailsObj.AttributeValueNew = obj.AttributeValue;
                anomalyDetailsObj.AttributeValueOld = obj.OldValue;
                anomalyDetailsObj.AttributeStatus = obj.AttributeStatus;

                anomalyDetailsList.Add(anomalyDetailsObj);
            }

            environmentScanReportDetails.ResourceName = resourceDetail.ResourceName;
            environmentScanReportDetails.ResourceTypeName = resourceTypeTable.ResourceTypeName;
            environmentScanReportDetails.OldReportDate = res.First().StartDate.ToString("dd/MM/yyyy");
            environmentScanReportDetails.NewReportDate = res.First().EndDate.ToString("dd/MM/yyyy");
            environmentScanReportDetails.EnvironmentScanAnomalyDetails = anomalyDetailsList;
            environmentScanReportDetails.PlatformName = new PlatformsDS().GetOne(new platforms() { PlatformId = platformId }).PlatformName;
            environmentScanReportDetails.TenantName = new TenantDS().GetOne(new tenant() { TenantId = tenantId }).Name;
            return environmentScanReportDetails;
        }

            //public BE.EnvironmentScanAnomalyDetails GetEnvironmentScanAnomalyDetails(string resourceId, int observationId,int platformId,int tenantId)
            //{
            //BE.EnvironmentScanAnomalyDetails anomalyDetailsBE = new BE.EnvironmentScanAnomalyDetails();
            //List<BE.ESInstalledSoftware> installedSoftwareList = new List<BE.ESInstalledSoftware>();
            //BE.ESOSDetails osDetails = new BE.ESOSDetails();
            //BE.ESScreenResolution screenResolution = new BE.ESScreenResolution();

            //EnvironmentScanMetricAnomalyDetailsDS environmentScanMetricAnomalyDetailsDS = new EnvironmentScanMetricAnomalyDetailsDS();
            //Environment_Scan_Metric_Anomaly_Details anomalyDetailsDE = new Environment_Scan_Metric_Anomaly_Details {ResourceId=resourceId,ObservationId=observationId,PlatformId=platformId,TenantId=tenantId };
            //var res = environmentScanMetricAnomalyDetailsDS.GetAll(anomalyDetailsDE);
            //var groupedList = res.GroupBy(entity => new { entity.MetricName, entity.MetricId });

            //foreach(var group in groupedList)
            //{
            //    if(group.Key.MetricName == "Installed Software")
            //    {
            //        BE.ESInstalledSoftware installedSoftware = new BE.ESInstalledSoftware();

            //        foreach (var groupObj in group)
            //        {                        
            //            switch (groupObj.AttributeName)
            //            {
            //                case "Display name":
            //                    installedSoftware.SoftwareNameNew = groupObj.AttributeValue;
            //                    installedSoftware.SoftwareNameOld = groupObj.Description;
            //                    break;
            //                case "Version":
            //                    installedSoftware.SoftwareVersionNew = groupObj.AttributeValue;
            //                    installedSoftware.SoftwareVersionOld = groupObj.Description;
            //                    break;
            //                case "InstallDate":
            //                    installedSoftware.InstalledDateNew = groupObj.AttributeValue;
            //                    installedSoftware.InstalledDateOld = groupObj.Description;
            //                    //installedSoftware.InstalledDateNew = Convert.ToDateTime(groupObj.AttributeValue);
            //                    //installedSoftware.InstalledDateOld = groupObj.Description != "" ? Convert.ToDateTime(groupObj.Description) : (DateTime?)null;
            //                    break;
            //                case "Publisher":
            //                    installedSoftware.PublisherNew = groupObj.AttributeValue;
            //                    installedSoftware.PublisherOld = groupObj.Description;
            //                    break;

            //            }
            //            installedSoftware.Status = groupObj.AttributeStatus;
            //        }
            //        installedSoftwareList.Add(installedSoftware);
            //    }
            //    else if (group.Key.MetricName == "OS Details")
            //    {
            //        foreach (var groupObj in group)
            //        {
            //            switch (groupObj.AttributeName)
            //            {
            //                case "Caption":
            //                    osDetails.OSNameNew = groupObj.AttributeValue;
            //                    osDetails.OSNameOld = groupObj.Description;
            //                    break;
            //                case "OS Architecture":
            //                    osDetails.SystemTypeNew = groupObj.AttributeValue;
            //                    osDetails.SystemTypeOld = groupObj.Description;
            //                    break;
            //                case "Version":
            //                    osDetails.VersionNew = groupObj.AttributeValue;
            //                    osDetails.VersionOld = groupObj.Description;
            //                    break;
            //                case "BuildNumber":
            //                    osDetails.BuildNumberNew = groupObj.AttributeValue;
            //                    osDetails.BuildNumberOld = groupObj.Description;
            //                    break;
            //            }
            //            osDetails.Status = groupObj.AttributeStatus;
            //        }
            //    }
            //    else if (group.Key.MetricName == "Screen Resolution")
            //    {
            //        foreach (var groupObj in group)
            //        {
            //            switch (groupObj.AttributeName)
            //            {
            //                case "PelsHeight":
            //                    screenResolution.HeightNew = groupObj.AttributeValue;
            //                    screenResolution.HeightOld = groupObj.Description;
            //                    break;
            //                case "PelsWidth":
            //                    screenResolution.WidthNew = groupObj.AttributeValue;
            //                    screenResolution.WidthOld = groupObj.Description;
            //                    break;
            //            }
            //            screenResolution.Status = groupObj.AttributeStatus;
            //        }
            //    }
            //}
            //anomalyDetailsBE.InstalledSoftware = installedSoftwareList;
            //anomalyDetailsBE.OSDetails = osDetails;
            //anomalyDetailsBE.ScreenResolution = screenResolution;
            //return anomalyDetailsBE;
            //}

        public static string RecMethod(object obj)
        {
            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_Start, "RecMethod", "NotificationBuilder"), LogHandler.Layer.Business, null);
            LogHandler.LogDebug(String.Format("The RecMethod method of NotificationBuilder class is getting executed with input: notification details = {0} ",obj),
                LogHandler.Layer.Business, null);
            string result = string.Empty;
            foreach (PropertyInfo pi in obj.GetType().GetProperties())
            {
                if (pi.PropertyType == typeof(List<BE.ActionDetails>))
                {
                    result = result + "Actions for the remediation plan:\n";
                    List<BE.ActionDetails> actionListpi = (List<BE.ActionDetails>)pi.GetValue(obj);
                    foreach (var actionObj in actionListpi)
                    {                        
                        result = result + RecMethod(actionObj);
                    }

                }
                else
                    result = result + pi.Name + " : " + pi.GetValue(obj) + "\n";
            }
            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_End, "RecMethod", "NotificationBuilder"), LogHandler.Layer.Business, null);
            return result;
        }

        public BE.ObservationDetails GetObservationsDetails(int observationId, int platformId, int tenantId)
        {
            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_Start, "GetObservationsDetails", "NotificationBuilder"), LogHandler.Layer.Business, null);
            LogHandler.LogDebug(String.Format("The GetObservationsDetails method of NotificationBuilder class is getting executed with input: observationId id={0} ; platformId={1} ; tenantId={1}", observationId, platformId, tenantId),
                LogHandler.Layer.Business, null);
            BE.ObservationDetails result = new BE.ObservationDetails();
            //ObservationsDS observationsDS = new ObservationsDS();
            AnomalyDetailsDS anomalyDetailsDS = new AnomalyDetailsDS();
            RemediationPlanDS remediationPlanDS = new RemediationPlanDS();
            RemediationPlanExecutionsDS remediationPlanExecutionsDS = new RemediationPlanExecutionsDS();
            var observationTable = (from obs in anomalyDetailsDS.GetAny() select obs).ToList();
            var remediationPlanExecutionTable = (from rpe in remediationPlanExecutionsDS.GetAny() select rpe).ToList();

            var observationDetails = (from obs in observationTable where obs.AnomalyId == observationId select obs).First();

            if (observationDetails.RemediationPlanExecId == null)
            {
                result = (from obs in observationTable                              
                              where obs.AnomalyId == observationId
                              && obs.TenantId == tenantId
                              && obs.PlatformId == platformId
                              select new BE.ObservationDetails
                              {
                                  ObservationId = obs.AnomalyId,
                                  ObservableId = obs.ObservableId,
                                  ObservableName = obs.ObservableName,
                                  ObservationStatus = obs.ObservationStatus,
                                  ObservationTime = obs.ObservationTime,
                                  Description = obs.Description,                                  
                                  RemediationStatus = obs.RemediationStatus,                                  
                                  ServerIp = obs.SourceIp,
                                  Source = obs.Source,
                                  AlertTime = obs.NotifiedTime
                              }).FirstOrDefault<BE.ObservationDetails>();
            }
            else
            {
                result = (from obs in observationTable
                              join rpe in remediationPlanExecutionTable
                              on obs.RemediationPlanExecId equals rpe.RemediationPlanExecId
                              where obs.AnomalyId == observationId
                              && obs.TenantId == tenantId
                              && obs.PlatformId == platformId
                              select new BE.ObservationDetails
                              {
                                  ObservationId = obs.AnomalyId,
                                  ObservableId = obs.ObservableId,
                                  ObservableName = obs.ObservableName,
                                  ObservationStatus = obs.ObservationStatus,
                                  ObservationTime = obs.ObservationTime,
                                  Description = obs.Description,
                                  RemediationPlanId = rpe.RemediationPlanId,
                                  RemediationStatus = obs.RemediationStatus,
                                  RemediationPlanTime = rpe.ExecutionEndDateTime,
                                  ServerIp = obs.SourceIp,
                                  Source = obs.Source,
                                  AlertTime = obs.NotifiedTime
                              }).FirstOrDefault<BE.ObservationDetails>();
            }

            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_End, "GetObservationsDetails", "NotificationBuilder"), LogHandler.Layer.Business, null);
            return result;
        }

        public BE.RemediationPlanDetails GetRemediationPlanDetails(int remediationPlanId, int tenantId)
        {
            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_Start, "GetRemediationPlanDetails", "NotificationBuilder"), LogHandler.Layer.Business, null);
            LogHandler.LogDebug(String.Format("The GetRemediationPlanDetails method of NotificationBuilder class is getting executed with input: remediation plan id={0} ; tenantId={1} ", remediationPlanId, tenantId),
                LogHandler.Layer.Business, null);
            //BE.RemediationPlanDetails remediationPlanDetails = new BE.RemediationPlanDetails();
            RemediationPlanDS remediationPlanDS = new RemediationPlanDS();
            //RemediationPlanActionMapDS remediationPlanActionMapDS = new RemediationPlanActionMapDS();
            //ActionDS actionDS = new ActionDS();
            var result = (from rp in remediationPlanDS.GetAny().ToArray()                                                   
                          where rp.RemediationPlanId == remediationPlanId
                          && rp.TenantId == tenantId
                          select new BE.RemediationPlanDetails
                          {
                              RemediationPlanId = rp.RemediationPlanId,
                              RemediationPlanName = rp.RemediationPlanName,
                              RemediationPlanDescription = rp.RemediationPlanDescription,
                              isUserDefined = rp.IsUserDefined
                          }).FirstOrDefault();

            //remediationPlanDetails.RemediationPlanId = result.First().RemediationPlanId;
            //remediationPlanDetails.RemediationPlanName = result.First().RemediationPlanName;
            //remediationPlanDetails.RemediationPlanDescription = result.First().RemediationPlanDescription;
            //remediationPlanDetails.isUserDefined = result.First().IsUserDefined;

            //List<BE.ActionDetails> actionList = new List<BE.ActionDetails>();
            //foreach (var res in result)
            //{
            //    BE.ActionDetails actionDetails = new BE.ActionDetails();
            //    actionDetails.ActionId = res.ActionId;
            //    actionDetails.ActionName = res.ActionName;
            //    actionDetails.ActionSequence = res.ActionSequence;
            //    actionDetails.ActionStageId = res.ActionStageId;

            //    actionList.Add(actionDetails);
            //}
            //remediationPlanDetails.ActionDetails = actionList;
            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_End, "GetRemediationPlanDetails", "NotificationBuilder"), LogHandler.Layer.Business, null);

            return result;
        }
        public List<string> getResourceHierarchy(string resourceId, int tenantId, int platformId)
        {
            ResourceDependencyMapDS resourceDependencyMapDS = new ResourceDependencyMapDS();
            string resourceIdTemp = resourceId;
            List<string> resourceIdList = new List<string>();
            LogHandler.LogDebug(String.Format("calling the getAny method of resourceDependencyMapDS from GetResourceDetails of NotificationBuilder class"),
                LogHandler.Layer.Business, null);
            var resDependencyTable = (from resMap in resourceDependencyMapDS.GetAny() select resMap).ToList();

            if (resDependencyTable == null)
            {
                LogHandler.LogError(String.Format(ErrorMessages.Method_Returned_Null, "getAny", "resourceDependencyMapDS", "No parameters"),
                                        LogHandler.Layer.Business, null);
                SuperbotDataItemNotFoundException exception = new SuperbotDataItemNotFoundException(String.Format(ErrorMessages.Method_Returned_Null, "getAny", "resourceDependencyMapDS", "No parameters"));
                List<ValidationError> validationErrors_List = new List<ValidationError>();
                ValidationError validationErr = new ValidationError();
                validationErr.Code = "1045";
                validationErr.Description = string.Format(ErrorMessages.Method_Returned_Null, "getAny", "resourceDependencyMapDS", "No parameters");
                validationErrors_List.Add(validationErr);

                if (validationErrors_List.Count > 0)
                {
                    exception.Data.Add("DataNotFoundErrors", validationErrors_List);
                    throw exception;
                }
            }

            LogHandler.LogDebug(String.Format("getting the list of dependency resource Id for resoruceId: {0}", resourceId),
                LogHandler.Layer.Business, null);
            while (true)
            {
                var res = (from resMap in resDependencyTable
                           where resMap.ResourceId == resourceIdTemp
                           && resMap.TenantId == tenantId
                           select new { resMap.ResourceId, resMap.DependencyResourceId }).First();
                resourceIdList.Add(res.ResourceId);
                if (res.DependencyResourceId == "")
                {
                    break;
                }
                else
                {
                    resourceIdTemp = res.DependencyResourceId;
                }
            }
            return resourceIdList;
        }
        public BE.ResourceDetails GetResourceDetails(string resourceId, int tenantId, int platformId)
        {
            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_Start, "GetResourceDetails", "NotificationBuilder"), LogHandler.Layer.Business, null);
            LogHandler.LogDebug(String.Format("The GetResourceDetails method of NotificationBuilder class is getting executed with input: resourceId={0} ; tenantId={1} ; platformId={2}", resourceId,tenantId,platformId),
                LogHandler.Layer.Business, null);

            //List<string> resourceIdList = getResourceHierarchy(resourceId,tenantId,platformId);
            List<string> resourceIdList = Helper.GetResourceHierarchy(resourceId, tenantId);

            List<string> hostname = new List<string>() { "hostname", "botclient"};
            var tenantTable = new TenantDS().GetAll();
            var platformsTable = new PlatformsDS().GetAll();
            var resourceTable = new ResourceDS().GetAll();
            var resourceTypeTable = new ResourceTypeDS().GetAll();
            var resourceAttributesTable = new ResourceAttributesDS().GetAll();
            
            var result = (from res in resourceTable
                          join t in tenantTable
                          on res.TenantId equals t.TenantId
                          join p in platformsTable
                          on res.PlatformId equals p.PlatformId
                          join resty in resourceTypeTable
                          on res.ResourceTypeId equals resty.ResourceTypeId                          
                          where res.ResourceId == resourceId
                          && res.TenantId == tenantId
                          && res.PlatformId == platformId                          
                          select new BE.ResourceDetails
                          {
                              ResourceId = res.ResourceId,
                              ResourceName = res.ResourceName,
                              ResourceTypeId = res.ResourceTypeId,
                              PlatformId = p.PlatformId,
                              PlatformName = p.PlatformName,
                              TenantId = t.TenantId,
                              TenantName = t.Name,
                              ResourceTypeName = resty.ResourceTypeName,
                              HostName = ""

                          }).FirstOrDefault();

            foreach (string resId in resourceIdList)
            {
                var res = (from resattr in resourceAttributesTable
                           where resattr.ResourceId == resId
                           && hostname.Contains(resattr.AttributeName.ToLower())
                           select resattr.AttributeValue).FirstOrDefault();
                if (res != "" && res != null)
                {
                    result.HostName = res;
                    break;
                }
            }
            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_End, "GetResourceDetails", "NotificationBuilder"), LogHandler.Layer.Business, null);
            return result;
        }

        public List<string> GetAuditLogOutputAttachments(int anomalyId)
        {
            List<string> returnList = new List<string>();
            Audit_Logs audit_Logs = new Audit_Logs() { AnomalyId = anomalyId };
            try
            {
                var AuditLogList = new AuditLogsDS().GetAll(audit_Logs).Where(a=>a.Output.Contains("Details:"));
                String[] spearator = { "Details:" };
                Int32 count = 2;
                int i = 0;
                foreach (Audit_Logs AuditLogObj in AuditLogList)
                {
                    returnList.Add(String.Format("\\r\\nDetail {0}:", ++i));
                    //returnList.Add(AuditLogObj.Output.Split(spearator, count,StringSplitOptions.RemoveEmptyEntries)[1].Replace("\\r\\n", Environment.NewLine).Trim());
                    returnList.Add(AuditLogObj.Output.Split(spearator, count, StringSplitOptions.RemoveEmptyEntries)[1].Trim());
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnList;
        }

        //public List<BE.NotificationSummaryDetails> BuildSummaryNotification(string configId, int platformId, string portfolioId, int tenantId)
        //{
        //    ObservationsDS observationsDS = new ObservationsDS();
        //    ResourceDS resourceDS = new ResourceDS();
        //    ResourceTypeDS resourceTypeDS = new ResourceTypeDS();

        //    List<BE.NotificationSummaryDetails> retList = new List<BE.NotificationSummaryDetails>();            
        //    List<observations> obsList = new List<observations>();

        //    var obsTable = (from obs in observationsDS.GetAll()
        //                    where obs.TenantId == tenantId
        //                    && obs.PlatformId == platformId
        //                    && (CheckCondition(obs.ConfigId, configId))
        //                    //&& (CheckCondition(obs.PlatformId.ToString(), platformId))
        //                    && (CheckCondition(obs.PortfolioId, portfolioId))
        //                    group obs by new { obs.ResourceId, obs.ObservableId } into g
        //                    select g).ToList();

        //    foreach (var grp in obsTable)
        //    {
        //        var obj = (from g in grp
        //                   orderby g.ObservationTime descending
        //                   select g).FirstOrDefault();
        //        obsList.Add(obj);
        //    }

        //    retList = (from obs in obsList
        //               join r in resourceDS.GetAll()
        //               on obs.ResourceId equals r.ResourceId
        //               join rt in resourceTypeDS.GetAll()
        //               on r.ResourceTypeId equals rt.ResourceTypeId
        //               select new BE.NotificationSummaryDetails
        //               {
        //                   ResourceId = r.ResourceId,
        //                   ResourceName =  r.ResourceName,
        //                   ResourceTypeId = rt.ResourceTypeId.ToString(),
        //                   ResourceTypeName = rt.ResourceTypeName,
        //                   ObservableId = obs.ObservableId.ToString(),
        //                   ObservableName = obs.ObservableName,
        //                   Value = obs.Value,
        //                   Status = obs.State,
        //                   ObservationTime =Convert.ToDateTime(obs.ObservationTime)
        //               }).ToList();

        //    return retList;
        //}


        public List<BE.NotificationSummaryDetails> BuildSummaryNotification(DE.Notification message)
        {
            ObservationsDS observationsDS = new ObservationsDS();
            ResourceDS resourceDS = new ResourceDS();
            ResourceTypeDS resourceTypeDS = new ResourceTypeDS();
            ObservableDS observableDS = new ObservableDS();

            List<observations> obsList = new List<observations>();
            List<BE.NotificationSummaryDetails> retList = new List<BE.NotificationSummaryDetails>();

            if (message.ApplicationName.Equals("HealthCheck",StringComparison.InvariantCultureIgnoreCase))
            {
                AuditLogsDS auditLogsDS = new AuditLogsDS();
                List<string> portfolioIdList = new List<string>();

                if (message.PortfolioId==null || message.PortfolioId == "")
                {
                    HealthCheckDetailsDS healthCheckDetailsDS = new HealthCheckDetailsDS();
                    var healthCheckDetailsList = healthCheckDetailsDS.GetAll(new healthcheck_details() { ConfigId = message.ConfigId }).ToList();

                    healthCheckDetailsList.ForEach(h => portfolioIdList.Add(h.ResourceId));                    
                }
                else
                {
                    portfolioIdList.Add(message.PortfolioId);
                }
                //al.PortfolioId == "01_8"
                obsList = (from al in auditLogsDS.GetAll()
                           join obs in observationsDS.GetAll() on new { A = al.ObservableID, B = al.ResourceID, C = al.IncidentId, D = al.PortfolioId, E = al.TransactionId, F = al.TenantID } equals new { A = obs.ObservableId, B = obs.ResourceId, C = obs.IncidentId, D = obs.PortfolioId, E = obs.TransactionId, F = obs.TenantId } into gj
                           from subObs in gj.DefaultIfEmpty()
                           where al.TransactionId == message.TransactionId && portfolioIdList.Contains(al.PortfolioId) && al.TenantID == message.TenantId
                           select new observations
                           {
                               ResourceId = al.ResourceID,
                               //ObservableId = subObs?.ObservableId ?? 0,
                               ObservableId = al.ObservableID,
                               ObservableName = subObs?.ObservableName ?? String.Empty,
                               State = subObs?.State ?? "NA",
                               Value = subObs?.Value ?? "NA",
                               ObservationTime = subObs?.ObservationTime ?? DateTime.MinValue                               
                           }).ToList();

            }
            else
            {
                var obsTable = (from obs in observationsDS.GetAll()
                                where obs.TenantId == message.TenantId
                                && obs.PlatformId == message.PlatformId
                                && (CheckCondition(obs.ConfigId, message.ConfigId))
                                //&& (CheckCondition(obs.PlatformId.ToString(), platformId))
                                && (CheckCondition(obs.PortfolioId, message.PortfolioId))
                                group obs by new { obs.ResourceId, obs.ObservableId } into g
                                select g).ToList();

                foreach (var grp in obsTable)
                {
                    var obj = (from g in grp
                               orderby g.ObservationTime descending
                               select g).FirstOrDefault();
                    obsList.Add(obj);
                }

                
            }
            retList = (from obs in obsList
                       join r in resourceDS.GetAll()
                       on obs.ResourceId equals r.ResourceId
                       join rt in resourceTypeDS.GetAll()
                       on r.ResourceTypeId equals rt.ResourceTypeId
                       join o in observableDS.GetAll()
                       on obs.ObservableId equals o.ObservableId
                       select new BE.NotificationSummaryDetails
                       {
                           ResourceId = r.ResourceId,
                           ResourceName = r.ResourceName,
                           ResourceTypeId = rt.ResourceTypeId.ToString(),
                           ResourceTypeName = rt.ResourceTypeName,
                           ObservableId = o.ObservableId.ToString(),
                           ObservableName = o.ObservableName,
                           Value = obs.Value,
                           Status = obs.State,
                           ObservationTime = Convert.ToDateTime(obs.ObservationTime)
                       }).ToList();

            return retList;
        }
        public static bool CheckCondition(string obsVar, string inputVar)
        {
            if (obsVar != null && obsVar != "")
            {
                if (inputVar != null && inputVar != "")
                {
                    if (obsVar.Equals(inputVar, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                    else
                        return false;
                }
                else
                    return true;
            }
            else
                return false;


        }

        public static string GetPlatformName(int platformId)
        {
            return new PlatformsDS().GetOne(new platforms() { PlatformId = platformId }).PlatformName;
        }
        public static string GetTenantName(int tenantId)
        {
            return new TenantDS().GetOne(new tenant() { TenantId = tenantId }).Name;
        }

        public string GetCongfigName(string configId)
        {
            return new HealthcheckMasterDS().GetOne(new healthcheck_master() { ConfigId = configId }).ConfigurationName;
        }
        public string GetResourceName(string resId)
        {
            return new ResourceDS().GetOne(new resource() { ResourceId = resId }).ResourceName;
        }

        public List<string> GetConsolidatedEnvironmentScanReport(int platformId, int tenantId,out string platformName, out string tenantName)
        {
            List<string> docPath = new List<string>();
            try
            {
                EnvironmentScanMetricAnalyser esma = new EnvironmentScanMetricAnalyser();
                EnvironmentScanMetricDS environmentScanMetricDS = new EnvironmentScanMetricDS();
                
                platformName = GetPlatformName(platformId);
                tenantName = GetTenantName(tenantId);
                string InstalledSoftwareFileName = string.Format("InstalledSoftware_{0}_{1:yyyy-MM-dd_hh-mm-ss-tt}.csv", platformName, DateTime.UtcNow);
                string osDetailsFileName = string.Format("OSDetails_{0}_{1:yyyy-MM-dd_hh-mm-ss-tt}.csv", platformName, DateTime.UtcNow);
                string SRFileName = string.Format("ScreenResolution_{0}_{1:yyyy-MM-dd_hh-mm-ss-tt}.csv", platformName, DateTime.UtcNow);

                StringBuilder installedSoftware = new StringBuilder("Resource,Resource Type,Software Name,Old Version,Old Installed Date,Old Publisher,New Version,New Installed Date,New Publisher,Status\n");
                StringBuilder osDetails = new StringBuilder("Resource,Resource Type,Name,Old Version,Old Build Number,New Version,New Build Number,Status\n");
                StringBuilder screenResolution = new StringBuilder("Resource,Resource Type,Old Resolution,New Resolution,Status\n");

                var groupedResult = (from esm in environmentScanMetricDS.GetAll()
                                     where esm.PlatformID == platformId
                                     && esm.TenantID == tenantId
                                     group esm by esm.ResourceID into g
                                     select g).ToList();

                //StringBuilder doc = new StringBuilder(String.Format("This is a consolidated report for Platform : {0} of Tenant : {1}",NotificationBuilder.GetPlatformName(platformId), NotificationBuilder.GetTenantName(tenantId)));

                foreach (var grp in groupedResult)
                {
                    var orderedList = grp.OrderByDescending(g => g.Version).ToList();

                    BE.EnvironmentScanComparisonReport reportDoc = esma.GetEnvironmentScanComparisonReport(orderedList[0].ResourceID, orderedList[0].GeneratedDate.ToString(), orderedList[1].GeneratedDate.ToString(), orderedList[0].PlatformID, orderedList[0].TenantID);

                    //var groupedObj = reportDoc.ResourceDetails.MetricDetails.MetricValue.GroupBy(m => new { m.MetricName, m.MetricKey });

                    foreach (var mvObj in reportDoc.ResourceDetails.MetricDetails.MetricValue)
                    {
                        switch (mvObj.MetricName.ToLower().RemoveSpaces())
                        {
                            case "installedsoftware":
                                installedSoftware.AppendLine(InstalledSoftwareRowCreation(mvObj, reportDoc.ResourceDetails.ResourceName, reportDoc.ResourceDetails.ResourceTypeName));
                                break;
                            case "osdetails":
                                osDetails.AppendLine(OsDetailsRowCreation(mvObj, reportDoc.ResourceDetails.ResourceName, reportDoc.ResourceDetails.ResourceTypeName));
                                break;
                            case "screenresolution":
                                screenResolution.AppendLine(SRRowCreation(mvObj, reportDoc.ResourceDetails.ResourceName, reportDoc.ResourceDetails.ResourceTypeName));
                                break;
                        }
                    }                   
                    
                }
                InstalledSoftwareFileName = WriteToCSVFile(InstalledSoftwareFileName, installedSoftware.ToString());
                osDetailsFileName = WriteToCSVFile(osDetailsFileName, osDetails.ToString());
                SRFileName = WriteToCSVFile(SRFileName, screenResolution.ToString());

                docPath.Add(InstalledSoftwareFileName);
                docPath.Add(osDetailsFileName);
                docPath.Add(SRFileName);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(string.Format("Error in GetConsolidatedEnvironmentScanReport method of NotificationBuilder class. The error is : {0} ; inner exception : {1}. stack trace : {2}", ex.Message, ex.InnerException.Message, ex.StackTrace),LogHandler.Layer.Business,null);
                throw ex;
            }
            return docPath;

        }

        private string InstalledSoftwareRowCreation(BE.ESMetricValues mvObj, string resName, string resTypeName)
        {
            string finalString = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}";
            string softwareName = string.Empty;
            string oldVersion = string.Empty;
            string newVersion = string.Empty;
            string oldInstalledDate = string.Empty;
            string newInstalledDate = string.Empty;
            string oldPublisher = string.Empty;
            string newPublisher = string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (var attObj in mvObj.Attributes)
            {
                switch (attObj.AttributeName.ToLower().RemoveSpaces())
                {
                    case "displayname":
                        softwareName = attObj.AttributeValue;
                        break;
                    case "oldversion":
                        oldVersion = attObj.AttributeValue;
                        break;
                    case "oldinstalldate":
                        oldInstalledDate = attObj.AttributeValue;
                        break;
                    case "oldpublisher":
                        oldPublisher = attObj.AttributeValue;
                        break;
                    case "version":
                        newVersion = attObj.AttributeValue;
                        break;
                    case "installdate":
                        newInstalledDate = attObj.AttributeValue;
                        break;
                    case "publisher":
                        newPublisher = attObj.AttributeValue;
                        break;
                    default:
                        break;

                }
            }
            finalString = String.Format(finalString, resName, resTypeName, softwareName, oldVersion, oldInstalledDate, oldPublisher, newVersion, newInstalledDate, newPublisher, mvObj.Status);
            return finalString;
        }

        private string OsDetailsRowCreation(BE.ESMetricValues mvObj, string resName, string resTypeName)
        {
            string finalString = "{0},{1},{2},{3},{4},{5},{6},{7}";
            string Name = string.Empty;
            string oldVersion = string.Empty;
            string newVersion = string.Empty;
            string oldBuildNumber = string.Empty;
            string newBuildNumber = string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (var attObj in mvObj.Attributes)
            {
                switch (attObj.AttributeName.ToLower().RemoveSpaces())
                {
                    case "caption":
                        Name = attObj.AttributeValue;
                        break;
                    case "version":
                        newVersion = attObj.AttributeValue;
                        break;
                    case "oldversion":
                        oldVersion = attObj.AttributeValue;
                        break;
                    case "buildnumber":
                        newBuildNumber = attObj.AttributeValue;
                        break;
                    case "oldbuildnumber":
                        oldBuildNumber = attObj.AttributeValue;
                        break;
                    default:
                        break;

                }
            }
            finalString = String.Format(finalString, resName, resTypeName, Name, oldVersion, oldBuildNumber, newVersion, newBuildNumber, mvObj.Status);
            return finalString;
        }

        private string SRRowCreation(BE.ESMetricValues mvObj, string resName, string resTypeName)
        {
            string finalString = "{0},{1},{2},{3},{4}";
            string oldHeight = string.Empty;
            string newHeight = string.Empty;
            string oldWidth = string.Empty;
            string newWidth = string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (var attObj in mvObj.Attributes)
            {
                switch (attObj.AttributeName.ToLower().RemoveSpaces())
                {
                    case "pelsheight":
                        newHeight = attObj.AttributeValue;
                        break;
                    case "oldpelsheight":
                        oldHeight = attObj.AttributeValue;
                        break;
                    case "PelsWidth":
                        newWidth = attObj.AttributeValue;
                        break;
                    case "oldpelswidth":
                        oldWidth = attObj.AttributeValue;
                        break;
                    default:
                        break;

                }
            }
            finalString = String.Format(finalString, resName, resTypeName, oldHeight + "*" + oldWidth, newHeight + "*" + newWidth, mvObj.Status);
            return finalString;
        }

        private string WriteToCSVFile(string fileName, string fileContent)
        {
            string filePath = AppDomain.CurrentDomain.BaseDirectory + fileName;
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            File.WriteAllText(filePath, fileContent);
            return filePath;
        }
    }
}
