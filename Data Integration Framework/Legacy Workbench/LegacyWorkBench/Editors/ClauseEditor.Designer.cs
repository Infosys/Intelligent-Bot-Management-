namespace Infosys.Lif.LegacyWorkbench.Editors
{
    partial class ClauseEditor
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
            System.Windows.Forms.Label label1;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label5;
            this.ListBox_All_DataItem = new System.Windows.Forms.ListBox();
            this.TextBox_ClauseEditor_DataItem = new System.Windows.Forms.TextBox();
            this.Button_ClauseEditor_Up = new System.Windows.Forms.Button();
            this.Button_ClauseEditor_Down = new System.Windows.Forms.Button();
            this.Button_ClauseEditor_Delete = new System.Windows.Forms.Button();
            this.Button_ClauseEditor_Add = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtProgramId = new System.Windows.Forms.Label();
            this.txtClauseName = new System.Windows.Forms.TextBox();
            this.txtItemLength = new System.Windows.Forms.TextBox();
            this.cmbItemType = new System.Windows.Forms.ComboBox();
            this.btnCancel = new System.Windows.Forms.Button();
            label1 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label1.Location = new System.Drawing.Point(327, 209);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(162, 17);
            label1.TabIndex = 7;
            label1.Text = "Name of The Data Item :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label2.Location = new System.Drawing.Point(327, 237);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(77, 17);
            label2.TabIndex = 8;
            label2.Text = "Item type : ";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label4.Location = new System.Drawing.Point(35, 16);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(74, 17);
            label4.TabIndex = 12;
            label4.Text = "Clause Id :";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label3.Location = new System.Drawing.Point(327, 264);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(90, 17);
            label3.TabIndex = 16;
            label3.Text = "Item Length :";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            label5.Location = new System.Drawing.Point(35, 48);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(100, 17);
            label5.TabIndex = 14;
            label5.Text = "Clause Name :";
            // 
            // ListBox_All_DataItem
            // 
            this.ListBox_All_DataItem.FormattingEnabled = true;
            this.ListBox_All_DataItem.Location = new System.Drawing.Point(3, 181);
            this.ListBox_All_DataItem.Name = "ListBox_All_DataItem";
            this.ListBox_All_DataItem.Size = new System.Drawing.Size(210, 264);
            this.ListBox_All_DataItem.TabIndex = 0;
            this.ListBox_All_DataItem.SelectedIndexChanged += new System.EventHandler(this.ListBox_All_DataItem_SelectedIndexChanged);
            // 
            // TextBox_ClauseEditor_DataItem
            // 
            this.TextBox_ClauseEditor_DataItem.Location = new System.Drawing.Point(507, 210);
            this.TextBox_ClauseEditor_DataItem.Name = "TextBox_ClauseEditor_DataItem";
            this.TextBox_ClauseEditor_DataItem.Size = new System.Drawing.Size(201, 20);
            this.TextBox_ClauseEditor_DataItem.TabIndex = 1;
            this.TextBox_ClauseEditor_DataItem.TextChanged += new System.EventHandler(this.TextBox_ClauseEditor_DataItem_TextChanged);
            // 
            // Button_ClauseEditor_Up
            // 
            this.Button_ClauseEditor_Up.Location = new System.Drawing.Point(219, 261);
            this.Button_ClauseEditor_Up.Name = "Button_ClauseEditor_Up";
            this.Button_ClauseEditor_Up.Size = new System.Drawing.Size(75, 23);
            this.Button_ClauseEditor_Up.TabIndex = 3;
            this.Button_ClauseEditor_Up.Text = "Up";
            this.Button_ClauseEditor_Up.UseVisualStyleBackColor = true;
            this.Button_ClauseEditor_Up.Click += new System.EventHandler(this.Button_Up_Click);
            // 
            // Button_ClauseEditor_Down
            // 
            this.Button_ClauseEditor_Down.Location = new System.Drawing.Point(219, 290);
            this.Button_ClauseEditor_Down.Name = "Button_ClauseEditor_Down";
            this.Button_ClauseEditor_Down.Size = new System.Drawing.Size(75, 23);
            this.Button_ClauseEditor_Down.TabIndex = 4;
            this.Button_ClauseEditor_Down.Text = "Down";
            this.Button_ClauseEditor_Down.UseVisualStyleBackColor = true;
            this.Button_ClauseEditor_Down.Click += new System.EventHandler(this.Button_Down_Click);
            // 
            // Button_ClauseEditor_Delete
            // 
            this.Button_ClauseEditor_Delete.Location = new System.Drawing.Point(219, 319);
            this.Button_ClauseEditor_Delete.Name = "Button_ClauseEditor_Delete";
            this.Button_ClauseEditor_Delete.Size = new System.Drawing.Size(75, 23);
            this.Button_ClauseEditor_Delete.TabIndex = 6;
            this.Button_ClauseEditor_Delete.Text = "Delete";
            this.Button_ClauseEditor_Delete.UseVisualStyleBackColor = true;
            this.Button_ClauseEditor_Delete.Click += new System.EventHandler(this.Button_Delete_Click);
            // 
            // Button_ClauseEditor_Add
            // 
            this.Button_ClauseEditor_Add.Location = new System.Drawing.Point(219, 348);
            this.Button_ClauseEditor_Add.Name = "Button_ClauseEditor_Add";
            this.Button_ClauseEditor_Add.Size = new System.Drawing.Size(75, 23);
            this.Button_ClauseEditor_Add.TabIndex = 10;
            this.Button_ClauseEditor_Add.Text = "Add";
            this.Button_ClauseEditor_Add.UseVisualStyleBackColor = true;
            this.Button_ClauseEditor_Add.Click += new System.EventHandler(this.Button_ClauseEditor_Add_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(230, 465);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(86, 30);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.Button_ClauseEditor_Save_Click);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.txtProgramId);
            this.panel1.Controls.Add(this.txtClauseName);
            this.panel1.Controls.Add(label5);
            this.panel1.Controls.Add(label4);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(705, 172);
            this.panel1.TabIndex = 14;
            // 
            // txtProgramId
            // 
            this.txtProgramId.AutoSize = true;
            this.txtProgramId.Location = new System.Drawing.Point(215, 16);
            this.txtProgramId.Name = "txtProgramId";
            this.txtProgramId.Size = new System.Drawing.Size(0, 13);
            this.txtProgramId.TabIndex = 16;
            // 
            // txtClauseName
            // 
            this.txtClauseName.Location = new System.Drawing.Point(215, 48);
            this.txtClauseName.Name = "txtClauseName";
            this.txtClauseName.Size = new System.Drawing.Size(201, 20);
            this.txtClauseName.TabIndex = 15;
            this.txtClauseName.TextChanged += new System.EventHandler(this.txtClauseName_TextChanged);
            // 
            // txtItemLength
            // 
            this.txtItemLength.Location = new System.Drawing.Point(507, 264);
            this.txtItemLength.Name = "txtItemLength";
            this.txtItemLength.Size = new System.Drawing.Size(201, 20);
            this.txtItemLength.TabIndex = 15;
            this.txtItemLength.TextChanged += new System.EventHandler(this.txtItemLength_TextChanged);
            // 
            // cmbItemType
            // 
            this.cmbItemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItemType.FormattingEnabled = true;
            this.cmbItemType.Items.AddRange(new object[] {
            "String",
            "Integer",
            "Date Time",
            "Boolean",
            "Float"});
            this.cmbItemType.Location = new System.Drawing.Point(507, 237);
            this.cmbItemType.Name = "cmbItemType";
            this.cmbItemType.Size = new System.Drawing.Size(201, 21);
            this.cmbItemType.TabIndex = 17;
            this.cmbItemType.SelectedIndexChanged += new System.EventHandler(this.cmbItemType_SelectedIndexChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(424, 465);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(96, 30);
            this.btnCancel.TabIndex = 18;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ClauseEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.cmbItemType);
            this.Controls.Add(label3);
            this.Controls.Add(this.txtItemLength);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.Button_ClauseEditor_Add);
            this.Controls.Add(label2);
            this.Controls.Add(label1);
            this.Controls.Add(this.Button_ClauseEditor_Delete);
            this.Controls.Add(this.Button_ClauseEditor_Down);
            this.Controls.Add(this.Button_ClauseEditor_Up);
            this.Controls.Add(this.TextBox_ClauseEditor_DataItem);
            this.Controls.Add(this.ListBox_All_DataItem);
            this.Name = "ClauseEditor";
            this.Size = new System.Drawing.Size(713, 509);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox ListBox_All_DataItem;
        private System.Windows.Forms.TextBox TextBox_ClauseEditor_DataItem;
        private System.Windows.Forms.Button Button_ClauseEditor_Up;
        private System.Windows.Forms.Button Button_ClauseEditor_Down;
        private System.Windows.Forms.Button Button_ClauseEditor_Delete;
        private System.Windows.Forms.Button Button_ClauseEditor_Add;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtItemLength;
        private System.Windows.Forms.ComboBox cmbItemType;
        private System.Windows.Forms.TextBox txtClauseName;
        private System.Windows.Forms.Label txtProgramId;
        private System.Windows.Forms.Button btnCancel;
    }
}
