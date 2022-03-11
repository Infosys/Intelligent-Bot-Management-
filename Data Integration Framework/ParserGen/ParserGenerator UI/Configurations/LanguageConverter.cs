using System;
using System.Collections;
using System.Xml.Serialization;
/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * The classes in this file will be used to read/write the 
 * configuration parameters from/to the config file.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/

namespace Infosys.Lif.LegacyParser.UI.Configurations
{
    #region LanguageConverter definition
    /// <summary>
    /// The various LanguageConverters in the config file will be 
    /// deserialized to objects of this LanguageConverter type.
    /// </summary>
    public class LanguageConverter
    {
        /// <summary>
        /// The user-friendly name given to the Language Converter
        /// </summary>
        string _name;
        /// <summary>
        /// The type which contains the implementation of the Language Converter
        /// </summary>
        string _type;

        /// <summary>
        /// The user-friendly name given to the Language Converter
        /// </summary>
        [XmlAttribute("name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The type which contains the implementation of the Language Converter
        /// </summary>
        [XmlAttribute("type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }
    }
    #endregion


    #region LanguageConverterCollecion definition
    /// <summary>
    /// Aggregation of LanguageConverter types can be built through 
    /// this implementation of ArrayList.
    /// </summary>
    public class LanguageConverterCollection : GenericCollection<LanguageConverter>
    {
        /// <summary>
        /// Used to retrieve the LanguagConverter in the collection which has name as 
        /// in the searchKey input.
        /// </summary>
        /// <param name="searchKey">The name which has to be searched 
        /// for in the collection.</param>
        /// <returns>
        /// The LanguageConverter object which has the name as defined in searchKey input
        /// </returns>
        public LanguageConverter this[string searchKey]
        {
            get
            {
                // Loop thru the items in the base List and return the object which has 
                // name = searchKey
                for (int i = 0; i < Count; i++)
                {
                    if (this[i].Name.Equals(searchKey))
                    {
                        return this[i];
                    }
                }
                return null;
            }
        }
        /// <summary>
        /// Method used to decide whether a name is already in the collection.
        /// </summary>
        /// <param name="key">
        /// The name whose existence we need to check in the collection.
        /// </param>
        /// <returns>
        /// True, if the name already exists in the collection. False, otherwise
        /// </returns>
        public bool ContainsKey(string key)
        {
            // Loop thru the base list and check for the presence of the 
            // key as a name for any of the objects in the collection.
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Name.Equals(key))
                {
                    return true;
                }
            }
            return false;
        }
    }
    #endregion
}
