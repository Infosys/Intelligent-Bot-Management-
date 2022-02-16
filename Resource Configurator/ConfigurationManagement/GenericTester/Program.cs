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
using Infosys.Solutions.Ainauto.ConfigurationManager.BusinessEntity;
using Infosys.Ainauto.ConfigurationManager.BusinessEntity;
using Infosys.Solutions.Ainauto.ConfigurationManager.BusinessComponent;
using RM = Infosys.Ainauto.ConfigurationManager.BusinessComponent;
using Infosys.Solutions.Ainauto.Resource.DataAccess;
using Infosys.Solutions.ConfigurationManager.Resource.Entity;
using Infosys.Ainauto.ConfigurationManager.BusinessComponent;
using System.Data;
using Infosys.Solutions.Ainauto.Resource.DataAccess.Facade;
using Infosys.Solutions.Ainauto.ConfigurationManagement.API.Controllers;
using IE = Infosys.Solutions.Ainauto.ConfigurationManagement.API.Models;


namespace GenericTester
{
    public class Program
    {
        static void Main(string[] args)
        {
             UiPathDiscovery();
            //GetAnomalyRules();
            //InsertAnomalyRules();
            //UpdateAnomalyRules();
            //DeleteAnomalyRules();
            //GetPlatforDetails();
            //ResourceAutoConfigModel();
           // testEncypt();
            //UpdateCategoryAndScriptDetails();
            //TestFacade();
            //RoughtMethod();
            //GetRemDetails();
        }
        static void UiPathDiscovery()
        {
            DiscoveryUiPath ui = new DiscoveryUiPath();
            ResourceModelConfigInput obj = new ResourceModelConfigInput();
            obj.name = "UiPath";
            obj.tenantId = "1";
            obj.labels = new List<Labels> { new Labels { name = "Platform Name", type = "text", value = "DemoUiPath105" } };
           


            var res  = ui.GenerateUiPathResourceModel(obj);
            Console.Read();
        }
        static void RoughtMethod()
        {
            UiPathService obj = new UiPathService();
          

            Console.Read();
        }
        private static void TestFacade()
        {
            FacadeClient fc = new FacadeClient();
            var result = fc.GetScriptList("scc_servername", "WEMScriptExecutor_Superbot/WEMScriptService.svc/GetAllScriptDetails/187");
            Console.WriteLine(result);
        }
        private static void UpdateCategoryAndScriptDetails()
        {
            CategoryAndScriptDetails obj = new CategoryAndScriptDetails()
            {
                TenantId = "1",
                SEEBaseUrl = "scc_servername",
                CategoryServiceName = "WEMScriptExecutor_Superbot/WEMCommonService.svc/GetAllCategoriesByCompany?companyId=1&module=2",
                ScriptDetailServiceName = "WEMScriptExecutor_Superbot/WEMScriptService.svc/GetAllScriptDetails/",
                AutomationEngineName = ""
            };
            ResourceModelBuilder resourceModelBuilder = new ResourceModelBuilder();
            resourceModelBuilder.UpdateCategoryandScriptId(obj);
           
        }

        private static void GetRemDetails()
        {
            RemediationPlanBuilder rm = new RemediationPlanBuilder();
            var res = rm.getRemediationPlanDetails(1);
        }

        private static void ResourceAutoConfigModel()
        {
            ResourceModelGenerationReqMsg msg = new ResourceModelGenerationReqMsg();
           


            ResourceModelBuilder resourceModelBuilder = new ResourceModelBuilder();
            resourceModelBuilder.GenerateResourceModel(msg);

            //ResourceModelBuilder resourceModelBuilder = new ResourceModelBuilder();
            //DataTable viewResult = resourceModelBuilder.ExecuteDBView(msg);

            Console.Read();
            

        }
        private static void testEncypt()
        {
            string testVar= ResourceModelBuilder.Encrypt("Infosys");
            Console.WriteLine(testVar);

        }
        private static void GetPlatforDetails()
        {
            var platform = new PlatformsDS().Insert(new platforms() { PlatformName = "sample platform", ExecutionMode="1",CreateDate=DateTime.UtcNow,CreatedBy="syed",TenantId=1});
            Console.WriteLine(platform.PlatformId);
            Console.ReadLine();
            //RM.ResourceModelBuilder rm = new RM.ResourceModelBuilder();
            //rm.GetPlatformDetails(1);
        }
        public static void GetAnomalyRules()
        {

        }
        public static void InsertAnomalyRules()
        {
            LogDetail logDetail = new LogDetail();
            logDetail.CreatedBy = "arman";
            logDetail.CreateDate = DateTime.UtcNow;
            logDetail.ValidityStart = DateTime.UtcNow;
            logDetail.ValidityEnd = DateTime.UtcNow;

            AnomalyDetectionRule anomalyDetectionRule = new AnomalyDetectionRule();
            anomalyDetectionRule.ObservableId = 42;
            anomalyDetectionRule.ResourceId = "1_3";
            anomalyDetectionRule.ResourceName = "my Resource";
            anomalyDetectionRule.ObservableName = "My Observable";
            anomalyDetectionRule.ResourceTypeId = 55;
            anomalyDetectionRule.ResourceTypeName = "My res type";
            anomalyDetectionRule.OperatorId = 55;
            anomalyDetectionRule.Operator = "%";
            anomalyDetectionRule.LowerThreshold = "low";
            anomalyDetectionRule.UpperThreshold = "up";
            anomalyDetectionRule.LogDetails = logDetail;

            List<AnomalyDetectionRule> list = new List<AnomalyDetectionRule>();
            list.Add(anomalyDetectionRule);

            AnomalyRulesDetails anomalyRulesDetails = new AnomalyRulesDetails
            {
                TenantId = 1,
                PlatformId = 1,
                AnomalyDetectionRules = list
            };

            AnomalyRulesConfigurationBuilder builder = new AnomalyRulesConfigurationBuilder();
            builder.InsertAnomalyRulesConfig(anomalyRulesDetails);
            Console.WriteLine("Insert is done");
        }

        public static void UpdateAnomalyRules()
        {
            LogDetail logDetail = new LogDetail();
            logDetail.CreatedBy = "arman";
            logDetail.CreateDate = DateTime.UtcNow;
            logDetail.ValidityStart = DateTime.UtcNow;
            logDetail.ValidityEnd = DateTime.UtcNow;

            AnomalyDetectionRule anomalyDetectionRule = new AnomalyDetectionRule();
            anomalyDetectionRule.ObservableId = 2;
            anomalyDetectionRule.ResourceId = "1_2";
            anomalyDetectionRule.ResourceName = "my Resource";
            anomalyDetectionRule.ObservableName = "My Observable";
            anomalyDetectionRule.ResourceTypeId = 1;
            anomalyDetectionRule.ResourceTypeName = "My res type";
            anomalyDetectionRule.OperatorId = 1;
            anomalyDetectionRule.Operator = ">";
            anomalyDetectionRule.LowerThreshold = "new updated low";
            anomalyDetectionRule.UpperThreshold = "new updated up";
            anomalyDetectionRule.LogDetails = logDetail;

            List<AnomalyDetectionRule> list = new List<AnomalyDetectionRule>();
            list.Add(anomalyDetectionRule);

            AnomalyRulesDetails anomalyRulesDetails = new AnomalyRulesDetails
            {
                TenantId = 1,
                PlatformId = 1,
                AnomalyDetectionRules = list
            };

            AnomalyRulesConfigurationBuilder builder = new AnomalyRulesConfigurationBuilder();
            var retr = builder.UpdateAnomalyRuleConfig(anomalyRulesDetails);
            Console.WriteLine(retr);
        }

        public static void DeleteAnomalyRules()
        {
            LogDetail logDetail = new LogDetail();
            logDetail.CreatedBy = "arman";
            logDetail.CreateDate = DateTime.UtcNow;
            logDetail.ValidityStart = DateTime.UtcNow;
            logDetail.ValidityEnd = DateTime.UtcNow;

            AnomalyDetectionRule anomalyDetectionRule = new AnomalyDetectionRule();
            anomalyDetectionRule.ObservableId = 42;
            anomalyDetectionRule.ResourceId = "1_3";
            anomalyDetectionRule.ResourceName = "my Resource";
            anomalyDetectionRule.ObservableName = "My Observable";
            anomalyDetectionRule.ResourceTypeId = 55;
            anomalyDetectionRule.ResourceTypeName = "My res type";
            anomalyDetectionRule.OperatorId = 88;
            anomalyDetectionRule.Operator = "$";
            anomalyDetectionRule.LowerThreshold = "new updated low";
            anomalyDetectionRule.UpperThreshold = "new updated up";
            anomalyDetectionRule.LogDetails = logDetail;

            List<AnomalyDetectionRule> list = new List<AnomalyDetectionRule>();
            list.Add(anomalyDetectionRule);

            AnomalyRulesDetails anomalyRulesDetails = new AnomalyRulesDetails
            {
                TenantId = 1,
                PlatformId = 1,
                AnomalyDetectionRules = list
            };

            AnomalyRulesConfigurationBuilder builder = new AnomalyRulesConfigurationBuilder();
            var retr = builder.DeleteAnomalyRuleConfig(anomalyRulesDetails);
            Console.WriteLine(retr);
        }
    }
}
