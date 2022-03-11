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
    public class AnomalyDetailsDS : Superbot.Resource.IDataAccess.IEntity<anomaly_details>
    {
        public SuperbotDB dbCon;
        public AnomalyDetailsDS()
        {
            dbCon = new SuperbotDB();
        }
        public bool Delete(anomaly_details entity)
        {
            throw new NotImplementedException();
        }

        #region IEntity<ScriptExecuteResponse> Members

        public IList<anomaly_details> GetAll()
        {
            using (dbCon = new SuperbotDB())
                return dbCon.anomaly_details.ToList();
        }

        public IList<anomaly_details> GetAll(anomaly_details Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<anomaly_details> GetAny()
        {
            return dbCon.anomaly_details;
        }

        public anomaly_details GetOne(anomaly_details Entity)
        {
            using (dbCon = new SuperbotDB())
            {
                // return dbCon.observable_resource_map.FirstOrDefault(sc => sc.observableid == Entity.observableid);                
                Entity = (from c in dbCon.anomaly_details
                          orderby c.AnomalyId descending
                          select c).FirstOrDefault();
            }
            return Entity;
        }

        public anomaly_details Insert(anomaly_details Entity)
        {
            using (dbCon = new SuperbotDB())
            {

                dbCon.anomaly_details.Add(Entity);
                dbCon.SaveChanges();
            }

            return Entity;
        }

        public IList<anomaly_details> InsertBatch(IList<anomaly_details> entities)
        {
            throw new NotImplementedException();
        }

        public anomaly_details Update(anomaly_details entity)
        {
            anomaly_details anomaly_obj = new anomaly_details();
            try
            {
                using (dbCon = new SuperbotDB())
                {
                    anomaly_obj = (from s in dbCon.anomaly_details where entity.AnomalyId == s.AnomalyId select s).FirstOrDefault();
                    //observations_obj = (from obs in dbCon.observations where obs.ObservationId == entity.ObservationId select obs).FirstOrDefault();
                    anomaly_obj.RemediationPlanExecId = entity.RemediationPlanExecId;
                    anomaly_obj.RemediationStatus = entity.RemediationStatus;
                    if (entity.State != null)
                        anomaly_obj.State = entity.State;
                    if (entity.IsNotified != null)
                        anomaly_obj.IsNotified = entity.IsNotified;
                    if (entity.NotifiedTime != null)
                        anomaly_obj.NotifiedTime = entity.NotifiedTime;
                    if (entity.ModifiedBy != null && entity.ModifiedDate != null)
                    {
                        anomaly_obj.ModifiedDate = entity.ModifiedDate;
                        anomaly_obj.ModifiedBy = entity.ModifiedBy;
                    }                        

                    dbCon.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return anomaly_obj;
        }

        public IList<anomaly_details> UpdateBatch(IList<anomaly_details> entities)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
