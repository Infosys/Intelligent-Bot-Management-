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
    public class ResourceDependencyMapDS : Superbot.Resource.IDataAccess.IEntity<resource_dependency_map>
    {
        public SuperbotDB dbCon;
        public ResourceDependencyMapDS()
        {
            dbCon = new SuperbotDB();
        }
        public bool Delete(resource_dependency_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<resource_dependency_map> GetAll()
        {
            using (dbCon = new SuperbotDB())
                return dbCon.resource_dependency_map.ToList();
        }

        public IList<resource_dependency_map> GetAll(resource_dependency_map Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<resource_dependency_map> GetAny()
        {
            //using (dbCon = new DataEntity())
            return dbCon.resource_dependency_map;
        }

        public resource_dependency_map GetOne(resource_dependency_map Entity)
        {
            resource_dependency_map resourceDependencyMap = (from rdmds in dbCon.resource_dependency_map
                                                             where rdmds.ResourceId == Entity.ResourceId
                                                             select rdmds).FirstOrDefault();
            return resourceDependencyMap;
        }

        public resource_dependency_map Insert(resource_dependency_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<resource_dependency_map> InsertBatch(IList<resource_dependency_map> entities)
        {
            throw new NotImplementedException();
        }

        public resource_dependency_map Update(resource_dependency_map entity)
        {
            throw new NotImplementedException();
        }

        public IList<resource_dependency_map> UpdateBatch(IList<resource_dependency_map> entities)
        {
            throw new NotImplementedException();
        }
    }

    public class ResourceDependencyMapDSExtn
    {
        public SuperbotDB dbCon;
        public ResourceDependencyMapDSExtn()
        {
            dbCon = new SuperbotDB();
        }
        public IList<GetHierarchyResources_Result> HierarchyResources(string resID, string tenantID)
        {
            return dbCon.GetHierarchyResources(resID, tenantID).ToList();
        }
        public IList<GetDependencyResources_Result> DependencyResources(string resID, string tenantID)
        {
            return dbCon.GetDependencyResources(resID, tenantID).ToList();
        }
        public IList<CustomDataEntity.ResourceDependencyDetails> GetDependencyResources(List<string> resID, int tenantID)
        {
            //var tab = (from r in dbCon.resource_dependency_map select r).ToList();
            var res = (from r in dbCon.resource_dependency_map                       
                       where resID.Contains(r.ResourceId)
                       && r.TenantId == tenantID
                       select new CustomDataEntity.ResourceDependencyDetails
                       {
                           Resourceid = r.ResourceId,
                           DependencyResourceId = r.DependencyResourceId,
                           DependencyType = r.DependencyType
                       }).ToList();

            return res;
        }
    }    

    
}
