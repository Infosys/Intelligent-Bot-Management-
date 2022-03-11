using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using Infosys.Solutions.CodeGeneration.Framework;

namespace Infosys.Lif.LegacyWorkbench.CodeProviders.HostCallers
{
    internal class WCFHostCP : ContentProvider
    {
        Entities.GenericCollection<ContractModuleMapping> contractsMapping;
        string projPrefix = string.Empty;
        string hostNamespace = string.Empty;

        [PlaceHolder("ServiceContractNamespace")]
        string ServiceContractNamespace
        {
            get
            {
                if (projPrefix != null || projPrefix != string.Empty)
                {
                    return projPrefix.Trim() + hostNamespace + ".ServiceContract";
                }
                else
                {
                    return hostNamespace + ".ServiceContract";
                }

            }
        }

        [PlaceHolder("ServiceImplementationNamespace")]
        string ServiceImplementationNamespace
        {
            get
            {
                if (projPrefix != null || projPrefix != string.Empty)
                {
                    return projPrefix.Trim() + hostNamespace + ".ServiceImplementation";
                }
                else
                {
                    return hostNamespace + ".ServiceImplementation";
                }

            }
        }

        [PlaceHolder("MethodBodies")]
        string MethodBodies
        {

            get
            {
                StringBuilder sb = new StringBuilder();                
                int counter = 0;
                int listCounter = 0;
                foreach (string strKey in contractsMappingHashTable.Keys)
                {                    
                    Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                        (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractsMappingHashTable[strKey];                    
                    
                    string templateName = "MethodTemplate";
                    switch (contractMapping[counter].Contract.MethodType)
                    {

                        case Entities.Contract.ContractMethodType.Insert:

                            if (contractMapping[counter].Contract.MethodName == null
                                || contractMapping[counter].Contract.MethodName.Length == 0)
                            {
                                contractMapping[counter].Contract.MethodName = "Insert";
                            }

                            break;

                        case Entities.Contract.ContractMethodType.Update:

                            if (contractMapping[counter].Contract.MethodName == null
                                || contractMapping[counter].Contract.MethodName.Length == 0)
                            {
                                contractMapping[counter].Contract.MethodName = "Update";
                            }

                            break;

                        case Entities.Contract.ContractMethodType.Delete:

                            if (contractMapping[counter].Contract.MethodName == null
                                || contractMapping[counter].Contract.MethodName.Length == 0)
                            {
                                contractMapping[counter].Contract.MethodName = "Delete";
                            }

                            break;

                        default:

                            if (contractMapping[counter].Contract.MethodName == null
                                || contractMapping[counter].Contract.MethodName.Length == 0)
                            {
                                contractMapping[counter].Contract.MethodName = "Select";
                            }

                            break;
                    }

                    Template template = ContentTemplate.RepeatingTemplate(templateName);

                    MethodTemplateCp codeProvider = new MethodTemplateCp(contractMapping[counter].Module, 
                        contractMapping[counter].Contract, template, projPrefix, 
                        hostNamespaceList[listCounter++].ToString());
                    sb.Append(codeProvider.GenerateContent());                    
                }

                return sb.ToString();
            }
        }
        
        Hashtable contractsMappingHashTable;
        ArrayList hostNamespaceList;

        internal WCFHostCP(Hashtable contractModuleMappings, Template contentTemplate,
            string _projPrefix, ArrayList _hostNamespace)
        {

            contractsMappingHashTable = contractModuleMappings;
            ContentTemplate = contentTemplate;
            projPrefix = _projPrefix;
            hostNamespaceList = _hostNamespace;
        }
    }

    class MethodTemplateCp : ContentProvider
    {
        Entities.Contract contractForGeneration;
        Entities.ContractModule moduleForGeneration;
        string projectPrefix;
        string hostNamespace;

        internal MethodTemplateCp(Entities.ContractModule contractModule,
            Entities.Contract contract, Template contentTemplate, string projPrefix, string hostAccessNamespace)
        {
            this.ContentTemplate = contentTemplate;
            this.contractForGeneration = contract;
            this.moduleForGeneration = contractModule;
            this.projectPrefix = projPrefix;
            this.hostNamespace = hostAccessNamespace;
        }

        [PlaceHolder("ServiceContractNamespace")]
        string ServiceContractNamespace
        {
            get
            {
                if (projectPrefix != null || projectPrefix != string.Empty)
                {
                    return projectPrefix.Trim() + hostNamespace + ".ServiceContract";
                }
                else
                {
                    return hostNamespace + ".ServiceContract";
                }

            }
        }

        [PlaceHolder("ServiceImplementationNamespace")]
        string ServiceImplementationNamespace
        {
            get
            {
                if (projectPrefix != null || projectPrefix != string.Empty)
                {
                    return projectPrefix.Trim() + hostNamespace + ".ServiceImplementation";
                }
                else
                {
                    return hostNamespace + ".ServiceImplementation";
                }

            }
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

        [PlaceHolder("TransacID")]
        string TransacID
        {
            get
            {
                if (contractForGeneration.TransactionId == null || contractForGeneration.TransactionId.Length == 0)
                {
                    return contractForGeneration.ContractName;
                }
                else
                {
                    return contractForGeneration.TransactionId;
                }
            }
        }

        [PlaceHolder("OperType")]
        string OperType
        {
            get
            {
                if (contractForGeneration.MethodType.ToString().Length == 0)
                {
                    return contractForGeneration.ContractName;
                }
                else
                {
                    return contractForGeneration.MethodType.ToString().Substring(0, 1);
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
                //use full contract name
                return contractForGeneration.ContractName;//.Substring(0,4);                     
            }
        }
    }
}
