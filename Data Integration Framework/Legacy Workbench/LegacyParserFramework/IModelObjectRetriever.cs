using System;
using System.Collections.Generic;
using System.Text;

namespace Infosys.Lif.LegacyWorkbench.Framework
{
    public interface IModelObjectRetriever
    {
        Entities.Entity RetrieveModelObjectDetails(string filePath);
    }
}
