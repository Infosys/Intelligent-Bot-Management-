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
    public class ActionDS : Superbot.Resource.IDataAccess.IEntity<action>
    {
        public SuperbotDB dbCon; 
        public ActionDS()
        {
            dbCon = new SuperbotDB();
        }

        public bool Delete(action entity)
        {
            throw new NotImplementedException();
        }

        public IList<action> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<action> GetAll(action Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<action> GetAny()
        {
            return dbCon.action;
        }

        public action GetOne(action Entity)
        {
            using (dbCon=new SuperbotDB())
            {
                Entity = (from s in dbCon.action 
                          where s.ActionId == Entity.ActionId 
                          && s.IsDeleted == false select s).FirstOrDefault();
            }
            return Entity;
        }

        public action Insert(action entity)
        {
            throw new NotImplementedException();
        }

        public IList<action> InsertBatch(IList<action> entities)
        {
            throw new NotImplementedException();
        }

        public action Update(action entity)
        {
            throw new NotImplementedException();
        }

        public IList<action> UpdateBatch(IList<action> entities)
        {
            throw new NotImplementedException();
        }
    }
}
