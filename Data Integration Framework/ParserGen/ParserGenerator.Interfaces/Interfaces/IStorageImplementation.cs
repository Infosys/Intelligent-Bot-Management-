using System;
using System.Collections.Generic;
using System.Text;


/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * This interface defines the interaction of the storage implementation 
 * with the Legacy parser tool. 
 * The implementors of this interface will be used to persist or 
 * retrieve the Project entity.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/
namespace Infosys.Lif.LegacyParser.Interfaces
{
    /// <summary>
    /// This interface defines how the implementors will be interacting 
    /// with the Legacy Parser tool. 
    /// The Legacy Parser utility will call the Store method to store 
    /// the project entity and the Retrieve method to retrieve the Project 
    /// entity.
    /// </summary>
    public interface IStorageImplementation
    {
        /// <summary>
        /// Will be used to store the data passed as a parameter
        /// </summary>
        /// <param name="projToBeStored">Project type which has to be stored.</param>
        void Store(Project projectToBeStored);

        /// <summary>
        /// Will be called to retrieve the data
        /// </summary>
        /// <returns>A project .NET type object.</returns>
        Project Retrieve();
    }
}
