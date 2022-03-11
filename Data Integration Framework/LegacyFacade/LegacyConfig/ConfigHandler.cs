using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Configuration;

namespace Infosys.Lif.LegacyConfig
{
    internal class ConfigHandler : ConfigurationSection
    {

        /// <summary>
        /// Method to call the deserialized 
        /// </summary>
        /// <returns></returns>
        protected override object GetRuntimeObject()
        {
            return configurations;
        }
        static LegacySettings configurations;

        public LegacySettings Configurations
        {
            get { return configurations; }
        }

        protected override void DeserializeElement(XmlReader reader,
            bool serializeCollectionKey)
        {
            XmlSerializer serializer
                = new XmlSerializer(typeof(LegacySettings));
            configurations = (LegacySettings)serializer.Deserialize(reader);
        }

        protected override bool IsModified()
        {
            return true;
        }

        protected override bool SerializeElement(System.Xml.XmlWriter writer,
            bool serializeCollectionKey)
        {

            if (writer == null)
            {
                return true;
            }
            XmlSerializer serializer
                = new XmlSerializer(typeof(LegacySettings));
            serializer.Serialize(writer, configurations);
            return true;
        }

    }
}

