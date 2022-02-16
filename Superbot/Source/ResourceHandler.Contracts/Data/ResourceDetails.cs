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

namespace Infosys.Solutions.Ainauto.Services.Superbot.Contracts.Data
{
    public class ResourceDetails
    {
        public int TenantId { get; set; }
        public string TenantName { set; get; }
        public int PlatformId { get; set; }
        public string PlatformName { set; get; }
        public string ResourceId { get; set; }
        public string ResourceName { set; get; }
        public int ResourceTypeId { get; set; }
        public string ResourceTypeName { set; get; }
        public string HostName { get; set; }

    }
}
