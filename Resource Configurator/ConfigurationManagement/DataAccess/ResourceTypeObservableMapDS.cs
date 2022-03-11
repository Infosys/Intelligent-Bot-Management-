/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.Solutions.ConfigurationManager.Resource.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.Solutions.Ainauto.Resource.DataAccess
{
    public class ResourceTypeObservableMapDS : ConfigurationManager.Resource.IDataAccess.IEntity<resourcetype_observable_map>
    {
        public ConfigurationDB dbCon;
        public ResourceTypeObservableMapDS()
        {
            dbCon = new ConfigurationDB();
        }
        public bool Delete(resourcetype_observable_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<resourcetype_observable_map> GetAll()
        {
            return dbCon.resourcetype_observable_map.ToList();
        }

        public IList<resourcetype_observable_map> GetAll(resourcetype_observable_map Entity)
        {
            return (from rt in dbCon.resourcetype_observable_map
                    where rt.ResourceTypeId == Entity.ResourceTypeId
                    && rt.TenantId == Entity.TenantId
                    select rt).ToList();
        }

        public IQueryable<resourcetype_observable_map> GetAny()
        {
            return dbCon.resourcetype_observable_map;
        }

        public resourcetype_observable_map GetOne(resourcetype_observable_map Entity)
        {
            throw new NotImplementedException();
        }

        public resourcetype_observable_map Insert(resourcetype_observable_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<resourcetype_observable_map> InsertBatch(IList<resourcetype_observable_map> entities)
        {
            throw new NotImplementedException();
        }

        public resourcetype_observable_map Update(resourcetype_observable_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<resourcetype_observable_map> UpdateBatch(IList<resourcetype_observable_map> entities)
        {
            throw new NotImplementedException();
        }
    }
}
