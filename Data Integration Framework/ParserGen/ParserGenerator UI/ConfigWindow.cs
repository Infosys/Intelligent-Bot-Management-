using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * This is the code behind for the form shown to modify the config settings.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/

namespace Infosys.Lif.LegacyParser.UI
{
    /// <summary>
    /// ConfigWindow is a modal dialog shown to allow the user 
    /// to change configuration settings. This class is the code behind.
    /// </summary>
    public partial class ConfigWindow : Form
    {

        /// <summary>
        /// The legacy parser configurations which are being modified.
        /// </summary>
        private Configurations.LegacyParserConfigurations legacyParserConfig;


        /// <summary>
        /// Public constructor for this dialog.
        /// </summary>
        public ConfigWindow()
        {
            legacyParserConfig = ConfigWrapper.GetConfiguration();
            if (legacyParserConfig == null)
            {
                legacyParserConfig = new Configurations.LegacyParserConfigurations();
            }
            InitializeComponent();
        }

        /// <summary>
        /// Load event handler for this form.
        /// </summary>
        /// <param name="sender">The object which raises this event.</param>
        /// <param name="e">Event arguments for this event.</param>
        private void ConfigWindow_Load(object sender, EventArgs e)
        {
            BindToData();
        }

        /// <summary>
        /// BindToData enables this form to bind the various textboxes and 
        /// other controls on the form with their respective fields in 
        /// the ParserConfigurations.
        /// </summary>
        private void BindToData()
        {
            // Bind the xsd obj generator's path to the textbox.
            txtXsdObjArguments.DataBindings.Add("Text", legacyParserConfig.XsdObjectConfig, "XsdObjectGeneratorArgs", false, DataSourceUpdateMode.OnValidation);

            txtXsdObjGeneratorCommand.DataBindings.Add("Text", legacyParserConfig.XsdObjectConfig, "XsdObjectGeneratorPath", false, DataSourceUpdateMode.OnValidation);

            // Bind the language converter selectable listbox
            cmbLanguage.DisplayMember = "Name";
            cmbLanguage.DataSource = legacyParserConfig.LanguageConverters;
            cmbLanguage.DataBindings.Add("Text", legacyParserConfig, "Language");

            // Bind the host controller to the selectable listbox
            cmbHostType.DisplayMember = "Name";
            cmbHostType.DataSource = legacyParserConfig.HostTypes;
            cmbHostType.DataBindings.Add("Text", legacyParserConfig, "HostType");

            // Bind the storage type to the selectable listbox
            cmbDataEntityStorage.DisplayMember = "Name";
            cmbDataEntityStorage.DataSource = legacyParserConfig.StorageImplementations;
            cmbDataEntityStorage.DataBindings.Add("Text", legacyParserConfig, "StorageType");

        }

        /// <summary>
        /// The Apply button's Click event handler.
        /// This handler writes the configuration settings to the config file.
        /// </summary>
        /// <param name="sender">
        /// Object raising this event.
        /// </param>
        /// <param name="e">
        /// Event arguments for this event.
        /// </param>
        private void btnApply_Click(object sender, EventArgs e)
        {
            // Use the Utility class to write the configurations.
            ConfigWrapper.WriteConfiguration(legacyParserConfig);
        }

        /// <summary>
        /// Event handler for Click of OK button.
        /// Ok will result in a call to apply and then closes the form.
        /// </summary>
        /// <param name="sender">The object raising the Click event.</param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            // Call button Apply's click event handler
            btnApply_Click(btnApply, e);
        }

        /// <summary>
        /// Browse button enables the user to browse to XSObject Generator.
        /// The click of this button results in a call to this event handler.
        /// </summary>
        /// <param name="sender">
        /// The object raising the Click event of the Browse button.
        /// </param>
        /// <param name="e">
        /// Event arguments for the Click event.
        /// </param>
        private void btnBrowse_Click(object sender, EventArgs e)
        {

            // Add the required filter 
            openFileDialog1.Filter =
                "XsdObjectGenerator|xsdObjectGen.exe|All Executables|*.exe|" 
                + "All Files|*.*";
            
            openFileDialog1.FileName = txtXsdObjGeneratorCommand.Text;
            // If the cancel button was not clicked byt the user.
            if (openFileDialog1.ShowDialog() != DialogResult.Cancel)
            {
                // Show it in the text box.
                txtXsdObjGeneratorCommand.Text = openFileDialog1.FileName;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }
    }
}
