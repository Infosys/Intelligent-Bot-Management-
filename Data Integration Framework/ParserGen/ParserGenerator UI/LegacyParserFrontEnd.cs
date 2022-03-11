using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;


using Infosys.Solutions.CodeGeneration.Framework;

using Infosys.Lif.LegacyParser;
using Infosys.Lif.LegacyParser.Interfaces;
using Infosys.Lif.LegacyParser.UI.Configurations;
using Infosys.Lif.LegacyParser.LanguageConverters;
using ContentProviders = Infosys.Lif.LegacyParser.ContentProviders;

/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * This file holds the front end for the tool
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/



namespace Infosys.Lif.LegacyParser.UI
{
    /// <summary>
    /// The front end for the legacy parser tool will be an object of this type.
    /// </summary>
    public partial class LegacyParserFrontEnd : Form
    {
        /// <summary>
        /// The configuration settings.
        /// Will be loaded in the Form_Load method and on change of 
        /// the configuration file.
        /// </summary>
        LegacyParserConfigurations configSettings;

        /// <summary>
        /// The start up path for the application. 
        /// This is where some of the files required by this 
        /// tool will be looked for.
        /// </summary>
        string appPath;

        /// <summary>
        /// The reference to the object which will be used to convert 
        /// the language specific host programs to our dotNet type
        /// </summary>
        ILanguageConverter languageConverter;

        /// <summary>
        /// The controller which should be used to generate the Serializers and Data Entities
        /// </summary>
        IHostSpecificController hostTypeController;

        /// <summary>
        /// Reference to the storage implementation which should be used to persist 
        /// the current project.
        /// </summary>
        IStorageImplementation storageImplementaion;

        /// <summary>
        /// The files which will be parsed.
        /// </summary>
        string[] filesToBeRead;

        /// <summary>
        /// The destination where the generated files should be placed.
        /// </summary>
        string destinationPath;

        /// <summary>
        /// The current project the user is working on.
        /// </summary>
        Project currProject = new Project();

        /// <summary>
        /// The default module to which any parsed entity will be added.
        /// </summary>
        LegacyParserModule ungroupedModule = new LegacyParserModule();

        /// <summary>
        /// The module the developer is working with.
        /// </summary>
        LegacyParserModule selectedModule;
        /// <summary>
        /// Default constructor.
        /// </summary>
        public LegacyParserFrontEnd()
        {
            Application.ThreadException
                += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
            InitializeComponent();




            // Read the configuration file.
            configSettings = ConfigWrapper.GetConfiguration();


            if (configSettings == null)
            {
                ShowError("The configuration file seems to be incorrect. Please correct this by utilizing the Configuration dialog.");
                // Call the load configurations event handler so that the 
                // configuration window can be shown.
                ChangeConfigurations(this, new EventArgs());
                configSettings = ConfigWrapper.GetConfiguration();
                if (configSettings == null)
                {
                    ShowError("The configuration file still seems to be incorrect. Please correct this and then start the application.");
                    return;
                }
            }
            // Perform required configuration handling.
            HandleChangedConfigs();

            // Set the application's path 
            appPath = Application.StartupPath;
        }

        /// <summary>
        /// Event Handler for the Application's ThreadException event.
        /// This is handled to gracefully close the application after 
        /// logging the exception without allowing the app to crash.
        /// </summary>
        /// <param name="sender">The object raising the event</param>
        /// <param name="e">
        /// Arguments for the raised event. 
        /// This holds the exception raised as a property.
        /// </param>
        private void Application_ThreadException(object sender,
            System.Threading.ThreadExceptionEventArgs e)
        {
            // Log the exception into the required repository
            LogException(e.Exception);
            // Display an user friendly message.
            ShowError("Internal exception occurred and the application " +
                "had to close. The exception has been logged to event log");
            // Quit the app.
            Application.Exit();
        }

        /// <summary>
        /// Called to log the exception into the required repository.
        /// </summary>
        /// <param name="exc">Exception which needs to be logged.</param>
        private static void LogException(Exception exceptionToBeLogged)
        {
            string eventLogSource = "Legacy Parser Tool";

            // Write the entry to event log
            System.Diagnostics.EventLog.WriteEntry(eventLogSource,
                exceptionToBeLogged.ToString(),
                System.Diagnostics.EventLogEntryType.Error);
        }

        /// <summary>
        /// Called when the user clicks on the Load menu item.
        /// </summary>
        /// <param name="sender">the object which raised the event</param>
        /// <param name="e">event args</param>
        private void LoadProject_Click(object sender, EventArgs e)
        {
            // Call the storage implementation's retrieve method.
            Project proj = storageImplementaion.Retrieve();
            // If the project was loaded successfully.
            if (proj != null)
            {
                // store a reference to the loaded project.
                currProject = proj;

                // bind the current project to the tree.
                BindTreeToProject(currProject);
                if (currProject.Modules.Count > 0)
                {
                    btnGenerate.Enabled = true;
                }
            }
        }

        /// <summary>
        /// Called on the load of the form.
        /// </summary>
        /// <param name="sender">The object raising which raised this event.</param>
        /// <param name="e">Event arguments to this event handler</param>
        private void LegacyParserFrontEnd_Load(object sender, EventArgs e)
        {

            // Add event handlers to handle changes in configuration files.
            ConfigWrapper.ConfigChanged +=
                new EventHandler(ConfigChangedEventHandler);

            // Name the ungrouped module.
            ungroupedModule.Name = "Ungrouped";

            // Add the ungrouped module so the user sees a ready to add interface.
            currProject.Modules.Add(ungroupedModule);

            // Refresh the tree to reflect changes.
            BindTreeToProject(currProject);

            // Keep the modules tree in expanded state.
            modulesListTree.ExpandAll();

            // Handle data item changes in the module editor control.
            moduleEditor.DataChanged +=
                new EventHandler<ChangedEventArgs<LegacyParserModule>>(moduleEditor_DataChanged);

            // Handle data item changes in the entity editor control.
            entityEditor.DataChanged +=
                new EventHandler<ChangedEventArgs<Entity>>(entityEditor_DataChanged);
        }

        /// <summary>
        /// Handles the changes in the config file.
        /// </summary>
        /// <param name="sender">Obejct raising the event</param>
        /// <param name="e">Event parameters.</param>
        void ConfigChangedEventHandler(object sender, EventArgs e)
        {
            // Read the config file.
            configSettings = ConfigWrapper.GetConfiguration();

            // Perform config file based operations.
            HandleChangedConfigs();
        }

        /// <summary>
        /// Should be called to load up all the classes which are 
        /// dependent on the config file.
        /// </summary>
        private void HandleChangedConfigs()
        {
            //Load the Language-dependent parser    --  Start
            LanguageConverter lngConverter = configSettings.LanguageConverters[configSettings.Language];
            if (lngConverter == null)
            {
                ThrowConfigException("Language converter default name is invalid");
            }
            string typeOfLanguage = lngConverter.Type;
            object objExtension = LoadType(typeOfLanguage);
            if (!(objExtension is ILanguageConverter))
            {
                string errorMessage = typeOfLanguage + " should inherit ILanguageConverter";
                ShowError(errorMessage);
                ThrowException(errorMessage);
            }
            languageConverter = (ILanguageConverter)objExtension;
            //Load the Language-dependent parser    --  End

            //Load the host specific controller     --  Start
            HostTypeController hstController = configSettings.HostTypes[configSettings.HostType];
            if (hstController == null)
            {
                ThrowConfigException("Host controller default name is invalid");
            }
            string typeOfHost = hstController.Type;
            objExtension = LoadType(typeOfHost);
            if (!(objExtension is IHostSpecificController))
            {
                string errorMessage = typeOfLanguage + " should inherit IHostSpecificController";
                ShowError(typeOfLanguage + " should inherit IHostSpecificController");
                ThrowException(errorMessage);
            }
            hostTypeController = (IHostSpecificController)objExtension;
            //Load the host specific controller     --  End

            //Load the Storage specific implementation  -- Start
            StorageImplementation storageClass = configSettings.StorageImplementations[configSettings.StorageType];
            if (storageClass == null)
            {
                ThrowConfigException("Storage Implementation default name is invalid");
            }
            string storageType = storageClass.Type;
            objExtension = LoadType(storageType);
            if (!(objExtension is IStorageImplementation))
            {
                string errorMessage = typeOfLanguage + " should inherit IStorageImplementation";
                ShowError(typeOfLanguage + " should inherit IStorageImplementation");
                ThrowException(errorMessage);
            }
            storageImplementaion = (IStorageImplementation)objExtension;
            //Load the Storage specific implementation  -- End

        }

        /// <summary>
        /// This method should be used to create an instance of any type
        /// </summary>
        /// <param name="typeToBeLoaded">Full Name of the type which should 
        /// be loaded.</param>
        /// <returns>An instance of the type requested for.</returns>
        private object LoadType(string typeToBeLoaded)
        {
            // Error checking   -   Start
            if (typeToBeLoaded == null || typeToBeLoaded.Length == 0)
            { ThrowConfigException("Type cannot be null in the configuration file"); }
            // Error checking   -   End

            if (typeToBeLoaded != null && typeToBeLoaded.Length > 0)
            {
                Type typeToBeActivated = Type.GetType(typeToBeLoaded);
                if (typeToBeActivated == null)
                { ThrowConfigException(typeToBeLoaded + " not found"); }

                object instanceOfObject = null;
                //Use reflection to create an instance of the type asked for
                if (typeToBeActivated != null)
                {
                    instanceOfObject = Activator.CreateInstance(typeToBeActivated);
                }
                //return the instance created.

                return instanceOfObject;
            }
            return null;
        }

        /// <summary>
        /// Used to throw exceptions.
        /// </summary>
        /// <param name="exceptionMessage"></param>
        private static void ThrowException(string exceptionMessage)
        {
            ShowError(exceptionMessage);
            throw new LegacyParserException(exceptionMessage);
        }

        private void ThrowConfigException(string exceptionMessage)
        {
            ShowError(exceptionMessage);
            if (MessageBox.Show("The configuration file contains invalid assembly/type names.\n Click OK to modify the configurations. Cancel to quit the application", "Invalid Configuration", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                ChangeConfigurations(this, new EventArgs());

            }
            else
            {
                Application.ExitThread();
            }
        }

        /// <summary>
        /// Handles change in entity data.
        /// </summary>
        /// <param name="sender">The control which raises the event</param>
        /// <param name="e">Event arguments</param>
        void entityEditor_DataChanged(object sender, ChangedEventArgs<Entity> e)
        {
            if (e.PropertyChanged.Equals("EntityName"))
            {
                //BindTreeToProject(currProject);
                modulesListTree.SelectedNode.Text = e.NewValue.EntityName;
                //modulesListTree.SelectedNode = parentNode.Nodes[e.NewValue.EntityName];
                //parentNode.Expand();
            }
        }


        /// <summary>
        /// Handles changes in the Module done using the Module Editor control
        /// </summary>
        /// <param name="sender">The module editor control which raises the event</param>
        /// <param name="e">Event arguments</param>
        void moduleEditor_DataChanged(object sender, ChangedEventArgs<LegacyParserModule> e)
        {
            if (e.PropertyChanged.Equals("ModuleName"))
            {
                //BindTreeToProject(currProject);
                modulesListTree.SelectedNode.Text = e.NewValue.Name;
            }
        }

        /// <summary>
        /// Selection of nodes in the tree will result in calls to this method.
        /// Event handler for AfterSelect event of the tree.
        /// </summary>
        /// <param name="sender">The tree which raises the event</param>
        /// <param name="e">Event Args</param>
        private void modulesListTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (e.Node.Level)
            {
                case 1:
                    // A module has been selected.
                    selectedModule = FindModule(e.Node.Text);
                    moduleEditor.LoadModule(ref selectedModule);

                    moduleEditor.Visible = true;
                    entityEditor.Visible = false;
                    break;
                case 2:
                    // An entity has been selected.
                    selectedModule = FindModule(e.Node.Parent.Text);
                    string entityName = e.Node.Text;
                    Entity selectedEntity =
                        FindEntityInModule(entityName, selectedModule);
                    entityEditor.LoadEntity(ref selectedEntity);
                    moduleEditor.Visible = false;
                    entityEditor.Visible = true;
                    break;
                default:
                    // The root node has been clicked.
                    moduleEditor.Visible = false;
                    entityEditor.Visible = false;
                    break;
            }
        }

        /// <summary>
        /// Method to locate an entity in module.
        /// </summary>
        /// <param name="entityName">The name of the entity which has to be searched for</param>
        /// <param name="selectedModule">The module in which the entity should be found</param>
        /// <returns>
        /// Returns a reference to the entity if an entity with 
        /// name as entityName is found. Null otherwise.
        /// </returns>
        private static Entity FindEntityInModule(string entityName,
            LegacyParserModule selectedModule)
        {

            // Loop thru the entities of the module and find the entity.
            for (int entityLooper = 0;
                entityLooper < selectedModule.Entities.Count;
                entityLooper++)
            {
                Entity currEntity =
                    selectedModule.Entities[entityLooper];
                // If the entity is found in the module 
                if (currEntity.EntityName.ToLowerInvariant().Equals(
                    entityName.ToLowerInvariant()))
                {
                    // return a reference to the entity
                    return currEntity;
                }
            }
            // The entity was not found in this module, so return null.
            return null;
        }


        /// <summary>
        /// Searches the current project to find a module by name.
        /// </summary>
        /// <param name="moduleName">The name of the module to be found.</param>
        /// <returns>
        /// The module if the module was found with 
        /// a corresponding moduleName. Null, otherwise.
        /// </returns>
        private LegacyParserModule FindModule(string moduleName)
        {
            // Loop the modules in the project and find a module having 
            // Name property as moduleName
            for (int moduleLooper = 0;
                    moduleLooper < currProject.Modules.Count;
                    moduleLooper++)
            {
                if (currProject.Modules[moduleLooper].Name.ToLowerInvariant()
                    .Equals(moduleName.ToLowerInvariant()))
                {
                    // return the module
                    return currProject.Modules[moduleLooper];
                }
            }
            // no module was found in the project, so return null.
            return null;
        }

        /// <summary>
        /// Event handler for click of browse button.
        /// </summary>
        /// <param name="sender">The button which raises the event.</param>
        /// <param name="e">Event Arguments</param>
        private void btnHstFilesBrowse_Click(object sender, EventArgs e)
        {
            openDialog.Filter = "All Files (*.*) |*.*";

            // Popup the OpenFileDialog, with multiple select.
            openDialog.Multiselect = true;
            DialogResult result = openDialog.ShowDialog();

            // If cancel has not been clicked.
            if (result != DialogResult.Cancel)
            {
                // Store the path of the files to be parsed.
                filesToBeRead = openDialog.FileNames;
                // Build up the string which needs to be displayed in the textbox.
                for (int fileListLooper = 0; fileListLooper < filesToBeRead.Length;
                    fileListLooper++)
                {
                    txtHostProgPaths.Text += filesToBeRead[fileListLooper] + ";";
                }
            }
        }

        /// <summary>
        /// Click event handler for the "Browse" button used to decide the destination
        /// for the serializer classes and Data Entities
        /// </summary>
        /// <param name="sender">The button which was clicked</param>
        /// <param name="e">Event Args</param>
        private void btnDestinationBrowse_Click(object sender, EventArgs e)
        {
            destinationDialog.ShowNewFolderButton = true;
            if (txtDestination.Text != null && txtDestination.Text.Length > 0)
            {
                destinationDialog.SelectedPath = txtDestination.Text;
            }
            DialogResult result = destinationDialog.ShowDialog();
            // If cancel was not clicked.
            if (result != DialogResult.Cancel)
            {
                destinationPath = destinationDialog.SelectedPath;
                txtDestination.Text = destinationDialog.SelectedPath;
            }
        }

        /// <summary>
        /// Method to refresh the data shown in the modules tree.
        /// </summary>
        /// <param name="projectToBeDisplayed">
        /// The project which has to be shown as a tree structure
        /// </param>
        private void BindTreeToProject(Project projectToBeDisplayed)
        {
            modulesListTree.Nodes[0].Nodes.Clear();

            // Loop the modules and add each to the root node. 
            // All entities for each module should be added to the corresponding module
            // as a level 2 node.
            for (int moduleLooper = 0;
                moduleLooper < projectToBeDisplayed.Modules.Count;
                moduleLooper++)
            {
                LegacyParserModule currModule =
                    projectToBeDisplayed.Modules[moduleLooper];


                TreeNode node = new TreeNode();
                node.Text = currModule.Name;
                // Loop the entities in the current module.
                for (int entitiesLooper = 0;
                    entitiesLooper < currModule.Entities.Count;
                    entitiesLooper++)
                {
                    Entity currEntity =
                        currModule.Entities[entitiesLooper];

                    TreeNode entityNode = new TreeNode();
                    entityNode.Text = currEntity.EntityName;
                    node.Nodes.Add(entityNode);
                }

                modulesListTree.Nodes[0].Nodes.Add(node);
            }
        }

        /// <summary>
        /// Event handler for click of the "Preview" button.
        /// </summary>
        /// <param name="sender">The object (usually the button) which raises this event</param>
        /// <param name="e">Event arguments</param>
        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (filesToBeRead == null)
            {
                return;
            }

            // Loop through the list of files
            // Build the Entities for each of the files by utilizing the ILanguageConverter 
            // implementation and add the entity to "ungrouped" module.
            for (int filesLooper = 0; filesLooper < filesToBeRead.Length;
                filesLooper++)
            {
                DisplayStatus("Converting input file " + filesToBeRead[filesLooper]);
                Entity newEntity
                    = languageConverter.Translate(filesToBeRead[filesLooper]);
                if (newEntity != null)
                {
                    if (!AddEntityToModule(ungroupedModule, newEntity))
                    {
                        ShowError("Unable to add entity " + newEntity.EntityName + "\n\rPlease check that it has not been already added.");
                    }
                }
                else
                {
                    ShowError("No entities were found in the file " + filesToBeRead[filesLooper] + ". This file will be ignored");
                }
            }
            DisplayStatus(string.Empty);
            // Refresh the tree.
            BindTreeToProject(currProject);
            btnGenerate.Enabled = true;
        }

        private static bool AddEntityToModule(LegacyParserModule module, Entity newEntity)
        {
            for (int entityLooper = 0; entityLooper < module.Entities.Count; entityLooper++)
            {
                if (module.Entities[entityLooper].EntityName.Equals(newEntity.EntityName))
                {
                    return false;
                }
            }
            module.Entities.Add(newEntity);
            return true;
        }

        /// <summary>
        /// Called to persist the current working project. 
        /// This method is an event handler for the "Save" button's click event
        /// </summary>
        /// <param name="sender">The clicked button</param>
        /// <param name="e">Event Args</param>
        private void saveMenuClickHandler(object sender, EventArgs e)
        {
            storageImplementaion.Store(currProject);
        }

        /// <summary>
        /// The event handler for click of a node in the tree. 
        /// We handle only the case of right click of any node. 
        /// </summary>
        /// <param name="sender">The object which raised the event</param>
        /// <param name="e">Event arguments for this event. This also contains the node 
        /// clicked, mouse button which resulted in the click, etc.
        /// </param>
        private void modulesListTree_NodeMouseClick(object sender,
            TreeNodeMouseClickEventArgs e)
        {
            // If the right button was clicked 
            if (e.Button == MouseButtons.Right)
            {
                // Display the appropriate context menu. After selecting the node
                TreeView tree = (TreeView)sender;
                tree.SelectedNode = e.Node;
                switch (e.Node.Level)
                {
                    case 0:
                        rootNodeMenuStrip.Show(tree, e.X, e.Y);
                        break;
                    case 1:
                        moduleMenuStrip.Show(tree, e.X, e.Y);
                        break;
                    case 2:
                        entityMenuStrip.Show(tree, e.X, e.Y);
                        break;
                }
            }
        }
        /// <summary>
        /// Event handler for the click event of the "Generate" button.
        /// </summary>
        /// <param name="sender">The object which raised the event</param>
        /// <param name="e">Event Arguments</param>
        private void btnGenerate_Click(object sender, EventArgs e)
        {
            try
            {
                TreeNode selectedNode = modulesListTree.SelectedNode;
                if (selectedNode == null)
                {
                    ShowError("Please select a module/entity to generate");
                    return;
                }
                LegacyParserModule selectedModule;
                Entity selectedEntity;

                DialogResult shouldServiceDeliveryBeGenerated = MessageBox.Show("Do you want the Service Components to be generated?", "Service Component Generation", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                bool isServiceComponentRequired = true;
                switch (shouldServiceDeliveryBeGenerated)
                {
                    case DialogResult.Cancel:
                        return;
                    case DialogResult.No:
                        isServiceComponentRequired = false;
                        break;
                }

                if (txtDestination.Text == null || txtDestination.Text.Length == 0)
                {
                    btnDestinationBrowse_Click(btnDestinationBrowse, new EventArgs());
                    if (txtDestination.Text == null || txtDestination.Text.Length == 0)
                    {
                        return;
                    }
                }
                destinationPath = txtDestination.Text;
                bool noErrorsOccured = true;

                switch (selectedNode.Level)
                {
                    case 2:
                        // The generate was clicked after selecting a single entity.
                        selectedModule = FindModule(selectedNode.Parent.Text);
                        selectedEntity = FindEntityInModule(selectedNode.Text,
                            selectedModule);
                        noErrorsOccured &= Generate(selectedModule, selectedEntity);
                        if (isServiceComponentRequired)
                        {
                            noErrorsOccured &= GenerateServiceComponents(selectedModule, selectedEntity);
                        }
                        break;
                    case 1:
                        ResetProgressBar();

                        // The generate was clicked after selecting a module.
                        selectedModule = FindModule(selectedNode.Text);
                        progressBar.Maximum = selectedModule.Entities.Count + 3;
                        if (isServiceComponentRequired)
                        {
                            noErrorsOccured &= GenerateServiceComponents(selectedModule, null);
                        }

                        // Find the module which was selected, by using the current project.
                        // Call the generate for each entity in this module.
                        for (int entityCounter = 0;
                            entityCounter < selectedModule.Entities.Count;
                            entityCounter++)
                        {
                            noErrorsOccured &= Generate(selectedModule, selectedModule.Entities[entityCounter]);
                            IncrementProgressBar();
                        }
                        if (selectedModule.Entities.Count > 0)
                        {
                            // Used to generate the project files required to open this solution in Visual Studio

                            // Used to generate the Data Entity project's proj file. -- Start
                            ContentProviders.Vs2005Project proj =
                                new ContentProviders.Vs2005Project(currProject,
                                ContentProviders.Vs2005Project.ProjectType.DataEntityProject);
                            proj.ContentTemplate =
                                Template.FromFile(appPath + @"\Templates\DataEntityProjectTemplateVS2005.txt");
                            string strProjFileContents = proj.GenerateContent();


                            WriteToFile(CreateDirectory("DataEntities\\Code")
                                + "\\" + currProject.DataEntityRootNamespace + ".csproj", strProjFileContents);
                            IncrementProgressBar();
                            // Used to generate the Data Entity project's proj file. -- End


                            // Used to generate the Serializer project's proj file.     --  Start
                            proj = new ContentProviders.Vs2005Project(currProject, ContentProviders.Vs2005Project.ProjectType.SerializerProject);

                            proj.ContentTemplate = Template.FromFile(appPath + @"\Templates\SerializerProjectTemplateVS2005.txt");

                            strProjFileContents = proj.GenerateContent();
                            WriteToFile(CreateDirectory("Serializers") + "\\" + currProject.SerializerRootNamespace + ".csproj", strProjFileContents);
                            IncrementProgressBar();
                            // Used to generate the Serializer project's proj file.     --  End
                            if (isServiceComponentRequired)
                            {
                                proj = new ContentProviders.Vs2005Project(currProject, ContentProviders.Vs2005Project.ProjectType.ServiceComponentProject);
                                proj.ContentTemplate = Template.FromFile(appPath + @"\Templates\ServiceComponents\ServiceComponentProject.csproj");
                                strProjFileContents = proj.GenerateContent();
                                WriteToFile(CreateDirectory("ServiceComponents") + "\\" + currProject.ServiceComponentRootNameSpace + ".csproj", strProjFileContents);

                                //////////////proj = new ContentProviders.Vs2005Project(currProject, ContentProviders.Vs2005Project.ProjectType.UnitTestCaseProject);
                                //////////////proj.ContentTemplate = Template.FromFile(appPath + @"\Templates\UnitTestCases\UnitTestCaseProject.csproj");
                                //////////////strProjFileContents = proj.GenerateContent();
                                //////////////WriteToFile(CreateDirectory("UnitTestCases") + "\\" + currProject.UnitTestCasesRootNameSpace + ".csproj", strProjFileContents);

                            }

                            // Used to generate the solution file which incorporates the above 2 proj files    --  Start
                            proj = new ContentProviders.Vs2005Project(currProject, ContentProviders.Vs2005Project.ProjectType.SolutionType);
                            proj.ContentTemplate = Template.FromFile(appPath + @"\Templates\SolutionTemplateVS2005.txt");
                            proj.IncludeServiceComponentsInSolution = isServiceComponentRequired;
                            strProjFileContents = proj.GenerateContent();
                            WriteToFile(CreateDirectory("") + @"\LIFGeneratedSolution.sln", strProjFileContents.TrimStart());
                            IncrementProgressBar();
                            // Used to generate the solution file which incorporates the above 2 proj files    --  End

                            // Used to generate the config files for the Legacy Facade and the Legacy Integrator -- Start
                            noErrorsOccured &= GenerateConfigFiles(selectedModule);

                            // Used to generate the config files for the Legacy Facade and the Legacy Integrator -- End

                        }
                        else
                        {
                            ShowError("Please add entities by previewing before clicking Generate");
                            // No entities present.
                            noErrorsOccured = false;
                        }
                        break;
                    default:
                        // Root node was selected.
                        ShowError("Please select a Entity/Module before clicking Generate");
                        return;
                }


                if (noErrorsOccured)
                {
                    // Copy the aseemblies that the project references  --  Start
                    string parserReferences = CreateDirectory("References");
                    System.IO.FileInfo fileInfo = new FileInfo(appPath + @"\Infosys.Lif.Serializer.Framework.dll");
                    fileInfo = fileInfo.CopyTo(parserReferences + @"\Infosys.Lif.Serializer.Framework.dll", true);

                    fileInfo = new FileInfo(appPath + @"\Infosys.Lif.LegacyParameters.dll");
                    fileInfo = fileInfo.CopyTo(parserReferences + @"\Infosys.Lif.LegacyParameters.dll", true);

                    if (isServiceComponentRequired)
                    {
                        fileInfo = new FileInfo(appPath + @"\Infosys.Lif.LegacyFacade.dll");
                        fileInfo = fileInfo.CopyTo(parserReferences + @"\Infosys.Lif.LegacyFacade.dll", true);
                    }
                    // Copy the aseemblies that the project references  --  End

                    // Display completion message.
                    DisplayStatus("Completed generation of parsers and data entities without errors");
                    MessageBox.Show("Generated successfully to " + txtDestination.Text,
                        "Legacy Parser Information", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                }
                else
                {
                    DisplayStatus("There were errors while generating the serializers and data entities.");
                }
                ResetProgressBar();
            }
            catch (Exception exc)
            {
                LogException(exc);
                ShowError("Errors occured while generating the serializers");
            }
        }

        private bool GenerateConfigFiles(LegacyParserModule selectedModule)
        {
            if (legacyFacadeConfigSettings == null)
            {
                legacyFacadeConfigSettings = new Infosys.Lif.LegacyConfig.LegacySettings();
            }
            foreach (Entity entity in selectedModule.Entities)
            {
                Infosys.Lif.LegacyConfig.Service service = new Infosys.Lif.LegacyConfig.Service();
                service.RegionName = "DEFAULT";
                service.SerializerClass = string.Empty;
                service.SerializerType = string.Empty;
                service.ServiceName = entity.EntityName;
                legacyFacadeConfigSettings.Services.ServiceCollection.Add(service);
            }
            if (legacyIntegratorSettings == null)
            {
                legacyIntegratorSettings = new Infosys.Lif.LegacyIntegratorService.LISettings();
                legacyIntegratorSettings.HIS.DllPath = string.Empty;
                legacyIntegratorSettings.HIS.EnableTrace = string.Empty;
                legacyIntegratorSettings.HIS.TypeName = string.Empty;
            }
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(legacyFacadeConfigSettings.GetType());
            string pathToConfigFiles = CreateDirectory("Config");
            System.IO.FileStream fileStream = new FileStream(pathToConfigFiles + "\\" + "LegacyFacade.config", FileMode.OpenOrCreate);
            serializer.Serialize(fileStream, legacyFacadeConfigSettings);

            fileStream.Close();

            serializer = new System.Xml.Serialization.XmlSerializer(legacyIntegratorSettings.GetType());
            fileStream = new FileStream(pathToConfigFiles + "\\" + "LegacyIntegrator.config", FileMode.OpenOrCreate);
            serializer.Serialize(fileStream, legacyIntegratorSettings);

            fileStream.Close();


            if (System.IO.File.Exists(Application.StartupPath + "\\Templates\\app.config"))
            {
                System.IO.File.Copy(Application.StartupPath + "\\Templates\\app.config", CreateDirectory("") + "\\app.config", true);
            }
            return true;
        }

        private bool GenerateServiceComponents(LegacyParserModule selectedModule, Entity selectedEntity)
        {
            try
            {
                // The location where the data entities xsd files should be placed after generation.
                // This is the relative path to the location selected by the user.
                string xsdFolderPath = "ServiceComponents\\" + selectedModule.Name;
                xsdFolderPath = CreateDirectory(xsdFolderPath);

                // The xsd file which should be generated.
                string xsdFileToBeGenerated = xsdFolderPath + "\\"
                    + "ServiceComponent.cs";

                // Calling the host controller to build the data entity.
                string strXsd =
                    hostTypeController.BuildServiceComponents(selectedEntity, selectedModule);

                // Write the generated xsd content to the file
                WriteToFile(xsdFileToBeGenerated, strXsd);



                //////////// The location where the data entities xsd files should be placed after generation.
                //////////// This is the relative path to the location selected by the user.
                //////////xsdFolderPath = "UnitTestCases\\" + selectedModule.Name;
                //////////xsdFolderPath = CreateDirectory(xsdFolderPath);

                //////////// The xsd file which should be generated.
                //////////xsdFileToBeGenerated = xsdFolderPath + "\\"
                //////////    + "TestCases.cs";

                //////////// Calling the host controller to build the data entity.
                //////////strXsd = hostTypeController.BuildUnitTestCases(selectedEntity,
                //////////    selectedModule);

                //////////// Write the generated xsd content to the file
                //////////WriteToFile(xsdFileToBeGenerated, strXsd);




                return true;
            }
            catch (Exception exc)
            {
                LogException(exc);
                return false;
            }

        }



        /// <summary>
        /// Used to display a message in the status bar.
        /// </summary>
        /// <param name="message">The message which should be displayed in the status bar</param>
        void DisplayStatus(string message)
        {
            statusBar.Text = message;
        }


        /// <summary>
        /// Used to reset the progress bar to initial values
        /// </summary>
        void ResetProgressBar()
        {
            progressBar.Visible = true;
            progressBar.Value = 0;
            progressBar.Minimum = 0;
        }

        /// <summary>
        /// Method to increment the progress indicated by the progress bar.
        /// </summary>
        void IncrementProgressBar()
        {
            progressBar.Value++;
        }
        /// <summary>
        /// Method to display an error which occured in the application.
        /// </summary>
        /// <param name="errorMessage">
        /// Error message which needs to be shown to the user.
        /// </param>
        static void ShowError(string errorMessage)
        {
            MessageBox.Show(errorMessage,
                "Legacy Parser Error Window",
                MessageBoxButtons.OK, MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);
        }

        /// <summary>
        /// Method to generate the parser and data entities for a given 
        /// module's entity.
        /// </summary>
        /// <param name="selectedModule">
        /// The module to which the entity belongs
        /// </param>
        /// <param name="selectedEntity">
        /// The entity for which the parsers and the data entities need to be generated.
        /// </param>
        /// <returns>False, if not successfull, true if successful.</returns>
        private bool Generate(LegacyParserModule selectedModule, Entity selectedEntity)
        {
            try
            {
                bool noErrorsOccured = true;
                // The location where the data entities xsd files should be placed after generation.
                // This is the relative path to the location selected by the user.
                string xsdFolderPath = "DataEntities\\XSD";
                xsdFolderPath = CreateDirectory(xsdFolderPath);

                // The path relative to the user selected path, where the data entities code file 
                // should be placed after generation.
                string dataEntitiesClassPath = "DataEntities\\Code\\" +
                    selectedModule.DataEntityNamespace;
                dataEntitiesClassPath = CreateDirectory(dataEntitiesClassPath);

                // The path relative to the user selected destination directory where the 
                // parsers' code file should be placed after generation.
                string parsersFolderPath = "Serializers\\" + selectedModule.SerializerNamespace;
                parsersFolderPath = CreateDirectory(parsersFolderPath);

                // The xsd file which should be generated.
                string xsdFileToBeGenerated = xsdFolderPath + "\\"
                    + selectedEntity.DataEntityClassName + ".xsd";

                // Calling the host controller to build the data entity.
                string strXsd =
                    hostTypeController.BuildDataEntity(selectedEntity, selectedModule);

                // Write the generated xsd content to the file
                WriteToFile(xsdFileToBeGenerated, strXsd);

                // Call xsd object generator to generate the code file for the above 
                // generated xsd file.
                // The xsd object gen will be called only if a path is mentioned in the 
                // config file.


                //configSettings.XsdObjectConfig.XsdObjectGeneratorPath.Length 
                // > 0
                // && 

                if (!ReplaceXsdObjGenCommand(selectedModule, selectedEntity, dataEntitiesClassPath))
                {
                    noErrorsOccured = false;
                    string errorMessage
                        = "There were errors while generating the code file " +
                        "for the data entities. Please check the path of xsd object generator.";
                    ShowError(errorMessage);
                }

                // File name for the parser's file
                string parsersFilePath
                    = parsersFolderPath + "\\" + selectedEntity.SerializerClassName + ".cs";

                // The code generated for the parsers file retrieved by calling the 
                // host controller.
                string strParserCode
                    = hostTypeController.BuildSerializer(selectedEntity, selectedModule);

                // Write the generated parser code to file.
                WriteToFile(parsersFilePath, strParserCode);
                return noErrorsOccured;
            }
            catch (Exception exc)
            {
                LogException(exc);
                return false;
            }
        }

        /// <summary>
        /// Method used to write a string to a given file name. 
        /// The file if it already exists will be replaced.
        /// </summary>
        /// <param name="filePath">
        /// The file name with path to which the input string should be written.
        /// </param>
        /// <param name="strToBeWritten">
        /// The string which should be written.
        /// </param>
        private void WriteToFile(string filePath, string strToBeWritten)
        {
            DisplayStatus("Writing to file " + filePath);
            const bool OVERWRITE = true;
            StreamWriter writer = new StreamWriter(filePath, !OVERWRITE);
            writer.Write(strToBeWritten);
            writer.Close();
        }

        /// <summary>
        /// This method should be used to create a directory as mentioned in the input 
        /// parameter. This path is relative to the destination selected by the user.
        /// </summary>
        /// <param name="relativeFolderPath">
        /// The directory to be created. This path will be created under the 
        /// path selected by the user.
        /// </param>
        /// <returns>
        /// The complete path to the folder path sent as the input.
        /// </returns>
        private string CreateDirectory(string relativeFolderPath)
        {

            // Prepend the destination path selected by the user.
            relativeFolderPath = destinationPath + "\\" + relativeFolderPath;

            // Create the folder.
            DirectoryInfo directoryInfo = new DirectoryInfo(relativeFolderPath);
            directoryInfo.Create();

            // return the complete path.
            return relativeFolderPath;
        }

        /// <summary>
        /// Method to execute the XSD Object generator. This method checkes 
        /// through the arguments to be passed to the xsd object generator,
        /// replaces the required variable values, and executes the xsd object generator.
        /// </summary>
        /// <param name="moduleToBeUsed">
        /// The module for which the code is being generated for xsd.
        /// </param>
        /// <param name="selectedEntity">
        /// The entity for which the code is being generated.
        /// </param>
        /// <param name="fileDirectory">
        /// The directory where the code file should be generated.
        /// </param>
        /// <returns>true, if xsd object generator executes successfully.</returns>
        private bool ReplaceXsdObjGenCommand(LegacyParserModule moduleToBeUsed,
            Entity selectedEntity, string fileDirectory)
        {
            string xsdObjectCommandArguments =
                configSettings.XsdObjectConfig.XsdObjectGeneratorArgs;

            // replace the %filename% parameter
            xsdObjectCommandArguments =
                xsdObjectCommandArguments.Replace("%filename%",
                fileDirectory + "\\" + selectedEntity.DataEntityClassName + ".cs");

            // replace the %namespace% parameter
            xsdObjectCommandArguments =
                xsdObjectCommandArguments.Replace("%namespace%",
                moduleToBeUsed.DataEntityNamespace);

            // replace the %schemafile% parameter
            xsdObjectCommandArguments =
                xsdObjectCommandArguments.Replace("%schemafile%",
                selectedEntity.DataEntityClassName + ".xsd");

            // replace the %schemafilepath% parameter
            xsdObjectCommandArguments =
                xsdObjectCommandArguments.Replace("%schemafilepath%",
                fileDirectory + @"\..\..\xsd");

            // replace the %schemafilepath% parameter
            xsdObjectCommandArguments =
                xsdObjectCommandArguments.Replace("%classname%",
                selectedEntity.DataEntityClassName);

            System.Diagnostics.Process xsdGenerationProcess
                = new System.Diagnostics.Process();

            xsdGenerationProcess.StartInfo.WindowStyle
                = System.Diagnostics.ProcessWindowStyle.Hidden;

            // Indicate the xsd executable file which should be run.
            xsdGenerationProcess.StartInfo.FileName
                = "\"" + configSettings.XsdObjectConfig.XsdObjectGeneratorPath + "\"";


            // Pass the arguments.
            xsdGenerationProcess.StartInfo.Arguments
                = xsdObjectCommandArguments;
            try
            {
                return xsdGenerationProcess.Start();
            }
            catch (Exception exc)
            {
                LogException(exc);
                return false;
            }

        }

        /// <summary>
        /// Method to allow the user to change the configurations of the tool.
        /// This also is the event handler for the "Change configurations" 
        /// menu item click.
        /// </summary>
        /// <param name="sender">The object which raises the event.</param>
        /// <param name="e">Event arguments to the event handler.</param>
        private void ChangeConfigurations(object sender, EventArgs e)
        {
            // Create an instance of the config window.
            ConfigWindow cfgWindow = new ConfigWindow();
            // display it as a dialog.
            cfgWindow.ShowDialog();
        }

        /// <summary>
        /// Event handler for add Module menu item click event.
        /// </summary>
        /// <param name="sender">
        /// The object which raises the event.
        /// </param>
        /// <param name="e">
        /// Event arguments for the event handler.
        /// </param>
        private void addModuleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Create an instance of a Module
            LegacyParserModule newModule = GenerateNewModule();
            {
                // Set the default values for the module.
                newModule.DataEntityNamespace = "";
                newModule.Entities.Clear();
                newModule.SerializerNamespace = "";

                // add the module to the current project.
                currProject.Modules.Add(newModule);
                // re-bind the tree.
                BindTreeToProject(currProject);
            }
        }

        /// <summary>
        /// Method to create a new module.
        /// </summary>
        /// <returns>A new module created with a default name.</returns>
        private LegacyParserModule GenerateNewModule()
        {
            LegacyParserModule newModule = new LegacyParserModule();
            string newModulePrefix = "NewModule";
            int newModuleLooper = 0;
            while (FindModule(newModulePrefix + newModuleLooper.ToString()) != null)
            {
                newModuleLooper++;
            }
            newModule.Name = newModulePrefix + newModuleLooper.ToString();
            return newModule;
        }

        /// <summary>
        /// Event handler for context menu mouse hover. The event is raised 
        /// when the user moves the mouse over the context menu of the entity node
        /// </summary>
        /// <param name="sender">the object which raises the event.</param>
        /// <param name="e">The event arguments for this event.</param>
        private void entityTreeNodeContextMenuHoverHandler(object sender, EventArgs e)
        {
            ToolStripMenuItem moveToMenu = (ToolStripMenuItem)sender;
            moveToMenu.DropDownItems.Clear();


            for (int moduleLooper = 0;
                moduleLooper < currProject.Modules.Count;
                moduleLooper++)
            {
                LegacyParserModule module = currProject.Modules[moduleLooper];
                ToolStripMenuItem menuItem = new ToolStripMenuItem();
                menuItem.Click += new EventHandler(moveToMenuItem_Click);
                menuItem.Text = module.Name;
                moveToMenu.DropDownItems.Add(menuItem);
            }
        }
        /// <summary>
        /// Event handler for click of context menu item. 
        /// The user can raise this event by clicking on any menu item indicating 
        /// that they want to move an entity from one module to another.
        /// </summary>
        /// <param name="sender">The object raising the event.</param>
        /// <param name="e">event arguments for the menu item click event.</param>
        void moveToMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = modulesListTree.SelectedNode;
            Entity selectedEntity =
                FindEntityInModule(selectedNode.Text, selectedModule);
            selectedModule.Entities.Remove(selectedEntity);
            ToolStripMenuItem selectedModuleName = (ToolStripMenuItem)sender;
            LegacyParserModule moveToModule = FindModule(selectedModuleName.Text);
            moveToModule.Entities.Add(selectedEntity);
            BindTreeToProject(currProject);

        }

        /// <summary>
        /// Event handler for the menu "Delete Entity" Click event.
        /// </summary>
        /// <param name="sender">Object raising thie event.</param>
        /// <param name="e">Arguments passed to this event handler.</param>
        private void DeleteEntity_Click(object sender, EventArgs e)
        {
            DeleteEntity();
        }

        /// <summary>
        /// Method used to delete an entity.
        /// </summary>
        private void DeleteEntity()
        {
            TreeNode entityNode = modulesListTree.SelectedNode;
            Entity entityToBeDeleted = FindEntityInModule(entityNode.Text, selectedModule);
            DialogResult resultOfConfirmation =
                MessageBox.Show("The entity selected " + entityNode.Text
                + " will be deleted.\nAre you sure?", "Entity delete confirmation",
                MessageBoxButtons.YesNo);
            // If the user agrees to delte the entity.
            if (resultOfConfirmation == DialogResult.Yes)
            {
                modulesListTree.SelectedNode = entityNode.Parent;
                selectedModule.Entities.Remove(entityToBeDeleted);
                BindTreeToProject(currProject);
            }
        }
        /// <summary>
        /// Event handler for KeyDown event of the tree.
        /// </summary>
        /// <param name="sender">Object raising the event.</param>
        /// <param name="e">Arguments passed</param>
        private void modulesListTree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteEntity();
            }
        }

        private void entityEditor_MaximizeWorkSpace(object sender, EventArgs e)
        {
            if (splitContainer1.Panel1Collapsed)
            {
                splitContainer1.Panel1Collapsed = false;
                splitContainer2.Panel1Collapsed = false;
                splitContainer3.Panel2Collapsed = false;
            }
            else
            {
                splitContainer1.Panel1Collapsed = true;
                splitContainer2.Panel1Collapsed = true;
                splitContainer3.Panel2Collapsed = true;
            }
        }
        Infosys.Lif.LegacyConfig.LegacySettings legacyFacadeConfigSettings = null;
        Infosys.Lif.LegacyIntegratorService.LISettings legacyIntegratorSettings = null;




        private void configurationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigurationSettingsDesign setting
                = new ConfigurationSettingsDesign();
            setting.legacySettings = legacyFacadeConfigSettings;
            setting.liSettings = legacyIntegratorSettings;
            setting.ShowDialog();
            legacyFacadeConfigSettings = setting.legacySettings;
            legacyIntegratorSettings = setting.liSettings;
        }

    }
}
