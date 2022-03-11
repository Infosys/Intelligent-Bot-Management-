using System;

using System.Windows.Forms;

namespace Infosys.Lif.LegacyWorkbench.Editors
{
    public partial class ClauseEditor : Editors.Editor
    {
        public class ModifiedEventArgs : EventArgs
        {
            Entities.Entity modifiedEntity;

            public Entities.Entity ModifiedEntity
            {
                get { return modifiedEntity; }
                set { modifiedEntity = value; }
            }
        }

        public delegate void OnModified(object sender, ModifiedEventArgs e);

        public event OnModified Modified;

        public Entities.Entity entityBoundTo;
        public Entities.Entity originalUnModifiedEntity;

        public ClauseEditor()
        {
            InitializeComponent();
        }



        public void Populate(Entities.Entity entityToBeEdited)
        {
            originalUnModifiedEntity = entityToBeEdited;
            entityBoundTo = Clone(entityToBeEdited);
            Populate_ListBox(entityBoundTo);
            //ListBox_All_DataItem.SelectedIndex = 0;
        }

        private Entities.Entity Clone(Entities.Entity entityToBeCloned)
        {
            System.IO.MemoryStream memStream = new System.IO.MemoryStream();
            System.Xml.Serialization.XmlSerializer serializer
                = new System.Xml.Serialization.XmlSerializer(typeof(Entities.Entity));
            serializer.Serialize(memStream, entityToBeCloned);

            memStream.Position = 0;


            Entities.Entity entityDeserialized
                = (Entities.Entity)serializer.Deserialize(memStream);
            return entityDeserialized;
        }


        private void Populate_ListBox(Entities.Entity ent)
        {


            txtProgramId.Text = ent.ProgramId;
            txtClauseName.Text = ent.EntityName;


            BindListBox(ent);
            ////if (ListBox_All_DataItem.Items.Count > 0)
            ////{
            ////    ListBox_All_DataItem.SelectedIndex = 0;
            ////}
        }

        private void BindListBox(Entities.Entity ent)
        {
            ListBox_All_DataItem.Items.Clear();
            Entities.GenericCollection<Entities.DataItem> items = ent.DataItems;

            ListBox_All_DataItem.DisplayMember = "DataItem.ItemName";

            for (int counter = 0; counter < items.Count; counter++)
            {
                Entities.DataItem item = items[counter];
                ListBox_All_DataItem.Items.Add(item);
            }
        }

        private void ListBox_All_DataItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateTextBoxes();
        }

        private void PopulateTextBoxes()
        {
            int selectedItemIndex = ListBox_All_DataItem.SelectedIndex;
            Entities.DataItem datatItem = entityBoundTo.DataItems[selectedItemIndex];

            TextBox_ClauseEditor_DataItem.Text = datatItem.ItemName;

            txtItemLength.Text = datatItem.Length.ToString();
            cmbItemType.SelectedIndexChanged -= cmbItemType_SelectedIndexChanged;
            switch (datatItem.ItemType)
            {
                case Entities.DataItem.DataItemType.Boolean:
                    cmbItemType.SelectedIndex = 3;
                    break;
                case Infosys.Lif.LegacyWorkbench.Entities.DataItem.DataItemType.DateType:
                    cmbItemType.SelectedIndex = 2;
                    break;
                case Infosys.Lif.LegacyWorkbench.Entities.DataItem.DataItemType.UnsignedIntegerType:
                case Infosys.Lif.LegacyWorkbench.Entities.DataItem.DataItemType.SignedIntegerType:
                    cmbItemType.SelectedIndex = 1;
                    break;
                case Infosys.Lif.LegacyWorkbench.Entities.DataItem.DataItemType.FloatType:
                    cmbItemType.SelectedIndex = 4;
                    break;
                default:
                    cmbItemType.SelectedIndex = 0;
                    break;
            }

            cmbItemType.SelectedIndexChanged += cmbItemType_SelectedIndexChanged;

            txtItemLength.Text = datatItem.Length.ToString();
            //cmbItemType.Text = datatItem.ItemType.ToString();
        }

        private void Button_Up_Click(object sender, EventArgs e)
        {
            int selectedItemIndex = ListBox_All_DataItem.SelectedIndex;
            if (selectedItemIndex > 0)
            {
                Entities.GenericCollection<Entities.DataItem> temp_items1
                    = entityBoundTo.DataItems;
                Entities.DataItem temp_ent;
                temp_ent = temp_items1[selectedItemIndex - 1];
                temp_items1[selectedItemIndex - 1] = temp_items1[selectedItemIndex];
                temp_items1[selectedItemIndex] = temp_ent;
                Populate_ListBox(entityBoundTo);
                ListBox_All_DataItem.SelectedIndex = selectedItemIndex - 1;
            }
        }

        private void Button_Down_Click(object sender, EventArgs e)
        {
            int selectedItemIndex = ListBox_All_DataItem.SelectedIndex;
            if (selectedItemIndex < ListBox_All_DataItem.Items.Count - 1)
            {
                Entities.GenericCollection<Entities.DataItem> temp_items1 = entityBoundTo.DataItems;
                Entities.DataItem temp_ent;
                temp_ent = temp_items1[selectedItemIndex];
                temp_items1[selectedItemIndex] = temp_items1[selectedItemIndex + 1];
                temp_items1[selectedItemIndex + 1] = temp_ent;
                Populate_ListBox(entityBoundTo);
                ListBox_All_DataItem.SelectedIndex = selectedItemIndex + 1;
            }
        }


        private void Button_Delete_Click(object sender, EventArgs e)
        {
            DialogResult result1 = MessageBox.Show("Are you sure you want to delete the SELECTED ITEM? ", "CONFIRM DELETION", MessageBoxButtons.YesNo);
            if (result1 == DialogResult.Yes)
            {
                int selectedItemIndex = ListBox_All_DataItem.SelectedIndex;
                entityBoundTo.DataItems.RemoveAt(selectedItemIndex);
                ListBox_All_DataItem.Items.Clear();
                Populate_ListBox(entityBoundTo);
            }
        }

        private void Button_ClauseEditor_Add_Click(object sender, EventArgs e)
        {
            int selectedIndex = ListBox_All_DataItem.SelectedIndex;
            {
                Entities.DataItem dataItem = new Entities.DataItem();
                dataItem.ItemName = "NewEntity" + ListBox_All_DataItem.Items.Count.ToString();

                entityBoundTo.DataItems.Insert(selectedIndex, dataItem);

                ListBox_All_DataItem.Items.Insert(selectedIndex, dataItem);
                ListBox_All_DataItem.SelectedIndex = selectedIndex;

                PopulateTextBoxes();

                //////btnSave.Enabled = true;
                //////Button_ClauseEditor_Add.Enabled = false;
                //////Button_ClauseEditor_Delete.Enabled = false;
                //////Button_ClauseEditor_Down.Enabled = false;
                //////Button_ClauseEditor_Up.Enabled = false;

                //////TextBox_ClauseEditor_DataItem.Clear();
            }
        }

        private void Button_ClauseEditor_Save_Click(object sender, EventArgs e)
        {
            if (Modified != null)
            {
                originalUnModifiedEntity = entityBoundTo;
                ModifiedEventArgs modifiedEventArgs = new ModifiedEventArgs();
                modifiedEventArgs.ModifiedEntity = entityBoundTo;
                Modified(this, modifiedEventArgs);
            }
        }


        private void txtClauseName_TextChanged(object sender, EventArgs e)
        {
            entityBoundTo.EntityName = txtClauseName.Text;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            entityBoundTo = Clone(originalUnModifiedEntity);
            Populate_ListBox(entityBoundTo);
        }

        private void TextBox_ClauseEditor_DataItem_TextChanged(object sender, EventArgs e)
        {
            if (ListBox_All_DataItem.SelectedIndex >= 0)
            {
                Entities.DataItem itemSelected = (Entities.DataItem)ListBox_All_DataItem.Items[ListBox_All_DataItem.SelectedIndex];
                itemSelected.ItemName = ((TextBox)sender).Text;

                int selectedIndex = ListBox_All_DataItem.SelectedIndex;
                BindListBox(entityBoundTo);
                ListBox_All_DataItem.SelectedIndex = selectedIndex;
            }


        }

        private void cmbItemType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ListBox_All_DataItem.SelectedIndex >= 0)
            {
                Entities.DataItem itemSelected = (Entities.DataItem)ListBox_All_DataItem.Items[ListBox_All_DataItem.SelectedIndex];
                switch (cmbItemType.SelectedItem.ToString().ToLowerInvariant())
                {
                    case "string":
                        itemSelected.ItemType = Entities.DataItem.DataItemType.StringType;
                        break;
                    case "integer":
                        itemSelected.ItemType = Entities.DataItem.DataItemType.SignedIntegerType;
                        break;
                    case "date time":
                        itemSelected.ItemType = Entities.DataItem.DataItemType.DateType;
                        break;
                    case "boolean":
                        itemSelected.ItemType = Entities.DataItem.DataItemType.Boolean;
                        break;
                    case "float":
                        itemSelected.ItemType = Entities.DataItem.DataItemType.FloatType;
                        break;
                }
            }

        }

        private void txtItemLength_TextChanged(object sender, EventArgs e)
        {
            if (ListBox_All_DataItem.SelectedIndex >= 0)
            {
                Entities.DataItem itemSelected = (Entities.DataItem)ListBox_All_DataItem.Items[ListBox_All_DataItem.SelectedIndex];
                itemSelected.Length = Convert.ToInt32(((TextBox)sender).Text);
            }
        }

        protected override void Save()
        {
            Button_ClauseEditor_Save_Click(this, new EventArgs());
            IsDirty = false;
        }

    }
}
