/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Infosys.Solutions.Superbot.Resource.Entity.Queue
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

        //newly added properties below
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

}
