using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using System.ComponentModel;

/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * This class is the Legacy parser .NET entity type. 
 * This entity type defines the Data Entity and serializer available to the developers.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/

namespace Infosys.Lif.LegacyWorkbench.Entities
{
    #region Definition of Entity class
    /// <summary>
    /// This class is generated from one mainframe copy book. This defines 
    /// a whole Data Entity and serializer. The data entity will 
    /// </summary>
    public class Entity
    {
        /// <summary>
        /// The program id of this object. The program id is specific to a mainframe copy 
        /// book and is used to identify the mainframe program.
        /// </summary>
        string programId;        
        /// <summary>
        /// The entity name as should appear finally to developers.
        /// </summary>
        string entityName = string.Empty;

        /// <summary>
        /// This property defines the various variables in the final Data Entity 
        /// and serializers.
        /// </summary>
        GenericCollection<DataItem> dataItems = new GenericCollection<DataItem>();

        /// <summary>
        /// The name which the serializer class should have.
        /// </summary>
        string serializerClassName = string.Empty;

        /// <summary>
        /// The name the Data Entity class should be generated as.
        /// </summary>
        string dataEntityClassName = string.Empty;

        /// <summary>
        /// The id of the object as decided by the mainframe.
        /// This will be used by the mainframe as to which object is being called
        /// </summary>
        string _objectId;

        /// <summary>
        /// The collection holding all th eenums required in this Data Entity.
        /// </summary>
        GenericCollection<EnumType> enums = new GenericCollection<EnumType>();



        [DisplayName("Program Id")]
        //making Program ID editable changes
        //[ReadOnly(true)]
        //making Program ID editable changes ends
        [Description("The id of the program, this is an unique identifier generated from the mainframe copy book.")]        
        /// <summary>
        /// The id of the program, this is an unique identifier generated from the 
        /// mainframe copy book.
        /// </summary>
        public string ProgramId
        {            
            get { return programId; }
            set { programId = value; }
        }


        [DisplayName("Model Object Name")]
        [Description("This is the name of the Data Entity which will be generated and is used only for display purposes.")]
        /// <summary>
        /// This is the name of the Data Entity which will be generated and 
        /// is used only for display purposes.
        /// </summary>
        public string EntityName
        {
            get { return entityName; }
            set { entityName = value; }
        }


        /// <summary>
        /// The various variables of the data entity will be contained in this property.
        /// </summary>
        [DisplayName("Data Items")]
        [Description("Collection of the data items in the model object")]
        public GenericCollection<DataItem> DataItems
        {
            get { return dataItems; }
        }



        [Browsable(false)]
        /// <summary>
        /// The class name of the serializer output of this copy book entity.
        /// </summary>
        public string SerializerClassName
        {
            get { return serializerClassName; }
            set { serializerClassName = value; }
        }

        [Browsable(false)]
        /// <summary>
        /// The name of the data entity class output of this copy book entity.
        /// </summary>
        public string DataEntityClassName
        {
            get { return dataEntityClassName; }
            set { dataEntityClassName = value; }
        }


        //[Browsable(false)]
        ///// <summary>
        ///// The mainframe identifies the object being called by using the value of this property.
        ///// </summary>
        //public string ObjectId
        //{
        //    get { return _objectId; }
        //    set { _objectId = value; }
        //}


        [Browsable(false)]
        /// <summary>
        /// The collection of Enums required in this data entity.
        /// </summary>
        public GenericCollection<EnumType> Enums
        {
            get { return enums; }
        }


        bool isToBeGenerated = false;
        [Browsable(false)]
        public bool IsToBeGenerated
        {
            get { return isToBeGenerated; }
            set { isToBeGenerated = value; }
        }


        string notes;
        [DisplayName("Notes")]
        [Description("Any details added along with the model object")]
        public string Notes
        {
            get { return notes; }
            set { notes = value; }
        }


        //to add a new item which will be the identity for this entity
        private string  referenceID;
        [Browsable(false)]
        [DisplayName("Reference Identity")]
        [Description("This is the identity for this entity, non editable")]
        [ReadOnly(true)]
        public string ReferenceID
        {
            get { return referenceID; }
            set { referenceID = value; }
        }
        
    }
    #endregion


}
