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

namespace Infosys.Solutions.ConfigurationManager.Infrastructure.Common
{
    public class Errors
    {
        //Enum to maintain application specific error code
        //Every error code listed here should have an entry 
        //created in the ErrorMessage resource file
        public enum ErrorCodes
        {
            Critical = 5000,
            Warning = 3000,
            Standard_Error = 1000,
            InvalidCharacter_Validation = 1040,
            Platform_Data_NotFound = 1041,
            RemediatioPlan_NotFound = 1042,
            SEE_Response_Null=1043,
            Value_NullOrEmpty_Error=1044,
            Method_Returned_Null=1045
        }
    }
}
