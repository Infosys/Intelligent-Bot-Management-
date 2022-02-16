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
    
    public class ResourceDS : ConfigurationManager.Resource.IDataAccess.IEntity<resource>
    {
        public ConfigurationDB dbCon;
        public ResourceDS()
        {
            dbCon = new ConfigurationDB();
        }
        public bool Delete(resource entity)
        {
            throw new NotImplementedException();
        }

        public IList<resource> GetAll()
        {
            //using (dbCon = new ConfigurationDB())
                return dbCon.resource.ToList();
        }

        public IList<resource> GetAll(resource Entity)
        {
            using (dbCon = new ConfigurationDB())
                return dbCon.resource.Where(r=>r.ResourceId==Entity.ResourceId && r.ResourceTypeId==Entity.ResourceTypeId).ToList();
        }

        public IQueryable<resource> GetAny()
        {
            //using (dbCon = new ConfigurationDB())            
                return dbCon.resource;
        }

        public resource GetOne(resource Entity)
        {
            using(dbCon=new ConfigurationDB())
            {
                Entity = (from c in dbCon.resource
                          where c.ResourceId == Entity.ResourceId
                          select c).FirstOrDefault();

                return Entity;
            }
            
        }

        public resource Insert(resource entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                if (entity.CreateDate == null || entity.CreateDate == DateTime.MinValue)
                {
                    entity.CreateDate = DateTime.UtcNow;
                }
                entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //entity.Priority to be used in future to set the order or execution

                dbCon.resource.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<resource> InsertBatch(IList<resource> entities)
        {
            using (dbCon = new ConfigurationDB())
            {
                foreach (resource entity in entities)
                {

                    if (entity.CreateDate == null)
                    {
                        entity.CreateDate = DateTime.UtcNow;
                    }
                    dbCon.resource.Add(entity);
                }

                dbCon.SaveChanges();
            }


            return entities;
        }

        public resource Update(resource entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                resource entityItem = dbCon.resource.Where(c => c.ResourceId == entity.ResourceId).FirstOrDefault();

                if (entityItem != null)
                {
                    dbCon.resource.Attach(entityItem);

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
                    EntityExtension<resource>.ApplyOnlyChanges(entityItem, entity);
                    dbCon.SaveChanges();
                }
            }
            return entity;
        }

        public IList<resource> UpdateBatch(IList<resource> entities)
        {
            using (dbCon = new ConfigurationDB())
            {
                foreach (resource entity in entities)
                {
                    resource entityItem = dbCon.resource.Where(c => c.ResourceId == entity.ResourceId).FirstOrDefault();

                    if (entityItem != null)
                    {
                        dbCon.resource.Attach(entityItem);

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
                        EntityExtension<resource>.ApplyOnlyChanges(entityItem, entity);
                        dbCon.SaveChanges();
                    }
                }
                dbCon.SaveChanges();
            }
            return entities;
        }
        public IQueryable<resource> GetResources()
        {
            IQueryable<resource> res;
            using (dbCon = new ConfigurationDB())
            {
                res = this.GetAny();
                
            }
            return res;
        }
                
    }
    public class ResourceDSExtended
    {
        public ConfigurationDB dbCon;
        public ResourceDSExtended()
        {
            dbCon = new ConfigurationDB();
        }
        public resource GetOne(resource entity)
        {
            using(dbCon = new ConfigurationDB())
            {
                entity = (from r in dbCon.resource
                          where r.ResourceTypeId == entity.ResourceTypeId
                          && r.PlatformId == entity.PlatformId
                          select r).FirstOrDefault();
            }
            return entity;
        }
        public List<resource> GetAll(resource entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                List<resource>  entityList = (from r in dbCon.resource
                          where r.ResourceTypeId == entity.ResourceTypeId
                          && r.PlatformId == entity.PlatformId
                          select r).ToList();

                return entityList;
            }            
        }
    }
}
