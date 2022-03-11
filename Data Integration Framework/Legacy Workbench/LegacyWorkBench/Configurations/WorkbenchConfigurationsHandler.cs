using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml.Serialization;
using System.Xml;

using Infosys.Lif.LegacyWorkbench.LegacyWorkbenchConfigurations;


namespace Infosys.Lif.LegacyWorkbench.Configurations
{
    class WorkbenchConfigurationsHandler : ConfigurationSection
    {
        /// <summary>
        /// Method to call the deserialized 
        /// </summary>
        /// <returns></returns>
        protected override object GetRuntimeObject()
        {
            return configurations;
        }
        static Infosys.Lif.LegacyWorkbench.LegacyWorkbenchConfigurations.LegacyWorkbenchConfigurations configurations;

        public static Infosys.Lif.LegacyWorkbench.LegacyWorkbenchConfigurations.LegacyWorkbenchConfigurations Configurations
        {
            get { return configurations; }
            set { configurations = value; isModified = true; }
        }

        protected override void DeserializeElement(XmlReader reader,
            bool serializeCollectionKey)
        {
            XmlSerializer serializer
                = new XmlSerializer(typeof(Infosys.Lif.LegacyWorkbench.LegacyWorkbenchConfigurations.LegacyWorkbenchConfigurations));
            configurations = (Infosys.Lif.LegacyWorkbench.LegacyWorkbenchConfigurations.LegacyWorkbenchConfigurations)serializer.Deserialize(reader);

        }

        protected override bool SerializeElement(XmlWriter writer,
            bool serializeCollectionKey)
        {
            if (writer == null)
            {
                return true;
            }

            XmlSerializer serializer
                = new XmlSerializer(typeof(Infosys.Lif.LegacyWorkbench.LegacyWorkbenchConfigurations.LegacyWorkbenchConfigurations));
            serializer.Serialize(writer, configurations);

            return false;
        }

        static bool isModified = false;

        protected override bool IsModified()
        {
            return isModified;
        }
    }
}
