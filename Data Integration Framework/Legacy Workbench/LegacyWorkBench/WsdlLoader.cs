using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using System.Web.Services.Description;
using Infosys.Lif.LegacyWorkbench;
using Infosys.Lif.LegacyWorkbench.Entities;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Collections;

namespace Infosys.Lif.LegacyWorkbench
{
    /// <summary>
    /// This class implements IStorage class and provides method to load and a wsdl file.
    /// </summary>
    class WsdlLoader : Framework.IStorage
    {
        #region IStorage Members

        /// <summary>
        /// This method is not implemented
        /// </summary>
        /// <param name="projectToBeSaved"> the project details whih are to be saved </param>
        /// <returns> true / false </returns>
        public bool Save(Infosys.Lif.LegacyWorkbench.Entities.Project projectToBeSaved)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// This method allows to load wsdl file in legacy workbench and model its data entities and contracts.
        /// </summary>
        /// <returns> project with details loaded from the wsdl file </returns>
        public Infosys.Lif.LegacyWorkbench.Entities.Project Load()
        {            
            WsdlLoadForm wsdlForm = new WsdlLoadForm();
            wsdlForm.ShowDialog();
            if (wsdlForm.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                Entities.Project projectLoaded = new Project();

                // retrieve data from wsdl
                ServiceDescription parsedWsdl = wsdlForm.WsdlDetails;

                #region wsdl loader old code
                ////// add contract module and contract info
                ////Entities.ContractModule contModuleToAdd = new ContractModule();
                ////contModuleToAdd.Name = "Ungrouped";
                ////contModuleToAdd.DataEntityNamespace = string.Empty;
                ////contModuleToAdd.SerializerNamespace = string.Empty;
                ////projectLoaded.ContractModules.Add(contModuleToAdd);

                ////#region contract init commented
                //////Entities.Contract contractToAdd = new Contract();
                ////////***** give operation name to the contract name.
                //////contractToAdd.ContractName = "Contract";
                //////contractToAdd.ContractDescription = string.Empty;
                //////contractToAdd.Feed_Title = string.Empty;
                //////contractToAdd.IsToBeGenerated = true;
                //////contractToAdd.MethodType = Contract.ContractMethodType.Select;
                //////contractToAdd.Notes = string.Empty;
                //////contractToAdd.TransactionId = string.Empty;

                //////get service details
                //////System.Web.Services.Description.ServiceCollection serviceColl = parsedWsdl.Services;
                //////contractToAdd.ServiceName = serviceColl[0].Name;
                ////#endregion

                ////// add model obj module
                ////Entities.ModelObjectModule modelObjModule = new ModelObjectModule();
                ////modelObjModule.Name = "Ungrouped";
                ////modelObjModule.DataEntityNamespace = string.Empty;
                ////modelObjModule.SerializerNamespace = string.Empty;

                ////projectLoaded.ModelObjectModules.Add(modelObjModule);

                ////XmlSchemas schemas = parsedWsdl.Types.Schemas;
                ////foreach (XmlSchema xmlSchemaInWsdl in schemas)
                ////{
                ////    XmlSchemaObjectCollection schemaObjCollection = xmlSchemaInWsdl.Items;
                ////    for (int schemaObjCount = 0; schemaObjCount < schemaObjCollection.Count; schemaObjCount++)
                ////    {
                ////        string schemaType = schemaObjCollection[schemaObjCount].GetType().Name;

                ////        if (schemaType.ToLower().Equals("xmlschemaelement"))
                ////        {
                ////            XmlSchemaElement xmlElement = new XmlSchemaElement();
                ////            xmlElement = (XmlSchemaElement)schemaObjCollection[schemaObjCount];

                ////            if (xmlElement.SchemaType.ToString() == "System.Xml.Schema.XmlSchemaSimpleType")
                ////            {
                ////                Entities.Entity modelObjEntity = new Entity();
                ////                modelObjEntity.EntityName = xmlElement.Name;
                ////                modelObjEntity.DataEntityClassName = string.Empty;
                ////                modelObjEntity.IsToBeGenerated = true;
                ////                modelObjEntity.Notes = string.Empty;
                ////                modelObjEntity.ProgramId = xmlElement.Name;
                ////                modelObjEntity.ReferenceID = string.Empty;
                ////                modelObjEntity.SerializerClassName = string.Empty;

                ////                //add entity
                ////                projectLoaded.ModelObjectModules[0].ModelObjects.Add(modelObjEntity);
                ////            }
                ////            #region commented code
                ////            //else
                ////            //{
                ////            //    XmlSchemaComplexType xmlComplexType = new XmlSchemaComplexType();
                ////            //    xmlComplexType = (XmlSchemaComplexType)xmlElement.SchemaType;
                ////            //    XmlSchemaSequence xmlSequence = new XmlSchemaSequence();
                ////            //    xmlSequence = (XmlSchemaSequence)xmlComplexType.Particle;
                ////            //    XmlSchemaObjectCollection schemaColl = xmlSequence.Items;
                ////            //    foreach (XmlSchemaElement schemaElement in schemaColl)
                ////            //    {
                ////            //        //textBox1.AppendText("\t" + "elements : " + schemaElement.Name + "\r\n");
                ////            //        //textBox1.AppendText("\t" + "Elem Type : " + schemaElement.SchemaTypeName.Name + "\r\n");
                ////            //        //Entities.ModelObject modelObj = new ModelObject();
                ////            //        //modelObj.Name = schemaElement.Name;

                ////            //    }
                ////            //}
                ////            #endregion
                ////        }

                ////        if (schemaType.ToLower().Equals("xmlschemacomplextype"))
                ////        {
                ////            XmlSchemaComplexType xmlComplexType = new XmlSchemaComplexType();
                ////            xmlComplexType = (XmlSchemaComplexType)schemaObjCollection[schemaObjCount];

                ////            //textBox2.AppendText("Type : " + element.Name + "\r\n");
                ////            Entities.Entity modelObjEntity = new Entity();
                ////            modelObjEntity.EntityName = xmlComplexType.Name;
                ////            modelObjEntity.DataEntityClassName = string.Empty;
                ////            modelObjEntity.IsToBeGenerated = true;
                ////            modelObjEntity.Notes = string.Empty;
                ////            modelObjEntity.ProgramId = xmlComplexType.Name;
                ////            modelObjEntity.ReferenceID = string.Empty;
                ////            modelObjEntity.SerializerClassName = string.Empty;                        

                ////            XmlSchemaSequence xmlSequence = new XmlSchemaSequence();
                ////            if (xmlComplexType.Particle != null)
                ////            {
                ////                xmlSequence = (XmlSchemaSequence)xmlComplexType.Particle;
                ////                XmlSchemaObjectCollection objCollection = xmlSequence.Items;
                ////                foreach (XmlSchemaElement schemaElement in objCollection)
                ////                {
                ////                    //textBox2.AppendText("\t" + "elements : " + elem.Name + "\r\n");
                ////                    //textBox2.AppendText("\t" + "Elem Type : " + elem.SchemaTypeName.Name + "\r\n");
                ////                    Entities.DataItem modelObjDataItem = new DataItem();
                ////                    modelObjDataItem.ItemName = schemaElement.Name;
                ////                    modelObjDataItem.Direction = DataItem.ParameterDirectionType.InputAndOutput;
                ////                    modelObjDataItem.Length = 0;
                ////                    modelObjDataItem.IsVisible = true;
                ////                    modelObjDataItem.NumberOfOccurences = 1;
                ////                    modelObjDataItem.Value = string.Empty;
                ////                    switch (schemaElement.SchemaTypeName.Name)
                ////                    {
                ////                        case "String":
                ////                            modelObjDataItem.ItemType = DataItem.DataItemType.String;
                ////                            break;
                ////                        case "Integer":
                ////                            modelObjDataItem.ItemType = DataItem.DataItemType.Integer;
                ////                            break;
                ////                        case "Boolean":
                ////                            modelObjDataItem.ItemType = DataItem.DataItemType.Boolean;
                ////                            break;
                ////                        case "Float":
                ////                            modelObjDataItem.ItemType = DataItem.DataItemType.Float;
                ////                            break;
                ////                        case "Enum":
                ////                            modelObjDataItem.ItemType = DataItem.DataItemType.Enum; 
                ////                            break;
                ////                        case "Date":
                ////                            modelObjDataItem.ItemType = DataItem.DataItemType.Date;
                ////                            break;

                ////                        default:
                ////                            modelObjDataItem.ItemType = DataItem.DataItemType.String;
                ////                            break;                                            
                ////                    }

                ////                    modelObjEntity.DataItems.Add(modelObjDataItem);
                ////                }

                ////                projectLoaded.ModelObjectModules[0].ModelObjects.Add(modelObjEntity);
                ////            }

                ////            if (xmlComplexType.ContentModel != null)
                ////            {
                ////                XmlSchemaContentModel schemaContentModel = xmlComplexType.ContentModel;
                ////                XmlSchemaContent schContent = schemaContentModel.Content;
                ////                XmlSchemaComplexContentExtension contentExtension = (XmlSchemaComplexContentExtension)schemaContentModel.Content;
                ////                XmlQualifiedName qualifiedName = new XmlQualifiedName();
                ////                qualifiedName = contentExtension.BaseTypeName;                            
                ////                Entities.DataItem modelObjDataItem = new DataItem();
                ////                modelObjDataItem.ItemName = qualifiedName.Name;
                ////                modelObjDataItem.Direction = DataItem.ParameterDirectionType.InputAndOutput;
                ////                modelObjDataItem.Length = 0;
                ////                modelObjDataItem.IsVisible = true;
                ////                modelObjDataItem.NumberOfOccurences = 1;
                ////                modelObjDataItem.Value = string.Empty;
                ////                switch (qualifiedName.Namespace)
                ////                {
                ////                    case "String":
                ////                        modelObjDataItem.ItemType = DataItem.DataItemType.String;
                ////                        break;
                ////                    case "Integer":
                ////                        modelObjDataItem.ItemType = DataItem.DataItemType.Integer;
                ////                        break;
                ////                    case "Boolean":
                ////                        modelObjDataItem.ItemType = DataItem.DataItemType.Boolean;
                ////                        break;
                ////                    case "Float":
                ////                        modelObjDataItem.ItemType = DataItem.DataItemType.Float;
                ////                        break;
                ////                    case "Enum":
                ////                        modelObjDataItem.ItemType = DataItem.DataItemType.Enum;
                ////                        break;
                ////                    case "Date":
                ////                        modelObjDataItem.ItemType = DataItem.DataItemType.Date;
                ////                        break;
                ////                    default:
                ////                        modelObjDataItem.ItemType = DataItem.DataItemType.String;
                ////                        break;                             
                ////                }

                ////                modelObjEntity.DataItems.Add(modelObjDataItem);
                ////                projectLoaded.ModelObjectModules[0].ModelObjects.Add(modelObjEntity);
                ////            }                                               
                ////        }
                ////    }
                ////}

                ////// get ports and operation level details
                ////System.Web.Services.Description.PortTypeCollection ports = parsedWsdl.PortTypes;

                ////for (int portsCount = 0; portsCount < ports.Count; portsCount++)
                ////{
                ////    System.Web.Services.Description.OperationCollection operation = ports[portsCount].Operations;

                ////    for (int operationsCount = 0; operationsCount < operation.Count; operationsCount++)
                ////    {                    
                ////        Entities.Contract contract = new Contract();

                ////        // initialize contract 
                ////        contract.ContractName = operation[operationsCount].Name;
                ////        contract.ContractDescription = string.Empty;
                ////        contract.Feed_Title = string.Empty;
                ////        contract.IsToBeGenerated = true;
                ////        contract.MethodType = Contract.ContractMethodType.Select;
                ////        contract.Notes = string.Empty;
                ////        contract.TransactionId = string.Empty;

                ////        // add service name
                ////        System.Web.Services.Description.ServiceCollection serviceColl = parsedWsdl.Services;
                ////        contract.ServiceName = serviceColl[0].Name;

                ////        contract.MethodName = operation[operationsCount].Name;

                ////        // input and output model obj of contract
                ////        Entities.ModelObject inputModelObj = new ModelObject();
                ////        inputModelObj.Name = "InputModelObject";
                ////        inputModelObj.HostName = inputModelObj.Name;
                ////        inputModelObj.MaxCount = 1;
                ////        inputModelObj.MinCount = 0;

                ////        Entities.ModelObject outputModelObj = new ModelObject();
                ////        outputModelObj.Name = "OutputModelObject";
                ////        outputModelObj.HostName = outputModelObj.Name;
                ////        outputModelObj.MaxCount = 1;
                ////        outputModelObj.MinCount = 0;

                ////        contract.InputModelObjects = new GenericCollection<ModelObject>();
                ////        contract.InputModelObjects.Add(inputModelObj);
                ////        contract.OutputModelObjects = new GenericCollection<ModelObject>();
                ////        contract.OutputModelObjects.Add(outputModelObj);                    

                ////        projectLoaded.ContractModules[0].Contracts.Add(contract);

                ////        #region commented code
                ////        //System.Web.Services.Description.OperationMessageCollection message = operation[operationsCount].Messages;
                ////        //for (int m = 0; m < message.Count; m++)
                ////        //{
                ////        //    System.Web.Services.Description.MessageCollection msgColl = parsedWsdl.Messages;

                ////        //    System.Web.Services.Description.Message msg = msgColl[message[m].Message.Name];
                ////        //    for (int pts = 0; pts < msg.Parts.Count; pts++)
                ////        //    {
                ////        //        System.Web.Services.Description.MessagePart parts = msg.Parts[pts];
                ////        //        textBox1.AppendText("PartName : " + parts.Name + "\r\n");
                ////        //        textBox1.AppendText("PartElem : " + parts.Element + "\r\n");
                ////        //        textBox1.AppendText("PartType : " + parts.Type + "\r\n");
                ////        //    }
                ////        //}
                ////        #endregion
                ////    }
                ////}
                #endregion

                // add contract module
                Entities.ContractModule contModuleToAdd = new ContractModule();
                contModuleToAdd.Name = "Ungrouped";
                contModuleToAdd.DataEntityNamespace = string.Empty;
                contModuleToAdd.SerializerNamespace = string.Empty;
                projectLoaded.ContractModules.Add(contModuleToAdd);

                // add model object module
                Entities.ModelObjectModule modelObjModule = new ModelObjectModule();
                modelObjModule.Name = "Ungrouped";
                modelObjModule.DataEntityNamespace = string.Empty;
                modelObjModule.SerializerNamespace = string.Empty;
                projectLoaded.ModelObjectModules.Add(modelObjModule);

                // create hashtable of messages in wsdl
                MessageCollection messageColl = parsedWsdl.Messages;

                Hashtable messageTable = new Hashtable(messageColl.Count);
                for (int msgCount = 0; msgCount < messageColl.Count; msgCount++)
                {
                    messageTable.Add(messageColl[msgCount].Name, messageColl[msgCount]);
                }

                // create hashtable of types in wsdl            
                Hashtable typeTable = new Hashtable();
                Hashtable complexTypeTable = new Hashtable();
                XmlSchemas schemas = parsedWsdl.Types.Schemas;
                for (int schemaCount = 0; schemaCount < schemas.Count; schemaCount++)
                {
                    for (int itemCount = 0; itemCount < schemas[schemaCount].Items.Count; itemCount++)
                    {
                        // dont add types other than complex types in the type list.
                        if (schemas[schemaCount].Items[itemCount].GetType().Name.ToLower() == "xmlschemaelement")
                        {
                            XmlSchemaElement xmlElement = (XmlSchemaElement)schemas[schemaCount].Items[itemCount];
                            typeTable.Add(xmlElement.Name, xmlElement);
                        }
                        else if (schemas[schemaCount].Items[itemCount].GetType().Name.ToLower() == "xmlschemacomplextype")
                        {
                            XmlSchemaComplexType complexType = (XmlSchemaComplexType)schemas[schemaCount].Items[itemCount];
                            complexTypeTable.Add(complexType.Name, complexType);

                            // add model object
                            Entities.Entity modelObjEntity = new Entity();
                            modelObjEntity.EntityName = complexType.Name;
                            modelObjEntity.DataEntityClassName = string.Empty;
                            modelObjEntity.IsToBeGenerated = true;
                            modelObjEntity.Notes = string.Empty;
                            modelObjEntity.ProgramId = complexType.Name;
                            modelObjEntity.ReferenceID = string.Empty;
                            modelObjEntity.SerializerClassName = string.Empty;

                            // add model object in module
                            projectLoaded.ModelObjectModules[0].ModelObjects.Add(modelObjEntity);

                            // get the elements of complex type & add the type into model objects                         
                            if (complexType.Particle != null)
                            {
                                XmlSchemaSequence xmlSequence = (XmlSchemaSequence)complexType.Particle;
                                for (int elemItemCount = 0; elemItemCount < xmlSequence.Items.Count; elemItemCount++)
                                {
                                    XmlSchemaElement schemaElement = (XmlSchemaElement)xmlSequence.Items[elemItemCount];

                                    Entities.DataItem modelObjDataItem = new DataItem();
                                    modelObjDataItem.ItemName = schemaElement.Name;
                                    modelObjDataItem.Direction = DataItem.ParameterDirectionType.InputAndOutput;
                                    modelObjDataItem.Length = 0;
                                    modelObjDataItem.IsVisible = true;
                                    modelObjDataItem.NumberOfOccurences = 1;
                                    modelObjDataItem.Value = string.Empty;
                                    modelObjDataItem.ItemType = this.GetDataItemType(schemaElement.SchemaTypeName.Name);

                                    modelObjEntity.DataItems.Add(modelObjDataItem);
                                }
                            }
                        }
                    }
                }

                // get operation details and its input/output message
                PortTypeCollection ports = parsedWsdl.PortTypes;
                for (int portsCount = 0; portsCount < ports.Count; portsCount++)
                {
                    OperationCollection operation = ports[portsCount].Operations;

                    for (int operCount = 0; operCount < operation.Count; operCount++)
                    {
                        // create a contract for each operation
                        Entities.Contract contract = new Contract();

                        // initialize contract 
                        contract.ContractName = operation[operCount].Name;
                        contract.ContractDescription = string.Empty;
                        contract.Feed_Title = string.Empty;
                        contract.IsToBeGenerated = true;
                        contract.MethodName = operation[operCount].Name;
                        contract.MethodType = Contract.ContractMethodType.Select;
                        contract.Notes = string.Empty;
                        contract.TransactionId = string.Empty;

                        // add service name
                        System.Web.Services.Description.ServiceCollection serviceColl = parsedWsdl.Services;
                        contract.ServiceName = serviceColl[0].Name;

                        OperationMessageCollection operMessages = operation[operCount].Messages;

                        // check input/output message                    
                        if (messageTable.Contains(operMessages.Input.Message.Name))
                        {
                            // add input message to operation input                        
                            Entities.ModelObject inputModelObj = new ModelObject();
                            inputModelObj.Name = operMessages.Input.Message.Name;
                            inputModelObj.HostName = inputModelObj.Name;
                            inputModelObj.MaxCount = 1;
                            inputModelObj.MinCount = 0;

                            contract.InputModelObjects = new GenericCollection<ModelObject>();
                            contract.InputModelObjects.Add(inputModelObj);

                            Entities.Entity modelObject =
                                this.GetMessageDetails((Message)messageTable[operMessages.Input.Message.Name], typeTable, complexTypeTable);

                            if (modelObject != null)
                            {
                                projectLoaded.ModelObjectModules[0].ModelObjects.Add(modelObject);
                            }
                        }

                        if (messageTable.Contains(operMessages.Output.Message.Name))
                        {
                            // add Output message to operation Output
                            Entities.ModelObject outputModelObj = new ModelObject();
                            outputModelObj.Name = operMessages.Output.Message.Name;
                            outputModelObj.HostName = outputModelObj.Name;
                            outputModelObj.MaxCount = 1;
                            outputModelObj.MinCount = 0;

                            contract.OutputModelObjects = new GenericCollection<ModelObject>();
                            contract.OutputModelObjects.Add(outputModelObj);

                            Entities.Entity modelObject =
                                this.GetMessageDetails((Message)messageTable[operMessages.Output.Message.Name], typeTable, complexTypeTable);

                            // if the element is of complex type dont add a model object for that
                            if (modelObject != null)
                            {
                                projectLoaded.ModelObjectModules[0].ModelObjects.Add(modelObject);
                            }
                        }

                        // add contract to its module
                        projectLoaded.ContractModules[0].Contracts.Add(contract);
                    }
                }

                // call method to fill project namespaces
                this.LoadNamespacesFromConfig(ref projectLoaded);

                return projectLoaded;
            }
            else
            {
                return null;
            }
        }

        #endregion

        /// <summary>
        /// This method retrieves the details of a message i.e. message name, type, elements.
        /// </summary>
        /// <param name="message"> input or output message </param>
        /// <param name="typeTable"> Hashtable of all types in wsdl </param>
        /// <param name="complexTypeTable"> hash table of all complex types in wsdl </param>
        /// <returns> the entity filled with message details </returns>
        private Entities.Entity GetMessageDetails(
            System.Web.Services.Description.Message message, 
            Hashtable typeTable,
            Hashtable complexTypeTable)
        {
            // create model object entity to wrap all parts of a message 
            Entities.Entity modelObjEntity = new Entity();
            modelObjEntity.EntityName = message.Name;
            modelObjEntity.DataEntityClassName = string.Empty;
            modelObjEntity.IsToBeGenerated = true;
            modelObjEntity.Notes = string.Empty;
            modelObjEntity.ProgramId = message.Name;
            modelObjEntity.ReferenceID = string.Empty;
            modelObjEntity.SerializerClassName = string.Empty;             

            for (int partsCount = 0; partsCount < message.Parts.Count; partsCount++)
            {
                string partName = message.Parts[partsCount].Name;
                
                // basic types parameters
                if (partName.ToLower() == "parameters")
                {
                    string partElement = message.Parts[partsCount].Element.Name;
                    
                    if (typeTable.Contains(partElement))
                    {
                        XmlSchemaElement xmlElement = (XmlSchemaElement)typeTable[partElement];
                        string schemaType = xmlElement.SchemaType.GetType().Name.ToString();

                        if (schemaType.ToLower().Equals("xmlschemacomplextype"))
                        {                             
                            XmlSchemaComplexType xmlComplexType = (XmlSchemaComplexType)xmlElement.SchemaType;                            
                            XmlSchemaSequence xmlSequence = new XmlSchemaSequence();
                            if (xmlComplexType.Particle != null)
                            {
                                xmlSequence = (XmlSchemaSequence)xmlComplexType.Particle;
                                XmlSchemaObjectCollection objCollection = xmlSequence.Items;                                

                                foreach (XmlSchemaElement schemaElement in objCollection)
                                {
                                    Entities.DataItem dataItem = new DataItem();
                                    dataItem.Direction = DataItem.ParameterDirectionType.InputAndOutput;
                                    dataItem.Length = 0;
                                    dataItem.IsVisible = true;
                                    dataItem.NumberOfOccurences = 1;
                                    dataItem.Value = string.Empty;
                                    dataItem.ItemName = schemaElement.Name;
                                    string itemType = schemaElement.SchemaTypeName.Name;

                                    if (complexTypeTable.Contains(itemType))
                                    {
                                        return null;
                                    }

                                    dataItem.ItemType = this.GetDataItemType(itemType);
                                    
                                    modelObjEntity.DataItems.Add(dataItem);                                    
                                }
                            }
                        }                        
                    }
                }
                else
                {                    
                    // if not a parameter but part type is mentioned
                    string partType = message.Parts[partsCount].Type.Name;                    
                    if (partType != null && partType != string.Empty)
                    {
                        Entities.DataItem dataItem = new DataItem();
                        dataItem.Direction = DataItem.ParameterDirectionType.InputAndOutput;
                        dataItem.Length = 0;
                        dataItem.IsVisible = true;
                        dataItem.NumberOfOccurences = 1;
                        dataItem.Value = string.Empty;
                        dataItem.ItemName = partName;                        

                        if (complexTypeTable.Contains(partType))
                        {
                            return null;
                        }

                        dataItem.ItemType = this.GetDataItemType(partType);

                        modelObjEntity.DataItems.Add(dataItem);
                    }
                    else
                    {
                        // for WSDL generated from Relativity tool
                        // ***** yet to test from the proper WSDL generated from tool.
                        string partElement = message.Parts[partsCount].Element.Name;
                        if (typeTable.Contains(partElement))
                        {
                            XmlSchemaElement xmlElement = (XmlSchemaElement)typeTable[partElement];
                            string schemaType = xmlElement.SchemaType.GetType().Name.ToString();

                            if (schemaType.ToLower().Equals("xmlschemacomplextype"))
                            {
                                XmlSchemaComplexType xmlComplexType = (XmlSchemaComplexType)xmlElement.SchemaType;
                                XmlSchemaSequence xmlSequence = new XmlSchemaSequence();
                                if (xmlComplexType.Particle != null)
                                {
                                    xmlSequence = (XmlSchemaSequence)xmlComplexType.Particle;
                                    XmlSchemaObjectCollection objCollection = xmlSequence.Items;

                                    Entities.DataItem dataItem = new DataItem();

                                    foreach (XmlSchemaElement schemaElement in objCollection)
                                    {
                                        dataItem.Direction = DataItem.ParameterDirectionType.InputAndOutput;
                                        dataItem.Length = 0;
                                        dataItem.IsVisible = true;
                                        dataItem.NumberOfOccurences = 1;
                                        dataItem.Value = string.Empty;
                                        dataItem.ItemName = schemaElement.Name;
                                        string itemType = schemaElement.SchemaTypeName.Name;

                                        if (complexTypeTable.Contains(itemType))
                                        {
                                            return null;
                                        }

                                        dataItem.ItemType = this.GetDataItemType(itemType);

                                        modelObjEntity.DataItems.Add(dataItem);
                                    }
                                }
                            }   
                        }
                    }
                }
            }

            return modelObjEntity;
        }

        /// <summary>
        /// This method retrieves proper data type for an item
        /// </summary>
        /// <param name="itemType"> item type of string type </param>
        /// <returns> corresponding DataItemType </returns>
        private Entities.DataItem.DataItemType GetDataItemType(string itemType)
        {
            Entities.DataItem.DataItemType dataItemType;
            switch (itemType.ToLower())
            {
                case "string":
                    dataItemType = DataItem.DataItemType.String;
                    break;
                case "int":
                    dataItemType = DataItem.DataItemType.Integer;
                    break;
                case "boolean":
                    dataItemType = DataItem.DataItemType.Boolean;
                    break;
                case "float":
                    dataItemType = DataItem.DataItemType.Float;
                    break;
                case "date":
                    dataItemType = DataItem.DataItemType.Date;
                    break;
                case "datetime":
                    dataItemType = DataItem.DataItemType.Date;
                    break;

                default:
                    dataItemType = DataItem.DataItemType.String;
                    break;
            }

            return dataItemType;
        }

        /// <summary>
        /// This method loads all the namespaces from config file into the project details
        /// </summary>
        /// <param name="projectToLoad"> the project where namespace details to be added </param>
        private void LoadNamespacesFromConfig(ref Entities.Project projectToLoad)
        {
            // retrieve info from config file
            LegacyWorkbenchConfigurations.LegacyWorkbenchConfigurations legacyWorkbenchSettings;
            legacyWorkbenchSettings = (LegacyWorkbenchConfigurations.LegacyWorkbenchConfigurations)
                ConfigurationManager.GetSection("LegacyWorkbenchConfigurations");
            LegacyWorkbenchConfigurations.CodeGeneratedNamespaceDefinition projNamespaces =
                legacyWorkbenchSettings.CodeGeneratedNamespaces;

            // fill contract namespaces
            projectToLoad.ContractNamespaces.DataEntityNamespace = projNamespaces.ContractNamespaces.DataEntityNamespace;
            projectToLoad.ContractNamespaces.DataEntityRootNamespace = projNamespaces.ContractNamespaces.DataEntityRootNamespace;
            projectToLoad.ContractNamespaces.HostAccessNamespace = projNamespaces.ContractNamespaces.HostAccessNamespace;
            projectToLoad.ContractNamespaces.HostAccessRootNamespace = projNamespaces.ContractNamespaces.HostAccessRootNamespace;
            projectToLoad.ContractNamespaces.SerializerNamespace = projNamespaces.ContractNamespaces.SerializerNamespace;
            projectToLoad.ContractNamespaces.SerializerRootNamespace = projNamespaces.ContractNamespaces.SerializerRootNamespace;
            projectToLoad.ContractNamespaces.XmlSchemaNamespace = this.FormatXmlNamespace(projNamespaces.ContractNamespaces.XmlNamespace);
                        
            // fill model object namespaces
            projectToLoad.ModelObjectNamespaces.DataEntityNamespace = projNamespaces.ModelObjectNamespaces.DataEntityNamespace;
            projectToLoad.ModelObjectNamespaces.DataEntityRootNamespace = projNamespaces.ModelObjectNamespaces.DataEntityRootNamespace;
            projectToLoad.ModelObjectNamespaces.SerializerNamespace = projNamespaces.ModelObjectNamespaces.SerializerNamespace;
            projectToLoad.ModelObjectNamespaces.SerializerRootNamespace = projNamespaces.ModelObjectNamespaces.SerializerRootNamespace;
            projectToLoad.ModelObjectNamespaces.XmlSchemaNamespace = this.FormatXmlNamespace(projNamespaces.ModelObjectNamespaces.XmlNamespace);
        }

        /// <summary>
        /// This method formats the model object/contract xml namespace in proper format
        /// </summary>
        /// <param name="xmlNamespace"> xml namespace </param>
        /// <returns> formatted xml namespace </returns>
        private string FormatXmlNamespace(string xmlNamespace)
        {
            if (xmlNamespace.Contains("CurrentYear"))
            {
                xmlNamespace = xmlNamespace.Replace("CurrentYear", DateTime.Now.Year.ToString());
            }

            if (xmlNamespace.Contains("CurrentMonth"))
            {
                string month = DateTime.Now.Month.ToString();
                if (month.Length == 1)
                {
                    month = "0" + month;
                }

                xmlNamespace = xmlNamespace.Replace("CurrentMonth", month);
            }

            return xmlNamespace;
        }
    }    
}

