using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;


using Infosys.Lif.LegacyParser;

/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * This file holds the Module editor's code behind.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/

namespace Infosys.Lif.LegacyParser.UI
{
    /// <summary>
    /// Code behind for ModuleEdit user control. This user control is 
    /// used to allow the user to modify a Module.
    /// </summary>
    public partial class ModuleEdit : UserControl
    {
        /// <summary>
        /// Event raised when the mdoule this control is bound to is modified.
        /// </summary>
        public event EventHandler<ChangedEventArgs<LegacyParserModule>> DataChanged;

        /// <summary>
        /// The module which should be edited.
        /// </summary>
        private LegacyParserModule moduleBeingEdited;
        /// <summary>
        /// Boolean flag indicating whether the text change is 
        /// occuring the first time (on form load), in which case the 
        /// DataChanged event should not be raised.
        /// </summary>
        private bool isLoaded;

        /// <summary>
        /// Public constructor for this module editor user control
        /// </summary>
        public ModuleEdit()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Loads up the module passed into this control.
        /// </summary>
        /// <param name="moduleToBeLoaded">The module which has been passed.</param>
        public void LoadModule(ref LegacyParserModule moduleToBeLoaded)
        {
            // Do not fire DataChanged event first time while binding 
            // this control to the module.
            isLoaded = false;
            
            // display the required fields in the appropriate text boxes.
            moduleBeingEdited = moduleToBeLoaded;
            txtDataEntityNamespace.Text = moduleBeingEdited.DataEntityNamespace;
            txtModuleName.Text = moduleBeingEdited.Name;
            txtParserNamespace.Text = moduleBeingEdited.SerializerNamespace;

            txtServiceComponents.Text = moduleBeingEdited.ServiceComponentsNamespace;
            txtTestCaseNamespace.Text = moduleBeingEdited.UnitTestCasesNamespace;

            // any more changes to the fields should result in DataChanged 
            // event being fired.
            isLoaded = true;
        }

        /// <summary>
        /// Will be called if any of the textboxes changes
        /// </summary>
        /// <param name="sender">Object raising the TextChanged event.</param>
        /// <param name="e">Event Arguments for the text changed event.</param>
        private void Items_TextChanged(object sender, EventArgs e)
        {
            // If this is not first load.
            if (isLoaded)
            {
                moduleBeingEdited.DataEntityNamespace = txtDataEntityNamespace.Text;
                moduleBeingEdited.Name = txtModuleName.Text;
                moduleBeingEdited.SerializerNamespace = txtParserNamespace.Text;

                // Any event handlers attached?
                if (DataChanged != null)
                {
                    ChangedEventArgs<LegacyParserModule> eventArgs
                        = new ChangedEventArgs<LegacyParserModule>();
                    eventArgs.NewValue = moduleBeingEdited;
                    Control ctrl = (Control)sender;
                    switch (ctrl.Name)
                    {
                        case "txtDataEntityNamespace":
                            eventArgs.PropertyChanged = "DENamespace";
                            break;
                        case "txtModuleName": eventArgs.PropertyChanged = "ModuleName";
                            break;
                        case "txtParserNamespace":
                            eventArgs.PropertyChanged = "ParserNamespace";
                            break;
                    }

                    DataChanged(this, eventArgs);
                }
            }

        }
    }
}