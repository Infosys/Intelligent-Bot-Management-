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
    public class OperatorDS : Superbot.Resource.IDataAccess.IEntity<@operator>
    {
        public SuperbotDB dbCon;
        public OperatorDS()
        {
            dbCon = new SuperbotDB();
        }
        public bool Delete(@operator entity)
        {
            throw new NotImplementedException();
        }

        public IList<@operator> GetAll()
        {
            using (dbCon = new SuperbotDB())
                return dbCon.@operator.ToList();
        }

        public IList<@operator> GetAll(@operator Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<@operator> GetAny()
        {
            return dbCon.@operator;
        }

        public @operator GetOne(@operator Entity)
        {
            using (dbCon = new SuperbotDB())
            {
                Entity = (from o in dbCon.@operator where o.OperatorId == Entity.OperatorId select o).FirstOrDefault();
                return Entity;
            }
        }

        public @operator Insert(@operator entity)
        {
            throw new NotImplementedException();
        }

        public IList<@operator> InsertBatch(IList<@operator> entities)
        {
            throw new NotImplementedException();
        }

        public @operator Update(@operator entity)
        {
            throw new NotImplementedException();
        }

        public IList<@operator> UpdateBatch(IList<@operator> entities)
        {
            throw new NotImplementedException();
        }
    }
}
