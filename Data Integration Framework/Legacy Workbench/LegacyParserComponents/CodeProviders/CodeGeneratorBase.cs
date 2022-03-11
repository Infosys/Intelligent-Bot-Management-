using System;
using System.Collections.Generic;
using System.Text;
using Infosys.Lif.LegacyWorkbench.Framework;
using Infosys.Solutions.CodeGeneration.Framework;

namespace Infosys.Lif.LegacyWorkbench.CodeProviders
{
    class CodeGeneratorBase
    {
        const string DirectoryPathSeperator = "\\";

        //to hold the name of generated solution
        string SolutionFileName;

        string _xsdObjGenPath;
        public string XsdObjGenPath
        {
            get { return _xsdObjGenPath; }
            set { _xsdObjGenPath = value; }
        }

        static bool isTins;

        public static bool IsTins
        {
            get { return isTins; }
            set { isTins = value; }
        }

        /// <summary>
        /// The Constructor initializes paths of templates directory, working directory and .net 
        /// framework directory.
        /// </summary>
        public CodeGeneratorBase()
        {
            templatesDirectory = System.Windows.Forms.Application.StartupPath + @"\Templates\";

            string workingDirectory = System.Environment.GetEnvironmentVariable("windir");
            DotNetFrameworkDirectory = workingDirectory + @"\Microsoft.NET\Framework\v2.0.50727\";
        }

        string DotNetFrameworkDirectory;
        string templatesDirectory;
        Entities.Project projectToBeGeneratedFor;
        string outputDirectory;        
        //initialize code generation confirmation form.
        CodeGenerationConfirmation frm;

        bool isErrorOccurred = false;

        public bool IsErrorOccurred
        {
            get { return isErrorOccurred; }
            set { isErrorOccurred = value; }
        }

        string errorReason;

        public string ErrorReason
        {
            get { return errorReason; }
            set { errorReason = value; }
        }

        /// <summary>
        /// Initializes Solution generation with tasks like creating namespace mappings, 
        /// showing code generation confirmation form and taking user inputs, copying 
        /// references and base XSD files of Model object and contract.
        /// </summary>
        /// <param name="codeProviderProject"></param>
        /// <param name="xsdObjGenPath"></param>
        public void InitializeBase(Entities.Project codeProviderProject, string xsdObjGenPath)
        {
            _xsdObjGenPath = xsdObjGenPath;

            DataEntities.ContractDataEntityCP.Initialize();


            XsdObjectGeneratorWrapper.ModelObjectNamespaceMapping = new System.Collections.Hashtable();
            Serializers.ContractSerializerCP.ForLoopCP.ModelObjectModuleMapping = 
                new System.Collections.Hashtable();
            Serializers.CobolContractSerializerCP.ForLoopCP.ModelObjectModuleMapping_cobol = 
                new System.Collections.Hashtable();
            ModelObjectDetailsMapping = new System.Collections.Hashtable();
            ModelObjectHashTable = new System.Collections.Hashtable();
            ModelObjectModuleMapping = new System.Collections.Hashtable();
            Mappings.Initialize();

            projectToBeGeneratedFor = codeProviderProject;
            if (!Validate())
            {
                isErrorOccurred = true;
                System.Windows.Forms.MessageBox.Show(
                    errorReason, 
                    "Legacy Workbench", 
                    System.Windows.Forms.MessageBoxButtons.OK, 
                    System.Windows.Forms.MessageBoxIcon.Information);
                return;
            }

            //show the form in modal view.
            frm = new CodeGenerationConfirmation();
            frm.ShowDialog();

            outputDirectory = frm.OutputDirectory;            
            SolutionFileName = frm.SolnFileName + ".sln";

            /*** yet to test, this error has been handelled in code generation form so commented here */
            //if ((outputDirectory == null || outputDirectory.Length == 0) && frm.GenerationCancelled == false)
            //{
            //    isErrorOccurred = true;
            //    ErrorReason = ("Path for code generation is not supplied");
            //    return;
            //}

            if (frm.GenerationCancelled == true)
            {
                isErrorOccurred = true;
                //ErrorReason = ("User cancelled the code generation");
                return;
            }

            //copy all reference files in the output directory
            string source, destination;
            string referencesDirectory = "References" + DirectoryPathSeperator;

            System.IO.Directory.CreateDirectory(outputDirectory + DirectoryPathSeperator + referencesDirectory);

            {
                string directoryPath = templatesDirectory + @"..\LifReferences";
                string[] strFiles = System.IO.Directory.GetFiles(directoryPath);
                foreach (string strFile in strFiles)
                {
                    char[] splitters = new char[1];
                    splitters[0] = '\\';
                    string[] strSplitFilePath = strFile.Split(splitters);
                    System.IO.File.Copy(strFile, outputDirectory + DirectoryPathSeperator + referencesDirectory + 
                        strSplitFilePath[strSplitFilePath.Length - 1], true);
                }
            }

            System.IO.Directory.CreateDirectory(outputDirectory + DirectoryPathSeperator + referencesDirectory + 
                "LegacyFacade");

            {
                string directoryPath = templatesDirectory + @"..\LifReferences\LegacyFacade";
                string[] strFiles = System.IO.Directory.GetFiles(directoryPath);
                foreach (string strFile in strFiles)
                {
                    char[] splitters = new char[1];
                    splitters[0] = '\\';
                    string[] strSplitFilePath = strFile.Split(splitters);
                    System.IO.File.Copy(strFile, outputDirectory + DirectoryPathSeperator + referencesDirectory + 
                        "LegacyFacade" + DirectoryPathSeperator + strSplitFilePath[strSplitFilePath.Length - 1], true);
                }
            }

            System.IO.Directory.CreateDirectory(outputDirectory + DirectoryPathSeperator + referencesDirectory + 
                "Adapter");

            {
                string directoryPath = templatesDirectory + @"..\LifReferences\Adapter";
                string[] strFiles = System.IO.Directory.GetFiles(directoryPath);
                foreach (string strFile in strFiles)
                {
                    char[] splitters = new char[1];
                    splitters[0] = '\\';
                    string[] strSplitFilePath = strFile.Split(splitters);
                    System.IO.File.Copy(strFile, outputDirectory + DirectoryPathSeperator + referencesDirectory + 
                        "Adapter" + DirectoryPathSeperator + strSplitFilePath[strSplitFilePath.Length - 1], true);
                }
            }

            {
                string fileName = "ContractBase.xsd";
                source = fileName;
                destination = referencesDirectory + fileName;
                CopyFile(source, destination);
            }

            {
                string fileName = "ModelObjectBase.xsd";
                source = fileName;
                destination = referencesDirectory + fileName;
                CopyFile(source, destination);
            }
            
            //create namespace mappings
            foreach (Entities.ContractModule module in projectToBeGeneratedFor.ContractModules)
            {
                foreach (Entities.Contract contract in module.Contracts)
                {
                    AddToEntitiesList(contract);
                    if (isErrorOccurred)
                    {
                        return;
                    }

                    {
                        string mappingsKey = "Contract:" + module.Name + ":" + contract.ContractName;
                        if (!Mappings.CSharpNameSpaceMappings.Contains(mappingsKey))
                        {
                            Mappings.CSharpNameSpaceMappings.Add(
                                mappingsKey,
                                Framework.Helper.BuildNamespace(module.DataEntityNamespace, contract));

                            string xsdNameSpaces = projectToBeGeneratedFor.ContractNamespaces.XmlSchemaNamespace;
                            xsdNameSpaces = Framework.Helper.BuildNamespace(xsdNameSpaces, module);
                            xsdNameSpaces = Framework.Helper.BuildNamespace(xsdNameSpaces, contract);

                            Mappings.XsdNameSpaceMappings.Add(mappingsKey, xsdNameSpaces);
                        }
                    }
                }
            }

            // Build a namespace vs ModelObject table
            foreach (Entities.ModelObjectModule ModelObjectModule in projectToBeGeneratedFor.ModelObjectModules)
            {
                foreach (Entities.Entity ModelObject in ModelObjectModule.ModelObjects)
                {
                    DataEntities.ModelObjectDetailsMapping mapping = new DataEntities.ModelObjectDetailsMapping();
                    mapping.ModelObjectName = ModelObject.EntityName;
                    mapping.ModuleName = ModelObjectModule.Name;
                    ModelObjectDetailsMapping.Add(ModelObject.ProgramId, mapping);

                    {
                        string mappingsKey = "ModelObject:" + ModelObjectModule.Name + ":" + ModelObject.ProgramId;

                        if (!Mappings.CSharpNameSpaceMappings.Contains(mappingsKey))
                        {
                            Mappings.CSharpNameSpaceMappings.Add(mappingsKey,
                                Framework.Helper.BuildNamespace(ModelObjectModule.DataEntityNamespace, ModelObject));

                            string xmlSchemaNameSpace =
                                Framework.Helper.BuildNamespace(projectToBeGeneratedFor.ModelObjectNamespaces.XmlSchemaNamespace, ModelObjectModule);
                            xmlSchemaNameSpace = Framework.Helper.BuildNamespace(xmlSchemaNameSpace, ModelObject);
                            Mappings.XsdNameSpaceMappings.Add(mappingsKey, xmlSchemaNameSpace);
                        }
                    }

                    XsdObjectGeneratorWrapper.ModelObjectNamespaceMapping.Add(ModelObject.EntityName, 
                        ModelObjectModule.DataEntityNamespace.Replace("#EntityName#", ModelObject.EntityName));
                }
            }

            foreach (Entities.ModelObjectModule ModelObjectModule in projectToBeGeneratedFor.ModelObjectModules)
            {
                foreach (Entities.Entity ModelObject in ModelObjectModule.ModelObjects)
                {
                    string childModelObjectProgramId = ModelObject.ProgramId;
                    if (!Serializers.ContractSerializerCP.ForLoopCP.ModelObjectModuleMapping.ContainsKey(
                        childModelObjectProgramId))
                    {
                        Serializers.ContractSerializerCP.ForLoopCP.ModelObjectModuleMapping.Add(
                            childModelObjectProgramId, ModelObjectModule);
                    }
                }
            }

            foreach (Entities.ModelObjectModule ModelObjectModule in projectToBeGeneratedFor.ModelObjectModules)
            {
                foreach (Entities.Entity ModelObject in ModelObjectModule.ModelObjects)
                {
                    string childModelObjectProgramId = ModelObject.ProgramId;
                    if (!Serializers.CobolContractSerializerCP.ForLoopCP.ModelObjectModuleMapping_cobol.ContainsKey(
                        childModelObjectProgramId))
                    {
                        Serializers.CobolContractSerializerCP.ForLoopCP.ModelObjectModuleMapping_cobol.Add(
                            childModelObjectProgramId, ModelObjectModule);
                    }
                }
            }

            Entities.GenericCollection<string> contractNames = new Infosys.Lif.LegacyWorkbench.Entities.GenericCollection<string>();
            foreach (Entities.ContractModule module in projectToBeGeneratedFor.ContractModules)
            {
                foreach (Entities.Contract contract in module.Contracts)
                {
                    if (contract.IsToBeGenerated)
                    {
                        BuildContractModelObjectMapping(contract.ContractName, contract.InputModelObjects);
                        BuildContractModelObjectMapping(contract.ContractName, contract.OutputModelObjects);
                        contractNames.Add(contract.ContractName);
                    }
                }
            }           
        }

        GenerationStatusDisplay statusDisplayWindow = new GenerationStatusDisplay();

        /// <summary>
        /// Builds Mapping of model objects in a contract.
        /// </summary>
        /// <param name="contractName"></param>
        /// <param name="ModelObjectCollection"></param>
        private void BuildContractModelObjectMapping(string contractName, 
            Entities.GenericCollection<Entities.ModelObject> ModelObjectCollection)
        {
            foreach (Entities.ModelObject ModelObject in ModelObjectCollection)
            {
                BuildContractModelObjectMapping(contractName, ModelObject.ModelObjectEntity.ProgramId, 
                    ModelObject.ModelObjects);

            }
        }

        /// <summary>
        /// Overload method : Builds Mapping of model objects in a contract for multiple levels.
        /// </summary>
        /// <param name="contractName"></param>
        /// <param name="parentModelObjectProgramId"></param>
        /// <param name="ModelObjectCollection"></param>
        private void BuildContractModelObjectMapping(string contractName, string parentModelObjectProgramId,
            Entities.GenericCollection<Entities.ModelObject> ModelObjectCollection)
        {
            string keyForMapping = contractName + ":" + parentModelObjectProgramId;

            Entities.GenericCollection<string> listOfChildModelObject;

            if (Mappings.contractModelObjectMapping.ContainsKey(keyForMapping))
            {
                listOfChildModelObject = 
                    (Entities.GenericCollection<string>)Mappings.contractModelObjectMapping[keyForMapping];
            }
            else
            {
                listOfChildModelObject = new Entities.GenericCollection<string>();
                Mappings.contractModelObjectMapping.Add(keyForMapping, listOfChildModelObject);
            }
            foreach (Entities.ModelObject ModelObject in ModelObjectCollection)
            {
                if (!listOfChildModelObject.Contains(ModelObject.Name))
                {
                    listOfChildModelObject.Add(ModelObject.Name);
                }
                BuildContractModelObjectMapping(contractName, ModelObject.ModelObjectEntity.ProgramId, 
                    ModelObject.ModelObjects);
            }

        }

        /// <summary>
        /// Validates namepsaces for model object and contract.
        /// </summary>
        /// <returns></returns>
        private bool Validate()
        {
            foreach (Entities.Module module in projectToBeGeneratedFor.ModelObjectModules)
            {
                if (module.DataEntityNamespace == null || module.DataEntityNamespace.Length == 0)
                {
                    ErrorReason = "ModelObject Module " + module.Name + " does not have a data entity namespace";
                    return false;
                }
                if (module.SerializerNamespace == null || module.SerializerNamespace.Length == 0)
                {
                    ErrorReason = "ModelObject Module " + module.Name + " does not have a serializer namespace";
                    return false;
                }
            }
            foreach (Entities.Module module in projectToBeGeneratedFor.ContractModules)
            {
                if (module.DataEntityNamespace == null || module.DataEntityNamespace.Length == 0)
                {
                    ErrorReason = "Contract Module " + module.Name + " does not have a data entity namespace";
                    return false;
                }
                if (module.SerializerNamespace == null || module.SerializerNamespace.Length == 0)
                {
                    ErrorReason = "Contract Module " + module.Name + " does not have a serializer namespace";
                    return false;
                }
            }

            if (!ValidateNameSpaces(projectToBeGeneratedFor.ModelObjectNamespaces))
            {
                return false;
            }

            if (!ValidateNameSpaces(projectToBeGeneratedFor.ContractNamespaces))
            {
                return false;
            }

            if (ValidateNameSpaceString(projectToBeGeneratedFor.ContractNamespaces.HostAccessRootNamespace))
            {
                ErrorReason = "HostAccessRootNamespace not filled";
                return false;
            }
            if (ValidateNameSpaceString(projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace))
            {
                ErrorReason = "HostAccessNamespace not filled";
                return false;
            }
            return true;
        }

        /// <summary>
        /// Validate project namespaces
        /// </summary>
        /// <param name="projectNamespaces"></param>
        /// <returns>true/false</returns>
        private bool ValidateNameSpaces(Entities.Project.ProjectNamespaces projectNamespaces)
        {
            const string ErrorMessage = " not filled";

            if (ValidateNameSpaceString(projectNamespaces.DataEntityRootNamespace))
            {
                ErrorReason = "DataEntityRootNamespace " + ErrorMessage;
                return false;
            }
            if (ValidateNameSpaceString(projectNamespaces.DataEntityNamespace))
            {
                ErrorReason = "DataEntityNamespace " + ErrorMessage;
                return false;
            }
            if (ValidateNameSpaceString(projectNamespaces.SerializerNamespace))
            {
                ErrorReason = "SerializerNamespace " + ErrorMessage;
                return false;
            }
            if (ValidateNameSpaceString(projectNamespaces.SerializerRootNamespace))
            {
                ErrorReason = "SerializerRootNamespace " + ErrorMessage;
                return false;
            }
            if (ValidateNameSpaceString(projectNamespaces.XmlSchemaNamespace))
            {
                ErrorReason = "XmlSchemaNamespace " + ErrorMessage;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the namespace exists or not
        /// </summary>
        /// <param name="projNamespace"></param>
        /// <returns>true/false</returns>
        private bool ValidateNameSpaceString(string projNamespace)
        {
            return (projNamespace == null || projNamespace.Length == 0);
        }

        System.Collections.Hashtable ModelObjectDetailsMapping = new System.Collections.Hashtable();

        /// <summary>
        /// Add input and out model objects of a Contracts in a list.
        /// </summary>
        /// <param name="contract"></param>
        private void AddToEntitiesList(Entities.Contract contract)
        {
            foreach (Entities.ModelObject ModelObject in contract.InputModelObjects)
            {
                AddToEntitiesList(ModelObject);
            }
            if (contract.OutputModelObjects != null)
            {
                foreach (Entities.ModelObject ModelObject in contract.OutputModelObjects)
                {
                    AddToEntitiesList(ModelObject);
                }
            }
        }

        /// <summary>
        /// overload method : Add model object and its child nodes in the list
        /// </summary>
        /// <param name="ModelObject"></param>
        private void AddToEntitiesList(Entities.ModelObject ModelObject)
        {
            if (ModelObject.ModelObjectEntity == null)
            {
                errorReason = "No entity found for ModelObject " + ModelObject.Name;
                isErrorOccurred = true;
                return;
            }
            string key = ModelObject.ModelObjectEntity.EntityName.ToLowerInvariant();
            Entities.GenericCollection<DataEntities.ImportModelObjectDefinition> childModelObjectsList;
            if (ModelObjectHashTable.ContainsKey(key))
            {
                childModelObjectsList
                    = (Entities.GenericCollection<DataEntities.ImportModelObjectDefinition>)ModelObjectHashTable[key];
            }
            else
            {
                childModelObjectsList = new Entities.GenericCollection<DataEntities.ImportModelObjectDefinition>();
                ModelObjectHashTable.Add(key, childModelObjectsList);
            }

            foreach (Entities.ModelObject childModelObject in ModelObject.ModelObjects)
            {
                AddToEntitiesList(childModelObject);
                DataEntities.ImportModelObjectDefinition importModelObject
                    = new DataEntities.ImportModelObjectDefinition();
                importModelObject.ChildModelObjectModule = 
                    FindModuleForEntity(childModelObject.ModelObjectEntity.ProgramId);
                importModelObject.ChildEntity = childModelObject.ModelObjectEntity;
                importModelObject.ChildModelObject = childModelObject;

                childModelObjectsList.Add(importModelObject);
            }
        }

        System.Collections.Hashtable ModelObjectModuleMapping = new System.Collections.Hashtable();

        /// <summary>
        /// associates the program ID of a model object to its module 
        /// </summary>
        /// <param name="childModelObjectProgramId"></param>
        /// <returns>Model Object Module</returns>
        private Entities.ModelObjectModule FindModuleForEntity(string childModelObjectProgramId)
        {
            foreach (Entities.ModelObjectModule ModelObjectModule in projectToBeGeneratedFor.ModelObjectModules)
            {
                foreach (Entities.Entity childEntity in ModelObjectModule.ModelObjects)
                {
                    if (childEntity.ProgramId == childModelObjectProgramId)
                    {
                        return ModelObjectModule;
                    }
                }
            }
            return null;
        }

        System.Collections.Hashtable ModelObjectHashTable = new System.Collections.Hashtable();
        System.Collections.Hashtable contractMergeHashTable = new System.Collections.Hashtable();

        public Infosys.Lif.LegacyWorkbench.Entities.Project ProjectToBeGeneratedFor
        {
            get
            {
                return projectToBeGeneratedFor;
            }
        }

        //Backup : for TINS
        /// <summary>
        /// Generates Data entity classes for TINS format.
        /// </summary>
        public void GenerateModelObjectDataEntitiesBase_tins()
        {
            isErrorOccurred = true;
            if (frm.PRTDataEntitySchemaModelObjects)
            {
                Template templateProvided =
                    RetrieveTemplate("ModelObjectDataEntity");

                foreach (Entities.ModelObjectModule ModelObjectModule in projectToBeGeneratedFor.ModelObjectModules)
                {
                    foreach (Entities.Entity ModelObject in ModelObjectModule.ModelObjects)
                    {
                        if (ModelObject.IsToBeGenerated)
                        {
                            DataEntities.ModelObjectDataEntityCP cpModelObjectDataEntity
                                = new DataEntities.ModelObjectDataEntityCP(ModelObjectModule, ModelObject, 
                                (Entities.GenericCollection<DataEntities.ImportModelObjectDefinition>)ModelObjectHashTable[ModelObject.EntityName.ToLowerInvariant()], templateProvided);
                            
                            string xsdDirectory = outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory + 
                                DirectoryPathSeperator + ModelObjectsDirectory + DirectoryPathSeperator + "xsd" + 
                                DirectoryPathSeperator + ModelObjectModule.Name;
                            string codeDirectory = outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory + 
                                DirectoryPathSeperator + ModelObjectsDirectory + DirectoryPathSeperator + "code" + 
                                DirectoryPathSeperator + ModelObjectModule.Name;

                            //Method to record start time in a hashtable with filename,start time as key-value pairs
                            StartTimeCapture(xsdDirectory + DirectoryPathSeperator + ModelObject.EntityName + ".xsd");

                            CreateFile(xsdDirectory, ModelObject.EntityName + ".xsd", cpModelObjectDataEntity.GenerateContent());

                            //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                            RecordTime(xsdDirectory + DirectoryPathSeperator + ModelObject.EntityName + ".xsd");
                        }
                    }
                }
            }
            if (frm.PRTDataentityModuleObjects)
            {
                foreach (Entities.ModelObjectModule ModelObjectModule in projectToBeGeneratedFor.ModelObjectModules)
                {
                    foreach (Entities.Entity ModelObject in ModelObjectModule.ModelObjects)
                    {
                        if (ModelObject.IsToBeGenerated)
                        {
                            string xsdDirectory = outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory + 
                                DirectoryPathSeperator + ModelObjectsDirectory + DirectoryPathSeperator + "xsd" + 
                                DirectoryPathSeperator + ModelObjectModule.Name;
                            string codeDirectory = outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory + 
                                DirectoryPathSeperator + ModelObjectsDirectory + DirectoryPathSeperator + "code" + 
                                DirectoryPathSeperator + ModelObjectModule.Name;

                            XsdObjectGeneratorWrapper xsdObjGen = new XsdObjectGeneratorWrapper(_xsdObjGenPath);

                            //Method to record start time in a hashtable with filename,start time as key-value pairs
                            StartTimeCapture(codeDirectory + DirectoryPathSeperator + ModelObject.EntityName + ".cs");
                                                       
                            xsdObjGen.CallXsdObjGen(xsdDirectory, codeDirectory, ModelObject.EntityName, XsdObjectGeneratorWrapper.ModelObjectNamespaceMapping[ModelObject.EntityName].ToString());
                            //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                            RecordTime(codeDirectory + DirectoryPathSeperator + ModelObject.EntityName + ".cs");
                        }
                    }
                }
            }

            isErrorOccurred = false;
        }

        //cobol Changes 
        /// <summary>
        /// Generates Data entity classes for Cobol format.
        /// </summary>
        public void GenerateModelObjectDataEntitiesBase_cobol()
        {
            isErrorOccurred = true;
            if (frm.PRTDataEntitySchemaModelObjects)
            {
                Template templateProvided =
                    RetrieveTemplate("CobolModelObjectDataEntity");

                foreach (Entities.ModelObjectModule ModelObjectModule in projectToBeGeneratedFor.ModelObjectModules)
                {
                    foreach (Entities.Entity ModelObject in ModelObjectModule.ModelObjects)
                    {
                        if (ModelObject.IsToBeGenerated)
                        {
                            DataEntities.CobolModelObjectDataEntityCP cpModelObjectDataEntity
                                = new DataEntities.CobolModelObjectDataEntityCP(ModelObjectModule, ModelObject);
                            cpModelObjectDataEntity.ContentTemplate = templateProvided;

                            string xsdDirectory = outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory + 
                                DirectoryPathSeperator + ModelObjectsDirectory + DirectoryPathSeperator + "xsd" + 
                                DirectoryPathSeperator + ModelObjectModule.Name;
                            string codeDirectory = outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory + 
                                DirectoryPathSeperator + ModelObjectsDirectory + DirectoryPathSeperator + "code" + 
                                DirectoryPathSeperator + ModelObjectModule.Name;

                            //Method to record start time in a hashtable with filename,start time as key-value pairs
                            StartTimeCapture(xsdDirectory + DirectoryPathSeperator + ModelObject.EntityName + ".xsd");

                            //name of generated file : entity name instead of prog ID 
                            CreateFile(xsdDirectory, ModelObject.EntityName + ".xsd", cpModelObjectDataEntity.GenerateContent());

                            //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                            RecordTime(xsdDirectory + DirectoryPathSeperator + ModelObject.EntityName + ".xsd");                            
                        }
                    }
                }
            }

            if (frm.PRTDataentityModuleObjects)
            {
                foreach (Entities.ModelObjectModule ModelObjectModule in projectToBeGeneratedFor.ModelObjectModules)
                {
                    foreach (Entities.Entity ModelObject in ModelObjectModule.ModelObjects)
                    {
                        if (ModelObject.IsToBeGenerated)
                        {
                            string xsdDirectory = outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory + 
                                DirectoryPathSeperator + ModelObjectsDirectory + DirectoryPathSeperator + "xsd" + 
                                DirectoryPathSeperator + ModelObjectModule.Name;
                            string codeDirectory = outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory + 
                                DirectoryPathSeperator + ModelObjectsDirectory + DirectoryPathSeperator + "code" + 
                                DirectoryPathSeperator + ModelObjectModule.Name;

                            XsdObjectGeneratorWrapper xsdObjGen = new XsdObjectGeneratorWrapper(_xsdObjGenPath);

                            //Method to record start time in a hashtable with filename,start time as key-value pairs
                            StartTimeCapture(codeDirectory + DirectoryPathSeperator + ModelObject.EntityName + ".cs");
                            
                            xsdObjGen.CallXsdObjGen(xsdDirectory, codeDirectory, ModelObject.EntityName, XsdObjectGeneratorWrapper.ModelObjectNamespaceMapping[ModelObject.EntityName].ToString());
                            //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                            RecordTime(codeDirectory + DirectoryPathSeperator + ModelObject.EntityName + ".cs");
                        }
                    }
                }
            }            
            isErrorOccurred = false;
        }

        const string xsdObjectGeneratorArguments = "{0} /n:{1}";
        
        //constants for directories of generated solution
        const string ContractsDirectory = "Contracts";
        const string ModelObjectsDirectory = "ModelObjects";
        const string SerializersDirectory = "Serializers";
        const string DataEntitiesDirectory = "DataEntities";
        const string HostAccessDirectory = "HostAccess";

        //constants for Web service directories
        const string WebServiceDataContract = "LegacyDataContract";
        const string WebServiceContract = "ServiceContract";
        const string WebServiceImplementation = "ServiceImplementation";
        const string WebServiceHost = "WebService";
        const string WebServiceConfigFiles = @"WebService\Configurations";
        const string WebServiceInterfaceDirectory = @"HostAccess\WebService";

        //constants for WCF directories
        const string WCFDataContract = "LegacyDataContract";
        const string WCFServiceContract = "ServiceContract";
        const string WCFServiceImplementation = "ServiceImplementation";
        const string WCFHost = "WCFHost";
        const string WCFConfigFiles = @"WCFHost\Configurations";
        const string WCFAssemblyInfoDC = @"LegacyDataContract\Properties";
        const string WCFAssemblyInfoSC = @"ServiceContract\Properties";
        const string WCFAssemblyInfoSI = @"ServiceImplementation\Properties";              

        //constants for RSS type solution
        const string RSSContDE=@"DataEntities\Contracts\code\Properties\";
        const string RSSContSe = @"Serializers\Contracts\Properties\";
        const string RSSMODE = @"DataEntities\ModelObjects\code\Properties\";
        const string RSSMOSe = @"Serializers\ModelObjects\Properties\";        

        /// <summary>
        /// Generate .XSD and .cs files for contract data entities 
        /// </summary>
        public void GenerateContractDataEntitiesBase()
        {
            isErrorOccurred = true;
            if (frm.PRTDataEntitySchemaContracts)
            {
                foreach (Entities.ContractModule contractModule in projectToBeGeneratedFor.ContractModules)
                {
                    Template template = RetrieveTemplate("ContractDataEntity");

                    foreach (Entities.Contract contract in contractModule.Contracts)
                    {
                        if (contract.IsToBeGenerated)
                        {
                            DataEntities.ContractDataEntityCP.Initialize();                            
                            DataEntities.ContractDataEntityCP codeProvider = 
                                new DataEntities.ContractDataEntityCP(contract, ModelObjectDetailsMapping);
                            codeProvider.ProvideTemplate(template);

                            string xsdDirectory = outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory + 
                                DirectoryPathSeperator + ContractsDirectory + DirectoryPathSeperator + "xsd" + 
                                DirectoryPathSeperator + contractModule.Name;
                            string codeOutput = outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory + 
                                DirectoryPathSeperator + ContractsDirectory + DirectoryPathSeperator + "code" + 
                                DirectoryPathSeperator + contractModule.Name;

                            //Method to record start time in a hashtable with filename,start time as key-value pairs
                            StartTimeCapture(xsdDirectory + DirectoryPathSeperator + contract.ContractName + ".xsd");

                            CreateFile(xsdDirectory, contract.ContractName + ".xsd", codeProvider.GenerateContent());

                            //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                            RecordTime(xsdDirectory + DirectoryPathSeperator + contract.ContractName + ".xsd");
                        }
                    }
                }
            }

            if (frm.PRTDataEntityContracts)
            {
                foreach (Entities.ContractModule contractModule in projectToBeGeneratedFor.ContractModules)
                {
                    Template template = RetrieveTemplate("ContractDataEntity");

                    foreach (Entities.Contract contract in contractModule.Contracts)
                    {
                        if (contract.IsToBeGenerated)
                        {
                            DataEntities.ContractDataEntityCP.Initialize();
                            DataEntities.ContractDataEntityCP codeProvider = 
                                new DataEntities.ContractDataEntityCP(contract, ModelObjectDetailsMapping);
                            codeProvider.ProvideTemplate(template);

                            string xsdDirectory = outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory + 
                                DirectoryPathSeperator + ContractsDirectory + DirectoryPathSeperator + "xsd" + 
                                DirectoryPathSeperator + contractModule.Name;
                            string codeOutput = outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory + 
                                DirectoryPathSeperator + ContractsDirectory + DirectoryPathSeperator + "code" + 
                                DirectoryPathSeperator + contractModule.Name;

                            XsdObjectGeneratorWrapper xsdObjGen = new XsdObjectGeneratorWrapper(_xsdObjGenPath);

                            //Method to record start time in a hashtable with filename,start time as key-value pairs
                            StartTimeCapture(codeOutput + DirectoryPathSeperator + contract.ContractName + ".cs");

                            xsdObjGen.CallXsdObjGen(xsdDirectory, codeOutput, contract.ContractName, contractModule.DataEntityNamespace.Replace("#EntityName#", contract.ContractName));

                            //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                            RecordTime(codeOutput + DirectoryPathSeperator + contract.ContractName + ".cs");
                        }                        
                    }
                }
            }
            isErrorOccurred = false;
        }

        /// <summary>
        /// Generate model object serializer classes for TINS format.
        /// </summary>
        public void GenerateModelObjectSerializersBase_tins()
        {
            isErrorOccurred = true;
            if (frm.PRTSerializerModelObjects)
            {
                foreach (Entities.ModelObjectModule module in projectToBeGeneratedFor.ModelObjectModules)
                {
                    foreach (Entities.Entity ModelObject in module.ModelObjects)
                    {
                        if (ModelObject.IsToBeGenerated)
                        {
                            Template templateProvided =
                                RetrieveTemplate("ModelObjectSerializer");
                            Serializers.ModelObjectSerializerCP cpModelObjectSerializer
                                = new Serializers.ModelObjectSerializerCP(module, ModelObject, templateProvided);

                            //Method to record start time in a hashtable with filename,start time as key-value pairs
                            Infosys.Lif.LegacyWorkbench.ReportManager.ReportGenerator.StartTimeCapture(outputDirectory + 
                                DirectoryPathSeperator + SerializersDirectory + DirectoryPathSeperator + 
                                ModelObjectsDirectory + DirectoryPathSeperator + module.Name + DirectoryPathSeperator + 
                                ModelObject.EntityName + ".cs");

                            CreateFile(outputDirectory + DirectoryPathSeperator + SerializersDirectory + 
                                DirectoryPathSeperator + ModelObjectsDirectory + DirectoryPathSeperator + module.Name,
                                ModelObject.EntityName + ".cs", cpModelObjectSerializer.GenerateContent());

                            //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                            Infosys.Lif.LegacyWorkbench.ReportManager.ReportGenerator.RecordTime(outputDirectory + 
                                DirectoryPathSeperator + SerializersDirectory + DirectoryPathSeperator + 
                                ModelObjectsDirectory + DirectoryPathSeperator + module.Name + DirectoryPathSeperator + 
                                ModelObject.EntityName + ".cs");                            
                        }
                    }
                }
            }
            isErrorOccurred = false;
        }

        /// <summary>
        /// Generate contract serializer classes for TINS format.
        /// </summary>
        public void GenerateContractSerializersBase_tins()
        {
            isErrorOccurred = true;
            if (frm.PRTSerializerContract)
            {
                StringBuilder sbConfigFile = new StringBuilder();

                foreach (Entities.ContractModule module in projectToBeGeneratedFor.ContractModules)
                {
                    foreach (Entities.Contract contract in module.Contracts)
                    {
                        if (contract.IsToBeGenerated)
                        {
                            Template templateProvided
                                = RetrieveTemplate("ContractSerializer");
                            
                            Serializers.ContractSerializerCP cpSerializer
                                = new Serializers.ContractSerializerCP(contract, module, templateProvided);

                            //Method to record start time in a hashtable with filename,start time as key-value pairs
                            StartTimeCapture(outputDirectory + DirectoryPathSeperator + SerializersDirectory + 
                                DirectoryPathSeperator + ContractsDirectory + DirectoryPathSeperator + module.Name + 
                                DirectoryPathSeperator + contract.ContractName + ".cs");

                            CreateFile(outputDirectory + DirectoryPathSeperator + SerializersDirectory + 
                                DirectoryPathSeperator + ContractsDirectory + DirectoryPathSeperator + module.Name,
                                contract.ContractName + ".cs", cpSerializer.GenerateContent());

                            //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                            RecordTime(outputDirectory + DirectoryPathSeperator + SerializersDirectory + 
                                DirectoryPathSeperator + ContractsDirectory + DirectoryPathSeperator + module.Name + 
                                DirectoryPathSeperator + contract.ContractName + ".cs");

                            string contractType = contract.ContractName.Substring(0, 4);
                            Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping;

                            if (contractMergeHashTable.ContainsKey(contractType))
                            {
                                contractMapping = (Entities.GenericCollection<HostCallers.ContractModuleMapping>)
                                    contractMergeHashTable[contractType];
                            }
                            else
                            {
                                contractMapping = new Entities.GenericCollection<HostCallers.ContractModuleMapping>();
                                contractMergeHashTable.Add(contractType, contractMapping);
                            }

                            {
                                // Helps in building multiple dlls for Host Access
                                string keyNameSpace = Framework.Helper.BuildNamespace
                                    (projectToBeGeneratedFor.ContractNamespaces.HostAccessRootNamespace, module);
                                Entities.GenericCollection<string> servicesCollection;

                                if (Mappings.NameSpaceServiceMapping.ContainsKey(keyNameSpace))
                                {
                                    servicesCollection = (Entities.GenericCollection<string>)
                                        Mappings.NameSpaceServiceMapping[keyNameSpace];
                                    for (int counter = 0; counter < servicesCollection.Count; counter++)
                                    {
                                        if (servicesCollection[counter] == contractType)
                                        {
                                            servicesCollection = null;
                                            break;
                                        }
                                    }
                                }

                                else
                                {
                                    servicesCollection = new Infosys.Lif.LegacyWorkbench.Entities.GenericCollection<string>();
                                    Mappings.NameSpaceServiceMapping.Add(keyNameSpace, servicesCollection);
                                }

                                if (servicesCollection != null)
                                {
                                    servicesCollection.Add(contractType);
                                }
                            }

                            HostCallers.ContractModuleMapping contractModuleMapping
                                = new HostCallers.ContractModuleMapping();
                            contractModuleMapping.Contract = contract;
                            contractModuleMapping.Module = module;
                            contractMapping.Add(contractModuleMapping);
                        }
                    }
                }
            }
            isErrorOccurred = false;
        }

        /// <summary>
        /// Generate model object serializer classes for Cobol format.
        /// </summary>
        public void GenerateModelObjectSerializersBase_cobol()
        {
            isErrorOccurred = true;
            if (frm.PRTSerializerModelObjects)
            {

                foreach (Entities.ModelObjectModule module in projectToBeGeneratedFor.ModelObjectModules)
                {
                    foreach (Entities.Entity ModelObject in module.ModelObjects)
                    {
                        if (ModelObject.IsToBeGenerated)
                        {
                            Template templateProvided =
                                RetrieveTemplate("CobolSerializer");                 
                            Serializers.CobolClauseSerializerCP cpCobolSerializer
                                = new Serializers.CobolClauseSerializerCP(module, ModelObject, templateProvided);
                            cpCobolSerializer.DataEntityNamespace = module.DataEntityNamespace;
                            cpCobolSerializer.Namespace = module.SerializerNamespace;

                            //Method to record start time in a hashtable with filename,start time as key-value pairs
                            StartTimeCapture(outputDirectory + DirectoryPathSeperator + SerializersDirectory + 
                                DirectoryPathSeperator + ModelObjectsDirectory + DirectoryPathSeperator + module.Name + 
                                DirectoryPathSeperator + ModelObject.EntityName + ".cs");

                            CreateFile(outputDirectory + DirectoryPathSeperator + SerializersDirectory + 
                                DirectoryPathSeperator + ModelObjectsDirectory + DirectoryPathSeperator + module.Name,
                                ModelObject.EntityName + ".cs", cpCobolSerializer.GenerateContent());

                            //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                            RecordTime(outputDirectory + DirectoryPathSeperator + SerializersDirectory + 
                                DirectoryPathSeperator + ModelObjectsDirectory + DirectoryPathSeperator + module.Name + 
                                DirectoryPathSeperator + ModelObject.EntityName + ".cs");
                        }
                    }
                }
            }
            isErrorOccurred = false;
        }

        /// <summary>
        /// Generate Contract Serializer classes for Cobol format.
        /// </summary>
        public void GenerateContractSerializersBase_cobol()
        {
            isErrorOccurred = true;
            if (frm.PRTSerializerContract)
            {

                StringBuilder sbConfigFile = new StringBuilder();

                foreach (Entities.ContractModule module in projectToBeGeneratedFor.ContractModules)
                {
                    foreach (Entities.Contract contract in module.Contracts)
                    {
                        if (contract.IsToBeGenerated)
                        {
                            Template templateProvided
                                = RetrieveTemplate("CobolContractSerializer");
                            
                            Serializers.CobolContractSerializerCP cpCobolSerializer
                                = new Serializers.CobolContractSerializerCP(contract, module, templateProvided);

                            //Method to record start time in a hashtable with filename,start time as key-value pairs
                            StartTimeCapture(outputDirectory + DirectoryPathSeperator + SerializersDirectory + 
                                DirectoryPathSeperator + ContractsDirectory + DirectoryPathSeperator + module.Name + 
                                DirectoryPathSeperator + contract.ContractName + ".cs");

                            CreateFile(outputDirectory + DirectoryPathSeperator + SerializersDirectory + 
                                DirectoryPathSeperator + ContractsDirectory + DirectoryPathSeperator + module.Name,
                                contract.ContractName + ".cs", cpCobolSerializer.GenerateContent());

                            //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                            RecordTime(outputDirectory + DirectoryPathSeperator + SerializersDirectory + 
                                DirectoryPathSeperator + ContractsDirectory + DirectoryPathSeperator + module.Name + 
                                DirectoryPathSeperator + contract.ContractName + ".cs");
                            
                            string contractType = contract.ContractName;
                            Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping;

                            if (contractMergeHashTable.ContainsKey(contractType))
                            {
                                contractMapping = (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[contractType];
                            }
                            else
                            {
                                contractMapping = new Entities.GenericCollection<HostCallers.ContractModuleMapping>();
                                contractMergeHashTable.Add(contractType, contractMapping);
                            }

                            {
                                // Helps in building multiple dlls for Host Access
                                string keyNameSpace = Framework.Helper.BuildNamespace(projectToBeGeneratedFor.ContractNamespaces.HostAccessRootNamespace,
                                    module);
                                Entities.GenericCollection<string> servicesCollection;

                                if (Mappings.NameSpaceServiceMapping.ContainsKey(keyNameSpace))
                                {
                                    servicesCollection = (Entities.GenericCollection<string>)
                                        Mappings.NameSpaceServiceMapping[keyNameSpace];
                                    for (int counter = 0; counter < servicesCollection.Count; counter++)
                                    {
                                        if (servicesCollection[counter] == contractType)
                                        {
                                            servicesCollection = null;
                                            break;
                                        }
                                    }
                                }

                                else
                                {
                                    servicesCollection = new Infosys.Lif.LegacyWorkbench.Entities.GenericCollection<string>();
                                    Mappings.NameSpaceServiceMapping.Add(keyNameSpace, servicesCollection);
                                }

                                if (servicesCollection != null)
                                {
                                    servicesCollection.Add(contractType);
                                }
                            }

                            HostCallers.ContractModuleMapping contractModuleMapping
                                = new HostCallers.ContractModuleMapping();
                            contractModuleMapping.Contract = contract;
                            contractModuleMapping.Module = module;
                            contractMapping.Add(contractModuleMapping);
                        }
                    }
                }
            }
            isErrorOccurred = false;
        }

        /// <summary>
        /// Generates the files required for all types of service interface projects.
        /// </summary>
        public void CleanUpBase()
        {
            //string constants defined for type of service interfaces
            const string hostAccessType = "hostaccess";
            const string webSiteType = "webservice";
            
            {
                //Generate Host Access service interface files
                if (frm.PRTcheServiceInterface && frm.PRTRadioHostAcess)
                {
                    //generate App.config file
                    Template tempConfig = RetrieveTemplate("AppConfig");

                    foreach (string strKey in contractMergeHashTable.Keys)
                    {
                        Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                            (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                        string strHostNameSpace = Framework.Helper.BuildNamespace(
                            projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, contractMapping[0].Module);

                        HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(contractMapping, tempConfig, 
                            strHostNameSpace);

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + "App.config");/////////////chkkkkkkk****************

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory, 
                            "App.config", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + "App.config");/////////////chkkkkkkk****************
                    }

                    //generate host access file
                    Template template = RetrieveTemplate("HostCall");

                    foreach (string strKey in contractMergeHashTable.Keys)
                    {
                        Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                            (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                        string strHostNameSpace = Framework.Helper.BuildNamespace(
                            projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, contractMapping[0].Module);
                        string projPrefix = projectToBeGeneratedFor.ProjectPrefix;

                        HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(contractMapping, template, 
                            strHostNameSpace, projPrefix);

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + contractMapping[0].Module.Name + DirectoryPathSeperator + 
                            strKey + ".cs");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory
                            + DirectoryPathSeperator + contractMapping[0].Module.Name,
                            strKey + ".cs", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + contractMapping[0].Module.Name + DirectoryPathSeperator + 
                            strKey + ".cs");
                    }

                    //LegacySettings.config
                    {
                        Template templateProvided = RetrieveTemplate("ConfigFile");
                        LegacyFacadeConfigFileCP codeProvider = new LegacyFacadeConfigFileCP(
                            projectToBeGeneratedFor, hostAccessType);
                        codeProvider.ContentTemplate = templateProvided;

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + "Configurations" + 
                            DirectoryPathSeperator + "LegacySettings.config");

                        CreateFile(outputDirectory + DirectoryPathSeperator + "Configurations",
                            "LegacySettings.config", codeProvider.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + "Configurations" + 
                            DirectoryPathSeperator + "LegacySettings.config");
                    }

                    //ServiceTransaction.config
                    {
                        Template templateTransaction = RetrieveTemplate("TransactionId");
                        CodeProviders.ServiceConfigCP codeGenerator = new ServiceConfigCP(
                            projectToBeGeneratedFor.ContractModules, 
                            ProjectToBeGeneratedFor.ContractNamespaces.XmlSchemaNamespace);
                        codeGenerator.ContentTemplate = templateTransaction;

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + "Configurations" + 
                            DirectoryPathSeperator + "ServiceTransaction.config");

                        CreateFile(outputDirectory + DirectoryPathSeperator + "Configurations",
                            "ServiceTransaction.config", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + "Configurations" + 
                            DirectoryPathSeperator + "ServiceTransaction.config");
                    }

                    //LISettings.config
                    {
                        Template templateLISConfig = RetrieveTemplate("LISettingsConfig");
                        LegacyFacadeConfigFileCP codeProvider = new LegacyFacadeConfigFileCP(projectToBeGeneratedFor, 
                            hostAccessType);
                        codeProvider.ContentTemplate = templateLISConfig;

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + "Configurations" + 
                            DirectoryPathSeperator + "LISettings.config");

                        CreateFile(outputDirectory + DirectoryPathSeperator + "Configurations",
                            "LISettings.config", codeProvider.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + "Configurations" + 
                            DirectoryPathSeperator + "LISettings.config");
                    }
                }

                //Generate files for WF service interface
                if (frm.PRTcheServiceInterface && frm.PRTRadioWFActivity)
                {
                    //generate WF Activity code
                    Template template = RetrieveTemplate("WFActivityTemplate");

                    foreach (string strKey in contractMergeHashTable.Keys)
                    {
                        Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                            (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                        string strHostNameSpace = Framework.Helper.BuildNamespace(
                            projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, contractMapping[0].Module);

                        HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(contractMapping, 
                            template, strHostNameSpace);

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + contractMapping[0].Module.Name + DirectoryPathSeperator + 
                            strKey + ".cs");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory
                            + DirectoryPathSeperator + contractMapping[0].Module.Name,
                            strKey + ".cs", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + contractMapping[0].Module.Name + DirectoryPathSeperator + 
                            strKey + ".cs");

                    }

                    //generate WF Activity Designer code
                    Template tempDesigner = RetrieveTemplate("WFActivityDesigner");

                    foreach (string strKey in contractMergeHashTable.Keys)
                    {
                        Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                            (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                        string strHostNameSpace = Framework.Helper.BuildNamespace(
                            projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, contractMapping[0].Module);

                        HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(contractMapping, 
                            tempDesigner, strHostNameSpace);

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + contractMapping[0].Module.Name + DirectoryPathSeperator + 
                            strKey + ".Designer.cs");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory
                            + DirectoryPathSeperator + contractMapping[0].Module.Name,
                            strKey + ".Designer.cs", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + contractMapping[0].Module.Name + DirectoryPathSeperator + 
                            strKey + ".Designer.cs");
                    }

                    //LegacySettings.config
                    {
                        Template templateProvided = RetrieveTemplate("ConfigFile");
                        LegacyFacadeConfigFileCP codeProvider = new LegacyFacadeConfigFileCP(projectToBeGeneratedFor, hostAccessType);
                        codeProvider.ContentTemplate = templateProvided;

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + "Configurations" + 
                            DirectoryPathSeperator + "LegacySettings.config");

                        CreateFile(outputDirectory + DirectoryPathSeperator + "Configurations",
                            "LegacySettings.config", codeProvider.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + "Configurations" + 
                            DirectoryPathSeperator + "LegacySettings.config");
                    }

                    //ServiceTransaction.config
                    {
                        Template templateTransaction = RetrieveTemplate("TransactionId");
                        CodeProviders.ServiceConfigCP codeGenerator = new ServiceConfigCP(
                            projectToBeGeneratedFor.ContractModules, 
                            projectToBeGeneratedFor.ContractNamespaces.XmlSchemaNamespace);
                        codeGenerator.ContentTemplate = templateTransaction;

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + "Configurations" + 
                            DirectoryPathSeperator + "ServiceTransaction.config");

                        CreateFile(outputDirectory + DirectoryPathSeperator + "Configurations",
                            "ServiceTransaction.config", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + "Configurations" + 
                            DirectoryPathSeperator + "ServiceTransaction.config");
                    }

                    //LISettings.config
                    {
                        Template templateLISConfig = RetrieveTemplate("LISettingsConfig");
                        LegacyFacadeConfigFileCP codeProvider = new LegacyFacadeConfigFileCP(projectToBeGeneratedFor, 
                            hostAccessType);
                        codeProvider.ContentTemplate = templateLISConfig;

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + "Configurations" + 
                            DirectoryPathSeperator + "LISettings.config");

                        CreateFile(outputDirectory + DirectoryPathSeperator + "Configurations",
                            "LISettings.config", codeProvider.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + "Configurations" + 
                            DirectoryPathSeperator + "LISettings.config");
                    }
                }

                //Generate files for WCF service interface
                if (frm.PRTcheServiceInterface && frm.PRTRadioWCFService)
                {
                    //WCF DataContract
                    Template templateWCFLegacyContextData = RetrieveTemplate("WCFLegacyContextData");
                    foreach (string strkey in contractMergeHashTable.Keys)
                    {
                        Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                        (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strkey];

                        string strHostNameSpace = Framework.Helper.BuildNamespace(
                            projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, contractMapping[0].Module);

                        string projPrefix = projectToBeGeneratedFor.ProjectPrefix;

                        HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(
                            contractMapping, templateWCFLegacyContextData, strHostNameSpace, projPrefix);

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFDataContract + DirectoryPathSeperator + "LegacyContextData.cs");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator + WCFDataContract,
                            "LegacyContextData.cs", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFDataContract + DirectoryPathSeperator + "LegacyContextData.cs");
                    }

                    //WCF Message Request
                    Template tempRequestMessageType = RetrieveTemplate("WCFMessageRequest");

                    foreach (string strKey in contractMergeHashTable.Keys)
                    {
                        Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                            (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                        string strHostNameSpace = Framework.Helper.BuildNamespace(
                            projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, contractMapping[0].Module);

                        string projPrefix = projectToBeGeneratedFor.ProjectPrefix;

                        HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(
                            contractMapping, tempRequestMessageType, strHostNameSpace, projPrefix);

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFServiceContract + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name + DirectoryPathSeperator + "RequestMessageType.cs");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFServiceContract + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name, "RequestMessageType.cs", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFServiceContract + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name + DirectoryPathSeperator + "RequestMessageType.cs");

                    }

                    //WCF Message Response
                    Template tempResponseMessageType = RetrieveTemplate("WCFMessageResponse");

                    foreach (string strKey in contractMergeHashTable.Keys)
                    {
                        Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                            (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                        string strHostNameSpace = Framework.Helper.BuildNamespace(
                            projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, contractMapping[0].Module);

                        string projPrefix = projectToBeGeneratedFor.ProjectPrefix;

                        HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(
                            contractMapping, tempResponseMessageType, strHostNameSpace, projPrefix);

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFServiceContract + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name + DirectoryPathSeperator + "ResponseMessageType.cs");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFServiceContract + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name, "ResponseMessageType.cs", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator + WCFServiceContract + DirectoryPathSeperator + contractMapping[0].Module.Name + DirectoryPathSeperator + "ResponseMessageType.cs");
                    }

                    //WCF Secrvice Interface
                    Template tempServiceInterface = RetrieveTemplate("WCFServiceInterface");

                    foreach (string strKey in contractMergeHashTable.Keys)
                    {
                        Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                            (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                        string strHostNameSpace = Framework.Helper.BuildNamespace(
                            projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, contractMapping[0].Module);

                        string projPrefix = projectToBeGeneratedFor.ProjectPrefix;

                        HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(
                            contractMapping, tempServiceInterface, strHostNameSpace, projPrefix);

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFServiceContract + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name + DirectoryPathSeperator + "I" + strKey + ".cs");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFServiceContract + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name, "I" + strKey + ".cs", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFServiceContract + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name + DirectoryPathSeperator + "I" + strKey + ".cs");
                    }

                    //WCF Service Implementation
                    Template tempServiceImplementation = RetrieveTemplate("WCFSercviceImplementation");

                    foreach (string strKey in contractMergeHashTable.Keys)
                    {
                        Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                            (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                        string strHostNameSpace = Framework.Helper.BuildNamespace(
                            projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, contractMapping[0].Module);

                        string projPrefix = projectToBeGeneratedFor.ProjectPrefix;

                        HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(
                            contractMapping, tempServiceImplementation, strHostNameSpace, projPrefix);

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFServiceImplementation + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name + DirectoryPathSeperator + strKey + ".cs");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFServiceImplementation + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name, strKey + ".cs", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFServiceImplementation + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name + DirectoryPathSeperator + strKey + ".cs");
                    }

                    //WCF Service.svc Generation
                    Template tempServiceSvc = RetrieveTemplate("WCFService");

                    foreach (string strKey in contractMergeHashTable.Keys)
                    {
                        Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                            (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                        string strHostNameSpace = Framework.Helper.BuildNamespace(
                            projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, contractMapping[0].Module);

                        string projPrefix = projectToBeGeneratedFor.ProjectPrefix;

                        HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(
                            contractMapping, tempServiceSvc, strHostNameSpace, projPrefix);

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFHost + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name + DirectoryPathSeperator + strKey + ".svc");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFHost + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name, strKey + ".svc", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFHost + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name + DirectoryPathSeperator + strKey + ".svc");
                    }

                    //WCF Web.Config Generation
                    {
                        Template tempWCFConfig = RetrieveTemplate("WCFWebConfig");
                        System.Collections.ArrayList hostNamespaceList = new System.Collections.ArrayList();

                        foreach (string strKey in contractMergeHashTable.Keys)
                        {
                            Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                                (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                            string strHostNameSpace = Framework.Helper.BuildNamespace(
                                projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, 
                                contractMapping[0].Module);
                            hostNamespaceList.Add(strHostNameSpace);
                        }
                        string projPrefix = projectToBeGeneratedFor.ProjectPrefix;

                        HostCallers.WCFHostCP codeGenerator = new HostCallers.WCFHostCP(
                            contractMergeHashTable, tempWCFConfig, projPrefix, hostNamespaceList);
                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFHost + DirectoryPathSeperator + "Web" + ".config");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFHost, "Web" + ".config", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFHost + DirectoryPathSeperator + "Web" + ".config");
                    }

                    //Legacy Settings
                    {
                        Template tempWCFLegacy = RetrieveTemplate("ConfigFile");
                        LegacyFacadeConfigFileCP codeProvider = new LegacyFacadeConfigFileCP(
                            projectToBeGeneratedFor, webSiteType);
                        codeProvider.ContentTemplate = tempWCFLegacy;

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFConfigFiles + DirectoryPathSeperator + 
                            "LegacySettings" + ".config");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFConfigFiles, "LegacySettings" + ".config", 
                            codeProvider.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFConfigFiles + DirectoryPathSeperator + 
                            "LegacySettings" + ".config");
                    }

                    //LI Settings
                    {
                        Template tempWCFLI = RetrieveTemplate("LISettingsConfig");
                        LegacyFacadeConfigFileCP codeProvider = new LegacyFacadeConfigFileCP(
                            projectToBeGeneratedFor, webSiteType);
                        codeProvider.ContentTemplate = tempWCFLI;

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFConfigFiles + DirectoryPathSeperator + 
                            "LISettings" + ".config");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFConfigFiles,
                            "LISettings" + ".config", codeProvider.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFConfigFiles + DirectoryPathSeperator + 
                            "LISettings" + ".config");
                    }

                    //ServiceTransaction.config
                    {
                        Template tempTransacConfig = RetrieveTemplate("TransactionId");
                        CodeProviders.ServiceConfigCP codeGenerator = new ServiceConfigCP(
                            projectToBeGeneratedFor.ContractModules, 
                            projectToBeGeneratedFor.ContractNamespaces.XmlSchemaNamespace);
                        codeGenerator.ContentTemplate = tempTransacConfig;

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFConfigFiles + DirectoryPathSeperator + 
                            "ServiceTransaction.config");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFConfigFiles,
                            "ServiceTransaction.config", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFConfigFiles + DirectoryPathSeperator + 
                            "ServiceTransaction.config");
                    }


                    //AssemblyInfo.cs DataContract                    
                    Guid assemblyInfoDataContract = Guid.NewGuid();
                    string assemnlyInfoDataContractName = "LegacyDataContract";

                    HostCallers.WCFAssemblyInfo codeGeneratorAssembly = new HostCallers.WCFAssemblyInfo(
                        assemnlyInfoDataContractName, assemblyInfoDataContract);
                    codeGeneratorAssembly.ContentTemplate = RetrieveTemplate("WCFAssemblyInfo");

                    //Method to record start time in a hashtable with filename,start time as key-value pairs
                    StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                        DirectoryPathSeperator + WCFAssemblyInfoDC + DirectoryPathSeperator + "AssemblyInfo" + ".cs");

                    CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                        DirectoryPathSeperator + WCFAssemblyInfoDC, "AssemblyInfo" + ".cs", 
                        codeGeneratorAssembly.GenerateContent());

                    //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                    RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                        DirectoryPathSeperator + WCFAssemblyInfoDC + DirectoryPathSeperator + "AssemblyInfo" + ".cs");


                    //AssemblyInfo.cs ServiceContract                    
                    Guid assemblyInfoserviceContract = Guid.NewGuid();
                    string assemnlyInfoserviceContractName = "ServiceContract";

                    HostCallers.WCFAssemblyInfo codeGeneratorAssemblySC = new HostCallers.WCFAssemblyInfo(
                        assemnlyInfoserviceContractName, assemblyInfoserviceContract);
                    codeGeneratorAssemblySC.ContentTemplate = RetrieveTemplate("WCFAssemblyInfo");

                    //Method to record start time in a hashtable with filename,start time as key-value pairs
                    StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                        DirectoryPathSeperator + WCFAssemblyInfoSC + DirectoryPathSeperator + "AssemblyInfo" + ".cs");

                    CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                        DirectoryPathSeperator + WCFAssemblyInfoSC, "AssemblyInfo" + ".cs", 
                        codeGeneratorAssemblySC.GenerateContent());

                    //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                    RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                        DirectoryPathSeperator + WCFAssemblyInfoSC + DirectoryPathSeperator + "AssemblyInfo" + ".cs");


                    //AssemblyInfo.cs ServiceImplementation                    
                    Guid assemblyInfoserviceImplementation = Guid.NewGuid();
                    string assemnlyInfoserviceImplementationName = "ServiceImplementation";

                    HostCallers.WCFAssemblyInfo codeGeneratorAssemblySI = new HostCallers.WCFAssemblyInfo(
                        assemnlyInfoserviceImplementationName, assemblyInfoserviceImplementation);
                    codeGeneratorAssemblySI.ContentTemplate = RetrieveTemplate("WCFAssemblyInfo");

                    //Method to record start time in a hashtable with filename,start time as key-value pairs
                    StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                        DirectoryPathSeperator + WCFAssemblyInfoSI + DirectoryPathSeperator + "AssemblyInfo" + ".cs");

                    CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                        DirectoryPathSeperator + WCFAssemblyInfoSI, "AssemblyInfo" + ".cs", 
                        codeGeneratorAssemblySI.GenerateContent());

                    //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                    RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                        DirectoryPathSeperator + WCFAssemblyInfoSI + DirectoryPathSeperator + "AssemblyInfo" + ".cs");
                }

                //Generate files for RSS Feed service interface
                if (frm.PRTcheServiceInterface && (frm.PRTRadioRSSFeed||frm.PrtRadioAtom))
                {
                    //RSSFeed DataContract
                    Template templateRSSFeedLegacyContextData = RetrieveTemplate("RSSDataContractLegacyContextData");
                    foreach (string strkey in contractMergeHashTable.Keys)
                    {
                        Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                            (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strkey];

                        string strHostNameSpace = Framework.Helper.BuildNamespace(
                            projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, contractMapping[0].Module);

                        string projPrefix = projectToBeGeneratedFor.ProjectPrefix;

                        HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(
                            contractMapping, templateRSSFeedLegacyContextData, strHostNameSpace, projPrefix);

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFDataContract + DirectoryPathSeperator + "LegacyContextData.cs");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFDataContract, "LegacyContextData.cs", 
                            codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFDataContract + DirectoryPathSeperator + "LegacyContextData.cs");
                    }                    

                    //RSSFeed Service.svc Generation
                    Template tempServiceSvc = RetrieveTemplate("RSSService");
                    foreach (string strKey in contractMergeHashTable.Keys)
                    {
                        Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                            (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];
                        string strHostNameSpace = Framework.Helper.BuildNamespace(
                            projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, contractMapping[0].Module);
                        string projPrefix = projectToBeGeneratedFor.ProjectPrefix;
                        HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(
                            contractMapping, tempServiceSvc, strHostNameSpace, projPrefix);

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFHost + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name + DirectoryPathSeperator + strKey + ".svc");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFHost + DirectoryPathSeperator + contractMapping[0].Module.Name, 
                            strKey + ".svc", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFHost + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name + DirectoryPathSeperator + strKey + ".svc");
                    }

                    //RSSFeed Legacy Settings
                    {
                        Template tempRSSFeedLegacy = RetrieveTemplate("ConfigFile");
                        LegacyFacadeConfigFileCP codeProvider = new LegacyFacadeConfigFileCP(
                            projectToBeGeneratedFor, webSiteType);
                        codeProvider.ContentTemplate = tempRSSFeedLegacy;

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFConfigFiles + DirectoryPathSeperator + 
                            "LegacySettings" + ".config");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFConfigFiles, "LegacySettings" + ".config", 
                            codeProvider.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFConfigFiles + DirectoryPathSeperator + 
                            "LegacySettings" + ".config");
                    }

                    //RSSFeed LI Settings
                    {
                        Template tempRSSFeedLI = RetrieveTemplate("LISettingsConfig");
                        LegacyFacadeConfigFileCP codeProvider = new LegacyFacadeConfigFileCP(
                            projectToBeGeneratedFor, webSiteType);
                        codeProvider.ContentTemplate = tempRSSFeedLI;

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFConfigFiles + DirectoryPathSeperator + "LISettings" + ".config");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFConfigFiles, "LISettings" + ".config", 
                            codeProvider.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFConfigFiles + DirectoryPathSeperator + "LISettings" + ".config");
                    }

                    //RSSFeed Service Transaction
                    {
                        Template tempTransacConfig = RetrieveTemplate("TransactionId");
                        CodeProviders.ServiceConfigCP codeGenerator = new ServiceConfigCP(
                            projectToBeGeneratedFor.ContractModules, 
                            projectToBeGeneratedFor.ContractNamespaces.XmlSchemaNamespace);
                        codeGenerator.ContentTemplate = tempTransacConfig;

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFConfigFiles + DirectoryPathSeperator + 
                            "ServiceTransaction.config");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator + WCFConfigFiles,
                            "ServiceTransaction.config", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFConfigFiles + DirectoryPathSeperator + 
                            "ServiceTransaction.config");

                    }

                    //RSS Web.Config Generation
                    //only for RSS and not for Atom
                    if (frm.PRTcheServiceInterface && frm.PRTRadioRSSFeed)
                    {
                        Template tempRSSFeedConfig = RetrieveTemplate("RSSWebConfig");
                        System.Collections.ArrayList hostNamespaceList = new System.Collections.ArrayList();

                        foreach (string strKey in contractMergeHashTable.Keys)
                        {
                            Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                                (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                            string strHostNameSpace = Framework.Helper.BuildNamespace(
                                projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, 
                                contractMapping[0].Module);
                            hostNamespaceList.Add(strHostNameSpace);
                        }
                        string projPrefix = projectToBeGeneratedFor.ProjectPrefix;

                        HostCallers.WCFHostCP codeGenerator = new HostCallers.WCFHostCP(
                            contractMergeHashTable, tempRSSFeedConfig, projPrefix, hostNamespaceList);

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFHost + DirectoryPathSeperator + "Web" + ".config");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFHost, "Web" + ".config", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFHost + DirectoryPathSeperator + "Web" + ".config");
                    }

                    //Atom Web.config generation
                    if (frm.PRTcheServiceInterface && frm.PrtRadioAtom)
                    {
                        Template tempRSSFeedConfig = RetrieveTemplate("AtomWebConfigTemplate");
                        System.Collections.ArrayList hostNamespaceList = new System.Collections.ArrayList();

                        foreach (string strKey in contractMergeHashTable.Keys)
                        {
                            Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                                (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                            string strHostNameSpace = Framework.Helper.BuildNamespace(
                                projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, 
                                contractMapping[0].Module);
                            hostNamespaceList.Add(strHostNameSpace);
                        }
                        string projPrefix = projectToBeGeneratedFor.ProjectPrefix;

                        HostCallers.WCFHostCP codeGenerator = new HostCallers.WCFHostCP(
                            contractMergeHashTable, tempRSSFeedConfig, projPrefix, hostNamespaceList);

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFHost + DirectoryPathSeperator + "Web" + ".config");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFHost, "Web" + ".config", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFHost + DirectoryPathSeperator + "Web" + ".config");
                    }
                                        
                    //RSS style sheet
                    {
                        Template tempRSSFeedStyleSheet = RetrieveTemplate("RSSStyleSheet");
                        System.Collections.ArrayList hostNamespaceList = new System.Collections.ArrayList();
                        
                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFHost + DirectoryPathSeperator + "StyleSheet" + ".css");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFHost, "StyleSheet" + ".css", 
                            tempRSSFeedStyleSheet.Body.ToString());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WCFHost + DirectoryPathSeperator + "StyleSheet" + ".css");
                    }
                    
                    //RSSSecrviceInterface generation
                    //only for RSS and not for Atom
                    if (frm.PRTcheServiceInterface && frm.PRTRadioRSSFeed)
                    {
                        Template tempServiceInterface = RetrieveTemplate("RSSServiceContractInterface");
                        
                        foreach (string strKey in contractMergeHashTable.Keys)
                        {
                            Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                                (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                            string strHostNameSpace = Framework.Helper.BuildNamespace(
                                projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, 
                                contractMapping[0].Module);
                            string projPrefix = projectToBeGeneratedFor.ProjectPrefix;

                            HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(
                                contractMapping, tempServiceInterface, strHostNameSpace, projPrefix);

                            //Method to record start time in a hashtable with filename,start time as key-value pairs
                            StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                                DirectoryPathSeperator + WCFServiceContract + DirectoryPathSeperator + 
                                contractMapping[0].Module.Name + DirectoryPathSeperator + "I" + strKey + ".cs");

                            CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                                DirectoryPathSeperator + WCFServiceContract + DirectoryPathSeperator + 
                                contractMapping[0].Module.Name, "I" + strKey + ".cs", codeGenerator.GenerateContent());

                            //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                            RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                                DirectoryPathSeperator + WCFServiceContract + DirectoryPathSeperator + 
                                contractMapping[0].Module.Name + DirectoryPathSeperator + "I" + strKey + ".cs");
                        }
                    }
                                        
                    //Atom ServiceInterface generation
                    if (frm.PRTcheServiceInterface && frm.PrtRadioAtom)
                    {
                        Template tempServiceInterface = RetrieveTemplate("AtomServiceInterfaceTemplate");                        
                    
                        foreach (string strKey in contractMergeHashTable.Keys)
                        {
                            Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                                (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                            string strHostNameSpace = Framework.Helper.BuildNamespace(
                                projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, 
                                contractMapping[0].Module);

                            string projPrefix = projectToBeGeneratedFor.ProjectPrefix;

                            HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(
                                contractMapping, tempServiceInterface, strHostNameSpace, projPrefix);

                            //Method to record start time in a hashtable with filename,start time as key-value pairs
                            StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                                DirectoryPathSeperator + WCFServiceContract + DirectoryPathSeperator + 
                                contractMapping[0].Module.Name + DirectoryPathSeperator + "I" + strKey + ".cs");

                            CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                                DirectoryPathSeperator + WCFServiceContract + DirectoryPathSeperator + 
                                contractMapping[0].Module.Name, "I" + strKey + ".cs", codeGenerator.GenerateContent());

                            //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                            RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                                DirectoryPathSeperator + WCFServiceContract + DirectoryPathSeperator + 
                                contractMapping[0].Module.Name + DirectoryPathSeperator + "I" + strKey + ".cs");
                        }
                    }                    

                    //RSS Service Implementation
                    //only for RSS and not for Atom
                    if (frm.PRTcheServiceInterface && frm.PRTRadioRSSFeed)
                    {
                        Template tempServiceImplementation = RetrieveTemplate("RSSServiceImplementation");

                        foreach (string strKey in contractMergeHashTable.Keys)
                        {
                            Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                                (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                            string strHostNameSpace = Framework.Helper.BuildNamespace(
                                projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, 
                                contractMapping[0].Module);

                            string projPrefix = projectToBeGeneratedFor.ProjectPrefix;

                            HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(
                                contractMapping, tempServiceImplementation, strHostNameSpace, projPrefix);

                            //Method to record start time in a hashtable with filename,start time as key-value pairs
                            StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                                DirectoryPathSeperator + WCFServiceImplementation + DirectoryPathSeperator + 
                                contractMapping[0].Module.Name + DirectoryPathSeperator + strKey + ".cs");

                            CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                                DirectoryPathSeperator + WCFServiceImplementation + DirectoryPathSeperator + 
                                contractMapping[0].Module.Name, strKey + ".cs", codeGenerator.GenerateContent());

                            //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                            RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                                DirectoryPathSeperator + WCFServiceImplementation + DirectoryPathSeperator + 
                                contractMapping[0].Module.Name + DirectoryPathSeperator + strKey + ".cs");
                        }
                    }

                    //Atom Service Implemenation generation
                    if (frm.PRTcheServiceInterface && frm.PrtRadioAtom)
                    {
                        Template tempServiceImplementation = RetrieveTemplate("AtomServiceImplementationTemplate");

                        foreach (string strKey in contractMergeHashTable.Keys)
                        {
                            Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                                (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                            string strHostNameSpace = Framework.Helper.BuildNamespace(
                                projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, 
                                contractMapping[0].Module);

                            string projPrefix = projectToBeGeneratedFor.ProjectPrefix;

                            HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(
                                contractMapping, tempServiceImplementation, strHostNameSpace, projPrefix);

                            //Method to record start time in a hashtable with filename,start time as key-value pairs
                            StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                                DirectoryPathSeperator + WCFServiceImplementation + DirectoryPathSeperator + 
                                contractMapping[0].Module.Name + DirectoryPathSeperator + strKey + ".cs");

                            CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                                DirectoryPathSeperator + WCFServiceImplementation + DirectoryPathSeperator + 
                                contractMapping[0].Module.Name, strKey + ".cs", codeGenerator.GenerateContent());

                            //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                            RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                                DirectoryPathSeperator + WCFServiceImplementation + DirectoryPathSeperator + 
                                contractMapping[0].Module.Name + DirectoryPathSeperator + strKey + ".cs");
                        }
                    }                    

                    //RSSAssemblyInfo.cs generation
                    //AssemblyInfo.cs DataContract                    
                    Guid assemblyInfoDataContract = Guid.NewGuid();
                    string assemnlyInfoDataContractName = "LegacyDataContract";

                    HostCallers.WCFAssemblyInfo codeGeneratorAssembly = new HostCallers.WCFAssemblyInfo(
                        assemnlyInfoDataContractName, assemblyInfoDataContract);
                    codeGeneratorAssembly.ContentTemplate = RetrieveTemplate("RSSAssemblyInfo");

                    //Method to record start time in a hashtable with filename,start time as key-value pairs
                    StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                        DirectoryPathSeperator + WCFAssemblyInfoDC + DirectoryPathSeperator + "AssemblyInfo" + ".cs");

                    CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator + 
                        WCFAssemblyInfoDC, "AssemblyInfo" + ".cs", codeGeneratorAssembly.GenerateContent());

                    //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                    RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                        DirectoryPathSeperator + WCFAssemblyInfoDC + DirectoryPathSeperator + "AssemblyInfo" + ".cs");


                    //AssemblyInfo.cs ServiceContract                    
                    Guid assemblyInfoserviceContract = Guid.NewGuid();
                    string assemnlyInfoserviceContractName = "ServiceContract";

                    HostCallers.WCFAssemblyInfo codeGeneratorAssemblySC = new HostCallers.WCFAssemblyInfo(
                        assemnlyInfoserviceContractName, assemblyInfoserviceContract);
                    codeGeneratorAssemblySC.ContentTemplate = RetrieveTemplate("RSSAssemblyInfo");

                    //Method to record start time in a hashtable with filename,start time as key-value pairs
                    StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                        DirectoryPathSeperator + WCFAssemblyInfoSC + DirectoryPathSeperator + "AssemblyInfo" + ".cs");

                    CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator + 
                        WCFAssemblyInfoSC, "AssemblyInfo" + ".cs", codeGeneratorAssemblySC.GenerateContent());

                    //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                    RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                        DirectoryPathSeperator + WCFAssemblyInfoSC + DirectoryPathSeperator + "AssemblyInfo" + ".cs");


                    //AssemblyInfo.cs ServiceImplementation                    
                    Guid assemblyInfoserviceImplementation = Guid.NewGuid();
                    string assemnlyInfoserviceImplementationName = "ServiceImplementation";

                    HostCallers.WCFAssemblyInfo codeGeneratorAssemblySI = new HostCallers.WCFAssemblyInfo(
                        assemnlyInfoserviceImplementationName, assemblyInfoserviceImplementation);
                    codeGeneratorAssemblySI.ContentTemplate = RetrieveTemplate("RSSAssemblyInfo");

                    //Method to record start time in a hashtable with filename,start time as key-value pairs
                    StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                        DirectoryPathSeperator + WCFAssemblyInfoSI + DirectoryPathSeperator + "AssemblyInfo" + ".cs");

                    CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator + 
                        WCFAssemblyInfoSI, "AssemblyInfo" + ".cs", codeGeneratorAssemblySI.GenerateContent());

                    //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                    RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                        DirectoryPathSeperator + WCFAssemblyInfoSI + DirectoryPathSeperator + "AssemblyInfo" + ".cs");

                    //AssemblyInfo.cs ContractDataEntity                    
                    Guid assemblyInfoContractDataEntity = Guid.NewGuid();
                    string assemnlyInfoContractDataEntityName = "ServiceImplementation";

                    HostCallers.WCFAssemblyInfo RSScodeGeneratorAssemblyCDE = new HostCallers.WCFAssemblyInfo(
                        assemnlyInfoContractDataEntityName, assemblyInfoContractDataEntity);
                    RSScodeGeneratorAssemblyCDE.ContentTemplate = RetrieveTemplate("RSSAssemblyInfo");

                    //Method to record start time in a hashtable with filename,start time as key-value pairs
                    StartTimeCapture(outputDirectory + DirectoryPathSeperator + RSSContDE + DirectoryPathSeperator + 
                        "AssemblyInfo" + ".cs");

                    CreateFile(outputDirectory + DirectoryPathSeperator + RSSContDE,
                        "AssemblyInfo" + ".cs", RSScodeGeneratorAssemblyCDE.GenerateContent());

                    //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                    RecordTime(outputDirectory + DirectoryPathSeperator + RSSContDE + DirectoryPathSeperator + 
                        "AssemblyInfo" + ".cs");


                    //AssemblyInfo.cs ContractSerializer                    
                    Guid assemblyInfoContractSerializer = Guid.NewGuid();
                    string assemnlyInfoContractSerializerName = "ServiceImplementation";

                    HostCallers.WCFAssemblyInfo RSScodeGeneratorAssemblyCS = new HostCallers.WCFAssemblyInfo(
                        assemnlyInfoContractSerializerName, assemblyInfoContractSerializer);
                    RSScodeGeneratorAssemblyCS.ContentTemplate = RetrieveTemplate("RSSAssemblyInfo");

                    //Method to record start time in a hashtable with filename,start time as key-value pairs
                    StartTimeCapture(outputDirectory + DirectoryPathSeperator + RSSContSe + DirectoryPathSeperator + 
                        "AssemblyInfo" + ".cs");

                    CreateFile(outputDirectory + DirectoryPathSeperator + RSSContSe, 
                        "AssemblyInfo" + ".cs", RSScodeGeneratorAssemblyCS.GenerateContent());

                    //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                    RecordTime(outputDirectory + DirectoryPathSeperator + RSSContSe + DirectoryPathSeperator + 
                        "AssemblyInfo" + ".cs");

                    //AssemblyInfo.cs ModelObjectDataEntity                    
                    Guid assemblyInfoModelObjectDataEntity = Guid.NewGuid();
                    string assemnlyInfoModelObjectDataEntitynName = "ServiceImplementation";

                    HostCallers.WCFAssemblyInfo RSScodeGeneratorAssemblyMDE = new HostCallers.WCFAssemblyInfo(
                        assemnlyInfoModelObjectDataEntitynName, assemblyInfoModelObjectDataEntity);
                    RSScodeGeneratorAssemblyMDE.ContentTemplate = RetrieveTemplate("RSSAssemblyInfo");

                    //Method to record start time in a hashtable with filename,start time as key-value pairs
                    StartTimeCapture(outputDirectory + DirectoryPathSeperator + RSSMODE + DirectoryPathSeperator + 
                        "AssemblyInfo" + ".cs");

                    CreateFile(outputDirectory + DirectoryPathSeperator + RSSMODE, 
                        "AssemblyInfo" + ".cs", RSScodeGeneratorAssemblyMDE.GenerateContent());

                    //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                    RecordTime(outputDirectory + DirectoryPathSeperator + RSSMODE + DirectoryPathSeperator + 
                        "AssemblyInfo" + ".cs");


                    //AssemblyInfo.cs ModelObjectSerializer                    
                    Guid assemblyInfoModelObjectSerializer = Guid.NewGuid();
                    string assemnlyInfoModelObjectSerializerName = "ServiceImplementation";

                    HostCallers.WCFAssemblyInfo RSScodeGeneratorAssemblyMS = new HostCallers.WCFAssemblyInfo(
                        assemnlyInfoModelObjectSerializerName, assemblyInfoModelObjectSerializer);
                    RSScodeGeneratorAssemblyMS.ContentTemplate = RetrieveTemplate("RSSAssemblyInfo");

                    //Method to record start time in a hashtable with filename,start time as key-value pairs
                    StartTimeCapture(outputDirectory + DirectoryPathSeperator + RSSMOSe + DirectoryPathSeperator + 
                        "AssemblyInfo" + ".cs");

                    CreateFile(outputDirectory + DirectoryPathSeperator + RSSMOSe,
                        "AssemblyInfo" + ".cs", RSScodeGeneratorAssemblyMS.GenerateContent());

                    //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                    RecordTime(outputDirectory + DirectoryPathSeperator + RSSMOSe + DirectoryPathSeperator + 
                        "AssemblyInfo" + ".cs");
                }                

                //Generate files for Web Service interface
                if (frm.PRTcheServiceInterface && frm.PRTRadioWebService)
                {
                    //LegacyContextData
                    Template tempLegacyContextData = RetrieveTemplate("LegacyContextData");

                    foreach (string strKey in contractMergeHashTable.Keys)
                    {
                        Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                            (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                        string strHostNameSpace = Framework.Helper.BuildNamespace(
                            projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, contractMapping[0].Module);

                        string projPrefix = projectToBeGeneratedFor.ProjectPrefix;

                        HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(
                            contractMapping, tempLegacyContextData, strHostNameSpace, projPrefix);
                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceDataContract + DirectoryPathSeperator + 
                            "LegacyContextData.cs");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceDataContract, "LegacyContextData.cs", 
                            codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceDataContract + DirectoryPathSeperator + 
                            "LegacyContextData.cs");
                    }

                    //RequestMessageType
                    Template tempRequestMessageType = RetrieveTemplate("RequestMessageType");

                    foreach (string strKey in contractMergeHashTable.Keys)
                    {
                        Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                            (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                        string strHostNameSpace = Framework.Helper.BuildNamespace(
                            projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, contractMapping[0].Module);

                        string projPrefix = projectToBeGeneratedFor.ProjectPrefix;

                        HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(
                            contractMapping, tempRequestMessageType, strHostNameSpace, projPrefix);
                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceContract + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name + DirectoryPathSeperator + "RequestMessageType.cs");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceContract + DirectoryPathSeperator +
                            contractMapping[0].Module.Name, "RequestMessageType.cs", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceContract + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name + DirectoryPathSeperator + "RequestMessageType.cs");
                    }

                    //ResponseMessageType
                    Template tempResponseMessageType = RetrieveTemplate("ResponseMessageType");

                    foreach (string strKey in contractMergeHashTable.Keys)
                    {
                        Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                            (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                        string strHostNameSpace = Framework.Helper.BuildNamespace(
                            projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, contractMapping[0].Module);

                        string projPrefix = projectToBeGeneratedFor.ProjectPrefix;

                        HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(contractMapping, 
                            tempResponseMessageType, strHostNameSpace, projPrefix);

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceContract + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name + DirectoryPathSeperator + "ResponseMessageType.cs");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator + WebServiceContract + DirectoryPathSeperator +
                            contractMapping[0].Module.Name, "ResponseMessageType.cs", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceContract + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name + DirectoryPathSeperator + "ResponseMessageType.cs");
                    }

                    //ServiceInterface
                    Template tempServiceInterface = RetrieveTemplate("ServiceInterface");

                    foreach (string strKey in contractMergeHashTable.Keys)
                    {
                        Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                            (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                        string strHostNameSpace = Framework.Helper.BuildNamespace(projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, contractMapping[0].Module);

                        string projPrefix = projectToBeGeneratedFor.ProjectPrefix;

                        HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(
                            contractMapping, tempServiceInterface, strHostNameSpace, projPrefix);

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceContract + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name + DirectoryPathSeperator + "I" + strKey + ".cs");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceContract + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name, "I" + strKey + ".cs", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceContract + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name + DirectoryPathSeperator + "I" + strKey + ".cs");
                    }

                    //ServiceImplementation
                    Template tempServiceImplementation = RetrieveTemplate("ServiceImplementation");

                    foreach (string strKey in contractMergeHashTable.Keys)
                    {
                        Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                            (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                        string strHostNameSpace = Framework.Helper.BuildNamespace(
                            projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, contractMapping[0].Module);

                        string projPrefix = projectToBeGeneratedFor.ProjectPrefix;

                        HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(
                            contractMapping, tempServiceImplementation, strHostNameSpace, projPrefix);

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceImplementation + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name + DirectoryPathSeperator + strKey + ".cs");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceImplementation + DirectoryPathSeperator +
                            contractMapping[0].Module.Name, strKey + ".cs", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceImplementation + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name + DirectoryPathSeperator + strKey + ".cs");
                    }

                    //asmx file
                    Template tempAsmx = RetrieveTemplate("WebServiceAsmx");

                    foreach (string strKey in contractMergeHashTable.Keys)
                    {
                        Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                            (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                        string strHostNameSpace = Framework.Helper.BuildNamespace(
                            projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, contractMapping[0].Module);
                        string projPrefix = projectToBeGeneratedFor.ProjectPrefix;
                        HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(
                            contractMapping, tempAsmx, strHostNameSpace, projPrefix);

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceHost + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name + DirectoryPathSeperator + strKey + ".asmx");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceHost + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name, strKey + ".asmx", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceHost + DirectoryPathSeperator + 
                            contractMapping[0].Module.Name + DirectoryPathSeperator + strKey + ".asmx");
                    }

                    //web.config file
                    Template tempWebConfig = RetrieveTemplate("WebConfig");

                    foreach (string strKey in contractMergeHashTable.Keys)
                    {
                        Entities.GenericCollection<HostCallers.ContractModuleMapping> contractMapping =
                            (Entities.GenericCollection<HostCallers.ContractModuleMapping>)contractMergeHashTable[strKey];

                        string strHostNameSpace = Framework.Helper.BuildNamespace(
                            projectToBeGeneratedFor.ContractNamespaces.HostAccessNamespace, contractMapping[0].Module);

                        HostCallers.HostCallCp codeGenerator = new HostCallers.HostCallCp(
                            contractMapping, tempWebConfig, strHostNameSpace);

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceHost + DirectoryPathSeperator + "Web.config");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceHost, "Web.config", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceHost + DirectoryPathSeperator + "Web.config");
                    }

                    //LegacySettings.config
                    {
                        Template templateProvided = RetrieveTemplate("ConfigFile");
                        LegacyFacadeConfigFileCP codeProvider = new LegacyFacadeConfigFileCP(
                            projectToBeGeneratedFor, webSiteType);
                        codeProvider.ContentTemplate = templateProvided;

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceConfigFiles + DirectoryPathSeperator + 
                            "LegacySettings.config");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceConfigFiles, "LegacySettings.config", 
                            codeProvider.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceConfigFiles + DirectoryPathSeperator + 
                            "LegacySettings.config");
                    }

                    //ServiceTransaction.config
                    {
                        Template template = RetrieveTemplate("TransactionId");
                        CodeProviders.ServiceConfigCP codeGenerator = new ServiceConfigCP(
                            projectToBeGeneratedFor.ContractModules, 
                            projectToBeGeneratedFor.ContractNamespaces.XmlSchemaNamespace);
                        codeGenerator.ContentTemplate = template;

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceConfigFiles + DirectoryPathSeperator + 
                            "ServiceTransaction.config");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceConfigFiles, "ServiceTransaction.config", 
                            codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceConfigFiles + DirectoryPathSeperator + 
                            "ServiceTransaction.config");
                    }

                    //LISettings.config
                    {
                        Template templateLISConfig = RetrieveTemplate("LISettingsConfig");
                        LegacyFacadeConfigFileCP codeProvider = new LegacyFacadeConfigFileCP(
                            projectToBeGeneratedFor, webSiteType);
                        codeProvider.ContentTemplate = templateLISConfig;

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceConfigFiles + DirectoryPathSeperator + 
                            "LISettings.config");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceConfigFiles, "LISettings.config", 
                            codeProvider.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + WebServiceConfigFiles + DirectoryPathSeperator + 
                            "LISettings.config");
                    }
                }
            }

            isErrorOccurred = true; 

            //call method to generate .csproj files
            GenerateProjectFiles();

            isErrorOccurred = false;
        }

        /// <summary>
        /// copies files from source path to destination path.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        private void CopyFile(string source, string destination)
        {
            source = templatesDirectory + DirectoryPathSeperator + source;
            destination = outputDirectory + DirectoryPathSeperator + destination;
            System.IO.File.Copy(source, destination, true);
        }
        
        /// <summary>
        /// Generate .csproj files for all projects in the generated solution.
        /// </summary>
        private void GenerateProjectFiles()
        {            
            Guid ModelObjectDataEntityProjectGuid = Guid.NewGuid();
            Guid contractDataEntityProjectGuid = Guid.NewGuid();
            Guid ModelObjectSerializerProjectGuid = Guid.NewGuid();
            Guid contractSerializerProjectGuid = Guid.NewGuid();
            Guid solutionGuid = Guid.NewGuid();

            string contractsEntityRootNameSpace, ModelObjectsEntityRootNameSpace, contractsSerializerRootNameSpace, 
                ModelObjectsSerializerRootNameSpace;

            isErrorOccurred = true;

            {
                // ModelObject Data Entity Project file
                ModelObjectsEntityRootNameSpace = projectToBeGeneratedFor.ModelObjectNamespaces.DataEntityRootNamespace;
                //to disallow the generation of dataentity and serializer csproj for RSS and Atom
                //as they are generated below for .net vs2008 type
                if (!(frm.PRTRadioRSSFeed || frm.PrtRadioAtom))
                {
                    if (frm.PRTDataentityModuleObjects)
                    {
                        ProjectFiles.ModelObjectDataEntityProjectCP codeGenerator =
                            new ProjectFiles.ModelObjectDataEntityProjectCP(ModelObjectDataEntityProjectGuid,
                            projectToBeGeneratedFor.ModelObjectModules, ModelObjectsEntityRootNameSpace);
                        codeGenerator.ContentTemplate = RetrieveTemplate("EntityProjectTemplate");

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory +
                            DirectoryPathSeperator + ModelObjectsDirectory + DirectoryPathSeperator + "Code" +
                            DirectoryPathSeperator + ModelObjectsEntityRootNameSpace + ".csproj");

                        CreateFile(outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory +
                            DirectoryPathSeperator + ModelObjectsDirectory + DirectoryPathSeperator + "Code",
                          ModelObjectsEntityRootNameSpace + ".csproj", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory +
                            DirectoryPathSeperator + ModelObjectsDirectory + DirectoryPathSeperator + "Code" +
                            DirectoryPathSeperator + ModelObjectsEntityRootNameSpace + ".csproj");
                    }
                }
            }

            {
                // Contract Data Entity Project file
                contractsEntityRootNameSpace = projectToBeGeneratedFor.ContractNamespaces.DataEntityRootNamespace;
                //to disallow the generation of dataentity and serializer csproj for RSS and Atom
                //as they are generated below for .net vs2008 type
                if (!(frm.PRTRadioRSSFeed || frm.PrtRadioAtom))
                {
                    if (frm.PRTDataEntityContracts)
                    {
                        ProjectFiles.ContractDataEntityProjectCP codeGenerator
                            = new ProjectFiles.ContractDataEntityProjectCP(contractDataEntityProjectGuid,
                            projectToBeGeneratedFor.ContractModules, contractsEntityRootNameSpace,
                            ModelObjectsEntityRootNameSpace);
                        codeGenerator.ContentTemplate = RetrieveTemplate("ContractEntityProjectTemplate");

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory +
                            DirectoryPathSeperator + ContractsDirectory + DirectoryPathSeperator + "Code" +
                            DirectoryPathSeperator + contractsEntityRootNameSpace + ".csproj");

                        CreateFile(outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory +
                            DirectoryPathSeperator + ContractsDirectory + DirectoryPathSeperator + "Code",
                            contractsEntityRootNameSpace + ".csproj", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory +
                            DirectoryPathSeperator + ContractsDirectory + DirectoryPathSeperator + "Code" +
                            DirectoryPathSeperator + contractsEntityRootNameSpace + ".csproj");
                    }
                }
            }

            {
                // ModelObject Serializer Project file
                ModelObjectsSerializerRootNameSpace = projectToBeGeneratedFor.ModelObjectNamespaces.SerializerRootNamespace;
                //to disallow the generation of dataentity and serializer csproj for RSS and Atom
                //as they are generated below for .net vs2008 type
                if (!(frm.PRTRadioRSSFeed || frm.PrtRadioAtom))
                {
                    if (frm.PRTSerializerModelObjects)
                    {
                        ProjectFiles.ModelObjectSerializerProjectCP codeGenerator =
                            new ProjectFiles.ModelObjectSerializerProjectCP(contractSerializerProjectGuid,
                            projectToBeGeneratedFor.ModelObjectModules, ModelObjectsSerializerRootNameSpace,
                            ModelObjectsEntityRootNameSpace);
                        codeGenerator.ContentTemplate = RetrieveTemplate("SerializerProjectTemplate");

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + SerializersDirectory +
                            DirectoryPathSeperator + ModelObjectsDirectory + DirectoryPathSeperator +
                            ModelObjectsSerializerRootNameSpace + ".csproj");

                        CreateFile(outputDirectory + DirectoryPathSeperator + SerializersDirectory +
                            DirectoryPathSeperator + ModelObjectsDirectory, ModelObjectsSerializerRootNameSpace +
                            ".csproj", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + SerializersDirectory +
                            DirectoryPathSeperator + ModelObjectsDirectory + DirectoryPathSeperator +
                            ModelObjectsSerializerRootNameSpace + ".csproj");
                    }
                }
            }

            {
                // Contract Serializer Project file
                contractsSerializerRootNameSpace = projectToBeGeneratedFor.ContractNamespaces.SerializerRootNamespace;
                //to disallow the generation of dataentity and serializer csproj for RSS and Atom
                //as they are generated below for .net vs2008 type
                if (!(frm.PRTRadioRSSFeed || frm.PrtRadioAtom))
                {
                    if (frm.PRTSerializerContract)
                    {
                        ProjectFiles.ContractSerializerProjectCP codeGenerator
                            = new ProjectFiles.ContractSerializerProjectCP(contractSerializerProjectGuid,
                            projectToBeGeneratedFor.ContractModules, ModelObjectsEntityRootNameSpace,
                            ModelObjectsSerializerRootNameSpace, contractsEntityRootNameSpace,
                            contractsSerializerRootNameSpace);
                        codeGenerator.ContentTemplate = RetrieveTemplate("ContractSerializerProjectTemplate");

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + SerializersDirectory +
                            DirectoryPathSeperator + ContractsDirectory + DirectoryPathSeperator +
                            contractsSerializerRootNameSpace + ".csproj");

                        CreateFile(outputDirectory + DirectoryPathSeperator + SerializersDirectory +
                            DirectoryPathSeperator + ContractsDirectory, contractsSerializerRootNameSpace + ".csproj",
                            codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + SerializersDirectory +
                            DirectoryPathSeperator + ContractsDirectory + DirectoryPathSeperator +
                            contractsSerializerRootNameSpace + ".csproj");
                    }
                }
            }

            //Host Access interface
            if (frm.PRTcheServiceInterface && frm.PRTRadioHostAcess)
            {
                string[] HostAccessRootNameSpaces;
                Guid[] hostAccessProjectGuids;

                {
                    string HostAccessRootNameSpace = projectToBeGeneratedFor.ContractNamespaces.HostAccessRootNamespace;

                    int numberOfHostAccessDlls = 1;

                    if (HostAccessRootNameSpace != Framework.Helper.BuildNamespace(HostAccessRootNameSpace, 
                        projectToBeGeneratedFor.ContractModules[0]))
                    {
                        // The host access root namespace does has #ModuleName# 
                        numberOfHostAccessDlls = projectToBeGeneratedFor.ContractModules.Count;
                    }
                    HostAccessRootNameSpaces = new string[numberOfHostAccessDlls];
                    hostAccessProjectGuids = new Guid[numberOfHostAccessDlls];
                    for (int counter = 0; counter < numberOfHostAccessDlls; counter++)
                    {
                        hostAccessProjectGuids[counter] = Guid.NewGuid();
                        HostAccessRootNameSpaces[counter] = Framework.Helper.BuildNamespace(
                            projectToBeGeneratedFor.ContractNamespaces.HostAccessRootNamespace,
                            projectToBeGeneratedFor.ContractModules[counter]);

                        Entities.GenericCollection<string> serviceList
                            = (Entities.GenericCollection<string>)Mappings.NameSpaceServiceMapping[
                            HostAccessRootNameSpaces[counter]];


                        // Host Access Project File
                        ProjectFiles.HostAccessProjectCP codeGenerator
                            = new ProjectFiles.HostAccessProjectCP(hostAccessProjectGuids[counter],
                            serviceList, contractMergeHashTable, contractsEntityRootNameSpace,
                            HostAccessRootNameSpaces[counter]);
                        codeGenerator.ContentTemplate = RetrieveTemplate("HostAccessProjectTemplate");

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + HostAccessRootNameSpaces[counter] + ".csproj");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory,
                            HostAccessRootNameSpaces[counter] + ".csproj", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + HostAccessRootNameSpaces[counter] + ".csproj");
                    }
                }

                {
                    //Host Access Solution Generation
                    ProjectFiles.SolutionCP codeGenerator = new ProjectFiles.SolutionCP(solutionGuid,
                        ModelObjectDataEntityProjectGuid, ModelObjectsEntityRootNameSpace,
                        ModelObjectSerializerProjectGuid, ModelObjectsSerializerRootNameSpace,
                        contractDataEntityProjectGuid, contractsEntityRootNameSpace,
                        contractSerializerProjectGuid, contractsSerializerRootNameSpace,
                        hostAccessProjectGuids, HostAccessRootNameSpaces);

                    codeGenerator.ContentTemplate = RetrieveTemplate("Solution");

                    //Method to record start time in a hashtable with filename,start time as key-value pairs
                    StartTimeCapture(outputDirectory + DirectoryPathSeperator + SolutionFileName);

                    CreateFile(outputDirectory, SolutionFileName, codeGenerator.GenerateContent());

                    //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                    RecordTime(outputDirectory + DirectoryPathSeperator + SolutionFileName);
                }
                
            }

            //Generate WF Project files
            if (frm.PRTcheServiceInterface && frm.PRTRadioWFActivity)
            {
                string[] HostAccessRootNameSpaces;
                Guid[] hostAccessProjectGuids;

                {
                    string HostAccessRootNameSpace = projectToBeGeneratedFor.ContractNamespaces.HostAccessRootNamespace;

                    int numberOfHostAccessDlls = 1;

                    if (HostAccessRootNameSpace != Framework.Helper.BuildNamespace(
                        HostAccessRootNameSpace, projectToBeGeneratedFor.ContractModules[0]))
                    {
                        // The host access root namespace does has #ModuleName# 
                        numberOfHostAccessDlls = projectToBeGeneratedFor.ContractModules.Count;
                    }
                    HostAccessRootNameSpaces = new string[numberOfHostAccessDlls];
                    hostAccessProjectGuids = new Guid[numberOfHostAccessDlls];
                    for (int counter = 0; counter < numberOfHostAccessDlls; counter++)
                    {
                        hostAccessProjectGuids[counter] = Guid.NewGuid();
                        HostAccessRootNameSpaces[counter] = Framework.Helper.BuildNamespace(
                            projectToBeGeneratedFor.ContractNamespaces.HostAccessRootNamespace,
                            projectToBeGeneratedFor.ContractModules[counter]);

                        Entities.GenericCollection<string> serviceList
                            = (Entities.GenericCollection<string>)Mappings.NameSpaceServiceMapping[
                            HostAccessRootNameSpaces[counter]];

                        // WF Activity Template + HostAccess CP
                        ProjectFiles.HostAccessProjectCP codeGenerator
                            = new ProjectFiles.HostAccessProjectCP(hostAccessProjectGuids[counter],
                            serviceList, contractMergeHashTable, contractsEntityRootNameSpace,
                            HostAccessRootNameSpaces[counter]);
                        codeGenerator.ContentTemplate = RetrieveTemplate("WFActivityProjectTemplate");

                        //Method to record start time in a hashtable with filename,start time as key-value pairs
                        StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + HostAccessRootNameSpaces[counter] + ".csproj");

                        CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory,
                            HostAccessRootNameSpaces[counter] + ".csproj", codeGenerator.GenerateContent());

                        //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                        RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                            DirectoryPathSeperator + HostAccessRootNameSpaces[counter] + ".csproj");
                    }
                }
                
                {
                    //Generate WF Solution
                    ProjectFiles.SolutionCP codeGenerator = new ProjectFiles.SolutionCP(
                        solutionGuid,
                        ModelObjectDataEntityProjectGuid, ModelObjectsEntityRootNameSpace,
                        ModelObjectSerializerProjectGuid, ModelObjectsSerializerRootNameSpace,
                        contractDataEntityProjectGuid, contractsEntityRootNameSpace,
                        contractSerializerProjectGuid, contractsSerializerRootNameSpace,
                        hostAccessProjectGuids, HostAccessRootNameSpaces);
                    codeGenerator.ContentTemplate
                        = RetrieveTemplate("Solution");

                    //Method to record start time in a hashtable with filename,start time as key-value pairs
                    StartTimeCapture(outputDirectory + DirectoryPathSeperator + SolutionFileName);

                    CreateFile(outputDirectory,
                        SolutionFileName, codeGenerator.GenerateContent());

                    //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                    RecordTime(outputDirectory + DirectoryPathSeperator + SolutionFileName);
                }
            }
            

            //Generate WebService projects
            if (frm.PRTcheServiceInterface && frm.PRTRadioWebService)
            {
                Guid WebServiceDataContractGuid = Guid.NewGuid();
                Guid WebServiceContractGuid = Guid.NewGuid();
                Guid WebServiceImplementationGuid = Guid.NewGuid();
                Guid WebServiceHostGuid = Guid.NewGuid();

                string HostAccessRootNameSpace = projectToBeGeneratedFor.ContractNamespaces.HostAccessRootNamespace;
                string WebServiceDataContractNameSpace = ProjectToBeGeneratedFor.ProjectPrefix + "LegacyDataContract";
                string WebServiceContractNameSpace = ProjectToBeGeneratedFor.ProjectPrefix + "ServiceContract";
                string WebServiceImplementationNameSpace = ProjectToBeGeneratedFor.ProjectPrefix + "ServiceImplementation";
                string contractDataEntityNamespace = projectToBeGeneratedFor.ContractNamespaces.DataEntityRootNamespace;

                //Data Contract Project
                ProjectFiles.WCFDataContractCP codeGenerator = new ProjectFiles.WCFDataContractCP(
                    WebServiceDataContractGuid, WebServiceDataContractNameSpace);
                codeGenerator.ContentTemplate = RetrieveTemplate("WebServiceDataContractProject");

                //Method to record start time in a hashtable with filename,start time as key-value pairs
                StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                    DirectoryPathSeperator + WebServiceDataContract + DirectoryPathSeperator + 
                    WebServiceDataContractNameSpace + ".csproj");

                CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator + 
                    WebServiceDataContract, WebServiceDataContractNameSpace + ".csproj", 
                    codeGenerator.GenerateContent());

                //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator + 
                    WebServiceDataContract + DirectoryPathSeperator + WebServiceDataContractNameSpace + ".csproj");


                //Service Contract Project
                ProjectFiles.WCFServiceContractandSeviceImplementationCP codeGeneratorSC =
                    new ProjectFiles.WCFServiceContractandSeviceImplementationCP(WebServiceContractGuid,
                     contractDataEntityProjectGuid, WebServiceDataContractGuid, 
                     ProjectToBeGeneratedFor.ContractModules, WebServiceDataContractNameSpace, 
                     WebServiceContractNameSpace, contractDataEntityNamespace);

                codeGeneratorSC.ContentTemplate = RetrieveTemplate("WebServiceContractProject");

                //Method to record start time in a hashtable with filename,start time as key-value pairs
                StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                    DirectoryPathSeperator + WebServiceContract + DirectoryPathSeperator + WebServiceContractNameSpace + ".csproj");

                CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator + 
                    WebServiceContract, WebServiceContractNameSpace + ".csproj", codeGeneratorSC.GenerateContent());

                //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator + 
                    WebServiceContract + DirectoryPathSeperator + WebServiceContractNameSpace + ".csproj");


                //Service Implementation Project
                ProjectFiles.WCFServiceContractandSeviceImplementationCP codeGeneratorSI =
                    new ProjectFiles.WCFServiceContractandSeviceImplementationCP(WebServiceImplementationGuid,
                    contractDataEntityProjectGuid, WebServiceDataContractGuid, WebServiceContractGuid,
                    ProjectToBeGeneratedFor.ContractModules, WebServiceDataContractNameSpace,
                    WebServiceContractNameSpace,WebServiceImplementationNameSpace, contractDataEntityNamespace);

                codeGeneratorSI.ContentTemplate = RetrieveTemplate("WebServiceImplementationProject");

                //Method to record start time in a hashtable with filename,start time as key-value pairs
                StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                    DirectoryPathSeperator + WebServiceImplementation + DirectoryPathSeperator + 
                    WebServiceImplementationNameSpace + ".csproj");

                CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator + 
                    WebServiceImplementation, WebServiceImplementationNameSpace + ".csproj", 
                    codeGeneratorSI.GenerateContent());

                //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator + 
                    WebServiceImplementation + DirectoryPathSeperator + WebServiceImplementationNameSpace + ".csproj");

                //generate web service solution
                ProjectFiles.WCFSolutionCP codeGeneratorWCFSoln = new ProjectFiles.WCFSolutionCP(solutionGuid, 
                    contractDataEntityProjectGuid, WebServiceDataContractGuid, WebServiceContractGuid, 
                    WebServiceImplementationGuid, ModelObjectDataEntityProjectGuid, contractSerializerProjectGuid, 
                    ModelObjectSerializerProjectGuid, contractsEntityRootNameSpace, ModelObjectsEntityRootNameSpace,
                    contractsSerializerRootNameSpace, ModelObjectsSerializerRootNameSpace, 
                    WebServiceDataContractNameSpace, WebServiceContractNameSpace, 
                    WebServiceImplementationNameSpace, WebServiceHostGuid);

                codeGeneratorWCFSoln.ContentTemplate
                    = RetrieveTemplate("WebServiceSolution");

                //Method to record start time in a hashtable with filename,start time as key-value pairs
                StartTimeCapture(outputDirectory + DirectoryPathSeperator + SolutionFileName);

                CreateFile(outputDirectory,
                    SolutionFileName, codeGeneratorWCFSoln.GenerateContent());

                //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                RecordTime(outputDirectory + DirectoryPathSeperator + SolutionFileName);
            }
            

            //Generate WCF Projects  
            if (frm.PRTcheServiceInterface && frm.PRTRadioWCFService)
            {                
                Guid WCFDataContractGuid = Guid.NewGuid();
                Guid WCFServiceContractGuid = Guid.NewGuid();
                Guid WCFServiceImplementationGuid = Guid.NewGuid();
                Guid WCFHostGuid = Guid.NewGuid();

                string WCFDataContractNameSpace = ProjectToBeGeneratedFor.ProjectPrefix + "LegacyDataContract";
                string WCFServiceContractNameSpace = ProjectToBeGeneratedFor.ProjectPrefix + "ServiceContract";
                string WCFServiceImplementationNameSpace = ProjectToBeGeneratedFor.ProjectPrefix + "ServiceImplementation";
                string contractDataEntityNamespace = projectToBeGeneratedFor.ContractNamespaces.DataEntityRootNamespace;


                //Data Contract Project
                ProjectFiles.WCFDataContractCP codeGenerator = new ProjectFiles.WCFDataContractCP(
                    WCFDataContractGuid, WCFDataContractNameSpace);
                codeGenerator.ContentTemplate = RetrieveTemplate("WCFDataContractProjectTemplate");

                //Method to record start time in a hashtable with filename,start time as key-value pairs
                StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                    DirectoryPathSeperator + WCFDataContract + DirectoryPathSeperator + 
                    WCFDataContractNameSpace + ".csproj");

                CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator + 
                    WCFDataContract, WCFDataContractNameSpace + ".csproj", codeGenerator.GenerateContent());

                //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator +
                    WCFDataContract + DirectoryPathSeperator + WCFDataContractNameSpace + ".csproj");


                //Service Contract Project
                ProjectFiles.WCFServiceContractandSeviceImplementationCP codeGeneratorSC = 
                    new ProjectFiles.WCFServiceContractandSeviceImplementationCP(WCFServiceContractGuid, 
                    contractDataEntityProjectGuid, WCFDataContractGuid, projectToBeGeneratedFor.ContractModules, 
                    WCFDataContractNameSpace,WCFServiceContractNameSpace, contractDataEntityNamespace);

                codeGeneratorSC.ContentTemplate = RetrieveTemplate("WCFServiceContractProjectTemplate");

                //Method to record start time in a hashtable with filename,start time as key-value pairs
                StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                    DirectoryPathSeperator + WCFServiceContract + DirectoryPathSeperator + 
                    WCFServiceContractNameSpace + ".csproj");

                CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator + 
                    WCFServiceContract, WCFServiceContractNameSpace + ".csproj", codeGeneratorSC.GenerateContent());

                //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator +
                    WCFServiceContract + DirectoryPathSeperator + WCFServiceContractNameSpace + ".csproj");


                //Service Implementation Project
                ProjectFiles.WCFServiceContractandSeviceImplementationCP codeGeneratorSI = 
                    new ProjectFiles.WCFServiceContractandSeviceImplementationCP(WCFServiceImplementationGuid, 
                    contractDataEntityProjectGuid, ModelObjectDataEntityProjectGuid, WCFDataContractGuid,
                    WCFServiceContractGuid, projectToBeGeneratedFor.ContractModules, WCFDataContractNameSpace, 
                    WCFServiceContractNameSpace,WCFServiceImplementationNameSpace, contractDataEntityNamespace);

                codeGeneratorSI.ContentTemplate = RetrieveTemplate("WCFServiceImplementationProject");

                //Method to record start time in a hashtable with filename,start time as key-value pairs
                StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator +
                    WCFServiceImplementation + DirectoryPathSeperator + WCFServiceImplementationNameSpace + ".csproj");

                CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator + WCFServiceImplementation,
                    WCFServiceImplementationNameSpace + ".csproj",
                    codeGeneratorSI.GenerateContent());

                //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator +
                    WCFServiceImplementation + DirectoryPathSeperator + WCFServiceImplementationNameSpace + ".csproj");

                //generate solution
                ProjectFiles.WCFSolutionCP codeGeneratorWCFSoln = new ProjectFiles.WCFSolutionCP(solutionGuid, contractDataEntityProjectGuid, 
                    WCFDataContractGuid, WCFServiceContractGuid, WCFServiceImplementationGuid, ModelObjectDataEntityProjectGuid, 
                    contractSerializerProjectGuid, ModelObjectSerializerProjectGuid, contractsEntityRootNameSpace, 
                    ModelObjectsEntityRootNameSpace, contractsSerializerRootNameSpace, ModelObjectsSerializerRootNameSpace,
                    WCFDataContractNameSpace, WCFServiceContractNameSpace,WCFServiceImplementationNameSpace, WCFHostGuid);

                codeGeneratorWCFSoln.ContentTemplate = RetrieveTemplate("WCFSolution");

                //Method to record start time in a hashtable with filename,start time as key-value pairs
                StartTimeCapture(outputDirectory + DirectoryPathSeperator + SolutionFileName);

                CreateFile(outputDirectory, SolutionFileName, codeGeneratorWCFSoln.GenerateContent());

                //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                RecordTime(outputDirectory + DirectoryPathSeperator + SolutionFileName);
            }            
            
            
            //Generate RSS Feed projects
            if (frm.PRTcheServiceInterface && (frm.PRTRadioRSSFeed || frm.PrtRadioAtom))
            {
                Guid WCFDataContractGuid = Guid.NewGuid();
                Guid WCFServiceContractGuid = Guid.NewGuid();
                Guid WCFServiceImplementationGuid = Guid.NewGuid();
                Guid WCFHostGuid = Guid.NewGuid();

                string WCFDataContractNameSpace = ProjectToBeGeneratedFor.ProjectPrefix + "LegacyDataContract";
                string WCFServiceContractNameSpace = ProjectToBeGeneratedFor.ProjectPrefix + "ServiceContract";
                string WCFServiceImplementationNameSpace = ProjectToBeGeneratedFor.ProjectPrefix + "ServiceImplementation";
                string contractDataEntityNamespace = projectToBeGeneratedFor.ContractNamespaces.DataEntityRootNamespace;
                string modelObjectDataEntityNamespace = projectToBeGeneratedFor.ModelObjectNamespaces.DataEntityRootNamespace;
                
                
                //Data Contract Project
                ProjectFiles.WCFDataContractCP codeGenerator = new ProjectFiles.WCFDataContractCP(
                    WCFDataContractGuid, WCFDataContractNameSpace);
                codeGenerator.ContentTemplate = RetrieveTemplate("RSSDataContractProject");

                //Method to record start time in a hashtable with filename,start time as key-value pairs
                StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                    DirectoryPathSeperator + WCFDataContract + DirectoryPathSeperator + 
                    WCFDataContractNameSpace + ".csproj");

                CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator +
                    WCFDataContract, WCFDataContractNameSpace + ".csproj", codeGenerator.GenerateContent());

                //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator +
                    WCFDataContract + DirectoryPathSeperator + WCFDataContractNameSpace + ".csproj");


                //Service Contract Project
                ProjectFiles.WCFServiceContractandSeviceImplementationCP codeGeneratorSC = 
                    new ProjectFiles.WCFServiceContractandSeviceImplementationCP(WCFServiceContractGuid, 
                    contractDataEntityProjectGuid, WCFDataContractGuid, projectToBeGeneratedFor.ContractModules,
                    WCFDataContractNameSpace, WCFServiceContractNameSpace, contractDataEntityNamespace);

                codeGeneratorSC.ContentTemplate = RetrieveTemplate("RSSServiceContractProject");

                //Method to record start time in a hashtable with filename,start time as key-value pairs
                StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                    DirectoryPathSeperator + WCFServiceContract + DirectoryPathSeperator + 
                    WCFServiceContractNameSpace + ".csproj");

                CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator +
                    WCFServiceContract, WCFServiceContractNameSpace + ".csproj", codeGeneratorSC.GenerateContent());

                //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator +
                    WCFServiceContract + DirectoryPathSeperator + WCFServiceContractNameSpace + ".csproj");

                
                //Service Implementation Project
                ProjectFiles.WCFServiceContractandSeviceImplementationCP codeGeneratorSI = 
                    new ProjectFiles.WCFServiceContractandSeviceImplementationCP(WCFServiceImplementationGuid, 
                    contractDataEntityProjectGuid, ModelObjectDataEntityProjectGuid, WCFDataContractGuid,
                    WCFServiceContractGuid, projectToBeGeneratedFor.ContractModules, WCFDataContractNameSpace,
                    WCFServiceContractNameSpace, WCFServiceImplementationNameSpace, contractDataEntityNamespace, 
                    modelObjectDataEntityNamespace);

                codeGeneratorSI.ContentTemplate = RetrieveTemplate("RSSServiceImplementationProject");

                //Method to record start time in a hashtable with filename,start time as key-value pairs
                StartTimeCapture(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + 
                    DirectoryPathSeperator + WCFServiceImplementation + DirectoryPathSeperator + 
                    WCFServiceImplementationNameSpace + ".csproj");

                CreateFile(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator + 
                    WCFServiceImplementation, WCFServiceImplementationNameSpace + ".csproj",
                    codeGeneratorSI.GenerateContent());

                //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                RecordTime(outputDirectory + DirectoryPathSeperator + HostAccessDirectory + DirectoryPathSeperator +
                    WCFServiceImplementation + DirectoryPathSeperator + WCFServiceImplementationNameSpace + ".csproj");


                //generate RSS solution
                ProjectFiles.WCFSolutionCP codeGeneratorWCFSoln = new ProjectFiles.WCFSolutionCP(solutionGuid, 
                    contractDataEntityProjectGuid, WCFDataContractGuid, WCFServiceContractGuid, 
                    WCFServiceImplementationGuid, ModelObjectDataEntityProjectGuid, contractSerializerProjectGuid, 
                    ModelObjectSerializerProjectGuid, contractsEntityRootNameSpace, ModelObjectsEntityRootNameSpace, 
                    contractsSerializerRootNameSpace, ModelObjectsSerializerRootNameSpace, WCFDataContractNameSpace, 
                    WCFServiceContractNameSpace, WCFServiceImplementationNameSpace, WCFHostGuid);

                codeGeneratorWCFSoln.ContentTemplate = RetrieveTemplate("RSSSolution");

                //Method to record start time in a hashtable with filename,start time as key-value pairs
                StartTimeCapture(outputDirectory + DirectoryPathSeperator + SolutionFileName);

                CreateFile(outputDirectory, SolutionFileName, codeGeneratorWCFSoln.GenerateContent());

                //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                RecordTime(outputDirectory + DirectoryPathSeperator + SolutionFileName);
                                
                
                // ModelObject Data Entity Project file for RSS
                ModelObjectsEntityRootNameSpace = projectToBeGeneratedFor.ModelObjectNamespaces.DataEntityRootNamespace;

                if (frm.PRTDataentityModuleObjects)
                {
                    ProjectFiles.ModelObjectDataEntityProjectCP RSScodeGenerator = 
                        new ProjectFiles.ModelObjectDataEntityProjectCP(ModelObjectDataEntityProjectGuid, 
                        projectToBeGeneratedFor.ModelObjectModules, ModelObjectsEntityRootNameSpace);

                    RSScodeGenerator.ContentTemplate = RetrieveTemplate("RSSModelObjectEntityProject");

                    //Method to record start time in a hashtable with filename,start time as key-value pairs
                    StartTimeCapture(outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory + 
                        DirectoryPathSeperator + ModelObjectsDirectory + DirectoryPathSeperator + "Code" + 
                        DirectoryPathSeperator + ModelObjectsEntityRootNameSpace + ".csproj");

                    CreateFile(outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory + 
                        DirectoryPathSeperator + ModelObjectsDirectory + DirectoryPathSeperator + "Code",
                        ModelObjectsEntityRootNameSpace + ".csproj", RSScodeGenerator.GenerateContent());

                    //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                    RecordTime(outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory + 
                        DirectoryPathSeperator + ModelObjectsDirectory + DirectoryPathSeperator + "Code" + 
                        DirectoryPathSeperator + ModelObjectsEntityRootNameSpace + ".csproj");
                }
                
                
                //Generate Contract Data Entity Project file for RSS
                contractsEntityRootNameSpace = projectToBeGeneratedFor.ContractNamespaces.DataEntityRootNamespace;

                if (frm.PRTDataEntityContracts)
                {
                    ProjectFiles.ContractDataEntityProjectCP RSScodeGenerator
                        = new ProjectFiles.ContractDataEntityProjectCP(contractDataEntityProjectGuid,
                        projectToBeGeneratedFor.ContractModules,contractsEntityRootNameSpace, 
                        ModelObjectsEntityRootNameSpace, ModelObjectDataEntityProjectGuid);

                    RSScodeGenerator.ContentTemplate = RetrieveTemplate("RSSContractEntityProjectTemplate");

                    //Method to record start time in a hashtable with filename,start time as key-value pairs
                    StartTimeCapture(outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory + 
                        DirectoryPathSeperator + ContractsDirectory + DirectoryPathSeperator + "Code" + 
                        DirectoryPathSeperator + contractsEntityRootNameSpace + ".csproj");

                    CreateFile(outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory + 
                        DirectoryPathSeperator + ContractsDirectory + DirectoryPathSeperator + "Code",
                        contractsEntityRootNameSpace + ".csproj", RSScodeGenerator.GenerateContent());

                    //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                    RecordTime(outputDirectory + DirectoryPathSeperator + DataEntitiesDirectory + 
                        DirectoryPathSeperator + ContractsDirectory + DirectoryPathSeperator + "Code" + 
                        DirectoryPathSeperator + contractsEntityRootNameSpace + ".csproj");
                }
                

                //Generate ModelObject Serializer Project file for RSS
                ModelObjectsSerializerRootNameSpace = projectToBeGeneratedFor.ModelObjectNamespaces.SerializerRootNamespace;

                if (frm.PRTSerializerModelObjects)
                {
                    ProjectFiles.ModelObjectSerializerProjectCP RSScodeGenerator = 
                        new ProjectFiles.ModelObjectSerializerProjectCP(contractSerializerProjectGuid, 
                        projectToBeGeneratedFor.ModelObjectModules, ModelObjectsSerializerRootNameSpace, 
                        ModelObjectsEntityRootNameSpace, ModelObjectDataEntityProjectGuid);

                    RSScodeGenerator.ContentTemplate = RetrieveTemplate("RSSModelObjectSerializerProject");

                    //Method to record start time in a hashtable with filename,start time as key-value pairs
                    StartTimeCapture(outputDirectory + DirectoryPathSeperator + SerializersDirectory + 
                        DirectoryPathSeperator + ModelObjectsDirectory + DirectoryPathSeperator + 
                        ModelObjectsSerializerRootNameSpace + ".csproj");

                    CreateFile(outputDirectory + DirectoryPathSeperator + SerializersDirectory + 
                        DirectoryPathSeperator + ModelObjectsDirectory, ModelObjectsSerializerRootNameSpace + 
                        ".csproj", RSScodeGenerator.GenerateContent());

                    //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                    RecordTime(outputDirectory + DirectoryPathSeperator + SerializersDirectory + 
                        DirectoryPathSeperator + ModelObjectsDirectory + DirectoryPathSeperator + 
                        ModelObjectsSerializerRootNameSpace + ".csproj");
                }
                
                
                //Generate Contract Serializer Project file for RSS
                contractsSerializerRootNameSpace = projectToBeGeneratedFor.ContractNamespaces.SerializerRootNamespace;

                if (frm.PRTSerializerContract)
                {
                    ProjectFiles.ContractSerializerProjectCP RSScodeGenerator
                        = new ProjectFiles.ContractSerializerProjectCP(contractSerializerProjectGuid, 
                        projectToBeGeneratedFor.ContractModules, ModelObjectsEntityRootNameSpace, 
                        ModelObjectsSerializerRootNameSpace, contractsEntityRootNameSpace, 
                        contractsSerializerRootNameSpace, contractDataEntityProjectGuid, 
                        ModelObjectDataEntityProjectGuid, ModelObjectSerializerProjectGuid);
                    
                    RSScodeGenerator.ContentTemplate = RetrieveTemplate("RSSContractSerializerProject");

                    //Method to record start time in a hashtable with filename,start time as key-value pairs
                    StartTimeCapture(outputDirectory + DirectoryPathSeperator + SerializersDirectory + 
                        DirectoryPathSeperator + ContractsDirectory + DirectoryPathSeperator + 
                        contractsSerializerRootNameSpace + ".csproj");

                    CreateFile(outputDirectory + DirectoryPathSeperator + SerializersDirectory + 
                        DirectoryPathSeperator + ContractsDirectory, contractsSerializerRootNameSpace + ".csproj",
                        RSScodeGenerator.GenerateContent());

                    //Method to record the time taken for generation in the hashtable with filename, time taken as key-value pairs
                    RecordTime(outputDirectory + DirectoryPathSeperator + SerializersDirectory + 
                        DirectoryPathSeperator + ContractsDirectory + DirectoryPathSeperator + 
                        contractsSerializerRootNameSpace + ".csproj");
                }                                
            }

            //check if the solution is to be compiled i.e. Compile option is checked
            if (frm.IsToBeCompiled)
            {
                //call method to compile the generated solution.
                Compile(outputDirectory, SolutionFileName);

                // Now we should copy the files to deployment directory if necessary
                if (frm.WebServicesToBeGenerated)
                {
                    AspnetCompile(outputDirectory + DirectoryPathSeperator + WebServiceInterfaceDirectory);
                }
            }
            
            //call this method only if view report is selected.
            if (frm.IsReportToBeGenerated)
            {
                Infosys.Lif.LegacyWorkbench.ReportManager.ReportGenerator.GenerateXMLForReport(outputDirectory, 
                    frm.SolnFileName, frm.ServiceInterface);
                Infosys.Lif.LegacyWorkbench.ReportManager.ReportGenerator.CleanUpStaticVariables();
            }            
            isErrorOccurred = false;
        }

        /// <summary>
        /// calls aspnet_compiler utility for the generated solution.
        /// </summary>
        /// <param name="webServicesDirectory"></param>
        private void AspnetCompile(string webServicesDirectory)
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            // Execute aspnet_compiler located in the .NET framework direcotry.
            startInfo.FileName = DotNetFrameworkDirectory + "aspnet_compiler";
            startInfo.WorkingDirectory = outputDirectory;
            startInfo.Arguments = SolutionFileName + " -v /HostAccess/WebServices -p " + webServicesDirectory;
            System.Diagnostics.Process.Start(startInfo);
        }

        /// <summary>
        /// Builds the generated solution.
        /// </summary>
        /// <param name="outputDirectory"></param>
        /// <param name="SolutionFileName"></param>
        private void Compile(string outputDirectory, string SolutionFileName)
        {
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            // Execute msbuild located in the .NET framework direcotry.
            startInfo.FileName = "\"" + DotNetFrameworkDirectory + "msbuild.exe" + "\"";

            startInfo.WorkingDirectory = outputDirectory;
            //string temp = "\""+ outputDirectory + @"\" + SolutionFileName + "\"";
            startInfo.Arguments = SolutionFileName + " /t:Rebuild /p:Configuration=Release";
            //startInfo.Arguments = temp + " /t:Rebuild /p:Configuration=Release";
            startInfo.UseShellExecute = false;
            System.Diagnostics.Process.Start(startInfo);

        }           
        

        /// <summary>
        /// Returns the template for a template name in string format.
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns>Template</returns>
        private Template RetrieveTemplate(string templateName)
        {
            //it will append .txt 
            templateName = Helper.apendtxt(templateName);
            return Template.FromFile(templatesDirectory + templateName);
        }

        /// <summary>
        /// Creates a file of given name in given path with provided contenets.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="fileName"></param>
        /// <param name="contentToBeWritten"></param>
        private void CreateFile(string directoryPath, string fileName, string contentToBeWritten)
        {
            System.IO.Directory.CreateDirectory(directoryPath);
            System.IO.StreamWriter sw
                = new System.IO.StreamWriter(directoryPath + DirectoryPathSeperator + fileName, false);
            sw.Write(contentToBeWritten);
            sw.Close();
        }

        // Start time called only if the user wants to view report.
        /// <summary>
        /// Calls Report Generator's method to capture the time in generating a file.
        /// </summary>
        /// <param name="absoluteFilePath"></param>
        private void StartTimeCapture(string absoluteFilePath)
        {
            //put if condition here
            if (frm.IsReportToBeGenerated)
            {
                Infosys.Lif.LegacyWorkbench.ReportManager.ReportGenerator.StartTimeCapture(absoluteFilePath);
            }
        }

        // Record time called only if the user wants to view report.
        /// <summary>
        /// Calls Report Generator's method to store the captured time for generating a file.
        /// </summary>
        /// <param name="absoluteFilePath"></param>
        private void RecordTime(string absoluteFilePath)
        {
            //put if condition here
            if (frm.IsReportToBeGenerated)
            {
                Infosys.Lif.LegacyWorkbench.ReportManager.ReportGenerator.RecordTime(absoluteFilePath);
            }
        }
    }
    
    internal static class Mappings
    {

        /// <summary>
        /// This will contain the ModuleName:EntityName as the key
        /// and the C# namespace as the key
        /// </summary>
        internal static System.Collections.Hashtable CSharpNameSpaceMappings
            = new System.Collections.Hashtable();

        /// <summary>
        /// This will contain the ModuleName:EntityName as the key
        /// and the XSD namespace as the key
        /// </summary>
        internal static System.Collections.Hashtable XsdNameSpaceMappings
            = new System.Collections.Hashtable();

        /// <summary>
        /// This will contain the ModuleName:EntityName as the key
        /// and the C# namespace as the key
        /// </summary>
        internal static System.Collections.Hashtable NameSpaceServiceMapping
            = new System.Collections.Hashtable();


        internal static System.Collections.Hashtable contractModelObjectMapping
            = new System.Collections.Hashtable();


        /// <summary>
        /// Initializes all hashtables to store namespace mappings.
        /// </summary>
        internal static void Initialize()
        {
            CSharpNameSpaceMappings = new System.Collections.Hashtable();
            XsdNameSpaceMappings = new System.Collections.Hashtable();
            NameSpaceServiceMapping = new System.Collections.Hashtable();
            contractModelObjectMapping = new System.Collections.Hashtable();
        }
    }
}
