namespace Infosys.Lif.LegacyWorkbench.Editors
{
    partial class ContractEditor
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
            this.Label_ContractEditor_ModelObjectName = new System.Windows.Forms.Label();
            this.TreeBox_Contract = new System.Windows.Forms.TreeView();
            this.contractMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.cutToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.TextBox_ContractEditor_ModelObjectName = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ModelObjectPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.cmbModelObjectId = new System.Windows.Forms.ComboBox();
            this.lblInputModelObjectDefnReplacement = new System.Windows.Forms.Label();
            this.gridModelObjectItems = new System.Windows.Forms.DataGridView();
            this.contractPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.toolTip_Up = new System.Windows.Forms.ToolTip(this.components);
            this.Button_ContractEditor_Up = new System.Windows.Forms.Button();
            this.toolTip_Down = new System.Windows.Forms.ToolTip(this.components);
            this.Button_ContractEditor_Down = new System.Windows.Forms.Button();
            this.toolTip_Delete = new System.Windows.Forms.ToolTip(this.components);
            this.Button_ContractEditor_Delete = new System.Windows.Forms.Button();
            this.toolTip_Add = new System.Windows.Forms.ToolTip(this.components);
            this.Button_ContractEditor_Add = new System.Windows.Forms.Button();
            this.toolTip_Cut = new System.Windows.Forms.ToolTip(this.components);
            this.btnCut = new System.Windows.Forms.Button();
            this.toolTip_Copy = new System.Windows.Forms.ToolTip(this.components);
            this.btnCopy = new System.Windows.Forms.Button();
            this.toolTip_Paste = new System.Windows.Forms.ToolTip(this.components);
            this.btnPaste = new System.Windows.Forms.Button();
            this.toolTip_Save = new System.Windows.Forms.ToolTip(this.components);
            this.btnSave = new System.Windows.Forms.Button();
            this.toolTip_Cancel = new System.Windows.Forms.ToolTip(this.components);
            this.btnCancel = new System.Windows.Forms.Button();
            this.contractMenuStrip.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridModelObjectItems)).BeginInit();
            this.SuspendLayout();
            // 
            // Label_ContractEditor_ModelObjectName
            // 
            this.Label_ContractEditor_ModelObjectName.AutoSize = true;
            this.Label_ContractEditor_ModelObjectName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label_ContractEditor_ModelObjectName.Location = new System.Drawing.Point(3, 6);
            this.Label_ContractEditor_ModelObjectName.Name = "Label_ContractEditor_ModelObjectName";
            this.Label_ContractEditor_ModelObjectName.Size = new System.Drawing.Size(180, 17);
            this.Label_ContractEditor_ModelObjectName.TabIndex = 2;
            this.Label_ContractEditor_ModelObjectName.Text = "Name of the Model Object :";
            // 
            // TreeBox_Contract
            // 
            this.TreeBox_Contract.ContextMenuStrip = this.contractMenuStrip;
            this.TreeBox_Contract.Dock = System.Windows.Forms.DockStyle.Left;
            this.TreeBox_Contract.HideSelection = false;
            this.TreeBox_Contract.Location = new System.Drawing.Point(0, 0);
            this.TreeBox_Contract.Name = "TreeBox_Contract";
            this.TreeBox_Contract.Size = new System.Drawing.Size(240, 582);
            this.TreeBox_Contract.TabIndex = 0;
            this.TreeBox_Contract.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.TreeBox_Contract_NodeMouseDoubleClick);
            this.TreeBox_Contract.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TreeBox_Contract_MouseClick);
            this.TreeBox_Contract.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.TreeBox_Contract_AfterSelect);
            // 
            // contractMenuStrip
            // 
            this.contractMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripSeparator1,
            this.cutToolStripMenuItem2,
            this.copyToolStripMenuItem1,
            this.pasteToolStripMenuItem1});
            this.contractMenuStrip.Name = "contractMenuStrip";
            this.contractMenuStrip.Size = new System.Drawing.Size(118, 120);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.Add;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
            this.toolStripMenuItem1.Text = "&Add";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.Delete;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(117, 22);
            this.toolStripMenuItem2.Text = "&Remove";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(114, 6);
            // 
            // cutToolStripMenuItem2
            // 
            this.cutToolStripMenuItem2.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.Cut;
            this.cutToolStripMenuItem2.Name = "cutToolStripMenuItem2";
            this.cutToolStripMenuItem2.Size = new System.Drawing.Size(117, 22);
            this.cutToolStripMenuItem2.Text = "Cu&t";
            this.cutToolStripMenuItem2.Click += new System.EventHandler(this.cutToolStripMenuItem2_Click);
            // 
            // copyToolStripMenuItem1
            // 
            this.copyToolStripMenuItem1.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.Copy;
            this.copyToolStripMenuItem1.Name = "copyToolStripMenuItem1";
            this.copyToolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
            this.copyToolStripMenuItem1.Text = "&Copy";
            this.copyToolStripMenuItem1.Click += new System.EventHandler(this.copyToolStripMenuItem1_Click);
            // 
            // pasteToolStripMenuItem1
            // 
            this.pasteToolStripMenuItem1.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.paste;
            this.pasteToolStripMenuItem1.Name = "pasteToolStripMenuItem1";
            this.pasteToolStripMenuItem1.Size = new System.Drawing.Size(117, 22);
            this.pasteToolStripMenuItem1.Text = "&Paste";
            this.pasteToolStripMenuItem1.Click += new System.EventHandler(this.pasteToolStripMenuItem1_Click);
            // 
            // TextBox_ContractEditor_ModelObjectName
            // 
            this.TextBox_ContractEditor_ModelObjectName.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.TextBox_ContractEditor_ModelObjectName.Location = new System.Drawing.Point(304, 1);
            this.TextBox_ContractEditor_ModelObjectName.MaxLength = 5;
            this.TextBox_ContractEditor_ModelObjectName.Name = "TextBox_ContractEditor_ModelObjectName";
            this.TextBox_ContractEditor_ModelObjectName.Size = new System.Drawing.Size(91, 20);
            this.TextBox_ContractEditor_ModelObjectName.TabIndex = 1;
            this.TextBox_ContractEditor_ModelObjectName.Visible = false;
            this.TextBox_ContractEditor_ModelObjectName.TextChanged += new System.EventHandler(this.TextBox_ContractEditor_ModelObjectName_TextChanged);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.ModelObjectPropertyGrid);
            this.panel1.Controls.Add(this.cmbModelObjectId);
            this.panel1.Controls.Add(this.Label_ContractEditor_ModelObjectName);
            this.panel1.Controls.Add(this.lblInputModelObjectDefnReplacement);
            this.panel1.Controls.Add(this.gridModelObjectItems);
            this.panel1.Controls.Add(this.TextBox_ContractEditor_ModelObjectName);
            this.panel1.Location = new System.Drawing.Point(279, 217);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(400, 292);
            this.panel1.TabIndex = 13;
            // 
            // ModelObjectPropertyGrid
            // 
            this.ModelObjectPropertyGrid.Location = new System.Drawing.Point(3, 33);
            this.ModelObjectPropertyGrid.Name = "ModelObjectPropertyGrid";
            this.ModelObjectPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.ModelObjectPropertyGrid.Size = new System.Drawing.Size(392, 143);
            this.ModelObjectPropertyGrid.TabIndex = 18;
            this.ModelObjectPropertyGrid.ToolbarVisible = false;
            // 
            // cmbModelObjectId
            // 
            this.cmbModelObjectId.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModelObjectId.FormattingEnabled = true;
            this.cmbModelObjectId.Location = new System.Drawing.Point(191, 6);
            this.cmbModelObjectId.Name = "cmbModelObjectId";
            this.cmbModelObjectId.Size = new System.Drawing.Size(204, 21);
            this.cmbModelObjectId.Sorted = true;
            this.cmbModelObjectId.TabIndex = 17;
            this.cmbModelObjectId.SelectedIndexChanged += new System.EventHandler(this.cmbModelObjectId_SelectedIndexChanged);
            // 
            // lblInputModelObjectDefnReplacement
            // 
            this.lblInputModelObjectDefnReplacement.AutoSize = true;
            this.lblInputModelObjectDefnReplacement.Location = new System.Drawing.Point(5, 184);
            this.lblInputModelObjectDefnReplacement.Name = "lblInputModelObjectDefnReplacement";
            this.lblInputModelObjectDefnReplacement.Size = new System.Drawing.Size(139, 13);
            this.lblInputModelObjectDefnReplacement.TabIndex = 16;
            this.lblInputModelObjectDefnReplacement.Text = "Clause Definition Not Found";
            // 
            // gridModelObjectItems
            // 
            this.gridModelObjectItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.gridModelObjectItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridModelObjectItems.Location = new System.Drawing.Point(3, 182);
            this.gridModelObjectItems.Name = "gridModelObjectItems";
            this.gridModelObjectItems.Size = new System.Drawing.Size(392, 104);
            this.gridModelObjectItems.TabIndex = 15;
            // 
            // contractPropertyGrid
            // 
            this.contractPropertyGrid.Location = new System.Drawing.Point(280, 4);
            this.contractPropertyGrid.Name = "contractPropertyGrid";
            this.contractPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.contractPropertyGrid.Size = new System.Drawing.Size(400, 208);
            this.contractPropertyGrid.TabIndex = 16;
            this.contractPropertyGrid.ToolbarVisible = false;
            this.contractPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.contractPropertyGrid_PropertyValueChanged);
            // 
            // Button_ContractEditor_Up
            // 
            this.Button_ContractEditor_Up.FlatAppearance.BorderSize = 0;
            this.Button_ContractEditor_Up.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.Up;
            this.Button_ContractEditor_Up.Location = new System.Drawing.Point(249, 138);
            this.Button_ContractEditor_Up.Name = "Button_ContractEditor_Up";
            this.Button_ContractEditor_Up.Size = new System.Drawing.Size(21, 21);
            this.Button_ContractEditor_Up.TabIndex = 7;
            this.toolTip_Up.SetToolTip(this.Button_ContractEditor_Up, "Click to Move the Node Up");
            this.Button_ContractEditor_Up.UseVisualStyleBackColor = true;
            this.Button_ContractEditor_Up.Click += new System.EventHandler(this.Button_ContractEditor_Up_Click);
            // 
            // Button_ContractEditor_Down
            // 
            this.Button_ContractEditor_Down.FlatAppearance.BorderSize = 0;
            this.Button_ContractEditor_Down.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.Down;
            this.Button_ContractEditor_Down.Location = new System.Drawing.Point(249, 167);
            this.Button_ContractEditor_Down.Name = "Button_ContractEditor_Down";
            this.Button_ContractEditor_Down.Size = new System.Drawing.Size(21, 21);
            this.Button_ContractEditor_Down.TabIndex = 8;
            this.toolTip_Down.SetToolTip(this.Button_ContractEditor_Down, "Click to Move the Node Down");
            this.Button_ContractEditor_Down.UseVisualStyleBackColor = true;
            this.Button_ContractEditor_Down.Click += new System.EventHandler(this.Button_ContractEditor_Down_Click);
            // 
            // Button_ContractEditor_Delete
            // 
            this.Button_ContractEditor_Delete.FlatAppearance.BorderSize = 0;
            this.Button_ContractEditor_Delete.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.Delete;
            this.Button_ContractEditor_Delete.Location = new System.Drawing.Point(249, 225);
            this.Button_ContractEditor_Delete.Name = "Button_ContractEditor_Delete";
            this.Button_ContractEditor_Delete.Size = new System.Drawing.Size(21, 21);
            this.Button_ContractEditor_Delete.TabIndex = 9;
            this.toolTip_Delete.SetToolTip(this.Button_ContractEditor_Delete, "Click to Delete the Selected Node");
            this.Button_ContractEditor_Delete.UseVisualStyleBackColor = true;
            this.Button_ContractEditor_Delete.Click += new System.EventHandler(this.Button_ContractEditor_Delete_Click);
            // 
            // Button_ContractEditor_Add
            // 
            this.Button_ContractEditor_Add.FlatAppearance.BorderSize = 0;
            this.Button_ContractEditor_Add.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.Add;
            this.Button_ContractEditor_Add.Location = new System.Drawing.Point(249, 197);
            this.Button_ContractEditor_Add.Name = "Button_ContractEditor_Add";
            this.Button_ContractEditor_Add.Size = new System.Drawing.Size(21, 21);
            this.Button_ContractEditor_Add.TabIndex = 11;
            this.toolTip_Add.SetToolTip(this.Button_ContractEditor_Add, "Click to Add a Node");
            this.Button_ContractEditor_Add.UseVisualStyleBackColor = true;
            this.Button_ContractEditor_Add.Click += new System.EventHandler(this.Button_ContractEditor_Add_Click);
            // 
            // btnCut
            // 
            this.btnCut.FlatAppearance.BorderSize = 0;
            this.btnCut.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.Cut;
            this.btnCut.Location = new System.Drawing.Point(249, 254);
            this.btnCut.Name = "btnCut";
            this.btnCut.Size = new System.Drawing.Size(21, 21);
            this.btnCut.TabIndex = 18;
            this.toolTip_Cut.SetToolTip(this.btnCut, "Click to Cut the Node");
            this.btnCut.UseVisualStyleBackColor = true;
            this.btnCut.Click += new System.EventHandler(this.btnCut_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.FlatAppearance.BorderSize = 0;
            this.btnCopy.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.Copy;
            this.btnCopy.Location = new System.Drawing.Point(249, 283);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(21, 21);
            this.btnCopy.TabIndex = 17;
            this.toolTip_Copy.SetToolTip(this.btnCopy, "Click to Copy the Node");
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnPaste
            // 
            this.btnPaste.FlatAppearance.BorderSize = 0;
            this.btnPaste.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.paste;
            this.btnPaste.Location = new System.Drawing.Point(249, 312);
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(21, 21);
            this.btnPaste.TabIndex = 19;
            this.toolTip_Paste.SetToolTip(this.btnPaste, "Click to Paste the Node Under Selected Node");
            this.btnPaste.UseVisualStyleBackColor = true;
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // btnSave
            // 
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.Save;
            this.btnSave.Location = new System.Drawing.Point(522, 514);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 25);
            this.btnSave.TabIndex = 12;
            this.btnSave.Text = "Save";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip_Save.SetToolTip(this.btnSave, "Click to Save the Modified Data");
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.Image = global::Infosys.Lif.LegacyWorkbench.Properties.Resources.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(603, 514);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 25);
            this.btnCancel.TabIndex = 15;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.toolTip_Cancel.SetToolTip(this.btnCancel, "Click to Cancel Unsaved Changes");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ContractEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(this.btnPaste);
            this.Controls.Add(this.btnCut);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.contractPropertyGrid);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.Button_ContractEditor_Add);
            this.Controls.Add(this.Button_ContractEditor_Delete);
            this.Controls.Add(this.Button_ContractEditor_Down);
            this.Controls.Add(this.Button_ContractEditor_Up);
            this.Controls.Add(this.TreeBox_Contract);
            this.Name = "ContractEditor";
            this.Size = new System.Drawing.Size(686, 582);
            this.Leave += new System.EventHandler(this.ContractEditor_Leave);
            this.Enter += new System.EventHandler(this.ContractEditor_Enter);
            this.contractMenuStrip.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridModelObjectItems)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView TreeBox_Contract;
        private System.Windows.Forms.TextBox TextBox_ContractEditor_ModelObjectName;
        private System.Windows.Forms.Button Button_ContractEditor_Up;
        private System.Windows.Forms.Button Button_ContractEditor_Down;
        private System.Windows.Forms.Button Button_ContractEditor_Delete;
        private System.Windows.Forms.Button Button_ContractEditor_Add;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView gridModelObjectItems;
        private System.Windows.Forms.Label lblInputModelObjectDefnReplacement;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox cmbModelObjectId;
        private System.Windows.Forms.PropertyGrid contractPropertyGrid;
        private System.Windows.Forms.PropertyGrid ModelObjectPropertyGrid;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.Button btnCut;
        private System.Windows.Forms.Button btnPaste;
        private System.Windows.Forms.ToolTip toolTip_Up;
        private System.Windows.Forms.ToolTip toolTip_Down;
        private System.Windows.Forms.ToolTip toolTip_Delete;
        private System.Windows.Forms.ToolTip toolTip_Add;
        private System.Windows.Forms.ToolTip toolTip_Cut;
        private System.Windows.Forms.ToolTip toolTip_Copy;
        private System.Windows.Forms.ToolTip toolTip_Paste;
        private System.Windows.Forms.ToolTip toolTip_Save;
        private System.Windows.Forms.ToolTip toolTip_Cancel;
        private System.Windows.Forms.Label Label_ContractEditor_ModelObjectName;
        private System.Windows.Forms.ContextMenuStrip contractMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
    }
}
