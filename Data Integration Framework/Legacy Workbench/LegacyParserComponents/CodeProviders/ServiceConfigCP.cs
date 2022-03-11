using System;
using System.Collections.Generic;
using System.Text;

using Infosys.Solutions.CodeGeneration.Framework;

namespace Infosys.Lif.LegacyWorkbench.CodeProviders
{
    internal class ServiceConfigCP:ContentProvider
    {

        Entities.GenericCollection<Entities.ContractModule> contractsGroup;
        string contractXmlName;

        internal ServiceConfigCP(Entities.GenericCollection<Entities.ContractModule> contractModules, string contractXmlNamespace)
        {
            contractsGroup = contractModules;
            contractXmlName = contractXmlNamespace;
        }

        [PlaceHolder("ServicesFilled")]
        string ServicesFilled
        {
            get
            {
                StringBuilder sb = new StringBuilder();                
                foreach (Entities.ContractModule module in contractsGroup)
                {                    
                    foreach (Entities.Contract contract in module.Contracts)
                    {                        
                        ServiceElementCP codeGenerator = new ServiceElementCP(contract, contractXmlName);
                        codeGenerator.ContentTemplate = ContentTemplate.RepeatingTemplate("ServiceElement");
                        sb.Append(codeGenerator.GenerateContent());
                    }
                }
                return sb.ToString();
            }
        }

        internal class ServiceElementCP : ContentProvider
        {

            Entities.Contract objectToBeGeneratedFor;
            string contractXmlNamespace;
            internal ServiceElementCP(Entities.Contract contract, string contXmlNamespace)
            {
                objectToBeGeneratedFor = contract;
                contractXmlNamespace = contXmlNamespace;
            }
            [PlaceHolder("ServiceName")]
            string ServiceName
            {
                get
                {
                    if(objectToBeGeneratedFor.ServiceName==null ||
                        objectToBeGeneratedFor.ServiceName.Length==0)
                    {
                        return objectToBeGeneratedFor.ContractName;
                    }
                    return objectToBeGeneratedFor.ServiceName;
                }
            }


            [PlaceHolder("TransactionId")]
            string TransactionId
            {
                get
                {
                    if (objectToBeGeneratedFor.TransactionId == null)
                    {
                        objectToBeGeneratedFor.TransactionId = string.Empty;
                    }
                    return objectToBeGeneratedFor.TransactionId;
                }
            }

            [PlaceHolder("ContractXml")]
            string ContractXml
            {
                get
                {
                    if (contractXmlNamespace.Contains("#EntityName#"))
                    {
                        contractXmlNamespace = contractXmlNamespace.Replace("#EntityName#", string.Empty);
                    }                    
                    return contractXmlNamespace;
                }
            }
        }
    }
}
