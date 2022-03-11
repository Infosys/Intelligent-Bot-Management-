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
    public class ActionModelController : ApiController
    {

        [Route("GetActionDetails/{TenantId=TenantId}")]
        public IE.action getActionDetails(int tenantID)
        {
            try
            {
                ActionBuilder ActionBuilder = new ActionBuilder();
                //ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                IE.action actionObj = Translator.ResourceModel_BE_IE.action_BEtoIE(ActionBuilder.getActionDetails(tenantID));
                return actionObj;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpPut]
        [Route("AddActionDetails")]
        public IE.addActionMessageDetails addActionDetails(IE.action actionObj)
        {
            IE.addActionMessageDetails addMsg = new IE.addActionMessageDetails();
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "AddResourceModelConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                ActionBuilder actBuilder = new ActionBuilder();
                // ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                string message = actBuilder.insertActionDetails(Translator.ResourceModel_BE_IE.action_IEtoBE(actionObj));
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
        [Route("UpdateActionDetails")]
        public IE.addActionMessageDetails updateActionDetails(IE.action actionObj)
        {
            IE.addActionMessageDetails addMsg = new IE.addActionMessageDetails();
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "AddResourceModelConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                ActionBuilder actBuilder = new ActionBuilder();
                // ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                string message = actBuilder.updateActionDetails(Translator.ResourceModel_BE_IE.action_IEtoBE(actionObj));
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
        [Route("DeleteActionDetails")]
        public IE.addActionMessageDetails deleteActionDetails(IE.action actionObj)
        {
            IE.addActionMessageDetails addMsg = new IE.addActionMessageDetails();
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "AddResourceModelConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                ActionBuilder actBuilder = new ActionBuilder();
                // ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                string message = actBuilder.deleteActionDetails(Translator.ResourceModel_BE_IE.action_IEtoBE(actionObj));
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
