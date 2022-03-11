using System;
using System.Collections.Generic;
using System.Text;

namespace Infosys.Lif.LegacyWorkbench.Entities
{
    public class ClauseModule : Module
    {

        GenericCollection<Entities.Entity> clauses 
            = new GenericCollection<Entities.Entity>();

        public GenericCollection<Entities.Entity> Clauses
        {
            get { return clauses; }
            set { clauses = value; }
        }
    }
}
