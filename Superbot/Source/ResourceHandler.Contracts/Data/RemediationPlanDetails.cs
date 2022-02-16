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
    public class RemediationPlanDetails
    {
        public int RemediationPlanId { get; set; }
        public string RemediationPlanName { get; set; }
        public string RemediationPlanDescription { get; set; }
        public bool isUserDefined { get; set; }
        //public List<ActionDetails> ActionDetails { get; set; }
    }
    public class ActionDetails
    {
        public int ActionId { get; set; }
        public string ActionName { get; set; }
        public string ActionStageId { get; set; }
        public int ActionSequence { get; set; }
    }
}
