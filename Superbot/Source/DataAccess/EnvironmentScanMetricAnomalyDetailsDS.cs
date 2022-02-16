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
    public class EnvironmentScanMetricAnomalyDetailsDS : Superbot.Resource.IDataAccess.IEntity<Environment_Scan_Metric_Anomaly_Details>
    {
        public SuperbotDB dbCon;
        public EnvironmentScanMetricAnomalyDetailsDS()
        {
            dbCon = new SuperbotDB();
        }
        public bool Delete(Environment_Scan_Metric_Anomaly_Details entity)
        {
            throw new NotImplementedException();
        }

        #region IEntity<ScriptExecuteResponse> Members

        public IList<Environment_Scan_Metric_Anomaly_Details> GetAll()
        {
            using (dbCon = new SuperbotDB())
                return dbCon.Environment_Scan_Metric_Anomaly_Details.ToList();
        }

        public IList<Environment_Scan_Metric_Anomaly_Details> GetAll(Environment_Scan_Metric_Anomaly_Details Entity)
        {
            using (dbCon = new SuperbotDB())
                return (from ad in dbCon.Environment_Scan_Metric_Anomaly_Details
                        where ad.TenantId == Entity.TenantId
                        && ad.PlatformId == Entity.PlatformId
                        &&ad.ResourceId == Entity.ResourceId
                        && ad.ObservationId == Entity.ObservationId
                        select ad).ToList();
        }

        public IQueryable<Environment_Scan_Metric_Anomaly_Details> GetAny()
        {
            return dbCon.Environment_Scan_Metric_Anomaly_Details;
        }

        public Environment_Scan_Metric_Anomaly_Details GetOne(Environment_Scan_Metric_Anomaly_Details Entity)
        {
            throw new NotImplementedException();
        }

        public Environment_Scan_Metric_Anomaly_Details Insert(Environment_Scan_Metric_Anomaly_Details Entity)
        {
            using (dbCon = new SuperbotDB())
            {

                dbCon.Environment_Scan_Metric_Anomaly_Details.Add(Entity);
                dbCon.SaveChanges();
            }

            return Entity;
        }

        public IList<Environment_Scan_Metric_Anomaly_Details> InsertBatch(IList<Environment_Scan_Metric_Anomaly_Details> entities)
        {
            throw new NotImplementedException();
        }

        public Environment_Scan_Metric_Anomaly_Details Update(Environment_Scan_Metric_Anomaly_Details entity)
        {
            throw new NotImplementedException();
        }

        public IList<Environment_Scan_Metric_Anomaly_Details> UpdateBatch(IList<Environment_Scan_Metric_Anomaly_Details> entities)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
