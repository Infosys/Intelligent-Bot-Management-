using System;
using System.Collections.Generic;
using System.Text;

using Infosys.Solutions.CodeGeneration.Framework;

namespace Infosys.Lif.LegacyWorkbench.CodeProviders.Serializers
{
    public class CobolContractSerializerCP : ContentProvider
    {
        Entities.Contract objectForContent;
        Entities.ContractModule objectModule;
        public CobolContractSerializerCP(Entities.Contract contractObject,
            Entities.ContractModule contractModule,
            Template contentTemplate)
        {
            ContentTemplate = contentTemplate;

            objectModule = contractModule;

            objectForContent = contractObject;
        }

        [PlaceHolder("ContractNameSpace")]
        string ContractNameSpace
        {
            get
            {
                return objectModule.SerializerNamespace;
            }
        }


        [PlaceHolder("DataEntityNameSpace")]
        string DataEntityNameSpace
        {
            get
            {
                return Framework.Helper.BuildNamespace(objectModule.DataEntityNamespace, objectForContent);
            }
        }

        [PlaceHolder("ContractDescription")]
        string ContractDescription
        {
            get
            {

                return (objectForContent.ContractDescription == null) ? string.Empty : objectForContent.ContractDescription;
            }
        }


        [PlaceHolder("ContractName")]
        string ContractName
        {
            get
            {
                TopLevelCP.TemplateForContract =
                    ContentTemplate.RepeatingTemplate(TopLevelCP.TemplateNameForContract);
                TopLevelCP.TemplateForModelObject =
                    ContentTemplate.RepeatingTemplate(TopLevelCP.TemplateNameForModelObject);
                CaseStatementContentProvider.Template
                    = ContentTemplate.RepeatingTemplate(CaseStatementContentProvider.TemplateName);
                ChildModelObjectFillerCP.TemplateForCode
                    = ContentTemplate.RepeatingTemplate("ChildModelObjectFiller");
                //////SwitchCaseContentProvider.Template
                //////    = ContentTemplate.RepeatingTemplate(SwitchCaseContentProvider.TemplateName);
                ////WhileLoopContentProvider.Template
                ////    = ContentTemplate.RepeatingTemplate(WhileLoopContentProvider.TemplateName);
                ////WhileLoopContentProviderMain.Template
                ////    = ContentTemplate.RepeatingTemplate(WhileLoopContentProviderMain.TemplateName);
                ForLoopCP.Template = ContentTemplate.RepeatingTemplate(ForLoopCP.TemplateName);
                //cdsrajan
                TemplateForMappingItem.Template = 
                    ContentTemplate.RepeatingTemplate(TemplateForMappingItem.TemplateName);
                TemplateForMappingItemCollection.Template = 
                    ContentTemplate.RepeatingTemplate(TemplateForMappingItemCollection.TemplateName);
                //cdsrajan ends

                return (objectForContent.ContractName == null) ? string.Empty : objectForContent.ContractName;
            }
        }

        [PlaceHolder("InputModelObjLength")]
        string InputModelObjLength
        {
            get
            {
                return(objectForContent.InputModelObjects.Count.ToString());
            }
        }

        [PlaceHolder("ImportsFilled")]
        string ImportsFilled
        {
            get
            {
                return string.Empty;
            }
        }

        #region For Serialization Bit
        [PlaceHolder("ForLoop")]
        string ForLoop
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (Entities.ModelObject ModelObject in objectForContent.InputModelObjects)
                {
                    ForLoopCP forLoopProvider
                        = new ForLoopCP(ModelObject, 0, "contractObject", "contractObject");
                    sb.Append(forLoopProvider.GenerateContent());
                }
                return sb.ToString();
            }
        }


        internal class ForLoopCP : ContentProvider
        {

            internal static System.Collections.Hashtable ModelObjectModuleMapping_cobol = new System.Collections.Hashtable();

            internal static string TemplateName = "TemplateForLoopRepeater";
            Entities.ModelObject objectForContent;
            internal static Template Template;
            int levelNumber = 0;

            string parentModelObjectName = string.Empty;
            string uniqueParentIdentifier;

            internal ForLoopCP(Entities.ModelObject ModelObjectObject, int _level,
                string _parentModelObjectName, string _uniqueParentIdentifier)
            {

                this.ContentTemplate = ForLoopCP.Template;
                objectForContent = ModelObjectObject;
                levelNumber = _level;
                parentModelObjectName = _parentModelObjectName;
                uniqueParentIdentifier = _uniqueParentIdentifier;
            }


            [PlaceHolder("HasChildModelObjects")]
            string HasChildModelObjects
            {
                get
                {
                    return ((objectForContent.ModelObjects.Count > 0) ? true.ToString().ToLowerInvariant() : false.ToString().ToLowerInvariant());
                }
            }

            [PlaceHolder("InputModelObjLength")]
            string InputModelObjLength
            {
                get
                {
                    return (objectForContent.ModelObjects.Count.ToString());
                }
            }

            [PlaceHolder("HasMaxLimit")]
            string HasMaxLimit
            {
                get
                {
                    return (objectForContent.MaxCount == -1) ? "false" : "true";
                }
            }
            [PlaceHolder("ParentModelObjectName")]
            string ParentModelObjectName
            {
                get
                {
                    return parentModelObjectName;
                }
            }

            [PlaceHolder("LevelNumber")]
            string LevelNumber
            {
                get
                {
                    return levelNumber.ToString();
                }
            }
            [PlaceHolder("ModelObjectName")]
            string ModelObjectPropertyName
            {
                get
                {
                    return objectForContent.Name;
                }
            }
            [PlaceHolder("ClassName")]
            string ClassName
            {
                get
                {
                    return objectForContent.ModelObjectEntity.DataEntityClassName;
                }
            }
            [PlaceHolder("ProgramId")]
            string ProgramId
            {
                get
                {
                    return objectForContent.ModelObjectEntity.ProgramId;
                }
            }
            [PlaceHolder("ModelObjectEntityName")]
            string ModelObjectEntityName
            {
                get
                {
                    return objectForContent.ModelObjectEntity.EntityName;
                }
            }

            [PlaceHolder("MaxCount")]
            string MaxCount
            {
                get
                {
                    return objectForContent.MaxCount.ToString();
                }
            }

            [PlaceHolder("ForLoop")]
            string ForLoop
            {
                get
                {

                    StringBuilder sb = new StringBuilder();

                    if (objectForContent.ModelObjects.Count > 0)
                    {
                        for (int i = 0; i < objectForContent.ModelObjects.Count; i++)
                        {

                            ForLoopCP forLoopCP = new ForLoopCP(objectForContent.ModelObjects[i],
                                levelNumber + 1, parentModelObjectName + "." + objectForContent.Name + "Collection[counter" + objectForContent.Name + "_" + levelNumber + "]",
                                uniqueParentIdentifier + objectForContent.Name);
                            forLoopCP.ContentTemplate = ContentTemplate;
                            sb.Append(forLoopCP.GenerateContent());
                        }
                    }


                    return sb.ToString();
                }
            }

            [PlaceHolder("HostName")]
            string HostName
            {
                get
                {
                    if (objectForContent.HostName == null ||
                        objectForContent.HostName.Length == 0)
                    {
                        return objectForContent.Name;
                    }
                    else
                    {
                        return objectForContent.HostName;
                    }
                }
            }


            [PlaceHolder("ModelObjectDataEntityNameSpace")]
            string ModelObjectDataEntityNameSpace
            {
                get
                {
                    //add prog ID to Model obj module mapping
                    return ((Entities.Module)ModelObjectModuleMapping_cobol[objectForContent.ModelObjectEntity.ProgramId]).DataEntityNamespace;
                }
            }
            [PlaceHolder("ModelObjectSerializerNameSpace")]
            string ModelObjectSerializerNameSpace
            {
                get
                {
                    //add prog ID to Model obj module mapping
                    return ((Entities.Module)ModelObjectModuleMapping_cobol[objectForContent.ModelObjectEntity.ProgramId]).SerializerNamespace;
                }
            }
            [PlaceHolder("CheckCardinality")]
            string CheckCardinality
            {
                get
                {
                    string toBeGenerated = "false";
                    foreach (Entities.ModelObject ModelObjectObj in objectForContent.ModelObjects)
                    {
                        if (CheckChildCardinality(ModelObjectObj))
                        {
                            toBeGenerated = "true";
                            break;
                        }
                        else
                        {
                            toBeGenerated = "false";
                            break;
                        }
                    }
                    return toBeGenerated;
                }
            }

            [PlaceHolder("ChildModelObjectFilled")]
            string ChildModelObjectFilled
            {
                get
                {
                    if (CheckCardinality == "true")
                    {
                        ChildModelObjectFillerCP codeGen = new ChildModelObjectFillerCP(objectForContent);
                        codeGen.ContentTemplate = ChildModelObjectFillerCP.TemplateForCode;
                        return codeGen.GenerateContent();
                    }
                    return string.Empty;
                }
            }


            [PlaceHolder("IsADDRL7xxModelObject")]
            string IsADDRL7xxModelObject
            {
                get
                {
                    if (objectForContent.HostName != null && objectForContent.HostName.ToUpperInvariant().StartsWith("ADDRL"))
                    {
                        string strModelObjectNumber = objectForContent.HostName.Substring(5);
                        int ModelObjectNumber = 0;
                        if (Int32.TryParse(strModelObjectNumber, out ModelObjectNumber))
                        {
                            if (ModelObjectNumber < 800 && ModelObjectNumber > 699)
                            {
                                return "true";
                            }
                        }
                    }
                    return "false";
                }
            }


            private bool CheckChildCardinality(Infosys.Lif.LegacyWorkbench.Entities.ModelObject ModelObjectObj)
            {
                if (ModelObjectObj.MinCount > 0)
                {
                    return true;
                }
                else
                {
                    foreach (Entities.ModelObject ModelObjectO in ModelObjectObj.ModelObjects)
                    {
                        if (CheckChildCardinality(ModelObjectO))
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }


        }
        #endregion For Serialization Bit

        internal class ChildModelObjectFillerCP : ContentProvider
        {
            internal static Template TemplateForCode = null;
            Entities.ModelObject objectForCode;
            internal ChildModelObjectFillerCP(Entities.ModelObject ModelObjectObject)
            {
                objectForCode = ModelObjectObject;
            }
            [PlaceHolder("ModelObjectName")]
            string ModelObjectName
            {
                get
                {
                    return objectForCode.Name;
                }
            }
            [PlaceHolder("ChildModelObjectsFilled")]
            string ChildModelObjectsFilled
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (Entities.ModelObject ModelObjectOb in objectForCode.ModelObjects)
                    {
                        if (CheckChildCardinality(ModelObjectOb))
                        {
                            ChildModelObjectFillerCP codeGen = new ChildModelObjectFillerCP(ModelObjectOb);
                            codeGen.ContentTemplate = ChildModelObjectFillerCP.TemplateForCode;
                            sb.Append(codeGen.GenerateContent());
                        }
                    }
                    return sb.ToString();
                }
            }



            private bool CheckChildCardinality(Entities.ModelObject ModelObjectObj)
            {
                if (ModelObjectObj.MinCount > 0)
                {
                    return true;
                }
                else
                {
                    foreach (Entities.ModelObject ModelObjectO in ModelObjectObj.ModelObjects)
                    {
                        if (CheckChildCardinality(ModelObjectO))
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }

        }

        #region For De-serialization bit
        [PlaceHolder("TopLevelFilled")]
        string TopLevelFilled
        {
            get
            {
                TopLevelCP codeGen = new TopLevelCP(objectForContent);
                return codeGen.GenerateContent();
            }

        }


        internal class TopLevelCP : ContentProvider
        {
            internal static System.Collections.Hashtable ModelObjectModuleMapping_cobol = new System.Collections.Hashtable();
            internal static string TemplateNameForContract = "TemplateForTopLevel";
            internal static string TemplateNameForModelObject = "TemplateForChildModelObjects";
            internal static Template TemplateForContract;
            internal static Template TemplateForModelObject;

            Entities.Contract objectForCodeGenerationContract;
            Entities.ModelObject objectForCodeGenerationModelObject;
            string parentNamePrefix, parentNameSuffix, parentUniqueIdPrefix;
            string counterName;
            int levelNumber;
            internal TopLevelCP(Entities.Contract contractObject)
            {
                ContentTemplate = TemplateForContract;
                objectForCodeGenerationContract = contractObject;

                parentNamePrefix = "contractObject.";
                parentNameSuffix = "Collection";
                parentUniqueIdPrefix = "contractObject";
                levelNumber = 0;
                counterName = "";
            }

            internal TopLevelCP(Entities.ModelObject ModelObjectObject,
                string _parentName, string _parentUniqueId, int _levelNumber)
            {
                ContentTemplate = TemplateForModelObject;
                objectForCodeGenerationModelObject = ModelObjectObject;
                parentNamePrefix = _parentName;
                parentNameSuffix = "Collection";
                parentUniqueIdPrefix = _parentUniqueId;
                levelNumber = _levelNumber;
            }


            [PlaceHolder("InputModelObjLength")]
            string InputModelObjLength
            {
                get
                {
                    if (objectForCodeGenerationContract == null)
                    {
                        return (objectForCodeGenerationModelObject.ModelObjects.Count.ToString());
                    }
                    else
                    {
                        return (objectForCodeGenerationContract.InputModelObjects.Count.ToString());
                    }
                }
            }

            [PlaceHolder("CountersForChildElements")]
            string CountersForChildElements
            {
                get
                {
                    StringBuilder sb = new StringBuilder();

                    Entities.GenericCollection<Entities.ModelObject> ModelObjectCollection;
                    if (objectForCodeGenerationModelObject == null)
                    {
                        ModelObjectCollection =
                            (objectForCodeGenerationContract.OutputModelObjects == null) ? objectForCodeGenerationContract.OutputModelObjects : objectForCodeGenerationContract.OutputModelObjects;
                    }
                    else
                    {
                        ModelObjectCollection = objectForCodeGenerationModelObject.ModelObjects;
                    }
                    foreach (Entities.ModelObject ModelObjectObj in ModelObjectCollection)
                    {
                        sb.AppendLine("int " + parentUniqueIdPrefix + ModelObjectObj.Name + "Counter=0;");
                    }
                    return sb.ToString();
                }
            }

            [PlaceHolder("CaseStatementsFilledForContract")]
            string CaseStatementsFilled
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    if (objectForCodeGenerationContract != null)
                    {

                        Entities.GenericCollection<Entities.ModelObject> ModelObjects =
                            (objectForCodeGenerationContract.OutputModelObjects == null) ? objectForCodeGenerationContract.InputModelObjects : objectForCodeGenerationContract.OutputModelObjects;

                        RetrieveContentForCase(sb, ModelObjects);
                    }
                    return sb.ToString();
                }
            }

            private void RetrieveContentForCase(StringBuilder sb, Entities.GenericCollection<Entities.ModelObject> ModelObjects)
            {
                foreach (Entities.ModelObject ModelObject in ModelObjects)
                {
                    CaseStatementContentProvider codeProvider
                        = new CaseStatementContentProvider(ModelObject, parentNamePrefix + ModelObject.Name + parentNameSuffix + "[" + parentUniqueIdPrefix + ModelObject.Name + "Counter]", levelNumber, parentUniqueIdPrefix + ModelObject.Name);
                    sb.Append(codeProvider.GenerateContent());
                }
            }


            [PlaceHolder("CaseStatementsFilledForModelObject")]
            string CaseStatementsFilledForModelObject
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    if (objectForCodeGenerationModelObject != null)
                    {
                        RetrieveContentForCase(sb, objectForCodeGenerationModelObject.ModelObjects);
                    }
                    return sb.ToString();
                }
            }

            [PlaceHolder("ParentIdentifier")]
            string ParentIdentifier
            {
                get
                {
                    return parentUniqueIdPrefix;
                }
            }

        }


        ////////////[PlaceHolder("WhileLoopFilled")]
        ////////////string WhileLoopFilled
        ////////////{
        ////////////    get
        ////////////    {
        ////////////        WhileLoopContentProviderMain whileLoopProvider
        ////////////            = new WhileLoopContentProviderMain(objectForContent, "contractObject", 0, "contractObject");

        ////////////        return whileLoopProvider.GenerateContent();
        ////////////    }
        ////////////}


        //////internal class WhileLoopContentProvider : ContentProvider
        //////{
        //////    internal const string TemplateName = "WhileLoop";
        //////    internal static Template Template;

        //////    [PlaceHolder("UniqueParentIdentifier")]
        //////    string UniqueParentIdentifier
        //////    {
        //////        get
        //////        {
        //////            return uniqueIdentifier;
        //////        }
        //////    }


        //////    [PlaceHolder("LevelNumber")]
        //////    string LevelNumber
        //////    {
        //////        get
        //////        {
        //////            return levelNumber.ToString();
        //////        }
        //////    }

        //////    [PlaceHolder("ModelObjectName")]
        //////    string ModelObjectName
        //////    {
        //////        get
        //////        {
        //////            return objectForContent.Name;
        //////        }
        //////    }
        //////    [PlaceHolder("ModelObjectEntityName")]
        //////    string ModelObjectEntityName
        //////    {
        //////        get
        //////        {
        //////            return objectForContent.ModelObjectEntity.EntityName;
        //////        }
        //////    }

        //////    [PlaceHolder("ParentModelObjectName")]
        //////    string ParentModelObjectName
        //////    {
        //////        get
        //////        {
        //////            return parentModelObjectName;
        //////        }
        //////    }

        //////    protected const string parentSuffix = "Collection";

        //////    [PlaceHolder("SwitchCaseFilled")]
        //////    string SwitchCaseFilled
        //////    {
        //////        get
        //////        {
        //////            SwitchCaseContentProvider contentProvider
        //////                = new SwitchCaseContentProvider(objectForContent,
        //////                parentModelObjectName + parentSuffix + "[" + parentModelObjectName + parentSuffix + ".Count]" + Environment.NewLine, levelNumber, uniqueIdentifier);
        //////            return contentProvider.GenerateContent();
        //////        }
        //////    }
        //////    [PlaceHolder("ModelObjectDataEntityNS")]
        //////    string ModelObjectDataEntityNS
        //////    {
        //////        get
        //////        {
        //////            return ((Entities.Module)ForLoopCP.ModelObjectModuleMapping_cobol[objectForContent.Name.Substring(0, 4)]).DataEntityNamespace;
        //////        }
        //////    }

        //////    Entities.ModelObject objectForContent;
        //////    string parentModelObjectName;

        //////    int levelNumber;
        //////    string uniqueIdentifier;
        //////    internal WhileLoopContentProvider(Entities.ModelObject ModelObjectObject,
        //////        string _parentModelObjectName, int _levelNumber, string _uniqueIdentifier)
        //////    {
        //////        this.ContentTemplate = WhileLoopContentProvider.Template;
        //////        objectForContent = ModelObjectObject;
        //////        levelNumber = _levelNumber;
        //////        parentModelObjectName = _parentModelObjectName;
        //////        uniqueIdentifier = _uniqueIdentifier;
        //////    }
        //////}


        ////internal class WhileLoopContentProviderMain : ContentProvider
        ////{

        ////    internal const string TemplateName = "WhileLoop";
        ////    internal static Template Template;

        ////    [PlaceHolder("UniqueParentIdentifier")]
        ////    string UniqueParentIdentifier
        ////    {
        ////        get
        ////        {
        ////            return string.Empty;
        ////        }
        ////    }

        ////    [PlaceHolder("ModelObjectName")]
        ////    string ModelObjectName
        ////    {
        ////        get
        ////        {
        ////            return string.Empty;
        ////        }
        ////    }
        ////    [PlaceHolder("ModelObjectEntityName")]
        ////    string ModelObjectEntityName
        ////    {
        ////        get
        ////        {
        ////            return string.Empty;
        ////        }
        ////    }

        ////    [PlaceHolder("ParentModelObjectName")]
        ////    string ParentModelObjectName
        ////    {
        ////        get
        ////        {
        ////            return string.Empty;
        ////        }
        ////    }

        ////    //protected string parentSuffix = "Collection[0]" + Environment.NewLine;

        ////    [PlaceHolder("SwitchCaseFilled")]
        ////    string SwitchCaseFilled
        ////    {
        ////        get
        ////        {

        ////            StringBuilder sb = new StringBuilder();

        ////            Entities.GenericCollection<Entities.ModelObject> ModelObjects =
        ////                (objectForContent.OutputModelObjects == null) ? objectForContent.InputModelObjects : objectForContent.OutputModelObjects;
        ////            foreach (Entities.ModelObject ModelObject in ModelObjects)
        ////            {

        ////                SwitchCaseContentProvider contentProvider
        ////                    = new SwitchCaseContentProvider(ModelObject,
        ////                    parentModelObjectName, 0, uniqueParentIdentifier);
        ////                sb.Append(contentProvider.GenerateContent());
        ////            }

        ////            return sb.ToString();
        ////        }
        ////    }
        ////    [PlaceHolder("ModelObjectDataEntityNS")]
        ////    string ModelObjectDataEntityNS
        ////    {
        ////        get
        ////        {

        ////            return string.Empty;
        ////        }
        ////    }

        ////    [PlaceHolder("LevelNumber")]
        ////    string LevelNumber
        ////    {
        ////        get
        ////        {
        ////            return levelNumber.ToString();
        ////        }
        ////    }

        ////    Entities.Contract objectForContent;
        ////    string parentModelObjectName;
        ////    int levelNumber;

        ////    string uniqueParentIdentifier;
        ////    internal WhileLoopContentProviderMain(Entities.Contract contractObject,
        ////        string _parentModelObjectName, int _level, string _UniqueParentIdentifier)
        ////    {
        ////        this.ContentTemplate = WhileLoopContentProvider.Template;
        ////        objectForContent = contractObject;
        ////        levelNumber = _level;
        ////        parentModelObjectName = _parentModelObjectName;
        ////        uniqueParentIdentifier = _UniqueParentIdentifier;
        ////        //parentSuffix = "Collection[0]" + Environment.NewLine;
        ////    }

        ////}



        ////////internal class SwitchCaseContentProvider : ContentProvider
        ////////{
        ////////    internal const string TemplateName = "SwitchCase";
        ////////    Entities.ModelObject objectForContent;
        ////////    string parentModelObjectName;
        ////////    int levelNumber;
        ////////    string uniqueParentIdentifier;
        ////////    internal SwitchCaseContentProvider(Entities.ModelObject ModelObjectObject,
        ////////        string _parentModelObjectName, int _levelNumber, string _uniqueParentIdentifier)
        ////////    {
        ////////        this.ContentTemplate = SwitchCaseContentProvider.Template;
        ////////        parentModelObjectName = _parentModelObjectName;
        ////////        objectForContent = ModelObjectObject;
        ////////        levelNumber = _levelNumber;
        ////////        uniqueParentIdentifier = _uniqueParentIdentifier;
        ////////    }


        ////////    internal static Template Template;


        ////////    [PlaceHolder("CaseStatementFilled")]
        ////////    string CaseStatementFilled
        ////////    {
        ////////        get
        ////////        {
        ////////            StringBuilder sb = new StringBuilder();
        ////////            foreach (Entities.ModelObject ModelObject in objectForContent.ModelObjects)
        ////////            {
        ////////                CaseStatementContentProvider caseProvider
        ////////                    = new CaseStatementContentProvider(ModelObject,
        ////////                    parentModelObjectName + "." + ModelObject.HostName + "Collection[0]" + Environment.NewLine, levelNumber, uniqueParentIdentifier + ModelObject.Name);// + "Collection[0]" + Environment.NewLine);


        ////////                sb.Append(caseProvider.GenerateContent());
        ////////            }
        ////////            return sb.ToString();
        ////////        }
        ////////    }

        ////////}


        internal class CaseStatementContentProvider : ContentProvider
        {
            Entities.Entity objectForContent;

            Entities.ModelObject objectForChildContent;

            internal static Template Template;

            internal const string TemplateName = "CaseStatement";


            string parentModelObjectName;
            string uniqueParentIdentifier;
            int levelNumber;
            internal CaseStatementContentProvider(Entities.ModelObject ModelObjectObject,
                string _parentModelObjectName, int _levelNumber, string _uniqueParentIdentifier)
            {
                this.ContentTemplate = CaseStatementContentProvider.Template;
                objectForContent = ModelObjectObject.ModelObjectEntity;
                objectForChildContent = ModelObjectObject;
                parentModelObjectName = _parentModelObjectName;
                levelNumber = _levelNumber;
                uniqueParentIdentifier = _uniqueParentIdentifier;
            }

            [PlaceHolder("ClassName")]
            string ClassName
            {
                get
                {
                    return objectForContent.DataEntityClassName;
                }
            }

            [PlaceHolder("InputModelObjLength")]
            string InputModelObjLength
            {
                get
                {
                    return (objectForChildContent.ModelObjects.Count.ToString());
                }
            }

            [PlaceHolder("ParentIdentifier")]
            string ParentIdentifier
            {
                get
                {
                    return uniqueParentIdentifier;
                }
            }



            [PlaceHolder("ParentModelObjectName")]
            string ParentModelObjectName
            {
                get
                {
                    return parentModelObjectName;
                }
            }


            [PlaceHolder("ParentModelObjectNameWithoutZero")]
            string ParentModelObjectNameWithoutZero
            {
                get
                {
                    string strTemp = parentModelObjectName.Substring(0, parentModelObjectName.LastIndexOf('['));//
                    return strTemp;

                    //////string strToBeDeleted = parentModelObjectName;

                    //////for (int counter = 0; counter <= levelNumber; counter++)
                    //////{
                    //////    int indexOfBracket = strToBeDeleted.IndexOf('[');
                    //////    strToBeDeleted = strToBeDeleted.Substring(indexOfBracket + 1);
                    //////}
                    //////strToBeDeleted = parentModelObjectName.Substring(0, parentModelObjectName.Length - strToBeDeleted.Length);
                    //////return strToBeDeleted;
                    //return parentModelObjectName.Substring(0, parentModelObjectName.LastIndexOf('['));
                }
            }


            [PlaceHolder("ChildModelObjectsFilled")]
            string ChildModelObjectsFilled
            {
                get
                {
                    TopLevelCP codeGenerator = new TopLevelCP(objectForChildContent,
                        parentModelObjectName + ".", uniqueParentIdentifier, levelNumber + 1);
                    return codeGenerator.GenerateContent();
                }
            }


            [PlaceHolder("HostName")]
            string HostName
            {
                get
                {
                    if (objectForChildContent.HostName == null ||
                        objectForChildContent.HostName.Length == 0)
                    {
                        return objectForChildContent.Name.ToUpperInvariant();
                    }
                    else
                    {
                        return objectForChildContent.HostName.ToUpperInvariant();
                    }
                }
            }




            [PlaceHolder("ProgramId")]
            string ProgramId
            {
                get
                {
                    return objectForChildContent.Name;
                }
            }

            [PlaceHolder("ModelObjectEntityId")]
            string ModelObjectEntityId
            {
                get
                {
                    return objectForContent.ProgramId;
                }
            }

            [PlaceHolder("ModelObjectName")]
            string ModelObjectSerializer
            {
                get
                {
                    return objectForContent.EntityName;
                }
            }
            [PlaceHolder("ModelObjectSerializerNS")]
            string ModelObjectSerializerNS
            {
                get
                {
                    //add prog ID to Model obj module mapping
                    return ((Entities.Module)ForLoopCP.ModelObjectModuleMapping_cobol[objectForChildContent.ModelObjectEntity.ProgramId]).SerializerNamespace;
                }
            }
            [PlaceHolder("ModelObjectDataEntityNS")]
            string ModelObjectDataEntityNS
            {
                get
                {
                    //add prog ID to Model obj module mapping
                    return Framework.Helper.BuildNamespace(((Entities.Module)ForLoopCP.ModelObjectModuleMapping_cobol[objectForChildContent.ModelObjectEntity.ProgramId]).DataEntityNamespace, objectForContent);
                }
            }



            //////[PlaceHolder("TopLevelFilled")]
            //////string TopLevelFilled
            //////{
            //////    get
            //////    {
            //////        StringBuilder sb = new StringBuilder();
            //////    }
            //////}

            ////////[PlaceHolder("WhileLoopFilled")]
            ////////string WhileLoopFilled
            ////////{
            ////////    get
            ////////    {
            ////////        StringBuilder sb = new StringBuilder();
            ////////        foreach (Entities.ModelObject ModelObject in objectForChildContent.ModelObjects)
            ////////        {
            ////////            WhileLoopContentProvider contentProvider
            ////////                = new WhileLoopContentProvider(ModelObject, parentModelObjectName + "." + ModelObject.HostName, levelNumber + 1, uniqueParentIdentifier + ModelObject.Name);
            ////////            sb.Append(contentProvider.GenerateContent());
            ////////        }
            ////////        return sb.ToString();
            ////////    }
            ////////}
            [PlaceHolder("LevelNumber")]
            string LevelNumber
            {
                get
                {
                    return levelNumber.ToString();
                }
            }
            //cdsrajan
            //[PlaceHolder("DataItemName")]
            //string DataItemName
            //{
            //    get
            //    {
            //        Entities.DataItem objName = new Entities.DataItem();
            //        return objName.ItemName.ToString();
            //    }
            //}
            [PlaceHolder("MappingItems")]
            string MappingItems
            {
                get
                {
                    StringBuilder sb = new StringBuilder();

                    //foreach (Entities.ModelObject ModelObject in objectForContent)
                    //{
                    //    TemplateForMappingItem forLoopProvider
                    //        = new TemplateForMappingItem(ModelObject, 0, "contractObject", "contractObject");
                    //    sb.Append(forLoopProvider.GenerateContent());
                    //}
                    //foreach(
                    //Entities.GenericCollection<Entities.DataItem> dataItemsCol=new Infosys.Lif.LegacyWorkbench.Entities.GenericCollection<Infosys.Lif.LegacyWorkbench.Entities.DataItem>();
                    //foreach (Entities.ModelObject ModelObject in objectForContent)
                    //{
                    //Entities.GenericCollection<Entities.ModelObject> ModelObjects;
                    //Entities.ModelObject ModelObjects;
                    //ModelObjects = objectForChildContent.ModelObjects;
                    //foreach (Entities.ModelObject ModelObject in objectForChildContent)
                    //{
                        for (int i = 0; i < objectForContent.DataItems[0].GroupItems.Count; i++)
                        {
                            if (objectForContent.DataItems[0].GroupItems[i].NumberOfOccurences>1)
                            {
                                TemplateForMappingItemCollection forLoopCol
                                = new TemplateForMappingItemCollection(objectForContent.DataItems[0].GroupItems[i], objectForChildContent);
                                sb.Append(forLoopCol.GenerateContent());
                            }
                            else
                            {
                                TemplateForMappingItem forLoopProvider
                                = new TemplateForMappingItem(objectForContent.DataItems[0].GroupItems[i], objectForChildContent);
                                sb.Append(forLoopProvider.GenerateContent());
                            }
                        }
                    //}
                    //}
                    return sb.ToString();
                }
            }

        }



        #endregion For De-serialization bit

        //cdsrajan

        //[PlaceHolder("MappingItems")]
        //string MappingItems
        //{
        //    get
        //    {
        //        StringBuilder sb = new StringBuilder();

        //        foreach (Entities.ModelObject ModelObject in objectForContent.InputModelObjects)
        //        {
        //            TemplateForMappingItem forLoopProvider
        //                = new TemplateForMappingItem(ModelObject, 0, "contractObject", "contractObject");
        //            sb.Append(forLoopProvider.GenerateContent());
        //        }
        //        return sb.ToString();
        //    }
        //}

        internal class TemplateForMappingItem : ContentProvider
        {
            internal static string TemplateName = "TemplateForMappingItem";
            internal static Template Template;
            Entities.ModelObject objectForContent;
            int levelNumber = 0;
            string parentModelObjectName = string.Empty;
            string uniqueParentIdentifier;
            Entities.DataItem dataItem;
            //internal TemplateForMappingItem(Entities.ModelObject ModelObjectObject, int _level,
            //    string _parentModelObjectName, string _uniqueParentIdentifier)
            //{

            //    this.ContentTemplate = ForLoopCP.Template;
            //    objectForContent = ModelObjectObject;
            //    levelNumber = _level;
            //    parentModelObjectName = _parentModelObjectName;
            //    uniqueParentIdentifier = _uniqueParentIdentifier;
            //}
            internal TemplateForMappingItem(Entities.DataItem obj, Entities.ModelObject ModelObjectObject)
            {
                this.ContentTemplate = TemplateForMappingItem.Template;
                objectForContent = ModelObjectObject;
                dataItem = obj;
            }
            [PlaceHolder("ClassName")]
            string ClassName
            {
                get
                {
                    return objectForContent.ModelObjectEntity.DataEntityClassName;
                }
            }

            [PlaceHolder("DataItemName")]
            string DataItemName
            {
                get
                {
                    //Entities.DataItem objName=new Entities.DataItem();
                    //return objName.ItemName.ToString();
                    return dataItem.ToString();
                }
            }
        }

        internal class TemplateForMappingItemCollection : ContentProvider
        {
            internal static string TemplateName = "TemplateForMappingItemCollection";
            internal static Template Template;
            Entities.ModelObject objectForContent;
            int levelNumber = 0;
            Entities.DataItem dataItem;
            string parentModelObjectName = string.Empty;
            string uniqueParentIdentifier;

            internal TemplateForMappingItemCollection(Entities.DataItem obj, Entities.ModelObject ModelObjectObject)
            {
                this.ContentTemplate = TemplateForMappingItemCollection.Template;
                objectForContent = ModelObjectObject;
                dataItem = obj;
            }
            [PlaceHolder("ClassName")]
            string ClassName
            {
                get
                {
                    return objectForContent.ModelObjectEntity.DataEntityClassName;
                }
            }

            [PlaceHolder("DataItemName")]
            string DataItemName
            {
                get
                {
                    return dataItem.ToString();
                }
            }
        }

    }
}
