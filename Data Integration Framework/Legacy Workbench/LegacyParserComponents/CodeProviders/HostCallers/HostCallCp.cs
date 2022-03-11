using System;
using System.Collections.Generic;
using System.Text;

using Infosys.Solutions.CodeGeneration.Framework;

namespace Infosys.Lif.LegacyWorkbench.CodeProviders.HostCallers
{
    internal class ContractModuleMapping
    {
        Entities.Contract contract;

        public Entities.Contract Contract
        {
            get { return contract; }
            set { contract = value; }
        }
        Entities.ContractModule module;

        public Entities.ContractModule Module
        {
            get { return module; }
            set { module = value; }
        }
    }

    internal class HostCallCp : ContentProvider
    {
        Entities.GenericCollection<ContractModuleMapping> contractsMapping;

        string hostNameSpace;

        //code to make projectPrefix as empty string instead of null       
        string projPrefix = string.Empty;

        internal HostCallCp(Entities.GenericCollection<ContractModuleMapping> contractModuleMappings,
            Template contentTemplate, string _hostNameSpace, string _projPrefix)
        {
            contractsMapping = contractModuleMappings;
            ContentTemplate = contentTemplate;
            hostNameSpace = _hostNameSpace;
            projPrefix = _projPrefix;
        }


        internal HostCallCp(Entities.GenericCollection<ContractModuleMapping> contractModuleMappings,
            Template contentTemplate, string _hostNameSpace)
        {
            contractsMapping = contractModuleMappings;
            ContentTemplate = contentTemplate;
            hostNameSpace = _hostNameSpace;            
        }


        [PlaceHolder("ImportsFilled")]
        string ImportsFilled
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (ContractModuleMapping contractMapping in contractsMapping)
                {
                    NameSpacesImportCp codeProvider = new NameSpacesImportCp();
                    codeProvider.ContentTemplate = ContentTemplate.RepeatingTemplate("NameSpacesImport");
                    codeProvider.ContractEntityNamespace = Framework.Helper.BuildNamespace(
                        contractMapping.Module.DataEntityNamespace, contractMapping.Contract);
                    codeProvider.ContractEntityName = contractMapping.Contract.ContractName;
                    sb.Append(codeProvider.GenerateContent());                    
                }
                return sb.ToString();
            }
        }        
             
   
        [PlaceHolder("HostAccessNameSpace")]
        string HostAccessNameSpace
        {
            get
            {
                return hostNameSpace;
            }
        }
        

        [PlaceHolder("DataContractNamespace")]
        string DataContractNamespace
        {
            get
            {
                if (projPrefix == null || projPrefix == string.Empty)
                {
                    return "LegacyDataContract";
                }
                else
                {
                    return projPrefix.Trim() + "LegacyDataContract"; 
                }                
            }
        }
        
        
        [PlaceHolder("ServiceContractNamespace")]
        string ServiceContractNamespace
        {
            get
            {
                if (projPrefix == null || projPrefix == string.Empty)
                {
                    return hostNameSpace + ".ServiceContract";
                }
                else
                {
                    return projPrefix.Trim() + hostNameSpace + ".ServiceContract"; 
                }                
            }
        }


        [PlaceHolder("ServiceImplementationNamespace")]
        string ServiceImplementationNamespace
        {
            get
            {
                if (projPrefix == null || projPrefix == string.Empty)
                {
                    return hostNameSpace + ".ServiceImplementation";
                }
                else
                {
                    return projPrefix.Trim() + hostNameSpace + ".ServiceImplementation"; 
                }

            }
        }

                
        [PlaceHolder("ContractName")]
        string ContractName
        {
            get
            {
                //to use full contract name in generated code
                return contractsMapping[0].Contract.ContractName;//.Substring(0, 4);
            }
        }


        class NameSpacesImportCp : ContentProvider
        {
            string _contractEntityNamespace;
            [PlaceHolder("ContractEntityNamespace")]
            internal string ContractEntityNamespace
            {
                get
                {
                    return _contractEntityNamespace;
                }
                set
                {
                    _contractEntityNamespace = value;
                }
            }


            string _contractEntityName;
            [PlaceHolder("ContractEntityName")]
            internal string ContractEntityName
            {
                get
                {
                    return _contractEntityName;
                }
                set
                {
                    _contractEntityName = value;
                }
            }
        }
        

        [PlaceHolder("MethodBodies")]
        string MethodBodies
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                foreach (ContractModuleMapping contractMapping in contractsMapping)
                {
                    string templateName = "MethodTemplate";
                    switch (contractMapping.Contract.MethodType)
                    {
                        case Entities.Contract.ContractMethodType.Insert:

                            if (contractMapping.Contract.MethodName == null
                                || contractMapping.Contract.MethodName.Length == 0)
                            {
                                contractMapping.Contract.MethodName = "Insert";
                            }
                            break;

                        case Entities.Contract.ContractMethodType.Update:
                            
                            if (contractMapping.Contract.MethodName == null
                                || contractMapping.Contract.MethodName.Length == 0)
                            {
                                contractMapping.Contract.MethodName = "Update";
                            }
                            break;

                        case Entities.Contract.ContractMethodType.Delete:
                            if (contractMapping.Contract.MethodName == null
                                || contractMapping.Contract.MethodName.Length == 0)
                            {
                                contractMapping.Contract.MethodName = "Delete";
                            }
                            break;

                        default:
                            if (contractMapping.Contract.MethodName == null
                                || contractMapping.Contract.MethodName.Length == 0)
                            {
                                contractMapping.Contract.MethodName = "Select";
                            }
                            break;
                    }

                    Template template = ContentTemplate.RepeatingTemplate(templateName);

                    MethodTemplateCp codeProvider = new MethodTemplateCp(contractMapping.Module, 
                        contractMapping.Contract, template, projPrefix, hostNameSpace);

                    sb.Append(codeProvider.GenerateContent());
                }

                return sb.ToString();
            }
        }


        class MethodTemplateCp : ContentProvider
        {
            Entities.Contract contractForGeneration;
            Entities.ContractModule moduleForGeneration;
            string projectPrefix;
            string hostNamespace;
            internal MethodTemplateCp(Entities.ContractModule contractModule, Entities.Contract contract, 
                Template contentTemplate, string projPrefix, string hostAccessNamespace)
            {
                this.ContentTemplate = contentTemplate;
                this.contractForGeneration = contract;
                this.moduleForGeneration = contractModule;
                this.projectPrefix = projPrefix;
                this.hostNamespace = hostAccessNamespace;
            }


            [PlaceHolder("ServiceContractNamespace")]
            string ServiceContractNamespace
            {
                get
                {
                    if (projectPrefix == null || projectPrefix == string.Empty)
                    {
                        return hostNamespace + ".ServiceContract";
                    }
                    else
                    {
                        return projectPrefix.Trim() + hostNamespace + ".ServiceContract"; 
                    }
                }
            }


            [PlaceHolder("ServiceImplementationNamespace")]
            string ServiceImplementationNamespace
            {
                get
                {
                    if (projectPrefix == null || projectPrefix == string.Empty)
                    {
                        return hostNamespace + ".ServiceImplementation"; 
                    }
                    else
                    {
                        return projectPrefix.Trim() + hostNamespace + ".ServiceImplementation";
                    }
                }
            }


            [PlaceHolder("ServiceName")]
            string ServiceName
            {
                get
                {
                    if (contractForGeneration.ServiceName == null || contractForGeneration.ServiceName.Length == 0)
                    {
                        return contractForGeneration.ContractName;
                    }
                    else
                    {
                        return contractForGeneration.ServiceName;
                    }
                }
            }


            [PlaceHolder("TransacID")]
            string TransacID
            {
                get
                {
                    if (contractForGeneration.TransactionId == null || contractForGeneration.TransactionId.Length == 0)
                    {
                        return (contractForGeneration.ContractName.Length>4 ? 
                            contractForGeneration.ContractName.Substring(0, 4) : contractForGeneration.ContractName);
                    }
                    else
                    {
                        return (contractForGeneration.TransactionId.Length>4 ? 
                            contractForGeneration.TransactionId.Substring(0, 4) : contractForGeneration.TransactionId);
                    }
                }
            }


            [PlaceHolder("OperType")]
            string OperType
            {
                get
                {
                    if (contractForGeneration.MethodType.ToString().Length == 0)
                    {
                        return contractForGeneration.ContractName;
                    }
                    else
                    {
                        return contractForGeneration.MethodType.ToString().Substring(0,1);
                    }
                }
            }


            [PlaceHolder("ContractEntityName")]
            string ContractEntityName
            {
                get
                {
                    return contractForGeneration.ContractName;
                }
            }
            

            [PlaceHolder("MethodName")]
            string MethodName
            {
                get
                {
                    return contractForGeneration.MethodName;
                }
            }


            [PlaceHolder("ContractName")]
            string ContractName
            {
                get
                {
                    //use full contract name
                    return contractForGeneration.ContractName;//.Substring(0,4);                     
                }
            }

            //place holders for RSS
            [PlaceHolder("TitleValue")]
            string TitleValue
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    //to check the contract position inside a contract module
                    int outer = 0;
                    for (outer = 0; outer < moduleForGeneration.Contracts.Count; outer++)
                    {
                        if (moduleForGeneration.Contracts[outer].ContractName == ContractName)
                        {
                            break;
                        }
                    }
                                    
                    //array to contain the text of RSS Item Feed
                    Array arrayObj = null;

                    //to fill the array if something is entered in the propertygrid(RSS Item Feed)
                    if (!(contractForGeneration.Feed_Title == null || contractForGeneration.Feed_Title == string.Empty))
                    {
                        arrayObj = contractForGeneration.Feed_Title.ToString().Split('%');                        
                    }

                    //new array containing the string within % sign                    
                    Array percentArray = null;

                    //fill the values from the 2nd loaction of arrayObj as this is the place where the 
                    //text withng % sign exists to fill the percentArray if arrayObj has values in the 2nd location
                    if (arrayObj != null && arrayObj.GetValue(1) != null)
                    {
                        percentArray = arrayObj.GetValue(1).ToString().Split('.');
                    }

                    //the percentArray is expected to have two counts; 
                    //the first count will have text before decimal and second will have text after decimal 
                    //it the structure of user's enetered sting does not match with the required one,
                    //the title of RSS Item Feed will be that of Contract Name
                    if (arrayObj == null || percentArray == null || percentArray.Length != 2 || arrayObj.Length != 3)
                    {                        
                        sb.Append("\"" + contractForGeneration.ContractName + "\"" + " ");                     
                    }

                    //if structure matches
                    else
                    {
                        //to check if the text before decimal is the name of the collection(correctly entered or not) 
                        if (percentArray.GetValue(0).ToString() == CollectionName.ToString())
                        {
                            Entities.GenericCollection<Entities.DataItem> dataItemsOfModule;
                            dataItemsOfModule = 
                                moduleForGeneration.Contracts[outer].InputModelObjects[0].ModelObjectEntity.DataItems;

                            //check the text after the decimal to match with any of the property of the model object entity
                            //if the property name matches then RSS Item Feed tiltle will have the name as the value
                            //of the property coming from the mainframe.
                            for (int loop = 0; loop < dataItemsOfModule[0].GroupItems.Count; loop++)
                            {
                                if (dataItemsOfModule[0].GroupItems[loop].ToString() == 
                                    percentArray.GetValue(1).ToString())
                                {
                                    if (dataItemsOfModule[0].GroupItems[loop].NumberOfOccurences == 1)
                                    {
                                        sb.Append("\"" + arrayObj.GetValue(0).ToString() + " " + "\"" + "+");
                                        sb.Append("objContracts." + CollectionName + "Collection[0]." + 
                                            dataItemsOfModule[0].GroupItems[loop].ToString() + ".ToString()" + 
                                            " " + "+");
                                        sb.Append("\"" + arrayObj.GetValue(2).ToString() + "\"");
                                    }

                                    else
                                    {
                                        sb.Append("\"" + arrayObj.GetValue(0).ToString() + " " + "\"" + "+");
                                        sb.Append("objContracts." + CollectionName + "Collection[0]." + 
                                            dataItemsOfModule[0].GroupItems[loop].ToString() + "Collection[loop]" + 
                                            ".ToString()" + " " + "+");
                                        sb.Append("\"" + arrayObj.GetValue(2).ToString() + "\"");
                                    }
                                }
                            }
                        }
                    }

                    //if the property name doesnot contain a value of is wrongly entered
                    //the title again takes the value of the ContractName
                    if (sb.ToString() == null || sb.ToString() == "")
                    {                        
                        sb.Append("\"" + contractForGeneration.ContractName + "\"" + " ");                     
                    }

                    return sb.ToString();
                }
            }


            [PlaceHolder("InputParameters")]
            string InputParameters
            {                
                get
                {
                    StringBuilder sb = new StringBuilder();

                    //to check the contract position inside a contract module
                    int outer = 0;
                    for (outer = 0; outer < moduleForGeneration.Contracts.Count; outer++)
                    {
                        if (moduleForGeneration.Contracts[outer].ContractName == ContractName)
                        {
                            break;
                        }
                    }

                    Entities.GenericCollection<Entities.DataItem> dataItemsOfModule;
                    dataItemsOfModule =
                        moduleForGeneration.Contracts[outer].InputModelObjects[0].ModelObjectEntity.DataItems;

                    for (int loop = 0; loop < dataItemsOfModule[0].GroupItems.Count; loop++)
                    {
                        if(dataItemsOfModule[0].GroupItems[loop].ItemType.ToString()=="Integer")
                        {
                            sb.Append("int");
                        }

                        if(dataItemsOfModule[0].GroupItems[loop].ItemType.ToString()=="String")
                        {
                            sb.Append("string");
                        }

                        sb.Append(" ");
                        sb.Append(dataItemsOfModule[0].GroupItems[loop].ToString());

                        if (loop != dataItemsOfModule[0].GroupItems.Count-1)
                        {
                            sb.Append(",");
                        }
                    }

                    return sb.ToString();
                }
            }


            [PlaceHolder("InputUriParameters")]
            string InputUriParameters
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    //to check the contract position inside a contract module
                    int outer = 0;

                    for (outer = 0; outer < moduleForGeneration.Contracts.Count; outer++)
                    {
                        if (moduleForGeneration.Contracts[outer].ContractName == ContractName)
                        {
                            break;
                        }
                    }

                    Entities.GenericCollection<Entities.DataItem> dataItemsOfModule;
                    dataItemsOfModule =
                        moduleForGeneration.Contracts[outer].InputModelObjects[0].ModelObjectEntity.DataItems;

                    for (int loop = 0; loop < dataItemsOfModule[0].GroupItems.Count; loop++)
                    {
                        sb.Append("&");
                        sb.Append(dataItemsOfModule[0].GroupItems[loop].ToString());
                        sb.Append("={");
                        sb.Append(dataItemsOfModule[0].GroupItems[loop].ToString());
                        sb.Append("}");

                    }

                    return sb.ToString();
                }
            }


            [PlaceHolder("InitializingInputParameters")]
            string InitializingInputParameters
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    //to check the contract position inside a contract module
                    int outer = 0;

                    for (outer = 0; outer < moduleForGeneration.Contracts.Count; outer++)
                    {
                        if (moduleForGeneration.Contracts[outer].ContractName == ContractName)
                        {
                            break;
                        }
                    }

                    Entities.GenericCollection<Entities.DataItem> dataItemsOfModule;
                    dataItemsOfModule =
                        moduleForGeneration.Contracts[outer].InputModelObjects[0].ModelObjectEntity.DataItems;
                    
                    for (int loop = 0; loop < dataItemsOfModule[0].GroupItems.Count; loop++)
                    {
                        if (dataItemsOfModule[0].GroupItems[loop].ItemType.ToString() == "String")
                        {
                            sb.Append(dataItemsOfModule[0].GroupItems[loop].ToString() + " =Initialize(" + 
                                dataItemsOfModule[0].GroupItems[loop].ToString() + ").ToString();");
                            sb.Append("\r\n");
                            sb.Append("\t\t\t");
                        }
                    }

                    return sb.ToString();
                }
            }


            [PlaceHolder("ParametersValueFilling")]
            string ParametersValueFilling
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    //to check the contract position inside a contract module
                    int outer = 0;

                    for (outer = 0; outer < moduleForGeneration.Contracts.Count; outer++)
                    {
                        if (moduleForGeneration.Contracts[outer].ContractName == ContractName)
                        {
                            break;
                        }
                    }

                    Entities.GenericCollection<Entities.DataItem> dataItemsOfModule;
                    dataItemsOfModule =
                        moduleForGeneration.Contracts[outer].InputModelObjects[0].ModelObjectEntity.DataItems;

                    for (int loop = 0; loop < dataItemsOfModule[0].GroupItems.Count; loop++)
                    {
                        if (dataItemsOfModule[0].GroupItems[loop].NumberOfOccurences == 1)
                        {
                            sb.Append("obj" + CollectionName + "." + dataItemsOfModule[0].GroupItems[loop].ToString() 
                                + "=" + dataItemsOfModule[0].GroupItems[loop].ToString());
                            sb.Append(";");
                            sb.Append("\r\n");
                            sb.Append("\t\t\t");
                        }

                        else
                        {
                            for (int loopInner = 0; loopInner < 
                                dataItemsOfModule[0].GroupItems[loop].NumberOfOccurences; loopInner++)
                            {
                                sb.Append("obj" + CollectionName + "." + 
                                    dataItemsOfModule[0].GroupItems[loop].ToString() + "Collection" + ".Add(" + 
                                    dataItemsOfModule[0].GroupItems[loop].ToString()+")");
                                sb.Append(";");
                                sb.Append("\r\n");
                                sb.Append("\t\t\t");
                            }
                        }
                    }

                    return sb.ToString();
                }
            }


            [PlaceHolder("HTMLFilledFirstRow")]
            string HTMLFilledFirstRow
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    //to check the contract position inside a contract module
                    int outer = 0;

                    for (outer = 0; outer < moduleForGeneration.Contracts.Count; outer++)
                    {
                        if (moduleForGeneration.Contracts[outer].ContractName == ContractName)
                        {
                            break;
                        }
                    }
                    
                    sb.Append("html.Append(" + "\"" + "<tr>" + "\"" + ");");
                    sb.Append("\r\n");
                    sb.Append("\t\t\t");

                    Entities.GenericCollection<Entities.DataItem> dataItemsOfModule;
                    dataItemsOfModule =
                        moduleForGeneration.Contracts[outer].InputModelObjects[0].ModelObjectEntity.DataItems;

                    for (int loop = 0; loop < dataItemsOfModule[0].GroupItems.Count; loop++)
                    {
                        sb.Append("html.Append(" + "\"" + "<td>" + "\"" + ");");
                        sb.Append("\r\n");
                        sb.Append("\t\t\t");
                        sb.Append("html.Append(" + "\"" + dataItemsOfModule[0].GroupItems[loop].ToString() + "\"" + ");");
                        sb.Append("\r\n");
                        sb.Append("\t\t\t");
                        sb.Append("html.Append(" + "\"" + "<" + "/" + "td>" + "\"" + ");");
                        sb.Append("\r\n");
                        sb.Append("\t\t\t");
                    }
                    sb.Append("html.Append(" + "\"" + "<" + "/" + "tr>" + "\"" + ");");
                    return sb.ToString();
                }
            }
                
        
            int maxNumberOfOccurences
            {
                get
                {
                    int var = 0;
                    //to check the contract position inside a contract module
                    int outer = 0;

                    for (outer = 0; outer < moduleForGeneration.Contracts.Count; outer++)
                    {
                        if (moduleForGeneration.Contracts[outer].ContractName == ContractName)
                        {
                            break;
                        }
                    }

                    Entities.GenericCollection<Entities.DataItem> dataItemsOfModule;
                    dataItemsOfModule =
                        moduleForGeneration.Contracts[outer].InputModelObjects[0].ModelObjectEntity.DataItems;
                    
                    for (int loop = 0; loop < dataItemsOfModule[0].GroupItems.Count; loop++)
                    {

                        if (dataItemsOfModule[0].GroupItems[loop].NumberOfOccurences > var)
                        {
                            var = dataItemsOfModule[0].GroupItems[loop].NumberOfOccurences;
                        }
                    }

                    return var;
                }
            }


            [PlaceHolder("HTMLFilledSecondRow")]
            string HTMLFilledSecondRow
            {
                get
                {
                    StringBuilder sb = new StringBuilder();                    
                    //to check the contract position inside a contract module
                    int outer = 0;

                    for (outer = 0; outer < moduleForGeneration.Contracts.Count; outer++)
                    {
                        if (moduleForGeneration.Contracts[outer].ContractName == ContractName)
                        {
                            break;
                        }
                    }
                   
                    sb.Append("html.Append(" + "\"" + "<tr>" + "\"" + ");");
                    sb.Append("\r\n");
                    sb.Append("\t\t\t");

                    Entities.GenericCollection<Entities.DataItem> dataItemsOfModule;
                    dataItemsOfModule =
                        moduleForGeneration.Contracts[outer].InputModelObjects[0].ModelObjectEntity.DataItems;

                    for (int loop = 0; loop < dataItemsOfModule[0].GroupItems.Count; loop++)
                    {                        
                        sb.Append("html.Append(" + "\"" + "<td>" + "\"" + ");");
                        sb.Append("\r\n");
                        sb.Append("\t\t\t");
                        
                        if (dataItemsOfModule[0].GroupItems[loop].NumberOfOccurences == 1)
                        {
                            sb.Append("html.Append(" + "objContracts." + CollectionName + "Collection[0]." + 
                                dataItemsOfModule[0].GroupItems[loop].ToString() + ".ToString()" + ");");
                        }

                        else
                        {
                            sb.Append("html.Append(" + "objContracts." + CollectionName + "Collection[0]." + 
                                dataItemsOfModule[0].GroupItems[loop].ToString() + "Collection[" + "loop" + "]" + 
                                ".ToString()" + ");");
                        }

                        sb.Append("\r\n");
                        sb.Append("\t\t\t");
                        sb.Append("html.Append(" + "\"" + "</td>" + "\"" + ");");
                        sb.Append("\r\n");
                        sb.Append("\t\t\t");
                    }

                    sb.Append("html.Append(" + "\"" + "</tr>" + "\"" + ");");                    
                    return sb.ToString();
                }
            }


            [PlaceHolder("CollectionName")]
            string CollectionName
            {
                get
                {
                    //to check the contract position inside a contract module
                    int outer = 0;

                    for (outer = 0; outer < moduleForGeneration.Contracts.Count; outer++)
                    {
                        if (moduleForGeneration.Contracts[outer].ContractName == ContractName)
                        {
                            break;
                        }
                    }
                   
                    return moduleForGeneration.Contracts[outer].InputModelObjects[0].ModelObjectEntity.EntityName.ToString();
                }
            }


            [PlaceHolder("ContentsBeforeTable")]
            string ContentsBeforeTable
            {
                get
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append("html.Append(" + "\"" + "<html>" + "\"" + ");");
                    sb.Append("\r\n");
                    sb.Append("\t\t\t");
                    sb.Append("html.Append(" + "\"" + "<head>" + "\"" + ");");
                    sb.Append("\r\n");
                    sb.Append("\t\t\t");
                    sb.Append("html.Append(" + "\"" + "<title>" + "\"" + ");");
                    sb.Append("\r\n");
                    sb.Append("\t\t\t");
                    sb.Append("html.Append(" + "\"" + ServiceName + "\"" + ");");
                    sb.Append("\r\n");
                    sb.Append("\t\t\t");
                    sb.Append("html.Append(" + "\"" + "<" + "/" + "title" + ">" + "\"" + ");");
                    sb.Append("\r\n");
                    sb.Append("\t\t\t");                   
                    sb.Append("html.Append(" + "\"" + "<link rel=" + "\"" + "+" + "\"" + "\\" + "\"" + "\"" + "+" + 
                        "\"" + "stylesheet" + "\"" + "+" + "\"" + "\\" + "\"" + "\"" + "+" + "\"" + " href=" + "\"" + 
                        "+" + "\"" + "\\" + "\"" + "\"" + "+" + "\"" + "../../../StyleSheet.css" + "\"" + "+" + "\"" + 
                        "\\" + "\"" + "\"" + "+" + "\"" + "/>" + "\"" + ");");
                    sb.Append("\r\n");
                    sb.Append("\t\t\t");
                    sb.Append("html.Append(" + "\"" + "<" + "/" + "head" + ">" + "\"" + ");");
                    sb.Append("\r\n");
                    sb.Append("\t\t\t");
                    sb.Append("html.Append(" + "\"" + "<body>" + "\"" + ");");
                    sb.Append("\r\n");
                    sb.Append("\t\t\t");
                    sb.Append("html.Append(" + "\"" + "<form" + " id=form1" + ">" + "\"" + ");");
                    sb.Append("\r\n");
                    sb.Append("\t\t\t");
                    sb.Append("html.Append(" + "\"" + "<div>" + "\"" + ");");
                    sb.Append("\r\n");
                    sb.Append("\t\t\t");
                    sb.Append("html.Append(" + "\"" + "<table" + " style=width:" + " 538px;" + " font-size:" + 
                        " 17 ;" + " height:" + " 141px;" + " border=0>" + "\"" + ");");
                    sb.Append("\r\n");
                    sb.Append("\t\t\t");

                    return sb.ToString();
                }                
            }


            [PlaceHolder("ContentsAfterTable")]
            string ContentsAfterTable
            {
                get
                {
                    StringBuilder sb = new StringBuilder();

                    sb.Append("html.Append(" + "\"" + "<" + "/" + "table>" + "\"" + ");");
                    sb.Append("\r\n");
                    sb.Append("\t\t\t");
                    sb.Append("html.Append(" + "\"" + "</div>" + "\"" + ");");
                    sb.Append("\r\n");
                    sb.Append("\t\t\t");
                    sb.Append("html.Append(" + "\"" + "</form>" + "\"" + ");");
                    sb.Append("\r\n");
                    sb.Append("\t\t\t");
                    sb.Append("html.Append(" + "\"" + "</body>" + "\"" + ");");
                    sb.Append("\r\n");
                    sb.Append("\t\t\t");
                    sb.Append("html.Append(" + "\"" + "<" + "/" + "html>" + "\"" + ");");
                    sb.Append("\r\n");
                    sb.Append("\t\t\t");
                   
                    return sb.ToString();
                }
            }

            [PlaceHolder("CollectionParameter")]
            string CollectionParameter
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    //to check the contract position inside a contract module
                    int outer = 0;

                    for (outer = 0; outer < moduleForGeneration.Contracts.Count; outer++)
                    {
                        if (moduleForGeneration.Contracts[outer].ContractName == ContractName)
                        {
                            break;
                        }
                    }

                    Entities.GenericCollection<Entities.DataItem> dataItemsOfModule;
                    dataItemsOfModule =
                        moduleForGeneration.Contracts[outer].InputModelObjects[0].ModelObjectEntity.DataItems;

                    for (int loop = 0; loop < dataItemsOfModule[0].GroupItems.Count; loop++)
                    {
                        if (dataItemsOfModule[0].GroupItems[loop].NumberOfOccurences == 1)
                        {
                            //have to return the collection parameter which comes first
                        }

                        else
                        {
                            sb.Append(dataItemsOfModule[0].GroupItems[loop].ToString());
                            break;
                        }
                    }

                    return sb.ToString();
                }
            }


            [PlaceHolder("CheckOutOrNot")]
            string CheckOutOrNot
            {
                get
                {                    
                    StringBuilder sb = new StringBuilder();
                    int counter = 0;
                    //to check the contract position inside a contract module
                    int outer = 0;

                    for (outer = 0; outer < moduleForGeneration.Contracts.Count; outer++)
                    {
                        if (moduleForGeneration.Contracts[outer].ContractName == ContractName)
                        {
                            break;
                        }
                    }

                    Entities.GenericCollection<Entities.DataItem> dataItemsOfModule;
                    dataItemsOfModule =
                        moduleForGeneration.Contracts[outer].InputModelObjects[0].ModelObjectEntity.DataItems;

                    for (int loop = 0; loop < dataItemsOfModule[0].GroupItems.Count; loop++)
                    {
                        if (dataItemsOfModule[0].GroupItems[loop].NumberOfOccurences == 1)
                        {
                            counter++;
                            //have to return the collection parameter which comes first
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (counter == dataItemsOfModule[0].GroupItems.Count)
                    {
                        sb.Append("//");
                        return sb.ToString();
                    }

                    else
                    {
                        sb.Append("");
                        return sb.ToString();
                    }
                }
            }


            [PlaceHolder("HTMLFilledNameValueRows")]
            string HTMLFilledNameValueRows
            {
                get
                {
                    StringBuilder sb = new StringBuilder();
                    //to check the contract position inside a contract module
                    int outer = 0;

                    for (outer = 0; outer < moduleForGeneration.Contracts.Count; outer++)
                    {
                        if (moduleForGeneration.Contracts[outer].ContractName == ContractName)
                        {
                            break;
                        }
                    }

                    Entities.GenericCollection<Entities.DataItem> dataItemsOfModule;
                    dataItemsOfModule =
                        moduleForGeneration.Contracts[outer].InputModelObjects[0].ModelObjectEntity.DataItems;

                    for (int loop = 0; loop < dataItemsOfModule[0].GroupItems.Count; loop++)
                    {
                        //add colours
                        string colourRow;
                        if (loop % 2 == 0)
                        {
                            colourRow = "Even";
                        }
                        else
                        {
                            colourRow = "Odd";
                        }
                        
                        sb.Append("html.Append(" + "\"" + "<tr>" + "\"" + ");");
                        sb.Append("\r\n");
                        sb.Append("\t\t\t");
                        sb.Append("html.Append(" + "\"" + "<td class=" + "\"" + "+" + "\"" + "\\" + "\"" + "\"" + 
                            " + " + "\"" + colourRow + "\"" + "+" + "\"" + "\\" + "\"" + "\"" + "+" + "\"" + ">" + 
                            "\"" + ");");
                        sb.Append("\r\n");
                        sb.Append("\t\t\t");
                        sb.Append("html.Append(" + "\"" + dataItemsOfModule[0].GroupItems[loop].ToString() + "\"" + 
                            ");");
                        sb.Append("\r\n");
                        sb.Append("\t\t\t");
                        sb.Append("html.Append(" + "\"" + "</td>" + "\"" + ");");
                        sb.Append("\r\n");
                        sb.Append("\t\t\t");
                        sb.Append("html.Append(" + "\"" + "<td class=" + "\"" + "+" + "\"" + "\\" + "\"" + "\"" + 
                            " + " + "\"" + colourRow + "\"" + "+" + "\"" + "\\" + "\"" + "\"" + "+" + "\"" + ">" + 
                            "\"" + ");");
                        sb.Append("\r\n");
                        sb.Append("\t\t\t");

                        if (dataItemsOfModule[0].GroupItems[loop].NumberOfOccurences == 1)
                        {
                            sb.Append("html.Append(" + "objContracts." + CollectionName + "Collection[0]." + 
                                dataItemsOfModule[0].GroupItems[loop].ToString() + ".ToString()" + ");");
                        }

                        else
                        {
                            sb.Append("html.Append(" + "objContracts." + CollectionName + "Collection[0]." + 
                                dataItemsOfModule[0].GroupItems[loop].ToString() + "Collection[" + "loop" + "]" + 
                                ".ToString()" + ");");
                        }

                        sb.Append("\r\n");
                        sb.Append("\t\t\t");
                        sb.Append("html.Append(" + "\"" + "</td>" + "\"" + ");");
                        sb.Append("\r\n");
                        sb.Append("\t\t\t");
                        sb.Append("html.Append(" + "\"" + "<" + "/" + "tr>" + "\"" + ");");
                        sb.Append("\r\n");
                        sb.Append("\t\t\t");
                    }
                    return sb.ToString();
                }
            }
        }
    }
}
