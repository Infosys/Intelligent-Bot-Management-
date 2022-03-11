/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.Solutions.Ainauto.Services.Superbot.Contracts;
using IE=Infosys.Solutions.Ainauto.Services.Superbot.Contracts.Data;
using Infosys.Solutions.Ainauto.Superbot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Infosys.Solutions.Ainauto.Superbot.BusinessComponent;
using Infosys.Solutions.Superbot.Infrastructure.Common;

namespace Infosys.Solutions.Ainauto.Superbot.Controllers
{
    public class ResourceHandlerController : ApiController, IResourceHandler
    {        
        public IE.PlatformInstanceDetails GetAllBotPlatformInstanceDependencies(string PlatformInstanceId, string TenantId, string dependencyResourceID)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                Monitor monitor = new Monitor();
                LogHandler.LogDebug(string.Format("Calling GetAllBotDependencies method and passing input arguments platformInstanceId:{0} & TenantId:{1} to get all platformdependencies", PlatformInstanceId, TenantId), LogHandler.Layer.WebServiceHost, null);
                IE.PlatformInstanceDetails res = Translator.ResourceHandler_BE_IE.PlatfromInstance_BEtoIE(monitor.GetAllBotDependencies(PlatformInstanceId, Convert.ToInt32(TenantId), dependencyResourceID));
                LogHandler.LogDebug(string.Format("Tranlated the response of GetAllBotDependencies() from BE to IE"), LogHandler.Layer.WebServiceHost, null);
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.Business, null);
                return res;
            }
            catch (Exception ex)
            {
                Console.Write("Exception Occured in GetAllBotPlatformInstanceDependencies " + ex.Message);
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.WebServiceHost, null);
                return null;
            }           

        }

        [Route("GetObservationsDetails/{observationId=observationId}/{platformId=platformId}/{tenantId=tenantId}")]
        public IE.ObservationDetails GetObservationsDetails(int observationId, int platformId, int tenantId)
        {
            //LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                NotificationBuilder notificationBuilder = new NotificationBuilder();
                // LogHandler.LogDebug(string.Format("Calling GetAllBotDependencies method and passing input arguments platformInstanceId:{0} & TenantId:{1} to get all platformdependencies", PlatformInstanceId, TenantId), LogHandler.Layer.WebServiceHost, null);
                IE.ObservationDetails res = Translator.ResourceHandler_BE_IE.NotificationObservationData_BEtoIE(notificationBuilder.GetObservationsDetails(observationId, platformId, tenantId));
                // LogHandler.LogDebug(string.Format("Tranlated the response of GetAllBotDependencies() from BE to IE"), LogHandler.Layer.WebServiceHost, null);
                // LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.Business, null);
                return res;
            }
            catch (Exception ex)
            {
                Console.Write("Exception Occured in GetNotificationTransactionDetails " + ex.Message);
                //LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.WebServiceHost, null);
                return null;
            }

        }

        [Route("GetRemediationPlanDetails/{remediationPlanId=remediationPlanId}/{tenantId=tenantId}")]
        public IE.RemediationPlanDetails GetRemediationPlanDetails(int remediationPlanId, int tenantId)
        {
            //LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                NotificationBuilder notificationBuilder = new NotificationBuilder();
                // LogHandler.LogDebug(string.Format("Calling GetAllBotDependencies method and passing input arguments platformInstanceId:{0} & TenantId:{1} to get all platformdependencies", PlatformInstanceId, TenantId), LogHandler.Layer.WebServiceHost, null);
                //IE.RemediationPlanDetails res = Translator.ResourceHandler_BE_IE.NotificationRemediationPlanData_BEtoIE(notificationBuilder.GetRemediationPlanDetails(remediationPlanId, tenantId));
                IE.RemediationPlanDetails res =Translator.ResourceHandler_BE_IE.NotificationRemediationPlanData_BEtoIE(notificationBuilder.GetRemediationPlanDetails(remediationPlanId, tenantId));
                // LogHandler.LogDebug(string.Format("Tranlated the response of GetAllBotDependencies() from BE to IE"), LogHandler.Layer.WebServiceHost, null);
                // LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.Business, null);
                return res;
            }
            catch (Exception ex)
            {
                Console.Write("Exception Occured in GetNotificationTransactionDetails " + ex.Message);
                //LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.WebServiceHost, null);
                return null;
            }

        }

        [Route("GetSMTPConfigurationDetails/{platformId=platformId}/{tenantId=tenantId}/{referenceType=referenceType}")]
        public List<IE.NotificationConfigurationDetails> GetSMTPConfigurationDetails(int platformId, int tenantId,string referenceType)
        {
            //LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                NotificationConfiguration notificationConfig = new NotificationConfiguration();
                // LogHandler.LogDebug(string.Format("Calling GetAllBotDependencies method and passing input arguments platformInstanceId:{0} & TenantId:{1} to get all platformdependencies", PlatformInstanceId, TenantId), LogHandler.Layer.WebServiceHost, null);
                List<IE.NotificationConfigurationDetails> res = Translator.ResourceHandler_BE_IE.NotificationConfigurationDetails_BEtoIE(notificationConfig.GetSMTPConfigurationDetails(platformId, tenantId, referenceType));
                // LogHandler.LogDebug(string.Format("Tranlated the response of GetAllBotDependencies() from BE to IE"), LogHandler.Layer.WebServiceHost, null);
                // LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.Business, null);
                return res;
            }
            catch (Exception ex)
            {
                Console.Write("Exception Occured in GetNotificationTransactionDetails " + ex.Message);
                //LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.WebServiceHost, null);
                return null;
            }

        }

        [Route("GetRecipientConfigurationDetails/{resourceId=resourceId}/{tenantId=tenantId}/{referenceType=referenceType}")]
        public List<IE.RecipientConfigurationDetails> GetRecipientConfigurationDetails(string resourceId, int tenantId, string referenceType)
        {
            //LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                NotificationConfiguration notificationConfig = new NotificationConfiguration();
                // LogHandler.LogDebug(string.Format("Calling GetAllBotDependencies method and passing input arguments platformInstanceId:{0} & TenantId:{1} to get all platformdependencies", PlatformInstanceId, TenantId), LogHandler.Layer.WebServiceHost, null);
                List<IE.RecipientConfigurationDetails> res = Translator.ResourceHandler_BE_IE.RecipientConfigurationDetails_BEtoIE(notificationConfig.GetRecipientConfigDetails(resourceId, tenantId, referenceType));
                // LogHandler.LogDebug(string.Format("Tranlated the response of GetAllBotDependencies() from BE to IE"), LogHandler.Layer.WebServiceHost, null);
                // LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.Business, null);
                return res;
            }
            catch (Exception ex)
            {
                Console.Write("Exception Occured in GetNotificationTransactionDetails " + ex.Message);
                //LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.WebServiceHost, null);
                return null;
            }

        }

        [Route("GetResourceDetails/{resourceId=resourceId}/{tenantId=tenantId}/{platformId=platformId}")]
        public IE.ResourceDetails GetResourceDetails(string resourceId, int tenantId, int platformId)
        {
            //LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                NotificationBuilder notificationBuilder = new NotificationBuilder();
                // LogHandler.LogDebug(string.Format("Calling GetAllBotDependencies method and passing input arguments platformInstanceId:{0} & TenantId:{1} to get all platformdependencies", PlatformInstanceId, TenantId), LogHandler.Layer.WebServiceHost, null);
                IE.ResourceDetails res = Translator.ResourceHandler_BE_IE.ResourceDetails_BEtoIE(notificationBuilder.GetResourceDetails(resourceId, tenantId, platformId));
                // LogHandler.LogDebug(string.Format("Tranlated the response of GetAllBotDependencies() from BE to IE"), LogHandler.Layer.WebServiceHost, null);
                // LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.Business, null);
                return res;
            }
            catch (Exception ex)
            {
                Console.Write("Exception Occured in GetNotificationTransactionDetails " + ex.Message);
                //LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.WebServiceHost, null);
                return null;
            }

        }

        [Route("GetAnomalyReason/{referenceType=referenceType}/{referenceKey=referenceKey}/{tenantId=tenantId}/{platformId=platformId}")]
        public IE.NotificationConfigurationDetails GetAnomalyReason(string referenceType, string referenceKey, int tenantId,int platformId)
        {
            //LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                NotificationConfiguration notificationConfig = new NotificationConfiguration();
                // LogHandler.LogDebug(string.Format("Calling GetAllBotDependencies method and passing input arguments platformInstanceId:{0} & TenantId:{1} to get all platformdependencies", PlatformInstanceId, TenantId), LogHandler.Layer.WebServiceHost, null);
                IE.NotificationConfigurationDetails res = Translator.ResourceHandler_BE_IE.AnomalyReason_BEtoIE(notificationConfig.GetAnomalyReason(referenceType, referenceKey, tenantId, platformId));
                // LogHandler.LogDebug(string.Format("Tranlated the response of GetAllBotDependencies() from BE to IE"), LogHandler.Layer.WebServiceHost, null);
                // LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.Business, null);
                return res;
            }
            catch (Exception ex)
            {
                Console.Write("Exception Occured in GetNotificationTransactionDetails " + ex.Message);
                //LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.WebServiceHost, null);
                return null;
            }

        }

        [Route("GetEnvironmentScanAnomalyDetails/{resourceId=resourceId}/{observationId=observationId}/{platformId=platformId}/{tenantId=tenantId}")]
        public IE.EnvironmentScanReportDetails GetEnvironmentScanAnomalyDetails(string resourceId, int observationId, int platformId, int tenantId)
        {
            try
            {
                NotificationBuilder notificationBuilder = new NotificationBuilder();
                // LogHandler.LogDebug(string.Format("Calling GetAllBotDependencies method and passing input arguments platformInstanceId:{0} & TenantId:{1} to get all platformdependencies", PlatformInstanceId, TenantId), LogHandler.Layer.WebServiceHost, null);
                IE.EnvironmentScanReportDetails res = Translator.ResourceHandler_BE_IE.EnvironmentScanAnomalyDetails_BEtoIE(notificationBuilder.GetEnvironmentScanAnomalyDetails(resourceId,observationId, platformId, tenantId));
                // LogHandler.LogDebug(string.Format("Tranlated the response of GetAllBotDependencies() from BE to IE"), LogHandler.Layer.WebServiceHost, null);
                // LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.Business, null);
                return res;
            }
            catch (Exception ex)
            {
                Console.Write("Exception Occured in GetNotificationTransactionDetails " + ex.Message);
                //LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.WebServiceHost, null);
                return null;
            }
        }

        [Route("GetEnvironmentScanComparisonReport/{resourceId=resourceId}/{startDate=startDate}/{endDate=endDate}/{platformId=platformId}/{tenantId=tenantId}")]
        public IE.EnvironmentScanComparisonReport GetEnvironmentScanComparisonReport(string resourceId, string startDate,string endDate, int platformId, int tenantId)
        {
            try
            {
                EnvironmentScanMetricAnalyser analyser = new EnvironmentScanMetricAnalyser();
                // LogHandler.LogDebug(string.Format("Calling GetAllBotDependencies method and passing input arguments platformInstanceId:{0} & TenantId:{1} to get all platformdependencies", PlatformInstanceId, TenantId), LogHandler.Layer.WebServiceHost, null);
                IE.EnvironmentScanComparisonReport res = Translator.ResourceHandler_BE_IE.EnvironmentScanComparisonReport_BEtoIE(analyser.GetEnvironmentScanComparisonReport(resourceId, startDate,endDate, platformId, tenantId));
                // LogHandler.LogDebug(string.Format("Tranlated the response of GetAllBotDependencies() from BE to IE"), LogHandler.Layer.WebServiceHost, null);
                // LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.Business, null);
                return res;
            }
            catch (Exception ex)
            {
                Console.Write("Exception Occured in GetNotificationTransactionDetails " + ex.Message);
                //LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.WebServiceHost, null);
                return null;
            }           
        }
        [Route("GetEnvironmentScanComparisonPlatformReport/{platformId=platformId}/{tenantId=tenantId}/{startDate=startDate}/{endDate=endDate}")]
        public IE.finalReport GetEnvironmentScanComparisonPlatformReport(int platformId, int tenantId, string startDate, string endDate)
        {
            try
            {
                EnvironmentScanMetricAnalyser analyser = new EnvironmentScanMetricAnalyser();
                IE.finalReport res = Translator.ResourceHandler_BE_IE.EnvironmentScanComparisonPlatformReport_BEtoIE(analyser.GetEnvironmentScanComparisonPlatformReport(platformId, tenantId, startDate, endDate));
                return res;
            }
            catch(Exception ex)
            {
                Console.Write("Exception Occured in GetNotificationTransactionDetails " + ex.Message);
                //LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAllBotPlatformInstanceDependencies", "ResourceHandlerController"), LogHandler.Layer.WebServiceHost, null);
                return null;
            }
        }



    }
}
