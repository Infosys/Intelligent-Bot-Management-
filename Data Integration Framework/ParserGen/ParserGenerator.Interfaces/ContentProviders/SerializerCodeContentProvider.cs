using System;
using System.Collections.Generic;
using System.Text;

// Infosys Code generator v1.1
using Infosys.Solutions.CodeGeneration.Framework;


/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * The classes in this file will be used to provide content to the 
 * parsers being generated, based on the templates.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 * ***************************************************************/

namespace Infosys.Lif.LegacyParser.ContentProviders
{
    #region SerializerCodeContentProvider definition
    /// <summary>
    /// This is the content provider for the serializer files. This contains the various parameters
    /// which will replace the placeholders on the template.
    /// </summary>
    public class SerializerCodeContentProvider : ContentProvider
    {
        /// <summary>
        /// This will determine the entity for which the Serializer is being generated.
        /// This will be initialized in the constructor.
        /// </summary>
        private Entity entityToBeUsed;
        /// <summary>
        /// The namespace to be given to the data entity.
        /// </summary>
        string _dataEntityNamespace = " ";

        /// <summary>
        /// This will be the namespace of the serializer class.
        /// </summary>
        string _namespace = " ";

        /// <summary>
        /// The publicly available constructor. Will initialize the internal 
        /// variables to store the entity for which the serializer is being generated.
        /// </summary>
        /// <param name="entityToBeBuilt">The entity for which the serializer has to be built.</param>
        public SerializerCodeContentProvider(Entity entityToBeBuilt)
        {
            entityToBeUsed = entityToBeBuilt;
        }

        /// <summary>
        /// The value of this property will be used to determine/set the namespace 
        /// of the Data Entity.
        /// </summary>
        [PlaceHolder("DataEntityNameSpace")]
        public string DataEntityNamespace
        {
            get
            {
                return _dataEntityNamespace;
            }
            set
            {
                _dataEntityNamespace = value;
            }
        }

        /// <summary>
        /// The value of this property determines the namespace of the serializer class.
        /// </summary>
        [PlaceHolder("Namespace")]
        public string Namespace
        {
            get
            {
                return _namespace;
            }
            set
            {
                _namespace = value;
            }
        }

        /// <summary>
        /// The value of this property determines the name to be used for this serializer class.
        /// </summary>
        [PlaceHolder("ClassName")]
        string ClassName
        {
            get
            {
                return entityToBeUsed.SerializerClassName;
            }
        }

        /// <summary>
        /// This property determines the name of the class being used as the 
        /// Data Entity.
        /// </summary>
        [PlaceHolder("DataEntityClass")]
        string DataEntityClass
        {
            get
            {
                return entityToBeUsed.DataEntityClassName;
            }
        }

        /// <summary>
        /// This property determines the header which should be passed as header for this object.
        /// This Object Id is utilized by the mainframe to determine which object should be called.
        /// </summary>
        [PlaceHolder("ObjectId")]
        string ObjectId
        {
            get
            {
                return entityToBeUsed.ObjectId;
            }
        }
        /// <summary>
        /// This is the length of the string generated by the serializer.
        /// </summary>
        [PlaceHolder("LengthOfDataEntity")]
        string LengthOfDataEntity
        {
            get
            {
                // Determine the length required as per the copy book.
                int totalLength = GetLength(entityToBeUsed.DataItems);
                return totalLength.ToString();
            }

        }

        private int GetLength(GenericCollection<DataItem> dataItems)
        {
            int totalLength = 0;
            for (int itemLooper = 0; itemLooper < dataItems.Count;
                itemLooper++)
            {
                if (dataItems[itemLooper].ItemType == DataItem.DataItemType.GroupItemType)
                {
                    totalLength += GetLength(dataItems[itemLooper].GroupItems);
                }
                else
                {
                    DataItem currItem = dataItems[itemLooper];
                    if (currItem.IsVisible)
                    {
                        totalLength += currItem.Length;
                    }
                }
            }
            return totalLength;
        }

        /// <summary>
        /// The code used to parse the data entity to a string is generated 
        /// by this property.
        /// </summary>
        [PlaceHolder("ParseToString")]
        string ParseToString
        {
            get
            {
                string strFilled = string.Empty;
                strFilled = BuildParseToString(entityToBeUsed.DataItems);

                // return all generated code.
                return strFilled;
            }
        }

        private string BuildParseToString(GenericCollection<DataItem> genericCollection)
        {
            string strFilled = string.Empty;
            // Loop the items in the entity
            for (int itemLooper = 0; itemLooper < genericCollection.Count;
                itemLooper++)
            {
                DataItem currItem = genericCollection[itemLooper];
                if (currItem.Direction == DataItem.ParameterDirectionType.InputAndOutput
                    ||
                    currItem.Direction == DataItem.ParameterDirectionType.InputType)
                {
                    if (currItem.ItemType == DataItem.DataItemType.GroupItemType)
                    {
                        strFilled += BuildParseToString(currItem.GroupItems);

                    }
                    // If there is no hardcoded value.
                    else if ((currItem.Value == null || currItem.Value == string.Empty)
                        && currItem.IsVisible && currItem.Length > 0
                        )
                    {
                        // If the item type is an enum, a special switch case 
                        // has to be generated. This can be genreated by using various 
                        // content providers.
                        if (currItem.ItemType == DataItem.DataItemType.EnumType)
                        {
                            EnumType enumObject = null;
                            for (int enumLooper = 0; enumLooper <
                                entityToBeUsed.Enums.Count; enumLooper++)
                            {
                                EnumType enumeration = entityToBeUsed.Enums[enumLooper];
                                if (enumeration.Name == currItem.EnumName)
                                {
                                    enumObject = enumeration;
                                    break;
                                }
                            }

                            if (enumObject != null)
                            {

                                Template ParseToStringItemEnumCaseTemplate =
                                    ContentTemplate.RepeatingTemplate(
                                    ParseToStringItemEnumCase.ProvidesContentFor);

                                // Generate the switch clause.
                                ParseToStringItemEnum enumContentProvider
                                    = new ParseToStringItemEnum(
                                    ParseToStringItemEnumCaseTemplate, currItem, enumObject);

                                // Set the values for the enum content provider.
                                enumContentProvider.DataEntityClass = ClassName;
                                enumContentProvider.ContentTemplate
                                    = ContentTemplate.RepeatingTemplate(
                                    ParseToStringItemEnum.ProvidesContentFor);
                                // Generate the content of the switch clause.
                                strFilled += enumContentProvider.GenerateContent();
                            }
                        }
                        else
                        {
                            if (currItem.ItemType == DataItem.DataItemType.SignedIntegerType)
                            {
                                ParserDataItemContentProvider signedItemContentProvider
                                    = new ParserDataItemContentProvider(currItem);
                                signedItemContentProvider.ContentTemplate
                                    = ContentTemplate.RepeatingTemplate("ParseToStringSignedType");
                                signedItemContentProvider.DataEntityClass = ClassName;
                                strFilled += signedItemContentProvider.GenerateContent();

                            }
                            // Provide content for the non-enum types.
                            ParserDataItemContentProvider itemContentProvider
                                = new ParserDataItemContentProvider(currItem);

                            itemContentProvider.ContentTemplate
                                = ContentTemplate.RepeatingTemplate("ParseToStringItem");

                            // Set the properties for the content provider.
                            itemContentProvider.DataEntityClass = DataEntityClass;

                            // Generate and retrieve the content.
                            strFilled += itemContentProvider.GenerateContent();
                        }
                    }
                    else if (currItem.Length > 0)
                    {
                        if (currItem.Value == null)
                        {
                            currItem.Value = string.Empty;
                        }
                        if (currItem.ItemType == DataItem.DataItemType.UnsignedIntegerType)
                        {
                            if (currItem.Value == string.Empty)
                            {
                                currItem.Value = "0";
                            }
                        }
                        if (currItem.ItemType == DataItem.DataItemType.SignedIntegerType)
                        {
                            if (currItem.Value == string.Empty)
                            {
                                currItem.Value = " 0";
                            }
                        }
                        // Generate the content for the hard coded template.
                        ParseToStringItemHardCodedProvider hardCodingContent
                            = new ParseToStringItemHardCodedProvider(
                            currItem);

                        hardCodingContent.ContentTemplate
                            = ContentTemplate.RepeatingTemplate(
                            ParseToStringItemHardCodedProvider.ProvidesContentFor);

                        strFilled += hardCodingContent.GenerateContent();
                    }
                }
            }
            return strFilled;
        }

        /// <summary>
        /// This property fills up the place holder to hold the code to 
        /// parse a string to Data Entity.
        /// </summary>
        [PlaceHolder("ParseToEntity")]
        string ParseToEntity
        {
            get
            {
                string strContent = BuildParseToDataEntity(entityToBeUsed.DataItems);

                return strContent;
            }
        }

        private string BuildParseToDataEntity(GenericCollection<DataItem> genericCollection)
        {
            string strContent = string.Empty;
            // Loop through the items (variables) in the entity.
            for (int itemLooper = 0;
                itemLooper < genericCollection.Count;
                itemLooper++)
            {
                DataItem item = genericCollection[itemLooper];

                if ((item.Direction == DataItem.ParameterDirectionType.OutputType
                    || item.Direction == DataItem.ParameterDirectionType.InputAndOutput)
                    )
                {
                    if (item.IsVisible)
                    {
                        if (item.ItemType == DataItem.DataItemType.GroupItemType)
                        {
                            strContent += BuildParseToDataEntity(item.GroupItems);
                        }
                        else if ((item.Value == null || item.Value == string.Empty) && item.Length > 0)
                        {

                            // As per the type of the variable, decide the code content.
                            switch (item.ItemType)
                            {
                                case DataItem.DataItemType.StringType:
                                    // The variable is a string type.
                                    ParseToEntityStringContentProvider contProv
                                        = new ParseToEntityStringContentProvider(item);

                                    contProv.ContentTemplate
                                        = ContentTemplate.RepeatingTemplate(
                                        ParseToEntityStringContentProvider.ProvidesContentFor);

                                    strContent += contProv.GenerateContent();

                                    break;
                                case DataItem.DataItemType.UnsignedIntegerType:

                                    // The variable is an integer type.
                                    ParseToEntityIntContentProvider intContProv
                                        = new ParseToEntityIntContentProvider(item);

                                    intContProv.ContentTemplate
                                        = ContentTemplate.RepeatingTemplate(
                                        ParseToEntityIntContentProvider.ProvidesContentFor);

                                    strContent += intContProv.GenerateContent();
                                    break;
                                default:
                                    // 
                                    break;
                            }
                        }
                        else if (item.Length > 0)
                        {
                            ParseToStringItemHardCodedProvider provider = new ParseToStringItemHardCodedProvider(item);
                            provider.ContentTemplate = ContentTemplate.RepeatingTemplate("ParseToEntityHardCoded");
                            strContent += provider.GenerateContent();
                        }
                    }
                }
            }
            return strContent;
        }
    }

    #endregion

    #region ParseToEntityIntContentProvider definition
    internal class ParseToEntityIntContentProvider : ContentProvider
    {
        internal const string ProvidesContentFor = "ParseToEntityInt";

        DataItem item;
        internal ParseToEntityIntContentProvider(DataItem stringItem)
        {
            item = stringItem;
        }
        [PlaceHolder("DataItemName")]
        internal string DataItemName
        {
            get
            {
                return item.ItemName;
            }
        }
        [PlaceHolder("Position")]
        internal string Position
        {
            get
            {
                return item.Position.ToString();
            }
        }
        [PlaceHolder("Length")]
        internal string Length
        {
            get
            {
                return item.Length.ToString();
            }
        }
    }
    #endregion

    #region ParseToEntityStringContentProvider definition
    internal class ParseToEntityStringContentProvider : ContentProvider
    {
        internal const string ProvidesContentFor = "ParseToEntityString";

        DataItem item;
        internal ParseToEntityStringContentProvider(DataItem stringItem)
        {
            item = stringItem;
        }
        [PlaceHolder("DataItemName")]
        internal string DataItemName
        {
            get
            {
                return item.ItemName;
            }
        }
        [PlaceHolder("Position")]
        internal string Position
        {
            get
            {
                return item.Position.ToString();
            }
        }
        [PlaceHolder("Length")]
        internal string Length
        {
            get
            {
                return item.Length.ToString();
            }
        }
    }

    #endregion

    #region ParseToStringItemHardCodedProvider definition
    internal class ParseToStringItemHardCodedProvider : ContentProvider
    {
        internal const string ProvidesContentFor = "ParseToStringItemHardCoded";
        DataItem _hardCodedItem = null;
        internal ParseToStringItemHardCodedProvider(DataItem itemToProvideCodeFor)
        {
            _hardCodedItem = itemToProvideCodeFor;
        }
        [PlaceHolder("Value")]
        string Value
        {
            get
            {
                if (_hardCodedItem.ItemType == DataItem.DataItemType.StringType)
                {
                    return "\"" + _hardCodedItem.Value + "\"";
                }
                else
                {
                    return _hardCodedItem.Value;
                }

            }
        }
        [PlaceHolder("ItemName")]
        string ItemName
        {
            get
            {
                return _hardCodedItem.ItemName;
            }
        }
        [PlaceHolder("ItemLength")]
        string ItemLength
        {
            get
            {
                return _hardCodedItem.Length.ToString();
            }
        }
        [PlaceHolder("PadString")]
        string PadString
        {
            get
            {
                switch (_hardCodedItem.ItemType)
                {
                    case DataItem.DataItemType.UnsignedIntegerType: return "0";
                    default: return " ";
                }
            }
        }
        [PlaceHolder("IsStringType")]
        string IsStringType
        {
            get
            {
                return (_hardCodedItem.ItemType == DataItem.DataItemType.StringType).ToString().ToLowerInvariant();
            }
        }
    }

    #endregion

    #region ParserDataItemContentProvider definition
    internal class ParserDataItemContentProvider : ContentProvider
    {
        DataItem dataItem;
        public ParserDataItemContentProvider(DataItem itemToBeUsed)
        {
            dataItem = itemToBeUsed;
        }
        [PlaceHolder("DataItemName")]
        string DataItemName
        {
            get
            {
                return dataItem.ItemName;
            }
        }
        [PlaceHolder("DataItemLength")]
        string DataItemLength
        {
            get
            {
                return dataItem.Length.ToString();
            }
        }
        [PlaceHolder("PadItem")]
        string PadItem
        {
            get
            {
                switch (dataItem.ItemType)
                {
                    case DataItem.DataItemType.StringType:
                        return " ";
                    case DataItem.DataItemType.UnsignedIntegerType:
                        return "0";
                    default:
                        return "0";
                }
            }
        }

        [PlaceHolder("IsStringType")]
        string IsStringType
        {
            get
            {
                switch (dataItem.ItemType)
                {
                    case DataItem.DataItemType.StringType:
                        return "true";
                    case DataItem.DataItemType.UnsignedIntegerType:
                        return "false";
                    case DataItem.DataItemType.SignedIntegerType:
                        return "false";
                    default:
                        return "false";
                }
            }
        }
        string _dataEntityClass = string.Empty;
        [PlaceHolder("DataEntityClass")]
        internal string DataEntityClass
        {
            get
            {
                return _dataEntityClass;
            }
            set
            {
                _dataEntityClass = value;
            }
        }


    }

    #endregion

    #region ParseToStringItemEnum definition
    internal class ParseToStringItemEnum : ContentProvider
    {
        private DataItem dataItem;
        private EnumType enumTypeObject;


        internal const string ProvidesContentFor = "ParseToStringItemEnum";
        internal Template ParseToStringItemEnumCaseTemplate;
        internal ParseToStringItemEnum(Template parseToStringItemEnumCaseTemplate,
            DataItem itemToBeUsed, EnumType enumObject)
        {
            ParseToStringItemEnumCaseTemplate = parseToStringItemEnumCaseTemplate;
            enumTypeObject = enumObject;
            dataItem = itemToBeUsed;
        }

        string _dataEntityClass = string.Empty;
        [PlaceHolder("DataEntityClass")]
        internal string DataEntityClass
        {
            get
            {
                return _dataEntityClass;
            }
            set
            {
                _dataEntityClass = value;
            }
        }
        [PlaceHolder("DataItemName")]
        private string DataItemName
        {
            get
            {
                return dataItem.ItemName;
            }
        }
        [PlaceHolder("ParseToStringItemEnumCases")]
        private string ParseToStringItemEnumCases
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (EnumPropertyType property in enumTypeObject.EnumProperties)
                {
                    ParseToStringItemEnumCase eachCase
                        = new ParseToStringItemEnumCase(property);
                    eachCase.ContentTemplate = ParseToStringItemEnumCaseTemplate;
                    eachCase.EnumName = enumTypeObject.Name;
                    sb.Append(eachCase.GenerateContent());
                }
                return sb.ToString();
            }
        }
    }
    #endregion


    #region ParseToStringItemEnumCase definition
    internal class ParseToStringItemEnumCase : ContentProvider
    {

        internal const string ProvidesContentFor = "ParseToStringItemEnumCase";

        string _enumName, _enumType, _enumValue;
        internal ParseToStringItemEnumCase(EnumPropertyType propertyType)
        {
            _enumType = propertyType.Name;
            _enumValue = propertyType.Value;

        }
        [PlaceHolder("EnumName")]
        internal string EnumName
        {
            get
            {
                return _enumName;
            }
            set
            {
                _enumName = value;
            }
        }
        [PlaceHolder("EnumTypeName")]
        string EnumTypeName
        {
            get
            {
                return _enumType;
            }
        }
        [PlaceHolder("EnumValue")]
        string EnumValue
        {
            get
            {
                _enumValue = _enumValue.Replace("'", "");
                return _enumValue;
            }
        }
    }

    #endregion


}
