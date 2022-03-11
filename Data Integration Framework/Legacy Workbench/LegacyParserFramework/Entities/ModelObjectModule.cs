using System;
using System.Collections.Generic;
using System.Text;

namespace Infosys.Lif.LegacyWorkbench.Entities
{
    public class ModelObjectModule : Module
    {

        GenericCollection<Entities.Entity> modelObjects 
            = new GenericCollection<Entities.Entity>();

        public GenericCollection<Entities.Entity> ModelObjects
        {
            get { return modelObjects; }
            set { modelObjects = value; }
        }
    }
}
