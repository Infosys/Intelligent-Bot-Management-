using System;
using System.Collections.Generic;
using System.Text;

using Infosys.Solutions.CodeGeneration.Framework;

namespace Infosys.Lif.LegacyWorkbench.CodeProviders.ProjectFiles
{
    internal class ContractSerializerProjectCP : ContentProvider
    {

        private string rootNameSpace;

        [PlaceHolder("RootNameSpace")]
        internal string RootNameSpace
        {
            get { return rootNameSpace; }
        }


        string contractDataEntityRootNameSpace;

        [PlaceHolder("ContractDataEntityRootNameSpace")]
        string ContractDataEntityRootNameSpace
        {
            get
            {
                return contractDataEntityRootNameSpace;
            }
        }

        string modelObjectSerializerRootNameSpace;

        [PlaceHolder("ModelObjectSerializerRootNameSpace")]
        string ModelObjectSerializerRootNameSpace
        {
            get
            {
                return modelObjectSerializerRootNameSpace;
            }
        }

        string modelObjectEntityRootNameSpace;

        [PlaceHolder("ModelObjectDataEntityRootNameSpace")]
        string ModelObjectDataEntityRootNameSpace
        {
            get
            {
                return modelObjectEntityRootNameSpace;
            }
        }

        Guid dataEntityProjectGuid;
        Entities.GenericCollection<Entities.ContractModule> objectForCodeGeneration;
        internal ContractSerializerProjectCP(Guid projectGuid,
            Entities.GenericCollection<Entities.ContractModule> contractModules,
            string ModelObjectdataEntityRootNS,
            string ModelObjectSerializerRootNS,
            string contractDataEntityRootNS,
            string contractSerializerRootNS
            )
        {

            modelObjectSerializerRootNameSpace = ModelObjectSerializerRootNS;
            modelObjectEntityRootNameSpace = ModelObjectdataEntityRootNS;
            contractDataEntityRootNameSpace = contractDataEntityRootNS ;
            rootNameSpace = contractSerializerRootNS;
            objectForCodeGeneration = contractModules;
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
        Guid ContEntityProject;
        Guid MOSerializerProject;

        [PlaceHolder("GuidModelObjectDataEntityAssembly")]
        string moEntityProject
        {
            get
            {
                return MOEntityProject.ToString();
            }
        }

        [PlaceHolder("GuidContractDataEntityAssembly")]
        string contEntityProject
        {
            get
            {
                return ContEntityProject.ToString();
            }
        }

        [PlaceHolder("GuidModelObjectSerializerAssembly")]
        string moSerializerProject
        {
            get
            {
                return MOSerializerProject.ToString();
            }
        }




        internal ContractSerializerProjectCP(Guid projectGuid,
    Entities.GenericCollection<Entities.ContractModule> contractModules,
    string ModelObjectdataEntityRootNS,
    string ModelObjectSerializerRootNS,
    string contractDataEntityRootNS,
    string contractSerializerRootNS,
            Guid ContractDataEntity,
            Guid ModelObjectDataEntity,
            Guid ModelObjectSerializer
    )
        {

            modelObjectSerializerRootNameSpace = ModelObjectSerializerRootNS;
            modelObjectEntityRootNameSpace = ModelObjectdataEntityRootNS;
            contractDataEntityRootNameSpace = contractDataEntityRootNS;
            rootNameSpace = contractSerializerRootNS;
            objectForCodeGeneration = contractModules;
            MOEntityProject = ModelObjectDataEntity;
            ContEntityProject = ContractDataEntity;
            MOSerializerProject = ModelObjectSerializer;
        }
    }
}
