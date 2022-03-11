using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infosys.Solutions.Ainauto.Superbot.Models
{
    public class NotificationTransactionData
    {
        public int ObservationId { get; set; }
        public string ObservableName { get; set; }
        public string ObservationStatus { get; set; }
        public Nullable<DateTime> ObservationTime { get; set; }
        public int RemediationPlanId { get; set; }
        public string RemediationPlanName { get; set; }
        public string RemediationStatus { get; set; }
        public Nullable<DateTime> ExecutionEndDateTime { get; set; }
    }
}