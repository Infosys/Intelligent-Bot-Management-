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

namespace Infosys.Solutions.Ainauto.Superbot.Infrastructure.ServiceClientLibrary
{
    public class ServiceConstants
    {

          public const string sScriptCentralUrl = "{0}/{1}.svc";
          public const string sSuperBotUrl = "{0}/{1}";

    }

    public enum Services
    {
        WEMScriptExecService,
        ResourceHandler
    }
}
