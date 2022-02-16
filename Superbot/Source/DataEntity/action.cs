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
    
    public partial class action
    {
        public action()
        {
            this.resourcetype_observable_action_map = new HashSet<resourcetype_observable_action_map>();
            this.remediation_plan_action_map = new HashSet<remediation_plan_action_map>();
            this.resource_observable_action_map = new HashSet<resource_observable_action_map>();
            this.Audit_Logs = new HashSet<Audit_Logs>();
        }
    
        public int ActionId { get; set; }
        public string ActionName { get; set; }
        public int ActionTypeId { get; set; }
        public string EndpointUri { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.DateTime> ValidityStart { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public Nullable<System.DateTime> ValidityEnd { get; set; }
        public int TenantId { get; set; }
        public Nullable<int> ScriptId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Nullable<int> AutomationEngineId { get; set; }
        public string AutomationEngineName { get; set; }
    
        public virtual ICollection<resourcetype_observable_action_map> resourcetype_observable_action_map { get; set; }
        public virtual ICollection<remediation_plan_action_map> remediation_plan_action_map { get; set; }
        public virtual ICollection<resource_observable_action_map> resource_observable_action_map { get; set; }
        public virtual ICollection<Audit_Logs> Audit_Logs { get; set; }
    }
}