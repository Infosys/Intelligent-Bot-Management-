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

namespace Infosys.Solutions.Ainauto.Resource.DataAccess
{
    public class RemediationPlanDS : ConfigurationManager.Resource.IDataAccess.IEntity<remediation_plan>
    {
        public ConfigurationDB dbCon;
        public RemediationPlanDS()
        {
            dbCon = new ConfigurationDB();
        }
        public bool Delete(remediation_plan entity)
        {
            throw new NotImplementedException();
        }

        public IList<remediation_plan> GetAll()
        {
            //using (dbCon = new ConfigurationDB())
                return dbCon.remediation_plan.ToList();
        }

        public IList<remediation_plan> GetAll(remediation_plan Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<remediation_plan> GetAny()
        {
            //using (dbCon = new ConfigurationDB())
                return dbCon.remediation_plan;
        }

        public remediation_plan GetOne(remediation_plan Entity)
        {
            throw new NotImplementedException();
        }

        public remediation_plan Insert(remediation_plan entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                if (entity.CreateDate == null || entity.CreateDate == DateTime.MinValue)
                {
                    entity.CreateDate = DateTime.UtcNow;
                }
                entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //entity.Priority to be used in future to set the order or execution

                dbCon.remediation_plan.Add(entity);
                
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<remediation_plan> InsertBatch(IList<remediation_plan> entities)
        {
            throw new NotImplementedException();
        }

        public remediation_plan Update(remediation_plan entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                remediation_plan entityItem = dbCon.remediation_plan.
                                                                Where( c => c.RemediationPlanId == entity.RemediationPlanId).FirstOrDefault();

                if (entityItem != null)
                {
                    dbCon.remediation_plan.Attach(entityItem);

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
                    EntityExtension<remediation_plan>.ApplyOnlyChanges(entityItem, entity);
                    dbCon.SaveChanges();
                }
            }

            return entity;
        }

        public IList<remediation_plan> UpdateBatch(IList<remediation_plan> entities)
        {
            using (dbCon = new ConfigurationDB())
            {
                foreach (remediation_plan entity in entities)
                {
                    remediation_plan entityItem = dbCon.remediation_plan.Where(c => c.RemediationPlanId == entity.RemediationPlanId).FirstOrDefault();



                    if (entityItem != null)
                    {
                        dbCon.remediation_plan.Attach(entityItem);

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

                        EntityExtension<remediation_plan>.ApplyOnlyChanges(entityItem, entity);
                    }
                    //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                }
                dbCon.SaveChanges();
            }
            return entities;
        }
    }
}
