using System;
using System.Collections;
using System.Xml.Serialization;
/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * This file defines the various types required to read the storage implementations
 * mentioned in the configuration file
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/

namespace Infosys.Lif.LegacyParser.UI.Configurations
{
    #region StorageImplementation definition
    /// <summary>
    /// This type is used to define a StorageImplementation in the configuration file.
    /// </summary>
    public class StorageImplementation
    {
        /// <summary>
        /// The type (class) to be used if this Storage implementation is selected.
        /// </summary>
        string _type;

        /// <summary>
        /// User friendly unique name for this storage implementation.
        /// </summary>
        string _name;

        /// <summary>
        /// User friendly unique name for this storage implementation.
        /// </summary>
        [XmlAttribute("name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The full name of the type to be loaded if this storage implementation 
        /// is required.
        /// </summary>
        [XmlAttribute("type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

    }
    #endregion

    #region StorageImplementationCollection definition
    /// <summary>
    /// An aggregation of StorageImplementation objects
    /// </summary>
    public class StorageImplementationCollection : 
        GenericCollection<StorageImplementation>
    {
        
        /// <summary>
        /// Used to refer to a StorageImplementation by name
        /// </summary>
        /// <param name="searchKey">The name of the storage Implementation object required.</param>
        /// <returns>The StorageImplementation object having the name as mentioned in the searchKey</returns>
        public StorageImplementation this[string searchKey]
        {
            get
            {
                // Loop thru the colection and retrieve the object which has the name 
                // as mentioned in the searchKey
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
        /// Method used to check whether a name is already present in 
        /// the collection.
        /// </summary>
        /// <param name="key">The name to be searched for in the collection</param>
        /// <returns>
        /// True, if the collection contains an object with the name as defined 
        /// by input parameter key.
        /// False, otherwise
        /// </returns>
        public bool ContainsKey(string key)
        {
            // Loop thru the collection and check whether any object has the name 
            // as mentioned in input parameter "key"
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
