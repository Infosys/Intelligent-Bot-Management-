/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Infosys.Solutions.Ainauto.ConfigurationManager.BusinessEntity;
using Infosys.Ainauto.ConfigurationManager.BusinessEntity;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Infosys.Solutions.Ainauto.Resource.DataAccess.Facade;

namespace Infosys.Ainauto.ConfigurationManager.BusinessComponent
{
    public class UiPathService
    {
        readonly HttpClient client;
        WebClient webClient;
        readonly string accessToken;
        /*public UiPathService(string clientId, string userKey, string tenantLogicalName)
        {
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12 | System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls;
            // webClient = new WebClient();
            client = new HttpClient();
            
            accessToken = GetUiPathAccessToken1(clientId,userKey,tenantLogicalName);

        }*/
        public string GetUiPathAccessToken(string clientId, string userKey, string tenantLogicalName)
        {


            //HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "https://account.uipath.com/oauth/token");
            Uri uri = new Uri("https://account.uipath.com/oauth/token");
            //string inputJsonBody = "{\"grant_type\":\"refresh_token\",\"client_id\":\"8DEv1AMNXczW3y4U15LL3jYf62jK93n5\",\"refresh_token\":\"1WDbZTJ_qeX4ZnsQhqyvq_W_NAzCfNKsAPqp3EADpFGYc\"}";
            string inputJsonBody = "{\"grant_type\":\"refresh_token\",\"client_id\":\"" + clientId + "\",\"refresh_token\":\"" + userKey + "\"}";

            webClient.Headers.Add("X-UIPATH-TenantName", tenantLogicalName);
            webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
            //HttpContent requestContent = new StreamContent(GenerateStreamFromString(inputJsonBody));
            ////requestContent.Headers.Add("X-UIPATH-TenantName", "InfosysDefapyud470494");
            //requestContent.Headers.Add("X-UIPATH-TenantName", tenantLogicalName);
            //requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //req.Content = requestContent;
            //HttpResponseMessage response = await client.SendAsync(req).ConfigureAwait(false);
            var response = webClient.UploadString(uri,"Post",inputJsonBody);
            

            if (response!=null)
            {
                string responseText = response;
                //Console.WriteLine(responseText);

                JObject jsonObj = (JObject)JsonConvert.DeserializeObject(responseText);
                string accessToken = (string)jsonObj.SelectToken("access_token");
                //Console.WriteLine(accessToken);

                return accessToken;
            }

            return "";

        }

        public string GetUiPathAccessToken1(string clientId, string userKey, string tenantLogicalName)
        {
            try
            {
                //HttpClient client = new HttpClient();
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, "https://account.uipath.com/oauth/token");

                //string inputJsonBody = "{\"grant_type\":\"refresh_token\",\"client_id\":\"8DEv1AMNXczW3y4U15LL3jYf62jK93n5\",\"refresh_token\":\"1WDbZTJ_qeX4ZnsQhqyvq_W_NAzCfNKsAPqp3EADpFGYc\"}";
                string inputJsonBody = "{\"grant_type\":\"refresh_token\",\"client_id\":\"" + clientId + "\",\"refresh_token\":\"" + userKey + "\"}";

                HttpContent requestContent = new StreamContent(GenerateStreamFromString(inputJsonBody));
                //requestContent.Headers.Add("X-UIPATH-TenantName", "InfosysDefapyud470494");
                requestContent.Headers.Add("X-UIPATH-TenantName", tenantLogicalName);
                requestContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                req.Content = requestContent;
                //HttpResponseMessage response = await client.SendAsync(req).ConfigureAwait(false);
                var response = client.SendAsync(req);
                response.Wait(3000);

                if (response.IsCompleted)
                {
                    string responseText = response.Result.Content.ReadAsStringAsync().Result;
                    //Console.WriteLine(responseText);

                    JObject jsonObj = (JObject)JsonConvert.DeserializeObject(responseText);
                    string accessToken = (string)jsonObj.SelectToken("access_token");
                    //Console.WriteLine(accessToken);

                    return accessToken;
                }
            }
            catch (HttpRequestException ex)
            {

                throw;
            }

            return "";

        }
        static Stream GenerateStreamFromString(string content)
        {
            byte[] byteArr = Encoding.UTF8.GetBytes(content);
            MemoryStream stream = new MemoryStream(byteArr);
            return stream;
        }

        #region Commented Code
        /*public async Task<List<UiPathRobot>> GetRobotDetails(string accountLogicalName, string tenantLogicalName)
        {
            using (HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://cloud.uipath.com/" + accountLogicalName + "/" + tenantLogicalName + "/odata/Robots"))
            {
                //string accessToken = await GetUiPathAccessToken(clientId, userKey, tenantLogicalName).ConfigureAwait(false);
                string accessTokenString = string.Format("Bearer {0}", accessToken);
                Console.WriteLine("Getting access Token for robot details...\n");

                httpRequestMessage.Headers.Add("Authorization", accessTokenString);
                httpRequestMessage.Headers.Add("X-UIPATH-TenantName", tenantLogicalName);

                var response = await client.SendAsync(httpRequestMessage).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    string responseText = response.Content.ReadAsStringAsync().Result;
                    dynamic jsonObj = JsonConvert.DeserializeObject(responseText);
                    List<UiPathRobot> rbList = new List<UiPathRobot>();
                    if (jsonObj!=null)
                    {
                        foreach (var obj in jsonObj.value)
                        {
                            UiPathRobot rob = new UiPathRobot
                            {
                                Id= obj.Id,
                                MachineName = obj.MachineName,
                                Name = obj.Name,
                                Type = obj.Type
                            };
                            rbList.Add(rob);
                        }
                    }

                    return rbList;
                }

                return null;
            }
        }
        public async Task<List<UiPathProcess>> GetAllProcesses(string accountLogicalName, string tenantLogicalName)
        {
            //string accessToken = await GetUiPathAccessToken(clientId, userKey, tenantLogicalName);
            string accessTokenString = string.Format("Bearer {0}", accessToken);

            using (HttpClient client = new HttpClient())
            {
                using (HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, "https://cloud.uipath.com/" + accountLogicalName + "/" + tenantLogicalName + "/odata/Releases"))
                {
                    httpRequestMessage.Headers.Add("Authorization", accessTokenString);
                    httpRequestMessage.Headers.Add("X-UIPATH-TenantName", tenantLogicalName);

                    var response = await client.SendAsync(httpRequestMessage).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseText = response.Content.ReadAsStringAsync().Result;
                        dynamic jsonObj = JsonConvert.DeserializeObject(responseText);
                        List<UiPathProcess> processList = new List<UiPathProcess>();
                        if (jsonObj != null)
                        {
                            foreach (var obj in jsonObj.value)
                            {
                                UiPathProcess pObj = new UiPathProcess
                                {
                                    Id = obj.Id,
                                    Key = obj.Key,
                                    Name = obj.Name,
                                    ProcessType = obj.ProcessType
                                };
                                processList.Add(pObj);
                            }
                        }

                        return processList;                        
                    }
                    return null;
                }
            }

        }
        */
        #endregion

        public List<UiPathRobot> GetRobotDetails(string accountLogicalName, string tenantLogicalName,List<Labels> orchestratorDetailsList, string tenantID)
        {
            List<UiPathRobot> rbList = new List<UiPathRobot>();
            #region SEE input data
            string robotSerivceURI = System.Configuration.ConfigurationManager.AppSettings["robotSerivceURI"];
            string robotSerivceURI2 = System.Configuration.ConfigurationManager.AppSettings["robotSerivceURI2"];
            string orchestratorurl = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "orchestratorurl" select o.value).FirstOrDefault();

            string[] inputPrams = new string[] { "authenticationURI","robotSerivceURI", "clientId","refreshToken" , "uiPathTenantName","userName","password" };
            InitiateExecutionReqMsg executionReqMsg = new InitiateExecutionReqMsg();
            ScriptIdentifier script = new ScriptIdentifier();
            script.ScriptId = 735;
            script.CategoryId = 201;
            script.CompanyId = Convert.ToInt32(tenantID);

            List<Parameter> parametersSE = new List<Parameter>();
            foreach(string s in inputPrams)
            {
                Parameter param1 = new Parameter();
                param1.ParameterName = s;
                switch (s)
                {
                    case "authenticationURI":
                        param1.ParameterValue= (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "authenticationurl" select o.value).FirstOrDefault();
                        break;
                    case "robotSerivceURI":
                        //param1.ParameterValue =string.IsNullOrEmpty(accountLogicalName) && string.IsNullOrEmpty(tenantLogicalName)? 
                        //    (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "orchestratorurl" select o.value).FirstOrDefault()+ "/odata/Robots":
                        //    (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "orchestratorurl" select o.value).FirstOrDefault()
                        //                        + accountLogicalName + "/" + tenantLogicalName + "/odata/Robots";

                        param1.ParameterValue = string.IsNullOrEmpty(accountLogicalName) && string.IsNullOrEmpty(tenantLogicalName) ?
                            String.Format(robotSerivceURI, orchestratorurl) : String.Format(robotSerivceURI2, orchestratorurl, accountLogicalName, tenantLogicalName);                           
                                                
                        break;
                    case "clientId":
                        param1.ParameterValue = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "clientid" select o.value).FirstOrDefault();
                        break;
                    case "userName":
                        param1.ParameterValue = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "orchestratorusername" select o.value).FirstOrDefault();
                        break;
                    case "password":
                        param1.ParameterValue = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "orchestratorpassword" select o.value).FirstOrDefault();
                        break;
                    case "refreshToken":
                        param1.ParameterValue = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "refreshtoken" select o.value).FirstOrDefault();
                        break;
                    case "uiPathTenantName":
                        param1.ParameterValue = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "uipathtenantname" select o.value).FirstOrDefault();
                        break;
                }
                if (param1.ParameterValue != null)
                {
                    parametersSE.Add(param1);
                }
                
            }
            
            script.Parameters = parametersSE;

            executionReqMsg.ScriptIdentifier = script;
            #endregion

            string responseText = ExecuteSEE(executionReqMsg);
            if (responseText != null && responseText.Contains("status=success"))
            {
                responseText = responseText.Contains("status=success") ? responseText.Split(new string[] { "RobotsDetails:" }, StringSplitOptions.None)[1] : null;
                //dynamic jsonObj = JsonConvert.DeserializeObject(responseText);
                dynamic jsonObj = JObject.Parse(responseText);
               
                if (jsonObj != null)
                {
                    foreach (var obj in jsonObj.value)
                    {
                        UiPathRobot rob = new UiPathRobot
                        {
                            Id = obj.Id,
                            MachineName = obj.MachineName,
                            Name = obj.Name,
                            Type ="Robot-"+obj.Type
                        };
                        rbList.Add(rob);
                    }
                }
                
            }
            return rbList;
        }

        public List<UiPathProcess> GetAllProcesses(string accountLogicalName, string tenantLogicalName, List<Labels> orchestratorDetailsList,string tenantID)
        {
            List<UiPathProcess> processList = new List<UiPathProcess>();
            #region SEE input data
            string processSerivceURI = System.Configuration.ConfigurationManager.AppSettings["processSerivceURI"];
            string processSerivceURI2 = System.Configuration.ConfigurationManager.AppSettings["processSerivceURI2"];

            string orchestratorurl = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "orchestratorurl" select o.value).FirstOrDefault();

            string[] inputPrams = new string[] { "authenticationURI", "processSerivceURI", "clientId", "refreshToken", "uiPathTenantName", "userName", "password" };
            InitiateExecutionReqMsg executionReqMsg = new InitiateExecutionReqMsg();
            ScriptIdentifier script = new ScriptIdentifier();
            script.ScriptId = 736;
            script.CategoryId = 201;
            script.CompanyId = Convert.ToInt32(tenantID);

            List<Parameter> parametersSE = new List<Parameter>();
            foreach (string s in inputPrams)
            {
                Parameter param1 = new Parameter();
                param1.ParameterName = s;
                switch (s)
                {
                    case "authenticationURI":
                        param1.ParameterValue = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "authenticationurl" select o.value).FirstOrDefault();
                        break;
                    case "processSerivceURI":
                        //param1.ParameterValue = string.IsNullOrEmpty(accountLogicalName) && string.IsNullOrEmpty(tenantLogicalName) ?
                        //    (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "orchestratorurl" select o.value).FirstOrDefault()
                        //                        + "/odata/Releases":
                        //    (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "orchestratorurl" select o.value).FirstOrDefault()
                        //                        + accountLogicalName + "/" + tenantLogicalName + "/odata/Releases";

                        param1.ParameterValue = string.IsNullOrEmpty(accountLogicalName) && string.IsNullOrEmpty(tenantLogicalName) ?
                            String.Format(processSerivceURI, orchestratorurl) : String.Format(processSerivceURI2, orchestratorurl, accountLogicalName, tenantLogicalName);
                        break;
                    case "userName":
                        param1.ParameterValue = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "orchestratorusername" select o.value).FirstOrDefault();
                        break;
                    case "password":
                        param1.ParameterValue = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "orchestratorpassword" select o.value).FirstOrDefault();
                        break;
                    case "clientId":
                        param1.ParameterValue = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "clientid" select o.value).FirstOrDefault();
                        break;
                    case "refreshToken":
                        param1.ParameterValue = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "refreshtoken" select o.value).FirstOrDefault();
                        break;
                    case "uiPathTenantName":
                        param1.ParameterValue = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "uipathtenantname" select o.value).FirstOrDefault();
                        break;
                }
                if (param1.ParameterValue != null)
                {
                    parametersSE.Add(param1);
                }
                
            }

            script.Parameters = parametersSE;

            executionReqMsg.ScriptIdentifier = script;
            #endregion

            string responseText = ExecuteSEE(executionReqMsg);
            if (responseText.Contains("status=success"))
            {
                responseText = responseText.Contains("status=success")?responseText.Split(new string[] { "ProcessDetails:" }, StringSplitOptions.None)[1] : null; 
                dynamic jsonObj = JsonConvert.DeserializeObject(responseText);
               
                if (jsonObj != null)
                {
                    foreach (var obj in jsonObj.value)
                    {
                        UiPathProcess pObj = new UiPathProcess
                        {
                            Id = obj.Id,
                            Key = obj.Key,
                            Name = obj.Name,
                            // ProcessType = obj.ProcessType
                            ProcessType = "Process"
                        };
                        processList.Add(pObj);
                    }
                }
            }
            return processList;

        }

        public string ExecuteSEE(InitiateExecutionReqMsg executionReqMsg)
        {
            try
            {
                #region WebClient

                // Create string to hold JSON response
                string jsonResponse = string.Empty;

                using (var client = new System.Net.WebClient())
                {
                    try
                    {
                        client.UseDefaultCredentials = true;
                        client.Headers.Add("Content-Type:application/json");
                        client.Headers.Add("Accept:application/json");
                        //client.Headers.Add("apiKey", "MyKey");
                        string SCCBaseURL = System.Configuration.ConfigurationManager.AppSettings["SCCServiceURL"];
                        var uri = new Uri(SCCBaseURL+"/WEMScriptExecService.svc/InitiateExecution");
                        // var content = JsonConvert.SerializeObject(request);
                        string input = JsonConvert.SerializeObject(executionReqMsg);
                        var response = client.UploadString(uri, "POST", input);
                        InitiateExecutionResMsg resMsg = JsonConvert.DeserializeObject<InitiateExecutionResMsg>(response);
                        jsonResponse = resMsg.ScriptResponse.FirstOrDefault().SuccessMessage;
                    }
                    catch (Exception ex)
                    {

                    }
                }
                #endregion
                return jsonResponse;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
