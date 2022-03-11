using System;
using System.Collections.Generic;
using System.Text;
/****************************************************************
 * This file is a part of the Legacy Parser utility.
 * This interface has to be implemented by all classes which need 
 * control logic for controlling the manner in which Serializer files 
 * and data entities are generated.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/
namespace Infosys.Lif.LegacyParser.Interfaces
{
    /// <summary>
    /// This interface controls the manner in which the control logic appears 
    /// publicly.
    /// </summary>
    public interface IHostSpecificController
    {
        /// <summary>
        /// This method accepts the various .NET entities and utilizes the 
        /// right templates to generate code string for a Serializer file.
        /// The output of this can be placed in a file to store the code.
        /// </summary>
        /// <param name="entity">.NET entity for which the Serializer should 
        /// be generated.</param>
        /// <param name="moduleOfEntity">The .NET module to which 
        /// the input entity belongs.</param>
        /// <returns>The code of the Serializer class</returns>
        string BuildSerializer(Entity entity, LegacyParserModule moduleOfEntity);
        /// <summary>
        /// This method accepts the .NET entity and module for which the 
        /// data entity needs to be generated.
        /// </summary>
        /// <param name="entity">The .NET entity for which the Data Entity 
        /// needs to be generated.</param>
        /// <param name="moduleOfEntity">The module to which the input 
        /// entity belongs.</param>
        /// <returns>This method returns the xsd string for the data entity.</returns>
        string BuildDataEntity(Entity entity, LegacyParserModule moduleOfEntity);

        string BuildServiceComponents(Entity entity, LegacyParserModule moduleOfEntity);
        string BuildUnitTestCases(Entity entity, LegacyParserModule moduleOfTheEntity);
    }
}
