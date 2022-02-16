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
    public class ObservableResourceMapDS : ConfigurationManager.Resource.IDataAccess.IEntity<observable_resource_map>
    {
        public ConfigurationDB dbCon;
        public ObservableResourceMapDS()
        {
            dbCon = new ConfigurationDB();
        }
        public bool Delete(observable_resource_map entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                var dbEntity = dbCon.observable_resource_map.Find(entity.ObservableId, entity.ResourceId);

                if (dbEntity != null)
                {
                    try
                    {
                        dbCon.observable_resource_map.Remove(dbEntity);
                        dbCon.SaveChanges();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public IList<observable_resource_map> GetAll()
        {
            return dbCon.observable_resource_map.ToList();
        }

        public IList<observable_resource_map> GetAll(observable_resource_map Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<observable_resource_map> GetAny()
        {
            return dbCon.observable_resource_map;
        }

        public observable_resource_map GetOne(observable_resource_map Entity)
        {
            throw new NotImplementedException();
        }

        public observable_resource_map Insert(observable_resource_map entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                if (entity.CreateDate == null || entity.CreateDate == DateTime.MinValue)
                {
                    entity.CreateDate = DateTime.UtcNow;
                }
                entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                //entity.Priority to be used in future to set the order or execution

                dbCon.observable_resource_map.Add(entity);
                dbCon.SaveChanges();
            }

            return entity;
        }

        public IList<observable_resource_map> InsertBatch(IList<observable_resource_map> entities)
        {
            using (dbCon = new ConfigurationDB())
            {
                foreach (observable_resource_map entity in entities)
                {

                    if (entity.CreateDate == null || entity.CreateDate == DateTime.MinValue)
                    {
                        entity.CreateDate = DateTime.UtcNow;
                    }
                    entity.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

                    dbCon.observable_resource_map.Add(entity);
                }

                dbCon.SaveChanges();
            }

            return entities;
        }

        public observable_resource_map Update(observable_resource_map entity)
        {
            using (dbCon = new ConfigurationDB())
            {
                observable_resource_map ExistingObj = dbCon.observable_resource_map.Find(entity.ObservableId,entity.ResourceId);
                if (ExistingObj != null)
                {                    
                    ExistingObj.CreatedBy = entity.CreatedBy;
                    ExistingObj.CreateDate = entity.CreateDate;
                    ExistingObj.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                    ExistingObj.ModifiedDate = DateTime.UtcNow;
                    ExistingObj.ValidityStart = entity.ValidityStart;
                    ExistingObj.ValidityEnd = entity.ValidityEnd;
                    ExistingObj.OperatorId = entity.OperatorId;
                    ExistingObj.LowerThreshold = entity.LowerThreshold;
                    ExistingObj.UpperThreshold = entity.UpperThreshold;
                    ExistingObj.TenantId = entity.TenantId;

                    dbCon.SaveChanges();
                }
                else
                {
                    entity = null;
                }
            }
            return entity;
        }

        public IList<observable_resource_map> UpdateBatch(IList<observable_resource_map> entities)
        {
            throw new NotImplementedException();
        }
    }
}
