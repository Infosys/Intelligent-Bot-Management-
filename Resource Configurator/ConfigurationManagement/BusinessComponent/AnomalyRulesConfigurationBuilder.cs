/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infosys.Solutions.Ainauto.Resource.DataAccess;
using Infosys.Solutions.ConfigurationManager.Resource.Entity;
using Infosys.Solutions.Ainauto.ConfigurationManager.BusinessEntity;

namespace Infosys.Solutions.Ainauto.ConfigurationManager.BusinessComponent
{
    public class AnomalyRulesConfigurationBuilder
    {
        public AnomalyRulesDetails GetAnomalyRulesConfig(int observableId, int resourcetypeId, int platformId, int tenantId)
        {
            ObservableResourceMapDS observableResourceMapDS = new ObservableResourceMapDS();
            ObservableDS observableDS = new ObservableDS();
            ResourceDS resourceDS = new ResourceDS();
            ResourceTypeDS resourceTypeDS = new ResourceTypeDS();
            OperatorDS operatorDS = new OperatorDS();



            try
            {
                var observableResourceMapTable = observableResourceMapDS.GetAll();
                var observableTable = observableDS.GetAll();
                var resourceTable = resourceDS.GetAll();
                var resourceTypeTable = resourceTypeDS.GetAll();
                var operatorTable = operatorDS.GetAll();

                AnomalyRulesDetails anomalyRulesDetails = new AnomalyRulesDetails();                

                List<AnomalyDetectionRule> anomalyDetectionRule = (from orm in observableResourceMapTable
                                       join obs in observableTable
                                       on orm.ObservableId equals obs.ObservableId
                                       join res in resourceTable
                                       on orm.ResourceId equals res.ResourceId
                                       join op in operatorTable
                                       on orm.OperatorId equals op.OperatorId.ToString().Trim()
                                       join rt in resourceTypeTable
                                       on res.ResourceTypeId equals rt.ResourceTypeId
                                       where
                                       orm.TenantId == tenantId &&
                                       res.PlatformId == platformId &&
                                       orm.ObservableId == observableId &&
                                       rt.ResourceTypeId == resourcetypeId &&
                                       orm.TenantId == obs.TenantId &&
                                       orm.TenantId == res.TenantId &&
                                       res.TenantId == rt.TenantId &&
                                       //res.PlatformId == rt.PlatformId &&
                                       orm.ValidityStart <= DateTime.Now && orm.ValidityEnd > DateTime.Now &&
                                       obs.ValidityStart <= DateTime.Now && obs.ValidityEnd > DateTime.Now &&
                                       res.ValidityStart <= DateTime.Now && res.ValidityEnd > DateTime.Now
                                   select new AnomalyDetectionRule
                                       { ObservableId = orm.ObservableId,
                                       ResourceId = orm.ResourceId,
                                       OperatorId = Convert.ToInt32(orm.OperatorId),
                                       UpperThreshold = orm.UpperThreshold,
                                       LowerThreshold = orm.LowerThreshold,
                                       ObservableName = obs.ObservableName,
                                       ResourceName = res.ResourceName,
                                       ResourceTypeId = rt.ResourceTypeId,
                                       ResourceTypeName = rt.ResourceTypeName,
                                       Operator = op.Operator1,
                                       LogDetails=new LogDetail
                                       {
                                           CreatedBy = orm.CreatedBy,
                                           CreateDate = orm.CreateDate,
                                           ModifiedBy = orm.ModifiedBy,
                                           ModifiedDate = orm.ModifiedDate,
                                           ValidityStart = orm.ValidityStart,
                                           ValidityEnd = orm.ValidityEnd
                                       }
                                       
                                   }).ToList();
                anomalyRulesDetails.AnomalyDetectionRules = anomalyDetectionRule;
                anomalyRulesDetails.TenantId = tenantId;
                anomalyRulesDetails.PlatformId = platformId;

                return anomalyRulesDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string InsertAnomalyRulesConfig(AnomalyRulesDetails anomalyRulesDetails)
        {            
            ObservableResourceMapDS observableResourceMapDS = new ObservableResourceMapDS();

            List<observable_resource_map> observable_Resource_Map_List = new List<observable_resource_map>();
            try
            {
                foreach (AnomalyDetectionRule anomalyDetectionRule in anomalyRulesDetails.AnomalyDetectionRules)
                {
                    observable_resource_map observable_Resource_Map = new observable_resource_map();

                    observable_Resource_Map.TenantId = anomalyRulesDetails.TenantId;
                    observable_Resource_Map.ObservableId = anomalyDetectionRule.ObservableId;
                    observable_Resource_Map.ResourceId = anomalyDetectionRule.ResourceId;
                    observable_Resource_Map.OperatorId = anomalyDetectionRule.OperatorId.ToString();
                    observable_Resource_Map.LowerThreshold = anomalyDetectionRule.LowerThreshold;
                    observable_Resource_Map.UpperThreshold = anomalyDetectionRule.UpperThreshold;
                    observable_Resource_Map.ValidityStart = anomalyDetectionRule.LogDetails.ValidityStart;
                    observable_Resource_Map.ValidityEnd = anomalyDetectionRule.LogDetails.ValidityEnd;
                    observable_Resource_Map.CreatedBy = anomalyDetectionRule.LogDetails.CreatedBy;
                    observable_Resource_Map.CreateDate = anomalyDetectionRule.LogDetails.CreateDate;
                    observable_Resource_Map.ModifiedBy = anomalyDetectionRule.LogDetails.ModifiedBy;
                    observable_Resource_Map.ModifiedDate = anomalyDetectionRule.LogDetails.ModifiedDate;

                    observable_Resource_Map_List.Add(observable_Resource_Map);
                }

                var retVal = observableResourceMapDS.InsertBatch(observable_Resource_Map_List);

                return (retVal != null && retVal.Count == observable_Resource_Map_List.Count) ? "Successfully Inserted" : "Insertion Failed";
            }
            catch (Exception ex)
            {
                throw ex;
            }            
        }

        public string UpdateAnomalyRuleConfig(AnomalyRulesDetails anomalyRulesDetails)
        {
            StringBuilder returnString = new StringBuilder();
            ObservableResourceMapDS observableResourceMapDS = new ObservableResourceMapDS();
           
            try
            {                
                foreach (AnomalyDetectionRule anomalyDetectionRule in anomalyRulesDetails.AnomalyDetectionRules)
                {
                    observable_resource_map observable_Resource_Map = new observable_resource_map();

                    observable_Resource_Map.ObservableId = anomalyDetectionRule.ObservableId;
                    observable_Resource_Map.ResourceId = anomalyDetectionRule.ResourceId;
                    observable_Resource_Map.OperatorId = anomalyDetectionRule.OperatorId.ToString();
                    observable_Resource_Map.LowerThreshold = anomalyDetectionRule.LowerThreshold;
                    observable_Resource_Map.UpperThreshold = anomalyDetectionRule.UpperThreshold;
                    observable_Resource_Map.CreatedBy = anomalyDetectionRule.LogDetails.CreatedBy;
                    observable_Resource_Map.CreateDate = anomalyDetectionRule.LogDetails.CreateDate;
                    observable_Resource_Map.ModifiedBy = anomalyDetectionRule.LogDetails.ModifiedBy;
                    observable_Resource_Map.ModifiedDate = anomalyDetectionRule.LogDetails.ModifiedDate;
                    observable_Resource_Map.ValidityStart = anomalyDetectionRule.LogDetails.ValidityStart;
                    observable_Resource_Map.ValidityEnd = anomalyDetectionRule.LogDetails.ValidityEnd;
                    observable_Resource_Map.TenantId = anomalyRulesDetails.TenantId;

                    var res = observableResourceMapDS.Update(observable_Resource_Map);
                    returnString.AppendLine(res==null?"Failed to update details for Observable ID:"+ anomalyDetectionRule.ObservableId +", Resource ID:"+ anomalyDetectionRule.ResourceId: "Updation successful for Observable ID:" + anomalyDetectionRule.ObservableId + ", Resource ID:" + anomalyDetectionRule.ResourceId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return returnString.ToString();
        }

        public string DeleteAnomalyRuleConfig(AnomalyRulesDetails anomalyRulesDetails)
        {
            StringBuilder returnString = new StringBuilder();
            ObservableResourceMapDS observableResourceMapDS = new ObservableResourceMapDS();

            try
            {
                foreach (AnomalyDetectionRule anomalyDetectionRule in anomalyRulesDetails.AnomalyDetectionRules)
                {
                    observable_resource_map observable_Resource_Map = new observable_resource_map();

                    observable_Resource_Map.ObservableId = anomalyDetectionRule.ObservableId;
                    observable_Resource_Map.ResourceId = anomalyDetectionRule.ResourceId;
                    //observable_Resource_Map.OperatorId = anomalyDetectionRule.OperatorId.ToString();
                    //observable_Resource_Map.LowerThreshold = anomalyDetectionRule.LowerThreshold;
                    //observable_Resource_Map.UpperThreshold = anomalyDetectionRule.UpperThreshold;
                    //observable_Resource_Map.CreatedBy = anomalyDetectionRule.LogDetails.CreatedBy;
                    //observable_Resource_Map.CreateDate = anomalyDetectionRule.LogDetails.CreateDate;
                    //observable_Resource_Map.ModifiedBy = anomalyDetectionRule.LogDetails.ModifiedBy;
                    //observable_Resource_Map.ModifiedDate = anomalyDetectionRule.LogDetails.ModifiedDate;
                    //observable_Resource_Map.ValidityStart = anomalyDetectionRule.LogDetails.ValidityStart;
                    //observable_Resource_Map.ValidityEnd = anomalyDetectionRule.LogDetails.ValidityEnd;
                    observable_Resource_Map.TenantId = anomalyRulesDetails.TenantId;

                    var res = observableResourceMapDS.Delete(observable_Resource_Map);
                    returnString.AppendLine(res == false ? "Failed to Delete details for Observable ID:" + anomalyDetectionRule.ObservableId + ", Resource ID:" + anomalyDetectionRule.ResourceId : "Deletion successful for Observable ID:" + anomalyDetectionRule.ObservableId + ", Resource ID:" + anomalyDetectionRule.ResourceId);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return returnString.ToString();
        }
    }
}

