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
    public class RemediationPlanController : ApiController
    {

        [Route("GetRemediationPlanDetails/{TenantId=TenantId}")]
        public IE.RemediationPlanDetails getRemediationPlanDetails(int tenantID)
        {
            try
            {
                RemediationPlanBuilder remBuilder = new RemediationPlanBuilder();
                //ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                IE.RemediationPlanDetails rem = Translator.ResourceModel_BE_IE.remediationPlan_BEtoIE(remBuilder.getRemediationPlanDetails(tenantID));
                return rem;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPut]
        [Route("InsertRemediationPlanDetails")]
        public IE.addRemediationPlanDetails insertRemediationPlanDetails(IE.RemediationPlanDetails remediation)
        {

            IE.addRemediationPlanDetails addMsg = new IE.addRemediationPlanDetails();
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "AddRemediationPlanDetails", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                RemediationPlanBuilder remBuilder = new RemediationPlanBuilder();
                // ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                string message = remBuilder.insertRemediationPlanDetails(Translator.ResourceModel_BE_IE.remediationPlan_IEtoBE(remediation));
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
        [Route("UpdateRemediationPlanDetails")]
        public IE.addRemediationPlanDetails updateRemediationPlanDetails(IE.RemediationPlanDetails remediation)
        {

            IE.addRemediationPlanDetails addMsg = new IE.addRemediationPlanDetails();
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "UpdateRemediationPlanDetails", "RemediationPlanController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                RemediationPlanBuilder remBuilder = new RemediationPlanBuilder();
                // ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                string message = remBuilder.updateRemediationPlanDetails(Translator.ResourceModel_BE_IE.remediationPlan_IEtoBE(remediation));
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
        [Route("DeleteRemediationPlanDetails")]
        public IE.addRemediationPlanDetails deleteRemediationPlanDetails(IE.RemediationPlanDetails remediation)
        {
            IE.addRemediationPlanDetails addMsg = new IE.addRemediationPlanDetails();
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "AddResourceModelConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                RemediationPlanBuilder remBuilder = new RemediationPlanBuilder();
                // ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                string message = remBuilder.deleteRemediationPlan(Translator.ResourceModel_BE_IE.remediationPlan_IEtoBE(remediation));
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
