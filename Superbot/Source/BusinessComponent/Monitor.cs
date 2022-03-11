/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using Infosys.Solutions.Superbot.Infrastructure.Common;
using DE = Infosys.Solutions.Superbot.Resource.Entity;
using IE = Infosys.Solutions.Ainauto.Services.Superbot.Contracts.Data;
using BE = Infosys.Solutions.Ainauto.BusinessEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infosys.Solutions.Ainauto.Resource.DataAccess;
using Infosys.Solutions.Ainauto.Resource.DataAccess.CustomDataEntity;
using System.Net.Http;
using System.Net.Http.Headers;
//using Newtonsoft.Json;
using System.Configuration;
using Infosys.Solutions.Ainauto.Services.Superbot.Contracts.Message;
using Infosys.Solutions.Ainauto.Resource.DataAccess.Queue;
using Infosys.Solutions.Ainauto.BusinessEntity;
using Infosys.Solutions.Ainauto.Superbot.Infrastructure.ServiceClientLibrary;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Reflection;
using Infosys.Solutions.Superbot.Resource.Entity.Queue;
using System.Xml.Linq;
using ES = Infosys.Solutions.Ainauto.Resource.DataAccess.ElasticSearch;
using Infosys.Solutions.Superbot.Infrastructure.Common.PerfMon;

namespace Infosys.Solutions.Ainauto.Superbot.BusinessComponent
{
    public class Monitor
    {
        public BusinessEntity.DeviceList deviceList = null;
        public static int tenantid = 0;
        public int HealthcheckInitiated(int platformId, int platformResourceModelVersion, string healthcheckSource, string resourceType)
        {
            int HealtcheckTrackingId = 0;
            try
            {
                DE.healthcheck_iteration_tracker healthcheckTracker = new DE.healthcheck_iteration_tracker();

                healthcheckTracker = Translator.HealthCheck_BE_DE.HealthCheckTrackerBEtoDE(platformId, platformResourceModelVersion, healthcheckSource, resourceType);
                healthcheckTracker.TenantId = tenantid;

                HealthcheckIterationTrackerDS healthcheckTrackerDS = new HealthcheckIterationTrackerDS();
                healthcheckTracker = healthcheckTrackerDS.Insert(healthcheckTracker);

                if (healthcheckTracker != null)
                {
                    HealtcheckTrackingId = healthcheckTracker.TrackingId;
                }
            }
            catch (Exception superBotException)
            {
                Console.WriteLine("Exception Occured in HealthcheckInitiated " + superBotException.Message);
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(superBotException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }

            }
            return HealtcheckTrackingId;
        }

        public LatestPlatform GetRecentPlatformDetails(string RPAType, string resourceType)
        {
            try
            {
                ResourceDS rDS = new ResourceDS();
                ResourceTypeDS tDS = new ResourceTypeDS();
                LatestPlatform platform = (from r in rDS.GetAny().ToArray()
                                           join t in tDS.GetAny().ToArray() on r.ResourceTypeId equals t.ResourceTypeId
                                           where t.PlatfromType.ToLower() == RPAType.ToLower()
                                           && t.ResourceTypeName.ToLower() == resourceType.ToLower()
                                           select new LatestPlatform
                                           {
                                               platformID = Convert.ToString(r.PlatformId),
                                               resouceID = r.ResourceId
                                           }).OrderByDescending(r => r.platformID).FirstOrDefault();
                return platform;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool CheckThresholdBreach(BE.Metric message)
        {
            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_Start, "CheckThresholdBreach", "Monitor"), LogHandler.Layer.Business, null);
            LogHandler.LogDebug(String.Format("The CheckThresholdBreach method of Monitor class is getting executed with input: Metric message={0}", message),
                LogHandler.Layer.Business, null);
            bool raiseAnamoly = true;
            string criteria = string.Empty;
            string state = string.Empty;
            try
            {
                ResourceDependencyMapDS resourceDependencyMapDS = new ResourceDependencyMapDS();
                // get observable id
                DE.observable observable = new DE.observable();
                observable.ObservableName = message.MetricName;

                LogHandler.LogDebug(String.Format("Calling GetOne method of ObservableDS with parameters: {0}", observable),
                    LogHandler.Layer.Business, null);
                ObservableDS observableDS = new ObservableDS();
                var resultObservableDS = observableDS.GetOne(observable);

                if (resultObservableDS == null)
                {
                    LogHandler.LogError(String.Format(ErrorMessages.RemediatioPlan_NotFound, "observable details", "observable", "ObservableName:" + message.MetricName),
                                        LogHandler.Layer.Business, null);
                    SuperbotDataItemNotFoundException exception = new SuperbotDataItemNotFoundException(String.Format(ErrorMessages.RemediatioPlan_NotFound, "observable details", "observable", "ObservableName:" + message.MetricName));
                    List<ValidationError> validationErrors_List = new List<ValidationError>();
                    ValidationError validationErr = new ValidationError();
                    validationErr.Code = "1042";
                    validationErr.Description = string.Format(ErrorMessages.RemediatioPlan_NotFound, "observable details", "observable", "ObservableName:" + message.MetricName);
                    validationErrors_List.Add(validationErr);

                    if (validationErrors_List.Count > 0)
                    {
                        exception.Data.Add("DataNotFoundErrors", validationErrors_List);
                        throw exception;
                    }
                }
                var unitOfMeasure = resultObservableDS.UnitOfMeasure.Trim();
                var observableId = resultObservableDS.ObservableId;
                var dataType = resultObservableDS.DataType.ToLower().Trim();
                // get thresholds
                DE.observable_resource_map observableResourceMap = new DE.observable_resource_map();
                observableResourceMap.ResourceId = message.ResourceId;
                observableResourceMap.ObservableId = observableId;
                LogHandler.LogDebug(String.Format("Calling GetOne method of ObservableResourceMapDS with parameters: ResourceId= {0}; ObservableId={1}", message.ResourceId, observableId),
                    LogHandler.Layer.Business, null);
                ObservableResourceMapDS observableResourceMapDS = new ObservableResourceMapDS();
                var resultObservableResourceMapDS = observableResourceMapDS.GetOne(observableResourceMap);

                if (resultObservableResourceMapDS == null)
                {
                    LogHandler.LogError(String.Format(ErrorMessages.RemediatioPlan_NotFound, "observable resource map details", "observable_resource_map", "ResourceId= " + message.ResourceId + "; ObservableId=" + observableId),
                                        LogHandler.Layer.Business, null);
                    SuperbotDataItemNotFoundException exception = new SuperbotDataItemNotFoundException(String.Format(ErrorMessages.RemediatioPlan_NotFound, "observable resource map details", "observable_resource_map", "ResourceId= " + message.ResourceId + "; ObservableId=" + observableId));
                    List<ValidationError> validationErrors_List = new List<ValidationError>();
                    ValidationError validationErr = new ValidationError();
                    validationErr.Code = "1042";
                    validationErr.Description = string.Format(ErrorMessages.RemediatioPlan_NotFound, "observable resource map details", "observable_resource_map", "ResourceId= " + message.ResourceId + "; ObservableId=" + observableId);
                    validationErrors_List.Add(validationErr);

                    if (validationErrors_List.Count > 0)
                    {
                        exception.Data.Add("DataNotFoundErrors", validationErrors_List);
                        throw exception;
                    }
                }
                var operation = Convert.ToInt32(resultObservableResourceMapDS.OperatorId);

                LogHandler.LogDebug(String.Format("Fetching data from operator table for operatorID={0}", operation),
                    LogHandler.Layer.Business, null);

                DE.@operator operatorObj = new DE.@operator();
                operatorObj.OperatorId = operation;
                OperatorDS operatorDS = new OperatorDS();
                operatorObj = operatorDS.GetOne(operatorObj);

                Validator validator = new Validator();
                // compare thresholds with queue message
                // operations : 1 = : 2 > : 3 < : 4 between : 5 not equal
                // observable : 1,2,5 varchar : 3,4,6 int
                switch (dataType)
                {
                    case "varchar":
                        criteria = validator.CheckCriteriaVarchar(message, resultObservableResourceMapDS, operatorObj, unitOfMeasure, out state);
                        break;
                    case "int":
                        criteria = validator.CheckCriteriaInt(message, resultObservableResourceMapDS, operatorObj, unitOfMeasure, out state);
                        break;
                    case "datetime":
                        criteria = validator.CheckCriteriaDateTime(message, resultObservableResourceMapDS, operatorObj, unitOfMeasure, out state);
                        break;
                    default:
                        LogHandler.LogWarning(String.Format(ErrorMessages.RemediatioPlan_NotFound, "DataType", "observable", "ObservableName=" + message.MetricName),
                            LogHandler.Layer.Business, null);
                        break;
                }

                // get resource type id
                DE.resource resource = new DE.resource();
                resource.ResourceId = message.ResourceId;

                LogHandler.LogDebug(String.Format("Fetching data from Resource table for ResourceId:{0}", message.ResourceId), LogHandler.Layer.Business, null);
                ResourceDS resourceDS = new ResourceDS();
                var resultResourceDS = resourceDS.GetOne(resource);
                if (resultResourceDS == null)
                {
                    LogHandler.LogError(String.Format(ErrorMessages.RemediatioPlan_NotFound, "resource details", "resource", "ResourceId= " + message.ResourceId),
                                   LogHandler.Layer.Business, null);
                    SuperbotDataItemNotFoundException exception = new SuperbotDataItemNotFoundException(String.Format(ErrorMessages.RemediatioPlan_NotFound, "observable resource map details", "observable_resource_map", "ResourceId= " + message.ResourceId + "; ObservableId=" + observableId));
                    List<ValidationError> validationErrors_List = new List<ValidationError>();
                    ValidationError validationErr = new ValidationError();
                    validationErr.Code = "1042";
                    validationErr.Description = string.Format(ErrorMessages.RemediatioPlan_NotFound, "observable resource map details", "observable_resource_map", "ResourceId= " + message.ResourceId + "; ObservableId=" + observableId);
                    validationErrors_List.Add(validationErr);

                    if (validationErrors_List.Count > 0)
                    {
                        exception.Data.Add("DataNotFoundErrors", validationErrors_List);
                        throw exception;
                    }
                }

                var resourceTypeId = resultResourceDS.ResourceTypeId;
                int platformId = Convert.ToInt32(resultResourceDS.PlatformId);


                //checking the last entry for failure
                LogHandler.LogDebug(String.Format("Fetching last entry from observations table for the ResourceId={0}; ResourceTypeId={1}; ObservableId={2}.", message.ResourceId, resourceTypeId, observableId),
                    LogHandler.Layer.Business, null);

                //DE.observations observations_last = new DE.observations();
                //ObservationsDS observationsDS = new ObservationsDS();                
                //anomaly_Details = (from o in anomalyDetailsDS.GetAll()
                //                     where o.PlatformId == platformId &&
                //                     o.ResourceId == message.ResourceId &&
                //                     o.ResourceTypeId == resourceTypeId &&
                //                     o.ObservableId == observableId &&
                //                     o.TenanatId == resultResourceDS.TenantId
                //                     orderby o.AnomalyId descending
                //                     select o).FirstOrDefault();
                DE.anomaly_details anomaly_Details = new DE.anomaly_details();
                AnomalyDetailsDS anomalyDetailsDS = new AnomalyDetailsDS();
                anomaly_Details = (from o in anomalyDetailsDS.GetAll()
                                   where o.PlatformId == platformId &&
                                   o.ResourceId == message.ResourceId &&
                                   o.ResourceTypeId == resourceTypeId &&
                                   o.ObservableId == observableId &&
                                    o.TenantId == resultResourceDS.TenantId
                                   orderby o.AnomalyId descending
                                   select o).FirstOrDefault();
                if (criteria != "")
                {
                    LogHandler.LogDebug(String.Format(criteria),
                        LogHandler.Layer.Business, null);
                    //get resource hierarchy here
                    List<string> resourceIdList = Helper.GetResourceHierarchy(message.ResourceId, resultResourceDS.TenantId);
                    //List<string> resourceIdList = new List<string>();
                    //string resourceId = message.ResourceId;
                    //var resDependencyTable = (from resMap in resourceDependencyMapDS.GetAny() select resMap).ToList();
                    //while (true)
                    //{
                    //    var res = (from resMap in resDependencyTable
                    //               where resMap.ResourceId == resourceId
                    //               select new { resMap.ResourceId, resMap.DependencyResourceId }).First();
                    //    resourceIdList.Add(res.ResourceId);
                    //    if (res.DependencyResourceId.Trim() == "")
                    //    {
                    //        break;
                    //    }
                    //    else
                    //    {
                    //        resourceId = res.DependencyResourceId;
                    //    }
                    //}



                    LogHandler.LogDebug(String.Format("Fetching data from platforms table for platformID:{0}", platformId), LogHandler.Layer.Business, null);

                    DE.platforms platforms = new DE.platforms();
                    platforms.PlatformId = platformId;
                    PlatformsDS platformsDS = new PlatformsDS();
                    platforms = platformsDS.GetOne(platforms);
                    if (anomaly_Details != null)
                    {
                        if (anomaly_Details.RemediationStatus.Equals("Not Started", StringComparison.InvariantCultureIgnoreCase) || anomaly_Details.RemediationStatus.Equals("In Progress", StringComparison.InvariantCultureIgnoreCase))
                        {
                            int minutes = 0;
                            int remediationActionThreshold = 0;
                            ResourceAttributesDS resourceAttributesDS = new ResourceAttributesDS();
                            var resourceAttributTable = (from resAtt in resourceAttributesDS.GetAny() select resAtt).ToList();
                            foreach (var resId in resourceIdList)
                            {
                                var res = (from resAtt in resourceAttributTable
                                           where resAtt.ResourceId == resId
                                           && resAtt.AttributeName == "remediationactionthreshold"
                                           select resAtt.AttributeValue).FirstOrDefault();
                                if (res != "" && res != null)
                                {
                                    remediationActionThreshold = Convert.ToInt32(res);
                                    break;
                                }
                            }
                            if (anomaly_Details.ObservationTime != null)
                                minutes = (int)DateTime.UtcNow.Subtract((DateTime)anomaly_Details.ObservationTime).TotalMinutes;

                            raiseAnamoly = false;
                            anomaly_Details.State = "Unhealthy";
                            anomaly_Details.ModifiedDate = DateTime.UtcNow;
                            anomaly_Details.ModifiedBy = "admin@123";
                            if (anomalyDetailsDS.Update(anomaly_Details) == null)
                            {
                                LogHandler.LogWarning(String.Format(ErrorMessages.ValueUpdationUnsuccessful, "observations"), LogHandler.Layer.Business, null);
                            }


                            if (minutes >= remediationActionThreshold)
                            {
                                if (Helper.CheckNotificationRestriction(anomaly_Details.ResourceId, anomaly_Details.ObservableId, anomaly_Details.PlatformId, anomaly_Details.TenantId))
                                {
                                    LogHandler.LogDebug(String.Format("Creating a Notification message to send to the queue as the time exceeded the RemediationActionThreshold"),
                                   LogHandler.Layer.Business, null);
                                    Notification notification = new Notification();

                                    notification.ObservationId = anomaly_Details.AnomalyId;
                                    notification.PlatformId = anomaly_Details.PlatformId;
                                    notification.ResourceId = anomaly_Details.ResourceId;
                                    notification.ResourceTypeId = anomaly_Details.ResourceTypeId;
                                    notification.ObservableId = anomaly_Details.ObservableId;
                                    notification.ObservableName = anomaly_Details.ObservableName;
                                    notification.ObservationStatus = "Failed";
                                    notification.Value = Convert.ToString(anomaly_Details.Value);
                                    notification.ThresholdExpression = "Anomaly detected in resource_type:" + anomaly_Details.ResourceTypeId + "-resource:" + anomaly_Details.ResourceId + " deployed on:" + anomaly_Details.SourceIp + " for the monitored_metric:" + anomaly_Details.ObservableName + ". Threshold breach rule:" + anomaly_Details.Description;
                                    notification.ServerIp = anomaly_Details.SourceIp;
                                    notification.ObservationTime = anomaly_Details.ObservationTime.ToString();
                                    notification.EventType = "Anomaly Notifcation";
                                    notification.Source = "Platform";
                                    notification.TenantId = anomaly_Details.TenantId;
                                    notification.Type = (int)NotificationType.ThresholdBreach;
                                    notification.Channel = (int)NotificationChannel.Email;
                                    notification.BaseURL = "";
                                    if (string.IsNullOrEmpty(notification.Description))
                                    { notification.Description = "The Time exceeded the RemediationActionThreshold"; }

                                    NotificationDS not = new NotificationDS();
                                    not.Send(notification, "");
                                    LogHandler.LogDebug(String.Format("The Notification message has been sent to the queue"),
                                        LogHandler.Layer.Business, null);
                                    //Setting notified time
                                    anomaly_Details.IsNotified = "Yes";
                                    anomaly_Details.NotifiedTime = DateTime.UtcNow;
                                    anomaly_Details.ModifiedDate = DateTime.UtcNow;
                                    anomaly_Details.ModifiedBy = "admin@123";
                                    if (anomalyDetailsDS.Update(anomaly_Details) == null)
                                    {
                                        LogHandler.LogWarning(String.Format(ErrorMessages.ValueUpdationUnsuccessful, "observations"), LogHandler.Layer.Business, null);
                                    }
                                }
                            }

                        }
                        else if (anomaly_Details.RemediationStatus.Equals("FAILED", StringComparison.InvariantCultureIgnoreCase))
                        {
                            int minutes = 0;
                            int notificationTimeThreshold = 0;
                            ResourceAttributesDS resourceAttributesDS = new ResourceAttributesDS();
                            var resourceAttributTable = (from resAtt in resourceAttributesDS.GetAny() select resAtt).ToList();
                            foreach (var resId in resourceIdList)
                            {
                                var res = (from resAtt in resourceAttributTable
                                           where resAtt.ResourceId == resId
                                           && resAtt.AttributeName == "notificationtimethreshold"
                                           select resAtt.AttributeValue).FirstOrDefault();
                                if (res != "" && res != null)
                                {
                                    notificationTimeThreshold = Convert.ToInt32(res);
                                    break;
                                }
                            }
                            if (anomaly_Details.NotifiedTime != null)
                                minutes = (int)DateTime.UtcNow.Subtract((DateTime)anomaly_Details.NotifiedTime).TotalMinutes;
                            if (anomaly_Details.IsNotified.Equals("Yes", StringComparison.InvariantCultureIgnoreCase))
                            {

                                if (minutes < notificationTimeThreshold)
                                {
                                    raiseAnamoly = false;
                                    anomaly_Details.State = "Unhealthy";
                                    anomaly_Details.ModifiedDate = DateTime.UtcNow;
                                    anomaly_Details.ModifiedBy = "admin@123";
                                    if (anomalyDetailsDS.Update(anomaly_Details) == null)
                                    {
                                        LogHandler.LogWarning(String.Format(ErrorMessages.ValueUpdationUnsuccessful, "observations"), LogHandler.Layer.Business, null);
                                    }
                                }
                                else
                                {
                                    raiseAnamoly = true;
                                }
                            }
                            else
                            {
                                raiseAnamoly = true;
                            }


                        }

                    }

                    if (raiseAnamoly)
                    {
                        // insert anomaly
                        LogHandler.LogDebug(String.Format("Populating Properties for observations and inserting into observations table"),
                            LogHandler.Layer.Business, null);

                        DE.anomaly_details anomalyDetailsObj = new DE.anomaly_details();

                        //observations.ObservationId = 19;
                        anomalyDetailsObj.PlatformId = platformId;
                        anomalyDetailsObj.ResourceId = message.ResourceId;
                        anomalyDetailsObj.ResourceTypeId = resourceTypeId;
                        anomalyDetailsObj.ObservableId = observableId;
                        anomalyDetailsObj.ObservableName = message.MetricName;
                        anomalyDetailsObj.ObservationStatus = "Failed";
                        anomalyDetailsObj.Value = message.MetricValue;
                        anomalyDetailsObj.ObservationTime = Convert.ToDateTime(DateTime.UtcNow);
                        anomalyDetailsObj.SourceIp = message.ServerIp??"NA";
                        anomalyDetailsObj.Description = "Anomaly detected in resource_type:" + resourceTypeId + "-resource:" + message.ResourceId + " deployed on:" + message.ServerIp + " for the monitored_metric:" + message.EventType + ". Threshold breach rule:" + criteria;

                        if (message.Description != null && message.Description != "" && message.Description != "NA")
                            anomalyDetailsObj.Description += "Reference Details: " + message.Description;


                        anomalyDetailsObj.RemediationStatus = "Not Started";
                        anomalyDetailsObj.EventType = "Anomaly Remediation";
                        anomalyDetailsObj.Source = "Platform";
                        anomalyDetailsObj.TenantId = resultObservableResourceMapDS.TenantId;
                        anomalyDetailsObj.CreatedBy = "admin@123";
                        anomalyDetailsObj.CreateDate = Convert.ToDateTime(DateTime.UtcNow);
                        anomalyDetailsObj.ModifiedDate = DateTime.UtcNow;
                        anomalyDetailsObj.ModifiedBy = "admin@123";
                        anomalyDetailsObj.State = "Unhealthy";

                        //LogHandler.LogDebug(string.Format($"The anoamly object to be inserted : {JsonConvert.SerializeObject(anomalyDetailsObj)}"),LogHandler.Layer.Business,null);

                        if (anomalyDetailsDS.Insert(anomalyDetailsObj) == null)
                        {
                            LogHandler.LogWarning(String.Format(ErrorMessages.ValueUpdationUnsuccessful, "observations"),
                                LogHandler.Layer.Business, null);
                        }
                    }

                }
                else
                {

                    if (anomaly_Details != null && anomaly_Details.RemediationStatus.Equals("FAILED", StringComparison.InvariantCultureIgnoreCase))
                    {
                        LogHandler.LogDebug(String.Format("The RemediationStatus is FAILED.So, Updating the state as Recoverd-Healthy(External)."),
                        LogHandler.Layer.Business, null);
                        anomaly_Details.State = "Recovered-Healthy(External)";
                        anomaly_Details.RemediationStatus = "Success";
                        anomaly_Details.ModifiedDate = DateTime.UtcNow;
                        anomaly_Details.ModifiedBy = "admin@123";
                        if (anomalyDetailsDS.Update(anomaly_Details) == null)
                        {
                            LogHandler.LogWarning(String.Format(ErrorMessages.ValueUpdationUnsuccessful, "observations"), LogHandler.Layer.Business, null);
                        }

                    }

                }

                //getting portfolioid from db                
                string portfolioId = String.Empty;

                if (message.PortfolioId == null || message.PortfolioId == "")
                {
                    DE.resource_dependency_map resourceDependencyMap = new DE.resource_dependency_map();
                    resourceDependencyMap.ResourceId = message.ResourceId;
                    portfolioId = resourceDependencyMapDS.GetOne(resourceDependencyMap).PortfolioId;
                }
                else
                {
                    portfolioId = message.PortfolioId;
                }

                string portfolioName = resourceDS.GetOne(new DE.resource { ResourceId = portfolioId }).ResourceName;

                //observations data
                ObservationsDS observationsDS = new ObservationsDS();
                DE.observations observationsDE = new DE.observations();
                //populating the observations object to insert
                observationsDE.PlatformId = platformId;
                observationsDE.ResourceId = message.ResourceId;
                observationsDE.ResourceTypeId = resourceTypeId;
                observationsDE.ObservableId = observableId;
                observationsDE.ObservableName = message.MetricName;

                if (state.Equals("Critical", StringComparison.InvariantCultureIgnoreCase))
                    observationsDE.ObservationStatus = "Failed";
                else
                    observationsDE.ObservationStatus = "Success";

                observationsDE.Value = message.MetricValue;
                observationsDE.ObservationTime = Convert.ToDateTime(DateTime.UtcNow);
                observationsDE.SourceIp = message.ServerIp != null ? message.ServerIp : "";
                if (raiseAnamoly)
                    observationsDE.Description = "Anomaly detected in resource_type:" + resourceTypeId + "-resource:" + message.ResourceId + " deployed on:" + message.ServerIp + " for the monitored_metric:" + message.EventType + ". Threshold breach rule:" + criteria;
                else
                    observationsDE.Description = "No Anomaly Found";
                //observationsDE.Description = "Anomaly detected in resource_type:" + resourceTypeId + "-resource:" + message.ResourceId + " deployed on:" + message.ServerIp + " for the monitored_metric:" + message.EventType + ". Threshold breach rule:" + criteria;
                observationsDE.RemediationStatus = "Not Started";
                observationsDE.EventType = "Anomaly Remediation";
                observationsDE.Source = "Platform";
                observationsDE.TenantId = resultObservableResourceMapDS.TenantId;
                observationsDE.CreatedBy = "admin@123";
                observationsDE.CreateDate = Convert.ToDateTime(DateTime.UtcNow);
                observationsDE.ModifiedDate = DateTime.UtcNow;
                observationsDE.ModifiedBy = "admin@123";
                observationsDE.State = state;
                observationsDE.PortfolioId = portfolioId;
                observationsDE.ConfigId = message.ConfigId;
                observationsDE.IncidentId = message.IncidentId;
                observationsDE.Application = message.Application;
                observationsDE.ObservationSequence = message.Count;
                observationsDE.TransactionId = message.TransactionId;

                if (observationsDS.Insert(observationsDE) == null)
                {
                    LogHandler.LogWarning(String.Format(ErrorMessages.ValueUpdationUnsuccessful, "observations"),
                        LogHandler.Layer.Business, null);
                }

                #region Inserting into Elastic Search
                var es = System.Configuration.ConfigurationManager.AppSettings["ESIndexName"];
                if (es != null)
                {


                    var resourceTypeObj = new ResourceTypeDS().GetOne(new DE.resourcetype() { ResourceTypeId = observationsDE.ResourceTypeId });
                    var ormObj = new ObservableResourceMapDS().GetOne(new DE.observable_resource_map() { ObservableId = Convert.ToInt32(message.ObservableId), ResourceId = message.ResourceId });

                    ES.Model.ElasticSearchInput objES = new ES.Model.ElasticSearchInput();


                    objES.ConfigId = message.ConfigId;
                    objES.PortfolioId = observationsDE.PortfolioId;
                    objES.PortfolioName = portfolioName;
                    objES.ResourceId = observationsDE.ResourceId;
                    objES.ResourceName = resultResourceDS.ResourceName;
                    objES.ObservabeId = observationsDE.ObservableId.ToString();
                    objES.ObservableName = observationsDE.ObservableName;
                    objES.Count = message.Count.ToString();
                    objES.MetricValue = message.MetricValue;
                    objES.MetricValueinNumeric = checkMetricValueNumeric(message.MetricValue, state);
                    objES.MetricTime = message.MetricTime.ConvertToDate();
                    objES.MetricTimeString = message.MetricTime.ToString();
                    objES.ResourceTypeId = observationsDE.ResourceTypeId.ToString();
                    objES.ResourceTypeName = resourceTypeObj.ResourceTypeName;
                    objES.IncidentId = message.IncidentId;
                    objES.IncidentCreateTime = message.IncidentTime.ConvertToDate();
                    objES.ServerState = state;
                    objES.IsCritical = state.Equals("Critical", StringComparison.InvariantCultureIgnoreCase) ? 1 : 0;
                    objES.IsWarning = state.Equals("Warning", StringComparison.InvariantCultureIgnoreCase) ? 1 : 0;
                    objES.IsHealthy = state.Equals("Healthy", StringComparison.InvariantCultureIgnoreCase) ? 1 : 0;
                    objES.LowerThreshold = ormObj.LowerThreshold;
                    objES.UpperThreshold = ormObj.UpperThreshold;
                    objES.LowerThresholdinNumeric = convertThrsholdValue(ormObj.LowerThreshold);
                    objES.UpperThresholdinNumeric = convertThrsholdValue(ormObj.UpperThreshold);

                    ES.ElasticSearch elasticSearch = new ES.ElasticSearch();
                    elasticSearch.Insert(objES, System.Configuration.ConfigurationManager.AppSettings["ESIndexName"]); //NOTE : INDEX NAME MUST ME LOWERCASE
                }
                #endregion

            }
            catch (Exception superBotException)
            {
                //logging to Performance metric - # of message errors detected
                PerfMonLogger.LogIncrementValue(PerfMonLogger.SuperBotCategories.SuperBot, PerfMonLogger.SuperBotCounters.NumOfErrors);

                LogHandler.LogError("Exception occured at Monitor class CheckThresholdBreach method. Message: {0}, Trace: {1}",LogHandler.Layer.Business, superBotException.Message, superBotException.StackTrace);
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(superBotException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }

            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_End, "CheckThresholdBreach", "Monitor"), LogHandler.Layer.Business, null);
            if (raiseAnamoly && !string.IsNullOrEmpty(criteria))
            {
                return false;
            }
            else
            {
                return true;
            }


        }
        private double convertThrsholdValue(string str)
        {
            double d;

            if (str != null)
            {
                if (Double.TryParse(str, out d))
                    return d;
                else
                    return 0;
            }
            return 0;
        }

        private double checkMetricValueNumeric(string str, string state)
        {
            double d;
            if (Double.TryParse(str, out d))
                return d;
            else
            {
                d = state.Equals("Critical", StringComparison.InvariantCultureIgnoreCase) ? 1 : 0;
            }
            return d;
        }

        public string HealthcheckCompleted(int healtcheckTrackingId, string status, string errorReason)
        {
            string healthcheckStatus = status;
            try
            {
                // check if status value is a valid one 
                if (Enum.IsDefined(typeof(HealthcheckStatus), status.ToLower()))
                {
                    DE.healthcheck_iteration_tracker healthcheckTracker = new DE.healthcheck_iteration_tracker();
                    healthcheckTracker.TrackingId = healtcheckTrackingId;
                    healthcheckTracker.Status = (int)Enum.Parse(typeof(HealthcheckStatus), status.ToLower());
                    healthcheckTracker.EndTime = DateTime.UtcNow;
                    healthcheckTracker.Error = errorReason;

                    HealthcheckIterationTrackerDS healthcheckTrackerDS = new HealthcheckIterationTrackerDS();
                    healthcheckTracker = healthcheckTrackerDS.Update(healthcheckTracker);

                    healthcheckStatus = status;
                }
                else
                    healthcheckStatus = String.Format("Status value - {0} is not valid for healtcheck TrackingId  - {1} ", status, healtcheckTrackingId);

            }
            catch (Exception superBotException)
            {
                Console.WriteLine("Exception Occured in HealthcheckCompleted " + superBotException.Message);
                healthcheckStatus = superBotException.ToString();
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(superBotException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                //if (rethrow)
                //{
                //    throw ex;
                //}
            }

            return healthcheckStatus;
        }


        public int ObservableHealthchecksUpdate(string healtcheckTrackingId, string serverid, string observableid, string scriptexecutedtransactionid, string status, string healthCheckType, string errorReason)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "ObservableHealthchecksUpdate", "Monitor"), LogHandler.Layer.Business, null);
            int ObservableHealthcheckTrackingId = 0;


            DE.healthcheck_iteration_tracker_details healthcheckTrackerDetails = new DE.healthcheck_iteration_tracker_details();
            healthcheckTrackerDetails = Translator.HealthCheck_BE_DE.ObservableHealthCheckBEtoDE(healtcheckTrackingId, serverid, observableid, scriptexecutedtransactionid, status, healthCheckType, errorReason);
            healthcheckTrackerDetails.TenantId = tenantid;

            HealthcheckIterationTrackerDetailsDS healthcheckTrackerDS = new HealthcheckIterationTrackerDetailsDS();
            healthcheckTrackerDetails = healthcheckTrackerDS.Insert(healthcheckTrackerDetails);

            ObservableHealthcheckTrackingId = healthcheckTrackerDetails.TrackingDetailsId;
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "ObservableHealthchecksUpdate", "Monitor"), LogHandler.Layer.Business, null);
            return ObservableHealthcheckTrackingId;
        }

        public int SystemHealthcheck(int PlatformId, int Tenantid, string dependencyResourceID,string resourceIds)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "SystemHealthcheck", "Monitor"), LogHandler.Layer.Business, null);
            string healthcheckType = "System";
            int HealtcheckTrackingId = 0;
            List<Task> taskList = new List<Task>();
            try
            {
                LogHandler.LogDebug(string.Format("Executing system health check method for parameters platformId:{0} and tenantId:{1}", PlatformId, Tenantid), LogHandler.Layer.Business, null);

                using (LogHandler.TraceOperations("Monitor:SystemHealthcheck", LogHandler.Layer.Business, Guid.NewGuid(), null))
                {
                    tenantid = Tenantid;
                    string platfromInstanceId = Convert.ToString(PlatformId);

                    LogHandler.LogDebug(string.Format("Calling API to get AllBotPlatformInstanceDependencies for parameters PlatformId:{0} and tenantId:{1}", PlatformId, Tenantid), LogHandler.Layer.Business, null);

                    Infrastructure.ServiceClientLibrary.SuperBot resourceHandler = new Infrastructure.ServiceClientLibrary.SuperBot();
                    var channel = resourceHandler.ServiceChannel;
                    IE.PlatformInstanceDetails platformInstanceDetails = channel.GetAllBotPlatformInstanceDependencies(Convert.ToString(PlatformId), Convert.ToString(Tenantid), dependencyResourceID);

                    LogHandler.LogDebug(string.Format("Completed execution of API and returned platformInstance details for PlatformId:{0} and tenantId:{1}", PlatformId, Tenantid), LogHandler.Layer.Business, null);

                    int actiontypeID = Convert.ToInt32(ActionTypes.HealthCheckScript);

                    //Calling HealthcheckInitiated to insert data into DB
                    if (platformInstanceDetails != null && platformInstanceDetails.servers.Count > 0)
                    {
                        LogHandler.LogDebug(string.Format("Executing HealthcheckInitiated method to insert platform details into DB and the parameters are PlatformId:{0},PlatformResourceModelVersion:{1},healthcheckSource:{2} & IPaddress:{3}",
                            platformInstanceDetails.PlatformId, platformInstanceDetails.PlatformResourceModelVersion, healthcheckType, platformInstanceDetails.servers[0].IPAddress), LogHandler.Layer.Business, null);
                        HealtcheckTrackingId = HealthcheckInitiated(platformInstanceDetails.PlatformId, platformInstanceDetails.PlatformResourceModelVersion, healthcheckType, platformInstanceDetails.servers[0].IPAddress);

                        LogHandler.LogDebug(string.Format("Execution of HealthcheckInitiated method completed", HealtcheckTrackingId), LogHandler.Layer.Business, null);

                        if (HealtcheckTrackingId > 0)
                        {
                            ResourceDS resDS = new ResourceDS();
                            var resources = resDS.GetAll();

                            LogHandler.LogDebug(string.Format("Data inserted into healthcheck_iteration_tracker table and  HealtcheckTrackingId:{0}", HealtcheckTrackingId), LogHandler.Layer.Business, null);

                            //Traverse through respective platformObservable Nodes
                            if (platformInstanceDetails.platformObservables != null)
                            {
                                string pID = Convert.ToString(platformInstanceDetails.PlatformId);
                                if (!string.IsNullOrEmpty(pID))
                                {
                                    bool isActive = Convert.ToBoolean(resources.Where(r => r.ResourceId == pID).FirstOrDefault() != null ? resources.Where(r => r.ResourceId == pID).FirstOrDefault().IsActive : false);
                                    if (isActive)
                                    {
                                        foreach (IE.PlatformObservable observable in platformInstanceDetails.platformObservables)
                                        {
                                            if (observable.Actions != null && observable.Actions.ActionName == "HealthCheck-Script")
                                            {
                                                string output = JsonConvert.SerializeObject(observable);
                                                IE.SystemHealthCheckObservable sysObservable = JsonConvert.DeserializeObject<IE.SystemHealthCheckObservable>(output);
                                                if (sysObservable.Actions.ExecutionMode == null?true: sysObservable.Actions.ExecutionMode.Equals("SYNC", StringComparison.InvariantCultureIgnoreCase))
                                                {
                                                    Console.WriteLine($"{sysObservable.Actions.ActionId} is a sync operation. Waiting for all the async tasks to complete.");
                                                    Task.WhenAll(taskList.ToArray()).Wait();
                                                }
                                                else
                                                {
                                                    Console.WriteLine($"{sysObservable.Actions.ActionId} is an async operation. Executing");
                                                }
                                                    
                                                taskList.Add(HealthCheckSystemAPI(sysObservable, Tenantid, PlatformId, platfromInstanceId, platformInstanceDetails.VendorName, HealtcheckTrackingId, healthcheckType, "NA", "Platform"));

                                                #region Commented Code
                                                /*
                                                                                        LogHandler.LogDebug(string.Format("Executing SEE for resourceID:{0} & ActionTypeId:{1}", platfromInstanceId, observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);

                                                                                        InitiateExecutionReqMsg executionReqMsg = new InitiateExecutionReqMsg();
                                                                                        IE.ScriptIdentifier script = new IE.ScriptIdentifier();
                                                                                        script.ScriptId = observable.Actions.ScriptId;
                                                                                        script.CategoryId = observable.Actions.CategoryId;
                                                                                        script.CompanyId = Tenantid;
                                                                                        if (observable.Actions.ParameterDetails != null)
                                                                                        {
                                                                                            List<IE.Parameter> parametersSE = new List<IE.Parameter>();
                                                                                            foreach (IE.ObservableParameters parameters in observable.Actions.ParameterDetails)
                                                                                            {
                                                                                                //RemoteServerNames
                                                                                                //RemoteUserName
                                                                                                //RemotePassword
                                                                                                if (string.Equals(parameters.ParamaterName, "RemoteServerNames", StringComparison.InvariantCultureIgnoreCase))
                                                                                                {
                                                                                                    script.RemoteServerNames = parameters.ParameterValue;
                                                                                                }
                                                                                                else if (string.Equals(parameters.ParamaterName, "RemoteUserName", StringComparison.InvariantCultureIgnoreCase))
                                                                                                {
                                                                                                    script.UserName = parameters.ParameterValue;
                                                                                                }
                                                                                                else if (string.Equals(parameters.ParamaterName, "RemotePassword", StringComparison.InvariantCultureIgnoreCase))
                                                                                                {
                                                                                                    script.Password = parameters.ParameterValue;
                                                                                                }
                                                                                                else
                                                                                                {
                                                                                                    IE.Parameter param = new IE.Parameter();
                                                                                                    param.ParameterName = parameters.ParamaterName;
                                                                                                    param.ParameterValue = parameters.ParameterValue;
                                                                                                    parametersSE.Add(param);
                                                                                                }
                                                                                            }
                                                                                            script.Parameters = parametersSE;
                                                                                        }
                                                                                        executionReqMsg.ScriptIdentifier = script;

                                                                                        LogHandler.LogDebug(string.Format("Executing SEE with values scriptId:{0} & categoryId:{1}", observable.Actions.ScriptId, observable.Actions.CategoryId), LogHandler.Layer.Business, null);

                                                                                        Console.WriteLine("Calling SEE for resource ID:" + platfromInstanceId);

                                                                                        ScriptExecute scriptExecute = new ScriptExecute();
                                                                                        //WEMProxy scriptExecute = new WEMProxy();
                                                                                        var channel1 = scriptExecute.ServiceChannel;
                                                                                        InitiateExecutionResMsg response = channel1.InitiateExecution(executionReqMsg);

                                                                                        LogHandler.LogDebug(string.Format("Execution of SEE completed for resourceID:{0} & ActionTypeId:{1}", platfromInstanceId, observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);
                                                                                        if (response != null && response.ScriptResponse != null && response.ScriptResponse.FirstOrDefault().SuccessMessage.ToLower() != null)
                                                                                        {
                                                                                            string[] stringSeparators = new string[] { "\r\n" };
                                                                                            string[] lines = response.ScriptResponse.FirstOrDefault().SuccessMessage.Split(stringSeparators, StringSplitOptions.None);
                                                                                            bool isExists = Array.Exists(lines, E => E.Contains("ishealthy")||E.Contains(observable.Name+"="));
                                                                                            if (isExists)
                                                                                            {
                                                                                                Console.WriteLine("Script execution success for resource ID:" + platfromInstanceId);

                                                                                                LogHandler.LogDebug("Script execution success for resource ID:" + platfromInstanceId, LogHandler.Layer.Business, null);
                                                                                                LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);

                                                                                                int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), platfromInstanceId, observable.ObservableId, response.ScriptResponse.FirstOrDefault().TransactionId, "success", healthcheckType,
                                                                                                    string.IsNullOrEmpty(response.ScriptResponse.FirstOrDefault().ErrorMessage) ? response.ScriptResponse.FirstOrDefault().SuccessMessage : response.ScriptResponse.FirstOrDefault().ErrorMessage);
                                                                                                LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);

                                                                                                DE.Queue.Metric metric = new DE.Queue.Metric();
                                                                                                string successMessage = response.ScriptResponse.FirstOrDefault().SuccessMessage;
                                                                                                string errorMessage = response.ScriptResponse.FirstOrDefault().ErrorMessage;
                                                                                                //metric = PopulateMetricData("Platform", successMessage, platfromInstanceId, observable.Name, errorMessage, healthcheckType);
                                                                                                BE.Metric metricBE = new BE.Metric();
                                                                                                metricBE = PopulateMetricData(platformInstanceDetails.VendorName, "Platform", successMessage, platfromInstanceId, observable.Name, errorMessage, healthcheckType, "NA");
                                                                                                EntityTranslator translator = new EntityTranslator();
                                                                                                metric = translator.MetricBEToDE(metricBE);
                                                                                                metric.Application = platformInstanceDetails.VendorName;

                                                                                                bool isValid = checkNotificationTimeThreshold(PlatformId, metric.ResourceId, Convert.ToInt32(observable.ObservableId), "Platform", metric.ServerIp, metric.MetricValue);
                                                                                                if (isValid)
                                                                                                {

                                                                                                    LogHandler.LogDebug(string.Format("Sending metric data to queue and details are EventType:{0}, Application:{1}, MetricTime:{2}, ResourceId:{3}, MetricName:{4}, MetricValue:{5} ,ServerIp:{6}, Description:{7}, Source:{8}",
                                                                                                        metric.EventType, metric.Application, metric.MetricTime, metric.ResourceId, metric.MetricName, metric.MetricValue, metric.ServerIp, metric.Description, metric.Source),
                                                                                                        LogHandler.Layer.Business, null);

                                                                                                    MetricProcessorDS processorDS = new MetricProcessorDS();
                                                                                                    string msgResponse = processorDS.Send(metric, null);
                                                                                                    Console.WriteLine("Sending message to queue for resource Id " + platfromInstanceId + " Response Message : " + msgResponse);
                                                                                                    LogHandler.LogDebug(string.Format("Metric data sent to queue for resource Id {0} and response message is {1}", platfromInstanceId, msgResponse), LogHandler.Layer.Business, null);
                                                                                                }
                                                                                            }
                                                                                            else
                                                                                            {
                                                                                                Console.WriteLine("Script execution failed for resource ID:" + platfromInstanceId + "Message " + response.ScriptResponse.FirstOrDefault().SuccessMessage);
                                                                                                LogHandler.LogDebug("Script execution failed for resource ID:" + platfromInstanceId, LogHandler.Layer.Business, null);
                                                                                                LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                                                                int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), platfromInstanceId, observable.ObservableId, response.ScriptResponse.FirstOrDefault().TransactionId, "failed", healthcheckType,
                                                                                                    string.IsNullOrEmpty(response.ScriptResponse.FirstOrDefault().ErrorMessage) ? response.ScriptResponse.FirstOrDefault().SuccessMessage : response.ScriptResponse.FirstOrDefault().ErrorMessage);
                                                                                                LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);
                                                                                            }
                                                                                        }
                                                                                        else
                                                                                        {
                                                                                            Console.WriteLine("Script execution failed for resource ID:" + platfromInstanceId);
                                                                                            LogHandler.LogDebug("Script execution failes for resource ID:" + platfromInstanceId, LogHandler.Layer.Business, null);
                                                                                            LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                                                            int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), platfromInstanceId, observable.ObservableId, response.ScriptResponse.FirstOrDefault().TransactionId, "failed", healthcheckType,
                                                                                                string.IsNullOrEmpty(response.ScriptResponse.FirstOrDefault().ErrorMessage) ? response.ScriptResponse.FirstOrDefault().SuccessMessage : response.ScriptResponse.FirstOrDefault().ErrorMessage);
                                                                                            LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);
                                                                                        }
                                                                                        */
                                                #endregion
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine(string.Format("ResourceID:{0} details not found/de-activated", pID));
                                        LogHandler.LogWarning(string.Format("ResourceID:{0} details not found/de-activated", pID), LogHandler.Layer.Business, null);
                                    }
                                }


                            }
                            else
                            {
                                Console.WriteLine("Platform observables not found for platform id " + platfromInstanceId);
                                LogHandler.LogWarning("Platform observables not found for platform id " + platfromInstanceId, LogHandler.Layer.Business, null);
                            }

                            //Traverse through respective Server Nodes
                            if (platformInstanceDetails.servers.Count > 0)
                            {
                                //resourceIds to filter
                                List<string> resourceIdList = resourceIds.Split(',').ToList(); //changed line

                                LogHandler.LogDebug("Iterating through each server in platformInstancedetails", LogHandler.Layer.Business, null);
                                foreach (IE.Server server in platformInstanceDetails.servers)
                                {
                                    if (server.ServerObservables != null)
                                    {
                                        LogHandler.LogDebug("Iterating through respective ServerObservable Nodes for server" + server.IPAddress + "& name:" + server.Name, LogHandler.Layer.Business, null);
                                        //Traverse through respective ServerObservable Nodes
                                        foreach (IE.ServerObservable observable in server.ServerObservables)
                                        {
                                            if (!string.IsNullOrEmpty(observable.ResourceId))
                                            {
                                                //changed line - next if statement
                                                if (string.IsNullOrEmpty(resourceIds) || resourceIdList.Contains(observable.ResourceId))
                                                {
                                                    bool isActive = Convert.ToBoolean(resources.Where(r => r.ResourceId == observable.ResourceId).FirstOrDefault() != null ? resources.Where(r => r.ResourceId == observable.ResourceId).FirstOrDefault().IsActive : false);
                                                    if (isActive)
                                                    {

                                                        if (observable.Actions != null && observable.Actions.ActionName == "HealthCheck-Script")
                                                        {
                                                            string output = JsonConvert.SerializeObject(observable);
                                                            IE.SystemHealthCheckObservable sysObservable = JsonConvert.DeserializeObject<IE.SystemHealthCheckObservable>(output);
                                                            if (sysObservable.Actions.ExecutionMode == null ? true : sysObservable.Actions.ExecutionMode.Equals("SYNC", StringComparison.InvariantCultureIgnoreCase))
                                                            {
                                                                Console.WriteLine($"{sysObservable.Actions.ActionId} is a sync operation. Waiting for all the async tasks to complete.");
                                                                Task.WhenAll(taskList.ToArray()).Wait();
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine($"{sysObservable.Actions.ActionId} is an async operation. Executing");
                                                            }
                                                            taskList.Add(HealthCheckSystemAPI(sysObservable, Tenantid, PlatformId, observable.ResourceId, platformInstanceDetails.VendorName, HealtcheckTrackingId, healthcheckType, server.IPAddress, "Server"));

                                                            #region Commented code

                                                            /*
                                                            LogHandler.LogDebug(string.Format("Executing SEE for resourceID:{0} & ActionTypeId:{1}", platfromInstanceId, observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);
                                                            InitiateExecutionReqMsg executionReqMsg = new InitiateExecutionReqMsg();
                                                            IE.ScriptIdentifier script = new IE.ScriptIdentifier();
                                                            script.ScriptId = observable.Actions.ScriptId;
                                                            script.CategoryId = observable.Actions.CategoryId;
                                                            script.CompanyId = Tenantid;
                                                            if (observable.Actions.ParameterDetails != null)
                                                            {
                                                                List<IE.Parameter> parametersSE = new List<IE.Parameter>();
                                                                foreach (IE.ObservableParameters parameters in observable.Actions.ParameterDetails)
                                                                {
                                                                    if (string.Equals(parameters.ParamaterName, "RemoteServerNames", StringComparison.InvariantCultureIgnoreCase))
                                                                    {
                                                                        script.RemoteServerNames = parameters.ParameterValue;
                                                                    }
                                                                    else if (string.Equals(parameters.ParamaterName, "RemoteUserName", StringComparison.InvariantCultureIgnoreCase))
                                                                    {
                                                                        script.UserName = parameters.ParameterValue;
                                                                    }
                                                                    else if (string.Equals(parameters.ParamaterName, "RemotePassword", StringComparison.InvariantCultureIgnoreCase))
                                                                    {
                                                                        script.Password = parameters.ParameterValue;
                                                                    }
                                                                    else
                                                                    {
                                                                        IE.Parameter param = new IE.Parameter();
                                                                    param.ParameterName = parameters.ParamaterName;
                                                                    param.ParameterValue = parameters.ParameterValue;
                                                                    parametersSE.Add(param);
                                                                        }
                                                                }
                                                                script.Parameters = parametersSE;
                                                            }
                                                            executionReqMsg.ScriptIdentifier = script;
                                                            LogHandler.LogDebug(string.Format("Executing SEE with values scriptId:{0} & categoryId:{1}", observable.Actions.ScriptId, observable.Actions.CategoryId), LogHandler.Layer.Business, null);
                                                            Console.WriteLine("Calling SEE for resource ID : " + observable.ResourceId);

                                                            ScriptExecute scriptExecute = new ScriptExecute();
                                                            //WEMProxy scriptExecute = new WEMProxy();
                                                            var channel1 = scriptExecute.ServiceChannel;
                                                            InitiateExecutionResMsg response = channel1.InitiateExecution(executionReqMsg);
                                                            LogHandler.LogDebug(string.Format("Execution of SEE completed for resourceID:{0} & ActionTypeId:{1}", platfromInstanceId, observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);
                                                            if (response != null && response.ScriptResponse != null && response.ScriptResponse.FirstOrDefault().SuccessMessage != null)
                                                            {
                                                                string[] stringSeparators = new string[] { "\r\n" };
                                                                string[] lines = response.ScriptResponse.FirstOrDefault().SuccessMessage.Split(stringSeparators, StringSplitOptions.None);
                                                                bool isExists = Array.Exists(lines, E => E.Contains("ishealthy") || E.Contains(observable.Name + "="));
                                                                if (isExists)
                                                                {
                                                                    Console.WriteLine(response.ScriptResponse.FirstOrDefault().SuccessMessage);
                                                                    Console.WriteLine("Script execution success for resource ID:" + observable.ResourceId);

                                                                    LogHandler.LogDebug("Script execution success for resource ID:" + platfromInstanceId, LogHandler.Layer.Business, null);
                                                                    LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                                    int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), observable.ResourceId, observable.ObservableId, response.ScriptResponse.FirstOrDefault().TransactionId, "success", healthcheckType,
                                                                        string.IsNullOrEmpty(response.ScriptResponse.FirstOrDefault().ErrorMessage) ? response.ScriptResponse.FirstOrDefault().SuccessMessage : response.ScriptResponse.FirstOrDefault().ErrorMessage);
                                                                    LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);

                                                                    DE.Queue.Metric metric = new DE.Queue.Metric();
                                                                    string successMessage = response.ScriptResponse.FirstOrDefault().SuccessMessage;
                                                                    string errorMessage = response.ScriptResponse.FirstOrDefault().ErrorMessage;
                                                                    //metric = PopulateMetricData("Server",successMessage, observable.ObservableId, observable.Name, errorMessage, healthcheckType);

                                                                    BE.Metric metricBE = new BE.Metric();
                                                                    metricBE = PopulateMetricData(platformInstanceDetails.VendorName, "Server", successMessage, observable.ResourceId, observable.Name, errorMessage, healthcheckType, server.IPAddress);
                                                                    EntityTranslator translator = new EntityTranslator();
                                                                    metric = translator.MetricBEToDE(metricBE);
                                                                    metric.Application = platformInstanceDetails.VendorName;

                                                                    bool isValid = checkNotificationTimeThreshold(PlatformId, metric.ResourceId, Convert.ToInt32(observable.ObservableId), "Server", metric.ServerIp, metric.MetricValue);
                                                                    if (isValid)
                                                                    {
                                                                        LogHandler.LogDebug(string.Format("Sending metric data to queue and details are EventType:{0}, Application:{1}, MetricTime:{2}, ResourceId:{3}, MetricName:{4}, MetricValue:{5} ,ServerIp:{6}, Description:{7}, Source:{8}",
                                                                                    metric.EventType, metric.Application, metric.MetricTime, metric.ResourceId, metric.MetricName, metric.MetricValue, metric.ServerIp, metric.Description, metric.Source),
                                                                                    LogHandler.Layer.Business, null);

                                                                        MetricProcessorDS processorDS = new MetricProcessorDS();
                                                                        string msgResponse = processorDS.Send(metric, null);
                                                                        Console.WriteLine("Sending message to queue for resource Id" + observable.ResourceId + "Response Message : " + msgResponse);
                                                                        LogHandler.LogDebug(string.Format("Metric data sent to queue for resource Id {0} and response message is {1}", observable.ResourceId, msgResponse), LogHandler.Layer.Business, null);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Script execution failed for resource ID:" + observable.ResourceId + "Message " + response.ScriptResponse.FirstOrDefault().SuccessMessage);
                                                                    LogHandler.LogDebug("Script execution failed for resource ID:" + observable.ResourceId, LogHandler.Layer.Business, null);
                                                                    LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                                    int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), observable.ResourceId, observable.ObservableId, response.ScriptResponse.FirstOrDefault().TransactionId, "failed", healthcheckType,
                                                                        string.IsNullOrEmpty(response.ScriptResponse.FirstOrDefault().ErrorMessage) ? response.ScriptResponse.FirstOrDefault().SuccessMessage : response.ScriptResponse.FirstOrDefault().ErrorMessage);
                                                                    LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Script execution failed for resource ID:" + observable.ResourceId);
                                                                LogHandler.LogDebug("Script execution failed for resource ID:" + observable.ResourceId, LogHandler.Layer.Business, null);
                                                                LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                                int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), observable.ResourceId, observable.ObservableId, response.ScriptResponse.FirstOrDefault().TransactionId, "failed", healthcheckType,
                                                                    string.IsNullOrEmpty(response.ScriptResponse.FirstOrDefault().ErrorMessage) ? response.ScriptResponse.FirstOrDefault().SuccessMessage : response.ScriptResponse.FirstOrDefault().ErrorMessage);
                                                                LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);
                                                            }
                                                            */
                                                            #endregion
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine(string.Format("ResourceID:{0} details not found/de-activated", observable.ResourceId));
                                                        LogHandler.LogWarning(string.Format("ResourceID:{0} details not found/de-activated", observable.ResourceId), LogHandler.Layer.Business, null);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Server details not found for platform Id" + platfromInstanceId);
                                        LogHandler.LogWarning("Server details not found for platform Id" + platfromInstanceId, LogHandler.Layer.Business, null);
                                    }

                                    if (server.bots != null)
                                    {
                                        LogHandler.LogDebug("Iterating through each bot in server:" + server.IPAddress, LogHandler.Layer.Business, null);
                                        //Traverse through respective bot Nodes
                                        foreach (IE.Bot bot in server.bots)
                                        {
                                            if (!string.IsNullOrEmpty(bot.BotInstanceId))
                                            {
                                                //changed line - next if statement
                                                if (string.IsNullOrEmpty(resourceIds) || resourceIdList.Contains(bot.BotInstanceId))
                                                {
                                                    bool isActive = Convert.ToBoolean(resources.Where(r => r.ResourceId == bot.BotInstanceId).FirstOrDefault() != null ? resources.Where(r => r.ResourceId == bot.BotInstanceId).FirstOrDefault().IsActive : false);
                                                    if (isActive)
                                                    {
                                                        LogHandler.LogDebug("Iterating through respective BotObservable Nodes for bot" + bot.BotInstanceId + "& name:" + bot.BotName, LogHandler.Layer.Business, null);
                                                        //Traverse through respective BotObservable Nodes
                                                        foreach (IE.BotObservable observable in bot.botObservables)
                                                        {
                                                            if (observable.Actions != null && observable.Actions.ActionName == "HealthCheck-Script")
                                                            {
                                                                string output = JsonConvert.SerializeObject(observable);
                                                                IE.SystemHealthCheckObservable sysObservable = JsonConvert.DeserializeObject<IE.SystemHealthCheckObservable>(output);
                                                                if (sysObservable.Actions.ExecutionMode == null ? true : sysObservable.Actions.ExecutionMode.Equals("SYNC", StringComparison.InvariantCultureIgnoreCase))
                                                                {
                                                                    Console.WriteLine($"{sysObservable.Actions.ActionId} is a sync operation. Waiting for all the async tasks to complete.");
                                                                    Task.WhenAll(taskList.ToArray()).Wait();
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine($"{sysObservable.Actions.ActionId} is an async operation. Executing");
                                                                }
                                                                taskList.Add(HealthCheckSystemAPI(sysObservable, Tenantid, PlatformId, bot.BotInstanceId, platformInstanceDetails.VendorName, HealtcheckTrackingId, healthcheckType, server.IPAddress, "Bot"));
                                                                #region Commented code
                                                                /*
                                                                                                                    LogHandler.LogDebug(string.Format("Executing SEE for resourceID:{0} & ActionTypeId:{1}", bot.BotInstanceId, observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);
                                                                                                                    InitiateExecutionReqMsg executionReqMsg = new InitiateExecutionReqMsg();
                                                                                                                    IE.ScriptIdentifier script = new IE.ScriptIdentifier();
                                                                                                                    script.ScriptId = observable.Actions.ScriptId;
                                                                                                                    script.CategoryId = observable.Actions.CategoryId;
                                                                                                                    script.CompanyId = Tenantid;
                                                                                                                    if (observable.Actions.ParameterDetails != null)
                                                                                                                    {
                                                                                                                        List<IE.Parameter> parametersSE = new List<IE.Parameter>();
                                                                                                                        foreach (IE.ObservableParameters parameters in observable.Actions.ParameterDetails)
                                                                                                                        {
                                                                                                                            if (string.Equals(parameters.ParamaterName, "RemoteServerNames", StringComparison.InvariantCultureIgnoreCase))
                                                                                                                            {
                                                                                                                                script.RemoteServerNames = parameters.ParameterValue;
                                                                                                                            }
                                                                                                                            else if (string.Equals(parameters.ParamaterName, "RemoteUserName", StringComparison.InvariantCultureIgnoreCase))
                                                                                                                            {
                                                                                                                                script.UserName = parameters.ParameterValue;
                                                                                                                            }
                                                                                                                            else if (string.Equals(parameters.ParamaterName, "RemotePassword", StringComparison.InvariantCultureIgnoreCase))
                                                                                                                            {
                                                                                                                                script.Password = parameters.ParameterValue;
                                                                                                                            }
                                                                                                                            else
                                                                                                                            {
                                                                                                                                IE.Parameter param = new IE.Parameter();
                                                                                                                                param.ParameterName = parameters.ParamaterName;
                                                                                                                                param.ParameterValue = parameters.ParameterValue;
                                                                                                                                parametersSE.Add(param);
                                                                                                                            }
                                                                                                                        }
                                                                                                                        script.Parameters = parametersSE;
                                                                                                                    }

                                                                                                                    executionReqMsg.ScriptIdentifier = script;

                                                                                                                    Console.WriteLine("Calling SEE for resource ID : " + bot.BotInstanceId);
                                                                                                                    LogHandler.LogDebug(string.Format("Executing SEE with values scriptId:{0} & categoryId:{1}", observable.Actions.ScriptId, observable.Actions.CategoryId), LogHandler.Layer.Business, null);
                                                                                                                    ScriptExecute scriptExecute = new ScriptExecute();
                                                                                                                    //WEMProxy scriptExecute = new WEMProxy();
                                                                                                                    var channel1 = scriptExecute.ServiceChannel;
                                                                                                                    InitiateExecutionResMsg response = channel1.InitiateExecution(executionReqMsg);
                                                                                                                    LogHandler.LogDebug(string.Format("Execution of SEE completed for resourceID:{0} & ActionTypeId:{1}", platfromInstanceId, observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);
                                                                                                                    if (response != null && response.ScriptResponse != null && response.ScriptResponse.FirstOrDefault().SuccessMessage.ToLower() != null)
                                                                                                                    {
                                                                                                                        string[] stringSeparators = new string[] { "\r\n" };
                                                                                                                        string[] lines = response.ScriptResponse.FirstOrDefault().SuccessMessage.Split(stringSeparators, StringSplitOptions.None);
                                                                                                                        bool isExists = Array.Exists(lines, E => E.Contains("ishealthy") || E.Contains(observable.Name + "="));
                                                                                                                        if (isExists)
                                                                                                                        {
                                                                                                                            Console.WriteLine("Script execution success for resource ID:" + bot.BotInstanceId);

                                                                                                                            LogHandler.LogDebug("Script execution success for resource ID:" + platfromInstanceId, LogHandler.Layer.Business, null);
                                                                                                                            LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                                                                                            int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), bot.BotInstanceId, observable.ObservableId, response.ScriptResponse.FirstOrDefault().TransactionId, "success", healthcheckType,
                                                                                                                                string.IsNullOrEmpty(response.ScriptResponse.FirstOrDefault().ErrorMessage) ? response.ScriptResponse.FirstOrDefault().SuccessMessage : response.ScriptResponse.FirstOrDefault().ErrorMessage);
                                                                                                                            LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);

                                                                                                                            DE.Queue.Metric metric = new DE.Queue.Metric();
                                                                                                                            string successMessage = response.ScriptResponse.FirstOrDefault().SuccessMessage;
                                                                                                                            string errorMessage = response.ScriptResponse.FirstOrDefault().ErrorMessage;

                                                                                                                            BE.Metric metricBE = new BE.Metric();
                                                                                                                            metricBE = PopulateMetricData(platformInstanceDetails.VendorName, "Bot", successMessage, bot.BotInstanceId, observable.Name, errorMessage, healthcheckType, server.IPAddress);
                                                                                                                            EntityTranslator translator = new EntityTranslator();
                                                                                                                            metric = translator.MetricBEToDE(metricBE);
                                                                                                                            metric.Application = platformInstanceDetails.VendorName;

                                                                                                                            bool isValid = checkNotificationTimeThreshold(PlatformId, metric.ResourceId, Convert.ToInt32(observable.ObservableId), "Bot", metric.ServerIp, metric.MetricValue);
                                                                                                                            if (isValid)
                                                                                                                            {
                                                                                                                                LogHandler.LogDebug(string.Format("Sending metric data to queue and details are EventType:{0}, Application:{1}, MetricTime:{2}, ResourceId:{3}, MetricName:{4}, MetricValue:{5} ,ServerIp:{6}, Description:{7}, Source:{8}",
                                                                                                                                    metric.EventType, metric.Application, metric.MetricTime, metric.ResourceId, metric.MetricName, metric.MetricValue, metric.ServerIp, metric.Description, metric.Source),
                                                                                                                                    LogHandler.Layer.Business, null);

                                                                                                                                MetricProcessorDS processorDS = new MetricProcessorDS();
                                                                                                                                string msgResponse = processorDS.Send(metric, null);
                                                                                                                                Console.WriteLine("Sending message to queue for Resource Id" + bot.BotInstanceId + " Response Message : " + msgResponse);
                                                                                                                                LogHandler.LogDebug(string.Format("Metric data sent to queue for resource Id {0} and response message is {1}", bot.BotInstanceId, msgResponse), LogHandler.Layer.Business, null);
                                                                                                                            }
                                                                                                                        }
                                                                                                                        else
                                                                                                                        {
                                                                                                                            Console.WriteLine("Script execution failed for resource ID:" + bot.BotInstanceId);
                                                                                                                            LogHandler.LogDebug("Script execution failed for resource ID:" + bot.BotInstanceId, LogHandler.Layer.Business, null);
                                                                                                                            LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                                                                                            int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), bot.BotInstanceId, observable.ObservableId, response.ScriptResponse.FirstOrDefault().TransactionId, "failed", healthcheckType,
                                                                                                                                string.IsNullOrEmpty(response.ScriptResponse.FirstOrDefault().ErrorMessage) ? response.ScriptResponse.FirstOrDefault().SuccessMessage : response.ScriptResponse.FirstOrDefault().ErrorMessage);
                                                                                                                            LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);
                                                                                                                        }
                                                                                                                    }
                                                                                                                    else
                                                                                                                    {
                                                                                                                        Console.WriteLine("Script execution failed for resource ID:" + bot.BotInstanceId);
                                                                                                                        LogHandler.LogDebug("Script execution failed for resource ID:" + bot.BotInstanceId, LogHandler.Layer.Business, null);
                                                                                                                        LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                                                                                        int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), bot.BotInstanceId, observable.ObservableId, response.ScriptResponse.FirstOrDefault().TransactionId, "failed", healthcheckType,
                                                                                                                            string.IsNullOrEmpty(response.ScriptResponse.FirstOrDefault().ErrorMessage) ? response.ScriptResponse.FirstOrDefault().SuccessMessage : response.ScriptResponse.FirstOrDefault().ErrorMessage);
                                                                                                                        LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);
                                                                                                                    }
                                                                                                                    */
                                                                #endregion
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine(string.Format("ResourceID:{0} details not found/de-activated", bot.BotInstanceId));
                                                        LogHandler.LogWarning(string.Format("ResourceID:{0} details not found/de-activated", bot.BotInstanceId), LogHandler.Layer.Business, null);
                                                    }
                                                }                                                
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Bot details not found for platform Id" + platfromInstanceId + " & server:" + server.IPAddress);
                                        LogHandler.LogWarning("Bot details not found for platform Id" + platfromInstanceId + " & server:" + server.IPAddress, LogHandler.Layer.Business, null);
                                    }

                                    if (server.services != null)
                                    {
                                        LogHandler.LogDebug("Iterating through each service in server:" + server.IPAddress, LogHandler.Layer.Business, null);
                                        //Traverse through respective Service Nodes
                                        foreach (IE.Service service in server.services)
                                        {
                                            if (!string.IsNullOrEmpty(service.ServiceId))
                                            {                                                
                                                if (string.IsNullOrEmpty(resourceIds) || resourceIdList.Contains(service.ServiceId))
                                                {
                                                    bool isActive = Convert.ToBoolean(resources.Where(r => r.ResourceId == service.ServiceId).FirstOrDefault() != null ? resources.Where(r => r.ResourceId == service.ServiceId).FirstOrDefault().IsActive : false);
                                                    if (isActive)
                                                    {
                                                        LogHandler.LogDebug("Iterating through respective ServiceObservable Nodes for service" + service.ServiceId + "& name:" + service.ServiceName, LogHandler.Layer.Business, null);
                                                        //Traverse through respective ServiceObservable Nodes
                                                        foreach (IE.ServiceObservable observable in service.serviceObservables)
                                                        {
                                                            if (observable.Actions != null && observable.Actions.ActionName == "HealthCheck-Script")
                                                            {
                                                                string output = JsonConvert.SerializeObject(observable);
                                                                IE.SystemHealthCheckObservable sysObservable = JsonConvert.DeserializeObject<IE.SystemHealthCheckObservable>(output);
                                                                if (sysObservable.Actions.ExecutionMode == null ? true : sysObservable.Actions.ExecutionMode.Equals("SYNC", StringComparison.InvariantCultureIgnoreCase))
                                                                {
                                                                    Console.WriteLine($"{sysObservable.Actions.ActionId} is a sync operation. Waiting for all the async tasks to complete.");
                                                                    Task.WhenAll(taskList.ToArray()).Wait();
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine($"{sysObservable.Actions.ActionId} is an async operation. Executing");
                                                                }
                                                                taskList.Add(HealthCheckSystemAPI(sysObservable, Tenantid, PlatformId, service.ServiceId, platformInstanceDetails.VendorName, HealtcheckTrackingId, healthcheckType, server.IPAddress, "Service"));
                                                                #region Commented Code
                                                                /*
                                                                                                                   LogHandler.LogDebug(string.Format("Executing SEE for resourceID:{0} & ActionTypeId:{1}", service.ServiceId, observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);
                                                                                                                   InitiateExecutionReqMsg executionReqMsg = new InitiateExecutionReqMsg();
                                                                                                                   IE.ScriptIdentifier script = new IE.ScriptIdentifier();
                                                                                                                   script.ScriptId = observable.Actions.ScriptId;
                                                                                                                   script.CategoryId = observable.Actions.CategoryId;
                                                                                                                   script.CompanyId = Tenantid;
                                                                                                                   if (observable.Actions.ParameterDetails != null)
                                                                                                                   {
                                                                                                                       List<IE.Parameter> parametersSE = new List<IE.Parameter>();
                                                                                                                       foreach (IE.ObservableParameters parameters in observable.Actions.ParameterDetails)
                                                                                                                       {
                                                                                                                           if (string.Equals(parameters.ParamaterName, "RemoteServerNames", StringComparison.InvariantCultureIgnoreCase))
                                                                                                                           {
                                                                                                                               script.RemoteServerNames = parameters.ParameterValue;
                                                                                                                           }
                                                                                                                           else if (string.Equals(parameters.ParamaterName, "RemoteUserName", StringComparison.InvariantCultureIgnoreCase))
                                                                                                                           {
                                                                                                                               script.UserName = parameters.ParameterValue;
                                                                                                                           }
                                                                                                                           else if (string.Equals(parameters.ParamaterName, "RemotePassword", StringComparison.InvariantCultureIgnoreCase))
                                                                                                                           {
                                                                                                                               script.Password = parameters.ParameterValue;
                                                                                                                           }
                                                                                                                           else
                                                                                                                           {
                                                                                                                               IE.Parameter param = new IE.Parameter();
                                                                                                                               param.ParameterName = parameters.ParamaterName;
                                                                                                                               param.ParameterValue = parameters.ParameterValue;
                                                                                                                               parametersSE.Add(param);
                                                                                                                           }
                                                                                                                       }
                                                                                                                       script.Parameters = parametersSE;
                                                                                                                   }
                                                                                                                   executionReqMsg.ScriptIdentifier = script;

                                                                                                                   Console.WriteLine("Calling SEE for resource ID : " + service.ServiceId);
                                                                                                                   LogHandler.LogDebug(string.Format("Executing SEE with values scriptId:{0} & categoryId:{1}", observable.Actions.ScriptId, observable.Actions.CategoryId), LogHandler.Layer.Business, null);

                                                                                                                   ScriptExecute scriptExecute = new ScriptExecute();
                                                                                                                   //WEMProxy scriptExecute = new WEMProxy();
                                                                                                                   var channel1 = scriptExecute.ServiceChannel;
                                                                                                                   InitiateExecutionResMsg response = channel1.InitiateExecution(executionReqMsg);
                                                                                                                   LogHandler.LogDebug(string.Format("Execution of SEE completed for resourceID:{0} & ActionTypeId:{1}", service.ServiceId, observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);
                                                                                                                   if (response != null && response.ScriptResponse.FirstOrDefault().SuccessMessage != null && response.ScriptResponse.FirstOrDefault().SuccessMessage.ToLower() != null)
                                                                                                                   {

                                                                                                                       string[] stringSeparators = new string[] { "\r\n" };
                                                                                                                       string[] lines = response.ScriptResponse.FirstOrDefault().SuccessMessage.Split(stringSeparators, StringSplitOptions.None);
                                                                                                                       bool isExists = Array.Exists(lines, E => E.Contains("ishealthy") || E.Contains(observable.Name + "="));
                                                                                                                       if (isExists)
                                                                                                                       {
                                                                                                                           Console.WriteLine("Script execution success for resource ID:" + service.ServiceId);
                                                                                                                           LogHandler.LogDebug("Script execution success for resource ID:" + service.ServiceId, LogHandler.Layer.Business, null);
                                                                                                                           LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                                                                                           int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), service.ServiceId, observable.ObservableId, response.ScriptResponse.FirstOrDefault().TransactionId, "success", healthcheckType,
                                                                                                                               string.IsNullOrEmpty(response.ScriptResponse.FirstOrDefault().ErrorMessage) ? response.ScriptResponse.FirstOrDefault().SuccessMessage : response.ScriptResponse.FirstOrDefault().ErrorMessage);
                                                                                                                           LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);

                                                                                                                           DE.Queue.Metric metric = new DE.Queue.Metric();
                                                                                                                           string successMessage = response.ScriptResponse.FirstOrDefault().SuccessMessage;
                                                                                                                           string errorMessage = response.ScriptResponse.FirstOrDefault().ErrorMessage;
                                                                                                                           BE.Metric metricBE = new BE.Metric();
                                                                                                                           metricBE = PopulateMetricData(platformInstanceDetails.VendorName, "Service", successMessage, service.ServiceId, observable.Name, errorMessage, healthcheckType, server.IPAddress);
                                                                                                                           EntityTranslator translator = new EntityTranslator();
                                                                                                                           metric = translator.MetricBEToDE(metricBE);
                                                                                                                           metric.Application = platformInstanceDetails.VendorName;

                                                                                                                           bool isValid = checkNotificationTimeThreshold(PlatformId, metric.ResourceId, Convert.ToInt32(observable.ObservableId), "Service", metric.ServerIp, metric.MetricValue);
                                                                                                                           if (isValid)
                                                                                                                           {
                                                                                                                               LogHandler.LogDebug(string.Format("Sending metric data to queue and details are EventType:{0}, Application:{1}, MetricTime:{2}, ResourceId:{3}, MetricName:{4}, MetricValue:{5} ,ServerIp:{6}, Description:{7}, Source:{8}",
                                                                                                                                   metric.EventType, metric.Application, metric.MetricTime, metric.ResourceId, metric.MetricName, metric.MetricValue, metric.ServerIp, metric.Description, metric.Source),
                                                                                                                                   LogHandler.Layer.Business, null);

                                                                                                                               MetricProcessorDS processorDS = new MetricProcessorDS();
                                                                                                                               string msgResponse = processorDS.Send(metric, null);
                                                                                                                               Console.WriteLine("Sending message to queue for resource ID " + service.ServiceId + " response Message : " + msgResponse);
                                                                                                                               LogHandler.LogDebug(string.Format("Metric data sent to queue for resource Id {0} and response message is {1}", service.ServiceId, msgResponse), LogHandler.Layer.Business, null);
                                                                                                                           }
                                                                                                                       }
                                                                                                                       else
                                                                                                                       {
                                                                                                                           Console.WriteLine("Script execution failed for resource ID:" + service.ServiceId + "Message "+ response.ScriptResponse.FirstOrDefault().SuccessMessage);
                                                                                                                           LogHandler.LogDebug("Script execution failed for resource ID:" + service.ServiceId, LogHandler.Layer.Business, null);
                                                                                                                           LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                                                                                           int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), service.ServiceId, observable.ObservableId, response.ScriptResponse.FirstOrDefault().TransactionId, "failed", healthcheckType,
                                                                                                                               string.IsNullOrEmpty(response.ScriptResponse.FirstOrDefault().ErrorMessage) ? response.ScriptResponse.FirstOrDefault().SuccessMessage : response.ScriptResponse.FirstOrDefault().ErrorMessage);
                                                                                                                           LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);
                                                                                                                       }
                                                                                                                   }
                                                                                                                   else
                                                                                                                   {
                                                                                                                       Console.WriteLine("Script execution failed for resource ID:" + service.ServiceId);
                                                                                                                       LogHandler.LogDebug("Script execution failed for resource ID:" + service.ServiceId, LogHandler.Layer.Business, null);
                                                                                                                       LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                                                                                       int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), service.ServiceId, observable.ObservableId, response.ScriptResponse.FirstOrDefault().TransactionId, "failed", healthcheckType,
                                                                                                                           string.IsNullOrEmpty(response.ScriptResponse.FirstOrDefault().ErrorMessage) ? response.ScriptResponse.FirstOrDefault().SuccessMessage : response.ScriptResponse.FirstOrDefault().ErrorMessage);
                                                                                                                       LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);
                                                                                                                   }
                                                                                                                   */
                                                                #endregion
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine(string.Format("ResourceID:{0} details not found/de-activated", service.ServiceId));
                                                        LogHandler.LogWarning(string.Format("ResourceID:{0} details not found/de-activated", service.ServiceId), LogHandler.Layer.Business, null);
                                                    }
                                                }                                                
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("service details not found for platform Id" + platfromInstanceId + " & server:" + server.IPAddress);
                                        LogHandler.LogWarning("service details not found for platform Id" + platfromInstanceId + " & server:" + server.IPAddress, LogHandler.Layer.Business, null);
                                    }

                                }
                                LogHandler.LogDebug("calling HealthcheckCompleted to update execution status value as success for TrackingId:" + HealtcheckTrackingId, LogHandler.Layer.Business, null);

                                //wait for all the health check to complete before calling healthcheck completed method
                                Task.WhenAll(taskList.ToArray()).Wait();

                                //call HealthcheckCompleted  to update the execution status
                                HealthcheckCompleted(HealtcheckTrackingId, "success", null);
                                LogHandler.LogDebug("Updated the status to success for TrackingId:" + HealtcheckTrackingId, LogHandler.Layer.Business, null);

                            }
                            else
                            {
                                LogHandler.LogWarning("Server details not found for system health check", LogHandler.Layer.Business, null);
                            }

                        }
                        else
                        {
                            LogHandler.LogError(ErrorMessages.ValuesInsertionUnsuccessful, LogHandler.Layer.Business, null);
                        }
                    }
                    else
                    {
                        LogHandler.LogError("Platform Instance details not found for platform=" + PlatformId, LogHandler.Layer.Business, null);

                        SuperbotDataItemNotFoundException dataNotfoundException = new SuperbotDataItemNotFoundException(string.Format(ErrorMessages.Platform_Data_NotFound, PlatformId, Tenantid));
                        List<ValidationError> validateErrs = new List<ValidationError>();
                        ValidationError validationErr = new ValidationError();
                        validationErr.Code = "1041";
                        validationErr.Description = string.Format(ErrorMessages.Platform_Data_NotFound, PlatformId, Tenantid);
                        validateErrs.Add(validationErr);

                        if (validateErrs.Count > 0)
                        {
                            dataNotfoundException.Data.Add("DataNotFoundErrors", validateErrs);
                            throw dataNotfoundException;
                        }
                    }
                }
            }
            catch (Exception superBotException)
            {
                Console.WriteLine("Exception occured while performing system health check. Error Message = " + superBotException.Message);
                LogHandler.LogError("Exception occured while performing system health check for platformID" + PlatformId + " & Error Message = " + superBotException.Message,
                    LogHandler.Layer.Business, null);

                if (HealtcheckTrackingId > 0)
                {
                    //wait for tasks to complete before calling completion method
                    Task.WhenAll(taskList.ToArray()).Wait();

                    LogHandler.LogDebug("calling HealthcheckCompleted to update execution status value as failed for TrackingId:" + HealtcheckTrackingId, LogHandler.Layer.Business, null);
                    HealthcheckCompleted(HealtcheckTrackingId, "failed", superBotException.Message);
                    LogHandler.LogDebug("Updated the status to failed for TrackingId:" + HealtcheckTrackingId, LogHandler.Layer.Business, null);
                }

                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(superBotException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);
                if (rethrow)
                {
                    throw ex;
                }
            }
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "SystemHealthcheck", "Monitor"), LogHandler.Layer.Business, null);
            return HealtcheckTrackingId;
        }


        public int PlatformHealthcheck(int PlatformId, int Tenantid, string dependencyResourceID, string resourceIds)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "PlatformHealthcheck", "Monitor"), LogHandler.Layer.Business, null);
            string healthcheckType = "Platform";
            int HealtcheckTrackingId = 0;
            List<Task> taskList = new List<Task>();
            try
            {
                LogHandler.LogDebug(string.Format("Executing PlatformHealthcheck for parameters platformId:{0} and tenantId:{1}", PlatformId, Tenantid), LogHandler.Layer.Business, null);
                tenantid = Tenantid;
                string platfromInstanceId = Convert.ToString(PlatformId);

                LogHandler.LogDebug(string.Format("Calling API to get AllBotPlatformInstanceDependencies for parameters PlatformId:{0} and tenantId:{1}", PlatformId, Tenantid), LogHandler.Layer.Business, null);
                Infrastructure.ServiceClientLibrary.SuperBot resourceHandler = new Infrastructure.ServiceClientLibrary.SuperBot();
                var channel = resourceHandler.ServiceChannel;
                IE.PlatformInstanceDetails platformInstanceDetails = channel.GetAllBotPlatformInstanceDependencies(Convert.ToString(PlatformId),
                    Convert.ToString(Tenantid), dependencyResourceID);
                LogHandler.LogDebug(string.Format("Completed execution of API and returned platformInstance details for PlatformId:{0} and tenantId:{1}", PlatformId, Tenantid), LogHandler.Layer.Business, null);

                int actiontypeID = Convert.ToInt32(ActionTypes.HealthCheckScript);


                //Calling HealthcheckInitiated to insert data into DB
                if (platformInstanceDetails != null && platformInstanceDetails.servers.Count > 0)
                {
                    LogHandler.LogDebug(string.Format("Executing HealthcheckInitiated method to insert platform details into DB and the parameters are PlatformId:{0},PlatformResourceModelVersion:{1},healthcheckSource:{2} & IPaddress:{3}",
                            platformInstanceDetails.PlatformId, platformInstanceDetails.PlatformResourceModelVersion, healthcheckType, platformInstanceDetails.servers[0].IPAddress), LogHandler.Layer.Business, null);

                    var controlTower = platformInstanceDetails.servers.Where(a => a.Type.ToLower() == ResourceType.ControlTower.ToString().ToLower());
                    string IpValue = controlTower.ToList().Count > 0 ? controlTower.FirstOrDefault().IPAddress : platformInstanceDetails.servers.FirstOrDefault().IPAddress;
                    //HealtcheckTrackingId = HealthcheckInitiated(platformInstanceDetails.PlatformId, platformInstanceDetails.PlatformResourceModelVersion, healthcheckType,
                    //    controlTower.FirstOrDefault().IPAddress);
                    HealtcheckTrackingId = HealthcheckInitiated(platformInstanceDetails.PlatformId, platformInstanceDetails.PlatformResourceModelVersion, healthcheckType,
                        IpValue);
                    LogHandler.LogDebug(string.Format("Execution of HealthcheckInitiated method completed", HealtcheckTrackingId), LogHandler.Layer.Business, null);
                    if (HealtcheckTrackingId > 0)
                    {
                        ResourceDS resDS = new ResourceDS();
                        var resources = resDS.GetAll();

                        LogHandler.LogDebug(string.Format("Data inserted into healthcheck_iteration_tracker table and  HealtcheckTrackingId:{0}", HealtcheckTrackingId), LogHandler.Layer.Business, null);
                        //Traverse through respective platformObservable Nodes
                        if (platformInstanceDetails.platformObservables != null)
                        {
                            string pID = Convert.ToString(platformInstanceDetails.PlatformId);
                            if (!string.IsNullOrEmpty(pID))
                            {
                                bool isActive = Convert.ToBoolean(resources.Where(r => r.ResourceId == pID).FirstOrDefault() != null ? resources.Where(r => r.ResourceId == pID).FirstOrDefault().IsActive : false);
                                if (isActive)
                                {
                                    LogHandler.LogDebug("Iterating through each observable in platformInstancedetails", LogHandler.Layer.Business, null);
                                    foreach (IE.PlatformObservable observable in platformInstanceDetails.platformObservables)
                                    {
                                        if (observable.Actions != null && observable.Actions.ActionName == "HealthCheck-PlatformDB")
                                        {
                                            string dbServer = null, dbName = null, viewName = null;
                                            Console.WriteLine("Calling DB for resource ID:" + platfromInstanceId);
                                            LogHandler.LogDebug(string.Format("Getting EndpointUri value from action table for actionTypeId:{0}", observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);

                                            ActionDS action = new ActionDS();
                                            var actionres = action.GetAny().ToArray().Where(a => a.ActionTypeId == observable.Actions.ActionTypeId).FirstOrDefault();

                                            if (actionres != null)
                                            {
                                                LogHandler.LogDebug(string.Format("EndpointUri value:{0}", actionres.EndpointUri), LogHandler.Layer.Business, null);
                                                //string endPointUri = actionres.EndpointUri;
                                                string endPointUri = observable.Actions.ParameterDetails.Where(p => p.ParamaterName.ToLower() == "endpointuri").Select(s => s.ParameterValue).FirstOrDefault();
                                                string[] objs = endPointUri.Split('/');
                                                foreach (string s in objs.Where(x => !string.IsNullOrEmpty(x)).ToArray())
                                                {
                                                    if (objs[2] == s)
                                                    {
                                                        dbServer = s;
                                                    }
                                                    if (objs[3] == s)
                                                    {
                                                        dbName = s;
                                                    }
                                                    if (objs[5] == s)
                                                    {
                                                        viewName = s;
                                                    }
                                                }
                                                if (dbServer != null && dbName != null && viewName != null)
                                                {
                                                    LogHandler.LogDebug(string.Format("Got the DBserver:{0},DBName:{1} & ViewName:{2} from EndpointUri value:{3}", dbServer, dbName, viewName, actionres.EndpointUri), LogHandler.Layer.Business, null);
                                                    LogHandler.LogDebug(string.Format("Calling ExecuteDBView method to get metric values"), LogHandler.Layer.Business, null);

                                                    DataTable dtResult = ExecuteDBView(dbServer, dbName, viewName, observable.Actions.ParameterDetails);
                                                    List<DataRow> dtlist = dtResult.AsEnumerable().ToList();

                                                    /*FacadeClient facadeClient = new FacadeClient();
                                                    Console.WriteLine("Created object for facade client");
                                                    List<DataRow> dtlist = facadeClient.GeHealthCheckPlatformDBMetrics();
                                                    Console.WriteLine("Fetched data from DB Adapter");
                                                    Console.WriteLine("No.of Rows fetched in facade framework:" + dtlist.Count);*/

                                                    var results = (from myRow in dtlist
                                                                   where myRow.Field<string>("bot_name").ToLower() == platformInstanceDetails.VendorName.ToLower() &&
                                                                   myRow.Field<string>("ipaddress") == platfromInstanceId &&
                                                                   myRow.Field<string>("metric_name").ToLower() == observable.Name.ToLower()
                                                                   select myRow).FirstOrDefault();
                                                    if (results != null)
                                                    {
                                                        Console.WriteLine("Metric details found for bot_name:" + platformInstanceDetails.VendorName + " & IPAddress:" + platformInstanceDetails.VendorName + " & metric_name:" + observable.Name);

                                                        LogHandler.LogDebug("Metric details found for bot_name:" + platformInstanceDetails.VendorName + " & IPAddress:" + platformInstanceDetails.VendorName + " & metric_name:" + observable.Name, LogHandler.Layer.Business, null);
                                                        LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                        int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), platfromInstanceId, observable.ObservableId, Guid.Empty.ToString(), "success", healthcheckType, "NA");
                                                        LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);

                                                        DE.Queue.Metric metric = new DE.Queue.Metric();

                                                        BE.Metric metricBE = new BE.Metric();
                                                        metricBE.Application = platformInstanceDetails.VendorName;
                                                        metricBE.EventType = results.Field<string>("event_type");
                                                        metricBE.Description = string.IsNullOrEmpty(results.Field<string>("description")) ? "NA" : results.Field<string>("description");
                                                        metricBE.MetricName = results.Field<string>("metric_name");
                                                        metricBE.MetricValue = results.Field<string>("metric_value");
                                                        metricBE.MetricTime = results.Field<string>("metric_time").ToString();
                                                        metricBE.ResourceId = platfromInstanceId;
                                                        metricBE.ServerIp = results.Field<string>("server_ip");
                                                        //metricBE.Source = results.Field<string>("source");
                                                        metricBE.Source = platformInstanceDetails.VendorName;

                                                        //metricBE = PopulateMetricData(platformInstanceDetails.VendorName, "Server", "success", bot.BotInstanceId, observable.Name, "NA", healthcheckType);
                                                        EntityTranslator translator = new EntityTranslator();
                                                        metric = translator.MetricBEToDE(metricBE);

                                                        bool isValid = checkNotificationTimeThreshold(PlatformId, metric.ResourceId, Convert.ToInt32(observable.ObservableId), "Platform", metric.ServerIp, metric.MetricValue);
                                                        if (isValid)
                                                        {
                                                            LogHandler.LogDebug(string.Format("Sending metric data to queue and details are EventType:{0}, Application:{1}, MetricTime:{2}, ResourceId:{3}, MetricName:{4}, MetricValue:{5} ,ServerIp:{6}, Description:{7}, Source:{8}",
                                                                    metric.EventType, metric.Application, metric.MetricTime, metric.ResourceId, metric.MetricName, metric.MetricValue, metric.ServerIp, metric.Description, metric.Source),
                                                                    LogHandler.Layer.Business, null);

                                                            MetricProcessorDS processorDS = new MetricProcessorDS();
                                                            string msgResponse = processorDS.Send(metric, null);
                                                            Console.WriteLine("Sending message to queue for resource Id " + platfromInstanceId + " Response Message : " + msgResponse);
                                                            LogHandler.LogDebug(string.Format("Metric data sent to queue for resource Id {0} and response message is {1}", platfromInstanceId, msgResponse), LogHandler.Layer.Business, null);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Metric details not found in view for bot_name:" + observable.Name + " & IPAddress:" + platfromInstanceId + " & metric_name:" + observable.Name);
                                                        string errorMsg = "Metric details not found in view for bot_name:" + observable.Name + " & IPAddress:" + platfromInstanceId + " & metric_name:" + observable.Name;
                                                        LogHandler.LogWarning(errorMsg, LogHandler.Layer.Business, null);
                                                        LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                        int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), platfromInstanceId, observable.ObservableId, Guid.Empty.ToString(), "failed", healthcheckType, errorMsg);
                                                        LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);
                                                    }

                                                }
                                                else
                                                {
                                                    Console.WriteLine("Invalid EndpointUri configured for resource ID:" + platfromInstanceId);
                                                    string errorMsg = "Invalid EndpointUri configured for resource ID:" + platfromInstanceId;
                                                    LogHandler.LogWarning(errorMsg, LogHandler.Layer.Business, null);
                                                    LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                    int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), platfromInstanceId, observable.ObservableId, Guid.Empty.ToString(), "failed", healthcheckType, errorMsg);
                                                    LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);

                                                }
                                            }
                                            else
                                            {
                                                LogHandler.LogWarning(string.Format("EndpointUri not configured in action table for actiontypeId:{0}", observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);
                                                Console.WriteLine(string.Format("EndpointUri not configured in action table for actiontypeId:{0}", observable.Actions.ActionTypeId));
                                                string errorMsg = string.Format("EndpointUri not configured in action table for actiontypeId:{0}", observable.Actions.ActionTypeId);
                                                int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), platfromInstanceId, observable.ObservableId, Guid.Empty.ToString(), "failed", healthcheckType, errorMsg);
                                            }

                                        }
                                        else if (observable.Actions != null && observable.Actions.ActionName == "HealthCheck-PlatformAPI")
                                        {
                                            LogHandler.LogDebug(string.Format("Executing SEE for resourceID:{0} & ActionTypeId:{1}", platfromInstanceId, observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);
                                            if (observable.Actions.ExecutionMode == null ? true : observable.Actions.ExecutionMode.Equals("SYNC", StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                Console.WriteLine($"{observable.Actions.ActionId} is a sync operation. Waiting for all the async tasks to complete.");
                                                Task.WhenAll(taskList.ToArray()).Wait();
                                            }
                                            else
                                            {
                                                Console.WriteLine($"{observable.Actions.ActionId} is an async operation. Executing");
                                            }
                                            taskList.Add(HealcheckPlatformAPI(Tenantid, healthcheckType, HealtcheckTrackingId, platformInstanceDetails, observable.Actions, platfromInstanceId,
                                                observable.ObservableId, "NA", observable.Name, "", ResourceType.Platform.ToString(), "", deviceList));
                                            LogHandler.LogDebug(string.Format("Execution of SEE completed for resourceID:{0} & ActionTypeId:{1}", platfromInstanceId, observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);
                                        }

                                    }
                                }
                                else
                                {
                                    Console.WriteLine(string.Format("ResourceID:{0} details not found/de-activated", pID));
                                    LogHandler.LogWarning(string.Format("ResourceID:{0} details not found/de-activated", pID), LogHandler.Layer.Business, null);
                                }
                            }

                        }
                        else
                        {
                            Console.WriteLine("Platform observables not found for platform id " + platfromInstanceId);
                            LogHandler.LogInfo("Platform observables not found for platform id " + platfromInstanceId, LogHandler.Layer.Business, null);
                        }



                        LogHandler.LogDebug("Checking no.of server in platformInstancedetails. No.of servers:" + platformInstanceDetails.servers.Count, LogHandler.Layer.Business, null);
                        //Traverse through respective Server Nodes
                        if (platformInstanceDetails.servers.Count > 0)
                        {
                            //resourceIds to filter
                            List<string> resourceIdList = resourceIds.Split(',').ToList(); //changed line
                            LogHandler.LogDebug("Iterating through each observable in platformInstancedetails", LogHandler.Layer.Business, null);
                            foreach (IE.Server server in platformInstanceDetails.servers)
                            {
                                string serverType = "";
                                if (server.ServerObservables != null)
                                {

                                    serverType = server.Type;
                                    /*switch (server.Type.ToLower())
                                    {
                                        case "controltower":
                                            serverType = ResourceType.ControlTower.ToString();
                                            break;
                                        case "botrunner":
                                            serverType = ResourceType.Bot_Runner.ToString();
                                            break;
                                        case "dbserver":
                                            serverType = ResourceType.DBServer.ToString();
                                            break;
                                        case "botcreator":
                                            serverType = ResourceType.Bot_Creator.ToString();
                                            break;
                                        default:
                                            break;
                                    }*/

                                    //Traverse through respective ServerObservable Nodes
                                    foreach (IE.ServerObservable observable in server.ServerObservables)
                                    {
                                        if (!string.IsNullOrEmpty(observable.ResourceId))
                                        {
                                            if (string.IsNullOrEmpty(resourceIds) || resourceIdList.Contains(observable.ResourceId))
                                            {
                                                bool isActive = Convert.ToBoolean(resources.Where(r => r.ResourceId == observable.ResourceId).FirstOrDefault() != null ? resources.Where(r => r.ResourceId == observable.ResourceId).FirstOrDefault().IsActive : false);
                                                if (isActive)
                                                {
                                                    if (observable.Actions != null && observable.Actions.ActionName == "HealthCheck-PlatformDB")
                                                    {
                                                        string dbServer = null, dbName = null, viewName = null;
                                                        Console.WriteLine("Calling DB for resource ID : " + observable.ResourceId);
                                                        LogHandler.LogDebug(string.Format("Getting EndpointUri value from action table for actionTypeId:{0}", observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);
                                                        ActionDS action = new ActionDS();
                                                        var actionres = action.GetAny().ToArray().Where(a => a.ActionTypeId == observable.Actions.ActionTypeId).FirstOrDefault();
                                                        if (actionres != null)
                                                        {
                                                            LogHandler.LogDebug(string.Format("EndpointUri value:{0}", actionres.EndpointUri), LogHandler.Layer.Business, null);
                                                            //string endPointUri = actionres.EndpointUri;
                                                            string endPointUri = observable.Actions.ParameterDetails.Where(p => p.ParamaterName.ToLower() == "endpointuri").Select(s => s.ParameterValue).FirstOrDefault();
                                                            string[] objs = endPointUri.Split('/');
                                                            foreach (string s in objs.Where(x => !string.IsNullOrEmpty(x)).ToArray())
                                                            {
                                                                if (objs[2] == s)
                                                                {
                                                                    dbServer = s;
                                                                }
                                                                if (objs[3] == s)
                                                                {
                                                                    dbName = s;
                                                                }
                                                                if (objs[5] == s)
                                                                {
                                                                    viewName = s;
                                                                }
                                                            }
                                                            if (dbServer != null && dbName != null && viewName != null)
                                                            {
                                                                LogHandler.LogDebug(string.Format("Got the DBserver:{0},DBName:{1} & ViewName:{2} from EndpointUri value:{3}", dbServer, dbName, viewName, actionres.EndpointUri), LogHandler.Layer.Business, null);
                                                                LogHandler.LogDebug(string.Format("Calling ExecuteDBView method to get metric values"), LogHandler.Layer.Business, null);

                                                                DataTable dtResult = ExecuteDBView(dbServer, dbName, viewName, observable.Actions.ParameterDetails);
                                                                List<DataRow> dtlist = dtResult.AsEnumerable().ToList();

                                                                /*FacadeClient facadeClient = new FacadeClient();
                                                                Console.WriteLine("Created object for facade client");
                                                                List<DataRow> dtlist = facadeClient.GeHealthCheckPlatformDBMetrics();
                                                                Console.WriteLine("Fetched data from DB Adapter");
                                                                Console.WriteLine("No.of Rows fetched in facade framework:" + dtlist.Count);*/

                                                                var results = (from myRow in dtlist
                                                                               where myRow.Field<string>("bot_name").ToLower() == server.Name.ToLower() &&
                                                                               myRow.Field<string>("ipaddress") == observable.IPAddress &&
                                                                               myRow.Field<string>("metric_name").ToLower() == observable.Name.ToLower()
                                                                               select myRow).FirstOrDefault();
                                                                if (results != null)
                                                                {
                                                                    Console.WriteLine("Metric details found for bot_name:" + server.Name + " & IPAddress:" + server.IPAddress + " & metric_name:" + observable.Name);

                                                                    LogHandler.LogDebug("Metric details found for bot_name:" + platformInstanceDetails.VendorName + " & IPAddress:" + platformInstanceDetails.VendorName + " & metric_name:" + observable.Name, LogHandler.Layer.Business, null);
                                                                    LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                                    int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), observable.ResourceId, observable.ObservableId, Guid.Empty.ToString(), "success", healthcheckType, "NA");
                                                                    LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);

                                                                    DE.Queue.Metric metric = new DE.Queue.Metric();

                                                                    BE.Metric metricBE = new BE.Metric();
                                                                    metricBE.Application = platformInstanceDetails.VendorName;
                                                                    metricBE.EventType = results.Field<string>("event_type");
                                                                    metricBE.Description = string.IsNullOrEmpty(results.Field<string>("description")) ? "NA" : results.Field<string>("description");
                                                                    metricBE.MetricName = results.Field<string>("metric_name");
                                                                    metricBE.MetricValue = results.Field<string>("metric_value");
                                                                    metricBE.MetricTime = results.Field<string>("metric_time").ToString();
                                                                    metricBE.ResourceId = observable.ResourceId;
                                                                    metricBE.ServerIp = results.Field<string>("server_ip");
                                                                    //metricBE.Source = results.Field<string>("source");
                                                                    metricBE.Source = platformInstanceDetails.VendorName;

                                                                    //metricBE = PopulateMetricData(platformInstanceDetails.VendorName, "Server", "success", bot.BotInstanceId, observable.Name, "NA", healthcheckType);
                                                                    EntityTranslator translator = new EntityTranslator();
                                                                    metric = translator.MetricBEToDE(metricBE);

                                                                    bool isValid = checkNotificationTimeThreshold(PlatformId, metric.ResourceId, Convert.ToInt32(observable.ObservableId), metric.EventType, metric.ServerIp, metric.MetricValue);
                                                                    if (isValid)
                                                                    {
                                                                        LogHandler.LogDebug(string.Format("Sending metric data to queue and details are EventType:{0}, Application:{1}, MetricTime:{2}, ResourceId:{3}, MetricName:{4}, MetricValue:{5} ,ServerIp:{6}, Description:{7}, Source:{8}",
                                                                            metric.EventType, metric.Application, metric.MetricTime, metric.ResourceId, metric.MetricName, metric.MetricValue, metric.ServerIp, metric.Description, metric.Source),
                                                                            LogHandler.Layer.Business, null);

                                                                        MetricProcessorDS processorDS = new MetricProcessorDS();
                                                                        string msgResponse = processorDS.Send(metric, null);
                                                                        Console.WriteLine("Sending message to queue for resource Id " + platfromInstanceId + " Response Message : " + msgResponse);
                                                                        LogHandler.LogDebug(string.Format("Metric data sent to queue for resource Id {0} and response message is {1}", platfromInstanceId, msgResponse), LogHandler.Layer.Business, null);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Metric details not found in view for bot_name:" + server.Name + " & IPAddress:" + observable.IPAddress + " & metric_name:" + observable.Name);
                                                                    string errorMsg = "Metric details not found in view for bot_name:" + server.Name + " & IPAddress:" + observable.IPAddress + " & metric_name:" + observable.Name;
                                                                    LogHandler.LogWarning(errorMsg, LogHandler.Layer.Business, null);
                                                                    LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                                    int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), observable.ResourceId, observable.ObservableId, Guid.Empty.ToString(), "failed", healthcheckType, errorMsg);
                                                                    LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Invalid EndpointUri configured for resource ID:" + platfromInstanceId);
                                                                string errorMsg = "Invalid EndpointUri configured for resource ID:" + platfromInstanceId;
                                                                LogHandler.LogWarning(errorMsg, LogHandler.Layer.Business, null);
                                                                LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                                int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), observable.ResourceId, observable.ObservableId, Guid.Empty.ToString(), "failed", healthcheckType, errorMsg);
                                                                LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("EndpointUri not configured for resource ID:" + platfromInstanceId);
                                                            string errorMsg = "EndpointUri not configured for resource ID:" + platfromInstanceId;
                                                            LogHandler.LogWarning(errorMsg, LogHandler.Layer.Business, null);
                                                            LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                            int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), observable.ResourceId, observable.ObservableId, Guid.Empty.ToString(), "failed", healthcheckType, errorMsg);
                                                            LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);
                                                        }
                                                    }
                                                    else if (observable.Actions != null && observable.Actions.ActionName == "HealthCheck-PlatformAPI")
                                                    {
                                                        LogHandler.LogDebug(string.Format("Calling HealcheckPlatformAPI method for resourceID:{0} & ActionTypeId:{1}", platfromInstanceId, observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);
                                                        if (observable.Actions.ExecutionMode == null ? true : observable.Actions.ExecutionMode.Equals("SYNC", StringComparison.InvariantCultureIgnoreCase))
                                                        {
                                                            Console.WriteLine($"{observable.Actions.ActionId} is a sync operation. Waiting for all the async tasks to complete.");
                                                            Task.WhenAll(taskList.ToArray()).Wait();
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine($"{observable.Actions.ActionId} is an async operation. Executing");
                                                        }
                                                        taskList.Add(HealcheckPlatformAPI(Tenantid, healthcheckType, HealtcheckTrackingId, platformInstanceDetails, observable.Actions,
                                                            observable.ResourceId, observable.ObservableId, observable.IPAddress, observable.Name, server.Name, ResourceType.Server.ToString(), serverType, deviceList));
                                                        LogHandler.LogDebug(string.Format("Execution of HealcheckPlatformAPI method completed for resourceID:{0} & ActionTypeId:{1}", platfromInstanceId, observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);
                                                    }
                                                }
                                                else
                                                {
                                                    Console.WriteLine(string.Format("ResourceID:{0} details not found/de-activated", observable.ResourceId));
                                                    LogHandler.LogWarning(string.Format("ResourceID:{0} details not found/de-activated", observable.ResourceId), LogHandler.Layer.Business, null);
                                                }
                                            }                                            
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Server details not found for platform Id" + platfromInstanceId);
                                    LogHandler.LogWarning("Server details not found for platform health check", LogHandler.Layer.Business, null);
                                }

                                if (server.bots != null)
                                {
                                    LogHandler.LogDebug("Iterating through each bot in server:{0}", LogHandler.Layer.Business, server.Name);
                                    //Traverse through respective bot Nodes
                                    foreach (IE.Bot bot in server.bots)
                                    {
                                        if (!string.IsNullOrEmpty(bot.BotInstanceId))
                                        {
                                            if (string.IsNullOrEmpty(resourceIds) || resourceIdList.Contains(bot.BotInstanceId))
                                            {
                                                bool isActive = Convert.ToBoolean(resources.Where(r => r.ResourceId == bot.BotInstanceId).FirstOrDefault() != null ? resources.Where(r => r.ResourceId == bot.BotInstanceId).FirstOrDefault().IsActive : false);
                                                if (isActive)
                                                {
                                                    LogHandler.LogDebug("Iterating through each observable in bot:" + bot.BotInstanceId, LogHandler.Layer.Business, null);
                                                    //Traverse through respective BotObservable Nodes
                                                    foreach (IE.BotObservable observable in bot.botObservables)
                                                    {
                                                        if (observable.Actions != null && observable.Actions.ActionName == "HealthCheck-PlatformDB")
                                                        {
                                                            string dbServer = null, dbName = null, viewName = null;
                                                            Console.WriteLine("Calling DB for resource ID : " + bot.BotInstanceId);
                                                            LogHandler.LogDebug(string.Format("Getting EndpointUri value from action table for actionTypeId:{0}", observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);
                                                            ActionDS action = new ActionDS();
                                                            var actionres = action.GetAny().ToArray().Where(a => a.ActionTypeId == observable.Actions.ActionTypeId).FirstOrDefault();
                                                            if (actionres != null)
                                                            {
                                                                LogHandler.LogDebug(string.Format("EndpointUri value:{0}", actionres.EndpointUri), LogHandler.Layer.Business, null);
                                                                //string endPointUri = actionres.EndpointUri;
                                                                string endPointUri = observable.Actions.ParameterDetails.Where(p=>p.ParamaterName.ToLower() == "endpointuri").Select(s=>s.ParameterValue).FirstOrDefault();
                                                                string[] objs = endPointUri.Split('/');
                                                                foreach (string s in objs.Where(x => !string.IsNullOrEmpty(x)).ToArray())
                                                                {
                                                                    if (objs[2] == s)
                                                                    {
                                                                        dbServer = s;
                                                                    }
                                                                    if (objs[3] == s)
                                                                    {
                                                                        dbName = s;
                                                                    }
                                                                    if (objs[5] == s)
                                                                    {
                                                                        viewName = s;
                                                                    }
                                                                }

                                                                if (dbServer != null && dbName != null && viewName != null)
                                                                {
                                                                    LogHandler.LogDebug(string.Format("Got the DBserver:{0},DBName:{1} & ViewName:{2} from EndpointUri value:{3}", dbServer, dbName, viewName, actionres.EndpointUri), LogHandler.Layer.Business, null);
                                                                    LogHandler.LogDebug(string.Format("Calling ExecuteDBView method to get metric values"), LogHandler.Layer.Business, null);

                                                                    DataTable dtResult = ExecuteDBView(dbServer, dbName, viewName, observable.Actions.ParameterDetails);
                                                                    List<DataRow> dtlist = dtResult.AsEnumerable().ToList();

                                                                    /*FacadeClient facadeClient = new FacadeClient();
                                                                    Console.WriteLine("Created object for facade client");
                                                                    string facadeResponse = facadeClient.TestFacadeClient();
                                                                    Console.WriteLine("Test facade response Msg: " + facadeResponse);
                                                                    List<DataRow> dtlist = facadeClient.GeHealthCheckPlatformDBMetrics();
                                                                    Console.WriteLine("Fetched data from DB Adapter");
                                                                    Console.WriteLine("No.of Rows fetched in facade framework:" + dtlist.Count);
                                                                    */
                                                                    var results = (from myRow in dtlist.AsEnumerable()
                                                                                   where myRow.Field<string>("bot_name").ToLower() == bot.BotName.ToLower() &&
                                                                                   myRow.Field<string>("ipaddress") == bot.IPAddress &&
                                                                                   myRow.Field<string>("metric_name").ToLower() == observable.Name.ToLower()
                                                                                   select myRow).FirstOrDefault();
                                                                    if (results != null)
                                                                    {
                                                                        Console.WriteLine("Metric details found for bot_name:" + bot.BotName + " & IPAddress:" + bot.IPAddress + " & metric_name:" + observable.Name);

                                                                        LogHandler.LogDebug("Metric details found for bot_name:" + platformInstanceDetails.VendorName + " & IPAddress:" + platformInstanceDetails.VendorName + " & metric_name:" + observable.Name, LogHandler.Layer.Business, null);
                                                                        LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                                        int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), bot.BotInstanceId, observable.ObservableId, Guid.Empty.ToString(), "success", healthcheckType, "NA");
                                                                        LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);

                                                                        DE.Queue.Metric metric = new DE.Queue.Metric();

                                                                        BE.Metric metricBE = new BE.Metric();
                                                                        metricBE.Application = platformInstanceDetails.VendorName;
                                                                        metricBE.EventType = results.Field<string>("event_type");
                                                                        metricBE.Description = string.IsNullOrEmpty(results.Field<string>("description")) ? "NA" : results.Field<string>("description");
                                                                        metricBE.MetricName = results.Field<string>("metric_name");
                                                                        metricBE.MetricValue = results.Field<string>("metric_value");
                                                                        metricBE.MetricTime = Convert.ToString(results.Field<string>("metric_time"));
                                                                        metricBE.ResourceId = bot.BotInstanceId;
                                                                        metricBE.ServerIp = results.Field<string>("server_ip");
                                                                        //metricBE.Source = results.Field<string>("source");
                                                                        metricBE.Source = platformInstanceDetails.VendorName;

                                                                        //metricBE = PopulateMetricData(platformInstanceDetails.VendorName, "Server", "success", bot.BotInstanceId, observable.Name, "NA", healthcheckType);
                                                                        EntityTranslator translator = new EntityTranslator();
                                                                        metric = translator.MetricBEToDE(metricBE);

                                                                        bool isValid = checkNotificationTimeThreshold(PlatformId, metric.ResourceId, Convert.ToInt32(observable.ObservableId), metric.EventType, metric.ServerIp, metric.MetricValue);
                                                                        if (isValid)
                                                                        {
                                                                            LogHandler.LogDebug(string.Format("Sending metric data to queue and details are EventType:{0}, Application:{1}, MetricTime:{2}, ResourceId:{3}, MetricName:{4}, MetricValue:{5} ,ServerIp:{6}, Description:{7}, Source:{8}",
                                                                            metric.EventType, metric.Application, metric.MetricTime, metric.ResourceId, metric.MetricName, metric.MetricValue, metric.ServerIp, metric.Description, metric.Source),
                                                                            LogHandler.Layer.Business, null);

                                                                            MetricProcessorDS processorDS = new MetricProcessorDS();
                                                                            string msgResponse = processorDS.Send(metric, null);
                                                                            Console.WriteLine("Sending message to queue for resource Id " + platfromInstanceId + " Response Message : " + msgResponse);
                                                                            LogHandler.LogDebug(string.Format("Metric data sent to queue for resource Id {0} and response message is {1}", platfromInstanceId, msgResponse), LogHandler.Layer.Business, null);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        Console.WriteLine("Metric details not found in view for bot_name:" + bot.BotName + " & IPAddress:" + bot.IPAddress + " & metric_name:" + observable.Name);
                                                                        string errorMsg = "Metric details not found in view for bot_name:" + bot.BotName + " & IPAddress:" + bot.IPAddress + " & metric_name:" + observable.Name;
                                                                        LogHandler.LogWarning(errorMsg, LogHandler.Layer.Business, null);
                                                                        LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                                        int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), bot.BotInstanceId, observable.ObservableId, Guid.Empty.ToString(), "failed", healthcheckType, errorMsg);
                                                                        LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);
                                                                    }

                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Invalid EndpointUri configured for resource ID:" + platfromInstanceId);
                                                                    string errorMsg = "Invalid EndpointUri configured for resource ID:" + platfromInstanceId;
                                                                    LogHandler.LogWarning(errorMsg, LogHandler.Layer.Business, null);
                                                                    LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                                    int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), bot.BotInstanceId, observable.ObservableId, Guid.Empty.ToString(), "failed", healthcheckType, errorMsg);
                                                                    LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                string errorMsg = "Action details not found for actiontype:" + observable.Actions.ActionTypeId;
                                                                Console.WriteLine(errorMsg);
                                                                LogHandler.LogWarning(errorMsg, LogHandler.Layer.Business, null);
                                                                LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                                int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), bot.BotInstanceId, observable.ObservableId, Guid.Empty.ToString(), "failed", healthcheckType, errorMsg);
                                                                LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);
                                                            }
                                                        }
                                                        else if (observable.Actions != null && observable.Actions.ActionName == "HealthCheck-PlatformAPI")
                                                        {
                                                            LogHandler.LogDebug(string.Format("Calling HealcheckPlatformAPI method for resourceID:{0} & ActionTypeId:{1}", platfromInstanceId, observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);
                                                            if (observable.Actions.ExecutionMode == null ? true : observable.Actions.ExecutionMode.Equals("SYNC", StringComparison.InvariantCultureIgnoreCase))
                                                            {
                                                                Console.WriteLine($"{observable.Actions.ActionId} is a sync operation. Waiting for all the async tasks to complete.");
                                                                Task.WhenAll(taskList.ToArray()).Wait();
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine($"{observable.Actions.ActionId} is an async operation. Executing");
                                                            }
                                                            taskList.Add(HealcheckPlatformAPI(Tenantid, healthcheckType, HealtcheckTrackingId, platformInstanceDetails, observable.Actions,
                                                            bot.BotInstanceId, observable.ObservableId, server.IPAddress, observable.Name, server.Name, ResourceType.BOT.ToString(), serverType, deviceList));
                                                            LogHandler.LogDebug(string.Format("Execution of HealcheckPlatformAPI method completed for resourceID:{0} & ActionTypeId:{1}", platfromInstanceId, observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);
                                                        }

                                                    }
                                                }
                                                else
                                                {
                                                    Console.WriteLine(string.Format("ResourceID:{0} details not found/de-activated", bot.BotInstanceId));
                                                    LogHandler.LogWarning(string.Format("ResourceID:{0} details not found/de-activated", bot.BotInstanceId), LogHandler.Layer.Business, null);
                                                }
                                            }                                            
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Bot details not found for platform Id" + platfromInstanceId);
                                    LogHandler.LogWarning("Bot details not found for platform Id" + platfromInstanceId, LogHandler.Layer.Business, null);
                                }

                                LogHandler.LogDebug("Iterating through each service in server:{0}", LogHandler.Layer.Business, server.Name);
                                //Traverse through respective Service Nodes
                                foreach (IE.Service service in server.services)
                                {
                                    if (!string.IsNullOrEmpty(service.ServiceId))
                                    {
                                        if(string.IsNullOrEmpty(resourceIds) || resourceIdList.Contains(service.ServiceId))
                                        {
                                            bool isActive = Convert.ToBoolean(resources.Where(r => r.ResourceId == service.ServiceId).FirstOrDefault() != null ? resources.Where(r => r.ResourceId == service.ServiceId).FirstOrDefault().IsActive : false);
                                            if (isActive)
                                            {
                                                LogHandler.LogDebug("Iterating through each observable in service:{0}", LogHandler.Layer.Business, service.ServiceId);
                                                //Traverse through respective ServiceObservable Nodes
                                                foreach (IE.ServiceObservable observable in service.serviceObservables)
                                                {
                                                    if (observable.Actions != null && observable.Actions.ActionName == "HealthCheck-PlatformDB")
                                                    {
                                                        string dbServer = null, dbName = null, viewName = null;
                                                        Console.WriteLine("Calling DB for resource ID : " + service.ServiceId);
                                                        LogHandler.LogDebug(string.Format("Getting EndpointUri value from action table for actionTypeId:{0}", observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);
                                                        ActionDS action = new ActionDS();
                                                        var actionres = action.GetAny().ToArray().Where(a => a.ActionTypeId == observable.Actions.ActionTypeId).FirstOrDefault();
                                                        if (actionres != null)
                                                        {
                                                            LogHandler.LogDebug(string.Format("EndpointUri value:{0}", actionres.EndpointUri), LogHandler.Layer.Business, null);
                                                            //string endPointUri = actionres.EndpointUri;
                                                            string endPointUri = observable.Actions.ParameterDetails.Where(p => p.ParamaterName.ToLower() == "endpointuri").Select(s => s.ParameterValue).FirstOrDefault();
                                                            string[] objs = endPointUri.Split('/');
                                                            foreach (string s in objs.Where(x => !string.IsNullOrEmpty(x)).ToArray())
                                                            {
                                                                if (objs[2] == s)
                                                                {
                                                                    dbServer = s;
                                                                }
                                                                if (objs[3] == s)
                                                                {
                                                                    dbName = s;
                                                                }
                                                                if (objs[5] == s)
                                                                {
                                                                    viewName = s;
                                                                }
                                                            }
                                                            if (dbServer != null && dbName != null && viewName != null)
                                                            {
                                                                LogHandler.LogDebug(string.Format("Got the DBserver:{0},DBName:{1} & ViewName:{2} from EndpointUri value:{3}", dbServer, dbName, viewName, actionres.EndpointUri), LogHandler.Layer.Business, null);
                                                                LogHandler.LogDebug(string.Format("Calling ExecuteDBView method to get metric values"), LogHandler.Layer.Business, null);

                                                                DataTable dtResult = ExecuteDBView(dbServer, dbName, viewName, observable.Actions.ParameterDetails);
                                                                List<DataRow> dtlist = dtResult.AsEnumerable().ToList();

                                                                //List<DataRow> dtlist = dtResult.AsEnumerable().ToList();
                                                                /* FacadeClient facadeClient = new FacadeClient();
                                                                 Console.WriteLine("Created object for facade client");
                                                                 List<DataRow> dtlist = facadeClient.GeHealthCheckPlatformDBMetrics();
                                                                 Console.WriteLine("Fetched data from DB Adapter");
                                                                 Console.WriteLine("No.of Rows fetched in facade framework:" + dtlist.Count);
                                                                 */
                                                                var results = (from myRow in dtlist.AsEnumerable()
                                                                               where myRow.Field<string>("bot_name").ToLower() == service.ServiceName.ToLower() &&
                                                                               myRow.Field<string>("ipaddress") == service.IPAddress &&
                                                                               myRow.Field<string>("metric_name").ToLower() == observable.Name.ToLower()
                                                                               select myRow).FirstOrDefault();
                                                                if (results != null)
                                                                {

                                                                    Console.WriteLine("Metric details found for bot_name:" + service.ServiceName + " & IPAddress:" + service.IPAddress + " & metric_name:" + observable.Name);
                                                                    LogHandler.LogDebug("Metric details found for bot_name:" + platformInstanceDetails.VendorName + " & IPAddress:" + platformInstanceDetails.VendorName + " & metric_name:" + observable.Name, LogHandler.Layer.Business, null);
                                                                    LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                                    int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), service.ServiceId, observable.ObservableId, Guid.Empty.ToString(), "success", healthcheckType, "NA");
                                                                    LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);

                                                                    DE.Queue.Metric metric = new DE.Queue.Metric();

                                                                    BE.Metric metricBE = new BE.Metric();
                                                                    metricBE.Application = platformInstanceDetails.VendorName;
                                                                    metricBE.EventType = results.Field<string>("event_type");
                                                                    metricBE.Description = string.IsNullOrEmpty(results.Field<string>("description")) ? "NA" : results.Field<string>("description");
                                                                    metricBE.MetricName = results.Field<string>("metric_name");
                                                                    metricBE.MetricValue = results.Field<string>("metric_value");
                                                                    metricBE.MetricTime = results.Field<string>("metric_time");
                                                                    metricBE.ResourceId = service.ServiceId;
                                                                    metricBE.ServerIp = results.Field<string>("server_ip");
                                                                    //metricBE.Source = results.Field<string>("source");
                                                                    metricBE.Source = platformInstanceDetails.VendorName;

                                                                    //metricBE = PopulateMetricData(platformInstanceDetails.VendorName, "Server", "success", bot.BotInstanceId, observable.Name, "NA", healthcheckType);
                                                                    EntityTranslator translator = new EntityTranslator();
                                                                    metric = translator.MetricBEToDE(metricBE);

                                                                    bool isValid = checkNotificationTimeThreshold(PlatformId, metric.ResourceId, Convert.ToInt32(observable.ObservableId), observable.Name, metric.ServerIp, metric.MetricValue);
                                                                    if (isValid)
                                                                    {
                                                                        LogHandler.LogDebug(string.Format("Sending metric data to queue and details are EventType:{0}, Application:{1}, MetricTime:{2}, ResourceId:{3}, MetricName:{4}, MetricValue:{5} ,ServerIp:{6}, Description:{7}, Source:{8}",
                                                                            metric.EventType, metric.Application, metric.MetricTime, metric.ResourceId, metric.MetricName, metric.MetricValue, metric.ServerIp, metric.Description, metric.Source),
                                                                            LogHandler.Layer.Business, null);

                                                                        MetricProcessorDS processorDS = new MetricProcessorDS();
                                                                        string msgResponse = processorDS.Send(metric, null);
                                                                        Console.WriteLine("Sending message to queue for resource Id " + platfromInstanceId + " Response Message : " + msgResponse);
                                                                        LogHandler.LogDebug(string.Format("Metric data sent to queue for resource Id {0} and response message is {1}", platfromInstanceId, msgResponse), LogHandler.Layer.Business, null);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    Console.WriteLine("Metric details not found in view for bot_name:" + service.ServiceName + " & IPAddress:" + service.IPAddress + " & metric_name:" + observable.Name);
                                                                    string errorMsg = "Metric details not found in view for bot_name:" + service.ServiceName + " & IPAddress:" + service.IPAddress + " & metric_name:" + observable.Name;
                                                                    LogHandler.LogWarning(errorMsg, LogHandler.Layer.Business, null);
                                                                    LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                                    int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), service.ServiceId, observable.ObservableId, results.Field<string>("activity_unique_id"), "failed", healthcheckType, errorMsg);
                                                                    LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                Console.WriteLine("Invalid EndpointUri configured for resource ID:" + platfromInstanceId);
                                                                string errorMsg = "Invalid EndpointUri configured for resource ID:" + platfromInstanceId;
                                                                LogHandler.LogWarning(errorMsg, LogHandler.Layer.Business, null);
                                                                LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                                int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), service.ServiceId, observable.ObservableId, null, "failed", healthcheckType, errorMsg);
                                                                LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            string errorMsg = "Action details not found for actiontype:" + observable.Actions.ActionTypeId;
                                                            Console.WriteLine(errorMsg);
                                                            LogHandler.LogWarning(errorMsg, LogHandler.Layer.Business, null);
                                                            LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                                                            int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), service.ServiceId, observable.ObservableId, null, "failed", healthcheckType, errorMsg);
                                                            LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);
                                                        }
                                                    }
                                                    else if (observable.Actions != null && observable.Actions.ActionName.Equals("HealthCheck-PlatformAPI", StringComparison.InvariantCultureIgnoreCase))
                                                    {
                                                        string resourceTypeVal = "";
                                                        if (serverType != ResourceType.DBServer.ToString())
                                                        {
                                                            resourceTypeVal = ResourceType.Services.ToString();
                                                        }
                                                        else
                                                        {
                                                            resourceTypeVal = ResourceType.DBService.ToString();
                                                        }
                                                        LogHandler.LogDebug(string.Format("Calling HealcheckPlatformAPI method for resourceID:{0} & ActionTypeId:{1}", platfromInstanceId, observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);
                                                        if (observable.Actions.ExecutionMode == null ? true : observable.Actions.ExecutionMode.Equals("SYNC", StringComparison.InvariantCultureIgnoreCase))
                                                        {
                                                            Console.WriteLine($"{observable.Actions.ActionId} is a sync operation. Waiting for all the async tasks to complete.");
                                                            Task.WhenAll(taskList.ToArray()).Wait();
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine($"{observable.Actions.ActionId} is an async operation. Executing");
                                                        }
                                                        taskList.Add(HealcheckPlatformAPI(Tenantid, healthcheckType, HealtcheckTrackingId, platformInstanceDetails, observable.Actions,
                                                                service.ServiceId, observable.ObservableId, server.IPAddress, observable.Name, server.Name, resourceTypeVal, serverType, deviceList));
                                                        LogHandler.LogDebug(string.Format("Execution of HealcheckPlatformAPI method completed for resourceID:{0} & ActionTypeId:{1}", platfromInstanceId, observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);
                                                    }

                                                }
                                            }
                                            else
                                            {
                                                Console.WriteLine(string.Format("ResourceID:{0} details not found/de-activated", service.ServiceId));
                                                LogHandler.LogWarning(string.Format("ResourceID:{0} details not found/de-activated", service.ServiceId), LogHandler.Layer.Business, null);
                                            }
                                        }                                        
                                    }
                                }
                            }
                            LogHandler.LogDebug("calling HealthcheckCompleted to update execution status value as success for TrackingId:" + HealtcheckTrackingId, LogHandler.Layer.Business, null);

                            //wait for tasks to complete before calling completion method
                            Task.WhenAll(taskList.ToArray()).Wait();

                            //call HealthcheckCompleted  to update the execution status
                            HealthcheckCompleted(HealtcheckTrackingId, "success", null);
                            LogHandler.LogDebug("Updated the status to success for TrackingId:" + HealtcheckTrackingId, LogHandler.Layer.Business, null);

                        }
                        else
                        {
                            LogHandler.LogWarning("Server details not found for platform health check", LogHandler.Layer.Business, null);
                        }

                    }
                    else
                    {
                        LogHandler.LogError(ErrorMessages.ValuesInsertionUnsuccessful, LogHandler.Layer.Business, null);
                    }
                }
                else
                {
                    LogHandler.LogError("Platform Instance details not found for platform=" + PlatformId, LogHandler.Layer.Business, null);

                    SuperbotDataItemNotFoundException dataNotfoundException = new SuperbotDataItemNotFoundException(string.Format(ErrorMessages.Platform_Data_NotFound, PlatformId, Tenantid));
                    List<ValidationError> validateErrs = new List<ValidationError>();
                    ValidationError validationErr = new ValidationError();
                    validationErr.Code = "1041";
                    validationErr.Description = string.Format(ErrorMessages.Platform_Data_NotFound, PlatformId, Tenantid);
                    validateErrs.Add(validationErr);

                    if (validateErrs.Count > 0)
                    {
                        dataNotfoundException.Data.Add("DataNotFoundErrors", validateErrs);
                        throw dataNotfoundException;
                    }
                }
            }
            catch (Exception superBotException)
            {
                Console.WriteLine("Exception occured while performing platform health check. Error Message = " + superBotException.Message + superBotException.StackTrace);
                LogHandler.LogError("Exception occured while performing platform health check for platformID" + PlatformId + " & Error Message = " + superBotException.Message,
                    LogHandler.Layer.Business, null);

                if (HealtcheckTrackingId > 0)
                {
                    //wait for tasks to complete before calling completion method
                    Task.WhenAll(taskList.ToArray()).Wait();

                    LogHandler.LogDebug("calling HealthcheckCompleted to update execution status value as failed for TrackingId:" + HealtcheckTrackingId, LogHandler.Layer.Business, null);
                    HealthcheckCompleted(HealtcheckTrackingId, "failed", superBotException.Message);
                    LogHandler.LogDebug("Updated the status to failed for TrackingId:" + HealtcheckTrackingId, LogHandler.Layer.Business, null);
                }
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(superBotException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "PlatformHealthcheck", "Monitor"), LogHandler.Layer.Business, null);
            return HealtcheckTrackingId;
        }

        public bool checkNotificationTimeThreshold(int platformId, string resourceId, int observableId, string observablename, string sourceIp, string observableValue)
        {
            bool isValid = true;
            try
            {
                /*PlatformsDS platformsDS = new PlatformsDS();
                var platform=platformsDS.GetAny().ToArray().Where(p => p.PlatformId == platformId).FirstOrDefault();*/
                ResourceAttributesDS attributesDS = new ResourceAttributesDS();
                var platform = attributesDS.GetAny().ToArray().Where(p => p.ResourceId == Convert.ToString(platformId) && p.AttributeName.ToLower() == "notificationtimethreshold".ToLower()).FirstOrDefault();
                if (platform != null)
                {
                    int thresholdVal = Convert.ToInt32(platform.AttributeValue);
                    ObservationsDS observationsDS = new ObservationsDS();
                    var observation = observationsDS.GetAll().Where(o => o.PlatformId == platformId &&
                                                                        o.ResourceId == resourceId &&
                                                                        //o.ResourceTypeId == resourceTypeId &&
                                                                        o.ObservableId == observableId &&
                                                                        o.ObservableName == observablename &&
                                                                        o.Value == observableValue &&
                                                                        o.SourceIp == sourceIp).FirstOrDefault();
                    if (observation != null && observation.RemediationStatus != null)
                    {
                        if (observation.NotifiedTime != null && string.Equals(observation.RemediationStatus, "FAILED", StringComparison.OrdinalIgnoreCase))
                        {
                            DateTime notificationTime = Convert.ToDateTime(observation.NotifiedTime);
                            double diffInSeconds = (DateTime.Now - notificationTime).TotalMinutes;
                            if (diffInSeconds < thresholdVal)
                                isValid = false;

                        }
                        else if (string.Equals(observation.RemediationStatus, "In Progress", StringComparison.OrdinalIgnoreCase) || string.Equals(observation.RemediationStatus, "Not Started", StringComparison.OrdinalIgnoreCase))
                        {
                            isValid = false;
                        }
                    }
                    //else if(observation != null && observation.NotifiedTime == null)
                    //{
                    //    LogHandler.LogWarning("Notificationtime is empty in observations table for resourceid:{0}", LogHandler.Layer.Business, resourceId);
                    //    //isValid = false;
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return isValid;
        }

        private async Task<bool> HealcheckPlatformAPI(int Tenantid, string healthcheckType, int HealtcheckTrackingId, IE.PlatformInstanceDetails platformInstanceDetails,
            IE.Actions obsActionId, string obsResourceId, string obsObservableId, string resIPAddress, string obsName, string serverName, string resourceType, string serverType, BusinessEntity.DeviceList devices)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "HealcheckPlatformAPI", "Monitor"), LogHandler.Layer.Business, null);
            string transcationId = Guid.Empty.ToString();
            string successMsg = "";
            string ErrorMsg = "";
            if (devices == null)
            {
                InitiateExecutionReqMsg executionReqMsg = new InitiateExecutionReqMsg();
                IE.ScriptIdentifier script = new IE.ScriptIdentifier();
                script.ScriptId = obsActionId.ScriptId;
                script.CategoryId = obsActionId.CategoryId;
                script.CompanyId = Tenantid;
                List<string> auditParams = new List<string>();
                if (obsActionId.ParameterDetails != null)
                {
                    List<IE.Parameter> parametersSE = new List<IE.Parameter>();
                    foreach (IE.ObservableParameters parameters in obsActionId.ParameterDetails)
                    {                              
                        IE.Parameter param = new IE.Parameter();
                        param.ParameterName = parameters.ParamaterName;
                        param.ParameterValue = parameters.IsSecret != null && Convert.ToBoolean(parameters.IsSecret) ? SecureData.UnSecure(parameters.ParameterValue, ApplicationConstants.SecureKeys.IAP2) : parameters.ParameterValue;
                        parametersSE.Add(param);
                        auditParams.Add(param.ParameterName + "=" + param.ParameterValue);
                    }
                    script.Parameters = parametersSE;
                }
                executionReqMsg.ScriptIdentifier = script;

                Console.WriteLine("Calling SEE for resource ID : " + obsResourceId);
                LogHandler.LogDebug(string.Format("Executing SEE with values scriptId:{0} & categoryId:{1}", obsActionId.ScriptId, obsActionId.CategoryId), LogHandler.Layer.Business, null);

                ScriptExecute scriptExecute = new ScriptExecute();
                //WEMProxy scriptExecute = new WEMProxy();
                var channel1 = scriptExecute.ServiceChannel;
                InitiateExecutionResMsg response;

                //get the execution mode for this action
                bool isAsync = obsActionId.ExecutionMode == null ? false : obsActionId.ExecutionMode.Equals("ASYNC", StringComparison.InvariantCultureIgnoreCase);                               

                if (isAsync)
                    response = await channel1.AsyncInitiateExecution(executionReqMsg);
                else
                    response = channel1.InitiateExecution(executionReqMsg);

                #region Log Audit Data
                BE.AuditLog auditLogObj = new AuditLog();
                auditLogObj.ResourceID = obsResourceId;
                auditLogObj.ObservableID = Convert.ToInt32(obsObservableId);
                auditLogObj.ActionID = obsActionId.ActionId;
                auditLogObj.ActionParams = string.Join(",", auditParams);
                auditLogObj.PlatformID = platformInstanceDetails.PlatformId;
                auditLogObj.TenantID = Tenantid;
                #endregion

                if (response != null && response.ScriptResponse != null && response.ScriptResponse.FirstOrDefault().SuccessMessage.ToLower() != null)
                {
                    string[] stringSeparators = new string[] { "\r\n" };
                    string[] lines = response.ScriptResponse.FirstOrDefault().SuccessMessage.Split(stringSeparators, StringSplitOptions.None);
                    bool isExists = Array.Exists(lines, E => E.Contains("status=success"));
                    string responseSuccess = response.ScriptResponse.FirstOrDefault().SuccessMessage.Replace("status=success", string.Empty).Trim();
                    responseSuccess = responseSuccess.Replace("deployeddevicesstatus:", "");
                    if (isExists)
                    {
                        auditLogObj.Status = "SUCCESS";
                        auditLogObj.Output = response.ScriptResponse.FirstOrDefault().SuccessMessage;
                        Console.WriteLine("Script execution success for resource ID:" + obsResourceId);
                        LogHandler.LogDebug("Script execution success for resource ID:" + obsResourceId, LogHandler.Layer.Business, null);
                        DE.Queue.Metric metric = new DE.Queue.Metric();
                        BE.Metric metricBE = null;

                        if (responseSuccess.Contains("DevicesStatus:"))
                        {
                            responseSuccess = responseSuccess.Replace("DevicesStatus:", "").Replace("@odata.", "");
                            var uiPathResponse = JsonConvert.DeserializeObject<BusinessEntity.UiPathDeviceList>(responseSuccess);
                            devices = deviceList = Translator.Translator_IEtoBE.UiPathDeviceList_DeviceList(uiPathResponse);
                            metricBE = PopulateMetricDataHealth(devices, resIPAddress, resIPAddress, platformInstanceDetails.VendorName, resourceType, obsResourceId, obsName, healthcheckType, resIPAddress);
                        }
                        else if (responseSuccess.Contains("JobStatus:"))
                        {
                            responseSuccess = responseSuccess.Replace("JobStatus:", "").Replace("@odata.", "");
                            var uiPathProcessResponse = JsonConvert.DeserializeObject<BusinessEntity.UiPathProcessList>(responseSuccess);
                            metricBE = new BE.Metric();
                            if (obsName.ToLower().Replace(" ", "") == "botstatus")
                            {
                                metricBE.MetricValue = uiPathProcessResponse.value != null && uiPathProcessResponse.value.FirstOrDefault() != null ? uiPathProcessResponse.value.FirstOrDefault().State : "";
                            }
                            else if (obsName.ToLower().Replace(" ", "") == "botrunningtime")
                            {
                                DateTime endtime = DateTime.Parse(uiPathProcessResponse.value.FirstOrDefault().EndTime);
                                DateTime starttime = DateTime.Parse(uiPathProcessResponse.value.FirstOrDefault().StartTime);
                                double diff = endtime.Subtract(starttime).TotalSeconds;
                                metricBE.MetricValue = Math.Round(diff).ToString();
                            }
                            //metricBE.MetricValue = uiPathProcessResponse.value!=null && uiPathProcessResponse.value.FirstOrDefault()!=null? uiPathProcessResponse.value.FirstOrDefault().State:"";
                            metricBE.ServerIp = resIPAddress;
                            metricBE.Description = "NA";
                            metricBE.Application = platformInstanceDetails.VendorName;
                            metricBE.EventType = resourceType;
                            metricBE.MetricTime = DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt");
                            metricBE.ResourceId = obsResourceId;
                            metricBE.MetricName = obsName;
                            metricBE.Source = healthcheckType;
                        }
                        else
                        {
                            devices = deviceList = JsonConvert.DeserializeObject<BusinessEntity.DeviceList>(responseSuccess);
                            metricBE = PopulateMetricDataHealth(devices, serverName, resIPAddress, platformInstanceDetails.VendorName, resourceType, obsResourceId, obsName, healthcheckType, serverType);
                        }

                        transcationId = response.ScriptResponse.FirstOrDefault().TransactionId;
                        successMsg = response.ScriptResponse.FirstOrDefault().SuccessMessage;
                        ErrorMsg = response.ScriptResponse.FirstOrDefault().ErrorMessage;

                        if (metricBE != null)
                        {
                            EntityTranslator translator = new EntityTranslator();
                            metric = translator.MetricBEToDE(metricBE);
                            bool isValid = checkNotificationTimeThreshold(platformInstanceDetails.PlatformId, metric.ResourceId, Convert.ToInt32(obsObservableId), metric.EventType, metric.ServerIp, metric.MetricValue);
                            if (isValid)
                            {
                                LogHandler.LogDebug(string.Format("Sending metric data to queue and details are EventType:{0}, Application:{1}, MetricTime:{2}, ResourceId:{3}, MetricName:{4}, MetricValue:{5} ,ServerIp:{6}, Description:{7}, Source:{8}",
                                                            metric.EventType, metric.Application, metric.MetricTime, metric.ResourceId, metric.MetricName, metric.MetricValue, metric.ServerIp, metric.Description, metric.Source),
                                                            LogHandler.Layer.Business, null);

                                MetricProcessorDS processorDS = new MetricProcessorDS();
                                string msgResponse = processorDS.Send(metric, null);
                                Console.WriteLine("Sending message to queue for resource Id" + obsResourceId + "Response Message : " + msgResponse);
                            }
                            LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                            int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), obsResourceId, obsObservableId, transcationId, "success", healthcheckType,
                                successMsg);
                            LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);

                        }
                        else
                        {
                            string errorMsg = "Metric details not found for server_name:" + serverName + " & IPAddress:" + resIPAddress + " & metric_name:" + obsName;
                            Console.WriteLine(errorMsg);
                            LogHandler.LogDebug(errorMsg, LogHandler.Layer.Business, null);
                            LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                            int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), obsResourceId, obsObservableId, transcationId, "failed", healthcheckType,
                                errorMsg);
                            LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);

                        }
                        deviceList = null;
                    }
                    else
                    {
                        auditLogObj.Status = "FAILED";
                        auditLogObj.Output = response.ScriptResponse.FirstOrDefault().SuccessMessage;
                        Console.WriteLine("Script execution failed for resource ID:" + obsResourceId + "Message " + response.ScriptResponse.FirstOrDefault().SuccessMessage);
                        LogHandler.LogDebug("Script execution failed for resource ID:" + obsResourceId, LogHandler.Layer.Business, null);
                        LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                        int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), obsResourceId, obsObservableId, response.ScriptResponse.FirstOrDefault().TransactionId, "failed", healthcheckType,
                            string.IsNullOrEmpty(response.ScriptResponse.FirstOrDefault().ErrorMessage) ? response.ScriptResponse.FirstOrDefault().SuccessMessage : response.ScriptResponse.FirstOrDefault().ErrorMessage);
                        LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);
                    }
                }
                else
                {
                    auditLogObj.Status = "FAILED";
                    auditLogObj.Output = response.ScriptResponse.FirstOrDefault().ErrorMessage != null ? response.ScriptResponse.FirstOrDefault().ErrorMessage : response.ScriptResponse.FirstOrDefault().SuccessMessage;
                    Console.WriteLine("Script execution failed for resource ID:" + obsResourceId);
                    LogHandler.LogDebug("Script execution failed for resource ID:" + obsResourceId, LogHandler.Layer.Business, null);
                    LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                    int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), obsResourceId, obsObservableId, response.ScriptResponse.FirstOrDefault().TransactionId, "failed", healthcheckType,
                        string.IsNullOrEmpty(response.ScriptResponse.FirstOrDefault().ErrorMessage) ? response.ScriptResponse.FirstOrDefault().SuccessMessage : response.ScriptResponse.FirstOrDefault().ErrorMessage);
                    LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);
                }
                Audit_Log audit = new Audit_Log();
                var isLogged = audit.LogAuditData(auditLogObj);
            }


            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "HealcheckPlatformAPI", "Monitor"), LogHandler.Layer.Business, null);

            return true;

        }

        public PlatformInstanceDetails GetAllBotDependencies(string PlatformInstanceId, int TenantId, string dependencyResourceID)
        {
            try
            {
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GetAllBotDependencies", "Monitor"), LogHandler.Layer.Business, null);

                ActionDS actDS = new ActionDS();
                ActionParamsDS actpDS = new ActionParamsDS();
                ObservableDS obsDS = new ObservableDS();
                ObservableResourceMapDS ormDS = new ObservableResourceMapDS();
                PlatformsDS platformsDS = new PlatformsDS();
                ResourceDS resDS = new ResourceDS();
                ResourceTypeDS typeDS = new ResourceTypeDS();
                ResourceDependencyMapDS rdmDS = new ResourceDependencyMapDS();
                ResourceObservableActionMapDS resObsActMapDS = new ResourceObservableActionMapDS();
                ResourcetypeObservableActionMapDS roamDS = new ResourcetypeObservableActionMapDS();
                ResourceAttributesDS raDs = new ResourceAttributesDS();
                ActionTypeDS actTypeDS = new ActionTypeDS();
                RemediationPlanActionMapDS remPlanActionMapDS = new RemediationPlanActionMapDS();
                ResourceDependencyMapDSExtn rdmDSExtn = new ResourceDependencyMapDSExtn();
                List<string> botsTemp = ConfigurationManager.AppSettings["BotResTypeNames"].Split(',').ToList();
                List<string> SevicesTemp = ConfigurationManager.AppSettings["ServiceResTypeNames"].Split(',').ToList();
                //List<string> botsTemp = new List<string>()
                //{
                //    "bot",
                //    "process"
                //};
                //List<string> SevicesTemp = new List<string>()
                //{
                //    "services",
                //    "db service",
                //    "queue service",
                //    "performance testing"
                //};

                LogHandler.LogDebug("Created objects for DS classes to fetch data from DB", LogHandler.Layer.Business, null);
                var resourceAttribute = raDs.GetAny().ToArray();

                LogHandler.LogDebug("Fetching resource dependency details for resourceId(s):{0} & tenantId:{1}", LogHandler.Layer.Business, dependencyResourceID, TenantId);                

                List<string> depIdList = dependencyResourceID.Split(',').ToList();
                List<ResourceDependencyDetails> resourcehierarchy = new List<ResourceDependencyDetails>();
                foreach (var depId in depIdList)
                {
                    resourcehierarchy.AddRange((from h in rdmDSExtn.HierarchyResources(depId, Convert.ToString(TenantId))
                                                select new ResourceDependencyDetails
                                                {
                                                    Resourceid = h.Resourceid,
                                                    DependencyResourceId = h.DependencyResourceId,
                                                    DependencyType = h.DependencyType
                                                }).ToList());
                }

                resourcehierarchy = resourcehierarchy.GroupBy(rh => rh.Resourceid).Select(g=>g.First()).OrderBy(o=>o.Resourceid).ToList();
                
                LogHandler.LogDebug("{0} records resource dependency details fetched for resourceId(s):{1} & tenantId:{2}", LogHandler.Layer.Business, resourcehierarchy.Count, dependencyResourceID, TenantId);

                LogHandler.LogDebug("Fetching resource,action,actionParam details for platformID:{0} & tenantId:{1}", LogHandler.Layer.Business, PlatformInstanceId, TenantId);

                //getting details from resource observale action map 
                var response = (from rh in resourcehierarchy
                                join res in resDS.GetAny().ToArray() on rh.Resourceid equals res.ResourceId
                                join rdm in rdmDS.GetAny().ToArray() on res.ResourceId equals rdm.ResourceId
                                join orm in ormDS.GetAny().ToArray() on res.ResourceId equals orm.ResourceId
                                join roam in resObsActMapDS.GetAny().ToArray() on res.ResourceId equals roam.ResourceId
                                join obs in obsDS.GetAny().ToArray() on orm.ObservableId equals obs.ObservableId
                                join act in actDS.GetAny().ToArray() on roam.ActionId equals act.ActionId
                                join actp in actpDS.GetAny().ToArray() on act.ActionId equals actp.ActionId
                                join rt in typeDS.GetAny().ToArray() on res.ResourceTypeId equals rt.ResourceTypeId
                                where
                                res.IsActive == true
                                && roam.ResourceId == res.ResourceId
                                && roam.ObservableId == obs.ObservableId
                                && act.IsDeleted == false
                                && (res.ValidityStart <= DateTime.Now && res.ValidityEnd > DateTime.Now)
                                && (obs.ValidityStart <= DateTime.Now && obs.ValidityEnd > DateTime.Now)
                                && (roam.ValidityStart <= DateTime.Now && roam.ValidityEnd > DateTime.Now)
                                select new
                                {
                                    res,
                                    rdm,
                                    orm,
                                    roam = new DE.resourcetype_observable_action_map
                                    {
                                        ObservableId = roam.ObservableId,
                                        ActionId = roam.ActionId,
                                        Name = roam.Name,
                                        ExecutionMode = roam.ExecutionMode
                                    },
                                    obs,
                                    act,
                                    actp,
                                    rt
                                }).Distinct().ToList();

                //resource Ids that are not available in resource observable action map
                var listTemp = (from rh in resourcehierarchy
                                join roam in resObsActMapDS.GetAny().ToArray() on rh.Resourceid equals roam.ResourceId
                                select roam.ResourceId).ToList();

                // remaining resource Ids details from resourcetype observable action map
                response.AddRange((from rh in resourcehierarchy
                                   join res in resDS.GetAny().ToArray() on rh.Resourceid equals res.ResourceId
                                   join rdm in rdmDS.GetAny().ToArray() on res.ResourceId equals rdm.ResourceId
                                   join orm in ormDS.GetAny().ToArray() on res.ResourceId equals orm.ResourceId
                                   join roam in roamDS.GetAny().ToArray() on res.ResourceTypeId equals roam.ResourceTypeId
                                   join obs in obsDS.GetAny().ToArray() on orm.ObservableId equals obs.ObservableId
                                   join act in actDS.GetAny().ToArray() on roam.ActionId equals act.ActionId
                                   join actp in actpDS.GetAny().ToArray() on act.ActionId equals actp.ActionId
                                   join rt in typeDS.GetAny().ToArray() on res.ResourceTypeId equals rt.ResourceTypeId
                                   where res.IsActive == true
                                   && roam.ResourceTypeId == res.ResourceTypeId
                                   && roam.ObservableId == obs.ObservableId
                                   && act.IsDeleted == false
                                   && !listTemp.Contains(rh.Resourceid)
                                   && (res.ValidityStart <= DateTime.Now && res.ValidityEnd > DateTime.Now)
                                   && (obs.ValidityStart <= DateTime.Now && obs.ValidityEnd > DateTime.Now)
                                   && (roam.ValidityStart <= DateTime.Now && roam.ValidityEnd > DateTime.Now)
                                   select new { res, rdm, orm, roam, obs, act, actp, rt }).Distinct().ToList());

                LogHandler.LogDebug("{0} records fetched for platformID:{1} & tenantId:{2}", LogHandler.Layer.Business, response.Count, PlatformInstanceId, TenantId);

                var resultResponse = (from rh in resourcehierarchy
                                      from res in response
                                      where rh.Resourceid == res.res.ResourceId &&
                                      rh.Resourceid == res.rdm.ResourceId
                                      select new
                                      {
                                          ResourceId = rh.Resourceid,
                                          ResourceName = res.res.ResourceName,
                                          ResourceTypeId = res.res.ResourceTypeId,
                                          ResourceTypeName = res.rt.ResourceTypeName,
                                          DependencyResourceId = rh.DependencyResourceId,
                                          DependencyType = rh.DependencyType,
                                          ObservableId = res.orm.ObservableId,
                                          ObservableName = res.obs.ObservableName,
                                          ActionName = res.act.ActionName,
                                          ActionTypeId = res.act.ActionTypeId,
                                          AutomationEngineId = res.act.AutomationEngineId,
                                          CategoryId = res.act.CategoryId,
                                          ScriptId = res.act.ScriptId,
                                          ParamId = res.actp.ParamId,
                                          Name = res.actp.Name,
                                          DefaultValue = res.actp.DefaultValue,
                                          IsMandatory = res.actp.IsMandatory,
                                          IpAddress = res.res.Source,
                                          FieldToMap = res.actp.FieldToMap,
                                          ExecutionMode = res.roam.ExecutionMode
                                      }).ToList();

                var responseGroupby = (from response1 in resultResponse
                                       group response1 by new
                                       {
                                           response1.ResourceId,
                                           response1.ResourceName,
                                           response1.IpAddress,
                                           response1.ResourceTypeId,
                                           response1.ResourceTypeName,
                                           response1.DependencyResourceId,
                                           response1.DependencyType,
                                           response1.ObservableId,
                                           response1.ObservableName,
                                           response1.ActionName,
                                           response1.ActionTypeId,
                                           response1.AutomationEngineId,
                                           response1.CategoryId,
                                           response1.ScriptId,
                                           response1.ExecutionMode
                                       } into params1
                                       select new
                                       {
                                           ResourceId = params1.Key.ResourceId,
                                           ResourceName = params1.Key.ResourceName,
                                           ResourceTypeId = params1.Key.ResourceTypeId,
                                           ResourceTypeName = params1.Key.ResourceTypeName,
                                           DependencyResourceId = params1.Key.DependencyResourceId,
                                           DependencyType = params1.Key.DependencyType,
                                           ObservableId = params1.Key.ObservableId,
                                           ObservableName = params1.Key.ObservableName,
                                           ActionName = params1.Key.ActionName,
                                           ActionTypeId = params1.Key.ActionTypeId,
                                           AutomationEngineId = params1.Key.AutomationEngineId,
                                           CategoryId = params1.Key.CategoryId,
                                           ScriptId = params1.Key.ScriptId,
                                           IpAddress = params1.Key.IpAddress,
                                           ExecutionMode = params1.Key.ExecutionMode
                                       }).ToList();

                LogHandler.LogDebug("Populating platform details for platformId:{0}", LogHandler.Layer.Business, PlatformInstanceId);
                //Populate platform details
                int platId = Convert.ToInt32(PlatformInstanceId);
                PlatformInstanceDetails instanceDetails = new PlatformInstanceDetails();
                var platform = platformsDS.GetAny().Where(p => p.PlatformId == platId && p.TenantId == TenantId).FirstOrDefault();
                instanceDetails.VendorName = platform.PlatformName;
                instanceDetails.PlatformId = platform.PlatformId;
                instanceDetails.TenantId = platform.TenantId;
                // instanceDetails.UID = platform.Username;
                // instanceDetails.Pwd = platform.Password;
                instanceDetails.PlatformResourceModelVersion = 1;

                LogHandler.LogDebug("Populating platform observables details for platfromID:{0}", LogHandler.Layer.Business, PlatformInstanceId);
                //populate platform observables
                int platformCount = responseGroupby.Where(p => p.ResourceTypeName.ToLower() == "platform").Count();
                if (platformCount > 0)
                {
                    List<PlatformObservable> platformObservables = new List<PlatformObservable>();
                    platformObservables = (from platformRes in responseGroupby
                                           where platformRes.ResourceTypeName.ToLower() == "platform"
                                           select new PlatformObservable
                                           {
                                               Name = platformRes.ObservableName,
                                               ObservableId = platformRes.ObservableId.ToString(),
                                               Actions = new Actions
                                               {
                                                   ActionId = actDS.GetAny().Where(a => a.ActionName == platformRes.ActionName).FirstOrDefault().ActionId,
                                                   ScriptId = Convert.ToInt32(platformRes.ScriptId),
                                                   CategoryId = Convert.ToInt32(platformRes.CategoryId),
                                                   AutomationEngineId = Convert.ToInt32(platformRes.AutomationEngineId),
                                                   ExecutionMode = platformRes.ExecutionMode,
                                                   ParameterDetails = (from p in resultResponse
                                                                       where p.ResourceTypeName.ToLower() == "platform"
                                                                       && p.ResourceId == platformRes.ResourceId
                                                                       && p.ResourceName == platformRes.ResourceName
                                                                       && p.IpAddress == platformRes.IpAddress
                                                                       && p.ResourceTypeId == platformRes.ResourceTypeId
                                                                       && p.DependencyType == platformRes.DependencyType
                                                                       && p.ObservableId == platformRes.ObservableId
                                                                       && p.ObservableName == platformRes.ObservableName
                                                                       && p.ActionName == platformRes.ActionName
                                                                       && p.ActionTypeId == platformRes.ActionTypeId
                                                                       && p.AutomationEngineId == platformRes.AutomationEngineId
                                                                       && p.CategoryId == platformRes.CategoryId
                                                                       && p.ScriptId == platformRes.ScriptId
                                                                       select new ObservableParameters
                                                                       {
                                                                           ParamaterName = p.Name,
                                                                           ParameterValue = p.DefaultValue
                                                                       }).ToList()

                                               }

                                           }).ToList();

                }

                LogHandler.LogDebug("Populating server details(serverObservables,botObservables,serviceObsrvables) for platfromID:{0}", LogHandler.Layer.Business, PlatformInstanceId);

                //getting id and name for resource and type for given resources
                var resourceDetails = (from resTable in resDS.GetAny().ToArray()
                                       join rh in resourcehierarchy
                                       on resTable.ResourceId equals rh.Resourceid
                                       join resTypeTable in typeDS.GetAny().ToArray()
                                       on resTable.ResourceTypeId equals resTypeTable.ResourceTypeId
                                       select new
                                       {
                                           ResourceId = resTable.ResourceId,
                                           ResourceName = resTable.ResourceName,
                                           ResourceTypeId = resTypeTable.ResourceTypeId,
                                           ResourceTypeName = resTypeTable.ResourceTypeName
                                       }).ToList();

                //list of resource ids to process
                List<string> resIds = (from rh in resourcehierarchy
                                       select rh.Resourceid).ToList();

                bool isPlatformDependency = false;


                List<Server> servers = new List<Server>();

                servers = (from resServer in responseGroupby
                           where
                           resIds.Contains(resServer.DependencyResourceId)
                           group resServer by new
                           {
                               resServer.DependencyResourceId
                           }
                               into serRes
                           select new Server
                           {
                               Type = resourceDetails.Where(r => r.ResourceId == serRes.FirstOrDefault().DependencyResourceId).Select(s => s.ResourceTypeName).FirstOrDefault(),
                               Name = resourceDetails.Where(r => r.ResourceId == serRes.FirstOrDefault().DependencyResourceId).Select(s => s.ResourceName).FirstOrDefault(),
                               IPAddress = serRes.FirstOrDefault().IpAddress,
                               ServerObservables = (from serverRes in responseGroupby
                                                    where (serverRes.ResourceId == serRes.FirstOrDefault().DependencyResourceId 
                                                    || serverRes.DependencyType == ConfigurationManager.AppSettings["DependencyType"]
                                                    )
                                                    && !(botsTemp.Contains(serverRes.ResourceTypeName.ToLower()) || SevicesTemp.Contains(serverRes.ResourceTypeName.ToLower()))
                                                    select new ServerObservable
                                                    {
                                                        IPAddress = serverRes.IpAddress,
                                                        ResourceId = serverRes.ResourceId,
                                                        Name = serverRes.ObservableName,
                                                        ObservableId = Convert.ToString(serverRes.ObservableId),
                                                        Actions = new Actions
                                                        {
                                                            ActionId = actDS.GetAny().Where(a => a.ActionName == serverRes.ActionName).FirstOrDefault().ActionId,
                                                            ScriptId = Convert.ToInt32(serverRes.ScriptId),
                                                            CategoryId = Convert.ToInt32(serverRes.CategoryId),
                                                            ActionTypeId = serverRes.ActionTypeId,
                                                            ActionName = actTypeDS.GetAny().Where(a => a.ActionTypeId == serverRes.ActionTypeId).FirstOrDefault().ActionType1,
                                                            AutomationEngineId = Convert.ToInt32(serverRes.AutomationEngineId),
                                                            ExecutionMode = serverRes.ExecutionMode,
                                                            ParameterDetails = (from p in resultResponse
                                                                                where p.ResourceTypeName == serverRes.ResourceTypeName
                                                                                && p.ResourceId == serverRes.ResourceId
                                                                                && p.ResourceName == serverRes.ResourceName
                                                                                && p.IpAddress == serverRes.IpAddress
                                                                                && p.ResourceTypeId == serverRes.ResourceTypeId
                                                                                && p.ResourceTypeName == serverRes.ResourceTypeName
                                                                                && p.DependencyType == serverRes.DependencyType
                                                                                && p.ObservableId == serverRes.ObservableId
                                                                                && p.ObservableName == serverRes.ObservableName
                                                                                && p.ActionName == serverRes.ActionName
                                                                                && p.ActionTypeId == serverRes.ActionTypeId
                                                                                && p.AutomationEngineId == serverRes.AutomationEngineId
                                                                                && p.CategoryId == serverRes.CategoryId
                                                                                && p.ScriptId == serverRes.ScriptId
                                                                                select new ObservableParameters
                                                                                {
                                                                                    ParamaterName = p.Name,
                                                                                    ParameterValue = p.FieldToMap == "resource_attribute.AttributeValue" ? (from param in resourceAttribute
                                                                                                                                                            where (param.ResourceId == p.ResourceId
                                                                                                                                                            || param.ResourceId == p.DependencyResourceId)
                                                                                                                                                            && param.AttributeName == p.Name
                                                                                                                                                            select param.AttributeValue).FirstOrDefault() : p.FieldToMap,
                                                                                    IsSecret = p.FieldToMap == "resource_attribute.AttributeValue" ? (from param in resourceAttribute
                                                                                                                                                            where (param.ResourceId == p.ResourceId
                                                                                                                                                            || param.ResourceId == p.DependencyResourceId)
                                                                                                                                                            && param.AttributeName == p.Name
                                                                                                                                                            select param.IsSecret).FirstOrDefault() : false
                                                                                }).ToList()
                                                        }
                                                    }).ToList(),
                               bots = (from botRes in serRes
                                       where botsTemp.Contains(botRes.ResourceTypeName.ToLower())
                                       group botRes by new
                                       {
                                           botRes.IpAddress,
                                           botRes.ResourceName,
                                           botRes.ResourceId
                                       } into botso
                                       select new Bot
                                       {
                                           IPAddress = botso.Key.IpAddress,
                                           BotInstanceId = botso.Key.ResourceId,
                                           BotName = botso.Key.ResourceName,
                                           botObservables = (from serverRes in responseGroupby
                                                             from bot in botso
                                                             where serverRes == bot
                                                             select new BotObservable
                                                             {
                                                                 Name = serverRes.ObservableName,
                                                                 ObservableId = Convert.ToString(serverRes.ObservableId),
                                                                 Actions = new Actions
                                                                 {
                                                                     ActionId = actDS.GetAny().Where(a => a.ActionName == serverRes.ActionName).FirstOrDefault().ActionId,
                                                                     ScriptId = Convert.ToInt32(serverRes.ScriptId),
                                                                     CategoryId = Convert.ToInt32(serverRes.CategoryId),
                                                                     ActionTypeId = serverRes.ActionTypeId,
                                                                     ActionName = actTypeDS.GetAny().Where(a => a.ActionTypeId == serverRes.ActionTypeId).FirstOrDefault().ActionType1,
                                                                     AutomationEngineId = Convert.ToInt32(serverRes.AutomationEngineId),
                                                                     ExecutionMode = serverRes.ExecutionMode,
                                                                     ParameterDetails = (from p in resultResponse
                                                                                         where botsTemp.Contains(p.ResourceTypeName.ToLower())
                                                                                         //p.ResourceTypeId == Convert.ToInt32(ResourceType.BOT)
                                                                                         && p.ResourceId == serverRes.ResourceId
                                                                                         && p.ResourceName == serverRes.ResourceName
                                                                                         && p.IpAddress == serverRes.IpAddress
                                                                                         && p.ResourceTypeId == serverRes.ResourceTypeId
                                                                                         && p.DependencyType == serverRes.DependencyType
                                                                                         && p.ObservableId == serverRes.ObservableId
                                                                                         && p.ObservableName == serverRes.ObservableName
                                                                                         && p.ActionName == serverRes.ActionName
                                                                                         && p.ActionTypeId == serverRes.ActionTypeId
                                                                                         && p.AutomationEngineId == serverRes.AutomationEngineId
                                                                                         && p.CategoryId == serverRes.CategoryId
                                                                                         && p.ScriptId == serverRes.ScriptId
                                                                                         select new ObservableParameters
                                                                                         {
                                                                                             ParamaterName = p.Name,
                                                                                             //ParameterValue = p.DefaultValue
                                                                                             ParameterValue = p.FieldToMap == "resource_attribute.AttributeValue" ? (from param in resourceAttribute
                                                                                                                                                                     join rdm in rdmDSExtn.DependencyResources(p.ResourceId, Convert.ToString(TenantId))
                                                                                                                                                                     on param.ResourceId equals rdm.Resourceid
                                                                                                                                                                     where param.ResourceId == rdm.Resourceid
                                                                                                                                                                     /* (param.ResourceId == p.ResourceId
                                                                                                                                                                      || param.ResourceId == p.DependencyResourceId
                                                                                                                                                                      || param.ResourceId== rdm.DependencyResourceId)*/
                                                                                                                                                                     && param.AttributeName == p.Name
                                                                                                                                                                     select param.AttributeValue).FirstOrDefault() : p.FieldToMap,
                                                                                             IsSecret = p.FieldToMap == "resource_attribute.AttributeValue" ? (from param in resourceAttribute
                                                                                                                                                               join rdm in rdmDSExtn.DependencyResources(p.ResourceId, Convert.ToString(TenantId))
                                                                                                                                                               on param.ResourceId equals rdm.Resourceid
                                                                                                                                                               where param.ResourceId == rdm.Resourceid
                                                                                                                                                               /* (param.ResourceId == p.ResourceId
                                                                                                                                                                || param.ResourceId == p.DependencyResourceId
                                                                                                                                                                || param.ResourceId== rdm.DependencyResourceId)*/
                                                                                                                                                               && param.AttributeName == p.Name
                                                                                                                                                               select param.IsSecret).FirstOrDefault() : false
                                                                                         }).ToList()
                                                                 }

                                                             }).ToList()
                                       }).ToList(),
                               services = (from botRes in serRes
                                           where SevicesTemp.Contains(botRes.ResourceTypeName.ToLower())
                                           group botRes by new
                                           {
                                               botRes.IpAddress,
                                               botRes.ResourceName,
                                               botRes.ResourceId
                                           } into serviceobj
                                           select new Service
                                           {
                                               IPAddress = serviceobj.Key.IpAddress,
                                               ServiceId = serviceobj.Key.ResourceId,
                                               ServiceName = serviceobj.Key.ResourceName,
                                               serviceObservables = (from serverRes in responseGroupby
                                                                     from service in serviceobj
                                                                     where serverRes == service
                                                                     select new ServiceObservable
                                                                     {
                                                                         Name = serverRes.ObservableName,
                                                                         ObservableId = Convert.ToString(serverRes.ObservableId),
                                                                         Actions = new Actions
                                                                         {
                                                                             ActionId = actDS.GetAny().Where(a => a.ActionName == serverRes.ActionName).FirstOrDefault().ActionId,
                                                                             ScriptId = Convert.ToInt32(serverRes.ScriptId),
                                                                             CategoryId = Convert.ToInt32(serverRes.CategoryId),
                                                                             ActionTypeId = serverRes.ActionTypeId,
                                                                             ActionName = actTypeDS.GetAny().Where(a => a.ActionTypeId == serverRes.ActionTypeId).FirstOrDefault().ActionType1,
                                                                             AutomationEngineId = Convert.ToInt32(serverRes.AutomationEngineId),
                                                                             ExecutionMode = serverRes.ExecutionMode,
                                                                             ParameterDetails = (from p in resultResponse
                                                                                                 where SevicesTemp.Contains(p.ResourceTypeName.ToLower())
                                                                                                 //(p.ResourceTypeId == Convert.ToInt32(ResourceType.Services) || p.ResourceTypeId == Convert.ToInt32(ResourceType.DBService))
                                                                                                 && p.ResourceId == serverRes.ResourceId
                                                                                                 && p.ResourceName == serverRes.ResourceName
                                                                                                 && p.IpAddress == serverRes.IpAddress
                                                                                                 && p.ResourceTypeId == serverRes.ResourceTypeId
                                                                                                 && p.DependencyType == serverRes.DependencyType
                                                                                                 && p.ObservableId == serverRes.ObservableId
                                                                                                 && p.ObservableName == serverRes.ObservableName
                                                                                                 && p.ActionName == serverRes.ActionName
                                                                                                 && p.ActionTypeId == serverRes.ActionTypeId
                                                                                                 && p.AutomationEngineId == serverRes.AutomationEngineId
                                                                                                 && p.CategoryId == serverRes.CategoryId
                                                                                                 && p.ScriptId == serverRes.ScriptId
                                                                                                 select new ObservableParameters
                                                                                                 {
                                                                                                     ParamaterName = p.Name,
                                                                                                     //ParameterValue = p.DefaultValue
                                                                                                     ParameterValue = p.FieldToMap == "resource_attribute.AttributeValue" ? (from param in resourceAttribute
                                                                                                                                                                             where (param.ResourceId == p.ResourceId
                                                                                                                                                                             || param.ResourceId == p.DependencyResourceId)
                                                                                                                                                                             && param.AttributeName == p.Name
                                                                                                                                                                             select param.AttributeValue).FirstOrDefault() : p.FieldToMap,
                                                                                                     IsSecret = p.FieldToMap == "resource_attribute.AttributeValue" ? (from param in resourceAttribute
                                                                                                                                                                             where (param.ResourceId == p.ResourceId
                                                                                                                                                                             || param.ResourceId == p.DependencyResourceId)
                                                                                                                                                                             && param.AttributeName == p.Name
                                                                                                                                                                             select param.IsSecret).FirstOrDefault() : false
                                                                                                 }).ToList()
                                                                         }

                                                                     }).ToList()
                                           }).ToList()
                           }).ToList();                

                instanceDetails.servers = servers;
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAllBotDependencies", "Monitor"), LogHandler.Layer.Business, null);
                return instanceDetails;


            }
            catch (Exception superBotException)
            {
                Console.WriteLine("Error occured while retrieving platform details. Error Message = " + superBotException.Message);
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(superBotException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAllBotDependencies", "Monitor"), LogHandler.Layer.Business, null);
                if (rethrow)
                {
                    throw superBotException;
                }
                return null;
            }
        }

        private object GetGroupedResource(object fullObject, object groupedObject, List<string> resourceIds)
        {
            if (resourceIds != null && resourceIds.Count > 0)
            {
                var retObj = (from serverRes in (IEnumerable<dynamic>)fullObject
                              where serverRes.ResourceId == ((IEnumerable<dynamic>)groupedObject).FirstOrDefault().ResourceId
                              select serverRes).ToList();
                return retObj;
            }
            else
            {
                var retObj = (from serverRes in (IEnumerable<dynamic>)fullObject
                              where serverRes.ResourceId == ((IEnumerable<dynamic>)groupedObject).FirstOrDefault().DependencyResourceId
                              select serverRes).ToList();
                return retObj;
            }
        }

        private List<string> CheckResourceValidity(string resourceIds)
        {
            ResourceDS resDS = new ResourceDS();

            var result = (from res in resDS.GetAny().ToArray()
                          where resourceIds.Split(',').ToList().Contains(res.ResourceId)
                          && res.IsActive == true
                          && res.ValidityStart <= DateTime.Now
                          && res.ValidityEnd > DateTime.Now
                          select res.ResourceId).ToList();

            return result;
        }
        private BE.Metric PopulateMetricData(string ApplicationName, string eventType, string message, string resourceId, string observableName, string errorMessage, string healthCheckType, string serverIp)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "PopulateMetricData", "Monitor"), LogHandler.Layer.Business, null);
            try
            {
                string[] stringSeparators = new string[] { "\r\n", "\n" };
                string[] lines = message.Split(stringSeparators, StringSplitOptions.None);
                LogHandler.LogDebug(string.Format("Populating metric properties from input arguments Applicationname:{0},eventType:{1},resourceId:{2},observableName:{3}" +
                    "healthCheckType:{4} & serverIp:{5}", ApplicationName, eventType, resourceId, observableName, healthCheckType, serverIp), LogHandler.Layer.Business, null);
                BE.Metric metric = new BE.Metric();
                string obsName = observableName.ToLower().Replace(" ", "");
                foreach (string s in lines.Where(x => !string.IsNullOrEmpty(x)).ToArray())
                {

                    if (s.ToLower().Contains("ishealthy=") || s.ToLower().Contains(observableName.ToLower() + "=") || s.ToLower().Contains("output=") || s.ToLower().Contains(obsName))
                    {
                        string[] val = s.Split('=');
                        metric.MetricValue = val.Length > 1 ? val[1] : null;
                    }

                }

                metric.ServerIp = serverIp;
                metric.Application = ApplicationName;
                metric.EventType = eventType;
                metric.MetricTime = DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt");
                metric.ResourceId = resourceId;
                metric.MetricName = observableName;
                // metric.MetricValue = "No";
                //metric.ServerIp = Array.Exists(lines, x => "ip".Contains(x)) ? Array.IndexOf(lines, "ip") : "0";
                if (string.IsNullOrEmpty(metric.Description))
                { metric.Description = "NA"; }
                else
                { metric.Description = errorMessage; }

                metric.Source = healthCheckType;
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "PopulateMetricData", "Monitor"), LogHandler.Layer.Business, null);
                return metric;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private BE.Metric PopulateMetricDataHealth(BusinessEntity.DeviceList deviceList, string serverName, string serverIP, string ApplicationName, string eventType, string resourceId, string observableName, string healthCheckType, string serverType)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "PopulateMetricDataHealth", "Monitor"), LogHandler.Layer.Business, null);
            try
            {
                BE.Metric metric = null;

                if (!string.IsNullOrEmpty(serverType) && !string.IsNullOrEmpty(serverName))
                {
                    serverType = serverType.Replace(" ", string.Empty).Replace("_", string.Empty);
                    var result = (from d in deviceList.list where d.type.Replace(" ", string.Empty).Replace("_", string.Empty).ToUpper() == serverType.ToUpper() && d.fullyQualifiedHostName.ToUpper() == serverName.ToUpper() select d).FirstOrDefault();
                    if (result != null)
                    {
                        metric = new BE.Metric();
                        metric.MetricValue = result.status;
                        metric.ServerIp = serverIP;
                        metric.Description = "NA";
                        metric.Application = ApplicationName;
                        metric.EventType = eventType;
                        metric.MetricTime = DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt");
                        metric.ResourceId = resourceId;
                        metric.MetricName = observableName;
                        metric.Source = healthCheckType;
                    }
                }
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "PopulateMetricDataHealth", "Monitor"), LogHandler.Layer.Business, null);
                return metric;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public DataTable ExecuteDBView(string dbserver, string dbname, string viewname, List<IE.ObservableParameters> inputParams)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "ExecuteDBView", "Monitor"), LogHandler.Layer.Business, null);
            // Table to store the query results
            DataTable table = new DataTable();
            try
            {
                LogHandler.LogDebug(String.Format("Executing ExecuteDBView with input arguments DBServer:{0},DBName:{1},ViewName:{2}", dbserver, dbname, viewname), LogHandler.Layer.Business, null);
                using (LogHandler.TraceOperations("Monitor:ExecuteDBView", LogHandler.Layer.Business, Guid.NewGuid(), null))
                {
                    string dbUserid = Convert.ToString(ConfigurationManager.AppSettings["DBUserID"]);
                    string dbPassword = Convert.ToString(ConfigurationManager.AppSettings["DBPassword"]);
                    if (inputParams != null)
                    {
                        foreach (IE.ObservableParameters param in inputParams)
                        {
                            if (string.Equals(param.ParamaterName, "dbusername", StringComparison.InvariantCultureIgnoreCase))
                                dbUserid = param.ParameterValue;
                            if (string.Equals(param.ParamaterName, "dbpassword", StringComparison.InvariantCultureIgnoreCase))
                            {
                                dbPassword = param.IsSecret != null && Convert.ToBoolean(param.IsSecret)? SecureData.UnSecure(param.ParameterValue, ApplicationConstants.SecureKeys.IAP2): param.ParameterValue;
                            }
                                
                        }

                    }
                   
                    string conn = "Data Source=" + dbserver + ";Initial Catalog=" + dbname + ";User Id=" + dbUserid + ";Password=" + dbPassword;
                    string cmd = "select * from " + viewname;

                    LogHandler.LogDebug(String.Format("Creation of DB connection for connection:{0}", conn), LogHandler.Layer.Business, null);
                    // Creates a SQL connection
                    using (var connection = new SqlConnection(conn))
                    {
                        connection.Open();
                        LogHandler.LogDebug(String.Format("DB connection opened for connection:{0}", conn), LogHandler.Layer.Business, null);

                        // Creates a SQL command
                        using (var command = new SqlCommand(cmd, connection))
                        {
                            // Loads the query results into the table
                            table.Load(command.ExecuteReader());
                            LogHandler.LogDebug(String.Format("Executed query:{0} and no.of records fetched:{1}", cmd, table.Rows.Count), LogHandler.Layer.Business, null);
                        }

                        connection.Close();
                        LogHandler.LogDebug(String.Format("DB connection closed for connection:{0}", conn), LogHandler.Layer.Business, null);
                    }
                }
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "ExecuteDBView", "Monitor"), LogHandler.Layer.Business, null);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(string.Format(ErrorMessages.ViewExecutionUnsuccessful, viewname, ex.Message), LogHandler.Layer.Business, null);
                Console.WriteLine("Error occured while executing view : " + viewname + " Error :" + ex.Message);
                throw ex;
            }
            return table;
        }

        public void GenerateEnvironmentMetricDetails(int platformId, int tenantId)
        {
            try
            {
                ResourcetypeObservableActionMapDS roamDS = new ResourcetypeObservableActionMapDS();
                ResourceDS resDS = new ResourceDS();
                ActionDS aDS = new ActionDS();
                ObservableDS obsDS = new ObservableDS();
                ResourceTypeDS rtDS = new ResourceTypeDS();
                EnvironmentScanMetricDS esmDS = new EnvironmentScanMetricDS();
                EnvironmentScanMetricDetailsDS esmdDS = new EnvironmentScanMetricDetailsDS();
                ResourceAttributesDS resAttrDS = new ResourceAttributesDS();
                ActionParamsDS actParamDS = new ActionParamsDS();
                ObservationsDS observationsDS = new ObservationsDS();
                PlatformsDS platDS = new PlatformsDS();
                List<int> observationIDs = new List<int>();


                var objs = (from roam in roamDS.GetAny().ToArray()
                            join res in resDS.GetAny().ToArray() on roam.ResourceTypeId equals res.ResourceTypeId
                            join rt in rtDS.GetAny().ToArray() on res.ResourceTypeId equals rt.ResourceTypeId
                            join obs in obsDS.GetAny().ToArray() on roam.ObservableId equals obs.ObservableId
                            join act in aDS.GetAny().ToArray() on roam.ActionId equals act.ActionId
                            where res.PlatformId == platformId && res.TenantId == tenantId
                            && roam.TenantId == tenantId
                            && res.IsActive == true
                            && act.ActionTypeId == Convert.ToInt32(ActionTypes.BaselineMetricScript)
                            group new { res, roam, rt } by new { rt.ResourceTypeId, rt.ResourceTypeName, res.ResourceId, res.ResourceName } into grpList
                            select new
                            {
                                grpList.Key.ResourceId,
                                grpList.Key.ResourceName,
                                grpList.Key.ResourceTypeId,
                                grpList.Key.ResourceTypeName,
                                observales = (from grp in grpList
                                              join ob in obsDS.GetAny().ToArray() on grp.roam.ObservableId equals ob.ObservableId
                                              join act in aDS.GetAny().ToArray() on grp.roam.ActionId equals act.ActionId
                                              join actP in actParamDS.GetAny().ToArray() on act.ActionId equals actP.ActionId
                                              where grp.roam.ResourceTypeId == grpList.Key.ResourceTypeId
                                              && act.ActionTypeId == Convert.ToInt32(ActionTypes.BaselineMetricScript)
                                              group new { ob, act, actP } by new { ob.ObservableId, ob.ObservableName } into grpObservables
                                              select new
                                              {
                                                  observableID = grpObservables.Key.ObservableId,
                                                  ObservableName = grpObservables.Key.ObservableName,
                                                  Actions = (from grpObs in grpObservables
                                                             where grpObs.ob.ObservableId == grpObservables.Key.ObservableId
                                                             group grpObs by new { grpObs.act.ActionId, grpObs.act.ActionTypeId, grpObs.act.ScriptId, grpObs.act.CategoryId } into grpActions
                                                             select new
                                                             {
                                                                 ActionId = grpActions.Key.ActionId,
                                                                 ActionTypeId = grpActions.Key.ActionTypeId,
                                                                 ScriptID = grpActions.Key.ScriptId,
                                                                 CategoryID = grpActions.Key.CategoryId,
                                                                 InputParams = (from grpAct in grpActions
                                                                                where grpAct.act.ActionId == grpActions.Key.ActionId
                                                                                group grpAct by grpAct.actP.ActionId into grpParams
                                                                                from p in grpParams
                                                                                select new
                                                                                {
                                                                                    ParamID = p.actP.ParamId,
                                                                                    Name = p.actP.Name,
                                                                                    FieldtoMap = p.actP.FieldToMap == "resource_attribute.AttributeValue" ? (from resAttr in resAttrDS.GetAny()
                                                                                                                                                             where resAttr.AttributeName == p.actP.Name &&
                                                                                                                                                             resAttr.ResourceId == grpList.Key.ResourceId
                                                                                                                                                             select resAttr.AttributeValue).FirstOrDefault() : p.actP.FieldToMap
                                                                                }).ToList(),
                                                             }).ToList()
                                              }).ToList()
                            }).ToList();

                string msg = "test";
                foreach (var res in objs)
                {
                    ResourceAttributesDS attributesDS = new ResourceAttributesDS();
                    var attributes = attributesDS.GetAny().Where(r => r.ResourceId == res.ResourceId).ToList();

                    esmDS = new EnvironmentScanMetricDS();
                    var environmentScanDetails = esmDS.GetAny().Where(e => e.ResourceID == res.ResourceId).OrderByDescending(x => x.Version).FirstOrDefault();
                    int newVersion = 1;
                    newVersion = environmentScanDetails != null ? Convert.ToInt32(environmentScanDetails.Version) + 1 : newVersion;

                    foreach (var observable in res.observales)
                    {
                        foreach (var action in observable.Actions)
                        {
                            List<string> auditParams = new List<string>();
                            InitiateExecutionReqMsg executionReqMsg = new InitiateExecutionReqMsg();
                            IE.ScriptIdentifier script = new IE.ScriptIdentifier();
                            script.ScriptId = Convert.ToInt32(action.ScriptID);
                            script.CategoryId = Convert.ToInt32(action.CategoryID);
                            script.CompanyId = tenantId;
                            if (action.InputParams != null)
                            {
                                List<IE.Parameter> parametersSE = new List<IE.Parameter>();
                                foreach (var parameters in action.InputParams)
                                {
                                    auditParams.Add(parameters.Name + "=" + parameters.FieldtoMap);
                                    //RemoteServerNames
                                    //RemoteUserName
                                    //RemotePassword
                                    if (string.Equals(parameters.Name, "RemoteServerNames", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        script.RemoteServerNames = parameters.FieldtoMap;
                                    }
                                    else if (string.Equals(parameters.Name, "RemoteUserName", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        script.UserName = parameters.FieldtoMap;
                                    }
                                    else if (string.Equals(parameters.Name, "RemotePassword", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        script.Password = parameters.FieldtoMap;
                                    }
                                    else if (string.Equals(parameters.Name, "RemoteExecutionMode", StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        script.RemoteExecutionMode = Convert.ToInt32(parameters.FieldtoMap);
                                    }
                                    else
                                    {
                                        var attr = attributes.Where(a => a.AttributeName == parameters.Name).FirstOrDefault();
                                        IE.Parameter param = new IE.Parameter();
                                        param.ParameterName = parameters.Name;
                                        param.ParameterValue = attr != null && Convert.ToBoolean(attr.IsSecret) ? SecureData.UnSecure(parameters.FieldtoMap, ApplicationConstants.SecureKeys.IAP2) : parameters.FieldtoMap;
                                        //param.ParameterValue = parameters.FieldtoMap;
                                        parametersSE.Add(param);
                                    }
                                }
                                script.Parameters = parametersSE;
                            }
                            executionReqMsg.ScriptIdentifier = script;

                            LogHandler.LogDebug(string.Format("Executing SEE with values scriptId:{0} & categoryId:{1}", action.ScriptID, action.CategoryID), LogHandler.Layer.Business, null);

                            Console.WriteLine("Calling SEE for resource ID:" + res.ResourceId);

                            ScriptExecute scriptExecute = new ScriptExecute();
                            var channel1 = scriptExecute.ServiceChannel;
                            InitiateExecutionResMsg response = channel1.AsyncInitiateExecution(executionReqMsg).Result;
                            #region Log Audit Data
                            BE.AuditLog auditLogObj = new AuditLog();
                            auditLogObj.ResourceID = res.ResourceId;
                            auditLogObj.ObservableID = observable.observableID;
                            auditLogObj.ActionID = action.ActionId;
                            auditLogObj.ActionParams = string.Join(",", auditParams);
                            auditLogObj.PlatformID = platformId;
                            auditLogObj.TenantID = tenantId;
                            #endregion


                            if (response != null && response.ScriptResponse != null && response.ScriptResponse.FirstOrDefault().SuccessMessage != null)
                            {
                                auditLogObj.Status = "SUCCESS";
                                auditLogObj.Output = response.ScriptResponse.FirstOrDefault().SuccessMessage;
                                Console.WriteLine("Script execution SUCCESS for resource ID:" + res.ResourceId);
                                string resMsg = response.ScriptResponse.FirstOrDefault().SuccessMessage.
                                    Replace("Info=[INFO]: Error occured while invocking the command invocation ", "");
                                List<BE.EnvScanMetric> metrics = JsonConvert.DeserializeObject<List<BE.EnvScanMetric>>(resMsg);
                                if (metrics != null)
                                {
                                    var isAdded = esmDS.Insert(new DE.Environment_Scan_Metric()
                                    {
                                        Version = newVersion,
                                        ResourceID = res.ResourceId,
                                        ObservableID = observable.observableID,
                                        TenantID = tenantId,
                                        PlatformID = platformId,
                                        GeneratedDate = DateTime.UtcNow
                                    });
                                    if (isAdded != null && isAdded.EnvironmentScanMetricID > 0)
                                    {
                                        Console.WriteLine("Data inserted into Environment_Scan_Metric table for resourceid:" + res.ResourceId + "& version:" + newVersion);
                                        Console.WriteLine(metrics.Count() + " metrics found for ResourceID:" + res.ResourceId);
                                        var distinctList = metrics.GroupBy(m => m.metrickey).Select(x => x.FirstOrDefault()).Where(m => m.metrickey != null).Distinct();
                                        //var jhfjdhgd = metrics.Where(m => m.metrickey != null).Count();
                                        foreach (BE.EnvScanMetric metric in distinctList)
                                        {
                                            esmdDS = new EnvironmentScanMetricDetailsDS();
                                            List<DE.Environment_Scan_Metric_Details> environment_Scan_Metric_Details = new List<DE.Environment_Scan_Metric_Details>();
                                            foreach (BE.EnvScanMetricAttributes attribute in metric.metricvalue)
                                            {
                                                DE.Environment_Scan_Metric_Details metric_Details = new DE.Environment_Scan_Metric_Details();
                                                metric_Details.EnvironmentScanMetricID = isAdded.EnvironmentScanMetricID;
                                                metric_Details.MetricName = metric.metricname;
                                                metric_Details.MetricID = Convert.ToInt32(metric.metricid);
                                                metric_Details.MetricKey = metric.metrickey;
                                                metric_Details.AttributeName = attribute.attributename;
                                                metric_Details.AttributeValue = attribute.attributevalue;
                                                metric_Details.DisplayName = attribute.displayname;
                                                metric_Details.isActive = true;
                                                metric_Details.TenantID = tenantId;
                                                environment_Scan_Metric_Details.Add(metric_Details);
                                            }

                                            if (environment_Scan_Metric_Details.Count > 0)
                                            {
                                                var isDetailsAdded = esmdDS.InsertBatch(environment_Scan_Metric_Details);
                                            }
                                        }
                                        Console.WriteLine("Data inserted into Environment_Scan_Metric_details table for EnvironmentScanID:" + isAdded.EnvironmentScanMetricID);

                                        // Insert details into Observation
                                        observationsDS = new ObservationsDS();
                                        DE.observations observations = new DE.observations()
                                        {
                                            PlatformId = platformId,
                                            TenantId = tenantId,
                                            ResourceId = res.ResourceId,
                                            ResourceTypeId = res.ResourceTypeId,
                                            ObservableId = observable.observableID,
                                            ObservableName = observable.ObservableName,
                                            ObservationStatus = "In Progress",
                                            Value = "In Progress",
                                            SourceIp = resDS.GetAny().Where(r => r.ResourceId == res.ResourceId).FirstOrDefault().Source
                                        };
                                        var obsRes = observationsDS.Insert(observations);
                                        Console.WriteLine("Data inserted into Observations table for resourceID:" + res.ResourceId);

                                        observationIDs.Add(obsRes.ObservationId);

                                        Console.WriteLine("Sending data to environmentscanmetrics Queue ");
                                        //send Data to environmentscanmetrics Queue      
                                        int seqno = 0;
                                        var envScanMetricList = metrics.GroupBy(m => m.metrickey).Select(x => x.FirstOrDefault()).Where(m => m.metrickey != null).Distinct();
                                        //metrics.Where(m => m.metrickey != null).Distinct();
                                        foreach (BE.EnvScanMetric metric in envScanMetricList)
                                        {
                                            seqno++;
                                            BE.Metric metricBE = new BE.Metric();
                                            metricBE.ServerIp = resDS.GetAny().Where(r => r.ResourceId == res.ResourceId).FirstOrDefault().Source;
                                            metricBE.Application = platDS.GetAny().Where(p => p.PlatformId == platformId).FirstOrDefault().PlatformName;
                                            metricBE.EventType = "Server";
                                            metricBE.MetricTime = DateTime.UtcNow.ToString("MM/dd/yyyy hh:mm tt");
                                            metricBE.ResourceId = res.ResourceId;
                                            metricBE.MetricName = observable.ObservableName;
                                            metricBE.Description = "NA";
                                            metricBE.Source = observable.ObservableName;
                                            metricBE.MetricValue = Utility.SerialiseToJSON(new BE.EnvScanMetricQueue()
                                            {
                                                observationid = obsRes.ObservationId,
                                                version = newVersion,
                                                metricid = metric.metricid,
                                                metricname = metric.metricname,
                                                metrickey = metric.metrickey,
                                                metricvalue = metric.metricvalue
                                            });
                                            metricBE.SequenceNumber = seqno + "/" + envScanMetricList.Count();

                                            EntityTranslator translator = new EntityTranslator();
                                            DE.Queue.Metric metricDE = translator.MetricBEToDE(metricBE);

                                            //bool isValid = checkNotificationTimeThreshold(PlatformId, metric.ResourceId, Convert.ToInt32(observable.ObservableId), "Platform", metric.ServerIp, metric.MetricValue);
                                            //if (isValid)
                                            //{

                                            LogHandler.LogDebug(string.Format("Sending metric data to queue and details are EventType:{0}, Application:{1}, MetricTime:{2}, ResourceId:{3}, MetricName:{4}, MetricValue:{5} ,ServerIp:{6}, Description:{7}, Source:{8}",
                                                metricDE.EventType, metricDE.Application, metricDE.MetricTime, metricDE.ResourceId, metricDE.MetricName, metricDE.MetricValue, metricDE.ServerIp, metricDE.Description, metricDE.Source),
                                                LogHandler.Layer.Business, null);

                                            MetricProcessorDS processorDS = new MetricProcessorDS();
                                            string msgResponse = processorDS.Send(metricDE, null);
                                            Console.WriteLine("Sending message to queue for metric Id " + metric.metricid + " Response Message : " + msgResponse);
                                            LogHandler.LogDebug(string.Format("Metric data sent to queue for metric Id  {0} and response message is {1}", metric.metricid, msgResponse), LogHandler.Layer.Business, null);
                                            // }
                                        }
                                    }
                                    else
                                    {
                                        Console.WriteLine("Data not inserted into Environment_Scan_Metric table for resourceid:" + res.ResourceId + "& version:" + newVersion);
                                    }
                                }
                            }
                            else
                            {
                                auditLogObj.Status = "FAILED";
                                auditLogObj.Output = response.ScriptResponse.FirstOrDefault().ErrorMessage != null ? response.ScriptResponse.FirstOrDefault().ErrorMessage : response.ScriptResponse.FirstOrDefault().SuccessMessage;
                                Console.WriteLine("Script execution failed for resource ID:" + res.ResourceId + "Message " + response.ScriptResponse.FirstOrDefault().SuccessMessage);
                                LogHandler.LogDebug("Script execution failed for resource ID:" + res.ResourceId, LogHandler.Layer.Business, null);
                            }
                            Audit_Log audit_Log_obj = new Audit_Log();
                            var isSuccess = audit_Log_obj.LogAuditData(auditLogObj);


                        }
                    }
                }
                EnvironmentScanConsolidatedReport(observationIDs, platformId, tenantId);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(string.Format("Error occured while generating EnvironmentMetricDetails. Error Message:{0}", ex.Message), LogHandler.Layer.Business, null);
                throw ex;
            }

        }

        private static List<string> GetConfiguredResourceTypes(string referenceKey)
        {
            List<string> resourceTypes = new List<string>();
            XElement root = XElement.Load(AppDomain.CurrentDomain.BaseDirectory + @"XML\AutomationType.xml");
            //var objType = from scripttype in root.Elements("Type")
            //              where scripttype.Attribute("key").Value.ToLower().Equals(referenceKey.ToLower())
            //              select scripttype.Value;

            //if (objType.FirstOrDefault() != null)
            //executionMode = objType.FirstOrDefault().ToString();
            var listgngjf = root.Elements(referenceKey).Select(x => x.Element("Type").Value.ToList()).ToList();

            var element = root.Elements("Type").Where(x => x.Attribute("key").Value.ToLower().Equals(referenceKey.ToLower(), StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (element != null)
            {
                foreach (var ele in element)
                {
                    resourceTypes.Add(ele.Value);
                }
                // executionMode = element.Attribute("executionMode").Value.ToLower();
            }

            return resourceTypes;
        }

        private async Task<bool> HealthCheckSystemAPI(IE.SystemHealthCheckObservable observable, int tenantID, int platformID, string resourceID,
            string platformName, int HealtcheckTrackingId, string healthcheckType, string ipAddress, string eventType)
        {
            LogHandler.LogDebug(string.Format("Executing SEE for resourceID:{0} & ActionTypeId:{1}", resourceID, observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);

            ResourceAttributesDS attributesDS = new ResourceAttributesDS();
            var attributes = attributesDS.GetAny().Where(r => r.ResourceId == resourceID).ToList();

            InitiateExecutionReqMsg executionReqMsg = new InitiateExecutionReqMsg();
            IE.ScriptIdentifier script = new IE.ScriptIdentifier();
            script.ScriptId = observable.Actions.ScriptId;
            script.CategoryId = observable.Actions.CategoryId;
            script.CompanyId = tenantID;
            List<string> auditParams = new List<string>();
            if (observable.Actions.ParameterDetails != null)
            {
                List<IE.Parameter> parametersSE = new List<IE.Parameter>();
                foreach (IE.ObservableParameters parameters in observable.Actions.ParameterDetails)
                {

                    auditParams.Add(parameters.ParamaterName + "=" + parameters.ParameterValue);
                    if (string.Equals(parameters.ParamaterName, "RemoteServerNames", StringComparison.InvariantCultureIgnoreCase))
                    {
                        script.RemoteServerNames = parameters.ParameterValue;
                    }
                    else if (string.Equals(parameters.ParamaterName, "RemoteUserName", StringComparison.InvariantCultureIgnoreCase))
                    {
                        script.UserName = parameters.ParameterValue;
                    }
                    else if (string.Equals(parameters.ParamaterName, "RemotePassword", StringComparison.InvariantCultureIgnoreCase))
                    {
                        script.Password = parameters.ParameterValue;
                    }
                    else if (string.Equals(parameters.ParamaterName, "RemoteExecutionMode", StringComparison.InvariantCultureIgnoreCase))
                    {
                        script.RemoteExecutionMode = Convert.ToInt32(parameters.ParameterValue);
                    }
                    else
                    {
                        var attr = attributes.Where(a => a.AttributeName == parameters.ParamaterName).FirstOrDefault();
                        IE.Parameter param = new IE.Parameter();
                        param.ParameterName = parameters.ParamaterName;
                        param.ParameterValue = attr != null && Convert.ToBoolean(attr.IsSecret) ? SecureData.UnSecure(parameters.ParameterValue, ApplicationConstants.SecureKeys.IAP2) : parameters.ParameterValue;
                        parametersSE.Add(param);
                    }
                }
                script.Parameters = parametersSE;
            }
            executionReqMsg.ScriptIdentifier = script;
            LogHandler.LogDebug(string.Format("Executing SEE with values scriptId:{0} & categoryId:{1}", observable.Actions.ScriptId, observable.Actions.CategoryId), LogHandler.Layer.Business, null);
            Console.WriteLine("Calling SEE for resource ID : " + resourceID);

            ScriptExecute scriptExecute = new ScriptExecute();
            //WEMProxy scriptExecute = new WEMProxy();
            var channel1 = scriptExecute.ServiceChannel;
            //InitiateExecutionResMsg response = channel1.AsyncInitiateExecution(executionReqMsg).Result;
            InitiateExecutionResMsg response;

            string creditApplicationJson = JsonConvert.SerializeObject(executionReqMsg);

            //get the execution mode for this action
            bool isAsync = observable.Actions.ExecutionMode == null ? false : observable.Actions.ExecutionMode.Equals("ASYNC", StringComparison.InvariantCultureIgnoreCase);

            if (isAsync)
                response = await channel1.AsyncInitiateExecution(executionReqMsg);
            else
                response = channel1.InitiateExecution(executionReqMsg);
           
            #region Log Audit Data
            BE.AuditLog auditLogObj = new AuditLog();
            auditLogObj.ResourceID = resourceID;
            auditLogObj.ObservableID = Convert.ToInt32(observable.ObservableId);
            auditLogObj.ActionID = observable.Actions.ActionId;
            auditLogObj.ActionParams = string.Join(",", auditParams);
            auditLogObj.PlatformID = platformID;
            auditLogObj.TenantID = tenantID;
            #endregion

            LogHandler.LogDebug(string.Format("Execution of SEE completed for resourceID:{0} & ActionTypeId:{1}", resourceID, observable.Actions.ActionTypeId), LogHandler.Layer.Business, null);
            if (response != null && response.ScriptResponse != null && response.ScriptResponse.FirstOrDefault().SuccessMessage != null)
            {
                string[] stringSeparators = new string[] { "\r\n", "\n" };
                string[] lines = response.ScriptResponse.FirstOrDefault().SuccessMessage.Split(stringSeparators, StringSplitOptions.None);
                string observableName = observable.Name.ToLower().Replace(" ", "");
                bool isExists = Array.Exists(lines, E => E.Contains("ishealthy") || E.Contains(observable.Name + "=") || E.Contains("output") || E.ToLower().Contains(observableName));
                if (isExists)
                {
                    auditLogObj.Status = "SUCCESS";
                    auditLogObj.Output = response.ScriptResponse.FirstOrDefault().SuccessMessage;

                    Console.WriteLine("Script execution success for resource ID:" + resourceID);

                    LogHandler.LogDebug("Script execution success for resource ID:" + resourceID, LogHandler.Layer.Business, null);
                    LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                    int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), resourceID, observable.ObservableId, response.ScriptResponse.FirstOrDefault().TransactionId, "success", healthcheckType,
                        string.IsNullOrEmpty(response.ScriptResponse.FirstOrDefault().ErrorMessage) ? response.ScriptResponse.FirstOrDefault().SuccessMessage : response.ScriptResponse.FirstOrDefault().ErrorMessage);
                    LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);

                    DE.Queue.Metric metric = new DE.Queue.Metric();
                    string successMessage = response.ScriptResponse.FirstOrDefault().SuccessMessage;
                    string errorMessage = response.ScriptResponse.FirstOrDefault().ErrorMessage;
                    //metric = PopulateMetricData("Server",successMessage, observable.ObservableId, observable.Name, errorMessage, healthcheckType);

                    BE.Metric metricBE = new BE.Metric();
                    metricBE = PopulateMetricData(platformName, eventType, successMessage, resourceID, observable.Name, errorMessage, healthcheckType, ipAddress);
                    EntityTranslator translator = new EntityTranslator();
                    metric = translator.MetricBEToDE(metricBE);
                    metric.Application = platformName;

                    bool isValid = checkNotificationTimeThreshold(platformID, metric.ResourceId, Convert.ToInt32(observable.ObservableId), "Server", metric.ServerIp, metric.MetricValue);
                    if (isValid)
                    {
                        LogHandler.LogDebug(string.Format("Sending metric data to queue and details are EventType:{0}, Application:{1}, MetricTime:{2}, ResourceId:{3}, MetricName:{4}, MetricValue:{5} ,ServerIp:{6}, Description:{7}, Source:{8}",
                                    metric.EventType, metric.Application, metric.MetricTime, metric.ResourceId, metric.MetricName, metric.MetricValue, metric.ServerIp, metric.Description, metric.Source),
                                    LogHandler.Layer.Business, null);

                        MetricProcessorDS processorDS = new MetricProcessorDS();
                        string msgResponse = processorDS.Send(metric, null);
                        Console.WriteLine("Sending message to queue for resource Id " + resourceID + " Response Message : " + msgResponse);
                        LogHandler.LogDebug(string.Format("Metric data sent to queue for resource Id {0} and response message is {1}", resourceID, msgResponse), LogHandler.Layer.Business, null);
                    }
                }
                else
                {
                    auditLogObj.Status = "FAILED";
                    auditLogObj.Output = response.ScriptResponse.FirstOrDefault().SuccessMessage;
                    Console.WriteLine("Script execution failed for resource ID:" + resourceID + "Message " + response.ScriptResponse.FirstOrDefault().SuccessMessage);
                    LogHandler.LogDebug("Script execution failed for resource ID:" + resourceID, LogHandler.Layer.Business, null);
                    LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                    int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), resourceID, observable.ObservableId, response.ScriptResponse.FirstOrDefault().TransactionId, "failed", healthcheckType,
                        string.IsNullOrEmpty(response.ScriptResponse.FirstOrDefault().ErrorMessage) ? response.ScriptResponse.FirstOrDefault().SuccessMessage : response.ScriptResponse.FirstOrDefault().ErrorMessage);
                    LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);
                }
            }
            else
            {
                auditLogObj.Status = "FAILED";
                auditLogObj.Output = response.ScriptResponse.FirstOrDefault().ErrorMessage != null ? response.ScriptResponse.FirstOrDefault().ErrorMessage : response.ScriptResponse.FirstOrDefault().SuccessMessage;
                Console.WriteLine("Script execution failed for resource ID:" + resourceID);
                LogHandler.LogDebug("Script execution failed for resource ID:" + resourceID, LogHandler.Layer.Business, null);
                LogHandler.LogDebug("Executing ObservableHealthchecksUpdate to insert data into healthcheck_iteration_tracker_details table", LogHandler.Layer.Business, null);
                int obsrvableTrackingId = ObservableHealthchecksUpdate(HealtcheckTrackingId.ToString(), resourceID, observable.ObservableId, response.ScriptResponse.FirstOrDefault().TransactionId, "failed", healthcheckType,
                    string.IsNullOrEmpty(response.ScriptResponse.FirstOrDefault().ErrorMessage) ? response.ScriptResponse.FirstOrDefault().SuccessMessage : response.ScriptResponse.FirstOrDefault().ErrorMessage);
                LogHandler.LogDebug("Execution ObservableHealthchecksUpdate completed and trackingdetailsId:" + obsrvableTrackingId, LogHandler.Layer.Business, null);
            }

            Audit_Log audit = new Audit_Log();
            var isLogged = audit.LogAuditData(auditLogObj);

            return true;
        }

        public void EnvironmentScanConsolidatedReport(List<int> observationIds, int platformid, int tenantId)
        {

            int i = 0;
            int waitTime = Convert.ToInt32(ConfigurationManager.AppSettings["WaitTimeinMins"]);
            int retries = Convert.ToInt32(ConfigurationManager.AppSettings["NoofRetries"]);
            do
            {
                ObservationsDS obrDS = new ObservationsDS();
                var res = obrDS.GetAny().Where(o => o.ObservableName == "Environment Scan"
                          && observationIds.Contains(o.ObservationId) && o.ObservationStatus == "In Progress").ToList();
                if (res != null && res.Count > 0)
                    //code block
                    System.Threading.Thread.Sleep(Convert.ToInt32(TimeSpan.FromMinutes(waitTime).TotalMilliseconds));
                else
                {
                    #region WebClient

                    // Create string to hold JSON response
                    string jsonResponse = string.Empty;

                    using (var client = new System.Net.WebClient())
                    {
                        try
                        {
                            client.UseDefaultCredentials = true;
                            client.Headers.Add("Content-Type:application/json");
                            client.Headers.Add("Accept:application/json");
                            //client.Headers.Add("apiKey", "MyKey");
                            string SuperbotURL = ConfigurationManager.AppSettings["ServiceBaseUrl"];
                            var uri = new Uri(SuperbotURL + "/HealthCheck/SendEnvironmentScanConsolidatedReport");
                            // var content = JsonConvert.SerializeObject(request);
                            string input = "{ \"PlatformId\": \"" + platformid + "\",\"TenantId\": \"" + tenantId + "\" }";
                            var response = client.UploadString(uri, "POST", input);
                            jsonResponse = response;
                        }
                        catch (Exception ex)
                        {

                        }
                    }

                    #endregion
                }
                i++;

            } while (i <= retries);


        }
    }

    public enum HealthcheckStatus
    {
        initiated = 0,
        success = 1,
        failed = 2
    };

    public enum ActionTypes
    {
        HealthCheckScript = 1,
        RemediationScript = 2,
        HealthCheckPlatformDB = 3,
        HealthCheckPlatformAPI = 4,
        BaselineMetricScript = 7
    }
    public class LatestPlatform
    {
        public string platformID { get; set; }
        public string resouceID { get; set; }
    }

}
