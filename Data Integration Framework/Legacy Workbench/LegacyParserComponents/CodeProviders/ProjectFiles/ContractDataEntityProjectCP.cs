using System;
using System.Collections.Generic;
using System.Text;

using Infosys.Solutions.CodeGeneration.Framework;

namespace Infosys.Lif.LegacyWorkbench.CodeProviders.ProjectFiles
{
    internal class ContractDataEntityProjectCP : ContentProvider
    {

        private string rootNameSpace;
        private string dataEntityRootNameSpace;

        [PlaceHolder("RootNameSpace")]
        internal string RootNameSpace
        {
            get { return rootNameSpace; }
        }

        [PlaceHolder("DataEntityRootNameSpace")]
        internal string DataEntityRootNameSpace
        {
            get { return dataEntityRootNameSpace; }
        }

        Guid dataEntityProjectGuid;
        Entities.GenericCollection<Entities.ContractModule> objectForCodeGeneration;
        internal ContractDataEntityProjectCP(Guid projectGuid,
            Entities.GenericCollection<Entities.ContractModule> contractModules,
            string contractEntityRootNS,
            string ModelObjectEntityRootNamespace)
        {
            rootNameSpace = contractEntityRootNS;
            objectForCodeGeneration = contractModules;

            dataEntityRootNameSpace = ModelObjectEntityRootNamespace;
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
                foreach (Entities.ContractModule module in objectForCodeGeneration)
                {
                    foreach (Entities.Contract entity in module.Contracts)
                    {
                        FileIncludeCP codeGenerator = new FileIncludeCP(module.Name, entity.ContractName);
                        codeGenerator.ContentTemplate = template;
                        sb.Append(codeGenerator.GenerateContent());
                    }
                }
                return sb.ToString();
            }
        }
        //RSS 
        Guid MOEntityProject;

        [PlaceHolder("GuidModelObjectDataEntityAssembly")]
        string moEntityProject
        {
            get
            {
                return MOEntityProject.ToString();
            }
        }

        internal ContractDataEntityProjectCP(Guid projectGuid,
    Entities.GenericCollection<Entities.ContractModule> contractModules,
    string contractEntityRootNS,
    string ModelObjectEntityRootNamespace, Guid ModelObjectEntityProjectGuid )
        {
            rootNameSpace = contractEntityRootNS;
            objectForCodeGeneration = contractModules;

            dataEntityRootNameSpace = ModelObjectEntityRootNamespace;
            MOEntityProject = ModelObjectEntityProjectGuid;
        }
    }
}
