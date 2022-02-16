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

namespace Infosys.Solutions.Ainauto.Superbot.BusinessEntity
{
    public class RecipientConfigurationDetails
    {
        public string ReferenceType { get; set; }
        public string ReferenceKey { get; set; }
        public string RecipientName { get; set; }
        public string ReferenceValue { get; set; }
        public Nullable<bool> isActive { get; set; }
    }
}
