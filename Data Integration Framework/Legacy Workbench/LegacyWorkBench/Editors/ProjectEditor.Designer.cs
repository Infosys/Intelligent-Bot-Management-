namespace Infosys.Lif.LegacyWorkbench.Editors
{
    partial class ProjectEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label5;
            this.lblHostAccessNS = new System.Windows.Forms.Label();
            this.lblHostAccessRootNS = new System.Windows.Forms.Label();
            this.txtEntityNameSpace = new System.Windows.Forms.TextBox();
            this.txtSerializerNamespace = new System.Windows.Forms.TextBox();
            this.txtXmlSchemaNamespace = new System.Windows.Forms.TextBox();
            this.txtSerializerRootNamespace = new System.Windows.Forms.TextBox();
            this.txtEntityRootNamespace = new System.Windows.Forms.TextBox();
            this.txtHostAccessNamespace = new System.Windows.Forms.TextBox();
            this.txtHostAccessRootNameSpace = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.toolTip_XmlSchema = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_DataEntity = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_Serializer = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_DEAssembly = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_SerAssembly = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_HostAccess = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_HAAssembly = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_Save = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_Cancel = new System.Windows.Forms.ToolTip(this.components);
            this.grpBoxNamespaces = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtProjectPrefix = new System.Windows.Forms.TextBox();
            this.toolTip_ProjectPrefix = new System.Windows.Forms.ToolTip(this.components);
            label3 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            this.grpBoxNamespaces.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label3.Location = new System.Drawing.Point(11, 31);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(164, 17);
            label3.TabIndex = 4;
            label3.Text = "Data Entity Namespace :";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label1.Location = new System.Drawing.Point(11, 79);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(154, 17);
            label1.TabIndex = 6;
            label1.Text = "Serializer Namespace :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label2.Location = new System.Drawing.Point(24, 19);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(178, 17);
            label2.TabIndex = 8;
            label2.Text = "XML Schema Namespace :";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label4.Location = new System.Drawing.Point(11, 175);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(180, 17);
            label4.TabIndex = 10;
            label4.Text = "Serializer Assembly Name :";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label5.Location = new System.Drawing.Point(11, 127);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(186, 17);
            label5.TabIndex = 12;
            label5.Text = "Data Entity Assembly Name:";
            // 
            // lblHostAccessNS
            // 
            this.lblHostAccessNS.AutoSize = true;
            this.lblHostAccessNS.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHostAccessNS.Location = new System.Drawing.Point(11, 223);
            this.lblHostAccessNS.Name = "lblHostAccessNS";
            this.lblHostAccessNS.Size = new System.Drawing.Size(173, 17);
            this.lblHostAccessNS.TabIndex = 14;
            this.lblHostAccessNS.Text = "Host Access Namespace :";
            // 
            // lblHostAccessRootNS
            // 
            this.lblHostAccessRootNS.AutoSize = true;
            this.lblHostAccessRootNS.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHostAccessRootNS.Location = new System.Drawing.Point(11, 271);
            this.lblHostAccessRootNS.Name = "lblHostAccessRootNS";
            this.lblHostAccessRootNS.Size = new System.Drawing.Size(199, 17);
            this.lblHostAccessRootNS.TabIndex = 16;
            this.lblHostAccessRootNS.Text = "Host Access Assembly Name :";
            // 
            // txtEntityNameSpace
            // 
            this.txtEntityNameSpace.Location = new System.Drawing.Point(220, 30);
            this.txtEntityNameSpace.Name = "txtEntityNameSpace";
            this.txtEntityNameSpace.ReadOnly = true;
            this.txtEntityNameSpace.Size = new System.Drawing.Size(341, 20);
            this.txtEntityNameSpace.TabIndex = 1;
            this.toolTip_DataEntity.SetToolTip(this.txtEntityNameSpace, "Enter Data Entity Name Space");
            // 
            // txtSerializerNamespace
            // 
            this.txtSerializerNamespace.Location = new System.Drawing.Point(220, 78);
            this.txtSerializerNamespace.Name = "txtSerializerNamespace";
            this.txtSerializerNamespace.ReadOnly = true;
            this.txtSerializerNamespace.Size = new System.Drawing.Size(341, 20);
            this.txtSerializerNamespace.TabIndex = 2;
            this.toolTip_Serializer.SetToolTip(this.txtSerializerNamespace, "Enter Serializer Name Space");
            // 
            // txtXmlSchemaNamespace
            // 
            this.txtXmlSchemaNamespace.Location = new System.Drawing.Point(233, 18);
            this.txtXmlSchemaNamespace.Name = "txtXmlSchemaNamespace";
            this.txtXmlSchemaNamespace.ReadOnly = true;
            this.txtXmlSchemaNamespace.Size = new System.Drawing.Size(341, 20);
            this.txtXmlSchemaNamespace.TabIndex = 0;
            this.toolTip_XmlSchema.SetToolTip(this.txtXmlSchemaNamespace, "Enter XML Schema Name Space");
            // 
            // txtSerializerRootNamespace
            // 
            this.txtSerializerRootNamespace.Location = new System.Drawing.Point(220, 174);
            this.txtSerializerRootNamespace.Name = "txtSerializerRootNamespace";
            this.txtSerializerRootNamespace.ReadOnly = true;
            this.txtSerializerRootNamespace.Size = new System.Drawing.Size(341, 20);
            this.txtSerializerRootNamespace.TabIndex = 4;
            this.toolTip_SerAssembly.SetToolTip(this.txtSerializerRootNamespace, "Enter Serializer Assembly Name Space");
            // 
            // txtEntityRootNamespace
            // 
            this.txtEntityRootNamespace.Location = new System.Drawing.Point(220, 126);
            this.txtEntityRootNamespace.Name = "txtEntityRootNamespace";
            this.txtEntityRootNamespace.ReadOnly = true;
            this.txtEntityRootNamespace.Size = new System.Drawing.Size(341, 20);
            this.txtEntityRootNamespace.TabIndex = 3;
            this.toolTip_DEAssembly.SetToolTip(this.txtEntityRootNamespace, "Enter Data Entity Assembly Name Space");
            // 
            // txtHostAccessNamespace
            // 
            this.txtHostAccessNamespace.Location = new System.Drawing.Point(220, 222);
            this.txtHostAccessNamespace.Name = "txtHostAccessNamespace";
            this.txtHostAccessNamespace.ReadOnly = true;
            this.txtHostAccessNamespace.Size = new System.Drawing.Size(341, 20);
            this.txtHostAccessNamespace.TabIndex = 5;
            this.toolTip_HAAssembly.SetToolTip(this.txtHostAccessNamespace, "Enter Host Access Name Space");
            this.toolTip_HostAccess.SetToolTip(this.txtHostAccessNamespace, "Enter Host Access Name Space");
            // 
            // txtHostAccessRootNameSpace
            // 
            this.txtHostAccessRootNameSpace.Location = new System.Drawing.Point(220, 270);
            this.txtHostAccessRootNameSpace.Name = "txtHostAccessRootNameSpace";
            this.txtHostAccessRootNameSpace.ReadOnly = true;
            this.txtHostAccessRootNameSpace.Size = new System.Drawing.Size(341, 20);
            this.txtHostAccessRootNameSpace.TabIndex = 6;
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.Save;
            this.btnSave.Location = new System.Drawing.Point(418, 419);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 25);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip_Save.SetToolTip(this.btnSave, "Click to Save The Modified Data");
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(499, 419);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip_Cancel.SetToolTip(this.btnCancel, "Click to Cancel Unsaved Changes");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // grpBoxNamespaces
            // 
            this.grpBoxNamespaces.Controls.Add(this.txtHostAccessRootNameSpace);
            this.grpBoxNamespaces.Controls.Add(this.lblHostAccessRootNS);
            this.grpBoxNamespaces.Controls.Add(this.txtHostAccessNamespace);
            this.grpBoxNamespaces.Controls.Add(this.lblHostAccessNS);
            this.grpBoxNamespaces.Controls.Add(this.txtEntityRootNamespace);
            this.grpBoxNamespaces.Controls.Add(label5);
            this.grpBoxNamespaces.Controls.Add(this.txtSerializerRootNamespace);
            this.grpBoxNamespaces.Controls.Add(label4);
            this.grpBoxNamespaces.Controls.Add(this.txtSerializerNamespace);
            this.grpBoxNamespaces.Controls.Add(label1);
            this.grpBoxNamespaces.Controls.Add(this.txtEntityNameSpace);
            this.grpBoxNamespaces.Controls.Add(label3);
            this.grpBoxNamespaces.Location = new System.Drawing.Point(13, 97);
            this.grpBoxNamespaces.Name = "grpBoxNamespaces";
            this.grpBoxNamespaces.Size = new System.Drawing.Size(574, 302);
            this.grpBoxNamespaces.TabIndex = 17;
            this.grpBoxNamespaces.TabStop = false;
            this.grpBoxNamespaces.Text = "Project Namespaces";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(24, 64);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(91, 17);
            this.label6.TabIndex = 18;
            this.label6.Text = "Project Prefix";
            // 
            // txtProjectPrefix
            // 
            this.txtProjectPrefix.Location = new System.Drawing.Point(233, 64);
            this.txtProjectPrefix.MaxLength = 30;
            this.txtProjectPrefix.Name = "txtProjectPrefix";
            this.txtProjectPrefix.Size = new System.Drawing.Size(341, 20);
            this.txtProjectPrefix.TabIndex = 19;
            this.toolTip_ProjectPrefix.SetToolTip(this.txtProjectPrefix, "Enter Project Prefix for Namespaces");
            this.txtProjectPrefix.TextChanged += new System.EventHandler(this.txtProjectPrefix_TextChanged);
            this.txtProjectPrefix.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtProjectPrefix_KeyPress);
            // 
            // ProjectEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.txtProjectPrefix);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.grpBoxNamespaces);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtXmlSchemaNamespace);
            this.Controls.Add(label2);
            this.Name = "ProjectEditor";
            this.Size = new System.Drawing.Size(601, 465);
            this.Load += new System.EventHandler(this.ProjectEditor_Load);
            this.grpBoxNamespaces.ResumeLayout(false);
            this.grpBoxNamespaces.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtEntityNameSpace;
        private System.Windows.Forms.TextBox txtSerializerNamespace;
        private System.Windows.Forms.TextBox txtXmlSchemaNamespace;
        private System.Windows.Forms.TextBox txtSerializerRootNamespace;
        private System.Windows.Forms.TextBox txtEntityRootNamespace;
        private System.Windows.Forms.TextBox txtHostAccessNamespace;
        private System.Windows.Forms.TextBox txtHostAccessRootNameSpace;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblHostAccessNS;
        private System.Windows.Forms.Label lblHostAccessRootNS;
        private System.Windows.Forms.ToolTip toolTip_XmlSchema;
        private System.Windows.Forms.ToolTip toolTip_DataEntity;
        private System.Windows.Forms.ToolTip toolTip_Serializer;
        private System.Windows.Forms.ToolTip toolTip_DEAssembly;
        private System.Windows.Forms.ToolTip toolTip_SerAssembly;
        private System.Windows.Forms.ToolTip toolTip_HostAccess;
        private System.Windows.Forms.ToolTip toolTip_HAAssembly;
        private System.Windows.Forms.ToolTip toolTip_Save;
        private System.Windows.Forms.ToolTip toolTip_Cancel;
        private System.Windows.Forms.GroupBox grpBoxNamespaces;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtProjectPrefix;
        private System.Windows.Forms.ToolTip toolTip_ProjectPrefix;
    }
}
