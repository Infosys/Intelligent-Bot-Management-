namespace Infosys.Lif.LegacyParser.UI
{
	partial class LegacyParserFrontEnd
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
            System.Windows.Forms.StatusStrip statusStrip;
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Root");
            this.statusBar = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.entityMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.moduleMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteModuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rootNodeMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addModuleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.configurationsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtHostProgPaths = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnHstFilesBrowse = new System.Windows.Forms.Button();
            this.modulesListTree = new System.Windows.Forms.TreeView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.moduleEditor = new Infosys.Lif.LegacyParser.UI.ModuleEdit();
            this.entityEditor = new Infosys.Lif.LegacyParser.UI.EntityEdit();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.btnDestinationBrowse = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDestination = new System.Windows.Forms.TextBox();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.openDialog = new System.Windows.Forms.OpenFileDialog();
            this.destinationDialog = new System.Windows.Forms.FolderBrowserDialog();
            statusStrip = new System.Windows.Forms.StatusStrip();
            statusStrip.SuspendLayout();
            this.entityMenuStrip.SuspendLayout();
            this.moduleMenuStrip.SuspendLayout();
            this.rootNodeMenuStrip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBar,
            this.progressBar});
            statusStrip.Location = new System.Drawing.Point(0, 610);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new System.Drawing.Size(833, 32);
            statusStrip.SizingGrip = false;
            statusStrip.TabIndex = 14;
            statusStrip.Text = "statusStrip";
            // 
            // statusBar
            // 
            this.statusBar.AutoSize = false;
            this.statusBar.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
                        | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.statusBar.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.statusBar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(700, 27);
            this.statusBar.Text = "Loaded";
            this.statusBar.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(100, 26);
            // 
            // entityMenuStrip
            // 
            this.entityMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem4,
            this.toolStripMenuItem5});
            this.entityMenuStrip.Name = "rootNodeMenuStrip";
            this.entityMenuStrip.Size = new System.Drawing.Size(148, 48);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(147, 22);
            this.toolStripMenuItem4.Text = "Delete Entity";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.DeleteEntity_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripMenuItem5.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem6});
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(147, 22);
            this.toolStripMenuItem5.Text = "Move To";
            this.toolStripMenuItem5.MouseEnter += new System.EventHandler(this.entityTreeNodeContextMenuHoverHandler);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(179, 22);
            this.toolStripMenuItem6.Text = "toolStripMenuItem6";
            // 
            // moduleMenuStrip
            // 
            this.moduleMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3,
            this.deleteModuleToolStripMenuItem});
            this.moduleMenuStrip.Name = "rootNodeMenuStrip";
            this.moduleMenuStrip.Size = new System.Drawing.Size(154, 48);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(153, 22);
            this.toolStripMenuItem3.Text = "Edit Module";
            // 
            // deleteModuleToolStripMenuItem
            // 
            this.deleteModuleToolStripMenuItem.Name = "deleteModuleToolStripMenuItem";
            this.deleteModuleToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
            this.deleteModuleToolStripMenuItem.Text = "Delete Module";
            // 
            // rootNodeMenuStrip
            // 
            this.rootNodeMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addModuleToolStripMenuItem});
            this.rootNodeMenuStrip.Name = "rootNodeMenuStrip";
            this.rootNodeMenuStrip.Size = new System.Drawing.Size(142, 26);
            // 
            // addModuleToolStripMenuItem
            // 
            this.addModuleToolStripMenuItem.Name = "addModuleToolStripMenuItem";
            this.addModuleToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.addModuleToolStripMenuItem.Text = "Add Module";
            this.addModuleToolStripMenuItem.Click += new System.EventHandler(this.addModuleToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(833, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(35, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(109, 22);
            this.toolStripMenuItem1.Text = "&Load";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.LoadProject_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(109, 22);
            this.toolStripMenuItem2.Text = "&Save";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.saveMenuClickHandler);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(106, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(109, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.configurationsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.optionsToolStripMenuItem.Text = "Advanced O&ptions";
            this.optionsToolStripMenuItem.Click += new System.EventHandler(this.ChangeConfigurations);
            // 
            // configurationsToolStripMenuItem
            // 
            this.configurationsToolStripMenuItem.Name = "configurationsToolStripMenuItem";
            this.configurationsToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.configurationsToolStripMenuItem.Text = "&Configurations";
            this.configurationsToolStripMenuItem.Click += new System.EventHandler(this.configurationsToolStripMenuItem_Click);
            // 
            // txtHostProgPaths
            // 
            this.txtHostProgPaths.Location = new System.Drawing.Point(170, 12);
            this.txtHostProgPaths.Name = "txtHostProgPaths";
            this.txtHostProgPaths.Size = new System.Drawing.Size(301, 20);
            this.txtHostProgPaths.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(151, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Location of Host definition files";
            // 
            // btnHstFilesBrowse
            // 
            this.btnHstFilesBrowse.Location = new System.Drawing.Point(483, 8);
            this.btnHstFilesBrowse.Name = "btnHstFilesBrowse";
            this.btnHstFilesBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnHstFilesBrowse.TabIndex = 3;
            this.btnHstFilesBrowse.Text = "Browse...";
            this.btnHstFilesBrowse.Click += new System.EventHandler(this.btnHstFilesBrowse_Click);
            // 
            // modulesListTree
            // 
            this.modulesListTree.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.modulesListTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modulesListTree.HideSelection = false;
            this.modulesListTree.Location = new System.Drawing.Point(0, 0);
            this.modulesListTree.Name = "modulesListTree";
            treeNode2.Name = "Node0";
            treeNode2.Text = "Root";
            this.modulesListTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode2});
            this.modulesListTree.Size = new System.Drawing.Size(273, 466);
            this.modulesListTree.Sorted = true;
            this.modulesListTree.TabIndex = 7;
            this.modulesListTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.modulesListTree_AfterSelect);
            this.modulesListTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.modulesListTree_NodeMouseClick);
            this.modulesListTree.KeyDown += new System.Windows.Forms.KeyEventHandler(this.modulesListTree_KeyDown);
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.modulesListTree);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.moduleEditor);
            this.splitContainer1.Panel2.Controls.Add(this.entityEditor);
            this.splitContainer1.Size = new System.Drawing.Size(831, 470);
            this.splitContainer1.SplitterDistance = 277;
            this.splitContainer1.TabIndex = 9;
            this.splitContainer1.Text = "splitContainer1";
            // 
            // moduleEditor
            // 
            this.moduleEditor.Location = new System.Drawing.Point(4, 4);
            this.moduleEditor.Name = "moduleEditor";
            this.moduleEditor.Size = new System.Drawing.Size(538, 150);
            this.moduleEditor.TabIndex = 1;
            this.moduleEditor.Visible = false;
            // 
            // entityEditor
            // 
            this.entityEditor.AutoSize = true;
            this.entityEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.entityEditor.Location = new System.Drawing.Point(0, 0);
            this.entityEditor.Name = "entityEditor";
            this.entityEditor.Size = new System.Drawing.Size(546, 466);
            this.entityEditor.TabIndex = 0;
            this.entityEditor.Visible = false;
            this.entityEditor.MaximizeWorkSpace += new System.EventHandler(this.entityEditor_MaximizeWorkSpace);
            // 
            // splitContainer2
            // 
            this.splitContainer2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.btnDestinationBrowse);
            this.splitContainer2.Panel1.Controls.Add(this.label2);
            this.splitContainer2.Panel1.Controls.Add(this.txtDestination);
            this.splitContainer2.Panel1.Controls.Add(this.btnPreview);
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            this.splitContainer2.Panel1.Controls.Add(this.txtHostProgPaths);
            this.splitContainer2.Panel1.Controls.Add(this.btnHstFilesBrowse);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainer2.Size = new System.Drawing.Size(833, 549);
            this.splitContainer2.SplitterDistance = 73;
            this.splitContainer2.TabIndex = 10;
            this.splitContainer2.Text = "splitContainer2";
            // 
            // btnDestinationBrowse
            // 
            this.btnDestinationBrowse.Location = new System.Drawing.Point(483, 43);
            this.btnDestinationBrowse.Name = "btnDestinationBrowse";
            this.btnDestinationBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnDestinationBrowse.TabIndex = 12;
            this.btnDestinationBrowse.Text = "Browse...";
            this.btnDestinationBrowse.Click += new System.EventHandler(this.btnDestinationBrowse_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Destination Directory";
            // 
            // txtDestination
            // 
            this.txtDestination.Location = new System.Drawing.Point(170, 46);
            this.txtDestination.Name = "txtDestination";
            this.txtDestination.Size = new System.Drawing.Size(301, 20);
            this.txtDestination.TabIndex = 10;
            // 
            // btnPreview
            // 
            this.btnPreview.Location = new System.Drawing.Point(588, 9);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(139, 57);
            this.btnPreview.TabIndex = 7;
            this.btnPreview.Text = "Preview";
            this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
            // 
            // btnGenerate
            // 
            this.btnGenerate.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnGenerate.Enabled = false;
            this.btnGenerate.Location = new System.Drawing.Point(755, 6);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(75, 23);
            this.btnGenerate.TabIndex = 11;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer3.IsSplitterFixed = true;
            this.splitContainer3.Location = new System.Drawing.Point(0, 24);
            this.splitContainer3.Name = "splitContainer3";
            this.splitContainer3.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.Controls.Add(this.btnGenerate);
            this.splitContainer3.Size = new System.Drawing.Size(833, 586);
            this.splitContainer3.SplitterDistance = 549;
            this.splitContainer3.TabIndex = 13;
            this.splitContainer3.Text = "splitContainer3";
            // 
            // destinationDialog
            // 
            this.destinationDialog.Description = "Please select the destination for the generated files";
            this.destinationDialog.SelectedPath = "folderBrowserDialog1";
            // 
            // LegacyParserFrontEnd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(833, 642);
            this.Controls.Add(this.splitContainer3);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(statusStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "LegacyParserFrontEnd";
            this.Text = "Legacy Parser";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.LegacyParserFrontEnd_Load);
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            this.entityMenuStrip.ResumeLayout(false);
            this.moduleMenuStrip.ResumeLayout(false);
            this.rootNodeMenuStrip.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            this.splitContainer3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.TextBox txtHostProgPaths;
		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnHstFilesBrowse;
		private System.Windows.Forms.TreeView modulesListTree;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.SplitContainer splitContainer2;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.Button btnGenerate;
		private System.Windows.Forms.SplitContainer splitContainer3;
		private System.Windows.Forms.Button btnPreview;
		private System.Windows.Forms.ContextMenuStrip rootNodeMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem addModuleToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip moduleMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
		private System.Windows.Forms.ToolStripMenuItem deleteModuleToolStripMenuItem;
		private System.Windows.Forms.ContextMenuStrip entityMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.OpenFileDialog openDialog;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripStatusLabel statusBar;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.Button btnDestinationBrowse;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDestination;
        private System.Windows.Forms.FolderBrowserDialog destinationDialog;
        private ModuleEdit moduleEditor;
        private EntityEdit entityEditor;
        private System.Windows.Forms.ToolStripMenuItem configurationsToolStripMenuItem;
	}
}