using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Infosys.Lif.LegacyWorkbench.Editors
{
    public  class Editor : UserControl
    {


        public Editor()
        {
            isDirty = false;
            VisibleChanged += new EventHandler(Editor_VisibleChanged);
        }


        protected virtual void Save()
        {
             
        }
        protected virtual void Cancel()
        {

        }

        static private string cancelClicked;

        public static string CancelClicked
        {
            get { return Editor.cancelClicked; }
            set { Editor.cancelClicked = value; }
        }

        void Editor_VisibleChanged(object sender, EventArgs e)
        {
            if (!Visible && IsDirty)
            {
                //MessageBox.Show("There are some unsaved changes.Please take care of that first then go to next node");

                //DialogResult result;
                //result = MessageBox.Show("There are unsaved changes. Moving to another screen will not save these changes",
                //                Application.ProductName, MessageBoxButtons.YesNo);

                //if (result == Cancel)
                //{
                    
                //}
                //if (DialogResult.Yes == result)
                //{
                //    ProjectEditor projectEditorObj = new ProjectEditor();
                //    projectEditorObj.btnSave_Click(sender, e);
                //    return;
                //}
                if (sender.GetType().FullName.ToString() ==
                "Infosys.Lif.LegacyWorkbench.Editors.ModuleEditor")
                {
                    ModuleEditor moduleEditor = (ModuleEditor)sender;
                    moduleEditor.Cancel();
                }
                if (sender.GetType().FullName.ToString() ==
                "Infosys.Lif.LegacyWorkbench.Editors.ModelObjectEditor")
                {
                    ModelObjectEditor modelObjEditor = (ModelObjectEditor)sender;
                    modelObjEditor.Cancel();
                }
                if (sender.GetType().FullName.ToString() ==
                "Infosys.Lif.LegacyWorkbench.Editors.ContractEditor")
                {
                    ContractEditor contractEditor = (ContractEditor)sender;
                    contractEditor.Cancel();
                }
                if (sender.GetType().FullName.ToString() ==
                "Infosys.Lif.LegacyWorkbench.Editors.ProjectEditor")
                {
                    ProjectEditor projectEditor = (ProjectEditor)sender;
                    projectEditor.Cancel();
                }

                //if (MessageBox.Show("There are unsaved changes. Do you want to save these changes?",
                //    "Save changes?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                //{
                //    if (sender.GetType().FullName.ToString() ==
                //        "Infosys.Lif.LegacyWorkbench.Editors.ModuleEditor")
                //    {
                //        ModuleEditor mod = (ModuleEditor)sender;
                        
                //        mod.Save();
                //    }
                //}
                //else
                //{
                //    if (sender.GetType().FullName.ToString() ==
                //    "Infosys.Lif.LegacyWorkbench.Editors.ModuleEditor")
                //    {
                //        ModuleEditor mod = (ModuleEditor)sender;
                //        mod.Cancel();
                //    }

                //}

            }
        }
        bool isDirty;

        protected bool IsDirty
        {
            get { return isDirty; }
            set { isDirty = value; }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Editor
            // 
            this.Name = "Editor";
            this.Size = new System.Drawing.Size(438, 335);
            this.ResumeLayout(false);

        }
    }
}
