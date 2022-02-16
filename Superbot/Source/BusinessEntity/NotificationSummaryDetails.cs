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

namespace Infosys.Solutions.Ainauto.Superbot.BusinessEntity
{
    public class NotificationSummaryDetails
    {
        public string ResourceId { get; set; }
        public string ResourceName { get; set; }
        public string ObservableId { get; set; }
        public string ObservableName { get; set; }
        public string ResourceTypeId { get; set; }
        public string ResourceTypeName { get; set; }
        public string Value { get; set; }
        public string Status { get; set; }
        public DateTime ObservationTime { get; set; }
    }
}
