using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Infosys.Lif.LegacyWorkbench.Editors
{
    public partial class ModuleEditor : Editor
    {


        public class ModifiedEventArgs : EventArgs
        {
            Entities.Module modifiedModule;

            public Entities.Module ModifiedModule
            {
                get { return modifiedModule; }
                set { modifiedModule = value; }
            }
        }

        public delegate void OnModified(object sender, ModifiedEventArgs e);

        public event OnModified Modified;

        public ModuleEditor()
        {
            InitializeComponent();
        }


        private Entities.Module Clone(Entities.Module entityToBeCloned)
        {
            System.IO.MemoryStream memStream = new System.IO.MemoryStream();
            System.Xml.Serialization.XmlSerializer serializer 
                = new System.Xml.Serialization.XmlSerializer(entityToBeCloned.GetType());
            serializer.Serialize(memStream, entityToBeCloned);

            memStream.Position = 0;


            Entities.Module entityDeserialized
                = (Entities.Module)serializer.Deserialize(memStream);
            return entityDeserialized;
        }


        Entities.Module moduleBeingEdited;
        Entities.Module originalUnModifiedModule;


        public void Populate(Entities.Module moduleToBeEdited)
        {

            originalUnModifiedModule = moduleToBeEdited;
            moduleBeingEdited = Clone(moduleToBeEdited);
            Populate_ModuleForm(moduleBeingEdited);
        }

        private void Populate_ModuleForm(Entities.Module moduleToBePopulated)
        {
            TextBox_ModuleEditor_ModuleName.Text
                = moduleToBePopulated.Name;
            TextBox_ModuleEditor_DataEntityNameSpace.Text
                = moduleBeingEdited.DataEntityNamespace;
            TextBox_ModuleEditor_SerializerNameSpace.Text
                = moduleToBePopulated.SerializerNamespace;            
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //to check whether Name of the Group textbox is left blank or not
            if (TextBox_ModuleEditor_ModuleName.Text == null || TextBox_ModuleEditor_ModuleName.Text.Trim().ToString() == "")
            {
                MessageBox.Show("The name of the Group cannot be left blank", "Blank ModuleName", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                TextBox_ModuleEditor_ModuleName.Focus();
                return;
            }
            //to get the changed name in the object moduleBeingEdited
            else
            {
                moduleBeingEdited.Name = TextBox_ModuleEditor_ModuleName.Text;
            }
            //check ends
            if (Modified != null)
            {
                originalUnModifiedModule.Name = moduleBeingEdited.Name;
                ModifiedEventArgs modifiedEventArgs = new ModifiedEventArgs();
                modifiedEventArgs.ModifiedModule = originalUnModifiedModule;
                Modified(this, modifiedEventArgs);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            moduleBeingEdited = Clone(originalUnModifiedModule);
            Populate_ModuleForm(moduleBeingEdited);
        }

        protected override void Save()
        {
            btnSave_Click(this, new EventArgs());
            IsDirty = false;
        }
        protected override void Cancel()
        {
            btnCancel_Click(this, new EventArgs());
            IsDirty = false;
        }


    }
}
