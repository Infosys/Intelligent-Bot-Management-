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
using Infosys.Solutions.Ainauto.Resource.DataAccess;
using Infosys.Solutions.ConfigurationManager.Infrastructure.Common;
using Infosys.Solutions.ConfigurationManager.Resource.Entity;
using BE = Infosys.Solutions.Ainauto.ConfigurationManager.BusinessEntity;
using BE2 = Infosys.Ainauto.ConfigurationManager.BusinessEntity;
using Infosys.Ainauto.ConfigurationManager.BusinessComponent;

namespace Infosys.Ainauto.ConfigurationManager.BusinessComponent
{
    public class DiscoveryUiPath
    {
        ResourceDS resourceDS;
        ResourceAttributesDS resourceAttributesDS;
        ResourceDependencyMapDS resourceDependencyMapDS;
        ResourceTypeMetadataDS resourceTypeMetadataDS;
        ResourceTypeDependencyMapDS resourceTypeDependencyMapDS;
        ResourceTypeServiceDetailsDS resourceTypeServiceDetailsDS;
        ResourceModelBuilder rmb;

        int tenantId;
        int platformId;
        string currentResourceId = string.Empty;
        int serverLevelIncrementer;
        int serviceLevelIncrementer;

        public DiscoveryUiPath()
        {
            resourceDS = new ResourceDS();
            resourceAttributesDS = new ResourceAttributesDS();
            resourceDependencyMapDS = new ResourceDependencyMapDS();
            resourceTypeMetadataDS = new ResourceTypeMetadataDS();
            resourceTypeDependencyMapDS = new ResourceTypeDependencyMapDS();
            resourceTypeServiceDetailsDS = new ResourceTypeServiceDetailsDS();
            rmb = new ResourceModelBuilder();
        }
        //public BusinessEntity.ResourceModelGenerationResMsg GenerateUiPathResourceModel(BE.ResourceModelConfigInput inpObj)
        //{
        //    tenantId = Convert.ToInt32(inpObj.tenantId);
        //    resource platformResource = PlatformLevelEntry(inpObj);
        //    platformId = (int)platformResource.PlatformId;
        //    currentResourceId = platformId.ToString()+"_";
        //    if (OrchestratorLevelEntry(inpObj))
        //    {
        //        if (RobotLevelEntry(inpObj))
        //        {
        //            string resourceTypeName = new ResourceTypeDS().GetOne(new resourcetype() { ResourceTypeId = platformResource.ResourceTypeId }).ResourceTypeName;

        //            BusinessEntity.ResourceModelGenerationResMsg response = new BusinessEntity.ResourceModelGenerationResMsg()
        //            {
        //                Tenantid = tenantId,
        //                PlatformId = platformId,
        //                ResourceTypeName = resourceTypeName
        //            };

        //            return response;
        //        }
        //    }

        //    return null;

        //}
        private bool CheckDbDetailsAvailability(BE.ResourceModelConfigInput inpObj)
        {
            List<BE.Labels> databaseDetailsList = (from s in inpObj.sections where s.name == "Database Details" select s.labels).FirstOrDefault();
            if (databaseDetailsList != null && databaseDetailsList.Count > 0)
                return true;
            else
                return false;
        }
        public BusinessEntity.ResourceModelGenerationResMsg GenerateUiPathResourceModel(BE.ResourceModelConfigInput inpObj)
        {
            ResourceModelBuilder rmb = new ResourceModelBuilder();
            tenantId = Convert.ToInt32(inpObj.tenantId);
            resource platformResource = PlatformLevelEntry(inpObj);
            platformId = (int)platformResource.PlatformId;
            currentResourceId = platformId.ToString() + "_";
            if (OrchestratorLevelEntry(inpObj))
            {
                if (RobotLevelEntry(inpObj))
                {
                    #region DB Entry
                    if (CheckDbDetailsAvailability(inpObj))
                    {
                        var inp = TranslateMethod(inpObj);
                        rmb.DBEntry(inp, currentResourceId,serverLevelIncrementer, platformId);
                    }
                    #endregion
                    string resourceTypeName = new ResourceTypeDS().GetOne(new resourcetype() { ResourceTypeId = platformResource.ResourceTypeId }).ResourceTypeName;

                    BusinessEntity.ResourceModelGenerationResMsg response = new BusinessEntity.ResourceModelGenerationResMsg()
                    {
                        Tenantid = tenantId,
                        PlatformId = platformId,
                        ResourceTypeName = resourceTypeName
                    };

                    return response;
                                        
                }
            }

            return null;

        }

        private resource PlatformLevelEntry(BE.ResourceModelConfigInput inpObj)
        {            
            try
            {
                PlatformsDS platformsDS = new PlatformsDS();                

                string platformName = (from o in inpObj.labels where o.name == "Platform Name" select o.value).FirstOrDefault();

                platforms platformDetails = platformsDS.Insert(new platforms() { PlatformName = platformName, PlatformType = inpObj.name, ExecutionMode = "1", CreatedBy = "admin", CreateDate = DateTime.UtcNow, TenantId = tenantId });

                if (platformDetails != null)
                {
                    resource resourceDetails = new resource()
                    {
                        ResourceId = platformDetails.PlatformId.ToString(),
                        ResourceName = platformName,
                        ResourceTypeId = rmb.GetResourceType(inpObj.name, System.Configuration.ConfigurationManager.AppSettings["Platform"]).ResourceTypeId,
                        Source = string.Empty,
                        ValidityStart = DateTime.UtcNow,
                        ValidityEnd = new DateTime(2099, 08, 22),
                        TenantId = tenantId,
                        PlatformId = platformDetails.PlatformId,
                        VersionNumber = "1",
                        IsActive = false
                    };

                    resourceDetails = resourceDS.Insert(resourceDetails);

                    if (resourceDetails != null)
                    {
                        platformId = platformDetails.PlatformId;

                        bool status = ResourceAttributeEntry(inpObj, resourceDetails, System.Configuration.ConfigurationManager.AppSettings["Platform"]);

                        if (status)
                        {
                            //Inserting into dependency Table
                            var resourceDependencyDetails = ResourceDependencyEntry(inpObj, resourceDetails);
                        }
                        //enter anomaly rules
                        bool AnomalyEntryStatus = AnomalyRulesEntry(resourceDetails);
                    }
                    else
                        platformId = 0;

                    return resourceDetails;
                }                
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return null;

        }

        private bool OrchestratorLevelEntry(BE.ResourceModelConfigInput inpObj)
        {
            bool retStatus = true;
            try
            {
                var orchestratorList = System.Configuration.ConfigurationManager.AppSettings["Orchestrator"].Split(',');
                foreach (var orchestrator in orchestratorList)
                {

                    //insert into resource table -- control tower
                    resourcetype resourceTypeDetails = rmb.GetResourceType(inpObj.name, orchestrator);

                    resource resourceDetails = new resource()
                    {
                        ResourceId = currentResourceId + ++serverLevelIncrementer,
                        ResourceName = orchestrator,
                        ResourceTypeId = resourceTypeDetails.ResourceTypeId,
                        Source = orchestrator,
                        ValidityStart = DateTime.UtcNow,
                        ValidityEnd = new DateTime(2099, 08, 22),
                        TenantId = tenantId,
                        PlatformId = platformId,
                        VersionNumber = "1",
                        IsActive = false
                    };

                    resourceDetails = resourceDS.Insert(resourceDetails);

                    //insert into resource attribute table
                    bool status = ResourceAttributeEntry(inpObj, resourceDetails, orchestrator);
                    if (status)
                    {
                        //Inserting into dependency Table
                        var resourceDependencyDetails = ResourceDependencyEntry(inpObj, resourceDetails);
                    }
                    else
                    {
                        //throw error
                    }
                    
                    bool serviceEntryStatus = ServiceEntry(inpObj, resourceDetails);

                    //enter anomaly rules
                    bool AnomalyEntryStatus = AnomalyRulesEntry(resourceDetails);

                    // set service incrementer to 0
                    serviceLevelIncrementer = 0;


                }
            }
            catch(Exception ex)
            {
                retStatus = false;
            }

            return retStatus;
            
        }

        private bool RobotLevelEntry(BE.ResourceModelConfigInput inpObj)
        {
            bool retStatus = true;
            try
            {
                var orchestratorDetailsList = (from i in inpObj.sections where i.name == "Orchestrator Details" select i.labels).SingleOrDefault();
                
                string tenantLogicalName = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "tenantlogicalname" select o.value).FirstOrDefault();
                string accountLogicalName = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "accountlogicalname" select o.value).FirstOrDefault();
                string clientId = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "clientid" select o.value).FirstOrDefault();
                string refreshToken = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "refreshtoken" select o.value).FirstOrDefault();
                

                //UiPathService serviceObj = new UiPathService(clientId,refreshToken,tenantLogicalName);
                UiPathService serviceObj = new UiPathService();
                List<BE.UiPathRobot> robotList = serviceObj.GetRobotDetails(accountLogicalName, tenantLogicalName, orchestratorDetailsList,inpObj.tenantId);
                List<BE.UiPathProcess> processList = serviceObj.GetAllProcesses(accountLogicalName, tenantLogicalName, orchestratorDetailsList, inpObj.tenantId);

                foreach (var robot in robotList)
                {

                    resourcetype rt = rmb.GetResourceType(inpObj.name, robot.Type);
                    if (rt == null)
                    {
                        continue;
                    }
                    resource resourceDetails = new resource()
                    {
                        ResourceId = currentResourceId + ++serverLevelIncrementer,
                        ResourceName = robot.Name,
                        ResourceTypeId = rmb.GetResourceType(inpObj.name, robot.Type).ResourceTypeId,
                        Source = robot.MachineName,
                        ValidityStart = DateTime.UtcNow,
                        ValidityEnd = new DateTime(2099, 08, 22),
                        TenantId = tenantId,
                        PlatformId = platformId,
                        VersionNumber = "1",
                        IsActive = false
                    };

                    resourceDetails = resourceDS.Insert(resourceDetails);

                    bool status = ResourceAttributeEntry(inpObj, resourceDetails,"Robot",robot);

                    if (status)
                    {
                        var res = ResourceDependencyEntry(inpObj, resourceDetails);
                    }
                    //enter anomaly rules
                    bool AnomalyEntryStatus = AnomalyRulesEntry(resourceDetails);
                    var botTypeList = System.Configuration.ConfigurationManager.AppSettings["RobotType"].Split(',');
                    foreach (var botType in botTypeList)

                        if (robot.Type.ToLower() == botType.ToLower())
                    {
                        //enter the process details
                        bool processEntryStatus = ProcessEntry(inpObj, processList, resourceDetails, robot);
                    }

                    bool serviceEntryStatus = ServiceEntry(inpObj, resourceDetails);

                    serviceLevelIncrementer = 0;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retStatus;


        }
        private bool ResourceAttributeEntry(BE.ResourceModelConfigInput inpObj, resource resourceDetails, string level, BE.UiPathRobot robotObj = null, BE.UiPathProcess processObj = null, resourcetype_service_details serviceObj = null)
        {
            bool status = true;

            try
            {
                List<resourcetype_metadata> resourcetype_Metadata_List = resourceTypeMetadataDS.GetAll(new resourcetype_metadata() { ResourceTypeId = resourceDetails.ResourceTypeId }).ToList();

                List<resource_attributes> resource_attributeList = new List<resource_attributes>();

                foreach (var resTypeMetaDataObj in resourcetype_Metadata_List)
                {
                    resource_attributes resAttObj = new resource_attributes();

                    resAttObj.ResourceId = resourceDetails.ResourceId;
                    resAttObj.AttributeName = resTypeMetaDataObj.AttributeName;                    
                    resAttObj.TenantId = tenantId;
                    resAttObj.DisplayName = resTypeMetaDataObj.DisplayName;
                    resAttObj.Description = resTypeMetaDataObj.Description;
                    resAttObj.CreatedBy = "admin";
                    resAttObj.CreateDate = DateTime.UtcNow;
                    resAttObj.IsSecret = resTypeMetaDataObj.issecret;

                    switch (level)
                    {
                        case "Platform":
                            resAttObj.AttributeValue = resTypeMetaDataObj.DefaultValue != null ? resTypeMetaDataObj.DefaultValue : String.Empty;
                            break;
                        case "Orchestrator":
                            resAttObj.AttributeValue = OrchestratorAttributeFinder(inpObj, resTypeMetaDataObj);
                            break;
                        case "Robot":
                            resAttObj.AttributeValue = RobotAttributeFinder(robotObj, resTypeMetaDataObj);
                            break;
                        case "Process":
                            resAttObj.AttributeValue = ProcessAttributeFinder(processObj, resTypeMetaDataObj);
                            break;
                        case "Service":
                            resAttObj.AttributeValue = ServiceAttributeFinder(serviceObj, resTypeMetaDataObj);
                            break;

                    }
                    
                    if (resTypeMetaDataObj.issecret==true && resAttObj.AttributeValue!=null)
                    {
                        

                        resAttObj.AttributeValue =   ResourceModelBuilder.Encrypt(resAttObj.AttributeValue) ;
                    }

                    resource_attributeList.Add(resAttObj);
                }

                




                //Inserting into resource attributes tables
                var result = resourceAttributesDS.InsertBatch(resource_attributeList);

                if (result == null || result.Count == 0)
                {
                    status = false;
                }

            }
            catch (Exception ex)
             {
                throw ex;
            }

            return status;
        }

        private resource_dependency_map ResourceDependencyEntry(BE.ResourceModelConfigInput inputMessage, resource resourceDetails)
        {
            try
            {
                //getting details from resourcetype dependency map table
                List<resourcetype_dependency_map> resourcetype_Dependency_Map = resourceTypeDependencyMapDS.GetAll(new resourcetype_dependency_map() { ResourcetypeId = resourceDetails.ResourceTypeId }).ToList();


                resource_dependency_map resource_Dependency_Map = new resource_dependency_map();
                resource_Dependency_Map.ResourceId = resourceDetails.ResourceId;
                resource_Dependency_Map.Priority = 1;
                resource_Dependency_Map.CreatedBy = "admin";
                resource_Dependency_Map.CreateDate = DateTime.UtcNow;
                resource_Dependency_Map.ValidityStart = DateTime.UtcNow;
                resource_Dependency_Map.ValidityEnd = new DateTime(2099, 08, 01);
                resource_Dependency_Map.TenantId = tenantId;

                if (resourcetype_Dependency_Map == null || resourcetype_Dependency_Map.Count == 0)
                {
                    //platform level
                    resource_Dependency_Map.DependencyResourceId = string.Empty;
                    resource_Dependency_Map.DependencyType = ResourceLevel.LEVEL0.ToString();
                    resource_Dependency_Map.PortfolioId = "";
                }
                else if (resourcetype_Dependency_Map.Count == 1)
                {
                    resource_Dependency_Map.DependencyType = resourcetype_Dependency_Map[0].DependencyType;
                    resource_Dependency_Map.PortfolioId = resourcetype_Dependency_Map[0].PortfolioId!=null? resourcetype_Dependency_Map[0].PortfolioId:"";
                    //getting the resource for the resource type
                    List<resource> dependencyResourceList = new ResourceDSExtended().GetAll(new resource() { ResourceTypeId = Convert.ToInt32(resourcetype_Dependency_Map[0].DependencyResourceTypeId), PlatformId = resourceDetails.PlatformId });

                    if (dependencyResourceList != null && dependencyResourceList.Count == 1)
                    {
                        resource_Dependency_Map.DependencyResourceId = dependencyResourceList[0].ResourceId;
                    }
                    else
                    {
                        // has multiple dependency resources. therefore, using parent resource as dependency resource

                        resource_Dependency_Map.DependencyResourceId = rmb.ExtractParentResource(resourceDetails.ResourceId);
                    }

                }
                else
                {
                    resource_Dependency_Map.DependencyType = resourcetype_Dependency_Map[0].DependencyType;
                    resource_Dependency_Map.PortfolioId = resourcetype_Dependency_Map[0].PortfolioId!=null? resourcetype_Dependency_Map[0].PortfolioId:"";
                    resource_Dependency_Map.DependencyResourceId = rmb.ExtractParentResource(resourceDetails.ResourceId);
                }

                resource_Dependency_Map = resourceDependencyMapDS.Insert(resource_Dependency_Map);

                //LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "ResourceDependencyEntry", "ResourceModelBuilder"), LogHandler.Layer.Business, null);

                return resource_Dependency_Map;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private string OrchestratorAttributeFinder(BE.ResourceModelConfigInput inputMessage, resourcetype_metadata metaDataObj)
        {
            string attributeValue = string.Empty;
            string url = string.Empty;
            try
            {
                var orchestratorDetailsList = (from i in inputMessage.sections where i.name == "Orchestrator Details" select i.labels).SingleOrDefault();
                attributeValue = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == metaDataObj.AttributeName.ToLower().Replace(" ", "") select o.value).FirstOrDefault();

                if(attributeValue == null)
                {
                    Dictionary<string, string> urlElements = new Dictionary<string, string>();

                    string orchestratorURL = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "orchestratorurl" select o.value).FirstOrDefault();
                    urlElements.Add("orchestratorurl", orchestratorURL);
                    string accountLogicalName = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "accountlogicalname" select o.value).FirstOrDefault();
                    urlElements.Add("accountlogicalname", accountLogicalName);
                    string tenantLogicalName = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "tenantlogicalname" select o.value).FirstOrDefault();
                    urlElements.Add("tenantlogicalname", tenantLogicalName);


                    switch (metaDataObj.AttributeName)
                    {
                        case "authenticationURI":
                            attributeValue = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "authenticationurl" select o.value).FirstOrDefault();
                            break;
                        case "clientId":
                            attributeValue = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "clientid" select o.value).FirstOrDefault();
                            break;
                        case "refreshToken":
                            attributeValue = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "refreshtoken" select o.value).FirstOrDefault();
                            break;
                        case "uiPathTenantName":
                            attributeValue = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "uipathtenantname" select o.value).FirstOrDefault();
                            break;
                        case "userName":
                            attributeValue = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "orchestratorusername" select o.value).FirstOrDefault();
                            break;
                        case "password":
                            attributeValue = (from o in orchestratorDetailsList where o.name.ToLower().Replace(" ", "") == "orchestratorpassword" select o.value).FirstOrDefault();
                            break;
                        case "deviceSerivceURI":
                            //<<Orchestrator URL>>/<<Account Logical Name>>/<<Tenant Logical Name>>/odata/sessions
                            //attributeValue = string.IsNullOrEmpty(accountLogicalName) && string.IsNullOrEmpty(tenantLogicalName) ?
                            //    String.Format("{0}/odata/Sessions", orchestratorURL):
                            //    String.Format(metaDataObj.DefaultValue, orchestratorURL, accountLogicalName, tenantLogicalName);
                            attributeValue = ConstructAttributeParam(metaDataObj.DefaultValue, urlElements);
                            break;
                        case "jobStatusServiceURI":
                            //<<Orchestrator URL>>/<<Account Logical Name>>/<<Tenant Logical Name>>/odata/Jobs
                            //attributeValue = string.IsNullOrEmpty(accountLogicalName) && string.IsNullOrEmpty(tenantLogicalName) ?
                            //    String.Format("{0}/odata/Jobs", orchestratorURL):
                            //    String.Format(metaDataObj.DefaultValue, orchestratorURL, accountLogicalName, tenantLogicalName);
                            attributeValue = ConstructAttributeParam(metaDataObj.DefaultValue, urlElements);
                            break;
                        case "startJobServiceURI":
                            //<<Orchestrator URL>>/<<Account Logical Name>>/<<Tenant Logical Name>>/odata/Jobs/UiPath.Server.Configuration.OData.StartJobs
                            //attributeValue = string.IsNullOrEmpty(accountLogicalName) && string.IsNullOrEmpty(tenantLogicalName) ?
                            //    String.Format("{0}/odata/Jobs/UiPath.Server.Configuration.OData.StartJobs", orchestratorURL):
                            //    String.Format(metaDataObj.DefaultValue, orchestratorURL, accountLogicalName, tenantLogicalName);
                            attributeValue = ConstructAttributeParam(metaDataObj.DefaultValue, urlElements);
                            break;
                    }
                  
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return attributeValue!=null?attributeValue:"";
        }

        private string ConstructAttributeParam(string str,Dictionary<string,string> urlElements)
        {
            foreach (var elem in urlElements)
            {
                if (str.Contains(elem.Key) && !String.IsNullOrEmpty(elem.Value))
                {
                    str = str.Replace(string.Format($"<{elem.Key}>"), elem.Value);
                }
            }
            str = str.Replace("<orchestratorurl>/", "");
            str = str.Replace("<accountlogicalname>/", "");
            str = str.Replace("<tenantlogicalname>/", "");
            return str;
        }

        private string RobotAttributeFinder(BE.UiPathRobot robotObj, resourcetype_metadata metaDataObj)
        {
            string attributeValue = string.Empty;
            try
            {
                switch (metaDataObj.AttributeName.ToLower().Trim().Replace(" ",""))
                {
                    case "ip":
                        attributeValue = robotObj.MachineName;
                        break;
                    case "robotid":
                        attributeValue = robotObj.Id;
                        break;
                    default:
                        attributeValue = metaDataObj.DefaultValue != null ? metaDataObj.DefaultValue : string.Empty;
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return attributeValue;
        }

        private bool ProcessEntry(BE.ResourceModelConfigInput inpObj, List<BE.UiPathProcess> processList, resource robotResObj, BE.UiPathRobot robotObj)
        {
            bool retStatus = true;
            try
            {
                foreach (var process in processList)
                {
                    resource resourceDetails = new resource()
                    {
                        ResourceId = robotResObj.ResourceId +"_"+ ++serviceLevelIncrementer,
                        ResourceName = process.Name,
                        ResourceTypeId = rmb.GetResourceType(inpObj.name, process.ProcessType).ResourceTypeId,
                        Source = robotObj.MachineName,
                        ValidityStart = DateTime.UtcNow,
                        ValidityEnd = new DateTime(2099, 08, 22),
                        TenantId = tenantId,
                        PlatformId = platformId,
                        VersionNumber = "1",
                        IsActive = false
                    };

                    resourceDetails = resourceDS.Insert(resourceDetails);

                    bool status = ResourceAttributeEntry(inpObj, resourceDetails,"Process",robotObj,process);

                    if (status)
                    {
                        var res = ResourceDependencyEntry(inpObj,resourceDetails);
                    }
                    //enter anomaly rules
                    bool AnomalyEntryStatus = AnomalyRulesEntry(resourceDetails);
                }
                
            }
            catch (Exception ex)
            {
                retStatus = false;
            }
            return retStatus;
        }

        private bool AnomalyRulesEntry(resource resourceDetails)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "AnomalyRulesEntry", "DiscoveryUiPath"), LogHandler.Layer.Business, null);

            ResourceTypeObservableMapDS resourceTypeObservableMapDS = new ResourceTypeObservableMapDS();
            ObservableResourceMapDS observableResourceMapDS = new ObservableResourceMapDS();
            try
            {
                //getting the rule from resourcetype_observable_map table
                List<resourcetype_observable_map> ResTypeObsList = resourceTypeObservableMapDS.GetAll(new resourcetype_observable_map() { ResourceTypeId = resourceDetails.ResourceTypeId, TenantId = resourceDetails.TenantId }).ToList();

                //inserting into the observable_resource_map table
                List<observable_resource_map> observable_Resource_Map_List = new List<observable_resource_map>();

                foreach (resourcetype_observable_map obj in ResTypeObsList)
                {
                    observable_resource_map observable_Resource_Map = new observable_resource_map()
                    {
                        ObservableId = obj.ObservableId,
                        ResourceId = resourceDetails.ResourceId,
                        ValidityStart = DateTime.UtcNow,
                        ValidityEnd = new DateTime(2099, 08, 01),
                        TenantId = resourceDetails.TenantId,
                        OperatorId = obj.OperatorId,
                        LowerThreshold = obj.LowerThreshold,
                        UpperThreshold = obj.UpperThreshold
                    };

                    observable_Resource_Map_List.Add(observable_Resource_Map);
                }

                observableResourceMapDS.InsertBatch(observable_Resource_Map_List);

                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "AnomalyRulesEntry", "DiscoveryUiPath"), LogHandler.Layer.Business, null);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private string ProcessAttributeFinder(BE.UiPathProcess processObj, resourcetype_metadata metaDataObj)
        {
            string attributeValue = string.Empty;
            try
            {
                switch (metaDataObj.AttributeName.ToLower().Trim().Replace(" ", ""))
                {
                    case "releasekey":
                        attributeValue = processObj.Key;
                        break;
                    case "releasename":
                        attributeValue = processObj.Name;
                        break;
                    default:
                        attributeValue = metaDataObj.DefaultValue != null ? metaDataObj.DefaultValue : string.Empty;
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return attributeValue;
        }

        private bool ServiceEntry(BE.ResourceModelConfigInput inpObj, resource resourceObj)
        {
            bool retStatus = true;
            string resourceTypeName = System.Configuration.ConfigurationManager.AppSettings["Services"];
            int resTypeId = rmb.GetResourceType(inpObj.name, resourceTypeName).ResourceTypeId;

            
            try
            {
                List<resourcetype_service_details> serviceList = resourceTypeServiceDetailsDS.GetAll(new resourcetype_service_details() { ResourceTypeId = resourceObj.ResourceTypeId }).ToList();
                foreach (var serviceObj in serviceList)
                {
                    resource resourceDetails = new resource()
                    {
                        ResourceId = resourceObj.ResourceId + "_" + ++serviceLevelIncrementer,
                        ResourceName = serviceObj.DisplayName,
                        ResourceTypeId = resTypeId,
                        Source = resourceObj.Source,
                        ValidityStart = DateTime.UtcNow,
                        ValidityEnd = new DateTime(2099, 08, 22),
                        TenantId = tenantId,
                        PlatformId = platformId,
                        VersionNumber = "1",
                        IsActive = false
                    };

                    resourceDetails = resourceDS.Insert(resourceDetails);

                    bool status = ResourceAttributeEntry(inpObj, resourceDetails, "Service",null,null,serviceObj);

                    if (status)
                    {
                        var res = ResourceDependencyEntry(inpObj, resourceDetails);
                    }
                    //enter anomaly rules
                    bool AnomalyEntryStatus = AnomalyRulesEntry(resourceDetails);
                }
                
            }
            catch (Exception ex)
            {
                retStatus = false;
            }

            return retStatus;
        }

        private string ServiceAttributeFinder(resourcetype_service_details serviceObj, resourcetype_metadata metaDataObj)
        {
            string attributeValue = string.Empty;
            try
            {
                switch (metaDataObj.AttributeName.ToLower().Trim().Replace(" ", ""))
                {
                    case "servicename":
                        attributeValue = serviceObj.ServiceName;
                        break;
                    default:
                        attributeValue = metaDataObj.DefaultValue != null ? metaDataObj.DefaultValue : string.Empty;
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return attributeValue;
        }

        private BE2.ResourceModelGenerationReqMsg TranslateMethod(BE.ResourceModelConfigInput objIE)
        {
            BE2.ResourceModelGenerationReqMsg objBE = new BE2.ResourceModelGenerationReqMsg();
            objBE.Platformtype = objIE.name;
            objBE.Tenantid = Convert.ToInt32(objIE.tenantId);
            objBE.Platformname = (from o in objIE.labels where o.name == "Platform Name" select o.value).FirstOrDefault();
            objBE.HostName = (from o in objIE.labels where o.name == "Host Name" select o.value).FirstOrDefault();

            //getting control tower details
            List<BE.Labels> controlTowerDetailsList = (from s in objIE.sections where s.name == "Orchestrator Details" select s.labels).FirstOrDefault();

            objBE.IPAddress = (from c in controlTowerDetailsList where c.name == "IP Address" select c.value).FirstOrDefault();
            objBE.API_URL = (from c in controlTowerDetailsList where c.name == "Orchestrator URL" select c.value).FirstOrDefault();
            objBE.API_UserName = (from c in controlTowerDetailsList where c.name == "Orchestrator User Name" select c.value).FirstOrDefault();
            objBE.API_Password = (from c in controlTowerDetailsList where c.name == "Orchestrator Password" select c.value).FirstOrDefault();
            objBE.Service_UserName = (from c in controlTowerDetailsList where c.name == "Service User Name" select c.value).FirstOrDefault();
            objBE.Service_Password = (from c in controlTowerDetailsList where c.name == "Service Password" select c.value).FirstOrDefault();

            //getting db details
            List<BE.Labels> databaseDetailsList = (from s in objIE.sections where s.name == "Database Details" select s.labels).FirstOrDefault();

            objBE.Database_HostName = (from d in databaseDetailsList where d.name == "Database Host Name" select d.value).FirstOrDefault();
            objBE.Database_Type = (from d in databaseDetailsList where d.name == "Database Type" select d.value).FirstOrDefault();
            objBE.Database_IPaddress = (from d in databaseDetailsList where d.name == "Database IP Address" select d.value).FirstOrDefault();
            objBE.Database_Name = (from d in databaseDetailsList where d.name == "Database Name" select d.value).FirstOrDefault();
            objBE.Database_UserName = (from d in databaseDetailsList where d.name == "Database User Name" select d.value).FirstOrDefault();
            objBE.Database_Password = (from d in databaseDetailsList where d.name == "Database Password" select d.value).FirstOrDefault();

            return objBE;
        }
    }
}
