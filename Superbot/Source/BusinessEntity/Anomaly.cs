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
    public class Anomaly
    {
        public int ObservationId { get; set; }
        public string PlatformId { get; set; }
        public string ResourceId { get; set; }
        public int ResourceTypeId { get; set; }
        public int ObservableId { get; set; }
        public string ObservableName { get; set; }
        public string ObservationStatus { get; set; }
        public string Value { get; set; }
        public string ThresholdExpression { get; set; }
        public string ServerIp { get; set; }
        public string ObservationTime { get; set; }
        public string Descriptionn { get; set; }
        public string EventType { get; set; }
        public string Source { get; set; }
    }
}
