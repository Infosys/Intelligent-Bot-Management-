using System;
using System.Collections.Generic;
using System.Text;

using Infosys.Solutions.CodeGeneration.Framework;

namespace Infosys.Lif.LegacyWorkbench.CodeProviders.DataEntities
{

    internal class ImportModelObjectDefinition
    {


        Entities.Entity childEntity;
        public Entities.Entity ChildEntity
        {
            get { return childEntity; }
            set { childEntity = value; }
        }


        Entities.ModelObjectModule childModelObjectModule;
        public Entities.ModelObjectModule ChildModelObjectModule
        {
            get { return childModelObjectModule; }
            set { childModelObjectModule = value; }
        }

        Entities.ModelObject childModelObject;

        public Entities.ModelObject ChildModelObject
        {
            get { return childModelObject; }
            set { childModelObject = value; }
        }


    }

    public class ModelObjectDataEntityCP : ContentProvider
    {

        Entities.Entity objectForCodeGeneration;
        Entities.ModelObjectModule ModelObjectModuleForCodeGeneration;

        Entities.GenericCollection<ImportModelObjectDefinition> importedModelObjects;

        internal ModelObjectDataEntityCP(Entities.ModelObjectModule ModelObjectModule,
            Entities.Entity entityObject, Entities.GenericCollection<ImportModelObjectDefinition> importModelObjects,
            Template templateObject)
        {
            importedModelObjects = importModelObjects;
            ModelObjectModuleForCodeGeneration = ModelObjectModule;
            objectForCodeGeneration = entityObject;
            ContentTemplate = templateObject;
        }

        [PlaceHolder("XsdNameSpace")]
        string XsdNameSpace
        {
            get
            {
                string keyForMapping
                    = "ModelObject:" + ModelObjectModuleForCodeGeneration.Name + ":"
                    + objectForCodeGeneration.ProgramId;
                return Mappings.XsdNameSpaceMappings[keyForMapping].ToString();
            }
        }

        ////////[PlaceHolder("NameSpaceImportsFilled")]
        ////////string NameSpaceImportsFilled
        ////////{
        ////////    get
        ////////    {
        ////////        StringBuilder sb = new StringBuilder();
        ////////        Template template = ContentTemplate.RepeatingTemplate(NameSpaceImports.TemplateName);
        ////////        if (importedModelObjects != null)
        ////////        {
        ////////            System.Collections.Hashtable hashTable = new System.Collections.Hashtable();

        ////////            foreach (ImportModelObjectDefinition importedModelObject in importedModelObjects)
        ////////            {
        ////////                if (!hashTable.ContainsKey(objectForCodeGeneration.ProgramId + "_" + importedModelObject.ChildEntity.ProgramId))
        ////////                {
        ////////                    NameSpaceImports importedCp = new NameSpaceImports(objectForCodeGeneration.ProgramId, importedModelObject.ChildEntity.ProgramId, importedModelObject.ChildModelObjectModule.Name, importedModelObject.ChildEntity.EntityName);
        ////////                    importedCp.ContentTemplate = template;
        ////////                    sb.Append(importedCp.GenerateContent());
        ////////                    hashTable.Add(objectForCodeGeneration.ProgramId + "_" + importedModelObject.ChildEntity.ProgramId, null);
        ////////                }
        ////////            }
        ////////        }
        ////////        return sb.ToString();
        ////////    }
        ////////}


        internal class NameSpaceImports : ContentProvider
        {
            internal static string TemplateName = "NameSpaceImports";

            string _ParentIdentifier;

            [PlaceHolder("ShortIdentifierParent")]
            public string ParentIdentifier
            {
                get { return _ParentIdentifier; }
            }


            string _EntityIdentifier;
            [PlaceHolder("ShortIdentifier")]
            public string EntityIdentifier
            {
                get { return _EntityIdentifier; }
            }


            string _ModuleName;
            [PlaceHolder("ModuleName")]
            public string ModuleName
            {
                get { return _ModuleName; }
            }

            string _EntityName;
            [PlaceHolder("EntityName")]
            public string EntityName
            {
                get { return _EntityName; }
            }


            [PlaceHolder("XsdNameSpace")]
            string XsdNameSpace
            {
                get
                {
                    string keyForMapping
                        = "ModelObject:" + _ModuleName + ":" + _EntityIdentifier;
                    return Mappings.XsdNameSpaceMappings[keyForMapping].ToString();
                }
            }

            internal NameSpaceImports(string parentIdentifier, string entityIdentifier, string moduleName, string entityName)
            {
                _ParentIdentifier = parentIdentifier;
                _EntityIdentifier = entityIdentifier;
                _EntityName = entityName;
                _ModuleName = moduleName;
            }


        }

        [PlaceHolder("ModuleName")]
        string ModuleName
        {
            get
            {
                return ModelObjectModuleForCodeGeneration.Name;
            }
        }


        [PlaceHolder("DataEntityName")]
        string DataEntityName
        {
            get
            {
                return objectForCodeGeneration.EntityName;
            }
        }

        [PlaceHolder("DataItems")]
        string DataItems
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (Entities.DataItem dataItem in objectForCodeGeneration.DataItems)
                {
                    DataItemCP contentProvider = new DataItemCP(dataItem,
                        ContentTemplate.RepeatingTemplate(DataItemCP.TemplateName));
                    sb.Append(contentProvider.GenerateContent());
                }
                return sb.ToString();
            }
        }

        //////[PlaceHolder("Imports")]
        //////string Imports
        //////{
        //////    get
        //////    {
        //////        Template templateObject
        //////            = ContentTemplate.RepeatingTemplate(ImportCP.TemplateName);

        //////        StringBuilder sb = new StringBuilder();
        //////        if (importedModelObjects != null)
        //////        {
        //////            Entities.GenericCollection<string> completed
        //////                = new Entities.GenericCollection<string>();
        //////            foreach (ImportModelObjectDefinition ModelObjectDefn in importedModelObjects)
        //////            {
        //////                bool isAlreadyAdded = false;
        //////                foreach (string completedProgram in completed)
        //////                {
        //////                    if (completedProgram == ModelObjectDefn.ChildEntity.ProgramId)
        //////                    {
        //////                        isAlreadyAdded = true;
        //////                        break;
        //////                    }
        //////                }
        //////                if (!isAlreadyAdded && objectForCodeGeneration.ProgramId != ModelObjectDefn.ChildEntity.ProgramId)
        //////                {
        //////                    ImportCP codeProvider = new ImportCP(ModelObjectDefn, objectForCodeGeneration.ProgramId);
        //////                    codeProvider.ContentTemplate = templateObject;
        //////                    sb.Append(codeProvider.GenerateContent());
        //////                    completed.Add(ModelObjectDefn.ChildEntity.ProgramId);
        //////                }
        //////            }
        //////        }
        //////        return sb.ToString();
        //////    }
        //////}



        ////////[PlaceHolder("ImportedClasses")]
        ////////string ImportedClasses
        ////////{
        ////////    get
        ////////    {
        ////////        StringBuilder sb = new StringBuilder();
        ////////        Template template = ContentTemplate.RepeatingTemplate(ReferenceToClassesCP.TemplateName);
        ////////        if (importedModelObjects != null)
        ////////        {
        ////////            foreach (ImportModelObjectDefinition childModelObject in importedModelObjects)
        ////////            {
        ////////                ReferenceToClassesCP codeProvider
        ////////                    = new ReferenceToClassesCP(childModelObject, objectForCodeGeneration);
        ////////                codeProvider.ContentTemplate = template;
        ////////                sb.Append(codeProvider.GenerateContent());
        ////////            }
        ////////        }

        ////////        return sb.ToString();
        ////////    }
        ////////}

        class ReferenceToClassesCP : ContentProvider
        {
            ImportModelObjectDefinition ModelObjectDefinition;
            Entities.Entity parent;
            string programId;
            internal ReferenceToClassesCP(ImportModelObjectDefinition importedModelObject, Entities.Entity parentEntity)
            {
                ModelObjectDefinition = importedModelObject;
                parent = parentEntity;
            }


            [PlaceHolder("ItemName")]
            string ItemName
            {
                get
                {
                    return ModelObjectDefinition.ChildEntity.EntityName;
                }
            }


            [PlaceHolder("ImportedModelObjectName")]
            string ImportedModelObjectName
            {
                get
                {
                    return ModelObjectDefinition.ChildModelObject.Name;
                }
            }



            [PlaceHolder("ModelObjectName")]
            string ModelObjectName
            {
                get
                {//TBD
                    return parent.ProgramId;
                }
            }
            [PlaceHolder("ImportModelObjectId")]
            string ImportModelObjectId
            {
                get
                {
                    return ModelObjectDefinition.ChildEntity.ProgramId;
                }
            }


            internal static string TemplateName = "ReferenceToClasses";

        }

        class DataItemCP : ContentProvider
        {
            internal static string TemplateName = "DataItem";

            Entities.DataItem objectForContent;
            internal DataItemCP(Entities.DataItem dataItem, Template template)
            {
                objectForContent = dataItem;
                ContentTemplate = template;
            }


            [PlaceHolder("Name")]
            string Name
            {
                get
                {
                    return objectForContent.ItemName;
                }
            }
            [PlaceHolder("Type")]
            string Type
            {
                get
                {
                    string type = string.Empty;
                    switch (objectForContent.ItemType)
                    {
                        case Entities.DataItem.DataItemType.Boolean:
                            type = "boolean";
                            break;
                        case Entities.DataItem.DataItemType.Date:
                            type = "date";
                            break;
                        case Infosys.Lif.LegacyWorkbench.Entities.DataItem.DataItemType.String:
                            type = "string";
                            break;
                        //case Infosys.Lif.LegacyWorkbench.Entities.DataItem.DataItemType.SignedIntegerType:
                        case Infosys.Lif.LegacyWorkbench.Entities.DataItem.DataItemType.Integer:
                            type = "int";
                            break;
                        case Infosys.Lif.LegacyWorkbench.Entities.DataItem.DataItemType.Float:
                            type = "double";
                            break;
                    }
                    return type;
                }
            }

        }


        class ImportCP : ContentProvider
        {

            static internal string TemplateName = "Import";
            ImportModelObjectDefinition ModelObjectDefinition;
            string currentModelObjectName;
            internal ImportCP(ImportModelObjectDefinition importModelObjectDefn, string currModelObjectName)
            {
                currentModelObjectName = currModelObjectName;
                ModelObjectDefinition = importModelObjectDefn;
            }

            [PlaceHolder("CurrentModelObjectName")]
            string CurrentModelObjectName
            {
                get
                {
                    return currentModelObjectName;
                }
            }

            [PlaceHolder("ImportModelObjectId")]
            string ImportModelObjectId
            {
                get
                {
                    return ModelObjectDefinition.ChildEntity.ProgramId;
                }
            }
            [PlaceHolder("ImportModuleName")]
            string ImportModuleName
            {
                get
                {
                    return ModelObjectDefinition.ChildModelObjectModule.Name;
                }
            }

            [PlaceHolder("ImportModelObjectName")]
            string ImportModelObjectName
            {
                get
                {
                    return ModelObjectDefinition.ChildEntity.EntityName;
                }
            }

            [PlaceHolder("XsdNameSpace")]
            string XsdNameSpace
            {
                get
                {
                    string keyForMapping
                        = "ModelObject:" + ModelObjectDefinition.ChildModelObjectModule.Name + ":"
                        + ModelObjectDefinition.ChildEntity.ProgramId;
                    return Mappings.XsdNameSpaceMappings[keyForMapping].ToString();
                }
            }

        }


    }
}
