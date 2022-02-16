using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.Solutions.Ainauto.Resource.DataAccess.Models
{
    public class RemediationPlan
    {
        public string remediationplanid { get; set; }
        public string remediationplanname { get; set; }
        public List<Actions> actions { get; set; }

    }

    public class Actions
    {
        public string remediationplanactionid { get; set; }
        public int actionid { get; set; }
        public string actionname { get; set; }
        public int scriptid { get; set; }
        public int categoryid { get; set; }
        public string actionstageid { get; set; }
        public List<Parameters> parameters { get; set; }

    }

    public class Parameters
    {
        public string remediationplanactionid { get; set; }
        public long paramid { get; set; }
        public string name { get; set; }
        public Nullable<bool> ismandatory { get; set; }
        public string defaultvalue { get; set; }
        public string providedvalue { get; set; }
        public bool isfield { get; set; }

    }
}
