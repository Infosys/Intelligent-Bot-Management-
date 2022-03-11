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
    public class ResourceObservableRemediationPlanMapDS : Superbot.Resource.IDataAccess.IEntity<resource_observable_remediation_plan_map>
    {
        public SuperbotDB dbCon;

        public bool Delete(resource_observable_remediation_plan_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<resource_observable_remediation_plan_map> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<resource_observable_remediation_plan_map> GetAll(resource_observable_remediation_plan_map Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<resource_observable_remediation_plan_map> GetAny()
        {
            throw new NotImplementedException();
        }

        public resource_observable_remediation_plan_map GetOne(resource_observable_remediation_plan_map Entity)
        {
            using (dbCon = new SuperbotDB())
            {
                Entity = (from s in dbCon.resource_observable_remediation_plan_map where s.ResourceId == Entity.ResourceId && s.ObservableId == Entity.ObservableId select s).FirstOrDefault();
            }
            return Entity;
        }

        public resource_observable_remediation_plan_map Insert(resource_observable_remediation_plan_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<resource_observable_remediation_plan_map> InsertBatch(IList<resource_observable_remediation_plan_map> entities)
        {
            throw new NotImplementedException();
        }

        public resource_observable_remediation_plan_map Update(resource_observable_remediation_plan_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<resource_observable_remediation_plan_map> UpdateBatch(IList<resource_observable_remediation_plan_map> entities)
        {
            throw new NotImplementedException();
        }
    }
}
