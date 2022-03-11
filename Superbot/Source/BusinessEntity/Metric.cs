/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.Solutions.Ainauto.BusinessEntity
{
    public class Metric
    {
        public string EventType { get; set; }
        public string Application { get; set; }
        public string MetricTime { get; set; }
        public string ResourceId { get; set; }
        public string MetricName { get; set; }
        public string MetricValue { get; set; }
        public string ServerIp { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public string SequenceNumber { get; set; }

        //newly added properrties for elastic search
        public string ConfigId { get; set; }
        public string PortfolioId { get; set; }
        public string ObservableId { get; set; }
        public int Count { get; set; }
        public string ResourceTypeId { get; set; }
        public string IncidentId { get; set; }
        public string IncidentTime { get; set; }
        public string Serverstate { get; set; }
        public string TransactionId { get; set; }
    }

    public class MetricMessage
    {
        public List<Metric> MetricMessages { get; set; }        
    }
    public class EnvScanMetric
    {
        public string metricid { get; set; }
        public string metricname { get; set; }
        public string metrickey { get; set; }
        public List<EnvScanMetricAttributes> metricvalue { get; set; }
    }

    public class EnvScanMetricAttributes
    {
        public string attributename { get; set; }
        public string attributevalue { get; set; }
        public string displayname { get; set; }
    }

    public class EnvScanMetricQueue
    {
        public int observationid { get; set; }
        public int version { get; set; }
        public string metricid { get; set; }
        public string metricname { get; set; }
        public string metrickey { get; set; }
        public List<EnvScanMetricAttributes> metricvalue { get; set; }
    }

}
