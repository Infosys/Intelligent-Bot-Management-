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
    public class ActionParamsDS : ConfigurationManager.Resource.IDataAccess.IEntity<action_params>
    {
        public ConfigurationDB dbCon;
        public ActionParamsDS()
        {
            dbCon = new ConfigurationDB();
        }
        public bool Delete(action_params entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                int paramId = (Convert.ToInt32(entity.ParamId));
                var entities = dbCon.action_params.Where(rt => rt.ParamId == entity.ParamId);
                if(entities != null)
                {
                    foreach(action_params actParam in entities)
                    {
                        entity = actParam;
                        dbCon.action_params.Attach(entity);
                        dbCon.action_params.Remove(entity);
                    }
                    dbCon.SaveChanges();
                    return true;
                }
            }
                
                //dbCon.action_params.Remove(entity);
                
            return false;
            
        }

        public IList<action_params> GetAll()
        {
            //using (dbCon = new ConfigurationDB())
            return dbCon.action_params.ToList();
        }

        public IList<action_params> GetAll(action_params Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<action_params> GetAny()
        {
            //using (dbCon = new ConfigurationDB())
            return this.dbCon.action_params;
        }

        public action_params GetOne(action_params Entity)
        {
            throw new NotImplementedException();
        }

        public action_params Insert(action_params entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                if (entity.CreateDate == null || entity.CreateDate == DateTime.MinValue)
                {
                    entity.CreateDate = DateTime.UtcNow;
                }
                entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //entity.Priority to be used in future to set the order or execution

                dbCon.action_params.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<action_params> InsertBatch(IList<action_params> entities)
        {
            throw new NotImplementedException();
        }

        public action_params Update(action_params entity)
        {
            throw new NotImplementedException();
        }

        public IList<action_params> UpdateBatch(IList<action_params> entities)
        {
            using (dbCon = new ConfigurationDB())
            {
                foreach (action_params entity in entities)
                {
                    action_params entityItem = dbCon.action_params.Where(c => c.ActionId == entity.ActionId && c.Name == entity.Name).FirstOrDefault();



                    if (entityItem != null)
                    {
                        dbCon.action_params.Attach(entityItem);

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

                        EntityExtension<action_params>.ApplyOnlyChanges(entityItem, entity);
                    }
                    //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                }
                dbCon.SaveChanges();
            }
            return entities;
        }
    }
}
