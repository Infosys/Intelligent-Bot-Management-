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

namespace Infosys.Solutions.Superbot.Infrastructure.Common
{
    public struct ApplicationConstants
    {

       /* public const string APP_NAME = "DIF";
        public const string DOCUMENTSTORE_KEY = "ImageFinderStore";
        public const string SERVICE_EXCEPTIONHANDLING_POLICY = "DIF.SERVICE";
        public const string ROBOT_CONFIGURATION_SERVICEINTERFACE = "/DocumentImageFinder/RobotConfiguration.svc";
        public const string CRAWLER_SERVICEINTERFACE = "/DocumentImageFinder/Crawler.svc";
        public const string IMAGE_RECOGNITION_SERVICEINTERFACE = "/DocumentImageFinder/ImageRecognition.svc";
        public const string SECURE_PASSCODE = "IAP2GO_SEC!URE";
        public const string JPEG_FILE_TYPE = "jpg";
        public const string XML_FILE_TYPE = "xml";
        public const string VALIDATION_ERROR_CODE = "1042";*/
        public const string WORKER_EXCEPTION_HANDLING_POLICY = "DocumentImageFinder.Worker";
        public const string SERVICE_EXCEPTIONHANDLING_POLICY = "Superbot.Service";

        public struct EmailTemplates
        {
            public const string AnomalyDetected = @"\Templates\Email\AnomalyDetected.html";
            public const string RemediationSuccess = @"\Templates\Email\RemediationSuccess.html";
            public const string RemediationSuccessWithAttachment = @"\Templates\Email\RemediationSuccessWithAttachment.html";
            public const string RemediationFailed = @"\Templates\Email\RemediationFailed.html";
            public const string ThresholdBreach = @"\Templates\Email\ThresholdBreach.html";
            public const string NoRemediation = @"\Templates\Email\ManualRemediation.html";
            public const string EnvironmentScan = @"\Templates\Email\EnvironmentScan.html";
            public const string SummaryNotification = @"\Templates\Email\SummaryNotification.html";
            public const string EnvironmentScanConsolidated = @"\Templates\Email\EnvironmentScanConsolidated.html";
        }
        public struct ReferenceTypes
        {
            public const string Email = "EMAIL";
            public const string ANOMALY_REASON = "ANOMALY_REASON";
            public const string SMS = "SMS";            
        }

        public struct ColorCodes
        {
            public const string Critical = "#FF5252";
            public const string Healthy = "#93FF00";
            public const string Warning = "#FF6000";
            public const string Black = "#333333";
        }

        public struct ReferenceKey
        {
            public const string SMTP_SERVER = "SMTP_SERVER";
            public const string SMTP_PORT = "SMTP_PORT";
            public const string SMTP_ID = "SMTP_ID";
            public const string SMTP_PASSWORD = "SMTP_PASSWORD";
            public const string SMTP_DOMAIN = "SMTP_DOMAIN";
            public const string RECIPIENT_NAME = "RECIPIENT_NAME";
            public const string RECIPIENT_EMAIL_ID = "RECIPIENT_EMAIL_ID";
        }

        public struct EmailPlaceholders
        {
            public const string recipientDisplayName = "recipientDisplayName";
            public const string resourceName = "resourceName";
            public const string resourceTypeName = "resourceTypeName";
            public const string resourceDetails = "resourceDetails";
            public const string FirstRemediationPlanName = "FirstRemediationPlanName";
            public const string RemediationPlanName = "RemediationPlanName";
            public const string reason = "reason";
            public const string TenantName = "TenantName";
            public const string PlatformName = "PlatformName";
            public const string FirstObservableName = "FirstObservableName";
            public const string ObservableName = "ObservableName";
            public const string ObservationStatus = "ObservationStatus";
            public const string ObservationId = "ObservationId";
            public const string RemediationStatus = "RemediationStatus";
            public const string RemediationTime = "RemediationTime";
            public const string HostName = "HostName";
            public const string ObservationTime = "ObservationTime";
            public const string notificationDescription = "notificationDescription";
            public const string remediationFailureMessage = "remediationFailureMessage";
            public const string anomalyTables = "anomalyTables";
            public const string oldReportDate = "oldReportDate";
            public const string newReportDate = "newReportDate";
            public const string filterCriteria = "filterCriteria";

            public const string senderDisplayName = "senderDisplayName";
            public const string prefaceText = "prefaceText";
            public const string heading = "heading";
            public const string presentationTitle = "presentationTitle";
            public const string userProfileURL = "userProfileURL";
            
            
            public const string personalizedMessage = "personalizedMessage";
            public const string title = "title";
            public const string personalizedMessageHeading = "personalizedMessageHeading";
            public const string sharedToList = "sharedToList";
            public const string groupTitle = "groupTitle";
            public const string groupDescription = "groupDescription";

            public const string referenceLink = "referenceLink";
        }

        public  struct SecureKeys
        {
            public static readonly string IAP2 = System.Configuration.ConfigurationManager.AppSettings["SecureKey"]?? "IAP2GO_SEC!URE";
        }
    }

    public enum NotificationType
    {
        None = 0,
        AnomalyDetected,
        RemediationFailure,
        ThresholdBreach,
        NoRemediation,
        EnvironmentScan,
        RemediationSuccess,
        Summary,
        EnvironmentScanConsolidated
    }

    public enum NotificationChannel
    {
        Email = 1,
        SMS = 2,
        Push = 4,
    }

    public static class ExtentionMethodsClass
    {
        public static string RemoveSpaces(this string str)
        {
            //StringBuilder newStr = new StringBuilder();
            //foreach (char c in str)
            //{
            //    if (!c.Equals(' '))
            //        newStr.Append(c);
            //}
            //return newStr.ToString();
            return str.Replace(" ", "");
           
            
        }

        public static double ConvertToDouble(this string str)
        {
            double d;
            if (Double.TryParse(str, out d))
                return d;
            else
                return 0;
        }
        public static DateTime ConvertToDate(this string str)
        {
            DateTime d;
            if (DateTime.TryParse(str,out d))
                return d;
            else
                return new DateTime();
        }
    }

}

