/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.Solutions.Ainauto.BusinessEntity
{
    public class PlatformInstanceDetails
    {
        public string VendorName { get; set; }
        public int PlatformId { get; set; }
        public int TenantId { get; set; }
        public string UID { get; set; }
        public string Pwd { get; set; }
        public int PlatformResourceModelVersion { get; set; }
        //public PlatformAttributes PlatformAttributes { get; set; }
        public List<PlatformObservable> platformObservables { get; set; }
        public List<Server> servers { get; set; }
    }

    public class PlatformObservable
    {
        public string Name { get; set; }
        public string ObservableId { get; set; }

        public Actions Actions { get; set; }

        // public List<ObservableParameters> ParameterDetails { get; set; }
        //public ObservableHealthcheck observableHealthcheck { get; set; }
    }

    public class Actions
    {
        public int ActionId { get; set; }
        public int ScriptId { get; set; }
        public int CategoryId { get; set; }
        public int AutomationEngineId { get; set; }
        public int ActionTypeId { get; set; }
        public string ActionName { get; set; }
        public string ExecutionMode { get; set; }
        public List<ObservableParameters> ParameterDetails { get; set; }
    }
    public class ObservableParameters
    {
        public string ParamaterName { get; set; }
        public string ParameterValue { get; set; }
        public bool? IsSecret { get; set; }
    }


    public class ObservableHealthcheck
    {
        public int ScriptId { get; set; }
        public int CategoryId { get; set; }
        public int AutomationEngineId { get; set; }
        public string UID { get; set; }
        public string Pwd { get; set; }

    }

    #region server properties

    public class Server
    {
        public string IPAddress { get; set; }
        //public ServerAttributes serverAttributes { get; set; }
        public List<ServerObservable> ServerObservables { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public List<Bot> bots { get; set; }
        public List<Service> services { get; set; }
    }
    //public class ServerAttributes
    //{
    //    public string IPAddress { get; set; }
    //}

    public class ServerObservable
    {
        public string IPAddress { get; set; }
        public string ResourceId { get; set; }
        public string Name { get; set; }
        public string ObservableId { get; set; }
        public Actions Actions { get; set; }
        //public List<ObservableParameters> ParameterDetails { get; set; }
        // public ObservableHealthcheck observableHealthcheck { get; set; }
    }


    public class Bot
    {
        public string IPAddress { get; set; }
        public string BotName { get; set; }
        public string BotInstanceId { get; set; }
        //public BotAttributes botAttributes { get; set; }
        public List<BotObservable> botObservables { get; set; }

    }

    //public class BotAttributes
    //{
    //    public string BotName { get; set; }
    //    public int BotInstanceId { get; set; }

    //}

    public class BotObservable
    {
        public string Name { get; set; }
        public string ObservableId { get; set; }

        public Actions Actions { get; set; }
        // public List<ObservableParameters> ParameterDetails { get; set; }
        //public ObservableHealthcheck observableHealthcheck { get; set; }
    }

    public class Service
    {
        public string IPAddress { get; set; }
        public string ServiceName { get; set; }
        public string ServiceId { get; set; }
        //public ServiceAttributes serviceAttributes { get; set; }
        public List<ServiceObservable> serviceObservables { get; set; }

    }
    //public class ServiceAttributes
    //{
    //    public string ServiceName { get; set; }
    //    public int ServiceId { get; set; }

    //}

    public class ServiceObservable
    {
        public string Name { get; set; }
        public string ObservableId { get; set; }

        public Actions Actions { get; set; }
        //public List<ObservableParameters> ParameterDetails { get; set; }
        //public ObservableHealthcheck observableHealthcheck { get; set; }
    }
        
    #endregion


    public enum Type
    {
        ControlTowerApp,
        BotRunner,
        ControlTowerDB
    };

    public enum ResourceType
    {
        Platform = 0,
        Bot_Runner = 1,
        ControlTower = 2,
        DBServer = 3,
        BOT = 4,
        Services = 5,
        DBService=6,        
        Bot_Creator=7,
        Server = 8,
        Oracle_Database=10,
        Default

    };
}
