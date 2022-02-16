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
    
    public class ResourceDS : Superbot.Resource.IDataAccess.IEntity<resource>
    {
        public SuperbotDB dbCon;     
        public ResourceDS()
        {
            dbCon = new SuperbotDB();
        }
        public bool Delete(resource entity)
        {
            throw new NotImplementedException();
        }

        public IList<resource> GetAll()
        {
            //using (dbCon = new SuperbotDB())
                return dbCon.resource.ToList();
        }

        public IList<resource> GetAll(resource Entity)
        {
            using (dbCon = new SuperbotDB())
                return dbCon.resource.Where(r=>r.ResourceId==Entity.ResourceId && r.ResourceTypeId==Entity.ResourceTypeId).ToList();
        }

        public IQueryable<resource> GetAny()
        {
            //using (dbCon = new DataEntity())
                return dbCon.resource;
        }

        public resource GetOne(resource Entity)
        {
            using(dbCon=new SuperbotDB())
            {
                Entity = (from c in dbCon.resource
                          where c.ResourceId == Entity.ResourceId
                          select c).FirstOrDefault();

                return Entity;
            }
            
        }

        public resource Insert(resource entity)
        {
            throw new NotImplementedException();
        }

        public IList<resource> InsertBatch(IList<resource> entities)
        {
            throw new NotImplementedException();
        }

        public resource Update(resource entity)
        {
            throw new NotImplementedException();
        }

        public IList<resource> UpdateBatch(IList<resource> entities)
        {
            throw new NotImplementedException();
        }
    }
}
