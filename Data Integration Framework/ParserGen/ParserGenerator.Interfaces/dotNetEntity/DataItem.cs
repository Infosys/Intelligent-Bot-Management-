using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * This class is a .NET specific type which denotes a variable. 
 * All the input mainframe copy books will be converted to members of this type,
 * This is a part which is specific to the Legacy Parser utility. 
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 * ***************************************************************/

namespace Infosys.Lif.LegacyParser
{
    /// <summary>
    /// This defines a single variable of the Data Entity. This stores 
    /// the various dimensions of the variables such as the length, 
    /// position in the output string,etc.
    /// </summary>
    public class DataItem
    {
        /// <summary>
        /// The name of the item.
        /// </summary>
        string _itemName;

        /// <summary>
        /// Should the developer be able to see this variable?
        /// </summary>
        bool isVisible = true;

        /// <summary>
        /// Decides the type of the variable
        /// </summary>
        DataItemType itemType;

        /// <summary>
        /// The length of this variable in the output string.
        /// </summary>
        int length;

        /// <summary>
        /// If this is an enum type the Enum name will be stored in this variable.
        /// </summary>
        string typeOfEnum = string.Empty;

        /// <summary>
        /// The position of this variable in the serializer-output string.
        /// </summary>
        int position;

        /// <summary>
        /// Hardcoded value, for this variable. 
        /// </summary>
        string _value;

        /// <summary>
        /// The name of the item as decided by the Language Converter.
        /// </summary>
        public string ItemName
        {
            get { return _itemName; }
            set { _itemName = value; }
        }

        /// <summary>
        /// Boolean inicating whether this variable is visible to the developers.
        /// </summary>
        public bool IsVisible
        {
            get { return isVisible; }
            set { isVisible = value; }
        }

        /// <summary>
        /// The variable type.
        /// </summary>
        public DataItemType ItemType
        {
            get { return itemType; }
            set { itemType = value; }
        }

        /// <summary>
        /// The length of this variable in the serializer-output.
        /// </summary>
        public int Length
        {
            get { return length; }
            set { length = value; }
        }


        /// <summary>
        /// If the variable type is an enum, this will be the name of the enum
        /// </summary>
        public string EnumName
        {
            get { return typeOfEnum; }
            set { typeOfEnum = value; }
        }


        /// <summary>
        /// The location of this variable in the serializer-outputs.
        /// </summary>
        public int Position
        {
            get { return position; }
            set { position = value; }
        }

        /// <summary>
        /// Hardcoded value of this variable.
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }

        /// <summary>
        /// The various possible types possible for a variable
        /// </summary>
        public enum DataItemType
        {
            /// <summary>
            /// This indicates that the variable is an integer type.
            /// </summary>
            UnsignedIntegerType,
            /// <summary>
            /// This indicates that the variable is a string type.
            /// </summary>
            StringType,
            /// <summary>
            /// This indicates that the varaible is an enumeration
            /// </summary>
            EnumType,
            /// <summary>
            /// This indicates that the varaible is an enumeration
            /// </summary>
            SignedIntegerType,
            /// <summary>
            /// This indicates that the variable definiton is a group type.
            /// </summary>
            GroupItemType
        }

        GenericCollection<DataItem> groupItems = new GenericCollection<DataItem>();

        /// <summary>
        /// If this data item is a group item this will contain all the items in
        /// the group.
        /// </summary>
        public GenericCollection<DataItem> GroupItems
        {
            get { return groupItems; }
            set { groupItems = value; }
        }
        private int numberOfOccurences = 1;

        public int NumberOfOccurences
        {
            get { return numberOfOccurences; }
            set { numberOfOccurences = value; }
        }

        public enum ParameterDirectionType
        {
            InputType,
            OutputType,
            InputAndOutput
        }
        private ParameterDirectionType direction = ParameterDirectionType.InputAndOutput;

        public ParameterDirectionType Direction
        {
            get { 
                return direction; }
            set 
            {
                if (ItemType == DataItemType.GroupItemType)
                {
                    for (int looper = 0; looper < groupItems.Count; looper++)
                    {
                        Correct(groupItems[looper], value);
                    }
                }
                direction = value; 
            }
        }

        private void Correct(DataItem dataItem, ParameterDirectionType value)
        {
            dataItem.Direction = value;
        }
        private int redefines = -1;

        public int Redefines
        {
            get { return redefines; }
            set { redefines = value; }
        }

    }
}
