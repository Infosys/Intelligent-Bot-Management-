using System;
using System.Collections.Generic;
using System.Text;

// Infosys Code generator v1.1
using Infosys.Solutions.CodeGeneration.Framework;



namespace Infosys.Lif.LegacyParser.ContentProviders
{
    public class ServiceDeliveryCodeContentProvider : ContentProvider
    {
        Entity entityToBeUsed = null;
        Infosys.Lif.LegacyParser.LegacyParserModule moduleToBeUsed = null;
        public ServiceDeliveryCodeContentProvider(Infosys.Lif.LegacyParser.LegacyParserModule module,
            Entity entityToBeBuilt)
        {
            moduleToBeUsed = module;
            entityToBeUsed = entityToBeBuilt;
        }

        [PlaceHolder("NameSpaceOfDataEntities")]
        public string NameSpaceOfDataEntities
        {
            get
            {
                string generatedContent = string.Empty;
                if (entityToBeUsed == null)
                {
                    for (int i = 0; i < moduleToBeUsed.Entities.Count; i++)
                    {
                        NameSpaceCodeProvider contentGenerator = new NameSpaceCodeProvider();
                        contentGenerator.ContentTemplate = ContentTemplate.RepeatingTemplate("NameSpaceOfEachDataEntity");
                        contentGenerator.DataEntityNameSpace = moduleToBeUsed.DataEntityNamespace;
                        contentGenerator.DataEntity= moduleToBeUsed.Entities[i].DataEntityClassName;
                        generatedContent += contentGenerator.GenerateContent();
                    }

                }
                else
                {
                    NameSpaceCodeProvider contentGenerator = new NameSpaceCodeProvider();
                    contentGenerator.ContentTemplate = ContentTemplate.RepeatingTemplate("NameSpaceOfEachDataEntity");
                    contentGenerator.DataEntityNameSpace = moduleToBeUsed.DataEntityNamespace;
                    contentGenerator.DataEntity = entityToBeUsed.DataEntityClassName;
                    generatedContent = contentGenerator.GenerateContent();
                }
                return generatedContent;
            }

        }
        [PlaceHolder("GeneratedCodeForAllComponents")]
        public string GeneratedCodeForAllComponents
        {
            get
            {
                string generatedContent = string.Empty;
                if (entityToBeUsed == null)
                {
                    for (int i = 0; i < moduleToBeUsed.Entities.Count; i++)
                    {
                        MethodsForComponentCodeProvider contentGenerator = new MethodsForComponentCodeProvider();
                        contentGenerator.ContentTemplate = ContentTemplate.RepeatingTemplate("MethodsForEachComponent");
                        contentGenerator._dataEntityNameSpace = moduleToBeUsed.DataEntityNamespace;
                        contentGenerator.entityToBeUsed = moduleToBeUsed.Entities[i];
                        generatedContent += contentGenerator.GenerateContent();
                    }
                }
                else
                {
                    MethodsForComponentCodeProvider contentGenerator = new MethodsForComponentCodeProvider();
                    contentGenerator.ContentTemplate = ContentTemplate.RepeatingTemplate("MethodsForEachComponent");
                    contentGenerator._dataEntityNameSpace = moduleToBeUsed.DataEntityNamespace;
                    contentGenerator.entityToBeUsed = entityToBeUsed;
                    generatedContent = contentGenerator.GenerateContent();
                }
                return generatedContent;
            }
        }
    }
    internal class MethodsForComponentCodeProvider : ContentProvider
    {
        internal Entity entityToBeUsed = null;
        internal string _dataEntityNameSpace = string.Empty;
        [PlaceHolder("DataEntityNameSpace")]
        internal string DataEntityNameSpace
        {
            get
            {
                return _dataEntityNameSpace;
            }
            set
            {
                _dataEntityNameSpace = value;
            }
        }
        [PlaceHolder("DataEntity")]
        public string DataEntity
        {
            get
            {
                return entityToBeUsed.DataEntityClassName;
            }
        }
        [PlaceHolder("ServiceName")]
        public string ServiceName
        {
            get
            {
                return entityToBeUsed.SerializerClassName;
            }
        }

    }
    internal class NameSpaceCodeProvider : ContentProvider
    {
        string _dataEntityNameSpace = string.Empty;
        [PlaceHolder("DataEntityNameSpace")]
        internal string DataEntityNameSpace
        {
            set
            {
                _dataEntityNameSpace = value;
            }
            get
            {
                return _dataEntityNameSpace;
            }
        }
        string _dataEntity = string.Empty;
        [PlaceHolder("DataEntity")]
        internal string DataEntity
        {
            set
            {
                _dataEntity = value;
            }
            get
            {
                return _dataEntity;
            }
        }

    }
}
