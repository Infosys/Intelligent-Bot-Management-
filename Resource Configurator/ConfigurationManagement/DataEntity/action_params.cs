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
    
    public partial class action_params
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public action_params()
        {
            this.remdiation_plan_action_param_map = new HashSet<remdiation_plan_action_param_map>();
        }
    
        public int ParamId { get; set; }
        public string Name { get; set; }
        public string FieldToMap { get; set; }
        public Nullable<bool> IsMandatory { get; set; }
        public string DefaultValue { get; set; }
        public string ParamType { get; set; }
        public int ActionId { get; set; }
        public Nullable<int> AutomationEngineParamId { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public int TenantId { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<remdiation_plan_action_param_map> remdiation_plan_action_param_map { get; set; }
    }
}
