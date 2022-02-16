/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using Infosys.Solutions.Ainauto.Resource.DataAccess.Queue;
using DE=Infosys.Solutions.Superbot.Resource.Entity.Queue;
using DE2 = Infosys.Solutions.Superbot.Resource.Entity;
using Infosys.Solutions.Ainauto.Processes;
using Infosys.Solutions.Ainauto.Superbot.BusinessComponent;
using BE=Infosys.Solutions.Ainauto.BusinessEntity;
using BE2 = Infosys.Solutions.Ainauto.Superbot.BusinessEntity;
using Infosys.Solutions.Superbot.Infrastructure.Common;
using Infosys.Solutions.Ainauto.Resource.DataAccess;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System;
using Infosys.Solutions.Ainauto.Services.Superbot.Contracts.Message;
using SE = Infosys.Solutions.Ainauto.Services.Superbot.Contracts.Data;
using Infosys.Solutions.Ainauto.Superbot.Infrastructure.ServiceClientLibrary;
using ES = Infosys.Solutions.Ainauto.Resource.DataAccess.ElasticSearch;
using Infosys.Solutions.Superbot.Infrastructure.Common.PerfMon;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.Reflection;

namespace GenericTester
{
    public class Sample
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
    class Tester
    {
        static void Main(string[] args)
        {
            new Tester();
        }

        Tester()
        {
           
            //testJson();
            //RoughMethod();
            //NotifySummary();
            //DeleteIndex();
            //ESInsertUtil();
            //ElasticSearchTest();
           // TestSelfHealing();
            //EnvironmentScanAnomalyTester();
            //FacadeTester();
            //GetES();
            //TestSendQueue();
            //NotificationTester();
            //ExecuteSEE();
            //FacadeClient facadeClient = new FacadeClient();
            //Console.WriteLine("Created object for facade client");
            //List<DataRow> dtlist = facadeClient.GeHealthCheckPlatformDBMetrics();
            //Console.WriteLine("Fetched data from DB Adapter");
            //Console.WriteLine("No.of Rows fetched in facade framework:" + dtlist.Count);
        }

        static void TestEF()
        {
            try
            {
                ResourceDS obj = new ResourceDS();
                var res = obj.GetAll();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        static void testJson()
        {
            List<Sample> sample = new List<Sample>()
            {
                new Sample()
                {
                    Id = 1,
                    Name = "Res_Name_1",
                    Type = "Res_Type_1"
                },
                new Sample()
                {
                    Id = 2,
                    Name = "Res_Name_2",
                    Type = "Res_Type_2"
                },
                new Sample()
                {
                     Id = 2,
                    Name = "Res_Name_2",
                    Type = "Res_Type_2"
                },
                new Sample()
                {
                    Id = 4,
                    Name = "Res_Name_4",
                    Type = "Res_Type_4"
                }
            };

            var res = (from ress in sample
                       group ress by new
                       {
                           ress.Id,
                           ress.Name,
                           ress.Type
                       } into res2
                       select new
                       {
                           Id = res2.Key.Id,
                           Name = res2.Key.Name,
                           Type = res2.Key.Type
                       }
                       ).ToList();
            Console.WriteLine();
        }
        static void SomeMet(int p,CancellationTokenSource cs)
        {
            try
            {
                Console.WriteLine($"Started :{p} and Thread: {Thread.CurrentThread.ManagedThreadId}");
                if (p == 6)
                {
                    Console.WriteLine("calling cancel from 3");
                    cs.Cancel();
                }
                //if (cs.IsCancellationRequested)
                //    throw new OperationCanceledException();
                Thread.Sleep(5000);


                //if (cs.IsCancellationRequested)
                //    throw new OperationCanceledException();
                Console.WriteLine($"Completed :{p} and Thread: {Thread.CurrentThread.ManagedThreadId}");

            }
            catch(OperationCanceledException ex)
            {
                Console.WriteLine($"Operation cancelled Exception thrown for Task: {p} on thread Id: {Thread.CurrentThread.ManagedThreadId}");
                //throw ex;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Other Exception thrown for Task: {p} on thread Id: {Thread.CurrentThread.ManagedThreadId}. Exception: {ex.Message}");
            }
            
        }
        static void RoughMethod()
        {
            Dictionary<string, int> someDict = new Dictionary<string, int>();
            someDict.Add("A", 1);
            someDict.Add("B", 2);
            someDict.Add("C", 3);
            Console.WriteLine(JsonConvert.SerializeObject(someDict));
        }
        
        private string DoSome(string str,Dictionary<string,string> elem)
        {
            string ret = string.Empty;
            foreach (var e in elem)
            {
                if (str.Contains(e.Key))
                {
                    str = str.Replace(string.Format("<<{0}>>", e.Key), e.Value);
                }
            }
            str = str.Replace("<<>>/","");
            return str;
        }
        static bool NoRestriction(List<DE2.notification_configuration> ncTable, string refKey, string refVal)
        {
            bool status = true;

            var res = (from nc in ncTable
                       where nc.ReferenceKey == refKey
                       && nc.ReferenceValue == refVal
                       select nc).ToList();

            if (res.Count > 0)
                status = false;

            return status;
        }
        static void NotifySummary()
        {
            DE.Notification not = new DE.Notification()
            {
                ObservationId = 177,
                PlatformId = 3,
                ResourceId = "2_1",
                ResourceTypeId = 1,
                ObservableId = 41,
                ObservableName = "High Load Query",
                ObservationStatus = "FAILED",
                Value = "0",
                ThresholdExpression = "12",
                ServerIp = "serverip",
                ObservationTime = "1/1/2019 12:45p",
                Description = "Server not reachable",
                EventType = "anomaly",
                Source = "Platform",
                TenantId = 1,
                Type = (int)NotificationType.Summary,
                Channel = 1,
                BaseURL = "",
                ConfigId = "bp_ist",
                PortfolioId = "",//01_8
                ApplicationName = "HealthCheck", //HealthCheck
                TransactionId = "HC20200617104513"
            };

            Notification n = new Notification();
            n.Process(not, 1, 1, 1);
            //NotificationBuilder nb = new NotificationBuilder();
            //nb.BuildSummaryNotification(not);
            //NotificationConfiguration nc = new NotificationConfiguration();
            //nc.GetRecipientConfigDetails("3_1",1,"EMAIL");
        }

        public static bool CheckCondition(string obsVar,string inputVar)
        {
            if (obsVar != null && obsVar != "")
            {
                if (inputVar != null && inputVar != "")
                {
                    if (obsVar.Equals(inputVar, StringComparison.InvariantCultureIgnoreCase))
                        return true;
                    else
                        return false;
                }
                else
                    return true;
            }
            else
                return false;
            
            
        }

        static void DeleteIndex()
        {
            ES.ElasticSearch es = new ES.ElasticSearch();
            var res = es.DeleteIndex("index_health_check_new");
        }
        static void ESInsertUtil()
        {
            Utility util = new Utility();
            bool ret = util.PushDataToElasticSearch(3, "index_health_check_test");
        }
        static void GetES()
        {
            ES.ElasticSearch es = new ES.ElasticSearch();
            var res = es.GetDocx("index_health_check","1_3_2");
            Console.WriteLine(res.Count);

            foreach (var r in res)
            {
                Console.WriteLine(r.ConfigId);
            }

            Console.Read();
        }
        private static void ElasticSearchTest()
        {
            ES.ElasticSearch es = new ES.ElasticSearch();
            ES.Model.ElasticSearchInputTest objES = new ES.Model.ElasticSearchInputTest();
            objES.ConfigId = "test configid";
            objES.PortfolioId = "test portfolioId";
            objES.ResourceId = "test resId";
            objES.ResourceName = "test res name";
            objES.ObservabeId = "test obs id";
            objES.ObservableName = "test obs name";
            objES.Count = "test count";
            objES.MetricValue = "test metric val";
            objES.MetricValueNumber = 3443;
            objES.MetricTime = new DateTime(2020,6,5);
            objES.MetricTimeString = "test configid";
            objES.ResourceTypeId = "test configid";
            objES.ResourceTypeName = "test configid";
            objES.IncidentId = "test configid";
            objES.IncidentCreateTime = new DateTime(2020, 6, 5);
            objES.ServerState = "test configid";
            objES.IsCritical = 1;
            objES.IsWarning = 0;
            objES.IsHealthy = 0;
            objES.LowerThreshold = "test configid";
            objES.UpperThreshold = "test configid";
            bool st = es.InsertWithManualMapping(objES, "test_index_healthcheck");
            Console.WriteLine(st);
            Console.Read();
        }
        
        private static void ExecuteSEE()
        {
            
        }
        private static void ExecuteSEE(int scriptId, int catId, List<SE.Parameter> parameters)
        {
            InitiateExecutionReqMsg executionReqMsg = new InitiateExecutionReqMsg();
            SE.ScriptIdentifier script = new SE.ScriptIdentifier();
            //script.ScriptId = 762;
            //script.CategoryId = 9;
            script.ScriptId = scriptId;
            script.CategoryId = catId;
            script.CompanyId =1;
            script.RemoteServerNames = "hostaddress";
            script.UserName = "username";
            script.Password = "password";
            script.RemoteExecutionMode = 1;
            script.Parameters = parameters;
            executionReqMsg.ScriptIdentifier = script;

            ScriptExecute scriptExecute = new ScriptExecute();
            var channel1 = scriptExecute.ServiceChannel;
            InitiateExecutionResMsg response = channel1.InitiateExecution(executionReqMsg);
            Console.WriteLine( $"\n Result for {scriptId} :\n {response.ScriptResponse.FirstOrDefault().SuccessMessage}\n *************************************************************************************** \n "  );
        }
        private void EnvironmentScanAnomalyTester()
        {
           
            string ins = string.Empty;
            string os = string.Empty;
            string sr = string.Empty;

            //EnvironmentScanMetricAnalyser ba = new EnvironmentScanMetricAnalyser();
            //ba.GetConsolidatedEnvironmentScanReport(1, 1, out ins,out os,out sr);
            //NotificationBuilder nb = new NotificationBuilder();
            //nb.GetEnvironmentScanAnomalyDetails("1_1", 6182, 1, 1);
        }


        private void TestSendQueue()
        {
            DE.MetricMessage metricMessage = new DE.MetricMessage();
            metricMessage.MetricMessages = new List<DE.Metric>();

            for (int i=0;i<1;i++)
            {
                DE.Metric metric = new DE.Metric()
                {
                    ConfigId = null,
                    ResourceId = "1_2_6",
                    MetricName = "Service Status",
                    Count = 0,
                    MetricValue = "unhealthy",
                    MetricTime = "05/19/2021 09:29 AM",
                    Serverstate = "NA",
                    EventType = "Service",
                    Application = "DemoAAPlatform",
                    ServerIp = "serverip",
                    Source = "System",
                    Description = "NA"
                };
                metricMessage.MetricMessages.Add(metric);
            }


            //MetricProcessorDS mpds = new MetricProcessorDS();
            //mpds.Send(metricMessage, "");

            MetricProcessor mp = new MetricProcessor();
            mp.Process(metricMessage, 1, 1, 1);
            //for (int i=0;i<10;i++)
            //{

            //    //mp.Process(metricMessage, 1, 1, 1);
            //}




            //MetricProcessor mp = new MetricProcessor();
            //mp.Process(metricMessage, 1, 1, 1);
        }
        private void TestSelfHealing()
        {
            DE.Anomaly anomaly = new DE.Anomaly()
            {
                ObservationId = 4,
                PlatformId = "1",
                ResourceId = "1_2_6",
                ResourceTypeId = 5,
                ObservableId = 5,
                ObservableName = "Service Status",
                ObservationStatus = "Failed",
                Value = "unhealthy",
                ThresholdExpression = "12",
                ServerIp = "serverip",
                ObservationTime = "1/1/2019 12:45p",
                Description = "Actual error msg if down",
                EventType = "anomaly",
                Source = "Platform"
            };


            SelfHealing selfHealing = new SelfHealing();
            selfHealing.Process(anomaly, 1, 1, 1);

            //var act = new Infosys.Solutions.Ainauto.Superbot.BusinessComponent.Action().GetDetailJson(@"Templates\Servicenow\DetailsTemplate.json",anomaly);
        }

        //private void FacadeTester()
        //{
        //   FacadeClient facadeClient = new FacadeClient();
        //    List<DataRow> dtResult = facadeClient.GeHealthCheckPlatformDBMetrics();
        //    var results = (from myRow in dtResult.AsEnumerable()

        //                   select myRow).FirstOrDefault();

        //}
        private void NotificationTester()
        {
            //NotificationConfiguration nc = new NotificationConfiguration();
            //nc.GetSMTPconfigIntermediate( 1, 1,"EMAIL");

            //NotificationBuilder nb = new NotificationBuilder();
            //nb.GetEnvironmentScanAnomalyDetails("1_1", 1234, 1, 1);
            DE.Notification not = new DE.Notification()
            {
               
            };

            NotificationDS notificationDS = new NotificationDS();
            notificationDS.Send(not, "");
        }
    }
}
