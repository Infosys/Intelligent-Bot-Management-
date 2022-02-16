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
    public class ActionTypeDS : Superbot.Resource.IDataAccess.IEntity<actiontype>
    {
        public SuperbotDB dbCon;
        public ActionTypeDS()
        {
            dbCon = new SuperbotDB();
        }

        public bool Delete(actiontype entity)
        {
            throw new NotImplementedException();
        }

        public IList<actiontype> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<actiontype> GetAll(actiontype Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<actiontype> GetAny()
        {
            return dbCon.actiontype;
        }

        public actiontype GetOne(actiontype Entity)
        {
            using (dbCon = new SuperbotDB())
            {
                Entity = (from s in dbCon.actiontype where s.ActionTypeId == Entity.ActionTypeId select s).FirstOrDefault();
            }
            return Entity;
        }

        public actiontype Insert(actiontype entity)
        {
            throw new NotImplementedException();
        }

        public IList<actiontype> InsertBatch(IList<actiontype> entities)
        {
            throw new NotImplementedException();
        }

        public actiontype Update(actiontype entity)
        {
            throw new NotImplementedException();
        }

        public IList<actiontype> UpdateBatch(IList<actiontype> entities)
        {
            throw new NotImplementedException();
        }
    }
}
