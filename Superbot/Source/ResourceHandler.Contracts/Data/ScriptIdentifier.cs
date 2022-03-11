/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.Solutions.Ainauto.Services.Superbot.Contracts.Data
{
    [DataContract]
    public class ScriptIdentifier
    {
        [DataMember]
        public int ScriptId { get; set; }
        [DataMember]
        public int CategoryId { get; set; }
        [DataMember]
        public int CompanyId { get; set; }
        [DataMember]
        public string ScriptName { get; set; }

        [DataMember]
        public string Path { get; set; }

        [DataMember]
        public List<Parameter> Parameters { get; set; }

        // Remote server names separated by comma
        [DataMember]
        public string RemoteServerNames { get; set; }
        // Property to hold user name for running script on remote machine under different account
        [DataMember]
        public string UserName { get; set; }

        // Property to hold password for running script on remote machine under different account
        [DataMember]
        public string Password { get; set; }

        // Property set for RPA or Automation Engines like Nia, AA etc
        [DataMember]
        public string ReferenceKey { get; set; }

        [DataMember]
        public int ExecutionMode { get; set; }

        [DataMember]
        public string Domain { get; set; }

        [DataMember]
        public int IapNodeTransport { get; set; }

        [DataMember]
        public string ResponseNotificationCallbackURL { get; set; }

        [DataMember]
        public int RemoteExecutionMode { get; set; }
    }

    [DataContract]
    public class Parameter
    {
        [DataMember]
        public string ParameterName { get; set; }
        [DataMember]
        public string ParameterValue { get; set; }
    }
}
