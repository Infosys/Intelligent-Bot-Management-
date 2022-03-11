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

namespace Infosys.Solutions.Ainauto.ConfigurationManager.BusinessEntity
{
    public class ResourceModelConfigInput
    {
        public string name { get; set; }
        public string tenantId { get; set; }
        public List<Labels> labels { get; set; }
        public List<Sections> sections { get; set; }
    }
    public class Labels
    {
        public string name { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }

    public class Sections
    {
        public string name { get; set; }
        public List<Labels> labels { get; set; }
    }
}
