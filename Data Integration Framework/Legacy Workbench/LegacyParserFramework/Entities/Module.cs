using System;
using System.Collections.Generic;
using System.Text;

namespace Infosys.Lif.LegacyWorkbench.Entities
{

    public class Module
    {

        internal Module() { }

        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }


        string dataEntityNamespace;

        public string DataEntityNamespace
        {
            get { return dataEntityNamespace; }
            set { dataEntityNamespace = value; }
        }
        string serializerNamespace;

        public string SerializerNamespace
        {
            get { return serializerNamespace; }
            set { serializerNamespace = value; }
        }
    }
}
