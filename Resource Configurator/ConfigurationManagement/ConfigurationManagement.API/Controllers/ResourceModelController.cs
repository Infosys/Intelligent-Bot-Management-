/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using IE =Infosys.Solutions.Ainauto.ConfigurationManagement.API.Models;
using Infosys.Solutions.ConfigurationManager.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Infosys.Ainauto.ConfigurationManager.BusinessComponent;
using Newtonsoft.Json;

namespace Infosys.Solutions.Ainauto.ConfigurationManagement.API.Controllers
{
    //[EnableCors(origins: "http://example.com", headers: "*", methods: "*")]
    public class ResourceModelController : ApiController
    {
        [Route("GetActiveResourceModelConfiguration/{PlatformInstanceId=PlatformInstanceId}/{TenantId=TenantId}/{ResourceTypeName=ResourceTypeName}")]
        public List<IE.Resources> getActiveResourceModelConfiguration(string PlatformInstanceId, string TenantId,string ResourceTypeName)

        {


            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "getActiveResourceModelConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                List<IE.Resources> resources = new List<IE.Resources>();
                var resource=Translator.ResourceModel_BE_IE.Resource_BEtoIE(objResourceModel.getActiveResource(PlatformInstanceId, TenantId, ResourceTypeName));
                resources.Add(resource);
                //var json = JsonConvert.SerializeObject(resources);
                return resources;
            }
            catch (Exception ex)
            {                
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "getActiveResourceModelConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
                throw ex;
            }

        }

        [Route("GetSummaryViewConfiguration/{PlatformInstanceId=PlatformInstanceId}/{TenantId=TenantId}/{ResourceTypeName=ResourceTypeName}")]
        public IE.SummaryResourceInfo getSummaryViewConfiguration(string PlatformInstanceId, string TenantId, string ResourceTypeName)
        {


            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "getActiveResourceModelConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                //List<IE.SummaryResourceInfo> resources = new List<IE.SummaryResourceInfo>();
                var resource = Translator.ResourceModel_BE_IE.SummaryResource_BEtoIE(objResourceModel.getSummaryResources(PlatformInstanceId, TenantId, ResourceTypeName));
               // resources.Add(resource)
                return resource;
            }
            catch (Exception ex)
            {
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "getActiveResourceModelConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
                throw ex;
            }

        }

        [HttpPost]
        [Route("UpdateSummaryViewConfiguration")]
        public IE.ResourceModelUpdate updateSummaryViewConfiguration(IE.SummaryResourceInfo resource)
        {
            IE.ResourceModelUpdate updateMsg = new IE.ResourceModelUpdate();
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "updateSummaryViewConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
            try
            {

                ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                string message = objResourceModel.updateSummaryViewConfiguration(Translator.ResourceModel_BE_IE.SummaryResource_IEtoBE(resource));
                updateMsg.Message = message;
                //rtConfigurations.Add(resourceType);
                return updateMsg;
            }
            catch (Exception ex)
            {
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "updateSummaryViewConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
                // return "Exception Occured in updateResourceModelConfiguration " + ex.Message;
                updateMsg.Message = "Exception Occured in updateSummaryViewConfiguration " + ex.Message;
                return updateMsg;
            }

        }

        [Route("GetAllResourceModelConfiguration/{PlatformInstanceId=PlatformInstanceId}/{TenantId=TenantId}")]
        public List<IE.ResourceModel> GetAllResourceModelConfiguration(string PlatformInstanceId, int TenantId)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GetAllResourceModelConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                List<IE.ResourceModel> resources = new List<IE.ResourceModel>();
                var resource = Translator.ResourceModel_BE_IE.ResourceModel_BEtoIE(objResourceModel.getAllResources(PlatformInstanceId, TenantId));
                resources.Add(resource);
                //var json = JsonConvert.SerializeObject(resources);
                return resources;
            }
            catch (Exception ex)
            {
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAllResourceModelConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
                throw ex;
            }

        }

        [Route("GetResourceTypeConfiguration/{PlatformInstanceId=PlatformInstanceId}/{TenantId=TenantId}")]
        public List<IE.ResourceTypeConfiguration> getResourceTypeConfiguration(string PlatformInstanceId, string TenantId)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "getResourceTypeConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                List<IE.ResourceTypeConfiguration> rtConfigurations = new List<IE.ResourceTypeConfiguration>();
                var resourceType = Translator.ResourceModel_BE_IE.ResourceType_BEtoIE(objResourceModel.getResourceTypeConfiguration(PlatformInstanceId, TenantId));
                rtConfigurations.Add(resourceType);
                return rtConfigurations;
            }
            catch (Exception ex)
            {
                Console.Write("Exception Occured in getResourceTypeConfiguration " + ex.Message);
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "getResourceTypeConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
                throw ex;
            }

        }

        [HttpPost]
        [Route("UpdateResourceModelConfiguration")]
        public IE.ResourceModelUpdate updateResourceModelConfiguration(IE.Resources resource)
        {
            IE.ResourceModelUpdate updateMsg = new IE.ResourceModelUpdate();
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "updateResourceModelConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                
                ResourceModelBuilder objResourceModel = new ResourceModelBuilder();                
                string message = objResourceModel.updateResourceModelConfiguration(Translator.ResourceModel_BE_IE.Resource_IEtoBE(resource));
                updateMsg.Message = message;
                //rtConfigurations.Add(resourceType);
                return updateMsg;
            }
            catch (Exception ex)
            {                
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "updateResourceModelConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
               // return "Exception Occured in updateResourceModelConfiguration " + ex.Message;
                updateMsg.Message = "Exception Occured in updateResourceModelConfiguration " + ex.Message;
                return updateMsg;
            }

        }
                       
        [Route("AddResourceModelConfiguration")]
        [HttpPut]
        public IE.ResourceModelUpdate AddResourceModelConfiguration(IE.Resources resource)
        {
            IE.ResourceModelUpdate addMsg = new IE.ResourceModelUpdate();
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "AddResourceModelConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
            try
            {

                ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                string message = objResourceModel.AddResourceModel(Translator.ResourceModel_BE_IE.Resource_IEtoBE(resource));
                addMsg.Message = message;
                return addMsg;
            }
            catch (Exception ex)
            {                
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "AddResourceModelConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
                // return "Exception Occured in updateResourceModelConfiguration " + ex.Message;
                addMsg.Message = "Exception Occured in AddResourceModelConfiguration " + ex.Message;
                return addMsg;
            }

        }

        [Route("GetObservablesandRemediationDetails/{PlatformId=PlatformId}/{TenantId=TenantId}/{ResourceTypeName=ResourceTypeName}")]
        public List<IE.ObservablesandRemediationPlanDetails> getObservablesandRemediationDetails(int PlatformId,int TenantId,string ResourceTypeName)
        {
            try
            {
                ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                List<IE.ObservablesandRemediationPlanDetails> remediationDetails = new List<IE.ObservablesandRemediationPlanDetails>();
                var remDetails = Translator.ResourceModel_BE_IE.ResourceRemediations_BEtoIE(objResourceModel.GetObservablesandRemediationDetails(PlatformId, TenantId, ResourceTypeName));
                remediationDetails.Add(remDetails);
                return remediationDetails;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        [Route("GetAllPortfolios/{TenantId=TenantId}")]
        public List<IE.PortfolioInfo> getAllPortfolios(int tenantId)
        {
            try
            {
                ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                List<IE.PortfolioInfo> portfolios = new List<IE.PortfolioInfo>();
                portfolios = Translator.ResourceModel_BE_IE.Portfolios_BE_IE(objResourceModel.getAllPortfolios(tenantId));
                //var remDetails = Translator.ResourceModel_BE_IE.ResourceRemediations_BEtoIE(objResourceModel.GetObservablesandRemediationDetails(PlatformId, TenantId, ResourceTypeName));
               // remediationDetails.Add(remDetails);
                return portfolios;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [Route("GetAllResourceTypeForPortfolios/{TenantId=TenantId}")]
        public List<IE.PortfolioResourceTypeInfo> getAllResourceTypeForPortfolios(int tenantId)
        {
            try
            {
                ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                List<IE.PortfolioResourceTypeInfo> portfolios = new List<IE.PortfolioResourceTypeInfo>();
                portfolios = Translator.ResourceModel_BE_IE.Portfolios_BE_IEnew(objResourceModel.getAllResourceTypePortfolios(tenantId));
                return portfolios;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


       [Route("GetTenantDetails/{TenantId=TenantId}")]
        public List<IE.TenantInfo> getTenantDetails(int tenantId=0)
        {
            try
            {
                ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                List<IE.TenantInfo> tinfo = new List<IE.TenantInfo>();
                tinfo = Translator.ResourceModel_BE_IE.TenantInfo_BEtoIE(objResourceModel.getTenantDetails(tenantId));
                return tinfo;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [Route("GetAllPlatformDetails/{TenantId=TenantId}")]
        public List<IE.PlatformDetails> GetAllPlatformDetails(int tenantId)
        {
            try
            {
                ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                List<IE.PlatformDetails> platformDetails = Translator.ResourceModel_BE_IE.PlatformDetails_BE_IE(objResourceModel.GetPlatformDetails(tenantId));
                return platformDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //[HttpPost]
        //[Route("GenerateResourceModel")]
        //public IE.ResourceModelGenerationResMsg GenerateResourceModel(IE.ResourceModelGenerationReqMsg generationReqMsg)
        //{
        //    try
        //    {                
        //        ResourceModelBuilder builderObj = new ResourceModelBuilder();
        //        IE.ResourceModelGenerationResMsg resMsg = Translator.ResourceModel_BE_IE.ResourceModelGenerationResMsg_BEtoIE( builderObj.GenerateResourceModel(Translator.ResourceModel_BE_IE.ResourceModelGenerationReqMsg_IEtoBE(generationReqMsg)));
        //        return resMsg;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        [HttpPost]
        [Route("GenerateResourceModel")]
        public IE.ResourceModelGenerationResMsg GenerateResourceModel(IE.ResourceModelConfigInput rmcObj)
        {
            try
            {
                var generationReqMsg = Translator.ResourceModelConfig.ResourceModelConfigToBE(rmcObj);
                ResourceModelBuilder builderObj = new ResourceModelBuilder();
                IE.ResourceModelGenerationResMsg resMsg = Translator.ResourceModel_BE_IE.ResourceModelGenerationResMsg_BEtoIE(builderObj.GenerateResourceModel(generationReqMsg));
                return resMsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        [Route("GenerateResourceModelUiPath")]
        public IE.ResourceModelGenerationResMsg GenerateResourceModelUiPath(IE.ResourceModelConfigInput rmcObj)
        {
            try
            {
                var generationReqMsg = Translator.ResourceModelConfig.ResourceModelConfigInputIEtoBE(rmcObj);
                DiscoveryUiPath builderObj = new DiscoveryUiPath();
                IE.ResourceModelGenerationResMsg resMsg = Translator.ResourceModel_BE_IE.ResourceModelGenerationResMsg_BEtoIE(builderObj.GenerateUiPathResourceModel(generationReqMsg));
                return resMsg;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPut]
        [Route("UpdateCategoryandScriptId")]
        public string UpdateCategoryandScriptId(IE.CategoryAndScriptDetails objIE)
        {
            ResourceModelBuilder resourceModelBuilder = new ResourceModelBuilder();
            try
            {
                bool status = resourceModelBuilder.UpdateCategoryandScriptId(Translator.CategoryAndScriptDetails.CategoryAndScriptDetails_IEtoBE(objIE));
                if (status)
                    return "Updation Successfull";
                return "Updation Failed";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("getDynamicAutomationTypes")]
        public List<IE.RPAType> getDynamicAutomationTypes()
        {
            try
            {
                ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                List<IE.RPAType> types = new List<IE.RPAType>();
                types = Translator.ResourceModel_BE_IE.AutomationTypes_BEtoIE(objResourceModel.GetDynamicAutomationTypes());
                
                return types;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPut]
        [Route("LoadSEEScripts")]
        public IE.GetSEEScriptDetailsResMsg LoadSEEScripts(IE.GetSEEScriptDetailsReqMsg obj)
        {
            try
            {
                ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                var requestObj = Translator.CategoryAndScriptDetails.GetSEEScriptDetailsReqMsg_IEtoBE(obj);
                IE.GetSEEScriptDetailsResMsg response = new IE.GetSEEScriptDetailsResMsg();
                response = Translator.CategoryAndScriptDetails.GetSEEScriptDetailsResMsg_BEtoIE(objResourceModel.GetSEEScriptDetails(requestObj));

                return response;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [HttpPut]
        [Route("LoadSEECategories")]
        public List<IE.Property> LoadSEECategories(IE.GetSEECategoriesReqMsg obj)
        {
            try
            {
                ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                var requestObj = Translator.CategoryAndScriptDetails.GetSEECategoriesReqMsg_IEtoBE(obj);
                List<IE.Property> response = new List<IE.Property>();
                response = Translator.CategoryAndScriptDetails.GetSEECategoriesResMsg_BEtoIE(objResourceModel.GetSEECategories(requestObj));

                return response;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        [Route("GetAADetails")]
        public List<IE.AABot> GetAADetails()
        {
            try
            {
                ResourceModelBuilder objResourceModel = new ResourceModelBuilder();

                IE.AABotList response = new IE.AABotList();
                response = Translator.CategoryAndScriptDetails.GetAABotDetails_BEtoIE(objResourceModel.GetAABotList());
                List<IE.AABot> res = response.list;
                return res;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }
}
