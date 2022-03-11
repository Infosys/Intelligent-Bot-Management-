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
using Infosys.Solutions.Superbot.Resource.IDataAccess;

namespace Infosys.Solutions.Ainauto.Resource.DataAccess
{
    public class ObservableDS : Superbot.Resource.IDataAccess.IEntity<observable>
    {
        public SuperbotDB dbCon;
        public ObservableDS()
        {
            dbCon = new SuperbotDB();
        }

        public bool Delete(observable entity)
        {
            throw new NotImplementedException();
        }

        public IList<observable> GetAll()
        {
            using (dbCon = new SuperbotDB())
                return dbCon.observable.ToList();
        }

        public IList<observable> GetAll(observable Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<observable> GetAny()
        {
            //using (dbCon = new DataEntity())
                return dbCon.observable;
        }

        public observable GetOne(observable Entity)
        {
            if (Entity.ObservableId!=null && Entity.ObservableId != 0)
            {
                Entity = (from c in dbCon.observable
                          where c.ObservableId == Entity.ObservableId
                          select c).FirstOrDefault();
            }
            else
            {
                Entity = (from c in dbCon.observable
                          where c.ObservableName == Entity.ObservableName
                          select c).FirstOrDefault();
            }            

            return Entity;
        }

        public observable Insert(observable entity)
        {
            throw new NotImplementedException();
        }

        public IList<observable> InsertBatch(IList<observable> entities)
        {
            throw new NotImplementedException();
        }

        public observable Update(observable entity)
        {
            throw new NotImplementedException();
        }

        public IList<observable> UpdateBatch(IList<observable> entities)
        {
            throw new NotImplementedException();
        }
    }
}
