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
    public class ResourcetypeObservableRemediationPlanMapDS : Superbot.Resource.IDataAccess.IEntity<resourcetype_observable_remediation_plan_map>
    {
        public SuperbotDB dbCon;        
        
        public bool Delete(resourcetype_observable_remediation_plan_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<resourcetype_observable_remediation_plan_map> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<resourcetype_observable_remediation_plan_map> GetAll(resourcetype_observable_remediation_plan_map Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<resourcetype_observable_remediation_plan_map> GetAny()
        {
            throw new NotImplementedException();
        }

        public resourcetype_observable_remediation_plan_map GetOne(resourcetype_observable_remediation_plan_map Entity)
        {
            using (dbCon = new SuperbotDB())
            {
                Entity = (from s in dbCon.resourcetype_observable_remediation_plan_map where s.ResourceTypeId == Entity.ResourceTypeId && s.ObservableId == Entity.ObservableId select s).FirstOrDefault();
            }
            return Entity;
        }

        public resourcetype_observable_remediation_plan_map Insert(resourcetype_observable_remediation_plan_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<resourcetype_observable_remediation_plan_map> InsertBatch(IList<resourcetype_observable_remediation_plan_map> entities)
        {
            throw new NotImplementedException();
        }

        public resourcetype_observable_remediation_plan_map Update(resourcetype_observable_remediation_plan_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<resourcetype_observable_remediation_plan_map> UpdateBatch(IList<resourcetype_observable_remediation_plan_map> entities)
        {
            throw new NotImplementedException();
        }
    }
}
