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
    public class RecipientConfigurationDS : Superbot.Resource.IDataAccess.IEntity<recipient_configuration>
    {
        public SuperbotDB dbCon;
        public RecipientConfigurationDS()
        {
            dbCon = new SuperbotDB();
        }
        public bool Delete(recipient_configuration entity)
        {
            throw new NotImplementedException();
        }

        #region IEntity<ScriptExecuteResponse> Members

        public IList<recipient_configuration> GetAll()
        {
            using (dbCon = new SuperbotDB())
                return dbCon.recipient_configuration.ToList();
        }

        public IList<recipient_configuration> GetAll(recipient_configuration Entity)
        {
            throw new NotImplementedException();
        }

        public IQueryable<recipient_configuration> GetAny()
        {
            return dbCon.recipient_configuration;
        }

        public recipient_configuration GetOne(recipient_configuration Entity)
        {
            throw new NotImplementedException();
        }

        public recipient_configuration Insert(recipient_configuration Entity)
        {
            throw new NotImplementedException();
        }

        public IList<recipient_configuration> InsertBatch(IList<recipient_configuration> entities)
        {
            throw new NotImplementedException();
        }

        public recipient_configuration Update(recipient_configuration entity)
        {
            throw new NotImplementedException();
        }

        public IList<recipient_configuration> UpdateBatch(IList<recipient_configuration> entities)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
