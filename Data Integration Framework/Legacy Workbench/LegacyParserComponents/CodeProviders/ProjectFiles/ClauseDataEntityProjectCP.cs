using System;
using System.Collections.Generic;
using System.Text;

using Infosys.Solutions.CodeGeneration.Framework;

namespace Infosys.Lif.LegacyWorkbench.CodeProviders.ProjectFiles
{
    internal class ModelObjectDataEntityProjectCP : ContentProvider
    {

        private string rootNameSpace;

        [PlaceHolder("RootNameSpace")]
        internal string RootNameSpace
        {
            get { return rootNameSpace; }
        } 



        Guid dataEntityProjectGuid;
        Entities.GenericCollection<Entities.ModelObjectModule> objectForCodeGeneration;
        internal ModelObjectDataEntityProjectCP(Guid projectGuid, Entities.GenericCollection<Entities.ModelObjectModule> entityModules, string rootNamespace)
        {
            rootNameSpace = rootNamespace;
            objectForCodeGeneration = entityModules;
        }
        [PlaceHolder("ProjectGuid")]
        string DataEntityGuid
        {
            get
            {
                return dataEntityProjectGuid.ToString();
            }
        }
        [PlaceHolder("FileIncludes")]
        string FileIncludes
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                Template template = ContentTemplate.RepeatingTemplate(FileIncludeCP.TemplateName);
                foreach (Entities.ModelObjectModule module in objectForCodeGeneration)
                {
                    foreach (Entities.Entity entity in module.ModelObjects)
                    {
                        FileIncludeCP codeGenerator = new FileIncludeCP(module.Name, entity.EntityName);
                        codeGenerator.ContentTemplate = template;
                        sb.Append(codeGenerator.GenerateContent());
                    }
                }
                return sb.ToString();
            }
        }


    }
}
