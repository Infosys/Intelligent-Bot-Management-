using System;
using System.Collections.Generic;
using System.Text;

using Infosys.Lif.LegacyWorkbench.Entities;

namespace Infosys.Lif.LegacyWorkbench.Framework
{
    public interface IContractImporter
    {
        // GenericCollection<Contract> RetrieveContractDetails();
        event ContractsRetrievedEventHandler ContractsRetrieved;
        
        Contract RetrieveContract(string fileName);
    }
    public delegate void ContractsRetrievedEventHandler(object sender, ContractsRetrievedEventArgs e);

    public class ContractsRetrievedEventArgs : EventArgs
    {
        GenericCollection<Contract> retrievedContracts = new GenericCollection<Contract>();

        public GenericCollection<Contract> RetrievedContracts
        {
            get { return retrievedContracts; }
            set { retrievedContracts = value; }
        }
    }
}
