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
using Infosys.Solutions.Superbot.Resource.Entity;

namespace Infosys.Solutions.Ainauto.Resource.DataAccess
{
    public class PlatformsDS : Superbot.Resource.IDataAccess.IEntity<platforms>
    {
        public SuperbotDB dbCon;
        public PlatformsDS()
        {
            dbCon = new SuperbotDB();
        }
        public bool Delete(platforms entity)
        {
            throw new NotImplementedException();
        }

        public IList<platforms> GetAll()
        {
            //using (dbCon = new SuperbotDB())
                return dbCon.platforms.ToList();
        }

        public IList<platforms> GetAll(platforms Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<platforms> GetAny()
        {           
                return dbCon.platforms;
        }

        public platforms GetOne(platforms Entity)
        {
            using(dbCon=new SuperbotDB())
            {
                Entity = (from p in dbCon.platforms where p.PlatformId == Entity.PlatformId select p).FirstOrDefault();
                return Entity;
            }
        }

        public platforms Insert(platforms entity)
        {
            throw new NotImplementedException();
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
