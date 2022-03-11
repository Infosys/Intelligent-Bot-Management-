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
    public class ResourceTypeDS : Superbot.Resource.IDataAccess.IEntity<resourcetype>
    {
        public SuperbotDB dbCon;
        public ResourceTypeDS()
        {
            dbCon = new SuperbotDB();
        }
        public bool Delete(resourcetype entity)
        {
            throw new NotImplementedException();
        }

        public IList<resourcetype> GetAll()
        {
            //using (dbCon = new SuperbotDB())
                return dbCon.resourcetype.ToList();
        }

        public IList<resourcetype> GetAll(resourcetype Entity)
        {
            using (dbCon = new SuperbotDB())
                return dbCon.resourcetype.Where(r=>r.ResourceTypeName==Entity.ResourceTypeName).ToList();
        }

        public IQueryable<resourcetype> GetAny()
        {            
                return dbCon.resourcetype;
        }

        public resourcetype GetOne(resourcetype Entity)
        {
            Entity = (from c in dbCon.resourcetype
                      where c.ResourceTypeId == Entity.ResourceTypeId
                      select c).FirstOrDefault();

            return Entity;
        }

        public resourcetype Insert(resourcetype entity)
        {
            throw new NotImplementedException();
        }

        public IList<resourcetype> InsertBatch(IList<resourcetype> entities)
        {
            throw new NotImplementedException();
        }

        public resourcetype Update(resourcetype entity)
        {
            throw new NotImplementedException();
        }

        public IList<resourcetype> UpdateBatch(IList<resourcetype> entities)
        {
            throw new NotImplementedException();
        }
    }
}
