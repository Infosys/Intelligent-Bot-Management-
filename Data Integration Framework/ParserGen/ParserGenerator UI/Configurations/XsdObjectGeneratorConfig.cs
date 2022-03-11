using System;
using System.Collections;
using System.Xml.Serialization;
/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * This defines the type for storing the XSD Obj generator configurations
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/

namespace Infosys.Lif.LegacyParser.UI.Configurations
{
    /// <summary>
    /// The type to hold configurations of the XSD Object generator tool.
    /// </summary>
    public class XsdObjectGeneratorConfig
    {
        /// <summary>
        /// The path to the executable used to generate code files from the 
        /// generated XSD files
        /// </summary>
        string _xsdObjectGeneratorPath;

        /// <summary>
        /// The command line input to the XSD Object generator
        /// </summary>
        string _xsdObjectGeneratorArgs;

        /// <summary>
        /// The path of the XSD Object gnerator tool.
        /// </summary>
        public string XsdObjectGeneratorPath
        {
            get { return _xsdObjectGeneratorPath; }
            set { _xsdObjectGeneratorPath = value; }
        }

        /// <summary>
        /// The arguments to be passed to the XSD Object generator.
        /// </summary>
        public string XsdObjectGeneratorArgs
        {
            get { return _xsdObjectGeneratorArgs; }
            set { _xsdObjectGeneratorArgs = value; }
        }
    }
}
