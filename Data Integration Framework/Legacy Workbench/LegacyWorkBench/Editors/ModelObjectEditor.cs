using System;
using System.Windows.Forms;

namespace Infosys.Lif.LegacyWorkbench.Editors
{
    public partial class ModelObjectEditor : Editors.Editor
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

        private bool notesAdded;

        public bool NotesAdded
        {
            get { return notesAdded; }
            set { notesAdded = value; }
        }

        public delegate void OnModified(object sender, ModifiedEventArgs e);

        public event OnModified Modified;

        public Entities.Entity entityBoundTo;
        public Entities.Entity originalUnModifiedEntity;
        //adding an entity which contains unchanged value of the entity 
        public Entities.Entity removedItem;
        //a static variable to which will be compared to arrange the entity alphabetically
        //when user changes the name of entity and save the changes
        public static bool isEntityChangesSaved;
        public ModelObjectEditor()
        {
            InitializeComponent();
        }

        public void Populate(Entities.Entity entityToBeEdited)
        {
            originalUnModifiedEntity = entityToBeEdited;
            entityBoundTo = Clone(entityToBeEdited);
            Populate_ListBox(entityBoundTo);         
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
            modelObjectProperties.SelectedObject = ent;           
        }
         
        private void Button_ModelObjectEditor_Save_Click(object sender, EventArgs e)
        {

            //to remove the blank spaces at the end of text if any from Entity name and program id
            entityBoundTo.EntityName = entityBoundTo.EntityName.Trim().ToString();
            entityBoundTo.ProgramId = entityBoundTo.ProgramId.Trim().ToString();
            //code for removing blank spaces ends
            //to check whether any changes is done
            if (Modified != null)
            {
                //remove the entity modified from the hashtable ModelObjectEntityMapping
                LegacyParser.ModelObjectEntityMapping.Remove(originalUnModifiedEntity.ProgramId.ToString());
                //remove the entity modified from the hashtable ModelObjectEntityCollection
                LegacyParser.ModelObjectEntityCollection.Remove(originalUnModifiedEntity.EntityName.ToString());
                ////update the hashtable ModelObjectReferenceIDEntityCollection
                //LegacyParser.ModelObjectReferenceIDEntityCollection.Remove(originalUnModifiedEntity.ReferenceID.ToString());
                
                removedItem = originalUnModifiedEntity;
                originalUnModifiedEntity = entityBoundTo;
                //add the modified entity to the hashtable ModelObjectEntityMapping
                LegacyParser.ModelObjectEntityMapping.Add(originalUnModifiedEntity.ProgramId, originalUnModifiedEntity);
                //add the modified entity to the hashtable ModelObjectEntityMapping
                LegacyParser.ModelObjectEntityCollection.Add(originalUnModifiedEntity.EntityName, originalUnModifiedEntity.ProgramId.ToString());
                ////update the hashtable ModelObjectReferenceIDEntityCollection
                //LegacyParser.ModelObjectReferenceIDEntityCollection.Add(LegacyParser.ModelObjectReferenceIDEntityCollection.Count + 1, originalUnModifiedEntity);
                ModifiedEventArgs modifiedEventArgs = new ModifiedEventArgs();
                modifiedEventArgs.ModifiedEntity = entityBoundTo;

                //notes change   
                NotesAdded = false;
                if (entityBoundTo.Notes != null && entityBoundTo.Notes.Trim().Length > 0
                    && !entityBoundTo.Notes.Equals(""))
                {
                    NotesAdded = true;
                }
                //change end

                //DataEntityClassName n SerializerClassName in proper case
                modifiedEventArgs.ModifiedEntity.DataEntityClassName = ParseToProperCase(modifiedEventArgs.ModifiedEntity.EntityName);
                modifiedEventArgs.ModifiedEntity.SerializerClassName = ParseToProperCase(modifiedEventArgs.ModifiedEntity.EntityName);

                Modified(this, modifiedEventArgs);
            }
            //boolean variable "isEntityChangesSaved" is made true
            //to check if entity name has been changed
            if (removedItem.EntityName != originalUnModifiedEntity.EntityName)
            {
                isEntityChangesSaved = true;
            }
        }

        private string ParseToProperCase(string entityName)
        {
            //as entity name is allowed to be empty,check if its empty or not
            if (entityName == string.Empty)
            {
                return entityName;
            }
            else
            {
                return (entityName.Substring(0, 1).ToString() + entityName.Substring(1).ToLowerInvariant());
            }
        }
        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            entityBoundTo = Clone(originalUnModifiedEntity);
            Populate_ListBox(entityBoundTo);
        }        

        protected override void Save()
        {
            Button_ModelObjectEditor_Save_Click(this, new EventArgs());
            IsDirty = false;
        }

        private void modelObjectProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            LegacyParser objLegacyParser = new LegacyParser();
            TreeView currentTreeModelObject = new TreeView();
            currentTreeModelObject = objLegacyParser.retreiveTreeModelObjectValue(s, e);
            //code changes
            PropertyGrid modelObjPropertyGrid = (PropertyGrid)s;
            Entities.Entity entityObj = (Entities.Entity)modelObjPropertyGrid.SelectedObject;            

            //code to avoid user to enter duplicate entity names
            string[] arrayOfKeys = new string[LegacyParser.ModelObjectEntityMapping.Count];
            LegacyParser.ModelObjectEntityMapping.Keys.CopyTo(arrayOfKeys, 0);            

            //code to chk individual uniqueness of model object name and program id
            if (e.ChangedItem.Label == "Program Id")
            {
                if (e.ChangedItem.Value.ToString() == e.OldValue.ToString())
                {
                    MessageBox.Show("Program Id is not changed","Legacy Workbench",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    return;
                }
                else if (LegacyParser.ModelObjectEntityMapping.ContainsKey(e.ChangedItem.Value.ToString()))
                {
                    MessageBox.Show("Value exists", "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    entityObj.ProgramId = e.OldValue.ToString();
                    return;
                }
            }
            else if (e.ChangedItem.Label == "Model Object Name")
            {
                if (e.ChangedItem.Value.ToString() == e.OldValue.ToString())
                {
                    MessageBox.Show("Model Object Name is not changed", "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else if (LegacyParser.ModelObjectEntityCollection.ContainsKey(e.ChangedItem.Value.ToString())) 
                {
                    MessageBox.Show("Value exists", "Legacy Workbench", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    entityObj.EntityName = e.OldValue.ToString();
                    return;
                }                
            }
            //code to avoid user to enter duplicate entity names ends

            //to check the length of model object name
            if (modelObjectProperties.SelectedGridItem.Label.Equals("Model Object Name") && 
                e.ChangedItem.Value.ToString().Length > 30)
            {                
                string changedValue = e.ChangedItem.Value.ToString();                                
                
                MessageBox.Show("The length of model object name should not exceed 30.\n The name has been truncated to 30 characters.",
                    "Model Object Name Validation", MessageBoxButtons.OK,MessageBoxIcon.Information);
                
                entityObj.EntityName = changedValue.ToString().Substring(0, 30);
            }
        }

        private void modelObjectProperties_MouseClick(object sender, MouseEventArgs e)
        {
            //strore the object having the value of currProject before user makes any changes
            LegacyParser objLegacyParser = new LegacyParser();
            TreeView currentTreeModelObject = new TreeView();
            currentTreeModelObject = objLegacyParser.retreiveTreeModelObjectValue(sender,e);
            
        }

        private void modelObjectProperties_Enter(object sender, EventArgs e)
        {
            //strore the object having the value of currProject before user makes any changes
            LegacyParser objLegacyParser = new LegacyParser();
            TreeView currentTreeModelObject = new TreeView();
            currentTreeModelObject = objLegacyParser.retreiveTreeModelObjectValue(sender, e);
        }

        private void ModelObjectEditor_Leave(object sender, EventArgs e)
        {
            entityBoundTo = originalUnModifiedEntity;            
        }

    }
}
