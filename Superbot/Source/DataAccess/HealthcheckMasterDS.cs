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
    public class HealthcheckMasterDS : Superbot.Resource.IDataAccess.IEntity<healthcheck_master>
    {
        public SuperbotDB dbCon;
        public HealthcheckMasterDS()
        {
            dbCon = new SuperbotDB();
        }

        public bool Delete(healthcheck_master entity)
        {
            throw new NotImplementedException();
        }

        public IList<healthcheck_master> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<healthcheck_master> GetAll(healthcheck_master Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<healthcheck_master> GetAny()
        {
            return dbCon.healthcheck_master;
        }

        public healthcheck_master GetOne(healthcheck_master Entity)
        {
            using (dbCon = new SuperbotDB())
            {
                Entity = (from s in dbCon.healthcheck_master where s.ConfigId == Entity.ConfigId select s).FirstOrDefault();
            }
            return Entity;
        }

        public healthcheck_master Insert(healthcheck_master entity)
        {
            throw new NotImplementedException();
        }

        public IList<healthcheck_master> InsertBatch(IList<healthcheck_master> entities)
        {
            throw new NotImplementedException();
        }

        public healthcheck_master Update(healthcheck_master entity)
        {
            throw new NotImplementedException();
        }

        public IList<healthcheck_master> UpdateBatch(IList<healthcheck_master> entities)
        {
            throw new NotImplementedException();
        }
    }
}
