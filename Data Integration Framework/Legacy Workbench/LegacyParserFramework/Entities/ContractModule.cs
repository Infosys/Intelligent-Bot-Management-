using System;
using System.Collections.Generic;
using System.Text;

namespace Infosys.Lif.LegacyWorkbench.Entities
{
    public class ContractModule:Module
    {

        GenericCollection<Contract> contracts = new GenericCollection<Contract>();

        public GenericCollection<Contract> Contracts
        {
            get { return contracts; }
            set { contracts = value; }
        }

    }
}
