using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel;

namespace Infosys.Lif.LegacyWorkbench.Entities
{
    public class Contract
    {        
        public enum ContractMethodType
        {
            Select,
            List,
            Insert,
            Update,
            Delete
        }

        Entities.GenericCollection<ModelObject> inputModelObjects;

        [Browsable(false)]
        public Entities.GenericCollection<ModelObject> InputModelObjects
        {
            get { return inputModelObjects; }
            set { inputModelObjects = value; }
        }


        Entities.GenericCollection<ModelObject> outputModelObjects;
        [Browsable(false)]
        public Entities.GenericCollection<ModelObject> OutputModelObjects
        {
            get { return outputModelObjects; }
            set { outputModelObjects = value; }
        }


        string contractName;
        [DisplayName("Contract Name")]
        [Description("A customized name assigned to a contract")]
        public string ContractName
        {
            get { return contractName; }
            set { contractName = value; }
        }

        ContractMethodType methodType;
        [DisplayName("Contract Method Type")]
        [Description("Method type of a contract is the operation to be performed in it i.e. select, list, insert, update, delete")]
        public ContractMethodType MethodType
        {
            get { return methodType; }
            set { methodType = value; }
        }

        string methodName;
        [DisplayName("Method Name")]
        [Description("A customized name can be assigned to the method")]
        public string MethodName
        {
            get { return methodName; }
            set { methodName = value; }
        }

        string contractDescription;
        [DisplayName("Contract Description")]
        [Description("A short description of the contract ")]
        public string ContractDescription
        {
            get { return contractDescription; }
            set { contractDescription = value; }
        }

        bool isToBeGenerated = false;
        [DisplayName("Is To Be Generated")]
        [Description("The Contract should be included in generated solution or not")]
        public bool IsToBeGenerated
        {
            get { return isToBeGenerated; }
            set { isToBeGenerated = value; }
        }

        string serviceName;
        [DisplayName("Service Name")]
        [Description("This is the name of the functionality which the framework will use to call the contract. A customized name can be assigned to the service.")]
        public string ServiceName
        {
            get { return serviceName; }
            set { serviceName = value; }
        }


        string transactionId;
        [DisplayName("Transaction ID")]
        [Description("This is the Transaction ID sent in the message header to the host along with data. It is used by the Host dispatcher to invoke business programs using this transaction id. A customized transaction ID can be supplied.")]
        public string TransactionId
        {
            get { return transactionId; }
            set { transactionId = value; }
        }

        string notes;
        [DisplayName("Notes")]
        [Description("Any details added along with the contract")]
        public string Notes
        {
            get { return notes; }
            set { notes = value; }
        }

        //a new row added to contain the information of the Feed title appicable to only RSS and Atom
        string feed_title;
        [DisplayName("Feed Item Title")]
        [Description("The title gets displayed for the RSS Item Feed.The format should be <User entered Text> <%><ModelObjectEntityName><.><PropertyName><%> <User enetered text>")]        
        public string Feed_Title
        {
            get
            {
                return feed_title;
            }
            set
            {
                feed_title = value;
            }
        }
    }
}
