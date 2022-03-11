namespace Infosys.Lif.LegacyWorkbench.Editors
{
    partial class ModelObjectEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ModelObjectEditor));
            this.btnSave = new System.Windows.Forms.Button();
            this.modelObjectProperties = new System.Windows.Forms.PropertyGrid();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.Location = new System.Drawing.Point(56, 243);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 25);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "Save";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.Button_ModelObjectEditor_Save_Click);
            // 
            // modelObjectProperties
            // 
            this.modelObjectProperties.Location = new System.Drawing.Point(3, 3);
            this.modelObjectProperties.Name = "modelObjectProperties";
            this.modelObjectProperties.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.modelObjectProperties.Size = new System.Drawing.Size(376, 211);
            this.modelObjectProperties.TabIndex = 17;
            this.modelObjectProperties.ToolbarVisible = false;
            this.modelObjectProperties.MouseClick += new System.Windows.Forms.MouseEventHandler(this.modelObjectProperties_MouseClick);
            this.modelObjectProperties.Enter += new System.EventHandler(this.modelObjectProperties_Enter);
            this.modelObjectProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.modelObjectProperties_PropertyValueChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.Location = new System.Drawing.Point(240, 243);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ModelObjectEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.modelObjectProperties);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Name = "ModelObjectEditor";
            this.Size = new System.Drawing.Size(382, 298);
            this.Leave += new System.EventHandler(this.ModelObjectEditor_Leave);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.PropertyGrid modelObjectProperties;
    }
}
