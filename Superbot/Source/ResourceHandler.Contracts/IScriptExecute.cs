/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using Infosys.Solutions.Ainauto.Services.Superbot.Contracts.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.Solutions.Ainauto.Services.Superbot.Contracts
{
    [ServiceContract]
    public interface IScriptExecute
    {
        [WebInvoke(Method = "POST",RequestFormat =WebMessageFormat.Json,ResponseFormat =WebMessageFormat.Json)]
        [OperationContract]
        [FaultContract(typeof(Data.ServiceFaultError))]
        InitiateExecutionResMsg InitiateExecution(InitiateExecutionReqMsg value);

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        [FaultContract(typeof(Data.ServiceFaultError))]
        Task<InitiateExecutionResMsg> AsyncInitiateExecution(InitiateExecutionReqMsg value);

        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [OperationContract]
        [FaultContract(typeof(Data.ServiceFaultError))]
        GetTransactionStatusResMsg GetTransactionStatus(GetTransactionStatusReqMsg value);
    }
}
