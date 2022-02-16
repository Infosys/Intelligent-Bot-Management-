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
using DE = Infosys.Solutions.Superbot.Resource.Entity;
using BE = Infosys.Solutions.Ainauto.BusinessEntity;

namespace Infosys.Solutions.Ainauto.Superbot.BusinessComponent.Translator
{
    public class AuditLog_BE_DE
    {
        public static DE.Audit_Logs AuditLogBEtoDE(BE.AuditLog auditLogBE)
        {
            DE.Audit_Logs audit_Logs_DE = new DE.Audit_Logs();
            audit_Logs_DE.AnomalyId = auditLogBE.AnomalyId;
            audit_Logs_DE.ResourceID = auditLogBE.ResourceID;
            audit_Logs_DE.ObservableID = auditLogBE.ObservableID;
            audit_Logs_DE.ActionID = auditLogBE.ActionID;
            audit_Logs_DE.ActionParameters = auditLogBE.ActionParams;
            audit_Logs_DE.Status = auditLogBE.Status;
            audit_Logs_DE.Output = auditLogBE.Output;
            audit_Logs_DE.PlatformID = auditLogBE.PlatformID;
            audit_Logs_DE.TenantID = auditLogBE.TenantID;
            return audit_Logs_DE;
        }
    }
}
