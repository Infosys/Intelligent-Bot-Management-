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
    public class RemdiationPlanActionParamMapDS : Superbot.Resource.IDataAccess.IEntity<remdiation_plan_action_param_map>
    {
        public SuperbotDB dbCon;        

        public bool Delete(remdiation_plan_action_param_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<remdiation_plan_action_param_map> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<remdiation_plan_action_param_map> GetAll(remdiation_plan_action_param_map Entity)
        {
            IList<remdiation_plan_action_param_map> remediationPlanActionParamMap_List = new List<remdiation_plan_action_param_map>();
            using (dbCon = new SuperbotDB())
            {
                remediationPlanActionParamMap_List = (from s in dbCon.remdiation_plan_action_param_map where s.RemediationPlanActionId == Entity.RemediationPlanActionId select s).ToList();
            }
            return remediationPlanActionParamMap_List;
        }

        public IQueryable<remdiation_plan_action_param_map> GetAny()
        {
            throw new NotImplementedException();
        }

        public remdiation_plan_action_param_map GetOne(remdiation_plan_action_param_map Entity)
        {
            throw new NotImplementedException();
        }

        public remdiation_plan_action_param_map Insert(remdiation_plan_action_param_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<remdiation_plan_action_param_map> InsertBatch(IList<remdiation_plan_action_param_map> entities)
        {
            throw new NotImplementedException();
        }

        public remdiation_plan_action_param_map Update(remdiation_plan_action_param_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<remdiation_plan_action_param_map> UpdateBatch(IList<remdiation_plan_action_param_map> entities)
        {
            throw new NotImplementedException();
        }
    }
}
