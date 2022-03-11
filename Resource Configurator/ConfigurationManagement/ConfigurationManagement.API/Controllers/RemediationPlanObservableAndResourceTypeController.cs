/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using IE = Infosys.Solutions.Ainauto.ConfigurationManagement.API.Models;
using Infosys.Solutions.ConfigurationManager.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Infosys.Solutions.Ainauto.ConfigurationManager.BusinessComponent;


namespace Infosys.Solutions.Ainauto.ConfigurationManagement.API.Controllers
{
    public class RemediationPlanObservableAndResourceTypeController : ApiController
    {
        [Route("GetRemediationPlanObservableAndResourceTypeDetails/{TenantId=TenantId}")]
        public IE.RemediationPlanObservableAndResourceTypeMap getRemediationPlanObservableAndResourceTypeDetails(int tenantID)
        {
            try
            {
                RemediationPlanObservableAndResourceTypeBuilder remBuilder = new RemediationPlanObservableAndResourceTypeBuilder();
                //ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                IE.RemediationPlanObservableAndResourceTypeMap remObj = Translator.ResourceModel_BE_IE.RemediationPlanObservableAndResourceTypeMap_BEtoIE(remBuilder.getRemediationPlanObservableAndResourceTypeDetails(tenantID));
                return remObj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPut]
        [Route("InsertRemediationPlanObservableAndResourceTypeDetails")]
        public IE.addRemePlanObsAndResourceTypeMap insertRemediationPlanObservableAndResourceTypeDetails(IE.RemediationPlanObservableAndResourceTypeMap remObj)
        {
            IE.addRemePlanObsAndResourceTypeMap addMsg = new IE.addRemePlanObsAndResourceTypeMap();
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "AddResourceModelConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                RemediationPlanObservableAndResourceTypeBuilder remBuilder = new RemediationPlanObservableAndResourceTypeBuilder();
                // ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                string message = remBuilder.insertRemediationPlanObservableAndResourceTypeDetails(Translator.ResourceModel_BE_IE.RemediationPlanObservableAndResourceTypeMap_IEtoBE(remObj));
                addMsg.Message = message;
                return addMsg;
            }
            catch (Exception ex)
            {
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "AddResourceModelConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
                // return "Exception Occured in updateResourceModelConfiguration " + ex.Message;
                addMsg.Message = "Exception Occured in AddResourceModelConfiguration " + ex.Message;
                if (ex.Message.Contains("exception"))
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }
                else
                {
                    //addMsg.Message = ex.Message;

                    return addMsg;
                }
                //return addMsg;
            }
        }

        [HttpPut]
        [Route("UpdateRemediationPlanObservableAndResourceTypeDetails")]
        public IE.addRemePlanObsAndResourceTypeMap updateRemediationPlanObservableAndResourceTypeDetails(IE.RemediationPlanObservableAndResourceTypeMap remObj)
        {
            IE.addRemePlanObsAndResourceTypeMap addMsg = new IE.addRemePlanObsAndResourceTypeMap();
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "AddResourceModelConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                RemediationPlanObservableAndResourceTypeBuilder remBuilder = new RemediationPlanObservableAndResourceTypeBuilder();
                // ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                string message = remBuilder.updateRemediationPlanObservableAndResourceTypeDetails(Translator.ResourceModel_BE_IE.RemediationPlanObservableAndResourceTypeMap_IEtoBE(remObj));
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

        [HttpPut]
        [Route("DeleteRemediationPlanObservableAndResourceTypeDetails")]
        public IE.addRemePlanObsAndResourceTypeMap deleteRemediationPlanObservableAndResourceTypeDetails(IE.RemediationPlanObservableAndResourceTypeMap remObj)
        {
            IE.addRemePlanObsAndResourceTypeMap addMsg = new IE.addRemePlanObsAndResourceTypeMap();
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "AddResourceModelConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                RemediationPlanObservableAndResourceTypeBuilder remBuilder = new RemediationPlanObservableAndResourceTypeBuilder();
                // ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                string message = remBuilder.deleteRemediationPlanObservableAndResourceTypeDetails(Translator.ResourceModel_BE_IE.RemediationPlanObservableAndResourceTypeMap_IEtoBE(remObj));
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





    }
}
