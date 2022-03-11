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
    public class HealthcheckIterationTrackerDetailsDS : Superbot.Resource.IDataAccess.IEntity<healthcheck_iteration_tracker_details>
    {
        public SuperbotDB dbCon;

        public bool Delete(healthcheck_iteration_tracker_details entity)
        {
            throw new NotImplementedException();
        }

        public IList<healthcheck_iteration_tracker_details> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<healthcheck_iteration_tracker_details> GetAll(healthcheck_iteration_tracker_details Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<healthcheck_iteration_tracker_details> GetAny()
        {
            throw new NotImplementedException();
        }

        public healthcheck_iteration_tracker_details GetOne(healthcheck_iteration_tracker_details Entity)
        {
            throw new NotImplementedException();
        }

        public healthcheck_iteration_tracker_details Insert(healthcheck_iteration_tracker_details entity)
        {
            using (dbCon = new SuperbotDB())
            {
                if (entity.CreatedOn == null || entity.CreatedOn == DateTime.MinValue)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                }
                entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //entity.Priority to be used in future to set the order or execution

                dbCon.healthcheck_iteration_tracker_details.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<healthcheck_iteration_tracker_details> InsertBatch(IList<healthcheck_iteration_tracker_details> entities)
        {
            throw new NotImplementedException();
        }

        public healthcheck_iteration_tracker_details Update(healthcheck_iteration_tracker_details entity)
        {
            throw new NotImplementedException();
        }

        public IList<healthcheck_iteration_tracker_details> UpdateBatch(IList<healthcheck_iteration_tracker_details> entities)
        {
            throw new NotImplementedException();
        }
    }
}
