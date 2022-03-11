using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * This class enables us to define Enums for an entity. 
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/

namespace Infosys.Lif.LegacyWorkbench.Entities
{
    #region EnumType Class Definition
    /// <summary>
    /// The class used to define an enumeration.
    /// </summary>
    public class EnumType
    {
        /// <summary>
        /// The name of the enumeration.
        /// </summary>
        string _name;

        /// <summary>
        /// The colleciton of various properties possible in this enumeration
        /// </summary>
        GenericCollection<EnumPropertyType> _enumProperties
            = new GenericCollection<EnumPropertyType>();

        /// <summary>
        /// The name of the enumeration.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The various possiblities for this enumeration can be 
        /// set/retrieved using this property
        /// </summary>
        public GenericCollection<EnumPropertyType> EnumProperties
        {
            get { return _enumProperties; }
        }
    } 
    #endregion

    #region EnumPropertyType definition
    /// <summary>
    /// An enumeration property can be defined through this type
    /// </summary>
    public class EnumPropertyType
    {
        /// <summary>
        /// The name by which this property should be known.
        /// </summary>
        string _name;

        /// <summary>
        /// The value which should be placed in the output of the serializer, 
        /// if this property is selected by the user.
        /// </summary>
        string _value;

        /// <summary>
        /// The name of the property as the final devloper should see.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// The value of this property. This is the value which will be placed in the output of the serializer
        /// if this property is selected.
        /// </summary>
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
    } 
    #endregion
}
