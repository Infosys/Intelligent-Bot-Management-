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
    
    public partial class healthcheck_details
    {
        public string ConfigId { get; set; }
        public string ResourceId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.DateTime> validitystart { get; set; }
        public Nullable<System.DateTime> validityend { get; set; }
        public int TenantId { get; set; }
    
        public virtual healthcheck_master healthcheck_master { get; set; }
        public virtual resource resource { get; set; }
    }
}
