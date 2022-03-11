//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Infosys.Solutions.ConfigurationManager.Resource.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class remediation_plan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public remediation_plan()
        {
            this.remediation_plan_action_map = new HashSet<remediation_plan_action_map>();
            this.resourcetype_observable_remediation_plan_map = new HashSet<resourcetype_observable_remediation_plan_map>();
            this.resource_observable_remediation_plan_map = new HashSet<resource_observable_remediation_plan_map>();
        }
    
        public int RemediationPlanId { get; set; }
        public string RemediationPlanName { get; set; }
        public string RemediationPlanDescription { get; set; }
        public bool IsUserDefined { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int TenantId { get; set; }
        public Nullable<bool> isDeleted { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<remediation_plan_action_map> remediation_plan_action_map { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<resourcetype_observable_remediation_plan_map> resourcetype_observable_remediation_plan_map { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<resource_observable_remediation_plan_map> resource_observable_remediation_plan_map { get; set; }
    }
}
