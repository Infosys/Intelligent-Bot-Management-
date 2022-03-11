using System;
using System.Collections.Generic;
using System.Text;

/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * The classes in this file will be utilized to 
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/

namespace Infosys.Lif.LegacyParser
{
    /// <summary>
    /// All the modules categorized in the Legacy Parser tool will be placed in projects of this type.
    /// </summary>
    public class Project
    {
        /// <summary>
        /// The modules belonging to this project 
        /// </summary>
        GenericCollection<LegacyParserModule> _allModules = new GenericCollection<LegacyParserModule>();
        
        /// <summary>
        /// This is the root name space and assembly name for the Data Entities assemblies.
        /// </summary>
        string _dataEntityRootNamespace = "Infosys.Lif.Generated.Data.Entity";

        /// <summary>
        /// This is the root name space and assembly name for the Serializer assembly.
        /// </summary>
        string _serializerRootNamespace = "Infosys.Lif.Generated.Serializers";

        string _serviceComponentRootNameSpace = "Infosys.Lif.Generated.Service.Components";

        string _unitTestCasesRootNameSpace = "Infosys.Lif.Generated.Unit.TestCases";


        /// <summary>
        /// All the modules belonging to this project should be added to this collection.
        /// </summary>
        public GenericCollection<LegacyParserModule> Modules
        {
            get { return _allModules; }
        }

        /// <summary>
        /// The root namespace and assembly name to be used for the data entitites classes.
        /// </summary>
        public string DataEntityRootNamespace
        {
            get { return _dataEntityRootNamespace; }
            set { _dataEntityRootNamespace = value; }
        }

        /// <summary>
        /// The root namespace and the assembly name to be used for the Serializer classes.
        /// </summary>
        public string SerializerRootNamespace
        {
            get { return _serializerRootNamespace; }
            set { _serializerRootNamespace = value; }
        }


        public string ServiceComponentRootNameSpace
        {
            get { return _serviceComponentRootNameSpace; }
            set { _serviceComponentRootNameSpace = value; }
        }

        public string UnitTestCasesRootNameSpace
        {
            get { return _unitTestCasesRootNameSpace; }
            set { _unitTestCasesRootNameSpace = value; }
        }

    }
}
