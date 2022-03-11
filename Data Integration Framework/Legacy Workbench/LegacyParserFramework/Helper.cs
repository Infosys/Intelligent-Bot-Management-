using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using System.Windows.Forms;

namespace Infosys.Lif.LegacyWorkbench.Framework
{

    public class DisplayStatusEventArgs : EventArgs
    {
        string statusMessage;

        public string StatusMessage
        {
            get { return statusMessage; }
            set { statusMessage = value; }
        }
    }
    public delegate void DisplayStatusEventHandler(object sender, DisplayStatusEventArgs e);

    public enum StatusTypes
    {
        Information, Warning, Error
    }
    
    public static class Helper
    {

        static ProgressBar statusProgressBar;
        static Label statusLabel;
        ////static Form legacyParser;
        static DisplayDelegate showSummaryMethod;
        static DisplayStatusDelegate showErrorMethod;

        public static event DisplayStatusEventHandler DisplayCurrentStatus;

        //////static Entities.Project projectBeingCodeGenerated;

        //////public static Entities.Project ProjectBeingCodeGenerated
        //////{
        //////    get { return Helper.projectBeingCodeGenerated; }
        //////    set { Helper.projectBeingCodeGenerated = value; }
        //////}


        ////static string RetrieveXmlSchemaNamespace(Entities.Module module, Entities.ModelObject ModelObject)
        ////{
        ////    string.Format(projectBeingCodeGenerated.ModelObjectNamespaces.XmlSchemaNamespace
        ////}


        public delegate void DisplayStatusDelegate(string strMessage, StatusTypes status);
        public delegate void DisplayDelegate(string strMessage);


        public static void SetHelperMethods(DisplayDelegate displaySummaryMethod,
            DisplayStatusDelegate displayStatusMethod, ProgressBar statusBar, Label progressStatusLabel)
        {
            showErrorMethod = displayStatusMethod;
            showSummaryMethod = displaySummaryMethod;
            statusProgressBar = statusBar;
            statusLabel = progressStatusLabel;
        }
        const string errorSeperators = "===========================================";
        public static void ShowSummary(string errorText)
        {

            if (showSummaryMethod != null)
            {
                showSummaryMethod(errorSeperators);
                showSummaryMethod(errorText);
                showSummaryMethod(errorSeperators);
            }
            //legacyParser.DisplaySummary(errorText);
        }

        public static void ShowWarning(string statusMessage)
        {
            if (showErrorMethod != null)
            {
                showErrorMethod(statusMessage, StatusTypes.Warning);
            }
        }
        public static void ShowError(string errorMessage)
        {
            if (showErrorMethod != null)
            {
                showErrorMethod(errorMessage, StatusTypes.Error);
            }
        }

        public static void InitializeStatusBar(int maxStatusNumber)
        {
            if (statusProgressBar != null)
            {
                statusProgressBar.Maximum = maxStatusNumber;
            }
        }

        public static void ShowStatusMessage(string statusMessage)
        {
            if (DisplayCurrentStatus != null)
            {
                DisplayStatusEventArgs eventArgs = new DisplayStatusEventArgs();
                eventArgs.StatusMessage = statusMessage;

                //to remove multicast delegate error when method is called from different sources
                DisplayCurrentStatus.GetInvocationList();
            }
        }

        public static void ShowStatusCompletionText(string statusText)
        {
            if (statusLabel != null)
            {
                statusLabel.Text = statusText;
                statusLabel.Refresh();
            }
        }
        public static void IncreaseStatusCompletion(int incrementAmount)
        {
            if (statusProgressBar != null)
            {
                statusProgressBar.Value += incrementAmount;
                statusProgressBar.Refresh();
            }
        }
        public static void ResetStatusBar()
        {
            if (statusProgressBar != null)
            {
                statusProgressBar.Value = 0;
            }
        }

        public static string BuildNamespace(string strNamespace, Entities.Contract contract)
        {
            return strNamespace.Replace("#EntityName#", contract.ContractName);
        }
        public static string BuildNamespace(string strNamespace, Entities.Entity ModelObjectEntity)
        {
            return strNamespace.Replace("#EntityName#", ModelObjectEntity.EntityName);
        }

        public static string BuildNamespace(string strNamespace, Entities.Module module)
        {
            return strNamespace.Replace("#ModuleName#", module.Name);
        }

        const string TRACE_CATEGORY = "LegacyIntegrator";
        private static void Trace(string message)
        {
            Logger.Write(message, TRACE_CATEGORY);
        }
    }
}





