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
    public class ResourcetypeObservableActionMapDS : ConfigurationManager.Resource.IDataAccess.IEntity<resourcetype_observable_action_map>
    {
        public ConfigurationDB dbCon;
        public ResourcetypeObservableActionMapDS()
        {
            dbCon = new ConfigurationDB();
        }
        public bool Delete(resourcetype_observable_action_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<resourcetype_observable_action_map> GetAll()
        {
            //using (dbCon = new ConfigurationDB())
                return dbCon.resourcetype_observable_action_map.ToList();
        }

        public IList<resourcetype_observable_action_map> GetAll(resourcetype_observable_action_map Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<resourcetype_observable_action_map> GetAny()
        {
            //using (dbCon = new ConfigurationDB())
                return dbCon.resourcetype_observable_action_map;
        }

        public resourcetype_observable_action_map GetOne(resourcetype_observable_action_map Entity)
        {
            throw new NotImplementedException();
        }

        public resourcetype_observable_action_map Insert(resourcetype_observable_action_map entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                if (entity.CreateDate == null || entity.CreateDate == DateTime.MinValue)
                {
                    entity.CreateDate = DateTime.UtcNow;
                }
                entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //entity.Priority to be used in future to set the order or execution

                dbCon.resourcetype_observable_action_map.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<resourcetype_observable_action_map> InsertBatch(IList<resourcetype_observable_action_map> entities)
        {
            using (dbCon = new ConfigurationDB())
            {
                foreach (resourcetype_observable_action_map entity in entities)
                {

                    if (entity.CreateDate == null)
                    {
                        entity.CreateDate = DateTime.UtcNow;
                    }
                    dbCon.resourcetype_observable_action_map.Add(entity);
                }

                dbCon.SaveChanges();
            }

            return entities;
        }

        public resourcetype_observable_action_map Update(resourcetype_observable_action_map entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                resourcetype_observable_action_map entityItem = dbCon.resourcetype_observable_action_map.
                                                                Where(c => c.ResourceTypeId == entity.ResourceTypeId &&
                                                                            c.ObservableId == entity.ObservableId &&
                                                                            c.ActionId==entity.ActionId).FirstOrDefault();

                if(entityItem!=null)
                {
                    dbCon.resourcetype_observable_action_map.Attach(entityItem);

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
                    EntityExtension<resourcetype_observable_action_map>.ApplyOnlyChanges(entityItem, entity);
                    dbCon.SaveChanges();
                }
            }

            return entity;
        }

        public IList<resourcetype_observable_action_map> UpdateBatch(IList<resourcetype_observable_action_map> entities)
        {
            using (dbCon = new ConfigurationDB())
            {
                foreach (resourcetype_observable_action_map entity in entities)
                {
                    resourcetype_observable_action_map entityItem = dbCon.resourcetype_observable_action_map.Where(c => c.ResourceTypeId == entity.ResourceTypeId &&
                                                                            c.ObservableId == entity.ObservableId &&
                                                                            c.ActionId == entity.ActionId).FirstOrDefault();

                    

                    if (entityItem != null)
                    {
                        dbCon.resourcetype_observable_action_map.Attach(entityItem);

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

                        EntityExtension<resourcetype_observable_action_map>.ApplyOnlyChanges(entityItem, entity);
                    }
                    //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                }
                dbCon.SaveChanges();
            }
            return entities;
            
        }
    }
}
