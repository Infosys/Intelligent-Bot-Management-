using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * These classes enable us to define modules in the Legacy Parser outputs. 
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/

namespace Infosys.Lif.LegacyParser
{
    #region LegacyParserModule Definition
    /// <summary>
    /// This class type decides the variousl parameters possible for a module.
    /// </summary>
    public class LegacyParserModule
    {
        /// <summary>
        /// An unique name for this module.
        /// </summary>
        string name = string.Empty;

        /// <summary>
        /// All entities belonging to this module should be added to this collection.
        /// </summary>
        GenericCollection<Entity> _entities = new GenericCollection<Entity>();

        /// <summary>
        /// The namespace for the data entities built as a part of this module.
        /// </summary>
        string dataEntityNamespace = "Infosys.Lif.Generated.Data.Entity";

        /// <summary>
        /// The namespace for the serializer built as a part of this module.
        /// </summary>
        string serializerNamespace = "Infosys.Lif.Generated.Serializers";

        string serviceComponentsNamespace = "Infosys.Lif.Generated.ServiceComponents";


        string unitTestCasesNamespace = "Infosys.Lif.Generated.UnitTestCases";



        /// <summary>
        /// An unique name for this module. 
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        /// <summary>
        /// All entities belonging to this module should be added to this collection.
        /// </summary>
        public GenericCollection<Entity> Entities
        {
            get { return _entities; }
        }

        /// <summary>
        /// The namespace which will be given to all the data entities coming as 
        /// a part of this module.
        /// </summary>
        public string DataEntityNamespace
        {
            get { return dataEntityNamespace; }
            set { dataEntityNamespace = value; }
        }

        /// <summary>
        /// The namespace which will be given to all the Serializer coming as 
        /// a part of this module.
        /// </summary>
        public string SerializerNamespace
        {
            get { return serializerNamespace; }
            set { serializerNamespace = value; }
        }

        /// <summary>
        /// The name space which will be given to all the service Components 
        /// which will be generated
        /// </summary>
        public string ServiceComponentsNamespace
        {
            get { return serviceComponentsNamespace; }
            set { serviceComponentsNamespace = value; }
        }
        public string UnitTestCasesNamespace
        {
            get { return unitTestCasesNamespace; }
            set { unitTestCasesNamespace = value; }
        }
    }

    #endregion

}
