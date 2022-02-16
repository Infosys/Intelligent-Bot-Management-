/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infosys.Lif
{
    class MemoryQueueMessage
    {
        // Message Id
        public string Id { get; set; }
        // Message Body
        public string Body { get; set; }
        // Message Label
        public string Label { get; set; }

    }
}
