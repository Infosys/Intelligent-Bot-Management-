using System;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Infosys.Lif.LegacyWorkbench.Editors
{
    public partial class ProjectEditor : Editors.Editor
    {
        /// <summary>
        /// constructor of project editor to initialize all design components.
        /// </summary>
        public ProjectEditor()
        {
            InitializeComponent();
        }

        /// <summary>
        /// This calss implements EventArgs and holds modified namespace values whenever project editor is modified
        /// </summary>
        public class ModifiedEventArgs : EventArgs
        {
            Entities.Project.ProjectNamespaces modifiedNameSpaces;

            public Entities.Project.ProjectNamespaces ModifiedNameSpaces
            {
                get { return modifiedNameSpaces; }
                set { modifiedNameSpaces = value; }
            }
        }
        
        //declare delegate and event for modifying project editor
        public delegate void OnModified(object sender, ModifiedEventArgs e);
        public event OnModified Modified;
        
        //declare/initialize variables
        Entities.Project.ProjectNamespaces namespacesBeingEdited;
        Entities.Project editedProject;
        private Entities.Project.ProjectNamespaces originalProjectNamespaces;

        bool isClausesBeingEdited = false;
        bool isModelObjNamespaceLoadedAtFirstTime = true;
        bool isContractNamespaceLoadedAtFirstTime = true;

        public bool IsProjectEditorDirty;
                
        public static string projectPrefix = null;
        public static bool projectEditorLeaveCalled;

        string dataEntityNamespace;

        public string DataEntityNamespace
        {
            get { return dataEntityNamespace; }
            set { dataEntityNamespace = value; }
        }

        string dataEntityRootNamespace;
        string serNamespace;
        string serRootNamespace;
        string hostAccessNamespace;
        string hostAccessRootNamespace;
        
        /// <summary>
        /// makes a copy of input and returns the same
        /// </summary>
        /// <param name="projectNamespacesToBeCloned"></param>
        /// <returns>the cloned ProjectNamespaces</returns>
        private Entities.Project.ProjectNamespaces Clone(Entities.Project.ProjectNamespaces projectNamespacesToBeCloned)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(Entities.Project.ProjectNamespaces));
            System.IO.MemoryStream memStream = new System.IO.MemoryStream();
            serializer.Serialize(memStream, projectNamespacesToBeCloned);
            memStream.Position = 0;
            return (Entities.Project.ProjectNamespaces)serializer.Deserialize(memStream);
        }

        /// <summary>
        /// populates the controls of project editor with the model object namespaces from the input project
        /// i.e. fills the value in controls
        /// </summary>
        /// <param name="projectToBeEdited"></param>
        /// <param name="isProjectEditorDirty"></param>
        public void PopulateModelObjects(Entities.Project projectToBeEdited, bool isProjectEditorDirty)
        {
            IsDirty = isProjectEditorDirty;
            isClausesBeingEdited = true;
            namespacesBeingEdited = projectToBeEdited.ModelObjectNamespaces;
            editedProject = projectToBeEdited;
            string isModelObject = "true";
            //Call Populate method to fill the values in controls
            Populate(isModelObject);

            //hide the labels and text boxes of host access for model object project editor
            txtHostAccessNamespace.Visible = false;
            txtHostAccessRootNameSpace.Visible = false;
            lblHostAccessNS.Visible = false;
            lblHostAccessRootNS.Visible = false;

            LegacyParser legacyParserObj = new LegacyParser();
            //provide value to project prefix text
            txtProjectPrefix.Text = editedProject.ProjectPrefix;

            //provide default namespaces
            txtEntityNameSpace.Text =editedProject.ProjectPrefix + legacyParserObj.ModelObjDataEntityNS;
            //"ModelObjectDataEntity.#EntityName#";

            txtEntityRootNamespace.Text = editedProject.ProjectPrefix + legacyParserObj.ModelObjDataEntityRootNS;
            //"ModelObjectDataEntityAssembly";

            txtSerializerNamespace.Text = editedProject.ProjectPrefix + legacyParserObj.ModelObjSerializerNS;
            //"ModelObjectSerializer";

            txtSerializerRootNamespace.Text = editedProject.ProjectPrefix + legacyParserObj.ModelObjSerializerRootNS;
            //"ModelObjectSerializerAssembly";

            txtXmlSchemaNamespace.Text = legacyParserObj.ModelObjXmlNS;
            //"ModelObjectXmlSchema.#EntityName#";            

            //for the first time of loading the namespaces, the values shud get saved
            if (isModelObjNamespaceLoadedAtFirstTime)
            {
                IsDirty = false;
                isModelObjNamespaceLoadedAtFirstTime = false;
            }
        }
        
        /// <summary>
        /// populates the controls of project editor with the Contract namespaces from the input project
        /// i.e. fills the value in controls
        /// </summary>
        /// <param name="projectToBeEdited"></param>
        public void PopulateContracts(Entities.Project projectToBeEdited)
        {
            LegacyParser legacyParserObj = new LegacyParser();
            
            isClausesBeingEdited = false;
            namespacesBeingEdited = projectToBeEdited.ContractNamespaces;
            namespacesBeingEdited.XmlSchemaNamespace = legacyParserObj.ContractXmlNS;
            txtXmlSchemaNamespace.Text = legacyParserObj.ContractXmlNS;            
            editedProject = projectToBeEdited;
            string isModelObject = "false";
            //Call Populate method to fill the values in controls
            Populate(isModelObject);
            //provide value to project prefix text
            txtProjectPrefix.Text = editedProject.ProjectPrefix;  
            //provide default namespaces
            txtEntityNameSpace.Text = editedProject.ProjectPrefix + legacyParserObj.ContractDataEntityNS;
            //"ContractDataEntity.#EntityName#";

            txtEntityRootNamespace.Text = editedProject.ProjectPrefix + legacyParserObj.ContractDataEntityRootNS;
            //"ContractDataEntityAssembly";

            txtSerializerNamespace.Text = editedProject.ProjectPrefix + legacyParserObj.ContractSerializerNS;
            //"ContractSerializer";

            txtSerializerRootNamespace.Text = editedProject.ProjectPrefix + legacyParserObj.ContractSerializerRootNS;
            //"ContractSerializerAssembly";

            txtXmlSchemaNamespace.Text = legacyParserObj.ContractXmlNS;
            //"ContractXmlSchema.#EntityName#";

            txtHostAccessNamespace.Text = editedProject.ProjectPrefix + legacyParserObj.ContractHostAccessNS;
            //"HostAccess.#ModuleName#";

            txtHostAccessRootNameSpace.Text = editedProject.ProjectPrefix + legacyParserObj.ContractHostAccessRootNS;
            //"HostAccessAssembly.#ModuleName#";

            if (isContractNamespaceLoadedAtFirstTime)
            {
                IsDirty = false;
                isContractNamespaceLoadedAtFirstTime = false;
            }
        }
        

        /// <summary>
        /// Get the values for controls in the project editor and fill those.
        /// </summary>
        private void Populate(string isModelObject)
        {
            LegacyParser legacyParserObj = new LegacyParser();

            string contractXmlNamespace = legacyParserObj.ContractXmlNS;
            string modelObjectXmlNamespace=legacyParserObj.ModelObjXmlNS;

            #region old code
            ////////****** Check why this ondition is used.
            ////////if (editedProject.ProjectPrefix == txtProjectPrefix.Text && editedProject.ProjectPrefix != string.Empty && 
            ////////    btnCancelClicked != true)
            ////////{
            ////////    return;
            ////////}

            ////////fill the value in controls without calling their corresponding text change event            
            ////////txtXmlSchemaNamespace.TextChanged -= txtXmlSchemaNamespace_TextChanged;

            ////////*******yet to check
            ////////if (namespacesBeingEdited.HostAccessNamespace != null)
            ////////{
            //////    //hostaccess namespces to be filled only for contract project editor
            //////    if (namespacesBeingEdited.XmlSchemaNamespace.Contains(contractXmlNamespace))
            //////    {
            //////        txtXmlSchemaNamespace.Text = contractXmlNamespace;
            //////    }

            //////    else if (namespacesBeingEdited.XmlSchemaNamespace.Contains(modelObjectXmlNamespace))
            //////    {
            //////        txtXmlSchemaNamespace.Text = modelObjectXmlNamespace;
            //////    }
            ////////}
            ////////txtXmlSchemaNamespace.TextChanged += txtXmlSchemaNamespace_TextChanged;

            ////////if cancel button clicked and there are unsaved changes then revert to last saved data
            //////if (btnCancelClicked == true)
            //////{
            //////    if (txtProjectPrefix.Text == "" && valuePrefix == null)
            //////    {
            //////        return;
            //////    }

            //////    txtProjectPrefix.Text = valuePrefix;
            //////    return;
            //////}

            ////////**
            //////if (namespacesBeingEdited.DataEntityNamespace != string.Empty)
            //////{
            //////    if (txtXmlSchemaNamespace.Text.Contains(contractXmlNamespace))
            //////    {
            //////        namespacesBeingEdited.DataEntityNamespace = legacyParserObj.ContractDataEntityNS;
            //////        namespacesBeingEdited.DataEntityRootNamespace = legacyParserObj.ContractDataEntityRootNS;
            //////        namespacesBeingEdited.HostAccessNamespace = legacyParserObj.ContractHostAccessNS;
            //////        namespacesBeingEdited.HostAccessRootNamespace = legacyParserObj.ContractHostAccessRootNS;
            //////        namespacesBeingEdited.SerializerNamespace = legacyParserObj.ContractSerializerNS;
            //////        namespacesBeingEdited.SerializerRootNamespace = legacyParserObj.ContractSerializerRootNS;
            //////    }

            //////    //making the value of namespacesbeingedited to original for modelobject being uploaded                    
            //////    else if(txtXmlSchemaNamespace.Text.Contains(modelObjectXmlNamespace))
            //////    {
            //////        namespacesBeingEdited.DataEntityNamespace = legacyParserObj.ModelObjDataEntityNS;
            //////        namespacesBeingEdited.DataEntityRootNamespace = legacyParserObj.ModelObjDataEntityRootNS;                        
            //////        namespacesBeingEdited.SerializerNamespace = legacyParserObj.ModelObjSerializerNS;
            //////        namespacesBeingEdited.SerializerRootNamespace = legacyParserObj.ModelObjSerializerRootNS;
            //////    }
            //////}
            
            //////originalProjectNamespaces = Clone(namespacesBeingEdited);

            ////////fill XML namespace
            //////txtProjectPrefix.TextChanged -= txtProjectPrefix_TextChanged;
            //////txtProjectPrefix.Text = editedProject.ProjectPrefix;
            ////////to remove the error in XML file wherein the initial value of project Prefix is not saved
            //////if (txtProjectPrefix.Text != "" && txtEntityNameSpace.Text == "" && txtEntityRootNamespace.Text == "")
            //////{
            //////    valuePrefix = txtProjectPrefix.Text;
            //////}

            //////txtProjectPrefix.TextChanged += txtProjectPrefix_TextChanged;

            ////////fill data entity namespace
            ////////txtEntityNameSpace.TextChanged -= txtEntityNameSpace_TextChanged;                   
            //////txtEntityNameSpace.Text = txtProjectPrefix.Text + namespacesBeingEdited.DataEntityNamespace;
            ////////txtEntityNameSpace.TextChanged += txtEntityNameSpace_TextChanged;

            ////////fill data entity root namespace
            ////////txtEntityRootNamespace.TextChanged -= txtEntityRootNamespace_TextChanged;
            //////txtEntityRootNamespace.Text = txtProjectPrefix.Text + namespacesBeingEdited.DataEntityRootNamespace;
            ////////txtEntityRootNamespace.TextChanged += txtEntityRootNamespace_TextChanged;

            ////////fill host access namespace
            ////////txtHostAccessNamespace.TextChanged -= txtHostAccessNamespace_TextChanged;
            //////txtHostAccessNamespace.Text = txtProjectPrefix.Text + namespacesBeingEdited.HostAccessNamespace;
            ////////txtHostAccessNamespace.TextChanged += txtHostAccessNamespace_TextChanged;

            ////////fill host access root namespace
            ////////txtHostAccessRootNameSpace.TextChanged -= txtHostAccessRootNameSpace_TextChanged;
            //////txtHostAccessRootNameSpace.Text = txtProjectPrefix.Text + namespacesBeingEdited.HostAccessRootNamespace;
            ////////txtHostAccessRootNameSpace.TextChanged += txtHostAccessRootNameSpace_TextChanged;

            ////////fill serializer namespace
            ////////txtSerializerNamespace.TextChanged -= txtSerializerNamespace_TextChanged;
            //////txtSerializerNamespace.Text = txtProjectPrefix.Text + namespacesBeingEdited.SerializerNamespace;
            ////////txtSerializerNamespace.TextChanged += txtSerializerNamespace_TextChanged;

            ////////fill serializer root namespace
            ////////txtSerializerRootNamespace.TextChanged -= txtSerializerRootNamespace_TextChanged;
            //////txtSerializerRootNamespace.Text = txtProjectPrefix.Text + namespacesBeingEdited.SerializerRootNamespace;
            ////////txtSerializerRootNamespace.TextChanged += txtSerializerRootNamespace_TextChanged;
            #endregion

            if (isModelObject == "true")
            {
                namespacesBeingEdited.DataEntityNamespace = valuePrefix + legacyParserObj.ModelObjDataEntityNS;
                namespacesBeingEdited.DataEntityRootNamespace = valuePrefix + legacyParserObj.ModelObjDataEntityRootNS;
                namespacesBeingEdited.SerializerNamespace = valuePrefix + legacyParserObj.ModelObjSerializerNS;
                namespacesBeingEdited.SerializerRootNamespace = valuePrefix + legacyParserObj.ModelObjSerializerRootNS;
                namespacesBeingEdited.XmlSchemaNamespace = valuePrefix + legacyParserObj.ModelObjXmlNS;
                originalProjectNamespaces = Clone(namespacesBeingEdited);
            }
            else if(isModelObject=="false")
            {
                //Figee: July 15th No need to add the prefixes it gets taken care in code generation.
                namespacesBeingEdited.DataEntityNamespace = legacyParserObj.ContractDataEntityNS;
                namespacesBeingEdited.DataEntityRootNamespace = legacyParserObj.ContractDataEntityRootNS;
                namespacesBeingEdited.SerializerNamespace = legacyParserObj.ContractSerializerNS;
                namespacesBeingEdited.SerializerRootNamespace = legacyParserObj.ContractSerializerRootNS;
                namespacesBeingEdited.HostAccessNamespace = legacyParserObj.ContractHostAccessNS;
                namespacesBeingEdited.HostAccessRootNamespace = legacyParserObj.ContractHostAccessRootNS;
                namespacesBeingEdited.XmlSchemaNamespace = legacyParserObj.ContractXmlNS;

                #region Non functioning code 14th July
                //namespacesBeingEdited.DataEntityNamespace = valuePrefix + legacyParserObj.ContractDataEntityNS;
                //namespacesBeingEdited.DataEntityRootNamespace = valuePrefix + legacyParserObj.ContractDataEntityRootNS;
                //// Figee: July 13th Got the culprit!!!!
                ////namespacesBeingEdited.SerializerNamespace = valuePrefix + valuePrefix + legacyParserObj.ContractSerializerNS;
                //namespacesBeingEdited.SerializerNamespace = valuePrefix + legacyParserObj.ContractSerializerNS;
                //namespacesBeingEdited.SerializerRootNamespace = valuePrefix + legacyParserObj.ContractSerializerRootNS;
                //namespacesBeingEdited.HostAccessNamespace = valuePrefix + legacyParserObj.ContractHostAccessNS;
                //namespacesBeingEdited.HostAccessRootNamespace = valuePrefix + legacyParserObj.ContractHostAccessRootNS;
                //namespacesBeingEdited.XmlSchemaNamespace = valuePrefix + legacyParserObj.ContractXmlNS;
                #endregion
                originalProjectNamespaces = Clone(namespacesBeingEdited);
            }
                
            if (isModelObject == "none")
            {
                //fill XML namespace
                txtProjectPrefix.TextChanged -= txtProjectPrefix_TextChanged;
                txtProjectPrefix.Text = editedProject.ProjectPrefix;
                //to remove the error in XML file wherein the initial value of project Prefix is not saved
                if (txtProjectPrefix.Text != "" && txtEntityNameSpace.Text == "" && txtEntityRootNamespace.Text == "")
                {
                    valuePrefix = txtProjectPrefix.Text;
                }

                txtProjectPrefix.TextChanged += txtProjectPrefix_TextChanged;

                //fill data entity namespace
                //txtEntityNameSpace.TextChanged -= txtEntityNameSpace_TextChanged;                   
                txtEntityNameSpace.Text = txtProjectPrefix.Text + originalProjectNamespaces.DataEntityNamespace;
                //txtEntityNameSpace.TextChanged += txtEntityNameSpace_TextChanged;

                //fill data entity root namespace
                //txtEntityRootNamespace.TextChanged -= txtEntityRootNamespace_TextChanged;
                txtEntityRootNamespace.Text = txtProjectPrefix.Text + originalProjectNamespaces.DataEntityRootNamespace;
                //txtEntityRootNamespace.TextChanged += txtEntityRootNamespace_TextChanged;

                //fill host access namespace
                //txtHostAccessNamespace.TextChanged -= txtHostAccessNamespace_TextChanged;
                txtHostAccessNamespace.Text = txtProjectPrefix.Text + originalProjectNamespaces.HostAccessNamespace;
                //txtHostAccessNamespace.TextChanged += txtHostAccessNamespace_TextChanged;

                //fill host access root namespace
                //txtHostAccessRootNameSpace.TextChanged -= txtHostAccessRootNameSpace_TextChanged;
                txtHostAccessRootNameSpace.Text = txtProjectPrefix.Text + originalProjectNamespaces.HostAccessRootNamespace;
                //txtHostAccessRootNameSpace.TextChanged += txtHostAccessRootNameSpace_TextChanged;

                //fill serializer namespace
                //txtSerializerNamespace.TextChanged -= txtSerializerNamespace_TextChanged;
                txtSerializerNamespace.Text = txtProjectPrefix.Text + originalProjectNamespaces.SerializerNamespace;
                //txtSerializerNamespace.TextChanged += txtSerializerNamespace_TextChanged;

                //fill serializer root namespace
                //txtSerializerRootNamespace.TextChanged -= txtSerializerRootNamespace_TextChanged;
                txtSerializerRootNamespace.Text = txtProjectPrefix.Text + originalProjectNamespaces.SerializerRootNamespace;
                //txtSerializerRootNamespace.TextChanged += txtSerializerRootNamespace_TextChanged;
            }
        }

        # region unusedCode
        /* this code is no longer in use since the text boxes are now unditable
        private void txtXmlSchemaNamespace_TextChanged(object sender, EventArgs e)
        {
            IsDirty = true;
            namespacesBeingEdited.XmlSchemaNamespace = txtXmlSchemaNamespace.Text;
        }

        private void txtEntityNameSpace_TextChanged(object sender, EventArgs e)
        {
            //if (calledPopulate == true)
            //{
            //    calledPopulate = false;
            //}
            //change
            dataEntityNamespace = txtEntityNameSpace.Text.Substring(txtProjectPrefix.Text.Length);

            IsDirty = true;
            namespacesBeingEdited.DataEntityNamespace = txtEntityNameSpace.Text;
            if (!((txtEntityNameSpace.Text.Length > 0 && txtEntityNameSpace.Text[txtEntityNameSpace.Text.Length - 1] == '{')
                ||
                (txtEntityNameSpace.Text.Length > 1 && txtEntityNameSpace.Text[txtEntityNameSpace.Text.Length - 2] == '{')))
            {

                if (isClausesBeingEdited)
                {
                    Entities.GenericCollection<Entities.ModelObjectModule> modulesToBeModified;
                    modulesToBeModified = editedProject.ModelObjectModules;
                    foreach (Entities.Module module in modulesToBeModified)
                    {
                        module.DataEntityNamespace = FormatNamespace(txtEntityNameSpace.Text, module);
                    }
                }
                else
                {
                    Entities.GenericCollection<Entities.ContractModule> modulesToBeModified;
                    modulesToBeModified = editedProject.ContractModules;
                    foreach (Entities.Module module in modulesToBeModified)
                    {
                        module.DataEntityNamespace = FormatNamespace(txtEntityNameSpace.Text, module);
                    }
                }
            }
        }

        private void txtSerializerNamespace_TextChanged(object sender, EventArgs e)
        {
            //change
            serNamespace = txtSerializerNamespace.Text.Substring(txtProjectPrefix.Text.Length);

            IsDirty = true;
            namespacesBeingEdited.SerializerNamespace = txtSerializerNamespace.Text;

            if (!((txtSerializerNamespace.Text.Length > 0 && txtSerializerNamespace.Text[txtSerializerNamespace.Text.Length - 1] == '{')
                    ||
                (txtSerializerNamespace.Text.Length > 1 && txtSerializerNamespace.Text[txtSerializerNamespace.Text.Length - 2] == '{')))
            {
                if (isClausesBeingEdited)
                {
                    Entities.GenericCollection<Entities.ModelObjectModule> modulesToBeModified;
                    modulesToBeModified = editedProject.ModelObjectModules;
                    foreach (Entities.Module module in modulesToBeModified)
                    {
                        module.SerializerNamespace = FormatNamespace(txtSerializerNamespace.Text, module);
                    }
                }
                else
                {
                    Entities.GenericCollection<Entities.ContractModule> modulesToBeModified;
                    modulesToBeModified = editedProject.ContractModules;
                    foreach (Entities.Module module in modulesToBeModified)
                    {
                        module.SerializerNamespace = FormatNamespace(txtSerializerNamespace.Text, module);
                    }
                }
            }
        }
         
        private void txtHostAccessRootNameSpace_TextChanged(object sender, EventArgs e)
        {
            //change
            hostAccessRootNamespace = txtHostAccessRootNameSpace.Text.Substring(txtProjectPrefix.Text.Length);

            IsDirty = true;
            namespacesBeingEdited.HostAccessRootNamespace = txtHostAccessRootNameSpace.Text;
        }

        private void txtHostAccessNamespace_TextChanged(object sender, EventArgs e)
        {
            //change
            hostAccessNamespace = txtHostAccessNamespace.Text.Substring(txtProjectPrefix.Text.Length);

            IsDirty = true;
            namespacesBeingEdited.HostAccessNamespace = txtHostAccessNamespace.Text;
        }

        private void txtSerializerRootNamespace_TextChanged(object sender, EventArgs e)
        {
            //change
            serRootNamespace = txtSerializerRootNamespace.Text.Substring(txtProjectPrefix.Text.Length);

            IsDirty = true;
            namespacesBeingEdited.SerializerRootNamespace = txtSerializerRootNamespace.Text;
        }
        
        private void txtEntityRootNamespace_TextChanged(object sender, EventArgs e)
        {
            //change
            dataEntityRootNamespace = txtEntityRootNamespace.Text.Substring(txtProjectPrefix.Text.Length);

            IsDirty = true;
            namespacesBeingEdited.DataEntityRootNamespace = txtEntityRootNamespace.Text;
        }
        
        */
        # endregion

        /// <summary>
        /// Replace the place holder #ModuleName# in the namespace with the module name
        /// </summary>
        /// <param name="strToBeFormatted"></param>
        /// <param name="module"></param>
        /// <returns>Namespace with value of module name filled</returns>
        private string FormatNamespace(string strToBeFormatted, Entities.Module module)
        {
            const string ModuleName = "#ModuleName#";
            while (strToBeFormatted.IndexOf(ModuleName) > -1)
            {
                strToBeFormatted = strToBeFormatted.Replace(ModuleName, module.Name);
            }
            return strToBeFormatted;
        }
        
        /// <summary>
        /// Call btnSave_Click event
        /// </summary>
        protected override void Save()
        {
            btnSave_Click(this, new EventArgs());
            IsDirty = false;
        }

        /// <summary>
        /// Call btnCancel_Click event
        /// </summary>
        protected override void Cancel()
        {
            btnCancel_Click(this, new EventArgs());
            IsDirty = false;
        }

        static public bool textChangedEventCalled;        
        static public string duplicatePrefixValue;

        /// <summary>
        /// Preserve the value of project prefix for further use
        /// </summary>
        /// <param name="valuePassed"></param>
        public void duplicatePrefixValueFunction(string valuePassed)
        {
            duplicatePrefixValue = valuePassed;
        }

        /// <summary>
        /// event occurs when project prefix is changed. Prefix the project prefix value to rest of the namespaces.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void txtProjectPrefix_TextChanged(object sender, EventArgs e)
        {
            //preserve the project prefix value for future use
            duplicatePrefixValueFunction(txtProjectPrefix.Text);

            textChangedEventCalled = true;
            projectPrefix = txtProjectPrefix.Text;
            IsDirty = true;

            LegacyParser legacyParserObj = new LegacyParser();

            string contractXmlNamespace = legacyParserObj.ContractXmlNS;
            
            //when the project editor is loaded for the first time, get all the namespace values from app.config
            if (dataEntityNamespace == null || dataEntityRootNamespace == null || hostAccessNamespace == null || 
                hostAccessRootNamespace == null || serNamespace == null || serRootNamespace == null)
            {   
                //for contract project editor
                if (namespacesBeingEdited.XmlSchemaNamespace.Contains(contractXmlNamespace))
                {
                    dataEntityNamespace = legacyParserObj.ContractDataEntityNS;
                    dataEntityRootNamespace = legacyParserObj.ContractDataEntityRootNS;
                    hostAccessNamespace = legacyParserObj.ContractHostAccessNS;
                    hostAccessRootNamespace = legacyParserObj.ContractHostAccessRootNS;
                    serNamespace = legacyParserObj.ContractSerializerNS;
                    serRootNamespace = legacyParserObj.ContractSerializerRootNS;
                }

                //for model object project editor
                else
                {
                    dataEntityNamespace = legacyParserObj.ModelObjDataEntityNS;
                    dataEntityRootNamespace = legacyParserObj.ModelObjDataEntityRootNS;                    
                    serNamespace = legacyParserObj.ModelObjSerializerNS;
                    serRootNamespace = legacyParserObj.ModelObjSerializerRootNS;
                }                
            }

            //fill controls with the project prefix + corresponding namespace value
            txtEntityNameSpace.Text = projectPrefix.Trim() + dataEntityNamespace.Trim();
            txtEntityRootNamespace.Text = projectPrefix.Trim() + dataEntityRootNamespace.Trim();
            txtSerializerNamespace.Text = projectPrefix.Trim() + serNamespace.Trim();
            txtSerializerRootNamespace.Text = projectPrefix.Trim() + serRootNamespace.Trim();
            //for contract fill host access namespaces too
            if (namespacesBeingEdited.XmlSchemaNamespace.Contains(contractXmlNamespace))
            {
                txtHostAccessNamespace.Text = projectPrefix.Trim() + hostAccessNamespace.Trim();
                txtHostAccessRootNameSpace.Text = projectPrefix.Trim() + hostAccessRootNamespace.Trim();
            }            
        }

        public static string valuePrefix;

        /// <summary>
        /// when save button of project editor is clicked, save all modofications
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void btnSave_Click(object sender, EventArgs e)
        {            
            valuePrefix = txtProjectPrefix.Text;
            editedProject.ProjectPrefix = valuePrefix;            
            
            if (Modified != null)
            {
                ModifiedEventArgs modifiedArgs = new ModifiedEventArgs();

                //check for uniqueness of namespaces : Dont delete the commented code
                //uniqueness of namespaces not to be forced.
                /*ArrayList namespacesList = new ArrayList(7);
                namespacesList.Add(namespacesBeingEdited.DataEntityNamespace);
                namespacesList.Add(namespacesBeingEdited.DataEntityRootNamespace);
                namespacesList.Add(namespacesBeingEdited.HostAccessNamespace);
                namespacesList.Add(namespacesBeingEdited.HostAccessRootNamespace);
                namespacesList.Add(namespacesBeingEdited.SerializerNamespace);
                namespacesList.Add(namespacesBeingEdited.SerializerRootNamespace);
                namespacesList.Add(namespacesBeingEdited.XmlSchemaNamespace);

                Hashtable htNamespaces = new Hashtable();
                foreach (string namespaceModified in namespacesList)
                {
                    if (namespaceModified != null)
                    {
                        if (htNamespaces.Contains(namespaceModified))
                        {
                            //if namespaces are not unique - show error message
                            MessageBox.Show("Namespaces provided must be unique.", "ERROR");
                            return;
                        }
                        else
                        {
                            htNamespaces.Add(namespaceModified, namespaceModified);
                        }
                    }
                }*/

                //save modified data
                modifiedArgs.ModifiedNameSpaces = namespacesBeingEdited;
                Modified(this, modifiedArgs);
            }
            originalProjectNamespaces = namespacesBeingEdited;
            IsDirty = false;
        }
        
        public bool btnCancelClicked;

        /// <summary>
        /// If cancel button is clicked, revert to last saved data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            string isModelObject = "none";
            //if the project prefix has been changed after last saving it
            if (valuePrefix != projectPrefix)
            {
                //if the last saved was null then project prefix is null
                if (valuePrefix == null)
                {
                    projectPrefix = string.Empty;
                    editedProject.ProjectPrefix = string.Empty;
                }

                else
                {
                    projectPrefix = valuePrefix.ToString();
                    editedProject.ProjectPrefix = valuePrefix.ToString();
                }
            }

            btnCancelClicked = true;
            IsDirty = false;

            //
            if (originalProjectNamespaces == null)
            {
                namespacesBeingEdited.DataEntityNamespace = string.Empty;
                namespacesBeingEdited.DataEntityRootNamespace = string.Empty;
                namespacesBeingEdited.HostAccessNamespace = string.Empty;
                namespacesBeingEdited.HostAccessRootNamespace = string.Empty;
                namespacesBeingEdited.SerializerNamespace = string.Empty;
                namespacesBeingEdited.SerializerRootNamespace = string.Empty;
                namespacesBeingEdited.XmlSchemaNamespace = string.Empty;
            }
            else
            {
                namespacesBeingEdited = originalProjectNamespaces;
            }
            Populate(isModelObject);
        }
        
        /// <summary>
        /// for any key pressed in project prefix text box, check its validity
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtProjectPrefix_KeyPress(object sender, KeyPressEventArgs e)
        {
            char keyChar = e.KeyChar;

            //check if the character just entered is valid or not
            if (!isValidChar(keyChar))
            {
                //show the error message and don't allow the invalid character
                MessageBox.Show("The character entered is not valid", "Legacy Workbench",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Handled = true;
            }
        }

        /// <summary>
        /// checks if the character is valid
        /// </summary>
        /// <param name="keyChar"></param>
        /// <returns>true/false</returns>
        private bool isValidChar(char keyChar)
        {
            //if A-Z or a-z or 0-9 or .(dot) or _(underscore) or backspace or del
            return ((keyChar >= 65 && keyChar <= 90) || (keyChar >= 97 && keyChar <= 122)
                || (keyChar >= 48 && keyChar <= 57) || keyChar == 46 || keyChar == 95
                || keyChar == 8 || keyChar == 127);
        }

        private void ProjectEditor_Load(object sender, EventArgs e)
        {

        }
    }
}
