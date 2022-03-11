/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.Solutions.Ainauto.Resource.DataAccess.Queue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE = Infosys.Solutions.Ainauto.BusinessEntity;
using DE = Infosys.Solutions.Superbot.Resource.Entity;
using Infosys.Solutions.Superbot.Infrastructure.Common;

namespace Infosys.Solutions.Ainauto.Superbot.BusinessComponent
{
    public class HealthCheckBuilder
    {
        public string PushData(BE.MetricMessage metricBE)
        {
            try
            {
                DE.Queue.MetricMessage metric = new DE.Queue.MetricMessage();
                EntityTranslator translator = new EntityTranslator();
                metric = translator.MetricBEToDE(metricBE);

                MetricProcessorDS processorDS = new MetricProcessorDS();
                string msgResponse = processorDS.Send(metric, null);
                //Console.WriteLine("Metric data sent to queue and Message:"+msgResponse);
                return msgResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string PushSummaryNotification(BE.SummaryNotificationReqMsg msg)
        {
            LogHandler.LogInfo(string.Format("PushSummaryNotification of HealthcheckBuilder class is being executed"), LogHandler.Layer.Business, null);
            try
            {
                DE.Queue.Notification metric = new DE.Queue.Notification()
                {
                    ConfigId = msg.ConfigId,
                    PortfolioId = msg.PortfolioId,
                    PlatformId = msg.PlatformId,
                    TenantId = msg.TenantId,
                    ApplicationName = msg.ApplicationName,
                    TransactionId = msg.TransactionId,
                    Type = 7,
                    Channel=1
                };
                

                NotificationDS processorDS = new NotificationDS();
                string msgResponse = processorDS.Send(metric, null);
                LogHandler.LogInfo(string.Format("Message Sent. Response : {0}", msgResponse), LogHandler.Layer.Business, null);
                //Console.WriteLine("Metric data sent to queue and Message:"+msgResponse);
                return msgResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string PushEnvironmentScanConsolidatedReport(BE.EnvironmentScanConsolidatedReqMsg msg)
        {
            LogHandler.LogInfo(string.Format("PushSummaryNotification of HealthcheckBuilder class is being executed"), LogHandler.Layer.Business, null);
            try
            {
                DE.Queue.Notification notification = new DE.Queue.Notification()
                {
                    PlatformId = msg.PlatformId,
                    TenantId = msg.TenantId,                    
                    Type = 8,
                    Channel = 1
                };


                NotificationDS processorDS = new NotificationDS();
                string msgResponse = processorDS.Send(notification, null);
                LogHandler.LogInfo(string.Format("Message Sent. Response : {0}", msgResponse), LogHandler.Layer.Business, null);
                //Console.WriteLine("Metric data sent to queue and Message:"+msgResponse);
                return msgResponse;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
