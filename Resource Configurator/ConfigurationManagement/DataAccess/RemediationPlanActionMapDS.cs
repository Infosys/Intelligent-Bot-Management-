/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using Infosys.Solutions.ConfigurationManager.Resource.Entity;
using System.Text;
using System.Threading.Tasks;
using Infosys.Solutions.ConfigurationManager.Resource.IDataAccess;

namespace Infosys.Solutions.Ainauto.Resource.DataAccess
{
    public class RemediationPlanActionMapDS : ConfigurationManager.Resource.IDataAccess.IEntity<remediation_plan_action_map>
    {
        public ConfigurationDB dbCon;

        public RemediationPlanActionMapDS()
        {
            dbCon = new ConfigurationDB();
        }

        public bool Delete(remediation_plan_action_map entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                entity = dbCon.remediation_plan_action_map.FirstOrDefault(rt => rt.RemediationPlanId == entity.RemediationPlanId
                         && rt.ActionId == entity.ActionId);

                //entity = CategoryWorkflowMapKeysExtension.GeneratePartitionKeyAndRowKey(entity);

                if (entity != null)
                {
                    dbCon.remediation_plan_action_map.Attach(entity);
                    dbCon.remediation_plan_action_map.Remove(entity);
                    dbCon.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public IList<remediation_plan_action_map> GetAll()
        {
            //using (dbCon = new ConfigurationDB())
            return dbCon.remediation_plan_action_map.ToList();
        }

        public IList<remediation_plan_action_map> GetAll(remediation_plan_action_map Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<remediation_plan_action_map> GetAny()
        {
            //using (dbCon = new ConfigurationDB())
            return dbCon.remediation_plan_action_map;
        }

        public remediation_plan_action_map GetOne(remediation_plan_action_map Entity)
        {
            throw new NotImplementedException();
        }

        public remediation_plan_action_map Insert(remediation_plan_action_map entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                if (entity.CreateDate == null || entity.CreateDate == DateTime.MinValue)
                {
                    entity.CreateDate = DateTime.UtcNow;
                }
                entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //entity.Priority to be used in future to set the order or execution

                dbCon.remediation_plan_action_map.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<remediation_plan_action_map> InsertBatch(IList<remediation_plan_action_map> entities)
        {
            throw new NotImplementedException();
        }

        public remediation_plan_action_map Update(remediation_plan_action_map entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                remediation_plan_action_map entityItem = dbCon.remediation_plan_action_map.
                                                                Where(c => c.RemediationPlanActionId == entity.RemediationPlanActionId).FirstOrDefault();

                if (entityItem != null)
                {
                    dbCon.remediation_plan_action_map.Attach(entityItem);

                    DateTime? lastModifiedOn = null;

                    lastModifiedOn = entityItem.ModifiedDate;

                    if (entity.ModifiedDate == null)
                    {
                        entity.ModifiedDate = DateTime.UtcNow;
                    }
                    if (lastModifiedOn == entity.ModifiedDate)
                    {
                        entity.ModifiedDate = DateTime.UtcNow;
                    }

                    //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                    EntityExtension<remediation_plan_action_map>.ApplyOnlyChanges(entityItem, entity);
                    dbCon.SaveChanges();
                }
            }

            return entity;
        }

        public IList<remediation_plan_action_map> UpdateBatch(IList<remediation_plan_action_map> entities)
        {
            using (dbCon = new ConfigurationDB())
            {
                foreach (remediation_plan_action_map entity in entities)
                {
                    remediation_plan_action_map entityItem = dbCon.remediation_plan_action_map.Where(c => c.RemediationPlanActionId == entity.RemediationPlanActionId).FirstOrDefault();



                    if (entityItem != null)
                    {
                        dbCon.remediation_plan_action_map.Attach(entityItem);

                        DateTime? lastModifiedOn = null;

                        lastModifiedOn = entityItem.ModifiedDate;

                        if (entity.ModifiedDate == null)
                        {
                            entity.ModifiedDate = DateTime.UtcNow;
                        }
                        if (lastModifiedOn == entity.ModifiedDate)
                        {
                            entity.ModifiedDate = DateTime.UtcNow;
                        }

                        EntityExtension<remediation_plan_action_map>.ApplyOnlyChanges(entityItem, entity);
                    }
                    //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                }
                dbCon.SaveChanges();
            }
            return entities;
        }
    }

}