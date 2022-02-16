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
    public class RemediationPlanDS : Superbot.Resource.IDataAccess.IEntity<remediation_plan>
    {
        public SuperbotDB dbCon;
        public RemediationPlanDS()
        {
            dbCon = new SuperbotDB();
        }
        public bool Delete(remediation_plan entity)
        {
            throw new NotImplementedException();
        }

        public IList<remediation_plan> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<remediation_plan> GetAll(remediation_plan Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<remediation_plan> GetAny()
        {
            return dbCon.remediation_plan;
        }

        public remediation_plan GetOne(remediation_plan Entity)
        {
            remediation_plan remediation_Plan = new remediation_plan();
            using (dbCon = new SuperbotDB())
            {
                remediation_Plan = (from s in dbCon.remediation_plan where s.RemediationPlanId == Entity.RemediationPlanId select s).FirstOrDefault();
            }
            return remediation_Plan;
        }

        public remediation_plan Insert(remediation_plan entity)
        {
            throw new NotImplementedException();
        }

        public IList<remediation_plan> InsertBatch(IList<remediation_plan> entities)
        {
            throw new NotImplementedException();
        }

        public remediation_plan Update(remediation_plan entity)
        {
            throw new NotImplementedException();
        }

        public IList<remediation_plan> UpdateBatch(IList<remediation_plan> entities)
        {
            throw new NotImplementedException();
        }
    }
}
