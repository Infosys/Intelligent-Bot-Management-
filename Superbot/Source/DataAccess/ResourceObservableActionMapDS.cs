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
    public class ResourceObservableActionMapDS : Superbot.Resource.IDataAccess.IEntity<resource_observable_action_map>
    {

        public SuperbotDB dbCon;
        public ResourceObservableActionMapDS()
        {
            dbCon = new SuperbotDB();
        }

        public bool Delete(resource_observable_action_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<resource_observable_action_map> GetAll()
        {
            using (dbCon = new SuperbotDB())
            {
                return dbCon.resource_observable_action_map.ToList();
            }
        }

        public IList<resource_observable_action_map> GetAll(resource_observable_action_map Entity)
        {
            using (dbCon = new SuperbotDB())
            {
                return (from roam in dbCon.resource_observable_action_map 
                        where roam.ResourceId == Entity.ResourceId 
                        && roam.ObservableId == Entity.ObservableId
                        select roam).ToList();
            }
        }

        public IQueryable<resource_observable_action_map> GetAny()
        {
            return dbCon.resource_observable_action_map;
        }

        public resource_observable_action_map GetOne(resource_observable_action_map Entity)
        {
            throw new NotImplementedException();
        }

        public resource_observable_action_map Insert(resource_observable_action_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<resource_observable_action_map> InsertBatch(IList<resource_observable_action_map> entities)
        {
            throw new NotImplementedException();
        }

        public resource_observable_action_map Update(resource_observable_action_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<resource_observable_action_map> UpdateBatch(IList<resource_observable_action_map> entities)
        {
            throw new NotImplementedException();
        }
    }
}
