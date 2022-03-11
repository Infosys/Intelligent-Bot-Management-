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
using Infosys.Solutions.Superbot.Infrastructure.Common;
using DE = Infosys.Solutions.Superbot.Resource.Entity;
using IE = Infosys.Solutions.Ainauto.Services.Superbot.Contracts.Data;
using BE = Infosys.Solutions.Ainauto.BusinessEntity;
using BE2 = Infosys.Solutions.Ainauto.Superbot.BusinessEntity;
using Infosys.Solutions.Ainauto.Resource.DataAccess;
using Newtonsoft.Json;
using Infosys.Solutions.Ainauto.Resource.DataAccess.Queue;
using System.Reflection;
using System.Globalization;
using System.IO;

namespace Infosys.Solutions.Ainauto.Superbot.BusinessComponent
{
    public class EnvironmentScanMetricAnalyser
    {

        public bool CheckAnomaly(BE.Metric metricMessage)
        {
            try
            {
                //DA object for entering anomaly
                EnvironmentScanMetricAnomalyDetailsDS environmentScanMetricAnomalyDetailsDS = new EnvironmentScanMetricAnomalyDetailsDS();
                EnvironmentScanMetricDS environmentScanMetricDS = new EnvironmentScanMetricDS();
                EnvironmentScanMetricDetailsDS environmentScanMetricDetailsDS = new EnvironmentScanMetricDetailsDS();

               //q EGet ObservableId
                ObservableDS observableDS = new ObservableDS();
                DE.observable observable = new DE.observable() { ObservableName = metricMessage.MetricName };
                int observableId = observableDS.GetOne(observable).ObservableId;

                //mapping the json value to metric object
                BE.EnvironmentScanMetricMessage message = JsonConvert.DeserializeObject<BE.EnvironmentScanMetricMessage>(metricMessage.MetricValue);

                //mapping attributes
                Dictionary<string, string> messageMetricDict = new Dictionary<string, string>();
                foreach (var att in message.MetricValue)
                {
                    messageMetricDict.Add(att.AttributeName, att.AttributeValue);
                }

                Dictionary<string, string> dbMetricDict = new Dictionary<string, string>();

                //getting Environment scan table -- old version
                DE.Environment_Scan_Metric environment_Scan_Metric = new DE.Environment_Scan_Metric() { ResourceID = metricMessage.ResourceId, ObservableID = observableId, Version = message.Version - 1 };
                var environmentScanMetricTable = environmentScanMetricDS.GetOne(environment_Scan_Metric);

                //getting Environment scan table -- New version
                environment_Scan_Metric.Version = message.Version;
                var environmentScanMetricTableNew = environmentScanMetricDS.GetOne(environment_Scan_Metric);

                //getting Metric details for a Environmentscan metric id
                DE.Environment_Scan_Metric_Details environment_Scan_Metric_Details = new DE.Environment_Scan_Metric_Details() { EnvironmentScanMetricID = environmentScanMetricTable.EnvironmentScanMetricID };
                var environmentScanMetricDetailsTable = environmentScanMetricDetailsDS.GetAll(environment_Scan_Metric_Details);

                var attributeDetails = (from bmd in environmentScanMetricDetailsTable
                                        where bmd.MetricName == message.MetricName
                                        && bmd.MetricKey == message.MetricKey
                                        select bmd).ToList();


                if (attributeDetails != null && attributeDetails.Count != 0)
                {
                    bool isModified = false;
                    foreach (var att in attributeDetails)
                    {
                        if (att.isActive && !dbMetricDict.ContainsKey(att.AttributeName))
                            dbMetricDict.Add(att.AttributeName, att.AttributeValue);
                    }

                    foreach (var dic in messageMetricDict)
                    {
                        if (dic.Value != dbMetricDict[dic.Key])
                        {
                            //values changed for attribute --  Anomaly detected - MODIFIED
                            //should insert all the metric messsage dictionary into db
                            //setting the flag as true
                            isModified = true;
                            break;
                        }
                    }
                    if (isModified)
                    {
                        //inserting all the messages record into table
                        foreach (var dic in messageMetricDict)
                        {
                            DE.Environment_Scan_Metric_Anomaly_Details environment_Scan_Metric_Anomaly_Details = new DE.Environment_Scan_Metric_Anomaly_Details();
                            environment_Scan_Metric_Anomaly_Details.ResourceId = metricMessage.ResourceId;
                            environment_Scan_Metric_Anomaly_Details.ObservationId = message.ObservationId;
                            environment_Scan_Metric_Anomaly_Details.OldVersion = message.Version - 1;
                            environment_Scan_Metric_Anomaly_Details.NewVersion = message.Version;
                            environment_Scan_Metric_Anomaly_Details.StartDate = environmentScanMetricTable.GeneratedDate; //for older version
                            environment_Scan_Metric_Anomaly_Details.EndDate = environmentScanMetricTableNew.GeneratedDate; //for new version
                            environment_Scan_Metric_Anomaly_Details.MetricId = attributeDetails.First().MetricID;
                            environment_Scan_Metric_Anomaly_Details.MetricName = message.MetricName;
                            environment_Scan_Metric_Anomaly_Details.MetricKey = message.MetricKey;
                            environment_Scan_Metric_Anomaly_Details.AttributeName = dic.Key;
                            environment_Scan_Metric_Anomaly_Details.AttributeValue = dic.Value;
                            environment_Scan_Metric_Anomaly_Details.AttributeStatus = "MODIFIED";
                            environment_Scan_Metric_Anomaly_Details.OldValue = dbMetricDict[dic.Key];//old value in description
                            environment_Scan_Metric_Anomaly_Details.PlatformId = environmentScanMetricTable.PlatformID;
                            environment_Scan_Metric_Anomaly_Details.TenantId = environmentScanMetricTable.TenantID;
                            environment_Scan_Metric_Anomaly_Details.CreatedBy = "admin@123";
                            environment_Scan_Metric_Anomaly_Details.CreateDate = DateTime.UtcNow;

                            environmentScanMetricAnomalyDetailsDS.Insert(environment_Scan_Metric_Anomaly_Details);
                        }
                    }
                }
                else
                {
                    //the metric message is of newly installed software -- Anomaly detected - ADDED
                    DE.Environment_Scan_Metric_Anomaly_Details environment_Scan_Metric_Anomaly_Details = new DE.Environment_Scan_Metric_Anomaly_Details();

                    //entering data into the anomaly table by looping the input metric message
                    foreach (var att in message.MetricValue)
                    {
                        environment_Scan_Metric_Anomaly_Details.ResourceId = metricMessage.ResourceId;
                        environment_Scan_Metric_Anomaly_Details.ObservationId = message.ObservationId;
                        environment_Scan_Metric_Anomaly_Details.OldVersion = message.Version - 1;
                        environment_Scan_Metric_Anomaly_Details.NewVersion = message.Version;
                        environment_Scan_Metric_Anomaly_Details.StartDate = environmentScanMetricTable.GeneratedDate;
                        environment_Scan_Metric_Anomaly_Details.EndDate = environmentScanMetricTableNew.GeneratedDate;
                        environment_Scan_Metric_Anomaly_Details.MetricId = message.MetricId;
                        environment_Scan_Metric_Anomaly_Details.MetricName = message.MetricName;
                        environment_Scan_Metric_Anomaly_Details.MetricKey = message.MetricKey;
                        environment_Scan_Metric_Anomaly_Details.AttributeName = att.AttributeName;
                        environment_Scan_Metric_Anomaly_Details.AttributeValue = att.AttributeValue;
                        environment_Scan_Metric_Anomaly_Details.AttributeStatus = "ADDED";
                        environment_Scan_Metric_Anomaly_Details.OldValue = "";
                        environment_Scan_Metric_Anomaly_Details.PlatformId = environmentScanMetricTable.PlatformID;
                        environment_Scan_Metric_Anomaly_Details.TenantId = environmentScanMetricTable.TenantID;
                        environment_Scan_Metric_Anomaly_Details.CreatedBy = "admin@123";
                        environment_Scan_Metric_Anomaly_Details.CreateDate = DateTime.UtcNow;

                        environmentScanMetricAnomalyDetailsDS.Insert(environment_Scan_Metric_Anomaly_Details);
                    }
                }

                //checking if this is the last message
                if (metricMessage.SequenceNumber.Trim().Split('/')[0].Trim() == metricMessage.SequenceNumber.Trim().Split('/')[1].Trim())
                {
                    //checking for any deleted software
                    //environment_Scan_Metric.Version = message.Version;
                    //environmentScanMetricTable = environmentScanMetricDS.GetOne(environment_Scan_Metric);

                    environment_Scan_Metric_Details.EnvironmentScanMetricID = environmentScanMetricTableNew.EnvironmentScanMetricID;
                    var environmentScanMetricDetailsTableNew = environmentScanMetricDetailsDS.GetAll(environment_Scan_Metric_Details);

                    var grouped_environmentScanMetricDetailsTable = environmentScanMetricDetailsTable.GroupBy(e => new { e.MetricKey, e.MetricName });

                    foreach (var groupp in grouped_environmentScanMetricDetailsTable)
                    {
                        var result = (from e in environmentScanMetricDetailsTableNew
                                      where e.MetricName == groupp.Key.MetricName
                                      && e.MetricKey == groupp.Key.MetricKey
                                      select e).ToList();

                        if (result == null || result.Count == 0)
                        {
                            //this software has been removed -- Anomaly detected - deleted
                            foreach (var groupObj in groupp)
                            {
                                DE.Environment_Scan_Metric_Anomaly_Details environment_Scan_Metric_Anomaly_Details = new DE.Environment_Scan_Metric_Anomaly_Details();
                                environment_Scan_Metric_Anomaly_Details.ResourceId = metricMessage.ResourceId;
                                environment_Scan_Metric_Anomaly_Details.ObservationId = message.ObservationId;
                                environment_Scan_Metric_Anomaly_Details.OldVersion = message.Version - 1;
                                environment_Scan_Metric_Anomaly_Details.NewVersion = message.Version;
                                environment_Scan_Metric_Anomaly_Details.StartDate = environmentScanMetricTable.GeneratedDate;
                                environment_Scan_Metric_Anomaly_Details.EndDate = environmentScanMetricTableNew.GeneratedDate;
                                environment_Scan_Metric_Anomaly_Details.MetricId = groupObj.MetricID;
                                environment_Scan_Metric_Anomaly_Details.MetricName = groupObj.MetricName;
                                environment_Scan_Metric_Anomaly_Details.MetricKey = groupObj.MetricKey;
                                environment_Scan_Metric_Anomaly_Details.AttributeName = groupObj.AttributeName;
                                environment_Scan_Metric_Anomaly_Details.AttributeValue = "";
                                environment_Scan_Metric_Anomaly_Details.AttributeStatus = "DELETED";
                                environment_Scan_Metric_Anomaly_Details.OldValue = groupObj.AttributeValue;
                                environment_Scan_Metric_Anomaly_Details.PlatformId = environmentScanMetricTableNew.PlatformID;
                                environment_Scan_Metric_Anomaly_Details.TenantId = environmentScanMetricTableNew.TenantID;
                                environment_Scan_Metric_Anomaly_Details.CreatedBy = "admin@123";
                                environment_Scan_Metric_Anomaly_Details.CreateDate = DateTime.UtcNow;

                                environmentScanMetricAnomalyDetailsDS.Insert(environment_Scan_Metric_Anomaly_Details);
                            }
                        }
                    }

                    //checking for anomalies to send a notification
                    DE.Environment_Scan_Metric_Anomaly_Details environment_Scan_Metric_Anomaly_Details_notification = new DE.Environment_Scan_Metric_Anomaly_Details();
                    environment_Scan_Metric_Anomaly_Details_notification.ResourceId = metricMessage.ResourceId;
                    environment_Scan_Metric_Anomaly_Details_notification.TenantId = environmentScanMetricTableNew.TenantID;
                    environment_Scan_Metric_Anomaly_Details_notification.PlatformId = environmentScanMetricTableNew.PlatformID;
                    environment_Scan_Metric_Anomaly_Details_notification.ObservationId = message.ObservationId;

                    var anomalyDetails = environmentScanMetricAnomalyDetailsDS.GetAll(environment_Scan_Metric_Anomaly_Details_notification);

                    DE.observations observationsDetails = new DE.observations { ObservationId = message.ObservationId };
                    ObservationsDS observationsDS = new ObservationsDS();

                    if (anomalyDetails != null && anomalyDetails.Count > 0)
                    {
                        //Entries in Anomaly details table -- Anomaly found 
                        #region updating observations table
                        observationsDetails.ObservationStatus = "Completed";
                        observationsDetails.Value = "Non compliance";
                        observationsDetails = observationsDS.Update(observationsDetails);
                        #endregion

                        if (Helper.CheckNotificationRestriction(metricMessage.ResourceId,observableId, environmentScanMetricTableNew.PlatformID, environmentScanMetricTableNew.TenantID))
                        {
                            #region sending notification
                            LogHandler.LogDebug(String.Format("Creating an Notification message to send to the queue"),
                                        LogHandler.Layer.Business, null);
                            DE.Queue.Notification notification = new DE.Queue.Notification();

                            notification.ObservationId = message.ObservationId;
                            notification.PlatformId = environmentScanMetricTableNew.PlatformID;
                            notification.ResourceId = metricMessage.ResourceId;
                            notification.ObservableId = Helper.ConvertToInt(metricMessage.ObservableId);
                            notification.ObservableName = metricMessage.MetricName;
                            notification.ObservationStatus = observationsDetails.ObservationStatus;
                            notification.Value = observationsDetails.Value;
                            notification.ThresholdExpression = observationsDetails.Description;
                            notification.ServerIp = metricMessage.ServerIp;
                            notification.ObservationTime = observationsDetails.ObservationTime.ToString();
                            notification.EventType = "Anomaly report";
                            notification.Source = "Platform";
                            notification.TenantId = environmentScanMetricTableNew.TenantID;
                            notification.Type = (int)NotificationType.EnvironmentScan;
                            notification.Channel = (int)NotificationChannel.Email;
                            notification.BaseURL = "";
                            notification.Description = observationsDetails.Description;

                            NotificationDS not = new NotificationDS();
                            not.Send(notification, "");
                            LogHandler.LogDebug(String.Format("The Notification message has been sent to the queue"),
                                LogHandler.Layer.Business, null);
                            #endregion
                        }

                    }
                    else
                    {
                        //No Entries in Anomaly details table -- Anomaly not found 
                        #region updating observations table
                        observationsDetails.ObservationStatus = "Completed";
                        observationsDetails.Value = "Compliance";
                        var res = observationsDS.Update(observationsDetails);
                        #endregion
                    }

                }
                return true;
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
        public BE2.finalReport GetEnvironmentScanComparisonPlatformReport(int platformId, int tenantId, string startDate, string endDate)
        {

            BE2.EnvironmentScanComparisonReport report = new BE2.EnvironmentScanComparisonReport();
            ResourceDS rs = new ResourceDS();
            ResourceTypeDS rsType = new ResourceTypeDS();
            List<BE2.EnvironmentScanComparisonReport2> scanReport = new List<BE2.EnvironmentScanComparisonReport2>();
            List<BE2.ActiveResource> activeResources = new List<BE2.ActiveResource>();
            List<BE2.Details> det = new List<BE2.Details>();
            List<BE2.OSResoutions> osDetailsList = new List<BE2.OSResoutions>();
            List<BE2.SRDetails> srDetailsList = new List<BE2.SRDetails>();
            activeResources = (from a in rs.GetAny().ToArray()
                                   join b in rsType.GetAny().ToArray() on a.ResourceTypeId equals b.ResourceTypeId
                                   where a.PlatformId == platformId &&
                                   a.TenantId == tenantId &&
                                   a.IsActive == true
                                   select new BE2.ActiveResource
                                   {
                                       ResourceId = a.ResourceId,
                                       ResourceName = a.ResourceName,
                                       ResourceTypeId = a.ResourceTypeId,
                                       ResourceTypeName = b.ResourceTypeName



                                   }).ToList();
            try {
                foreach (BE2.ActiveResource activers in activeResources)
                {
                    try
                    {
                        report = GetEnvironmentScanComparisonReport(activers.ResourceId, startDate, endDate, platformId, tenantId);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                    


                    foreach (BE2.ESMetricValues matValue in report.ResourceDetails.MetricDetails.MetricValue)
                    {
                        if (matValue.MetricName.Equals("Installed Software"))
                        {
                            BE2.Details softwareDet = new BE2.Details();
                            softwareDet.resource = activers.ResourceName;
                            softwareDet.resourcetype = activers.ResourceTypeName;
                            softwareDet.status = matValue.Status;

                            foreach (BE2.ESAttributes attr in matValue.Attributes)
                            {
                                
                                if (attr.AttributeName.Equals("Display name"))
                                {
                                    softwareDet.SoftwareName = attr.AttributeValue;
                                }
                                else if (attr.AttributeName.Equals("Old Version"))
                                {
                                    softwareDet.oldversion = attr.AttributeValue;
                                }
                                else if (attr.AttributeName.Equals("Old InstallDate"))
                                {
                                    softwareDet.oldinstalleddate = attr.AttributeValue;
                                }
                                else if (attr.AttributeName.Equals("Old Publisher"))
                                {
                                    softwareDet.oldpublisher = attr.AttributeValue;
                                }
                                else if (attr.AttributeName.Equals("Version"))
                                {
                                    softwareDet.newversion = attr.AttributeValue;
                                }
                                else if (attr.AttributeName.Equals("InstalDate"))
                                {
                                    softwareDet.newinstalleddate = attr.AttributeValue;
                                }
                                else if (attr.AttributeName.Equals("Publisher"))
                                {
                                    softwareDet.newpublisher = attr.AttributeValue;

                                }
                                


                            }
                            det.Add(softwareDet);

                        }

                       else if (matValue.MetricName.Equals("OS Details"))
                        {


                            BE2.OSResoutions osdet = new BE2.OSResoutions();
                            osdet.resource = activers.ResourceName;
                            osdet.resourcetype = activers.ResourceTypeName;
                            osdet.status = matValue.Status;
                            foreach (BE2.ESAttributes attr in matValue.Attributes)
                            {
                                if (attr.AttributeName.Equals("Computer Name"))
                                {
                                    osdet.name = attr.AttributeValue;

                                }
                                else if (attr.AttributeName.Equals("Old Version"))
                                {
                                    osdet.oldVersion = attr.AttributeValue;

                                }

                                else if (attr.AttributeName.Equals("Old BuildNumber"))
                                {
                                    osdet.oldbuildnumber = attr.AttributeValue;

                                }
                                else if (attr.AttributeName.Equals("Version"))
                                {
                                    osdet.newVersion = attr.AttributeValue;

                                }
                                else if (attr.AttributeName.Equals("BuildNumber"))
                                {
                                    osdet.newbuildnumber = attr.AttributeValue;

                                }

                                

                            }
                            osDetailsList.Add(osdet);

                        }
                        else if (matValue.MetricName.Equals("Screen Resolution"))
                        {

                            BE2.SRDetails SR = new BE2.SRDetails();
                            SR.resource = activers.ResourceName;
                            SR.resourceyype = activers.ResourceTypeName;
                            SR.status = matValue.Status;
                            foreach (BE2.ESAttributes attr in matValue.Attributes)
                            {
                                if (attr.AttributeName.Equals("Old Version"))
                                {
                                    SR.oldversion = attr.AttributeValue;

                                }


                                else if (attr.AttributeName.Equals("Version"))
                                {
                                    SR.newversion = attr.AttributeValue;

                                }
                                
                            }
                            srDetailsList.Add(SR);



                        }



                    }
                }

                    BE2.EnvironmentScanComparisonReport2 softwareDetailsReport2 = new BE2.EnvironmentScanComparisonReport2();
                    softwareDetailsReport2.metricName = "SoftwareDetails";
                    softwareDetailsReport2.details = det;
                    BE2.OSDetails osDetailsReport = new BE2.OSDetails();
                    osDetailsReport.metricName = "OSDetails";
                    osDetailsReport.details = osDetailsList;
                    BE2.ScreenResoution screenResolutionReport = new BE2.ScreenResoution();
                    screenResolutionReport.metricName = "SR Resolution";
                    screenResolutionReport.details = srDetailsList;

                    BE2.finalReport finalReport = new BE2.finalReport();
                    finalReport.softwareReport = softwareDetailsReport2;
                    finalReport.osDetailsReport = osDetailsReport;
                    finalReport.srDetails = screenResolutionReport;
                   
                    return finalReport;

                }
                catch (Exception ex)
                {
                    throw ex;
                

                }

                


            
            
        }










                // List<BE.>



            
        

       public BE2.EnvironmentScanComparisonReport GetEnvironmentScanComparisonReport(string resourceId, string startDate, string endDate, int platformId, int tenantId)
       {

        BE2.EnvironmentScanComparisonReport returnObj = new BE2.EnvironmentScanComparisonReport();

        PlatformsDS platformsDS = new PlatformsDS();
        TenantDS tenantDS = new TenantDS();            
        ObservableDS observableDS = new ObservableDS();

        EnvironmentScanMetricDS environmentScanMetricDS = new EnvironmentScanMetricDS();
        EnvironmentScanMetricDetailsDS environmentScanMetricDetailsDS = new EnvironmentScanMetricDetailsDS();

        //getting the EnvironmentScanMetric details based on date
        DE.Environment_Scan_Metric environmentScanMetricStart_DE = new DE.Environment_Scan_Metric ();
        DE.Environment_Scan_Metric environmentScanMetricEnd_DE = new DE.Environment_Scan_Metric ();

        var environmentScanMetricTable = environmentScanMetricDS.GetAll();

        environmentScanMetricStart_DE = (from e in environmentScanMetricTable
                                         where e.ResourceID == resourceId
                                         && e.PlatformID == platformId
                                         && e.TenantID == tenantId
                                         && e.GeneratedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) == Convert.ToDateTime(startDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                                         select e).LastOrDefault();
        environmentScanMetricEnd_DE = (from e in environmentScanMetricTable
                                       where e.ResourceID == resourceId
                                       && e.PlatformID == platformId
                                       && e.TenantID == tenantId
                                       && e.GeneratedDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture) == Convert.ToDateTime(endDate).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)
                                       select e).LastOrDefault();

        //GATHERING BASIC DETAILS 
        DE.platforms platformsObj_DE = new DE.platforms { PlatformId = platformId};
        platformsObj_DE = new PlatformsDS().GetOne(platformsObj_DE);

        DE.tenant tenantObj_DE = new DE.tenant { TenantId = tenantId };
        tenantObj_DE = new TenantDS().GetOne(tenantObj_DE);

        DE.resource resourceObj_DE = new DE.resource { ResourceId = resourceId };
        resourceObj_DE = new ResourceDS().GetOne(resourceObj_DE);

        DE.resourcetype resourcetypeObj_DE = new DE.resourcetype { ResourceTypeId = resourceObj_DE.ResourceTypeId };
        resourcetypeObj_DE = new ResourceTypeDS().GetOne(resourcetypeObj_DE);

        DE.observable observableObj_DE = new DE.observable { ObservableId = environmentScanMetricEnd_DE.ObservableID};
        observableObj_DE = new ObservableDS().GetOne(observableObj_DE);

        DE.Environment_Scan_Metric_Details environmentScanMetricDetailsStart_DE = new DE.Environment_Scan_Metric_Details { EnvironmentScanMetricID = environmentScanMetricStart_DE.EnvironmentScanMetricID};
        DE.Environment_Scan_Metric_Details environmentScanMetricDetailsEnd_DE = new DE.Environment_Scan_Metric_Details { EnvironmentScanMetricID = environmentScanMetricEnd_DE.EnvironmentScanMetricID };

        var environmentScanMetricDetailsStart = environmentScanMetricDetailsDS.GetAll(environmentScanMetricDetailsStart_DE);
        var environmentScanMetricDetailsEnd = environmentScanMetricDetailsDS.GetAll(environmentScanMetricDetailsEnd_DE);

        var grouped_environmentScanMetricDetailsStart = environmentScanMetricDetailsStart.GroupBy(e => new { e.MetricKey, e.MetricName, e.MetricID });
        var grouped_environmentScanMetricDetailsEnd = environmentScanMetricDetailsEnd.GroupBy(e => new { e.MetricKey, e.MetricName, e.MetricID });

        List<BE2.ESMetricValues> metricValuesList = new List<BE2.ESMetricValues>();

        //loop throught end (new) group to find added attributes -- FOR ADDED
        foreach (var groupp in grouped_environmentScanMetricDetailsEnd)
        {
            var result = (from e in environmentScanMetricDetailsStart
                          where e.MetricName == groupp.Key.MetricName
                          && e.MetricKey == groupp.Key.MetricKey
                          && e.MetricID == groupp.Key.MetricID
                          select e).ToList();
            if(result==null || result.Count == 0)
            {
                //added
                BE2.ESMetricValues metricValues = new BE2.ESMetricValues();                    
                List<BE2.ESAttributes>  attributesList = new List<BE2.ESAttributes>();
                foreach (var grouppObj in groupp)
                {
                    BE2.ESAttributes attributes = new BE2.ESAttributes();

                    attributes.AttributeName = grouppObj.AttributeName;
                    attributes.AttributeValue = grouppObj.AttributeValue;
                    attributes.DisplayName = grouppObj.DisplayName;

                    attributesList.Add(attributes);
                }

                metricValues.MetricId = groupp.Key.MetricID.ToString();
                metricValues.MetricKey = groupp.Key.MetricKey;
                metricValues.MetricName = groupp.Key.MetricName;
                metricValues.Status = "ADDED";
                metricValues.Attributes = attributesList;

                metricValuesList.Add(metricValues);
            }
            else
            {
                //check for value modifications
                Dictionary<string, string> startDict = new Dictionary<string, string>();
                Dictionary<string, string> endDict = new Dictionary<string, string>();                    
                bool isModified = false;

                foreach (var groupObj in groupp)
                {
                    if (groupObj.isActive)
                        endDict.Add(groupObj.AttributeName, groupObj.AttributeValue);
                }
                foreach (var resultObj in result)
                {
                    if (resultObj.isActive)
                        startDict.Add(resultObj.AttributeName, resultObj.AttributeValue);
                }

                foreach (var dic in endDict)
                {
                    if (dic.Value != startDict[dic.Key])
                    {
                        //values changed for attribute --  Anomaly detected - MODIFIED
                        //should insert all the metric messsage dictionary into db
                        //setting the flag as true
                        isModified = true;
                        break;
                    }
                }
                if (isModified)
                {
                    BE2.ESMetricValues metricValues = new BE2.ESMetricValues();
                    List<BE2.ESAttributes> attributesList = new List<BE2.ESAttributes>();

                    foreach (var grouppObj in groupp)
                    {
                        BE2.ESAttributes attributes = new BE2.ESAttributes();

                        attributes.AttributeName = grouppObj.AttributeName;
                        attributes.AttributeValue = grouppObj.AttributeValue;
                        attributes.DisplayName = grouppObj.DisplayName;

                        attributesList.Add(attributes);

                        BE2.ESAttributes attributesOld = new BE2.ESAttributes();

                        attributesOld.AttributeName = "Old " + grouppObj.AttributeName;
                        attributesOld.AttributeValue = startDict[grouppObj.AttributeName];
                        attributesOld.DisplayName = "Old " + grouppObj.DisplayName;

                        attributesList.Add(attributesOld);
                    }

                    metricValues.MetricId = groupp.Key.MetricID.ToString();
                    metricValues.MetricKey = groupp.Key.MetricKey;
                    metricValues.MetricName = groupp.Key.MetricName;
                    metricValues.Status = "MODIFIED";
                    metricValues.Attributes = attributesList;

                    metricValuesList.Add(metricValues);
                }
            }
        }
        //loop through start(old) group to find deleted attributes -- FOR DELETED
        foreach (var groupp in grouped_environmentScanMetricDetailsStart)
        {
            var result = (from e in environmentScanMetricDetailsEnd
                          where e.MetricName == groupp.Key.MetricName
                          && e.MetricKey == groupp.Key.MetricKey
                          && e.MetricID == groupp.Key.MetricID
                          select e).ToList();

            if (result == null || result.Count == 0)
            {
                //added
                BE2.ESMetricValues metricValues = new BE2.ESMetricValues();
                List<BE2.ESAttributes> attributesList = new List<BE2.ESAttributes>();
                foreach (var grouppObj in groupp)
                {
                    BE2.ESAttributes attributes = new BE2.ESAttributes();

                    attributes.AttributeName = "Old " + grouppObj.AttributeName;
                    attributes.AttributeValue = grouppObj.AttributeValue;
                    attributes.DisplayName = "Old " + grouppObj.DisplayName;

                    attributesList.Add(attributes);
                }

                metricValues.MetricId = groupp.Key.MetricID.ToString();
                metricValues.MetricKey = groupp.Key.MetricKey;
                metricValues.MetricName = groupp.Key.MetricName;
                metricValues.Status = "DELETED";
                metricValues.Attributes = attributesList;

                metricValuesList.Add(metricValues);
            }
        }

        BE2.ESMetricDetails metricDetails = new BE2.ESMetricDetails();
        metricDetails.ObservableId = observableObj_DE.ObservableId.ToString();
        metricDetails.ObservableName = observableObj_DE.ObservableName;
        metricDetails.MetricValue = metricValuesList;

        BE2.ESResourceDetails resourceDetails = new BE2.ESResourceDetails();
        resourceDetails.ResourceId = resourceObj_DE.ResourceId;
        resourceDetails.ResourceName = resourceObj_DE.ResourceName;
        resourceDetails.ResourceTypeId = resourcetypeObj_DE.ResourceTypeId.ToString();
        resourceDetails.ResourceTypeName = resourcetypeObj_DE.ResourceTypeName;
        resourceDetails.MetricDetails = metricDetails;

        returnObj.PlatformId = platformsObj_DE.PlatformId.ToString();
        returnObj.PlatformName = platformsObj_DE.PlatformName;
        returnObj.TenantId = tenantObj_DE.TenantId.ToString();
        returnObj.TenantName = tenantObj_DE.Name;
        returnObj.StartDate = environmentScanMetricStart_DE.GeneratedDate.ToString();
        returnObj.EndDate = environmentScanMetricEnd_DE.GeneratedDate.ToString();
        returnObj.ResourceDetails = resourceDetails;

        return returnObj;

        }

    }
        
}
