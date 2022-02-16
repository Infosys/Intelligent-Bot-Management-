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

namespace Infosys.Solutions.Ainauto.Services.Superbot.Contracts.Data
{
    public class ObservationDetails
    {
        public int ObservationId { get; set; }
        public int ObservableId { get; set; }
        public string ObservableName { get; set; }
        public string ObservationStatus { get; set; }
        public string Description { get; set; }
        public Nullable<DateTime> ObservationTime { get; set; }
        public int RemediationPlanId { get; set; }
        public string RemediationStatus { get; set; }
        public Nullable<DateTime> RemediationPlanTime { get; set; }
        public string ServerIp { get; set; }
        public string Source { get; set; }
        public Nullable<DateTime> AlertTime { get; set; }

    }
}
