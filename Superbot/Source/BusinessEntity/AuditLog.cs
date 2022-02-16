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

namespace Infosys.Solutions.Ainauto.BusinessEntity
{
    public class AuditLog
    {
        public int AnomalyId { get; set; }
        public string ResourceID { get; set; }
        public int ObservableID { get; set; }
        public int ActionID { get; set; }
        public string ActionParams { get; set; }
        public DateTime LogDate { get; set; }
        public string Status { get; set; }
        public string Output { get; set; }
        public int PlatformID { get; set; }
        public int TenantID { get; set; }
    }
}
