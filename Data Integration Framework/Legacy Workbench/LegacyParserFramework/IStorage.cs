using System;
using System.Collections.Generic;
using System.Text;

namespace Infosys.Lif.LegacyWorkbench.Framework
{
    public interface IStorage
    {
        /// <summary>
        /// Method to be called to Save a project
        /// </summary>
        /// <param name="projectToBeSaved"></param>
        /// <returns></returns>
        bool Save(Entities.Project projectToBeSaved);

        /// <summary>
        /// Method to be called to load up a Project
        /// </summary>
        /// <returns></returns>
        Entities.Project Load();
    }
}
