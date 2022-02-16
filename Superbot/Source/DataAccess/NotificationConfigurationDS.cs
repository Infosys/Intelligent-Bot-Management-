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
    public class NotificationConfigurationDS : Superbot.Resource.IDataAccess.IEntity<notification_configuration>
    {
        public SuperbotDB dbCon;
        public NotificationConfigurationDS()
        {
            dbCon = new SuperbotDB();
        }
        public bool Delete(notification_configuration entity)
        {
            throw new NotImplementedException();
        }

        #region IEntity<ScriptExecuteResponse> Members

        public IList<notification_configuration> GetAll()
        {
            using (dbCon = new SuperbotDB())
                return dbCon.notification_configuration.ToList();
        }

        public IList<notification_configuration> GetAll(notification_configuration Entity)
        {
            using (dbCon = new SuperbotDB())
                return (from nc in dbCon.notification_configuration
                        where nc.PlatformId == Entity.PlatformId
                        && nc.ReferenceType == Entity.ReferenceType
                        && nc.TenantId == Entity.TenantId
                        select nc).ToList();
        }

        public IQueryable<notification_configuration> GetAny()
        {
            return dbCon.notification_configuration;
        }

        public notification_configuration GetOne(notification_configuration Entity)
        {
            throw new NotImplementedException();
        }

        public notification_configuration Insert(notification_configuration Entity)
        {
            throw new NotImplementedException();
        }

        public IList<notification_configuration> InsertBatch(IList<notification_configuration> entities)
        {
            throw new NotImplementedException();
        }

        public notification_configuration Update(notification_configuration entity)
        {
            throw new NotImplementedException();
        }

        public IList<notification_configuration> UpdateBatch(IList<notification_configuration> entities)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
