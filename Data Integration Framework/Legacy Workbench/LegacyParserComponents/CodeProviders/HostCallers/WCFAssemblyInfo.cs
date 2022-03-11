using System;
using System.Collections.Generic;
using System.Text;
using Infosys.Solutions.CodeGeneration.Framework;

namespace Infosys.Lif.LegacyWorkbench.CodeProviders.HostCallers
{
    internal class WCFAssemblyInfo : ContentProvider
    {
        Guid guidAssemblyInfo;

        [PlaceHolder("GuidAssemblyInfo")]
        string GuidAssemblyInfo
        {
            get { return guidAssemblyInfo.ToString(); }
        }

        string project;

        [PlaceHolder("Project")]
        string Project
        {
            get { return project; }
        }


        internal WCFAssemblyInfo(string Pro, Guid guidAssem)
        {
            guidAssemblyInfo = guidAssem;
            project = Pro;
        }
    }
}
