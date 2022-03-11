/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infosys.Solutions.Ainauto.Superbot.Models
{
    public class HealthCheckReq
    {
        public List<HealthCheckReqMsg> MetricMessages { get; set; }
    }
    public class HealthCheckReqMsg
    {
        public string TransactionId { get; set; }
        public string ConfigId { get; set; }
        public string PortfolioId { get; set; }
        public string ResourceId { get; set; }
        public string ObservableId { get; set; }
        public string MetricName { get; set; }
        public int Count { get; set; }
        public string MetricValue { get; set; }
        public string MetricTime { get; set; }
        public string ResourceTypeId { get; set; }
        public string IncidentId { get; set; }
        public string IncidentTime { get; set; }
        public string Serverstate { get; set; }
        public string EventType { get; set; }
        public string Application { get; set; }
        public string Source { get; set; }
        public string ServerIp { get; set; }
        public string Description { get; set; }
    }

    public class HealthCheckResMsg
    {
        public string Message { get; set; }
    }

    public class SummaryNotificationReqMsg
    {
        public string ConfigId { get; set; }
        public string PortfolioId { get; set; }
        public int PlatformId { get; set; }
        public int TenantId { get; set; }
        public string TransactionId { get; set; }
        public string ApplicationName { get; set; }
    }

    public class EnvironmentScanConsolidatedReqMsg
    {
        public int PlatformId { get; set; }
        public int TenantId { get; set; }
    }
}