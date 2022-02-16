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
    public class OperatorDS : ConfigurationManager.Resource.IDataAccess.IEntity<@operator>
    {
        public ConfigurationDB dbCon;
        public OperatorDS()
        {
            dbCon = new ConfigurationDB();
        }
        public bool Delete(@operator entity)
        {
            throw new NotImplementedException();
        }

        public IList<@operator> GetAll()
        {
            //using (dbCon = new ConfigurationDB())
            return dbCon.@operator.ToList();
        }

        public IList<@operator> GetAll(@operator Entity)
        {
            //using (dbCon = new ConfigurationDB())
            //    return dbCon.resource.Where(r => r.ResourceId == Entity.ResourceId && r.ResourceTypeId == Entity.ResourceTypeId).ToList();
            throw new NotImplementedException();
        }

        public IQueryable<@operator> GetAny()
        {
            //using (dbCon = new ConfigurationDB())            
            return dbCon.@operator;
        }

        public @operator GetOne(@operator Entity)
        {
            //using (dbCon = new ConfigurationDB())
            //{
            //    Entity = (from c in dbCon.@operator
            //              where c.ResourceId == Entity.ResourceId
            //              select c).FirstOrDefault();

            //    return Entity;
            //}
            throw new NotImplementedException();

        }

        public @operator Insert(@operator entity)
        {
            //using (dbCon = new ConfigurationDB())
            //{
            //    if (entity.CreateDate == null || entity.CreateDate == DateTime.MinValue)
            //    {
            //        entity.CreateDate = DateTime.UtcNow;
            //    }
            //    entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            //    //entity.Priority to be used in future to set the order or execution

            //    dbCon.resource.Add(entity);
            //    dbCon.SaveChanges();
            //}

            //return entity;
            throw new NotImplementedException();
        }

        public IList<@operator> InsertBatch(IList<@operator> entities)
        {
            //using (dbCon = new ConfigurationDB())
            //{
            //    foreach (resource entity in entities)
            //    {

            //        if (entity.CreateDate == null)
            //        {
            //            entity.CreateDate = DateTime.UtcNow;
            //        }
            //        dbCon.resource.Add(entity);
            //    }

            //    dbCon.SaveChanges();
            //}


            //return entities;
            throw new NotImplementedException();
        }

        public @operator Update(@operator entity)
        {
            //using (dbCon = new ConfigurationDB())
            //{
            //    resource entityItem = dbCon.resource.Where(c => c.ResourceId == entity.ResourceId).FirstOrDefault();

            //    if (entityItem != null)
            //    {
            //        dbCon.resource.Attach(entityItem);

            //        DateTime? lastModifiedOn = null;

            //        lastModifiedOn = entityItem.ModifiedDate;

            //        if (entity.ModifiedDate == null)
            //        {
            //            entity.ModifiedDate = DateTime.UtcNow;
            //        }
            //        if (lastModifiedOn == entity.ModifiedDate)
            //        {
            //            entity.ModifiedDate = DateTime.UtcNow;
            //        }

            //        //dbCon.Entry(entityItem).CurrentValues.SetValues(entity);
            //        EntityExtension<resource>.ApplyOnlyChanges(entityItem, entity);
            //        dbCon.SaveChanges();
            //    }
            //}
            //return entity;
            throw new NotImplementedException();
        }

        public IList<@operator> UpdateBatch(IList<@operator> entities)
        {
            throw new NotImplementedException();
        }       

    }
}
