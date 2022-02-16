//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Infosys.Solutions.Superbot.Resource.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class remediation_plan_executions
    {
        public remediation_plan_executions()
        {
            this.remediation_plan_execution_actions = new HashSet<remediation_plan_execution_actions>();
        }
    
        public int RemediationPlanExecId { get; set; }
        public int RemediationPlanId { get; set; }
        public string ResourceId { get; set; }
        public int ObservableId { get; set; }
        public int ObservationId { get; set; }
        public string NodeDetails { get; set; }
        public string ExecutedBy { get; set; }
        public Nullable<System.DateTime> ExecutionStartDateTime { get; set; }
        public Nullable<System.DateTime> ExecutionEndDateTime { get; set; }
        public Nullable<int> ExecutionPercentage { get; set; }
        public string Status { get; set; }
        public Nullable<bool> IsNotified { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<bool> IsPicked { get; set; }
        public int TenantId { get; set; }
    
        public virtual ICollection<remediation_plan_execution_actions> remediation_plan_execution_actions { get; set; }
    }
}
