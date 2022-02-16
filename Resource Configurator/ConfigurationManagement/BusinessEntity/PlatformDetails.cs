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

namespace Infosys.Ainauto.ConfigurationManager.BusinessEntity
{
    public class PlatformDetails
    {
        public string PlatformId { get; set; }
        public string PlatformTypeName { get; set; }
        public string PlatformInstanceName { get; set; }
        public string ResourceTypeId { get; set; }
        public string ResourceTypeName { get; set; }
    }
}
