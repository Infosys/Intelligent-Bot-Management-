using System;
using System.Collections.Generic;
using System.Text;

using Infosys.Solutions.CodeGeneration.Framework;
namespace Infosys.Lif.LegacyWorkbench.CodeProviders.Serializers
{
    public class ModelObjectSerializerCP : ContentProvider
    {
        Entities.Entity objectForCodeGeneration;
        Entities.ModelObjectModule ModelObjectModuleForCodeGeneration;
        public ModelObjectSerializerCP(Entities.ModelObjectModule ModelObjectModule,
            Entities.Entity entityObject,
            Template templateObject)
        {
            ModelObjectModuleForCodeGeneration = ModelObjectModule;
            objectForCodeGeneration = entityObject;
            ContentTemplate = templateObject;
        }


        [PlaceHolder("Namespace")]
        string Namespace
        {
            get
            {
                return ((ModelObjectModuleForCodeGeneration.SerializerNamespace == null) ? string.Empty : ModelObjectModuleForCodeGeneration.SerializerNamespace);
            }
        }
        [PlaceHolder("ModelObjectName")]
        string ModelObjectName
        {
            get
            {
                return objectForCodeGeneration.EntityName;
            }
        }
        [PlaceHolder("DataEntityClass")]
        string DataEntityClass
        {
            get
            {
                return objectForCodeGeneration.EntityName;
            }
        }

        [PlaceHolder("DataEntityNameSpace")]
        string DataEntityNameSpace
        {
            get
            {
                return Framework.Helper.BuildNamespace(ModelObjectModuleForCodeGeneration.DataEntityNamespace, objectForCodeGeneration);

            }
        }

        [PlaceHolder("ProgramId")]
        string ProgramId
        {
            get
            {
                return objectForCodeGeneration.ProgramId;
            }
        }

        [PlaceHolder("AppendedItems")]
        string AppendedItems
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (Entities.DataItem dataItem in objectForCodeGeneration.DataItems)
                {
                    CodeProviderForDataItem codeProvider
                        = new CodeProviderForDataItem(dataItem);
                    codeProvider.ContentTemplate
                        = ContentTemplate.RepeatingTemplate(
                        CodeProviderForDataItem.TemplateName);
                    codeProvider.templateForDateContent
                        = ContentTemplate.RepeatingTemplate(
                        "DateAppender");
                    codeProvider.templateForStringContent
                        = ContentTemplate.RepeatingTemplate(
                        "StringAppender");
                    codeProvider.templateForIntegerContent
                        = ContentTemplate.RepeatingTemplate(
                        "IntegerAppender");
                    codeProvider.templateForBooleanContent
                        = ContentTemplate.RepeatingTemplate(
                        "BooleanAppender");
                    codeProvider.templateForFloatContent
                        = ContentTemplate.RepeatingTemplate(
                        "FloatAppender");


                    sb.Append(codeProvider.GenerateContent());
                }
                return sb.ToString();
            }
        }



        [PlaceHolder("DataItemsFilled")]
        string DataItemsFilled
        {
            get
            {
                Template template = ContentTemplate.RepeatingTemplate(DataItemsCP.TemplateName);

                Template templateForBoolean = ContentTemplate.RepeatingTemplate(DataItemsCP.TemplateName + "Boolean");

                StringBuilder sb = new StringBuilder();
                foreach (Entities.DataItem dataItem in objectForCodeGeneration.DataItems)
                {
                    DataItemsCP codeProvider = new DataItemsCP(dataItem);
                    if (dataItem.ItemType == Entities.DataItem.DataItemType.Boolean)
                    {
                        codeProvider.ContentTemplate = templateForBoolean;
                    }
                    else
                    {
                        codeProvider.ContentTemplate = template;
                    }
                    sb.Append(codeProvider.GenerateContent());
                }
                return sb.ToString();
            }
        }

        class DataItemsCP : ContentProvider
        {
            internal static string TemplateName = "DataItems";
            Entities.DataItem objectForCodeGeneration;
            internal DataItemsCP(Entities.DataItem dataItem)
            {
                objectForCodeGeneration = dataItem;
            }

            [PlaceHolder("DataItemName")]
            string DataItemName
            {
                get
                {
                    return objectForCodeGeneration.ItemName;
                }
            }
            [PlaceHolder("DataType")]
            string DataType
            {
                get
                {
                    switch (objectForCodeGeneration.ItemType)
                    {
                        case Entities.DataItem.DataItemType.Integer:
                        //case Entities.DataItem.DataItemType.SignedIntegerType:
                            return "Int32";
                            break;
                        case Entities.DataItem.DataItemType.Boolean:
                            return "Boolean";
                            break;
                        case Entities.DataItem.DataItemType.String:
                            return "String";
                            break;
                        case Entities.DataItem.DataItemType.Date:
                            return "DateTime";
                            break;
                        case Infosys.Lif.LegacyWorkbench.Entities.DataItem.DataItemType.Float:
                            return "Double";
                            break;
                    }
                    return string.Empty;
                }
            }
        }

        class CodeProviderForDataItem : ContentProvider
        {
            internal Template templateForStringContent;
            internal Template templateForDateContent;
            internal Template templateForIntegerContent;
            internal Template templateForBooleanContent;
            internal Template templateForFloatContent;


            Entities.DataItem objectForContent;
            public CodeProviderForDataItem(Entities.DataItem dataItem)
            {
                objectForContent = dataItem;

            }
            public static string TemplateName = "ItemsAppender";

            [PlaceHolder("DataItemName")]
            string DataItemName
            {
                get
                {
                    return objectForContent.ItemName;
                }
            }

            [PlaceHolder("FilledItems")]
            string FilledItems
            {
                get
                {
                    DataItemFillerContentProvider codeProvider
                        = new DataItemFillerContentProvider(objectForContent);
                    switch (objectForContent.ItemType)
                    {
                        case Entities.DataItem.DataItemType.String:
                            codeProvider.ContentTemplate = templateForStringContent;
                            break;
                        case Entities.DataItem.DataItemType.Date:
                            codeProvider.ContentTemplate = templateForDateContent;
                            break;
                        case Infosys.Lif.LegacyWorkbench.Entities.DataItem.DataItemType.Integer:

                        //case Entities.DataItem.DataItemType.SignedIntegerType:
                        //    codeProvider.ContentTemplate = templateForIntegerContent;
                        //    break;
                        case Entities.DataItem.DataItemType.Boolean:
                            codeProvider.ContentTemplate = templateForBooleanContent;
                            break;
                        case Entities.DataItem.DataItemType.Float:
                            codeProvider.ContentTemplate = templateForIntegerContent;
                            break;
                    }
                    return codeProvider.GenerateContent();
                }
            }
        }

        class DataItemFillerContentProvider : ContentProvider
        {
            Entities.DataItem objectForContent;
            internal DataItemFillerContentProvider(Entities.DataItem dataItem)
            {
                objectForContent = dataItem;
            }


            [PlaceHolder("DataItemName")]
            string DataItemName
            {
                get
                {
                    return objectForContent.ItemName;
                }
            }
        }
    }
}
