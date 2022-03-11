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
    public class ObservableResourceTypeModelController : ApiController
    {
        [Route("GetObservableResourceTypeActionMapDetails/{TenantId=TenantId}")]
        public IE.ObservableResourceTypeActionMap getObservableResourceTypeActionMapDetails(int tenantID)
        {
            try
            {
                ObservationResourceTypeActionMapBuilder observationResourceTypeActionMapBuilder = new ObservationResourceTypeActionMapBuilder();
                //ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                IE.ObservableResourceTypeActionMap observableResourceTypeActionMap = Translator.ResourceModel_BE_IE.ResourceTypeActionMap_BEtoIE(observationResourceTypeActionMapBuilder.getObservableResourceTypeActionMapDetails(tenantID));
                return observableResourceTypeActionMap;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        [HttpPut]
        [Route("AddResourceTypeActionMap")]
        public IE.addResourceTypeObservableActionMap AddResourceTypeActionMap(IE.ObservableResourceTypeActionMap observableResourceTypeActionMap)
        {
            IE.addResourceTypeObservableActionMap addMsg = new IE.addResourceTypeObservableActionMap();
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "AddResourceModelConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                ObservationResourceTypeActionMapBuilder observationResourceTypeActionMapBuilder = new ObservationResourceTypeActionMapBuilder();
                // ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                string message = observationResourceTypeActionMapBuilder.createObservableResourceTypeActionMapDetails(Translator.ResourceModel_BE_IE.observableResourceTypeActionMap_IEtoBE(observableResourceTypeActionMap));
                if (message.Contains("exception"))
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }
                else
                {
                    addMsg.Message = message;

                    return addMsg;
                }
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
                
            }


        }
        [HttpPut]
        [Route("UpdateResourceTypeActionMap")]
        public IE.addResourceTypeObservableActionMap UpdateResourceTypeActionMap(IE.ObservableResourceTypeActionMap observableResourceTypeActionMap)
        {
            IE.addResourceTypeObservableActionMap addMsg = new IE.addResourceTypeObservableActionMap();
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "AddResourceModelConfiguration", "ResourceModelController"), LogHandler.Layer.WebServiceHost, null);
            try
            {
                ObservationResourceTypeActionMapBuilder observationResourceTypeActionMapBuilder = new ObservationResourceTypeActionMapBuilder();
                // ResourceModelBuilder objResourceModel = new ResourceModelBuilder();
                string message = observationResourceTypeActionMapBuilder.updatebservableResourceTypeActionMapDetails(Translator.ResourceModel_BE_IE.observableResourceTypeActionMap_IEtoBE(observableResourceTypeActionMap));
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



        [Route("GetDummyVariable")]
        [HttpGet]
        public string GetDummyVariable()
        {
            ObservationResourceTypeActionMapBuilder observationResourceTypeActionMapBuilder = new ObservationResourceTypeActionMapBuilder();
            return observationResourceTypeActionMapBuilder.returnDummyVariable();
        }


    }
}
