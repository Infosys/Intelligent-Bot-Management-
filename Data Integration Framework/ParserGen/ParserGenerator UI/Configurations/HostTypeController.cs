using System;
using System.Collections;
using System.Xml.Serialization;
/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * The classes in this file are used as parts of the 
 * configruation settings reader and writers.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/

namespace Infosys.Lif.LegacyParser.UI.Configurations
{
    #region HostTypeController definition
    /// <summary>
    /// This is the class to which the Host controller configuration 
    /// sections will be serialized. 
    /// </summary>
    public class HostTypeController
    {
        /// <summary>
        /// The name of the host type.
        /// </summary>
        string _name;

        /// <summary>
        /// The full type name of the class which should be used if this is selected 
        /// as the host type
        /// </summary>
        string _type;

        /// <summary>
        /// The name of the host type.
        /// </summary>
        [XmlAttribute("name")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The full class name having the implementation of the host type 
        /// controller.
        /// </summary>
        [XmlAttribute("type")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }
    }
    #endregion

    #region HostTypeControllerCollection definition
    /// <summary>
    /// The type for aggregation of HostTypeController types.
    /// </summary>
    public class HostTypeControllerCollection : GenericCollection<HostTypeController>
    {
        /// <summary>
        /// This method allows to check whether a name already exists in the list.
        /// </summary>
        /// <param name="key">
        /// The name of the HostTypeController which has to be checked for.
        /// </param>
        /// <returns>
        /// returns true, if the name as mentioned in the key exists, else false.
        /// </returns>
        public bool ContainsKey(string key)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Name.Equals(key))
                {
                    return true;
                }
            }
            return false;
        }
        
        
        
        
        /// <summary>
        /// Used to retrieve the HostTypeController in the collection which has name as 
        /// in the searchKey input.
        /// </summary>
        /// <param name="searchKey">The name which has to be searched 
        /// for in the collection.</param>
        /// <returns>
        /// The HostTypeController object which has the name as defined in searchKey input
        /// </returns>
        public HostTypeController this[string searchKey]
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
    }
    #endregion
}
