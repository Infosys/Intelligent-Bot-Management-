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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class ConfigurationDB : DbContext
    {
        public ConfigurationDB()
            : base("name=ConfigurationDB")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<action> action { get; set; }
        public virtual DbSet<action_params> action_params { get; set; }
        public virtual DbSet<actiontype> actiontype { get; set; }
        public virtual DbSet<anomaly_details> anomaly_details { get; set; }
        public virtual DbSet<healthcheck_iteration_tracker> healthcheck_iteration_tracker { get; set; }
        public virtual DbSet<healthcheck_iteration_tracker_details> healthcheck_iteration_tracker_details { get; set; }
        public virtual DbSet<notification_configuration> notification_configuration { get; set; }
        public virtual DbSet<observable> observable { get; set; }
        public virtual DbSet<observable_resource_map> observable_resource_map { get; set; }
        public virtual DbSet<observations> observations { get; set; }
        public virtual DbSet<@operator> @operator { get; set; }
        public virtual DbSet<recipient_configuration> recipient_configuration { get; set; }
        public virtual DbSet<remediation_plan> remediation_plan { get; set; }
        public virtual DbSet<remediation_plan_action_map> remediation_plan_action_map { get; set; }
        public virtual DbSet<remediation_plan_execution_actions> remediation_plan_execution_actions { get; set; }
        public virtual DbSet<remediation_plan_executions> remediation_plan_executions { get; set; }
        public virtual DbSet<resource> resource { get; set; }
        public virtual DbSet<resource_observable_action_map> resource_observable_action_map { get; set; }
        public virtual DbSet<resource_observable_remediation_plan_map> resource_observable_remediation_plan_map { get; set; }
        public virtual DbSet<resourcetype> resourcetype { get; set; }
        public virtual DbSet<resourcetype_observable_action_map> resourcetype_observable_action_map { get; set; }
        public virtual DbSet<resourcetype_observable_remediation_plan_map> resourcetype_observable_remediation_plan_map { get; set; }
        public virtual DbSet<remdiation_plan_action_param_map> remdiation_plan_action_param_map { get; set; }
        public virtual DbSet<Audit_Logs> Audit_Logs { get; set; }
        public virtual DbSet<Environment_Scan_Metric> Environment_Scan_Metric { get; set; }
        public virtual DbSet<Environment_Scan_Metric_Anomaly_Details> Environment_Scan_Metric_Anomaly_Details { get; set; }
        public virtual DbSet<Environment_Scan_Metric_Details> Environment_Scan_Metric_Details { get; set; }
        public virtual DbSet<errors> errors { get; set; }
        public virtual DbSet<tickets> tickets { get; set; }
        public virtual DbSet<platforms> platforms { get; set; }
        public virtual DbSet<resourcetype_metadata> resourcetype_metadata { get; set; }
        public virtual DbSet<resourcetype_observable_map> resourcetype_observable_map { get; set; }
        public virtual DbSet<resourcetype_service_details> resourcetype_service_details { get; set; }
        public virtual DbSet<resourcetype_dependency_map> resourcetype_dependency_map { get; set; }
        public virtual DbSet<healthcheck_details> healthcheck_details { get; set; }
        public virtual DbSet<healthcheck_master> healthcheck_master { get; set; }
        public virtual DbSet<healthcheck_tracker> healthcheck_tracker { get; set; }
        public virtual DbSet<resource_dependency_map> resource_dependency_map { get; set; }
        public virtual DbSet<actionstages> actionstages { get; set; }
        public virtual DbSet<resource_attributes> resource_attributes { get; set; }
        public virtual DbSet<tenant> tenant { get; set; }
        public virtual DbSet<portfolio_project_map> portfolio_project_map { get; set; }
    
        public virtual ObjectResult<GetHierarchyResources_Result> GetHierarchyResources(string resourceID, string tenantID)
        {
            var resourceIDParameter = resourceID != null ?
                new ObjectParameter("resourceID", resourceID) :
                new ObjectParameter("resourceID", typeof(string));
    
            var tenantIDParameter = tenantID != null ?
                new ObjectParameter("tenantID", tenantID) :
                new ObjectParameter("tenantID", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<GetHierarchyResources_Result>("GetHierarchyResources", resourceIDParameter, tenantIDParameter);
        }
    }
}
