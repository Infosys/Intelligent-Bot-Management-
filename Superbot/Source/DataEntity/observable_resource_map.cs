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
    
    public partial class observable_resource_map
    {
        public int ObservableId { get; set; }
        public string ResourceId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public System.DateTime ValidityStart { get; set; }
        public System.DateTime ValidityEnd { get; set; }
        public int TenantId { get; set; }
        public string OperatorId { get; set; }
        public string LowerThreshold { get; set; }
        public string UpperThreshold { get; set; }
    
        public virtual observable observable { get; set; }
        public virtual resource resource { get; set; }
    }
}
