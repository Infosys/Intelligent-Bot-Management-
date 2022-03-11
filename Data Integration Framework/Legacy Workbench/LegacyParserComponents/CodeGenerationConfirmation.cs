using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Infosys.Lif.LegacyWorkbench.CodeProviders.Serializers;
using Infosys.Solutions.CodeGeneration.Framework.Generators;
using Infosys.Lif.LegacyWorkbench.CodeProviders.DataEntities;
using Infosys.Lif.LegacyWorkbench.CodeProviders;

namespace Infosys.Lif.LegacyWorkbench.CodeProviders
{
    /// <summary>
    /// This class contains properties and methods related to code generation form
    /// </summary>
    public partial class CodeGenerationConfirmation : Form
    {
        // default name for generated solution
        const string LifGeneratedSolnName = "LifGeneratedSolution";

        /// <summary>
        /// Name of solution file to be generated
        /// </summary>        
        public string SolnFileName
        {
            get
            {
                return textSolnName.Text;
            }
        }

        /// <summary>
        /// Is Web service interface to be generated
        /// </summary>        
        public bool WebServicesToBeGenerated
        {
            get
            {
                return radWebServices.Enabled && radWebServices.Checked;
            }
        }

        /// <summary>
        /// Is generated solution to be compiled
        /// </summary>        
        public bool IsToBeCompiled
        {
            get
            {
                return cheCompile.Checked;
            }
        }
        
        private bool isReportToBeGenerated;

        /// <summary>
        /// Is the report to be generated for the solution being generated
        /// </summary>        
        public bool IsReportToBeGenerated
        {
            get
            {
                return cheViewReport.Checked;
            }

            set
            {
                isReportToBeGenerated = value;
            }
        }
        
        private bool generationCancelled = false;

        /// <summary>
        /// Is generation been cancelled
        /// </summary>        
        public bool GenerationCancelled
        {
            get { return generationCancelled; }
            set { generationCancelled = value; }
        }
        
        bool prtcheServiceInterface;

        /// <summary>
        /// Is service interface to be generated
        /// </summary>        
        public bool PRTcheServiceInterface
        {
            get { return prtcheServiceInterface; }
            set { prtcheServiceInterface = value; }
        }

        bool prtSerializerClass;

        /// <summary>
        /// Is serializer class to be generated
        /// </summary>        
        public bool PRTSerializerClass
        {
            get { return prtSerializerClass; }
            set { prtSerializerClass = value; }
        }

        bool prtSerializerContract;

        /// <summary>
        /// Is serializer class to be generated for contracts
        /// </summary>        
        public bool PRTSerializerContract
        {
            get { return prtSerializerContract; }
            set { prtSerializerContract = value; }
        }

        bool prtSerializerModelObjects;

        /// <summary>
        /// Is serializer class to be generated for model objects
        /// </summary>        
        public bool PRTSerializerModelObjects
        {
            get { return prtSerializerModelObjects; }
            set { prtSerializerModelObjects = value; }
        }

        bool prtDataEntityClass;

        /// <summary>
        /// Is data entity class to be generated
        /// </summary>        
        public bool PRTDataEntityClass
        {
            get { return prtDataEntityClass; }
            set { prtDataEntityClass = value; }
        }

        bool prtDataEntityContracts;

        /// <summary>
        /// Is data entity class to be generated for contracts
        /// </summary>        
        public bool PRTDataEntityContracts
        {
            get { return prtDataEntityContracts; }
            set { prtDataEntityContracts = value; }
        }

        bool prtDataEntityModelObjects;

        /// <summary>
        /// Is data entity class to be generated for model objects
        /// </summary>        
        public bool PRTDataentityModuleObjects
        {
            get { return prtDataEntityModelObjects; }
            set { prtDataEntityModelObjects = value; }
        }

        bool prtDataEntitySchemaClass;

        /// <summary>
        /// Is data entity schema(XSD) to be generated
        /// </summary>        
        public bool PRTDataEntitySchemaClass
        {
            get { return prtDataEntitySchemaClass; }
            set { prtDataEntitySchemaClass = value; }
        }

        bool prtDataEntitySchemaContracts;

        /// <summary>
        /// Is data entity schema(XSD) to be generated for contracts
        /// </summary>        
        public bool PRTDataEntitySchemaContracts
        {
            get { return prtDataEntitySchemaContracts; }
            set { prtDataEntitySchemaContracts = value; }
        }

        bool prtDataEntitySchemaModelObjects;

        /// <summary>
        /// Is data entity schema(XSD) to be generated for model objects
        /// </summary>        
        public bool PRTDataEntitySchemaModelObjects
        {
            get { return prtDataEntitySchemaModelObjects; }
            set { prtDataEntitySchemaModelObjects = value; }
        }
                
        bool prtRadioHostAcess;  
    
        /// <summary>
        /// Is host access service interface to be generated
        /// </summary>        
        public bool PRTRadioHostAcess
        {
          get { return prtRadioHostAcess; }
          set { prtRadioHostAcess = value; }
        }
                        
        bool prtRadioWFActivity;

        /// <summary>
        /// Is WF activity to be generated as a service interface
        /// </summary>        
        public bool PRTRadioWFActivity
        {
            get { return prtRadioWFActivity; }
            set { prtRadioWFActivity = value; }
        }  
                      
        bool prtRadioWebService;

        /// <summary>
        /// Is web service interface to be generated
        /// </summary>       
        public bool PRTRadioWebService
        {
            get { return prtRadioWebService; }
            set { prtRadioWebService = value; }
        }
        
        bool prtRadioWCFService;

        /// <summary>
        /// is WCF service interface to be generated
        /// </summary>        
        public bool PRTRadioWCFService
        {
            get { return prtRadioWCFService; }
            set { prtRadioWCFService = value; }
        }
                
        bool prtRadioRSSFeed;

        /// <summary>
        /// Is RSS Feed service interface to be generated
        /// </summary>        
        public bool PRTRadioRSSFeed
        {
            get { return prtRadioRSSFeed; }
            set { prtRadioRSSFeed = value; }
        }

        bool prtRadioAtom;

        /// <summary>
        /// Is Atom service interface to be generated
        /// </summary>        
        public bool PrtRadioAtom
        {
            get { return prtRadioAtom; }
            set { prtRadioAtom = value; }
        }
                       
        string generationFormat;

        /// <summary>
        /// which type of format to use (TINS or Cobol)
        /// </summary>        
        public string GenerationFormat
        {
            get { return generationFormat; }
            set { generationFormat = value; }
        }

        string outputDirectory;

        /// <summary>
        /// Output location where the solution is to be generated
        /// </summary>        
        public string OutputDirectory
        {
            get { return outputDirectory; }
            set { outputDirectory = value; }
        }
        
        string serviceInterface;

        /// <summary>
        /// which type of service interface is being generated
        /// </summary>        
        public string ServiceInterface
        {
            get { return serviceInterface; }
            set { serviceInterface = value; }
        }
                
        /// <summary>
        /// Constructor to initialize form components
        /// </summary>
        public CodeGenerationConfirmation()
        {
            InitializeComponent();
        }
                        
        /// <summary>
        /// Opens the Folder Browser Dialog and allows the user to select a path
        /// </summary>
        /// <returns>output location where the solution is to be generated</returns>
        private string GetOutputDirecory()
        {
            System.Windows.Forms.FolderBrowserDialog destinationSelector = new System.Windows.Forms.FolderBrowserDialog();

            if (destinationSelector.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return destinationSelector.SelectedPath;
            }

            return string.Empty;
        }
        
        /// <summary>
        /// Handles the event of click on browse button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowseCodeGC_Click(object sender, EventArgs e)
        {
            outputDirectory = GetOutputDirecory();
            textGenPath.Text = outputDirectory.ToString();
        }
        
        /// <summary>
        /// Handle check change event for Serializer class check box
        /// if its checked, check model object and contract check box too and vice versa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cheSerializerClass_CheckedChanged(object sender, EventArgs e)
        {
            if (cheSerializerClass.Checked)
            {
                cheSerializerContracts.Checked = true;
                cheSerializerModelObjects.Checked = true;
            }
            else
            {
                cheSerializerContracts.Checked = false;
                cheSerializerModelObjects.Checked = false;
                cheCompile.Checked = false;
            }           
        }
        
        /// <summary>
        /// Handle the click event of the Generate button. Check which all things to be generated and
        /// output location is provided for generating the solution.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnGenrateCodeGC_Click(object sender, EventArgs e)
        {            
            // set all variables according to the inputs of controls on generation form
            prtSerializerClass = cheSerializerClass.Checked;
            prtSerializerContract = cheSerializerContracts.Checked;
            prtSerializerModelObjects = cheSerializerModelObjects.Checked;
            prtDataEntityClass = cheDataEntitiesClass.Checked;
            prtDataEntityContracts = cheDataEntitesContracts.Checked;
            prtDataEntityModelObjects = cheDataEntiesModelObjects.Checked;
            prtDataEntitySchemaClass = cheDataEntitesSchema.Checked;
            prtDataEntitySchemaContracts = cheDataEntitesSchemaContracts.Checked;
            prtDataEntitySchemaModelObjects = cheDataEntitesSchemaModelObjects.Checked;
            prtRadioHostAcess = radHostAccess.Checked;            
            prtRadioWFActivity = radWfActivity.Checked;            
            prtRadioWebService = radWebServices.Checked;            
            prtRadioWCFService = radWCF.Checked;            
            prtRadioRSSFeed = radRSSFeed.Checked;
            prtRadioAtom = radAtom.Checked;
            prtcheServiceInterface = cheServiceInterface.Checked;

            // set format of genaration
            generationFormat = "cobol";

            // check which type of service interface is being generated ans set ServiceInterface property
            if (prtRadioHostAcess == true)
            {
                ServiceInterface = radHostAccess.Text;
            }

            if (prtRadioWebService == true)
            {
                ServiceInterface = radWebServices.Text;
            }

            if (prtRadioWCFService == true)
            {
                ServiceInterface = radWCF.Text;
            }

            if (prtRadioRSSFeed == true)
            {
                ServiceInterface = radRSSFeed.Text;
            }

            if (prtRadioWFActivity == true)
            {
                ServiceInterface = radWfActivity.Text;
            }

            if (prtRadioAtom == true)
            {
                ServiceInterface = radAtom.Text;
            }

            // if Generate button is clicked without supplying output path, show error message
            if (this.OutputDirectory == null || this.OutputDirectory == string.Empty)
            {
                MessageBox.Show("Please provide the output path for the solution", "ERROR", MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);                
                this.DialogResult = DialogResult.None;                
            }

            // if Solution name is not provided, show error message
            else if (this.SolnFileName.Trim().Equals(string.Empty) || this.SolnFileName == null)
            {
                MessageBox.Show("Please provide the solution name", "ERROR", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                this.DialogResult = DialogResult.None;
            }
            else
            {                
                this.DialogResult = DialogResult.OK;
            }            
        }
        
        /// <summary>
        /// Handle check change event for data entity class check box
        /// if its checked, check model object and contract check box too and vice versa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cheDataEntityClass_CheckedChanged(object sender, EventArgs e)
        {
            if (cheDataEntitiesClass.Checked)
            {
                cheDataEntitesContracts.Checked = true;
                cheDataEntiesModelObjects.Checked = true;
            }
            else
            {
                cheDataEntitesContracts.Checked = false;
                cheDataEntiesModelObjects.Checked = false;
                cheCompile.Checked = false;
            }  
        }

        /// <summary>
        /// Handle check change event for Service Interface check box
        /// if its checked, activate the radio buttons for all service types and vice versa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cheServiceInterface_CheckedChanged(object sender, EventArgs e)
        {
            // if Service Interface check box is checked, activate all radio buttons to select 
            // one service interface type to generate
            if (cheServiceInterface.Checked == true)
            {
                radHostAccess.Enabled = false;
                radWfActivity.Enabled = false;
                radWCF.Enabled = false;                
                radRSSFeed.Enabled = false;
                radWebServices.Enabled = true;
                radHostAccess.Checked = false;               
            }

            // else dont activate radio buttons
            else
            {
                radHostAccess.Enabled = false;
                radWfActivity.Enabled = false;
                radWCF.Enabled = false;                
                radRSSFeed.Enabled = false;
                radWebServices.Enabled = false;
                cheCompile.Checked = false;
            }
        }

        /// <summary>
        /// Handle check change event for data entity schema check box
        /// if its checked, check model object and contract check box too and vice versa
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cheDataEntitySchema_CheckedChanged(object sender, EventArgs e)
        {
            // check the check box status
            if (cheDataEntitesSchema.Checked)
            {
                cheDataEntitesSchemaContracts.Checked = true;
                cheDataEntitesSchemaModelObjects.Checked = true;
            }
            else
            {
                cheDataEntitesSchemaContracts.Checked = false;
                cheDataEntitesSchemaModelObjects.Checked = false;
                cheCompile.Checked = false;
            }
        }

        /// <summary>
        /// Handle click event of cancel event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            generationCancelled = true;                       
        }

        /// <summary>
        /// handle load event of code generation form. Set default values of all controls
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CodeGenerationConfirmation_Load(object sender, EventArgs e)
        {
            // set default values of all controls on the form.
            cheDataEntitesSchema.Checked = true;
            cheDataEntitesSchemaContracts.Checked = true;
            cheDataEntitesSchemaModelObjects.Checked = true;
            cheServiceInterface.Checked = true;
            cheDataEntitiesClass.Checked = true;
            cheDataEntitesContracts.Checked = true;
            cheDataEntiesModelObjects.Checked = true;
            cheSerializerClass.Checked = true;
            cheSerializerContracts.Checked = true;
            cheSerializerModelObjects.Checked = true;
            cheCompile.Checked = true; 

            // set default name of Generated soln
            textSolnName.Text = LifGeneratedSolnName;            
        }
        
        /// <summary>
        /// Handle check change event of compile check box.
        /// if its checked, check all other check boxes too.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cheCompile_CheckedChanged(object sender, EventArgs e)
        {
            // the solution can only be compiled if its all components are being generated
            if (cheCompile.Checked == true)
            {
                cheDataEntitesSchema.Checked = true;
                cheDataEntitesSchemaContracts.Checked = true;
                cheDataEntitesSchemaModelObjects.Checked = true; 
                cheServiceInterface.Checked = true;               
                cheDataEntitiesClass.Checked = true;
                cheDataEntitesContracts.Checked = true;
                cheDataEntiesModelObjects.Checked = true; 
                cheSerializerClass.Checked = true;
                cheSerializerContracts.Checked = true;
                cheSerializerModelObjects.Checked = true;                
            }   
        }

        /// <summary>
        /// Handle check change event of Contract Serializer class check box.
        /// if its unchecked, uncheck compile check box too.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cheSerializerContracts_CheckedChanged(object sender, EventArgs e)
        {
            if (cheSerializerContracts.Checked == false)
            {
                cheCompile.Checked = false;
            }
        }

        /// <summary>
        /// Handle check change event of Model Object Serializer check box.
        /// if its unchecked, uncheck compile check box too.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cheSerializerModelObjects_CheckedChanged(object sender, EventArgs e)
        {
            if (cheSerializerModelObjects.Checked == false)
            {
                cheCompile.Checked = false;
            }
        }

        /// <summary>
        /// Handle check change event of Model Object Data Entity check box.
        /// if its unchecked, uncheck compile check box too.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cheDataEntiesModelObjects_CheckedChanged(object sender, EventArgs e)
        {
            if (cheDataEntiesModelObjects.Checked == false)
            {
                cheCompile.Checked = false;
            }
        }

        /// <summary>
        /// Handle check change event of Contract Data Entity check box.
        /// if its unchecked, uncheck compile check box too.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cheDataEntitesContracts_CheckedChanged(object sender, EventArgs e)
        {
            if (cheDataEntitesContracts.Checked == false)
            {
                cheCompile.Checked = false;
            }
        }
        
        /// <summary>
        /// Handle check change event of Contract Data Entity Schema check box.
        /// if its unchecked, uncheck compile check box too.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cheDataEntitesShemaContracts_CheckedChanged(object sender, EventArgs e)
        {
            if (cheDataEntitesSchemaContracts.Checked == false)
            {
                cheCompile.Checked = false;
            }
        }

        /// <summary>
        /// Handle check change event of Model Object Data Entity Schema check box.
        /// if its unchecked, uncheck compile check box too.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cheDataEntitesSchemaModelObjects_CheckedChanged(object sender, EventArgs e)
        {
            if (cheDataEntitesSchemaModelObjects.Checked == false)
            {
                cheCompile.Checked = false;
            }
        }

        /// <summary>
        /// Handle check change event for View Report check box.
        /// If its checked, set IsReportToBeGenerated as true
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cheViewReport_CheckedChanged(object sender, EventArgs e)
        {
            if (cheViewReport.Checked == true)
            {
                IsReportToBeGenerated = true;
            }
        }

        /// <summary>
        /// Handle form close event for the code generation form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CodeGenerationConfirmation_FormClosed(object sender, FormClosedEventArgs e)
        {             
            // if the form is closed using the close button then set generationCancelled as true
            if (this.DialogResult != DialogResult.OK)
            {
                generationCancelled = true;
            }
        }       
    }
}
