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
    public class ResourceAttributesDS : Superbot.Resource.IDataAccess.IEntity<resource_attributes>
    {
        public SuperbotDB dbCon;
        public ResourceAttributesDS()
        {
            dbCon = new SuperbotDB();
        }

        public bool Delete(resource_attributes entity)
        {
            throw new NotImplementedException();
        }

        public IList<resource_attributes> GetAll()
        {
            return dbCon.resource_attributes.ToList();
        }

        public IList<resource_attributes> GetAll(resource_attributes Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<resource_attributes> GetAny()
        {
            return dbCon.resource_attributes;
        }

        public resource_attributes GetOne(resource_attributes Entity)
        {
            using (dbCon = new SuperbotDB())
            {
                Entity = (from s in dbCon.resource_attributes where s.ResourceId == Entity.ResourceId && s.AttributeName==Entity.AttributeName select s).FirstOrDefault();
            }
            return Entity;
        }

        public resource_attributes Insert(resource_attributes entity)
        {
            throw new NotImplementedException();
        }

        public IList<resource_attributes> InsertBatch(IList<resource_attributes> entities)
        {
            throw new NotImplementedException();
        }

        public resource_attributes Update(resource_attributes entity)
        {
            throw new NotImplementedException();
        }

        public IList<resource_attributes> UpdateBatch(IList<resource_attributes> entities)
        {
            throw new NotImplementedException();
        }
    }
}
