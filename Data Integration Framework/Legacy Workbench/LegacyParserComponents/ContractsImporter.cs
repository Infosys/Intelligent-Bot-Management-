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
    public partial class ContractsImporter : UserControl, IContractImporter
    {
        public ContractsImporter()
        {
            InitializeComponent();
        }

        public Contract RetrieveContract(string fileName)
        {
            return RetrieveContractFromFile(RetrieveContractRetrievers(), fileName);
        }

        private void btnHstFilesBrowse_Click(object sender, EventArgs e)
        {
            if (openDialog.ShowDialog() == DialogResult.OK)
            {
                //Code added by sid to resolve issue regarding
                //Dynamic loading (with the activator.createinstance) of modal importer and contract importer assembly gets 
                //reset to the file selected path in the open file dialog.

                //code added by Srajan J Lad on 17th Oct 2007
                //to prevent the user from uploading files other than contract files
                if (!openDialog.FileName.ToLowerInvariant().EndsWith(".txt"))
                {
                    MessageBox.Show("Upload only contract files with .txt extension", "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                Environment.CurrentDirectory = System.Windows.Forms.Application.StartupPath;                             

                //set progress bar
                progressBar_BrowseContracts.Visible = true;
                progressBar_BrowseContracts.Minimum = 1;
                progressBar_BrowseContracts.Maximum = openDialog.FileNames.Length;
                progressBar_BrowseContracts.Value = 1;
                progressBar_BrowseContracts.Step = 1;

                Framework.Helper.ShowStatusMessage("Uploading contracts...");
                Application.DoEvents();                

                foreach (string strFileName in openDialog.FileNames)
                {
                    txtContractsFileLocations.Text += strFileName + Constants.File_Seperator;                    
                }

                ContractsRetrievedEventArgs eventArgs = new ContractsRetrievedEventArgs();
                try
                {
                    eventArgs.RetrievedContracts = RetrieveContractDetails();
                    progressBar_BrowseContracts.PerformStep();
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
                        eventArgs.RetrievedContracts = (Entities.GenericCollection<Entities.Contract>)lpException.PlaceHolder[1];
                        Framework.Helper.ShowWarning("Errors occured while retrieving the contract details. Please check the summary tab");
                    }
                    
                    progressBar_BrowseContracts.Hide();
                    txtContractsFileLocations.Clear();
                    return;
                }

                if (eventArgs.RetrievedContracts == null)
                {
                    progressBar_BrowseContracts.Hide();
                    txtContractsFileLocations.Clear();
                    return;
                }

                if (ContractsRetrieved != null)
                {
                    ContractsRetrieved(this, eventArgs);
                    progressBar_BrowseContracts.PerformStep();
                }

                progressBar_BrowseContracts.Visible = false;
            }
        }

        const string ContractRetrieversConfigSection = "ContractRetrievers";

        private Configurations.Retrievers.Retrievers RetrieveContractRetrievers()
        {
            return (Configurations.Retrievers.Retrievers)System.Configuration.ConfigurationManager.GetSection(ContractRetrieversConfigSection);
        }

        private GenericCollection<Contract> RetrieveContractDetails()
        {
            GenericCollection<Contract> contracts
                = new GenericCollection<Contract>();
            string[] contractsLocation = RetrieveContractFilePaths();

            Configurations.Retrievers.Retrievers contractReaders
                = RetrieveContractRetrievers();

            for ( int i = 0; i < contractsLocation.Length; i++)
            {                
                string contractFileNameWithPath = contractsLocation[i];

                Framework.Helper.ShowStatusMessage("Retrieving Contract " + contractFileNameWithPath + "...");

                Contract contractRetrieved = RetrieveContractFromFile(contractReaders, contractFileNameWithPath);
                
                if (contractRetrieved == null)
                {
                    contracts = null;
                    return contracts;
                }

                if (contractRetrieved != null)
                {
                    contracts.Add(contractRetrieved);
                }

                Framework.Helper.ShowStatusMessage("Retrieved Contract " + contractRetrieved.ContractName + "...");
                progressBar_BrowseContracts.PerformStep();
                Application.DoEvents();
            }

            txtContractsFileLocations.Text = string.Empty;

            Framework.Helper.ShowStatusMessage("Retrieved " + contracts.Count + " contracts");
            return contracts;
        }

        private Contract RetrieveContractFromFile(Configurations.Retrievers.Retrievers contractReaders, string contractFileNameWithPath)
        {
            Contract contractRetrieved = null;
            try
            {
                string contractFileName = contractFileNameWithPath.Substring(contractFileNameWithPath.LastIndexOf('\\') + 1);
                Framework.Helper.ShowStatusMessage("Retrieving contract from the file " + contractFileName + "...");

                int contractFileNameLength = contractFileName.Length;
                bool isContractRetrieved = false;
                int indexOfLastDot = contractFileName.LastIndexOf('.');
                string fileExtension = contractFileName.Substring(indexOfLastDot + 1);

                if (indexOfLastDot == -1)
                {
                    // There is no extension for the file. 
                    fileExtension = string.Empty;
                }

                for (int j = 0; j < contractReaders.Count; j++)
                {
                    if (fileExtension.ToLowerInvariant()
                        == contractReaders[j].fileExtension.ToLowerInvariant())
                    {
                        contractRetrieved =
                            RetrieveContractFromRetriever(contractReaders[j].assemblyPath, contractReaders[j].type,
                               contractFileNameWithPath);
                        isContractRetrieved = true;
                        break;
                    }
                }

                if (!isContractRetrieved)
                {
                    for (int j = 0; j < contractReaders.Count; j++)
                    {
                        if (contractReaders[j].name.ToLowerInvariant() == contractReaders.defaultRetriever.ToLowerInvariant())
                        {
                            contractRetrieved = RetrieveContractFromRetriever(contractReaders[j].assemblyPath, contractReaders[j].type,
                                contractFileNameWithPath);
                            isContractRetrieved = true;
                            break;
                        }
                    }
                }

                Framework.Helper.ShowStatusMessage("Retrieved contract from the file " + contractFileName + "...");

            }
            catch (Exception exc)
            {
                string errorMessage = "Error Occured while retrieving details from contract " + contractFileNameWithPath + Environment.NewLine;
                Infosys.Lif.LegacyWorkbench.Framework.Helper.ShowError(errorMessage);
                Infosys.Lif.LegacyWorkbench.Framework.Helper.ShowSummary(exc.ToString());
            }
            return contractRetrieved;
        }

        private Contract RetrieveContractFromRetriever(string assemblyPath, string type, string filePath)
        {
            IContractRetriever contractImporter
                = BuildContractRetriever(assemblyPath, type);

            Contract contractRetrieved
                = contractImporter.RetrieveContractDetails(filePath);

            return contractRetrieved;
        }

        private IContractRetriever BuildContractRetriever(string assemblyPath, string typeName)
        {
            object unwrappedObject = Helper.CreateInstanceOf(assemblyPath, typeName);
            if (!(unwrappedObject is IContractRetriever))
            {
                LegacyParserException legParserExc = new LegacyParserException();
                legParserExc.ErrorReason = LegacyParserException.ErrorReasonCode.IncorrectRetrieverType;
                legParserExc.PlaceHolder.Add("Retriever " + typeName + " does not implement interface IContractRetriever");
                throw legParserExc;
            }

            return (IContractRetriever)unwrappedObject;
        }

        private string[] RetrieveContractFilePaths()
        {
            string[] listOfFiles = txtContractsFileLocations.Text.Split(Constants.File_Name_Seperators,
                StringSplitOptions.RemoveEmptyEntries);
            for (int counter = 0; counter < listOfFiles.Length; counter++)
            {
                listOfFiles[counter] = listOfFiles[counter].Trim();
            }

            return listOfFiles;
        }

        #region IContractImporter Members

        public event ContractsRetrievedEventHandler ContractsRetrieved;

        #endregion
    }
}
