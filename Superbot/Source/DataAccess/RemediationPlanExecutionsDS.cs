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
    public class RemediationPlanExecutionsDS : Superbot.Resource.IDataAccess.IEntity<remediation_plan_executions>
    {
        public SuperbotDB dbCon;
        public RemediationPlanExecutionsDS()
        {
            dbCon = new SuperbotDB();
        }
        //public List<remediation_plan_action_map> GetRemediationPlanAction(string remediationPlanId)
        //{
        //    List<remediation_plan_action_map> remediationPlanActionMap_List = new List<remediation_plan_action_map>();
        //    using (dbCon = new DataEntity())
        //    {
        //        remediationPlanActionMap_List = (from s in dbCon.remediation_plan_action_map where s.RemediationPlanId == remediationPlanId select s).ToList();
        //    }
        //    return remediationPlanActionMap_List;
        //}

        public bool Delete(remediation_plan_executions entity)
        {
            throw new NotImplementedException();
        }

        public IList<remediation_plan_executions> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<remediation_plan_executions> GetAll(remediation_plan_executions Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<remediation_plan_executions> GetAny()
        {
            return dbCon.remediation_plan_executions;
        }

        public remediation_plan_executions GetOne(remediation_plan_executions Entity)
        {
            using(dbCon=new SuperbotDB())
            {
                Entity = (from s in dbCon.remediation_plan_executions where s.RemediationPlanExecId == Entity.RemediationPlanExecId select s).FirstOrDefault();
                return Entity;
            }            
        }

        public remediation_plan_executions Insert(remediation_plan_executions entity)
        {            
            try
            {
                using (dbCon = new SuperbotDB())
                {
                    dbCon.remediation_plan_executions.Add(entity);                
                    dbCon.SaveChanges();
                    entity = this.GetOne(entity);
                }
                
            }
            catch(Exception ex)
            {                
                throw ex;
            }
            
            return entity;            
        }

        public IList<remediation_plan_executions> InsertBatch(IList<remediation_plan_executions> entities)
        {
            throw new NotImplementedException();
        }

        public remediation_plan_executions Update(remediation_plan_executions entity)
        {
            remediation_plan_executions remediation_Plan_Executions = new remediation_plan_executions();
            try
            {
                using(dbCon=new SuperbotDB())
                {
                    remediation_Plan_Executions = (from s in dbCon.remediation_plan_executions where entity.RemediationPlanExecId == s.RemediationPlanExecId select s).First();
                    remediation_Plan_Executions.Status = entity.Status;
                    remediation_Plan_Executions.ExecutionEndDateTime = DateTime.Now;
                    dbCon.SaveChanges();
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }

            return remediation_Plan_Executions;
        }

        public IList<remediation_plan_executions> UpdateBatch(IList<remediation_plan_executions> entities)
        {
            throw new NotImplementedException();
        }
    }
}
