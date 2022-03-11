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
    public class EnvironmentScanMetricDS : Superbot.Resource.IDataAccess.IEntity<Environment_Scan_Metric>
    {
        public SuperbotDB dbCon;
        public EnvironmentScanMetricDS()
        {
            dbCon = new SuperbotDB();
        }
        public bool Delete(Environment_Scan_Metric entity)
        {
            throw new NotImplementedException();
        }

        #region IEntity<ScriptExecuteResponse> Members

        public IList<Environment_Scan_Metric> GetAll()
        {
            using (dbCon = new SuperbotDB())
                return dbCon.Environment_Scan_Metric.ToList();
        }

        public IList<Environment_Scan_Metric> GetAll(Environment_Scan_Metric Entity)
        {
            using (dbCon = new SuperbotDB())
            {
                return (from e in dbCon.Environment_Scan_Metric
                        where e.ResourceID == Entity.ResourceID
                        && e.ObservableID == Entity.ObservableID
                        && e.Version == Entity.Version
                        select e).ToList();
            }                
        }

        public IQueryable<Environment_Scan_Metric> GetAny()
        {
            return dbCon.Environment_Scan_Metric;
        }

        public Environment_Scan_Metric GetOne(Environment_Scan_Metric Entity)
        {
            using (dbCon = new SuperbotDB())
            {
                return (from e in dbCon.Environment_Scan_Metric
                        where e.ResourceID == Entity.ResourceID
                        && e.ObservableID == Entity.ObservableID
                        && e.Version == Entity.Version
                        select e).FirstOrDefault();
            }
        }

        public Environment_Scan_Metric Insert(Environment_Scan_Metric entity)
        {
            using (dbCon = new SuperbotDB())
            {
                if (entity.CreatedDate == null || entity.CreatedDate == DateTime.MinValue)
                {
                    entity.CreatedDate = DateTime.UtcNow;
                }
                entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //entity.Priority to be used in future to set the order or execution

                dbCon.Environment_Scan_Metric.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<Environment_Scan_Metric> InsertBatch(IList<Environment_Scan_Metric> entities)
        {
            throw new NotImplementedException();
        }

        public Environment_Scan_Metric Update(Environment_Scan_Metric entity)
        {
            throw new NotImplementedException();
        }

        public IList<Environment_Scan_Metric> UpdateBatch(IList<Environment_Scan_Metric> entities)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
