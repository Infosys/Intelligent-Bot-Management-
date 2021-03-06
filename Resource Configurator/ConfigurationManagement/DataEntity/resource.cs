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
    
    public partial class resource
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public resource()
        {
            this.observable_resource_map = new HashSet<observable_resource_map>();
            this.resource_observable_remediation_plan_map = new HashSet<resource_observable_remediation_plan_map>();
            this.resource_observable_action_map = new HashSet<resource_observable_action_map>();
            this.resource_observable_action_map1 = new HashSet<resource_observable_action_map>();
            this.Audit_Logs = new HashSet<Audit_Logs>();
            this.healthcheck_details = new HashSet<healthcheck_details>();
            this.resource_dependency_map = new HashSet<resource_dependency_map>();
            this.resource_attributes = new HashSet<resource_attributes>();
        }
    
        public string ResourceId { get; set; }
        public string ResourceName { get; set; }
        public int ResourceTypeId { get; set; }
        public string Source { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public System.DateTime ValidityStart { get; set; }
        public System.DateTime ValidityEnd { get; set; }
        public int TenantId { get; set; }
        public string ResourceRef { get; set; }
        public Nullable<int> PlatformId { get; set; }
        public string VersionNumber { get; set; }
        public string Comments { get; set; }
        public Nullable<bool> IsActive { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<observable_resource_map> observable_resource_map { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<resource_observable_remediation_plan_map> resource_observable_remediation_plan_map { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<resource_observable_action_map> resource_observable_action_map { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<resource_observable_action_map> resource_observable_action_map1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Audit_Logs> Audit_Logs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<healthcheck_details> healthcheck_details { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<resource_dependency_map> resource_dependency_map { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<resource_attributes> resource_attributes { get; set; }
    }
}
