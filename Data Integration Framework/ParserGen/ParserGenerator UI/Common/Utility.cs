using System;
using System.Collections.Generic;
using System.Text;

//// The namespace for the Configuration management Application block
//using Microsoft.Practices.EnterpriseLibrary.Configuration;
///////// Above commented while moving to new EL Jan 2006
using System.Configuration;


// The configurations used for the parser generator
using Infosys.Lif.LegacyParser.UI.Configurations;
/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * This class is a .NET specific type which denotes a variable. 
 * All the input mainframe copy books will be converted to members of this type,
 * This is a part which is specific to the parser generator utility. 
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/


namespace Infosys.Lif.LegacyParser.UI
{
    /// <summary>
    /// All constants/utility functions of the Parser Generator tool will 
    /// be placed in this class.
    /// This also functions as the wrapper for the config App Block.
    /// </summary>
    static class ConfigWrapper
    {
        /// <summary>
        /// The config secion name for the configurations of the Parser Generator tool.
        /// </summary>
        static System.Configuration.Configuration configurations;

        // Static constructor.
        static ConfigWrapper()
        {
            configurations
                = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            ConfigHandler.ConfigChanged += new EventHandler(ConfigFileChanged);

            ////Add the event handlers for the Configuration Management block.
            //ConfigurationManager.ConfigurationChanging +=
            //    new ConfigurationChangingEventHandler(
            //    ConfigurationManager_ConfigurationChanging);

            //ConfigurationManager.ConfigurationChanged +=
            //    new ConfigurationChangedEventHandler(
            //    ConfigurationManager_ConfigurationChanged);
        }

        ///////////// <summary>
        ///////////// Handler for ConfigurationChanging event raised by the 
        ///////////// Configuration Management Application block.
        ///////////// </summary>
        ///////////// <param name="sender">The object which raises this event</param>
        ///////////// <param name="e">Event arguments passed</param>
        //////////static void ConfigurationManager_ConfigurationChanging(object sender,
        //////////    ConfigurationChangingEventArgs e)
        //////////{
        //////////    // If any handlers are attached to our custom wrapper, call it.
        //////////    if (ConfigChanging != null)
        //////////    {
        //////////        // If the section changing is our config section.
        //////////        if (e.SectionName.Equals(ConfigSectionName))
        //////////        {
        //////////            // Call the event handler
        //////////            ConfigChanging(sender, e);
        //////////        }
        //////////    }
        //////////}

        ///////////// <summary>
        ///////////// The event handler for ConfigurationChanged event raised by the Configuration 
        ///////////// App Block.
        ///////////// </summary>
        ///////////// <param name="sender">The object which raises this event.</param>
        ///////////// <param name="e">Event arguments passed ot this event handler</param>
        //////////static void ConfigurationManager_ConfigurationChanged(object sender,
        //////////    ConfigurationChangedEventArgs e)
        //////////{
        //////////    // If any event handlers are present for our custom wrapped event handler.
        //////////    if (ConfigChanged != null)
        //////////    {
        //////////        // If the secion changed is our section.
        //////////        if (e.SectionName.Equals(ConfigSectionName))
        //////////        {
        //////////            // Call the event handlers registered to receive our custom wrapped event.
        //////////            ConfigChanged(sender, e);
        //////////        }
        //////////    }
        //////////}

        static string ConfigSectionName = "LegacyParserConfigurationSection";
        static ConfigHandler configHandler;
        /// <summary>
        /// Method to retrieve the configurations from the config file's section named by the ConfigSectionName variable.
        /// </summary>
        /// <returns>Returns the configurations as a .NET type</returns>
        internal static LegacyParserConfigurations GetConfiguration()
        {
            try
            {
                configurations = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configHandler = (ConfigHandler)configurations.GetSection(ConfigSectionName);

                return configHandler.Configurations;
            }
            catch (System.Configuration.ConfigurationException cfgExc)
            {
                return null;
            }
        }


        internal static void ConfigFileChanged(object sender, EventArgs e)
        {
            if (ConfigChanged != null)
            {
                ConfigChanged(sender, e);
            }
        }

        /// <summary>
        /// This method is used to store the configurations in the configuration file. This utilizes the Config AppBlock.
        /// </summary>
        /// <param name="configValue">
        /// The configurations which have to be written to the config file. 
        /// This will replace all the values already stored in the config file.
        /// </param>
        internal static void WriteConfiguration(LegacyParserConfigurations configValue)
        {
            configurations.Sections.Remove(ConfigSectionName);
            configurations.Sections.Add(ConfigSectionName, configHandler);

            configurations.Save(ConfigurationSaveMode.Full);
        }
        /// <summary>
        /// The custom event raised when our defined Config section is 
        /// changed in the config file.
        /// </summary>
        internal static event EventHandler ConfigChanged;


        /////////// <summary>
        /////////// The custom event raised when our defined Config section is 
        /////////// changing in the config file.
        /////////// </summary>
        ////////internal static event ConfigurationChangingEventHandler ConfigChanging;
    }
}
