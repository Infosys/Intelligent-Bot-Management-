using System;
using System.Collections.Generic;
using System.Text;

using Infosys.Solutions.CodeGeneration.Framework;

namespace Infosys.Lif.LegacyWorkbench.CodeProviders.ProjectFiles
{
    internal class WCFSolutionCP : ContentProvider
    {
        const string webSiteGuid="FAE04EC0-301F-11D3-BF4B-00C04F79EFBC";

        Guid solution;
        [PlaceHolder("Solution")]
        internal string Solution
        {            
            get { return (webSiteGuid); }
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

        Guid guidProjectServiceContract;
        [PlaceHolder("GuidProjectServiceContract")]
        internal string GuidProjectServiceContract
        {
            get { return guidProjectServiceContract.ToString(); }
        }

        Guid guidProjectServiceImplementation;
        [PlaceHolder("GuidProjectServiceImplementation")]
        internal string GuidProjectServiceImplementation
        {
            get { return guidProjectServiceImplementation.ToString(); }
        }

        Guid guidModelObjectDataEntityAssembly;
        [PlaceHolder("GuidModelObjectDataEntityAssembly")]
        internal string GuidModelObjectDataEntityAssembly
        {
            get { return guidModelObjectDataEntityAssembly.ToString(); }
        }

        Guid guidContractSerializerAssembly;
        [PlaceHolder("GuidContractSerializerAssembly")]
        internal string GuidContractSerializerAssembly
        {
            get { return guidContractSerializerAssembly.ToString(); }
        }

        Guid guidModelObjectSerializerAssembly;
        [PlaceHolder("GuidModelObjectSerializerAssembly")]
        internal string GuidModelObjectSerializerAssembly
        {
            get { return guidModelObjectSerializerAssembly.ToString(); }
        }


        string contractsEntityRootNameSpace;
        [PlaceHolder("ContractsEntityRootNameSpace")]
        internal string ContractsEntityRootNameSpace
        {
            get { return contractsEntityRootNameSpace; }
        }

        string modelObjectsEntityRootNameSpace;
        [PlaceHolder("ModelObjectsEntityRootNameSpace")]
        internal string ModelObjectsEntityRootNameSpace
        {
            get { return modelObjectsEntityRootNameSpace; }
        }

        string contractsSerializerRootNameSpace;
        [PlaceHolder("ContractsSerializerRootNameSpace")]
        internal string ContractsSerializerRootNameSpace
        {
            get { return contractsSerializerRootNameSpace; }
        }

        string modelObjectsSerializerRootNameSpace;
        [PlaceHolder("ModelObjectsSerializerRootNameSpace")]
        internal string ModelObjectsSerializerRootNameSpace
        {
            get { return modelObjectsSerializerRootNameSpace; }
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

        Guid guidHost;
        [PlaceHolder("GuidHost")]
        internal string GuidHost
        {
            get { return guidHost.ToString(); }
        }

        internal WCFSolutionCP(Guid Soln, Guid ContractDataEntityAssembly, Guid ProjectDataContract, Guid ProjectServiceContract,
            Guid ProjectServiceImplementation, Guid ModelObjectDataEntityAssembly, Guid ContractSerializerAssembly,
            Guid ModelObjectSerializerAssembly, string contractsEntityRootNameS, string modelObjectsEntityRootNameS,
            string contractsSerializerRootNameS, string modelObjectsSerializerRootNameS, string legacyDataContractNameS, 
            string serviceContractNameS, string serviceImplementationNameS, Guid GuidHost)
        {
            solution = Soln;
            guidContractDataEntityAssembly = ContractDataEntityAssembly;
            guidProjectDataContract = ProjectDataContract;
            guidProjectServiceContract = ProjectServiceContract;
            guidProjectServiceImplementation = ProjectServiceImplementation;
            guidModelObjectDataEntityAssembly = ModelObjectDataEntityAssembly;
            guidContractSerializerAssembly = ContractSerializerAssembly;
            guidModelObjectSerializerAssembly = ModelObjectSerializerAssembly;
            contractsEntityRootNameSpace = contractsEntityRootNameS;
            modelObjectsEntityRootNameSpace = modelObjectsEntityRootNameS;
            contractsSerializerRootNameSpace = contractsSerializerRootNameS;
            modelObjectsSerializerRootNameSpace = modelObjectsSerializerRootNameS;
            legacyDataContractNamespace = legacyDataContractNameS;
            serviceContractNamespace = serviceContractNameS;
            serviceImplementationNamespace = serviceImplementationNameS;
            guidHost = GuidHost;
        }
        
    }
}
