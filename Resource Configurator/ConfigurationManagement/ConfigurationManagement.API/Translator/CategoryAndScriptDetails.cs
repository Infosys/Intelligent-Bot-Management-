/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BE = Infosys.Ainauto.ConfigurationManager.BusinessEntity;
using IE = Infosys.Solutions.Ainauto.ConfigurationManagement.API.Models;
using Newtonsoft.Json;

namespace Infosys.Solutions.Ainauto.ConfigurationManagement.API.Translator
{
    public class CategoryAndScriptDetails
    {
        public static BE.CategoryAndScriptDetails CategoryAndScriptDetails_IEtoBE(IE.CategoryAndScriptDetails objIE)
        {
            BE.CategoryAndScriptDetails objBE = new BE.CategoryAndScriptDetails();
            var json = JsonConvert.SerializeObject(objIE);
            objBE = JsonConvert.DeserializeObject<BE.CategoryAndScriptDetails>(json);
            return objBE;
        }
        public static BE.GetSEEScriptDetailsReqMsg GetSEEScriptDetailsReqMsg_IEtoBE(IE.GetSEEScriptDetailsReqMsg objIE)
        {
            BE.GetSEEScriptDetailsReqMsg objBE = new BE.GetSEEScriptDetailsReqMsg();
            var json = JsonConvert.SerializeObject(objIE);
            objBE = JsonConvert.DeserializeObject<BE.GetSEEScriptDetailsReqMsg>(json);
            return objBE;
        }
        public static BE.GetSEECategoriesReqMsg GetSEECategoriesReqMsg_IEtoBE(IE.GetSEECategoriesReqMsg objIE)
        {
            BE.GetSEECategoriesReqMsg objBE = new BE.GetSEECategoriesReqMsg();
            var json = JsonConvert.SerializeObject(objIE);
            objBE = JsonConvert.DeserializeObject<BE.GetSEECategoriesReqMsg>(json);
            return objBE;
        }

        public static IE.GetSEEScriptDetailsResMsg GetSEEScriptDetailsResMsg_BEtoIE(BE.GetSEEScriptDetailsResMsg objBE)
        {
            IE.GetSEEScriptDetailsResMsg objIE = new IE.GetSEEScriptDetailsResMsg();
            var json = JsonConvert.SerializeObject(objBE);
            objIE = JsonConvert.DeserializeObject<IE.GetSEEScriptDetailsResMsg>(json);
            return objIE;
        }
        public static List<IE.Property> GetSEECategoriesResMsg_BEtoIE(List<BE.Property> objBE)
        {
            List<IE.Property> objIE = new List<IE.Property>();
            var json = JsonConvert.SerializeObject(objBE);
            objIE = JsonConvert.DeserializeObject<List<IE.Property>>(json);
            return objIE;
        }

        public static IE.AABotList GetAABotDetails_BEtoIE(BE.AABotList objBE)
        {
            IE.AABotList objIE = new IE.AABotList();
            var json = JsonConvert.SerializeObject(objBE);
            objIE = JsonConvert.DeserializeObject<IE.AABotList>(json);
            return objIE;
        }
    }
}