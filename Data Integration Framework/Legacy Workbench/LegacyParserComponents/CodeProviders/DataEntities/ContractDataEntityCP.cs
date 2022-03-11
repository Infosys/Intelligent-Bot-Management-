using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

using Infosys.Solutions.CodeGeneration.Framework;

namespace Infosys.Lif.LegacyWorkbench.CodeProviders.DataEntities
{
    public class ModelObjectDetailsMapping
    {
        internal ModelObjectDetailsMapping()
        { }

        internal string ModuleName;
        internal string ModelObjectName;
    }

    internal class ContractDataEntityCP : ContentProvider
    {        
        internal static void Initialize()
        {
            alreadyImportedFiles = new System.Collections.ArrayList();
            alreadyImportedNameSpaces = new System.Collections.ArrayList();
            alreadyDefinedModelObjects = new System.Collections.ArrayList();
        }

        static System.Collections.ArrayList alreadyImportedFiles
            = new System.Collections.ArrayList();
        static System.Collections.ArrayList alreadyImportedNameSpaces = new System.Collections.ArrayList();
        static System.Collections.ArrayList alreadyDefinedModelObjects = new System.Collections.ArrayList();


        Entities.Contract objectForCodeGeneration;
        internal static System.Collections.Hashtable ModelObjectDetailsMapping;
     
        internal ContractDataEntityCP(Entities.Contract contractToBeGenerated,
            System.Collections.Hashtable mappings)
        {
            objectForCodeGeneration = contractToBeGenerated;
            ModelObjectDetailsMapping = mappings;            
        }


        internal void ProvideTemplate(Template contentProvider)
        {
            ContentTemplate = contentProvider;
        }


        [PlaceHolder("XsdNameSpace")]
        string XsdNameSpace
        {
            get
            {

                foreach(string strKey in Mappings.XsdNameSpaceMappings.Keys)
                {
                    if(strKey.StartsWith("Contract:")
                        && strKey.EndsWith(objectForCodeGeneration.ContractName))
                    {
                        return Mappings.XsdNameSpaceMappings[strKey].ToString().Trim();
                    }
                }
                return string.Empty;
            }
        }


        [PlaceHolder("ContractDefinitionFilled")]
        string ContractDefinitionFilled
        {
            get
            {
                ContractDefinitionCP codeGen = new ContractDefinitionCP(objectForCodeGeneration);
                codeGen.ContentTemplate = ContentTemplate.RepeatingTemplate("ContractDefinition");
                ChildModelObjectInstancesCP.TemplateForCode = ContentTemplate.RepeatingTemplate("ChildModelObjectInstancesTemplate");
                return codeGen.GenerateContent();
            }
        }


        internal class ContractDefinitionCP : ContentProvider
        {
            Entities.Contract objForCodeGeneration;
            internal ContractDefinitionCP(Entities.Contract contractObj)
            {
                objForCodeGeneration = contractObj;
            }

            [PlaceHolder("ContractName")]
            string ContractName
            {
                get
                {
                    return objForCodeGeneration.ContractName;
                }
            }

            [PlaceHolder("ModelObjectsInstancesFilled")]
            string ModelObjectsInstancesFilled
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    ChildModelObjectInstancesCP codeProvider;
                    Hashtable htInModelObjNames = new Hashtable(); 
                    foreach (Entities.ModelObject InModelObject in objForCodeGeneration.InputModelObjects)
                    {
                        codeProvider = new ChildModelObjectInstancesCP(InModelObject.Name, InModelObject.ModelObjectEntity.ProgramId);
                        sb.Append(codeProvider.GenerateContent());
                        htInModelObjNames.Add(InModelObject.Name.Trim(), InModelObject.Name.Trim()); 
                    }

                    //change : to remove duplicate XSD elements
                    foreach (Entities.ModelObject OutModelObject in objForCodeGeneration.OutputModelObjects)
                    {
                        if (htInModelObjNames.Contains(OutModelObject.Name.Trim()))
                        {
                            break;
                        }   
                        else
                        {
                            codeProvider = new ChildModelObjectInstancesCP(OutModelObject.Name, OutModelObject.ModelObjectEntity.ProgramId);
                            sb.Append(codeProvider.GenerateContent());
                        }                                      
                    }
                    //end
                    return sb.ToString();
                }
            }
        }

        [PlaceHolder("NameSpacesFilled")]
        string NameSpacesFilled
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                RetrieveImportNS(sb, objectForCodeGeneration.InputModelObjects);
                RetrieveImportNS(sb, objectForCodeGeneration.OutputModelObjects);
                return sb.ToString();

            }
        }

        private void RetrieveImportNS(StringBuilder sb, Infosys.Lif.LegacyWorkbench.Entities.GenericCollection<Infosys.Lif.LegacyWorkbench.Entities.ModelObject> genericCollection)
        {
            foreach (Entities.ModelObject ModelObject in genericCollection)
            {
                ImportedNamespacesCP codeProv = new ImportedNamespacesCP(ModelObject);
                codeProv.ContentTemplate = ContentTemplate.RepeatingTemplate("ImportedNamespaces");
                
                bool isToBeAdded = true;
                foreach (string str in alreadyImportedNameSpaces)
                {
                    if (str == ModelObject.ModelObjectEntity.ProgramId)
                    {
                        isToBeAdded = false;
                        break;
                    }
                }
                if (isToBeAdded)
                {
                    alreadyImportedNameSpaces.Add(ModelObject.ModelObjectEntity.ProgramId);
                    sb.Append(codeProv.GenerateContent());
                }
                RetrieveImportNS(sb, ModelObject.ModelObjects);
            }
        }


        internal class ImportedNamespacesCP : ContentProvider
        {
            Entities.ModelObject ModelObjectForGeneration;
            internal ImportedNamespacesCP(Entities.ModelObject ModelObject)
            {
                ModelObjectForGeneration = ModelObject;
            }

            [PlaceHolder("ProgramId")]
            string ProgramId
            {
                get
                {
                    return ModelObjectForGeneration.ModelObjectEntity.ProgramId;
                }
            }

            [PlaceHolder("XsdNameSpace")]
            string XsdNameSpace
            {
                get
                {
                    //TINS
                    if (CodeGeneratorBase.IsTins)
                    {
                        return Mappings.XsdNameSpaceMappings["ModelObject:"
                        + ((ModelObjectDetailsMapping)ContractDataEntityCP.ModelObjectDetailsMapping[ModelObjectForGeneration.ModelObjectEntity.ProgramId]).ModuleName
                        + ":" + ModelObjectForGeneration.Name.Substring(0, 4)].ToString();
                    }

                    //Cobol
                    else
                    {
                        //add Prog ID in XSD namespace mapping
                        return Mappings.XsdNameSpaceMappings["ModelObject:"
                        + ((ModelObjectDetailsMapping)ContractDataEntityCP.ModelObjectDetailsMapping[ModelObjectForGeneration.ModelObjectEntity.ProgramId]).ModuleName
                        + ":" + ModelObjectForGeneration.ModelObjectEntity.ProgramId].ToString();
                    }
                }
            }
        }

        [PlaceHolder("ImportFilesFilled")]
        string ImportFilesFilled
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                ImportTemplateCP.TemplateForCode = ContentTemplate.RepeatingTemplate("ImportTemplate");
                foreach (Entities.ModelObject ModelObject in objectForCodeGeneration.InputModelObjects)
                {
                    BuildImportCode(sb, ModelObject);
                }

                foreach (Entities.ModelObject ModelObject in objectForCodeGeneration.OutputModelObjects)
                {
                    BuildImportCode(sb, ModelObject);
                }


                return sb.ToString();
            }
        }

        private void BuildImportCode(StringBuilder sb, Infosys.Lif.LegacyWorkbench.Entities.ModelObject ModelObject)
        {
            ImportTemplateCP codeProvider = new ImportTemplateCP(ModelObject);

            foreach (Entities.ModelObject ModelObjectObj in ModelObject.ModelObjects)
            {
                BuildImportCode(sb, ModelObjectObj);
            }

            bool isToBeAdded = true;

            foreach (string str in alreadyImportedFiles)
            {
                if (str == ModelObject.ModelObjectEntity.ProgramId)
                {
                    isToBeAdded = false;
                    break;
                }
            }

            if (isToBeAdded)
            {
                alreadyImportedFiles.Add(ModelObject.ModelObjectEntity.ProgramId);
                sb.Append(codeProvider.GenerateContent());
            }
        }


        [PlaceHolder("ModelObjectDefinitionFilled")]
        string ModelObjectDefinitionFilled
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                BuildModelObjectDefinition(sb, objectForCodeGeneration.InputModelObjects, objectForCodeGeneration.ContractName);
                BuildModelObjectDefinition(sb, objectForCodeGeneration.OutputModelObjects, objectForCodeGeneration.ContractName);
                return sb.ToString();
            }
        }

        private void BuildModelObjectDefinition(StringBuilder sb,
            Entities.GenericCollection<Entities.ModelObject> genericCollection, string contractName)
        {
            ModelObjectDefinition.TemplateForCode = ContentTemplate.RepeatingTemplate("ModelObjectDefinitionTemplate");
            ChildModelObjectInstancesCP.TemplateForCode = ContentTemplate.RepeatingTemplate("ChildModelObjectInstancesTemplate");

            foreach (Entities.ModelObject ModelObject in genericCollection)
            {
                if (!alreadyDefinedModelObjects.Contains(ModelObject.ModelObjectEntity.ProgramId))
                {
                    ModelObjectDefinition codeGen = new ModelObjectDefinition(ModelObject, contractName);
                    sb.Append(codeGen.GenerateContent());
                    alreadyDefinedModelObjects.Add(ModelObject.ModelObjectEntity.ProgramId);
                }
                BuildModelObjectDefinition(sb, ModelObject.ModelObjects, contractName);
            }            
        }

        internal class ModelObjectDefinition : ContentProvider
        {
            internal static Template TemplateForCode;

            Entities.ModelObject ModelObjectForGeneration;            
            string _contractName;
            internal ModelObjectDefinition(Entities.ModelObject ModelObject, string contractName)
            {
                ContentTemplate = TemplateForCode;
                ModelObjectForGeneration = ModelObject;
                _contractName = contractName;
            }
            [PlaceHolder("ModelObjectName")]
            string ModelObjectName
            {
                get
                {
                    return ModelObjectForGeneration.Name;
                }
            }
            [PlaceHolder("ModelObjectEntityName")]
            string ModelObjectEntityName
            {
                get
                {                    
                    //TINS
                    if (CodeGeneratorBase.IsTins)
                    {
                        return ModelObjectForGeneration.ModelObjectEntity.EntityName;
                    }
                    //Cobol
                    else
                    {
                        return ModelObjectForGeneration.ModelObjectEntity.DataEntityClassName;
                    }
                }
            }
            [PlaceHolder("ModelObjectEntityId")]
            string ModelObjectEntityId
            {
                get
                {
                    return ModelObjectForGeneration.ModelObjectEntity.ProgramId;
                }
            }

            [PlaceHolder("ChildModelObjectInstancesFilled")]
            string ChildModelObjectInstancesFilled
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    BuildChildModelObjectInstances(sb);
                    return sb.ToString();
                }
            }

            private void BuildChildModelObjectInstances(StringBuilder sb)
            {
                string keyForMapping = _contractName + ":" + ModelObjectForGeneration.ModelObjectEntity.ProgramId;
                Entities.GenericCollection<string> listOfChildModelObjects = (Entities.GenericCollection<string>)Mappings.contractModelObjectMapping[keyForMapping];
                if (listOfChildModelObjects != null)
                {
                    foreach (string str in listOfChildModelObjects)
                    {
                        ChildModelObjectInstancesCP codeGen = new ChildModelObjectInstancesCP(str, str.Substring(0, 4));
                        sb.Append(codeGen.GenerateContent());
                    }
                }
            }            
        }

        internal class ChildModelObjectInstancesCP : ContentProvider
        {
            internal static Template TemplateForCode;
            string _ModelObjectName;
            string _ProgramId;
            internal ChildModelObjectInstancesCP(string ModelObjectName, string programId)
            {
                ContentTemplate = TemplateForCode;
                _ModelObjectName = ModelObjectName;
                _ProgramId = programId;
            }

            [PlaceHolder("ModelObjectName")]
            string ModelObjectName
            {
                get
                {
                    return _ModelObjectName;
                }
            }

            [PlaceHolder("ModelObjectProgramId")]
            string ModelObjectProgramId
            {
                get
                {
                    return _ProgramId;
                }
            }
        }
        internal class ImportTemplateCP : ContentProvider
        {
            internal string TemplateName = "ImportTemplate";
            internal static Template TemplateForCode;
            Entities.ModelObject objectForCodeGeneration;

            internal ImportTemplateCP(Entities.ModelObject ModelObject)
            {
                ContentTemplate = TemplateForCode;
                objectForCodeGeneration = ModelObject;
            }
            [PlaceHolder("ProgramId")]
            string ProgramId
            {
                get
                {
                    return objectForCodeGeneration.ModelObjectEntity.ProgramId;
                }
            }
            [PlaceHolder("ModuleName")]
            string ModuleName
            {
                get
                {
                    return ((ModelObjectDetailsMapping)ContractDataEntityCP.ModelObjectDetailsMapping[objectForCodeGeneration.ModelObjectEntity.ProgramId]).ModuleName;
                }
            }
            [PlaceHolder("ModelObjectEntityName")]
            string ModelObjectName
            {
                get
                {
                    return objectForCodeGeneration.Name;
                }
            }

            [PlaceHolder("ModelObjectXmlNameSpace")]
            string ModelObjectXmlNameSpace
            {
                get
                {
                    //Tins
                    if (CodeGeneratorBase.IsTins)
                    {
                        return Mappings.XsdNameSpaceMappings["ModelObject:"
                        + ((ModelObjectDetailsMapping)ContractDataEntityCP.ModelObjectDetailsMapping[objectForCodeGeneration.ModelObjectEntity.ProgramId]).ModuleName
                        + ":" + objectForCodeGeneration.Name.Substring(0, 4)].ToString();
                    }

                    //Cobol
                    else
                    {
                        //add Prog ID in XSD namespace mapping
                        return Mappings.XsdNameSpaceMappings["ModelObject:"
                        + ((ModelObjectDetailsMapping)ContractDataEntityCP.ModelObjectDetailsMapping[objectForCodeGeneration.ModelObjectEntity.ProgramId]).ModuleName
                        + ":" + objectForCodeGeneration.ModelObjectEntity.ProgramId].ToString();
                    }
                }
            }
        }
    }
}
