/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.Solutions.Ainauto.Infrastructure.ProcessScheduler.Framework;
using Infosys.Solutions.Superbot.Infrastructure.Common;
using Infosys.Solutions.Ainauto.Superbot.BusinessComponent;
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using UserProfile;
//using ApplicationCore;
using QueueNotificationEntity = Infosys.Solutions.Superbot.Resource.Entity.Queue;
using CommonUtils = Infosys.Solutions.Superbot.Infrastructure.Common.Utility;
////using Infosys.PPTWare.Infrastructure.ApplicationCore;
using System.Text.RegularExpressions;
using System.Xml;
using System.Net;
using System.Net.Mail;
using BE = Infosys.Solutions.Ainauto.Superbot.BusinessEntity;
using System.Collections.Specialized;
using System.IO;

namespace Infosys.Solutions.Ainauto.Processes
{
    public class Notification : ProcessHandlerBase<QueueNotificationEntity.Notification>
    {
        public override void Dump(QueueNotificationEntity.Notification message)
        {

        }

        //private int index = 16;

        public override bool Process(QueueNotificationEntity.Notification message, int a, int b, int c)
        {
            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_Start, "Process", "Notification"), LogHandler.Layer.Business, null);
            LogHandler.LogDebug(String.Format("The Process Method of Notification class is getting executed with parameters : Metric message={0}; robotId={1};runInstanceId={2}; robotTaskMapId={3}", message, robotId, runInstanceId, robotTaskMapId),
                LogHandler.Layer.Business, null);
            using (LogHandler.TraceOperations("Notification:Process", LogHandler.Layer.Business, Guid.NewGuid(), null))
            {
                //LogHandler.LogBusinessFunction("Send notifications to {0} from {1} of messagetype {2} . (presentationurl:{3})",
                //    LogHandler.BusinessFunctionConstants.NOTIFICATIONS,
                //    message.Audience, message.From, message.Type, message.PresentationUrl);
                bool isSuccess = true;
                string templateFileName = string.Empty;
                string subject = string.Empty;
                //creating the object for both scenarios
                BE.Notification notificationObj = new BE.Notification();
                BE.EnvironmentScanReportDetails environmentScanReportDetails = new BE.EnvironmentScanReportDetails();
                List<BE.EnvironmentScanAnomalyDetails> environmentScanAnomalyDetailsList = new List<BE.EnvironmentScanAnomalyDetails>();

                // form a dictionary of elements whose text is to be replaced
                var elementDict = new Dictionary<string, string>();

                NotificationBuilder notificationBuilder = new NotificationBuilder();

                var attachments = new List<Attachment>();
                string filePath = AppDomain.CurrentDomain.BaseDirectory;

                if (message.Type == (int)NotificationType.EnvironmentScan)
                {
                    LogHandler.LogDebug("The notification is of type Environment scan, Calling the GetEnvironmentScanAnomalyDetails method of notification builder class",
                              LogHandler.Layer.Business, null);
                    //calling the notification builder for environment scan
                    //environmentScanAnomalyDetailsList = notificationBuilder.GetEnvironmentScanAnomalyDetails(message.ResourceId,message.ObservationId,message.PlatformId,message.TenantId);
                    environmentScanReportDetails = notificationBuilder.BuildEnvironmentScanNotification(message);

                    subject = string.Format(InfoMessages.EMAIL_SUBJECT_ENVIRONMENT_SCAN, environmentScanReportDetails.ResourceName, environmentScanReportDetails.PlatformName, environmentScanReportDetails.TenantName);

                    environmentScanAnomalyDetailsList = environmentScanReportDetails.EnvironmentScanAnomalyDetails;
                    LogHandler.LogDebug("populating the dictionary to replace the html elements with the notification details",
                              LogHandler.Layer.Business, null);
                    string finalString = string.Empty;                   
                    string installedSoftwareString = "";
                    string osDetailsString = "";
                    string screenResoulutionString = "";

                    var groupedAnomalyDetails = environmentScanAnomalyDetailsList.GroupBy(e => new { e.MetricName, e.MetricKey });
                    
                    foreach (var group in groupedAnomalyDetails)
                    {
                        switch (group.Key.MetricName.ToLower().RemoveSpaces())
                        {
                            case "installedsoftware":
                                string softwareName = string.Empty;
                                string versionOld = string.Empty;
                                string versionNew = string.Empty;
                                string installDateOld = string.Empty;
                                string installDateNew = string.Empty;
                                string publisherOld = string.Empty;
                                string publisherNew = string.Empty;
                                string status = string.Empty;                             
                                    

                                foreach (var obj in group)
                                {
                                    switch (obj.AttributeName.ToLower().RemoveSpaces())
                                    {
                                        case "displayname":
                                            softwareName = obj.AttributeValueNew == "" ? obj.AttributeValueOld : obj.AttributeValueNew;
                                            break;
                                        case "version":
                                            versionNew = obj.AttributeValueNew;
                                            versionOld = obj.AttributeValueOld;
                                            break;
                                        case "installdate":
                                            installDateNew = obj.AttributeValueNew;
                                            installDateOld = obj.AttributeValueOld;
                                            break;
                                        case "publisher":
                                            publisherNew = obj.AttributeValueNew;
                                            publisherOld = obj.AttributeValueOld;
                                            break;
                                        default:
                                            break;
                                    }
                                    status = obj.AttributeStatus;
                                }
                                installedSoftwareString += String.Format(InfoMessages.Installed_Software_Row, softwareName, versionOld, installDateOld, publisherOld, versionNew, installDateNew, publisherNew, status);
                                                                
                                break;
                            case "osdetails":
                                string osName = string.Empty;
                                string osVersionOld = string.Empty;
                                string osVersionNew = string.Empty;
                                string buildNumberOld = string.Empty;
                                string buildNumberNew = string.Empty;
                                string systemTypeOld = string.Empty;
                                string systemTypeNew = string.Empty;
                                string osStatus = string.Empty;

                                foreach (var obj in group)
                                {
                                    switch (obj.AttributeName.ToLower().RemoveSpaces())
                                    {
                                        case "caption":
                                            osName = obj.AttributeValueNew;
                                            break;
                                        case "osarchitecture":
                                            systemTypeNew = obj.AttributeValueNew;
                                            systemTypeOld = obj.AttributeValueOld;
                                            break;
                                        case "version":
                                            osVersionNew = obj.AttributeValueNew;
                                            osVersionOld = obj.AttributeValueOld;
                                            break;
                                        case "buildnumber":
                                            buildNumberNew = obj.AttributeValueNew;
                                            buildNumberOld = obj.AttributeValueOld;
                                            break;
                                        default:
                                            break;
                                    }
                                    osStatus = obj.AttributeStatus;
                                }
                                osDetailsString += String.Format(InfoMessages.OSDetails_Row, osName, systemTypeOld, osVersionOld, buildNumberOld, systemTypeNew,  osVersionNew,  buildNumberNew, osStatus);

                                break;
                            case "screenresolution":
                                string oldHeight = string.Empty;
                                string newHeight = string.Empty;
                                string oldWidth = string.Empty;
                                string newWidth = string.Empty;
                                string srStatus = string.Empty;                               

                                foreach (var obj in group)
                                {
                                    switch (obj.AttributeName.ToLower().RemoveSpaces())
                                    {
                                        case "pelsheight":
                                            oldHeight = obj.AttributeValueOld;
                                            newHeight = obj.AttributeValueNew;
                                            break;
                                        case "pelswidth":
                                            oldWidth = obj.AttributeValueOld;
                                            newWidth = obj.AttributeValueNew;
                                            break;
                                        default:
                                            break;
                                    }
                                    srStatus = obj.AttributeStatus;
                                }
                                screenResoulutionString += String.Format(InfoMessages.SR_Row,oldHeight+"*"+oldWidth ,newHeight+"*"+newWidth,srStatus );

                                break;
                            default:
                                break;
                        }
                    }
                    if (installedSoftwareString!="")
                    {
                        finalString = String.Format(InfoMessages.Installed_Software_Body,installedSoftwareString);
                    }
                    if (osDetailsString != "")
                    {
                        if (finalString == string.Empty)
                        {
                            finalString = String.Format(InfoMessages.OSDetails_Body,osDetailsString);
                        }
                        else
                        {
                            finalString += String.Format(InfoMessages.OSDetails_Body, osDetailsString);
                        }
                    }
                    if (screenResoulutionString!="")
                    {
                        if (finalString == string.Empty)
                        {
                            finalString = String.Format(InfoMessages.SR_Body, screenResoulutionString);
                        }
                        else
                        {
                            finalString += String.Format(InfoMessages.SR_Body, screenResoulutionString);
                        }
                    }
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.resourceName, environmentScanReportDetails.ResourceName);
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.resourceTypeName, environmentScanReportDetails.ResourceTypeName);
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.oldReportDate, environmentScanReportDetails.OldReportDate);
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.newReportDate, environmentScanReportDetails.NewReportDate);
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.anomalyTables,finalString);
                }
                else if (message.Type == (int)NotificationType.Summary)
                {
                    string row = string.Empty;
                    string finalString = string.Empty;                    
                    string filterCriteria = string.Empty;
                    string colorCode = ApplicationConstants.ColorCodes.Black;

                    //get platform name
                    string platformName = NotificationBuilder.GetPlatformName(message.PlatformId);

                    //get tenant name
                    string tenantName = NotificationBuilder.GetTenantName(message.TenantId);

                    subject = string.Format(InfoMessages.EMAIL_SUBJECT_SUMMARY,platformName, tenantName);

                    var summaryObjList = notificationBuilder.BuildSummaryNotification(message);

                    #region Constructing Filter Criteria



                    if (message.ConfigId != null && message.ConfigId != "")
                    { 
                        filterCriteria += string.Format("Configuration : {0}", notificationBuilder.GetCongfigName(message.ConfigId));
                    }
                    if (message.PortfolioId != null && message.PortfolioId != "")
                    {
                        filterCriteria += string.Format(" and Portfolio : {0}", notificationBuilder.GetResourceName(message.PortfolioId));
                    }

                    filterCriteria += string.Format(" in Platform : {0} @ {1}.", platformName, summaryObjList[0].ObservationTime.ToString());

                    #endregion

                    //to get recipient details 
                    message.ResourceId = summaryObjList[0].ResourceId;

                    foreach (var obj in summaryObjList)
                    {
                        switch (obj.Status.ToLower().Trim())
                        {
                            case "healthy":
                                colorCode = ApplicationConstants.ColorCodes.Healthy;
                                break;
                            case "critical":
                                colorCode = ApplicationConstants.ColorCodes.Critical;
                                break;
                            case "warning":
                                colorCode = ApplicationConstants.ColorCodes.Warning;
                                break;
                            default:
                                colorCode = ApplicationConstants.ColorCodes.Black;
                                break;
                        }
                        row += String.Format(InfoMessages.SummaryNotification_Row,obj.ResourceName, obj.ResourceTypeName, obj.ObservableName, obj.Value, colorCode, obj.Status);
                    }
                    finalString = String.Format(InfoMessages.SummaryNotiffication_Body, row);

                    elementDict.Add(ApplicationConstants.EmailPlaceholders.filterCriteria, filterCriteria);
                    
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.anomalyTables, finalString);

                }
                else if (message.Type == (int)NotificationType.EnvironmentScanConsolidated)
                {                    
                    List<string> pathList = notificationBuilder.GetConsolidatedEnvironmentScanReport(message.PlatformId, message.TenantId, out string platformName, out string tenantName);
                    
                    if (pathList!=null && pathList.Count>0)
                    {
                        pathList.ForEach(p => attachments.Add(new Attachment(p)));
                    }
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.PlatformName,
                        platformName);
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.TenantName,
                        tenantName);
                }
                else
                {
                    LogHandler.LogDebug("Calling the BuildNotification method of notification builder class",
                              LogHandler.Layer.Business, null);
                    notificationObj = notificationBuilder.BuildNotification(message);

                    LogHandler.LogDebug("populating the dictionary to replace the html elements with the notification details",
                                  LogHandler.Layer.Business, null);
                    //string resourceDetailsString = notificationObj.ResourceTypeName + " - " + notificationObj.ResourceName + " in " + notificationObj.HostName + " at " + notificationObj.ObservationTime;
                    string resourceDetailsString = notificationObj.ResourceTypeName + " - " + notificationObj.ResourceName;
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.resourceDetails,
                               resourceDetailsString);
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.FirstRemediationPlanName,
                        notificationObj.RemediationPlanName);
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.RemediationPlanName,
                        notificationObj.RemediationPlanName);
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.reason,
                        notificationObj.AnomalyReason);
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.TenantName,
                        notificationObj.TenantName);
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.PlatformName,
                        notificationObj.PlatformName);
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.HostName,
                        notificationObj.HostName);
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.ObservationTime,
                        notificationObj.ObservationTime);
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.FirstObservableName,
                        notificationObj.ObservableName);
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.ObservableName,
                        notificationObj.ObservableName);
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.ObservationStatus,
                        notificationObj.ObservationStatus);
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.ObservationId,
                        notificationObj.ObservationId);
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.RemediationStatus,
                        notificationObj.RemediationPlanStatus);
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.RemediationTime,
                        notificationObj.RemediationPlanTime);
                    elementDict.Add(ApplicationConstants.EmailPlaceholders.notificationDescription,
                        notificationObj.Description);

                    var description = message.Description.Split(new string[] { "Reference Details: " }, StringSplitOptions.None);

                    elementDict.Add(ApplicationConstants.EmailPlaceholders.remediationFailureMessage,
                        description[0]);

                    if(description.Length > 1)
                        elementDict.Add(ApplicationConstants.EmailPlaceholders.referenceLink, description[1]);
                }

                // select template as per notification type
                switch (message.Type)
                {
                    case (int)NotificationType.AnomalyDetected:
                        #region
                        LogHandler.LogDebug("Setting the constants for Notification type :{0}",
                              LogHandler.Layer.Business, NotificationType.AnomalyDetected);
                        
                        subject = string.Format(InfoMessages.EMAIL_SUBJECT_ANOMALY_DETECTED,notificationObj.ResourceName, notificationObj.PlatformName, notificationObj.TenantName);

                        templateFileName = ApplicationConstants.EmailTemplates.AnomalyDetected;
                        
                        break;
                    #endregion
                    case (int)NotificationType.RemediationFailure:
                        #region
                        LogHandler.LogDebug("Setting the constants for Notification type :{0}",
                              LogHandler.Layer.Business, NotificationType.RemediationFailure);
                        
                        subject = string.Format(InfoMessages.EMAIL_SUBJECT_REMEDIATION_FAILURE, notificationObj.ResourceName, notificationObj.PlatformName, notificationObj.TenantName);

                        //owner = userProfileProvider.GetUserProfileDetails(message.PresentationOwner, userDomain);

                        templateFileName = ApplicationConstants.EmailTemplates.RemediationFailed;
                        
                        break;
                    #endregion
                    case (int)NotificationType.RemediationSuccess:
                        #region
                        LogHandler.LogDebug("Setting the constants for Notification type :{0}",
                              LogHandler.Layer.Business, NotificationType.RemediationFailure);
                        
                        var attachment_content_List = notificationBuilder.GetAuditLogOutputAttachments(message.ObservationId);

                        if (attachment_content_List!=null && attachment_content_List.Count>0)
                        {
                            //string tempName = notificationObj.ObservableName + notificationObj.ResourceName + DateTime.UtcNow.ToString();
                            //with attachment template
                            filePath = filePath + string.Format(InfoMessages.Attachment_FileName,notificationObj.ObservableName.RemoveSpaces(),notificationObj.ResourceName.RemoveSpaces(),DateTime.UtcNow);
                            templateFileName = ApplicationConstants.EmailTemplates.RemediationSuccessWithAttachment;

                            subject = string.Format(InfoMessages.EMAIL_SUBJECT_REMEDIATION_SUCCESS_ATTACHMENT, notificationObj.ResourceName, notificationObj.PlatformName, notificationObj.TenantName);

                            List<string> lines = new List<string>();
                            int i = 1; 
                            foreach (string content in attachment_content_List)
                            {
                                //string line = content.Replace("\\r\\n", Environment.NewLine);
                                //lines.Add(String.Format("Detail {0}:",i++));                                
                                lines.Add(content + Environment.NewLine);
                                
                            }
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                            File.AppendAllLines(filePath, lines);
                            attachments.Add(new Attachment(filePath));
                        }
                        else
                        {
                            //without attachment template
                            templateFileName = ApplicationConstants.EmailTemplates.RemediationSuccess;
                            subject = string.Format(InfoMessages.EMAIL_SUBJECT_REMEDIATION_SUCCESS, notificationObj.ResourceName, notificationObj.PlatformName, notificationObj.TenantName);
                        }
                        break;
                    #endregion
                    case (int)NotificationType.ThresholdBreach:
                        #region
                        //LogHandler.LogDebug("Setting constants for {0}",
                        //    LogHandler.Layer.WorkerHost, NotificationType.CommentPresentation);
                        LogHandler.LogDebug("Setting the constants for Notification type :{0}",
                             LogHandler.Layer.Business, NotificationType.ThresholdBreach);
                        //targetAudience = GetUserDetails(message.PresentationOwner, userDomain, userProfileProvider);
                        subject = string.Format(InfoMessages.EMAIL_SUBJECT_THRESHOLD_BREACH, notificationObj.ResourceName, notificationObj.PlatformName, notificationObj.TenantName);

                        //owner = userProfileProvider.GetUserProfileDetails(message.PresentationOwner, userDomain);

                        templateFileName = ApplicationConstants.EmailTemplates.ThresholdBreach;
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.senderDisplayName,
                        //    sender.DisplayName);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.prefaceText,
                        //    InformationMessages.EMAIL_PREFACE_THRESHOLD_BREACH);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.resourceDetails,
                        //    resourceDetailsString);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.RemediationPlanName,
                        //    notificationObj.RemediationPlanName);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.reason,
                        //    notificationObj.AnomalyReason);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.TenantName,
                        //    notificationObj.TenantName);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.PlatformName,
                        //    notificationObj.PlatformName);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.FirstObservableName,
                        //    notificationObj.ObservableName);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.ObservableName,
                        //    notificationObj.ObservableName);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.ObservationStatus,
                        //    notificationObj.ObservationStatus);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.ObservationId,
                        //    notificationObj.ObservationId);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.RemediationStatus,
                        //    notificationObj.ObservationId);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.RemediationTime,
                        //    notificationObj.ObservationId);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.notificationDescription,
                        //    notificationObj.Description);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.presentationTitle,
                        //    message.PresentationTitle);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.userProfileURL,
                        //    owner.DisplayName);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.tagString,
                        //    message.PresentationTags);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.notificationDescription,
                        //    notificationDescription);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.personalizedMessage,
                        //    message.Message);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.personalizedMessageHeading,
                        //    "Comment: ");
                        break;
                    #endregion
                    case (int)NotificationType.NoRemediation:
                        #region
                        //LogHandler.LogDebug("Setting constants for {0}",
                        //    LogHandler.Layer.WorkerHost, NotificationType.CommentPresentation);
                        LogHandler.LogDebug("Setting the constants for Notification type :{0}",
                             LogHandler.Layer.Business, NotificationType.NoRemediation);
                        //targetAudience = GetUserDetails(message.PresentationOwner, userDomain, userProfileProvider);
                        subject = string.Format(InfoMessages.EMAIL_SUBJECT_NO_REMEDIATION, notificationObj.ResourceName, notificationObj.PlatformName, notificationObj.TenantName);

                        templateFileName = ApplicationConstants.EmailTemplates.NoRemediation;

                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.resourceDetails,
                        //    resourceDetailsString);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.RemediationPlanName,
                        //    notificationObj.RemediationPlanName);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.reason,
                        //    notificationObj.AnomalyReason);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.TenantName,
                        //    notificationObj.TenantName);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.PlatformName,
                        //    notificationObj.PlatformName);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.FirstObservableName,
                        //    notificationObj.ObservableName);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.ObservableName,
                        //    notificationObj.ObservableName);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.ObservationStatus,
                        //    notificationObj.ObservationStatus);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.ObservationId,
                        //    notificationObj.ObservationId);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.RemediationStatus,
                        //    notificationObj.ObservationId);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.RemediationTime,
                        //    notificationObj.ObservationId);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.notificationDescription,
                        //    notificationObj.Description);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.notificationDescription,
                        //    notificationDescription);
                        //owner = userProfileProvider.GetUserProfileDetails(message.PresentationOwner, userDomain);


                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.senderDisplayName,
                        //    sender.DisplayName);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.prefaceText,
                        //    InformationMessages.EMAIL_PREFACE_THRESHOLD_BREACH);

                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.presentationTitle,
                        //    message.PresentationTitle);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.userProfileURL,
                        //    owner.DisplayName);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.tagString,
                        //    message.PresentationTags);

                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.personalizedMessage,
                        //    message.Message);
                        //elementDict.Add(ApplicationConstants.EmailPlaceholders.personalizedMessageHeading,
                        //    "Comment: ");
                        break;
                    #endregion
                    case (int)NotificationType.EnvironmentScan:
                        #region
                        LogHandler.LogDebug("Setting the constants for Notification type :{0}",
                              LogHandler.Layer.Business, NotificationType.EnvironmentScan);
                        
                        //TO DO : create a subject in resource file for this type
                        
                        
                        templateFileName = ApplicationConstants.EmailTemplates.EnvironmentScan;
                        
                        break;
                        #endregion
                    case (int)NotificationType.Summary:
                        #region
                        templateFileName = ApplicationConstants.EmailTemplates.SummaryNotification;
                        break;
                    #endregion
                    case (int)NotificationType.EnvironmentScanConsolidated:
                        #region
                        //we need resource id to get recipient
                        //so we are assigning the platform id to resource Id
                        message.ResourceId = message.PlatformId.ToString();

                        subject = string.Format(InfoMessages.EMAIL_SUBJECT_ENVIRONMENT_SCAN_CONSOLIDATED, elementDict[ApplicationConstants.EmailPlaceholders.PlatformName], elementDict[ApplicationConstants.EmailPlaceholders.TenantName]);
                        templateFileName = ApplicationConstants.EmailTemplates.EnvironmentScanConsolidated;
                        break;
                    #endregion
                    default:
                        #region
                        //LogHandler.LogWarning("Notification handler not implemented for type {0}. Ignoring...",
                        //    LogHandler.Layer.WorkerHost, (NotificationType)message.Type);
                        return true;
                        #endregion
                }

                NotificationConfiguration notificationConfiguration = new NotificationConfiguration();

                LogHandler.LogDebug("calling the ReplaceTemplateParameters for template file name :{0}",
                              LogHandler.Layer.Business, templateFileName);
                // replace parameters in the template
                XmlDocumentHelper mailTemplate = ReplaceTemplateParameters(elementDict, templateFileName, message);
                
                switch (message.Channel)
                {
                    case (int)NotificationChannel.Email:
                        LogHandler.LogDebug("setting values for notification channel :{0}",
                              LogHandler.Layer.Business, NotificationChannel.Email);

                        string smtpServer = String.Empty;
                        int smtpPort=0;
                        string smtpId = String.Empty;
                        string smtpPwd = String.Empty;
                        string smtpDomain = String.Empty;
                        //LogHandler.LogDebug("Initalizing SMTP client", LogHandler.Layer.WorkerHost);
                        
                        // BE.NotificationConfigurationDetails notificationConfigurationDetails= notificationConfiguration.GetSMTPconfigIntermediate(message.PlatformId, message.TenantId, ApplicationConstants.ReferenceTypes.Email);
                        LogHandler.LogDebug("calling GetSMTPconfigIntermediate method of NotificationConfiguration class",
                              LogHandler.Layer.Business, NotificationChannel.Email);

                        List<BE.NotificationConfigurationDetails> smtpDetails = notificationConfiguration.GetSMTPconfigIntermediate(Convert.ToInt32(message.PlatformId),message.TenantId,ApplicationConstants.ReferenceTypes.Email);
                        if (smtpDetails == null)
                        {
                            LogHandler.LogError(String.Format(ErrorMessages.Method_Returned_Null, "GetSMTPconfigIntermediate", "NotificationConfiguration", "platform ID ={0} ; tenant Id ={1} ; referenceType :{2}"),
                                                    LogHandler.Layer.Business, message.PlatformId, message.TenantId, ApplicationConstants.ReferenceTypes.Email);
                            SuperbotDataItemNotFoundException exception = new SuperbotDataItemNotFoundException(String.Format(ErrorMessages.Method_Returned_Null, "GetSMTPconfigIntermediate", "NotificationConfiguration", "platform ID ={0} ; tenant Id ={1} ; referenceType :{2}"));
                            List<ValidationError> validationErrors_List = new List<ValidationError>();
                            ValidationError validationErr = new ValidationError();
                            validationErr.Code = "1045";
                            validationErr.Description = string.Format(ErrorMessages.Method_Returned_Null, "GetSMTPconfigIntermediate", "NotificationConfiguration", "platform ID ={0} ; tenant Id ={1} ; referenceType :{2}");
                            validationErrors_List.Add(validationErr);

                            if (validationErrors_List.Count > 0)
                            {
                                exception.Data.Add("DataNotFoundErrors", validationErrors_List);
                                throw exception;
                            }
                        }

                        foreach (var smtpObj in smtpDetails)
                        {
                            switch (smtpObj.ReferenceKey)
                            {
                                case ApplicationConstants.ReferenceKey.SMTP_SERVER:
                                    smtpServer = smtpObj.ReferenceValue;
                                    break;
                                case ApplicationConstants.ReferenceKey.SMTP_PORT:
                                    smtpPort = Convert.ToInt32(smtpObj.ReferenceValue);
                                    break;
                                case ApplicationConstants.ReferenceKey.SMTP_ID:
                                    smtpId = smtpObj.ReferenceValue;
                                    break;
                                case ApplicationConstants.ReferenceKey.SMTP_PASSWORD:
                                    smtpPwd = smtpObj.ReferenceValue;
                                    break;
                                case ApplicationConstants.ReferenceKey.SMTP_DOMAIN:
                                    smtpDomain = smtpObj.ReferenceValue;
                                    break;
                                default:
                                    break;

                            }
                        }

                        SmtpClient client = new SmtpClient(smtpServer, smtpPort);
                        //should use secure string for password
                        client.Credentials = new NetworkCredential(smtpId, smtpPwd);
                        
                        List<BE.RecipientConfigurationDetails> recipientDetails = notificationConfiguration.GetRecipientconfigIntermediate(message.ResourceId,message.TenantId,ApplicationConstants.ReferenceTypes.Email);
                        
                        if (recipientDetails!=null && recipientDetails.Count > 0)
                        {
                            foreach (var target in recipientDetails)
                            {
                                string targetName = target.RecipientName;
                                string targetEmailAddress = target.ReferenceValue;
                                var recipientDisplayName = mailTemplate[ApplicationConstants.EmailPlaceholders.recipientDisplayName];
                                recipientDisplayName.InnerText = targetName;
                                string body = mailTemplate.OuterXml;
                                using (MailMessage mailMsg = new MailMessage(smtpId, targetEmailAddress, subject, body))
                                {
                                    mailMsg.IsBodyHtml = true;

                                    if (attachments != null && attachments.Any())
                                    {
                                        attachments.ForEach(att => mailMsg.Attachments.Add(att));
                                    }
                                    LogHandler.LogInfo("Mail constructed", LogHandler.Layer.Business, null);
                                    try
                                    {
                                        client.Send(mailMsg);
                                        LogHandler.LogInfo("Mail sent for " + targetName, LogHandler.Layer.Business, null);
                                    }
                                    catch (Exception ex)
                                    {
                                        LogHandler.LogInfo("exception occured in process of notification", LogHandler.Layer.Business, null);
                                        //log the error
                                        //string errorMessage = string.Format(
                                        //    ErrorMessages.Share_Notification_Failed, targetEmailAddress, ex.Message);
                                        //LogHandler.LogError(errorMessage, LogHandler.Layer.WorkerHost);
                                        throw ex;
                                    }
                                }
                                    
                            }
                            //if (attachments != null && attachments.Any())
                            //{
                            //    var attList = attachments.Select(att => att.ContentStream).OfType<FileStream>().Select(fs => fs.Name);

                            //    foreach (var attachment in attList)
                            //    {
                            //        File.Delete(attachment);
                            //    }
                            //}
                        }
                        break;
                }
                LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_End, "Process", "Notification"), LogHandler.Layer.Business, null);
                return isSuccess;
            }
            
        }



        private XmlDocumentHelper ReplaceTemplateParameters(
            Dictionary<string, string> elementDict,
            string templateName,
            QueueNotificationEntity.Notification message)
        {
            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_Start, "ReplaceTemplate", "Notification"), LogHandler.Layer.Business, null);
            
            var mailTemplate = new XmlDocumentHelper(AppDomain.CurrentDomain.BaseDirectory + templateName);
            

            var xmlNsMgr = new XmlNamespaceManager(mailTemplate.NameTable);
            xmlNsMgr.AddNamespace("slidens", "http://www.w3.org/1999/xhtml");

            LogHandler.LogDebug("replacing the template elements",
                             LogHandler.Layer.Business, null);
            // replace elements' innertext as mentioned in dictionary
            //if (message.Type == (int)NotificationType.EnvironmentScan)
            //{
            foreach (var item in elementDict)
            {
                var element = mailTemplate[item.Key];
                if (element != null)
                {
                    if(item.Key == ApplicationConstants.EmailPlaceholders.anomalyTables || item.Key == ApplicationConstants.EmailPlaceholders.referenceLink)
                        element.InnerXml = item.Value;
                    else
                        element.InnerText = item.Value;
                }
                        
            }
            //}
            //else
            //{
            //    foreach (var item in elementDict)
            //    {
            //        var element = mailTemplate[item.Key];
            //        if (element != null)
            //            element.InnerText = item.Value;
            //    }
            //}
            
                
            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_End, "ReplaceParameter", "Notification"), LogHandler.Layer.Business, null);
            return mailTemplate;
        }

       
    }
}
