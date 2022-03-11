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
    public class ObservationsDS : Superbot.Resource.IDataAccess.IEntity<observations>
    {
        public SuperbotDB dbCon;
        public ObservationsDS()
        {
            dbCon = new SuperbotDB();
        }
        public bool Delete(observations entity)
        {
            throw new NotImplementedException();
        }

        #region IEntity<ScriptExecuteResponse> Members

        public IList<observations> GetAll()
        {
            using (dbCon = new SuperbotDB())
                return dbCon.observations.ToList();
        }

        public IList<observations> GetAll(observations Entity)
        {
            using (dbCon = new SuperbotDB())
            {
                return (from o in dbCon.observations
                              where o.PlatformId == Entity.PlatformId
                              select o).ToList();
                          
            }
            
        }

        public IQueryable<observations> GetAny()
        {
            return dbCon.observations;
        }

        public observations GetOne(observations Entity)
        {
            using (dbCon = new SuperbotDB())
            {
                // return dbCon.observable_resource_map.FirstOrDefault(sc => sc.observableid == Entity.observableid);                
                Entity = (from c in dbCon.observations   
                          orderby c.ObservationId descending
                          select c).FirstOrDefault();
            }
            return Entity;
        }

        public observations Insert(observations Entity)
        {
            try
            {
                using (dbCon = new SuperbotDB())
                {
                    if (Entity.CreateDate == null || Entity.CreateDate == DateTime.MinValue)
                    {
                        Entity.CreateDate = DateTime.UtcNow;
                    }
                    Entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

                    dbCon.observations.Add(Entity);
                    dbCon.SaveChanges();
                }

                return Entity;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            
        }

        public IList<observations> InsertBatch(IList<observations> entities)
        {
            throw new NotImplementedException();
        }

        public observations Update(observations entity)
        {
            observations observations_obj = new observations();
            try
            {
                using (dbCon = new SuperbotDB())
                {
                    observations_obj = (from s in dbCon.observations where entity.ObservationId == s.ObservationId select s).FirstOrDefault();
                    //observations_obj = (from obs in dbCon.observations where obs.ObservationId == entity.ObservationId select obs).FirstOrDefault();
                    if (entity.RemediationPlanExecId != null)
                        observations_obj.RemediationPlanExecId = entity.RemediationPlanExecId;
                    if (entity.RemediationStatus != null)
                        observations_obj.RemediationStatus = entity.RemediationStatus;
                    if (entity.State != null)
                        observations_obj.State = entity.State;                    
                    if (entity.IsNotified!=null)                    
                        observations_obj.IsNotified = entity.IsNotified;
                    if(entity.NotifiedTime!=null)
                        observations_obj.NotifiedTime = entity.NotifiedTime;
                    if (entity.Value != null)
                        observations_obj.Value = entity.Value;
                    if (entity.ObservationStatus != null)
                        observations_obj.ObservationStatus = entity.ObservationStatus;

                    dbCon.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return observations_obj;
        }

        public IList<observations> UpdateBatch(IList<observations> entities)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
