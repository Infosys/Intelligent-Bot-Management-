/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.Solutions.Superbot.Infrastructure.Common;
using Infosys.Solutions.Ainauto.Superbot.BusinessComponent;
using Infosys.Solutions.Ainauto.Superbot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Infosys.Solutions.Ainauto.Superbot.Controllers
{    
    public class HealthCheckController : ApiController
    {
        [HttpPost]
        [Route("ProcessHealthCheckData")]
        public string ProcessHealthCheckData(HealthCheckReq metric)
        {
            try
            {
                HealthCheckResMsg msg = new HealthCheckResMsg();
                HealthCheckBuilder obj = new HealthCheckBuilder();                
                string response =obj.PushData(Translator.HealthCheck_IE_BE.HealthCheckMetric_IE_BE(metric));
                msg.Message = response;
                return response;
            }
            catch (Exception ex)
            {

                return "Error Occued in sending data queue. Error Message is " + ex.Message + " Inner Exception:" + ex.InnerException;
            }
        }

        [HttpPost]
        [Route("SendSummaryNotification")]       
        public string SendSummaryNotification(SummaryNotificationReqMsg msgInp)
        {
            LogHandler.LogInfo(string.Format("SendSummaryNotification of Healthcheck controller class is being executed"), LogHandler.Layer.Business, null);
            try
            {
                HealthCheckResMsg msg = new HealthCheckResMsg();
                HealthCheckBuilder obj = new HealthCheckBuilder();
                string response = obj.PushSummaryNotification(Translator.HealthCheck_IE_BE.SummaryNotificationReqMsg_IE_BE(msgInp));
                LogHandler.LogInfo(string.Format("SendSummaryNotification response : {0}",response), LogHandler.Layer.Business, null);
                msg.Message = response;
                return response;
            }
            catch (Exception ex)
            {

                return "Error Occued in sending data queue. Error Message is " + ex.Message + " Inner Exception:" + ex.InnerException;
            }
        }

        [HttpPost]
        [Route("SendEnvironmentScanConsolidatedReport")]
        public string SendEnvironmentScanConsolidatedReport(EnvironmentScanConsolidatedReqMsg msgInp)
        {
            LogHandler.LogInfo(string.Format("SendSummaryNotification of Healthcheck controller class is being executed"), LogHandler.Layer.Business, null);
            try
            {
                HealthCheckResMsg msg = new HealthCheckResMsg();
                HealthCheckBuilder obj = new HealthCheckBuilder();
                string response = obj.PushEnvironmentScanConsolidatedReport(Translator.HealthCheck_IE_BE.EnvironmentScanConsolidatedReqMsg_IE_BE(msgInp));
                LogHandler.LogInfo(string.Format("SendSummaryNotification response : {0}", response), LogHandler.Layer.Business, null);
                msg.Message = response;
                return response;
            }
            catch (Exception ex)
            {

                return "Error Occued in sending data queue. Error Message is " + ex.Message + " Inner Exception:" + ex.InnerException;
            }
        }

    }
}
