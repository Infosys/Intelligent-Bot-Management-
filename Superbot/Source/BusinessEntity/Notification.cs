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
    public class Notification
    {
        public string TenantId { get; set; }
        public string TenantName { get; set; } //no
        public string PlatformId { get; set; }
        public string PlatformName { get; set; } //n
        public string ResourceId { get; set; }
        public string ResourceName { get; set; }//n
        public string ResourceTypeId { get; set; }
        public string ResourceTypeName { get; set; }
        public string ObservableId { get; set; }
        public string ObservableName { get; set; }
        public string ObservationId { get; set; }
        public string ObservationStatus { get; set; }
        public string ObservationTime { get; set; }
        public string AnomalyReason { get; set; }
        public string RemediationPlanName { get; set; }
        public string RemediationPlanStatus { get; set; }
        public string RemediationPlanTime { get; set; }
        //public List<ActionDetails> ActionDetails { get; set; }
        public string Description { get; set; }
        public string HostName { get; set; }
        public string ServerIp { get; set; }
        public string Source { get; set; }
        //public string ThresholdExpression { get; set; }
        public string AlertTime { get; set; }
        public string NotificationSender { get; set; }

    }
    public class ActionDetails
    {
        public int ActionId { get; set; }
        public string ActionName { get; set; }
        public string ActionStageId { get; set; }
        public int ActionSequence { get; set; }
    }
}
