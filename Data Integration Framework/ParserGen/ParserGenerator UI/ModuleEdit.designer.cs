namespace Infosys.Lif.LegacyParser.UI
{
	partial class ModuleEdit
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
            this.txtDataEntityNamespace = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtParserNamespace = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtModuleName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtServiceComponents = new System.Windows.Forms.TextBox();
            this.txtTestCaseNamespace = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtDataEntityNamespace
            // 
            this.txtDataEntityNamespace.Location = new System.Drawing.Point(227, 53);
            this.txtDataEntityNamespace.Name = "txtDataEntityNamespace";
            this.txtDataEntityNamespace.Size = new System.Drawing.Size(197, 20);
            this.txtDataEntityNamespace.TabIndex = 19;
            this.txtDataEntityNamespace.TextChanged += new System.EventHandler(this.Items_TextChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 60);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(119, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "Data Entity Namespace";
            // 
            // txtParserNamespace
            // 
            this.txtParserNamespace.Location = new System.Drawing.Point(227, 27);
            this.txtParserNamespace.Name = "txtParserNamespace";
            this.txtParserNamespace.Size = new System.Drawing.Size(197, 20);
            this.txtParserNamespace.TabIndex = 17;
            this.txtParserNamespace.TextChanged += new System.EventHandler(this.Items_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 34);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Serializer Namespace";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Module Name";
            // 
            // txtModuleName
            // 
            this.txtModuleName.Location = new System.Drawing.Point(227, 3);
            this.txtModuleName.Name = "txtModuleName";
            this.txtModuleName.Size = new System.Drawing.Size(197, 20);
            this.txtModuleName.TabIndex = 14;
            this.txtModuleName.TextChanged += new System.EventHandler(this.Items_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(165, 13);
            this.label1.TabIndex = 20;
            this.label1.Text = "Service Components Namespace";
            // 
            // txtServiceComponents
            // 
            this.txtServiceComponents.Location = new System.Drawing.Point(227, 80);
            this.txtServiceComponents.Name = "txtServiceComponents";
            this.txtServiceComponents.Size = new System.Drawing.Size(197, 20);
            this.txtServiceComponents.TabIndex = 21;
            // 
            // txtTestCaseNamespace
            // 
            this.txtTestCaseNamespace.Location = new System.Drawing.Point(227, 108);
            this.txtTestCaseNamespace.Name = "txtTestCaseNamespace";
            this.txtTestCaseNamespace.Size = new System.Drawing.Size(197, 20);
            this.txtTestCaseNamespace.TabIndex = 23;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 115);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(142, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Unit Test Cases Namespace";
            // 
            // ModuleEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtTestCaseNamespace);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtServiceComponents);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtDataEntityNamespace);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtParserNamespace);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtModuleName);
            this.Name = "ModuleEdit";
            this.Size = new System.Drawing.Size(538, 150);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txtDataEntityNamespace;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox txtParserNamespace;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox txtModuleName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtServiceComponents;
        private System.Windows.Forms.TextBox txtTestCaseNamespace;
        private System.Windows.Forms.Label label2;
	}
}
