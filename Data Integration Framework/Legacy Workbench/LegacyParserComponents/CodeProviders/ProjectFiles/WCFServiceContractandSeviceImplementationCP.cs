using System;
using System.Collections.Generic;
using System.Text;
using Infosys.Solutions.CodeGeneration.Framework;

namespace Infosys.Lif.LegacyWorkbench.CodeProviders.ProjectFiles
{
    internal class WCFServiceContractandSeviceImplementationCP : ContentProvider
    {
        Guid projectGuid;

        [PlaceHolder("ProjectGuid")]
        internal string ProjectGuid
        {
            get { return projectGuid.ToString(); }
        }

        //string fileName;

        //[PlaceHolder("FileName")]
        //internal string FileName
        //{
        //    get { return fileName; }
        //}

        [PlaceHolder("FileIncludes")]
        string FileIncludes
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                Template template = ContentTemplate.RepeatingTemplate(FileIncludeCP.TemplateName);
                foreach (Entities.ContractModule module in projContractModules)
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

        Guid guidModelObjectDataEntityAssembly;        

        [PlaceHolder("GuidModelObjectDataEntityAssembly")]
        internal string GuidModelObjectDataEntityAssembly
        {
            get { return guidModelObjectDataEntityAssembly.ToString(); }
        }

        Guid guidProjectServiceContract;

        [PlaceHolder("GuidProjectServiceContract")]
        internal string GuidProjectServiceContract
        {
            get { return guidProjectServiceContract.ToString(); }
        }


        Guid guidContractDataEntityAssembly;
        [PlaceHolder("GuidContractDataEntityAssembly")]
        internal string GuidContractDataEntityAssembly
        {
            get { return guidContractDataEntityAssembly.ToString(); }
        }

        Guid guidProjectDataContract;
        [PlaceHolder("GuidProjectDataContract")]
        internal string GuidProjectDataContract
        {
            get { return guidProjectDataContract.ToString(); }
        }

        string legacyDataContractNamespace;
        [PlaceHolder("LegacyDataContractNamespace")]
        public string LegacyDataContractNamespace
        {
            get { return legacyDataContractNamespace; }
            set { legacyDataContractNamespace = value; }
        }

        string serviceContractNamespace;
        [PlaceHolder("ServiceContractNamespace")]
        public string ServiceContractNamespace
        {
            get { return serviceContractNamespace; }
            set { serviceContractNamespace = value; }
        }

        string serviceImplementationNamespace;
        [PlaceHolder("ServiceImplementationNamespace")]
        public string ServiceImplementationNamespace
        {
            get { return serviceImplementationNamespace; }
            set { serviceImplementationNamespace = value; }
        }

        string contractDataEntityNamespace;
        [PlaceHolder("ContractDataEntityNamespace")]
        public string ContractDataEntityNamespace
        {
            get { return contractDataEntityNamespace; }
            set { contractDataEntityNamespace = value; }
        }

        string modelDataEntityNamespace;
        [PlaceHolder("ModelDataEntityNamespace")]
        public string ModelDataEntityNamespace
        {
            get
            {
                if (modelDataEntityNamespace == null)
                {
                    modelDataEntityNamespace = "";
                } 
                return modelDataEntityNamespace;
            }
            set { modelDataEntityNamespace = value; }
        }
        
        Entities.GenericCollection<Entities.ContractModule> projContractModules;
        System.Collections.Hashtable contractsModuleHashTable;

        internal WCFServiceContractandSeviceImplementationCP(Guid GetProjectGuid, Guid ContractDEGuid,
            Guid ModelObjDEGuid, Guid DataContractGuid, Guid ServContractGuid,
            Entities.GenericCollection<Entities.ContractModule> contractModules, string legacyDataContractNameS, 
            string serviceContractNameS, string serviceImplementationNameS, string contractDataEntityNameS)
        {
            projectGuid = GetProjectGuid;
            projContractModules = contractModules;
            guidContractDataEntityAssembly = ContractDEGuid;
            guidProjectDataContract = DataContractGuid;
            guidModelObjectDataEntityAssembly = ModelObjDEGuid;
            guidProjectServiceContract = ServContractGuid;
            legacyDataContractNamespace = legacyDataContractNameS;
            serviceContractNamespace = serviceContractNameS;
            serviceImplementationNamespace = serviceImplementationNameS;
            contractDataEntityNamespace = contractDataEntityNameS;            
        }

        internal WCFServiceContractandSeviceImplementationCP(Guid GetProjectGuid, Guid ContractDEGuid,
            Guid ModelObjDEGuid, Guid DataContractGuid, Guid ServContractGuid,
            Entities.GenericCollection<Entities.ContractModule> contractModules, string legacyDataContractNameS,
            string serviceContractNameS, string serviceImplementationNameS, string contractDataEntityNameS, string modelDataEntityNameS)
        {
            projectGuid = GetProjectGuid;
            projContractModules = contractModules;
            guidContractDataEntityAssembly = ContractDEGuid;
            guidProjectDataContract = DataContractGuid;
            guidModelObjectDataEntityAssembly = ModelObjDEGuid;
            guidProjectServiceContract = ServContractGuid;
            legacyDataContractNamespace = legacyDataContractNameS;
            serviceContractNamespace = serviceContractNameS;
            serviceImplementationNamespace = serviceImplementationNameS;
            contractDataEntityNamespace = contractDataEntityNameS;
            modelDataEntityNamespace = modelDataEntityNameS;
        }

        internal WCFServiceContractandSeviceImplementationCP(Guid GetProjectGuid, Guid ContractDEGuid,
            Guid DataContractGuid, Entities.GenericCollection<Entities.ContractModule> contractModules,
            string legacyDataContractNameS, string serviceContractNameS, string contractDataEntityNameS)
        {
            projectGuid = GetProjectGuid;            
            projContractModules = contractModules;
            guidContractDataEntityAssembly = ContractDEGuid;
            guidProjectDataContract = DataContractGuid;
            legacyDataContractNamespace = legacyDataContractNameS;
            serviceContractNamespace = serviceContractNameS;
            contractDataEntityNamespace = contractDataEntityNameS;
            serviceImplementationNamespace = string.Empty;
        }

        internal WCFServiceContractandSeviceImplementationCP(Guid GetProjectGuid, Guid ContractDEGuid,
            Guid DataContractGuid, Guid ServiceContractGuid, Entities.GenericCollection<Entities.ContractModule> contractModules,
            string legacyDataContractNameS, string serviceContractNameS, string serviceImplementationNameS, string contractDataEntityNameS)
        {
            projectGuid = GetProjectGuid;            
            projContractModules = contractModules;
            guidContractDataEntityAssembly = ContractDEGuid;
            guidProjectDataContract = DataContractGuid;
            guidProjectServiceContract = ServiceContractGuid;
            legacyDataContractNamespace = legacyDataContractNameS;
            serviceContractNamespace = serviceContractNameS;
            serviceImplementationNamespace = serviceImplementationNameS;
            contractDataEntityNamespace = contractDataEntityNameS;
        }        
    }
}
