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
    public class EnvironmentScanMetricDetailsDS : Superbot.Resource.IDataAccess.IEntity<Environment_Scan_Metric_Details>
    {
        public SuperbotDB dbCon;
        public EnvironmentScanMetricDetailsDS()
        {
           dbCon = new SuperbotDB();
        }
        public bool Delete(Environment_Scan_Metric_Details entity)
        {
            throw new NotImplementedException();
        }

        #region IEntity<ScriptExecuteResponse> Members

        public IList<Environment_Scan_Metric_Details> GetAll()
        {
            using (dbCon = new SuperbotDB())
                return dbCon.Environment_Scan_Metric_Details.ToList();
        }

        public IList<Environment_Scan_Metric_Details> GetAll(Environment_Scan_Metric_Details Entity)
        {
            using (dbCon = new SuperbotDB())
            {
                return (from e in dbCon.Environment_Scan_Metric_Details
                        where e.EnvironmentScanMetricID == Entity.EnvironmentScanMetricID
                        select e).ToList();
            }
                
        }

        public IQueryable<Environment_Scan_Metric_Details> GetAny()
        {
            return dbCon.Environment_Scan_Metric_Details;
        }

        public Environment_Scan_Metric_Details GetOne(Environment_Scan_Metric_Details Entity)
        {
            throw new NotImplementedException();
        }

        public Environment_Scan_Metric_Details Insert(Environment_Scan_Metric_Details entity)
        {
            using (dbCon = new SuperbotDB())
            {
                if (entity.CreatedDate == null || entity.CreatedDate == DateTime.MinValue)
                {
                    entity.CreatedDate = DateTime.UtcNow;
                }
                entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //entity.Priority to be used in future to set the order or execution

                dbCon.Environment_Scan_Metric_Details.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<Environment_Scan_Metric_Details> InsertBatch(IList<Environment_Scan_Metric_Details> entities)
        {
            using (dbCon = new SuperbotDB())
            {
                foreach (Environment_Scan_Metric_Details entity in entities)
                {

                    if (entity.CreatedDate == null || entity.CreatedDate == DateTime.MinValue)
                    {
                        entity.CreatedDate = DateTime.UtcNow;
                    }
                    entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    //entity.Priority to be used in future to set the order or execution

                    dbCon.Environment_Scan_Metric_Details.Add(entity);
                }

                dbCon.SaveChanges();
            }

            return entities;
        }

        public Environment_Scan_Metric_Details Update(Environment_Scan_Metric_Details entity)
        {
            throw new NotImplementedException();
        }

        public IList<Environment_Scan_Metric_Details> UpdateBatch(IList<Environment_Scan_Metric_Details> entities)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
