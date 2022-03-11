/******************************************************************************
 * This file is a part of the Legacy Integration Framework.
 * This file retrieves configuration data from config file
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 *****************************************************************************/

using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using System.Configuration;
using System.Xml.Serialization;
using System.Xml;


namespace Infosys.Lif.DataLoader
{
    /// <summary>
    /// Class to pre-load configuration file
    /// </summary>
    public class ConfigHandler : ConfigurationSection
    {
        private ConfigHandler()
        {
        }

        /// <summary>
        /// Gets object from config file
        /// </summary>
        /// <returns>object</returns>
        protected override object GetRuntimeObject()
        {
            return configurations;
        }
        static DataLoaderSettings configurations;

        public DataLoaderSettings Configurations
        {
            get { return configurations; }
        }

        /// <summary>
        /// Reads the config file and deserializes it
        /// </summary>
        /// <param name="reader"> XmlReader object </param>
        /// <param name="serializeCollectionKey"> boolean key </param>
        protected override void DeserializeElement(XmlReader reader,
            bool serializeCollectionKey)
        {            
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(DataLoaderSettings));
                configurations = (DataLoaderSettings)serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {                
                throw new LoaderException(Infosys.Lif.DataLoader.Properties.Resource1.ExcepConfigHandler, ex.InnerException);
            }

            //if configurations object is null, throw an exception
            if (configurations.Equals(null))
            {                
                throw new LoaderException(Infosys.Lif.DataLoader.Properties.Resource1.ExcepConfigHandler);
            }
        }
    }
}
