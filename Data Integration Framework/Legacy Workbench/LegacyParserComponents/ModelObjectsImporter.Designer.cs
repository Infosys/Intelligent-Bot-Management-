namespace Infosys.Lif.LegacyWorkbench
{
    partial class ModelObjectsImporter
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
            this.txtHostProgPaths = new System.Windows.Forms.TextBox();
            this.btnHstFilesBrowse = new System.Windows.Forms.Button();
            this.openDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolTip_ModelObj = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_Browse = new System.Windows.Forms.ToolTip(this.components);
            this.progressBar_BrowseModelObjects = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Location of Model Object Files :";
            // 
            // txtHostProgPaths
            // 
            this.txtHostProgPaths.Location = new System.Drawing.Point(184, 15);
            this.txtHostProgPaths.Name = "txtHostProgPaths";
            this.txtHostProgPaths.ReadOnly = true;
            this.txtHostProgPaths.Size = new System.Drawing.Size(302, 20);
            this.txtHostProgPaths.TabIndex = 10;
            // 
            // btnHstFilesBrowse
            // 
            this.btnHstFilesBrowse.FlatAppearance.BorderSize = 0;
            this.btnHstFilesBrowse.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHstFilesBrowse.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.Browse;
            this.btnHstFilesBrowse.Location = new System.Drawing.Point(484, 15);
            this.btnHstFilesBrowse.Name = "btnHstFilesBrowse";
            this.btnHstFilesBrowse.Size = new System.Drawing.Size(23, 20);
            this.btnHstFilesBrowse.TabIndex = 12;
            this.toolTip_Browse.SetToolTip(this.btnHstFilesBrowse, "Click to Browse and Preview Model Objects");
            this.btnHstFilesBrowse.Click += new System.EventHandler(this.btnHstFilesBrowse_Click);
            // 
            // openDialog
            // 
            this.openDialog.Filter = "ST Files|*.st|Cobol Files|*.cob|All Files|*.*";
            this.openDialog.Multiselect = true;
            this.openDialog.Title = "ModelObject Definition Files";
            // 
            // progressBar_BrowseModelObjects
            // 
            this.progressBar_BrowseModelObjects.Location = new System.Drawing.Point(523, 18);
            this.progressBar_BrowseModelObjects.Name = "progressBar_BrowseModelObjects";
            this.progressBar_BrowseModelObjects.Size = new System.Drawing.Size(150, 13);
            this.progressBar_BrowseModelObjects.Step = 1;
            this.progressBar_BrowseModelObjects.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar_BrowseModelObjects.TabIndex = 13;
            this.progressBar_BrowseModelObjects.Visible = false;
            // 
            // ModelObjectsImporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.progressBar_BrowseModelObjects);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtHostProgPaths);
            this.Controls.Add(this.btnHstFilesBrowse);
            this.Name = "ModelObjectsImporter";
            this.Size = new System.Drawing.Size(710, 50);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtHostProgPaths;
        private System.Windows.Forms.Button btnHstFilesBrowse;
        private System.Windows.Forms.OpenFileDialog openDialog;
        private System.Windows.Forms.ToolTip toolTip_ModelObj;
        private System.Windows.Forms.ToolTip toolTip_Browse;
        private System.Windows.Forms.ProgressBar progressBar_BrowseModelObjects;
    }
}
