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
    public class ActionDS : ConfigurationManager.Resource.IDataAccess.IEntity<action>
    {
        public ConfigurationDB dbCon;
        public ActionDS()
        {
            dbCon = new ConfigurationDB();
        }
        public bool Delete(action entity)
        {
            throw new NotImplementedException();
        }

        public IList<action> GetAll()
        {
            using (dbCon = new ConfigurationDB())
                return dbCon.action.ToList();
        }

        public IList<action> GetAll(action Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<action> GetAny()
        {
            //using (dbCon = new ConfigurationDB())
                return this.dbCon.action;
        }

        public action GetOne(action Entity)
        {
            throw new NotImplementedException();
        }

        public action Insert(action entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                if (entity.CreateDate == null || entity.CreateDate == DateTime.MinValue)
                {
                    entity.CreateDate = DateTime.UtcNow;
                }
                entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //entity.Priority to be used in future to set the order or execution

                dbCon.action.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<action> InsertBatch(IList<action> entities)
        {
            throw new NotImplementedException();
        }

        public action Update(action entity)
        {
            throw new NotImplementedException();
        }

        public IList<action> UpdateBatch(IList<action> entities)
        {
            using (dbCon = new ConfigurationDB())
            {
                foreach (action entity in entities)
                {
                    action entityItem = dbCon.action.Where(c => c.ActionId == entity.ActionId).FirstOrDefault();



                    if (entityItem != null)
                    {
                        dbCon.action.Attach(entityItem);

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

                        EntityExtension<action>.ApplyOnlyChanges(entityItem, entity);
                    }
                    //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                }
                dbCon.SaveChanges();
            }
            return entities;
        }
    }
}
