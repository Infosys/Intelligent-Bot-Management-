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
    public class ResourceDependencyMapDS : ConfigurationManager.Resource.IDataAccess.IEntity<resource_dependency_map>
    {
        public ConfigurationDB dbCon;
        public ResourceDependencyMapDS()
        {
            dbCon = new ConfigurationDB();
        }
        public bool Delete(resource_dependency_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<resource_dependency_map> GetAll()
        {
            //using (dbCon = new ConfigurationDB())
                return dbCon.resource_dependency_map.ToList();
        }

        public IList<resource_dependency_map> GetAll(resource_dependency_map Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<resource_dependency_map> GetAny()
        {
            //using (dbCon = new ConfigurationDB())
                return dbCon.resource_dependency_map;
        }

        public resource_dependency_map GetOne(resource_dependency_map Entity)
        {
            throw new NotImplementedException();
        }

        public resource_dependency_map Insert(resource_dependency_map entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                if (entity.CreateDate == null || entity.CreateDate == DateTime.MinValue)
                {
                    entity.CreateDate = DateTime.UtcNow;
                }
               
                //entity.Priority to be used in future to set the order or execution

                dbCon.resource_dependency_map.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<resource_dependency_map> InsertBatch(IList<resource_dependency_map> entities)
        {
            using (dbCon = new ConfigurationDB())
            {
                foreach (resource_dependency_map entity in entities)
                {

                    if (entity.CreateDate == null)
                    {
                        entity.CreateDate = DateTime.UtcNow;
                    }
                    dbCon.resource_dependency_map.Add(entity);
                }

                dbCon.SaveChanges();
            }

            return entities;
        }

        public resource_dependency_map Update(resource_dependency_map entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                resource_dependency_map entityItem = dbCon.resource_dependency_map.Where(c => c.ResourceId == entity.ResourceId).FirstOrDefault();

                if (entityItem != null)
                {
                    dbCon.resource_dependency_map.Attach(entityItem);

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
                    EntityExtension<resource_dependency_map>.ApplyOnlyChanges(entityItem, entity);
                    dbCon.SaveChanges();
                }
            }
            return entity;
        }

        public IList<resource_dependency_map> UpdateBatch(IList<resource_dependency_map> entities)
        {
            throw new NotImplementedException();
        }
    }
    public class ResourceDependencyMapDSExtn
    {
        public ConfigurationDB dbCon;
        public ResourceDependencyMapDSExtn()
        {
            dbCon = new ConfigurationDB();
        }
        public IList<GetHierarchyResources_Result> HierarchyResources(string resID,string tenantID)
        {
            return dbCon.GetHierarchyResources(resID, tenantID).ToList();
        }
    }
}
