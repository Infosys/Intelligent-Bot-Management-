/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IE = Infosys.Solutions.Ainauto.ConfigurationManagement.API.Models;
using BE = Infosys.Ainauto.ConfigurationManager.BusinessEntity;
using BE2 = Infosys.Solutions.Ainauto.ConfigurationManager.BusinessEntity;
using Newtonsoft.Json;

namespace Infosys.Solutions.Ainauto.ConfigurationManagement.API.Translator
{
    public class ResourceModelConfig
    {
        public static BE.ResourceModelGenerationReqMsg ResourceModelConfigToBE(IE.ResourceModelConfigInput objIE)
        {
            BE.ResourceModelGenerationReqMsg objBE = new BE.ResourceModelGenerationReqMsg();
            objBE.Platformtype = objIE.name;
            objBE.Tenantid = Convert.ToInt32(objIE.tenantId);
            objBE.Platformname = (from o in objIE.labels where o.name == "Platform Name" select o.value).FirstOrDefault();
            objBE.HostName = (from o in objIE.labels where o.name == "Host Name" select o.value).FirstOrDefault();

            //getting control tower details
            List<IE.Labels> controlTowerDetailsList = (from s in objIE.sections where s.name == "Control Tower Details" select s.labels).FirstOrDefault();

            objBE.IPAddress = (from c in controlTowerDetailsList where c.name == "IP Address" select c.value).FirstOrDefault();
            objBE.API_URL = (from c in controlTowerDetailsList where c.name == "Control Tower URL" select c.value).FirstOrDefault();
            objBE.API_UserName = (from c in controlTowerDetailsList where c.name == "Control Tower User Name" select c.value).FirstOrDefault();
            objBE.API_Password = (from c in controlTowerDetailsList where c.name == "Control Tower Password" select c.value).FirstOrDefault();
            objBE.Service_UserName = (from c in controlTowerDetailsList where c.name == "Service User Name" select c.value).FirstOrDefault();
            objBE.Service_Password = (from c in controlTowerDetailsList where c.name == "Service Password" select c.value).FirstOrDefault();

            //getting db details
            List<IE.Labels> databaseDetailsList = (from s in objIE.sections where s.name == "Database Details" select s.labels).FirstOrDefault();

            objBE.Database_HostName = (from d in databaseDetailsList where d.name == "Database Host Name" select d.value).FirstOrDefault();
            objBE.Database_Type = (from d in databaseDetailsList where d.name == "Database Type" select d.value).FirstOrDefault();
            objBE.Database_IPaddress = (from d in databaseDetailsList where d.name == "Database IP Address" select d.value).FirstOrDefault();
            objBE.Database_Name = (from d in databaseDetailsList where d.name == "Database Name" select d.value).FirstOrDefault();
            objBE.Database_UserName = (from d in databaseDetailsList where d.name == "Database User Name" select d.value).FirstOrDefault();
            objBE.Database_Password = (from d in databaseDetailsList where d.name == "Database Password" select d.value).FirstOrDefault();

            return objBE;
        }

        public static BE2.ResourceModelConfigInput ResourceModelConfigInputIEtoBE(IE.ResourceModelConfigInput objIE)
        {
            BE2.ResourceModelConfigInput objBE = new BE2.ResourceModelConfigInput();
            var json = JsonConvert.SerializeObject(objIE);
            objBE = JsonConvert.DeserializeObject<BE2.ResourceModelConfigInput>(json);
            return objBE;
        }
    }
}