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
    public class ActionParamsDS : Superbot.Resource.IDataAccess.IEntity<action_params>
    {
        public SuperbotDB dbCon;       

        public ActionParamsDS()
        {
            dbCon = new SuperbotDB();
        }
        public bool Delete(action_params entity)
        {
            throw new NotImplementedException();
        }

        public IList<action_params> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<action_params> GetAll(action_params Entity)
        {
            IList<action_params> actionParams_List = new List<action_params>();
            using (dbCon = new SuperbotDB())
            {
                actionParams_List = (from s in dbCon.action_params where s.ActionId == Entity.ActionId select s).ToList();
            }
            return actionParams_List;
        }

        public IQueryable<action_params> GetAny()
        {
            return dbCon.action_params;
        }

        public action_params GetOne(action_params Entity)
        {
            using (dbCon = new SuperbotDB())
            {
                Entity = (from s in dbCon.action_params where s.ParamId == Entity.ParamId select s).FirstOrDefault();
            }
            return Entity;
        }

        public action_params Insert(action_params entity)
        {
            throw new NotImplementedException();
        }

        public IList<action_params> InsertBatch(IList<action_params> entities)
        {
            throw new NotImplementedException();
        }

        public action_params Update(action_params entity)
        {
            throw new NotImplementedException();
        }

        public IList<action_params> UpdateBatch(IList<action_params> entities)
        {
            throw new NotImplementedException();
        }
    }
}
