using System;
using System.Collections.Generic;
using System.Text;

using Infosys.Solutions.CodeGeneration.Framework;
using Infosys.Lif.LegacyParser.ContentProviders;

/****************************************************************
 * This file is a part of the Legacy Parser tool. 
 * This class is a CICS specific implementation of the IHostSpecificController.
 * This is the control logic required to drive the generation of the serializers for 
 * a CCIS implementation.
 * Copyright (c) 2003 - 2005 Infosys Technologies Ltd. All Rights Reserved.
 ***************************************************************/


namespace Infosys.Lif.LegacyParser
{
    /// <summary>
    /// The Cics Specific implementation of the Host Specific Controller.
    /// This will be utilized to 
    /// </summary>
    public class CcisController : Interfaces.IHostSpecificController
    {
        string appPath;
        public CcisController()
        {
            appPath = System.Windows.Forms.Application.StartupPath;
        }


        #region IHostSpecificController Members
        /// <summary>
        /// A part of the implementation for the IHostSpecificController interface.
        /// This builds up the serializer class as decided by the template located at 
        /// Templates\SerializerClass.cs in the application's startup path
        /// This class being specific to the Cics utilizes the Object List to help 
        /// in parsing.
        /// </summary>
        /// <param name="entity">The .NET entity which will be utilized to build up the serializers.</param>
        /// <param name="moduleOfEntity">The .NET Module type which will be utilized to assist 
        /// in the building of the serializers.</param>
        /// <returns>The content of the serializer which has been generated. 
        /// This can be inserted into a file and utilized as necessary.</returns>
        public string BuildSerializer(Entity entity, LegacyParserModule moduleOfEntity)
        {
            //An object which parses the Object.LST file. 
            //This will enable the length, location, object id to be found.
            ObjectListParser objectListParser = new ObjectListParser();

            objectListParser.Parse(
                appPath + @"\HelperFiles\OBJECT.LST", moduleOfEntity.Entities);

            //The content provider for the serializer. 
            SerializerCodeContentProvider coder = new SerializerCodeContentProvider(entity);

            //The template to be utilized for generating the serializer.
            coder.ContentTemplate = Template.FromFile(
                appPath + @"\Templates", "SerializerClass.cs");

            //Fill up the data entity namespace in the serializer content provider.
            coder.DataEntityNamespace = moduleOfEntity.DataEntityNamespace;

            //Fill up the serializer namespace in the serializer content provider.
            coder.Namespace = moduleOfEntity.SerializerNamespace;

            //generate the serializer and return the string content.
            return coder.GenerateContent();
        }

        /// <summary>
        /// A part of the implementation for the IHostSpecificController interface.
        /// This builds the Data Entity XSD file, utilizing the template file found at 
        /// Templates\DataEntityTemplate.xsd in application's startup path
        /// </summary>
        /// <param name="entity">The .NET data entity for which the Data Entity has to be built.</param>
        /// <param name="moduleOfEntity">The module to which the entity belongs. This is required to 
        /// determine various parameters such as namespace.</param>
        /// <returns>The content for the XSD which will enable the generation of the entity. 
        /// This can be inserted into a file to prepare an XSD.</returns>
        public string BuildDataEntity(Entity entity, LegacyParserModule moduleOfEntity)
        {
            XsdContentProvider xsdGenerator
                = new XsdContentProvider(entity);
            xsdGenerator.ContentTemplate = Template.FromFile(
                appPath + @"\Templates", "DataEntityTemplate.xsd");
            return xsdGenerator.GenerateContent();
        }

        public string BuildServiceComponents(Entity entity, LegacyParserModule moduleOfEntity)
        {
            ServiceDeliveryCodeContentProvider contProvider
                = new ServiceDeliveryCodeContentProvider(moduleOfEntity, entity);
            contProvider.ContentTemplate = Template.FromFile(appPath + @"\Templates\ServiceComponents", "Service.cs");
            string strTemp = contProvider.GenerateContent();
            return strTemp;
        }

        public string BuildUnitTestCases(Entity entity, LegacyParserModule moduleOfTheEntity)
        {
            ServiceDeliveryCodeContentProvider contProvider
                = new ServiceDeliveryCodeContentProvider(moduleOfTheEntity, entity);
            contProvider.ContentTemplate = Template.FromFile(appPath + @"\Templates\UnitTestCases", "UnitTestCase.cs");
            string strTemp = contProvider.GenerateContent();
            return strTemp;
        }

        #endregion
    }
}
