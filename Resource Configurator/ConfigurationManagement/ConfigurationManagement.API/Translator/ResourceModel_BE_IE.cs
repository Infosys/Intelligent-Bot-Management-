/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using BE =Infosys.Ainauto.ConfigurationManager.BusinessEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IE=Infosys.Solutions.Ainauto.ConfigurationManagement.API.Models;
using Newtonsoft.Json;

namespace Infosys.Solutions.Ainauto.ConfigurationManagement.API.Translator
{
    public class ResourceModel_BE_IE
    {
        public static IE.Resources Resource_BEtoIE(BE.PlatformInfo resourcesBE)
        {
            IE.Resources resources = new IE.Resources();
            var json = JsonConvert.SerializeObject(resourcesBE);
            resources = JsonConvert.DeserializeObject<IE.Resources>(json);
            return resources;
        }

        public static IE.SummaryResourceInfo SummaryResource_BEtoIE(BE.SummaryResourceInfo resourcesBE)
        {
            IE.SummaryResourceInfo resources = new IE.SummaryResourceInfo();
            var json = JsonConvert.SerializeObject(resourcesBE);
            resources = JsonConvert.DeserializeObject<IE.SummaryResourceInfo>(json);
            return resources;
        }

        public static BE.SummaryResourceInfo SummaryResource_IEtoBE(IE.SummaryResourceInfo resourceIE)
        {
            BE.SummaryResourceInfo resourceBE = new BE.SummaryResourceInfo();
            var json = JsonConvert.SerializeObject(resourceIE);
            resourceBE = JsonConvert.DeserializeObject<BE.SummaryResourceInfo>(json);
            return resourceBE;
        }

        public static BE.PlatformInfo Resource_IEtoBE(IE.Resources resourceIE)
        {
            BE.PlatformInfo resourceBE = new BE.PlatformInfo();
            var json = JsonConvert.SerializeObject(resourceIE);
            resourceBE = JsonConvert.DeserializeObject<BE.PlatformInfo>(json);
            return resourceBE;
        }
        public static IE.ResourceTypeConfiguration ResourceType_BEtoIE(BE.ResourceTypeConfiguration resourcesBE)
        {
            IE.ResourceTypeConfiguration resources = new IE.ResourceTypeConfiguration();
            var json = JsonConvert.SerializeObject(resourcesBE);
            resources = JsonConvert.DeserializeObject<IE.ResourceTypeConfiguration>(json);
            return resources;
        }

        public static IE.ObservablesandRemediationPlanDetails ResourceRemediations_BEtoIE(BE.ObservablesandRemediationPlanDetails remediationDetailsBE)
        {
            IE.ObservablesandRemediationPlanDetails remediationDetailsIE = new IE.ObservablesandRemediationPlanDetails();
            var json = JsonConvert.SerializeObject(remediationDetailsBE);
            remediationDetailsIE = JsonConvert.DeserializeObject<IE.ObservablesandRemediationPlanDetails>(json);
            return remediationDetailsIE;
        }

        public static IE.ResourceModel ResourceModel_BEtoIE(BE.ResourceModel resourcesBE)
        {
            IE.ResourceModel resourceModel = new IE.ResourceModel();
            var json = JsonConvert.SerializeObject(resourcesBE);
            resourceModel = JsonConvert.DeserializeObject<IE.ResourceModel>(json);
            return resourceModel;
        }
        public static BE.action action_IEtoBE(IE.action actionObj)
        {
            BE.action obs = new BE.action();
            var json = JsonConvert.SerializeObject(actionObj);
            obs = JsonConvert.DeserializeObject<BE.action>(json);
            return obs;
        }
        public static IE.action action_BEtoIE(BE.action actionobj)
        {
            IE.action actionobj1 = new IE.action();
            var json = JsonConvert.SerializeObject(actionobj);
            actionobj1 = JsonConvert.DeserializeObject<IE.action>(json);
            return actionobj1;
        }
        public static IE.observable observable_BEtoIE(BE.observable observable)
        {
            IE.observable obs = new IE.observable();
            var json = JsonConvert.SerializeObject(observable);
            obs = JsonConvert.DeserializeObject<IE.observable>(json);
            return obs;
        }

        public static BE.observable observable_IEtoBE(IE.observable obserbvable)
        {
            BE.observable obs = new BE.observable();
            var json = JsonConvert.SerializeObject(obserbvable);
            obs = JsonConvert.DeserializeObject<BE.observable>(json);
            return obs;
        }
        public static IE.actionModelMap actionModel_BEtoIE(BE.actionModelMap observable)
        {
            IE.actionModelMap obs = new IE.actionModelMap();
            var json = JsonConvert.SerializeObject(observable);
            obs = JsonConvert.DeserializeObject<IE.actionModelMap>(json);
            return obs;
        }
        public static IE.resourceTypeModelMap resourceType_BEtoIE(BE.resourceTypeModelMap observable)
        {
            IE.resourceTypeModelMap obs = new IE.resourceTypeModelMap();
            var json = JsonConvert.SerializeObject(observable);
            obs = JsonConvert.DeserializeObject<IE.resourceTypeModelMap>(json);
            return obs;
        }
        public static IE.observablemodelmap observablemodel_BEtoIE(BE.observablemodelmap observable)
        {
            IE.observablemodelmap obs = new IE.observablemodelmap();
            var json = JsonConvert.SerializeObject(observable);
            obs = JsonConvert.DeserializeObject<IE.observablemodelmap>(json);
            return obs;
        }

        public static IE.actionTypeDetails actionTypeDetails_BEtoIE(BE.actionTypeDetails observable)
        {
            IE.actionTypeDetails obs = new IE.actionTypeDetails();
            var json = JsonConvert.SerializeObject(observable);
            obs = JsonConvert.DeserializeObject<IE.actionTypeDetails>(json);
            return obs;
        }
        public static IE.ObservableResourceTypeActionMap ResourceTypeActionMap_BEtoIE(BE.ObservableResourceTypeActionMap observable)
        {
            IE.ObservableResourceTypeActionMap obs = new IE.ObservableResourceTypeActionMap();
            var json = JsonConvert.SerializeObject(observable);
            obs = JsonConvert.DeserializeObject<IE.ObservableResourceTypeActionMap>(json);
            return obs;
        }
        public static BE.ObservableResourceTypeActionMap remediationPlan_IEtoBE(IE.ObservableResourceTypeActionMap obserbvable)
        {
            BE.ObservableResourceTypeActionMap obs = new BE.ObservableResourceTypeActionMap();
            var json = JsonConvert.SerializeObject(obserbvable);
            obs = JsonConvert.DeserializeObject<BE.ObservableResourceTypeActionMap>(json);
            return obs;
        }
        public static IE.RemediationPlanDetails remediationPlan_BEtoIE(BE.RemediationPlanDetails observable)
        {
            IE.RemediationPlanDetails obs = new IE.RemediationPlanDetails();
            var json = JsonConvert.SerializeObject(observable);
            obs = JsonConvert.DeserializeObject<IE.RemediationPlanDetails>(json);
            return obs;
        }
        public static BE.RemediationPlanDetails remediationPlan_IEtoBE(IE.RemediationPlanDetails obserbvable)
        {
            BE.RemediationPlanDetails obs = new BE.RemediationPlanDetails();
            var json = JsonConvert.SerializeObject(obserbvable);
            obs = JsonConvert.DeserializeObject<BE.RemediationPlanDetails>(json);
            return obs;
        }

        public static IE.RemediationPlanObservableAndResourceTypeMap RemediationPlanObservableAndResourceTypeMap_BEtoIE(BE.RemediationPlanObservableAndResourceTypeMap observable)
        {
            IE.RemediationPlanObservableAndResourceTypeMap obs = new IE.RemediationPlanObservableAndResourceTypeMap();
            var json = JsonConvert.SerializeObject(observable);
            obs = JsonConvert.DeserializeObject<IE.RemediationPlanObservableAndResourceTypeMap>(json);
            return obs;
        }

        public static BE.ObservableResourceTypeActionMap observableResourceTypeActionMap_IEtoBE(IE.ObservableResourceTypeActionMap obserbvable)
        {
            BE.ObservableResourceTypeActionMap obs = new BE.ObservableResourceTypeActionMap();
            var json = JsonConvert.SerializeObject(obserbvable);
            obs = JsonConvert.DeserializeObject<BE.ObservableResourceTypeActionMap>(json);
            return obs;
        }

        public static BE.RemediationPlanObservableAndResourceTypeMap RemediationPlanObservableAndResourceTypeMap_IEtoBE(IE.RemediationPlanObservableAndResourceTypeMap obserbvable)
        {
            BE.RemediationPlanObservableAndResourceTypeMap obs = new BE.RemediationPlanObservableAndResourceTypeMap();
            var json = JsonConvert.SerializeObject(obserbvable);
            obs = JsonConvert.DeserializeObject<BE.RemediationPlanObservableAndResourceTypeMap>(json);
            return obs;
        }

        public static List<IE.PortfolioInfo> Portfolios_BE_IE(List<BE.PortfolioInfo> portfoliosBE)
        {
            List<IE.PortfolioInfo> portfoliosIE = new List<IE.PortfolioInfo>();
            var json = JsonConvert.SerializeObject(portfoliosBE);
            portfoliosIE = JsonConvert.DeserializeObject<List<IE.PortfolioInfo>>(json);
            return portfoliosIE;
        }

        public static List<IE.PortfolioResourceTypeInfo> Portfolios_BE_IEnew(List<BE.PortfolioResourceTypeInfo> portfoliosBE)
        {
            List<IE.PortfolioResourceTypeInfo> portfoliosIE = new List<IE.PortfolioResourceTypeInfo>();
            var json = JsonConvert.SerializeObject(portfoliosBE);
            portfoliosIE = JsonConvert.DeserializeObject<List<IE.PortfolioResourceTypeInfo>>(json);     
            return portfoliosIE;
        }


        public static List<IE.PlatformDetails> PlatformDetails_BE_IE(List<BE.PlatformDetails> platformDetailsListBE)
        {
            List<IE.PlatformDetails> platformDetailsListIE = new List<IE.PlatformDetails>();
            foreach (BE.PlatformDetails platformDetailsBE in platformDetailsListBE)
            {
                IE.PlatformDetails platformDetailsIE = new IE.PlatformDetails()
                {
                    PlatformId = platformDetailsBE.PlatformId,
                    PlatformTypeName = platformDetailsBE.PlatformTypeName,
                    PlatformInstanceName = platformDetailsBE.PlatformInstanceName,
                    ResourceTypeId = platformDetailsBE.ResourceTypeId,
                    ResourceTypeName = platformDetailsBE.ResourceTypeName
                };

                platformDetailsListIE.Add(platformDetailsIE);
            }
            

            return platformDetailsListIE;
        }

        public static BE.ResourceModelGenerationReqMsg ResourceModelGenerationReqMsg_IEtoBE(IE.ResourceModelGenerationReqMsg objIE)
        {
            BE.ResourceModelGenerationReqMsg objBE = new BE.ResourceModelGenerationReqMsg()
            {
                Tenantid = objIE.Tenantid,
                Platformname = objIE.Platformname,
                Platformtype = objIE.Platformtype,
                HostName = objIE.HostName,
                IPAddress = objIE.IPAddress,
                Database_HostName = objIE.Database_HostName,
                Database_Type = objIE.Database_Type,
                Database_IPaddress = objIE.Database_IPaddress,
                Database_Name = objIE.Database_Name,
                Database_UserName = objIE.Database_UserName,
                Database_Password = objIE.Database_Password,
                API_URL = objIE.API_URL,
                API_UserName = objIE.API_UserName,
                API_Password = objIE.API_Password,
                Service_UserName = objIE.Service_UserName,
                Service_Password = objIE.Service_Password
            };

            return objBE;
        }

        public static List<IE.TenantInfo> TenantInfo_BEtoIE(List<BE.TenantInfo> TenantsListBE)
        {
            List<IE.TenantInfo> TenantInfoListIE = new List<IE.TenantInfo>();

            foreach (BE.TenantInfo tiBE in TenantsListBE)
            {
                IE.TenantInfo tenantObjIE = new IE.TenantInfo()
                {
                    tenantid = tiBE.tenantid,
                    name = tiBE.name,
                    tenantConfig = tiBE.tenantConfig,
                   
                };

                TenantInfoListIE.Add(tenantObjIE);
            }
            return TenantInfoListIE;
        }


        public static IE.ResourceModelGenerationResMsg ResourceModelGenerationResMsg_BEtoIE(BE.ResourceModelGenerationResMsg objBE)
        {
            IE.ResourceModelGenerationResMsg objIE = new IE.ResourceModelGenerationResMsg()
            {
                Tenantid = objBE.Tenantid,
                PlatformId = objBE.PlatformId,
                ResourceTypeName = objBE.ResourceTypeName
            };

            return objIE;
        }

        public static List<IE.RPAType> AutomationTypes_BEtoIE(List<BE.RPAType> typesBE)
        {
            List<IE.RPAType> typesIE = new List<IE.RPAType>();
            var json = JsonConvert.SerializeObject(typesBE);
            typesIE = JsonConvert.DeserializeObject<List<IE.RPAType>>(json);
            return typesIE;
        }
    }
}