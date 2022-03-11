using System;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Text;

namespace Infosys.Lif.LegacyWorkbench.Editors
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ContractEditor : Editor
    {
        /// <summary>
        /// to store the info of the work done on this page
        /// </summary>
        string stringLogData = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public class ModifiedEventArgs : EventArgs
        {
            Entities.Contract modifiedContract;

            public Entities.Contract ModifiedContract
            {
                get { return modifiedContract; }
                set { modifiedContract = value; }
            }
        }
                
        /// <summary>
        /// 
        /// </summary>
        private bool notesAdded;

        /// <summary>
        /// 
        /// </summary>
        public bool NotesAdded
        {
            get { return notesAdded; }
            set { notesAdded = value; }
        }        
   /// <summary>
   /// 
   /// </summary>
   /// <param name="sender"></param>
   /// <param name="e"></param>
        public delegate void OnModified(object sender, ModifiedEventArgs e);
        /// <summary>
        /// 
        /// </summary>
        public event OnModified Modified;


        /// <summary>
        /// 
        /// </summary>
        public class NavigateToModelObjectEventArgs : EventArgs
        {
            Entities.Entity modelObjectEntity;

            public Entities.Entity ModelObjectEntity
            {
                get { return modelObjectEntity; }
                set { modelObjectEntity = value; }
            }
        }

        //public TreeNode lastSelectedNode;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void NavigateToModelObjectEvent(object sender, NavigateToModelObjectEventArgs e);

        public event NavigateToModelObjectEvent NavigateToModelObject;

        Entities.Contract contractBoundTo;
        Entities.Contract originalUnModifiedContract;
        int k;

        /// <summary>
        /// 
        /// </summary>
        public ContractEditor()
        {
            InitializeComponent();
            StyleModelObjectsGrids(gridModelObjectItems);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contractToBeEdited"></param>
        public void Populate(Entities.Contract contractToBeEdited)
        {
            cmbModelObjectId.Items.Clear();

            cmbModelObjectId.DisplayMember = "Entity.EntityName";
            foreach (Entities.Entity entity in LegacyParser.ModelObjectEntityMapping.Values)
            {
                cmbModelObjectId.Items.Add(entity);
            }

            originalUnModifiedContract = contractToBeEdited;
            contractBoundTo = Clone(contractToBeEdited);

            Populate_TreeBox(contractBoundTo);

            
            //if (lastSelectedNode.Text == "" || lastSelectedNode.Text == string.Empty || lastSelectedNode.Text == null)
            //{
                //TreeBox_Contract.SelectedNode = TreeBox_Contract.Nodes[0];
            //}
            //else
            //{
            //    //TreeBox_Contract
            //    //TreeBox_Contract.AfterSelect += TreeBox_Contract_AfterSelect;
            //    //string str = "TreeBox_Contract.Nodes[0].Nodes[0]";
            //    TreeBox_Contract.SelectedNode = lastSelectedNode;
            //    GetSelectedNodeLocation(lastSelectedNode);
            //    //TreeBox_Contract.AfterSelect -= TreeBox_Contract_AfterSelect;
            //}
            //string str = "Srajan";
            //TreeNode str = new TreeNode();
            //TreeNode returnSelectedNode = GetSelectedNodeLocation();
            TreeBox_Contract.SelectedNode = TreeBox_Contract.Nodes[0];
            //TreeBox_Contract.SelectedNode = returnSelectedNode;
            //TreeNode node2=new TreeNode(@"Input\ClientPolicyInfo");
            //TreeBox_Contract.SelectedNode = node2;

            //reassigning the values
            //lastSelectedNode.FullPath = null;
            //lastSelectedNode.Handle = 0;
            //lastSelectedNode = null;
            //lastSelectedNode=
        }
        /// <summary>
        /// this method clears the items from dropdown and the fill it again from 
        /// hashtable "ModelObjectEntityMapping" called when tab is changed
        /// </summary>
        public void autoFillDropDownList()
        {
            //clearing existing items
            cmbModelObjectId.Items.Clear();
            //clearing ends

            //add from hashtable "ModelObjectEntityMapping"
            foreach (Entities.Entity entity in LegacyParser.ModelObjectEntityMapping.Values)
            {
                cmbModelObjectId.Items.Add(entity);
            }
            //adding ends
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityToBeCloned"></param>
        /// <returns></returns>
        private Entities.Contract Clone(Entities.Contract entityToBeCloned)
        {
            System.IO.MemoryStream memStream = new System.IO.MemoryStream();
            System.Xml.Serialization.XmlSerializer serializer
                = new System.Xml.Serialization.XmlSerializer(typeof(Entities.Contract));
            serializer.Serialize(memStream, entityToBeCloned);

            memStream.Position = 0;


            Entities.Contract entityDeserialized
                = (Entities.Contract)serializer.Deserialize(memStream);
            return entityDeserialized;
        }


        /// <summary>
        /// 
        /// </summary>
        const string InputNodeText = "Input";
        /// <summary>
        /// 
        /// </summary>
        const string OutputNodeText = "Output";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contractToBePopulated"></param>
        private void Populate_TreeBox(Entities.Contract contractToBePopulated)
        {
            lblInputModelObjectDefnReplacement.Visible = false;
            gridModelObjectItems.Visible = false;
            TreeBox_Contract.Visible = false;
            TreeBox_Contract.Nodes.Clear();

            contractPropertyGrid.SelectedObject = contractToBePopulated;


            ////txtContractName.TextChanged -= txtContractName_TextChanged;
            ////txtContractName.Text = contractToBePopulated.ContractName;
            ////txtContractName.TextChanged += txtContractName_TextChanged;


            ////txtContractDescription.TextChanged -= txtContractDescription_TextChanged;
            ////txtContractDescription.Text = contractToBePopulated.ContractDescription;
            ////txtContractDescription.TextChanged += txtContractDescription_TextChanged;

            ////txtMethodName.TextChanged -= txtMethodName_TextChanged;
            ////txtMethodName.Text = contractToBePopulated.MethodName;
            ////txtMethodName.TextChanged += txtMethodName_TextChanged;


            ////txtServiceName.TextChanged -= txtServiceName_TextChanged;
            ////txtServiceName.Text = contractBoundTo.ServiceName;
            ////txtServiceName.TextChanged += txtServiceName_TextChanged;

            ////txtTransactionId.TextChanged -= txtTransactionId_TextChanged;
            ////txtTransactionId.Text = contractBoundTo.TransactionId;
            ////txtTransactionId.TextChanged += txtTransactionId_TextChanged;



            ////switch (contractToBePopulated.MethodType)
            ////{
            ////    case Infosys.Lif.LegacyWorkbench.Entities.Contract.ContractMethodType.Select:
            ////        cmbMethodType.SelectedIndex = 0;
            ////        break;
            ////    case Infosys.Lif.LegacyWorkbench.Entities.Contract.ContractMethodType.Insert:
            ////        cmbMethodType.SelectedIndex = 1;
            ////        break;
            ////    case Infosys.Lif.LegacyWorkbench.Entities.Contract.ContractMethodType.Update:
            ////        cmbMethodType.SelectedIndex = 2;
            ////        break;
            ////    case Infosys.Lif.LegacyWorkbench.Entities.Contract.ContractMethodType.Delete:
            ////        cmbMethodType.SelectedIndex = 3;
            ////        break;
            ////}


            TreeNode nodeInput = new TreeNode();
            nodeInput.Text = InputNodeText;
            TreeNode nodeOutput = new TreeNode();
            nodeOutput.Text = OutputNodeText;

            if (TreeBox_Contract.Nodes.Count == 0)
            {
                TreeBox_Contract.Nodes.Add(nodeInput);
                foreach (Entities.ModelObject ModelObject in
                    contractToBePopulated.InputModelObjects)
                {
                    TreeNode nodeToBeAdded = new TreeNode();
                    DisplayTreeStructure(ModelObject, nodeToBeAdded);

                    AddNode(nodeInput.Nodes, nodeToBeAdded);
                }


                TreeBox_Contract.Nodes.Add(nodeOutput);
                foreach (Entities.ModelObject modelObject in contractToBePopulated.OutputModelObjects)
                {
                    TreeNode n1 = new TreeNode();
                    DisplayTreeStructure(modelObject, n1);
                    AddNode(nodeOutput.Nodes, n1);
                }

                TreeBox_Contract.ExpandAll();
                TreeBox_Contract.Visible = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentNodeCollection"></param>
        /// <param name="nodeToBeAdded"></param>
        private void AddNode(TreeNodeCollection parentNodeCollection, TreeNode nodeToBeAdded)
        {

            //////string nodeText = nodeToBeAdded.Text;
            //////int nodeCounter = 0;
            //////for (nodeCounter = 0; nodeCounter < parentNodeCollection.Count; nodeCounter++)
            //////{
            //////    if (parentNodeCollection[nodeCounter].Text.CompareTo(nodeText) > 0)
            //////    { break; }
            //////}
            parentNodeCollection.Add(nodeToBeAdded);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gridModelObjects"></param>
        private void StyleModelObjectsGrids(DataGridView gridModelObjects)
        {
            gridModelObjects.AutoGenerateColumns = false;
            gridModelObjects.ReadOnly = true;
            {
                string fieldName = "ItemName";
                DataGridViewColumn column = new DataGridViewTextBoxColumn();
                column.DataPropertyName = fieldName;
                column.HeaderText = "Item Name";
                gridModelObjects.Columns.Add(column);
            }

            {
                string fieldName = "ItemType";
                DataGridViewColumn column = new DataGridViewTextBoxColumn();
                column.DataPropertyName = fieldName;
                column.HeaderText = "Item Type";
                gridModelObjects.Columns.Add(column);
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="topModelObject"></param>
        /// <param name="nodeToBeAddedTo"></param>
        private void DisplayTreeStructure(Entities.ModelObject topModelObject, TreeNode nodeToBeAddedTo)
        {
            nodeToBeAddedTo.Text = topModelObject.Name;
            nodeToBeAddedTo.Tag = topModelObject;
            foreach (Entities.ModelObject entity in topModelObject.ModelObjects)
            {
                TreeNode node = new TreeNode();
                DisplayTreeStructure(entity, node);

                AddNode(nodeToBeAddedTo.Nodes, node);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="topModelObject"></param>
        /// <param name="nodeToBeAddedTo"></param>
        private void OnPasteDisplayTreeStructure(Entities.ModelObject topModelObject, TreeNode nodeToBeAddedTo)
        {
            //nodeToBeAddedTo.Text = topModelObject.Name;
           // nodeToBeAddedTo.Tag = topModelObject;
            TreeNode parentNodeToAdd = new TreeNode();
            DisplayTreeStructure(topModelObject, parentNodeToAdd);
           
            AddNode(nodeToBeAddedTo.Nodes, parentNodeToAdd);
           
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeBox_Contract_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Entities.ModelObject selectedModelObject;
            //to allow call when cmbbox itemis changed
            if (e != null)
            {
                if (e.Node.Level == 0)
                {
                    ModelObjectPropertyGrid.SelectedObject = null;
                    gridModelObjectItems.DataSource = null;
                    //make the cmbModelObjectID = empty
                    cmbModelObjectId.SelectedIndexChanged -= cmbModelObjectId_SelectedIndexChanged;
                    cmbModelObjectId.SelectedIndex = -1;
                    cmbModelObjectId.SelectedIndexChanged += cmbModelObjectId_SelectedIndexChanged;

                    //make the gridModelObject visible false
                    gridModelObjectItems.Visible = false;
                    return;
                }
            }

            selectedModelObject = (Entities.ModelObject)TreeBox_Contract.SelectedNode.Tag;
            //assign the new value of host name
            //to check whether the call is after binding the contract or not
            if (selectedModelObject.ModelObjectEntity != null)
            {   
                selectedModelObject.HostName = selectedModelObject.ModelObjectEntity.ProgramId.ToString();
            }
            cmbModelObjectId.SelectedIndexChanged -= cmbModelObjectId_SelectedIndexChanged;
            cmbModelObjectId.SelectedItem = selectedModelObject.ModelObjectEntity;
            cmbModelObjectId.SelectedIndexChanged += cmbModelObjectId_SelectedIndexChanged;

            if (selectedModelObject.ModelObjectEntity == null)
            {
                cmbModelObjectId.SelectedIndexChanged -= cmbModelObjectId_SelectedIndexChanged;
                cmbModelObjectId.Text = "NewModelObject";//string.Empty;
                cmbModelObjectId.SelectedIndexChanged += cmbModelObjectId_SelectedIndexChanged;
            }
            else
            {
                cmbModelObjectId.SelectedIndexChanged -= cmbModelObjectId_SelectedIndexChanged;
                //EntityName -> Prog ID
                cmbModelObjectId.Text = selectedModelObject.ModelObjectEntity.EntityName;
                cmbModelObjectId.SelectedIndexChanged += cmbModelObjectId_SelectedIndexChanged;
            }


            if (selectedModelObject.Name.Length > 4)
            {
                TextBox_ContractEditor_ModelObjectName.TextChanged -= TextBox_ContractEditor_ModelObjectName_TextChanged;

                //clear text initially if Cobol                
                //TextBox_ContractEditor_ModelObjectName.Text = selectedModelObject.Name.Substring(4);
                
                TextBox_ContractEditor_ModelObjectName.TextChanged += TextBox_ContractEditor_ModelObjectName_TextChanged;
            }


            ModelObjectPropertyGrid.SelectedObject = selectedModelObject;

            //TextBox_ContractEditor_Max_Occurences.TextChanged -= TextBox_ContractEditor_Max_Occurences_TextChanged;
            //TextBox_ContractEditor_Max_Occurences.Text = c1.MaxCount.ToString();
            //TextBox_ContractEditor_Max_Occurences.TextChanged += TextBox_ContractEditor_Max_Occurences_TextChanged;

            //TextBox_ContractEditor_Min_Occurences.TextChanged -= TextBox_ContractEditor_Min_Occurences_TextChanged;
            //TextBox_ContractEditor_Min_Occurences.Text = c1.MinCount.ToString();
            //TextBox_ContractEditor_Min_Occurences.TextChanged += TextBox_ContractEditor_Min_Occurences_TextChanged;



            //txtHostName.TextChanged -= txtHostName_TextChanged;
            //txtHostName.Text = c1.HostName;
            //txtHostName.TextChanged += txtHostName_TextChanged;


            k = TreeBox_Contract.SelectedNode.Index;


            if (selectedModelObject.ModelObjectEntity == null)
            {
                lblInputModelObjectDefnReplacement.Visible = true;
                gridModelObjectItems.Visible = false;
            }
            else
            {
                gridModelObjectItems.Visible = true;
                lblInputModelObjectDefnReplacement.Visible = false;
                gridModelObjectItems.DataSource = selectedModelObject.ModelObjectEntity.DataItems;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_ContractEditor_Up_Click(object sender, EventArgs e)
        {
            //to check if a node is selected or not
            if (TreeBox_Contract.SelectedNode == null)
            {
                MessageBox.Show("Please selact a node before performing any action.",
                    "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (TreeBox_Contract.SelectedNode.Text == "Input" || TreeBox_Contract.SelectedNode.Text == "Output")
            {
                MessageBox.Show("This is the top node", "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //code to move up down
            int currentNodeIndex = TreeBox_Contract.SelectedNode.Index;
            int tobeReplacedNodeIndex = currentNodeIndex - 1;
            if (currentNodeIndex == 0)
            {
                MessageBox.Show("This is the top node", "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                TreeNode temp = TreeBox_Contract.SelectedNode;
                TreeNode node = (TreeNode)temp.Clone();
                TreeBox_Contract.SelectedNode.Parent.Nodes.Insert(tobeReplacedNodeIndex, node);
                TreeBox_Contract.SelectedNode.Parent.Nodes.RemoveAt(currentNodeIndex + 1);
            }

            //code ends
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_ContractEditor_Delete_Click(object sender, EventArgs e)
        {
            //to check if a node is selected or not
            if (TreeBox_Contract.SelectedNode == null)
            {
                MessageBox.Show("Please selact a node before performing any action.",
                    "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            {
                if (TreeBox_Contract.SelectedNode.Level > 0)
                {
                    DialogResult confirmationResult = MessageBox.Show("This action will delete the selected node. Continue?", "Legacy Workbench", MessageBoxButtons.YesNo,MessageBoxIcon.Information);
                    if (confirmationResult == DialogResult.Yes)
                    {

                        Entities.GenericCollection<Entities.ModelObject> ModelObjectsToBeRemovedFrom;
                        if (TreeBox_Contract.SelectedNode.Parent.Text == InputNodeText)
                        {
                            ModelObjectsToBeRemovedFrom = contractBoundTo.InputModelObjects;
                        }
                        else if (TreeBox_Contract.SelectedNode.Parent.Text == OutputNodeText)
                        {
                            ModelObjectsToBeRemovedFrom = contractBoundTo.OutputModelObjects;
                        }
                        else
                        {
                            Entities.ModelObject c3 = (Entities.ModelObject)TreeBox_Contract.SelectedNode.Parent.Tag;
                            ModelObjectsToBeRemovedFrom = c3.ModelObjects;
                        }

                        ModelObjectsToBeRemovedFrom.Remove((Entities.ModelObject)TreeBox_Contract.SelectedNode.Tag);

                        TreeBox_Contract.SelectedNode.Remove();

                        // Start of comment by Sahcin Nayak
                        // This code used to result in a complete refresh of the treeview. 
                        // which did not look good.
                        //TreeBox_Contract.Nodes.Clear();
                        //Populate_TreeBox(contractBoundTo);
                        // End of comment by Sachin Nayak
                    }

                    //else
                    //{
                    //    MessageBox.Show("Delete Operation Cancelled.", "ERROR");
                    //}
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_ContractEditor_Down_Click(object sender, EventArgs e)
        {
            //to check if a node is selected or not
            if (TreeBox_Contract.SelectedNode == null)
            {
                MessageBox.Show("Please selact a node before performing any action.",
                    "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (TreeBox_Contract.SelectedNode.Text == "Input" || TreeBox_Contract.SelectedNode.Text == "Output")
            {
                MessageBox.Show("This is the top node", "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (TreeBox_Contract.SelectedNode == null)
            {
                MessageBox.Show("Please Select the input or output model object node for Shifting.", "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            int currentNodeIndex = TreeBox_Contract.SelectedNode.Index;
            int tobeReplacedNodeIndex = currentNodeIndex + 1;
            if (currentNodeIndex == TreeBox_Contract.SelectedNode.Parent.Nodes.Count-1)
            {
                MessageBox.Show("This is the last node", "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            else
            {
                TreeNode temp = TreeBox_Contract.SelectedNode.Parent.Nodes[tobeReplacedNodeIndex];
                TreeNode node = (TreeNode)temp.Clone();
                TreeBox_Contract.SelectedNode.Parent.Nodes.Insert(currentNodeIndex, node);
                TreeBox_Contract.SelectedNode.Parent.Nodes.RemoveAt(tobeReplacedNodeIndex + 1);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_ContractEditor_Add_Click(object sender, EventArgs e)
        {
            //to check if a node is selected or not
            if (TreeBox_Contract.SelectedNode == null)
            {
                MessageBox.Show("Please selact a node before performing any action.",
                    "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Entities.ModelObject newModelObject = new Entities.ModelObject();
            newModelObject.Name = "New ModelObject";
            newModelObject.MinCount = 0;
            newModelObject.MaxCount = 0;

            TreeNode newNode = new TreeNode();

            DisplayTreeStructure(newModelObject, newNode);

            AddNode(TreeBox_Contract.SelectedNode.Nodes, newNode);

            Entities.GenericCollection<Entities.ModelObject> ModelObjectsToBeAddedTo;

            if (TreeBox_Contract.SelectedNode.Level == 0)
            {
                if (TreeBox_Contract.SelectedNode.Text == InputNodeText)
                {
                    ModelObjectsToBeAddedTo = contractBoundTo.InputModelObjects;
                }
                else
                {
                    ModelObjectsToBeAddedTo = contractBoundTo.OutputModelObjects;
                }
            }
            else
            {
                ModelObjectsToBeAddedTo = ((Entities.ModelObject)TreeBox_Contract.SelectedNode.Tag).ModelObjects;
            }
            ModelObjectsToBeAddedTo.Add(newModelObject);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {

            if (Modified != null)
            {
                originalUnModifiedContract = contractBoundTo;
                ModifiedEventArgs modifiedEventArgs = new ModifiedEventArgs();
                modifiedEventArgs.ModifiedContract = contractBoundTo;
                
                //notes change   
                NotesAdded = false;
                if (contractBoundTo.Notes != null && contractBoundTo.Notes.Trim().Length > 0
                    && !contractBoundTo.Notes.Equals(""))
                {
                    NotesAdded = true;
                }
                //change end

                Modified(this, modifiedEventArgs);
            }

            ////////Entities.Clause New_Clause = new Entities.Clause();
            ////////if ((TextBox_ContractEditor_ClauseName.Text.Length == 0) || (TextBox_ContractEditor_Max_Occurences.Text.Length == 0) || (TextBox_ContractEditor_Min_Occurences.Text.Length == 0))
            ////////{
            ////////    MessageBox.Show("One or more text-box(es) containing the CLAUSE DETAILS have NOT been filled.", "INSERTION ERROR");
            ////////}
            ////////else
            ////////{
            ////////    New_Clause.Name = TextBox_ContractEditor_ClauseName.Text;
            ////////    New_Clause.MaxCount = System.Int32.Parse(TextBox_ContractEditor_Max_Occurences.Text);
            ////////    New_Clause.MinCount = System.Int32.Parse(TextBox_ContractEditor_Min_Occurences.Text);

            ////////    Entities.Clause c7 = (Entities.Clause)TreeBox_Contract.SelectedNode.Parent.Tag;
            ////////    c7.Clauses.Insert(k + 1, New_Clause);

            ////////    TreeBox_Contract.Nodes.Clear();
            ////////    Populate_TreeBox(contractBoundTo);

            ////////    btnSave.Enabled = false;
            ////////    Button_ContractEditor_Add.Enabled = true;
            ////////    Button_ContractEditor_Delete.Enabled = true;
            ////////    Button_ContractEditor_Down.Enabled = true;
            ////////    Button_ContractEditor_Up.Enabled = true;

            ////////}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeBox_Contract_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //to disable the error message when user clicks the Input node and Output node in treebox_contracts
            if (TreeBox_Contract.SelectedNode.Parent == null)
            {
                MessageBox.Show("This is not a model object", "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //disabling code ends
            NavigateToModelObjectEventArgs eventArgs = new NavigateToModelObjectEventArgs();
            eventArgs.ModelObjectEntity = ((Entities.ModelObject)e.Node.Tag).ModelObjectEntity;
            if (this.NavigateToModelObject != null)
            {
                //to check if the contract is binded to a model object or not
                if (eventArgs.ModelObjectEntity == null)
                {
                    MessageBox.Show("This contract is not bound to a model object", "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                //check ends
                else
                {
                    this.NavigateToModelObject(this, eventArgs);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            TreeBox_Contract.Nodes.Clear();
            contractBoundTo = Clone(originalUnModifiedContract);
            Populate_TreeBox(contractBoundTo);

            //to clear the ModelObjectPropertyGrid and the cmbModelObjectId data
            ModelObjectPropertyGrid.SelectedObject = null;
            cmbModelObjectId.SelectedIndexChanged -= cmbModelObjectId_SelectedIndexChanged;
            cmbModelObjectId.SelectedIndex = -1;
            cmbModelObjectId.SelectedIndexChanged += cmbModelObjectId_SelectedIndexChanged;
        }

        ////private void txtContractName_TextChanged(object sender, EventArgs e)
        ////{
        ////    contractBoundTo.ContractName = txtContractName.Text;
        ////}


        ////private void cmbMethodType_SelectedIndexChanged(object sender, EventArgs e)
        ////{
        ////    switch (cmbMethodType.SelectedText)
        ////    {
        ////        case "List":
        ////            contractBoundTo.MethodType = Entities.Contract.ContractMethodType.List;
        ////            break;
        ////        case "Select":
        ////            contractBoundTo.MethodType = Entities.Contract.ContractMethodType.Select;
        ////            break;
        ////        case "Insert":
        ////            contractBoundTo.MethodType = Entities.Contract.ContractMethodType.Insert;
        ////            break;
        ////        case "Update":
        ////            contractBoundTo.MethodType = Entities.Contract.ContractMethodType.Update;
        ////            break;
        ////        case "Delete":
        ////            contractBoundTo.MethodType = Entities.Contract.ContractMethodType.Delete;
        ////            break;
        ////    }
        ////}

        ////private void txtMethodName_TextChanged(object sender, EventArgs e)
        ////{
        ////    contractBoundTo.MethodName = txtMethodName.Text;
        ////}

        ////private void txtContractDescription_TextChanged(object sender, EventArgs e)
        ////{
        ////    contractBoundTo.ContractDescription = txtContractDescription.Text;
        ////}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_ContractEditor_ModelObjectName_TextChanged(object sender, EventArgs e)
        {
            if (TreeBox_Contract.SelectedNode.Level >= 1)
            {
                //Prog ID -> EntityName
                string modelObjectName = ((Entities.Entity)cmbModelObjectId.SelectedItem).EntityName + ((TextBox)sender).Text;

                TreeBox_Contract.SelectedNode.Text = modelObjectName;
                ((Entities.ModelObject)TreeBox_Contract.SelectedNode.Tag).Name = modelObjectName;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_ContractEditor_Max_Occurences_TextChanged(object sender, EventArgs e)
        {
            if (TreeBox_Contract.SelectedNode.Level >= 1)
            {
                if (((TextBox)sender).Text.Length > 0)
                {
                    ((Entities.ModelObject)TreeBox_Contract.SelectedNode.Tag).MaxCount = Convert.ToInt32(((TextBox)sender).Text);
                }
                else
                {
                    ((Entities.ModelObject)TreeBox_Contract.SelectedNode.Tag).MaxCount = 0;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_ContractEditor_Min_Occurences_TextChanged(object sender, EventArgs e)
        {
            if (TreeBox_Contract.SelectedNode.Level >= 1)
            {
                if (((TextBox)sender).Text.Length > 0)
                {
                    ((Entities.ModelObject)TreeBox_Contract.SelectedNode.Tag).MinCount = Convert.ToInt32(((TextBox)sender).Text);
                }
                else
                {
                    ((Entities.ModelObject)TreeBox_Contract.SelectedNode.Tag).MinCount = 0;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbModelObjectId_SelectedIndexChanged(object sender, EventArgs e)
        {
            //to check if the cmbModelObjectId is selected or not
            if (TreeBox_Contract.SelectedNode == null)
            {
                cmbModelObjectId.Items.Add("");
                MessageBox.Show("Please selact a node before performing any action.",
                    "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmbModelObjectId.SelectedIndexChanged -= cmbModelObjectId_SelectedIndexChanged;
                cmbModelObjectId.SelectedItem = cmbModelObjectId.Items[0];
                cmbModelObjectId.SelectedIndexChanged += cmbModelObjectId_SelectedIndexChanged;
                cmbModelObjectId.Items.Remove("");
                return;
            }
            
            if (TreeBox_Contract.SelectedNode.Text == "Input")
            {
                cmbModelObjectId.Items.Add("");
                MessageBox.Show("Input node cannot be mapped to a model object", "Legacy Workbench", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmbModelObjectId.SelectedIndexChanged -= cmbModelObjectId_SelectedIndexChanged;
                cmbModelObjectId.SelectedItem = cmbModelObjectId.Items[0];
                cmbModelObjectId.SelectedIndexChanged += cmbModelObjectId_SelectedIndexChanged;
                cmbModelObjectId.Items.Remove("");
                return;
            }
            if (TreeBox_Contract.SelectedNode.Text == "Output")
            {
                cmbModelObjectId.Items.Add("");
                MessageBox.Show("Output node cannot be mapped to a model object", "Legacy Workbench", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                cmbModelObjectId.SelectedIndexChanged -= cmbModelObjectId_SelectedIndexChanged;
                cmbModelObjectId.SelectedItem = cmbModelObjectId.Items[0];
                cmbModelObjectId.SelectedIndexChanged += cmbModelObjectId_SelectedIndexChanged;
                cmbModelObjectId.Items.Remove("");
                return;
            }

            ((Entities.ModelObject)TreeBox_Contract.SelectedNode.Tag).Name = cmbModelObjectId.Text + TextBox_ContractEditor_ModelObjectName.Text;

            //((Entities.ModelObject)TreeBox_Contract.SelectedNode.Tag).ModelObjectEntity = (Entities.Entity)LegacyParser.ModelObjectEntityMapping[cmbModelObjectId.Text];
            ((Entities.ModelObject)TreeBox_Contract.SelectedNode.Tag).ModelObjectEntity = (Entities.Entity)cmbModelObjectId.SelectedItem;

            string modelObjectName = ((ComboBox)sender).Text + TextBox_ContractEditor_ModelObjectName.Text;

            TreeBox_Contract.SelectedNode.Text = modelObjectName;
            //PropertyGrid obj = new PropertyGrid();
            TreeBox_Contract_AfterSelect(sender, e as TreeViewEventArgs);
            //ModelObjectPropertyGrid.SelectedObjects.SetValue("srajan", 0);
        }        

        ////private void txtServiceName_TextChanged(object sender, EventArgs e)
        ////{
        ////    contractBoundTo.ServiceName = txtServiceName.Text;
        ////}

        /// <summary>
        /// 
        /// </summary>
        protected override void Save()
        {
            btnSave_Click(this, new EventArgs());
            IsDirty = false;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void Cancel()
        {
            btnCancel_Click(this, new EventArgs());
            IsDirty = false;
        }

        //////private void txtTransactionId_TextChanged(object sender, EventArgs e)
        //////{
        //////    contractBoundTo.TransactionId = txtTransactionId.Text;

        //////}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtHostName_TextChanged(object sender, EventArgs e)
        {
            if (TreeBox_Contract.SelectedNode.Level >= 1)
            {
                ((Entities.ModelObject)TreeBox_Contract.SelectedNode.Tag).HostName = ((TextBox)sender).Text;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        const string LegacyParserCopiedString = "LegacyParserCopiedString";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCopy_Click(object sender, EventArgs e)
        {
            //to check if a node is selected or not
            if (TreeBox_Contract.SelectedNode == null)
            {
                MessageBox.Show("Please selact a node before performing any action.",
                    "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            TreeNode selectedNode = TreeBox_Contract.SelectedNode;

            if (selectedNode.Level > 0)
            {
                object storedObject = selectedNode.Tag;
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                System.IO.StreamReader strReader = new System.IO.StreamReader(stream);

                System.Xml.Serialization.XmlSerializer serializer
                    = new System.Xml.Serialization.XmlSerializer(storedObject.GetType());
                serializer.Serialize(stream, storedObject);

                stream.Position = 0;
                string storedObjectType = storedObject.GetType().AssemblyQualifiedName;


                string strSerialized = strReader.ReadToEnd();
                strSerialized = LegacyParserCopiedString + storedObjectType + Environment.NewLine + strSerialized;
                Clipboard.SetData(DataFormats.UnicodeText, strSerialized);

            }
        }

        /// <summary>
        /// 
        /// </summary>
        string clipBoardFormat = DataFormats.UnicodeText;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPaste_Click(object sender, EventArgs e)
        {
            //to check if a node is selected or not
            if (TreeBox_Contract.SelectedNode == null)
            {
                MessageBox.Show("Please selact a node before performing any action.",
                    "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            TreeNode selectedNode = TreeBox_Contract.SelectedNode;
            //if (selectedNode.Level > 0)
            //{
                if (Clipboard.ContainsData(clipBoardFormat))
                {
                    string clipBoardData = (string)Clipboard.GetData(clipBoardFormat);
                    if (LegacyParserCopiedString.StartsWith(LegacyParserCopiedString))
                    {
                        string[] seperators = new string[1];
                        seperators[0] = Environment.NewLine;
                        string[] strSerialized = clipBoardData.Split(seperators, StringSplitOptions.None);
                        string serializedType = strSerialized[0].Substring(LegacyParserCopiedString.Length);
                        Type typeForDeserialization = Type.GetType(serializedType);

                        XmlSerializer serializer = new XmlSerializer(typeof(Entities.ModelObject));

                        System.IO.MemoryStream stream = new System.IO.MemoryStream();
                        System.IO.StreamWriter strWriter = new System.IO.StreamWriter(stream);
                        clipBoardData = clipBoardData.Substring(LegacyParserCopiedString.Length + Environment.NewLine.Length + serializedType.Length);
                        strWriter.Write(clipBoardData);
                        strWriter.Flush();
                        stream.Position = 0;
                        try
                        {
                            Entities.ModelObject modelObjectToBePasted = (Entities.ModelObject)serializer.Deserialize(stream);

                        //code to allow pasting in Input and Output node as well
                        if (selectedNode.Level == 0)
                        {
                            Entities.GenericCollection<Entities.ModelObject> ModelObjectsRoot;

                            if (TreeBox_Contract.SelectedNode.Level == 0)
                            {
                                if (TreeBox_Contract.SelectedNode.Text == InputNodeText)
                                {
                                    ModelObjectsRoot = contractBoundTo.InputModelObjects;
                                }
                                else
                                {
                                    ModelObjectsRoot = contractBoundTo.OutputModelObjects;
                                }

                                ModelObjectsRoot.Add(modelObjectToBePasted);

                                TreeNode nodeToBePasted = selectedNode;

                                OnPasteDisplayTreeStructure(modelObjectToBePasted, nodeToBePasted);
                            }
                            
                        }
                        //code ends
                        else
                        {
                            Entities.GenericCollection<Entities.ModelObject> modelObjects;
                            if (selectedNode.Tag is Entities.GenericCollection<Entities.ModelObject>)
                            {
                                modelObjects = (Entities.GenericCollection<Entities.ModelObject>)selectedNode.Tag;
                            }
                            else
                            {
                                Entities.ModelObject modelObject = (Entities.ModelObject)selectedNode.Tag;
                                modelObjects = modelObject.ModelObjects;
                            }
                            modelObjects.Add(modelObjectToBePasted);

                            //TreeNode nodeToBePasted = new TreeNode();
                            TreeNode nodeToBePasted = selectedNode;

                            OnPasteDisplayTreeStructure(modelObjectToBePasted, nodeToBePasted);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Please cut a node to paste it", "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    }
                //}
            }

            ////code to allow pasting in Input and Output node
            //Entities.GenericCollection<Entities.ModelObject> ModelObjectsToBePasted;

            //if (TreeBox_Contract.SelectedNode.Level == 0)
            //{
            //    if (TreeBox_Contract.SelectedNode.Text == InputNodeText)
            //    {
            //        ModelObjectsToBePasted = contractBoundTo.InputModelObjects;
            //    }
            //    else
            //    {
            //        ModelObjectsToBePasted = contractBoundTo.OutputModelObjects;
            //    }
            //}

            //ModelObjectsToBePasted.Add(newModelObject);
            ////code ends

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCut_Click(object sender, EventArgs e)
        {
            //to check if a node is selected or not
            if (TreeBox_Contract.SelectedNode == null)
            {
                MessageBox.Show("Please selact a node before performing any action.",
                    "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            btnCopy_Click(sender, e);
            TreeNode selectedNode = TreeBox_Contract.SelectedNode;
            if (selectedNode.Level > 0)
            {
                // Go ahead and delete the selected model object.
                Button_ContractEditor_Delete_Click(sender, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cutToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            btnCut_Click(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            btnCopy_Click(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            btnPaste_Click(sender, e);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Button_ContractEditor_Add_Click(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Button_ContractEditor_Delete_Click(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        private void contractPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (contractPropertyGrid.SelectedGridItem.Label.Equals("Contract Name") &&
                e.ChangedItem.Value.ToString().Length > 30)
            {
                string changedValue = e.ChangedItem.Value.ToString();
                PropertyGrid contractObjPropertyGrid = (PropertyGrid)s;
                Entities.Contract entityObj = (Entities.Contract)contractPropertyGrid.SelectedObject;
                //changedValue = e.OldValue.ToString();
                MessageBox.Show("The length of Contract name should not exceed 30.\n The name has been truncated to 30 characters.",
                    "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
                entityObj.ContractName = changedValue.ToString().Substring(0, 30);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeBox_Contract_MouseClick(object sender, MouseEventArgs e)
        {

        }
        /// <summary>
        /// to store the information of the user who is editing the current page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContractEditor_Enter(object sender, EventArgs e)
        {
            //stringLogData     
            //TreeBox_Contract_AfterSelect(sender, e as TreeViewEventArgs);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContractEditor_Leave(object sender, EventArgs e)
        {
            //Entities.ModelObject selectedModelObject;

            //selectedModelObject = (Entities.ModelObject)TreeBox_Contract.SelectedNode.Tag;
            //cmbModelObjectId.SelectedIndexChanged -= cmbModelObjectId_SelectedIndexChanged;
            //cmbModelObjectId.SelectedItem = selectedModelObject.ModelObjectEntity;
            //cmbModelObjectId.SelectedIndexChanged += cmbModelObjectId_SelectedIndexChanged;

            //TreeView clonedTV = new TreeView();
            //clonedTV = TreeBox_Contract;
            //lastSelectedNode = clonedTV.SelectedNode;
            
            //Clone
        }


        //public void GetSelectedNodeLocation(TreeNode node)
        //{
        //    //StringBuilder sb = new StringBuilder();
        //    //TreeNode node = new TreeNode();
        //    //node.Text = sb.ToString();
            


        //    //int levelOfSelectedNode = lastSelectedNode.Level;
        //    //string[] array = new string[levelOfSelectedNode];
        //    //for (int loop = 0; loop < levelOfSelectedNode; loop++)
        //    //{
        //    //    if (loop == 0)
        //    //    {
        //    //        sb.Append("Nodes[" + lastSelectedNode.Level + "]");
        //    //    }
        //    //    else
        //    //    {
        //    //        sb.Append("Nodes[" + lastSelectedNode.Level + "]");
        //    //    }
        //    //    if (lastSelectedNode.Parent != null || lastSelectedNode.Parent.Text != string.Empty)
        //    //    {
        //    //        lastSelectedNode = lastSelectedNode.Parent;
        //    //    }
        //    //}

        //    //return node;



        //    Entities.ModelObject selectedModelObject;
        //    //to allow call when cmbbox itemis changed
        //    if (node != null)
        //    {
        //        if (node.Level == 0)
        //        {
        //            ModelObjectPropertyGrid.SelectedObject = null;
        //            gridModelObjectItems.DataSource = null;
        //            //make the cmbModelObjectID = empty
        //            cmbModelObjectId.SelectedIndexChanged -= cmbModelObjectId_SelectedIndexChanged;
        //            cmbModelObjectId.SelectedIndex = -1;
        //            cmbModelObjectId.SelectedIndexChanged += cmbModelObjectId_SelectedIndexChanged;

        //            //make the gridModelObject visible false
        //            gridModelObjectItems.Visible = false;
        //            return;
        //        }
        //    }

        //    selectedModelObject = (Entities.ModelObject)node.Tag;
        //    //assign the new value of host name
        //    //to check whether the call is after binding the contract or not
        //    if (selectedModelObject.ModelObjectEntity != null)
        //    {
        //        selectedModelObject.HostName = selectedModelObject.ModelObjectEntity.ProgramId.ToString();
        //    }
        //    cmbModelObjectId.SelectedIndexChanged -= cmbModelObjectId_SelectedIndexChanged;
        //    cmbModelObjectId.SelectedItem = selectedModelObject.ModelObjectEntity;
        //    cmbModelObjectId.SelectedIndexChanged += cmbModelObjectId_SelectedIndexChanged;

        //    if (selectedModelObject.ModelObjectEntity == null)
        //    {
        //        cmbModelObjectId.SelectedIndexChanged -= cmbModelObjectId_SelectedIndexChanged;
        //        cmbModelObjectId.Text = "NewModelObject";//string.Empty;
        //        cmbModelObjectId.SelectedIndexChanged += cmbModelObjectId_SelectedIndexChanged;
        //    }
        //    else
        //    {
        //        cmbModelObjectId.SelectedIndexChanged -= cmbModelObjectId_SelectedIndexChanged;
        //        //EntityName -> Prog ID
        //        cmbModelObjectId.Text = selectedModelObject.ModelObjectEntity.EntityName;
        //        cmbModelObjectId.SelectedIndexChanged += cmbModelObjectId_SelectedIndexChanged;
        //    }


        //    if (selectedModelObject.Name.Length > 4)
        //    {
        //        TextBox_ContractEditor_ModelObjectName.TextChanged -= TextBox_ContractEditor_ModelObjectName_TextChanged;

        //        //clear text initially if Cobol                
        //        //TextBox_ContractEditor_ModelObjectName.Text = selectedModelObject.Name.Substring(4);

        //        TextBox_ContractEditor_ModelObjectName.TextChanged += TextBox_ContractEditor_ModelObjectName_TextChanged;
        //    }


        //    ModelObjectPropertyGrid.SelectedObject = selectedModelObject;

        //    //TextBox_ContractEditor_Max_Occurences.TextChanged -= TextBox_ContractEditor_Max_Occurences_TextChanged;
        //    //TextBox_ContractEditor_Max_Occurences.Text = c1.MaxCount.ToString();
        //    //TextBox_ContractEditor_Max_Occurences.TextChanged += TextBox_ContractEditor_Max_Occurences_TextChanged;

        //    //TextBox_ContractEditor_Min_Occurences.TextChanged -= TextBox_ContractEditor_Min_Occurences_TextChanged;
        //    //TextBox_ContractEditor_Min_Occurences.Text = c1.MinCount.ToString();
        //    //TextBox_ContractEditor_Min_Occurences.TextChanged += TextBox_ContractEditor_Min_Occurences_TextChanged;



        //    //txtHostName.TextChanged -= txtHostName_TextChanged;
        //    //txtHostName.Text = c1.HostName;
        //    //txtHostName.TextChanged += txtHostName_TextChanged;


        //    k = node.Index;


        //    if (selectedModelObject.ModelObjectEntity == null)
        //    {
        //        lblInputModelObjectDefnReplacement.Visible = true;
        //        gridModelObjectItems.Visible = false;
        //    }
        //    else
        //    {
        //        gridModelObjectItems.Visible = true;
        //        lblInputModelObjectDefnReplacement.Visible = false;
        //        gridModelObjectItems.DataSource = selectedModelObject.ModelObjectEntity.DataItems;
        //    }
        //}
    }
}