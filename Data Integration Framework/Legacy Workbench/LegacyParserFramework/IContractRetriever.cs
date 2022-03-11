using System;
using System.Collections.Generic;
using System.Text;

namespace Infosys.Lif.LegacyWorkbench.Framework
{
    public interface IContractRetriever
    {
        Entities.Contract RetrieveContractDetails(string filePath);
    }
}
