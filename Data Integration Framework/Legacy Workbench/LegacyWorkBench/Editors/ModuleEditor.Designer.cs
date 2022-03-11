namespace Infosys.Lif.LegacyWorkbench.Editors
{
    partial class ModuleEditor
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.TextBox_ModuleEditor_ModuleName = new System.Windows.Forms.TextBox();
            this.TextBox_ModuleEditor_SerializerNameSpace = new System.Windows.Forms.TextBox();
            this.TextBox_ModuleEditor_DataEntityNameSpace = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.toolTip_GrpName = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_SerializerNS = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_HostEntityNS = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_Save = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_Cancel = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 113);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(165, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Host Entity NameSpace :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(14, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(160, 17);
            this.label2.TabIndex = 1;
            this.label2.Text = "Serializer NameSpace : ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(14, 14);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(157, 17);
            this.label3.TabIndex = 2;
            this.label3.Text = "Name  of  the  Group :  ";
            // 
            // TextBox_ModuleEditor_ModuleName
            // 
            this.TextBox_ModuleEditor_ModuleName.Location = new System.Drawing.Point(184, 13);
            this.TextBox_ModuleEditor_ModuleName.MaxLength = 30;
            this.TextBox_ModuleEditor_ModuleName.Name = "TextBox_ModuleEditor_ModuleName";
            this.TextBox_ModuleEditor_ModuleName.Size = new System.Drawing.Size(320, 20);
            this.TextBox_ModuleEditor_ModuleName.TabIndex = 3;
            this.toolTip_GrpName.SetToolTip(this.TextBox_ModuleEditor_ModuleName, "Enter the Name of Group");
            // 
            // TextBox_ModuleEditor_SerializerNameSpace
            // 
            this.TextBox_ModuleEditor_SerializerNameSpace.Location = new System.Drawing.Point(184, 62);
            this.TextBox_ModuleEditor_SerializerNameSpace.Name = "TextBox_ModuleEditor_SerializerNameSpace";
            this.TextBox_ModuleEditor_SerializerNameSpace.ReadOnly = true;
            this.TextBox_ModuleEditor_SerializerNameSpace.Size = new System.Drawing.Size(320, 20);
            this.TextBox_ModuleEditor_SerializerNameSpace.TabIndex = 4;
            this.toolTip_SerializerNS.SetToolTip(this.TextBox_ModuleEditor_SerializerNameSpace, "Enter Serializer Name Space");
            // 
            // TextBox_ModuleEditor_DataEntityNameSpace
            // 
            this.TextBox_ModuleEditor_DataEntityNameSpace.Location = new System.Drawing.Point(184, 113);
            this.TextBox_ModuleEditor_DataEntityNameSpace.Name = "TextBox_ModuleEditor_DataEntityNameSpace";
            this.TextBox_ModuleEditor_DataEntityNameSpace.ReadOnly = true;
            this.TextBox_ModuleEditor_DataEntityNameSpace.Size = new System.Drawing.Size(320, 20);
            this.TextBox_ModuleEditor_DataEntityNameSpace.TabIndex = 5;
            this.toolTip_HostEntityNS.SetToolTip(this.TextBox_ModuleEditor_DataEntityNameSpace, "Enter Host Entity Name Space");
            // 
            // btnSave
            // 
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.Save;
            this.btnSave.Location = new System.Drawing.Point(348, 168);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 25);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip_Save.SetToolTip(this.btnSave, "Click to Save Modified Data");
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(429, 168);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip_Cancel.SetToolTip(this.btnCancel, "Click to Cancel Unsaved Changes");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ModuleEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.TextBox_ModuleEditor_DataEntityNameSpace);
            this.Controls.Add(this.TextBox_ModuleEditor_SerializerNameSpace);
            this.Controls.Add(this.TextBox_ModuleEditor_ModuleName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ModuleEditor";
            this.Size = new System.Drawing.Size(521, 215);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox TextBox_ModuleEditor_SerializerNameSpace;
        private System.Windows.Forms.TextBox TextBox_ModuleEditor_DataEntityNameSpace;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        protected System.Windows.Forms.TextBox TextBox_ModuleEditor_ModuleName;
        private System.Windows.Forms.ToolTip toolTip_GrpName;
        private System.Windows.Forms.ToolTip toolTip_SerializerNS;
        private System.Windows.Forms.ToolTip toolTip_HostEntityNS;
        private System.Windows.Forms.ToolTip toolTip_Save;
        private System.Windows.Forms.ToolTip toolTip_Cancel;
    }
}
