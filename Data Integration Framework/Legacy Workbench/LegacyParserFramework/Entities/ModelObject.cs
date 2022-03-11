using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel;

namespace Infosys.Lif.LegacyWorkbench.Entities
{
    public class ModelObject
    {
        string name;
        [ReadOnly(true)]
        [DisplayName("Model Object Name")]
        [Description("A name given to the model object")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        int minCount;
        [DisplayName("Minimum Count")]
        [Description("Minimum cardinality of a model object in a contract")]
        public int MinCount
        {
            get { return minCount; }
            set { minCount = value; }
        }


        int maxCount;
        [DisplayName("Maximum Count")]
        [Description("Maximum cardinality of a model object in a contract")]
        public int MaxCount
        {
            get { return maxCount; }
            set { maxCount = value; }
        }

        GenericCollection<ModelObject> modelObjects = new GenericCollection<ModelObject>();

        [Browsable(false)]
        public GenericCollection<ModelObject> ModelObjects
        {
            get { return modelObjects; }
            set { modelObjects = value; }
        }

        Entities.Entity modelObjectEntity;



        // We have placed an XML Ignore, because the serialization of this object
        // will result in increasing the size of the file, when saved.
        // Also what we really reuqire is the ModelObjectEntity to point to the correct
        // Entity, but if we do not place the Ignore attribute, a new instance of the 
        // Entity will be created. 
        [Browsable(false)]
        //[System.Xml.Serialization.XmlIgnore]
        public Entities.Entity ModelObjectEntity
        {
            get { return modelObjectEntity; }
            set { modelObjectEntity = value; }
        }


        string hostName;
        [ReadOnly(true)]
        [DisplayName("Host Name")]
        [Description("Name of model object as mentioned at host")]
        public string HostName
        {
            get
            {
                if (hostName == null || hostName.Length == 0)
                {
                    return name;
                }
                else
                {
                    return hostName;
                }
            }
            set { hostName = value; }
        }

    }
}
