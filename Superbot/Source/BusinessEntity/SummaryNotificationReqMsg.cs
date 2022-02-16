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
    public class SummaryNotificationReqMsg
    {
        public string ConfigId { get; set; }
        public string PortfolioId { get; set; }
        public int PlatformId { get; set; }
        public int TenantId { get; set; }
        public string TransactionId { get; set; }
        public string ApplicationName { get; set; }
    }
}
