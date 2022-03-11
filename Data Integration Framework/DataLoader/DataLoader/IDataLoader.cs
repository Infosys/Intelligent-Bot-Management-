/******************************************************************************
 * This file is a part of the Legacy Integration Framework.
 * This file provides an interface to CustomLoader
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Infosys.Lif.DataLoader
{
    /// <summary>
    /// interface to call GetObject() which is implemented by CustomLoader.XmlDataLoader 
    /// </summary>
    public interface IDataLoader
    {
        T GetData <T> (string fileName, T entity);
    }
}
