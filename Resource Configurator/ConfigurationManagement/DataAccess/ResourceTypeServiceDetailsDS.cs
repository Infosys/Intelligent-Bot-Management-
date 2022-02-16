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
using Infosys.Solutions.ConfigurationManager.Resource.Entity;

namespace Infosys.Solutions.Ainauto.Resource.DataAccess
{
    public class ResourceTypeServiceDetailsDS : ConfigurationManager.Resource.IDataAccess.IEntity<resourcetype_service_details>
    {
        public ConfigurationDB dbCon;
        public ResourceTypeServiceDetailsDS()
        {
            dbCon = new ConfigurationDB();
        }
        public bool Delete(resourcetype_service_details entity)
        {
            throw new NotImplementedException();
        }

        public IList<resourcetype_service_details> GetAll()
        {
            //using (dbCon = new ConfigurationDB())
            return dbCon.resourcetype_service_details.ToList();
        }

        public IList<resourcetype_service_details> GetAll(resourcetype_service_details Entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                List<resourcetype_service_details> retVal = (from rsd in dbCon.resourcetype_service_details
                                                             where rsd.ResourceTypeId == Entity.ResourceTypeId select rsd).ToList();
                return retVal;
            }
        }

        public IQueryable<resourcetype_service_details> GetAny()
        {
            //using (dbCon = new SuperbotDB())
            return dbCon.resourcetype_service_details;
        }

        public resourcetype_service_details GetOne(resourcetype_service_details Entity)
        {
            throw new NotImplementedException();
        }

        public resourcetype_service_details Insert(resourcetype_service_details entity)
        {
            throw new NotImplementedException();
        }

        public IList<resourcetype_service_details> InsertBatch(IList<resourcetype_service_details> entities)
        {
            throw new NotImplementedException();
        }

        public resourcetype_service_details Update(resourcetype_service_details entity)
        {
            throw new NotImplementedException();
        }

        public IList<resourcetype_service_details> UpdateBatch(IList<resourcetype_service_details> entities)
        {
            throw new NotImplementedException();
        }

    }
}
