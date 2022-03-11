namespace Infosys.Lif.LegacyWorkbench
{
    partial class ContractsImporter
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
            this.txtContractsFileLocations = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.openDialog = new System.Windows.Forms.OpenFileDialog();
            this.toolTip_Contracts = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip_Browse = new System.Windows.Forms.ToolTip(this.components);
            this.progressBar_BrowseContracts = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // txtContractsFileLocations
            // 
            this.txtContractsFileLocations.Location = new System.Drawing.Point(184, 15);
            this.txtContractsFileLocations.Name = "txtContractsFileLocations";
            this.txtContractsFileLocations.ReadOnly = true;
            this.txtContractsFileLocations.Size = new System.Drawing.Size(302, 20);
            this.txtContractsFileLocations.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.Browse;
            this.button1.Location = new System.Drawing.Point(484, 15);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(23, 20);
            this.button1.TabIndex = 1;
            this.toolTip_Browse.SetToolTip(this.button1, "Click to Browse and Preview Contracts");
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnHstFilesBrowse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Location of Contract Files :";
            // 
            // openDialog
            // 
            this.openDialog.Filter = "Text Files|*.txt|CSV Files|*.csv|All Files|*.*";
            this.openDialog.Multiselect = true;
            // 
            // progressBar_BrowseContracts
            // 
            this.progressBar_BrowseContracts.Location = new System.Drawing.Point(523, 18);
            this.progressBar_BrowseContracts.Name = "progressBar_BrowseContracts";
            this.progressBar_BrowseContracts.Size = new System.Drawing.Size(150, 13);
            this.progressBar_BrowseContracts.Step = 1;
            this.progressBar_BrowseContracts.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar_BrowseContracts.TabIndex = 3;
            this.progressBar_BrowseContracts.Visible = false;
            // 
            // ContractsImporter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.progressBar_BrowseContracts);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtContractsFileLocations);
            this.Name = "ContractsImporter";
            this.Size = new System.Drawing.Size(710, 50);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtContractsFileLocations;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.OpenFileDialog openDialog;
        private System.Windows.Forms.ToolTip toolTip_Contracts;
        private System.Windows.Forms.ToolTip toolTip_Browse;
        private System.Windows.Forms.ProgressBar progressBar_BrowseContracts;
    }
}
