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
    public class TenantDS : ConfigurationManager.Resource.IDataAccess.IEntity<tenant>
    {
        public ConfigurationDB dbCon;
        public TenantDS()
        {
            dbCon = new ConfigurationDB();
        }
        public bool Delete(tenant entity)
        {
            throw new NotImplementedException();
        }

        public IList<tenant> GetAll()
        {
            //using (dbCon = new ConfigurationDB())
            return dbCon.tenant.ToList();
        }

        public IList<tenant> GetAll(tenant Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tenant> GetAny()
        {
            //using (dbCon = new ConfigurationDB())
            return dbCon.tenant;

        }

        public tenant GetOne(tenant Entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                Entity = (from p in dbCon.tenant where p.TenantId == Entity.TenantId select p).FirstOrDefault();
                return Entity;
            }
        }

        public tenant Insert(tenant entity)
        {
            throw new NotImplementedException();
        }

        public IList<tenant> InsertBatch(IList<tenant> entities)
        {
            throw new NotImplementedException();
        }

        public tenant Update(tenant entity)
        {
            throw new NotImplementedException();
        }

        public IList<tenant> UpdateBatch(IList<tenant> entities)
        {
            throw new NotImplementedException();
        }
    }
}

