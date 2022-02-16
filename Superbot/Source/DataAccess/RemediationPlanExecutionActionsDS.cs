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
    public class RemediationPlanExecutionActionsDS : Superbot.Resource.IDataAccess.IEntity<remediation_plan_execution_actions>
    {
        public SuperbotDB dbCon;
        //public List<remediation_plan_action_map> GetRemediationPlanAction(string remediationPlanId)
        //{
        //    List<remediation_plan_action_map> remediationPlanActionMap_List = new List<remediation_plan_action_map>();
        //    using (dbCon = new DataEntity())
        //    {
        //        remediationPlanActionMap_List = (from s in dbCon.remediation_plan_action_map where s.RemediationPlanId == remediationPlanId select s).ToList();
        //    }
        //    return remediationPlanActionMap_List;
        //}

        public bool Delete(remediation_plan_execution_actions entity)
        {
            throw new NotImplementedException();
        }

        public IList<remediation_plan_execution_actions> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<remediation_plan_execution_actions> GetAll(remediation_plan_execution_actions Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<remediation_plan_execution_actions> GetAny()
        {
            throw new NotImplementedException();
        }

        public remediation_plan_execution_actions GetOne(remediation_plan_execution_actions Entity)
        {
            throw new NotImplementedException();
        }

        public remediation_plan_execution_actions Insert(remediation_plan_execution_actions entity)
        {
            try
            {
                using (dbCon = new SuperbotDB())
                {
                    dbCon.remediation_plan_execution_actions.Add(entity);
                    dbCon.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return entity;
        }

        public IList<remediation_plan_execution_actions> InsertBatch(IList<remediation_plan_execution_actions> entities)
        {
            try
            {
                using (dbCon = new SuperbotDB())
                {
                    foreach (remediation_plan_execution_actions entity in entities)
                    {
                        dbCon.remediation_plan_execution_actions.Add(entity);                        
                    }
                    dbCon.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return entities;
        }

        public remediation_plan_execution_actions Update(remediation_plan_execution_actions entity)
        {
            throw new NotImplementedException();
        }

        public IList<remediation_plan_execution_actions> UpdateBatch(IList<remediation_plan_execution_actions> entities)
        {
            throw new NotImplementedException();
        }
    }
}
