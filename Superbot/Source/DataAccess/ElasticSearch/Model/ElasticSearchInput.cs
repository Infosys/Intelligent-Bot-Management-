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

namespace Infosys.Solutions.Ainauto.Resource.DataAccess.ElasticSearch.Model
{
    public class ElasticSearchInput
    {
        public string ConfigId { get; set; }
        public string PortfolioId { get; set; }
        public string PortfolioName { get; set; }
        public string ResourceId { get; set; }
        public string ResourceName { get; set; }
        public string ObservabeId { get; set; }
        public string ObservableName { get; set; }
        public string Count { get; set; }
        public string MetricValue { get; set; }
        public double MetricValueinNumeric { get; set; }
        public DateTime MetricTime { get; set; }
        public string MetricTimeString { get; set; }
        public string ResourceTypeId { get; set; }
        public string ResourceTypeName { get; set; }
        public string IncidentId { get; set; }
        public DateTime IncidentCreateTime { get; set; }
        public string ServerState { get; set; }
        public long IsCritical { get; set; }
        public long IsHealthy { get; set; }
        public long IsWarning { get; set; }
        public string LowerThreshold { get; set; }
        public string UpperThreshold { get; set; }
        public double LowerThresholdinNumeric { get; set; }
        public double UpperThresholdinNumeric { get; set; }
    }
}
