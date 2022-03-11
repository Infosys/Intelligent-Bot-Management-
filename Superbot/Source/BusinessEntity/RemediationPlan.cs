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
    public class RemediationPlan
    {
        public int remediationplanid { get; set; }
        public string remediationplanname { get; set; }
        public bool CancelIfFailed { get; set; }
        public List<RPActions> actions { get; set; }

    }

    public class RPActions
    {
        public int remediationplanactionid { get; set; }
        public int actionid { get; set; }
        public string actionname { get; set; }
        public string actionTypeName { get; set; }
        public int scriptid { get; set; }
        public int categoryid { get; set; }
        public string actionstageid { get; set; }
        public string ExecutionMode { get; set; }
        public List<RPParameters> parameters { get; set; }

    }

    public class RPParameters
    {
        public int remediationplanactionid { get; set; }
        public long paramid { get; set; }
        public string name { get; set; }
        public Nullable<bool> ismandatory { get; set; }
        public string defaultvalue { get; set; }
        public string providedvalue { get; set; }
        public bool isfield { get; set; }

    }
}
