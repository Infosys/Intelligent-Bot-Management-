using System;
using System.Collections.Generic;
using System.Text;

using Infosys.Solutions.CodeGeneration.Framework;


namespace Infosys.Lif.LegacyWorkbench.CodeProviders.WFActivity
{    
    internal class ContractModuleMapping
    {
        Entities.Contract contract;

        public Entities.Contract Contract
        {
            get { return contract; }
            set { contract = value; }
        }


        Entities.ContractModule module;

        public Entities.ContractModule Module
        {
            get { return module; }
            set { module = value; }
        }
    }

    internal class WFActivityCP : ContentProvider
    {

        Entities.GenericCollection<ContractModuleMapping> contractsMapping;

        string hostNameSpace;
        internal WFActivityCP(Entities.GenericCollection<ContractModuleMapping> contractModuleMappings,
            Template contentTemplate, string _hostNameSpace)
        {
            contractsMapping = contractModuleMappings;
            ContentTemplate = contentTemplate;
            hostNameSpace = _hostNameSpace;
        }

        [PlaceHolder("ImportsFilled")]
        string ImportsFilled
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (ContractModuleMapping contractMapping in contractsMapping)
                {
                    NameSpacesImportCp codeProvider = new NameSpacesImportCp();
                    codeProvider.ContentTemplate = ContentTemplate.RepeatingTemplate("NameSpacesImport");
                    codeProvider.ContractEntityNamespace = Framework.Helper.BuildNamespace(contractMapping.Module.DataEntityNamespace, contractMapping.Contract);
                    codeProvider.ContractName = contractMapping.Contract.ContractName;
                    sb.Append(codeProvider.GenerateContent());
                }
                return sb.ToString();
            }
        }


        [PlaceHolder("HostAccessNameSpace")]
        string HostAccessNameSpace
        {
            get
            {
                return hostNameSpace;
            }
        }


        [PlaceHolder("ContractName")]
        string ContractName
        {
            get
            {
                return contractsMapping[0].Contract.ContractName.Substring(0, 4);
            }
        }

        class NameSpacesImportCp : ContentProvider
        {
            string _contractEntityNamespace;
            [PlaceHolder("ContractEntityNamespace")]
            internal string ContractEntityNamespace
            {
                get
                {
                    return _contractEntityNamespace;
                }
                set
                {
                    _contractEntityNamespace = value;
                }
            }

            string _contractName;
            [PlaceHolder("ContractName")]
            internal string ContractName
            {
                get
                {
                    return _contractName;
                }
                set
                {
                    _contractName = value;
                }
            }
        }



        

        [PlaceHolder("MethodBodies")]
        string MethodBodies
        {

            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (ContractModuleMapping contractMapping in contractsMapping)
                {

                    string templateName = "MethodTemplate";
                    switch (contractMapping.Contract.MethodType)
                    {

                        case Entities.Contract.ContractMethodType.Insert:
                            if (contractMapping.Contract.MethodName == null
                                || contractMapping.Contract.MethodName.Length == 0)
                            {
                                contractMapping.Contract.MethodName = "Insert";
                            }
                            break;
                        case Entities.Contract.ContractMethodType.Update:
                            if (contractMapping.Contract.MethodName == null
                                || contractMapping.Contract.MethodName.Length == 0)
                            {
                                contractMapping.Contract.MethodName = "Update";
                            }

                            break;
                        case Entities.Contract.ContractMethodType.Delete:
                            if (contractMapping.Contract.MethodName == null
                                || contractMapping.Contract.MethodName.Length == 0)
                            {
                                contractMapping.Contract.MethodName = "Delete";
                            }

                            break;
                        default:
                            if (contractMapping.Contract.MethodName == null
                                || contractMapping.Contract.MethodName.Length == 0)
                            {
                                contractMapping.Contract.MethodName = "Select";
                            }

                            break;
                    }

                    Template template = ContentTemplate.RepeatingTemplate(templateName);

                    MethodTemplateCp codeProvider = new MethodTemplateCp(contractMapping.Module, contractMapping.Contract
                        , template);
                    sb.Append(codeProvider.GenerateContent());
                }

                return sb.ToString();
            }
        }
        class MethodTemplateCp : ContentProvider
        {
            Entities.Contract contractForGeneration;
            Entities.ContractModule moduleForGeneration;
            internal MethodTemplateCp(Entities.ContractModule contractModule,
                Entities.Contract contract, Template contentTemplate)
            {
                this.ContentTemplate = contentTemplate;
                this.contractForGeneration = contract;
                this.moduleForGeneration = contractModule;
            }

            [PlaceHolder("ServiceName")]
            string ServiceName
            {
                get
                {
                    if (contractForGeneration.ServiceName == null || contractForGeneration.ServiceName.Length == 0)
                    {
                        return contractForGeneration.ContractName;
                    }
                    else
                    {
                        return contractForGeneration.ServiceName;
                    }
                }
            }

            [PlaceHolder("ContractEntityName")]
            string ContractEntityName
            {
                get
                {
                    return contractForGeneration.ContractName;
                }
            }
            

            [PlaceHolder("MethodName")]
            string MethodName
            {
                get
                {
                    return contractForGeneration.MethodName;
                }
            }

            [PlaceHolder("ContractName")]
            string ContractName
            {
                get
                {
                    return contractForGeneration.ContractName;
                }
            }
        }
    }
}
