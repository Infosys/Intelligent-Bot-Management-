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
    public class HealthCheckDetailsDS : Superbot.Resource.IDataAccess.IEntity<healthcheck_details>
    {
        public SuperbotDB dbCon;
        public HealthCheckDetailsDS()
        {
            dbCon = new SuperbotDB();
        }

        public bool Delete(healthcheck_details entity)
        {
            throw new NotImplementedException();
        }

        public IList<healthcheck_details> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<healthcheck_details> GetAll(healthcheck_details Entity)
        {
            using (dbCon = new SuperbotDB())
            {
                return (from s in dbCon.healthcheck_details where s.ConfigId == Entity.ConfigId select s).ToList();
            }            
        }

        public IQueryable<healthcheck_details> GetAny()
        {
            return dbCon.healthcheck_details;
        }

        public healthcheck_details GetOne(healthcheck_details Entity)
        {
            using (dbCon = new SuperbotDB())
            {
                Entity = (from s in dbCon.healthcheck_details where s.ConfigId == Entity.ConfigId select s).FirstOrDefault();
            }
            return Entity;
        }

        public healthcheck_details Insert(healthcheck_details entity)
        {
            throw new NotImplementedException();
        }

        public IList<healthcheck_details> InsertBatch(IList<healthcheck_details> entities)
        {
            throw new NotImplementedException();
        }

        public healthcheck_details Update(healthcheck_details entity)
        {
            throw new NotImplementedException();
        }

        public IList<healthcheck_details> UpdateBatch(IList<healthcheck_details> entities)
        {
            throw new NotImplementedException();
        }
    }
}
