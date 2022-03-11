using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

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
    public class ParserEntity
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

        /// <summary>
        /// The id of the program, this is an unique identifier gnerated from the 
        /// mainframe copy book.
        /// </summary>
        public string ProgramId
        {
            get { return programId; }
            set { programId = value; }
        }

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
        public GenericCollection<DataItem> DataItems
        {
            get { return dataItems; }
        }

        /// <summary>
        /// The class name of the serializer output of this copy book entity.
        /// </summary>
        public string SerializerClassName
        {
            get { return serializerClassName; }
            set { serializerClassName = value; }
        }

        /// <summary>
        /// The name of the data entity class output of this copy book entity.
        /// </summary>
        public string DataEntityClassName
        {
            get { return dataEntityClassName; }
            set { dataEntityClassName = value; }
        }

        /// <summary>
        /// The mainframe identifies the object being called by using the value of this property.
        /// </summary>
        public string ObjectId
        {
            get { return _objectId; }
            set { _objectId = value; }
        }

        /// <summary>
        /// The collection of Enums required in this data entity.
        /// </summary>
        public GenericCollection<EnumType> Enums
        {
            get { return enums; }
        }

    }
    #endregion


}
