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
    public class ObservableDS : ConfigurationManager.Resource.IDataAccess.IEntity<observable>
    {
        public ConfigurationDB dbCon;
        public ObservableDS()
        {
            dbCon = new ConfigurationDB();
        }
        public bool Delete(observable entity)
        {
            throw new NotImplementedException();
        }

        public IList<observable> GetAll()
        {
            //using (dbCon = new ConfigurationDB())
                return dbCon.observable.ToList();
        }

        public IList<observable> GetAll(observable Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<observable> GetAny()
        {
            //using (dbCon = new ConfigurationDB())
                return dbCon.observable;
        }

        public observable GetOne(observable Entity)
        {
            throw new NotImplementedException();
        }

        public observable Insert(observable entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                if (entity.CreateDate == null || entity.CreateDate == DateTime.MinValue)
                {
                    entity.CreateDate = DateTime.UtcNow;
                }
                entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //entity.Priority to be used in future to set the order or execution

                dbCon.observable.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<observable> InsertBatch(IList<observable> entities)
        {
            using (dbCon = new ConfigurationDB())
            {
                foreach (observable entity in entities)
                {

                    if (entity.CreateDate == null)
                    {
                        entity.CreateDate = DateTime.UtcNow;
                    }
                    dbCon.observable.Add(entity);
                }

                dbCon.SaveChanges();
            }

            return entities;
        }

        public observable Update(observable entity)
        {
            throw new NotImplementedException();
        }

        public IList<observable> UpdateBatch(IList<observable> entities)
        {
            using (dbCon = new ConfigurationDB())
            {
                foreach (observable entity in entities)
                {
                    observable entityItem = dbCon.observable.Where(c => c.ObservableId == entity.ObservableId).FirstOrDefault();



                    if (entityItem != null)
                    {
                        dbCon.observable.Attach(entityItem);

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

                        EntityExtension<observable>.ApplyOnlyChanges(entityItem, entity);
                    }
                    //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
                }
                dbCon.SaveChanges();
            }
            return entities;
        }
    }
}
