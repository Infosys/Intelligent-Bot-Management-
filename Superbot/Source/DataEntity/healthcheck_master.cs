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
    
    public partial class healthcheck_master
    {
        public healthcheck_master()
        {
            this.healthcheck_details = new HashSet<healthcheck_details>();
            this.healthcheck_tracker = new HashSet<healthcheck_tracker>();
        }
    
        public string ConfigId { get; set; }
        public string ConfigurationName { get; set; }
        public string Description { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.DateTime> validitystart { get; set; }
        public Nullable<System.DateTime> validityend { get; set; }
        public int TenantId { get; set; }
    
        public virtual ICollection<healthcheck_details> healthcheck_details { get; set; }
        public virtual ICollection<healthcheck_tracker> healthcheck_tracker { get; set; }
    }
}