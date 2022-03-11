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
    public class PlatformsDS : ConfigurationManager.Resource.IDataAccess.IEntity<platforms>
    {
        public ConfigurationDB dbCon;
        public PlatformsDS()
        {
            dbCon = new ConfigurationDB();
        }
        public bool Delete(platforms entity)
        {
            throw new NotImplementedException();
        }

        public IList<platforms> GetAll()
        {
            //using (dbCon = new ConfigurationDB())
                return dbCon.platforms.ToList();
        }

        public IList<platforms> GetAll(platforms Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<platforms> GetAny()
        {
            //using (dbCon = new ConfigurationDB())
                return dbCon.platforms;
            
        }

        public platforms GetOne(platforms Entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                Entity = (from p in dbCon.platforms where p.PlatformId == Entity.PlatformId select p).FirstOrDefault();
                return Entity;
            }
        }

        public platforms Insert(platforms entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                if (entity.CreateDate == null || entity.CreateDate == DateTime.MinValue)
                {
                    entity.CreateDate = DateTime.UtcNow;
                }
                entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;                

                dbCon.platforms.Add(entity);
                dbCon.SaveChanges();
                return entity;
            }

            
        }

        public IList<platforms> InsertBatch(IList<platforms> entities)
        {
            throw new NotImplementedException();
        }

        public platforms Update(platforms entity)
        {
            throw new NotImplementedException();
        }

        public IList<platforms> UpdateBatch(IList<platforms> entities)
        {
            throw new NotImplementedException();
        }
    }
}
