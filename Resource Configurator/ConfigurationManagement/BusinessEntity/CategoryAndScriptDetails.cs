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

namespace Infosys.Ainauto.ConfigurationManager.BusinessEntity
{
    public class CategoryAndScriptDetails
    {
        public string AutomationEngineName { get; set; }
        public string SEEBaseUrl { get; set; }
        public string CategoryServiceName { get; set; }
        public string ScriptDetailServiceName { get; set; }

        public string TenantId { get; set; }
    }

    public class GetSEEScriptDetailsReqMsg
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public  string SEEBaseUrl { get; set; }
        public string ServiceName { get; set; }

    }
    public class GetSEEScriptDetailsResMsg
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public List<Scripts> Scripts { get; set; }
    }

    public class GetSEECategoriesReqMsg
    {       
        public string SEEBaseUrl { get; set; }
        public string ServiceName { get; set; }

    }
    public class GetSEECategoriesResMsg
    {
        public List<TestCategory> Categories { get; set; }
    }    

    public class TestCategory
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int CategoryId { get; set; }
        public int ParentCategoryId { get; set; } //its value is zero in case of Category and non-zero in case of SubCategory 
        public bool IsDeleted { get; set; }
        public int? ParentId { get; set; }
        public int CompanyId { get; set; }
        public int ModuleID { get; set; }
        public string NewName { get; set; }
        public int NumberOfScripts { get; set; }
    }

    public class Property
    {
        public int id { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public List<Property> children { get; set; }
    }

    public class AABot
    {
        public string name { get; set; }
        public string botid { get; set; }
    }
    public class AABotList
    {
        public List<AABot> list { get; set; }
            
    }
}
