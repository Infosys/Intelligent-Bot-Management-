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
using IE = Infosys.Solutions.Ainauto.Services.Superbot.Contracts.Data;
using BE = Infosys.Solutions.Ainauto.Superbot.BusinessEntity;
using DE = Infosys.Solutions.Superbot.Resource.Entity.Queue;
using Infosys.Solutions.Ainauto.Resource.DataAccess;
using Infosys.Solutions.Superbot.Infrastructure.Common;

namespace Infosys.Solutions.Ainauto.Superbot.BusinessComponent
{
    public class NotificationConfiguration
    {

        public List<BE.NotificationConfigurationDetails> GetSMTPconfigIntermediate(int platformId, int tenantId, string referenceType)
        {
            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_Start, "GetSMTPconfigIntermediate", "NotificationConfiguration"), LogHandler.Layer.Business, null);
            LogHandler.LogDebug(String.Format("The GetSMTPconfigIntermediate method of NotificationConfiguration class is getting executed with input: platform ID:{0} ; tenant id: {1} ; referenceType : {2} ", platformId,tenantId,referenceType),
                LogHandler.Layer.Business, null);
            LogHandler.LogDebug("Creating a service channel in GetSMTPconfigIntermediate method of NotificationConfiguration class",
                              LogHandler.Layer.Business, NotificationChannel.Email);
            Infrastructure.ServiceClientLibrary.SuperBot resourceHandler = new Infrastructure.ServiceClientLibrary.SuperBot();
            var channel = resourceHandler.ServiceChannel;

            LogHandler.LogDebug("calling the GetSMTPConfigurationDetails service of resourceHandlerController class",
                              LogHandler.Layer.Business, NotificationChannel.Email);
            List<BE.NotificationConfigurationDetails> notificationConfigurationDetails = Translator.NotificationDetails.NotificationConfigurationDetails_IEtoBE(channel.GetSMTPConfigurationDetails(platformId, tenantId, referenceType));

            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_End, "GetSMTPconfigIntermediate", "NotificationConfiguration"), LogHandler.Layer.Business, null);
            return notificationConfigurationDetails;
        }
        public List<BE.NotificationConfigurationDetails> GetSMTPConfigurationDetails(int platformId, int tenantId, string referenceType)
        {
            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_Start, "GetSMTPConfigurationDetails", "NotificationConfiguration"), LogHandler.Layer.Business, null);
            LogHandler.LogDebug(String.Format("The GetSMTPConfigurationDetails method of NotificationConfiguration class is getting executed with input: platform ID:{0} ; tenant id: {1} ; referenceType : {2} ", platformId, tenantId, referenceType),
                LogHandler.Layer.Business, null);
            List<BE.NotificationConfigurationDetails> notificationConfigurationDetails = new List<BE.NotificationConfigurationDetails>();
            NotificationConfigurationDS notificationConfigurationDS = new NotificationConfigurationDS();
            var result = (from nCon in notificationConfigurationDS.GetAny()
                          where nCon.TenantId == tenantId
                          && nCon.PlatformId == platformId
                          && nCon.ReferenceType == referenceType
                          select new BE.NotificationConfigurationDetails
                          {
                              ReferenceType = nCon.ReferenceType,
                              ReferenceKey = nCon.ReferenceKey,
                              ReferenceValue = nCon.ReferenceValue
                          }).ToList();

            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_End, "GetSMTPConfigurationDetails", "NotificationConfiguration"), LogHandler.Layer.Business, null);
            return result;
        }

        public List<BE.RecipientConfigurationDetails> GetRecipientconfigIntermediate(string resourceId, int tenantId, string referenceType)
        {
            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_Start, "GetRecipientconfigIntermediate", "NotificationConfiguration"), LogHandler.Layer.Business, null);
            LogHandler.LogDebug(String.Format("The GetRecipientconfigIntermediate method of NotificationConfiguration class is getting executed with input: resource ID:{0} ; tenant id: {1} ; referenceType : {2} ", resourceId, tenantId, referenceType),
                LogHandler.Layer.Business, null);

            LogHandler.LogDebug("Creating a service channel in GetRecipientconfigIntermediate method of NotificationConfiguration class",
                              LogHandler.Layer.Business, NotificationChannel.Email);
            Infrastructure.ServiceClientLibrary.SuperBot resourceHandler = new Infrastructure.ServiceClientLibrary.SuperBot();
            var channel = resourceHandler.ServiceChannel;

            LogHandler.LogDebug("calling the GetRecipientconfigIntermediate service of resourceHandlerController class",
                              LogHandler.Layer.Business, NotificationChannel.Email);
            List<BE.RecipientConfigurationDetails> recipientConfigurationDetails =Translator.NotificationDetails.RecipientConfigurationDetails_IEtoBE(channel.GetRecipientConfigurationDetails(resourceId, tenantId, referenceType));

            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_End, "GetRecipientconfigIntermediate", "NotificationConfiguration"), LogHandler.Layer.Business, null);

            return recipientConfigurationDetails;
        }
        public List<BE.RecipientConfigurationDetails> GetRecipientConfigDetails(string resourceId,int tenantId,string referenceType)
        {
            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_Start, "GetRecipientConfigDetails", "NotificationConfiguration"), LogHandler.Layer.Business, null);
            LogHandler.LogDebug(String.Format("The GetRecipientConfigDetails method of NotificationConfiguration class is getting executed with input: resource ID:{0} ; tenant id: {1} ; referenceType : {2} ", resourceId, tenantId, referenceType),
                LogHandler.Layer.Business, null);
            ResourceDependencyMapDS resourceDependencyMapDS = new ResourceDependencyMapDS();
            List<string> resourceIdList = Helper.GetResourceHierarchy(resourceId, tenantId);
            //var res = (from rdm in resourceDependencyMapDS.GetAny()
            //           where rdm.ResourceId == resourceId
            //           select new { rdm.ResourceId, rdm.DependencyResourceId, rdm.DependencyType }).ToList();
            //var res1 = (from resTree in (res.Concat((from rdm2 in resourceDependencyMapDS.GetAny()
            //                                        join r in res
            //                                        on rdm2.ResourceId equals r.DependencyResourceId
            //                                        select new { rdm2.ResourceId, rdm2.DependencyResourceId, rdm2.DependencyType }).ToList()))
            //          select resTree).ToList();
            //var resDependencyTable = (from resMap in resourceDependencyMapDS.GetAny() select resMap).ToList();
            //while (true)
            //{
            //    var res = (from resMap in resDependencyTable
            //               where resMap.ResourceId == resourceId
            //               select new { resMap.ResourceId, resMap.DependencyResourceId }).First();
            //    resourceIdList.Add(res.ResourceId);
            //    if (res.DependencyResourceId == "")
            //    {
            //        break;
            //    }
            //    else
            //    {
            //        resourceId = res.DependencyResourceId;
            //    }
            //}
            List<BE.RecipientConfigurationDetails> recipientConfigurationDetails = new List<BE.RecipientConfigurationDetails>();
            RecipientConfigurationDS recipientConfigurationDS = new RecipientConfigurationDS();
            var resConfigTable = (from nCon in recipientConfigurationDS.GetAny() select nCon).ToList();

            foreach (var resId in resourceIdList)
            {
                var result = (from nCon in resConfigTable
                              where nCon.TenantId == tenantId
                              && nCon.ResourceId == resId
                              && nCon.ReferenceType == referenceType
                              select new BE.RecipientConfigurationDetails
                              {
                                  ReferenceType = nCon.ReferenceType,
                                  RecipientName = nCon.RecipientName,
                                  ReferenceKey = nCon.ReferenceKey,
                                  ReferenceValue = nCon.ReferenceValue,
                                  isActive = nCon.isActive
                              }).ToList();
                if (result.Count > 0)
                {
                    result = (from r in result where r.isActive == true select r).ToList();
                    recipientConfigurationDetails = result;
                    break;
                }
            }
            //foreach (var resId in resourceIdList)
            //{
            //    var result = (from nCon in recipientConfigurationDS.GetAny()
            //                  where nCon.TenantId == tenantId
            //                  && nCon.ResourceId == resId
            //                  && nCon.ReferenceType == referenceType
            //                  select new BE.RecipientConfigurationDetails
            //                  {
            //                      ReferenceType = nCon.ReferenceType,
            //                      RecipientName = nCon.RecipientName,
            //                      ReferenceKey = nCon.ReferenceKey,
            //                      ReferenceValue = nCon.ReferenceValue
            //                  }).ToList();
            //    if (result.Count>0)
            //    {
            //        recipientConfigurationDetails = result;
            //        break;
            //    }
            //}

            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_End, "GetRecipientConfigDetails", "NotificationConfiguration"), LogHandler.Layer.Business, null);
            return recipientConfigurationDetails;
        }
        //public List<BE.RecipientConfigurationDetails> GetRecipientConfigDetails(string resourceId, int tenantId, string referenceType)
        //{
        //    ResourceDependencyMapDS resourceDependencyMapDS = new ResourceDependencyMapDS();
        //    List<string> resourceIdList = new List<string>();
        //    //var res = (from rdm in resourceDependencyMapDS.GetAny()
        //    //           where rdm.ResourceId == resourceId
        //    //           select new { rdm.ResourceId, rdm.DependencyResourceId, rdm.DependencyType }).ToList();
        //    //var res1 = (from resTree in (res.Concat((from rdm2 in resourceDependencyMapDS.GetAny()
        //    //                                        join r in res
        //    //                                        on rdm2.ResourceId equals r.DependencyResourceId
        //    //                                        select new { rdm2.ResourceId, rdm2.DependencyResourceId, rdm2.DependencyType }).ToList()))
        //    //          select resTree).ToList();
        //    var resDependencyTable = (from resMap in resourceDependencyMapDS.GetAny() select resMap).ToList();
        //    while (true)
        //    {
        //        var res = (from resMap in resDependencyTable
        //                   where resMap.ResourceId == resourceId
        //                   select new { resMap.ResourceId, resMap.DependencyResourceId }).First();
        //        resourceIdList.Add(res.ResourceId);
        //        if (res.DependencyResourceId == "")
        //        {
        //            break;
        //        }
        //        else
        //        {
        //            resourceId = res.DependencyResourceId;
        //        }
        //    }
        //    List<BE.RecipientConfigurationDetails> recipientConfigurationDetails = new List<BE.RecipientConfigurationDetails>();
        //    RecipientConfigurationDS recipientConfigurationDS = new RecipientConfigurationDS();
        //    var resConfigTable = (from nCon in recipientConfigurationDS.GetAny() select nCon).ToList();

        //    foreach (var resId in resourceIdList)
        //    {
        //        var result = (from nCon in resConfigTable
        //                      where nCon.TenantId == tenantId
        //                      && nCon.ResourceId == resId
        //                      && nCon.ReferenceType == referenceType
        //                      && nCon.isActive == true
        //                      select new BE.RecipientConfigurationDetails
        //                      {
        //                          ReferenceType = nCon.ReferenceType,
        //                          RecipientName = nCon.RecipientName,
        //                          ReferenceKey = nCon.ReferenceKey,
        //                          ReferenceValue = nCon.ReferenceValue
        //                      }).ToList();
        //        if (result.Count > 0)
        //        {
        //            recipientConfigurationDetails = result;
        //            break;
        //        }
        //    }
        //    //foreach (var resId in resourceIdList)
        //    //{
        //    //    var result = (from nCon in recipientConfigurationDS.GetAny()
        //    //                  where nCon.TenantId == tenantId
        //    //                  && nCon.ResourceId == resId
        //    //                  && nCon.ReferenceType == referenceType
        //    //                  select new BE.RecipientConfigurationDetails
        //    //                  {
        //    //                      ReferenceType = nCon.ReferenceType,
        //    //                      RecipientName = nCon.RecipientName,
        //    //                      ReferenceKey = nCon.ReferenceKey,
        //    //                      ReferenceValue = nCon.ReferenceValue
        //    //                  }).ToList();
        //    //    if (result.Count>0)
        //    //    {
        //    //        recipientConfigurationDetails = result;
        //    //        break;
        //    //    }
        //    //}


        //    return recipientConfigurationDetails;
        //}

        public BE.NotificationConfigurationDetails GetAnomalyReasonIntermediate(string referenceType, string referenceKey, int tenantId, int platformId)
        {            
            Infrastructure.ServiceClientLibrary.SuperBot resourceHandler = new Infrastructure.ServiceClientLibrary.SuperBot();
            var channel = resourceHandler.ServiceChannel;
            BE.NotificationConfigurationDetails notificationConfigurationDetails = Translator.NotificationDetails.AnomalyReason_IEtoBE(channel.GetAnomalyReason(referenceType,referenceKey, tenantId, platformId));
            return notificationConfigurationDetails;
        }

        public BE.NotificationConfigurationDetails GetAnomalyReason(string referenceType, string referenceKey, int tenantId, int platformId)
        {
            BE.NotificationConfigurationDetails notificationConfigurationDetails = new BE.NotificationConfigurationDetails();
            NotificationConfigurationDS notificationConfigurationDS = new NotificationConfigurationDS();
            var result = (from nCon in notificationConfigurationDS.GetAny()
                          where nCon.TenantId == tenantId
                          && nCon.PlatformId == platformId
                          && nCon.ReferenceType == referenceType
                          && nCon.ReferenceKey == referenceKey
                          select new BE.NotificationConfigurationDetails
                          {
                              ReferenceType = nCon.ReferenceType,
                              ReferenceKey = nCon.ReferenceKey,
                              ReferenceValue = nCon.ReferenceValue
                          }).FirstOrDefault();

            return result;
        }

    }
}
