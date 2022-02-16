/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.Solutions.Ainauto.ConfigurationManager.BusinessComponent;
using Infosys.Solutions.ConfigurationManager.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IE = Infosys.Solutions.Ainauto.ConfigurationManagement.API.Models;

namespace Infosys.Solutions.Ainauto.ConfigurationManagement.API.Controllers
{
    public class MetadataController : ApiController
    {
        [Route("getResourceTypeMetaData/{TenantId=TenantId}")]
        public List<IE.ResourceTypeMetadata> getResourceTypeMetaData(int TenantId)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "getResourceTypeMetaData", "MetadataController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                MetaDataBuilder objMetadata = new MetaDataBuilder();
                List<IE.ResourceTypeMetadata> rtMedatadata = new List<IE.ResourceTypeMetadata>();

                rtMedatadata = Translator.ResouceTypeMetaDataBE_IE.metadata_BEListtoIEList(objMetadata.getResourceTypeMetaData(TenantId));
                //rtMedatadata.Add(result);
                return rtMedatadata;
            }
            catch (Exception ex)
            {
                Console.Write("Exception Occured in getResourceTypeMetaData " + ex.Message);
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "getResourceTypeMetaData", "MetadataController"), LogHandler.Layer.WebServiceHost, null);
                throw ex;
            }

        }

        [Route("getPlatformDetails/{TenantId=TenantId}")]
        public List<IE.Platform_Info> getPlatformDetails(int TenantId)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "getPlatformDetails", "MetadataController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                MetaDataBuilder objMetadata = new MetaDataBuilder();
                List<IE.Platform_Info> platformMedatadata = new List<IE.Platform_Info>();
                //platformMedatadata = Translator.ResouceTypeMetaDataBE_IE.Platform_BEtoIE(objMetadata.getPlatformDetails(TenantId));
                return platformMedatadata;
            }
            catch (Exception ex)
            {
                Console.Write("Exception Occured in getPlatformDetails " + ex.Message);
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "getPlatformDetails", "MetadataController"), LogHandler.Layer.WebServiceHost, null);
                throw ex;
            }

        }

        [HttpPut]
        [Route("AddResourceTypeAttributes")]
        public IE.Metadata_ResponseMsg AddResourceTypeAttributes(IE.ResourceType_Attribute attribute)
        {
            IE.Metadata_ResponseMsg responseMsg = new IE.Metadata_ResponseMsg();
            try
            {
                MetaDataBuilder objMetadata = new MetaDataBuilder();              
                string respone = objMetadata.AddResourceTypeAttributes(Translator.ResouceTypeMetaDataBE_IE.ResourceTypeAttributesIE_BE(attribute));
                responseMsg.message = respone;
                
            }
            catch (Exception ex)
            {
                Console.Write("Exception Occured in AddResourceTypeAttributes " + ex.Message);
                LogHandler.LogError(string.Format("Exception occures while adding resource attributes in {0} and {1}. Error Message:{2}", "AddResourceTypeAttributes", "MetadataController", ex.Message), LogHandler.Layer.WebServiceHost, null);
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "AddResourceTypeAttributes", "MetadataController"), LogHandler.Layer.WebServiceHost, null);
                responseMsg.message = "Exception occured while adding resource attributes. Error Message: " + ex.Message;
            }
            return responseMsg;
        }

        [HttpPost]
        [Route("DeleteResourceTypeAttributes")]
        public string DeleteResourceTypeAttributes(IE.ResourceType_Attribute attribute)
        {
            try
            {
                MetaDataBuilder objMetadata = new MetaDataBuilder();
                string respone;
                respone = objMetadata.DeleteResourceTypeAttributes(Translator.ResouceTypeMetaDataBE_IE.ResourceTypeAttributesIE_BE(attribute));
                return respone;
            }
            catch (Exception ex)
            {
                Console.Write("Exception Occured in DeleteResourceTypeAttributes " + ex.Message);
                LogHandler.LogError(string.Format("Exception occures while adding resource attributes in {0} and {1}. Error Message:{2}", "DeleteResourceTypeAttributes", "MetadataController", ex.Message), LogHandler.Layer.WebServiceHost, null);
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "DeleteResourceTypeAttributes", "MetadataController"), LogHandler.Layer.WebServiceHost, null);
                return "Exception occured while adding resource attributes. Error Message: " + ex.Message;
            }
        }

        [HttpPost]
        [Route("UpdateResourceTypeMetaData")]
        public string UpdateResourceTypeMetaData(IE.ResourceTypeMetadata metadata)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "updateResourceTypeMetaData", "MetadataController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                MetaDataBuilder objMetadata = new MetaDataBuilder();               
                var response = objMetadata.UpdateResourceTypeMetaData(Translator.ResouceTypeMetaDataBE_IE.metadata_IEtoBE(metadata));
                //rtMedatadata.Add(Translator.ResouceTypeMetaDataBE_IE.metadata_BEtoIE(response));
                return response;
            }
            catch (Exception ex)
            {               
                LogHandler.LogError(string.Format(InfoMessages.Method_Execution_End, "updateResourceTypeMetaData", "MetadataController"), LogHandler.Layer.WebServiceHost, null);
                return "Exception occured while chaging metadata mapping. Error Message: " + ex.Message;
            }
        }
    }
}