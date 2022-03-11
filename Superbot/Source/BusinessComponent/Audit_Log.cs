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
using Infosys.Solutions.Superbot.Infrastructure.Common;
using DE = Infosys.Solutions.Superbot.Resource.Entity;
using BE = Infosys.Solutions.Ainauto.BusinessEntity;
using Infosys.Solutions.Ainauto.Resource.DataAccess;

namespace Infosys.Solutions.Ainauto.Superbot.BusinessComponent
{
    public class Audit_Log
    {

        public bool LogAuditData(BE.AuditLog auditLog)
        {
            try
            {
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "LogAuditData", "Monitor"), LogHandler.Layer.Business, null);
                DE.Audit_Logs auditLogs = Translator.AuditLog_BE_DE.AuditLogBEtoDE(auditLog);
                AuditLogsDS logsDS = new AuditLogsDS();
                var res = logsDS.Insert(auditLogs);
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "LogAuditData", "Monitor"), LogHandler.Layer.Business, null);
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
