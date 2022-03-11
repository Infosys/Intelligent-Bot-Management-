using System;
using System.Collections.Generic;
using System.Text;

using Infosys.Solutions.CodeGeneration.Framework;

namespace Infosys.Lif.LegacyWorkbench.CodeProviders.ProjectFiles
{
    class FileIncludeCP : ContentProvider
    {
        internal static string TemplateName = "FileInclude";

        string moduleNameForCode;
        [PlaceHolder("ModuleName")]
        string ModuleName
        {
            get { return moduleNameForCode; }
        }

        string entityNameForCode;
        [PlaceHolder("FileName")]
        string EntityName
        {
            get { return entityNameForCode; }
        }
        internal FileIncludeCP(string moduleName, string entityName)
        {
            moduleNameForCode = moduleName;
            entityNameForCode = entityName;
        }
    }
}
