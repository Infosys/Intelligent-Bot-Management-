using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Infosys.Lif.LegacyWorkbench.Framework;
using Infosys.Lif.LegacyWorkbench.Entities;

namespace Infosys.Lif.LegacyWorkbench
{
    public partial class ModelObjectsImporter : UserControl, IModelObjectImporter
    {
        const string ModelObjectRetrieversConfigSection = "ModelObjectRetrievers";

        public ModelObjectsImporter()
        {
            InitializeComponent();
        }

        public Entity RetrieveModelObjectDetails(string fileName)
        {
            Entity entity = RetrieveEntityDetails(RetrieveModelObjectRetrievers(), fileName);
            return entity;
        }

        #region IModelObjectImporter Members

        public event ModelObjectsRetrievedEventHandler ModelObjectsRetrieved;

        #endregion

        #region Retrieval Members

        private GenericCollection<Entity> RetrieveModelObjectDetails()
        {
            GenericCollection<Framework.LegacyParserException> lpExceptions
                = new GenericCollection<LegacyParserException>();

            GenericCollection<Entity> retrievedModelObjects = new GenericCollection<Entity>();

            string[] ModelObjectsLocation = RetrieveModelObjectFilePaths();

            Configurations.Retrievers.Retrievers modelObjectReaders
                = RetrieveModelObjectRetrievers();

            for (int fileLooper = 0; fileLooper < ModelObjectsLocation.Length; fileLooper++)
            {
                string fileWithPath = ModelObjectsLocation[fileLooper];
                try
                {
                    Entity entity = RetrieveEntityDetails(modelObjectReaders, fileWithPath);

                    if (entity != null)
                    {
                        retrievedModelObjects.Add(entity);
                    }

                    progressBar_BrowseModelObjects.PerformStep();
                    Application.DoEvents();

                    if (entity.ProgramId == null || entity.ProgramId.Length == 0)
                    {
                        LegacyParserException lpException = new LegacyParserException();
                        lpException.ErrorReason = LegacyParserException.ErrorReasonCode.ProgramIdNotFound;
                        lpException.PlaceHolder.Add(fileWithPath);
                        lpExceptions.Add(lpException);
                    }
                }

                catch (Framework.LegacyParserException lpException)
                {
                    lpExceptions.Add(lpException);
                }
            }

            if (lpExceptions.Count > 0)
            {
                LegacyParserException lpException = new LegacyParserException();
                lpException.PlaceHolder.Add(lpExceptions);
                lpException.ErrorReason = LegacyParserException.ErrorReasonCode.ErrorsDuringModelObjectRetrieval;
                lpException.PlaceHolder.Add(retrievedModelObjects);
                throw lpException;
            }

            txtHostProgPaths.Text = string.Empty;
            Framework.Helper.ShowStatusMessage("Retrieved " + retrievedModelObjects.Count + " Model Objects");
            return retrievedModelObjects;
        }

        private Entity RetrieveEntityDetails(Configurations.Retrievers.Retrievers modelObjectReaders, string fileWithPath)
        {
            Entity entity = null;
            string modelObjectFileName = fileWithPath.Substring(fileWithPath.LastIndexOf('\\') + 1);
            Framework.Helper.ShowStatusMessage("Retrieving model object " + modelObjectFileName + "...");
            int modelObjectFileNameLength = modelObjectFileName.Length;

            bool isModelObjectRetrieved = false;
            int indexOfLastDot = modelObjectFileName.LastIndexOf('.');
            string fileExtension = modelObjectFileName.Substring(indexOfLastDot + 1);

            if (indexOfLastDot == -1)
            {
                // There is no extension for the file. 
                fileExtension = string.Empty;
            }

            for (int i = 0; i < modelObjectReaders.Count; i++)
            {
                if (fileExtension.ToLowerInvariant()
                    == modelObjectReaders[i].fileExtension.ToLowerInvariant())
                {
                    entity = RetrieveModelObjectFromRetriever(modelObjectReaders[i].assemblyPath, modelObjectReaders[i].type,
                        fileWithPath);
                    isModelObjectRetrieved = true;
                    break;
                }
            }

            if (!isModelObjectRetrieved)
            {
                for (int j = 0; j < modelObjectReaders.Count; j++)
                {
                    if (modelObjectReaders[j].name.ToLowerInvariant() == modelObjectReaders.defaultRetriever.ToLowerInvariant())
                    {
                        entity = RetrieveModelObjectFromRetriever(modelObjectReaders[j].assemblyPath, modelObjectReaders[j].type,
                            fileWithPath);
                        isModelObjectRetrieved = true;
                        break;
                    }
                }
            }

            Framework.Helper.ShowStatusMessage("Retrieved model object " + modelObjectFileName + "...");
            return entity;
        }

        private Entity RetrieveModelObjectFromRetriever(string assemblyPath, string type, string filePath)
        {
            IModelObjectRetriever modelObjectRetriever = BuildModelObjectRetriever(assemblyPath, type);
            Entity entityRetrieved = modelObjectRetriever.RetrieveModelObjectDetails(filePath);
            return entityRetrieved;
        }

        private IModelObjectRetriever BuildModelObjectRetriever(string assemblyPath, string type)
        {
            object unwrappedObject = Helper.CreateInstanceOf(assemblyPath, type);
            if (!(unwrappedObject is IModelObjectRetriever))
            {
                LegacyParserException legParserExc = new LegacyParserException();
                legParserExc.ErrorReason = LegacyParserException.ErrorReasonCode.IncorrectRetrieverType;
                legParserExc.PlaceHolder.Add("Retriever " + type + " does not implement interface IModelObjectRetriever");
                throw legParserExc;
            }

            return (IModelObjectRetriever)unwrappedObject;
        }

        private Configurations.Retrievers.Retrievers RetrieveModelObjectRetrievers()
        {
            return (Configurations.Retrievers.Retrievers)System.Configuration.ConfigurationManager.GetSection(ModelObjectRetrieversConfigSection);
        }

        private string[] RetrieveModelObjectFilePaths()
        {
            string[] listOfFiles = txtHostProgPaths.Text.Split(Constants.File_Name_Seperators, StringSplitOptions.RemoveEmptyEntries);
            for (int counter = 0; counter < listOfFiles.Length; counter++)
            {
                listOfFiles[counter] = listOfFiles[counter].Trim();
            }

            return listOfFiles;
        }

        #endregion

        private void btnHstFilesBrowse_Click(object sender, EventArgs e)
        {            
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                //Code added by sid to resolve issue regarding
                //Dynamic loading (with the activator.createinstance) of modal importer and contract importer assembly gets 
                //reset to the file selected path in the open file dialog.

                //Code added by Srajan J Lad on 17th Oct 2007
                //Code to prevent user from uploading files other than cobol files
                if (!openDialog.FileName.ToLowerInvariant().EndsWith(".cob"))
                {
                    MessageBox.Show("Upload only cobol files with .cob extension","Legacy Workbench",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }                

                Environment.CurrentDirectory = System.Windows.Forms.Application.StartupPath;

                //set progress bar
                progressBar_BrowseModelObjects.Visible = true;
                progressBar_BrowseModelObjects.Minimum = 1;
                progressBar_BrowseModelObjects.Maximum = openDialog.FileNames.Length;
                progressBar_BrowseModelObjects.Value = 1;
                progressBar_BrowseModelObjects.Step = 1;                                

                Framework.Helper.ShowStatusMessage("Uploading Model Objects...");
                Application.DoEvents();

                foreach (string str in openDialog.FileNames)
                {
                    txtHostProgPaths.Text += str + Constants.File_Seperator;                   
                }

                ModelObjectsRetrievedEventArgs eventArgs = new ModelObjectsRetrievedEventArgs();
                try
                {
                    eventArgs.RetrievedModelObjects = RetrieveModelObjectDetails();
                }
                catch (LegacyParserException lpException)
                {
                    if (lpException.ErrorReason
                    == Framework.LegacyParserException.ErrorReasonCode.ErrorsDuringModelObjectRetrieval)
                    {
                        Entities.GenericCollection<Framework.LegacyParserException> lpExceptions
                            = (Entities.GenericCollection<Framework.LegacyParserException>)lpException.PlaceHolder[0];
                        foreach (Framework.LegacyParserException returnedException in lpExceptions)
                        {
                            string errorMessage = returnedException.Message;
                            Framework.Helper.ShowSummary(errorMessage);
                        }

                        eventArgs.RetrievedModelObjects = (Entities.GenericCollection<Entities.Entity>)lpException.PlaceHolder[1];
                        Framework.Helper.ShowWarning("Errors occured while retrieving the clause details. Please check the summary tab");
                    }
                    
                    txtHostProgPaths.Clear();
                    progressBar_BrowseModelObjects.Hide();
                    return;
                }

                if (ModelObjectsRetrieved != null)
                {
                    ModelObjectsRetrieved(this, eventArgs);
                    progressBar_BrowseModelObjects.PerformStep();
                }

                progressBar_BrowseModelObjects.Visible = false;
            }
        }        
    }
}
