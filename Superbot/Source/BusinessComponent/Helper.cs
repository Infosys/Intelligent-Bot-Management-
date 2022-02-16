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
using DE = Infosys.Solutions.Superbot.Resource.Entity;

namespace Infosys.Solutions.Ainauto.Superbot.BusinessComponent
{
    public class Helper
    {
        public static List<string> GetResourceHierarchy(string resourceId, int tenantId)
        {
            ResourceDependencyMapDS resourceDependencyMapDS = new ResourceDependencyMapDS();
            List<string> resourceIdList = new List<string>();

            var resDependencyTable = (from resMap in resourceDependencyMapDS.GetAll() select resMap).ToList();
            while (true)
            {
                var res = (from resMap in resDependencyTable
                           where resMap.ResourceId == resourceId
                           && resMap.TenantId == tenantId
                           select new { resMap.ResourceId, resMap.DependencyResourceId }).First();
                resourceIdList.Add(res.ResourceId);
                if (res.DependencyResourceId.Trim() == "")
                {
                    break;
                }
                else
                {
                    resourceId = res.DependencyResourceId;
                }
            }

            return resourceIdList;
        }

        public static int ConvertToInt(string str)
        {
            int convertedInt = 0;
            if (Int32.TryParse(str, out convertedInt))
                return convertedInt;
            else
                return 0;
        }

        public static bool CheckNotificationRestriction(string resourceId, int observableId, int platformId, int tenantId)
        {
            NotificationConfigurationDS ncDs = new NotificationConfigurationDS();
            bool status = false;

            List<DE.notification_configuration> notConTable = ncDs.GetAll(new DE.notification_configuration() { PlatformId = platformId, ReferenceType = "RESTRICTION", TenantId =  tenantId }).ToList();

            if (NoRestriction(notConTable, "PLATFORM_LEVEL", platformId.ToString()))
            {
                if (NoRestriction(notConTable, "RESOURCE_LEVEL", resourceId))
                {
                    if (NoRestriction(notConTable, "OBSERVABLE_LEVEL", observableId.ToString()))
                    {
                        status = true;
                    }
                }
                
            }

            return status;
        }
        static bool NoRestriction(List<DE.notification_configuration> ncTable, string referenceKey, string referenceValue)
        {
            bool status = true;

            var res = (from nc in ncTable
                       where nc.ReferenceKey == referenceKey
                       && nc.ReferenceValue == referenceValue
                       select nc).ToList();

            if (res.Count > 0)
                status = false;

            return status;
        }

        public static int GetRemediationPlanId(string resourceId, int resourceTypeId, int observableId)
        {
            int remediationPlanId = 0;
            DE.resource_observable_remediation_plan_map resourceObservableRemediationPlanMap = new DE.resource_observable_remediation_plan_map();
            resourceObservableRemediationPlanMap.ResourceId = resourceId;
            resourceObservableRemediationPlanMap.ObservableId = observableId;
            //resourceObservableRemediationPlanMap.TenantId = tenantId;
            ResourceObservableRemediationPlanMapDS resourceObservableRemediationPlanMapDS = new ResourceObservableRemediationPlanMapDS();
            resourceObservableRemediationPlanMap = resourceObservableRemediationPlanMapDS.GetOne(resourceObservableRemediationPlanMap);

            if (resourceObservableRemediationPlanMap != null)
            {
                if (resourceObservableRemediationPlanMap.ValidityEnd < DateTime.UtcNow)
                    return -1;
                else
                    remediationPlanId = resourceObservableRemediationPlanMap.RemediationPlanId;
            }
            else
            {
                DE.resourcetype_observable_remediation_plan_map resourcetypeObservableRemediationPlanMap = new DE.resourcetype_observable_remediation_plan_map();
                resourcetypeObservableRemediationPlanMap.ResourceTypeId = resourceTypeId;
                resourcetypeObservableRemediationPlanMap.ObservableId = observableId;
                ResourcetypeObservableRemediationPlanMapDS resourcetypeObservableRemediationPlanMapDS = new ResourcetypeObservableRemediationPlanMapDS();
                resourcetypeObservableRemediationPlanMap = resourcetypeObservableRemediationPlanMapDS.GetOne(resourcetypeObservableRemediationPlanMap);

                if (resourcetypeObservableRemediationPlanMap == null)
                {
                    return 0;
                }
                else
                {
                    if (resourcetypeObservableRemediationPlanMap.ValidityEnd < DateTime.UtcNow)
                        return -1;
                    else
                        remediationPlanId = resourcetypeObservableRemediationPlanMap.RemediationPlanId;
                }

            }
            return remediationPlanId;
        }
    }
}
