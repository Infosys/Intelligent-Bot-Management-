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
    public class RemediationPlanActionMapDS : Superbot.Resource.IDataAccess.IEntity<remediation_plan_action_map>
    {
        public SuperbotDB dbCon;
        public RemediationPlanActionMapDS()
        {
            dbCon = new SuperbotDB();
        }
        public bool Delete(remediation_plan_action_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<remediation_plan_action_map> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<remediation_plan_action_map> GetAll(remediation_plan_action_map Entity)
        {
            IList<remediation_plan_action_map> remediationPlanActionMap_List = new List<remediation_plan_action_map>();
            using (dbCon = new SuperbotDB())
            {
                remediationPlanActionMap_List = (from s in dbCon.remediation_plan_action_map where s.RemediationPlanId == Entity.RemediationPlanId orderby s.ActionSequence ascending select s ).ToList() ;
            }
            return remediationPlanActionMap_List;
        }

        public IQueryable<remediation_plan_action_map> GetAny()
        {
            return dbCon.remediation_plan_action_map;
        }

        public remediation_plan_action_map GetOne(remediation_plan_action_map Entity)
        {
            remediation_plan_action_map remediationPlanActionMap = new remediation_plan_action_map();
            using (dbCon = new SuperbotDB())
            {
                remediationPlanActionMap = (from s in dbCon.remediation_plan_action_map where s.RemediationPlanId == Entity.RemediationPlanId && s.ActionId == Entity.ActionId select s).FirstOrDefault();
            }
            return remediationPlanActionMap;
        }

        public remediation_plan_action_map Insert(remediation_plan_action_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<remediation_plan_action_map> InsertBatch(IList<remediation_plan_action_map> entities)
        {
            throw new NotImplementedException();
        }

        public remediation_plan_action_map Update(remediation_plan_action_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<remediation_plan_action_map> UpdateBatch(IList<remediation_plan_action_map> entities)
        {
            throw new NotImplementedException();
        }
    }
}
