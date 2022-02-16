/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using BE = Infosys.Ainauto.ConfigurationManager.BusinessEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infosys.Solutions.Ainauto.Resource.DataAccess;
using Infosys.Solutions.ConfigurationManager.Resource.Entity;
using Newtonsoft.Json;
using Infosys.Solutions.ConfigurationManager.Infrastructure.Common;

namespace Infosys.Solutions.Ainauto.ConfigurationManager.BusinessComponent
{
   public class ActionBuilder
    {
        public BE.action getActionDetails(int tenantId)
        {
            ActionDS actDS = new ActionDS();
            ActionTypeDS actionTypeDs = new ActionTypeDS();
            ActionParamsDS actionParamDS = new ActionParamsDS();
            
            try {
                var actionList = (from a in actDS.GetAll().ToArray()
                                      // join c in actionParamDS.GetAll().ToArray() on a.ActionId equals c.ActionId into actParams
                                      // from r in actParams.DefaultIfEmpty()
                                  join b in actionTypeDs.GetAll().ToArray() on a.ActionTypeId equals b.ActionTypeId
                                  where a.TenantId == b.TenantId &&
                                        a.IsDeleted == false &&
                                        a.TenantId == tenantId && (a.ValidityEnd > DateTime.Today)
                                  select new BE.actiondetails
                                  {
                                      actionid = a.ActionId,
                                      actionname = a.ActionName,
                                      actiontypeid = a.ActionTypeId,
                                      automationengineid = Convert.ToInt32(a.AutomationEngineId),
                                      automationenginename=a.AutomationEngineName,
                                      categoryid = Convert.ToInt32(a.CategoryId),
                                      categoryname=a.CategoryName,
                                      CreateDate = a.CreateDate,
                                      createdby = a.CreatedBy,
                                      endpointuri = a.EndpointUri,
                                      ModifiedBy = a.ModifiedBy,
                                      ModifiedDate = Convert.ToDateTime(a.ModifiedDate),
                                      scriptid = Convert.ToInt32(a.ScriptId),
                                      //  a.TenantId,
                                      ValidityEnd = Convert.ToString(a.ValidityEnd),
                                      ValidityStart = Convert.ToString(a.ValidityStart),
                                      actiontype = b.ActionType,
                                      actionparams = (from c in actionParamDS.GetAll().ToArray()
                                                      where c.ActionId == a.ActionId
                                                      select c).FirstOrDefault() != null ? (from p in actionParamDS.GetAll().ToArray()
                                                                                            where p.ActionId == a.ActionId
                                                                                            select new BE.actionParams
                                                                                            {

                                                                                                name = p.Name,
                                                                                                fieldtomap = p.FieldToMap,
                                                                                                ismandatory = Convert.ToBoolean(p.IsMandatory),
                                                                                                defaultvalue = p.DefaultValue,
                                                                                                automationengineparamid = Convert.ToInt32(p.AutomationEngineParamId)
                                                                                            }).ToList() : null,

                                  }).ToList();
                              

                List<BE.actiondetails> actionDetails = new List<BE.actiondetails>();
                actionDetails = actionList;
               /* actionDetails = (from o in actionList
                                 group o by o.ActionId into g
                                 from ch in g
                                 //{ o.ActionId, o.ActionName, o.ActionTypeId, o.ActionType, o.EndpointUri, o.ScriptId, o.CategoryId, o.AutomationEngineId,
                                   //  o.CreatedBy, o.CreateDate, o.ModifiedBy, o.ModifiedDate, o.ValidityStart, o.ValidityEnd, o.ActionParams } into g
                                 select new BE.actiondetails
                                 {
                                     actionid = g.Key,
                                     actionname = ch.ActionName,
                                     actiontypeid = ch.ActionTypeId,
                                     endpointuri = ch.EndpointUri,
                                     scriptid = Convert.ToInt32(ch.ScriptId),
                                     categoryid = Convert.ToInt32(ch.CategoryId),
                                     automationengineid = Convert.ToInt32(ch.AutomationEngineId),
                                     createdby = ch.CreatedBy,
                                     CreateDate = ch.CreateDate,
                                     ModifiedBy = ch.ModifiedBy,
                                     ModifiedDate = Convert.ToDateTime(ch.ModifiedDate),
                                     ValidityStart = Convert.ToString(ch.ValidityStart),
                                     ValidityEnd = Convert.ToString(ch.ValidityEnd),
                                     actiontype =ch.ActionType,

                                    
                                     actionparams = (from a in ch.ActionParams
                                                     select new BE.actionParams
                                                     {
                                                         name = a.Name,
                                                         fieldtomap = a.FieldToMap,
                                                         ismandatory = Convert.ToBoolean(a.IsMandatory),
                                                         defaultvalue = a.DefaultValue,
                                                         automationengineparamid = Convert.ToInt32(a.AutomationEngineParamId)

                                                     }).ToList()
                                 }).ToList();*/


                BE.action actionObj = new BE.action();
                actionObj.tenantid = tenantId;
                actionObj.actiondetails = actionDetails;
                return actionObj;
            }
            catch(Exception ex)
            {

                throw ex;
            }
     }

        public string insertActionDetails(BE.action actionObject)
        {
            StringBuilder responseMessage = new StringBuilder();
            ActionDS actDS = new ActionDS();
            ActionTypeDS actionTypeDs = new ActionTypeDS();
            ActionParamsDS actionParamDS = new ActionParamsDS();
            int actionTypeId = 0;
            try
            {
                foreach(BE.actiondetails actDetails in actionObject.actiondetails)
                {
                    var actiontypelist = (from o in actionTypeDs.GetAll().ToArray()
                                          select o.ActionType).ToArray();
                    if(!Array.Exists(actiontypelist,element => element == actDetails.actiontype))
                    {
                        actiontype actionTypeObjToAdd = new actiontype();
                        actionTypeObjToAdd.ActionTypeId = actDetails.actiontypeid;
                        actionTypeObjToAdd.ActionType = actDetails.actiontype;
                        actionTypeObjToAdd.CreatedBy = actDetails.createdby;
                        actionTypeObjToAdd.CreateDate = DateTime.UtcNow;
                        actionTypeObjToAdd.TenantId = actionObject.tenantid;
                        var action_typeRes=actionTypeDs.Insert(actionTypeObjToAdd);
                        responseMessage.Append(action_typeRes == null ? "\n Insertion of child resource data failed for the ActionType:" + actDetails.actiontype : "\n Insertion of child resource data success for the resourceID:" + actDetails.actiontype);

                        

                    }

                    if(actDetails.actiontypeid==null || actDetails.actiontypeid == 0)
                    {
                        actionTypeId = (from o in actionTypeDs.GetAll().ToArray()
                                            where o.ActionType == actDetails.actiontype && o.TenantId == actionObject.tenantid
                                            select o.ActionTypeId).FirstOrDefault();
                    }

                    else
                    {
                        actionTypeId = actDetails.actiontypeid;
                    }
                    var actionList = (from a in actDS.GetAll().ToArray()
                                      select a.ActionId).ToArray();
                    int modifiedActionId = 0;
                    if(!Array.Exists(actionList,element => element == actDetails.actionid))
                    {
                        action actionObjToAdd = new action();
                        actionObjToAdd.ActionId = actDetails.actionid;
                        actionObjToAdd.ActionName = actDetails.actionname;
                        actionObjToAdd.ActionTypeId = actionTypeId;
                        actionObjToAdd.EndpointUri = actDetails.endpointuri;
                        actionObjToAdd.CreatedBy = actDetails.createdby;
                        actionObjToAdd.CreateDate = Convert.ToDateTime(actDetails.CreateDate);
                        actionObjToAdd.ModifiedBy = actDetails.ModifiedBy;
                        actionObjToAdd.ModifiedDate = Convert.ToDateTime(actDetails.ModifiedDate);
                       // actionObjToAdd.ValidityStart = Convert.ToDateTime(actDetails.ValidityStart);
                        actionObjToAdd.IsDeleted = false;
                        // actionObjToAdd.ValidityEnd = Convert.ToDateTime(actDetails.ValidityEnd);
                        actionObjToAdd.ValidityStart = actDetails.ValidityStart.Contains("India Standard Time") ? DateTime.ParseExact(actDetails.ValidityStart, "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(India Standard Time)'", System.Globalization.CultureInfo.InvariantCulture) : Convert.ToDateTime(actDetails.ValidityStart);
                        actionObjToAdd.ValidityEnd = actDetails.ValidityEnd.Contains("India Standard Time") ? DateTime.ParseExact(actDetails.ValidityEnd, "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(India Standard Time)'", System.Globalization.CultureInfo.InvariantCulture) : Convert.ToDateTime(actDetails.ValidityEnd);
                        actionObjToAdd.TenantId = actionObject.tenantid;
                        actionObjToAdd.ScriptId = actDetails.scriptid;
                        actionObjToAdd.CategoryId = actDetails.categoryid;
                        actionObjToAdd.CategoryName = actDetails.categoryname;
                        actionObjToAdd.AutomationEngineId = actDetails.automationengineid;
                        actionObjToAdd.AutomationEngineName = actDetails.automationenginename;

                        var actionRes = actDS.Insert(actionObjToAdd);
                        
                        responseMessage.Append(actionRes == null ? "\n Insertion of child resource data failed for the Action:" + actionObjToAdd.ActionId : "\n Insertion of child resource data success for the Action:" + actionObjToAdd.ActionId);
                        modifiedActionId = actionObjToAdd.ActionId;
                    }

                    

                    var actionParamsList = (from a in actionParamDS.GetAll().ToArray()
                                            where a.ActionId == actDetails.actionid
                                            select a.Name).ToArray();
                                            
                    foreach (BE.actionParams actParams in actDetails.actionparams)
                    {
                        if(!Array.Exists(actionParamsList,element => element == actParams.name))
                        {
                            action_params actionParamObj = new action_params();
                            actionParamObj.Name = actParams.name;
                            actionParamObj.FieldToMap = actParams.fieldtomap;
                            actionParamObj.IsMandatory = actParams.ismandatory;
                            actionParamObj.DefaultValue = actParams.defaultvalue;
                            actionParamObj.AutomationEngineParamId = actParams.automationengineparamid;
                            actionParamObj.ActionId = modifiedActionId;
                            actionParamObj.CreateDate = Convert.ToDateTime(actDetails.CreateDate);
                            actionParamObj.CreatedBy = actDetails.createdby;
                            actionParamObj.TenantId = actionObject.tenantid;


                            var actionParamRes = actionParamDS.Insert(actionParamObj);
                            responseMessage.Append(actionParamRes == null ? "\n Insertion of child resource data failed for the ActionParam:" + actionParamObj.Name : "\n Insertion of child resource data success for the Action:" + actionParamObj.Name);
                        }
                        
                    }

                   





                }

                return responseMessage.ToString();



            }
            catch(Exception ex)
            {
                throw ex;
            }

            
        }

        public string updateActionDetails(BE.action actionObject)
        {
            StringBuilder responseMessage = new StringBuilder();
            ActionDS actDS = new ActionDS();
            ActionTypeDS actionTypeDs = new ActionTypeDS();
            ActionParamsDS actionParamDS = new ActionParamsDS();
            int actionTypeId = 0;
            try
            {
                if (actionObject != null)
                {
                    List<actiontype> actionTypeList = new List<actiontype>();
                    List<action> actionList = new List<action>();
                    List<action_params> actionParamsList = new List<action_params>();
                    foreach (BE.actiondetails actionTypeObj in actionObject.actiondetails)
                    {
                        actiontype actType = new actiontype();
                        actType.ActionType = actionTypeObj.actiontype;
                        actType.CreatedBy = actionTypeObj.createdby;
                        actType.CreateDate = actionTypeObj.CreateDate;
                        actType.TenantId = actionObject.tenantid;
                        actType.ModifiedBy = actionTypeObj.ModifiedBy;
                        actType.ModifiedDate = actionTypeObj.ModifiedDate;
                        actType.ActionTypeId = actionTypeObj.actiontypeid;
                        actionTypeList.Add(actType);


                        if (actionTypeObj.actiontypeid == null || actionTypeObj.actiontypeid == 0)
                        {
                            actionTypeId = (from o in actionTypeDs.GetAll().ToArray()
                                            where o.ActionType == actionTypeObj.actiontype && o.TenantId == actionObject.tenantid
                                            select o.ActionTypeId).FirstOrDefault();
                        }

                        else
                        {
                            actionTypeId = actionTypeObj.actiontypeid;
                        }


                        action actionObjToAdd = new action();
                        actionObjToAdd.ActionId = actionTypeObj.actionid;
                        actionObjToAdd.ActionName = actionTypeObj.actionname;
                        actionObjToAdd.ActionTypeId = actionTypeId;
                        actionObjToAdd.EndpointUri = actionTypeObj.endpointuri;
                        actionObjToAdd.CreatedBy = actionTypeObj.createdby;
                        actionObjToAdd.CreateDate = actionTypeObj.CreateDate;
                        actionObjToAdd.ModifiedBy = actionTypeObj.ModifiedBy;
                        actionObjToAdd.ModifiedDate = actionTypeObj.ModifiedDate;
                        actionObjToAdd.ValidityStart = actionTypeObj.ValidityStart.Contains("India Standard Time") ? DateTime.ParseExact(actionTypeObj.ValidityStart, "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(India Standard Time)'", System.Globalization.CultureInfo.InvariantCulture) : Convert.ToDateTime(actionTypeObj.ValidityStart);
                            //actionTypeObj.ValidityStart;
                        actionObjToAdd.IsDeleted = false;
                        actionObjToAdd.ValidityEnd = actionTypeObj.ValidityEnd.Contains("India Standard Time") ? DateTime.ParseExact(actionTypeObj.ValidityEnd, "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(India Standard Time)'", System.Globalization.CultureInfo.InvariantCulture) : Convert.ToDateTime(actionTypeObj.ValidityEnd);
                        //actionTypeObj.ValidityEnd;
                        actionObjToAdd.TenantId = actionObject.tenantid;
                        actionObjToAdd.ScriptId = actionTypeObj.scriptid;
                        actionObjToAdd.CategoryId = actionTypeObj.categoryid;
                        actionObjToAdd.CategoryName = actionTypeObj.categoryname;
                        actionObjToAdd.AutomationEngineId = actionTypeObj.automationengineid;
                        actionObjToAdd.AutomationEngineName = actionTypeObj.automationenginename;
                        actionList.Add(actionObjToAdd);

                        var actionParamsList2 = (from a in actionParamDS.GetAll().ToArray()
                                                where a.ActionId == actionTypeObj.actionid
                                                select a.Name).ToArray();
                        foreach (BE.actionParams actParamsObj in actionTypeObj.actionparams)
                        {
                            if (!Array.Exists(actionParamsList2, element => element == actParamsObj.name))
                            {
                                action_params actionParamObj = new action_params();
                                actionParamObj.Name = actParamsObj.name;
                                actionParamObj.FieldToMap = actParamsObj.fieldtomap;
                                actionParamObj.IsMandatory = actParamsObj.ismandatory;
                                actionParamObj.DefaultValue = actParamsObj.defaultvalue;
                                actionParamObj.AutomationEngineParamId = actParamsObj.automationengineparamid;
                                actionParamObj.ActionId = actionTypeObj.actionid;
                                actionParamObj.CreateDate = Convert.ToDateTime(actionTypeObj.CreateDate);
                                actionParamObj.CreatedBy = actionTypeObj.createdby;
                                actionParamObj.TenantId = actionObject.tenantid;
                                


                                var actionParamRes = actionParamDS.Insert(actionParamObj);
                            }
                            else {
                                if(actParamsObj.isDeleted == true)
                                {
                                    action_params actParamObj = new action_params();
                                    actParamObj.Name = actParamsObj.name; 
                                    actParamObj.FieldToMap = actParamsObj.fieldtomap;
                                    actParamObj.IsMandatory = actParamsObj.ismandatory;
                                    actParamObj.DefaultValue = actParamsObj.defaultvalue;
                                    actParamObj.AutomationEngineParamId = actParamsObj.automationengineparamid;
                                    actParamObj.ActionId = actionTypeObj.actionid;
                                    var paramId = (from a in actionParamDS.GetAll().ToArray()
                                                   where a.ActionId == actParamObj.ActionId && a.Name == actParamObj.Name
                                                   select a.ParamId).FirstOrDefault();
                                    actParamObj.ParamId = paramId;
                                    actionParamDS.Delete(actParamObj);
                                }
                                else {
                                    action_params actParamObj = new action_params();
                                    actParamObj.Name = actParamsObj.name;
                                    actParamObj.FieldToMap = actParamsObj.fieldtomap;
                                    actParamObj.IsMandatory = actParamsObj.ismandatory;
                                    actParamObj.DefaultValue = actParamsObj.defaultvalue;
                                    actParamObj.AutomationEngineParamId = actParamsObj.automationengineparamid;
                                    actParamObj.ActionId = actionTypeObj.actionid;

                                    actionParamsList.Add(actParamObj);
                                }
                                
                            }

                        }


                    }
                    var res = actionTypeDs.UpdateBatch(actionTypeList);
                    responseMessage.Append(res == null ? "\n Updation of actionType data failed " : "\n Updation of actionType data success ");
                    var res2 = actionParamDS.UpdateBatch(actionParamsList);
                    responseMessage.Append(res2 == null ? "\n Updation of actionParam data failed " : "\n Updation of actionParam data success ");
                    var res3 = actDS.UpdateBatch(actionList);
                    responseMessage.Append(res3 == null ? "\n Updation of action data failed " : "\n Updation of action data success ");



                }
            }catch(Exception ex)
            {
                throw ex;
            }

            return responseMessage.ToString();



        }

        public string deleteActionDetails(BE.action actionObj)
        {
            StringBuilder responseMessage = new StringBuilder();
            ActionDS actDS = new ActionDS();
            ActionTypeDS actionTypeDs = new ActionTypeDS();
            ActionParamsDS actionParamDS = new ActionParamsDS();
            int actionTypeId = 0;
            try {
                if (actionObj != null)
                {
                    List<actiontype> actionTypeList = new List<actiontype>();
                    List<action> actionList = new List<action>();
                    List<action_params> actionParamsList = new List<action_params>();
                    foreach (BE.actiondetails actionTypeObj in actionObj.actiondetails)
                    {
                        action actionToDelete = new action();
                        if (actionTypeObj.actiontypeid == null || actionTypeObj.actiontypeid == 0)
                        {
                            actionTypeId = (from o in actionTypeDs.GetAll().ToArray()
                                            where o.ActionType == actionTypeObj.actiontype && o.TenantId == actionObj.tenantid
                                            select o.ActionTypeId).FirstOrDefault();
                        }

                        else
                        {
                            actionTypeId = actionTypeObj.actiontypeid;
                        }


                        //action actionObjToAdd = new action();
                        actionToDelete.ActionId = actionTypeObj.actionid;
                        actionToDelete.ActionName = actionTypeObj.actionname;
                        actionToDelete.ActionTypeId = actionTypeId;
                        actionToDelete.EndpointUri = actionTypeObj.endpointuri;
                        actionToDelete.CreatedBy = actionTypeObj.createdby;
                        actionToDelete.CreateDate = actionTypeObj.CreateDate;
                        actionToDelete.ModifiedBy = actionTypeObj.ModifiedBy;
                        actionToDelete.IsDeleted = true;
                        actionToDelete.ModifiedDate = actionTypeObj.ModifiedDate;
                        actionToDelete.ValidityStart = actionTypeObj.ValidityStart.Contains("India Standard Time") ? DateTime.ParseExact(actionTypeObj.ValidityStart, "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(India Standard Time)'", System.Globalization.CultureInfo.InvariantCulture) : Convert.ToDateTime(actionTypeObj.ValidityStart);
                        //actionTypeObj.ValidityStart;
                        actionToDelete.ValidityEnd = actionTypeObj.ValidityEnd.Contains("India Standard Time") ? DateTime.ParseExact(actionTypeObj.ValidityEnd, "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(India Standard Time)'", System.Globalization.CultureInfo.InvariantCulture) : Convert.ToDateTime(actionTypeObj.ValidityEnd);
                        // actionToDelete.ValidityStart = actionTypeObj.ValidityStart;

                        //actionToDelete.ValidityEnd = actionTypeObj.ValidityEnd;
                        actionToDelete.TenantId = actionObj.tenantid;
                        actionToDelete.ScriptId = actionTypeObj.scriptid;
                        actionToDelete.CategoryId = actionTypeObj.categoryid;
                        actionToDelete.AutomationEngineId = actionTypeObj.automationengineid;
                        actionList.Add(actionToDelete);
                        var res = actDS.UpdateBatch(actionList);
                        responseMessage.Append(res == null ? "\n Deletion of action data failed " : "\n Deletion of actionType data success ");


                    }
                }

            }catch(Exception ex)
            {
                throw ex;
            }

            return responseMessage.ToString();
            }
            
        }


    }

