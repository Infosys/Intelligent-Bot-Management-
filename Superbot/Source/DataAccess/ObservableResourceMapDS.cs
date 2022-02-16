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
    public class ObservableResourceMapDS : Superbot.Resource.IDataAccess.IEntity<observable_resource_map>
    {
        public SuperbotDB dbCon;
        public ObservableResourceMapDS()
        {
            dbCon = new SuperbotDB();
        }
        public bool Delete(observable_resource_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<observable_resource_map> GetAll()
        {
            using (dbCon = new SuperbotDB())
                return dbCon.observable_resource_map.ToList();
        }

        public IList<observable_resource_map> GetAll(observable_resource_map Entity)
        {
            using (dbCon = new SuperbotDB())
                return dbCon.observable_resource_map.Where(o=>o.ResourceId==Entity.ResourceId && o.ObservableId==Entity.ObservableId).ToList();
        }

        public IQueryable<observable_resource_map> GetAny()
        {
            //using (dbCon = new DataEntity())
                return dbCon.observable_resource_map;
        }

        public observable_resource_map GetOne(observable_resource_map Entity)
        {
            observable_resource_map entity = null;

            using (dbCon = new SuperbotDB())
            {
                // return dbCon.observable_resource_map.FirstOrDefault(sc => sc.observableid == Entity.observableid);

                entity = (from c in dbCon.observable_resource_map
                          where c.ObservableId == Entity.ObservableId && c.ResourceId == Entity.ResourceId
                          select c).FirstOrDefault();
            }
            return entity;
        }

        public observable_resource_map Insert(observable_resource_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<observable_resource_map> InsertBatch(IList<observable_resource_map> entities)
        {
            throw new NotImplementedException();
        }

        public observable_resource_map Update(observable_resource_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<observable_resource_map> UpdateBatch(IList<observable_resource_map> entities)
        {
            throw new NotImplementedException();
        }
    }
}
