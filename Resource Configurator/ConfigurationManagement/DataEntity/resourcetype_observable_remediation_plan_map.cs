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
    
    public partial class resourcetype_observable_remediation_plan_map
    {
        public int ResourceTypeId { get; set; }
        public int ObservableId { get; set; }
        public int RemediationPlanId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int TenantId { get; set; }
        public Nullable<System.DateTime> ValidityStart { get; set; }
        public Nullable<System.DateTime> ValidityEnd { get; set; }
    
        public virtual observable observable { get; set; }
        public virtual remediation_plan remediation_plan { get; set; }
        public virtual resourcetype resourcetype { get; set; }
    }
}
