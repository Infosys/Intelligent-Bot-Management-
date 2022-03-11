/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using Infosys.Solutions.Ainauto.Services.Superbot.Contracts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.Solutions.Ainauto.Services.Superbot.Contracts
{
    [ServiceContract]
    public interface IResourceHandler
    {        

        [OperationContract]
        //[WebGet(UriTemplate = "/GetAllBotPlatformInstanceDependencies?platformId={PlatformInstanceId}&tenantId={TenantId}")]
        [WebGet]
        PlatformInstanceDetails GetAllBotPlatformInstanceDependencies(string PlatformInstanceId, string TenantId,string dependencyResourceID);

        [OperationContract]
        //[WebGet(UriTemplate = "/GetAllBotPlatformInstanceDependencies?platformId={PlatformInstanceId}&tenantId={TenantId}")]
        [WebGet]
        ObservationDetails GetObservationsDetails(int observationId, int platformId, int tenantId);

        [OperationContract]
        //[WebGet(UriTemplate = "/GetAllBotPlatformInstanceDependencies?platformId={PlatformInstanceId}&tenantId={TenantId}")]
        [WebGet]
        RemediationPlanDetails GetRemediationPlanDetails(int remediationPlanId, int tenantId);

        [OperationContract]
        //[WebGet(UriTemplate = "/GetAllBotPlatformInstanceDependencies?platformId={PlatformInstanceId}&tenantId={TenantId}")]
        [WebGet]
        List<NotificationConfigurationDetails> GetSMTPConfigurationDetails(int platformId, int tenantId, string referenceType);

        [OperationContract]
        //[WebGet(UriTemplate = "/GetAllBotPlatformInstanceDependencies?platformId={PlatformInstanceId}&tenantId={TenantId}")]
        [WebGet]
        List<RecipientConfigurationDetails> GetRecipientConfigurationDetails(string resourceId, int tenantId, string referenceType);

        [OperationContract]
        //[WebGet(UriTemplate = "/GetAllBotPlatformInstanceDependencies?platformId={PlatformInstanceId}&tenantId={TenantId}")]
        [WebGet]
        ResourceDetails GetResourceDetails(string resourceId, int tenantId, int platformId);

        [OperationContract]
        //[WebGet(UriTemplate = "/GetAllBotPlatformInstanceDependencies?platformId={PlatformInstanceId}&tenantId={TenantId}")]
        [WebGet]
        NotificationConfigurationDetails GetAnomalyReason(string referenceType, string referenceKey, int tenantId, int platformId);

        [OperationContract]
        //[WebGet(UriTemplate = "/GetAllBotPlatformInstanceDependencies?platformId={PlatformInstanceId}&tenantId={TenantId}")]
        [WebGet]
        EnvironmentScanReportDetails GetEnvironmentScanAnomalyDetails(string resourceId, int observationId, int platformId, int tenantId);

        [OperationContract]
        //[WebGet(UriTemplate = "/GetAllBotPlatformInstanceDependencies?platformId={PlatformInstanceId}&tenantId={TenantId}")]
        [WebGet]
        EnvironmentScanComparisonReport GetEnvironmentScanComparisonReport(string resourceId, string startDate, string endDate, int platformId, int tenantId);

    }

}
