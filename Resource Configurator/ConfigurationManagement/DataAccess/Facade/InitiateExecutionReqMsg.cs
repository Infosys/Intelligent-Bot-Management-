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

namespace Infosys.Solutions.Ainauto.Resource.DataAccess.Facade
{
    public class InitiateExecutionReqMsg
    {
        public ScriptIdentifier ScriptIdentifier { get; set; }
    }
    public class ScriptIdentifier
    {
        public int ScriptId { get; set; }
        public int CategoryId { get; set; }
        public int CompanyId { get; set; }
        public string ScriptName { get; set; }
        public string Path { get; set; }
        public List<Parameter> Parameters { get; set; }
        // Remote server names separated by comma
        public string RemoteServerNames { get; set; }
        // Property to hold user name for running script on remote machine under different account
        public string UserName { get; set; }
        // Property to hold password for running script on remote machine under different account
        public string Password { get; set; }
        // Property set for RPA or Automation Engines like Nia, AA etc
        public string ReferenceKey { get; set; }
        public int ExecutionMode { get; set; }
        public string Domain { get; set; }
        public int IapNodeTransport { get; set; }
        public string ResponseNotificationCallbackURL { get; set; }
        public int RemoteExecutionMode { get; set; }
    }

    
    public class Parameter
    {
        public string ParameterName { get; set; }
        public string ParameterValue { get; set; }
    }

    public class InitiateExecutionResMsg
    {       
        public List<ScriptResponse> ScriptResponse { get; set; }
    }

    public class ScriptResponse
    {
        
        public string TransactionId { get; set; }
        public string Status { get; set; }
        public string SuccessMessage { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSuccess { get; set; }
        public string ComputerName { get; set; } 
        public string InputCommand { get; set; }
        public List<Parameter> OutParameters { get; set; }
        public string LogData { get; set; }
        public string SourceTransactionId { get; set; }
    }
}
