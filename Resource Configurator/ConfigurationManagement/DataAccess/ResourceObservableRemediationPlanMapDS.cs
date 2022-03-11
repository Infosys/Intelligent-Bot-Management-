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
using Infosys.Solutions.ConfigurationManager.Resource.Entity;

namespace Infosys.Solutions.Ainauto.Resource.DataAccess
{
    public class ResourceObservableRemediationPlanMapDS : ConfigurationManager.Resource.IDataAccess.IEntity<resource_observable_remediation_plan_map>
    {
        public ConfigurationDB dbcon;
        public ResourceObservableRemediationPlanMapDS()
        {
            dbcon = new ConfigurationDB();
        }
        public bool Delete(resource_observable_remediation_plan_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<resource_observable_remediation_plan_map> GetAll()
        {
            return dbcon.resource_observable_remediation_plan_map.ToList();
        }

        public IList<resource_observable_remediation_plan_map> GetAll(resource_observable_remediation_plan_map Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<resource_observable_remediation_plan_map> GetAny()
        {
            return dbcon.resource_observable_remediation_plan_map;
        }

        public resource_observable_remediation_plan_map GetOne(resource_observable_remediation_plan_map Entity)
        {
            throw new NotImplementedException();
        }

        public resource_observable_remediation_plan_map Insert(resource_observable_remediation_plan_map entity)
        {
            using (dbcon = new ConfigurationDB())
            {
                if (entity.CreateDate == null || entity.CreateDate == DateTime.MinValue)
                {
                    entity.CreateDate = DateTime.UtcNow;
                }
                entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //entity.Priority to be used in future to set the order or execution

                dbcon.resource_observable_remediation_plan_map.Add(entity);
                dbcon.SaveChanges();
            }

            return entity;
        }

        public IList<resource_observable_remediation_plan_map> InsertBatch(IList<resource_observable_remediation_plan_map> entities)
        {
            throw new NotImplementedException();
        }

        public resource_observable_remediation_plan_map Update(resource_observable_remediation_plan_map entity)
        {
            using (dbcon = new ConfigurationDB())
            {
                resource_observable_remediation_plan_map entityItem = dbcon.resource_observable_remediation_plan_map.Where(c => c.ResourceId == entity.ResourceId &&
                                                                            c.ObservableId == entity.ObservableId &&
                                                                            c.RemediationPlanId == entity.RemediationPlanId).FirstOrDefault();

                if (entityItem != null)
                {
                    dbcon.resource_observable_remediation_plan_map.Attach(entityItem);

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
                    EntityExtension<resource_observable_remediation_plan_map>.ApplyOnlyChanges(entityItem, entity);
                    dbcon.SaveChanges();
                }
            }
            return entity;

        }

        public IList<resource_observable_remediation_plan_map> UpdateBatch(IList<resource_observable_remediation_plan_map> entities)
        {
            throw new NotImplementedException();
        }
    }
}
