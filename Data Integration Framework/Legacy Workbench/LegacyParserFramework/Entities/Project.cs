using System;
using System.Collections.Generic;
using System.Text;

namespace Infosys.Lif.LegacyWorkbench.Entities
{
    public class Project
    {
        string projectPrefix;

        public string ProjectPrefix
        {
            get { return projectPrefix; }
            set { projectPrefix = value; }
        }


        GenericCollection<ContractModule> contractModules = new GenericCollection<ContractModule>();

        public GenericCollection<ContractModule> ContractModules
        {
            get { return contractModules; }
            set { contractModules = value; }
        }
        

        GenericCollection<ModelObjectModule> modelObjectModules = new GenericCollection<ModelObjectModule>();

        public GenericCollection<ModelObjectModule> ModelObjectModules
        {
            get { return modelObjectModules; }
            set { modelObjectModules = value; }
        }


        ProjectNamespaces modelObjectNamespaces = new ProjectNamespaces();

        public ProjectNamespaces ModelObjectNamespaces
        {
            get { return modelObjectNamespaces; }
            set { modelObjectNamespaces = value; }
        }
        
        
        ProjectNamespaces contractNamespaces = new ProjectNamespaces();

        public ProjectNamespaces ContractNamespaces
        {
            get { return contractNamespaces; }
            set { contractNamespaces = value; }
        }

        
        public class ProjectNamespaces
        {
            string xmlSchemaNamespace = string.Empty;

            public string XmlSchemaNamespace
            {
                get { return xmlSchemaNamespace; }
                set { xmlSchemaNamespace = value; }
            }


            string dataEntityNamespace = string.Empty;

            public string DataEntityNamespace
            {
                get { return dataEntityNamespace; }
                set { dataEntityNamespace = value; }
            }
            
            
            string serializerNamespace = string.Empty;

            public string SerializerNamespace
            {
                get { return serializerNamespace; }
                set { serializerNamespace = value; }
            }


            string dataEntityRootNamespace = string.Empty;

            public string DataEntityRootNamespace
            {
                get { return dataEntityRootNamespace; }
                set { dataEntityRootNamespace = value; }
            }


            string serializerRootNamespace = string.Empty;

            public string SerializerRootNamespace
            {
                get { return serializerRootNamespace; }
                set { serializerRootNamespace = value; }
            }


            string hostAccessRootNamespace;

            public string HostAccessRootNamespace
            {
                get { return hostAccessRootNamespace; }
                set { hostAccessRootNamespace = value; }
            }

            string hostAccessNamespace;

            public string HostAccessNamespace
            {
                get { return hostAccessNamespace; }
                set { hostAccessNamespace = value; }
            }

        }

    }

}
