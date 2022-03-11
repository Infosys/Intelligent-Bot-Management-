namespace Infosys.Lif.LegacyWorkbench.CodeProviders
{
    partial class CodeGenerationConfirmation
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CodeGenerationConfirmation));
            this.groupBox_ServInterface = new System.Windows.Forms.GroupBox();
            this.radAtom = new System.Windows.Forms.RadioButton();
            this.radWCF = new System.Windows.Forms.RadioButton();
            this.radRSSFeed = new System.Windows.Forms.RadioButton();
            this.radWfActivity = new System.Windows.Forms.RadioButton();
            this.radHostAccess = new System.Windows.Forms.RadioButton();
            this.radWebServices = new System.Windows.Forms.RadioButton();
            this.groupBox_LIFComponents = new System.Windows.Forms.GroupBox();
            this.cheDataEntitesSchemaContracts = new System.Windows.Forms.CheckBox();
            this.cheDataEntitesSchemaModelObjects = new System.Windows.Forms.CheckBox();
            this.cheSerializerClass = new System.Windows.Forms.CheckBox();
            this.cheDataEntitesSchema = new System.Windows.Forms.CheckBox();
            this.cheSerializerContracts = new System.Windows.Forms.CheckBox();
            this.cheDataEntitesContracts = new System.Windows.Forms.CheckBox();
            this.cheSerializerModelObjects = new System.Windows.Forms.CheckBox();
            this.cheDataEntiesModelObjects = new System.Windows.Forms.CheckBox();
            this.cheDataEntitiesClass = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textSolnName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.textGenPath = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cheServiceInterface = new System.Windows.Forms.CheckBox();
            this.cheCompile = new System.Windows.Forms.CheckBox();
            this.toolTip_Path = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_Browse = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_Generate = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_Cancel = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_SerClass = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_DEClass = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_DESchemas = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_Service = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_Compile = new System.Windows.Forms.ToolTip(this.components);
            this.cheViewReport = new System.Windows.Forms.CheckBox();
            this.toolTip_ServicePanel = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_ViewReport = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox_ServInterface.SuspendLayout();
            this.groupBox_LIFComponents.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox_ServInterface
            // 
            this.groupBox_ServInterface.Controls.Add(this.radAtom);
            this.groupBox_ServInterface.Controls.Add(this.radWCF);
            this.groupBox_ServInterface.Controls.Add(this.radRSSFeed);
            this.groupBox_ServInterface.Controls.Add(this.radWfActivity);
            this.groupBox_ServInterface.Controls.Add(this.radHostAccess);
            this.groupBox_ServInterface.Controls.Add(this.radWebServices);
            this.groupBox_ServInterface.Location = new System.Drawing.Point(172, 106);
            this.groupBox_ServInterface.Name = "groupBox_ServInterface";
            this.groupBox_ServInterface.Size = new System.Drawing.Size(119, 168);
            this.groupBox_ServInterface.TabIndex = 5;
            this.groupBox_ServInterface.TabStop = false;
            this.groupBox_ServInterface.Text = "Service Interfaces";
            this.toolTip_ServicePanel.SetToolTip(this.groupBox_ServInterface, "Select the Required Service Interface");
            // 
            // radAtom
            // 
            this.radAtom.AutoSize = true;
            this.radAtom.Enabled = false;
            this.radAtom.Location = new System.Drawing.Point(13, 142);
            this.radAtom.Name = "radAtom";
            this.radAtom.Size = new System.Drawing.Size(67, 17);
            this.radAtom.TabIndex = 5;
            this.radAtom.Text = "Atom 1.0";
            this.radAtom.UseVisualStyleBackColor = true;
            // 
            // radWCF
            // 
            this.radWCF.AutoSize = true;
            this.radWCF.Enabled = false;
            this.radWCF.Location = new System.Drawing.Point(13, 68);
            this.radWCF.Name = "radWCF";
            this.radWCF.Size = new System.Drawing.Size(49, 17);
            this.radWCF.TabIndex = 2;
            this.radWCF.Text = "WCF";
            this.radWCF.UseVisualStyleBackColor = true;
            // 
            // radRSSFeed
            // 
            this.radRSSFeed.AutoSize = true;
            this.radRSSFeed.Enabled = false;
            this.radRSSFeed.Location = new System.Drawing.Point(13, 118);
            this.radRSSFeed.Name = "radRSSFeed";
            this.radRSSFeed.Size = new System.Drawing.Size(65, 17);
            this.radRSSFeed.TabIndex = 4;
            this.radRSSFeed.Text = "RSS 2.0";
            this.radRSSFeed.UseVisualStyleBackColor = true;
            // 
            // radWfActivity
            // 
            this.radWfActivity.AutoSize = true;
            this.radWfActivity.Enabled = false;
            this.radWfActivity.Location = new System.Drawing.Point(13, 43);
            this.radWfActivity.Name = "radWfActivity";
            this.radWfActivity.Size = new System.Drawing.Size(78, 17);
            this.radWfActivity.TabIndex = 1;
            this.radWfActivity.Text = "WF activity";
            this.radWfActivity.UseVisualStyleBackColor = true;
            // 
            // radHostAccess
            // 
            this.radHostAccess.AutoSize = true;
            this.radHostAccess.Enabled = false;
            this.radHostAccess.Location = new System.Drawing.Point(13, 18);
            this.radHostAccess.Name = "radHostAccess";
            this.radHostAccess.Size = new System.Drawing.Size(84, 17);
            this.radHostAccess.TabIndex = 0;
            this.radHostAccess.Text = "Host access";
            this.radHostAccess.UseVisualStyleBackColor = true;
            // 
            // radWebServices
            // 
            this.radWebServices.AutoSize = true;
            this.radWebServices.Location = new System.Drawing.Point(13, 93);
            this.radWebServices.Name = "radWebServices";
            this.radWebServices.Size = new System.Drawing.Size(85, 17);
            this.radWebServices.TabIndex = 3;
            this.radWebServices.Text = "Web service";
            this.radWebServices.UseVisualStyleBackColor = true;
            // 
            // groupBox_LIFComponents
            // 
            this.groupBox_LIFComponents.Controls.Add(this.cheDataEntitesSchemaContracts);
            this.groupBox_LIFComponents.Controls.Add(this.cheDataEntitesSchemaModelObjects);
            this.groupBox_LIFComponents.Controls.Add(this.cheSerializerClass);
            this.groupBox_LIFComponents.Controls.Add(this.cheDataEntitesSchema);
            this.groupBox_LIFComponents.Controls.Add(this.cheSerializerContracts);
            this.groupBox_LIFComponents.Controls.Add(this.cheDataEntitesContracts);
            this.groupBox_LIFComponents.Controls.Add(this.cheSerializerModelObjects);
            this.groupBox_LIFComponents.Controls.Add(this.cheDataEntiesModelObjects);
            this.groupBox_LIFComponents.Controls.Add(this.cheDataEntitiesClass);
            this.groupBox_LIFComponents.Location = new System.Drawing.Point(1, 82);
            this.groupBox_LIFComponents.Name = "groupBox_LIFComponents";
            this.groupBox_LIFComponents.Size = new System.Drawing.Size(151, 246);
            this.groupBox_LIFComponents.TabIndex = 0;
            this.groupBox_LIFComponents.TabStop = false;
            this.groupBox_LIFComponents.Text = "LIF Components";            
            // 
            // cheDataEntitesSchemaContracts
            // 
            this.cheDataEntitesSchemaContracts.AutoSize = true;
            this.cheDataEntitesSchemaContracts.Location = new System.Drawing.Point(33, 197);
            this.cheDataEntitesSchemaContracts.Name = "cheDataEntitesSchemaContracts";
            this.cheDataEntitesSchemaContracts.Size = new System.Drawing.Size(71, 17);
            this.cheDataEntitesSchemaContracts.TabIndex = 8;
            this.cheDataEntitesSchemaContracts.Text = "Contracts";
            this.cheDataEntitesSchemaContracts.UseVisualStyleBackColor = true;
            this.cheDataEntitesSchemaContracts.CheckedChanged += new System.EventHandler(this.cheDataEntitesShemaContracts_CheckedChanged);
            // 
            // cheDataEntitesSchemaModelObjects
            // 
            this.cheDataEntitesSchemaModelObjects.AutoSize = true;
            this.cheDataEntitesSchemaModelObjects.Location = new System.Drawing.Point(33, 219);
            this.cheDataEntitesSchemaModelObjects.Name = "cheDataEntitesSchemaModelObjects";
            this.cheDataEntitesSchemaModelObjects.Size = new System.Drawing.Size(91, 17);
            this.cheDataEntitesSchemaModelObjects.TabIndex = 7;
            this.cheDataEntitesSchemaModelObjects.Text = "ModelObjects";
            this.cheDataEntitesSchemaModelObjects.UseVisualStyleBackColor = true;
            this.cheDataEntitesSchemaModelObjects.CheckedChanged += new System.EventHandler(this.cheDataEntitesSchemaModelObjects_CheckedChanged);
            // 
            // cheSerializerClass
            // 
            this.cheSerializerClass.AutoSize = true;
            this.cheSerializerClass.Location = new System.Drawing.Point(12, 23);
            this.cheSerializerClass.Name = "cheSerializerClass";
            this.cheSerializerClass.Size = new System.Drawing.Size(106, 17);
            this.cheSerializerClass.TabIndex = 0;
            this.cheSerializerClass.Text = "Serializer classes";
            this.toolTip_SerClass.SetToolTip(this.cheSerializerClass, "Check to Generate Serializer Classes");
            this.cheSerializerClass.UseVisualStyleBackColor = true;
            this.cheSerializerClass.CheckedChanged += new System.EventHandler(this.cheSerializerClass_CheckedChanged);
            // 
            // cheDataEntitesSchema
            // 
            this.cheDataEntitesSchema.AutoSize = true;
            this.cheDataEntitesSchema.Location = new System.Drawing.Point(12, 175);
            this.cheDataEntitesSchema.Name = "cheDataEntitesSchema";
            this.cheDataEntitesSchema.Size = new System.Drawing.Size(122, 17);
            this.cheDataEntitesSchema.TabIndex = 6;
            this.cheDataEntitesSchema.Text = "Data entity schemas";
            this.toolTip_DESchemas.SetToolTip(this.cheDataEntitesSchema, "Check to Generate Data Entity Schemas");
            this.cheDataEntitesSchema.UseVisualStyleBackColor = true;
            this.cheDataEntitesSchema.CheckedChanged += new System.EventHandler(this.cheDataEntitySchema_CheckedChanged);
            // 
            // cheSerializerContracts
            // 
            this.cheSerializerContracts.AutoSize = true;
            this.cheSerializerContracts.Location = new System.Drawing.Point(32, 45);
            this.cheSerializerContracts.Name = "cheSerializerContracts";
            this.cheSerializerContracts.Size = new System.Drawing.Size(71, 17);
            this.cheSerializerContracts.TabIndex = 1;
            this.cheSerializerContracts.Text = "Contracts";
            this.cheSerializerContracts.UseVisualStyleBackColor = true;
            this.cheSerializerContracts.CheckedChanged += new System.EventHandler(this.cheSerializerContracts_CheckedChanged);
            // 
            // cheDataEntitesContracts
            // 
            this.cheDataEntitesContracts.AutoSize = true;
            this.cheDataEntitesContracts.Location = new System.Drawing.Point(33, 119);
            this.cheDataEntitesContracts.Name = "cheDataEntitesContracts";
            this.cheDataEntitesContracts.Size = new System.Drawing.Size(71, 17);
            this.cheDataEntitesContracts.TabIndex = 5;
            this.cheDataEntitesContracts.Text = "Contracts";
            this.cheDataEntitesContracts.UseVisualStyleBackColor = true;
            this.cheDataEntitesContracts.CheckedChanged += new System.EventHandler(this.cheDataEntitesContracts_CheckedChanged);
            // 
            // cheSerializerModelObjects
            // 
            this.cheSerializerModelObjects.AutoSize = true;
            this.cheSerializerModelObjects.Location = new System.Drawing.Point(32, 67);
            this.cheSerializerModelObjects.Name = "cheSerializerModelObjects";
            this.cheSerializerModelObjects.Size = new System.Drawing.Size(91, 17);
            this.cheSerializerModelObjects.TabIndex = 2;
            this.cheSerializerModelObjects.Text = "ModelObjects";
            this.cheSerializerModelObjects.UseVisualStyleBackColor = true;
            this.cheSerializerModelObjects.CheckedChanged += new System.EventHandler(this.cheSerializerModelObjects_CheckedChanged);
            // 
            // cheDataEntiesModelObjects
            // 
            this.cheDataEntiesModelObjects.AutoSize = true;
            this.cheDataEntiesModelObjects.Location = new System.Drawing.Point(33, 141);
            this.cheDataEntiesModelObjects.Name = "cheDataEntiesModelObjects";
            this.cheDataEntiesModelObjects.Size = new System.Drawing.Size(91, 17);
            this.cheDataEntiesModelObjects.TabIndex = 4;
            this.cheDataEntiesModelObjects.Text = "ModelObjects";
            this.cheDataEntiesModelObjects.UseVisualStyleBackColor = true;
            this.cheDataEntiesModelObjects.CheckedChanged += new System.EventHandler(this.cheDataEntiesModelObjects_CheckedChanged);
            // 
            // cheDataEntitiesClass
            // 
            this.cheDataEntitiesClass.AutoSize = true;
            this.cheDataEntitiesClass.Location = new System.Drawing.Point(12, 97);
            this.cheDataEntitiesClass.Name = "cheDataEntitiesClass";
            this.cheDataEntitiesClass.Size = new System.Drawing.Size(115, 17);
            this.cheDataEntitiesClass.TabIndex = 3;
            this.cheDataEntitiesClass.Text = "Data entity classes";
            this.toolTip_DEClass.SetToolTip(this.cheDataEntitiesClass, "Check to Generate Data Entity Classes");
            this.cheDataEntitiesClass.UseVisualStyleBackColor = true;
            this.cheDataEntitiesClass.CheckedChanged += new System.EventHandler(this.cheDataEntityClass_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textSolnName);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.btnBrowse);
            this.groupBox3.Controls.Add(this.textGenPath);
            this.groupBox3.Location = new System.Drawing.Point(1, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(301, 69);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Generation Details";
            // 
            // textSolnName
            // 
            this.textSolnName.Location = new System.Drawing.Point(83, 43);
            this.textSolnName.Name = "textSolnName";
            this.textSolnName.Size = new System.Drawing.Size(192, 20);
            this.textSolnName.TabIndex = 4;
            this.toolTip_Path.SetToolTip(this.textSolnName, "Enter the Location to Save Generated Data");
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Solution Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Path";
            // 
            // btnBrowse
            // 
            this.btnBrowse.FlatAppearance.BorderSize = 0;
            this.btnBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowse.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.Open;
            this.btnBrowse.Location = new System.Drawing.Point(274, 17);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(23, 20);
            this.btnBrowse.TabIndex = 1;
            this.toolTip_Browse.SetToolTip(this.btnBrowse, "Click to Browse the Location to Save Generated Data");
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowseCodeGC_Click);
            // 
            // textGenPath
            // 
            this.textGenPath.Location = new System.Drawing.Point(83, 17);
            this.textGenPath.Name = "textGenPath";
            this.textGenPath.ReadOnly = true;
            this.textGenPath.Size = new System.Drawing.Size(192, 20);
            this.textGenPath.TabIndex = 0;
            this.toolTip_Path.SetToolTip(this.textGenPath, "Enter the Location to Save Generated Data");
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(67, 346);
            this.btnOk.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.Generate;
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(81, 28);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "Generate";
            this.btnOk.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip_Generate.SetToolTip(this.btnOk, "Click to Generate");
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnGenrateCodeGC_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(154, 346);
            this.btnCancel.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(81, 28);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip_Cancel.SetToolTip(this.btnCancel, "Click to Cancel Generation and Close the Window");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cheServiceInterface
            // 
            this.cheServiceInterface.AutoSize = true;
            this.cheServiceInterface.Location = new System.Drawing.Point(175, 82);
            this.cheServiceInterface.Name = "cheServiceInterface";
            this.cheServiceInterface.Size = new System.Drawing.Size(110, 17);
            this.cheServiceInterface.TabIndex = 4;
            this.cheServiceInterface.Text = "Service Interface ";
            this.toolTip_Service.SetToolTip(this.cheServiceInterface, "Check to Generate Service Interface");
            this.cheServiceInterface.UseVisualStyleBackColor = true;
            this.cheServiceInterface.CheckedChanged += new System.EventHandler(this.cheServiceInterface_CheckedChanged);
            // 
            // cheCompile
            // 
            this.cheCompile.AutoSize = true;
            this.cheCompile.Location = new System.Drawing.Point(175, 282);
            this.cheCompile.Name = "cheCompile";
            this.cheCompile.Size = new System.Drawing.Size(63, 17);
            this.cheCompile.TabIndex = 5;
            this.cheCompile.Text = "Compile";
            this.toolTip_Compile.SetToolTip(this.cheCompile, "Check to Compile the Generated Solution");
            this.cheCompile.UseVisualStyleBackColor = true;
            this.cheCompile.CheckedChanged += new System.EventHandler(this.cheCompile_CheckedChanged);
            // 
            // cheViewReport
            // 
            this.cheViewReport.AutoSize = true;
            this.cheViewReport.Location = new System.Drawing.Point(175, 308);
            this.cheViewReport.Name = "cheViewReport";
            this.cheViewReport.Size = new System.Drawing.Size(84, 17);
            this.cheViewReport.TabIndex = 6;
            this.cheViewReport.Text = "View Report";
            this.toolTip_ViewReport.SetToolTip(this.cheViewReport, "Check to View the Reports");
            this.cheViewReport.UseVisualStyleBackColor = true;
            this.cheViewReport.CheckedChanged += new System.EventHandler(this.cheViewReport_CheckedChanged);
            // 
            // CodeGenerationConfirmation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 377);
            this.Controls.Add(this.cheViewReport);
            this.Controls.Add(this.cheCompile);
            this.Controls.Add(this.cheServiceInterface);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox_LIFComponents);
            this.Controls.Add(this.groupBox_ServInterface);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CodeGenerationConfirmation";
            this.Text = "Code Generation";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.CodeGenerationConfirmation_FormClosed);
            this.Load += new System.EventHandler(this.CodeGenerationConfirmation_Load);
            this.groupBox_ServInterface.ResumeLayout(false);
            this.groupBox_ServInterface.PerformLayout();
            this.groupBox_LIFComponents.ResumeLayout(false);
            this.groupBox_LIFComponents.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox_ServInterface;
        private System.Windows.Forms.GroupBox groupBox_LIFComponents;
        private System.Windows.Forms.CheckBox cheDataEntitesSchemaContracts;
        private System.Windows.Forms.CheckBox cheDataEntitesSchemaModelObjects;
        private System.Windows.Forms.CheckBox cheSerializerClass;
        private System.Windows.Forms.CheckBox cheDataEntitesSchema;
        private System.Windows.Forms.CheckBox cheSerializerContracts;
        private System.Windows.Forms.CheckBox cheDataEntitesContracts;
        private System.Windows.Forms.CheckBox cheSerializerModelObjects;
        private System.Windows.Forms.CheckBox cheDataEntiesModelObjects;
        private System.Windows.Forms.CheckBox cheDataEntitiesClass;
        private System.Windows.Forms.RadioButton radWebServices;
        private System.Windows.Forms.RadioButton radWCF;
        private System.Windows.Forms.RadioButton radWfActivity;
        private System.Windows.Forms.RadioButton radHostAccess;        
        private System.Windows.Forms.RadioButton radRSSFeed;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox textGenPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox cheServiceInterface;
        private System.Windows.Forms.CheckBox cheCompile;
        private System.Windows.Forms.ToolTip toolTip_Path;
        private System.Windows.Forms.ToolTip toolTip_Browse;
        private System.Windows.Forms.ToolTip toolTip_Generate;
        private System.Windows.Forms.ToolTip toolTip_Cancel;
        private System.Windows.Forms.ToolTip toolTip_SerClass;
        private System.Windows.Forms.ToolTip toolTip_DEClass;
        private System.Windows.Forms.ToolTip toolTip_DESchemas;
        private System.Windows.Forms.ToolTip toolTip_Service;
        private System.Windows.Forms.ToolTip toolTip_Compile;
        private System.Windows.Forms.ToolTip toolTip_ServicePanel;
        private System.Windows.Forms.TextBox textSolnName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cheViewReport;
        private System.Windows.Forms.RadioButton radAtom;
        private System.Windows.Forms.ToolTip toolTip_ViewReport;
    }
}