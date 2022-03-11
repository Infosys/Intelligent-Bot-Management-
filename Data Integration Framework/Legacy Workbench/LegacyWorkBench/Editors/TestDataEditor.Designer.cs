namespace Infosys.Lif.LegacyWorkbench.Editors
{
    partial class TestDataEditor
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
            this.TreeBox_Contract = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblInputModelObjectDefnReplacement = new System.Windows.Forms.Label();
            this.gridModelObjectItems = new System.Windows.Forms.DataGridView();
            this.btnGenerateData = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridModelObjectItems)).BeginInit();
            this.SuspendLayout();
            // 
            // TreeBox_Contract
            // 
            this.TreeBox_Contract.Location = new System.Drawing.Point(0, 0);
            this.TreeBox_Contract.Name = "TreeBox_Contract";
            this.TreeBox_Contract.Size = new System.Drawing.Size(240, 582);
            this.TreeBox_Contract.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lblInputModelObjectDefnReplacement);
            this.panel1.Controls.Add(this.gridModelObjectItems);
            this.panel1.Location = new System.Drawing.Point(246, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(437, 541);
            this.panel1.TabIndex = 17;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(272, 270);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(27, 13);
            this.label8.TabIndex = 24;
            this.label8.Text = "Max";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(188, 270);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(24, 13);
            this.label7.TabIndex = 23;
            this.label7.Text = "Min";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 408);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 13);
            this.label5.TabIndex = 21;
            this.label5.Text = "No. of Rows";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 375);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 13);
            this.label4.TabIndex = 20;
            this.label4.Text = "Seed Value";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 340);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Regular Expression";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 270);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Range";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 227);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(139, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Test Data Generation Rules";
            // 
            // lblInputModelObjectDefnReplacement
            // 
            this.lblInputModelObjectDefnReplacement.AutoSize = true;
            this.lblInputModelObjectDefnReplacement.Location = new System.Drawing.Point(5, 5);
            this.lblInputModelObjectDefnReplacement.Name = "lblInputModelObjectDefnReplacement";
            this.lblInputModelObjectDefnReplacement.Size = new System.Drawing.Size(139, 13);
            this.lblInputModelObjectDefnReplacement.TabIndex = 16;
            this.lblInputModelObjectDefnReplacement.Text = "Clause Definition Not Found";
            // 
            // gridModelObjectItems
            // 
            this.gridModelObjectItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.gridModelObjectItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridModelObjectItems.Location = new System.Drawing.Point(3, 3);
            this.gridModelObjectItems.Name = "gridModelObjectItems";
            this.gridModelObjectItems.Size = new System.Drawing.Size(429, 200);
            this.gridModelObjectItems.TabIndex = 15;
            // 
            // btnGenerateData
            // 
            this.btnGenerateData.Location = new System.Drawing.Point(491, 550);
            this.btnGenerateData.Name = "btnGenerateData";
            this.btnGenerateData.Size = new System.Drawing.Size(86, 23);
            this.btnGenerateData.TabIndex = 18;
            this.btnGenerateData.Text = "Generate Data";
            this.btnGenerateData.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(592, 550);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 23);
            this.btnCancel.TabIndex = 19;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(27, 302);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 13);
            this.label6.TabIndex = 25;
            this.label6.Text = "Length";
            // 
            // TestDataEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnGenerateData);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.TreeBox_Contract);
            this.Name = "TestDataEditor";
            this.Size = new System.Drawing.Size(686, 582);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridModelObjectItems)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView TreeBox_Contract;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblInputModelObjectDefnReplacement;
        private System.Windows.Forms.DataGridView gridModelObjectItems;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGenerateData;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label6;
    }
}
