using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Configuration;

namespace Infosys.Lif.LegacyParser.UI.Configurations
{
    internal class ConfigHandler : ConfigurationSection
    {
        /// <summary>
        /// The custom event raised when our defined Config section is 
        /// changed in the config file.
        /// </summary>
        internal static event EventHandler ConfigChanged;


        
        void watcher_Changed(object sender, System.IO.FileSystemEventArgs e)
        {

            if (ConfigChanged != null)
            {
                ConfigChanged(sender, e);
            }
        }


        /// <summary>
        /// Method to call the deserialized 
        /// </summary>
        /// <returns></returns>
        protected override object GetRuntimeObject()
        {
            return configurations;
        }
        static LegacyParserConfigurations configurations;

        public LegacyParserConfigurations Configurations
        {
            get { return configurations; }
        }

        private static bool watcherAdded = false;

        protected override void DeserializeElement(XmlReader reader,
            bool serializeCollectionKey)
        {
            if (!watcherAdded)
            {
                AddFileWatcher();
            }



            string str = this.ElementInformation.Source;

            reader.Read();// ("LegacyParserConfigurations");

            XmlSerializer serializer
                = new XmlSerializer(typeof(LegacyParserConfigurations));
            configurations = (LegacyParserConfigurations)serializer.Deserialize(reader);

        }

        private void AddFileWatcher()
        {
            string configFilePath = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            int indexOfLastSlash = configFilePath.LastIndexOf('\\');

            string configFileDirectory = configFilePath.Substring(0, indexOfLastSlash);
            string configFileName = configFilePath.Substring(indexOfLastSlash + 1, configFilePath.Length - indexOfLastSlash - 1);

            if (!(SectionInformation.ConfigSource == null || SectionInformation.ConfigSource.Length == 0))
            {
                string configSource = SectionInformation.ConfigSource;
                indexOfLastSlash = configSource.LastIndexOf('\\');
                configFileDirectory += "\\" + configSource.Substring(0, indexOfLastSlash);
                configFileName = configSource.Substring(indexOfLastSlash + 1, configSource.Length - indexOfLastSlash - 1);
            }

            System.IO.FileSystemWatcher watcher
                = new System.IO.FileSystemWatcher(configFileDirectory,
                configFileName);

            watcher.NotifyFilter = System.IO.NotifyFilters.LastWrite;
            watcher.EnableRaisingEvents = true;

            watcher.Changed
                += new System.IO.FileSystemEventHandler(watcher_Changed);

            watcherAdded = true;
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
                = new XmlSerializer(typeof(LegacyParserConfigurations));
            serializer.Serialize(writer, configurations);
            return true;
        }

    }
}

