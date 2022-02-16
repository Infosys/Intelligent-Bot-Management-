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
    public class ObservableController : ApiController
    {
        [Route("GetObservableDetails/{TenantId=TenantId}")]
        public IE.observable getObservableDetails(int tenantID)
        {
            try
            {
                ObservableBuilder obsBuilder = new ObservableBuilder();
                //ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                IE.observable obs = Translator.ResourceModel_BE_IE.observable_BEtoIE(obsBuilder.getObservableDetails(tenantID));
                return obs;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        [HttpPut]
        [Route("AddObservableDetails")]
        public IE.addObservableDetails addObservableDetails(IE.observable observable)
        {

            IE.addObservableDetails addMsg = new IE.addObservableDetails();
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "AddResourceModelConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                ObservableBuilder obsBuilder = new ObservableBuilder();
                // ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                string message = obsBuilder.createObservableDetails(Translator.ResourceModel_BE_IE.observable_IEtoBE(observable));
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
        [Route("UpdateObservableDetails")]
        public IE.addObservableDetails UpdateObservableDetails(IE.observable observable)
        {
            IE.addObservableDetails addMsg = new IE.addObservableDetails();
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "AddResourceModelConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                ObservableBuilder obsBuilder = new ObservableBuilder();
                // ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                string message = obsBuilder.updateObservableDetails(Translator.ResourceModel_BE_IE.observable_IEtoBE(observable));
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
