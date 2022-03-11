using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * This class is a .NET specific type which denotes a variable. 
 * All the input mainframe copy books will be converted to members of this type,
 * This is a part which is specific to the parser generator utility. 
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/

namespace Infosys.Lif.LegacyParser.UI
{
    #region Generic ChangedEventArgs definition
    /// <summary>
    /// Objects of this type will be used to pass the parameters for ChangedEventHandler 
    /// </summary>
    /// <typeparam name="T">Either an Entity or Module depending on the item changed.</typeparam>
    public class ChangedEventArgs<T> : EventArgs
    {
        string propertyChanged = string.Empty;

        public string PropertyChanged
        {
            get { return propertyChanged; }
            set { propertyChanged = value; }
        }


        T newValue;
        public T NewValue
        {
            get { return newValue; }
            set { newValue = value; }
        }
    } 
    #endregion
}
