using System;
using System.Collections.Generic;
using System.Text;

using Infosys.Solutions.CodeGeneration.Framework;

namespace Infosys.Lif.LegacyWorkbench.CodeProviders.ProjectFiles
{
    class WFActivityProjectCP : ContentProvider
    {

        System.Collections.Hashtable contractsModuleHashTable;
        Entities.GenericCollection<string> listOfServices;
        internal WFActivityProjectCP(Guid _projectGuid,
            Entities.GenericCollection<string> serviceList,
            System.Collections.Hashtable contractMergeTable, 
            string contractRootNS,
            string hostAccessRootNS
            )
        {
            projectGuid = _projectGuid;
            listOfServices = serviceList;
            contractsModuleHashTable = contractMergeTable;
            rootNamespace = hostAccessRootNS;
            contractEntitiesRootNameSpace = contractRootNS;
        }
        Guid projectGuid;
        [PlaceHolder("ProjectGuid")]
        string ProjectGuid
        {
            get
            {
                return projectGuid.ToString();
            }
        }


        [PlaceHolder("FileIncludes")]
        string FileIncludes
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                Template template
                    = ContentTemplate.RepeatingTemplate(FileIncludeCP.TemplateName);
                foreach (string strServiceName in listOfServices)
                {
                    FileIncludeCP codeGenerator
                        = new FileIncludeCP(((Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractsModuleHashTable[strServiceName])[0].Module.Name,
                        strServiceName);
                    codeGenerator.ContentTemplate = template;
                    sb.Append(codeGenerator.GenerateContent());
                }
                return sb.ToString();
            }
        }

        string contractEntitiesRootNameSpace;
        [PlaceHolder("ContractEntitiesRootNameSpace")]
        string ContractEntitiesRootNameSpace
        {
            get
            {
                return contractEntitiesRootNameSpace;
            }
        }


        string rootNamespace;
        [PlaceHolder("RootNamespace")]
        string RootNamespace
        {
            get
            {
                return rootNamespace;
            }
        }
    }
}

