/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using IE = Infosys.Solutions.Ainauto.ConfigurationManagement.API.Models;
using Infosys.Solutions.ConfigurationManager.Infrastructure.Common;
using Infosys.Solutions.Ainauto.ConfigurationManager.BusinessComponent;
using Newtonsoft.Json;

namespace Infosys.Solutions.Ainauto.ConfigurationManagement.API.Controllers
{
    public class AnomalyRulesConfigurationController : ApiController
    {
        AnomalyRulesConfigurationBuilder anomalyRulesConfigurationBuilder;

        AnomalyRulesConfigurationController()
        {
            anomalyRulesConfigurationBuilder = new AnomalyRulesConfigurationBuilder();
        }

        [Route("GetAnomalyRulesConfiguration/{observableId=observableId}/{resourcetypeId=resourcetypeId}/{platformId=platformId}/{tenantId=tenantId}")]
        public IE.AnomalyRulesDetails GetAnomalyRulesConfiguration(int observableId, int resourcetypeId, int platformId, int tenantId)
        {
            //LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GetAnomalyRulesConfiguration", "AnomalyRulesConfiguration Controller"), LogHandler.Layer.WebServiceHost, null);
            IE.AnomalyRulesDetails anomalyRulesDetails = new IE.AnomalyRulesDetails();
            AnomalyRulesConfigurationBuilder anomalyRulesConfigurationBuilder = new AnomalyRulesConfigurationBuilder();
            try
            {
                anomalyRulesDetails = Translator.AnomalyRulesDetailsBE_IE.AnomalyRulesDetails_BE_IE(anomalyRulesConfigurationBuilder.GetAnomalyRulesConfig(observableId, resourcetypeId, platformId, tenantId));
                return anomalyRulesDetails;
            }
            catch (Exception ex)
            {
                //LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetAnomalyRulesConfiguration", "AnomalyRulesConfiguration Controller"), LogHandler.Layer.WebServiceHost, null);
                throw ex;
            }

        }

        [HttpPost]
        [Route("InsertAnomalyRulesConfiguration")]
        public string InsertAnomalyRulesConfiguration(IE.AnomalyRulesDetails anomalyRulesDetails)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "InsertAnomalyRulesConfiguration", "AnomalyRulesConfiguration Controller"), LogHandler.Layer.WebServiceHost, null);
            string returnString = string.Empty;
            AnomalyRulesConfigurationBuilder anomalyRulesConfigurationBuilder = new AnomalyRulesConfigurationBuilder();
            try
            {
                returnString = anomalyRulesConfigurationBuilder.InsertAnomalyRulesConfig(Translator.AnomalyRulesDetailsBE_IE.AnomalyRulesDetails_IE_BE(anomalyRulesDetails));
            }
            catch (Exception ex)
            {
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "InsertAnomalyRulesConfiguration", "AnomalyRulesConfiguration Controller"), LogHandler.Layer.WebServiceHost, null);
                throw ex;
            }
            return returnString;

        }

        [HttpPut]
        [Route("UpdateAnomalyRulesConfiguration")]
        public string UpdateAnomalyRulesConfiguration(IE.AnomalyRulesDetails anomalyRulesDetails)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "InsertAnomalyRulesConfiguration", "AnomalyRulesConfiguration Controller"), LogHandler.Layer.WebServiceHost, null);
            string returnString = string.Empty;
            AnomalyRulesConfigurationBuilder anomalyRulesConfigurationBuilder = new AnomalyRulesConfigurationBuilder();
            try
            {
                returnString = anomalyRulesConfigurationBuilder.UpdateAnomalyRuleConfig(Translator.AnomalyRulesDetailsBE_IE.AnomalyRulesDetails_IE_BE(anomalyRulesDetails));
            }
            catch (Exception ex)
            {
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "InsertAnomalyRulesConfiguration", "AnomalyRulesConfiguration Controller"), LogHandler.Layer.WebServiceHost, null);
                throw ex;
            }
            return returnString;

        }

        [HttpPut]
        [Route("DeleteAnomalyRulesConfiguration")]
        public string DeleteAnomalyRulesConfiguration(IE.AnomalyRulesDetails anomalyRulesDetails)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "InsertAnomalyRulesConfiguration", "AnomalyRulesConfiguration Controller"), LogHandler.Layer.WebServiceHost, null);
            string returnString = string.Empty;
            AnomalyRulesConfigurationBuilder anomalyRulesConfigurationBuilder = new AnomalyRulesConfigurationBuilder();
            try
            {
                returnString = anomalyRulesConfigurationBuilder.DeleteAnomalyRuleConfig(Translator.AnomalyRulesDetailsBE_IE.AnomalyRulesDetails_IE_BE(anomalyRulesDetails));
            }
            catch (Exception ex)
            {
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "InsertAnomalyRulesConfiguration", "AnomalyRulesConfiguration Controller"), LogHandler.Layer.WebServiceHost, null);
                throw ex;
            }
            return returnString;

        }
    }
}
