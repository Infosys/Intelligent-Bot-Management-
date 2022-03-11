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
    public class ResourceTypeDependencyMapDS : ConfigurationManager.Resource.IDataAccess.IEntity<resourcetype_dependency_map>
    {
        public ConfigurationDB dbCon;
        public ResourceTypeDependencyMapDS()
        {
            dbCon = new ConfigurationDB();
        }
        public bool Delete(resourcetype_dependency_map entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                int depResID = Convert.ToInt32(entity.DependencyResourceTypeId);
                var entities = dbCon.resourcetype_dependency_map.Where(rt => rt.DependencyResourceTypeId == entity.DependencyResourceTypeId 
                || rt.ResourcetypeId == depResID).ToList();
                if (entities != null)
                {
                    foreach (resourcetype_dependency_map entity1 in entities)
                    {
                        entity = entity1;
                        dbCon.resourcetype_dependency_map.Attach(entity);
                        dbCon.resourcetype_dependency_map.Remove(entity);
                    }
                    dbCon.SaveChanges();
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

        public IList<resourcetype_dependency_map> GetAll()
        {
            return dbCon.resourcetype_dependency_map.ToList();
        }

        public IList<resourcetype_dependency_map> GetAll(resourcetype_dependency_map Entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                var result = (from r in dbCon.resourcetype_dependency_map
                              where r.ResourcetypeId == Entity.ResourcetypeId
                              select r).ToList();

                return result;
            }
        }

        public IQueryable<resourcetype_dependency_map> GetAny()
        {
            return dbCon.resourcetype_dependency_map;
        }

        public resourcetype_dependency_map GetOne(resourcetype_dependency_map Entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                Entity = (from r in dbCon.resourcetype_dependency_map
                          where r.ResourcetypeId == Entity.ResourcetypeId
                          select r).FirstOrDefault();

                return Entity;
            }
        }

        public resourcetype_dependency_map Insert(resourcetype_dependency_map entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                if (entity.CreateDate == null || entity.CreateDate == DateTime.MinValue)
                {
                    entity.CreateDate = DateTime.UtcNow;
                }
                entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //entity.Priority to be used in future to set the order or execution

                dbCon.resourcetype_dependency_map.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<resourcetype_dependency_map> InsertBatch(IList<resourcetype_dependency_map> entities)
        {
            using (dbCon = new ConfigurationDB())
            {
                foreach (resourcetype_dependency_map entity in entities)
                {

                    if (entity.CreateDate == null || entity.CreateDate == DateTime.MinValue)
                    {
                        entity.CreateDate = DateTime.UtcNow;
                    }
                    entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    dbCon.resourcetype_dependency_map.Add(entity);
                }

                dbCon.SaveChanges();
            }

            return entities;
        }

        public resourcetype_dependency_map Update(resourcetype_dependency_map entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                resourcetype_dependency_map entityItem = dbCon.resourcetype_dependency_map.Where(c => c.ResourcetypeId == entity.ResourcetypeId 
                && c.DependencyResourceTypeId==entity.DependencyResourceTypeId).FirstOrDefault();

                if (entityItem != null)
                {
                    dbCon.resourcetype_dependency_map.Attach(entityItem);

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
                    entity.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                    EntityExtension<resourcetype_dependency_map>.ApplyOnlyChanges(entityItem, entity);
                    dbCon.SaveChanges();
                }
            }
            return entity;
        }

        public IList<resourcetype_dependency_map> UpdateBatch(IList<resourcetype_dependency_map> entities)
        {
            throw new NotImplementedException();
        }
    }
}
