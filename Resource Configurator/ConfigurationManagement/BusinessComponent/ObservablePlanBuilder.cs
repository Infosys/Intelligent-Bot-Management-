/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
//using BE = Infosys.Ainauto.ConfigurationManager.BusinessEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infosys.Solutions.Ainauto.Resource.DataAccess;
//using Infosys.Solutions.ConfigurationManager.Resource.Entity;
using Newtonsoft.Json;
using Infosys.Solutions.ConfigurationManager.Infrastructure.Common;
using BE=Infosys.Ainauto.ConfigurationManager.BusinessEntity;

namespace Infosys.Solutions.Ainauto.ConfigurationManager.BusinessComponent
{
    public class ObservablePlanBuilder
    {
        public BE.actionModelMap GetActions(int tenantID)
        {
            ActionDS actDS = new ActionDS();
            BE.actionModelMap actModel = new BE.actionModelMap();
            
            try
            {
                List<BE.actionModel> actionList = new List<BE.actionModel>();
                actionList = (from a in actDS.GetAll().ToArray()
                              where a.TenantId == tenantID
                              select new BE.actionModel
                              {
                                  actionid = a.ActionId,
                                  actionname = a.ActionName,
                                  actiontypeid = a.ActionTypeId,
                                  endpointuri = a.EndpointUri,
                                  scriptid = Convert.ToInt32(a.ScriptId),
                                  categoryid = Convert.ToInt32(a.CategoryId),
                                  automationengineid = Convert.ToInt32(a.AutomationEngineId),
                                  createdby = a.CreatedBy,
                                  CreateDate = a.CreateDate,
                                  ModifiedBy = a.ModifiedBy,
                                  ModifiedDate = Convert.ToDateTime(a.ModifiedDate),
                                  ValidityEnd = Convert.ToDateTime(a.ValidityEnd),
                                  ValidityStart = Convert.ToDateTime(a.ValidityStart)

                              }
                              ).ToList();
                actModel.tenantID = tenantID;
                actModel.actList = actionList;
                return actModel;
            }
            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        public BE.resourceTypeModelMap GetResourceType(int tenantID)
        {
            ResourceTypeDS resType = new ResourceTypeDS();
            BE.resourceTypeModelMap rMap = new BE.resourceTypeModelMap();
            try
            {
                List<BE.resourceTypeModel> rTypeList = new List<BE.resourceTypeModel>();
                rTypeList = (from a in resType.GetAll().ToArray()
                             where a.TenantId == tenantID 
                             //&& a.PlatformId == platformID

                             select new BE.resourceTypeModel
                             {
                                 resourcetypeid = a.ResourceTypeId,
                                 resourcetypename = a.ResourceTypeName,
                                 resourcetypedisplayname= a.ResourceTypeDisplayName

                             }).ToList();

                rMap.tenantid = tenantID;
               // rMap.platformid = platformID;
                rMap.resTypeList = rTypeList;
                return rMap;
            }
            catch(Exception ex)
            {
                throw ex;
            }

        }

        public BE.resourceTypeModelMap GetResourceType(int platformId, int tenantID)
        {
            ResourceDS resourceDS = new ResourceDS();
            ResourceTypeDS resType = new ResourceTypeDS();
            BE.resourceTypeModelMap rMap = new BE.resourceTypeModelMap();
            try
            {
                var resTypeList = (from r in resourceDS.GetAll()
                                where r.PlatformId == platformId
                                && r.TenantId == tenantID
                                group r by r.ResourceTypeId into g
                                select g.Key).ToList();

                List<BE.resourceTypeModel> rTypeList = new List<BE.resourceTypeModel>();
                rTypeList = (from a in resType.GetAll().ToArray()
                             where resTypeList.Contains(a.ResourceTypeId)
                             select new BE.resourceTypeModel
                             {
                                 resourcetypeid = a.ResourceTypeId,
                                 resourcetypename = a.ResourceTypeName,
                                 resourcetypedisplayname= a.ResourceTypeDisplayName

                             }).ToList();

                rMap.tenantid = tenantID;
                rMap.platformid = platformId;
                rMap.resTypeList = rTypeList;
                return rMap;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public BE.observablemodelmap GetobservableModel(int tenantID)
        {
            ObservableDS obs = new ObservableDS();
            BE.observablemodelmap obsMap = new BE.observablemodelmap();
            try
            {
                List<BE.observableModel> obsList = new List<BE.observableModel>();
                obsList = (from a in obs.GetAll().ToArray()
                             where a.TenantId == tenantID 

                             select new BE.observableModel
                             {
                                 observableid = a.ObservableId,
                                 observablename = a.ObservableName,
                                 unitofmeasure = a.UnitOfMeasure,
                                 createdby = a.CreatedBy,
                                 CreateDate = a.CreateDate,
                                 ModifiedBy = a.ModifiedBy,
                                 ModifiedDate = Convert.ToDateTime(a.ModifiedDate),
                                 ValidityEnd = a.ValidityEnd,
                                 ValidityStart = a.ValidityStart,
                                 datatype = a.DataType


                             }).ToList();
                obsMap.tenentId = tenantID;
                obsMap.obsList = obsList;
                return obsMap;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public BE.actionTypeDetails getActionTypeDetails(int tenantId)
        {
            ActionTypeDS actDS = new ActionTypeDS();
            List<BE.actiontype> actionTypeList = new List<BE.actiontype>();
            BE.actionTypeDetails actionDetailsObj = new BE.actionTypeDetails();
            

            try
            {
                actionTypeList = (from a in actDS.GetAll().ToArray()
                                  where a.TenantId==tenantId
                                  select new BE.actiontype
                                  {
                                      ActionType = a.ActionType,
                                      ActionTypeId = a.ActionTypeId,
                                      CreateDate = a.CreateDate,
                                      ModifiedDate = Convert.ToDateTime(a.ModifiedDate),
                                      ModifiedBy = a.ModifiedBy,
                                      createdby = a.CreatedBy


                                  }).ToList();


                actionDetailsObj.TenantId = tenantId;
                actionDetailsObj.actionTypeList = actionTypeList;
                return actionDetailsObj;

            }
            catch(Exception ex)
            {
                throw ex;
            }


            
        }



    }
}
