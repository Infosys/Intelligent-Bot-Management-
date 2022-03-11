/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BE = Infosys.Solutions.Ainauto.BusinessEntity;
using IE = Infosys.Solutions.Ainauto.Superbot.Models;

namespace Infosys.Solutions.Ainauto.Superbot.Translator
{
    public static class HealthCheck_IE_BE
    {
        public static BE.MetricMessage HealthCheckMetric_IE_BE(IE.HealthCheckReq healthCheckMsgIE)
        {
            BE.MetricMessage metricMsg_BE = new BE.MetricMessage();
            metricMsg_BE.MetricMessages = new List<BE.Metric>();
            foreach (var healthCheckIE in healthCheckMsgIE.MetricMessages)
            {
                BE.Metric metric = new BE.Metric();

                metric.ConfigId = healthCheckIE.ConfigId;
                metric.PortfolioId = healthCheckIE.PortfolioId;
                metric.ResourceId = healthCheckIE.ResourceId;
                metric.ObservableId = healthCheckIE.ObservableId;
                metric.MetricName = healthCheckIE.MetricName;
                metric.Count = healthCheckIE.Count;
                metric.MetricValue = healthCheckIE.MetricValue;
                metric.MetricTime = healthCheckIE.MetricTime;
                metric.ResourceTypeId = healthCheckIE.ResourceTypeId;
                metric.IncidentId = healthCheckIE.IncidentId;
                metric.IncidentTime = healthCheckIE.IncidentTime;
                metric.Serverstate = healthCheckIE.Serverstate;
                metric.EventType = healthCheckIE.EventType;
                metric.Application = healthCheckIE.Application;
                metric.Source = healthCheckIE.Source;
                metric.ServerIp = healthCheckIE.ServerIp;
                metric.TransactionId = healthCheckIE.TransactionId;
                metric.Description = healthCheckIE.Description;

                metricMsg_BE.MetricMessages.Add(metric);
            }
            
            return metricMsg_BE;
        }

        public static BE.SummaryNotificationReqMsg SummaryNotificationReqMsg_IE_BE(IE.SummaryNotificationReqMsg objIE)
        {
            BE.SummaryNotificationReqMsg objBE = new BE.SummaryNotificationReqMsg();
            objBE.ConfigId = objIE.ConfigId;
            objBE.ApplicationName = objIE.ApplicationName;
            objBE.PortfolioId = objIE.PortfolioId;
            objBE.PlatformId = objIE.PlatformId;
            objBE.TenantId = objIE.TenantId;
            objBE.TransactionId = objIE.TransactionId;

            return objBE;
        }

        public static BE.EnvironmentScanConsolidatedReqMsg EnvironmentScanConsolidatedReqMsg_IE_BE(IE.EnvironmentScanConsolidatedReqMsg objIE)
        {
            BE.EnvironmentScanConsolidatedReqMsg objBE = new BE.EnvironmentScanConsolidatedReqMsg();
            
            objBE.PlatformId = objIE.PlatformId;
            objBE.TenantId = objIE.TenantId;            

            return objBE;
        }
    }
}