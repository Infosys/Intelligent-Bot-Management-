/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IE = Infosys.Solutions.Ainauto.Services.Superbot.Contracts.Data;
using BE = Infosys.Solutions.Ainauto.BusinessEntity;
using BE2 = Infosys.Solutions.Ainauto.Superbot.BusinessEntity;

namespace Infosys.Solutions.Ainauto.Superbot.Translator
{
    public static class ResourceHandler_BE_IE
    {
        public static IE.PlatformInstanceDetails PlatfromInstance_BEtoIE(BE.PlatformInstanceDetails platformInstanceBE)
        {
            IE.PlatformInstanceDetails platformInstanceIE = new IE.PlatformInstanceDetails();
            platformInstanceIE.VendorName = platformInstanceBE.VendorName;
            platformInstanceIE.PlatformId = platformInstanceBE.PlatformId;
            platformInstanceIE.TenantId = platformInstanceBE.TenantId;
            platformInstanceIE.UID = platformInstanceBE.UID;
            platformInstanceIE.Pwd = platformInstanceBE.Pwd;
            platformInstanceIE.PlatformResourceModelVersion = platformInstanceBE.PlatformResourceModelVersion;

            List<IE.PlatformObservable> platformObservablesIE = new List<IE.PlatformObservable>();
            if (platformInstanceBE.platformObservables!=null)
            {
                foreach (BE.PlatformObservable platformObservableBE in platformInstanceBE.platformObservables)
                {
                    IE.PlatformObservable platformObservableIE = new IE.PlatformObservable();
                    platformObservableIE.Name = platformObservableBE.Name;
                    platformObservableIE.ObservableId = platformObservableBE.ObservableId;
                    if (platformObservableBE.Actions != null)
                    {
                        IE.Actions actionsIE = new IE.Actions();
                        actionsIE.ActionId = platformObservableBE.Actions.ActionId;
                        actionsIE.ScriptId = platformObservableBE.Actions.ScriptId;
                        actionsIE.CategoryId = platformObservableBE.Actions.CategoryId;
                        actionsIE.AutomationEngineId = platformObservableBE.Actions.AutomationEngineId;
                        actionsIE.ActionTypeId = platformObservableBE.Actions.ActionTypeId;
                        actionsIE.ExecutionMode = platformObservableBE.Actions.ExecutionMode;
                        if (platformObservableBE.Actions.ParameterDetails != null)
                        {
                            foreach (BE.ObservableParameters param in platformObservableBE.Actions.ParameterDetails)
                            {
                                IE.ObservableParameters parameterIE = new IE.ObservableParameters();
                                parameterIE.ParamaterName = param.ParamaterName;
                                parameterIE.ParameterValue = param.ParameterValue;
                                parameterIE.IsSecret = param.IsSecret;
                                actionsIE.ParameterDetails.Add(parameterIE);
                            }
                        }
                        platformObservableIE.Actions = actionsIE;
                    }
                    platformObservablesIE.Add(platformObservableIE);

                }
                platformInstanceIE.platformObservables = platformObservablesIE;
            }

            if (platformInstanceBE.servers != null)
            {
                List<IE.Server> serversIE = new List<IE.Server>();
                foreach (BE.Server serverBE in platformInstanceBE.servers)
                {
                    IE.Server serverIE = new IE.Server();
                    serverIE.IPAddress = serverBE.IPAddress;
                    serverIE.Name = serverBE.Name;
                    serverIE.Type = serverBE.Type;
                   
                    //translate server observables
                    if (serverBE.ServerObservables != null)
                    {
                        List<IE.ServerObservable> serverObservablesIE = new List<IE.ServerObservable>();
                        foreach (BE.ServerObservable serverObservableBE in serverBE.ServerObservables)
                        {
                            IE.ServerObservable serverObservableIE = new IE.ServerObservable();
                            serverObservableIE.IPAddress = serverObservableBE.IPAddress;
                            serverObservableIE.Name = serverObservableBE.Name;
                            serverObservableIE.ObservableId = serverObservableBE.ObservableId;
                            serverObservableIE.ResourceId = serverObservableBE.ResourceId;
                            if (serverObservableBE.Actions != null)
                            {
                                IE.Actions actionsIE = new IE.Actions();
                                actionsIE.ActionId = serverObservableBE.Actions.ActionId;
                                actionsIE.ScriptId = serverObservableBE.Actions.ScriptId;
                                actionsIE.CategoryId = serverObservableBE.Actions.CategoryId;
                                actionsIE.AutomationEngineId = serverObservableBE.Actions.AutomationEngineId;
                                actionsIE.ActionTypeId = serverObservableBE.Actions.ActionTypeId;
                                actionsIE.ActionName = serverObservableBE.Actions.ActionName;
                                actionsIE.ExecutionMode = serverObservableBE.Actions.ExecutionMode;
                                if (serverObservableBE.Actions.ParameterDetails != null)
                                {
                                    actionsIE.ParameterDetails = new List<IE.ObservableParameters>();
                                    foreach(BE.ObservableParameters param in serverObservableBE.Actions.ParameterDetails)
                                    {
                                        IE.ObservableParameters parameterIE = new IE.ObservableParameters();
                                        parameterIE.ParamaterName = param.ParamaterName;
                                        parameterIE.ParameterValue = param.ParameterValue;
                                        parameterIE.IsSecret = param.IsSecret;
                                        actionsIE.ParameterDetails.Add(parameterIE);
                                    }
                                }
                                serverObservableIE.Actions = actionsIE;
                            }
                            serverObservablesIE.Add(serverObservableIE);
                        }
                        serverIE.ServerObservables = serverObservablesIE;
                    }
                                       
                    
                    //translate bots list
                    if (serverBE.bots != null)
                    {
                        List<IE.Bot> botsIE = new List<IE.Bot>();
                        foreach (BE.Bot botBE in serverBE.bots)
                        {
                            IE.Bot botIE = new IE.Bot();
                            botIE.IPAddress = botBE.IPAddress;
                            botIE.BotName = botBE.BotName;
                            botIE.BotInstanceId = botBE.BotInstanceId;
                            //tranlate bot observables
                            if (botBE.botObservables != null)
                            {
                                List<IE.BotObservable> botObservablesIE = new List<IE.BotObservable>();
                                foreach (BE.BotObservable botObservableBE in botBE.botObservables)
                                {
                                    IE.BotObservable botObservableIE = new IE.BotObservable();
                                    botObservableIE.Name = botObservableBE.Name;
                                    botObservableIE.ObservableId = botObservableBE.ObservableId;
                                    if (botObservableBE.Actions != null)
                                    {
                                        IE.Actions actionsIE = new IE.Actions();
                                        actionsIE.ActionId = botObservableBE.Actions.ActionId;
                                        actionsIE.ScriptId = botObservableBE.Actions.ScriptId;
                                        actionsIE.CategoryId = botObservableBE.Actions.CategoryId;
                                        actionsIE.AutomationEngineId = botObservableBE.Actions.AutomationEngineId;
                                        actionsIE.ActionTypeId = botObservableBE.Actions.ActionTypeId;
                                        actionsIE.ActionName = botObservableBE.Actions.ActionName;
                                        actionsIE.ExecutionMode = botObservableBE.Actions.ExecutionMode;
                                        if (botObservableBE.Actions.ParameterDetails != null)
                                        {
                                            actionsIE.ParameterDetails = new List<IE.ObservableParameters>();
                                            foreach (BE.ObservableParameters param in botObservableBE.Actions.ParameterDetails)
                                            {
                                                IE.ObservableParameters parameterIE = new IE.ObservableParameters();
                                                parameterIE.ParamaterName = param.ParamaterName;
                                                parameterIE.ParameterValue = param.ParameterValue;
                                                parameterIE.IsSecret = param.IsSecret;
                                                actionsIE.ParameterDetails.Add(parameterIE);
                                            }
                                        }
                                        botObservableIE.Actions = actionsIE;
                                    }
                                    botObservablesIE.Add(botObservableIE);
                                }
                                botIE.botObservables = botObservablesIE;
                            }
                            botsIE.Add(botIE);
                        }
                        serverIE.bots = botsIE;
                    }

                    //translate services list
                    if (serverBE.services != null)
                    {
                        List<IE.Service> servicesIE = new List<IE.Service>();
                        foreach (BE.Service serviceBE in serverBE.services)
                        {
                            IE.Service serviceIE = new IE.Service();
                            serviceIE.IPAddress = serviceBE.IPAddress;
                            serviceIE.ServiceName = serviceBE.ServiceName;
                            serviceIE.ServiceId = serviceBE.ServiceId;
                            //tranlate service observables
                            if (serviceBE.serviceObservables != null)
                            {
                                List<IE.ServiceObservable> serviceObservablesIE = new List<IE.ServiceObservable>();
                                foreach (BE.ServiceObservable serviceObservableBE in serviceBE.serviceObservables)
                                {
                                    IE.ServiceObservable serviceObservableIE = new IE.ServiceObservable();
                                    serviceObservableIE.Name = serviceObservableBE.Name;
                                    serviceObservableIE.ObservableId = serviceObservableBE.ObservableId;
                                    if (serviceObservableBE.Actions != null)
                                    {
                                        IE.Actions actionsIE = new IE.Actions();
                                        actionsIE.ActionId = serviceObservableBE.Actions.ActionId;
                                        actionsIE.ScriptId = serviceObservableBE.Actions.ScriptId;
                                        actionsIE.CategoryId = serviceObservableBE.Actions.CategoryId;
                                        actionsIE.AutomationEngineId = serviceObservableBE.Actions.AutomationEngineId;
                                        actionsIE.ActionTypeId = serviceObservableBE.Actions.ActionTypeId;
                                        actionsIE.ActionName = serviceObservableBE.Actions.ActionName;
                                        actionsIE.ExecutionMode = serviceObservableBE.Actions.ExecutionMode;
                                        if (serviceObservableBE.Actions.ParameterDetails != null)
                                        {
                                            actionsIE.ParameterDetails = new List<IE.ObservableParameters>();
                                            foreach (BE.ObservableParameters param in serviceObservableBE.Actions.ParameterDetails)
                                            {
                                                IE.ObservableParameters parameterIE = new IE.ObservableParameters();
                                                parameterIE.ParamaterName = param.ParamaterName;
                                                parameterIE.ParameterValue = param.ParameterValue;
                                                parameterIE.IsSecret = param.IsSecret;
                                                actionsIE.ParameterDetails.Add(parameterIE);
                                            }
                                        }
                                        serviceObservableIE.Actions = actionsIE;
                                    }
                                    serviceObservablesIE.Add(serviceObservableIE);
                                }
                                serviceIE.serviceObservables = serviceObservablesIE;
                            }
                            servicesIE.Add(serviceIE);
                        }
                        serverIE.services = servicesIE;
                    }
                    serversIE.Add(serverIE);
                }
                platformInstanceIE.servers = serversIE;
            }

            return platformInstanceIE;
        }

        public static IE.ObservationDetails NotificationObservationData_BEtoIE(BE2.ObservationDetails notificationTransactionDataBE)
        {
            IE.ObservationDetails notificationTransactionDataIE = new IE.ObservationDetails();
            notificationTransactionDataIE.ObservationId = notificationTransactionDataBE.ObservationId;
            notificationTransactionDataIE.ObservableId = notificationTransactionDataBE.ObservableId;
            notificationTransactionDataIE.ObservableName = notificationTransactionDataBE.ObservableName;
            notificationTransactionDataIE.ObservationStatus = notificationTransactionDataBE.ObservationStatus;
            notificationTransactionDataIE.ObservationTime = notificationTransactionDataBE.ObservationTime;
            notificationTransactionDataIE.Description = notificationTransactionDataBE.Description;
            notificationTransactionDataIE.RemediationPlanId = notificationTransactionDataBE.RemediationPlanId;
            notificationTransactionDataIE.RemediationStatus = notificationTransactionDataBE.RemediationStatus;
            notificationTransactionDataIE.RemediationPlanTime = notificationTransactionDataBE.RemediationPlanTime;
            notificationTransactionDataIE.ServerIp = notificationTransactionDataBE.ServerIp;
            notificationTransactionDataIE.Source = notificationTransactionDataBE.Source;
            notificationTransactionDataIE.AlertTime = notificationTransactionDataBE.AlertTime;
            return notificationTransactionDataIE;
        }

        public static IE.RemediationPlanDetails NotificationRemediationPlanData_BEtoIE(BE2.RemediationPlanDetails remediationPlanDataBE)
        {
            IE.RemediationPlanDetails RemediationPlanDataIE = new IE.RemediationPlanDetails();
            RemediationPlanDataIE.RemediationPlanId = remediationPlanDataBE.RemediationPlanId;
            RemediationPlanDataIE.RemediationPlanName = remediationPlanDataBE.RemediationPlanName;
            RemediationPlanDataIE.RemediationPlanDescription = remediationPlanDataBE.RemediationPlanDescription;
            RemediationPlanDataIE.isUserDefined = remediationPlanDataBE.isUserDefined;
            //List<IE.ActionDetails> actionList = new List<IE.ActionDetails>();
            //foreach (var action in remediationPlanDataBE.ActionDetails)
            //{
            //    IE.ActionDetails actiondetails = new IE.ActionDetails();
            //    actiondetails.ActionId = action.ActionId;
            //    actiondetails.ActionName = action.ActionName;
            //    actiondetails.ActionSequence = action.ActionSequence;
            //    actiondetails.ActionStageId = action.ActionStageId;
            //    actionList.Add(actiondetails);
            //}
            //RemediationPlanDataIE.ActionDetails = actionList;
            return RemediationPlanDataIE;
        }

        public static List<IE.NotificationConfigurationDetails> NotificationConfigurationDetails_BEtoIE(List<BE2.NotificationConfigurationDetails> notificationConfigurationBE)
        {
            List<IE.NotificationConfigurationDetails> notificationConfigurationListIE = new List<IE.NotificationConfigurationDetails>();
            foreach(BE2.NotificationConfigurationDetails notificationConfigObject in notificationConfigurationBE)
            {
                IE.NotificationConfigurationDetails notificationConfigObjectIE = new IE.NotificationConfigurationDetails();
                notificationConfigObjectIE.ReferenceKey = notificationConfigObject.ReferenceKey;
                notificationConfigObjectIE.ReferenceType = notificationConfigObject.ReferenceType;
                notificationConfigObjectIE.ReferenceValue = notificationConfigObject.ReferenceValue;
                notificationConfigurationListIE.Add(notificationConfigObjectIE);
            }
            
            return notificationConfigurationListIE;
        }

        public static List<IE.RecipientConfigurationDetails> RecipientConfigurationDetails_BEtoIE(List<BE2.RecipientConfigurationDetails> recipientConfigurationBE)
        {
            List<IE.RecipientConfigurationDetails> recipientConfigurationListIE = new List<IE.RecipientConfigurationDetails>();
            foreach (BE2.RecipientConfigurationDetails recipientConfigObject in recipientConfigurationBE)
            {
                IE.RecipientConfigurationDetails recipientConfigObjectIE = new IE.RecipientConfigurationDetails();
                recipientConfigObjectIE.ReferenceKey = recipientConfigObject.ReferenceKey;
                recipientConfigObjectIE.RecipientName = recipientConfigObject.RecipientName;
                recipientConfigObjectIE.ReferenceType = recipientConfigObject.ReferenceType;
                recipientConfigObjectIE.ReferenceValue = recipientConfigObject.ReferenceValue;
                recipientConfigObjectIE.isActive = recipientConfigObject.isActive;
                recipientConfigurationListIE.Add(recipientConfigObjectIE);
            }

            return recipientConfigurationListIE;
        }

        public static IE.ResourceDetails ResourceDetails_BEtoIE(BE2.ResourceDetails resourceDetailsBE)
        {
            IE.ResourceDetails resourceDetailsIE = new IE.ResourceDetails();
            resourceDetailsIE.ResourceId = resourceDetailsBE.ResourceId;
            resourceDetailsIE.ResourceName = resourceDetailsBE.ResourceName;
            resourceDetailsIE.ResourceTypeId = resourceDetailsBE.ResourceTypeId;
            resourceDetailsIE.TenantId = resourceDetailsBE.TenantId;
            resourceDetailsIE.TenantName = resourceDetailsBE.TenantName;
            resourceDetailsIE.PlatformId = resourceDetailsBE.PlatformId;
            resourceDetailsIE.PlatformName = resourceDetailsBE.PlatformName;
            resourceDetailsIE.ResourceTypeName = resourceDetailsBE.ResourceTypeName;
            resourceDetailsIE.HostName = resourceDetailsBE.HostName;
            return resourceDetailsIE;
        }

        public static IE.NotificationConfigurationDetails AnomalyReason_BEtoIE(BE2.NotificationConfigurationDetails notificationConfigurationBE)
        {            
            IE.NotificationConfigurationDetails notificationConfigObjectIE = new IE.NotificationConfigurationDetails();
            notificationConfigObjectIE.ReferenceKey = notificationConfigurationBE.ReferenceKey;
            notificationConfigObjectIE.ReferenceType = notificationConfigurationBE.ReferenceType;
            notificationConfigObjectIE.ReferenceValue = notificationConfigurationBE.ReferenceValue;     
            
            return notificationConfigObjectIE;
        }

        public static IE.EnvironmentScanReportDetails EnvironmentScanAnomalyDetails_BEtoIE(BE2.EnvironmentScanReportDetails environmentScanReportDetailsBE)
        {
            IE.EnvironmentScanReportDetails environmentScanReportDetailsIE = new IE.EnvironmentScanReportDetails();
            List<IE.EnvironmentScanAnomalyDetails> environmentScanAnomalyDetailsIE = new List<IE.EnvironmentScanAnomalyDetails>();

            foreach (var obj in environmentScanReportDetailsBE.EnvironmentScanAnomalyDetails)
            {
                IE.EnvironmentScanAnomalyDetails environmentScanAnomalyDetailsIEObj = new IE.EnvironmentScanAnomalyDetails();

                environmentScanAnomalyDetailsIEObj.MetricName = obj.MetricName;
                environmentScanAnomalyDetailsIEObj.MetricId = obj.MetricId;
                environmentScanAnomalyDetailsIEObj.MetricKey = obj.MetricKey;
                environmentScanAnomalyDetailsIEObj.AttributeName = obj.AttributeName;
                environmentScanAnomalyDetailsIEObj.AttributeValueNew = obj.AttributeValueNew;
                environmentScanAnomalyDetailsIEObj.AttributeValueOld = obj.AttributeValueOld;
                environmentScanAnomalyDetailsIEObj.AttributeStatus = obj.AttributeStatus;

                environmentScanAnomalyDetailsIE.Add(environmentScanAnomalyDetailsIEObj);
            }
            environmentScanReportDetailsIE.ResourceName = environmentScanReportDetailsBE.ResourceName;
            environmentScanReportDetailsIE.ResourceTypeName = environmentScanReportDetailsBE.ResourceTypeName;
            environmentScanReportDetailsIE.OldReportDate = environmentScanReportDetailsBE.OldReportDate;
            environmentScanReportDetailsIE.NewReportDate = environmentScanReportDetailsBE.NewReportDate;
            environmentScanReportDetailsIE.PlatformName = environmentScanReportDetailsBE.PlatformName;
            environmentScanReportDetailsIE.TenantName = environmentScanReportDetailsBE.TenantName;
            environmentScanReportDetailsIE.EnvironmentScanAnomalyDetails = environmentScanAnomalyDetailsIE;

            return environmentScanReportDetailsIE;
        }

        public static IE.finalReport EnvironmentScanComparisonPlatformReport_BEtoIE(BE2.finalReport environmentScanComparisonReportBE)
        {
           // IE.finalReport environmentScanComparisonPlatformReportIE = new IE.finalReport();
            IE.finalReport finalReport = new IE.finalReport();
            IE.EnvironmentScanComparisonReport2 environmentScanComparisonReport2 = new IE.EnvironmentScanComparisonReport2();
            IE.OSDetails osd = new IE.OSDetails();
            IE.ScreenResoution screenResolution = new IE.ScreenResoution();
            List<IE.SRDetails> SRDetailsListIE = new List<IE.SRDetails>();
            List<IE.OSResoutions> OSResolutionsIE = new List<IE.OSResoutions>();
            List<IE.Details> softwareDetailsIE = new List<IE.Details>();


            IE.EnvironmentScanComparisonReport2 softwareDetails = new IE.EnvironmentScanComparisonReport2();
            List<IE.Details> softwareDetailsList = new List<IE.Details>();
            softwareDetails.metricName = environmentScanComparisonReportBE.softwareReport.metricName;

            foreach (var obj in environmentScanComparisonReportBE.softwareReport.details)
            {
                IE.Details softdet = new IE.Details();
                softdet.newinstalleddate = obj.newinstalleddate;
                softdet.newpublisher = obj.newpublisher;
                softdet.newversion = obj.newversion;
                softdet.oldinstalleddate = obj.oldinstalleddate;
                softdet.oldpublisher = obj.oldpublisher;
                softdet.oldversion = obj.oldversion;
                softdet.resource = obj.resource;
                softdet.resourcetype = obj.resourcetype;
                softdet.SoftwareName = obj.SoftwareName;
                softdet.status = obj.status;
                softwareDetailsIE.Add(softdet);
            }
            softwareDetails.details = softwareDetailsIE; /////

            IE.OSDetails OSDetailsIE = new IE.OSDetails();
            OSDetailsIE.metricName = environmentScanComparisonReportBE.osDetailsReport.metricName;
            foreach (var obj2 in environmentScanComparisonReportBE.osDetailsReport.details)
            {
                IE.OSResoutions osr = new IE.OSResoutions();
                osr.name = obj2.name;
                osr.newbuildnumber = obj2.newbuildnumber;
                osr.newVersion = obj2.newVersion;
                osr.oldbuildnumber = obj2.oldbuildnumber;
                osr.oldVersion = obj2.oldVersion;
                osr.resource = obj2.resource;
                osr.resourcetype = obj2.resourcetype;
                osr.status = osr.status;
                OSResolutionsIE.Add(osr);

            }
            OSDetailsIE.details = OSResolutionsIE;
            IE.ScreenResoution screenResolutionIE = new IE.ScreenResoution();
            screenResolutionIE.metricName = environmentScanComparisonReportBE.srDetails.metricName;

            foreach (var obj3 in environmentScanComparisonReportBE.srDetails.details)
            {
                IE.SRDetails srd = new IE.SRDetails();
                srd.newversion = obj3.newversion;
                srd.oldversion = obj3.oldversion;
                srd.resource = obj3.resource;
                srd.resourceyype = obj3.resourceyype;
                srd.status = obj3.status;
                SRDetailsListIE.Add(srd);

            }
            screenResolutionIE.details = SRDetailsListIE;

            finalReport.softwareReport = softwareDetails;
            finalReport.osDetailsReport = OSDetailsIE;
            finalReport.srDetails = screenResolutionIE;
            //environmentScanComparisonPlatformReportIE.metricName = "PlatformReport";
          //  environmentScanComparisonPlatformReportIE.metricValue = finalReport;
            return finalReport;
        }
        public static IE.EnvironmentScanComparisonReport EnvironmentScanComparisonReport_BEtoIE(BE2.EnvironmentScanComparisonReport environmentScanComparisonReportBE)
        {
            IE.EnvironmentScanComparisonReport environmentScanComparisonReportIE = new IE.EnvironmentScanComparisonReport();
            IE.ESResourceDetails resourceDetailsIE = new IE.ESResourceDetails();
            IE.ESMetricDetails metricDetailsIE = new IE.ESMetricDetails();
            List<IE.ESMetricValues> metricValuesListIE = new List<IE.ESMetricValues>();
            

            foreach (var metricValObj in environmentScanComparisonReportBE.ResourceDetails.MetricDetails.MetricValue)
            {
                IE.ESMetricValues metricValueObjIE = new IE.ESMetricValues();
                List<IE.ESAttributes> attributeListIE = new List<IE.ESAttributes>();
                metricValueObjIE.MetricId = metricValObj.MetricId;
                metricValueObjIE.MetricKey = metricValObj.MetricKey;
                metricValueObjIE.MetricName = metricValObj.MetricName;
                metricValueObjIE.Status = metricValObj.Status;

                

                foreach (var attrObj in metricValObj.Attributes)
                {
                    IE.ESAttributes attributesObjIE = new IE.ESAttributes();
                    attributesObjIE.AttributeName = attrObj.AttributeName;
                    attributesObjIE.AttributeValue = attrObj.AttributeValue;
                    attributesObjIE.DisplayName = attrObj.DisplayName;

                    attributeListIE.Add(attributesObjIE);
                }
                metricValueObjIE.Attributes = attributeListIE;
                metricValuesListIE.Add(metricValueObjIE);

            }
            metricDetailsIE.ObservableId = environmentScanComparisonReportBE.ResourceDetails.MetricDetails.ObservableId;
            metricDetailsIE.ObservableName = environmentScanComparisonReportBE.ResourceDetails.MetricDetails.ObservableName;
            metricDetailsIE.MetricValue = metricValuesListIE;

            resourceDetailsIE.ResourceId = environmentScanComparisonReportBE.ResourceDetails.ResourceId;
            resourceDetailsIE.ResourceName = environmentScanComparisonReportBE.ResourceDetails.ResourceName;
            resourceDetailsIE.ResourceTypeId = environmentScanComparisonReportBE.ResourceDetails.ResourceTypeId;
            resourceDetailsIE.ResourceTypeName = environmentScanComparisonReportBE.ResourceDetails.ResourceTypeName;
            resourceDetailsIE.MetricDetails = metricDetailsIE;

            environmentScanComparisonReportIE.TenantId = environmentScanComparisonReportBE.TenantId;
            environmentScanComparisonReportIE.TenantName = environmentScanComparisonReportBE.TenantName;
            environmentScanComparisonReportIE.PlatformId = environmentScanComparisonReportBE.PlatformId;
            environmentScanComparisonReportIE.PlatformName = environmentScanComparisonReportBE.PlatformName;
            environmentScanComparisonReportIE.StartDate = environmentScanComparisonReportBE.StartDate;
            environmentScanComparisonReportIE.EndDate = environmentScanComparisonReportBE.EndDate;
            environmentScanComparisonReportIE.ResourceDetails = resourceDetailsIE;
            return environmentScanComparisonReportIE;
        }
            //public static List<IE.EnvironmentScanAnomalyDetails> EnvironmentScanAnomalyDetails_BEtoIE(List<BE2.EnvironmentScanAnomalyDetails> environmentScanAnomalyDetailsBE)
            //{
            //    if (environmentScanAnomalyDetailsBE!=null)
            //    {
            //        IE.EnvironmentScanAnomalyDetails environmentScanAnomalyDetailsIE = new IE.EnvironmentScanAnomalyDetails();

            //        try
            //        {
            //            if (environmentScanAnomalyDetailsBE.InstalledSoftware.Count>0)
            //            {
            //                List<IE.ESInstalledSoftware> installedSoftwareListIE = new List<IE.ESInstalledSoftware>();

            //                foreach (var softwareObj in environmentScanAnomalyDetailsBE.InstalledSoftware)
            //                {
            //                    IE.ESInstalledSoftware installedSoftwareIE = new IE.ESInstalledSoftware();

            //                    installedSoftwareIE.SoftwareNameNew = softwareObj.SoftwareNameNew;
            //                    installedSoftwareIE.PublisherNew = softwareObj.PublisherNew;
            //                    installedSoftwareIE.InstalledDateNew = softwareObj.InstalledDateNew;
            //                    installedSoftwareIE.SoftwareVersionNew = softwareObj.SoftwareVersionNew;
            //                    installedSoftwareIE.Status = softwareObj.Status;
            //                    installedSoftwareIE.SoftwareNameOld = softwareObj.SoftwareNameOld;
            //                    installedSoftwareIE.PublisherOld = softwareObj.PublisherOld;
            //                    installedSoftwareIE.InstalledDateOld = softwareObj.InstalledDateOld;
            //                    installedSoftwareIE.SoftwareVersionOld = softwareObj.SoftwareVersionOld;

            //                    installedSoftwareListIE.Add(installedSoftwareIE);
            //                }
            //                environmentScanAnomalyDetailsIE.InstalledSoftware = installedSoftwareListIE;
            //            }
            //            if (environmentScanAnomalyDetailsBE.OSDetails!=null)
            //            {
            //                IE.ESOSDetails osDetailsIE = new IE.ESOSDetails();

            //                osDetailsIE.OSNameNew = environmentScanAnomalyDetailsBE.OSDetails.OSNameNew;
            //                osDetailsIE.OSNameOld = environmentScanAnomalyDetailsBE.OSDetails.OSNameOld;
            //                osDetailsIE.VersionNew = environmentScanAnomalyDetailsBE.OSDetails.VersionNew;
            //                osDetailsIE.VersionOld = environmentScanAnomalyDetailsBE.OSDetails.VersionOld;
            //                osDetailsIE.BuildNumberNew = environmentScanAnomalyDetailsBE.OSDetails.BuildNumberNew;
            //                osDetailsIE.BuildNumberOld = environmentScanAnomalyDetailsBE.OSDetails.BuildNumberOld;
            //                osDetailsIE.SystemTypeNew = environmentScanAnomalyDetailsBE.OSDetails.SystemTypeNew;
            //                osDetailsIE.SystemTypeOld = environmentScanAnomalyDetailsBE.OSDetails.SystemTypeOld;
            //                osDetailsIE.Status = environmentScanAnomalyDetailsBE.OSDetails.Status;

            //                environmentScanAnomalyDetailsIE.OSDetails = osDetailsIE;
            //            }
            //            if (environmentScanAnomalyDetailsBE.ScreenResolution != null)
            //            {
            //                IE.ESScreenResolution screenResolutionIE = new IE.ESScreenResolution();

            //                screenResolutionIE.HeightNew = environmentScanAnomalyDetailsBE.ScreenResolution.HeightNew;
            //                screenResolutionIE.HeightOld = environmentScanAnomalyDetailsBE.ScreenResolution.HeightOld;
            //                screenResolutionIE.WidthNew = environmentScanAnomalyDetailsBE.ScreenResolution.WidthNew;
            //                screenResolutionIE.WidthOld = environmentScanAnomalyDetailsBE.ScreenResolution.WidthOld;
            //                screenResolutionIE.Status = environmentScanAnomalyDetailsBE.ScreenResolution.Status;

            //                environmentScanAnomalyDetailsIE.ScreenResolution = screenResolutionIE;
            //            }

            //            return environmentScanAnomalyDetailsIE;
            //        }
            //        catch(Exception ex)
            //        {
            //            throw ex;
            //        }
            //    }
            //    return null;           


            //}
        }
}