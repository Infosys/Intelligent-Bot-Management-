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
    public class ActionTypeDS : ConfigurationManager.Resource.IDataAccess.IEntity<actiontype>
    {
        public ConfigurationDB dbCon;
        public ActionTypeDS()
        {
            dbCon = new ConfigurationDB();
        }
        public bool Delete(actiontype entity)
        {
            throw new NotImplementedException();
        }

        public IList<actiontype> GetAll()
        {
            //using (dbCon = new ConfigurationDB())
            return dbCon.actiontype.ToList();
        }

        public IList<actiontype> GetAll(actiontype Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<actiontype> GetAny()
        {
            //using (dbCon = new ConfigurationDB())
            return this.dbCon.actiontype;
        }

        public actiontype GetOne(actiontype Entity)
        {
            throw new NotImplementedException();
        }

        public actiontype Insert(actiontype entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                if (entity.CreateDate == null || entity.CreateDate == DateTime.MinValue)
                {
                    entity.CreateDate = DateTime.UtcNow;
                }
                entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //entity.Priority to be used in future to set the order or execution

                dbCon.actiontype.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<actiontype> InsertBatch(IList<actiontype> entities)
        {
            throw new NotImplementedException();
        }

        public actiontype Update(actiontype entity)
        {
            throw new NotImplementedException();
        }

        public IList<actiontype> UpdateBatch(IList<actiontype> entities)
        {
            using (dbCon = new ConfigurationDB())
            {
                foreach (actiontype entity in entities)
                {
                    actiontype entityItem = dbCon.actiontype.Where(c => c.ActionTypeId == entity.ActionTypeId).FirstOrDefault();



                    if (entityItem != null)
                    {
                        dbCon.actiontype.Attach(entityItem);

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

                        EntityExtension<actiontype>.ApplyOnlyChanges(entityItem, entity);
                    }
                    //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                }
                dbCon.SaveChanges();
            }
            return entities;
        }
    }
}
