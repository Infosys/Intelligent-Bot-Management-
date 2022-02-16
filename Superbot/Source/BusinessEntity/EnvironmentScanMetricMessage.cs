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
    public class EnvironmentScanMetricMessage
    {
        public int ObservationId { get; set; }
        public int Version { get; set; }
        public int MetricId { get; set; }
        public string MetricName { get; set; }
        public string MetricKey { get; set; }        
        public List<MetricValue> MetricValue { get; set; }        
    }
    public class MetricValue
    {
        public string DisplayName { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
    }
}
