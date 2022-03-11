using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using System.Xml;

namespace Infosys.Lif.LegacyWorkbench
{
    public partial class ConfigurationWindow : Form
    {

        public const string ConfigSectionName = "LegacyWorkbenchConfigurations";

        public LegacyWorkbenchConfigurations.LegacyWorkbenchConfigurations modifiedSettings = null;

        public readonly LegacyWorkbenchConfigurations.LegacyWorkbenchConfigurations configurations = (LegacyWorkbenchConfigurations.LegacyWorkbenchConfigurations)System.Configuration.ConfigurationManager.GetSection(ConfigSectionName);

        //public System.Configuration.ConfigurationSection scs = null;


        public ConfigurationWindow()
        {
            InitializeComponent();
        }

        private void ConfigurationWindow_Load(object sender, EventArgs e)
        {


            LegacyWorkbenchConfigurations.LegacyWorkbenchConfigurations configurations
                = (LegacyWorkbenchConfigurations.LegacyWorkbenchConfigurations)System.Configuration.ConfigurationManager.GetSection(ConfigSectionName);
            modifiedSettings = configurations;

            configurationPropertyGrid.SelectedObject = configurations;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            /*
            System.Configuration.Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration
                (System.Configuration.ConfigurationUserLevel.None);


            //foreach (ConfigurationSection section in config.Sections)
            //{
            //    if (section.SectionInformation.Name == ConfigSectionName)
            //    {
            //        config.Sections.Remove(ConfigSectionName);
            //        ConfigurationSection reflectedSection = Activator.CreateInstance(section.GetType()) as ConfigurationSection;
            //        config.Save();
            //    }
            //}
            //XmlDocument xmlDoc = new XmlDocument();
            //XmlNode appSettingsNode = xmlDoc.SelectSingleNode(ConfigSectionName);
            //try
            //{
            //    if (KeyExists(strKey))
            //        throw new ArgumentException("Key name: <" + strKey +
            //                  "> already exists in the configuration.");
            //    XmlNode newChild = appSettingsNode.FirstChild.Clone();
            //    newChild.Attributes["key"].Value = strKey;
            //    newChild.Attributes["value"].Value = strValue;
            //    appSettingsNode.AppendChild(newChild);
            //    //We have to save the configuration in two places, 
            //    //because while we have a root App.config,
            //    //we also have an ApplicationName.exe.config.
            //    xmlDoc.Save(AppDomain.CurrentDomain.BaseDirectory +
            //                                 "..\\..\\App.config");
            //    xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
            #region tried code

            //foreach (ConfigurationSection section in config.Sections)
            //{
            //    if (section.ElementInformation.Source != null && section.ElementInformation.Source.Equals(config.FilePath))
            //    {
            //        //Enumerator is on AppSettings. So we update the appSettings 
            //        if (section is AppSettingsSection)
            //        {
            //            foreach (KeyValueConfigurationElement element in config.AppSettings.Settings)
            //            {
            //                config.AppSettings.Settings.Remove(element.Key);
            //                config.Save(ConfigurationSaveMode.Full, false);
            //                config.AppSettings.Settings.Add(element);
            //            }
            //        }
            //        //Enumerator is on a custom section 
            //        else
            //        {
            //            //Remove from target and add from source.  
            //            if (section.SectionInformation.SectionName == ConfigSectionName)
            //            {
            //                config.Sections.Remove(ConfigSectionName);
            //                //Just paranoid. 
            //                config.Save(ConfigurationSaveMode.Full, false);
            //                //Using reflection to instantiate since no public ctor and the instance we hold is tied to "Source" 
            //                ConfigurationSection reflectedSection = Activator.CreateInstance(section.GetType()) as ConfigurationSection;
            //                reflectedSection.SectionInformation.SetRawXml(section.SectionInformation.GetRawXml());
            //                Sections.Add(section.SectionInformation.Name, Section);
            //                config.Sections.Add(section.SectionInformation.Name, reflectedSection);
            //            }
            //        }
            //        ConfigurationManager.RefreshSection(section.SectionInformation.SectionName);
            //    }

            //}




























            //string fileName = config.FilePath;
            ////try
            ////{
            //    //Console.WriteLine("trying to open file {0}...", fileName);
            //    StreamReader streamReader = File.OpenText(fileName);

            //    string strReader;
            //    //int count = 0;
            //    while ((strReader = streamReader.ReadToEnd()) != null)
            //    {
            //        //count++;

            //        strReader.r.Replace("?xml", "srajan");

            //        //if (str == "My Name is SRAJAN")
            //        //{
            //        //    din.Close();
            //        //    StreamWriter sw = new StreamWriter(fileName);
            //        //    sw.WriteLine("    <XsdObjectGeneratorPath>C:\\XsdObjGen\\XSDObjectGen19.exe</XsdObjectGeneratorPath>");
            //        //    sw.Close();
            //        //    break;
            //        //}
            //        //Console.WriteLine("line {0}: {1}", count, str);
            //    }
            //    //din.Close();
            ////}
            ////catch (Exception ex)
            ////{
            ////    //if (dynamic_cast<FileNotFoundException^>(e))
            ////    //   Console.WriteLine("file '{0}' not found", fileName);
            ////    //else
            ////    //   Console.WriteLine("problem reading file '{0}'", fileName);
            ////}

            ////Console.ReadLine();


































            //foreach (ConfigurationSection section in config.Sections)
            //{
            //    if (section.ElementInformation.Source != null && section.ElementInformation.Source.Equals(config.FilePath))
            //    {
            //        //Enumerator is on AppSettings. So we update the appSettings 
            //        if (section is AppSettingsSection)
            //        {
            //            foreach (KeyValueConfigurationElement element in config.AppSettings.Settings)
            //            {
            //                config.AppSettings.Settings.Remove(element.Key);
            //                config.Save(ConfigurationSaveMode.Full, false);
            //                config.AppSettings.Settings.Add(element);
            //            }
            //        }
            //        //Enumerator is on a custom section 
            //        else
            //        {
            //            //Remove from target and add from source.  
            //            if (section.SectionInformation.SectionName == ConfigSectionName)
            //            {
            //config.Sections.Remove(ConfigSectionName);
            ////Just paranoid. 
            //config.Save(ConfigurationSaveMode.Full, false);
            ////Using reflection to instantiate since no public ctor and the instance we hold is tied to "Source" 
            //ConfigurationSection reflectedSection = Activator.CreateInstance(section.GetType()) as ConfigurationSection;
            //reflectedSection.SectionInformation.SetRawXml(section.SectionInformation.GetRawXml());
            //Bug/Feature in framework prevents target.Sections.Add(section.SectionInformation.Name, Section); 
            //config.Sections.Add(section.SectionInformation.Name, reflectedSection);
            //            }
            //        }
            //        ConfigurationManager.RefreshSection(section.SectionInformation.SectionName);
            //    } 

            //}
























            //config.Sections.Remove(ConfigSectionName);
            ////System.Configuration.SectionInformation si = new System.Configuration.SectionInformation();

            //conficsec conficsecobj = new conficsec();
            //conficsecobj.SectionInformation.SetRawXml(@"E:\xmltouse\xmltouse.xml");
            //config.Sections.Add(ConfigSectionName, conficsecobj);
            ////config.Sections.Add(ConfigSectionName, conficsec);
            ////string str = @"<LegacyWorkbenchConfigurations xmlns:xsi=" + "\"" + "http://www.w3.org/2001/XMLSchema-instance" + "\"" + " xmlns:xsd=" + "\"" + "http://www.w3.org/2001/XMLSchema" + "\"" + @"><XsdObjectGeneratorPath>C:\XsdObjGen\XSDObjectGen.exe</XsdObjectGeneratorPath></LegacyWorkbenchConfigurations >";
            //config.Save();
            //System.Configuration.ConfigurationSection csc;
            //csc.SectionInformation.ConfigSource = str;
            //System.Configuration.ConfigurationSection csc =;
            //csc.SectionInformation.ConfigSource = modifiedSettings;
            ////config.Sections.Add(ConfigSectionName,
            //config.Sections.Remove(ConfigSectionName);

            //scs = (System.Configuration.ConfigurationSection)modifiedSettings;

            //System.Configuration.ConfigurationSaveMode csm = new System.Configuration.ConfigurationSaveMode();

            //config.SaveAs(@"D:\MTCProject\LIF\Code\Framework\Legacy Workbench\LegacyWorkBench\bin\Debug\LegacyWorkbench2.vshost.exe.config", csm, true);


            //System.Configuration.ConfigurationSection scs = config.Sections;


            ////config.Sections.Add(ConfigSectionName,System.Configuration.ConfigurationSection
            //config.Sections.Add

            //config.Sections.Add(ConfigSectionName, modifiedSettings);
            //        Infosys.Lif.LegacyWorkbench.Configurations.WorkbenchConfigurationsHandler configHandler
            //= new Infosys.Lif.LegacyWorkbench.Configurations.WorkbenchConfigurationsHandler();



            //        Infosys.Lif.LegacyWorkbench.Configurations.WorkbenchConfigurationsHandler.Configurations = (LegacyWorkbenchConfigurations.LegacyWorkbenchConfigurations)configurationPropertyGrid.SelectedObject;
            //       // configHandler.Configurations = (LegacyWorkbenchConfigurations.LegacyWorkbenchConfigurations)configurationPropertyGrid.SelectedObject;

            //        config.Sections.Remove(ConfigurationWindow.ConfigSectionName);
            //        config.Sections.Add(ConfigurationWindow.ConfigSectionName,
            //            Infosys.Lif.LegacyWorkbench.Configurations.WorkbenchConfigurationsHandler.Configurations);
            //        config.Save();

#endregion
        }


    }





    /*********************************new classes*************************************/
        }
        public sealed class conficsec : ConfigurationSection
        {

        }
        /*********************************new classes ends********************************/
    }
}
