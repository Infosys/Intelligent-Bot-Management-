using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using Infosys.Lif.LegacyParser;

/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * This file holds the user control which allows the user to modify entities.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/

namespace Infosys.Lif.LegacyParser.UI
{
    /// <summary>
    /// The code-behind for the EntityEdit user control, allowing the user to modify 
    /// an entity.
    /// </summary>
    public partial class EntityEdit : UserControl
    {

        /// <summary>
        /// Event indicating that data in this editor has changed.
        /// </summary>
        [Description("Event to indicate that data in this editor has been changed by the user.")]
        public event EventHandler<ChangedEventArgs<Entity>> DataChanged;

        /// <summary>
        /// The entity which this user control binds to.
        /// </summary>
        private Entity currentEntity;

        /// <summary>
        /// Boolean indicating whether the form has loaded. This is to prevent 
        /// the DataChanged event from being fired on first load.
        /// </summary>
        private bool isLoaded;

        /// <summary>
        /// Constructor for this user control.
        /// </summary>
        public EntityEdit()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Method to load an entity in the form.
        /// </summary>
        /// <param name="entityToBeShown">
        /// The entity to which this user control should be bound.
        /// </param>
        public void LoadEntity(ref Entity entityToBeShown)
        {
            // Prevent the Changed event from being fired.
            isLoaded = false;


            currentEntity = entityToBeShown;
            txtEntityName.Text = entityToBeShown.EntityName;
            txtParserClassName.Text = entityToBeShown.SerializerClassName;
            txtDataEntityClassName.Text = entityToBeShown.DataEntityClassName;
            txtObjectId.Text = entityToBeShown.ObjectId;

            treeView1.Nodes.Clear();

            //dataGridView1.DataSource = entityToBeShown;
            BuildTreeNodes(entityToBeShown.DataItems[0].GroupItems, treeView1.Nodes);
            treeView1.ExpandAll();

            // Allow Changed event to be fired.
            isLoaded = true;
        }

        private void BuildTreeNodes(GenericCollection<DataItem> genericCollection, TreeNodeCollection treeNodeCollection)
        {
            for (int looper = 0; looper < genericCollection.Count; looper++)
            {
                TreeNode node = new TreeNode();
                node.Text = genericCollection[looper].ItemName;
                node.Tag = genericCollection[looper];
                treeNodeCollection.Add(node);
                if (genericCollection[looper].ItemType == DataItem.DataItemType.GroupItemType)
                {
                    BuildNodes(genericCollection[looper].GroupItems, node);
                }
            }
        }

        private void BuildNodes(GenericCollection<DataItem> dataItems, TreeNode node)
        {

            foreach (DataItem dataItem in dataItems)
            {
                int indexOfNode = node.Nodes.Add(new TreeNode());
                node.Nodes[indexOfNode].Text = dataItem.ItemName;
                node.Nodes[indexOfNode].Tag = dataItem;

                if (dataItem.ItemType == DataItem.DataItemType.GroupItemType)
                {
                    BuildNodes(dataItem.GroupItems, node.Nodes[indexOfNode]);
                }
            }
        }

        /// <summary>
        /// Event handler for Text Changed of the text boxes. This will further raise the 
        /// DataChanged event which should be handled by the form hosting this control.
        /// </summary>
        /// <param name="sender">The object raising the event.</param>
        /// <param name="e">Event arguments for the TextChanged event.</param>
        private void EntityDataChangedHandler(object sender, EventArgs e)
        {
            if (isLoaded)
            {
                currentEntity.EntityName = txtEntityName.Text;
                currentEntity.SerializerClassName = txtParserClassName.Text;
                currentEntity.DataEntityClassName = txtDataEntityClassName.Text;
                currentEntity.ObjectId = txtObjectId.Text;
                if (DataChanged != null)
                {
                    ChangedEventArgs<Entity> changedEventArgs
                        = new ChangedEventArgs<Entity>();
                    changedEventArgs.NewValue = currentEntity;
                    Control ctrl = (Control)sender;
                    switch (ctrl.Name)
                    {
                        case "txtDataEntityClassName":
                            changedEventArgs.PropertyChanged = "DEClassName";
                            break;
                        case "txtEntityName":
                            changedEventArgs.PropertyChanged = "EntityName";
                            break;
                        case "txtObjectId":
                            changedEventArgs.PropertyChanged = "ObjectId";
                            break;
                        case "txtParserClassName":
                            changedEventArgs.PropertyChanged = "PClassName";
                            break;
                    }
                    DataChanged(this, changedEventArgs);
                }
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {

            if (e.Control.GetType().Equals(typeof(DataGridViewComboBoxEditingControl)))
            {
                DataGridViewComboBoxEditingControl editingControl = (DataGridViewComboBoxEditingControl)e.Control;
                if (DataItem.DataItemType.EnumType == (DataItem.DataItemType)dataGridView1.Rows[dataGridView1.SelectedCells[0].RowIndex].Cells["ItemType"].Value)
                {
                    //Binding bin = new Binding("Text", editingControl.DataSource, "Value");
                    //editingControl.DataBindings.Add(bin);
                    editingControl.DropDownStyle = ComboBoxStyle.DropDownList;
                }
                else
                {
                    editingControl.DropDownStyle = ComboBoxStyle.Simple;
                }
                //if ( == 0)
                //{
                //    ((DataGridViewComboBoxEditingControl)e.Control).DropDownStyle = ComboBoxStyle.DropDown;
                //}
                //else
                //{
                //    ((DataGridViewComboBoxEditingControl)e.Control).DropDownStyle = ComboBoxStyle.DropDownList;
                //}
                //MessageBox.Show(dataGridView1.SelectedCells[0].RowIndex.ToString());
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            switch (e.Context)
            {
                case DataGridViewDataErrorContexts.Commit:

                    if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.GetType()
                        == typeof(string)
                        )
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = string.Empty;
                    }
                    if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.GetType()
                        == typeof(int))
                    {
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0;
                    }
                    break;
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            MessageBox.Show(e.ColumnIndex.ToString());
            if (dataGridView1.EditingControl != null)
            {
                MessageBox.Show(dataGridView1.EditingControl.Text);
            }
            else
            {
                MessageBox.Show("Null");
            }
        }

        public event EventHandler MaximizeWorkSpace;

        private void btnMaximize_Click(object sender, EventArgs e)
        {
            MaximizeWorkSpace(this, new EventArgs());
            splitContainer1.Panel1Collapsed = !splitContainer1.Panel1Collapsed;
            btnMaximize.Text = ((splitContainer1.Panel1Collapsed) ? ">>" : "<<");

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Tag != null)
            {
                DataItem itemToBeShown = ((DataItem)e.Node.Tag);
                if (itemToBeShown.ItemType == DataItem.DataItemType.GroupItemType)
                {
                    dataGridView1.DataSource = itemToBeShown.GroupItems;
                }
                else
                {
                    dataGridView1.DataSource = null;
                }
            }
        }

    }
}
