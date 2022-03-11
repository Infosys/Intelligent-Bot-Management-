using System;
using System.Collections.Generic;
using System.Text;

using Infosys.Solutions.CodeGeneration.Framework;

namespace Infosys.Lif.LegacyWorkbench.CodeProviders
{
    internal class LegacyFacadeConfigFileCP : ContentProvider
    {
        Entities.Project objectForCodeGeneration;
        string projectType;
        internal LegacyFacadeConfigFileCP(Entities.Project projectForConfig, string projType)
        {
            objectForCodeGeneration = projectForConfig;
            projectType = projType;
        }

        const string hostAccessRelativePath = @"..\..";
        const string webServiceRelativePath = @"..\..\..";

        [PlaceHolder("RelativePath")]
        string RelativePath
        {
            get
            {
                if (projectType.Trim().ToLowerInvariant().Equals("hostaccess"))
                {
                    return hostAccessRelativePath;
                }
                else
                {
                    return webServiceRelativePath;
                }
            }
        }

        [PlaceHolder("ServiceTemplatesFilled")]
        string ServiceTemplatesFilled
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                Template templateProvided = ContentTemplate.RepeatingTemplate(ServiceTemplateCP.TemplateName);

                foreach (Entities.ContractModule contractModule in objectForCodeGeneration.ContractModules)
                {
                    foreach (Entities.Contract contract in contractModule.Contracts)
                    {
                        if (contract.IsToBeGenerated)
                        {

                            ServiceTemplateCP codeProvider = new ServiceTemplateCP(
                                objectForCodeGeneration,
                                contractModule, contract, projectType);
                            codeProvider.ContentTemplate = templateProvided;

                            sb.Append(codeProvider.GenerateContent());
                        }
                    }
                }


                return sb.ToString();
            }
        }

        class ServiceTemplateCP : ContentProvider
        {
            internal const string TemplateName = "ServiceTemplate";
            Entities.Contract objectForCodeGeneration;
            Entities.ContractModule objectModuleForCodeGeneration;
            Entities.Project objectProjectForCodeGeneration;
            string projectType;


            internal ServiceTemplateCP(Entities.Project projectObject,
                Entities.ContractModule contractModule,
                Entities.Contract contractObject,
                string projType)
            {
                objectProjectForCodeGeneration = projectObject;
                objectModuleForCodeGeneration = contractModule;
                objectForCodeGeneration = contractObject;
                projectType = projType;
            }

            [PlaceHolder("SerializerName")]
            string SerializerName
            {
                get
                {
                    return objectForCodeGeneration.ContractName;
                }
            }


            [PlaceHolder("ServiceName")]
            string ServiceName
            {
                get
                {
                    if (objectForCodeGeneration.ServiceName == null || objectForCodeGeneration.ServiceName.Length == 0)
                    {
                        return objectForCodeGeneration.ContractName;
                    }

                    return objectForCodeGeneration.ServiceName;
                }
            }

            [PlaceHolder("RelativePath")]
            string RelativePath
            {
                get
                {
                    if (projectType.Trim().ToLowerInvariant().Equals("hostaccess"))
                    {
                        return hostAccessRelativePath;
                    }
                    else
                    {
                        return webServiceRelativePath;
                    }
                }
            }

            [PlaceHolder("SerializerNameSpace")]
            string SerializerNameSpace
            {
                get
                {
                    return objectModuleForCodeGeneration.SerializerNamespace;
                }
            }

            [PlaceHolder("SerializerRootNameSpace")]
            string SerializerRootNameSpace
            {
                get
                {
                    return objectProjectForCodeGeneration.ContractNamespaces.SerializerRootNamespace;
                }
            }


        }
    }
}
