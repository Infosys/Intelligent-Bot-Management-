using System;
using System.Collections.Generic;
using System.Text;
/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * This file defines the manner in which the config file should be 
 * deserialized/serialized.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/
namespace Infosys.Lif.LegacyParser.UI.Configurations
{
    #region LegacyParserConfigurations definition
    /// <summary>
    /// The type to which the config file will be serialized/deserialized.
    /// </summary>
    public class LegacyParserConfigurations
    {
        /// <summary>
        /// The configurations of the XSD Object generator, or any such tool 
        /// is accessible through this variable.
        /// </summary>
        XsdObjectGeneratorConfig _xsdObjectConfig
            = new XsdObjectGeneratorConfig();

        /// <summary>
        /// This decides the LanguageConverter configuration to be used.
        /// This variable should be set to the name of the LanguageConverter
        /// that should be used.
        /// </summary>
        string _language;

        /// <summary>
        /// The name of the HostTypeController which has to be 
        /// utilized for the parse generation should be specified to this variable. 
        /// </summary>
        string _defaultHostType;

        /// <summary>
        /// The name of the StorageImplementation to be used should 
        /// be mentioned to this variable.
        /// </summary>
        string _defaultStorageType;

        /// <summary>
        /// The language converters as decided by the configuration file will 
        /// be placed in this variable
        /// </summary>
        LanguageConverterCollection _languageConverters
            = new LanguageConverterCollection();

        /// <summary>
        /// The host type controllers as decided by the configuration file 
        /// will be placed in this variable
        /// </summary>
        HostTypeControllerCollection _hostTypes
            = new HostTypeControllerCollection();
        /// <summary>
        /// The various possible storage implementations referenced in the config file 
        /// will be store here.
        /// </summary>
        StorageImplementationCollection _storageImplementations
            = new StorageImplementationCollection();

        /// <summary>
        /// This property decides the configurations of the XSDObject Generator
        /// </summary>
        public XsdObjectGeneratorConfig XsdObjectConfig
        {
            get { return _xsdObjectConfig; }
            set { _xsdObjectConfig = value; }
        }
        /// <summary>
        /// The default LanguageCOnverter should be referred to using this property
        /// </summary>
        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }

        /// <summary>
        /// The default host type controller's name should be entered to this property.
        /// </summary>
        public string HostType
        {
            get { return _defaultHostType; }
            set { _defaultHostType = value; }
        }
        /// <summary>
        /// This is used to set/retrieve the name of the default storage type implementation
        /// </summary>
        public string StorageType
        {
            get { return _defaultStorageType; }
            set { _defaultStorageType = value; }
        }

        /// <summary>
        /// The various Language converters are referenced here.
        /// </summary>
        public LanguageConverterCollection LanguageConverters
        {
            get { return _languageConverters; }
        }
        /// <summary>
        /// The various Host Type Controllers are referenced here.
        /// </summary>
        public HostTypeControllerCollection HostTypes
        {
            get { return _hostTypes; }
        }
        /// <summary>
        /// The various storage implementations are set/retrieved using this property.
        /// </summary>
        public StorageImplementationCollection StorageImplementations
        {
            get { return _storageImplementations; }
        }

    } 
    #endregion
}
