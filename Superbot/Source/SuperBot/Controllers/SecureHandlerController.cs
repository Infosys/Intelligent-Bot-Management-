/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.Solutions.Ainauto.Superbot.BusinessComponent;
using Infosys.Solutions.Ainauto.Superbot.Models;
using Infosys.Solutions.Superbot.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Infosys.Solutions.Ainauto.Superbot.Controllers
{
    public class SecureHandlerController : ApiController
    {
        [HttpGet]
        [Route("EncryptData/{textToSecure=textToSecure}/{passCode=passCode}")]
        public string EncryptData(string textToSecure,string passCode)
        {
            try
            {
                return SecureData.Secure(textToSecure, passCode);
            }
            catch (Exception ex)
            {

                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "EncryptData", "SecureHandlerController"), LogHandler.Layer.WebServiceHost, null);
                return "Exception occured .. Error Message: "+ex.Message;
            }
        }

        [HttpGet]
        [Route("DecryptText/{secureText=secureText}/{passCode=passCode}")]
        public string DecryptText(string secureText, string passCode)
        {
            try
            {
                return SecureData.UnSecure(secureText, passCode);
            }
            catch (Exception ex)
            {

                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "DecryptData", "SecureHandlerController"), LogHandler.Layer.WebServiceHost, null);
                return "Exception occured .. Error Message: " + ex.Message;
            }
        }       

        
    }
}
