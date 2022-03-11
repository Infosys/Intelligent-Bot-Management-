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
    public class CategoriesList
    {
        public List<Categories> Categories { get; set; }
    }

    public class Categories
    {
        public int CategoryId { get; set; }
        public int CompanyId { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public int ModuleID { get; set; }
        public string Name { get; set; }
        public string NewName { get; set; }
        public int NumberOfScripts { get; set; }
        public int ParentCategoryId { get; set; }
        public int ParentId { get; set; }

    }

    public class ScriptList
    {
        public List<Scripts> Scripts { get; set; }
    }

    public class Scripts
    {
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int ScriptId { get; set; }
        public List<ScriptParam> Params { get; set; }
    }
    

    public class ScriptParam
    {
        public string Name { get; set; }
        public string DefaultValue { get; set; }
        public bool IsMandatory { get; set; }
        public int ScriptId { get; set; }
        public int ParamId { get; set; }
        public int ParamType { get; set; }

    }
}
