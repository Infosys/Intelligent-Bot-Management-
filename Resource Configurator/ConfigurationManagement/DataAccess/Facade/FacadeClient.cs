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
using Infosys.Ainauto.Framework.Facade;
using Infosys.Ainauto.Framework.Facade.Entity;
using Infosys.Solutions.ConfigurationManager.Infrastructure.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Infosys.Solutions.Ainauto.Resource.DataAccess.Facade
{
    public class FacadeClient
    {
        const string ADAPTER_TYPE = "SEE";
        const string ADAPTER_METHOD_NAME = "Get";
        static class InterfaceConsts
        {
            public const string IAdapterBase = "Infosys.Ainauto.Adapters.Interfaces.IAdapterBase";
            public const string IDBAdapter = "Infosys.Ainauto.Adapters.Interfaces.IDBAdapter";
            public const string IApiAdapter = "Infosys.Ainauto.Adapters.Interfaces.IApiAdapter";

        }

        public string GetCategoryList(string baseURI, string endpoint)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GetCategoryList", "FacadeClient"), LogHandler.Layer.Business, null);
            //LogHandler.LogInfo(string.Format("GetCategoryList method with parameters : BaseURI :{0} ; endpoint: {1}",baseURI,endpoint), LogHandler.Layer.Business, null);
            try
            {
                Infosys.Ainauto.Framework.Facade.Facade facade = new Infosys.Ainauto.Framework.Facade.Facade();
                FacadeResult facadeResult = null;

                Dictionary<string, string> dictConfigOverride = new Dictionary<string, string>();
                
                dictConfigOverride.Add("BaseURI", baseURI);
                dictConfigOverride.Add("Operation", "Get");
                dictConfigOverride.Add("EndPoint", endpoint);


                Dictionary<string, string> dictReqParams = new Dictionary<string, string>();
                facadeResult = facade.Execute(ADAPTER_TYPE, InterfaceConsts.IApiAdapter, "Get", "GetAllCategory", dictReqParams, dictConfigOverride);
                //LogHandler.LogInfo(string.Format("Facade Result status: {0} ; Data : {1}", facadeResult.Status,facadeResult.Data), LogHandler.Layer.Business, null);
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetCategoryList", "FacadeClient"), LogHandler.Layer.Business, null);

                if (facadeResult.Status == FacadeResultStatus.Success)
                {
                    return facadeResult.Data;
                }
                //LogHandler.LogInfo(string.Format("Facade Result validation detrails: {0} ", facadeResult.ValidationDetails), LogHandler.Layer.Business, null);
                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public string GetScriptList(string baseURI, string endpoint)
        {
            Infosys.Ainauto.Framework.Facade.Facade facade = new Infosys.Ainauto.Framework.Facade.Facade();
            FacadeResult facadeResult = null;

            Dictionary<string, string> dictConfigOverride = new Dictionary<string, string>();
            dictConfigOverride.Add("BaseURI", baseURI);
            dictConfigOverride.Add("Operation", "Get");
            dictConfigOverride.Add("EndPoint", endpoint);


            Dictionary<string, string> dictReqParams = new Dictionary<string, string>();
            facadeResult = facade.Execute(ADAPTER_TYPE, InterfaceConsts.IApiAdapter, "Get", "GetAllScript", dictReqParams, dictConfigOverride);

            if (facadeResult.Status == FacadeResultStatus.Success)
            {
                return facadeResult.Data;
            }
            return "";
        }

        public string getActivityList()
        {
            string token = getAuthToken();
            if (token != null)
            {
                Infosys.Ainauto.Framework.Facade.Facade facade = new Infosys.Ainauto.Framework.Facade.Facade();
                FacadeResult facadeResult = null;

                Dictionary<string, string> dictConfigOverride = new Dictionary<string, string>();
                dictConfigOverride.Add("EndPoint", "v2/repository/file/list");
                dictConfigOverride.Add("Body", "{\"filter\":{}}");
                //dictConfigOverride.Add("Body", "{\"sort\":[{\"field\":\"endDateTime\",\"direction\":\"desc\"}],\"filter\":{},\"fields\":[],\"page\":{\"length\":200,\"offset\":0}}");
                dictConfigOverride.Add("HttpHeaders.X-Authorization", token);
                Dictionary<string, string> dictReqParams = new Dictionary<string, string>();
                facadeResult = facade.Execute(ADAPTER_TYPE, InterfaceConsts.IApiAdapter, "Post", "ActivityList", dictReqParams, dictConfigOverride);
                if (facadeResult.Status == FacadeResultStatus.Success)
                {
                    return facadeResult.Data;
                }
                return "";
            }
            return "";
        }

        private static string getAuthToken()
        {
            Infosys.Ainauto.Framework.Facade.Facade facade = new Infosys.Ainauto.Framework.Facade.Facade();
            FacadeResult facadeResult = null;

            Dictionary<string, string> dictConfigOverride = new Dictionary<string, string>();
            dictConfigOverride.Add("EndPoint", "v1/authentication");
            dictConfigOverride.Add("Body", "{\"username\":\"apiuser\",\"password\":\"apipassword\"}");

            Dictionary<string, string> dictReqParams = new Dictionary<string, string>();

            facadeResult = facade.Execute(ADAPTER_TYPE, InterfaceConsts.IApiAdapter, "Post", "Authenticate", dictReqParams, dictConfigOverride);

            if (facadeResult.Status == FacadeResultStatus.Success)
            {
                JObject jsonObj = (JObject)JsonConvert.DeserializeObject(facadeResult.Data);

                string authToken = (string)jsonObj.SelectToken("AuthToken");
                return authToken;
            }
            return null;
        }

        public string EncryptText(string baseURI, string endpoint)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "Encrypt", "FacadeClient"), LogHandler.Layer.Business, null);
            try
            {
                Infosys.Ainauto.Framework.Facade.Facade facade = new Infosys.Ainauto.Framework.Facade.Facade();
                FacadeResult facadeResult = null;

                Dictionary<string, string> dictConfigOverride = new Dictionary<string, string>();
                dictConfigOverride.Add("BaseURI", baseURI);
                dictConfigOverride.Add("Operation", "Get");
                dictConfigOverride.Add("EndPoint", endpoint);


                Dictionary<string, string> dictReqParams = new Dictionary<string, string>();
                facadeResult = facade.Execute(ADAPTER_TYPE, InterfaceConsts.IApiAdapter, "Get", "Encrypt", dictReqParams, dictConfigOverride);
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "Encrypt", "FacadeClient"), LogHandler.Layer.Business, null);

                if (facadeResult.Status == FacadeResultStatus.Success)
                {
                    return facadeResult.Data;
                }
                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string DecryptText(string baseURI, string endpoint)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "Encrypt", "FacadeClient"), LogHandler.Layer.Business, null);
            try
            {
                Infosys.Ainauto.Framework.Facade.Facade facade = new Infosys.Ainauto.Framework.Facade.Facade();
                FacadeResult facadeResult = null;

                Dictionary<string, string> dictConfigOverride = new Dictionary<string, string>();
                dictConfigOverride.Add("BaseURI", baseURI);
                dictConfigOverride.Add("Operation", "Get");
                dictConfigOverride.Add("EndPoint", endpoint);


                Dictionary<string, string> dictReqParams = new Dictionary<string, string>();
                facadeResult = facade.Execute(ADAPTER_TYPE, InterfaceConsts.IApiAdapter, "Get", "Decrypt", dictReqParams, dictConfigOverride);
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "Decrypt", "FacadeClient"), LogHandler.Layer.Business, null);

                if (facadeResult.Status == FacadeResultStatus.Success)
                {
                    return facadeResult.Data;
                }
                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //if (facadeResult.Status == FacadeResultStatus.Success)
        //{
        //    return facadeResult.Data;
        //}
        //return "";


        public string UiPathGetAccessToken(string baseURI, string endpoint)
        {
            string token = string.Empty;
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "UiPathGetAccessToken", "FacadeClient"), LogHandler.Layer.Business, null);
            //LogHandler.LogInfo(string.Format("GetCategoryList method with parameters : BaseURI :{0} ; endpoint: {1}",baseURI,endpoint), LogHandler.Layer.Business, null);
            try
            {
                Infosys.Ainauto.Framework.Facade.Facade facade = new Infosys.Ainauto.Framework.Facade.Facade();
                FacadeResult facadeResult = null;

                Dictionary<string, string> dictConfigOverride = new Dictionary<string, string>();
                dictConfigOverride.Add("BaseURI", baseURI);
                dictConfigOverride.Add("Operation", "Post");
                dictConfigOverride.Add("EndPoint", endpoint);



                Dictionary<string, string> dictReqParams = new Dictionary<string, string>();
                facadeResult = facade.Execute(ADAPTER_TYPE, InterfaceConsts.IApiAdapter, "Post", "GetAccessToken", dictReqParams, dictConfigOverride);
                //LogHandler.LogInfo(string.Format("Facade Result status: {0} ; Data : {1}", facadeResult.Status,facadeResult.Data), LogHandler.Layer.Business, null);
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetCategoryList", "FacadeClient"), LogHandler.Layer.Business, null);

                if (facadeResult.Status == FacadeResultStatus.Success)
                {
                    return facadeResult.Data;
                }
                
                return "";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ExecuteSEEScript(InitiateExecutionReqMsg reqMsg)
        {
            string token = string.Empty;
            string body = JsonConvert.SerializeObject(reqMsg);
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "UiPathGetAccessToken", "FacadeClient"), LogHandler.Layer.Business, null);
            //LogHandler.LogInfo(string.Format("GetCategoryList method with parameters : BaseURI :{0} ; endpoint: {1}",baseURI,endpoint), LogHandler.Layer.Business, null);
            try
            {
                Infosys.Ainauto.Framework.Facade.Facade facade = new Infosys.Ainauto.Framework.Facade.Facade();
                FacadeResult facadeResult = null;

                Dictionary<string, string> dictConfigOverride = new Dictionary<string, string>();
                dictConfigOverride.Add("BaseURI", "localhost");
                dictConfigOverride.Add("EndPoint", "WEMScriptExecutor_Dev/WEMScriptExecService.svc/InitiateExecution");
                dictConfigOverride.Add("Body", body);

                Dictionary<string, string> dictReqParams = new Dictionary<string, string>();
                facadeResult = facade.Execute(ADAPTER_TYPE, InterfaceConsts.IApiAdapter, "Post", "ExecuteScript", dictReqParams, dictConfigOverride);

                

                if (facadeResult.Status == FacadeResultStatus.Success)
                {
                    return facadeResult.Data;
                }

                return "";


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    } 
}
