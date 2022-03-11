/******************************************************************************
 * This file is a part of the Legacy Integration Framework.
 * This file handles deserialization to return object with data from XML file
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Resources;
using Infosys.Lif.DataLoader;

namespace Infosys.Lif.CustomLoader
{
    /// <summary>
    /// Implements IDataLoader interface
    /// </summary>
    public class XmlDataLoader : IDataLoader
    {
        /// <summary>
        /// reads given XML file and deserialize it
        /// </summary>
        /// <param name="fileName">XML file name</param>
        /// <param name="entity">object to refer for type</param>
        /// <returns>returns an object for passed object</returns>
        public T GetData <T> (string fileName, T entity)
        {                        
            object entityToReturn = null;

            //validate entity and file name
            if (entity.Equals(null) || String.IsNullOrEmpty(fileName.Trim()))
            {
                //throw exception                
                throw new LoaderException(Infosys.Lif.DataLoader.Properties.Resource1.ExcepCustomLoaderObjNull);
            }

            else
            {
                //xml deserialization
                Type typeOfEntity = entity.GetType();
                try
                {
                    XmlSerializer xs = new XmlSerializer(typeOfEntity);
                    FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    entityToReturn = (object)xs.Deserialize(fs);
                    fs.Close();
                }
                catch (Exception ex)
                {                    
                    throw new LoaderException(Infosys.Lif.DataLoader.Properties.Resource1.ExcepCustomLoaderDeserialize, ex.InnerException);
                }
            }
            return (T)entityToReturn;
        }
    }
}
