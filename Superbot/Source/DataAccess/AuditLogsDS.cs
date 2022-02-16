/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.Solutions.Superbot.Resource.Entity;
using Infosys.Solutions.Superbot.Resource.IDataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.Solutions.Ainauto.Resource.DataAccess
{
    public class AuditLogsDS : Superbot.Resource.IDataAccess.IEntity<Audit_Logs>
    {

        public SuperbotDB dbCon;
        public AuditLogsDS()
        {
            dbCon = new SuperbotDB();
        }

        public bool Delete(Audit_Logs entity)
        {
            throw new NotImplementedException();
        }

        public IList<Audit_Logs> GetAll()
        {
            using (dbCon = new SuperbotDB())
            {
                return dbCon.Audit_Logs.ToList();
            }
        }

        public IList<Audit_Logs> GetAll(Audit_Logs Entity)
        {
            using (dbCon = new SuperbotDB())
            {
                return (from al in dbCon.Audit_Logs where al.AnomalyId == Entity.AnomalyId select al).ToList();
            }
        }

        public IQueryable<Audit_Logs> GetAny()
        {
            throw new NotImplementedException();
        }

        public Audit_Logs GetOne(Audit_Logs Entity)
        {
            throw new NotImplementedException();
        }

        public Audit_Logs Insert(Audit_Logs entity)
        {
            using (dbCon = new SuperbotDB())
            {
                if (entity.LogDate == null || entity.LogDate == DateTime.MinValue)
                {
                    entity.LogDate = DateTime.UtcNow;
                }
               // entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //entity.Priority to be used in future to set the order or execution

                dbCon.Audit_Logs.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<Audit_Logs> InsertBatch(IList<Audit_Logs> entities)
        {
            throw new NotImplementedException();
        }

        public Audit_Logs Update(Audit_Logs entity)
        {
            throw new NotImplementedException();
        }

        public IList<Audit_Logs> UpdateBatch(IList<Audit_Logs> entities)
        {
            throw new NotImplementedException();
        }
    }
}
