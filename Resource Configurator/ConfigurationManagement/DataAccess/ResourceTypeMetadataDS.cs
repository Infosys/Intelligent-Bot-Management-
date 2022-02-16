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
    public class ResourceTypeMetadataDS : ConfigurationManager.Resource.IDataAccess.IEntity<resourcetype_metadata>
    {
        public ConfigurationDB dbCon;
        public ResourceTypeMetadataDS()
        {
            dbCon = new ConfigurationDB();
        }
        public bool Delete(resourcetype_metadata entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                entity = dbCon.resourcetype_metadata.FirstOrDefault(rt => rt.ResourceTypeId == entity.ResourceTypeId
                         && rt.AttributeName==entity.AttributeName);

                //entity = CategoryWorkflowMapKeysExtension.GeneratePartitionKeyAndRowKey(entity);

                if (entity != null)
                {
                    dbCon.resourcetype_metadata.Attach(entity);
                    dbCon.resourcetype_metadata.Remove(entity);
                    dbCon.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public IList<resourcetype_metadata> GetAll()
        {
            return dbCon.resourcetype_metadata.ToList();
        }

        public IList<resourcetype_metadata> GetAll(resourcetype_metadata Entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                var returnVal = (from rtmd in dbCon.resourcetype_metadata
                                 where rtmd.ResourceTypeId == Entity.ResourceTypeId
                                 select rtmd).ToList();
                return returnVal;
            }
        }

        public IQueryable<resourcetype_metadata> GetAny()
        {
            return dbCon.resourcetype_metadata;
        }

        public resourcetype_metadata GetOne(resourcetype_metadata Entity)
        {
            throw new NotImplementedException();
        }

        public resourcetype_metadata Insert(resourcetype_metadata entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                if (entity.CreateDate == null || entity.CreateDate == DateTime.MinValue)
                {
                    entity.CreateDate = DateTime.UtcNow;
                }
                entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //entity.Priority to be used in future to set the order or execution

                dbCon.resourcetype_metadata.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<resourcetype_metadata> InsertBatch(IList<resourcetype_metadata> entities)
        {
            using (dbCon = new ConfigurationDB())
            {
                foreach (resourcetype_metadata entity in entities)
                {

                    if (entity.CreateDate == null || entity.CreateDate == DateTime.MinValue)
                    {
                        entity.CreateDate = DateTime.UtcNow;
                    }
                    dbCon.resourcetype_metadata.Add(entity);
                }

                dbCon.SaveChanges();
            }

            return entities;
        }

        public resourcetype_metadata Update(resourcetype_metadata entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                resourcetype_metadata entityItem = dbCon.resourcetype_metadata.Where(c => c.ResourceTypeId == entity.ResourceTypeId &&
                                                 c.AttributeName == entity.AttributeName).FirstOrDefault();
                
                if (entityItem != null)
                {
                    dbCon.resourcetype_metadata.Attach(entityItem);

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

                    EntityExtension<resourcetype_metadata>.ApplyOnlyChanges(entityItem, entity);
                }
                dbCon.SaveChanges();
            }
            return entity;
        }

        public IList<resourcetype_metadata> UpdateBatch(IList<resourcetype_metadata> entities)
        {
            using (dbCon = new ConfigurationDB())
            {
                foreach (resourcetype_metadata entity in entities)
                {
                    resourcetype_metadata entityItem = dbCon.resourcetype_metadata.Where(c => c.ResourceTypeId == entity.ResourceTypeId &&
                                                     c.AttributeName == entity.AttributeName).FirstOrDefault();

                    /*resource_attributes entityItem = dbCon.resource_attributes.Single(c => c.ResourceId == entity.ResourceId &&
                                                     c.AttributeName == entity.AttributeName);*/

                    if (entityItem != null)
                    {
                        dbCon.resourcetype_metadata.Attach(entityItem);

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

                        EntityExtension<resourcetype_metadata>.ApplyOnlyChanges(entityItem, entity);
                    }
                    //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                }
                dbCon.SaveChanges();
            }
            return entities;
        }
    }
}
