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
    public class ResourceObservableActionMapDS : ConfigurationManager.Resource.IDataAccess.IEntity<resource_observable_action_map>
    {
        public ConfigurationDB dbCon;
        public ResourceObservableActionMapDS()
        {
            dbCon = new ConfigurationDB();
        }
        public bool Delete(resource_observable_action_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<resource_observable_action_map> GetAll()
        {
            return dbCon.resource_observable_action_map.ToList();
        }

        public IList<resource_observable_action_map> GetAll(resource_observable_action_map Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<resource_observable_action_map> GetAny()
        {
            return dbCon.resource_observable_action_map;
        }

        public resource_observable_action_map GetOne(resource_observable_action_map Entity)
        {
            throw new NotImplementedException();
        }

        public resource_observable_action_map Insert(resource_observable_action_map entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                if (entity.CreateDate == null || entity.CreateDate == DateTime.MinValue)
                {
                    entity.CreateDate = DateTime.UtcNow;
                }
                entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //entity.Priority to be used in future to set the order or execution

                dbCon.resource_observable_action_map.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<resource_observable_action_map> InsertBatch(IList<resource_observable_action_map> entities)
        {
            throw new NotImplementedException();
        }

        public resource_observable_action_map Update(resource_observable_action_map entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                resource_observable_action_map entityItem = dbCon.resource_observable_action_map.Where(c => c.ResourceId == entity.ResourceId &&
                                                                            c.ObservableId == entity.ObservableId &&
                                                                            c.ActionId == entity.ActionId).FirstOrDefault();

                if (entityItem != null)
                {
                    dbCon.resource_observable_action_map.Attach(entityItem);

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
                    EntityExtension<resource_observable_action_map>.ApplyOnlyChanges(entityItem, entity);
                    dbCon.SaveChanges();
                }
            }
            return entity;
        }

        public IList<resource_observable_action_map> UpdateBatch(IList<resource_observable_action_map> entities)
        {
            throw new NotImplementedException();
        }
    }
}
