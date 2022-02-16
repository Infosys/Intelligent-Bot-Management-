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
    public class ResourceAttributesDS : ConfigurationManager.Resource.IDataAccess.IEntity<resource_attributes>
    {
        public ConfigurationDB dbCon;

        public ResourceAttributesDS()
        {
            dbCon = new ConfigurationDB();
        }

        public bool Delete(resource_attributes entity)
        {
            throw new NotImplementedException();
        }

        public IList<resource_attributes> GetAll()
        {
            return dbCon.resource_attributes.ToList();
        }

        public IList<resource_attributes> GetAll(resource_attributes Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<resource_attributes> GetAny()
        {
            //using (dbCon = new ConfigurationDB())
                return dbCon.resource_attributes;
        }

        public resource_attributes GetOne(resource_attributes Entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                Entity = (from s in dbCon.resource_attributes where s.ResourceId == Entity.ResourceId && s.AttributeName==Entity.AttributeName select s).FirstOrDefault();
            }
            return Entity;
        }

        public resource_attributes Insert(resource_attributes entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                if (entity.CreateDate == null || entity.CreateDate == DateTime.MinValue)
                {
                    entity.CreateDate = DateTime.UtcNow;
                }
                entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //entity.Priority to be used in future to set the order or execution

                dbCon.resource_attributes.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<resource_attributes> InsertBatch(IList<resource_attributes> entities)
        {
            using (dbCon = new ConfigurationDB())
            {
                foreach (resource_attributes entity in entities)
                {

                    if (entity.CreateDate == null || entity.CreateDate == DateTime.MinValue)
                    {
                        entity.CreateDate = DateTime.UtcNow;
                    }
                    entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    dbCon.resource_attributes.Add(entity);
                }

                dbCon.SaveChanges();
            }
            
            return entities;
        }

        public resource_attributes Update(resource_attributes entity)
        {
            throw new NotImplementedException();
        }

        public IList<resource_attributes> UpdateBatch(IList<resource_attributes> entities)
        {
            using (dbCon = new ConfigurationDB())
            {
                foreach (resource_attributes entity in entities)
                {
                    resource_attributes entityItem = dbCon.resource_attributes.Where(c => c.ResourceId == entity.ResourceId &&
                                                     c.AttributeName == entity.AttributeName).FirstOrDefault();

                    /*resource_attributes entityItem = dbCon.resource_attributes.Single(c => c.ResourceId == entity.ResourceId &&
                                                     c.AttributeName == entity.AttributeName);*/

                    if (entityItem != null)
                    {
                        dbCon.resource_attributes.Attach(entityItem);

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

                        EntityExtension<resource_attributes>.ApplyOnlyChanges(entityItem, entity);
                    }                   
                    //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                }
                dbCon.SaveChanges();
            }
            return entities;
        }
    }
}
