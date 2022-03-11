namespace Infosys.Lif.LegacyWorkbench
{
    partial class WsdlLoadForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WsdlLoadForm));
            this.radUrl = new System.Windows.Forms.RadioButton();
            this.radFilePath = new System.Windows.Forms.RadioButton();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.txtFilePath = new System.Windows.Forms.TextBox();
            this.openWsdlDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panelLoadType = new System.Windows.Forms.Panel();
            this.toolTipOk = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipCancel = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipUrl = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipFilePath = new System.Windows.Forms.ToolTip(this.components);
            this.toolTipBrowse = new System.Windows.Forms.ToolTip(this.components);
            this.panelLoadType.SuspendLayout();
            this.SuspendLayout();
            // 
            // radUrl
            // 
            this.radUrl.AutoSize = true;
            this.radUrl.Location = new System.Drawing.Point(4, 10);
            this.radUrl.Name = "radUrl";
            this.radUrl.Size = new System.Drawing.Size(47, 17);
            this.radUrl.TabIndex = 0;
            this.radUrl.Text = "URL";
            this.radUrl.UseVisualStyleBackColor = true;
            this.radUrl.CheckedChanged += new System.EventHandler(this.radUrl_CheckedChanged);
            // 
            // radFilePath
            // 
            this.radFilePath.AutoSize = true;
            this.radFilePath.Location = new System.Drawing.Point(5, 38);
            this.radFilePath.Name = "radFilePath";
            this.radFilePath.Size = new System.Drawing.Size(66, 17);
            this.radFilePath.TabIndex = 1;
            this.radFilePath.TabStop = true;
            this.radFilePath.Text = "File Path";
            this.radFilePath.UseVisualStyleBackColor = true;
            this.radFilePath.CheckedChanged += new System.EventHandler(this.radFilePath_CheckedChanged);
            // 
            // txtUrl
            // 
            this.txtUrl.Location = new System.Drawing.Point(79, 6);
            this.txtUrl.MaxLength = 255;
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(254, 20);
            this.txtUrl.TabIndex = 2;
            this.toolTipUrl.SetToolTip(this.txtUrl, "Enter URL of the WDSL to be read");
            // 
            // txtFilePath
            // 
            this.txtFilePath.Location = new System.Drawing.Point(79, 34);
            this.txtFilePath.MaxLength = 255;
            this.txtFilePath.Name = "txtFilePath";
            this.txtFilePath.Size = new System.Drawing.Size(254, 20);
            this.txtFilePath.TabIndex = 3;
            this.toolTipFilePath.SetToolTip(this.txtFilePath, "Enter File Path of WSDL to be read");
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(339, 33);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(26, 23);
            this.btnBrowse.TabIndex = 4;
            this.btnBrowse.Text = "...";
            this.toolTipFilePath.SetToolTip(this.btnBrowse, "Browse the WSDL file");
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(87, 61);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 5;
            this.btnOk.Text = "OK";
            this.toolTipOk.SetToolTip(this.btnOk, "Click OK to load the WSDL specified by URL or file path");
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(201, 61);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.toolTipCancel.SetToolTip(this.btnCancel, "Cancel to close the form");
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // panelLoadType
            // 
            this.panelLoadType.Controls.Add(this.radFilePath);
            this.panelLoadType.Controls.Add(this.radUrl);
            this.panelLoadType.Cursor = System.Windows.Forms.Cursors.Default;
            this.panelLoadType.Location = new System.Drawing.Point(0, -3);
            this.panelLoadType.Name = "panelLoadType";
            this.panelLoadType.Size = new System.Drawing.Size(76, 59);
            this.panelLoadType.TabIndex = 7;
            // 
            // WsdlLoadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(366, 89);
            this.Controls.Add(this.panelLoadType);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtFilePath);
            this.Controls.Add(this.txtUrl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.ImeMode = System.Windows.Forms.ImeMode.On;
            this.Location = new System.Drawing.Point(320, 200);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "WsdlLoadForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "WsdlLoadForm";
            this.Load += new System.EventHandler(this.WsdlLoadForm_Load);
            this.panelLoadType.ResumeLayout(false);
            this.panelLoadType.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton radUrl;
        private System.Windows.Forms.RadioButton radFilePath;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.TextBox txtFilePath;
        private System.Windows.Forms.OpenFileDialog openWsdlDialog;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panelLoadType;
        private System.Windows.Forms.ToolTip toolTipOk;
        private System.Windows.Forms.ToolTip toolTipCancel;
        private System.Windows.Forms.ToolTip toolTipUrl;
        private System.Windows.Forms.ToolTip toolTipFilePath;
        private System.Windows.Forms.ToolTip toolTipBrowse;
    }
}