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
    
    public partial class remediation_plan_execution_actions
    {
        public int RemediationPlanExecActionId { get; set; }
        public int RemediationPlanExecId { get; set; }
        public int RemediationPlanActionId { get; set; }
        public string CorrelationId { get; set; }
        public string Status { get; set; }
        public string Output { get; set; }
        public string Logdata { get; set; }
        public string OrchestratorDetails { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int TenantId { get; set; }
    
        public virtual remediation_plan_executions remediation_plan_executions { get; set; }
    }
}
