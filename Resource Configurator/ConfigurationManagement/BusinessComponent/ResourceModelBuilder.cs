/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using BE =Infosys.Ainauto.ConfigurationManager.BusinessEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infosys.Solutions.Ainauto.Resource.DataAccess;
using Infosys.Solutions.ConfigurationManager.Resource.Entity;
using Newtonsoft.Json;
using Infosys.Solutions.ConfigurationManager.Infrastructure.Common;
using SC=System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using Infosys.Solutions.Ainauto.Resource.DataAccess.Facade;
using System.Xml.Linq;
using System.IO;


namespace Infosys.Ainauto.ConfigurationManager.BusinessComponent
{
    public class ResourceModelBuilder
    {
        public List<BE.TestCategory> categories;
        List<BE.Property> SEECategories = new List<BE.Property>();
        
        public BE.PlatformInfo getActiveResource(string PlatformInstanceId, string TenantId, string ResourceTypeName)
        {
            BE.PlatformInfo resources = new BE.PlatformInfo();
            try
            {
                ResourceDS resDS = new ResourceDS();
                ResourceTypeDS typeDS = new ResourceTypeDS();
                PlatformsDS platformsDS = new PlatformsDS();
                ResourceDependencyMapDS rdmDS = new ResourceDependencyMapDS();
                ResourceAttributesDS raDs = new ResourceAttributesDS();
                ResourcetypeObservableActionMapDS rtoamDS = new ResourcetypeObservableActionMapDS();
                ResourcetypeObservableRemediationPlanMapDS rtorPlanMapDS = new ResourcetypeObservableRemediationPlanMapDS();
                RemediationPlanDS rPlanDS = new RemediationPlanDS();
                ActionDS actionDS = new ActionDS();
                ObservableDS observable = new ObservableDS();
                ResourceObservableActionMapDS roamDS = new ResourceObservableActionMapDS();
                ResourceObservableRemediationPlanMapDS rorPlanMapDS = new ResourceObservableRemediationPlanMapDS();


                var main = (from rdm in rdmDS.GetAll()
                            join res in resDS.GetAll() on rdm.ResourceId equals res.ResourceId
                            join resType in typeDS.GetAll() on res.ResourceTypeId equals resType.ResourceTypeId
                            where rdm.TenantId == res.TenantId && res.TenantId == resType.TenantId && resType.IsMainEntiry == true
                            && rdm.TenantId == Convert.ToInt32(TenantId) && resType.ResourceTypeName.ToLower() == ResourceTypeName.ToLower()
                            && res.PlatformId == Convert.ToInt32(PlatformInstanceId)
                            //select new { rdm.ResourceId, rdm.DependencyResourceId, rdm.DependencyType, res.ResourceName, resType.IsMainEntiry, resType.ResourceTypeName, resType.ResourceTypeId })
                            select new { rdm.ResourceId, rdm.DependencyResourceId, rdm.DependencyType, res.ResourceName, resType.IsMainEntiry, resType.ResourceTypeName, resType.ResourceTypeDisplayName,
                                resType.ResourceTypeId, res.ValidityStart, res.ValidityEnd, res.IsActive, rdm.PortfolioId,res.Source })
                            .ToList();
                var childResources = (from rdm in rdmDS.GetAll()
                                      join res in resDS.GetAll() on rdm.ResourceId equals res.ResourceId
                                      join resType in typeDS.GetAll() on res.ResourceTypeId equals resType.ResourceTypeId
                                      where resType.IsMainEntiry == false && res.PlatformId == Convert.ToInt32(PlatformInstanceId)
                                      select new { rdm.ResourceId, rdm.DependencyResourceId, res.ResourceName, resType.ResourceTypeId, resType.ResourceTypeName,
                                          resType.ResourceTypeDisplayName, res.ValidityStart, res.ValidityEnd, res.IsActive, rdm.PortfolioId,
                                          res.Source
                                      })
                                         .ToList();

                var mainResources = (from main1 in main
                                     join res in resDS.GetAll() on main1.DependencyResourceId equals res.ResourceId
                                     join rsType in typeDS.GetAll() on res.ResourceTypeId equals rsType.ResourceTypeId
                                     join child in childResources on main1.ResourceId equals child.DependencyResourceId into yg
                                     from child1 in yg.DefaultIfEmpty()
                                     select new resourcesObj
                                     {
                                         ResourceId = main1.ResourceId,
                                         ResourceName = main1.ResourceName,
                                         ResourceTypeId = main1.ResourceTypeId,
                                         ResourceTypeName = main1.ResourceTypeName,
                                         ResourceTypeDisplayName=main1.ResourceTypeDisplayName,
                                         portfolioId = main1.PortfolioId,
                                         Isactive = Convert.ToBoolean(main1.IsActive),
                                         Source = main1.Source,
                                         startdate = main1.ValidityStart,
                                         enddate = main1.ValidityEnd,
                                         Parent_ResourceId = res.ResourceId,
                                         Parent_ResourceName = res.ResourceName,
                                         Parent_ResourceTypeId = rsType.ResourceTypeId,
                                         parent_ResourceTypeName = rsType.ResourceTypeName,
                                         parent_ResourceTypeDisplayName=rsType.ResourceTypeDisplayName,
                                         Child_ResourceId = child1 != null ? child1.ResourceId : null,
                                         Child_ResourceName = child1 != null ? child1.ResourceName : null,
                                         Child_ResourceTypeId = child1 != null ? child1.ResourceTypeId : 0,
                                         Child_ResourceTypeName = child1 != null ? child1.ResourceTypeName : null,
                                         Child_ResourceTypeDisplayName=child1!=null ? child1.ResourceTypeDisplayName:null,
                                         Child_PortfolioId = child1 != null ? child1.PortfolioId : null,
                                         Child_Isactive = child1 != null ? Convert.ToBoolean(child1.IsActive) : false,
                                         Child_startdate = child1 != null ? child1.ValidityStart : (DateTime?)null,
                                         Child_enddate = child1 != null ? child1.ValidityEnd : (DateTime?)null,
                                         Child_Source = child1 != null ? child1.Source : null,
                                     }).ToList();
                if (ResourceTypeName.ToLower() == "platform")
                {
                    //var child1 = new object();

                    mainResources = (from res in resDS.GetAll()
                                     join rdm in rdmDS.GetAll() on res.ResourceId equals rdm.ResourceId
                                     where res.TenantId == rdm.TenantId
                                    //&& (string.IsNullOrEmpty(rdm.DependencyResourceId) || rdm.DependencyResourceId=="0")
                                    && rdm.ResourceId == PlatformInstanceId
                                     select new resourcesObj
                                     {
                                         ResourceId = res.ResourceId,
                                         ResourceName = res.ResourceName,
                                         ResourceTypeId = res.ResourceTypeId,
                                         ResourceTypeName = "Platform",
                                         portfolioId = rdm.PortfolioId,
                                         Isactive = Convert.ToBoolean(res.IsActive),
                                         startdate = res.ValidityStart,
                                         enddate = res.ValidityEnd,
                                         Source=res.Source,
                                         Parent_ResourceId = "",
                                         Parent_ResourceName = "",
                                         Parent_ResourceTypeId = 0,
                                         parent_ResourceTypeName = "",
                                         Child_ResourceId = "",
                                         Child_ResourceName = "",
                                         Child_ResourceTypeId = 0,
                                         Child_ResourceTypeName = "",
                                         Child_PortfolioId = null,
                                         Child_Isactive = false,
                                         Child_startdate = null,
                                         Child_enddate = null, 
                                         Child_Source=""
                                     }).ToList();
                }

                var ResourceLevelObservableandRemediations = GetResourceLevelObservablesandRemediationDetails(Convert.ToInt32(PlatformInstanceId), Convert.ToInt32(TenantId), ResourceTypeName);
                var ObservableandRemediations = GetObservablesandRemediationDetails(Convert.ToInt32(PlatformInstanceId), Convert.ToInt32(TenantId), ResourceTypeName);


                #region CommentedCode


                /*var ResourceLevelObservableandRemediations1 = (from a in roamDS.GetAny().ToArray()
                                                              join b in rorPlanMapDS.GetAny().ToArray() on a.ResourceId equals b.ResourceId
                                                              into rorPlans
                                                              from ror in rorPlans.DefaultIfEmpty()
                                                                  //join c in typeDS.GetAny().ToArray() on a.re equals c.ResourceTypeId
                                                              join d in observable.GetAny().ToArray() on a.ObservableId equals d.ObservableId
                                                              join e in actionDS.GetAny().ToArray() on a.ActionId equals e.ActionId
                                                              //join f in rPlanDS.GetAny().ToArray() on ror.RemediationPlanId equals f.RemediationPlanId
                                                              where
                                                                    //a.TenantId == ror.TenantId &&
                                                                    //a.ObservableId == ror.ObservableId &&
                                                                    //a.TenantId == c.TenantId &&
                                                                    a.ObservableId == d.ObservableId &&
                                                                    a.TenantId == d.TenantId &&
                                                                    a.TenantId == e.TenantId &&
                                                                    // ror.TenantId == f.TenantId &&
                                                                    // (a.ValidityStart <= DateTime.Now && a.ValidityEnd > DateTime.Now) &&
                                                                    // (c.ValidityStart <= DateTime.Now && c.ValidityEnd > DateTime.Now) &&
                                                                    (d.ValidityStart <= DateTime.Now && d.ValidityEnd > DateTime.Now) &&
                                                                    (e.ValidityStart <= DateTime.Now && e.ValidityEnd > DateTime.Now) &&
                                                                    e.IsDeleted == false &&
                                                                    a.TenantId == Convert.ToInt32(TenantId)
                                                              //c.PlatformId == Convert.ToInt32(PlatformInstanceId)

                                                              select new
                                                              {
                                                                  a.Name,
                                                                  //c.ResourceTypeName,
                                                                  a.ResourceId,
                                                                  a.ObservableId,
                                                                  d.ObservableName,
                                                                  a.ActionId,
                                                                  e.ActionName,
                                                                  RemediationPlanId = ror != null ? ror.RemediationPlanId : 0,
                                                                  RemediationPlanName = ror != null ? rPlanDS.GetAny().Where(r => r.RemediationPlanId == ror.RemediationPlanId).FirstOrDefault().RemediationPlanName : null,
                                                                  obsValidityStart = a.ValidityStart,
                                                                  obsValidityEnd = a.ValidityEnd,
                                                                  RemValidityStart = ror != null ? ror.ValidityStart : null,
                                                                  RemValidityEnd = ror != null ? ror.ValidityEnd : null,
                                                              }).Distinct().ToList();*/

                /*var ObservableandRemediations1 = (from a in rtoamDS.GetAny().ToArray()
                                                 join b in rtorPlanMapDS.GetAny().ToArray() on a.ResourceTypeId equals b.ResourceTypeId into rtorPlans
                                                 from rtor in rtorPlans.DefaultIfEmpty()
                                                 join c in typeDS.GetAny().ToArray() on a.ResourceTypeId equals c.ResourceTypeId
                                                 join d in observable.GetAny().ToArray() on a.ObservableId equals d.ObservableId
                                                 join e in actionDS.GetAny().ToArray() on a.ActionId equals e.ActionId
                                                 //join f in rPlanDS.GetAny().ToArray() on rtor.RemediationPlanId equals f.RemediationPlanId
                                                 where
                                                       // a.TenantId == rtor.TenantId &&
                                                       //a.ObservableId == rtor.ObservableId &&
                                                       a.TenantId == c.TenantId &&
                                                       a.ObservableId == d.ObservableId &&
                                                       a.TenantId == d.TenantId &&
                                                       a.TenantId == e.TenantId &&
                                                       // rtor.TenantId == f.TenantId &&
                                                       //(a.ValidityStart <= DateTime.Now && a.ValidityEnd > DateTime.Now) &&
                                                       (c.ValidityStart <= DateTime.Now && c.ValidityEnd > DateTime.Now) &&
                                                       (d.ValidityStart <= DateTime.Now && d.ValidityEnd > DateTime.Now) &&
                                                       (e.ValidityStart <= DateTime.Now && e.ValidityEnd > DateTime.Now) &&
                                                       e.IsDeleted == false &&
                                                       a.TenantId == Convert.ToInt32(TenantId)
                                                 //&& c.PlatformId == Convert.ToInt32(PlatformInstanceId)
                                                 select new
                                                 {
                                                     a.Name,
                                                     c.ResourceTypeName,
                                                     a.ResourceTypeId,
                                                     a.ObservableId,
                                                     d.ObservableName,
                                                     a.ActionId,
                                                     e.ActionName,
                                                     RemediationPlanId = rtor != null ? (
                                                                    rtorPlanMapDS.GetAny().Where(d => d.ResourceTypeId == a.ResourceTypeId && d.ObservableId == a.ObservableId).FirstOrDefault() != null ? rtorPlanMapDS.GetAny().Where(d => d.ResourceTypeId == a.ResourceTypeId && d.ObservableId == a.ObservableId).FirstOrDefault().RemediationPlanId : 0) : 0,
                                                     RemediationPlanName = rtor != null ? (rtorPlanMapDS.GetAny().Where(d => d.ResourceTypeId == a.ResourceTypeId && d.ObservableId == a.ObservableId).FirstOrDefault() != null ?
                                                     rPlanDS.GetAny().Where(r => r.RemediationPlanId == rtor.RemediationPlanId).FirstOrDefault().RemediationPlanName : null) : null,
                                                     obsValidityStart = a.ValidityStart,
                                                     obsValidityEnd = a.ValidityEnd,
                                                     RemValidityStart = rtor != null ? (rtorPlanMapDS.GetAny().Where(d => d.ResourceTypeId == a.ResourceTypeId && d.ObservableId == a.ObservableId).FirstOrDefault() != null ?
                                                     rtorPlanMapDS.GetAny().Where(d => d.ResourceTypeId == a.ResourceTypeId && d.ObservableId == a.ObservableId).FirstOrDefault().ValidityStart : null) : null,
                                                     RemValidityEnd = rtor != null ? (rtorPlanMapDS.GetAny().Where(d => d.ResourceTypeId == a.ResourceTypeId && d.ObservableId == a.ObservableId).FirstOrDefault() != null ?
                                                     rtorPlanMapDS.GetAny().Where(d => d.ResourceTypeId == a.ResourceTypeId && d.ObservableId == a.ObservableId).FirstOrDefault().ValidityEnd : null) : null,
                                                 }).Distinct().ToList();
                                                 */
                #endregion

                var resourceAttributes = (from res in resDS.GetAny().ToArray()
                                          join resType in typeDS.GetAny().ToArray() on res.ResourceTypeId equals resType.ResourceTypeId
                                          join rdm in rdmDS.GetAny().ToArray() on res.ResourceId equals rdm.ResourceId
                                          join resAttr in raDs.GetAny().ToArray() on res.ResourceId equals resAttr.ResourceId
                                          where
                                          res.TenantId == resType.TenantId
                                          && res.TenantId == rdm.TenantId
                                          && res.TenantId == resAttr.TenantId
                                          && (res.ValidityStart <= DateTime.Now && res.ValidityEnd > DateTime.Now)
                                          && (resType.ValidityStart <= DateTime.Now && resType.ValidityEnd > DateTime.Now)
                                          && (rdm.ValidityStart <= DateTime.Now && rdm.ValidityEnd > DateTime.Now)
                                          && res.TenantId == Convert.ToInt32(TenantId) && res.PlatformId == Convert.ToInt32(PlatformInstanceId)
                                          select new
                                          {
                                              res.ResourceName,
                                              res.ResourceId,
                                              resType.ResourceTypeName,
                                              resType.ResourceTypeDisplayName,
                                              resType.ResourceTypeId,
                                              rdm.DependencyResourceId,
                                              rdm.DependencyType,
                                              rdm.Priority,
                                              resAttr.AttributeName,
                                              AttributeValue=Convert.ToBoolean(resAttr.IsSecret)?Decrypt(resAttr.AttributeValue): resAttr.AttributeValue,
                                              resAttr.DisplayName,
                                              resAttr.Description,
                                              resAttr.IsSecret,
                                              resAttr.Remarks,
                                              resAttr.AttributeCategory,
                                              resAttr.isHiddenInUi
                                          }).ToList();

                var platforms = (from p in platformsDS.GetAny().ToArray()
                                 where p.PlatformId == Convert.ToInt32(PlatformInstanceId)
                                 && p.TenantId == Convert.ToInt32(TenantId)
                                 select p).FirstOrDefault();

                if (platforms != null)
                {
                    resources.platformid = platforms.PlatformId;
                    resources.tenantid = platforms.TenantId;
                    resources.platformtype = platforms.PlatformName;
                    resources.resourcemodelversion = "1";
                    resources.status = "Active";
                    resources.resourcedetails = (from res in mainResources
                                                 group res by new
                                                 {
                                                     res.ResourceId,
                                                     res.ResourceName,
                                                     res.ResourceTypeId,
                                                     res.ResourceTypeName,
                                                     res.ResourceTypeDisplayName
                                                 } into params1
                                                 select new BE.ResourceDetails
                                                 {
                                                     resourceid = params1.Key.ResourceId,
                                                     resourcename = params1.Key.ResourceName,
                                                     resourcetypename = params1.Key.ResourceTypeName,
                                                     resourcetypedisplayname=params1.Key.ResourceTypeDisplayName,
                                                     resourcetypeid = Convert.ToString(params1.Key.ResourceTypeId),
                                                     source= params1.Where(r => r.ResourceId == params1.Key.ResourceId).FirstOrDefault().Source,
                                                     portfolioid = params1.Where(r => r.ResourceId == params1.Key.ResourceId).FirstOrDefault().portfolioId,
                                                     dontmonitor = params1.Where(r => r.ResourceId == params1.Key.ResourceId).FirstOrDefault().Isactive,
                                                     startdate = Convert.ToDateTime(params1.Where(r => r.ResourceId == params1.Key.ResourceId).FirstOrDefault().startdate).ToString("yyyy-MM-ddTHH:mm:ss"),
                                                     enddate = Convert.ToDateTime(params1.Where(r => r.ResourceId == params1.Key.ResourceId).FirstOrDefault().enddate).ToString("yyyy-MM-ddTHH:mm:ss"),
                                                     parentdetails = params1.Where(p => !string.IsNullOrEmpty(p.Parent_ResourceId)).FirstOrDefault() != null ? (from r in params1.ToList()
                                                                                                                                                                group r by new
                                                                                                                                                                {
                                                                                                                                                                    r.Parent_ResourceId,
                                                                                                                                                                    r.Parent_ResourceName,
                                                                                                                                                                    r.Parent_ResourceTypeId,
                                                                                                                                                                    r.parent_ResourceTypeName,
                                                                                                                                                                    r.parent_ResourceTypeDisplayName
                                                                                                                                                                } into pd
                                                                                                                                                                select new BE.ParentDetails
                                                                                                                                                                {
                                                                                                                                                                    resourceid = pd.Key.Parent_ResourceId,
                                                                                                                                                                    resourcename = pd.Key.Parent_ResourceName,
                                                                                                                                                                    resourcetypename = pd.Key.parent_ResourceTypeName,
                                                                                                                                                                    resourcetypedisplayname=pd.Key.parent_ResourceTypeDisplayName,
                                                                                                                                                                    resourcetypeid = Convert.ToString(pd.Key.Parent_ResourceTypeId),
                                                                                                                                                                }).ToList() : null,
                                                     childdetails = params1.Where(p => !string.IsNullOrEmpty(p.Child_ResourceId)).FirstOrDefault() != null ?
                                                                    (from r in params1.ToList()
                                                                     group r by new
                                                                     {
                                                                         r.Child_ResourceId,
                                                                         r.Child_ResourceName,
                                                                      
                                                                         r.Child_ResourceTypeId,
                                                                         r.Child_ResourceTypeName,
                                                                         r.Child_ResourceTypeDisplayName
                                                                     } into pd
                                                                     select new BE.ChildResource
                                                                     {
                                                                         resourceid = pd.Key.Child_ResourceId,
                                                                         resourcename = pd.Key.Child_ResourceName,
                                                                         resourcetypename = pd.Key.Child_ResourceTypeName,
                                                                         resourcetypedisplayname=pd.Key.Child_ResourceTypeDisplayName,
                                                                         resourcetypeid = Convert.ToString(pd.Key.Child_ResourceTypeId),
                                                                         source = pd.Where(r => r.Child_ResourceId == pd.Key.Child_ResourceId).FirstOrDefault().Child_Source,
                                                                         portfolioid = pd.Where(r => r.Child_ResourceId == pd.Key.Child_ResourceId).FirstOrDefault().Child_PortfolioId,
                                                                         dontmonitor = pd.Where(r => r.Child_ResourceId == pd.Key.Child_ResourceId).FirstOrDefault().Child_Isactive,
                                                                         startdate = Convert.ToDateTime(pd.Where(r => r.Child_ResourceId == pd.Key.Child_ResourceId).FirstOrDefault().Child_startdate).ToString("yyyy-MM-ddTHH:mm:ss"),
                                                                         enddate = Convert.ToDateTime(pd.Where(r => r.Child_ResourceId == pd.Key.Child_ResourceId).FirstOrDefault().Child_enddate).ToString("yyyy-MM-ddTHH:mm:ss"),
                                                                         resourceattribute = (from ra in resourceAttributes
                                                                                              where ra.ResourceId == pd.Key.Child_ResourceId
                                                                                              select new BE.ResourceAttribute
                                                                                              {
                                                                                                  attributename = ra.AttributeName,
                                                                                                  attributevalue = ra.AttributeValue,
                                                                                                  displayname = ra.DisplayName,
                                                                                                  description = ra.Description,
                                                                                                  IsSecret=Convert.ToBoolean(ra.IsSecret),
                                                                                                  attributeCategory = ra.AttributeCategory,
                                                                                                  remarks = ra.Remarks,
                                                                                                  Ishiddeninui = Convert.ToBoolean(ra.isHiddenInUi)
                                                                                              }).ToList(),
                                                                         observablesandremediations = (from ra in roamDS.GetAny().ToArray()
                                                                                                       join ob in rorPlanMapDS.GetAny().ToArray() on ra.ResourceId equals ob.ResourceId
                                                                                                       where ra.ResourceId == pd.Key.Child_ResourceId
                                                                                                       select ra).FirstOrDefault() != null ?
                                                                                 (from rob in GetResourceLevelObservablesandRemediationDetails(Convert.ToInt32(PlatformInstanceId), Convert.ToInt32(TenantId), pd.Key.Child_ResourceTypeName).observablesandremediationplans
                                                                                  where rob.resourcetypeid == Convert.ToString(pd.Key.Child_ResourceId)
                                                                                  from ro in rob.observablesandremediations
                                                                                  select new BE.observablesandremediation
                                                                                  {
                                                                                      name = ro.name,
                                                                                      ObservableId = Convert.ToString(ro.ObservableId),
                                                                                      ObservableName = ro.ObservableName,
                                                                                      ObservableActionId = Convert.ToString(ro.ObservableActionId),
                                                                                      ObservableActionName = ro.ObservableActionName,
                                                                                      RemediationPlanId = Convert.ToString(ro.RemediationPlanId),
                                                                                      RemediationPlanName = ro.RemediationPlanName,
                                                                                      isObsSelected = ro.isObsSelected,
                                                                                      isRemSelected = ro.isRemSelected,
                                                                                  }).ToList() : (from OR1 in GetObservablesandRemediationDetails(Convert.ToInt32(PlatformInstanceId), Convert.ToInt32(TenantId), pd.Key.Child_ResourceTypeName).observablesandremediationplans
                                                                                                 where OR1.resourcetypeid ==Convert.ToString(pd.Key.Child_ResourceTypeId)
                                                                                                 from OR in OR1.observablesandremediations
                                                                                                 select new BE.observablesandremediation
                                                                                                 {
                                                                                                     name = OR.name,
                                                                                                     ObservableId = Convert.ToString(OR.ObservableId),
                                                                                                     ObservableName = OR.ObservableName,
                                                                                                     ObservableActionId = Convert.ToString(OR.ObservableActionId),
                                                                                                     ObservableActionName = OR.ObservableActionName,
                                                                                                     RemediationPlanId = Convert.ToString(OR.RemediationPlanId),
                                                                                                     RemediationPlanName = OR.RemediationPlanName,
                                                                                                     isObsSelected = OR.isObsSelected,
                                                                                                     isRemSelected = OR.isRemSelected,
                                                                                                 }).ToList()
                                                                     }).ToList() : null,
                                                     logdetails = (from rs in resDS.GetAny().ToArray()
                                                                   where rs.ResourceId == params1.Key.ResourceId
                                                                   select new BE.LogDetails
                                                                   {
                                                                       CreatedBy = rs.CreatedBy,
                                                                       CreateDate = rs.CreateDate,
                                                                       ModifiedBy = rs.ModifiedBy,
                                                                       ModifiedDate = Convert.ToDateTime(rs.ModifiedDate),
                                                                       ValidityStart = rs.ValidityStart,
                                                                       ValidityEnd = rs.ValidityEnd
                                                                   }).ToList(),
                                                     resourceattribute = (from ra in resourceAttributes
                                                                          where ra.ResourceId == params1.Key.ResourceId
                                                                          select new BE.ResourceAttribute
                                                                          {
                                                                              attributename = ra.AttributeName,
                                                                              attributevalue = ra.AttributeValue,
                                                                              displayname = ra.DisplayName,
                                                                              description = ra.Description,
                                                                              IsSecret=Convert.ToBoolean(ra.IsSecret),
                                                                              attributeCategory = ra.AttributeCategory,
                                                                              remarks = ra.Remarks,
                                                                              Ishiddeninui = Convert.ToBoolean(ra.isHiddenInUi)
                                                                          }
                                                                         ).ToList(),
                                                     observablesandremediations = (from ra1 in ResourceLevelObservableandRemediations.observablesandremediationplans
                                                                                   where ra1.resourcetypeid == params1.Key.ResourceId
                                                                                   select ra1).FirstOrDefault() != null ?
                                                                                 (from rob1 in ResourceLevelObservableandRemediations.observablesandremediationplans
                                                                                  where rob1.resourcetypeid == params1.Key.ResourceId
                                                                                  from rob in rob1.observablesandremediations
                                                                                  select new BE.observablesandremediation
                                                                                  {
                                                                                      name = rob.name,
                                                                                      ObservableId = Convert.ToString(rob.ObservableId),
                                                                                      ObservableName = rob.ObservableName,
                                                                                      ObservableActionId = Convert.ToString(rob.ObservableActionId),
                                                                                      ObservableActionName = rob.ObservableActionName,
                                                                                      RemediationPlanId = Convert.ToString(rob.RemediationPlanId),
                                                                                      RemediationPlanName = rob.RemediationPlanName,
                                                                                      isObsSelected = rob.isObsSelected,
                                                                                      isRemSelected = rob.isRemSelected,
                                                                                  }).ToList() : (from OR1 in ObservableandRemediations.observablesandremediationplans
                                                                                                 where OR1.resourcetypeid ==Convert.ToString(params1.Key.ResourceTypeId)
                                                                                                 from OR in OR1.observablesandremediations
                                                                                                 select new BE.observablesandremediation
                                                                                                 {
                                                                                                     name = OR.name,
                                                                                                     ObservableId = Convert.ToString(OR.ObservableId),
                                                                                                     ObservableName = OR.ObservableName,
                                                                                                     ObservableActionId = Convert.ToString(OR.ObservableActionId),
                                                                                                     ObservableActionName = OR.ObservableActionName,
                                                                                                     RemediationPlanId = Convert.ToString(OR.RemediationPlanId),
                                                                                                     RemediationPlanName = OR.RemediationPlanName,
                                                                                                     isObsSelected = OR.isObsSelected,
                                                                                                     isRemSelected = OR.isRemSelected,
                                                                                                 }).ToList()


                                                 }).ToList();


                }

                return resources;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public BE.SummaryResourceInfo getSummaryResources(string PlatformInstanceId, string TenantId, string ResourceTypeName)
        {
            BE.SummaryResourceInfo SummaryResources = new BE.SummaryResourceInfo();
            try
            {
                BE.PlatformInfo resources = getActiveResource(PlatformInstanceId, TenantId, ResourceTypeName);
                SummaryResources.resourcetypename = ResourceTypeName;
                List<BE.SummaryResource> summaryResList = new List<BE.SummaryResource>();
                if (resources.resourcedetails != null)
                {
                    foreach (BE.ResourceDetails resdetails in resources.resourcedetails)
                    {
                        BE.SummaryResource summaryRes = new BE.SummaryResource();
                        summaryRes.resourceid = resdetails.resourceid;
                        summaryRes.resourcename = resdetails.resourcename;
                        summaryRes.isactive = resdetails.dontmonitor;
                        SummaryResources.resourcetypeid = resdetails.resourcetypeid;

                        List<BE.SummaryResourceParent> parentResList = new List<BE.SummaryResourceParent>();
                        if (resdetails.parentdetails != null)
                        {
                            foreach (BE.ParentDetails parent in resdetails.parentdetails)
                            {
                                BE.SummaryResourceParent parentRes = new BE.SummaryResourceParent();
                                parentRes.resourceid = parent.resourceid;
                                parentRes.resourcename = parent.resourcename;
                                parentRes.resourcetypeid = parent.resourcetypeid;
                                parentRes.resourcetypename = parent.resourcetypename;
                                parentResList.Add(parentRes);
                            }
                        }
                        summaryRes.parentdetails = parentResList;

                        List<BE.SummaryResourceChild> childResList = new List<BE.SummaryResourceChild>();
                        if (resdetails.childdetails != null)
                        {
                            foreach (BE.ChildResource child in resdetails.childdetails)
                            {
                                BE.SummaryResourceChild childRes = new BE.SummaryResourceChild();
                                childRes.resourceid = child.resourceid;
                                childRes.resourcename = child.resourcename;
                                childRes.resourcetypeid = child.resourcetypeid;
                                childRes.resourcetypename = child.resourcetypename;
                                childRes.isactive = child.dontmonitor;
                                childResList.Add(childRes);
                            }
                        }
                        summaryRes.childdetails = childResList;

                        summaryResList.Add(summaryRes);
                    }
                }
                SummaryResources.resourcedetails = summaryResList;
                return SummaryResources;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public string updateSummaryViewConfiguration(BE.SummaryResourceInfo resource)
        {
            
            string responseMessage = null;
            
            List<resource> updateResources = new List<resource>();
            try
            {
                if(resource!=null && resource.resourcedetails!=null)
                {
                    foreach(BE.SummaryResource res in resource.resourcedetails)
                    {
                        ResourceDS resDS = new ResourceDS();
                        var resDB = resDS.GetAny().Where(r => r.ResourceId == res.resourceid).FirstOrDefault();
                        if(resDB!=null && resDB.IsActive!=res.isactive)   
                        {
                            resDB.IsActive = res.isactive;
                            updateResources.Add(resDB);
                        }

                        foreach(BE.SummaryResourceChild ch in res.childdetails)
                        {
                            ResourceDS resDS1 = new ResourceDS();
                            var resCh = resDS1.GetAny().Where(r => r.ResourceId == ch.resourceid).FirstOrDefault();
                            if (resCh != null && resCh.IsActive != ch.isactive)
                            {
                                resCh.IsActive = ch.isactive;
                                updateResources.Add(resCh);
                            }
                        }

                    }
                    if(updateResources.Count()>0)
                    {
                        ResourceDS resDSobj = new ResourceDS();
                        var res = resDSobj.UpdateBatch(updateResources);
                        responseMessage = res != null ? "Resource details updated successfully" : "Resource details updation failed";
                    }
                    else
                    {
                        responseMessage = "Resource details updated successfully";
                    }
                }
                else
                {
                    responseMessage = "Invalid data";
                }

                return responseMessage;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public List<BE.PortfolioInfo> getAllPortfolios(int tenantId)
        {
            try
            {
                ResourceDS resDS = new ResourceDS();
                ResourceDependencyMapDS rdmDS = new ResourceDependencyMapDS();

                var response = (from rdm in rdmDS.GetAny().ToArray()
                                join res in resDS.GetAny().ToArray()  on rdm.ResourceId equals res.ResourceId
                                where rdm.TenantId == tenantId && res.TenantId == tenantId
                                && rdm.DependencyType.Trim() == "PORTFOLIO"
                                select new BE.PortfolioInfo
                                {
                                    portfolioid = rdm.ResourceId,
                                    portfolioname = res.ResourceName
                                }).ToList();
                return response;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<BE.PortfolioResourceTypeInfo> getAllResourceTypePortfolios(int tenantId)
        {
            try
            {
                ResourceDS resDS = new ResourceDS();
                ResourceDependencyMapDS rdmDS = new ResourceDependencyMapDS();
                ResourceDependencyMapDS portfolioDS = new ResourceDependencyMapDS();
                ResourceTypeDS rtDS = new ResourceTypeDS();
                List<BE.PortfolioResourceTypeInfo> portfolioinfoList = new List<BE.PortfolioResourceTypeInfo>();
                List < BE.PortfolioInfo> portfolios= getAllPortfolios(tenantId);
                foreach (BE.PortfolioInfo portfolio in portfolios)
                {
                    BE.PortfolioResourceTypeInfo portfolioInfo = new BE.PortfolioResourceTypeInfo();
                    portfolioInfo.portfolioid = portfolio.portfolioid;
                    portfolioInfo.portfolioname = portfolio.portfolioname;
                    List<BE.resourceTypeModel> resourceTypeList = new List<BE.resourceTypeModel>();

                    var response = (from rdm in rdmDS.GetAny().ToArray()
                                    join res in resDS.GetAny().ToArray() on rdm.ResourceId equals res.ResourceId
                                    join rt in rtDS.GetAny().ToArray() on res.ResourceTypeId equals rt.ResourceTypeId

                                    where rdm.TenantId == tenantId && res.TenantId == tenantId && rt.TenantId == tenantId && rdm.PortfolioId== portfolio.portfolioid
                                    group rt by new { rt.ResourceTypeId, rt.ResourceTypeName }
                                    into g

                                    select new BE.resourceTypeModel
                                    {
                                        
                                        resourcetypeid = g.Key.ResourceTypeId,
                                        resourcetypename = g.Key.ResourceTypeName


                                    }).ToList();
                    foreach (BE.resourceTypeModel info in response)
                    {

              
                        resourceTypeList.Add(info);
                    }
                    portfolioInfo.resourceTypeList = resourceTypeList;
                    portfolioinfoList.Add(portfolioInfo);
                }

    
                return portfolioinfoList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public List<BE.TenantInfo> getTenantDetails(int tenantId)
        {

            try
            {
                TenantDS tDS = new TenantDS();
                if (tenantId == 0)
                {
                   var result = (from ti in tDS.GetAny()
                                  select new BE.TenantInfo
                                  {
                                      tenantid = ti.TenantId,
                                      name = ti.Name,
                                      tenantConfig = ti.TenantConfig
                                  }
                              ).ToList();
                    return result;
                }
                else
                {
                  var result = (from ti in tDS.GetAny()
                                  where ti.TenantId == tenantId
                                  select new BE.TenantInfo
                                  {
                                      tenantid = ti.TenantId,
                                      name = ti.Name,
                                      tenantConfig = ti.TenantConfig
                                  }
                                  ).ToList();
                    return result;

                }
                
                

            }
            catch (Exception ex)
            {
                throw ex;
            }
            }

        public BE.ResourceModel getAllResources(string platformInstanceId, int tenantId)
        {
            try
            {
                ResourceDependencyMapDS rdmDS = new ResourceDependencyMapDS();
                ResourceDS resDS = new ResourceDS();
                ResourceTypeDS rtDS = new ResourceTypeDS();
                string dependencyResID = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DependencyResourceID"]);


                var resourceIDs1 = (from rdm in rdmDS.GetAny()
                                    where rdm.DependencyResourceId == platformInstanceId &&
                                    (rdm.ValidityStart <= DateTime.Now && rdm.ValidityEnd > DateTime.Now) &&
                                    rdm.TenantId == tenantId
                                    select new { rdm.ResourceId, rdm.DependencyType }
                                ).ToList();
                var mainresources = (from rdm in rdmDS.GetAny().ToArray()
                                     join res in resourceIDs1 on rdm.DependencyResourceId equals res.ResourceId
                                     where (rdm.ValidityStart <= DateTime.Now && rdm.ValidityEnd > DateTime.Now) &&
                                     rdm.TenantId == tenantId
                                     group rdm by rdm.DependencyResourceId into rdm1
                                     select rdm1
                                 ).ToList();

                if (dependencyResID == platformInstanceId)
                {
                    mainresources = (from rdm in rdmDS.GetAny().ToArray()
                                     where rdm.DependencyResourceId == dependencyResID &&
                                     (rdm.ValidityStart <= DateTime.Now && rdm.ValidityEnd > DateTime.Now) &&
                                     rdm.TenantId == tenantId
                                     group rdm by rdm.DependencyResourceId into rdm1
                                     select rdm1
                                 ).ToList();
                }

                /*var resourceIDs = (from rdm in rdmDS.GetAny()
                                   where rdm.DependencyResourceId == platformInstanceId &&
                                   (rdm.ValidityStart <= DateTime.Now && rdm.ValidityEnd > DateTime.Now) &&
                                   rdm.TenantId == tenantId
                                   select rdm.ResourceId
                                 ).ToList();

                var mainresources1 = (from rdm in rdmDS.GetAny()
                                     where resourceIDs.Contains(rdm.DependencyResourceId) &&
                                    // where rdm.DependencyResourceId == platformInstanceId &&
                                     (rdm.ValidityStart <= DateTime.Now && rdm.ValidityEnd > DateTime.Now) &&
                                     rdm.TenantId == tenantId
                                     group rdm by rdm.DependencyResourceId into rdm1
                                     select rdm1
                                 ).ToList();
                                 */

                var response = (from mr in mainresources
                                join rs in resDS.GetAny().ToArray() on mr.Key equals rs.ResourceId
                                join t in rtDS.GetAny().ToArray() on rs.ResourceTypeId equals t.ResourceTypeId
                                where rs.ValidityStart <= DateTime.Now && rs.ValidityEnd > DateTime.Now &&
                                    t.ValidityStart <= DateTime.Now && t.ValidityEnd > DateTime.Now
                                group new { mr, rs, t } by new
                                {
                                    t.ResourceTypeId,
                                    t.ResourceTypeName,
                                    t.ResourceTypeDisplayName,
                                    mr.Key,
                                    rs.ResourceName,
                                    t.IsMainEntiry
                                } into grpRMD
                                select new BE.Resourcemodeldetail
                                {
                                    resourcetypeid = Convert.ToString(grpRMD.Key.ResourceTypeId),
                                    resourcetypename = grpRMD.Key.ResourceTypeName,
                                    resourcetypedisplayname= grpRMD.Key.ResourceTypeDisplayName,
                                    resourceid = grpRMD.Key.Key,
                                    resourcename = grpRMD.Key.ResourceName,
                                    ismainentiy = Convert.ToBoolean(grpRMD.Key.IsMainEntiry),
                                    childdetails = (from grp in grpRMD
                                                    from r in resDS.GetAny().ToArray()
                                                    join rt in rtDS.GetAny().ToArray() on r.ResourceTypeId equals rt.ResourceTypeId
                                                    where grp.mr.ToList().Select(r => r.ResourceId).ToList().Contains(r.ResourceId) &&
                                                    r.ValidityStart <= DateTime.Now && r.ValidityEnd > DateTime.Now &&
                                                    rt.ValidityStart <= DateTime.Now && rt.ValidityEnd > DateTime.Now &&
                                                    r.ResourceId.StartsWith(grp.mr.Key + "_")
                                                    && grp.mr.Key != platformInstanceId
                                                    group new { rt, r } by new
                                                    {
                                                        r.ResourceTypeId,
                                                        rt.ResourceTypeName,
                                                        rt.ResourceTypeDisplayName,
                                                        rt.IsMainEntiry
                                                    } into gpRT
                                                    select new BE.Childdetail
                                                    {
                                                        resourcetypeid = Convert.ToString(gpRT.Key.ResourceTypeId),
                                                        resourcetypename = gpRT.Key.ResourceTypeName,
                                                        resourcetypedisplayname= gpRT.Key.ResourceTypeDisplayName,
                                                        ismainentiy = Convert.ToBoolean(gpRT.Key.IsMainEntiry),
                                                        resourcetypedetails = (from gp in gpRT.ToList()
                                                                               select new BE.Resourcetypedetail
                                                                               {
                                                                                   resourceid = gp.r.ResourceId,
                                                                                   resourcename = gp.r.ResourceName
                                                                               }).ToList()
                                                    }).ToList(),
                                    resourcedetails = (from grp in grpRMD
                                                       from r in resDS.GetAny().ToArray()
                                                       join rt in rtDS.GetAny().ToArray() on r.ResourceTypeId equals rt.ResourceTypeId
                                                       where grp.mr.ToList().Select(r => r.ResourceId).ToList().Contains(r.ResourceId) &&
                                                       r.ValidityStart <= DateTime.Now && r.ValidityEnd > DateTime.Now &&
                                                       rt.ValidityStart <= DateTime.Now && rt.ValidityEnd > DateTime.Now &&
                                                      //!r.ResourceId.StartsWith(grp.mr.Key + "_")
                                                      (grp.mr.Key == platformInstanceId || !r.ResourceId.StartsWith(grp.mr.Key + "_"))
                                                       group new { rt, r } by new
                                                       {
                                                           r.ResourceTypeId,
                                                           rt.ResourceTypeName,
                                                           rt.ResourceTypeDisplayName,
                                                           rt.IsMainEntiry
                                                       } into gpRT
                                                       select new BE.Resourcedetail
                                                       {
                                                           resourcetypeid = Convert.ToString(gpRT.Key.ResourceTypeId),
                                                           resourcetypename = gpRT.Key.ResourceTypeName,
                                                           resourcetypedisplayname= gpRT.Key.ResourceTypeDisplayName,
                                                           ismainentiy = Convert.ToBoolean(gpRT.Key.IsMainEntiry),
                                                           details = (from gp in gpRT.ToList()
                                                                      select new BE.Detail
                                                                      {
                                                                          resourceid = gp.r.ResourceId,
                                                                          resourcename = gp.r.ResourceName,
                                                                          childdetails = (from rdm in rdmDS.GetAny().ToArray()
                                                                                          join r in resDS.GetAny().ToArray() on rdm.ResourceId equals r.ResourceId
                                                                                          join rt in rtDS.GetAny().ToArray() on r.ResourceTypeId equals rt.ResourceTypeId
                                                                                          where rdm.DependencyResourceId == gp.r.ResourceId &&
                                                                                          (r.ValidityStart <= DateTime.Now && r.ValidityEnd > DateTime.Now) &&
                                                                                          (rt.ValidityStart <= DateTime.Now && rt.ValidityEnd > DateTime.Now)
                                                                                          group new { rt, r } by new
                                                                                          {
                                                                                              r.ResourceTypeId,
                                                                                              rt.ResourceTypeName,
                                                                                              rt.ResourceTypeDisplayName,
                                                                                              rt.IsMainEntiry
                                                                                          } into gpch
                                                                                          select new BE.Childdetail
                                                                                          {
                                                                                              resourcetypeid = Convert.ToString(gpch.Key.ResourceTypeId),
                                                                                              resourcetypename = gpch.Key.ResourceTypeName,
                                                                                              resourcetypedisplayname=gpch.Key.ResourceTypeDisplayName,
                                                                                              ismainentiy = Convert.ToBoolean(gpch.Key.IsMainEntiry),
                                                                                              resourcetypedetails = (from gp1 in gpch.ToList()
                                                                                                                     select new BE.Resourcetypedetail
                                                                                                                     {
                                                                                                                         resourceid = gp1.r.ResourceId,
                                                                                                                         resourcename = gp1.r.ResourceName
                                                                                                                     }).ToList()
                                                                                          }).ToList()

                                                                      }).ToList()
                                                       }).ToList()
                                }).ToList();

                var jhjfdfg = JsonConvert.SerializeObject(response);

                PlatformsDS pDS = new PlatformsDS();
                BE.ResourceModel resourceModel = new BE.ResourceModel();
                var platform = pDS.GetAll().Where(p => p.PlatformId == Convert.ToInt32(platformInstanceId)).FirstOrDefault();
                if (platform != null)
                {
                    resourceModel.tenantid = Convert.ToString(tenantId);
                    resourceModel.platformid = Convert.ToString(platform.PlatformId);
                    resourceModel.platformname = platform.PlatformName;
                    resourceModel.resourcemodeldetails = response;
                }
                return resourceModel;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        //public BE.ResourceModel getAllResources(string platformInstanceId, int tenantId)
        //{
        //    try
        //    {
        //        ResourceDependencyMapDS rdmDS = new ResourceDependencyMapDS();
        //        ResourceDS resDS = new ResourceDS();
        //        ResourceTypeDS rtDS = new ResourceTypeDS();
        //        string dependencyResID =Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DependencyResourceID"]);


        //        var resourceIDs1 = (from rdm in rdmDS.GetAny()
        //                           where rdm.DependencyResourceId == platformInstanceId &&
        //                           (rdm.ValidityStart <= DateTime.Now && rdm.ValidityEnd > DateTime.Now) &&
        //                           rdm.TenantId == tenantId
        //                           select new { rdm.ResourceId, rdm.DependencyType }
        //                        ).ToList();
        //        var mainresources = (from rdm in rdmDS.GetAny().ToArray()
        //                              join res in resourceIDs1 on rdm.DependencyResourceId equals res.ResourceId
        //                             where (rdm.ValidityStart <= DateTime.Now && rdm.ValidityEnd > DateTime.Now) &&
        //                             rdm.TenantId == tenantId
        //                             group rdm by rdm.DependencyResourceId into rdm1
        //                             select rdm1
        //                         ).ToList();

        //        if (dependencyResID == platformInstanceId)
        //        {
        //            mainresources = (from rdm in rdmDS.GetAny().ToArray()
        //                                 where rdm.DependencyResourceId == dependencyResID &&
        //                                 (rdm.ValidityStart <= DateTime.Now && rdm.ValidityEnd > DateTime.Now) &&
        //                                 rdm.TenantId == tenantId
        //                                 group rdm by rdm.DependencyResourceId into rdm1
        //                                 select rdm1
        //                         ).ToList();
        //        }

        //        /*var resourceIDs = (from rdm in rdmDS.GetAny()
        //                           where rdm.DependencyResourceId == platformInstanceId &&
        //                           (rdm.ValidityStart <= DateTime.Now && rdm.ValidityEnd > DateTime.Now) &&
        //                           rdm.TenantId == tenantId
        //                           select rdm.ResourceId
        //                         ).ToList();

        //        var mainresources1 = (from rdm in rdmDS.GetAny()
        //                             where resourceIDs.Contains(rdm.DependencyResourceId) &&
        //                            // where rdm.DependencyResourceId == platformInstanceId &&
        //                             (rdm.ValidityStart <= DateTime.Now && rdm.ValidityEnd > DateTime.Now) &&
        //                             rdm.TenantId == tenantId
        //                             group rdm by rdm.DependencyResourceId into rdm1
        //                             select rdm1
        //                         ).ToList();
        //                         */

        //        var response = (from mr in mainresources
        //                        join rs in resDS.GetAny().ToArray() on mr.Key equals rs.ResourceId
        //                        join t in rtDS.GetAny().ToArray() on rs.ResourceTypeId equals t.ResourceTypeId
        //                        where rs.IsActive == true &&
        //                            rs.ValidityStart <= DateTime.Now && rs.ValidityEnd > DateTime.Now &&
        //                            t.ValidityStart <= DateTime.Now && t.ValidityEnd > DateTime.Now
        //                        group new { mr, rs, t } by new
        //                        {
        //                            t.ResourceTypeId,
        //                            t.ResourceTypeName,
        //                            mr.Key,
        //                            rs.ResourceName,
        //                            t.IsMainEntiry
        //                        } into grpRMD
        //                        select new BE.Resourcemodeldetail
        //                        {
        //                            resourcetypeid = Convert.ToString(grpRMD.Key.ResourceTypeId),
        //                            resourcetypename = grpRMD.Key.ResourceTypeName,
        //                            resourceid = grpRMD.Key.Key,
        //                            resourcename = grpRMD.Key.ResourceName,
        //                            ismainentiy=Convert.ToBoolean(grpRMD.Key.IsMainEntiry),
        //                            childdetails = (from grp in grpRMD
        //                                            from r in resDS.GetAny().ToArray()
        //                                            join rt in rtDS.GetAny().ToArray() on r.ResourceTypeId equals rt.ResourceTypeId
        //                                            where r.IsActive == true 
        //                                            && grp.mr.ToList().Select(r => r.ResourceId).ToList().Contains(r.ResourceId) &&
        //                                            r.ValidityStart <= DateTime.Now && r.ValidityEnd > DateTime.Now &&
        //                                            rt.ValidityStart <= DateTime.Now && rt.ValidityEnd > DateTime.Now &&
        //                                            r.ResourceId.StartsWith(grp.mr.Key + "_")
        //                                            && grp.mr.Key!=platformInstanceId
        //                                            group new { rt, r } by new
        //                                            {
        //                                                r.ResourceTypeId,
        //                                                rt.ResourceTypeName,
        //                                                rt.IsMainEntiry
        //                                            } into gpRT
        //                                            select new BE.Childdetail
        //                                            {
        //                                                resourcetypeid = Convert.ToString(gpRT.Key.ResourceTypeId),
        //                                                resourcetypename = gpRT.Key.ResourceTypeName,
        //                                                ismainentiy = Convert.ToBoolean(gpRT.Key.IsMainEntiry),
        //                                                resourcetypedetails = (from gp in gpRT.ToList()
        //                                                           select new BE.Resourcetypedetail
        //                                                           {
        //                                                               resourceid = gp.r.ResourceId,
        //                                                               resourcename = gp.r.ResourceName                                                                       
        //                                                           }).ToList()
        //                                            }).ToList(),
        //                            resourcedetails = (from grp in grpRMD
        //                                                from r in resDS.GetAny().ToArray()
        //                                                join rt in rtDS.GetAny().ToArray() on r.ResourceTypeId equals rt.ResourceTypeId
        //                                                where r.IsActive == true 
        //                                                && grp.mr.ToList().Select(r => r.ResourceId).ToList().Contains(r.ResourceId) &&
        //                                                r.ValidityStart <= DateTime.Now && r.ValidityEnd > DateTime.Now &&
        //                                                rt.ValidityStart <= DateTime.Now && rt.ValidityEnd > DateTime.Now &&
        //                                                //!r.ResourceId.StartsWith(grp.mr.Key + "_")
        //                                               (grp.mr.Key == platformInstanceId||!r.ResourceId.StartsWith(grp.mr.Key+"_"))
        //                                               group new { rt,r} by new
        //                                                {
        //                                                    r.ResourceTypeId,
        //                                                    rt.ResourceTypeName,
        //                                                    rt.IsMainEntiry
        //                                                } into gpRT
        //                                                select new BE.Resourcedetail
        //                                                {
        //                                                    resourcetypeid=Convert.ToString(gpRT.Key.ResourceTypeId),
        //                                                    resourcetypename= gpRT.Key.ResourceTypeName,
        //                                                    ismainentiy = Convert.ToBoolean(gpRT.Key.IsMainEntiry),
        //                                                    details =(from gp in gpRT.ToList()
        //                                                             select new  BE.Detail
        //                                                             {
        //                                                                 resourceid = gp.r.ResourceId,
        //                                                                 resourcename= gp.r.ResourceName,                                                                         
        //                                                                 childdetails= (from rdm in rdmDS.GetAny().ToArray()
        //                                                                                join r in resDS.GetAny().ToArray() on rdm.ResourceId equals r.ResourceId
        //                                                                                join rt in rtDS.GetAny().ToArray() on r.ResourceTypeId equals rt.ResourceTypeId
        //                                                                                where r.IsActive == true && 
        //                                                                                rdm.DependencyResourceId==gp.r.ResourceId &&
        //                                                                                (r.ValidityStart <= DateTime.Now && r.ValidityEnd > DateTime.Now) &&
        //                                                                                (rt.ValidityStart <= DateTime.Now && rt.ValidityEnd > DateTime.Now )
        //                                                                                group new { rt, r } by new
        //                                                                                {
        //                                                                                    r.ResourceTypeId,
        //                                                                                    rt.ResourceTypeName,
        //                                                                                    rt.IsMainEntiry
        //                                                                                } into gpch
        //                                                                                select new BE.Childdetail
        //                                                                                {
        //                                                                                    resourcetypeid = Convert.ToString(gpch.Key.ResourceTypeId),
        //                                                                                    resourcetypename = gpch.Key.ResourceTypeName,
        //                                                                                    ismainentiy = Convert.ToBoolean(gpch.Key.IsMainEntiry),
        //                                                                                    resourcetypedetails = (from gp1 in gpch.ToList()
        //                                                                                               select new BE.Resourcetypedetail
        //                                                                                               {
        //                                                                                                   resourceid = gp1.r.ResourceId,
        //                                                                                                   resourcename = gp1.r.ResourceName                                                                                                           
        //                                                                                               }).ToList()
        //                                                                                }).ToList()

        //                                                             }).ToList()
        //                                                }).ToList()
        //                        }).ToList();

        //        var jhjfdfg =JsonConvert.SerializeObject(response);

        //        PlatformsDS pDS = new PlatformsDS();
        //        BE.ResourceModel resourceModel = new BE.ResourceModel();
        //        var platform = pDS.GetAll().Where(p => p.PlatformId == Convert.ToInt32(platformInstanceId)).FirstOrDefault();
        //        if(platform!=null)
        //        {                    
        //            resourceModel.tenantid = Convert.ToString(tenantId);
        //            resourceModel.platformid = Convert.ToString(platform.PlatformId);
        //            resourceModel.platformname = platform.PlatformName;
        //            resourceModel.resourcemodeldetails = response;
        //        }
        //        return resourceModel;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw ex;
        //    }
        //}

        public BE.ResourceTypeConfiguration getResourceTypeConfiguration(string platformInstanceId, string tenantId)
        {
            BE.ResourceTypeConfiguration typeConfiguration = new BE.ResourceTypeConfiguration();
            try
            {
                ResourceDS resDS = new ResourceDS();
                ResourceTypeDS typeDS = new ResourceTypeDS();
                PlatformsDS platformsDS = new PlatformsDS();

                var platforms = (from p in platformsDS.GetAny().ToArray()
                                 where p.PlatformId == Convert.ToInt32(platformInstanceId)
                                 && p.TenantId == Convert.ToInt32(tenantId)
                                 select p).FirstOrDefault();

                if (platforms != null)
                {
                    typeConfiguration.platformid = Convert.ToString(platforms.PlatformId);
                    typeConfiguration.tenantid = platforms.TenantId;
                    typeConfiguration.platformtype = platforms.PlatformName;

                    typeConfiguration.resourcetypedetails = (from r in resDS.GetAny().ToArray()
                                                             join rt in typeDS.GetAny().ToArray() on r.ResourceTypeId equals rt.ResourceTypeId
                                                             where r.TenantId == rt.TenantId && rt.IsMainEntiry == true
                                                             && r.PlatformId == Convert.ToInt32(platformInstanceId) && r.TenantId == Convert.ToInt32(tenantId)
                                                             group new { rt, r } by new { rt.ResourceTypeId, rt.ResourceTypeName,rt.ResourceTypeDisplayName } into g
                                                             //group rt by new {rt.ResourceTypeId,rt.ResourceTypeName} into g 
                                                             select new BE.ResourceTypeDetails
                                                             {
                                                                 resourcetypename = g.Key.ResourceTypeName,
                                                                 resourcetypedisplayname=g.Key.ResourceTypeDisplayName,
                                                                 resourcetypeid = Convert.ToString(g.Key.ResourceTypeId),
                                                                 details = (from d in g
                                                                            select new BE.ResDetails {
                                                                                resourceid = d.r.ResourceId,
                                                                                resourcename = d.r.ResourceName
                                                                            }).ToList()

                                                             }).Distinct().OrderBy(r=>r.resourcetypeid).ToList();

                }

            }
            catch (Exception exception)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(exception, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return typeConfiguration;
        }

        public string AddChildresource(BE.ChildResource childResource, BE.ResourceDetails parentResource, int tenantid, int platformid, string resourcemodelversion, int child_observableId)
        {
            StringBuilder responseMessage = new StringBuilder();
            try
            {
                string ParentResourceID = parentResource.resourceid;
                ResourceDS res = new ResourceDS();
                ResourceAttributesDS resAttr = new ResourceAttributesDS();
                ResourceDependencyMapDS rdMap = new ResourceDependencyMapDS();
                ObservableResourceMapDS observable = new ObservableResourceMapDS();

                int childResourceTypeId = Convert.ToInt32(childResource.resourcetypeid);
                string max_childResID = string.Empty;
                var Childresource_level = (from r in res.GetAny().ToArray()
                                           join d in rdMap.GetAny().ToArray() on r.ResourceId equals d.ResourceId
                                           where r.ResourceTypeId == childResourceTypeId
                                           select d.DependencyType).FirstOrDefault().Trim();

                if (Childresource_level.ToLower() == "level1")
                {
                    max_childResID = (from r in res.GetAll()
                                      join d in rdMap.GetAll() on r.ResourceId equals d.ResourceId
                                      where d.DependencyType.Trim() == Childresource_level
                                      select r.ResourceId).OrderByDescending(r => r.Length).ThenByDescending(r => r).FirstOrDefault();
                }
                else if (Childresource_level.ToLower() == "level2")
                {
                    /*max_childResID = (from r in res.GetAll()
                                 join d in rdMap.GetAll() on r.ResourceId equals d.ResourceId
                                 where d.DependencyResourceId == resourceDetails.parentdetails.FirstOrDefault().resourceid
                                 && r.ResourceTypeId == resourceTypeId
                                 select r.ResourceId).OrderByDescending(r => r.Length).ThenByDescending(r => r).FirstOrDefault();*/
                    max_childResID = (from r in res.GetAll()
                                      join d in rdMap.GetAll() on r.ResourceId equals d.ResourceId
                                      where d.DependencyResourceId == ParentResourceID
                                      // && r.ResourceTypeId == childResourceTypeId
                                      && r.ResourceId.StartsWith(ParentResourceID)
                                      select r.ResourceId).OrderByDescending(r => r.Length).ThenByDescending(r => r).FirstOrDefault();
                }

                string newChildResID = getNewResourceid(max_childResID != null ? max_childResID : ParentResourceID + "_0");
                resource resourceDetailsToAdd = new resource();

                //Add child data to resource table
                resourceDetailsToAdd.ResourceName = childResource.resourcename;
                resourceDetailsToAdd.ResourceId = newChildResID;
                resourceDetailsToAdd.ResourceTypeId = Convert.ToInt32(childResource.resourcetypeid);
                resourceDetailsToAdd.TenantId = tenantid;
                resourceDetailsToAdd.PlatformId = platformid;
                resourceDetailsToAdd.VersionNumber = resourcemodelversion;
                resourceDetailsToAdd.Source = childResource.source;
                //resourceDetailsToAdd.Source = childResource.resourceattribute.Where(a => a.attributename.ToLower() == "ipaddress").FirstOrDefault() != null ?
                //                           childResource.resourceattribute.Where(a => a.attributename.ToLower() == "ipaddress").FirstOrDefault().attributevalue : childResource.resourcename;
                resourceDetailsToAdd.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                resourceDetailsToAdd.ValidityStart = childResource.startdate != null ? (childResource.startdate.Contains("India Standard Time") ? DateTime.ParseExact(childResource.startdate, "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(India Standard Time)'", System.Globalization.CultureInfo.InvariantCulture) : Convert.ToDateTime(childResource.startdate)) : DateTime.Parse("2019-01-08 00:00:00.000000").ToUniversalTime();
                resourceDetailsToAdd.ValidityEnd = childResource.enddate != null ? (childResource.enddate.Contains("India Standard Time") ? DateTime.ParseExact(childResource.enddate, "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(India Standard Time)'", System.Globalization.CultureInfo.InvariantCulture) : Convert.ToDateTime(childResource.enddate)) : DateTime.Parse("2099-08-01 00:00:00.000000").ToUniversalTime();
                //resourceDetailsToAdd.ValidityStart = childResource.startdate != null ?Convert.ToDateTime(childResource.startdate) : DateTime.Parse("2019-01-08 00:00:00.000000").ToUniversalTime();
                //resourceDetailsToAdd.ValidityEnd = childResource.enddate != null ? Convert.ToDateTime(childResource.enddate) : DateTime.Parse("2099-08-01 00:00:00.000000").ToUniversalTime();
                resourceDetailsToAdd.IsActive = childResource.dontmonitor;
                var childres_result1 = res.Insert(resourceDetailsToAdd);
                responseMessage.Append(childres_result1 == null ? "\n Insertion of child resource data failed for the resourceID:" + ParentResourceID : "\n Insertion of child resource data success for the resourceID:" + ParentResourceID);

                if (childres_result1 != null)
                {
                    //add data to resource_dependency_map table 
                    resource_dependency_map dependency_Map = new resource_dependency_map()
                    {
                        ResourceId = childres_result1.ResourceId,
                        DependencyResourceId = ParentResourceID,
                        DependencyType = Childresource_level.ToUpper(),
                        Priority = 1,
                        CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name,
                        TenantId = tenantid,
                        ValidityStart = DateTime.Parse("2019-01-08 00:00:00.000000").ToUniversalTime(),
                        ValidityEnd = DateTime.Parse("2099-08-01 00:00:00.000000").ToUniversalTime(),
                        PortfolioId = parentResource.portfolioid
                    };
                    var child_rdp_result = rdMap.Insert(dependency_Map);
                    responseMessage.Append(child_rdp_result == null ? "\n Insertion of resource_dependency_map data failed for the child resourceID:" + childres_result1.ResourceId : "\n Insertion of resource_dependency_map data success for the child resourceID:" + childres_result1.ResourceId);

                    //add data to observable_resource_map
                    //var child_observableID = Convert.ToInt32(resource.observablesandremediationplans.Where(r => r.resourcetypeid == childResource.resourcetypeid).FirstOrDefault().observablesandremediations.FirstOrDefault().ObservableId);
                    var prevObservable = (from ob in observable.GetAny()
                                          where ob.ResourceId == max_childResID
                                          && ob.ObservableId == child_observableId
                                          select ob).FirstOrDefault();
                    if (prevObservable == null)
                    {
                        prevObservable = (from o in observable.GetAny()
                                          where o.ObservableId == child_observableId
                                          select o).OrderByDescending(r => r.ResourceId.Length).ThenByDescending(r => r.ResourceId).FirstOrDefault();
                    }
                    if (prevObservable != null)
                    {
                        observable_resource_map resource_Map = new observable_resource_map();
                        resource_Map.ResourceId = childres_result1.ResourceId;
                        resource_Map.ObservableId = child_observableId;
                        resource_Map.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                        resource_Map.TenantId = tenantid;
                        resource_Map.OperatorId = prevObservable.OperatorId;
                        resource_Map.LowerThreshold = prevObservable.LowerThreshold;
                        resource_Map.UpperThreshold = prevObservable.UpperThreshold;
                        resource_Map.ValidityStart = DateTime.Parse("2019-01-08 00:00:00.000000").ToUniversalTime();
                        resource_Map.ValidityEnd = DateTime.Parse("2099-08-01 00:00:00.000000").ToUniversalTime();

                        var observale_result = observable.Insert(resource_Map);
                        responseMessage.Append(observale_result == null ? "\n Insertion of observable_resource_map data failed for the child resourceID:" + childres_result1.ResourceId : "\n Insertion of observable_resource_map data success for the child resourceID:" + childres_result1.ResourceId);
                    }



                    //add data to resource_attributes table 
                    List<resource_attributes> Child_resource_Attributes = new List<resource_attributes>();
                    foreach (BE.ResourceAttribute resourceAttr in childResource.resourceattribute)
                    {
                        Child_resource_Attributes.Add(new resource_attributes()
                        {
                            AttributeName = resourceAttr.attributename,
                            AttributeValue = resourceAttr.IsSecret ? Encrypt(resourceAttr.attributevalue) : resourceAttr.attributevalue,
                            DisplayName = resourceAttr.displayname,
                            Description = resourceAttr.description,
                            ResourceId = newChildResID,
                            IsSecret= resourceAttr.IsSecret,
                            TenantId = tenantid,
                            AttributeCategory = resourceAttr.attributeCategory,
                            Remarks = resourceAttr.remarks,
                            isHiddenInUi = resourceAttr.Ishiddeninui,
                            CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name
                        });
                    }
                    var result = resAttr.InsertBatch(Child_resource_Attributes);
                    responseMessage.Append(result == null ? "\n Insertion of resource_attributes data failed for the child resourceID:" + childres_result1.ResourceId : "\n Insertion of observable_resource_map data success for the child resourceID:" + childres_result1.ResourceId);

                    if (childResource.observablesandremediations != null && childResource.observablesandremediations.Where(r => r.ismodified == true).FirstOrDefault() != null)
                    {
                        string resultMsg = AddObservationsandRemediations(childResource.observablesandremediations, newChildResID, tenantid);
                    }
                }
                return responseMessage.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string AddObservationsandRemediations(List<BE.observablesandremediation> observablesandremediations, string resourceID, int tenantID)
        {
            try
            {
                ResourceObservableActionMapDS roamDS = new ResourceObservableActionMapDS();
                ResourceObservableRemediationPlanMapDS rorPlanMapDS = new ResourceObservableRemediationPlanMapDS();
                StringBuilder responseMessage = new StringBuilder();
                if (observablesandremediations != null && observablesandremediations.Where(r => r.ismodified == true).FirstOrDefault() != null)
                {
                    foreach (BE.observablesandremediation obrBE in observablesandremediations)
                    {
                        roamDS = new ResourceObservableActionMapDS();

                        var entity = roamDS.GetAll().Where(c => c.ResourceId == resourceID &&
                                                                c.ObservableId == Convert.ToInt32(obrBE.ObservableId) &&
                                                                c.ActionId == Convert.ToInt32(obrBE.ObservableActionId)).FirstOrDefault();
                        if (entity != null)
                        {
                            //Update observable details 
                            entity.ValidityStart = entity.ValidityStart == null ? DateTime.Parse("2019-08-08").ToUniversalTime() : entity.ValidityStart;
                            entity.ValidityEnd = obrBE.isObsSelected ? DateTime.Parse("2099-08-08").ToUniversalTime() : DateTime.Now.ToUniversalTime();
                            entity.Name = obrBE.name;
                            entity.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                            var resmsg = roamDS.Update(entity);
                        }
                        else
                        {
                            resource_observable_action_map action_Map = new resource_observable_action_map()
                            {
                                ResourceId = resourceID,
                                ObservableId = Convert.ToInt32(obrBE.ObservableId),
                                ActionId = Convert.ToInt32(obrBE.ObservableActionId),
                                ValidityStart = DateTime.Parse("2019-08-08").ToUniversalTime(),
                                ValidityEnd = obrBE.isObsSelected ? DateTime.Parse("2099-08-08").ToUniversalTime() : DateTime.Now.ToUniversalTime(),
                                Name = obrBE.name,
                                TenantId = tenantID,
                                CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name
                            };
                            var resmsg = roamDS.Insert(action_Map);
                        }
                        rorPlanMapDS = new ResourceObservableRemediationPlanMapDS();
                        var remEntity = rorPlanMapDS.GetAll().Where(c => c.ResourceId == resourceID &&
                                                                  c.ObservableId == Convert.ToInt32(obrBE.ObservableId) &&
                                                                  c.RemediationPlanId == Convert.ToInt32(obrBE.RemediationPlanId)).FirstOrDefault();
                        if (remEntity != null)
                        {
                            remEntity.ValidityStart = remEntity.ValidityStart == null ? DateTime.Parse("2019-08-08").ToUniversalTime() : remEntity.ValidityStart;
                            remEntity.ValidityEnd = obrBE.isRemSelected ? DateTime.Parse("2099-08-08").ToUniversalTime() : DateTime.Now.ToUniversalTime();
                            // remEntity.Name = obrBE.name;
                            remEntity.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                            var resmsg = rorPlanMapDS.Update(remEntity);

                        }
                        else
                        {
                            //Update remediation details 
                            resource_observable_remediation_plan_map remediation_Plan_Map = new resource_observable_remediation_plan_map()
                            {
                                ResourceId = resourceID,
                                ObservableId = Convert.ToInt32(obrBE.ObservableId),
                                RemediationPlanId = Convert.ToInt32(obrBE.RemediationPlanId),
                                TenantId = tenantID,
                                ValidityStart = DateTime.Parse("2019-08-08").ToUniversalTime(),
                                ValidityEnd = obrBE.isRemSelected ? DateTime.Parse("2099-08-08").ToUniversalTime() : DateTime.Now.ToUniversalTime(),
                                CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name
                            };
                            var response = rorPlanMapDS.Insert(remediation_Plan_Map);
                            responseMessage.Append(response == null ? "Updation of Observation & Remediation details  failed for the resourceId:" + resourceID : " Updation of Observation & Remediation details success for the resource:" + resourceID);
                        }

                    }
                }
                return responseMessage.ToString();
            }
            catch (Exception ex)
            {

                return ex.Message;
            }
        }

        public string AddResourceModel(BE.PlatformInfo resource)
        {
            StringBuilder responseMessage = new StringBuilder();
            string newResourceID = null;
            try
            {
                if (resource != null)
                {
                    ResourceDS res = new ResourceDS();
                    ResourceAttributesDS resAttr = new ResourceAttributesDS();
                    ResourceDependencyMapDS rdMap = new ResourceDependencyMapDS();
                    ObservableResourceMapDS observable = new ObservableResourceMapDS();
                    ResourcetypeObservableActionMapDS rtoamDS = new ResourcetypeObservableActionMapDS();
                    ResourcetypeObservableRemediationPlanMapDS rtorPlanMapDS = new ResourcetypeObservableRemediationPlanMapDS();

                    resource resourceDetailsToAdd = new resource();
                    foreach (BE.ResourceDetails resourceDetails in resource.resourcedetails)
                    {

                        int resourceTypeId = Convert.ToInt32(resourceDetails.resourcetypeid);
                        string max_resID = string.Empty;
                        var resource_level = (from r in res.GetAll()
                                              join d in rdMap.GetAll() on r.ResourceId equals d.ResourceId
                                              where r.ResourceTypeId == resourceTypeId
                                              select d.DependencyType).FirstOrDefault().Trim();

                        if (resource_level.ToLower() == "level1")
                        {
                            max_resID = (from r in res.GetAll()
                                         join d in rdMap.GetAll() on r.ResourceId equals d.ResourceId
                                         where d.DependencyType.Trim() == resource_level && r.PlatformId==resource.platformid
                                         select r.ResourceId).OrderByDescending(r => r.Length).ThenByDescending(r => r).FirstOrDefault();
                        }
                        else if (resource_level.ToLower() == "level2")
                        {
                            max_resID = (from r in res.GetAll()
                                         join d in rdMap.GetAll() on r.ResourceId equals d.ResourceId
                                         where d.DependencyResourceId == resourceDetails.parentdetails.FirstOrDefault().resourceid
                                         //&& r.ResourceTypeId == resourceTypeId
                                         select r.ResourceId).OrderByDescending(r => r.Length).ThenByDescending(r => r).FirstOrDefault();
                        }
                        newResourceID = getNewResourceid(max_resID);


                        //Add data to resource table

                        resourceDetailsToAdd.ResourceName = resourceDetails.resourcename;
                        resourceDetailsToAdd.ResourceId = newResourceID;
                        resourceDetailsToAdd.ResourceTypeId = Convert.ToInt32(resourceDetails.resourcetypeid);
                        resourceDetailsToAdd.TenantId = resource.tenantid;
                        resourceDetailsToAdd.PlatformId = resource.platformid;
                        resourceDetailsToAdd.VersionNumber = resource.resourcemodelversion;
                        resourceDetailsToAdd.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                        resourceDetailsToAdd.Source = resourceDetails.source;
                        //resourceDetailsToAdd.Source = resourceDetails.resourceattribute.Where(a => a.attributename.ToLower() == "ipaddress").FirstOrDefault() != null ?
                        //                              resourceDetails.resourceattribute.Where(a => a.attributename.ToLower() == "ipaddress").FirstOrDefault().attributevalue : resourceDetails.resourcename;
                        resourceDetailsToAdd.ValidityStart = resourceDetails.startdate != null ? (resourceDetails.startdate.Contains("India Standard Time") ? DateTime.ParseExact(resourceDetails.startdate, "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(India Standard Time)'", System.Globalization.CultureInfo.InvariantCulture) : Convert.ToDateTime(resourceDetails.startdate)) : DateTime.Parse("2019-01-08 00:00:00.000000").ToUniversalTime();
                        resourceDetailsToAdd.ValidityEnd = resourceDetails.enddate != null ? (resourceDetails.enddate.Contains("India Standard Time") ? DateTime.ParseExact(resourceDetails.enddate, "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(India Standard Time)'", System.Globalization.CultureInfo.InvariantCulture) : Convert.ToDateTime(resourceDetails.enddate)) : DateTime.Parse("2099-08-01 00:00:00.000000").ToUniversalTime();
                        //resourceDetailsToAdd.ValidityStart = resourceDetails.startdate!=null? Convert.ToDateTime(resourceDetails.startdate):DateTime.Parse("2019-01-08 00:00:00.000000").ToUniversalTime();
                        //resourceDetailsToAdd.ValidityEnd = resourceDetails.enddate != null ?Convert.ToDateTime(resourceDetails.enddate) : DateTime.Parse("2099-08-01 00:00:00.000000").ToUniversalTime();
                        resourceDetailsToAdd.IsActive = resourceDetails.dontmonitor;
                        var result1 = res.Insert(resourceDetailsToAdd);
                        responseMessage.Append(result1 == null ? "Insertion of resource data failed for the resourcetype:" + resourceDetails.resourcetypename : "Insertion of resource data success for the resourcetype:" + resourceDetails.resourcetypename);

                        if (result1 != null)
                        {
                            //add data to resource_dependency_map table
                            resource_dependency_map dependency_Map = new resource_dependency_map()
                            {
                                ResourceId = result1.ResourceId,
                                DependencyResourceId = resourceDetails.parentdetails.FirstOrDefault() != null ? resourceDetails.parentdetails.FirstOrDefault().resourceid : Convert.ToString(0),
                                DependencyType = resource_level.ToUpper(),
                                Priority = 1,
                                CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name,
                                TenantId = resource.tenantid,
                                ValidityStart = DateTime.Parse("2019-01-08 00:00:00.000000").ToUniversalTime(),
                                ValidityEnd = DateTime.Parse("2099-08-01 00:00:00.000000").ToUniversalTime(),
                                PortfolioId = resourceDetails.portfolioid!=null? resourceDetails.portfolioid :""

                            };
                            var rdp_result = rdMap.Insert(dependency_Map);
                            responseMessage.Append(rdp_result == null ? "\n Insertion of resource_dependency_map data failed for the resourceID:" + result1.ResourceId : "\n Insertion of resource_dependency_map data success for the resourceID:" + result1.ResourceId);


                            //add data to observable_resource_map
                            //var obser = resource.observablesandremediationplans.Where(r => r.resourcetypeid == resourceDetails.resourcetypeid).FirstOrDefault();
                            var obser = new BE.observablesandremediationplan();
                            if (obser != null && obser.observablesandremediations != null)
                            {
                                var observablesandremediationsDetails = obser.observablesandremediations.FirstOrDefault();

                                if (observablesandremediationsDetails != null)
                                {
                                    var observableID = Convert.ToInt32(observablesandremediationsDetails.ObservableId);
                                    var prevObservable = (from ob in observable.GetAll()
                                                          where ob.ResourceId == max_resID
                                                          && ob.ObservableId == observableID
                                                          select ob).FirstOrDefault();
                                    if (prevObservable != null)
                                    {
                                        observable_resource_map resource_Map = new observable_resource_map();
                                        resource_Map.ResourceId = result1.ResourceId;
                                        resource_Map.ObservableId = observableID;
                                        resource_Map.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                        resource_Map.TenantId = resource.tenantid;
                                        resource_Map.OperatorId = prevObservable.OperatorId;
                                        resource_Map.LowerThreshold = prevObservable.LowerThreshold;
                                        resource_Map.UpperThreshold = prevObservable.UpperThreshold;
                                        resource_Map.ValidityStart = DateTime.Parse("2019-01-08 00:00:00.000000").ToUniversalTime();
                                        resource_Map.ValidityEnd = DateTime.Parse("2099-08-01 00:00:00.000000").ToUniversalTime();
                                        var observale_result = observable.Insert(resource_Map);
                                        responseMessage.Append(observale_result == null ? "\n Insertion of observable_resource_map data failed for the resourceID:" + result1.ResourceId : "\n Insertion of observable_resource_map data success for the resourceID:" + result1.ResourceId);
                                    }
                                }
                                else
                                {
                                    responseMessage.Append("observablesandremediations not found for resourcetypeID:" + resourceDetails.resourcetypeid);
                                }
                            }
                            else
                                responseMessage.Append("observablesandremediationplans not found for resourcetypeID:" + resourceDetails.resourcetypeid);


                            //add data to resource_attributes table
                            IList<resource_attributes> resource_Attributes = new List<resource_attributes>();
                            foreach (BE.ResourceAttribute resourceAttr in resourceDetails.resourceattribute)
                            {
                                resource_Attributes.Add(new resource_attributes()
                                {
                                    AttributeName = resourceAttr.attributename,
                                    AttributeValue =resourceAttr.IsSecret?Encrypt(resourceAttr.attributevalue):resourceAttr.attributevalue,
                                    DisplayName = resourceAttr.displayname,
                                    Description = resourceAttr.description,
                                    ResourceId = newResourceID,
                                    IsSecret= resourceAttr.IsSecret,
                                    TenantId = resource.tenantid,
                                    AttributeCategory = resourceAttr.attributeCategory,
                                    Remarks = resourceAttr.remarks,
                                    isHiddenInUi = resourceAttr.Ishiddeninui,
                                    CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name

                                });
                            }
                            var result = resAttr.InsertBatch(resource_Attributes);
                            responseMessage.Append(result == null ? "\n Insertion of resource_attributes data failed for the resourceID:" + result1.ResourceId : "\n Insertion of observable_resource_map data success for the resourceID:" + result1.ResourceId);

                            if (resourceDetails.observablesandremediations != null && resourceDetails.observablesandremediations.Where(r => r.ismodified == true).FirstOrDefault() != null)
                            {
                                string resultMsg = AddObservationsandRemediations(resourceDetails.observablesandremediations, newResourceID, resource.tenantid);
                            }
                        }


                        IList<resource_attributes> Child_resource_Attributes = new List<resource_attributes>();

                        if (resourceDetails.childdetails != null)
                        {
                            foreach (BE.ChildResource childResource in resourceDetails.childdetails)
                            {
                                res = new ResourceDS();
                                resAttr = new ResourceAttributesDS();
                                rdMap = new ResourceDependencyMapDS();
                                observable = new ObservableResourceMapDS();

                                int childResourceTypeId = Convert.ToInt32(childResource.resourcetypeid);
                                string max_childResID = string.Empty;
                                var Childresource_level = (from r in res.GetAny().ToArray()
                                                           join d in rdMap.GetAny().ToArray() on r.ResourceId equals d.ResourceId
                                                           where r.ResourceTypeId == childResourceTypeId
                                                           select d.DependencyType).FirstOrDefault().Trim();

                                if (Childresource_level.ToLower() == "level1")
                                {
                                    max_childResID = (from r in res.GetAll()
                                                      join d in rdMap.GetAll() on r.ResourceId equals d.ResourceId
                                                      where d.DependencyType.Trim() == Childresource_level && r.PlatformId==resource.platformid
                                                      select r.ResourceId).OrderByDescending(r => r.Length).ThenByDescending(r => r).FirstOrDefault();
                                }
                                else if (Childresource_level.ToLower() == "level2")
                                {
                                    /*max_childResID = (from r in res.GetAll()
                                                 join d in rdMap.GetAll() on r.ResourceId equals d.ResourceId
                                                 where d.DependencyResourceId == resourceDetails.parentdetails.FirstOrDefault().resourceid
                                                 && r.ResourceTypeId == resourceTypeId
                                                 select r.ResourceId).OrderByDescending(r => r.Length).ThenByDescending(r => r).FirstOrDefault();*/
                                    max_childResID = (from r in res.GetAll()
                                                      join d in rdMap.GetAll() on r.ResourceId equals d.ResourceId
                                                      where d.DependencyResourceId == result1.ResourceId
                                                      && r.ResourceTypeId == childResourceTypeId
                                                      && r.ResourceId.StartsWith(newResourceID)
                                                      select r.ResourceId).OrderByDescending(r => r.Length).ThenByDescending(r => r).FirstOrDefault();
                                }

                                string newChildResID = getNewResourceid(max_childResID != null ? max_childResID : newResourceID + "_0");
                                resourceDetailsToAdd = new resource();

                                //Add child data to resource table
                                resourceDetailsToAdd.ResourceName = childResource.resourcename;
                                resourceDetailsToAdd.ResourceId = newChildResID;
                                resourceDetailsToAdd.ResourceTypeId = Convert.ToInt32(childResource.resourcetypeid);
                                resourceDetailsToAdd.TenantId = resource.tenantid;
                                resourceDetailsToAdd.PlatformId = resource.platformid;
                                resourceDetailsToAdd.VersionNumber = resource.resourcemodelversion;
                                resourceDetailsToAdd.Source = childResource.source;
                                //resourceDetailsToAdd.Source = childResource.resourceattribute.Where(a => a.attributename.ToLower() == "ipaddress").FirstOrDefault() != null ?
                                //                          childResource.resourceattribute.Where(a => a.attributename.ToLower() == "ipaddress").FirstOrDefault().attributevalue : childResource.resourcename;
                                resourceDetailsToAdd.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                //resourceDetailsToAdd.ValidityStart = childResource.startdate != null ?Convert.ToDateTime(childResource.startdate) : DateTime.Parse("2019-01-08 00:00:00.000000").ToUniversalTime();
                                //resourceDetailsToAdd.ValidityEnd = childResource.enddate != null ?Convert.ToDateTime(childResource.enddate) : DateTime.Parse("2099-08-01 00:00:00.000000").ToUniversalTime();
                                resourceDetailsToAdd.ValidityStart = childResource.startdate != null ? (childResource.startdate.Contains("India Standard Time") ? DateTime.ParseExact(childResource.startdate, "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(India Standard Time)'", System.Globalization.CultureInfo.InvariantCulture) : Convert.ToDateTime(childResource.startdate)) : DateTime.Parse("2019-01-08 00:00:00.000000").ToUniversalTime();
                                resourceDetailsToAdd.ValidityEnd = childResource.enddate != null ? (childResource.enddate.Contains("India Standard Time") ? DateTime.ParseExact(childResource.enddate, "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(India Standard Time)'", System.Globalization.CultureInfo.InvariantCulture) : Convert.ToDateTime(childResource.enddate)) : DateTime.Parse("2099-08-01 00:00:00.000000").ToUniversalTime();
                                resourceDetailsToAdd.IsActive = childResource.dontmonitor;

                                var childres_result1 = res.Insert(resourceDetailsToAdd);
                                responseMessage.Append(childres_result1 == null ? "\n Insertion of child resource data failed for the resourceID:" + result1.ResourceId : "\n Insertion of child resource data success for the resourceID:" + result1.ResourceId);

                                if (childres_result1 != null)
                                {
                                    //add data to resource_dependency_map table 
                                    resource_dependency_map dependency_Map = new resource_dependency_map()
                                    {
                                        ResourceId = childres_result1.ResourceId,
                                        DependencyResourceId = result1.ResourceId,
                                        DependencyType = Childresource_level.ToUpper(),
                                        Priority = 1,
                                        CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name,
                                        TenantId = resource.tenantid,
                                        ValidityStart = DateTime.Parse("2019-01-08 00:00:00.000000").ToUniversalTime(),
                                        ValidityEnd = DateTime.Parse("2099-08-01 00:00:00.000000").ToUniversalTime(),
                                        PortfolioId = resourceDetails.portfolioid

                                    };
                                    var child_rdp_result = rdMap.Insert(dependency_Map);
                                    responseMessage.Append(child_rdp_result == null ? "\n Insertion of resource_dependency_map data failed for the child resourceID:" + childres_result1.ResourceId : "\n Insertion of resource_dependency_map data success for the child resourceID:" + childres_result1.ResourceId);

                                    //add data to observable_resource_map
                                    //var child_observableID = Convert.ToInt32(resource.observablesandremediationplans.Where(r => r.resourcetypeid == childResource.resourcetypeid).FirstOrDefault().observablesandremediations.FirstOrDefault().ObservableId);
                                    var child_observableID = 1;
                                    var prevObservable = (from ob in observable.GetAny()
                                                          where ob.ResourceId == max_childResID
                                                          && ob.ObservableId == child_observableID
                                                          select ob).FirstOrDefault();
                                    if (prevObservable == null)
                                    {
                                        prevObservable = (from o in observable.GetAny()
                                                          where o.ObservableId == child_observableID
                                                          select o).OrderByDescending(r => r.ResourceId.Length).ThenByDescending(r => r.ResourceId).FirstOrDefault();
                                    }
                                    if (prevObservable != null)
                                    {
                                        observable_resource_map resource_Map = new observable_resource_map();
                                        resource_Map.ResourceId = childres_result1.ResourceId;
                                        resource_Map.ObservableId = child_observableID;
                                        resource_Map.CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                        resource_Map.TenantId = resource.tenantid;
                                        resource_Map.OperatorId = prevObservable.OperatorId;
                                        resource_Map.LowerThreshold = prevObservable.LowerThreshold;
                                        resource_Map.UpperThreshold = prevObservable.UpperThreshold;
                                        resource_Map.ValidityStart = DateTime.Parse("2019-01-08 00:00:00.000000").ToUniversalTime();
                                        resource_Map.ValidityEnd = DateTime.Parse("2099-08-01 00:00:00.000000").ToUniversalTime();

                                        var observale_result = observable.Insert(resource_Map);
                                        responseMessage.Append(observale_result == null ? "\n Insertion of observable_resource_map data failed for the child resourceID:" + childres_result1.ResourceId : "\n Insertion of observable_resource_map data success for the child resourceID:" + childres_result1.ResourceId);
                                    }



                                    //add data to resource_attributes table 
                                    Child_resource_Attributes = new List<resource_attributes>();
                                    foreach (BE.ResourceAttribute resourceAttr in childResource.resourceattribute)
                                    {
                                        Child_resource_Attributes.Add(new resource_attributes()
                                        {
                                            AttributeName = resourceAttr.attributename,
                                            AttributeValue = resourceAttr.IsSecret?Encrypt(resourceAttr.attributevalue): resourceAttr.attributevalue,
                                            DisplayName = resourceAttr.displayname,
                                            Description = resourceAttr.description,
                                            ResourceId = newChildResID,
                                            IsSecret= resourceAttr.IsSecret,
                                            TenantId = resource.tenantid,
                                            AttributeCategory = resourceAttr.attributeCategory,
                                            Remarks = resourceAttr.remarks,
                                            isHiddenInUi = resourceAttr.Ishiddeninui,
                                            CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name
                                        });
                                    }
                                    var result = resAttr.InsertBatch(Child_resource_Attributes);
                                    responseMessage.Append(result == null ? "\n Insertion of resource_attributes data failed for the child resourceID:" + childres_result1.ResourceId : "\n Insertion of observable_resource_map data success for the child resourceID:" + childres_result1.ResourceId);

                                    if (childResource.observablesandremediations != null && childResource.observablesandremediations.Where(r => r.ismodified == true).FirstOrDefault() != null)
                                    {
                                        string resultMsg = AddObservationsandRemediations(childResource.observablesandremediations, newChildResID, resource.tenantid);
                                    }
                                }

                            }
                        }

                        if (resourceDetails.cascadetochild)
                        {
                            updateCascadetoChild(result1.ResourceId, resource.platformid, resource.tenantid, resourceDetails.dontmonitor);
                        }
                    }


                }

            }

            catch (Exception ex)
            {
                throw ex;
            }
            return responseMessage.ToString();
        }

        public string getNewResourceid(String ID)
        {
            var splitString = ID.Split('_');
            int n = Convert.ToInt32(splitString[splitString.Length - 1]);
            n = n + 1;
            splitString[splitString.Length - 1] = Convert.ToString(n);
            //int[] result = n.ToString().Select(o => Convert.ToInt32(o)).ToArray();
            //int i = 0;
            string newResourceID = null;
            for (int i = 0; i < splitString.Length; i++)
            {
                if (i != splitString.Length - 1)
                {
                    newResourceID = newResourceID + splitString[i] + "_";
                }
                else
                {
                    newResourceID = newResourceID + splitString[i];
                }
            }

            return newResourceID;
        }

        public string getResourceDependencyLevel()
        {

            return null;
        }
        public string updateResourceModelConfiguration(BE.PlatformInfo resource)
        {
            
            StringBuilder responseMessage = new StringBuilder();
            try
            {
                if (resource != null)
                {
                    ResourceAttributesDS raDs = new ResourceAttributesDS();
                    ResourcetypeObservableActionMapDS rtoamDS = new ResourcetypeObservableActionMapDS();
                    ResourcetypeObservableRemediationPlanMapDS rtorPlanMapDS = new ResourcetypeObservableRemediationPlanMapDS();
                    ResourceDS resDS = new ResourceDS();
                    ResourceObservableActionMapDS roamDS = new ResourceObservableActionMapDS();
                    ResourceObservableRemediationPlanMapDS rorPlanMapDS = new ResourceObservableRemediationPlanMapDS();
                    ResourceDependencyMapDS rdmDS = new ResourceDependencyMapDS();

                    var resourceAttributes = raDs.GetAll();
                    foreach (BE.ResourceDetails resDetails in resource.resourcedetails)
                    {

                        if (resDetails.resourceid.Equals(resDetails.resourcename, StringComparison.InvariantCultureIgnoreCase))
                        {
                            string response = AddResourceModel(resource);
                            responseMessage.Append(response);
                        }
                        else
                        {
                            string resourceID = resDetails.resourceid;
                            resource resr = resDS.GetAny().Where(r => r.ResourceId == resourceID).FirstOrDefault();
                            if (resr != null)
                            {
                                resr.IsActive = resDetails.dontmonitor;
                                resr.ValidityStart = resDetails.startdate.Contains("India Standard Time") ? DateTime.ParseExact(resDetails.startdate, "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(India Standard Time)'", System.Globalization.CultureInfo.InvariantCulture) : Convert.ToDateTime(resDetails.startdate);
                                resr.ValidityEnd = resDetails.enddate.Contains("India Standard Time") ? DateTime.ParseExact(resDetails.enddate, "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(India Standard Time)'", System.Globalization.CultureInfo.InvariantCulture) : Convert.ToDateTime(resDetails.enddate);
                                resr.Source = resDetails.source;
                                var isSuccess = resDS.Update(resr);
                                responseMessage.Append(isSuccess == null ? "Updation of Resource details failed for the resourceId:" + resourceID : " Resource details updated successfully for the resourceId:" + resourceID);
                            }

                            //Update Portfolio for resource
                            var rdmObj = rdmDS.GetAny().Where(r => r.ResourceId == resDetails.resourceid).FirstOrDefault();
                            if (rdmObj != null)
                            {
                                rdmObj.PortfolioId = resDetails.portfolioid;
                                var isUpdated = rdmDS.Update(rdmObj);
                                responseMessage.Append(isUpdated == null ? "Updation of Resource portfolio details failed for the resourceId:" + resourceID : " Portfolio details updated successfully for the resourceId:" + resourceID);
                            }

                            //Update main resource attributes 
                            

                            IList<resource_attributes> resource_Attributes = new List<resource_attributes>();
                            IList<resource_attributes> Insert_resource_Attributes = new List<resource_attributes>();
                            foreach (BE.ResourceAttribute resAttr in resDetails.resourceattribute)
                            {
                                var entityItem = resourceAttributes.Where(c => c.ResourceId == resourceID &&
                                                         c.AttributeName == resAttr.attributename).FirstOrDefault();
                                if (entityItem != null)
                                {
                                    resource_Attributes.Add(new resource_attributes()
                                    {
                                        AttributeName = resAttr.attributename,
                                        AttributeValue =resAttr.IsSecret ? Encrypt(resAttr.attributevalue):resAttr.attributevalue,
                                        DisplayName = resAttr.displayname,
                                        Description = resAttr.description,
                                        ResourceId = resourceID,
                                        IsSecret=resAttr.IsSecret,
                                        AttributeCategory = resAttr.attributeCategory,
                                        Remarks = resAttr.remarks,
                                        isHiddenInUi = resAttr.Ishiddeninui,
                                        ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name,                                        
                                    });
                                }
                                else
                                {
                                    Insert_resource_Attributes.Add(new resource_attributes()
                                    {
                                        AttributeName = resAttr.attributename,
                                        AttributeValue = resAttr.IsSecret  ? Encrypt(resAttr.attributevalue) : resAttr.attributevalue,
                                        DisplayName = resAttr.displayname,
                                        Description = resAttr.description,
                                        ResourceId = resourceID,
                                        TenantId = resource.tenantid,
                                        IsSecret = resAttr.IsSecret,
                                        AttributeCategory = resAttr.attributeCategory,
                                        Remarks = resAttr.remarks,
                                        isHiddenInUi = resAttr.Ishiddeninui,
                                        CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name
                                    });
                                }
                            }
                            var res = raDs.UpdateBatch(resource_Attributes);
                            responseMessage.Append(res == null ? "Updation of Resource attributes failed for the resourceId:" + resourceID : " Resource attributes updated successfully for the resourceId:" + resourceID);
                            if (Insert_resource_Attributes.Count > 0)
                            {
                                var res1 = raDs.InsertBatch(Insert_resource_Attributes);
                                responseMessage.Append(res1 == null ? "Insertion of Resource attributes failed for the resourceId:" + resourceID : " Resource attributes inserted successfully for the resourceId:" + resourceID);

                            }

                            if (resDetails.observablesandremediations != null && resDetails.observablesandremediations.Where(r => r.ismodified == true).FirstOrDefault() != null)
                            {
                                foreach (BE.observablesandremediation obrBE in resDetails.observablesandremediations)
                                {
                                    ResourceObservableActionMapDS roamDS1 = new ResourceObservableActionMapDS();

                                    var entity = roamDS1.GetAll().Where(c => c.ResourceId == resourceID &&
                                                                            c.ObservableId == Convert.ToInt32(obrBE.ObservableId) &&
                                                                            c.ActionId == Convert.ToInt32(obrBE.ObservableActionId)).FirstOrDefault();
                                    if (entity != null)
                                    {
                                        //Update observable details 
                                        entity.ValidityStart = entity.ValidityStart == null ? DateTime.Parse("2019-08-08").ToUniversalTime() : entity.ValidityStart;
                                        entity.ValidityEnd = obrBE.isObsSelected ? DateTime.Parse("2099-08-08").ToUniversalTime() : DateTime.Now.ToUniversalTime();
                                        entity.Name = obrBE.name;
                                        entity.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                        var resmsg = roamDS1.Update(entity);
                                    }
                                    else
                                    {
                                        resource_observable_action_map action_Map = new resource_observable_action_map()
                                        {
                                            ResourceId = resourceID,
                                            ObservableId = Convert.ToInt32(obrBE.ObservableId),
                                            ActionId = Convert.ToInt32(obrBE.ObservableActionId),
                                            ValidityStart = DateTime.Parse("2019-08-08").ToUniversalTime(),
                                            ValidityEnd = obrBE.isObsSelected ? DateTime.Parse("2099-08-08").ToUniversalTime() : DateTime.Now.ToUniversalTime(),
                                            Name = obrBE.name,
                                            TenantId = resource.tenantid,
                                            CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name
                                        };
                                        var resmsg = roamDS1.Insert(action_Map);
                                    }
                                    if (!string.IsNullOrEmpty(obrBE.RemediationPlanId))
                                    {
                                        ResourceObservableRemediationPlanMapDS rorPlanMapDS1 = new ResourceObservableRemediationPlanMapDS();
                                        var remEntity = rorPlanMapDS1.GetAll().Where(c => c.ResourceId == resourceID &&
                                                                                  c.ObservableId == Convert.ToInt32(obrBE.ObservableId) &&
                                                                                  c.RemediationPlanId == Convert.ToInt32(obrBE.RemediationPlanId)).FirstOrDefault();
                                        if (remEntity != null)
                                        {
                                            remEntity.ValidityStart = remEntity.ValidityStart == null ? DateTime.Parse("2019-08-08").ToUniversalTime() : remEntity.ValidityStart;
                                            remEntity.ValidityEnd = obrBE.isRemSelected ? DateTime.Parse("2099-08-08").ToUniversalTime() : DateTime.Now.ToUniversalTime();
                                            // remEntity.Name = obrBE.name;
                                            remEntity.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                            var resmsg = rorPlanMapDS1.Update(remEntity);

                                        }
                                        else
                                        {
                                            //Update remediation details 
                                            resource_observable_remediation_plan_map remediation_Plan_Map = new resource_observable_remediation_plan_map()
                                            {
                                                ResourceId = resourceID,
                                                ObservableId = Convert.ToInt32(obrBE.ObservableId),
                                                RemediationPlanId = Convert.ToInt32(obrBE.RemediationPlanId),
                                                TenantId = resource.tenantid,
                                                ValidityStart = DateTime.Parse("2019-08-08").ToUniversalTime(),
                                                ValidityEnd = obrBE.isRemSelected ? DateTime.Parse("2099-08-08").ToUniversalTime() : DateTime.Now.ToUniversalTime(),
                                                CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name
                                            };
                                            var response = rorPlanMapDS1.Insert(remediation_Plan_Map);
                                            responseMessage.Append(response == null ? "Updation of Observation & Remediation details  failed for the resourceId:" + resourceID : " Updation of Observation & Remediation details success for the resource:" + resourceID);
                                        }
                                    }

                                }
                            }
                            if (resDetails.childdetails != null)
                            {
                                //Update child attributes 
                                resource_Attributes = new List<resource_attributes>();
                                Insert_resource_Attributes = new List<resource_attributes>();
                                foreach (BE.ChildResource childRes in resDetails.childdetails)
                                {
                                    if (childRes.resourceid.Equals(childRes.resourcename, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        //add child resource 
                                        // var child_observableID = Convert.ToInt32(resource.observablesandremediationplans.Where(r => r.resourcetypeid == childRes.resourcetypeid).FirstOrDefault().observablesandremediations.FirstOrDefault().ObservableId);
                                        var child_observableID = 1;
                                        string response = AddChildresource(childRes, resDetails, resource.tenantid, resource.platformid, resource.resourcemodelversion, child_observableID);
                                        responseMessage.Append(response);
                                    }
                                    else
                                    {
                                        ResourceDependencyMapDS rdmDS1 = new ResourceDependencyMapDS();
                                        //Update Portfolio for child resource
                                        var childrdmObj = rdmDS1.GetAny().Where(r => r.ResourceId == childRes.resourceid).FirstOrDefault();
                                        if (childrdmObj != null)
                                        {
                                            childrdmObj.PortfolioId = childRes.portfolioid;
                                            var isUpdated = rdmDS1.Update(childrdmObj);
                                            responseMessage.Append(isUpdated == null ? "Updation of Resource portfolio details failed for the resourceId:" + childRes.resourceid : " Portfolio details updated successfully for the resourceId:" + childRes.resourceid);
                                        }

                                        ResourceDS dS = new ResourceDS();
                                        resource child_res = dS.GetAny().Where(r => r.ResourceId == childRes.resourceid).FirstOrDefault();
                                        if (child_res != null)
                                        {
                                            child_res.IsActive = childRes.dontmonitor;
                                            child_res.Source = childRes.source;
                                            child_res.ValidityStart = childRes.startdate.Contains("India Standard Time") ? DateTime.ParseExact(childRes.startdate, "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(India Standard Time)'", System.Globalization.CultureInfo.InvariantCulture) : Convert.ToDateTime(childRes.startdate);
                                            child_res.ValidityEnd = childRes.enddate.Contains("India Standard Time") ? DateTime.ParseExact(childRes.enddate, "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(India Standard Time)'", System.Globalization.CultureInfo.InvariantCulture) : Convert.ToDateTime(childRes.enddate);
                                            var isSuccess = resDS.Update(child_res);
                                            responseMessage.Append(isSuccess == null ? "Updation of child Resource details failed for the resourceId:" + resourceID : "Child Resource details updated successfully for the resourceId:" + resourceID);
                                        }

                                        //Update resource attributes of child resoure
                                        foreach (BE.ResourceAttribute childResAttr in childRes.resourceattribute)
                                        {
                                            var entityItem = resourceAttributes.Where(c => c.ResourceId == childRes.resourceid &&
                                                                 c.AttributeName == childResAttr.attributename).FirstOrDefault();
                                            if (entityItem != null)
                                            {
                                                resource_Attributes.Add(new resource_attributes()
                                                {
                                                    AttributeName = childResAttr.attributename,
                                                    AttributeValue = childResAttr.IsSecret ? Encrypt(childResAttr.attributevalue) : childResAttr.attributevalue,
                                                    DisplayName = childResAttr.displayname,
                                                    Description = childResAttr.description,
                                                    ResourceId = childRes.resourceid,
                                                    TenantId = resource.tenantid,
                                                    IsSecret = childResAttr.IsSecret,
                                                    AttributeCategory = childResAttr.attributeCategory,
                                                    Remarks = childResAttr.remarks,
                                                    isHiddenInUi = childResAttr.Ishiddeninui,

                                                    ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name
                                                });
                                            }
                                            else
                                            {
                                                Insert_resource_Attributes.Add(new resource_attributes()
                                                {
                                                    AttributeName = childResAttr.attributename,
                                                    AttributeValue = childResAttr.IsSecret ? Encrypt(childResAttr.attributevalue): childResAttr.attributevalue,
                                                    DisplayName = childResAttr.displayname,
                                                    Description = childResAttr.description,
                                                    ResourceId = childRes.resourceid,
                                                    TenantId = resource.tenantid,
                                                    IsSecret = childResAttr.IsSecret,
                                                    AttributeCategory = childResAttr.attributeCategory,
                                                    Remarks = childResAttr.remarks,
                                                    isHiddenInUi = childResAttr.Ishiddeninui,
                                                    CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name
                                                });
                                            }
                                        }
                                        var result = raDs.UpdateBatch(resource_Attributes);
                                        responseMessage.Append(result == null ? "Updation of child Resource attributes fialed for the resourceId:" + resourceID : " Child Resource attributes updated successfully for the resourceId:" + resourceID);
                                        if (Insert_resource_Attributes.Count > 0)
                                        {
                                            var result1 = raDs.InsertBatch(Insert_resource_Attributes);
                                            responseMessage.Append(result1 == null ? "Insertion of child Resource attributes fialed for the resourceId:" + resourceID : " Child Resource attributes inserted successfully for the resourceId:" + resourceID);
                                        }

                                        if (childRes.observablesandremediations != null && childRes.observablesandremediations.Where(r => r.ismodified == true).FirstOrDefault() != null)
                                        {
                                            foreach (BE.observablesandremediation obrBE in childRes.observablesandremediations)
                                            {
                                                ResourceObservableActionMapDS roamDSch = new ResourceObservableActionMapDS();

                                                var entity = roamDSch.GetAll().Where(c => c.ResourceId == childRes.resourceid &&
                                                                                        c.ObservableId == Convert.ToInt32(obrBE.ObservableId) &&
                                                                                        c.ActionId == Convert.ToInt32(obrBE.ObservableActionId)).FirstOrDefault();
                                                if (entity != null)
                                                {
                                                    //Update observable details 
                                                    entity.ValidityStart = entity.ValidityStart == null ? DateTime.Parse("2019-08-08").ToUniversalTime() : entity.ValidityStart;
                                                    entity.ValidityEnd = obrBE.isObsSelected ? DateTime.Parse("2099-08-08").ToUniversalTime() : DateTime.Now.ToUniversalTime();
                                                    entity.Name = obrBE.name;
                                                    entity.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                                    var resmsg = roamDSch.Update(entity);
                                                }
                                                else
                                                {
                                                    resource_observable_action_map action_Map = new resource_observable_action_map()
                                                    {
                                                        ResourceId = childRes.resourceid,
                                                        ObservableId = Convert.ToInt32(obrBE.ObservableId),
                                                        ActionId = Convert.ToInt32(obrBE.ObservableActionId),
                                                        ValidityStart = DateTime.Parse("2019-08-08").ToUniversalTime(),
                                                        ValidityEnd = obrBE.isObsSelected ? DateTime.Parse("2099-08-08").ToUniversalTime() : DateTime.Now.ToUniversalTime(),
                                                        Name = obrBE.name,
                                                        TenantId = resource.tenantid,
                                                        CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name
                                                    };
                                                    var resmsg = roamDSch.Insert(action_Map);

                                                }
                                                if (!string.IsNullOrEmpty(obrBE.RemediationPlanId))
                                                {
                                                    ResourceObservableRemediationPlanMapDS rorPlanMapDSch = new ResourceObservableRemediationPlanMapDS();
                                                    var remEntity = rorPlanMapDSch.GetAll().Where(c => c.ResourceId == childRes.resourceid &&
                                                                                              c.ObservableId == Convert.ToInt32(obrBE.ObservableId) &&
                                                                                              c.RemediationPlanId == Convert.ToInt32(obrBE.RemediationPlanId)).FirstOrDefault();
                                                    if (remEntity != null)
                                                    {
                                                        remEntity.ValidityStart = remEntity.ValidityStart == null ? DateTime.Parse("2019-08-08").ToUniversalTime() : remEntity.ValidityStart;
                                                        remEntity.ValidityEnd = obrBE.isRemSelected ? DateTime.Parse("2099-08-08").ToUniversalTime() : DateTime.Now.ToUniversalTime();
                                                        // remEntity.Name = obrBE.name;
                                                        remEntity.ModifiedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                                                        var resmsg = rorPlanMapDSch.Update(remEntity);

                                                    }
                                                    else
                                                    {
                                                        //Insert remediation details 
                                                        resource_observable_remediation_plan_map remediation_Plan_Map = new resource_observable_remediation_plan_map()
                                                        {
                                                            ResourceId = childRes.resourceid,
                                                            ObservableId = Convert.ToInt32(obrBE.ObservableId),
                                                            RemediationPlanId = Convert.ToInt32(obrBE.RemediationPlanId),
                                                            TenantId = resource.tenantid,
                                                            ValidityEnd = obrBE.isRemSelected ? DateTime.Parse("2099-08-08").ToUniversalTime() : DateTime.Now.ToUniversalTime(),
                                                            CreatedBy = System.Security.Principal.WindowsIdentity.GetCurrent().Name
                                                        };
                                                        var response = rorPlanMapDSch.Insert(remediation_Plan_Map);
                                                        responseMessage.Append(response == null ? "Updation of Observation & Remediation details  failed for the resourceId:" + resourceID : " Updation of Observation & Remediation details success for the resource:" + resourceID);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }


                            if (resDetails.cascadetochild)
                            {
                                updateCascadetoChild(resDetails.resourceid, resource.platformid, resource.tenantid, resDetails.dontmonitor);
                            }

                        }



                    }

                }
                else
                    responseMessage.Append("Invalid input data");
            }
            catch (Exception exception)
            {
                responseMessage.Append("Updation of Observation & Remediation details failed.. Error Message:" + exception.Message);
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(exception, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;
                }
            }
            return responseMessage.ToString();
        }

        public BE.ObservablesandRemediationPlanDetails GetObservablesandRemediationDetails(int PlatformId, int TenantId, string ResourceTypeName)
        {

            ResourceDS resDS = new ResourceDS();
            ResourceTypeDS typeDS = new ResourceTypeDS();

            PlatformsDS platformsDS = new PlatformsDS();
            ResourceDependencyMapDS rdmDS = new ResourceDependencyMapDS();
            ResourceAttributesDS raDs = new ResourceAttributesDS();
            ResourcetypeObservableActionMapDS rtoamDS = new ResourcetypeObservableActionMapDS();
            ResourcetypeObservableRemediationPlanMapDS rtorPlanMapDS = new ResourcetypeObservableRemediationPlanMapDS();
            RemediationPlanDS rPlanDS = new RemediationPlanDS();
            ActionDS actionDS = new ActionDS();
            ObservableDS observable = new ObservableDS();
            try
            {
                var ObrandRemList = (from a in rtoamDS.GetAny().ToArray()
                                     join c in typeDS.GetAny().ToArray() on a.ResourceTypeId equals c.ResourceTypeId
                                     join d in observable.GetAny().ToArray() on a.ObservableId equals d.ObservableId
                                     join e in actionDS.GetAny().ToArray() on a.ActionId equals e.ActionId
                                     // join f in rPlanDS.GetAny().ToArray() on a.TenantId equals f.TenantId
                                     //join r in resDS.GetAny().ToArray() on c.ResourceTypeId equals r.ResourceTypeId
                                     where
                                           a.TenantId == c.TenantId &&
                                           a.TenantId == d.TenantId &&
                                           a.TenantId == e.TenantId &&
                                           (d.ValidityStart <= DateTime.Now && d.ValidityEnd > DateTime.Now) &&
                                            e.IsDeleted == false && a.TenantId == Convert.ToInt32(TenantId) &&
                                           c.ResourceTypeName.ToLower() == ResourceTypeName.ToLower()
                                     //&& r.PlatformId == PlatformId
                                     group new { a, c, d, e } by new { a.Name,
                                         c.ResourceTypeName,
                                         a.ResourceTypeId,
                                         a.ObservableId,
                                         d.ObservableName,
                                         a.ActionId,
                                         e.ActionName
                                     } into g
                                     from t in g
                                     select new
                                     {
                                         g.Key.Name,
                                         g.Key.ResourceTypeName,
                                         g.Key.ResourceTypeId,
                                         g.Key.ObservableId,
                                         g.Key.ObservableName,
                                         g.Key.ActionId,
                                         g.Key.ActionName,
                                         obsValidityStart = t.a.ValidityStart,
                                         obsValidityEnd = t.a.ValidityEnd,
                                         Remediations = (from rd in rtorPlanMapDS.GetAny().ToArray()
                                                         where rd.ResourceTypeId == t.a.ResourceTypeId && rd.ObservableId == t.a.ObservableId
                                                         && rd.TenantId == TenantId
                                                         select rd).FirstOrDefault() != null ? (from rd in rtorPlanMapDS.GetAny().ToArray()
                                                                                                where rd.ResourceTypeId == t.a.ResourceTypeId && rd.ObservableId == t.a.ObservableId
                                                                                                && rd.TenantId == TenantId
                                                                                                select rd).ToList() : null
                                     }).ToList();

                var resType_grp = (from O in ObrandRemList
                                   group O by new { O.ResourceTypeId, O.ResourceTypeName } into g
                                   select new
                                   {
                                       g.Key.ResourceTypeId,
                                       g.Key.ResourceTypeName
                                   }).ToList();
                BE.ObservablesandRemediationPlanDetails remediationDetails = new BE.ObservablesandRemediationPlanDetails();
                List<BE.ObrandRemDetails> OandR_Plans = new List<BE.ObrandRemDetails>();
                foreach (var resType in resType_grp)
                {
                    BE.ObrandRemDetails obrandRemBE = new BE.ObrandRemDetails();
                    obrandRemBE.resourcetypeid = Convert.ToString(resType.ResourceTypeId);
                    obrandRemBE.resourcetypename = resType.ResourceTypeName;

                    List<BE.ObrandRem> obrBEList = new List<BE.ObrandRem>();
                    foreach (var gp in ObrandRemList.Where(r => r.ResourceTypeId == resType.ResourceTypeId).ToList())
                    {
                        if (gp.Remediations != null)
                        {
                            foreach (var rd in gp.Remediations)
                            {
                                BE.ObrandRem obrBE = new BE.ObrandRem()
                                {
                                    name = gp.Name,
                                    ObservableId = Convert.ToString(gp.ObservableId),
                                    ObservableName = gp.ObservableName,
                                    ObservableActionId = Convert.ToString(gp.ActionId),
                                    ObservableActionName = gp.ActionName,
                                    RemediationPlanId = Convert.ToString(rd.RemediationPlanId),
                                    RemediationPlanName = rPlanDS.GetAny().Where(r => r.RemediationPlanId == rd.RemediationPlanId).FirstOrDefault().RemediationPlanName,
                                    isObsSelected = (gp.obsValidityStart != null && gp.obsValidityEnd != null && gp.obsValidityStart <= DateTime.Now && gp.obsValidityEnd > DateTime.Now) ? true : false,
                                    isRemSelected = (rd.ValidityStart != null && rd.ValidityEnd != null && rd.ValidityStart <= DateTime.Now && rd.ValidityEnd > DateTime.Now) ? true : false
                                };
                                obrBEList.Add(obrBE);
                            }
                        }
                        else
                        {
                            BE.ObrandRem obrBE = new BE.ObrandRem()
                            {
                                name = gp.Name,
                                ObservableId = Convert.ToString(gp.ObservableId),
                                ObservableName = gp.ObservableName,
                                ObservableActionId = Convert.ToString(gp.ActionId),
                                ObservableActionName = gp.ActionName,
                                RemediationPlanId = null,
                                RemediationPlanName = null,
                                isObsSelected = (gp.obsValidityStart != null && gp.obsValidityEnd != null && gp.obsValidityStart <= DateTime.Now && gp.obsValidityEnd > DateTime.Now) ? true : false,
                                isRemSelected = false
                            };
                            obrBEList.Add(obrBE);
                        }

                    }
                    obrandRemBE.observablesandremediations = obrBEList;
                    OandR_Plans.Add(obrandRemBE);
                }

                /* var ObservableandRemediationsList = (from a in rtoamDS.GetAny().ToArray()
                                                      join b in rtorPlanMapDS.GetAny().ToArray() on a.ResourceTypeId equals b.ResourceTypeId into rtorPlanMaps
                                                      from rtor in rtorPlanMaps.DefaultIfEmpty()
                                                      join c in typeDS.GetAny().ToArray() on a.ResourceTypeId equals c.ResourceTypeId
                                                      join d in observable.GetAny().ToArray() on a.ObservableId equals d.ObservableId
                                                      join e in actionDS.GetAny().ToArray() on a.ActionId equals e.ActionId
                                                      join f in rPlanDS.GetAny().ToArray() on a.TenantId equals f.TenantId
                                                     // join r in resDS.GetAny().ToArray() on c.ResourceTypeId equals r.ResourceTypeId
                                                      where
                                                            //a.TenantId == rtor.TenantId &&
                                                            // a.ObservableId == rtor.ObservableId &&
                                                            a.TenantId == c.TenantId &&
                                                            a.ObservableId == d.ObservableId &&
                                                            a.TenantId == d.TenantId &&
                                                            a.TenantId == e.TenantId &&
                                                            //rtor.TenantId == f.TenantId &&
                                                            (d.ValidityStart <= DateTime.Now && d.ValidityEnd > DateTime.Now) &&
                                                             //(f.ValidityStart <= DateTime.Now && f.ValidityEnd > DateTime.Now) &&
                                                             e.IsDeleted == false && a.TenantId == Convert.ToInt32(TenantId) &&
                                                            // c.PlatformId == Convert.ToInt32(PlatformId) && 
                                                            c.ResourceTypeName.ToLower() == ResourceTypeName.ToLower()
                                                           // && r.PlatformId == PlatformId
                                                      //&& rtor != null ? rtor.RemediationPlanId == f.RemediationPlanId :
                                                      select new
                                                      {
                                                          a.Name,
                                                          c.ResourceTypeName,
                                                          a.ResourceTypeId,
                                                          a.ObservableId,
                                                          d.ObservableName,
                                                          a.ActionId,
                                                          e.ActionName,
                                                          RemediationPlanId = rtor != null ? rtor.RemediationPlanId : 0,
                                                          // RemediationPlanName = rtor != null ? f.RemediationPlanName : null,
                                                          obsValidityStart = a.ValidityStart,
                                                          obsValidityEnd = a.ValidityEnd,
                                                          RemValidityStart = rtor != null ? rtor.ValidityStart : null,
                                                          RemValidityEnd = rtor != null ? rtor.ValidityEnd : null,
                                                      }).ToList().Distinct();
                 BE.ObservablesandRemediationPlanDetails remediationDetails = new BE.ObservablesandRemediationPlanDetails();

                 List<BE.ObrandRemDetails> OandR_Plans = new List<BE.ObrandRemDetails>();

                 OandR_Plans = (from O in ObservableandRemediationsList
                                group O by new { O.ResourceTypeId, O.ResourceTypeName } into g
                                select new BE.ObrandRemDetails
                                {
                                    resourcetypeid = Convert.ToString(g.Key.ResourceTypeId),
                                    resourcetypename = g.Key.ResourceTypeName,
                                    observablesandremediations = (from OR in g
                                                                  where OR.ResourceTypeId == g.Key.ResourceTypeId
                                                                  group OR by new { OR.ObservableId, OR.ActionId, OR.RemediationPlanId } into grpObr
                                                                  from gp in grpObr
                                                                  select new BE.ObrandRem
                                                                  {
                                                                      name = gp.Name,
                                                                      ObservableId = Convert.ToString(gp.ObservableId),
                                                                      ObservableName = gp.ObservableName,
                                                                      ObservableActionId = Convert.ToString(gp.ActionId),
                                                                      ObservableActionName = gp.ActionName,
                                                                      RemediationPlanId = Convert.ToString(gp.RemediationPlanId),
                                                                      // RemediationPlanName = gp.RemediationPlanName,
                                                                      isObsSelected = (gp.obsValidityStart != null && gp.obsValidityEnd != null && gp.obsValidityStart <= DateTime.Now && gp.obsValidityEnd > DateTime.Now) ? true : false,
                                                                      isRemSelected = (gp.RemValidityStart != null && gp.RemValidityEnd != null && gp.RemValidityStart <= DateTime.Now && gp.RemValidityEnd > DateTime.Now) ? true : false
                                                                  }).ToList()

                                }).ToList();*/
                remediationDetails.platformid = PlatformId;
                remediationDetails.tenantid = TenantId;
                remediationDetails.observablesandremediationplans = OandR_Plans;
                return remediationDetails;

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        private BE.ObservablesandRemediationPlanDetails GetResourceLevelObservablesandRemediationDetails(int PlatformId, int TenantId, string ResourceTypeName)
        {

            ResourceDS resDS = new ResourceDS();
            ResourceTypeDS typeDS = new ResourceTypeDS();

            PlatformsDS platformsDS = new PlatformsDS();
            ResourceDependencyMapDS rdmDS = new ResourceDependencyMapDS();
            ResourceAttributesDS raDs = new ResourceAttributesDS();
            ResourceObservableActionMapDS rtoamDS = new ResourceObservableActionMapDS();
            ResourceObservableRemediationPlanMapDS rtorPlanMapDS = new ResourceObservableRemediationPlanMapDS();
            RemediationPlanDS rPlanDS = new RemediationPlanDS();
            ActionDS actionDS = new ActionDS();
            ObservableDS observable = new ObservableDS();
            try
            {
                var ObrandRemList = (from a in rtoamDS.GetAny().ToArray()
                                    join r in resDS.GetAny().ToArray() on a.ResourceId equals r.ResourceId
                                     join c in typeDS.GetAny().ToArray() on r.ResourceTypeId equals c.ResourceTypeId
                                     join d in observable.GetAny().ToArray() on a.ObservableId equals d.ObservableId
                                     join e in actionDS.GetAny().ToArray() on a.ActionId equals e.ActionId
                                     // join f in rPlanDS.GetAny().ToArray() on a.TenantId equals f.TenantId
                                     //join r in resDS.GetAny().ToArray() on c.ResourceTypeId equals r.ResourceTypeId
                                     where
                                           a.TenantId == c.TenantId &&
                                           a.TenantId == d.TenantId &&
                                           a.TenantId == e.TenantId &&
                                           (d.ValidityStart <= DateTime.Now && d.ValidityEnd > DateTime.Now) &&
                                            e.IsDeleted == false && a.TenantId == Convert.ToInt32(TenantId) &&
                                           c.ResourceTypeName.ToLower() == ResourceTypeName.ToLower()
                                     //&& r.PlatformId == PlatformId
                                     group new { a,r, c, d, e } by new
                                     {
                                         a.Name,
                                         c.ResourceTypeName,
                                         //r.ResourceTypeId,
                                         r.ResourceId,
                                         a.ObservableId,
                                         d.ObservableName,
                                         a.ActionId,
                                         e.ActionName
                                     } into g
                                     from t in g
                                     select new
                                     {
                                         g.Key.Name,
                                         g.Key.ResourceTypeName,
                                         //g.Key.ResourceTypeId,
                                         g.Key.ObservableId,
                                         g.Key.ObservableName,
                                         g.Key.ActionId,
                                         g.Key.ActionName,
                                         g.Key.ResourceId,
                                         obsValidityStart = t.a.ValidityStart,
                                         obsValidityEnd = t.a.ValidityEnd,
                                         Remediations = (from rd in rtorPlanMapDS.GetAny().ToArray()
                                                         where rd.ResourceId == t.r.ResourceId && rd.ObservableId == t.a.ObservableId
                                                         && rd.TenantId == TenantId
                                                         select rd).FirstOrDefault() != null ? (from rd in rtorPlanMapDS.GetAny().ToArray()
                                                                                                where rd.ResourceId == t.r.ResourceId && rd.ObservableId == t.a.ObservableId
                                                                                                && rd.TenantId == TenantId
                                                                                                select rd).ToList() : null
                                     }).ToList();

                var resType_grp = (from O in ObrandRemList
                                   group O by new { O.ResourceId,
                                      // O.ResourceTypeId,
                                       O.ResourceTypeName } into g
                                   select new
                                   {
                                       //g.Key.ResourceTypeId,
                                       g.Key.ResourceTypeName,
                                       g.Key.ResourceId
                                   }).ToList();
                BE.ObservablesandRemediationPlanDetails remediationDetails = new BE.ObservablesandRemediationPlanDetails();
                List<BE.ObrandRemDetails> OandR_Plans = new List<BE.ObrandRemDetails>();
                foreach (var resType in resType_grp)
                {
                    BE.ObrandRemDetails obrandRemBE = new BE.ObrandRemDetails();
                    obrandRemBE.resourcetypeid = Convert.ToString(resType.ResourceId);
                    obrandRemBE.resourcetypename = resType.ResourceTypeName;

                    List<BE.ObrandRem> obrBEList = new List<BE.ObrandRem>();
                    foreach (var gp in ObrandRemList.Where(r => r.ResourceId == resType.ResourceId).ToList())
                    {
                        if (gp.Remediations != null)
                        {
                            foreach (var rd in gp.Remediations)
                            {
                                BE.ObrandRem obrBE = new BE.ObrandRem()
                                {
                                    name = gp.Name,
                                    ObservableId = Convert.ToString(gp.ObservableId),
                                    ObservableName = gp.ObservableName,
                                    ObservableActionId = Convert.ToString(gp.ActionId),
                                    ObservableActionName = gp.ActionName,
                                    RemediationPlanId = Convert.ToString(rd.RemediationPlanId),
                                    RemediationPlanName = rPlanDS.GetAny().Where(r => r.RemediationPlanId == rd.RemediationPlanId).FirstOrDefault().RemediationPlanName,
                                    isObsSelected = (gp.obsValidityStart != null && gp.obsValidityEnd != null && gp.obsValidityStart <= DateTime.Now && gp.obsValidityEnd > DateTime.Now) ? true : false,
                                    isRemSelected = (rd.ValidityStart != null && rd.ValidityEnd != null && rd.ValidityStart <= DateTime.Now && rd.ValidityEnd > DateTime.Now) ? true : false
                                };
                                obrBEList.Add(obrBE);
                            }
                        }
                        else
                        {
                            BE.ObrandRem obrBE = new BE.ObrandRem()
                            {
                                name = gp.Name,
                                ObservableId = Convert.ToString(gp.ObservableId),
                                ObservableName = gp.ObservableName,
                                ObservableActionId = Convert.ToString(gp.ActionId),
                                ObservableActionName = gp.ActionName,
                                RemediationPlanId = null,
                                RemediationPlanName = null,
                                isObsSelected = (gp.obsValidityStart != null && gp.obsValidityEnd != null && gp.obsValidityStart <= DateTime.Now && gp.obsValidityEnd > DateTime.Now) ? true : false,
                                isRemSelected = false
                            };
                            obrBEList.Add(obrBE);
                        }

                    }
                    obrandRemBE.observablesandremediations = obrBEList;
                    OandR_Plans.Add(obrandRemBE);
                }

                
                remediationDetails.platformid = PlatformId;
                remediationDetails.tenantid = TenantId;
                remediationDetails.observablesandremediationplans = OandR_Plans;
                return remediationDetails;

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public List<BE.PlatformDetails> GetPlatformDetails(int tenantId)
        {
            List<BE.PlatformDetails> platformDetails = new List<BE.PlatformDetails>();
            var platformsTable = new PlatformsDS().GetAll();
            var resourceTable = new ResourceDS().GetAny();
            var resourceTypeTable = new ResourceTypeDS().GetAny();

            platformDetails = (from p in platformsTable
                               join r in resourceTable
                               on p.PlatformId.ToString() equals r.ResourceId
                               join rt in resourceTypeTable
                               on r.ResourceTypeId equals rt.ResourceTypeId
                               where p.TenantId == r.TenantId
                               && p.TenantId == tenantId
                               select new BE.PlatformDetails
                               {
                                   PlatformId = p.PlatformId.ToString(),
                                   PlatformTypeName = p.PlatformType,
                                   PlatformInstanceName = r.ResourceName,
                                   ResourceTypeId = r.ResourceTypeId.ToString(),
                                   ResourceTypeName = rt.ResourceTypeName
                               }).ToList();

            return platformDetails;
        }

        public resource GenerationLevel0(BE.ResourceModelGenerationReqMsg inputMessage)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GenerationLevel0", "ResourceModelBuilder"), LogHandler.Layer.Business, null);
            try
            {
                //Inserting into Platform table
                PlatformsDS platformsDS = new PlatformsDS();
                ResourceDS resourceDS = new ResourceDS();

                platforms platformDetails = platformsDS.Insert(new platforms() { PlatformName = inputMessage.Platformname, PlatformType = inputMessage.Platformtype, ExecutionMode = "1", CreatedBy = "admin", CreateDate = DateTime.UtcNow, TenantId = inputMessage.Tenantid });

                if (platformDetails == null)
                {
                    //throw error
                }
                //Insert into Resource Table
                resource resourceDetails = new resource()
                {
                    ResourceId = platformDetails.PlatformId.ToString(),
                    ResourceName = inputMessage.Platformname,
                    ResourceTypeId = GetResourceType(inputMessage.Platformtype, System.Configuration.ConfigurationManager.AppSettings["Platform"]).ResourceTypeId,
                    Source = string.Empty,
                    ValidityStart = DateTime.UtcNow,
                    ValidityEnd = new DateTime(2099, 08, 22),
                    TenantId = inputMessage.Tenantid,
                    PlatformId = platformDetails.PlatformId,
                    VersionNumber = "1",
                    IsActive = false
                };

                resourceDetails = resourceDS.Insert(resourceDetails);
                //resourceDetails = ResourceEntry(inputMessage, platformDetails.PlatformId.ToString(),platformDetails.PlatformId);

                if (resourceDetails == null)
                {
                    //throw error
                }

                //Insert into Resource Attributes Table
                bool status = ResourceAttributesEntry(inputMessage, resourceDetails, null, null);

                if (status)
                {
                    //Inserting into dependency Table
                    var resourceDependencyDetails = ResourceDependencyEntry(inputMessage, resourceDetails);
                }
                else
                {
                    //throw error
                }

                //enter anomaly rules
                bool AnomalyEntryStatus = AnomalyRulesEntry(resourceDetails);

                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GenerationLevel0", "ResourceModelBuilder"), LogHandler.Layer.Business, null);

                return resourceDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private string GenerationLevel1(BE.ResourceModelGenerationReqMsg inputMessage, string platformId)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GenerationLevel1", "ResourceModelBuilder"), LogHandler.Layer.Business, null);
            try
            {
                ResourceDS resourceDS = new ResourceDS();

                int serverLevelIncrementor = 0;

                string resourceIdServerLevel = platformId + "_";
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GenerationLevel1 -- Control tower", "ResourceModelBuilder"), LogHandler.Layer.Business, null);

                var controlTowerList = System.Configuration.ConfigurationManager.AppSettings["ControlTower"].Split(',');
                foreach (var controlTower in controlTowerList)
                {
                    #region Control tower
                    //insert into resource table -- control tower
                    resourcetype resourceTypeDetails = GetResourceType(inputMessage.Platformtype, controlTower);

                    resource resourceDetails = new resource()
                    {
                        ResourceId = resourceIdServerLevel + ++serverLevelIncrementor,
                        ResourceName = inputMessage.HostName,
                        ResourceTypeId = resourceTypeDetails.ResourceTypeId,
                        Source = inputMessage.IPAddress,
                        ValidityStart = DateTime.UtcNow,
                        ValidityEnd = new DateTime(2099, 08, 22),
                        TenantId = inputMessage.Tenantid,
                        PlatformId = Convert.ToInt32(platformId),
                        VersionNumber = "1",
                        IsActive = false
                    };

                    resourceDetails = resourceDS.Insert(resourceDetails);

                    //insert into resource attribute table
                    bool status = ResourceAttributesEntry(inputMessage, resourceDetails, null, null);
                    if (status)
                    {
                        //Inserting into dependency Table
                        var resourceDependencyDetails = ResourceDependencyEntry(inputMessage, resourceDetails);
                    }
                    else
                    {
                        //throw error
                    }

                    //service detaiols entry
                    bool serviceStatus = RPAServiceEntry(inputMessage, resourceDetails, resourceTypeDetails, 0, "Services");

                    //enter anomaly rules
                    bool AnomalyEntryStatus = AnomalyRulesEntry(resourceDetails);

                    #endregion
                }

                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GenerationLevel1 -- Control tower", "ResourceModelBuilder"), LogHandler.Layer.Business, null);
                DataTable viewResult = ExecuteDBView(inputMessage);

                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GenerationLevel1 -- Bot Runner", "ResourceModelBuilder"), LogHandler.Layer.Business, null);
                //bot runner entry
                var botRunnerList = System.Configuration.ConfigurationManager.AppSettings["BotRunner"].Split(',');
                foreach (var botRunnerType in botRunnerList)
                {
                    serverLevelIncrementor = ServerLevelEntry(inputMessage, viewResult, resourceIdServerLevel, serverLevelIncrementor, platformId, botRunnerType);
                }

                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GenerationLevel1 -- Bot Runnerr", "ResourceModelBuilder"), LogHandler.Layer.Business, null);

                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GenerationLevel1 -- Bot creator", "ResourceModelBuilder"), LogHandler.Layer.Business, null);
                //bot creator entry
                var botCreatorList = System.Configuration.ConfigurationManager.AppSettings["BotCreator"].Split(',');
                foreach (var botCreatorType in botCreatorList)
                {
                    serverLevelIncrementor = ServerLevelEntry(inputMessage, viewResult, resourceIdServerLevel, serverLevelIncrementor, platformId, botCreatorType);
                }

                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GenerationLevel1 -- Bot creator", "ResourceModelBuilder"), LogHandler.Layer.Business, null);

                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GenerationLevel1 -- DB Server", "ResourceModelBuilder"), LogHandler.Layer.Business, null);

                bool dbStatus = DBEntry(inputMessage, resourceIdServerLevel, serverLevelIncrementor,Convert.ToInt32(platformId));

                //#region Database Details
                //var dbServerList = System.Configuration.ConfigurationManager.AppSettings["DBServer"].Split(',');
                //foreach (var dbServerType in dbServerList)
                //{
                    
                //    resourcetype dbResourceTypeDetails = GetResourceType(inputMessage.Platformtype, dbServerType);

                //    resource dbResourceDetails = new resource()
                //    {
                //        ResourceId = resourceIdServerLevel + ++serverLevelIncrementor,
                //        ResourceName = inputMessage.Database_HostName,
                //        ResourceTypeId = dbResourceTypeDetails.ResourceTypeId,
                //        Source = inputMessage.Database_IPaddress,
                //        ValidityStart = DateTime.UtcNow,
                //        ValidityEnd = new DateTime(2099, 08, 22),
                //        TenantId = inputMessage.Tenantid,
                //        PlatformId = Convert.ToInt32(platformId),
                //        VersionNumber = "1",
                //        IsActive = false
                //    };

                //    dbResourceDetails = resourceDS.Insert(dbResourceDetails);
                //    //resource resourceDetails = ResourceEntry(inputMessage, resourceIdLevel1,Convert.ToInt32(platformId));

                //    //insert into resource attribute table
                //    bool dbStatus = ResourceAttributesEntry(inputMessage, dbResourceDetails, null, null);
                //    if (dbStatus)
                //    {
                //        //Inserting into dependency Table
                //        var dsresourceDependencyDetails = ResourceDependencyEntry(inputMessage, dbResourceDetails);
                //    }
                //    else
                //    {
                //        //throw error
                //    }

                //    //service detaiols entry
                //    bool dbServiceStatus = RPAServiceEntry(inputMessage, dbResourceDetails, dbResourceTypeDetails, 0, "DBService");

                //    //enter anomaly rules
                //    bool AnomalyEntryStatusDB = AnomalyRulesEntry(dbResourceDetails);

                    
                //}
                //#endregion

                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GenerationLevel1 -- DB Server", "ResourceModelBuilder"), LogHandler.Layer.Business, null);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GenerationLevel1", "ResourceModelBuilder"), LogHandler.Layer.Business, null);
            return "SUCCESS";
        }

        public bool DBEntry(BE.ResourceModelGenerationReqMsg inputMessage,string resourceIdServerLevel,int serverLevelIncrementor,int platformId)
        {
            #region Database Details
            try
            {
                ResourceDS resourceDS = new ResourceDS();
                var dbServerList = System.Configuration.ConfigurationManager.AppSettings["DBServer"].Split(',');
                foreach (var dbServerType in dbServerList)
                {
                    resourcetype dbResourceTypeDetails = GetResourceType(inputMessage.Platformtype, dbServerType);

                    resource dbResourceDetails = new resource()
                    {
                        ResourceId = resourceIdServerLevel + ++serverLevelIncrementor,
                        ResourceName = inputMessage.Database_HostName,
                        ResourceTypeId = dbResourceTypeDetails.ResourceTypeId,
                        Source = inputMessage.Database_IPaddress,
                        ValidityStart = DateTime.UtcNow,
                        ValidityEnd = new DateTime(2099, 08, 22),
                        TenantId = inputMessage.Tenantid,
                        PlatformId = Convert.ToInt32(platformId),
                        VersionNumber = "1",
                        IsActive = false
                    };

                    dbResourceDetails = resourceDS.Insert(dbResourceDetails);
                    //resource resourceDetails = ResourceEntry(inputMessage, resourceIdLevel1,Convert.ToInt32(platformId));

                    //insert into resource attribute table
                    bool dbStatus = ResourceAttributesEntry(inputMessage, dbResourceDetails, null, null);
                    if (dbStatus)
                    {
                        //Inserting into dependency Table
                        var dsresourceDependencyDetails = ResourceDependencyEntry(inputMessage, dbResourceDetails);
                    }
                    else
                    {
                        //throw error
                    }

                    //service detaiols entry
                    bool dbServiceStatus = RPAServiceEntry(inputMessage, dbResourceDetails, dbResourceTypeDetails, 0, "DBService");

                    //enter anomaly rules
                    bool AnomalyEntryStatusDB = AnomalyRulesEntry(dbResourceDetails);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
            #endregion
        }
        private int ServerLevelEntry(BE.ResourceModelGenerationReqMsg inputMessage, DataTable viewResult, string resourceIdServerLevel, int serverLevelIncementor, string platformId, string resourceTypeName)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "ServerLevelEntry", "ResourceModelBuilder"), LogHandler.Layer.Business, null);
            try
            {
                #region Bot Runner / Creator
                var BotRunner = from row in viewResult.AsEnumerable() where row.Field<string>("resourcetypename") == resourceTypeName select row;

                if (BotRunner != null)
                {
                    foreach (DataRow row in BotRunner)
                    {
                        string botRunnerResourceId = resourceIdServerLevel + ++serverLevelIncementor;
                        resource BotRunnerResourceDetails = new resource()
                        {
                            ResourceId = botRunnerResourceId,
                            ResourceName = row["resourcename"].ToString(),
                            Source = row["source"].ToString(),
                            TenantId = inputMessage.Tenantid,
                            PlatformId = Convert.ToInt32(platformId)
                        };

                        resourcetype botRunnerResourceTypeDetails = new resourcetype()
                        {
                            PlatfromType = inputMessage.Platformtype,
                            ResourceTypeName = row["resourcetypename"].ToString()
                        };

                        BotRunnerResourceDetails = ResourceEntry(BotRunnerResourceDetails, botRunnerResourceTypeDetails);

                        bool botRunnerStatus = ResourceAttributesEntry(inputMessage, BotRunnerResourceDetails, row, null);

                        resource_dependency_map botRunnerResourceDependencyDetails = ResourceDependencyEntry(inputMessage, BotRunnerResourceDetails);

                        //enter anomaly rules
                        bool AnomalyEntryStatus = AnomalyRulesEntry(BotRunnerResourceDetails);

                        #region BOT
                        //fetching all the bots for this bot runner
                        var botList = from botR in viewResult.AsEnumerable()
                                      where botR.Field<string>("resourcetypename") == System.Configuration.ConfigurationManager.AppSettings["Bot"]
                                      && botR.Field<string>("hostname") == row.Field<string>("hostname")
                                      && botR.Field<string>("ipaddress") == row.Field<string>("ipaddress")
                                      select botR;
                        int serviceLevelIncrementor = 0;
                        foreach (DataRow botRow in botList)
                        {
                            //construct resource id
                            string botResourceId = BotRunnerResourceDetails.ResourceId + "_" + ++serviceLevelIncrementor;

                            //enter into resource table
                            resource BotResourceDetails = new resource()
                            {
                                ResourceId = botResourceId,
                                ResourceName = botRow["resourcename"].ToString(),
                                Source = botRow["source"].ToString(),
                                TenantId = inputMessage.Tenantid,
                                PlatformId = Convert.ToInt32(platformId)
                            };

                            resourcetype botResourceTypeDetails = new resourcetype()
                            {
                                PlatfromType = inputMessage.Platformtype,
                                ResourceTypeName = botRow["resourcetypename"].ToString()
                            };

                            BotResourceDetails = ResourceEntry(BotResourceDetails, botResourceTypeDetails);

                            bool botStatus = ResourceAttributesEntry(inputMessage, BotResourceDetails, botRow, null);

                            resource_dependency_map botResourceDependencyDetails = ResourceDependencyEntry(inputMessage, BotResourceDetails);

                            //enter anomaly rules
                            bool AnomalyEntryStatusBot = AnomalyRulesEntry(BotResourceDetails);

                        }
                        #endregion
                        #region service
                        bool botRunnerServiceStatus = RPAServiceEntry(inputMessage, BotRunnerResourceDetails, botRunnerResourceTypeDetails, serviceLevelIncrementor, "Services");
                        #endregion

                    }
                }


                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "ServerLevelEntry", "ResourceModelBuilder"), LogHandler.Layer.Business, null);
                return serverLevelIncementor;
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private resource ResourceEntry(resource resourceDetails, resourcetype resourceTypeDetails)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "ResourceEntry", "ResourceModelBuilder"), LogHandler.Layer.Business, null);
            try
            {
                ResourceDS resourceDS = new ResourceDS();
                ResourceTypeDSEntended resourceTypeDS = new ResourceTypeDSEntended();

                //getting resourceType

                int resourceTypeId = resourceTypeDS.GetOne(resourceTypeDetails).ResourceTypeId;

                resourceDetails.ResourceTypeId = resourceTypeId;
                resourceDetails.ValidityStart = DateTime.UtcNow;
                resourceDetails.ValidityEnd = new DateTime(2099, 08, 22);
                resourceDetails.VersionNumber = "1";
                resourceDetails.IsActive = false;

                resourceDetails = resourceDS.Insert(resourceDetails);

                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "ResourceEntry", "ResourceModelBuilder"), LogHandler.Layer.Business, null);

                return resourceDetails;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private bool ResourceAttributesEntry(BE.ResourceModelGenerationReqMsg inputMessage, resource resourceDetails, DataRow viewRow, resourcetype_service_details rpaServiceObj)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "ResourceAttributesEntry", "ResourceModelBuilder"), LogHandler.Layer.Business, null);

            try
            {
                ResourceTypeMetadataDS resourceTypeMetadataDS = new ResourceTypeMetadataDS();
                ResourceAttributesDS resourceAttributesDS = new ResourceAttributesDS();

                //getting the resource attribute data that are to be added
                List<resourcetype_metadata> resourcetype_Metadata_List = resourceTypeMetadataDS.GetAll(new resourcetype_metadata() { ResourceTypeId = resourceDetails.ResourceTypeId }).ToList();

                List<resource_attributes> resource_attributeList = new List<resource_attributes>();
                //populating the resource attribute objects to insert into db
                foreach (var resTypeMetaDataObj in resourcetype_Metadata_List)
                {
                    resource_attributes resAttObj = new resource_attributes();
                    resAttObj.ResourceId = resourceDetails.ResourceId;
                    resAttObj.AttributeName = resTypeMetaDataObj.AttributeName;

                    resAttObj.IsSecret = resTypeMetaDataObj.issecret;
                    



                    if (viewRow != null)
                    {
                        if (viewRow.Table.Columns.Contains(resTypeMetaDataObj.AttributeName))
                        {
                            resAttObj.AttributeValue = viewRow[resTypeMetaDataObj.AttributeName].ToString();
                        }
                        else
                        {
                            switch (resTypeMetaDataObj.AttributeName)
                            {
                                case "RemoteServerNames":
                                    resAttObj.AttributeValue = viewRow["hostname"].ToString();
                                    break;
                                case "RemoteUserName":
                                    resAttObj.AttributeValue = inputMessage.Service_UserName;
                                    break;
                                case "RemotePassword":
                                    resAttObj.AttributeValue = Encrypt(inputMessage.Service_Password);
                                    break;
                                default:
                                    resAttObj.AttributeValue = resTypeMetaDataObj.DefaultValue != null ? resTypeMetaDataObj.DefaultValue : String.Empty;
                                    break;
                            }
                        }
                        //resAttObj.AttributeValue = viewRow[resTypeMetaDataObj.AttributeName]!=null?viewRow[resTypeMetaDataObj.AttributeName].ToString():string.Empty;
                    }
                    else if (rpaServiceObj != null)
                    {
                        switch (resTypeMetaDataObj.AttributeName.ToLower())
                        {
                            case "servicename":
                                resAttObj.AttributeValue = rpaServiceObj.ServiceName;
                                break;
                            default:
                                resAttObj.AttributeValue = resTypeMetaDataObj.DefaultValue != null ? resTypeMetaDataObj.DefaultValue : String.Empty;
                                break;
                        }
                    }
                    else
                    {
                        switch (resTypeMetaDataObj.AttributeName)
                        {
                            case "hostname":
                                if (resourceDetails.ResourceTypeId == 3 || resourceDetails.ResourceTypeId == 24)
                                    resAttObj.AttributeValue = inputMessage.Database_HostName;
                                else
                                    resAttObj.AttributeValue = inputMessage.HostName;
                                break;
                            case "ipaddress":
                                if (resourceDetails.ResourceTypeId == 3 || resourceDetails.ResourceTypeId == 24)
                                    resAttObj.AttributeValue = inputMessage.Database_IPaddress;
                                else
                                    resAttObj.AttributeValue = inputMessage.IPAddress;
                                break;
                            case "username":
                                resAttObj.AttributeValue = inputMessage.Service_UserName;
                                break;
                            case "password":
                                resAttObj.AttributeValue = inputMessage.Service_Password;
                                break;
                            case "controlRoomBaseURI":
                                resAttObj.AttributeValue = inputMessage.API_URL;
                                break;
                            case "controlRoomPassword":
                                resAttObj.AttributeValue = inputMessage.API_Password;
                                break;
                            case "controlRoomUsername":
                                resAttObj.AttributeValue = inputMessage.API_UserName;
                                break;
                            case "ip":
                                if (resourceDetails.ResourceTypeId == 3 || resourceDetails.ResourceTypeId == 24)
                                    resAttObj.AttributeValue = inputMessage.Database_IPaddress;
                                else
                                    resAttObj.AttributeValue = inputMessage.HostName.Split('.')[0];
                                break;
                            case "RemoteServerNames":
                                if (resourceDetails.ResourceTypeId == 3 || resourceDetails.ResourceTypeId == 24)
                                    resAttObj.AttributeValue = inputMessage.Database_HostName;
                                else
                                    resAttObj.AttributeValue = inputMessage.HostName;
                                break;
                            case "RemoteUserName":
                                resAttObj.AttributeValue = inputMessage.Service_UserName;
                                break;
                            case "RemotePassword":
                                resAttObj.AttributeValue = Encrypt(inputMessage.Service_Password);
                                break;
                            case "input":
                                resAttObj.AttributeValue = inputMessage.Database_HostName;
                                break;
                            case "dbusername":
                                resAttObj.AttributeValue = inputMessage.Database_UserName;
                                break;
                            case "dbpassword":
                                resAttObj.AttributeValue = inputMessage.Database_Password;
                                break;
                            case "EndpointUri":
                                resAttObj.AttributeValue = resTypeMetaDataObj.DefaultValue.Replace("<host_name>", inputMessage.Database_HostName).Replace("<db_name>", inputMessage.Database_Name);
                                break;
                            default:
                                resAttObj.AttributeValue = resTypeMetaDataObj.DefaultValue != null ? resTypeMetaDataObj.DefaultValue : String.Empty;
                                break;
                        }
                    }

                    resAttObj.TenantId = inputMessage.Tenantid;
                    resAttObj.DisplayName = resTypeMetaDataObj.DisplayName;
                    resAttObj.Description = resTypeMetaDataObj.Description;
                    resAttObj.CreatedBy = "admin";
                    resAttObj.CreateDate = DateTime.UtcNow;
                    if (resAttObj.IsSecret == true && resAttObj.AttributeValue != null && resAttObj.AttributeName != "RemotePassword" )
                    {
                        resAttObj.AttributeValue = Encrypt(resAttObj.AttributeValue);
                    }

                    resource_attributeList.Add(resAttObj);
                }

                //Inserting into resource attributes tables
                var result = resourceAttributesDS.InsertBatch(resource_attributeList);

                if (result == null || result.Count == 0)
                {
                    return false;
                }

                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "ResourceAttributesEntry", "ResourceModelBuilder"), LogHandler.Layer.Business, null);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private resource_dependency_map ResourceDependencyEntry(BE.ResourceModelGenerationReqMsg inputMessage, resource resourceDetails)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "ResourceDependencyEntry", "ResourceModelBuilder"), LogHandler.Layer.Business, null);

            ResourceTypeDependencyMapDS resourceTypeDependencyMapDS = new ResourceTypeDependencyMapDS();
            ResourceDependencyMapDS resourceDependencyMapDS = new ResourceDependencyMapDS();

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
                resource_Dependency_Map.TenantId = inputMessage.Tenantid;

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
                    //resource_Dependency_Map.PortfolioId = resourcetype_Dependency_Map[0].PortfolioId!=null? resourcetype_Dependency_Map[0].PortfolioId:"";
                    resource_Dependency_Map.PortfolioId = resourcetype_Dependency_Map[0]?.PortfolioId ?? "";
                    //getting the resource for the resource type
                    if (resourcetype_Dependency_Map[0].DependencyResourceTypeId == "" || resourcetype_Dependency_Map[0].DependencyResourceTypeId == null)
                    {
                        resource_Dependency_Map.DependencyResourceId = string.Empty ;
                    }
                    else
                    {
                        List<resource> dependencyResourceList = new ResourceDSExtended().GetAll(new resource() { ResourceTypeId = Convert.ToInt32(resourcetype_Dependency_Map[0].DependencyResourceTypeId), PlatformId = resourceDetails.PlatformId });

                        if (dependencyResourceList != null && dependencyResourceList.Count == 1)
                        {
                            resource_Dependency_Map.DependencyResourceId = dependencyResourceList[0].ResourceId;
                        }
                        else
                        {
                            // has multiple dependency resources. therefore, using parent resource as dependency resource

                            resource_Dependency_Map.DependencyResourceId = ExtractParentResource(resourceDetails.ResourceId);
                        }
                    }                    

                }
                else
                {
                    resource_Dependency_Map.DependencyType = resourcetype_Dependency_Map[0].DependencyType;
                    resource_Dependency_Map.PortfolioId = resourcetype_Dependency_Map[0]?.PortfolioId??"";
                    resource_Dependency_Map.DependencyResourceId = ExtractParentResource(resourceDetails.ResourceId);
                }

                resource_Dependency_Map = resourceDependencyMapDS.Insert(resource_Dependency_Map);

                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "ResourceDependencyEntry", "ResourceModelBuilder"), LogHandler.Layer.Business, null);

                return resource_Dependency_Map;
            }
            catch (Exception ex)
            {
                LogHandler.LogError(string.Format($"Exception occured in resource dependency entry method. THe message is :{ex.Message}"), LogHandler.Layer.Business, null);
                throw ex;
            }



        }

        private bool RPAServiceEntry(BE.ResourceModelGenerationReqMsg inputMessage, resource resourceDetails, resourcetype resourceTypeDetails, int serviceLevelIncrementor, String serviceName)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "RPAServiceEntry", "ResourceModelBuilder"), LogHandler.Layer.Business, null);
            ResourceTypeServiceDetailsDS resourceTypeServiceDetailsDS = new ResourceTypeServiceDetailsDS();

            try
            {
                //service details
                List<resourcetype_service_details> rpaServiceList = resourceTypeServiceDetailsDS.GetAll(new resourcetype_service_details() { ResourceTypeId = resourceDetails.ResourceTypeId }).ToList();
                string serviceLevelResourceId = resourceDetails.ResourceId + "_";

                foreach (resourcetype_service_details rpaService in rpaServiceList)
                {
                    string resourceId = serviceLevelResourceId + ++serviceLevelIncrementor;

                    //inserting into resource table
                    resource serviceResourceDetails = new resource()
                    {
                        ResourceId = resourceId,
                        ResourceName = rpaService.ServiceName,
                        Source = resourceDetails.Source,
                        TenantId = inputMessage.Tenantid,
                        PlatformId = resourceDetails.PlatformId
                    };

                    resourcetype serviceResourcetypeDetails = new resourcetype()
                    {
                        PlatfromType = inputMessage.Platformtype,
                        ResourceTypeName = serviceName //resource type name  for services
                    };

                    serviceResourceDetails = ResourceEntry(serviceResourceDetails, serviceResourcetypeDetails);

                    bool serviceStatus = ResourceAttributesEntry(inputMessage, serviceResourceDetails, null, rpaService);

                    resource_dependency_map ServiceResourceDependencyDetails = ResourceDependencyEntry(inputMessage, serviceResourceDetails);

                    //enter anomaly rules
                    bool AnomalyEntryStatusBot = AnomalyRulesEntry(serviceResourceDetails);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "RPAServiceEntry", "ResourceModelBuilder"), LogHandler.Layer.Business, null);
            return true;


        }

        private bool AnomalyRulesEntry(resource resourceDetails)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "AnomalyRulesEntry", "ResourceModelBuilder"), LogHandler.Layer.Business, null);

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

                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "AnomalyRulesEntry", "ResourceModelBuilder"), LogHandler.Layer.Business, null);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public BE.ResourceModelGenerationResMsg GenerateResourceModel(BE.ResourceModelGenerationReqMsg inputMessage)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GenerateResourceModel", "ResourceModelBuilder"), LogHandler.Layer.Business, null);
            resource resourceDetails = GenerationLevel0(inputMessage);
            string resourceIdLevel1 = GenerationLevel1(inputMessage, resourceDetails.ResourceId);
            string resourceTypeName = new ResourceTypeDS().GetOne(new resourcetype() { ResourceTypeId = resourceDetails.ResourceTypeId }).ResourceTypeName;

            BE.ResourceModelGenerationResMsg response = new BE.ResourceModelGenerationResMsg()
            {
                Tenantid = resourceDetails.TenantId,
                PlatformId = Convert.ToInt32(resourceDetails.PlatformId),
                ResourceTypeName = resourceTypeName
            };
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GenerateResourceModel", "ResourceModelBuilder"), LogHandler.Layer.Business, null);

            return response;
        }

        public resourcetype GetResourceType(string platformType, string resourceTypeName)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GetResourceType", "ResourceModelBuilder"), LogHandler.Layer.Business, null);
            return new ResourceTypeDSEntended().GetOne(new resourcetype() { PlatfromType = platformType, ResourceTypeName = resourceTypeName });
        }

        public string ExtractParentResource(string resourceId)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "ExtractParentResource", "ResourceModelBuilder"), LogHandler.Layer.Business, null);
            var stringArr = resourceId.Split('_');
            StringBuilder parentResourceId = new StringBuilder(stringArr[0]);
            for (int i = 1; i < stringArr.Length - 1; i++)
            {
                parentResourceId.Append("_" + stringArr[i]);
            }
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "ExtractParentResource", "ResourceModelBuilder"), LogHandler.Layer.Business, null);
            return parentResourceId.ToString();
        }

        public DataTable ExecuteDBView(BE.ResourceModelGenerationReqMsg inputMessage)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "ExecuteDBView", "Monitor"), LogHandler.Layer.Business, null);
            // Table to store the query results
            DataTable table = new DataTable();
            try
            {
                // LogHandler.LogDebug(String.Format("Executing ExecuteDBView with input arguments DBServer:{0},DBName:{1},ViewName:{2}", dbserver, dbname, viewname), LogHandler.Layer.Business, null);
                //using (LogHandler.TraceOperations("Monitor:ExecuteDBView", LogHandler.Layer.Business, Guid.NewGuid(), null))
                //{
                //string dbUserid = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DBUserID"]);
                //string dbPassword = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DBPassword"]);
                string conn = String.Format(System.Configuration.ConfigurationManager.ConnectionStrings["RPAdb"].ConnectionString, inputMessage.Database_Name, inputMessage.Database_HostName, inputMessage.Database_UserName, inputMessage.Database_Password);
                //"Data Source=" + dbserver + ";Initial Catalog=" + dbname + ";User Id=" + dbUserid + ";Password=" + dbPassword;
                string cmd = "select * from " + System.Configuration.ConfigurationManager.AppSettings["ViewName"];

                LogHandler.LogDebug(String.Format("Creation of DB connection for connection:{0}", conn), LogHandler.Layer.Business, null);
                // Creates a SQL connection
                using (var connection = new SqlConnection(conn))
                {
                    connection.Open();
                    LogHandler.LogDebug(String.Format("DB connection opened for connection:{0}", conn), LogHandler.Layer.Business, null);

                    // Creates a SQL command
                    using (var command = new SqlCommand(cmd, connection))
                    {
                        // Loads the query results into the table
                        table.Load(command.ExecuteReader());
                        LogHandler.LogDebug(String.Format("Executed query:{0} and no.of records fetched:{1}", cmd, table.Rows.Count), LogHandler.Layer.Business, null);
                    }

                    connection.Close();
                    LogHandler.LogDebug(String.Format("DB connection closed for connection:{0}", conn), LogHandler.Layer.Business, null);
                }
                // }
                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "ExecuteDBView", "Monitor"), LogHandler.Layer.Business, null);
            }
            catch (Exception ex)
            {
                //LogHandler.LogError(string.Format(ErrorMessages.ViewExecutionUnsuccessful, viewname, ex.Message), LogHandler.Layer.Business, null);
                /// Console.WriteLine("Error occured while executing view : " + viewname + " Error :" + ex.Message);
                throw ex;
            }
            return table;
        }

        private void updateCascadetoChild(string resourceId, int platformId, int tenantId, bool isActive)
        {
            ResourceDS resDS = new ResourceDS();
            ResourceDependencyMapDS rdmDS = new ResourceDependencyMapDS();
            ResourceDependencyMapDSExtn rdmDSExtn = new ResourceDependencyMapDSExtn();

            var resourcehierarchy = rdmDSExtn.HierarchyResources(resourceId, Convert.ToString(tenantId));
            if (resourcehierarchy != null)
            {
                var Ids = resourcehierarchy.Select(r => r.Resourceid).ToList();

                //Update active flag for resource table
                List<resource> resourcesList = resDS.GetAny().Where(r => Ids.Contains(r.ResourceId)).ToList();
                foreach (resource r in resourcesList)
                {
                    r.IsActive = isActive;
                }
                var res = resDS.UpdateBatch(resourcesList);
            }

        }

        public bool UpdateCategoryandScriptId(BE.CategoryAndScriptDetails obj)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "UpdateCategoryandScriptId", "ResourceModelBuilder"), LogHandler.Layer.Business, null);
            FacadeClient facadeClient = new FacadeClient();
            ActionDS actionDS = new ActionDS();
            //bool status = false;
            try
            {
                // getting the category list
                
                string categoryList = facadeClient.GetCategoryList(obj.SEEBaseUrl, obj.CategoryServiceName);
                if (categoryList == null || categoryList == "")
                {
                    return false;
                }

                BE.CategoriesList categoryListObj = JsonConvert.DeserializeObject<BE.CategoriesList>(categoryList);

                var actionTable = (from a in actionDS.GetAll() where a.TenantId == Convert.ToInt32(obj.TenantId) select a).ToList();

                //getting the action categories
                var actionList = (from a in actionTable
                                  group a by new { a.CategoryId, a.CategoryName } into grp
                                  select grp).ToList();
                int categoryId = 0;

                foreach (var group in actionList)
                {
                    if (group.Key.CategoryName != null && group.Key.CategoryName != "")
                    {
                        //finding the category Id from the categoryList
                        categoryId = GetCategoryId(categoryListObj, group.Key.CategoryName);
                        //update category Id
                        List<action> objList = (from a in actionTable
                                                where a.CategoryName == @group.Key.CategoryName
                                                select a).ToList();
                        objList.ForEach(o => o.CategoryId = categoryId);

                        if (actionDS.UpdateBatch(objList) == null)
                            return false;
                    }
                }


                //get the updated action table data
                List<action> updatedActionData = (from a in actionDS.GetAll()
                                                  where a.TenantId == Convert.ToInt32(obj.TenantId) && a.CategoryName != null && a.ActionName != "" select a).ToList();

                foreach (action action in updatedActionData)
                {
                    action.ScriptId = GetScriptId(Convert.ToInt32(action.CategoryId), action.ActionName, obj);
                }

                LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "UpdateCategoryandScriptId", "ResourceModelBuilder"), LogHandler.Layer.Business, null);

                if (actionDS.UpdateBatch(updatedActionData) != null)
                {
                    return true;
                }
                else
                    return false;


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int GetScriptId(int categoryId, string scriptName, BE.CategoryAndScriptDetails obj)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GetScriptId", "ResourceModelBuilder"), LogHandler.Layer.Business, null);

            int scriptId = 0;
            //var scriptList = ServiceCaller("http://" + obj.SEEBaseUrl + "/" + obj.ScriptDetailServiceName + categoryId);
            string scriptList = new FacadeClient().GetCategoryList(obj.SEEBaseUrl, obj.ScriptDetailServiceName + categoryId);
            if (scriptList == null || scriptList == "")
            {
                return 0;
            }

            BE.ScriptList scriptListobj = JsonConvert.DeserializeObject<BE.ScriptList>(scriptList);
            scriptId = (from s in scriptListobj.Scripts
                        where s.Name == scriptName
                        select s.ScriptId).FirstOrDefault();

            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "GetScriptId", "ResourceModelBuilder"), LogHandler.Layer.Business, null);

            return scriptId;
        }

        private int GetCategoryId(BE.CategoriesList categoryListObj, string categoryName)
        {
            int parentCategoryId = 0;
            string[] categorynameList = categoryName.Split('.');
            foreach (string catName in categorynameList)
            {
                parentCategoryId = (from c in categoryListObj.Categories
                                    where c.Name.Replace(" ", "") == catName.Replace(" ", "")
                                    && c.ParentId == parentCategoryId
                                    select c.CategoryId).FirstOrDefault();
            }
            return parentCategoryId;
        }
        private string ServiceCaller(string url)
        {
            using (HttpClient client = new HttpClient())
            using (var response = client.GetAsync(url).Result)
            {
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    if (data != null)
                    {
                        return data;
                    }
                    return "";
                    //using (HttpContent content = response.Content)
                    //{
                    //    string data = content.ReadAsStringAsync();
                    //    if (data != null)
                    //    {
                    //        return data;
                    //    }
                    //}
                    //return "";
                }
                return "";
            }
        }

        public List<BE.RPAType> GetDynamicAutomationTypes()
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GetDynamicAutomationTypes", "ResourceModelBuilder"), LogHandler.Layer.Business, null);
            try
            {
                using (StreamReader r = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + @"XML\AutomationTypes.json"))
                {
                    string json = r.ReadToEnd();                    
                    List<BE.RPAType> items = JsonConvert.DeserializeObject<List<BE.RPAType>>(json);
                    return items;
                }               
            }
            catch (Exception ex)
            {
                LogHandler.LogError(string.Format("Exeception Occured in {0}. Error Message {1}", "GetDynamicAutomationTypes", ex.Message), LogHandler.Layer.Business, null);
                throw ex;
            }
            
        }

        public BE.GetSEEScriptDetailsResMsg GetSEEScriptDetails(BE.GetSEEScriptDetailsReqMsg obj)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GetSEEScriptDetails", "ResourceModelBuilder"), LogHandler.Layer.Business, null);
            BE.GetSEEScriptDetailsResMsg response = new BE.GetSEEScriptDetailsResMsg();
            try
            {
                FacadeClient client = new FacadeClient();
                var res = client.GetScriptList(obj.SEEBaseUrl, obj.ServiceName + obj.CategoryId);
                if (res == null || res == "")
                {
                    return null;
                }
                response = JsonConvert.DeserializeObject<BE.GetSEEScriptDetailsResMsg>(res);
                
                response.CategoryId = obj.CategoryId;
                response.Name = obj.Name;
                
            }
            catch (Exception ex)
            {
                LogHandler.LogError(string.Format("Exeception Occured in {0}. Error Message {1}", "GetSEEScriptDetails", ex.Message), LogHandler.Layer.Business, null);
                throw ex;
            }
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "UpdateCategoryandScriptId", "ResourceModelBuilder"), LogHandler.Layer.Business, null);
            return response;
        }

        public List<BE.Property> GetSEECategories(BE.GetSEECategoriesReqMsg obj)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GetSEECategories", "ResourceModelBuilder"), LogHandler.Layer.Business, null);
            BE.GetSEECategoriesResMsg response = new BE.GetSEECategoriesResMsg();
            try
            {
                FacadeClient client = new FacadeClient();
                var res = client.GetCategoryList(obj.SEEBaseUrl, obj.ServiceName);
                if (res == null || res == "")
                {
                    return null;
                }
                response = JsonConvert.DeserializeObject<BE.GetSEECategoriesResMsg>(res);
                
                if (response != null && response.Categories != null)
                {
                    categories = response.Categories;
                    LoadCategory();
                }

                //string response1 = JsonConvert.SerializeObject(SEECategories);

            }
            catch (Exception ex)
            {
                LogHandler.LogError(string.Format("Exeception Occured in {0}. Error Message {1}", "GetSEECategories", ex.Message), LogHandler.Layer.Business, null);
                throw ex;
            }
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "GetSEECategories", "ResourceModelBuilder"), LogHandler.Layer.Business, null);
            string removeSEEcategories= System.Configuration.ConfigurationManager.AppSettings["RemoveSEECategories"];
            if (removeSEEcategories!=null)
            {
                List<BE.Property> newSEECategories = new List<BE.Property>();
                foreach (BE.Property c in SEECategories){
                    if (!removeSEEcategories.Contains(c.name))
                    {
                        newSEECategories.Add(c);
                    }

                }
                return newSEECategories;
            }
            return SEECategories;
        }

        #region test Code


        public void LoadCategory()
        {
            try
            {
                if (categories != null)
                {
                    for (int i = 0; i < categories.Count; i++)
                    {
                        var c = categories[i];
                        if (c.CompanyId == 0 && c.ParentId == 0)
                        {

                            var subCat = categories.Where(sc => sc.CompanyId == 0 && sc.ParentId == Convert.ToInt32(c.CategoryId));
                            if (subCat == null || subCat.Count() == 0)
                            {
                                categories.Remove(c);
                            }
                        }
                    }

                    // RemoveEmptyCategories();

                    PopulateCategories();
                    
                }
                else
                {
                    string msg = "No Category Exists";
                }

            }
            catch (Exception ex)
            {
                string err = ex.Message;

            }

        }
        private void PopulateCategories()
        { 
            categories.Where(g => g.ParentId == 0).ToList().
                ForEach(sg =>
                {
                    AddNode(sg, null);
                });           

        }

        private void AddNode(BE.TestCategory c,BE.Property parent)
        {
            
            if (parent == null)
            {
                parent = new BE.Property() {
                    id=c.CategoryId,
                    name= c.Name,
                    path=c.Name,
                    children = new List<BE.Property>()
                };                    
                SEECategories.Add(parent);                
            }
            else
            {
                BE.Property child = new BE.Property()
                {
                    id = c.CategoryId,
                    name = c.Name,
                    path=parent.path+"/"+c.Name,
                    children = new List<BE.Property>()
                };
                parent.children.Add(child);               
                parent = child;               
            }

            categories.Where(sc => sc.ParentId == Convert.ToInt32(c.CategoryId)).ToList().ForEach(child =>
            {
                AddNode(child, parent);
            });

        }
        #endregion

        public BE.AABotList GetAABotList()
        {
            try
            {
                BE.AABotList botList = new BE.AABotList();
                FacadeClient fc = new FacadeClient();
                var result = fc.getActivityList();
                if(result!=null)
                {
                    botList = JsonConvert.DeserializeObject<BE.AABotList>(result);
                }

                return botList;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static string Encrypt(string text)
        {
            try
            {
                string baseURL = System.Configuration.ConfigurationManager.AppSettings["SuperbotBaseUrl"];
                string endPoint = baseURL.Substring(baseURL.IndexOf('/') + 1);
                string baseURI = baseURL.Split('/')[0];
                FacadeClient client = new FacadeClient();
                string response = client.EncryptText(baseURI, "/" + endPoint + "/SecureHandler/EncryptData?textToSecure=" + text + "&passCode=IAP2GO_SEC!URE");
                // sample response : "{"response":"Encrypted String"}"
                response = JsonConvert.DeserializeObject<dynamic>(response)?.response;
                return response;
            }
            catch(Exception ex)
            {
                LogHandler.LogError("Exception thrown in Encrypt Method of ResourceModelBuilder class. The Exception : {0}",LogHandler.Layer.Business,ex.Message);                
            }
            return null;
            //return client.DecryptText("localhost", "superbot/api/SecureHandler/DecryptText?secureText=uAjkvXNdHZ1rzdtrSC8h5DYAMwA3ADIAOAA3ADcAMQA3ADUAMgA1ADIAMQAyADMANQA5AA==aWFwMTYy&passCode=IAP2GO_SEC!URE");
        }
        public static string Decrypt(string text)
        {
            try
            {
                //text = text.Replace(@"+", @"%2B");
                string baseURL = System.Configuration.ConfigurationManager.AppSettings["SuperbotBaseUrl"];
                string endPoint = baseURL.Substring(baseURL.IndexOf('/') + 1);
                string baseURI = baseURL.Split('/')[0];
                FacadeClient client = new FacadeClient();
                string response = client.DecryptText(baseURI, "/" + endPoint + "/SecureHandler/DecryptText?secureText=" + text + "&passCode=IAP2GO_SEC!URE");
                // sample response : "{"response":"Decrypted String"}"
                response = JsonConvert.DeserializeObject<dynamic>(response)?.response;
                return response;
            }
            catch (Exception ex)
            {
                LogHandler.LogError("Exception thrown in Decrypt Method of ResourceModelBuilder class. The Exception : {0}", LogHandler.Layer.Business, ex.Message);
            }
            return null;
        }
    }

    public class resourcesObj
    {
        public string ResourceId { get; set; }
        public string ResourceName { get; set; }
        public int ResourceTypeId { get; set; }
        public string ResourceTypeName { get; set; }
        public string ResourceTypeDisplayName { get; set; }
        public string portfolioId { get; set; }
        public bool Isactive { get; set; }
        public DateTime? startdate { get; set; }
        public DateTime? enddate { get; set; }
        public string Source { get; set; }

        public string Parent_ResourceId { get; set; }
        public string Parent_ResourceName { get; set; }
        public int Parent_ResourceTypeId { get; set; }
        public string parent_ResourceTypeName { get; set; }
        public string parent_ResourceTypeDisplayName { get; set; }


        public string Child_ResourceId { get; set; }
        public string Child_ResourceName { get; set; }
        public int Child_ResourceTypeId { get; set; }
        public string Child_ResourceTypeName { get; set; }
        public string Child_ResourceTypeDisplayName { get; set; }
        public string Child_PortfolioId { get; set; }
        public bool Child_Isactive { get; set; }
        public DateTime? Child_startdate { get; set; }
        public DateTime? Child_enddate { get; set; }
        public string Child_Source { get; set; }
    }

    public class DecryptDataReqMsg
    {
        public string secureText { get; set; }
        public string passCode { get; set; }
    }

}
