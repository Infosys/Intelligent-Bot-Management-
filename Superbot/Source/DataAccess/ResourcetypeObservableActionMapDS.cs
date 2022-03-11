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
    public class ResourcetypeObservableActionMapDS : Superbot.Resource.IDataAccess.IEntity<resourcetype_observable_action_map>
    {
        public SuperbotDB dbCon;
        public ResourcetypeObservableActionMapDS()
        {
            dbCon = new SuperbotDB();
        }
        public bool Delete(resourcetype_observable_action_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<resourcetype_observable_action_map> GetAll()
        {
            using (dbCon = new SuperbotDB())
                return dbCon.resourcetype_observable_action_map.ToList();
        }

        public IList<resourcetype_observable_action_map> GetAll(resourcetype_observable_action_map Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<resourcetype_observable_action_map> GetAny()
        {
            //using (dbCon = new DataEntity())
                return dbCon.resourcetype_observable_action_map;
        }

        public resourcetype_observable_action_map GetOne(resourcetype_observable_action_map Entity)
        {
            throw new NotImplementedException();
        }

        public resourcetype_observable_action_map Insert(resourcetype_observable_action_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<resourcetype_observable_action_map> InsertBatch(IList<resourcetype_observable_action_map> entities)
        {
            throw new NotImplementedException();
        }

        public resourcetype_observable_action_map Update(resourcetype_observable_action_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<resourcetype_observable_action_map> UpdateBatch(IList<resourcetype_observable_action_map> entities)
        {
            throw new NotImplementedException();
        }
    }
}
