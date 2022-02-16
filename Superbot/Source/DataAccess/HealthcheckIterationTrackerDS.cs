/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.Solutions.Superbot.Resource.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.Solutions.Ainauto.Resource.DataAccess
{ 
    public class HealthcheckIterationTrackerDS: Superbot.Resource.IDataAccess.IEntity<healthcheck_iteration_tracker>
    {
        public SuperbotDB dbCon;

        public bool Delete(healthcheck_iteration_tracker entity)
        {
            throw new NotImplementedException();
        }

        #region IEntity<ScriptExecuteResponse> Members

        public IList<healthcheck_iteration_tracker> GetAll()
        {
            using (dbCon = new SuperbotDB())
                return dbCon.healthcheck_iteration_tracker.ToList();
        }

        public IList<healthcheck_iteration_tracker> GetAll(healthcheck_iteration_tracker Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<healthcheck_iteration_tracker> GetAny()
        {
            throw new NotImplementedException();
        }

        public healthcheck_iteration_tracker GetOne(healthcheck_iteration_tracker Entity)
        {
            using (dbCon = new SuperbotDB())
            {
                return dbCon.healthcheck_iteration_tracker.FirstOrDefault(sc => sc.TrackingId == Entity.TrackingId);
            }
        }

        public healthcheck_iteration_tracker Insert(healthcheck_iteration_tracker entity)
        {
            using (dbCon = new SuperbotDB())
            {
                if (entity.CreatedOn == null || entity.CreatedOn == DateTime.MinValue)
                {
                    entity.CreatedOn = DateTime.UtcNow;
                }
                entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //entity.Priority to be used in future to set the order or execution

                dbCon.healthcheck_iteration_tracker.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<healthcheck_iteration_tracker> InsertBatch(IList<healthcheck_iteration_tracker> entities)
        {
            using (dbCon = new SuperbotDB())
            {
                foreach (healthcheck_iteration_tracker entity in entities)
                {
                    if (entity.CreatedOn == null || entity.CreatedOn == DateTime.MinValue)
                    {
                        entity.CreatedOn= DateTime.UtcNow;
                    }
                    dbCon.healthcheck_iteration_tracker.Add(entity);
                }
                dbCon.SaveChanges();
            }
            return entities;
        }

       

        public healthcheck_iteration_tracker Update(healthcheck_iteration_tracker entity)
        {
            using (dbCon = new SuperbotDB())
            {
                healthcheck_iteration_tracker entityItem = new healthcheck_iteration_tracker();
               
                    entityItem = dbCon.healthcheck_iteration_tracker.Single(c => c.TrackingId == entity.TrackingId);

                dbCon.healthcheck_iteration_tracker.Attach(entityItem);

                if (entity.ModifiedOn == null || entity.ModifiedOn == DateTime.MinValue)
                {
                    entity.ModifiedOn= DateTime.UtcNow;
                }
                entity.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                entity.TrackingId = entityItem.TrackingId;
                EntityExtension<healthcheck_iteration_tracker>.ApplyOnlyChanges(entityItem, entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<healthcheck_iteration_tracker> UpdateBatch(IList<healthcheck_iteration_tracker> entities)
        {
            using (dbCon = new SuperbotDB())
            {
                foreach (healthcheck_iteration_tracker entity in entities)
                {
                    healthcheck_iteration_tracker entityItem = dbCon.healthcheck_iteration_tracker.Single(c => c.TrackingId == entity.TrackingId );

                    dbCon.healthcheck_iteration_tracker.Attach(entityItem);

                    if (entity.ModifiedOn == null || entity.ModifiedOn == DateTime.MinValue)
                    {
                        entity.ModifiedOn = DateTime.UtcNow;
                    }
                    EntityExtension<healthcheck_iteration_tracker>.ApplyOnlyChanges(entityItem, entity);
                }
                dbCon.SaveChanges();
            }
            return entities;
        }       


        #endregion
    }

}
