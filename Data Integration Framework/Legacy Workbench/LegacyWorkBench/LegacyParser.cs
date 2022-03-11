using System;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;
using System.Configuration;
using Infosys.Lif.LegacyWorkbench.Framework;
using Infosys.Lif.LegacyWorkbench.Entities;
using Infosys.Lif.LegacyWorkbench.Editors;
using Infosys.Lif.LegacyWorkbench.LegacyWorkbenchConfigurations;
using Microsoft.Reporting.WinForms;
using System.Data;

namespace Infosys.Lif.LegacyWorkbench
{
    /// <summary>
    /// This is the main form of the Legacy Workbench tool. This is the main controller class for the tool. 
    /// This class contains all properties, functions and events for the controls in the form. This form holds 
    /// all other user controls i.e. project editor, Module editor, model object editor and contract editor on 
    /// different tabs & file importer for model objects and contracts.
    /// </summary>
    public partial class LegacyParser : Form
    {
        string extensionsCodeGroup = "";                
        public static string addedCalled = "No";
        private const string LEGACY_WORKBENCH = "LegacyWorkbenchConfigurations";
        Hashtable modelObjectIdFileMapping = null;
        private LegacyWorkbenchConfigurations.LegacyWorkbenchConfigurations legacyWorkbenchSettings;
        
        string modelObjDataEntityNS;

        /// <summary>
        /// Model object data entity namespace
        /// </summary>
        public string ModelObjDataEntityNS
        {
            get { return modelObjDataEntityNS; }
            set { modelObjDataEntityNS = value; }
        }

        string modelObjDataEntityRootNS;

        /// <summary>
        /// Model object data entity assembly namespace
        /// </summary>
        public string ModelObjDataEntityRootNS
        {
            get { return modelObjDataEntityRootNS; }
            set { modelObjDataEntityRootNS = value; }
        }

        string modelObjSerializerNS;

        /// <summary>
        /// Model object serializer namespace
        /// </summary>
        public string ModelObjSerializerNS
        {
            get { return modelObjSerializerNS; }
            set { modelObjSerializerNS = value; }
        }
        
        string modelObjSerializerRootNS;

        /// <summary>
        /// Model object serializer assembly namespace
        /// </summary>
        public string ModelObjSerializerRootNS
        {
            get { return modelObjSerializerRootNS; }
            set { modelObjSerializerRootNS = value; }
        }

        string modelObjXmlNS;

        /// <summary>
        /// Model object XML namespace to be used in XSD
        /// </summary>
        public string ModelObjXmlNS
        {
            get { return modelObjXmlNS; }
            set { modelObjXmlNS = value; }
        }

        string contractDataEntityNS;

        /// <summary>
        /// Contract data entity namespace
        /// </summary>
        public string ContractDataEntityNS
        {
            get { return contractDataEntityNS; }
            set { contractDataEntityNS = value; }
        }
        
        string contractDataEntityRootNS;

        /// <summary>
        /// Contract data entity assembly namespace
        /// </summary>
        public string ContractDataEntityRootNS
        {
            get { return contractDataEntityRootNS; }
            set { contractDataEntityRootNS = value; }
        }
        
        string contractSerializerNS;

        /// <summary>
        /// Contract serializer namespace
        /// </summary>
        public string ContractSerializerNS
        {
            get { return contractSerializerNS; }
            set { contractSerializerNS = value; }
        }

        string contractSerializerRootNS;

        /// <summary>
        /// Contract serializer assembly namespace
        /// </summary>
        public string ContractSerializerRootNS
        {
            get { return contractSerializerRootNS; }
            set { contractSerializerRootNS = value; }
        }

        string contractHostAccessNS;

        /// <summary>
        /// Contract host access namespace
        /// </summary>
        public string ContractHostAccessNS
        {
            get { return contractHostAccessNS; }
            set { contractHostAccessNS = value; }
        }

        string contractHostAccessRootNS;

        /// <summary>
        /// Contract host access assembly namespace
        /// </summary>
        public string ContractHostAccessRootNS
        {
            get { return contractHostAccessRootNS; }
            set { contractHostAccessRootNS = value; }
        }
        
        string contractXmlNS;

        /// <summary>
        /// Contract XML namespace to be used in XSD
        /// </summary>
        public string ContractXmlNS
        {
            get { return contractXmlNS; }
            set { contractXmlNS = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadFromDirectory()
        {
            modelObjectIdFileMapping = new Hashtable();

            string PreLoadModelObjectPath = System.Configuration.ConfigurationManager.AppSettings["PreLoadModelObjectPath"];
            // If the Preload path is empty, ignore otherwise load from the preloader directory
            if (!(PreLoadModelObjectPath == null || PreLoadModelObjectPath.Length == 0))
            {

                string[] filesFromPreLoadDirectory = System.IO.Directory.GetFiles(PreLoadModelObjectPath);
                if (filesFromPreLoadDirectory.Length == 0)
                {
                    for (int fileLooper = 0; fileLooper < filesFromPreLoadDirectory.Length; fileLooper++)
                    {
                        Entities.Entity entityRetrieved = modelObjectImporter.RetrieveModelObjectDetails(filesFromPreLoadDirectory[fileLooper]);
                        modelObjectIdFileMapping.Add(entityRetrieved.ProgramId, filesFromPreLoadDirectory[fileLooper]);
                    }
                }
            }
        }

        //getting the selected module number
        int nodeCounter = 0;
        const string RootNodeText = "Root";
        const string UngroupedNodeText = "UnGrouped";

        const int TabbtnModelObjectsIndex = 0;
        const int TabContractsIndex = 1;
        const int TabSummaryIndex = 2;
        
        static public bool calledClearMethod;
        
        StatusDisplay statusDisplay;
        Framework.IModelObjectImporter modelObjectImporter = null;
        Framework.ICodeGenerator codeGenerator = null;

        IContractImporter contractsImporter;
        bool isProjectEditorDirty;

        private Hashtable htReports = new Hashtable();
        private Hashtable htReportsForMenu = new Hashtable();

        /// <summary>
        /// Constructor which initializes all hash tables to be used, retrieves config details,
        /// initlializes report data, initializes all controls
        /// </summary>
        public LegacyParser()
        {
            Hashtable htCodeGroup = new Hashtable();
            Hashtable htCapersJonesConversionFactors = new Hashtable();
            Hashtable htCustomConversionFactors = new Hashtable();                       

            decimal conversionFactor = 0;

            //retrieve config details
            legacyWorkbenchSettings = (LegacyWorkbenchConfigurations.LegacyWorkbenchConfigurations)
                ConfigurationManager.GetSection(LEGACY_WORKBENCH);
            
            ReportingConfigurationsType reportingConfigurations = new ReportingConfigurationsType();           
            reportingConfigurations = (ReportingConfigurationsType)legacyWorkbenchSettings.ReportingConfigurations;


            //Adding all the file Names and their respective locations in the htReports hashtable
            int itemsInReportsColl = 0;
            while (itemsInReportsColl < reportingConfigurations.Reports.ReportCollection.Count)
            {
                if (!htReports.Contains(reportingConfigurations.Reports.ReportCollection[itemsInReportsColl].Name))
                {
                    htReports.Add(reportingConfigurations.Reports.ReportCollection[itemsInReportsColl].Name, reportingConfigurations.Reports.ReportCollection[itemsInReportsColl].FileLocation);
                    itemsInReportsColl++;
                }
            }

            itemsInReportsColl = 0;
            while (itemsInReportsColl < reportingConfigurations.Reports.ReportCollection.Count)
            {
                if (!htReportsForMenu.Contains(reportingConfigurations.Reports.ReportCollection[itemsInReportsColl].Alias))
                {
                    htReportsForMenu.Add(reportingConfigurations.Reports.ReportCollection[itemsInReportsColl].Alias, reportingConfigurations.Reports.ReportCollection[itemsInReportsColl].FileLocation);
                    itemsInReportsColl++;
                }
            }

            //Adding all the file extension code group names and their values in the htCodeGroup hashtable
            int itemsInFileTypeColl = 0;
            while (itemsInFileTypeColl < reportingConfigurations.FileTypeAssociations.FileTypeCollection.Count)
            {
                if (!htCodeGroup.Contains(reportingConfigurations.FileTypeAssociations.FileTypeCollection[itemsInFileTypeColl].Key))
                {
                    htCodeGroup.Add(reportingConfigurations.FileTypeAssociations.FileTypeCollection[itemsInFileTypeColl].Key, reportingConfigurations.FileTypeAssociations.FileTypeCollection[itemsInFileTypeColl].Value);
                    itemsInFileTypeColl++;
                }
            }

            //adding the Capers Jones conversion factors related to the extensions to the hashtable htCapersJonesConversionFactors(change name).
            {
                int countExtensionsToAnalyse = 0;

                //scanning through all the extensions to analyse.
                while (countExtensionsToAnalyse < legacyWorkbenchSettings.ReportingConfigurations.EffortAnalysis.CapersJones.ExtensionsToAnalyze.Count)
                {
                    string extensionsGroup = "";
                    string excludeExtensions = "";

                    extensionsGroup = legacyWorkbenchSettings.ReportingConfigurations.EffortAnalysis.CapersJones.ExtensionsToAnalyze.ExtensionMappingCollection[countExtensionsToAnalyse].GroupKey.ToString();
                    excludeExtensions = legacyWorkbenchSettings.ReportingConfigurations.EffortAnalysis.CapersJones.ExtensionsToAnalyze.ExtensionMappingCollection[countExtensionsToAnalyse].ExcludeExtensions.ToString();

                    int countFileTypeColl = 0;
                    while (countFileTypeColl < legacyWorkbenchSettings.ReportingConfigurations.FileTypeAssociations.FileTypeCollection.Count)
                    {
                        if (legacyWorkbenchSettings.ReportingConfigurations.FileTypeAssociations.FileTypeCollection[countFileTypeColl].Key == extensionsGroup)
                        {
                            Array extnsGp = legacyWorkbenchSettings.ReportingConfigurations.FileTypeAssociations.FileTypeCollection[countFileTypeColl].Value.ToString().Split(';');
                            Array excludeExtnsGp = excludeExtensions.Split(';');
                            foreach (string extension in extnsGp)
                            {
                                bool match = true;

                                foreach (string excludeExtension in excludeExtnsGp)
                                {
                                    //chking if the extension is not 1 of those to be excluded
                                    if (!(extension == excludeExtension))
                                    {
                                        //for the particular code language in extension mapping calculate the conversion factor from the collection of functionPointMapping and add the extension and corresponding conversion factor to the hashtable.
                                        int FnPointMappingCount = 0;
                                        while (FnPointMappingCount < legacyWorkbenchSettings.ReportingConfigurations.EffortAnalysis.CapersJones.FunctionPointMapping.Count)
                                        {
                                            //computing the conversion factor according to the language type.
                                            if (legacyWorkbenchSettings.ReportingConfigurations.EffortAnalysis.CapersJones.ExtensionsToAnalyze.ExtensionMappingCollection[countExtensionsToAnalyse].CodeLanguage == legacyWorkbenchSettings.ReportingConfigurations.EffortAnalysis.CapersJones.FunctionPointMapping.LanguageCollection[FnPointMappingCount].Name)
                                            {
                                                decimal PersonHrsPerFP = 0;
                                                PersonHrsPerFP = Convert.ToDecimal(legacyWorkbenchSettings.ReportingConfigurations.EffortAnalysis.CapersJones.FunctionPointMapping.LanguageCollection[FnPointMappingCount].PersonHrsPerFunctionPoint);
                                                decimal fPPerUnitLOC = Convert.ToDecimal(1 / Convert.ToDecimal(legacyWorkbenchSettings.ReportingConfigurations.EffortAnalysis.CapersJones.FunctionPointMapping.LanguageCollection[FnPointMappingCount].UnitLOCPerFunctionPoint));

                                                conversionFactor = PersonHrsPerFP * fPPerUnitLOC;
                                                match = false;
                                                break;
                                            }
                                            FnPointMappingCount++;
                                        }
                                    }
                                    else
                                    {
                                        match = true;
                                        break;
                                    }
                                }
                                ///////////////
                                if (match == false)
                                {
                                    if (!htCapersJonesConversionFactors.Contains(extension))
                                    {
                                        htCapersJonesConversionFactors.Add(extension, conversionFactor);
                                    }
                                }
                            }
                            break;
                        }
                        countFileTypeColl++;
                    }
                    countExtensionsToAnalyse++;
                }
                //repeat for custom
            }

            //adding the Custom conversion factors related to the extensions to the hashtable htCustomConversionFactors
            {
                int countExtensionsToAnalyse = 0;
                //scanning through all the extensions to analyse.
                while (countExtensionsToAnalyse < legacyWorkbenchSettings.ReportingConfigurations.EffortAnalysis.Custom.ExtensionsToAnalyze.Count)
                {
                    string extensionsGroup = "";
                    string excludeExtensions = "";

                    extensionsGroup = legacyWorkbenchSettings.ReportingConfigurations.EffortAnalysis.Custom.ExtensionsToAnalyze.ExtensionMappingCollection[countExtensionsToAnalyse].GroupKey.ToString();
                    excludeExtensions = legacyWorkbenchSettings.ReportingConfigurations.EffortAnalysis.Custom.ExtensionsToAnalyze.ExtensionMappingCollection[countExtensionsToAnalyse].ExcludeExtensions.ToString();

                    int countFileTypeColl = 0;
                    while (countFileTypeColl < legacyWorkbenchSettings.ReportingConfigurations.FileTypeAssociations.FileTypeCollection.Count)
                    {
                        if (legacyWorkbenchSettings.ReportingConfigurations.FileTypeAssociations.FileTypeCollection[countFileTypeColl].Key == extensionsGroup)
                        {
                            Array extnsGp = legacyWorkbenchSettings.ReportingConfigurations.FileTypeAssociations.FileTypeCollection[countFileTypeColl].Value.ToString().Split(';');
                            Array excludeExtnsGp = excludeExtensions.Split(';');
                            
                            foreach (string extension in extnsGp)
                            {
                                bool match = true;
                                foreach (string excludeExtension in excludeExtnsGp)
                                {
                                    //chking if the extension is not 1 of those to be excluded
                                    if (!(extension == excludeExtension))
                                    {
                                        //for the particular code language in extension mapping calculate the conversion factor from the collection of functionPointMapping and add the extension and corresponding conversion factor to the hashtable.
                                        int FnPointMappingCount = 0;
                                        while (FnPointMappingCount < legacyWorkbenchSettings.ReportingConfigurations.EffortAnalysis.Custom.FunctionPointMapping.Count)
                                        {
                                            //computing the conversion factor according to the language type.
                                            if (legacyWorkbenchSettings.ReportingConfigurations.EffortAnalysis.Custom.ExtensionsToAnalyze.ExtensionMappingCollection[countExtensionsToAnalyse].CodeLanguage == legacyWorkbenchSettings.ReportingConfigurations.EffortAnalysis.Custom.FunctionPointMapping.LanguageCollection[FnPointMappingCount].Name)
                                            {
                                                decimal PersonHrsPerFP = 0;
                                                PersonHrsPerFP = Convert.ToDecimal(legacyWorkbenchSettings.ReportingConfigurations.EffortAnalysis.Custom.FunctionPointMapping.LanguageCollection[FnPointMappingCount].PersonHrsPerFunctionPoint);
                                                decimal fPPerUnitLOC = Convert.ToDecimal(1 / Convert.ToDecimal(legacyWorkbenchSettings.ReportingConfigurations.EffortAnalysis.Custom.FunctionPointMapping.LanguageCollection[FnPointMappingCount].UnitLOCPerFunctionPoint));

                                                conversionFactor = PersonHrsPerFP * fPPerUnitLOC;
                                                match = false;
                                                break;
                                            }
                                            FnPointMappingCount++;
                                        }
                                    }
                                    else
                                    {
                                        match = true;
                                        break;
                                    }
                                }
                                if (match == false)
                                {
                                    if (!htCustomConversionFactors.Contains(extension))
                                    {
                                        htCustomConversionFactors.Add(extension, conversionFactor);
                                    }
                                }
                            }
                            break;
                        }
                        countFileTypeColl++;
                    }
                    countExtensionsToAnalyse++;
                }
            }
            
            //Calling the constructor of report generator, assigning values 
            Infosys.Lif.LegacyWorkbench.ReportManager.ReportGenerator reportGenerator = new Infosys.Lif.LegacyWorkbench.ReportManager.ReportGenerator(htCodeGroup,htCapersJonesConversionFactors,htCustomConversionFactors);

            //retrieve default namespace values in properties from config file
            //Model Obj Namespaces
            ModelObjDataEntityNS = legacyWorkbenchSettings.CodeGeneratedNamespaces.ModelObjectNamespaces.DataEntityNamespace;
            ModelObjDataEntityRootNS = legacyWorkbenchSettings.CodeGeneratedNamespaces.ModelObjectNamespaces.DataEntityRootNamespace;            
            ModelObjSerializerNS = legacyWorkbenchSettings.CodeGeneratedNamespaces.ModelObjectNamespaces.SerializerNamespace;
            ModelObjSerializerRootNS = legacyWorkbenchSettings.CodeGeneratedNamespaces.ModelObjectNamespaces.SerializerRootNamespace;
            ModelObjXmlNS = FormatXmlNamespace(legacyWorkbenchSettings.CodeGeneratedNamespaces.ModelObjectNamespaces.XmlNamespace);
                        
            //Contract Namespaces
            ContractDataEntityNS = legacyWorkbenchSettings.CodeGeneratedNamespaces.ContractNamespaces.DataEntityNamespace;
            ContractDataEntityRootNS = legacyWorkbenchSettings.CodeGeneratedNamespaces.ContractNamespaces.DataEntityRootNamespace;
            ContractHostAccessNS = legacyWorkbenchSettings.CodeGeneratedNamespaces.ContractNamespaces.HostAccessNamespace;
            ContractHostAccessRootNS = legacyWorkbenchSettings.CodeGeneratedNamespaces.ContractNamespaces.HostAccessRootNamespace;
            ContractSerializerNS = legacyWorkbenchSettings.CodeGeneratedNamespaces.ContractNamespaces.SerializerNamespace;
            ContractSerializerRootNS = legacyWorkbenchSettings.CodeGeneratedNamespaces.ContractNamespaces.SerializerRootNamespace;
            ContractXmlNS = FormatXmlNamespace(legacyWorkbenchSettings.CodeGeneratedNamespaces.ContractNamespaces.XmlNamespace);
            
            statusDisplay = new StatusDisplay();

            ungroupedbtnModelObjectsModule.Name = UngroupedNodeText;

            ungroupedContractModule.Name = UngroupedNodeText;

            InitializeComponent();

            Framework.Helper.SetHelperMethods(DisplaySummary, DisplayStatus,
                statusDisplay.statusProgressBar,
                statusDisplay.statusProgressLabel);
            Framework.Helper.DisplayCurrentStatus += new DisplayStatusEventHandler(Helper_DisplayCurrentStatus);

            Application.ThreadException
                += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);

            modelObjectEditor = new ModelObjectEditor();
            modelObjectEditor.Modified += new ModelObjectEditor.OnModified(LegacyParser_Modified);

            moduleEditor = new ModuleEditor();
            moduleEditor.Modified += new ModuleEditor.OnModified(moduleDetailsModified);

            contractProjectEditor = new ProjectEditor();
            contractProjectEditor.Modified += new ProjectEditor.OnModified(ContractsProjEditor_Modified);

            modelObjectProjectEditor = new ProjectEditor();
            modelObjectProjectEditor.Modified += new ProjectEditor.OnModified(ModelObjectsProjEditor_Modified);

            contractEditor = new ContractEditor();
            contractEditor.Modified += new ContractEditor.OnModified(LegacyParser_Modified);
            contractEditor.NavigateToModelObject
                += new Editors.ContractEditor.NavigateToModelObjectEvent(LegacyParser_NavigateToModelObject);
        }

        /// <summary>
        /// This method puts current month and year in xml name space to format it
        /// </summary>
        /// <param name="xmlNamespace"> input XML namespace </param>
        /// <returns> formatted XML namespace</returns>
        public string FormatXmlNamespace(string xmlNamespace)
        {
            if (xmlNamespace.Contains("CurrentYear"))
            {
                xmlNamespace = xmlNamespace.Replace("CurrentYear", DateTime.Now.Year.ToString());
            }

            if (xmlNamespace.Contains("CurrentMonth"))
            {
                string month = DateTime.Now.Month.ToString();
                if (month.Length == 1)
                {
                    month = "0" + month;
                }
                xmlNamespace = xmlNamespace.Replace("CurrentMonth", month);
            }

            return xmlNamespace;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Helper_DisplayCurrentStatus(object sender, DisplayStatusEventArgs e)
        {
            object[] obj = new object[1];
            obj[0] = e.StatusMessage;
            Invoke(new StatusDisplayDelegate(DisplayCurrStatusInFormThread), obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusMessage"></param>
        /// <param name="statusIndicator"></param>
        internal void DisplayStatus(string statusMessage, Framework.StatusTypes statusIndicator)
        {
            MessageBoxIcon boxIcon = MessageBoxIcon.Information;            
            switch (statusIndicator)
            {
                case Infosys.Lif.LegacyWorkbench.Framework.StatusTypes.Error:
                    boxIcon = MessageBoxIcon.Error;
                    break;
                case Infosys.Lif.LegacyWorkbench.Framework.StatusTypes.Warning:
                    boxIcon = MessageBoxIcon.Warning;
                    break;
            }
            MessageBox.Show(statusMessage, "Legacy Workbench", MessageBoxButtons.OK, boxIcon);
        }


        /// <summary>
        /// Display summary in Log tab
        /// </summary>
        /// <param name="summaryText"> summary </param>
        internal void DisplaySummary(string summaryText)
        {
            txtSummary.Text += summaryText + Environment.NewLine;
        }

        delegate void StatusDisplayDelegate(string statusMessageText);

        /// <summary>
        /// Displays status of legacy workbench in the status bar
        /// </summary>
        /// <param name="statusMessageText"> status message details </param>
        internal void DisplayCurrStatusInFormThread(object statusMessageText)
        {
            lblStatusMessage.Text = Convert.ToString(statusMessageText);
        }

        TreeNode ModelObjectsRootNode
        {
            get
            {
                TreeNode node
                    = (treeModelObjects.Nodes.Count > 0) ? treeModelObjects.Nodes[0] : null;
                if (node == null)
                {
                }
                return node;
            }
        }

        TreeNode contractsRootNode
        {
            get
            {
                TreeNode node
                    = (treeContracts.Nodes.Count > 0) ? treeContracts.Nodes[0] : null;
                return node;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void BindModelObjectsTreeToProject()
        {
            // Display Clauses
            if (ModelObjectsRootNode != null)
            {
                ModelObjectsRootNode.Remove();
            }

            treeModelObjects.Nodes.Add(RootNodeText);
            treeModelObjects.Nodes[0].Tag = currProject;

            LoopModelObjects(ModelObjectsRootNode, currProject.ModelObjectModules);
        }

        /// <summary>
        /// 
        /// </summary>
        private void BindContractsTreeToProject()
        {
            {
                // Display Contracts
                if (contractsRootNode != null)
                {
                    contractsRootNode.Remove();
                }

                treeContracts.Nodes.Add(RootNodeText);
                treeContracts.Nodes[0].Tag = currProject;

                LoopContracts(contractsRootNode, currProject.ContractModules);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="treeNodeToAddTo"></param>
        /// <param name="genericCollection"></param>
        private void LoopContracts(TreeNode treeNodeToAddTo,
            Entities.GenericCollection<Entities.ContractModule> genericCollection)
        {
            foreach (Entities.ContractModule contract in genericCollection)
            {

                TreeNode nodeToBeAdded = new TreeNode();
                treeNodeToAddTo.Nodes.Add(nodeToBeAdded);
                nodeToBeAdded.Text = contract.Name;
                nodeToBeAdded.Tag = contract;

                for (int entityLooper = 0;
                    entityLooper < contract.Contracts.Count;
                    entityLooper++)
                {
                    TreeNode entityNode = new TreeNode();

                    entityNode.Text = contract.Contracts[entityLooper].ContractName;//.InputClause.Name;
                    entityNode.Tag = contract.Contracts[entityLooper];

                    if (contract.Contracts[entityLooper].IsToBeGenerated)
                    {
                        entityNode.Checked = true;
                    }

                    //Notes Change
                    ////image list creation
                    //ImageList ContNotesImageList = new ImageList();
                    //ContNotesImageList.Images.Add(Infosys.Lif.LegacyWorkbench.Properties.Resources.Add);
                    //ContNotesImageList.Images.Add(Infosys.Lif.LegacyWorkbench.Properties.Resources.Copy);

                    //// Assign the ImageList to the TreeView.
                    //treeContracts.ImageList = ContNotesImageList;

                    if (contract.Contracts[entityLooper].Notes != null)
                    {
                        entityNode.Text = entityNode.Text + "  * ";
                        breadcumContractsElements.Text = entityNode.Text;
                        //entityNode.ImageIndex = 1;                        
                    }
                    //end change

                    AddNodeAlphabetically(nodeToBeAdded.Nodes, entityNode);
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="treeNodeToAddTo"></param>
        /// <param name="genericCollection"></param>
        public void LoopModelObjects(TreeNode treeNodeToAddTo,
            Entities.GenericCollection<Entities.ModelObjectModule> genericCollection)
        {
            foreach (Entities.ModelObjectModule module in genericCollection)
            {
                TreeNode nodeToBeAdded = new TreeNode();
                treeNodeToAddTo.Nodes.Add(nodeToBeAdded);
                nodeToBeAdded.Text = module.Name;
                nodeToBeAdded.Tag = module;

                for (int entityLooper = 0;
                    entityLooper < module.ModelObjects.Count;
                    entityLooper++)
                {
                    TreeNode entityNode = new TreeNode();

                    entityNode.Text = module.ModelObjects[entityLooper].EntityName;
                    entityNode.Tag = module.ModelObjects[entityLooper];
                    if (module.ModelObjects[entityLooper].IsToBeGenerated)
                    {
                        entityNode.Checked = true;
                    }

                    //notes change
                    if (module.ModelObjects[entityLooper].Notes != null)
                    {
                        entityNode.Text = entityNode.Text + "  * ";
                        breadcumModelObjectElements.Text = entityNode.Text;
                    }
                    //end change

                    AddNodeAlphabetically(nodeToBeAdded.Nodes, entityNode);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exc"></param>
        private void HandleException(Exception exc)
        {
            DisplaySummary("================================================================");
            DisplaySummary(exc.Message);
            DisplaySummary(exc.StackTrace);
            DisplaySummary("================================================================");
            MessageBox.Show(exc.Message, "Legacy Workbench",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (statusDisplay != null)
            {
                statusDisplay.Hide();
            }
        }

        Entities.Project currProject = new Entities.Project();

        Entities.ModelObjectModule ungroupedbtnModelObjectsModule = new Entities.ModelObjectModule();

        Entities.ContractModule ungroupedContractModule = new Entities.ContractModule();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ModelObject"></param>
        /// <returns></returns>
        private bool ModelObjectAlreadyExists(Entities.Entity ModelObject)
        {
            string ModelObjectsEntityName = ModelObject.EntityName.ToString();

            //chk for Program ID also

            string ModelObjectProgramID = ModelObject.ProgramId.ToString();

            if (ModelObjectEntityMapping.ContainsKey(ModelObjectProgramID) || ModelObjectEntityCollection.ContainsKey(ModelObjectsEntityName))
            {
                return true;
            }

            return false;
        }

        bool areContractsBoundToModelObjects = false;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ModelObject"></param>
        public void callFromModelObjectImporter(Entities.Entity ModelObject)
        {
            AddModelObjectToProject(ModelObject);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ModelObject"></param>
        private void AddModelObjectToProject(Entities.Entity ModelObject)
        {
            int numberOfModelObjects = Convert.ToInt32(lblNumberOfModelObjects.Text);
            numberOfModelObjects++;
            lblNumberOfModelObjects.Text = numberOfModelObjects.ToString();
            
            areContractsBoundToModelObjects = false;
            if (currProject.ModelObjectModules.Count == 0)
            {
                currProject.ModelObjectModules.Add(ungroupedbtnModelObjectsModule);
            }
            
            if (nodesAddedFromStart == true)
            {
                ungroupedbtnModelObjectsModule = currProject.ModelObjectModules[0];
            }

            currProject.ModelObjectModules[0].ModelObjects.Add(ModelObject);
            
            //change the name of the key to include modelobjectmodule name also
            //add the new entity to hashtable ModelObjectEntityMapping
            ModelObjectEntityMapping.Add(ModelObject.ProgramId.ToString(), ModelObject);

            //add the new entity to hashtable ModelObjectEntityCollection
            ModelObjectEntityCollection.Add(ModelObject.EntityName.ToString(), ModelObject.ProgramId.ToString());
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        private bool ContractAlreadyExists(Entities.Contract contract)
        {
            foreach (Entities.Contract currentContract in ungroupedContractModule.Contracts)
            {
                bool isContractAlreadDefined = currentContract.ContractName.ToLowerInvariant().Equals(contract.ContractName.ToLowerInvariant());
                if (isContractAlreadDefined)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contract"></param>
        private void AddContractToProject(Entities.Contract contract)
        {
            int numberOfContracts = Convert.ToInt32(lblNumberOfContracts.Text);
            numberOfContracts++;
            lblNumberOfContracts.Text = numberOfContracts.ToString();

            areContractsBoundToModelObjects = false;
            if (currProject.ContractModules.Count == 0)
            {
                currProject.ContractModules.Add(ungroupedContractModule);
            }
            ungroupedContractModule.Contracts.Add(contract);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="editorToBeLoaded"></param>
        /// <param name="panelToBeLoadedIn"></param>
        private void SizeEditorToPanel(Control editorToBeLoaded, Panel panelToBeLoadedIn)
        {
            editorToBeLoaded.Width = panelToBeLoadedIn.Width - 5;
            editorToBeLoaded.Height = panelToBeLoadedIn.Height - 5;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeContracts_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeContracts.SelectedNode = e.Node;
                DisplayMenu(e.Node, treeContracts, e.X, e.Y);
                PopulateMoveToMenu(e);
            }
        }

        /// <summary>
        /// Contract Modified,.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LegacyParser_Modified(object sender, Editors.ContractEditor.ModifiedEventArgs e)
        {
            TreeNode moduleNode = treeContracts.SelectedNode.Parent;
            
            // Get the clause module containing the modified clause
            Entities.ContractModule module = (Entities.ContractModule)moduleNode.Tag;
            int nodeIndex = moduleNode.Nodes.IndexOf(treeContracts.SelectedNode);
            module.Contracts.RemoveAt(nodeIndex);
            module.Contracts.Insert(nodeIndex, e.ModifiedContract);

            treeContracts.SelectedNode.Tag = e.ModifiedContract;

            //notes change
            if (contractEditor.NotesAdded)
            {
                treeContracts.SelectedNode.Text = e.ModifiedContract.ContractName + " * ";
                breadcumContractsElements.Text = e.ModifiedContract.ContractName + " * ";
                /******Dont delete - to add pic in tree nodes */
                //MessageBox.Show(contractEditor.NotesText);
                //PictureBox picNotes = new PictureBox();
                //System.Drawing.Bitmap img = new System.Drawing.Bitmap(@"C:\Icons\copy1.jpg");
                //picNotes.Image = img;
                ////picNotes.Image = (System.Drawing.Image)LegacyWorkbench.Properties.Resources.Add;                
                ////set image size
                //picNotes.Size = new System.Drawing.Size(15, 15);
                ////set location
                //picNotes.Location = new System.Drawing.Point(300,300);
                ////tooltip = notes
                //ToolTip toolTipNotes = new ToolTip();
                //toolTipNotes.SetToolTip(picNotes,contractEditor.NotesText);
            }
            else
            {
                treeContracts.SelectedNode.Text = e.ModifiedContract.ContractName;
                breadcumContractsElements.Text = e.ModifiedContract.ContractName;
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LegacyParser_NavigateToModelObject(object sender, Editors.ContractEditor.NavigateToModelObjectEventArgs e)
        {
            if (treeModelObjects.Nodes.Count > 0)
            {
                foreach (TreeNode treeNode in treeModelObjects.Nodes[0].Nodes)
                {
                    foreach (TreeNode childNode in treeNode.Nodes)
                    {
                        Entities.Entity boundEntity = (Entities.Entity)childNode.Tag;
                        if (boundEntity.ProgramId == e.ModelObjectEntity.ProgramId)
                        {
                            treeModelObjects.SelectedNode = childNode;
                            tabControlMain.SelectedIndex = 0;
                            return;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenerate_Click(object sender, EventArgs e)
        {

            ////to check if user has changed the model object program id after binding it with contract
            ////ModelObjectEntityMapping
            //int contractModuleCounter = 0;
            //int contractCounter = 0;
            //int contractInputCounter = 0;
            //int contractOutputCounter = 0;
            //for (contractModuleCounter = 0; contractModuleCounter < currProject.ContractModules.Count; contractModuleCounter++)
            //{
            //    for (contractCounter = 0; contractCounter < currProject.ContractModules[contractModuleCounter].Contracts.Count; contractCounter++)
            //    {
            //        for (contractInputCounter = 0; contractInputCounter < currProject.ContractModules[contractModuleCounter].Contracts[contractCounter].InputModelObjects.Count; contractInputCounter++)
            //        {
            //            if (!ModelObjectEntityMapping.ContainsKey(currProject.ContractModules[contractModuleCounter].Contracts[contractCounter].InputModelObjects[contractInputCounter].HostName.ToString()))
            //            {
            //                MessageBox.Show("Binded ModelObjects to Contracts has been changed. Please bind the existing ModelObject", "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //                return;
            //            }
            //        }
            //        for (contractOutputCounter = 0; contractOutputCounter < currProject.ContractModules[contractModuleCounter].Contracts[contractCounter].OutputModelObjects.Count; contractOutputCounter++)
            //        {
            //            if (!ModelObjectEntityMapping.ContainsKey(currProject.ContractModules[contractModuleCounter].Contracts[contractCounter].OutputModelObjects[contractInputCounter].HostName.ToString()))
            //            {
            //                MessageBox.Show("Binded ModelObjects to Contracts has been changed. Please bind the existing ModelObject", "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //                return;
            //            }
            //        }
            //    }
            //}
            ////check ends

            //to switch to model object tab if user is curremtly on any tab other than model object or contract
            if (!(tabControlMain.SelectedTab == tabModelObjects || tabControlMain.SelectedTab == tabContracts))
            {
                tabControlMain.SelectedTab = tabModelObjects;
            }

            //to check if any changes is made in name of entity
            //if any arrange it alphabetically
            if (ModelObjectEditor.isEntityChangesSaved == true)
            {
                BindModelObjectsTreeToProject();
                ModelObjectEditor.isEntityChangesSaved = false;
            }
            

            if (currProject.ModelObjectModules.Count == 0 || currProject.ContractModules.Count == 0)
            {
                Framework.Helper.ShowError("Please upload ModelObjects and contracts");
                return;
            }

            // check if XSD object generator exists in the location mentioned in the app.config
            string xsdPath = legacyWorkbenchSettings.XsdObjectGeneratorPath;            
            System.IO.FileInfo xsdFileInfo = new System.IO.FileInfo(xsdPath);

            if (xsdFileInfo.Exists == false)
            {
                MessageBox.Show(
                "The XSD Object Generator tool does not exist in the path mentioned in the config file",
                "Legacy Workbench",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
                return;
            }            
            
            if (!areContractsBoundToModelObjects)
            {
                BindContractToModelObjects();
                areContractsBoundToModelObjects = true;
            }

            Framework.ICodeGenerator codeGenerator = null;

            string CodeGeneratorDll = legacyWorkbenchSettings.CodeGenerator.assembly;
            string CodeGeneratorClass = legacyWorkbenchSettings.CodeGenerator.fullType;
            string xsdObjGenPath = legacyWorkbenchSettings.XsdObjectGeneratorPath;

            // Create Instance of Model Object Importer
            if ((CodeGeneratorClass == null || CodeGeneratorClass.Length == 0)
                &&
                (CodeGeneratorDll == null || CodeGeneratorDll.Length == 0))
            {

            }
            else
            {
                object createdInstance = CreateInstance(CodeGeneratorDll, CodeGeneratorClass);
                if (createdInstance != null)
                {
                    if (createdInstance is Framework.ICodeGenerator)
                    {
                        codeGenerator = (Framework.ICodeGenerator)createdInstance;
                    }
                    else
                    {
                        Helper.ShowError(CodeGeneratorClass + " does not implement ICodeGenerator");
                    }
                }
            }

            statusDisplay = new StatusDisplay();
            Framework.Helper.SetHelperMethods(DisplaySummary, DisplayStatus,
                statusDisplay.statusProgressBar,
                statusDisplay.statusProgressLabel);

            Framework.Helper.InitializeStatusBar(100);
            Framework.Helper.ShowStatusCompletionText("Initializing...");
            codeGenerator.Initialize(currProject, xsdObjGenPath);
                        
            if (codeGenerator.IsErrorOccurred)
            {
                return;
            }

            statusDisplay.Show();
            Framework.Helper.IncreaseStatusCompletion(15);
            Framework.Helper.ShowStatusCompletionText("Generating ModelObject data entities...");

            codeGenerator.GenerateModelObjectDataEntities();
            if (CheckCodeGenerationSuccess(codeGenerator))
            {
                statusDisplay.Hide();
                return;
            }

            Framework.Helper.IncreaseStatusCompletion(25);

            Framework.Helper.ShowStatusCompletionText("Generating contract data entities...");
            codeGenerator.GenerateContractDataEntities();

            if (CheckCodeGenerationSuccess(codeGenerator))
            {
                statusDisplay.Hide();
                return;
            }

            Framework.Helper.IncreaseStatusCompletion(15);
            Framework.Helper.ShowStatusCompletionText("Generating ModelObject serializers...");

            codeGenerator.GenerateModelObjectSerializers();
            if (CheckCodeGenerationSuccess(codeGenerator))
            {
                statusDisplay.Hide();
                return;
            }

            Framework.Helper.IncreaseStatusCompletion(15);
            Framework.Helper.ShowStatusCompletionText("Generating contract serializers...");
            codeGenerator.GenerateContractSerializers();
            if (CheckCodeGenerationSuccess(codeGenerator))
            {
                statusDisplay.Hide();
                return;
            }

            Framework.Helper.IncreaseStatusCompletion(15);
            Framework.Helper.ShowStatusCompletionText("Performing clean up...");
            codeGenerator.CleanUp();
            if (CheckCodeGenerationSuccess(codeGenerator))
            {
                statusDisplay.Hide();
                return;
            }

            Framework.Helper.IncreaseStatusCompletion(15);

            Framework.Helper.ShowStatusCompletionText("Completed...");

            statusDisplay.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codeGenerator"></param>
        /// <returns></returns>
        private bool CheckCodeGenerationSuccess(Framework.ICodeGenerator codeGenerator)
        {
            if (codeGenerator.IsErrorOccurred)
            {
                DisplayStatus(codeGenerator.ErrorReason, Framework.StatusTypes.Warning);
            }
            return codeGenerator.IsErrorOccurred;
        }


        internal static System.Collections.Hashtable ModelObjectEntityMapping = new Hashtable();
        //declaring a hashtable containing EntityName as Key and Program Id as value
        internal static Hashtable ModelObjectEntityCollection = new Hashtable();

        //a new hashtable with key = "Reference ID" and value "entity object"
        internal static Hashtable ModelObjectReferenceIDEntityCollection = new Hashtable();

        /// <summary>
        /// 
        /// </summary>
        private void BindContractToModelObjects()
        {
            //to be cleared to avoid repeatation of entities in drop down list
            ModelObjectEntityMapping.Clear();
            areContractsBoundToModelObjects = true;
            foreach (Entities.ModelObjectModule ModelObjectModule in currProject.ModelObjectModules)
            {
                foreach (Entities.Entity ModelObjectEntity in ModelObjectModule.ModelObjects)
                {                    
                    string key = ModelObjectEntity.ProgramId.ToString();

                    if (!ModelObjectEntityMapping.ContainsKey(key))
                    {
                        ModelObjectEntityMapping.Add(key, ModelObjectEntity);
                    }
                }
            }

            foreach (Entities.ContractModule contractModule in currProject.ContractModules)
            {
                foreach (Entities.Contract contract in contractModule.Contracts)
                {
                    foreach (Entities.ModelObject modelObject in contract.InputModelObjects)
                    {
                        BindModelObjectToEntity(modelObject);
                    }
                    if (contract.OutputModelObjects != null)
                    {
                        foreach (Entities.ModelObject modelObject in contract.OutputModelObjects)
                        {
                            BindModelObjectToEntity(modelObject);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modelObject"></param>
        private void BindModelObjectToEntity(Entities.ModelObject modelObject)
        {
            {
                foreach (Entities.ModelObject modelObjectChild in modelObject.ModelObjects)
                {
                    modelObjectIdFileMapping = new Hashtable();
                    ///<TBD> Currently we are looking at only the first 4 characters of the 
                    /// name to do the mapping between the entity and model object
                    /// </TBD>
                    string modelObjectChildEntityReference = modelObjectChild.Name.Substring(0, 4).ToString();
                    if (ModelObjectEntityMapping.ContainsKey(modelObjectChildEntityReference))
                    {
                        modelObjectChild.ModelObjectEntity
                            = (Entities.Entity)ModelObjectEntityMapping[modelObjectChildEntityReference];
                    }
                        
                    else if (modelObjectIdFileMapping.ContainsKey(modelObjectChildEntityReference))
                    {
                        string modelObjectFileName = (string)modelObjectIdFileMapping[modelObjectChildEntityReference];

                        Entities.Entity entityRetrieved = modelObjectImporter.RetrieveModelObjectDetails(modelObjectFileName);

                        if (!ModelObjectAlreadyExists(entityRetrieved))
                        {
                            AddModelObjectToProject(entityRetrieved);
                        }

                        DisplaySummary("Loaded model for " + modelObjectChildEntityReference + " from " + modelObjectFileName);
                    }

                    else
                    {
                        DisplaySummary(modelObjectChild.Name + " model object not uploaded.");
                    }

                    BindModelObjectToEntity(modelObjectChild);
                }


            }

            ///<TBD> Currently we are looking at only the first 4 characters of the 
            /// name to do the mapping between the entity and model object
            /// </TBD>
            string modelObjectEntityReference = modelObject.Name.Substring(0, 4).ToString();

            if (ModelObjectEntityMapping.ContainsKey(modelObjectEntityReference))
            {
                modelObject.ModelObjectEntity = (Entities.Entity)ModelObjectEntityMapping[modelObjectEntityReference];
            }

            else
            {
                DisplaySummary(modelObject.Name + " model object not uploaded.");
            }
        }

        //countModuleNumber contains the count Program Id    
        int countModuleNumber = 0;
        //countEntityNumber gives the counter for Entity
        int countEntityNumber = 0;
        int countModuleNodeNumber = 0;
        int outerLoopIndex = 0;
        int innerLoopIndex = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteModuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedIndex == TabbtnModelObjectsIndex)
            {
                Entities.ModelObjectModule ModelObjectModuleToBeDeleted = ((Entities.ModelObjectModule)rightClickedNode.Tag);               
                
                if (rightClickedNode.Parent.Tag == null)
                {
                    rightClickedNode.Parent.Tag = new Infosys.Lif.LegacyWorkbench.Entities.Project();
                    ((Entities.Project)rightClickedNode.Parent.Tag).ModelObjectModules = currProject.ModelObjectModules;
                }
                
                //remove using remove method
                currProject.ModelObjectModules.RemoveAt(treeModelObjects.SelectedNode.Index);

                //remove the related entities from the hashtablea modelobjectentitymapping and modelobjectentitycollection
                for (int loop = 0; loop < ModelObjectModuleToBeDeleted.ModelObjects.Count; loop++)
                {
                    ModelObjectEntityMapping.Remove(ModelObjectModuleToBeDeleted.ModelObjects[loop].ProgramId.ToString());
                    ModelObjectEntityCollection.Remove(ModelObjectModuleToBeDeleted.ModelObjects[loop].EntityName.ToString());
                    ////update the hashtable ModelObjectReferenceIDEntityCollection
                    //ModelObjectReferenceIDEntityCollection.Remove(ModelObjectModuleToBeDeleted.ModelObjects[loop].ReferenceID.ToString());
                }

                rightClickedNode.Remove();
            }
            else
            {
                Entities.ContractModule contractModuleToBeDeleted = ((Entities.ContractModule)rightClickedNode.Tag);
                ((Entities.Project)rightClickedNode.Parent.Tag).ModelObjectModules.Remove(contractModuleToBeDeleted);                
                rightClickedNode.Remove();
            }
            
            ModelObjectEntityMapping.Clear();
            lblNumberOfModelObjects.Text= ModelObjectEntityMapping.Count.ToString();
            countModuleNumber = 0;            
        }           
        
        //this function is called to return the count of the entities
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public int counterForToBeNewlyAddedNode(object sender, EventArgs e)
        {
            //to generate an unique Entity name and Program Id
            int maximumCountAlreadyPresent = 0;
            string[] strArray;
            //for (int iLoop = 0; iLoop < currProject.ModelObjectModules[0].ModelObjects.Count; iLoop++)
            for(int iLoop=0;iLoop<treeModelObjects.SelectedNode.Nodes.Count;iLoop++)
            {
                //chk if selected node is root
                if (treeModelObjects.SelectedNode.Text == "Root")
                {
                    maximumCountAlreadyPresent = treeModelObjects.SelectedNode.Nodes.Count;
                    break;
                }
                if (currProject.ModelObjectModules[0].ModelObjects[iLoop].EntityName.StartsWith("NewNode"))
                {
                    try
                    {
                        int chk = Convert.ToInt32(currProject.ModelObjectModules[0].ModelObjects[iLoop].EntityName.Substring(7));
                        if (chk > maximumCountAlreadyPresent)
                        {
                            maximumCountAlreadyPresent = chk;
                        }
                    }
                    catch
                    {
                    }
                }

                if (currProject.ModelObjectModules[0].ModelObjects[iLoop].ProgramId.StartsWith("NewNode"))
                {
                    try
                    {
                        int chk = Convert.ToInt32(currProject.ModelObjectModules[0].ModelObjects[iLoop].ProgramId.Substring(7));
                        if (chk > maximumCountAlreadyPresent)
                        {
                            maximumCountAlreadyPresent = chk;
                        }
                    }
                    catch
                    {
                    }
                }
            }

            int iLoop1 = 0;
            if (treeModelObjects.SelectedNode.Text == "Root")
            {
                iLoop1 = maximumCountAlreadyPresent;
            }
            else
            {
                for (iLoop1 = 0; iLoop1 <= maximumCountAlreadyPresent; iLoop1++)
                {
                    string valueExists = "No";
                    for (int iLoopInner = 0; iLoopInner < treeModelObjects.SelectedNode.Nodes.Count; iLoopInner++)
                    {
                        if (currProject.ModelObjectModules[0].ModelObjects[iLoopInner].EntityName == "NewNode" + iLoop1 || currProject.ModelObjectModules[0].ModelObjects[iLoopInner].ProgramId == "NewNode" + iLoop1)
                        {
                            valueExists = "Yes";
                            break;
                        }
                    }
                    if (valueExists == "No")
                    {
                        break;
                    }
                }
            }

            return iLoop1;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int counterForToBeNewlyAddedNode1()
        {
            int loopOuter = 0;
            for (loopOuter = 0; loopOuter < ModelObjectEntityMapping.Count; loopOuter++)
            {
                if (!(ModelObjectEntityMapping.ContainsKey("NewNode" + loopOuter) || ModelObjectEntityMapping.ContainsKey("NewNode"+loopOuter)))
                {                    
                    break;
                }
            }
            return loopOuter;
        }

        //this function checks for the modelobjectmodule node in which entity is to be added
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public int RetrieveNodeCount(object sender, EventArgs e)
        {
            int nodeCounter = 0;
            for (int iLoop = 0; iLoop < treeModelObjects.Nodes[0].Nodes.Count; iLoop++)
            {
                if (treeModelObjects.Nodes[0].Nodes[iLoop].IsSelected == true)
                {
                    nodeCounter = iLoop;
                    break;
                }
            }
            //it returns the selected node number in which new entity is to be added.

            return nodeCounter;
        }

        //this function is called for the number of the node in which new entity is added
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public int modelObjectModuleSelectedNodeForNewlyAddedNode(object sender, TreeViewEventArgs e)
        {
            int nodeCounter = 0;
            for (int iLoop = 0; iLoop < treeModelObjects.Nodes[0].Nodes.Count; iLoop++)
            {
                if (treeModelObjects.Nodes[0].Nodes[iLoop].Nodes.Count > 0)
                {
                    if (treeModelObjects.Nodes[0].Nodes[iLoop].LastNode.IsSelected == true)
                    {
                        nodeCounter = iLoop;
                        break;
                    }
                }
            }
            //it returns the selected node number in which new entity is added.

            return nodeCounter;
        }

        static public string lastSelectedNode;

        /// <summary>
        /// 
        /// </summary>
        public string LastSelectedNode
        {
            get { return lastSelectedNode; }
            set { lastSelectedNode = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void treeModelObjects_AfterSelect(object sender, TreeViewEventArgs e)
        {
            lastSelectedNode = previousSelectedNode;
            breadcumModelObjectRootImage.Visible = true;
            breadcumModelObjectModuleImage.Visible = true;
            breadcumModelObjectRoot.Visible = true;
            breadcumModoleobjectModules.Visible = true;
            breadcumModelObjectElements.Visible = true;
            {
                breadcumModelObjectRoot.Text = "";
                breadcumModoleobjectModules.Text = "";
                breadcumModelObjectElements.Text = "";
                breadcumModelObjectModuleImage.Visible = false;
                //if Root Node and the Module are not null.
                if (e.Node.Parent != null && e.Node.Parent.Parent != null)
                {
                    breadcumModelObjectRoot.Tag = e.Node.Parent.Parent;
                    breadcumModoleobjectModules.Tag = e.Node.Parent;
                    breadcumModelObjectElements.Tag = e.Node;
                    breadcumModelObjectModuleImage.Visible = true;
                    breadcumModelObjectRoot.Text = e.Node.Parent.Parent.Text;
                    breadcumModoleobjectModules.Text = e.Node.Parent.Text;
                    breadcumModelObjectElements.Text = e.Node.Text;
                }
                //if module node is not null.
                else if (e.Node.Parent != null)
                {
                    breadcumModelObjectRoot.Text = e.Node.Parent.Text;
                    breadcumModoleobjectModules.Text = e.Node.Text;
                }
                // Module node element is not equal to null.        
                else if (e.Node != null)
                {
                    breadcumModelObjectRoot.Text = e.Node.Text;
                    breadcumModelObjectRootImage.Visible = false;
                }
            }
            //countModuleNumber = counterForToBeNewlyAddedNode1(sender, e);
            //this if condition checks if the node is added by using right click option

            if (e.Node.Text == "NewNode" + (countEntityNumber) && currProject.ModelObjectModules[treeModelObjects.SelectedNode.Parent.Index].ModelObjects[treeModelObjects.SelectedNode.Parent.Nodes.Count - 1].ProgramId.ToString() == "NewNode" + countModuleNumber)
            {
                int nodeCounter1 = 0;
                nodeCounter = modelObjectModuleSelectedNodeForNewlyAddedNode(sender, e);

                int modelCount = 0;
                
                for (int i = 0; i < e.Node.Parent.Nodes.Count; i++)
                {
                    if (currProject.ModelObjectModules[nodeCounter].ModelObjects[i].EntityName.ToLowerInvariant() == e.Node.Text.ToLowerInvariant())
                    {
                        modelCount = i;
                        break;
                    }
                }
                currProject.ModelObjectModules[nodeCounter].ModelObjects[modelCount].ProgramId = "NewNode" + (countModuleNumber);

                //newEntity.ProgramId = "NewNode" + modelCount;
                currProject.ModelObjectModules[nodeCounter].ModelObjects[modelCount].EntityName = "NewNode" + (countEntityNumber);

                //newEntity.EntityName = "NewNode" + modelCount;
                currProject.ModelObjectModules[nodeCounter].ModelObjects[modelCount].DataEntityClassName = "Newnode" + (countEntityNumber);

                //newEntity.DataEntityClassName = "Newnode" + modelCount;
                currProject.ModelObjectModules[nodeCounter].ModelObjects[modelCount].SerializerClassName = "newnode" + (countEntityNumber);

                //newEntity.SerializerClassName = "newnode" + modelCount;
                e.Node.Tag = currProject.ModelObjectModules[nodeCounter].ModelObjects[modelCount];
                                
                if (treeModelObjects.SelectedNode.Parent.Checked == true)
                {
                    treeModelObjects.SelectedNode.Checked = true;
                }
            }            

            //this assigns the value to the e.node.tag
            //in case the nodes are added from start.
            //the value assigned depends upon the level of the node.
            if (nodesAddedFromStart == true)
            {
                if (treeModelObjects.SelectedNode.Level == 0)
                {
                    e.Node.Tag = new Infosys.Lif.LegacyWorkbench.Entities.Project();                    
                }

                if (treeModelObjects.SelectedNode.Level == 1)
                {
                    e.Node.Tag = new Infosys.Lif.LegacyWorkbench.Entities.ModelObjectModule();                
                }
            }

            if (e.Node.Tag != null)
            {
                Control editorToBeLoaded = null;

                if (e.Node.Tag is Entities.Entity)
                {
                    editorToBeLoaded = modelObjectEditor;
                    
                    //if (!areContractsBoundToModelObjects)
                    {
                        BindContractToModelObjects();                        
                    }

                    modelObjectEditor.Populate((Entities.Entity)e.Node.Tag);
                }

                if (e.Node.Tag is Entities.ModelObjectModule)
                {
                    // Clause Module Editor to be loaded
                    editorToBeLoaded = moduleEditor;
                    Entities.Module moduleToBeEdited = (Entities.Module)e.Node.Tag;
                    if (currProject.ProjectPrefix == null)
                    {
                        currProject.ProjectPrefix = "";
                    }
                    moduleToBeEdited.DataEntityNamespace = currProject.ProjectPrefix + modelObjDataEntityNS;
                    moduleToBeEdited.SerializerNamespace = currProject.ProjectPrefix + modelObjSerializerNS;

                    if (nodesAddedFromStart == true)
                    {
                        moduleToBeEdited.DataEntityNamespace = currProject.ProjectPrefix + legacyWorkbenchSettings.CodeGeneratedNamespaces.ModelObjectNamespaces.DataEntityNamespace;
                        moduleToBeEdited.SerializerNamespace = currProject.ProjectPrefix + legacyWorkbenchSettings.CodeGeneratedNamespaces.ModelObjectNamespaces.SerializerNamespace;
                        moduleToBeEdited.Name = treeModelObjects.SelectedNode.Text;                        
                    }

                    moduleEditor.Populate(moduleToBeEdited);
                }

                if (e.Node.Tag is Entities.Project)
                {
                    editorToBeLoaded = modelObjectProjectEditor;
                    Entities.Project projectToBeEdited = (Entities.Project)e.Node.Tag;
                    modelObjectProjectEditor.PopulateModelObjects(currProject,isProjectEditorDirty);
                }

                //notes change
                Entities.Entity modelObj = new Entities.Entity();
                if (modelObj.GetType() == treeModelObjects.SelectedNode.Tag.GetType())
                {
                    Entities.Entity modelObj1 = (Entities.Entity)e.Node.Tag;
                    e.Node.ToolTipText = modelObj1.Notes;
                }
                
                calledClearMethod = true;
                
                modelObjectEditPanel.Controls.Clear();                
                
                SizeEditorToPanel(editorToBeLoaded, modelObjectEditPanel);

                modelObjectEditPanel.Controls.Add(editorToBeLoaded);
            }
            
            lblNumberOfModelObjects.Text = ModelObjectEntityMapping.Count.ToString();
        }
        
        Editors.ModelObjectEditor modelObjectEditor;
        Editors.ProjectEditor contractProjectEditor;
        Editors.ProjectEditor modelObjectProjectEditor;
        Editors.ModuleEditor moduleEditor;
        Editors.ContractEditor contractEditor;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void moduleDetailsModified(object sender, Editors.ModuleEditor.ModifiedEventArgs e)
        {
            TreeView selectedTree = null;

            switch (tabControlMain.SelectedIndex)
            {
                case TabContractsIndex:
                    selectedTree = treeContracts;
                    //reflect changes in breadcumcontractobject
                    breadcumContractModel.Text = e.ModifiedModule.Name.ToString();
                    break;

                case TabbtnModelObjectsIndex:
                    selectedTree = treeModelObjects;
                    //reflect changes in breadcummodelobject
                    breadcumModoleobjectModules.Text = e.ModifiedModule.Name.ToString();
                    break;
            }

            ((Entities.Module)selectedTree.SelectedNode.Tag).Name = e.ModifiedModule.Name.ToString();
            ((Entities.Module)selectedTree.SelectedNode.Tag).DataEntityNamespace = e.ModifiedModule.DataEntityNamespace;
            ((Entities.Module)selectedTree.SelectedNode.Tag).SerializerNamespace = e.ModifiedModule.SerializerNamespace;

            selectedTree.SelectedNode.Text = e.ModifiedModule.Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LegacyParser_Modified(object sender, Editors.ModelObjectEditor.ModifiedEventArgs e)
        {
            //AddModelObjectToProject
            if (treeModelObjects.SelectedNode.Level == 2)
            {
                currProject.ModelObjectModules[treeModelObjects.SelectedNode.Parent.Index].ModelObjects.Remove(modelObjectEditor.removedItem);
                currProject.ModelObjectModules[treeModelObjects.SelectedNode.Parent.Index].ModelObjects.Add(modelObjectEditor.originalUnModifiedEntity);
            }

            TreeNode moduleNode = treeModelObjects.SelectedNode.Parent;

            // Get the clause module containing the modified clause
            Entities.ModelObjectModule module = (Entities.ModelObjectModule)moduleNode.Tag;

            Entities.Entity entityToBeRemoved = new Entities.Entity();
            foreach (Entities.Entity ModelObject in module.ModelObjects)
            {
                if (ModelObject.ProgramId == e.ModifiedEntity.ProgramId)
                {
                    entityToBeRemoved = ModelObject;
                    break;
                }
            }

            module.ModelObjects.Remove(entityToBeRemoved);
            module.ModelObjects.Add(e.ModifiedEntity);            
            treeModelObjects.SelectedNode.Tag = e.ModifiedEntity;
            
            //Update Contract Mapping model object
            entityToBeRemoved =(Entities.Entity) ModelObjectEntityMapping[e.ModifiedEntity.ProgramId.ToString()];
            
            //notes change
            if (modelObjectEditor.NotesAdded)
            {
                treeModelObjects.SelectedNode.Text = e.ModifiedEntity.EntityName + " * ";
                breadcumModelObjectElements.Text = e.ModifiedEntity.EntityName + " *";
            }
            else
            {
                treeModelObjects.SelectedNode.Text = e.ModifiedEntity.EntityName;
                breadcumModelObjectElements.Text = e.ModifiedEntity.EntityName;
            }
            //change end
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            //to switch to model object tab if user is curremtly on any tab other than model object or contract
            if (!(tabControlMain.SelectedTab == tabModelObjects || tabControlMain.SelectedTab == tabContracts))
            {
                tabControlMain.SelectedTab = tabModelObjects;
            }

            //to check if any change is made in name of entity
            //if any arrange it alphabetically
            if (ModelObjectEditor.isEntityChangesSaved == true)
            {
                BindModelObjectsTreeToProject();
                ModelObjectEditor.isEntityChangesSaved = false;
            }

            if (currProject.ContractModules.Count == 0 && currProject.ModelObjectModules.Count == 0)
            {
                return;
            }

            FileStorage storageImplementation = new FileStorage();

            if (!storageImplementation.Save(currProject))
            {
                DisplayStatus("Unable to save the current Project. Please check the summary tab", Framework.StatusTypes.Error);
            }
        }

        /// <summary>
        /// Close the application
        /// </summary>
        /// <param name="sender"> sender object </param>
        /// <param name="e"> event args </param>
        private void mnuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoad_Click(object sender, EventArgs e)
        {
            //to switch to model object tab if user is curremtly on any tab other than model object or contract
            if (!(tabControlMain.SelectedTab == tabModelObjects || tabControlMain.SelectedTab == tabContracts))
            {
                tabControlMain.SelectedTab = tabModelObjects;
            }

            Entities.Project retrievedProject = null;
            //check if already any XML has been uploaded
            if (currProject != null)
            {
                if ((currProject.ContractModules.Count != 0) || (currProject.ModelObjectModules.Count != 0))
                {
                    DialogResult confirmationResult = MessageBox.Show(
                        "You have already opened a workbench instance. Do you want to open a new one?",
                        "Legacy Workbench", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (confirmationResult == DialogResult.Yes)
                    {
                        //refresh workbench n open diaglog to upload new XML
                        ClearParser();

                        FileStorage storageImplementation = new FileStorage();
                        retrievedProject = storageImplementation.Load();
                    }
                }

                //XML uploaded for first time
                else
                {
                    FileStorage storageImplementation = new FileStorage();
                    retrievedProject = storageImplementation.Load();
                }
            }

            //XML uploaded after clearing workbench
            else
            {
                FileStorage storageImplementation = new FileStorage();
                retrievedProject = storageImplementation.Load();
            }

            //upload XML if retrieved proj is not null
            if (retrievedProject != null)            
            {
                currProject = retrievedProject;
                //populate the hashtables ModelObjectEntityMapping and ModelObjectEntityCollection
                for (int loop = 0; loop < currProject.ModelObjectModules.Count; loop++)
                {
                    for(int loopInner=0;loopInner< currProject.ModelObjectModules[loop].ModelObjects.Count;loopInner++)
                    {
                        try
                        {
                            ModelObjectEntityMapping.Add(currProject.ModelObjectModules[loop].ModelObjects[loopInner].ProgramId.ToString(), currProject.ModelObjectModules[loop].ModelObjects[loopInner]);
                            ModelObjectEntityCollection.Add(currProject.ModelObjectModules[loop].ModelObjects[loopInner].EntityName.ToString(), currProject.ModelObjectModules[loop].ModelObjects[loopInner].ProgramId.ToString());
                        }
                        catch
                        {
                            MessageBox.Show("the repeated model object node" + currProject.ModelObjectModules[loop].ModelObjects[loopInner].EntityName.ToString() + "has been discarded",
                                "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }

                areContractsBoundToModelObjects = false;

                if (currProject.ModelObjectModules != null && currProject.ModelObjectModules.Count > 0)
                {
                    BindModelObjectsTreeToProject();
                }

                else
                {
                    MessageBox.Show("The uploaded XML does not contain any model object", "Legacy Workbench",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (currProject.ContractModules != null && currProject.ContractModules.Count > 0)
                {
                    BindContractsTreeToProject();
                }

                else
                {
                    MessageBox.Show("The uploaded XML does not contain any model object", "Legacy Workbench",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                int numberOfContracts = 0;
                foreach (Entities.ContractModule module in currProject.ContractModules)
                {
                    numberOfContracts += module.Contracts.Count;
                }

                lblNumberOfContracts.Text = numberOfContracts.ToString();

                int numberOfModelObjects = 0;
                foreach (Entities.ModelObjectModule module in currProject.ModelObjectModules)
                {
                    numberOfModelObjects += module.ModelObjects.Count;
                }

                lblNumberOfModelObjects.Text = numberOfModelObjects.ToString();
                
                //code changed to allow uploading of XML files which contains only model objects or contracts
                if (currProject.ModelObjectModules != null && currProject.ModelObjectModules.Count > 0)
                {
                    ungroupedbtnModelObjectsModule = currProject.ModelObjectModules[0];
                }

                if (currProject.ContractModules != null && currProject.ContractModules.Count > 0)
                {
                    ungroupedContractModule = currProject.ContractModules[0];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClear_Click(object sender, EventArgs e)
        {            
            if (currProject.ContractModules.Count != 0 || currProject.ModelObjectModules.Count != 0 || repVwrDisplayReport.Visible == true)
            {
                DialogResult confirmationResult = MessageBox.Show(
                        "You have made some changes in current workbench instance, launching a new workbench will loose it. Do you want to continue?",
                        "Legacy Workbench", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                if (confirmationResult == DialogResult.Yes)
                {
                    //refresh workbench
                    ClearParser();
                }                
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ClearParser()
        {
            //reset editors
            modelObjectEditor = new ModelObjectEditor();
            modelObjectEditor.Modified += new ModelObjectEditor.OnModified(LegacyParser_Modified);

            moduleEditor = new ModuleEditor();
            moduleEditor.Modified += new ModuleEditor.OnModified(moduleDetailsModified);

            contractProjectEditor = new ProjectEditor();
            contractProjectEditor.Modified += new ProjectEditor.OnModified(ContractsProjEditor_Modified);

            modelObjectProjectEditor = new ProjectEditor();
            modelObjectProjectEditor.Modified += new ProjectEditor.OnModified(ModelObjectsProjEditor_Modified);

            contractEditor = new ContractEditor();
            contractEditor.Modified += new ContractEditor.OnModified(LegacyParser_Modified);
            contractEditor.NavigateToModelObject
                += new Editors.ContractEditor.NavigateToModelObjectEvent(LegacyParser_NavigateToModelObject);

            //clear model obj mapping
            ModelObjectEntityMapping.Clear();

            //clear the value of ModelObjectEntityCollection
            ModelObjectEntityCollection.Clear();

            //Clear the editor space
            contractEditPanel.Controls.Clear();
            modelObjectEditPanel.Controls.Clear();

            //clear tree view
            treeContracts.Nodes.Clear();
            treeModelObjects.Nodes.Clear();

            //clear entities
            currProject = null;
            ungroupedbtnModelObjectsModule = null;
            ungroupedContractModule = null;
            currProject = new Entities.Project();
            ungroupedbtnModelObjectsModule = new Entities.ModelObjectModule();
            ungroupedContractModule = new Entities.ContractModule();
            ungroupedbtnModelObjectsModule.Name = UngroupedNodeText;
            ungroupedContractModule.Name = UngroupedNodeText;

            //clear breadcrumbs : contract
            breadcumContractModel.Visible = false;
            breadcumContractModelImage.Visible = false;
            breadcumContractRootImage.Visible = false;
            breadcumContractsElements.Visible = false;
            breadcumContractRoot.Visible = false;

            //clear breadcrumbs : model object
            breadcumModelObjectElements.Visible = false;
            breadcumModelObjectModuleImage.Visible = false;
            breadcumModelObjectRoot.Visible = false;
            breadcumModelObjectRootImage.Visible = false;
            breadcumModoleobjectModules.Visible = false;

            //clear Status label
            lblStatusMessage.Text = String.Empty;
            lblNumberOfContracts.Text = "0";
            lblNumberOfModelObjects.Text = "0";

            //clear project editor's static values
            ProjectEditor.duplicatePrefixValue = string.Empty;
            ProjectEditor.projectPrefix = string.Empty;
            ProjectEditor.valuePrefix = string.Empty;                        

            //clear Log summary
            txtSummary.Text = String.Empty;

            //clear Report tab
            repVwrDisplayReport.Visible = false;
            comboBoxReportType.SelectedIndex = 0;
            
            //clear Test Data tab
            //to be implemented when Test Data functionality is implemented

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treebtnModelObjects_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeModelObjects.SelectedNode = e.Node;                
                rightClickedNode = e.Node;
                DisplayMenu(e.Node, treeModelObjects, e.X, e.Y);
                PopulateMoveToMenu(e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void PopulateMoveToMenu(TreeNodeMouseClickEventArgs e)
        {
            moveToMenuItem.DropDownItems.Clear();
            if (e.Node.Level == 2)
            {
                TreeView treeView = e.Node.TreeView;
                for (int i = 0; i < treeView.Nodes[0].Nodes.Count; i++)
                {
                    TreeNode currNode = treeView.Nodes[0].Nodes[i];
                    if (currNode != e.Node.Parent)
                    {
                        moveToMenuItem.DropDownItems.Add(currNode.Text);
                    }
                }

                if (moveToMenuItem.DropDownItems.Count == 0)
                {
                    moveToMenuItem.Visible = false;
                }
                else
                {
                    moveToMenuItem.Visible = true;
                }
            }
        }
               
        TreeNode rightClickedNode = null;  
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="clickedNode"></param>
        /// <param name="clickedTreeView"></param>
        /// <param name="locationLeft"></param>
        /// <param name="locationTop"></param>
        private void DisplayMenu(TreeNode clickedNode, TreeView clickedTreeView, int locationLeft, int locationTop)
        {
            rightClickedNode = clickedNode;
            switch (clickedNode.Level)
            {
                case 0:
                    rootNodeMenuStrip.Show(clickedTreeView, locationLeft, locationTop);
                    break;

                case 1:
                    moduleMenuStrip.Show(clickedTreeView, locationLeft, locationTop);
                    break;

                case 2:
                    entityMenuStrip.Show(clickedTreeView, locationLeft, locationTop);
                    break;
            }
        }             

        /// <summary>
        ///this function is used to display the contextmenustrip1
        //this function contains only three parameters as compared to displayMenu()
        //the parameter clickedTreeView is deleted as the click is not on a node but
        //on an empty panel.
        /// </summary>
        /// <param name="clickedTreeView"></param>
        /// <param name="locationLeft"></param>
        /// <param name="locationTop"></param>
        private void DisplayMenuRootNode(TreeView clickedTreeView, int locationLeft, int locationTop)
        {
            if (treeModelObjects.Nodes.Count == 0)
            {
                contextMenuStrip1.Show(treeModelObjects, locationLeft, locationTop);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addModuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode newTreeNode;
            switch (tabControlMain.SelectedIndex)
            {
                case TabbtnModelObjectsIndex:
                    newTreeNode = AddModuleToProject(CreateNewModelObjectModule());
                    treeModelObjects.Nodes[0].Nodes.Add(newTreeNode);                    
                    treeModelObjects.SelectedNode = newTreeNode;

                    currProject.ModelObjectModules.Add(newTreeNode.Tag);
                    
                    //this loop checks if the parent checkbox is checked or not
                    //to ensure that the newly added node's checkbox is also
                    //checked or unchecked depending on the parent checkbox's status.

                    if (treeModelObjects.SelectedNode.Parent.Checked == true)
                    {
                        treeModelObjects.SelectedNode.Checked = true;
                    }

                    break;

                case TabContractsIndex:
                    newTreeNode = AddModuleToProject(CreateNewContractModule());
                    treeContracts.Nodes[0].Nodes.Add(newTreeNode);
                    treeContracts.SelectedNode = newTreeNode;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleToBeAdded"></param>
        /// <returns></returns>
        private TreeNode AddModuleToProject(Entities.Module moduleToBeAdded)
        {
            TreeNode newTreeNode = new TreeNode();
            if (moduleToBeAdded.GetType() == typeof(Entities.ModelObjectModule))
            {                
                int countModelObjectModules = 0;
                int outerLoopIndex = 0;
                countModelObjectModules = treeModelObjects.Nodes[0].Nodes.Count;
                bool isNodeAlreadyExisting = false;

                for (outerLoopIndex = 0; outerLoopIndex < countModelObjectModules; outerLoopIndex++)
                {
                    isNodeAlreadyExisting = false;
                    for (int innerLoopIndex = 0; innerLoopIndex < countModelObjectModules; innerLoopIndex++)
                    {
                        if (treeModelObjects.Nodes[0].Nodes[innerLoopIndex].Text == "Untitled" + outerLoopIndex)
                        {
                            isNodeAlreadyExisting = true;
                            break;
                        }
                    }
                    if (isNodeAlreadyExisting == false)
                    {
                        break;
                    }
                }
                newTreeNode.Text = "Untitled" + outerLoopIndex; 
            }

            else
            {
                currProject.ContractModules.Add((Entities.ContractModule)moduleToBeAdded);
                newTreeNode.Text = "Untitled" + (currProject.ContractModules.Count - 1).ToString();
            }

            moduleToBeAdded.Name = newTreeNode.Text;
            newTreeNode.Tag = moduleToBeAdded;
            return newTreeNode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Entities.ModelObjectModule CreateNewModelObjectModule()
        {
            Entities.ModelObjectModule ModelObjectModule = new Entities.ModelObjectModule();
            return ModelObjectModule;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Entities.ContractModule CreateNewContractModule()
        {
            Entities.ContractModule contractModule = new Entities.ContractModule();
            return contractModule;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateEntityToolStripMentItem_Click(object sender, EventArgs e)
        {            
            //Entity and Program Id are unique value at Project level
            //countModuleNumber gives the counter for Program ID
            countModuleNumber = counterForToBeNewlyAddedNode1();

            //get the count for Entity
            countEntityNumber = counterForEntity();
            
            Entities.Entity currEntity = new Entities.Entity();
            rightClickedNode.Nodes.Add("NewNode" + (countEntityNumber));
            currEntity.DataEntityClassName = "Newnode" + (countEntityNumber);
            currEntity.EntityName = "NewNode" + (countEntityNumber);
            currEntity.ProgramId = "NewNode" + (countModuleNumber);
            currEntity.SerializerClassName = "Newnode" + (countEntityNumber);
            currEntity.DataItems.Add(new DataItem());
            currEntity.DataItems[0].ItemType = DataItem.DataItemType.GroupItem;
            currEntity.IsToBeGenerated = true;

            currEntity.DataItems[0].EnumName = "";
            currEntity.DataItems[0].IsVisible = true;
            currEntity.DataItems[0].ItemName = "Dfhcommarea";
            currEntity.DataItems[0].Value = null;

            //currEntity.DataItems[0].GroupItems[0]
            currEntity.DataItems[0].GroupItems.Add(new DataItem());
            currEntity.DataItems[0].GroupItems[0].ItemType = DataItem.DataItemType.GroupItem;
            currEntity.DataItems[0].GroupItems[0].EnumName = "";
            currEntity.DataItems[0].GroupItems[0].IsVisible = true;
            currEntity.DataItems[0].GroupItems[0].ItemName = "Ws_Item1";
            currEntity.DataItems[0].GroupItems[0].Value = null;
            //End of currEntity.DataItems[0].GroupItems[0]

            //currEntity.DataItems[0].GroupItems[1]
            currEntity.DataItems[0].GroupItems.Add(new DataItem());
            currEntity.DataItems[0].GroupItems[1].ItemType = DataItem.DataItemType.GroupItem;
            currEntity.DataItems[0].GroupItems[1].EnumName = "";
            currEntity.DataItems[0].GroupItems[1].IsVisible = true;
            currEntity.DataItems[0].GroupItems[1].ItemName = "Ws_Item2";
            currEntity.DataItems[0].GroupItems[1].Value = null;
            //End of currEntity.DataItems[0].GroupItems[1]

            //currEntity.DataItems[0].GroupItems[2]
            currEntity.DataItems[0].GroupItems.Add(new DataItem());
            currEntity.DataItems[0].GroupItems[2].ItemType = DataItem.DataItemType.GroupItem;
            currEntity.DataItems[0].GroupItems[2].EnumName = "";
            currEntity.DataItems[0].GroupItems[2].IsVisible = true;
            currEntity.DataItems[0].GroupItems[2].ItemName = "Ws_Item3";
            currEntity.DataItems[0].GroupItems[2].Value = null;
            //end of currEntity.DataItems[0].GroupItems[2]            

            //Adding default entity to currProject
            nodeCounter = 0;
            nodeCounter = RetrieveNodeCount(sender, e);
            currProject.ModelObjectModules[nodeCounter].ModelObjects.Add(currEntity);  

            //now add the new entity to ModeobjectentityMapping also
            ModelObjectEntityMapping.Add("NewNode" + countModuleNumber, currEntity);

            //adding the entity name and program Id to modelobjectentitycollection
            ModelObjectEntityCollection.Add("NewNode" + countEntityNumber, "NewNode" + countModuleNumber);
            treeModelObjects.SelectedNode = treeModelObjects.SelectedNode.LastNode;            
                        
            //code added to arrange the newly added entity in an alphabetical order
            BindModelObjectsTreeToProject();            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private int counterForEntity()
        {
            int returnParameter = 0;
            for (returnParameter = 0; returnParameter < ModelObjectEntityCollection.Count; returnParameter++)
            {
                if (!ModelObjectEntityCollection.ContainsKey("NewNode"+returnParameter))
                {
                    break;
                }
            }
            return returnParameter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void moveToMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            TreeNode newModuleNode = FindModule(e.ClickedItem.Text);
            int indexOfParentNode = treeContracts.SelectedNode.Parent.Index;
            int indexTargetNode = 0;
            for (int loop = 0; loop < treeContracts.Nodes[0].Nodes.Count; loop++)
            {
                if (treeContracts.Nodes[0].Nodes[loop].Text == e.ClickedItem.Text)
                {
                    indexTargetNode = loop;
                }
            }
            switch (tabControlMain.SelectedIndex)
            {
                case TabbtnModelObjectsIndex:
                    TreeNode selectedNode = rightClickedNode;
                    Entities.Entity ModelObjectToBeMoved = (Entities.Entity)selectedNode.Tag;
                    Entities.ModelObjectModule module = (Entities.ModelObjectModule)newModuleNode.Tag;

                    //logic for moving to new folder
                    bool loopBreak = false;

                    for (int loop = 0; loop < newModuleNode.Nodes.Count; loop++)
                    {                        
                        if (newModuleNode.Nodes[loop].Text == selectedNode.Text)
                        {
                            loopBreak = true;
                            MessageBox.Show("This node already exists in " + e.ClickedItem.Text, "Legacy Workbench",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                        }
                    }

                    if (loopBreak == false)
                    {
                        ((Entities.ModelObjectModule)selectedNode.Parent.Tag).ModelObjects.Remove(ModelObjectToBeMoved);
                        selectedNode.Parent.Nodes.Remove(selectedNode);
                        //remove it from the currProject
                        //location of current module
                        //int currentNodeCount = RetrieveNodeCount(sender, e);
                        currProject.ModelObjectModules[indexOfParentNode].ModelObjects.Remove(ModelObjectToBeMoved);
                        //add the new node to the new location
                        currProject.ModelObjectModules[indexTargetNode].ModelObjects.Add(ModelObjectToBeMoved);
                        newModuleNode.Nodes.Add(selectedNode);
                        //module.ModelObjects.Add(ModelObjectToBeMoved);                        
                    }

                    break;

                case TabContractsIndex:
                    TreeNode selectedContractNode = rightClickedNode;
                    Entities.Contract contractToBeMoved = (Entities.Contract)selectedContractNode.Tag;
                    Entities.ContractModule contractModule = (Entities.ContractModule)newModuleNode.Tag;
                    ((Entities.ContractModule)selectedContractNode.Parent.Tag).Contracts.Remove(contractToBeMoved);
                    selectedContractNode.Parent.Nodes.Remove(selectedContractNode);
                    newModuleNode.Nodes.Add(selectedContractNode);
                    contractModule.Contracts.Add(contractToBeMoved);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        private TreeNode FindModule(string moduleName)
        {
            TreeView treeViewToBeSearched = null;

            switch (tabControlMain.SelectedIndex)
            {
                case TabbtnModelObjectsIndex:
                    treeViewToBeSearched = treeModelObjects;
                    break;

                case TabContractsIndex:
                    treeViewToBeSearched = treeContracts;
                    break;
            }

            for (int i = 0; i < treeViewToBeSearched.Nodes[0].Nodes.Count; i++)
            {
                TreeNode currNode = treeViewToBeSearched.Nodes[0].Nodes[i];
                if (currNode.Text == moduleName)
                {
                    return currNode;
                }
            }
            return null;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeContracts_AfterSelect(object sender, TreeViewEventArgs e)
        {           

            breadcumContractRoot.Visible = true;
            breadcumContractModel.Visible = true;
            breadcumContractsElements.Visible = true;
            breadcumContractRootImage.Visible = true;
            breadcumContractModelImage.Visible = true;

            {
                breadcumContractRoot.Text = "";
                breadcumContractModel.Text = "";
                breadcumContractsElements.Text = "";
                breadcumContractModelImage.Visible = false;

                //if Root node And Module node Not equal to null.
                if (e.Node.Parent != null && e.Node.Parent.Parent != null)
                {
                    breadcumContractRoot.Tag = e.Node.Parent.Parent;
                    breadcumContractModel.Tag = e.Node.Parent;
                    breadcumContractsElements.Tag = e.Node.Parent;
                    breadcumContractModelImage.Visible = true;
                    breadcumContractRoot.Text = e.Node.Parent.Parent.Text;
                    breadcumContractModel.Text = e.Node.Parent.Text;
                    breadcumContractsElements.Text = e.Node.Text;
                }

                //Else Module is not equal to null.
                else if (e.Node.Parent != null)
                {
                    breadcumContractRoot.Text = e.Node.Parent.Text;
                    breadcumContractModel.Text = e.Node.Text;
                }

                //else Module element not equal to null.
                else if (e.Node != null)
                {
                    breadcumContractRoot.Text = e.Node.Text;
                    breadcumContractRootImage.Visible = false;
                }
            }

            if (e.Node.Tag != null)
            {
                Control editorToBeLoaded = new Control();

                //for contract node
                if (e.Node.Tag is Entities.Contract)
                {
                    editorToBeLoaded = contractEditor;
                    BindContractToModelObjects();                    
                    contractEditor.Populate((Entities.Contract)e.Node.Tag);
                }

                //for contract module node
                if (e.Node.Tag is Entities.ContractModule)
                {
                    editorToBeLoaded = moduleEditor;
                    Entities.Module moduleToBeEdited = (Entities.Module)e.Node.Tag;

                    if (currProject.ProjectPrefix == null)
                    {
                        currProject.ProjectPrefix = "";
                    }

                    moduleToBeEdited.DataEntityNamespace = currProject.ProjectPrefix + contractDataEntityNS;
                    moduleToBeEdited.SerializerNamespace = currProject.ProjectPrefix + ContractSerializerNS;
                    moduleEditor.Populate(moduleToBeEdited);
                }

                //for root node
                if (e.Node.Tag is Entities.Project)
                {
                    editorToBeLoaded = contractProjectEditor;
                    Entities.Project projectToBeEdited = (Entities.Project)e.Node.Tag;
                    contractProjectEditor.PopulateContracts(projectToBeEdited);
                }

                //notes change
                Entities.Contract cont = new Entities.Contract();
                if (cont.GetType() == treeContracts.SelectedNode.Tag.GetType())
                {
                    Entities.Contract cont1 = (Entities.Contract)e.Node.Tag;
                    e.Node.ToolTipText = cont1.Notes;
                }
                //change end   
                
                calledClearMethod = true;               

                contractEditPanel.Controls.Clear();

                SizeEditorToPanel(editorToBeLoaded, contractEditPanel);

                contractEditPanel.Controls.Add(editorToBeLoaded);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ContractsProjEditor_Modified(object sender, Infosys.Lif.LegacyWorkbench.Editors.ProjectEditor.ModifiedEventArgs e)
        {
            currProject.ContractNamespaces = e.ModifiedNameSpaces;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ModelObjectsProjEditor_Modified(object sender, Infosys.Lif.LegacyWorkbench.Editors.ProjectEditor.ModifiedEventArgs e)
        {
            currProject.ModelObjectNamespaces = e.ModifiedNameSpaces;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treebtnModelObjects_AfterCheck(object sender, TreeViewEventArgs e)
        {
            // **
            if (e.Node.Level <= 1)
            {
                CheckAllLowerNodes(e.Node, e.Node.Checked);
            }

            // **
            else if (e.Node.Level == 2)
            {
                if (e.Node.Checked == false)
                {
                    treeModelObjects.AfterCheck -= treebtnModelObjects_AfterCheck;
                    e.Node.Parent.Checked = false;
                    e.Node.Parent.Parent.Checked = false;
                    treeModelObjects.AfterCheck += new TreeViewEventHandler(treebtnModelObjects_AfterCheck);
                }

                try
                {
                    // **
                    ((Entities.Entity)e.Node.Tag).IsToBeGenerated = e.Node.Checked;
                }

                catch
                {
                    // to be called in case checkbox is checked before selecting the node
                    treeModelObjects_AfterSelect(sender, e);
                }
            }
        }

        /// <summary>
        /// This is a recursive method which checks/unchecks the check box of all the lower level nodes 
        /// when a parent node is checked/unchecked
        /// </summary>
        /// <param name="treeNode"> treen node which is checked </param>
        /// <param name="isChecked"> True- if checked, False - if unchecked </param>
        private void CheckAllLowerNodes(TreeNode treeNode, bool isChecked)
        {
            for (int i = 0; i < treeNode.Nodes.Count; i++)
            {
                treeNode.Nodes[i].Checked = isChecked;
                
                if (isChecked)
                {
                    //linkLabel2.Text = Application["textboxvalue"];
                }

                //call method recursively to check/uncheck the child nodes of current tree node
                CheckAllLowerNodes(treeNode.Nodes[i], isChecked);
            }
        }

        /// <summary>
        /// This event occurs when node of a contract tree is checked
        /// </summary>
        /// <param name="sender"> sender object </param>
        /// <param name="e"> tree event args </param>
        private void treeContracts_AfterCheck(object sender, TreeViewEventArgs e)
        {
            // **
            if (e.Node.Level <= 1)
            {
                CheckAllLowerNodes(e.Node, e.Node.Checked);
            }

            // **
            else if (e.Node.Level == 2)
            {
                if (e.Node.Checked == false)
                {
                    treeContracts.AfterCheck -= treeContracts_AfterCheck;
                    e.Node.Parent.Checked = false;
                    e.Node.Parent.Parent.Checked = false;
                    treeContracts.AfterCheck += new TreeViewEventHandler(treeContracts_AfterCheck);
                }

                try
                {
                    // **
                    ((Entities.Contract)e.Node.Tag).IsToBeGenerated = e.Node.Checked;
                }

                catch
                {
                    //to be called in case chech box is checked before selecting the node
                    treeContracts_AfterSelect(sender, e);
                }
            }
        }
        
        /// <summary>
        /// The method adds a new node in the tree view in alphabetic order
        /// </summary>
        /// <param name="parentNodeCollection"> All nodes which are already in the tree view </param>
        /// <param name="nodeToBeAdded"> the new node to be added in tree view </param>
        private void AddNodeAlphabetically(TreeNodeCollection parentNodeCollection, TreeNode nodeToBeAdded)
        {
            string nodeText = nodeToBeAdded.Text;
            int nodeCounter = 0;

            for (nodeCounter = 0; nodeCounter < parentNodeCollection.Count; nodeCounter++)
            {
                if (parentNodeCollection[nodeCounter].Text.CompareTo(nodeText) > 0)
                { break; }
            }

            parentNodeCollection.Insert(nodeCounter, nodeToBeAdded);
        }

        /// <summary>
        /// The event occurs when the Legacy parser is loaded 
        /// </summary>
        /// <param name="sender"> sender object </param>
        /// <param name="e"> event args </param>
        private void LegacyParser_Load(object sender, EventArgs e)
        {
            //Binding the combo box with the various types of reports.
            int count = 0;
            while (count < legacyWorkbenchSettings.ReportingConfigurations.Reports.Count)
            {
                comboBoxReportType.Items.Add(legacyWorkbenchSettings.ReportingConfigurations.Reports.ReportCollection[count].Alias.ToString());
                count++;
            }            

            //set all breadcrumbs' visibility as false initially
            breadcumModelObjectRoot.Visible = false;
            breadcumModoleobjectModules.Visible = false;
            breadcumModelObjectElements.Visible = false;
            breadcumContractRoot.Visible = false;
            breadcumContractModel.Visible = false;
            breadcumContractsElements.Visible = false;
            breadcumModelObjectRootImage.Visible = false;
            breadcumModelObjectModuleImage.Visible = false;
            breadcumContractRootImage.Visible = false;
            breadcumContractModelImage.Visible = false;
            
            {
                //retrieve model obj importer details from config file
                string ModelObjectImporterDll = legacyWorkbenchSettings.ModelObjectImporter.assembly;

                string ModelObjectImporterClass = legacyWorkbenchSettings.ModelObjectImporter.fullType;
                                
                // If no details found for model object importer in the config file, remove model object tab
                if ((ModelObjectImporterClass == null || ModelObjectImporterClass.Length == 0)
                    &&
                    (ModelObjectImporterDll == null || ModelObjectImporterDll.Length == 0))
                {
                    tabControlMain.TabPages.Remove(tabModelObjects);
                }

                else
                {
                    // Create Instance of Model Object Importer
                    object createdInstance = CreateInstance(ModelObjectImporterDll, ModelObjectImporterClass);

                    if (createdInstance != null)
                    {
                        if (createdInstance is UserControl)
                        {
                            // Place the created instance of model Object Importer in the panel
                            pnlModelObjectHolder.Controls.Add((UserControl)createdInstance);
                        }

                        if (createdInstance is Framework.IModelObjectImporter)
                        {
                            // Keep a reference of the model Object Importer, to enable easy retrieval.
                            modelObjectImporter = (Framework.IModelObjectImporter)createdInstance;
                            modelObjectImporter.ModelObjectsRetrieved += new ModelObjectsRetrievedEventHandler(modelObjectImporter_ModelObjectsRetrieved);
                        }

                        else
                        {
                            Helper.ShowError(ModelObjectImporterClass + " does not implement IModelObjectImporter");
                        }
                    }
                }
            }

            {
                //retrieve model obj importer details from config file
                string ContractsImporterDll = legacyWorkbenchSettings.ContractsImporter.assembly;
                string ContractsImporterClass = legacyWorkbenchSettings.ContractsImporter.fullType;

                // If no details found for contract importer in the config file, remove contract tab
                if ((ContractsImporterClass == null || ContractsImporterClass.Length == 0)
                    &&
                    (ContractsImporterDll == null || ContractsImporterDll.Length == 0))
                {
                    tabControlMain.TabPages.Remove(tabModelObjects);
                }

                else
                {
                    // Create Instance of contract Importer
                    object createdInstance = CreateInstance(ContractsImporterDll, ContractsImporterClass);

                    if (createdInstance != null)
                    {
                        if (createdInstance is UserControl)
                        {
                            // Place the created instance of contract Importer in the panel
                            panModelObjectContracts.Controls.Add((UserControl)createdInstance);                           
                        }

                        if (createdInstance is Framework.IContractImporter)
                        {
                            // Keep a reference of the contract Importer, to enable easy retrieval.

                            contractsImporter = (Framework.IContractImporter)createdInstance;
                            contractsImporter.ContractsRetrieved += new ContractsRetrievedEventHandler(contractsImporter_ContractsRetrieved);
                        }

                        else
                        {
                            Helper.ShowError(ContractsImporterClass + " does not implement IContractImporter");
                        }
                    }
                }
            }
            this.repVwrDisplayReport.RefreshReport();            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void modelObjectImporter_ModelObjectsRetrieved(object sender, Framework.ModelObjectsRetrievedEventArgs e)
        {
            TreeNode currentSelectedNode = treeModelObjects.SelectedNode;
            bool errorsOccured = false;

            Entities.GenericCollection<Entities.Entity> modelObjectsRetrieved = e.RetrievedModelObjects;
            
            foreach (Entities.Entity modelObject in modelObjectsRetrieved)
            {
                if (!ModelObjectAlreadyExists(modelObject))
                {
                    try
                    {
                        AddModelObjectToProject(modelObject);
                    }

                    catch (Exception exc)
                    {
                        HandleException(exc);
                    }
                }

                else
                {
                    MessageBox.Show("The entity " + modelObject.EntityName + " already exists.", "Legacy Workbench",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    DisplaySummary("The entity " + modelObject.EntityName + " already exists.");                   
                }
            }

            if (!errorsOccured)
            {
                BindModelObjectsTreeToProject();
            }

            treeModelObjects.SelectedNode = currentSelectedNode;
        }

        /// <summary>
        /// creates instance of the class using the full class name and dll path retrieved from config file using reflection
        /// </summary>
        /// <param name="objectImporterDll"> dll path </param>
        /// <param name="objectImporterClass"> full qualified class name</param>
        /// <returns> created object of class </returns>
        private object CreateInstance(string objectImporterDll, string objectImporterClass)
        {
            System.Runtime.Remoting.ObjectHandle objHandle = Activator.CreateInstance(objectImporterDll, objectImporterClass);
            object createdInstance = objHandle.Unwrap();
            return createdInstance;
        }

        /// <summary>
        /// This event occurs when the user moves from one tab to the another
        /// </summary>
        /// <param name="sender"> sender object </param>
        /// <param name="e"> event args </param>
        private void tabControlMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab.Text == "Contracts")
            {
                //refill the combobox
                contractEditor.autoFillDropDownList();
               
                moduleMenuStrip.Items[0].Visible = false;
                moduleMenuStrip.Items[1].Visible = false;
                rootNodeMenuStrip.Items[0].Visible = true;
            }

            if (tabControlMain.SelectedTab.Text == "Model Objects")
            {
                moduleMenuStrip.Items[0].Visible = true;
                moduleMenuStrip.Items[1].Visible = true;
                rootNodeMenuStrip.Items[0].Visible = true;
            }
            
            //to prevent user to access Test Data Tab
            if (tabControlMain.SelectedTab.Text == "Test Data")
            {
                MessageBox.Show("This functionality is under construction", "Legacy Workbench",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //Display Report
            if (tabControlMain.SelectedTab.Text == "Report")
            {                
                lblReportType.Visible = true;
                btnBrowseForReport.Visible = true;
                comboBoxReportType.Visible = true;
                comboBoxReportType.Text = "Select Report Type";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void contractsImporter_ContractsRetrieved(object sender, Framework.ContractsRetrievedEventArgs e)
        {
            bool errorsOccured = false;
            foreach (Entities.Contract contract in e.RetrievedContracts)
            {
                if (!ContractAlreadyExists(contract))
                {
                    try
                    {
                        AddContractToProject(contract);
                    }

                    catch (Exception exc)
                    {
                        HandleException(exc);
                    }
                }

                else
                {
                    DisplayStatus("The contract " + contract.ContractName + " already exists.", Framework.StatusTypes.Warning);
                }
            }

            if (!errorsOccured)
            {
                BindContractsTreeToProject();
            }
        }

        /// <summary>
        /// the event occurs when the user clicks on Options -> Configurations menu
        /// </summary>
        /// <param name="sender"> sender object </param>
        /// <param name="e"> event args </param>
        private void configurationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigurationWindow configEditor = new ConfigurationWindow();
            if (configEditor.ShowDialog() == DialogResult.OK)
            {
                //functionality is yet to b implemented
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void breadcumModelObjectModules_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel label = (LinkLabel)sender;
            treeModelObjects.SelectedNode = (TreeNode)label.Tag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void breadcumModelObjectRoot_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel label = (LinkLabel)sender;
            treeModelObjects.SelectedNode = (TreeNode)label.Tag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void breadcumContractModule_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel label = (LinkLabel)sender;
            treeContracts.SelectedNode = (TreeNode)label.Tag;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void breadcumContractRoot_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel label = (LinkLabel)sender;
            treeContracts.SelectedNode = (TreeNode)label.Tag;
        }
       
        /// <summary>
        /// this function calls the function DisplayMenuRootNode() when contextmenustrip1 item is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeModelObjects_MouseClick(object sender, MouseEventArgs e)
        {            
            if (e.Button == MouseButtons.Right)
            {
                DisplayMenuRootNode(treeModelObjects, e.X, e.Y);
            }
        }

        static public string previousSelectedNode;        
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void callBetween(object sender, TreeViewEventArgs e)
        {
            treeModelObjects_AfterSelect(sender, e);
        }

        bool nodesAddedFromStart;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addRootNodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeModelObjects.Nodes.Add(RootNodeText);
            nodesAddedFromStart = true;
        }
        
        /// <summary>
        /// this function is called when the mouse click is released
        /// </summary>
        /// <param name="sender"> sender object </param>
        /// <param name="e"> event args </param>
        private void treeModelObjects_MouseUp(object sender, MouseEventArgs e)
        {
            //condition to check is the release of the mouse is through
            //left click or right click
            if (e.Button == MouseButtons.Right)
            {
                //if nodes are already existing then call the function DisplayMenu()
                if (treeModelObjects.SelectedNode != null)
                {
                    contextMenuStrip1.Visible = false;
                    DisplayMenu(treeModelObjects.SelectedNode, treeModelObjects, e.X, e.Y);
                    return;
                }

                //if no node exist then call the function DisplayMenurootNode()
                if (!(treeModelObjects.Nodes.Count > 0))
                {
                    contextMenuStrip1.Visible = true;
                    DisplayMenuRootNode(treeModelObjects, e.X, e.Y);
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteEntityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TreeView treeView;
            if (tabControlMain.SelectedIndex == TabbtnModelObjectsIndex)
            {
                treeView = treeModelObjects;
                Entities.Entity modelObjectToBeDeleted = ((Entities.Entity)rightClickedNode.Tag);

                //delete the value from ModelObjectEntityCollection
                ModelObjectEntityCollection.Remove(modelObjectToBeDeleted.EntityName);

                if (ModelObjectEntityMapping.Contains(modelObjectToBeDeleted.ProgramId.ToString()))
                {
                    ModelObjectEntityMapping.Remove(modelObjectToBeDeleted.ProgramId.ToString());
                }

                currProject.ModelObjectModules[treeModelObjects.SelectedNode.Parent.Index].ModelObjects.Remove(modelObjectToBeDeleted);
                ((Entities.ModelObjectModule)rightClickedNode.Parent.Tag).ModelObjects.Remove(modelObjectToBeDeleted);
                rightClickedNode.Remove();
                
                lblNumberOfModelObjects.Text = (Convert.ToInt32(lblNumberOfModelObjects.Text) - 1).ToString();
                lblNumberOfModelObjects.Text = ModelObjectEntityMapping.Count.ToString();
            }

            else
            {
                treeView = treeContracts;
                Entities.Contract contractToBeDeleted = ((Entities.Contract)rightClickedNode.Tag);
                ((Entities.ContractModule)rightClickedNode.Parent.Tag).Contracts.Remove(contractToBeDeleted);
                rightClickedNode.Remove();

                lblNumberOfContracts.Text = (Convert.ToInt32(lblNumberOfContracts.Text) - 1).ToString();
            }
        }

        public string lastSelectedTab;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControlMain_Deselected(object sender, TabControlEventArgs e)
        {
            //to check if any cahnegs is made inthe name of entity
            //if changes exist then arrange it alphabetically
            if (ModelObjectEditor.isEntityChangesSaved == true)
            {
                BindModelObjectsTreeToProject();
                ModelObjectEditor.isEntityChangesSaved = false;
            }

            lastSelectedTab = tabControlMain.SelectedTab.Text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabControlMain_Selecting(object sender, TabControlCancelEventArgs e)
        {
            int nodeCount;
            if (tabControlMain.SelectedTab.Text == "Test Data")
            {
                MessageBox.Show("This site is under construction", "Legacy Workbench", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                if (lastSelectedTab == "Model Objects")
                {
                    nodeCount = 0;
                }

                else if (lastSelectedTab == "Contracts")
                {
                    nodeCount = 1;
                }

                else if (lastSelectedTab == "Log")
                {
                    nodeCount = 2;
                }

                else if (lastSelectedTab == "Report")
                {
                    nodeCount = 4;
                }

                else
                {
                    nodeCount = 3;
                }

                tabControlMain.SelectTab(nodeCount);
            }
        }        
          
        /// <summary>
        /// function to get the value of the treenode when called from the ModelObjectEditor, 
        /// when user tries to edit the ModelObjectEditor
        /// </summary>
        /// <param name="sender"> object sender </param>
        /// <param name="e"> event args </param>
        /// <returns></returns>
        public TreeView retreiveTreeModelObjectValue(object sender, EventArgs e)
        {
            //return the current value of the treeModelObject
            return treeModelObjects;
        }

        /// <summary>
        /// This event occurs when browse button in the report tab is clicked.
        /// </summary>
        /// <param name="sender"> object sender </param>
        /// <param name="e"> event args </param>
        private void btnBrowseForReport_Click(object sender, EventArgs e)
        {            
            fileDlgBrowseForReport.Reset();
            fileDlgBrowseForReport.Title = "Browse For Report File Dialog";
            fileDlgBrowseForReport.Filter = "XML Files|*.xml|All Files|*.*";
            fileDlgBrowseForReport.DefaultExt = "xml";
            fileDlgBrowseForReport.FileName = "";
            bool isXml = true;
            
            string filePath = "";
            if (fileDlgBrowseForReport.ShowDialog() == DialogResult.OK)
            {
                //code to prevent uploading of files other than .xml
                if (!fileDlgBrowseForReport.FileName.ToLowerInvariant().EndsWith(".xml"))
                {
                    MessageBox.Show("Upload only Xml files with .xml extension","Legacy Workbench",
                        MessageBoxButtons.OK,MessageBoxIcon.Error);
                    isXml = false;
                    return;
                }

                if (isXml == true)
                {
                    filePath = fileDlgBrowseForReport.FileName;

                    DataSet dsReport = new DataSet("LegacyWorkbenchReport");                    

                    //Storing the contents of the xml into the dataset.
                    try
                    {
                        dsReport.ReadXml(filePath);
                    }
                    catch
                    {
                        MessageBox.Show("This is an invalid xml.", "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    Hashtable htExtensionMapping = new Hashtable();
                    ArrayList conversionFactorAndExtensions = new ArrayList();
                    int count = 0;

                    while (count < legacyWorkbenchSettings.ReportingConfigurations.Reports.ReportCollection.Count)
                    {
                        //Identifying the type of report.
                        if (comboBoxReportType.Text == legacyWorkbenchSettings.ReportingConfigurations.Reports.ReportCollection[count].Alias.ToString())
                        {
                            //repVwrDisplayReport.Enabled = true;
                            repVwrDisplayReport.Visible = true;

                            decimal effortSavedUsingLIF = 0;

                            repVwrDisplayReport.Refresh();

                            //Necessary to reset the report viewer control and bind it with a new data source.
                            repVwrDisplayReport.Reset();
                            Microsoft.Reporting.WinForms.ReportDataSource fileDataSource = new Microsoft.Reporting.WinForms.ReportDataSource();
                            fileDataSource.Name = "Infosys_Lif_LegacyWorkbench_ReportManager_File";
                            fileDataSource.Value = this.FileBindingSource;
                            this.repVwrDisplayReport.LocalReport.DataSources.Add(fileDataSource);

                            repVwrDisplayReport.LocalReport.ReportEmbeddedResource = legacyWorkbenchSettings.ReportingConfigurations.Reports.ReportCollection[count].FileLocation;
                            effortSavedUsingLIF = Convert.ToDecimal(legacyWorkbenchSettings.ReportingConfigurations.EffortAnalysis.EffortSavedUsingLIF);
                            this.FileBindingSource.DataSource = dsReport;
                            this.FileBindingSource.DataMember = "File";

                            if (comboBoxReportType.Text == "Effort Saving Report-Capers Jones")
                            {
                                ReportParameter effortSavingType = new ReportParameter("EffortSavingType", "CapersJones");
                                repVwrDisplayReport.LocalReport.SetParameters(new ReportParameter[] { effortSavingType });
                                ReportParameter rpEffortSavedUsingLIF = new ReportParameter("EffortSavedUsingLIF", effortSavedUsingLIF.ToString());
                                repVwrDisplayReport.LocalReport.SetParameters(new ReportParameter[] { rpEffortSavedUsingLIF });
                            }
                            if (comboBoxReportType.Text == "Effort Saving Report-Custom")
                            {
                                ReportParameter effortSavingType = new ReportParameter("EffortSavingType", "Custom");
                                repVwrDisplayReport.LocalReport.SetParameters(new ReportParameter[] { effortSavingType });
                                ReportParameter rpEffortSavedUsingLIF = new ReportParameter("EffortSavedUsingLIF", effortSavedUsingLIF.ToString());
                                repVwrDisplayReport.LocalReport.SetParameters(new ReportParameter[] { rpEffortSavedUsingLIF });
                            }
                            
                            repVwrDisplayReport.RefreshReport();
                            break;
                        }
                        count++;
                    }
                }

                lblReportType.Visible = true;
                comboBoxReportType.Visible = true;
                btnBrowseForReport.Visible = true;
            }
        }        

        /// <summary>
        /// This event occurs when Code Generation Report menu button is clicked
        /// </summary>
        /// <param name="sender"> sender object </param>
        /// <param name="e"> event args </param>
        private void generationReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectTab(tabReport);

            bool isXml = true;

            fileDlgBrowseForReport.Reset();
            fileDlgBrowseForReport.Title = "Browse For Report File Dialog";
            fileDlgBrowseForReport.Filter = "XML Files|*.xml|All Files|*.*";
            fileDlgBrowseForReport.DefaultExt = "xml";
            
            string filePath = "";

            if (fileDlgBrowseForReport.ShowDialog() == DialogResult.OK)
            {
                //code to prevent uploading of files other than .xml
                if (!fileDlgBrowseForReport.FileName.ToLowerInvariant().EndsWith(".xml"))
                {
                    MessageBox.Show("Upload only Xml files with .xml extension", "Legacy Workbench",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    isXml = false;
                }

                if (isXml == true)
                {
                    filePath = fileDlgBrowseForReport.FileName;

                    DataSet dsReport = new DataSet("LegacyWorkbenchReport");

                    try
                    {
                        dsReport.ReadXml(filePath);
                    }
                    catch
                    {
                        MessageBox.Show("This is an invalid xml.", "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    foreach (DictionaryEntry de in htReportsForMenu)
                    {
                        if (generationReportToolStripMenuItem.Text == de.Key.ToString())
                        {
                            repVwrDisplayReport.Refresh();
                            repVwrDisplayReport.Reset();
                            Microsoft.Reporting.WinForms.ReportDataSource fileDataSource = new Microsoft.Reporting.WinForms.ReportDataSource();
                            fileDataSource.Name = "Infosys_Lif_LegacyWorkbench_ReportManager_File";
                            fileDataSource.Value = this.FileBindingSource;
                            this.repVwrDisplayReport.LocalReport.DataSources.Add(fileDataSource);

                            repVwrDisplayReport.LocalReport.ReportEmbeddedResource = de.Value.ToString();

                            this.FileBindingSource.DataSource = dsReport;
                            this.FileBindingSource.DataMember = "File";
                            break;
                        }
                    }

                    this.repVwrDisplayReport.RefreshReport();
                    repVwrDisplayReport.Visible = true;
                }
            }
        }

        /// <summary>
        /// This event occurs when Caper Jones Effort Estimation Report menu button is clicked
        /// </summary>
        /// <param name="sender"> sender object </param>
        /// <param name="e"> event args </param>
        private void capersJonesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectTab(tabReport);

            bool isXml = true;

            decimal effortSavedUsingLIF = 0;
            effortSavedUsingLIF = Convert.ToDecimal(legacyWorkbenchSettings.ReportingConfigurations.EffortAnalysis.EffortSavedUsingLIF);

            fileDlgBrowseForReport.Reset();
            fileDlgBrowseForReport.Title = "Browse For Report File Dialog";
            fileDlgBrowseForReport.Filter = "XML Files|*.xml|All Files|*.*";
            fileDlgBrowseForReport.DefaultExt = "xml";
            
            string filePath = "";
            if (fileDlgBrowseForReport.ShowDialog() == DialogResult.OK)
            {
                //code to prevent uploading of files other than .xml
                if (!fileDlgBrowseForReport.FileName.ToLowerInvariant().EndsWith(".xml"))
                {
                    MessageBox.Show("Upload only Xml files with .xml extension", "Legacy Workbench",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    isXml = false;
                }

                if (isXml == true)
                {
                    filePath = fileDlgBrowseForReport.FileName;

                    DataSet dsReport = new DataSet("LegacyWorkbenchReport");

                    try
                    {
                        dsReport.ReadXml(filePath);
                    }
                    catch
                    {
                        MessageBox.Show("This is an invalid xml.", "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    foreach (DictionaryEntry de in htReportsForMenu)
                    {
                        if (de.Key.ToString().Contains(capersJonesToolStripMenuItem.Text))
                        {
                            repVwrDisplayReport.Refresh();
                            repVwrDisplayReport.Reset();
                            Microsoft.Reporting.WinForms.ReportDataSource fileDataSource = new Microsoft.Reporting.WinForms.ReportDataSource();
                            fileDataSource.Name = "Infosys_Lif_LegacyWorkbench_ReportManager_File";
                            fileDataSource.Value = this.FileBindingSource;
                            this.repVwrDisplayReport.LocalReport.DataSources.Add(fileDataSource);

                            repVwrDisplayReport.LocalReport.ReportEmbeddedResource = de.Value.ToString();

                            this.FileBindingSource.DataSource = dsReport;
                            this.FileBindingSource.DataMember = "File";

                            ReportParameter effortSavingType = new ReportParameter("EffortSavingType", "CapersJones");
                            repVwrDisplayReport.LocalReport.SetParameters(new ReportParameter[] { effortSavingType });
                            ReportParameter rpEffortSavedUsingLIF = new ReportParameter("EffortSavedUsingLIF", effortSavedUsingLIF.ToString());
                            repVwrDisplayReport.LocalReport.SetParameters(new ReportParameter[] { rpEffortSavedUsingLIF });

                            break;
                        }
                    }

                    this.repVwrDisplayReport.RefreshReport();
                    repVwrDisplayReport.Visible = true;
                }
            }
        }

        /// <summary>
        /// This event occurs when Custom Effort Estimation Report menu button is clicked
        /// </summary>
        /// <param name="sender"> sender object </param>
        /// <param name="e"> event args </param>
        private void customToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectTab(tabReport);

            bool isXml = true;

            decimal effortSavedUsingLIF = 0;
            effortSavedUsingLIF = Convert.ToDecimal(legacyWorkbenchSettings.ReportingConfigurations.EffortAnalysis.EffortSavedUsingLIF);

            fileDlgBrowseForReport.Reset();
            fileDlgBrowseForReport.Title = "Browse For Report File Dialog";
            fileDlgBrowseForReport.Filter = "XML Files|*.xml|All Files|*.*";
            fileDlgBrowseForReport.DefaultExt = "xml";
           
            string filePath = "";
            if (fileDlgBrowseForReport.ShowDialog() == DialogResult.OK)
            {
                //code to prevent uploading of files other than .xml
                if (!fileDlgBrowseForReport.FileName.ToLowerInvariant().EndsWith(".xml"))
                {
                    MessageBox.Show("Upload only Xml files with .xml extension", "Legacy Workbench",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    isXml = false;
                }

                if (isXml == true)
                {
                    filePath = fileDlgBrowseForReport.FileName;

                    DataSet dsReport = new DataSet("LegacyWorkbenchReport");

                    try
                    {
                        dsReport.ReadXml(filePath);
                    }
                    catch
                    {
                        MessageBox.Show("This is an invalid xml.", "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    foreach (DictionaryEntry de in htReportsForMenu)
                    {
                        if (de.Key.ToString().Contains(customToolStripMenuItem.Text))
                        {
                            repVwrDisplayReport.Refresh();
                            repVwrDisplayReport.Reset();
                            Microsoft.Reporting.WinForms.ReportDataSource fileDataSource = new Microsoft.Reporting.WinForms.ReportDataSource();
                            fileDataSource.Name = "Infosys_Lif_LegacyWorkbench_ReportManager_File";
                            fileDataSource.Value = this.FileBindingSource;
                            this.repVwrDisplayReport.LocalReport.DataSources.Add(fileDataSource);

                            repVwrDisplayReport.LocalReport.ReportEmbeddedResource = de.Value.ToString();

                            this.FileBindingSource.DataSource = dsReport;
                            this.FileBindingSource.DataMember = "File";

                            ReportParameter effortSavingType = new ReportParameter("EffortSavingType", "Custom");
                            repVwrDisplayReport.LocalReport.SetParameters(new ReportParameter[] { effortSavingType });
                            ReportParameter rpEffortSavedUsingLIF = new ReportParameter("EffortSavedUsingLIF", effortSavedUsingLIF.ToString());
                            repVwrDisplayReport.LocalReport.SetParameters(new ReportParameter[] { rpEffortSavedUsingLIF });
                            break;
                        }
                    }
                    
                    this.repVwrDisplayReport.RefreshReport();
                    repVwrDisplayReport.Visible = true;
                }
            }
        }

        /// <summary>
        /// This event occurs when Quality Efficiency Report menu button is clicked
        /// </summary>
        /// <param name="sender"> sender object </param>
        /// <param name="e"> event args </param>
        private void qualityEfficiencyReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControlMain.SelectTab(tabReport);

            bool isXml = true;

            fileDlgBrowseForReport.Reset();
            fileDlgBrowseForReport.Title = "Browse For Report File Dialog";
            fileDlgBrowseForReport.Filter = "XML Files|*.xml|All Files|*.*";
            fileDlgBrowseForReport.DefaultExt = "xml";

            string filePath = "";
            if (fileDlgBrowseForReport.ShowDialog() == DialogResult.OK)
            {
                //code to prevent uploading of files other than .xml
                if (!fileDlgBrowseForReport.FileName.ToLowerInvariant().EndsWith(".xml"))
                {
                    MessageBox.Show("Upload only Xml files with .xml extension", "Legacy Workbench",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    isXml = false;
                }

                if (isXml == true)
                {
                    filePath = fileDlgBrowseForReport.FileName;

                    DataSet dsReport = new DataSet("LegacyWorkbenchReport");

                    try
                    {
                        dsReport.ReadXml(filePath);
                    }
                    catch
                    {
                        MessageBox.Show("This is an invalid xml.", "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    foreach (DictionaryEntry de in htReportsForMenu)
                    {
                        if (de.Key.ToString().Contains(qualityEfficiencyReportToolStripMenuItem.Text))
                        {
                            repVwrDisplayReport.Refresh();
                            repVwrDisplayReport.Reset();
                            Microsoft.Reporting.WinForms.ReportDataSource fileDataSource = new Microsoft.Reporting.WinForms.ReportDataSource();
                            fileDataSource.Name = "Infosys_Lif_LegacyWorkbench_ReportManager_File";
                            fileDataSource.Value = this.FileBindingSource;
                            this.repVwrDisplayReport.LocalReport.DataSources.Add(fileDataSource);

                            repVwrDisplayReport.LocalReport.ReportEmbeddedResource = de.Value.ToString();

                            this.FileBindingSource.DataSource = dsReport;
                            this.FileBindingSource.DataMember = "File";
                            break;
                        }
                    }

                    this.repVwrDisplayReport.RefreshReport();
                    repVwrDisplayReport.Visible = true;
                }
            }
        }

        /// <summary>
        /// This event occurs when Load WSDL button is clicked
        /// </summary>
        /// <param name="sender"> sender object </param>
        /// <param name="e"> event args </param>
        private void btnLoadWsdl_Click(object sender, EventArgs e)
        {
            //to switch to model object tab if user is curremtly on any tab other than model object or contract
            if (!(tabControlMain.SelectedTab == tabModelObjects || tabControlMain.SelectedTab == tabContracts))
            {
                tabControlMain.SelectedTab = tabModelObjects;
            }

            Entities.Project retrievedProject = null;
            //chk if already any XML has been uploaded
            if (currProject != null)
            {
                if ((currProject.ContractModules.Count != 0) || (currProject.ModelObjectModules.Count != 0))
                {
                    DialogResult confirmationResult = MessageBox.Show(
                        "You have already opened a workbench instance. Do you want to open a new one?",
                        "Information", MessageBoxButtons.YesNo, MessageBoxIcon.Information);

                    if (confirmationResult == DialogResult.Yes)
                    {
                        //refresh workbench n open diaglog to upload new XML
                        ClearParser();

                        WsdlLoader loader = new WsdlLoader();
                        retrievedProject = loader.Load();
                    }
                }

                //XML uploaded for first time
                else
                {
                    WsdlLoader loader = new WsdlLoader();
                    retrievedProject = loader.Load();
                }
            }

            //XML uploaded after clearing workbench
            else
            {
                WsdlLoader loader = new WsdlLoader();
                retrievedProject = loader.Load();
            }

            //upload XML if retrieved proj is not null
            if (retrievedProject != null)
            {

                currProject = retrievedProject;
                //populate the hashtables ModelObjectEntityMapping and ModelObjectEntityCollection
                for (int loop = 0; loop < currProject.ModelObjectModules.Count; loop++)
                {
                    for (int loopInner = 0; loopInner < currProject.ModelObjectModules[loop].ModelObjects.Count; loopInner++)
                    {
                        ModelObjectEntityMapping.Add(currProject.ModelObjectModules[loop].ModelObjects[loopInner].ProgramId.ToString(), currProject.ModelObjectModules[loop].ModelObjects[loopInner]);
                        ModelObjectEntityCollection.Add(currProject.ModelObjectModules[loop].ModelObjects[loopInner].EntityName.ToString(), currProject.ModelObjectModules[loop].ModelObjects[loopInner].ProgramId.ToString());
                    }
                }

                areContractsBoundToModelObjects = false;

                if (currProject.ModelObjectModules != null && currProject.ModelObjectModules.Count > 0)
                {
                    BindModelObjectsTreeToProject();
                }

                else
                {
                    MessageBox.Show("The uploaded XML does not contain any model object","Legacy Workbench",
                        MessageBoxButtons.OK,MessageBoxIcon.Information);
                }

                if (currProject.ContractModules != null && currProject.ContractModules.Count > 0)
                {
                    BindContractsTreeToProject();
                }

                else
                {
                    MessageBox.Show("The uploaded XML does not contain any contract", "Legacy Workbench",
                       MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                int numberOfContracts = 0;
                foreach (Entities.ContractModule module in currProject.ContractModules)
                {
                    numberOfContracts += module.Contracts.Count;
                }

                lblNumberOfContracts.Text = numberOfContracts.ToString();

                int numberOfModelObjects = 0;
                foreach (Entities.ModelObjectModule module in currProject.ModelObjectModules)
                {
                    numberOfModelObjects += module.ModelObjects.Count;
                }

                lblNumberOfModelObjects.Text = numberOfModelObjects.ToString();

                //code changed to allow uploading of XML files which contains only model objects or contracts
                if (currProject.ModelObjectModules != null && currProject.ModelObjectModules.Count > 0)
                {
                    ungroupedbtnModelObjectsModule = currProject.ModelObjectModules[0];
                }

                if (currProject.ContractModules != null && currProject.ContractModules.Count > 0)
                {
                    ungroupedContractModule = currProject.ContractModules[0];
                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabModelObjects_Enter(object sender, EventArgs e)
        {
            if (treeModelObjects != null && treeModelObjects.Nodes.Count>0)
            {
                if (currProject.ProjectPrefix == null)
                {
                    currProject.ProjectPrefix = string.Empty;
                }
                modelObjectProjectEditor.Controls["txtProjectPrefix"].TextChanged -= modelObjectProjectEditor.txtProjectPrefix_TextChanged;
                modelObjectProjectEditor.Controls["txtProjectPrefix"].Text = currProject.ProjectPrefix;
                modelObjectProjectEditor.Controls["txtProjectPrefix"].TextChanged += modelObjectProjectEditor.txtProjectPrefix_TextChanged;
                modelObjectProjectEditor.Controls["grpBoxNamespaces"].Controls["txtEntityNameSpace"].Text = currProject.ProjectPrefix + currProject.ModelObjectNamespaces.DataEntityNamespace;
                modelObjectProjectEditor.Controls["grpBoxNamespaces"].Controls["txtSerializerNamespace"].Text = currProject.ProjectPrefix + currProject.ModelObjectNamespaces.SerializerNamespace;
                modelObjectProjectEditor.Controls["grpBoxNamespaces"].Controls["txtEntityRootNamespace"].Text = currProject.ProjectPrefix + currProject.ModelObjectNamespaces.DataEntityRootNamespace;
                modelObjectProjectEditor.Controls["grpBoxNamespaces"].Controls["txtSerializerRootNamespace"].Text = currProject.ProjectPrefix + currProject.ModelObjectNamespaces.SerializerRootNamespace;
                modelObjectProjectEditor.Controls["grpBoxNamespaces"].Controls["txtHostAccessNamespace"].Text = currProject.ProjectPrefix + currProject.ModelObjectNamespaces.HostAccessNamespace;
                modelObjectProjectEditor.Controls["grpBoxNamespaces"].Controls["txtHostAccessRootNameSpace"].Text = currProject.ProjectPrefix + currProject.ModelObjectNamespaces.HostAccessRootNamespace;
                moduleEditor.Controls["TextBox_ModuleEditor_SerializerNameSpace"].Text = currProject.ProjectPrefix + currProject.ModelObjectNamespaces.SerializerNamespace;
                moduleEditor.Controls["TextBox_ModuleEditor_DataEntityNameSpace"].Text = currProject.ProjectPrefix + currProject.ModelObjectNamespaces.DataEntityNamespace;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tabContracts_Enter(object sender, EventArgs e)
        {
            if (treeModelObjects != null && treeContracts.Nodes.Count > 0)
            {
                if (currProject.ProjectPrefix == null)
                {
                    currProject.ProjectPrefix = string.Empty;
                }
                contractProjectEditor.Controls["txtProjectPrefix"].TextChanged -= contractProjectEditor.txtProjectPrefix_TextChanged;
                contractProjectEditor.Controls["txtProjectPrefix"].Text = currProject.ProjectPrefix;
                contractProjectEditor.Controls["txtProjectPrefix"].TextChanged += contractProjectEditor.txtProjectPrefix_TextChanged;
                contractProjectEditor.Controls["grpBoxNamespaces"].Controls["txtEntityNameSpace"].Text = currProject.ProjectPrefix + currProject.ContractNamespaces.DataEntityNamespace;
                contractProjectEditor.Controls["grpBoxNamespaces"].Controls["txtSerializerNamespace"].Text = currProject.ProjectPrefix + currProject.ContractNamespaces.SerializerNamespace;
                contractProjectEditor.Controls["grpBoxNamespaces"].Controls["txtEntityRootNamespace"].Text = currProject.ProjectPrefix + currProject.ContractNamespaces.DataEntityRootNamespace;
                contractProjectEditor.Controls["grpBoxNamespaces"].Controls["txtSerializerRootNamespace"].Text = currProject.ProjectPrefix + currProject.ContractNamespaces.SerializerRootNamespace;
                contractProjectEditor.Controls["grpBoxNamespaces"].Controls["txtHostAccessNamespace"].Text = currProject.ProjectPrefix + currProject.ContractNamespaces.HostAccessNamespace;
                contractProjectEditor.Controls["grpBoxNamespaces"].Controls["txtHostAccessRootNameSpace"].Text = currProject.ProjectPrefix + currProject.ContractNamespaces.HostAccessRootNamespace;
                moduleEditor.Controls["TextBox_ModuleEditor_SerializerNameSpace"].Text = currProject.ProjectPrefix + currProject.ContractNamespaces.SerializerNamespace;
                moduleEditor.Controls["TextBox_ModuleEditor_DataEntityNameSpace"].Text = currProject.ProjectPrefix + currProject.ContractNamespaces.DataEntityNamespace;
            }

            //contractEditor.Select();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClearTab_Click(object sender, EventArgs e)
        {
            string currentTab = tabControlMain.SelectedTab.Text.Trim().ToLower();

            switch (currentTab)
            {
                case "model objects" :
                    ClearModelObjectTab();
                    break;

                case "contracts":
                    ClearContractTab();
                    break;

                case "log":
                    ClearLogTab();
                    break;

                case "test data":
                    ClearTestDataTab();
                    break;

                case "report":
                    ClearReportTab();
                    break;

                default:
                    ClearParser();
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ClearModelObjectTab()
        {
            //reset editors
            modelObjectEditor = new ModelObjectEditor();
            modelObjectEditor.Modified += new ModelObjectEditor.OnModified(LegacyParser_Modified);

            //moduleEditor = new ModuleEditor();
            //moduleEditor.Modified += new ModuleEditor.OnModified(moduleDetailsModified);
                        
            modelObjectProjectEditor = new ProjectEditor();
            modelObjectProjectEditor.Modified += new ProjectEditor.OnModified(ModelObjectsProjEditor_Modified);
                        
            //clear model obj mapping
            ModelObjectEntityMapping.Clear();

            //clear the value of ModelObjectEntityCollection
            ModelObjectEntityCollection.Clear();

            //Clear the editor space            
            modelObjectEditPanel.Controls.Clear();

            //clear tree view            
            treeModelObjects.Nodes.Clear();

            //clear entities
            //currProject = null;
            currProject.ModelObjectModules.Clear();
            currProject.ModelObjectNamespaces = null;
            
            ungroupedbtnModelObjectsModule = null;
            ungroupedbtnModelObjectsModule = new Entities.ModelObjectModule();            
            ungroupedbtnModelObjectsModule.Name = UngroupedNodeText;            
                        
            //clear breadcrumbs : model object
            breadcumModelObjectElements.Visible = false;
            breadcumModelObjectModuleImage.Visible = false;
            breadcumModelObjectRoot.Visible = false;
            breadcumModelObjectRootImage.Visible = false;
            breadcumModoleobjectModules.Visible = false;

            //clear Status label
            lblStatusMessage.Text = "Clearing Model Object Tab...";            
            lblNumberOfModelObjects.Text = "0";

            //clear project editor's static values
            //ProjectEditor.duplicatePrefixValue = string.Empty;
            //ProjectEditor.projectPrefix = string.Empty;
            //ProjectEditor.valuePrefix = string.Empty;    
        }

        /// <summary>
        /// 
        /// </summary>
        private void ClearContractTab()
        {
            //reset editors
            //modelObjectEditor = new ModelObjectEditor();
            //modelObjectEditor.Modified += new ModelObjectEditor.OnModified(LegacyParser_Modified);

            //moduleEditor = new ModuleEditor();
            //moduleEditor.Modified += new ModuleEditor.OnModified(moduleDetailsModified);

            contractProjectEditor = new ProjectEditor();
            contractProjectEditor.Modified += new ProjectEditor.OnModified(ContractsProjEditor_Modified);

            //modelObjectProjectEditor = new ProjectEditor();
            //modelObjectProjectEditor.Modified += new ProjectEditor.OnModified(ModelObjectsProjEditor_Modified);

            contractEditor = new ContractEditor();
            contractEditor.Modified += new ContractEditor.OnModified(LegacyParser_Modified);
            contractEditor.NavigateToModelObject
                += new Editors.ContractEditor.NavigateToModelObjectEvent(LegacyParser_NavigateToModelObject);

            //clear model obj mapping
            //ModelObjectEntityMapping.Clear();

            //clear the value of ModelObjectEntityCollection
            //ModelObjectEntityCollection.Clear();

            //Clear the editor space
            contractEditPanel.Controls.Clear();
            //modelObjectEditPanel.Controls.Clear();

            //clear tree view
            treeContracts.Nodes.Clear();
            //treeModelObjects.Nodes.Clear();

            //clear entities
            //currProject = null;
            currProject.ContractModules.Clear();
            currProject.ContractNamespaces = null;

            //ungroupedbtnModelObjectsModule = null;
            ungroupedContractModule = null;
            //currProject = new Entities.Project();
            //ungroupedbtnModelObjectsModule = new Entities.ModelObjectModule();
            ungroupedContractModule = new Entities.ContractModule();
            //ungroupedbtnModelObjectsModule.Name = UngroupedNodeText;
            ungroupedContractModule.Name = UngroupedNodeText;

            //clear breadcrumbs : contract
            breadcumContractModel.Visible = false;
            breadcumContractModelImage.Visible = false;
            breadcumContractRootImage.Visible = false;
            breadcumContractsElements.Visible = false;
            breadcumContractRoot.Visible = false;

            //clear breadcrumbs : model object
            //breadcumModelObjectElements.Visible = false;
            //breadcumModelObjectModuleImage.Visible = false;
            //breadcumModelObjectRoot.Visible = false;
            //breadcumModelObjectRootImage.Visible = false;
            //breadcumModoleobjectModules.Visible = false;

            //clear Status label
            lblStatusMessage.Text = "Clearing Contract Tab...";
            lblNumberOfContracts.Text = "0";
            //lblNumberOfModelObjects.Text = "0";

            //clear project editor's static values
            //ProjectEditor.duplicatePrefixValue = string.Empty;
            //ProjectEditor.projectPrefix = string.Empty;
            //ProjectEditor.valuePrefix = string.Empty;    
        }

        /// <summary>
        /// 
        /// </summary>
        private void ClearLogTab()
        {
            this.txtSummary.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        private void ClearTestDataTab()
        {
            //yet to implement
        }

        /// <summary>
        /// 
        /// </summary>
        private void ClearReportTab()
        {
            repVwrDisplayReport.Visible = false;
            comboBoxReportType.SelectedIndex = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void modelObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearModelObjectTab();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contractToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearContractTab();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearLogTab();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void testDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearTestDataTab();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reportToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ClearReportTab();
        }
    }
}   


