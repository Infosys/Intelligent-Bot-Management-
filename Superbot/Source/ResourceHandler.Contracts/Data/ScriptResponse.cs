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
    public class ScriptResponse
    {
        [DataMember]
        public string TransactionId { get; set; }
        [DataMember]
        public string Status { get; set; }
        [DataMember]
        public string SuccessMessage { get; set; }
        [DataMember]
        public string ErrorMessage { get; set; }
        [DataMember]
        public bool IsSuccess { get; set; }
        [DataMember]
        public string ComputerName { get; set; } //will be the machine on which command gets executed
        [DataMember]
        public string InputCommand { get; set; }
        [DataMember]
        public List<Parameter> OutParameters { get; set; }
        [DataMember]
        public string LogData { get; set; }
        [DataMember]
        public string SourceTransactionId { get; set; }
    }
}
