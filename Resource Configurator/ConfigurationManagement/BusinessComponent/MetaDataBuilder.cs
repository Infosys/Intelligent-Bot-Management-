/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.Ainauto.ConfigurationManager.BusinessEntity;
using Infosys.Solutions.Ainauto.Resource.DataAccess;
using Infosys.Solutions.ConfigurationManager.Resource.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE = Infosys.Solutions.Ainauto.ConfigurationManager.BusinessEntity;

namespace Infosys.Solutions.Ainauto.ConfigurationManager.BusinessComponent
{
    public class MetaDataBuilder
    {
        public List<BE.ResourceTypeMetadata> getResourceTypeMetaData(int tenantId)
        {
            List<BE.ResourceTypeMetadata> metadata = null;
            try
            {
                ResourceTypeDS rtDS = new ResourceTypeDS();
                ResourceTypeMetadataDS rtmetaDS = new ResourceTypeMetadataDS();
                ResourceTypeDependencyMapDS rtdmDS = new ResourceTypeDependencyMapDS();
                PlatformsDS pfDS = new PlatformsDS();

                metadata = (from rt in rtDS.GetAny().ToArray()
                            //join p in pfDS.GetAny().ToArray() on rt.PlatformId equals p.PlatformId
                            where rt.TenantId == tenantId && (rt.PlatfromType != " " || rt.PlatfromType==null)                            
                            //&& p.TenantId == tenantId
                            group new { rt } by new
                            {
                                rt.PlatfromType
                               // p.PlatformName
                            } into grpPT
                            select new BE.ResourceTypeMetadata
                            {
                               // platformid = Convert.ToString(grpPT.Key.PlatformId),
                                tenantid = Convert.ToString(tenantId),
                                platformtype = grpPT.Key.PlatfromType,
                                resourcetypedetails = (from pt in grpPT
                                                       group pt by new
                                                       {
                                                           pt.rt.ResourceTypeName,
                                                           pt.rt.ResourceTypeId,
                                                           pt.rt.IsMainEntiry,
                                                       } into rtdet
                                                       select new BE.Resourcetypedetails
                                                       {
                                                           resourcetypename = rtdet.Key.ResourceTypeName,
                                                           resourcetypeid = Convert.ToString(rtdet.Key.ResourceTypeId),
                                                           ismainentry = Convert.ToBoolean(rtdet.Key.IsMainEntiry),
                                                           //portfolioid=
                                                           parentdetails = (from pdt in grpPT
                                                                            join  rtdm in rtdmDS.GetAny().ToArray() on Convert.ToString(pdt.rt.ResourceTypeId) equals rtdm.DependencyResourceTypeId
                                                                            where rtdm.ResourcetypeId == rtdet.Key.ResourceTypeId
                                                                            //&& rtdm.DependencyType != "LEVEL2" 
                                                                            && rtdm.TenantId == tenantId
                                                                            select new BE.Parent_detail
                                                                            {
                                                                                resourcetypeid = Convert.ToString(pdt.rt.ResourceTypeId),
                                                                                resourcetypename = pdt.rt.ResourceTypeName,
                                                                                portfolioid=rtdm.PortfolioId
                                                                            }).ToList(),

                                                           childdetails = (from pdt in grpPT
                                                                           join rtdm in rtdmDS.GetAny().ToArray() on pdt.rt.ResourceTypeId equals rtdm.ResourcetypeId
                                                                           where rtdm.DependencyResourceTypeId== Convert.ToString(rtdet.Key.ResourceTypeId) &&
                                                                            //rtdm.DependencyType.Trim().ToUpper()=="LEVEL2" &&
                                                                            rtdm.TenantId==tenantId
                                                                           select new BE.Child_detail
                                                                           {
                                                                               resourcetypeid = Convert.ToString(pdt.rt.ResourceTypeId),
                                                                               resourcetypename = pdt.rt.ResourceTypeName,
                                                                               priority=Convert.ToInt32(rtdm.Priority)
                                                                               //portfolioid=rtdm.PortfolioId
                                                                           }).ToList(),
                                                           resourcetypemetadata = (from rtmeta in rtmetaDS.GetAny().ToArray()
                                                                                   where rtmeta.ResourceTypeId== rtdet.Key.ResourceTypeId && rtmeta.TenantId==tenantId
                                                                                   select new BE.Resourcetypedata
                                                                                   {
                                                                                       attributename=rtmeta.AttributeName,
                                                                                       DefaultValue=rtmeta.DefaultValue,
                                                                                       attributetype=rtmeta.AttributeType,
                                                                                       description=rtmeta.Description,
                                                                                       displayname=rtmeta.DisplayName,
                                                                                       ismandatory=rtmeta.IsMandatory,
                                                                                       issecret=(bool)rtmeta.issecret,
                                                                                       Sequence=Convert.ToString(rtmeta.Sequence)
                                                                                   }).ToList(),
                                                           logdetails=(from grp in grpPT
                                                                       where grp.rt.ResourceTypeId== rtdet.Key.ResourceTypeId
                                                                       select new BE.Logdetails
                                                                       {
                                                                         CreatedBy= grp.rt.CreatedBy,
                                                                         CreateDate= grp.rt.CreateDate,
                                                                         ModifiedBy= grp.rt.ModifiedBy,
                                                                         ModifiedDate=Convert.ToDateTime(grp.rt.ModifiedDate),
                                                                         ValidityStart= grp.rt.ValidityStart,
                                                                         ValidityEnd=grp.rt.ValidityEnd
                                                                       }).FirstOrDefault()


                                                         }
                                                       ).ToList()
                              }
                            ).ToList();
                foreach(BE.ResourceTypeMetadata obj in metadata)
                {
                    foreach (BE.Resourcetypedetails objDetails in obj.resourcetypedetails)
                    {
                        ResourceTypeDependencyMapDS rtdmDS1 = new ResourceTypeDependencyMapDS();
                        string portfolioID = rtdmDS1.GetAny().Where(p => p.DependencyResourceTypeId == objDetails.resourcetypeid).FirstOrDefault()!=null? 
                                            rtdmDS1.GetAny().Where(p => p.DependencyResourceTypeId == objDetails.resourcetypeid).FirstOrDefault().PortfolioId:"0_0";
                        objDetails.portfolioid = portfolioID;
                            //objDetails.parentdetails != null && objDetails.parentdetails.Count() > 0 ? objDetails.parentdetails[0].portfolioid : null;
                    }
                }
                return metadata;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string UpdateResourceTypeMetaData(BE.ResourceTypeMetadata metadata)
        {
            try
            {
                StringBuilder resMsg = new StringBuilder();

                if (metadata.resourcetypedetails!=null)
                {
                    ResourceTypeDS resTypeDS = new ResourceTypeDS();
                    ResourceTypeDependencyMapDS rtdmDS = new ResourceTypeDependencyMapDS();

                    IList<resourcetype> Updateresourcetypes = new List<resourcetype>();
                    var resourceTypes = resTypeDS.GetAll();
                    foreach (BE.Resourcetypedetails rtDetails in metadata.resourcetypedetails)
                    {
                        int resTypeId = Convert.ToInt32(rtDetails.resourcetypeid);
                        var resEntity = resourceTypes.Where(r => r.ResourceTypeId == resTypeId).FirstOrDefault();
                        if(resEntity!=null && resEntity.IsMainEntiry!=rtDetails.ismainentry)
                        {
                            resEntity.IsMainEntiry = rtDetails.ismainentry;
                            Updateresourcetypes.Add(resEntity);
                        }
                    }
                    if(Updateresourcetypes!=null && Updateresourcetypes.Count>0)
                    {
                        var response = resTypeDS.UpdateBatch(Updateresourcetypes);
                        resMsg.Append("ResourceType details updated successfully ");
                    }
                    var mappingDeletedResources = metadata.resourcetypedetails.Where(r => r.ismappingdeleted == true).ToList();
                    foreach (BE.Resourcetypedetails rtdetails in mappingDeletedResources)
                    {                        
                        if (rtdetails.ismappingdeleted)
                        {
                            //Code to remove the mapping
                            var entity = rtdmDS.Delete(new resourcetype_dependency_map()
                            {
                                DependencyResourceTypeId = rtdetails.resourcetypeid
                            });
                            resMsg.Append(entity ? "Mapping deleted successfully for resourcetypeID:" + rtdetails.resourcetypeid : "Mapping deletion failed for resourcetypeID:" + rtdetails.resourcetypeid);
                        }
                    }

                    var newMappingResources = metadata.resourcetypedetails.Where(r => r.ismappingdeleted== false).ToList();
                    foreach (BE.Resourcetypedetails rtdetails in newMappingResources)
                    {
                        if (rtdetails.isnewmapping)
                        {
                            List<resourcetype_dependency_map> list = new List<resourcetype_dependency_map>();
                            if (rtdetails.parentdetails != null)
                            {
                                foreach (BE.Parent_detail parent in rtdetails.parentdetails)
                                {
                                    resourcetype_dependency_map dependency_Map = new resourcetype_dependency_map()
                                    {
                                        ResourcetypeId = Convert.ToInt32(rtdetails.resourcetypeid),
                                        DependencyResourceTypeId = parent.resourcetypeid,
                                        DependencyType = "LEVEL1",
                                        PortfolioId= parent.portfolioid,
                                        Priority = 1,
                                        TenantId = Convert.ToInt32(metadata.tenantid)
                                    };
                                    list.Add(dependency_Map);
                                }
                            }
                            if (rtdetails.childdetails != null)
                            {
                                foreach (BE.Child_detail child in rtdetails.childdetails)
                                {
                                    resourcetype_dependency_map dependency_Map = new resourcetype_dependency_map()
                                    {
                                        ResourcetypeId = Convert.ToInt32(child.resourcetypeid),
                                        DependencyResourceTypeId = rtdetails.resourcetypeid,
                                        DependencyType = "LEVEL2",
                                        PortfolioId=child.portfolioid,
                                        Priority = child.priority,
                                        TenantId = Convert.ToInt32(metadata.tenantid)
                                    };
                                    list.Add(dependency_Map);
                                }
                            }
                            var res = rtdmDS.InsertBatch(list);
                            resMsg.Append(res != null ? "Mapping details added successfully for resourcetypeId:" + rtdetails.resourcetypeid : "Mapping details not added for resourcetypeId:" + rtdetails.resourcetypeid);

                        }
                        else
                        {
                            ResourceTypeDependencyMapDS rtdmDS1 = new ResourceTypeDependencyMapDS();
                            var resouceInfo = rtdmDS1.GetAny().Where(r => r.DependencyResourceTypeId ==rtdetails.resourcetypeid).ToList();
                            var DBchildRes = resouceInfo.Select(r => r.ResourcetypeId).ToList();
                            //rtdmDS1.GetAny().Where(r => r.DependencyResourceTypeId == rtdetails.resourcetypeid).Select(r => r.ResourcetypeId).ToList();
                            if (DBchildRes.Count() < rtdetails.childdetails.Count())
                            {
                                var newChildRes = (from rt in rtdetails.childdetails
                                                   where !DBchildRes.Contains(Convert.ToInt32(rt.resourcetypeid))
                                                   select rt).ToList();
                                List<resourcetype_dependency_map> list = new List<resourcetype_dependency_map>();
                                foreach (BE.Child_detail ch in newChildRes)
                                {
                                    if (!mappingDeletedResources.Select(r => r.resourcetypeid).ToList().Contains(ch.resourcetypeid))
                                    {
                                        resourcetype_dependency_map dependency_Map = new resourcetype_dependency_map()
                                        {
                                            ResourcetypeId = Convert.ToInt32(ch.resourcetypeid),
                                            DependencyResourceTypeId = rtdetails.resourcetypeid,
                                            DependencyType = "LEVEL2",
                                            Priority = ch.priority,
                                            PortfolioId=rtdetails.portfolioid,
                                            TenantId = Convert.ToInt32(metadata.tenantid)
                                        };
                                        list.Add(dependency_Map);
                                    }
                                }
                                var res = rtdmDS.InsertBatch(list);
                                resMsg.Append(res != null ? "Mapping details added successfully for resourcetypeId:" + rtdetails.resourcetypeid : "Mapping details not added for resourcetypeId:" + rtdetails.resourcetypeid);
                            }
                            else
                            {
                               foreach(resourcetype_dependency_map obj in resouceInfo)
                                {
                                    ResourceTypeDependencyMapDS rtdmDS2 = new ResourceTypeDependencyMapDS();
                                    obj.PortfolioId = rtdetails.portfolioid;
                                    var res = rtdmDS2.Update(obj);
                                }
                            }
                        }
                    }
                }
                return resMsg.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string AddResourceTypeAttributes(BE.ResourceType_Attribute attribute)
        {
            try
            {
                StringBuilder resMsg = new StringBuilder();
                if (attribute != null && !string.IsNullOrEmpty(attribute.resourcetypeid) && !string.IsNullOrEmpty(attribute.attributename))
                {
                    ResourceTypeMetadataDS rtmdDS = new ResourceTypeMetadataDS();
                    int restypeID = Convert.ToInt32(attribute.resourcetypeid);
                    var resourcetype = rtmdDS.GetAny().Where(r => r.ResourceTypeId == restypeID && r.AttributeName == attribute.attributename).FirstOrDefault();
                    if (resourcetype != null)
                    {
                        resourcetype.Description = attribute.description;
                        resourcetype.DisplayName = attribute.displayname;
                        resourcetype.DefaultValue = attribute.defaultvalue;
                        resourcetype.issecret = attribute.issecret;
                        var res=rtmdDS.Update(resourcetype);
                        resMsg.Append(res != null ? string.Format("Attribute:{0} updated successfully for resourcetypeid:{1}", attribute.attributename, attribute.resourcetypeid) : string.Format("Attribute:{0} updation failed for resourcetypeid:{1}", attribute.attributename, attribute.resourcetypeid));
                    }
                    else
                    {
                        resourcetype_metadata metadata = new resourcetype_metadata()
                        {
                            ResourceTypeId = Convert.ToInt32(attribute.resourcetypeid),
                            Description = attribute.description,
                            AttributeName = attribute.attributename,
                            DefaultValue = attribute.defaultvalue,
                            DisplayName=attribute.displayname,
                            AttributeType=attribute.attributetype,
                            Sequence = rtmdDS.GetAny().Where(r => r.ResourceTypeId == restypeID).OrderByDescending(r => r.Sequence).FirstOrDefault() != null ?
                                        Convert.ToInt32(rtmdDS.GetAny().Where(r => r.ResourceTypeId == restypeID).OrderByDescending(r => r.Sequence).FirstOrDefault().Sequence)+1: 1,
                            IsMandatory = true,
                            TenantId=1                            
                        };
                        var isAdded = rtmdDS.Insert(metadata);
                        resMsg.Append(isAdded != null ? string.Format("Attribute:{0} added successfully for resourcetypeid:{1}", attribute.attributename, attribute.resourcetypeid) : string.Format("Attribute:{0} insertion failed for resourcetypeid:{1}", attribute.attributename, attribute.resourcetypeid));
                    }
                }
                else
                    resMsg.Append("Invalid input Data");

                return resMsg.ToString();
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string DeleteResourceTypeAttributes(BE.ResourceType_Attribute attribute)
        {
            try
            {
                StringBuilder resMsg = new StringBuilder();
                if (attribute != null && !string.IsNullOrEmpty(attribute.resourcetypeid) && !string.IsNullOrEmpty(attribute.attributename))
                {
                    ResourceTypeMetadataDS rtmdDS = new ResourceTypeMetadataDS();
                    int resTypeID = Convert.ToInt32(attribute.resourcetypeid);
                    var resourcetype = rtmdDS.GetAny().Where(r => r.ResourceTypeId == resTypeID && r.AttributeName == attribute.attributename).FirstOrDefault();
                    if (resourcetype != null)
                    {                        
                        bool isSuccess = rtmdDS.Delete(resourcetype);
                        resMsg.Append(isSuccess ? string.Format("Attribute:{0} deleted successfully for resourcetypeid:{1}", attribute.attributename, attribute.resourcetypeid) : string.Format("Attribute:{0} deletion failed for resourcetypeid:{1}", attribute.attributename, attribute.resourcetypeid));
                    }
                    else
                    {
                        resMsg.Append(string.Format("Attribute:{0} not exists for resourcetypeid:{1}", attribute.attributename, attribute.resourcetypeid));
                    }
                }
                else
                    resMsg.Append("Invalid input Data");

                return resMsg.ToString();
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<BE.Platform_Info> getPlatformDetails(int tenantId)
        {
            try
            {
                ResourceTypeDS rtDS = new ResourceTypeDS();
                PlatformsDS pfDS = new PlatformsDS();

                var result = (from rt in rtDS.GetAny().ToArray()
                              //join p in pfDS.GetAny().ToArray() on rt.PlatformId equals p.PlatformId
                              where rt.TenantId == tenantId 
                              //&& p.TenantId == tenantId
                              group new { rt } by new
                              {
                                  rt.PlatfromType
                                 // p.PlatformName
                              } into grpPT
                              select new BE.Platform_Info
                              {
                                  platformname=grpPT.Key.PlatfromType,
                                  resourcetypedetails=(from pt in grpPT
                                                       group pt by new
                                                       {
                                                           pt.rt.ResourceTypeName
                                                       } into rtdet
                                                       select new BE.Resourcetype_detail
                                                       {
                                                           resourcetypename=rtdet.Key.ResourceTypeName
                                                       }
                                                       ).ToList()
                              }
                            ).ToList();

                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
