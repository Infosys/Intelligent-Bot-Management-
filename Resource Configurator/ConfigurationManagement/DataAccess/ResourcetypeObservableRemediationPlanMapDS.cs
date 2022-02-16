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
    public class ResourcetypeObservableRemediationPlanMapDS : ConfigurationManager.Resource.IDataAccess.IEntity<resourcetype_observable_remediation_plan_map>
    {
        public ConfigurationDB dbcon;
        public ResourcetypeObservableRemediationPlanMapDS()
        {
            dbcon = new ConfigurationDB();
        }
        public bool Delete(resourcetype_observable_remediation_plan_map entity)
        {
            using (dbcon = new ConfigurationDB())
            {
                //int depResID = Convert.ToInt32(entity.DependencyResourceTypeId);
                var entities = dbcon.resourcetype_observable_remediation_plan_map.Where(rt => rt.ObservableId == entity.ObservableId
                && rt.RemediationPlanId == entity.RemediationPlanId 
                && rt.ResourceTypeId == entity.ResourceTypeId).ToList();
                if (entities != null)
                {
                    foreach (resourcetype_observable_remediation_plan_map entity1 in entities)
                    {
                        entity = entity1;
                        dbcon.resourcetype_observable_remediation_plan_map.Attach(entity);
                        dbcon.resourcetype_observable_remediation_plan_map.Remove(entity); 



                    }
                    dbcon.SaveChanges();
                    return true;
                }

                /*entity = dbCon.resourcetype_dependency_map.FirstOrDefault(rt => rt.DependencyResourceTypeId == entity.DependencyResourceTypeId || Convert.ToString(rt.ResourcetypeId) == entity.DependencyResourceTypeId);
                if (entity != null)
                {
                    dbCon.resourcetype_dependency_map.Attach(entity);
                    dbCon.resourcetype_dependency_map.Remove(entity);
                    dbCon.SaveChanges();
                    return true;
                }*/

            }
            return false;
        }

        public IList<resourcetype_observable_remediation_plan_map> GetAll()
        {
            return dbcon.resourcetype_observable_remediation_plan_map.ToList();
        }

        public IList<resourcetype_observable_remediation_plan_map> GetAll(resourcetype_observable_remediation_plan_map Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<resourcetype_observable_remediation_plan_map> GetAny()
        {
            //using (dbcon = new ConfigurationDB())
                return dbcon.resourcetype_observable_remediation_plan_map;
        }

        public resourcetype_observable_remediation_plan_map GetOne(resourcetype_observable_remediation_plan_map Entity)
        {
            throw new NotImplementedException();
        }

        public resourcetype_observable_remediation_plan_map Insert(resourcetype_observable_remediation_plan_map entity)
        {
            using (dbcon = new ConfigurationDB())
            {
                if (entity.CreateDate == null || entity.CreateDate == DateTime.MinValue)
                {
                    entity.CreateDate = DateTime.UtcNow;
                }
                entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //entity.Priority to be used in future to set the order or execution

                dbcon.resourcetype_observable_remediation_plan_map.Add(entity);
                dbcon.SaveChanges();
            }

            return entity;
        }

        public IList<resourcetype_observable_remediation_plan_map> InsertBatch(IList<resourcetype_observable_remediation_plan_map> entities)
        {
            throw new NotImplementedException();
        }

        public resourcetype_observable_remediation_plan_map Update(resourcetype_observable_remediation_plan_map entity)
        {
            using (dbcon = new ConfigurationDB())
            {
                resourcetype_observable_remediation_plan_map entityItem = dbcon.resourcetype_observable_remediation_plan_map.
                                                                            Where(c => c.ResourceTypeId == entity.ResourceTypeId &&
                                                                                        c.ObservableId == entity.ObservableId 
                                                                                        ).FirstOrDefault();

               if(entityItem!=null)
                {
                    dbcon.resourcetype_observable_remediation_plan_map.Attach(entityItem);

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
                    EntityExtension<resourcetype_observable_remediation_plan_map>.ApplyOnlyChanges(entityItem, entity);
                    dbcon.SaveChanges();
                }
            }

            return entity;
        }

        public IList<resourcetype_observable_remediation_plan_map> UpdateBatch(IList<resourcetype_observable_remediation_plan_map> entities)
        {
            throw new NotImplementedException();
        }
    }
}
