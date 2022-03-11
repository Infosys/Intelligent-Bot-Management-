/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.Solutions.ConfigurationManager.Resource.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.Solutions.Ainauto.Resource.DataAccess
{
    public class ResourceTypeDS : ConfigurationManager.Resource.IDataAccess.IEntity<resourcetype>
    {
        public ConfigurationDB dbCon;
        public ResourceTypeDS()
        {
            dbCon = new ConfigurationDB();
        }
        public bool Delete(resourcetype entity)
        {
            throw new NotImplementedException();
        }

        public IList<resourcetype> GetAll()
        {
            //using (dbCon = new ConfigurationDB())
                return dbCon.resourcetype.ToList();
        }

        public IList<resourcetype> GetAll(resourcetype Entity)
        {
            using (dbCon = new ConfigurationDB())
                return dbCon.resourcetype.Where(r=>r.ResourceTypeName==Entity.ResourceTypeName).ToList();
        }

        public IQueryable<resourcetype> GetAny()
        {
            //using (dbCon = new SuperbotDB())
                return dbCon.resourcetype;
        }

        public resourcetype GetOne(resourcetype Entity)
        {
            Entity = (from c in dbCon.resourcetype
                      where c.ResourceTypeId == Entity.ResourceTypeId
                      select c).FirstOrDefault();

            return Entity;
        }

        public resourcetype Insert(resourcetype entity)
        {
            throw new NotImplementedException();
        }

        public IList<resourcetype> InsertBatch(IList<resourcetype> entities)
        {
            throw new NotImplementedException();
        }

        public resourcetype Update(resourcetype entity)
        {
            throw new NotImplementedException();
        }

        public IList<resourcetype> UpdateBatch(IList<resourcetype> entities)
        {
            using (dbCon = new ConfigurationDB())
            {
                foreach (resourcetype entity in entities)
                {
                    resourcetype entityItem = dbCon.resourcetype.Where(c => c.ResourceTypeId == entity.ResourceTypeId).FirstOrDefault();

                    /*resource_attributes entityItem = dbCon.resource_attributes.Single(c => c.ResourceId == entity.ResourceId &&
                                                     c.AttributeName == entity.AttributeName);*/

                    if (entityItem != null)
                    {
                        dbCon.resourcetype.Attach(entityItem);

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

                        EntityExtension<resourcetype>.ApplyOnlyChanges(entityItem, entity);
                    }
                    //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                }
                dbCon.SaveChanges();
            }
            return entities;
        }

        public IQueryable<resourcetype> GetResourceTypes()
        {
            IQueryable<resourcetype> res;
            using (dbCon = new ConfigurationDB())
            {
                res = this.GetAny();

            }
            return res;
        }       
    }

    public class ResourceTypeDSEntended
    {
        public ConfigurationDB dbCon;
        public ResourceTypeDSEntended()
        {
            dbCon = new ConfigurationDB();
        }

        public resourcetype GetOne(resourcetype entity)
        {
            using (dbCon=new ConfigurationDB())
            {
                entity = (from r in dbCon.resourcetype
                          where r.ResourceTypeName.Replace(" ","").ToLower() == entity.ResourceTypeName.Replace(" ", "").ToLower()
                          && r.PlatfromType.Replace(" ", "").ToLower() == entity.PlatfromType.Replace(" ", "").ToLower()
                          select r).FirstOrDefault();

            }
            return entity;
        }
    }
}
