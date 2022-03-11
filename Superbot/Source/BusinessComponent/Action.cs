/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.Solutions.Superbot.Infrastructure.Common;
using DE = Infosys.Solutions.Superbot.Resource.Entity;
using DA = Infosys.Solutions.Ainauto.Resource.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infosys.Solutions.Ainauto.Resource.DataAccess;
using Infosys.Solutions.Ainauto.Services.Superbot.Contracts.Message;
using SE = Infosys.Solutions.Ainauto.Services.Superbot.Contracts.Data;
//using Infosys.Solutions.Ainauto.BotClientLibrary;
using BE = Infosys.Solutions.Ainauto.BusinessEntity;
using Infosys.Solutions.Ainauto.Superbot.Infrastructure.ServiceClientLibrary;
using System.IO;
using Newtonsoft.Json;
using System.Threading;

namespace Infosys.Solutions.Ainauto.Superbot.BusinessComponent
{
    public class Action
    {
        private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
        bool isRemediationFailed = false;
        static int tenantId;
        public BE.RemediationPlan GetRemediationPlan(BE.Anomaly anomaly)
        {
            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_Start, "GetRemediationPlan", "Action"), LogHandler.Layer.Business, null);
            LogHandler.LogDebug(String.Format("Executing the GetRemediation method of Action class with parameters:\n anomaly={0} ", anomaly),
                LogHandler.Layer.Business, null);
            try
            {

                BE.RemediationPlan returnObject = new BE.RemediationPlan();

                int remediationPlanId = Helper.GetRemediationPlanId(anomaly.ResourceId,anomaly.ResourceTypeId,anomaly.ObservableId);
                //-1 : remediation plan Expired ; 0 : remediation plan not found 
                if (remediationPlanId == -1)
                    return null;                
                
                //fetching RemediationPlan ID
                LogHandler.LogDebug("Fetching th RemediationPlanId from resourcetype_observable_remediation_plan_map table with ResourceTypeId:{0} and ObservableId:{1}",
                    LogHandler.Layer.Business, anomaly.ResourceTypeId, anomaly.ObservableId);
                

                if (remediationPlanId != 0)
                {
                    //Fetching RemediationPlan Details
                    LogHandler.LogDebug("Fetching th RemediationPlan Details from remediation_plan table with RemediationPlanId:{0}",
                         LogHandler.Layer.Business, remediationPlanId);

                    DE.remediation_plan remediation_Plan = new DE.remediation_plan();
                    remediation_Plan.RemediationPlanId = remediationPlanId;
                    RemediationPlanDS remediationPlanDS = new RemediationPlanDS();
                    remediation_Plan = remediationPlanDS.GetOne(remediation_Plan);

                    if (remediation_Plan != null)
                    {
                        tenantId = remediation_Plan.TenantId;
                        //assigning value to the remediationPlan Object
                        returnObject = Translator.ActionTranslator.RemediationPlan_DE_BE(remediation_Plan);

                        //fetching the List of actions for the RemediationPlan
                        LogHandler.LogDebug("Fetching List of actions for the RemediationPlan from remediation_plan_action_map table with RemediationPlanId:{0}",
                            LogHandler.Layer.Business, remediation_Plan.RemediationPlanId);

                        DE.remediation_plan_action_map remediationPlanActionMap = new DE.remediation_plan_action_map();
                        remediationPlanActionMap.RemediationPlanId = remediation_Plan.RemediationPlanId;
                        IList<DE.remediation_plan_action_map> remediationPlanActionMap_List = new List<DE.remediation_plan_action_map>();
                        RemediationPlanActionMapDS remediationPlanActionMapDS = new RemediationPlanActionMapDS();
                        remediationPlanActionMap_List = remediationPlanActionMapDS.GetAll(remediationPlanActionMap);

                        if (remediationPlanActionMap_List != null && remediationPlanActionMap_List.Count > 0)
                        {
                            List<BE.RPActions> action_List = new List<BE.RPActions>();

                            //looping through all the actions
                            LogHandler.LogDebug("looping through all the actions for remediationPlanId:{0}",
                                    LogHandler.Layer.Business, remediation_Plan.RemediationPlanId);

                            foreach (DE.remediation_plan_action_map action in remediationPlanActionMap_List)
                            {
                                BE.RPActions actionObj = new BE.RPActions();

                                //fetching Details of this action
                                LogHandler.LogDebug("Fetching  action details for the ActionID:{0} from action table}",
                                    LogHandler.Layer.Business, action.ActionId);

                                ActionDS actionDS = new ActionDS();
                                DE.action actionObj_Temp = new DE.action();
                                actionObj_Temp.ActionId = action.ActionId;
                                actionObj_Temp = actionDS.GetOne(actionObj_Temp);   

                                if (actionObj_Temp != null)
                                {
                                    string actionTypeName = new ActionTypeDS().GetOne(new DE.actiontype() { ActionTypeId = actionObj_Temp.ActionTypeId }).ActionType1;
                                    //assigning values into action Object
                                    actionObj = Translator.ActionTranslator.Actions_DE_BE(action, actionObj_Temp);
                                    actionObj.actionTypeName = actionTypeName;

                                    //Fetching all Parameters for this action
                                    LogHandler.LogDebug("Fetching all Parameters for ActionID:{0} from action_params table",
                                        LogHandler.Layer.Business, action.ActionId);

                                    IList<DE.action_params> actionParams_List = new List<DE.action_params>();
                                    DE.action_params actionParams = new DE.action_params();
                                    actionParams.ActionId = action.ActionId;
                                    ActionParamsDS actionParamsDS = new ActionParamsDS();
                                    actionParams_List = actionParamsDS.GetAll(actionParams);

                                    if (actionParams_List != null && actionParams_List.Count > 0)
                                    {
                                        List<BE.RPParameters> params_List = new List<BE.RPParameters>();

                                        //looping through all the parameters for this action
                                        LogHandler.LogDebug("looping through all the parameters for the actionId:{0}",
                                            LogHandler.Layer.Business, action.ActionId);

                                        foreach (DE.action_params param in actionParams_List)
                                        {
                                            LogHandler.LogDebug("Fetching the Parameter details for paramID:{0} from action_params table",
                                                LogHandler.Layer.Business, param.ParamId);

                                            //assigning values into parameter Object 
                                            BE.RPParameters paramObj = new BE.RPParameters();
                                            paramObj = Translator.ActionTranslator.Param_DE_BE(action, param);

                                            //adding this parameter object to the parameter list for this action                                            
                                            params_List.Add(paramObj);
                                        }

                                        //assigning the parameter list for this action
                                        actionObj.parameters = params_List;

                                        //adding this action object to the action list for this remediationPlan                                        
                                        action_List.Add(actionObj);
                                    }
                                    else
                                    {
                                        LogHandler.LogError(String.Format(ErrorMessages.RemediatioPlan_NotFound, "List of Parameters", "action_params", "ActionId:" + action.ActionId),
                                            LogHandler.Layer.Business, null);
                                        SuperbotDataItemNotFoundException exception = new SuperbotDataItemNotFoundException(String.Format(ErrorMessages.RemediatioPlan_NotFound, "List of Parameters", "action_params", "ActionId:" + action.ActionId));
                                        List<ValidationError> validationErrors_List = new List<ValidationError>();
                                        ValidationError validationErr = new ValidationError();
                                        validationErr.Code = "1042";
                                        validationErr.Description = string.Format(ErrorMessages.RemediatioPlan_NotFound, "List of Parameters", "action_params", "ActionId:" + action.ActionId);
                                        validationErrors_List.Add(validationErr);

                                        if (validationErrors_List.Count > 0)
                                        {
                                            exception.Data.Add("DataNotFoundErrors", validationErrors_List);
                                            throw exception;
                                        }
                                    }
                                }
                                //else
                                //{
                                //    LogHandler.LogError(String.Format(ErrorMessages.RemediatioPlan_NotFound, "Action details", "action", "ActionId:" + action.ActionId),
                                //        LogHandler.Layer.Business, null);
                                //    SuperbotDataItemNotFoundException exception = new SuperbotDataItemNotFoundException(String.Format(ErrorMessages.RemediatioPlan_NotFound, "Action details", "action", "ActionId:" + action.ActionId));
                                //    List<ValidationError> validationErrors_List = new List<ValidationError>();
                                //    ValidationError validationErr = new ValidationError();
                                //    validationErr.Code = "1042";
                                //    validationErr.Description = string.Format(ErrorMessages.RemediatioPlan_NotFound, "Action details", "action", "ActionId:" + action.ActionId);
                                //    validationErrors_List.Add(validationErr);

                                //    if (validationErrors_List.Count > 0)
                                //    {
                                //        exception.Data.Add("DataNotFoundErrors", validationErrors_List);
                                //        throw exception;
                                //    }
                                //}


                            }

                            //assigning the action list for this remediationPlan
                            returnObject.actions = action_List;

                            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_End, "GetRemediationPlan", "Action"), LogHandler.Layer.Business, null);

                        }
                        else
                        {
                            LogHandler.LogError(String.Format(ErrorMessages.RemediatioPlan_NotFound, "List of Actions", "remediation_plan_action_map", "RemediationPlanId:" + remediation_Plan.RemediationPlanId),
                                LogHandler.Layer.Business, null);
                            SuperbotDataItemNotFoundException exception = new SuperbotDataItemNotFoundException(String.Format(ErrorMessages.RemediatioPlan_NotFound, "List of Actions", "remediation_plan_action_map", "RemediationPlanId:" + remediation_Plan.RemediationPlanId));
                            List<ValidationError> validationErrors_List = new List<ValidationError>();
                            ValidationError validationErr = new ValidationError();
                            validationErr.Code = "1042";
                            validationErr.Description = string.Format(ErrorMessages.RemediatioPlan_NotFound, "List of Actions", "remediation_plan_action_map", "RemediationPlanId:" + remediation_Plan.RemediationPlanId);
                            validationErrors_List.Add(validationErr);

                            if (validationErrors_List.Count > 0)
                            {
                                exception.Data.Add("DataNotFoundErrors", validationErrors_List);
                                throw exception;
                            }
                        }
                    }
                    else
                    {
                        LogHandler.LogError(String.Format(ErrorMessages.RemediatioPlan_NotFound, "RemediationPlan Details", "remediation_plan", "RemediationPlanId:" + remediationPlanId),
                            LogHandler.Layer.Business, null);
                        SuperbotDataItemNotFoundException exception = new SuperbotDataItemNotFoundException(String.Format(ErrorMessages.RemediatioPlan_NotFound, "RemediationPlan Details", "remediation_plan", "RemediationPlanId:" + remediationPlanId));
                        List<ValidationError> validationErrors_List = new List<ValidationError>();
                        ValidationError validationErr = new ValidationError();
                        validationErr.Code = "1042";
                        validationErr.Description = string.Format(ErrorMessages.RemediatioPlan_NotFound, "RemediationPlan Details", "remediation_plan", "RemediationPlanId:" + remediationPlanId);
                        validationErrors_List.Add(validationErr);

                        if (validationErrors_List.Count > 0)
                        {
                            exception.Data.Add("DataNotFoundErrors", validationErrors_List);
                            throw exception;
                        }
                    }
                }
                else
                {
                    LogHandler.LogError(String.Format(ErrorMessages.RemediatioPlan_NotFound, "RemediationPlanId", "resourcetype_observable_remediation_plan_map", "ResourceTypeId:" + anomaly.ResourceTypeId + "; ObservableId:" + anomaly.ObservableId),
                        LogHandler.Layer.Business, null);
                    SuperbotDataItemNotFoundException exception = new SuperbotDataItemNotFoundException(String.Format(ErrorMessages.RemediatioPlan_NotFound, "RemediationPlanId", "resourcetype_observable_remediation_plan_map", "ResourceTypeId:" + anomaly.ResourceTypeId + "; ObservableId:" + anomaly.ObservableId));
                    List<ValidationError> validationErrors_List = new List<ValidationError>();
                    ValidationError validationErr = new ValidationError();
                    validationErr.Code = "1042";
                    validationErr.Description = string.Format(ErrorMessages.RemediatioPlan_NotFound, "RemediationPlanId", "resourcetype_observable_remediation_plan_map", "ResourceTypeId:" + anomaly.ResourceTypeId + "; ObservableId:" + anomaly.ObservableId);
                    validationErrors_List.Add(validationErr);

                    if (validationErrors_List.Count > 0)
                    {
                        exception.Data.Add("DataNotFoundErrors", validationErrors_List);
                        throw exception;
                    }
                }
                return returnObject;

            }
            catch (Exception ex)
            {
                LogHandler.LogDebug("Execption occured in the GetRemediationPlan method of Action class in BusinessComponent with the message: {0}",
                    LogHandler.Layer.Business, ex.Message);
                throw ex;
            }

        }

        public bool RemediateIssue(BE.RemediationPlan remediationPlan, BE.Anomaly anomaly)
        {
            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_Start, "RemediateIssue", "Action"), LogHandler.Layer.Business, null);
            LogHandler.LogDebug(String.Format("Executing the RemediateIssue method of Action class with parameters: RemediationPlan={0}; anomaly={1} ", remediationPlan, anomaly),
                LogHandler.Layer.Business, null);

            try
            {
                RemediationPlanExecutionsDS remediationPlanExecutionsDS = new RemediationPlanExecutionsDS();
                AnomalyDetailsDS anomalyDetailsDS = new AnomalyDetailsDS();

                DE.remediation_plan_executions remediationPlanExecutions_obj = new DE.remediation_plan_executions
                {
                    RemediationPlanId = remediationPlan.remediationplanid,
                    ResourceId = anomaly.ResourceId,
                    ObservableId = anomaly.ObservableId,
                    ObservationId = anomaly.ObservationId,
                    NodeDetails = "NA",
                    ExecutedBy = "SUPERBOT",
                    ExecutionStartDateTime = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    Status = "In Progress",
                    CreatedBy = "SUPERBOT",
                    ModifiedBy = "SUPERBOT",
                    CreateDate = DateTime.Now,
                    TenantId = tenantId
                };                

                LogHandler.LogDebug("Inserting data into remediation_plan_executions table to get the RemediationPlanExecutionId. Data: {0}",
                    LogHandler.Layer.Business, remediationPlanExecutions_obj);
                
                remediationPlanExecutions_obj = remediationPlanExecutionsDS.Insert(remediationPlanExecutions_obj);

                if (remediationPlanExecutions_obj == null)
                {
                    LogHandler.LogWarning(String.Format(ErrorMessages.ValuesInsertionUnsuccessful, "remediation_plan_executions"), LogHandler.Layer.Business, null);
                }
                LogHandler.LogDebug("Updating the  RemediationStatus to InProgress in Observations table for the RemediationPlanExecId: {0}",
                        LogHandler.Layer.Business, remediationPlanExecutions_obj.RemediationPlanExecId);

                DE.anomaly_details anomalyDetails_Obj = new DE.anomaly_details()
                {
                    AnomalyId = anomaly.ObservationId,
                    RemediationPlanExecId = remediationPlanExecutions_obj.RemediationPlanExecId,
                    RemediationStatus = "In Progress",
                    ModifiedDate = DateTime.UtcNow,
                    ModifiedBy = "admin@123"
                };                
                
                anomalyDetails_Obj = anomalyDetailsDS.Update(anomalyDetails_Obj);

                if (anomalyDetails_Obj == null)
                {
                    LogHandler.LogWarning(ErrorMessages.ValueUpdationUnsuccessful, LogHandler.Layer.Business, null);
                }

                LogHandler.LogDebug("looping through all the actions for the RemediationplanID:{0}",
                       LogHandler.Layer.Business, remediationPlan.remediationplanid);                
                
                //getting resource Hierarchy
                List<string> resourceIdList = Helper.GetResourceHierarchy(anomaly.ResourceId,tenantId);

                //A list to store the tasks that are started for all the actions 
                List<Task> taskList = new List<Task>(remediationPlan.actions.Count);
                foreach (BE.RPActions action in remediationPlan.actions)
                {
                    bool isAsync = action.ExecutionMode == null?false: action.ExecutionMode.Equals("ASYNC",StringComparison.InvariantCultureIgnoreCase);

                    //if remediation failed and cancel is enabled
                    if (isRemediationFailed && remediationPlan.CancelIfFailed)
                        break;                    
                    else if (isAsync)
                    {
                        taskList.Add(Task.Factory.StartNew(()=>
                        {
                            ActionExecution(action, resourceIdList, anomaly, remediationPlanExecutions_obj, remediationPlan.CancelIfFailed, tokenSource);
                        }, tokenSource.Token));
                    }
                    else
                    {
                        //check if there are any async tasks running
                        if (taskList.Count > 0)
                        {
                            //enclosing in try, because the tasks in the list could be cancelled. 
                            //Waiting for cancelled task will throw error
                            try
                            {
                                //if so wait for them to complete
                                Task.WhenAll(taskList.ToArray()).Wait();
                            }
                            catch (OperationCanceledException)
                            {
                                isRemediationFailed = true;
                            }
                            
                        }
                        //if remediation failed on any async tasks ran and cancel is enabled
                        if (isRemediationFailed && remediationPlan.CancelIfFailed)
                            break;

                        //start executing synchronously
                        ActionExecution(action, resourceIdList, anomaly, remediationPlanExecutions_obj, remediationPlan.CancelIfFailed, tokenSource);
                    }                      


                }

                //wait for all the action that are running (if any) asynchronously to complete 
                try
                {
                    if (taskList.Count > 0)
                    {
                        Task.WhenAll(taskList.ToArray()).Wait();
                    }                        
                }
                catch (OperationCanceledException)
                {
                    isRemediationFailed = true;
                    //Console.WriteLine("catched OperationCanceledException exception");
                }
                finally
                {
                    tokenSource.Dispose();
                }

                if (!isRemediationFailed)
                {
                    remediationPlanExecutions_obj.Status = "Success";
                    anomalyDetails_Obj.RemediationStatus = "Success";
                    anomalyDetails_Obj.State = "Healthy";
                    anomalyDetails_Obj.ModifiedDate = DateTime.UtcNow;
                    anomalyDetails_Obj.ModifiedBy = "admin@123";

                    if (Helper.CheckNotificationRestriction(remediationPlanExecutions_obj.ResourceId, remediationPlanExecutions_obj.ObservableId, Convert.ToInt32(anomaly.PlatformId), remediationPlanExecutions_obj.TenantId))
                    {
                        #region sending notification for success scenario
                        DA.Queue.NotificationDS notificationDS = new DA.Queue.NotificationDS();
                        DE.Queue.Notification notification = new DE.Queue.Notification();
                        notification.ObservationId = remediationPlanExecutions_obj.ObservationId;
                        notification.PlatformId = Convert.ToInt32(anomaly.PlatformId);
                        notification.ResourceId = remediationPlanExecutions_obj.ResourceId;
                        notification.ResourceTypeId = anomaly.ResourceTypeId;
                        notification.ObservableId = remediationPlanExecutions_obj.ObservableId;
                        notification.ObservableName = anomaly.ObservableName;
                        notification.ObservationStatus = anomaly.ObservationStatus;
                        notification.Value = anomaly.Value;
                        notification.ThresholdExpression = anomaly.ThresholdExpression;
                        notification.ServerIp = anomaly.ServerIp;
                        notification.ObservationTime = anomaly.ObservationTime;
                        notification.Description = "";
                        notification.EventType = anomaly.EventType;
                        notification.Source = anomaly.Source;
                        notification.TenantId = remediationPlanExecutions_obj.TenantId;
                        notification.Type = (int)NotificationType.RemediationSuccess;
                        notification.Channel = (int)NotificationChannel.Email;

                        notificationDS.Send(notification, "");
                        #endregion
                    }
                }
                else
                {
                    remediationPlanExecutions_obj.Status = "FAILED";
                    anomalyDetails_Obj.RemediationStatus = "FAILED";
                    anomalyDetails_Obj.State = "Unhealthy";
                    anomalyDetails_Obj.IsNotified = "Yes";
                    anomalyDetails_Obj.NotifiedTime = DateTime.UtcNow;
                    anomalyDetails_Obj.ModifiedDate = DateTime.UtcNow;
                    anomalyDetails_Obj.ModifiedBy = "admin@123";
                }

                LogHandler.LogDebug("Updating the Remediation_plan_Executions table with status:{0}",
                LogHandler.Layer.Business, remediationPlanExecutions_obj.Status);

                if (remediationPlanExecutionsDS.Update(remediationPlanExecutions_obj) == null)
                {
                    LogHandler.LogWarning(String.Format(ErrorMessages.ValueUpdationUnsuccessful, "remediation_plan_executions"), LogHandler.Layer.Business, null);
                }

                LogHandler.LogDebug("Updating the observations table with RemediationStatus:{0}",
                LogHandler.Layer.Business, anomalyDetails_Obj.RemediationStatus);

                if (anomalyDetailsDS.Update(anomalyDetails_Obj) == null)
                {
                    LogHandler.LogWarning(String.Format(ErrorMessages.ValueUpdationUnsuccessful, "observations"), LogHandler.Layer.Business, null);
                }

                LogHandler.LogInfo("The RemediateIssue method of Action class in BusinessComponent executed successfully", LogHandler.Layer.Business, null);
                LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_End, "RemediateIssue", "Action"), LogHandler.Layer.Business, null);

                return !isRemediationFailed;

            }
            catch (Exception ex)
            {
                LogHandler.LogDebug("Exception occured in the RemediateIssue method of Action class in BusinessComponent with message:{0}",
                   LogHandler.Layer.Business, ex.Message);
                throw ex;
            }
        }       

        private void ActionExecution(BE.RPActions action,List<string> resourceIdList, BE.Anomaly anomaly, DE.remediation_plan_executions remediationPlanExecutions_obj, bool isCancellable, CancellationTokenSource tokenSource)
        {
            try
            {
                LogHandler.LogDebug("populating execution request message and parameters for actionId:{0}",
                       LogHandler.Layer.Business, action.actionid);


                List<string> auditParams = new List<string>();

                InitiateExecutionReqMsg executionReqMsg = new InitiateExecutionReqMsg();
                SE.ScriptIdentifier script = new SE.ScriptIdentifier();
                script.ScriptId = action.scriptid;
                script.CategoryId = action.categoryid;
                script.CompanyId = tenantId;
                List<SE.Parameter> parameter_List = new List<SE.Parameter>();

                ActionParamsDS actionParamsDS = new ActionParamsDS();
                ResourceAttributesDS resourceAttributesDS = new ResourceAttributesDS();
                var resourceAttributTable = (from resAtt in resourceAttributesDS.GetAny() select resAtt).ToList();
                foreach (BE.RPParameters param in action.parameters)
                {
                    LogHandler.LogDebug("Fetching data from action_params table for paramID:{0}",
                        LogHandler.Layer.Business, param.paramid);

                    DE.action_params actionParams = new DE.action_params()
                    {
                        ParamId = Convert.ToInt32(param.paramid)
                    };
                    actionParams = actionParamsDS.GetOne(actionParams);

                    if (actionParams != null)
                    {
                        string paramName = param.name;
                        string paramVal = string.Empty;

                        if (actionParams.FieldToMap.Contains("resource_attribute.AttributeValue"))
                        {
                            LogHandler.LogDebug("Accessing the resource_attributes table to get the attribute value with ResourceId:{0}; AttributeName:{1} for paramId:{2}",
                                LogHandler.Layer.Business, anomaly.ResourceId, param.name, param.paramid);

                            foreach (var resId in resourceIdList)
                            {
                                var res = (from resAtt in resourceAttributTable
                                           where resAtt.ResourceId == resId
                                           && resAtt.AttributeName == param.name
                                           select resAtt.AttributeValue).FirstOrDefault();
                                if (res != "" && res != null)
                                {
                                    paramVal = res;
                                    break;
                                }
                            }

                            if (paramVal == string.Empty)
                            {
                                LogHandler.LogError(String.Format(ErrorMessages.RemediatioPlan_NotFound, "Resource details", "resource_attributes", "ResourceId:" + anomaly.ResourceId + "; Attribute Name:" + param.name),
                                    LogHandler.Layer.Business, null);
                                SuperbotDataItemNotFoundException exception = new SuperbotDataItemNotFoundException(String.Format(ErrorMessages.RemediatioPlan_NotFound, "Resource details", "resource_attributes", "ResourceId:" + anomaly.ResourceId + "; Attribute Name:" + param.name));
                                List<ValidationError> validationErrors_List = new List<ValidationError>();
                                ValidationError validationErr = new ValidationError();
                                validationErr.Code = "1042";
                                validationErr.Description = string.Format(ErrorMessages.RemediatioPlan_NotFound, "Resource details", "resource_attributes", "ResourceId:" + anomaly.ResourceId + "; Attribute Name:" + param.name);
                                validationErrors_List.Add(validationErr);

                                if (validationErrors_List.Count > 0)
                                {
                                    exception.Data.Add("DataNotFoundErrors", validationErrors_List);
                                    throw exception;
                                }
                            }
                            else if (actionParams.ParamType != null && actionParams.ParamType.ToLower().Trim().Equals("template"))
                            {
                                paramVal = GetDetailJson(paramVal, anomaly);
                            }

                        }
                        else
                        {
                            paramVal = actionParams.FieldToMap;
                            if (actionParams.ParamType != null && actionParams.ParamType.ToLower().Trim().Equals("template"))
                            {
                                paramVal = GetDetailJson(paramVal, anomaly);
                            }
                        }

                        //newly added code for remote details
                        if (string.Equals(paramName, "RemoteServerNames", StringComparison.InvariantCultureIgnoreCase))
                        {
                            script.RemoteServerNames = paramVal;
                        }
                        else if (string.Equals(paramName, "RemoteUserName", StringComparison.InvariantCultureIgnoreCase))
                        {
                            script.UserName = paramVal;
                        }
                        else if (string.Equals(paramName, "RemotePassword", StringComparison.InvariantCultureIgnoreCase))
                        {
                            script.Password = paramVal;
                        }
                        else if (string.Equals(paramName, "RemoteExecutionMode", StringComparison.InvariantCultureIgnoreCase))
                        {
                            script.RemoteExecutionMode = Convert.ToInt32(paramVal);
                        }
                        else
                        {
                            LogHandler.LogDebug("populating parameter properties for paramId:{0} for the actionId:{1}",
                                LogHandler.Layer.Business, param.paramid, action.actionid);

                            SE.Parameter paramObj = new SE.Parameter();
                            paramObj.ParameterName = paramName;
                            paramObj.ParameterValue = paramVal;
                            parameter_List.Add(paramObj);
                        }

                        auditParams.Add(paramName + "=" + paramVal);
                    }
                    else
                    {
                        LogHandler.LogError(String.Format(ErrorMessages.RemediatioPlan_NotFound, "Parameter details", "action_params", "ParamId:" + param.paramid),
                            LogHandler.Layer.Business, null);
                        SuperbotDataItemNotFoundException exception = new SuperbotDataItemNotFoundException(String.Format(ErrorMessages.RemediatioPlan_NotFound, "Parameter", "action_params", "ParamId:" + param.paramid));
                        List<ValidationError> validationErrors_List = new List<ValidationError>();
                        ValidationError validationErr = new ValidationError();
                        validationErr.Code = "1042";
                        validationErr.Description = string.Format(ErrorMessages.RemediatioPlan_NotFound, "Parameter", "action_params", "ParamId:" + param.paramid);
                        validationErrors_List.Add(validationErr);

                        if (validationErrors_List.Count > 0)
                        {
                            exception.Data.Add("DataNotFoundErrors", validationErrors_List);
                            throw exception;
                        }
                    }

                }

                script.Parameters = parameter_List;

                executionReqMsg.ScriptIdentifier = script;
                LogHandler.LogDebug("populated the parameters for Execution request message for actionId:{0}",
                    LogHandler.Layer.Business, action.actionid);

                LogHandler.LogDebug("calling ServiceChannel method from scriptExecute class",
                    LogHandler.Layer.Business, null);
                ScriptExecute scriptExecute = new ScriptExecute();
                var channel1 = scriptExecute.ServiceChannel;

                LogHandler.LogDebug("Calling SEE for actionId:{0} with parameters:{1}",
                    LogHandler.Layer.Business, action.actionid, executionReqMsg);

                if (tokenSource.Token.IsCancellationRequested)
                    throw new OperationCanceledException();

                InitiateExecutionResMsg response = channel1.AsyncInitiateExecution(executionReqMsg).Result;
                

                #region LogAudit
                BE.AuditLog auditLogObj = new BE.AuditLog();
                auditLogObj.AnomalyId = anomaly.ObservationId;
                auditLogObj.ResourceID = anomaly.ResourceId;
                auditLogObj.ObservableID = anomaly.ObservableId;
                auditLogObj.ActionID = action.actionid;
                auditLogObj.ActionParams = string.Join(",", auditParams);
                auditLogObj.PlatformID = Convert.ToInt32(anomaly.PlatformId);
                auditLogObj.TenantID = tenantId;
                #endregion

                if (response != null && isRemediationFailed == false)
                {
                    if (!CheckSEEResponse(response))
                    {
                        //SEE FAILED
                        LogHandler.LogDebug(String.Format("The SEE execution failed with response :{0} ", response),
                         LogHandler.Layer.Business, null);


                        auditLogObj.Status = "FAILED";
                        auditLogObj.Output = response.ScriptResponse.FirstOrDefault().ErrorMessage != null ? response.ScriptResponse.FirstOrDefault().ErrorMessage : response.ScriptResponse.FirstOrDefault().SuccessMessage;

                        Audit_Log audit_Log_obj = new Audit_Log();
                        var isSuccess = audit_Log_obj.LogAuditData(auditLogObj);

                        

                        if (Helper.CheckNotificationRestriction(remediationPlanExecutions_obj.ResourceId, remediationPlanExecutions_obj.ObservableId, Convert.ToInt32(anomaly.PlatformId), remediationPlanExecutions_obj.TenantId))
                        {
                            DA.Queue.NotificationDS notificationDS = new DA.Queue.NotificationDS();
                            DE.Queue.Notification notification = new DE.Queue.Notification();
                            notification.ObservationId = remediationPlanExecutions_obj.ObservationId;
                            notification.PlatformId = Convert.ToInt32(anomaly.PlatformId);
                            notification.ResourceId = remediationPlanExecutions_obj.ResourceId;
                            notification.ResourceTypeId = anomaly.ResourceTypeId;
                            notification.ObservableId = remediationPlanExecutions_obj.ObservableId;
                            notification.ObservableName = anomaly.ObservableName;
                            notification.ObservationStatus = anomaly.ObservationStatus;
                            notification.Value = anomaly.Value;
                            notification.ThresholdExpression = anomaly.ThresholdExpression;
                            notification.ServerIp = anomaly.ServerIp;
                            notification.ObservationTime = anomaly.ObservationTime;
                            if (response.ScriptResponse.FirstOrDefault().ErrorMessage != "" && response.ScriptResponse.FirstOrDefault().ErrorMessage != null)
                            {
                                notification.Description = response.ScriptResponse.FirstOrDefault().ErrorMessage;
                            }
                            else
                            {
                                if (response.ScriptResponse.FirstOrDefault().SuccessMessage.Contains("error=") || response.ScriptResponse.FirstOrDefault().SuccessMessage.Contains("[ERROR002]: []:"))
                                {
                                    notification.Description = response.ScriptResponse.FirstOrDefault().SuccessMessage.Split(new[] { "[ERROR002]: []:", "error=" }, StringSplitOptions.None)[1];
                                }

                            }

                            notification.EventType = anomaly.EventType;
                            notification.Source = anomaly.Source;
                            notification.TenantId = remediationPlanExecutions_obj.TenantId;
                            notification.Type = (int)NotificationType.RemediationFailure;
                            notification.Channel = (int)NotificationChannel.Email;
                            notification.BaseURL = response.ScriptResponse.FirstOrDefault().ErrorMessage != null ? response.ScriptResponse.FirstOrDefault().ErrorMessage : response.ScriptResponse.FirstOrDefault().SuccessMessage;

                            notificationDS.Send(notification, "");
                        }

                        isRemediationFailed = true;
                        if (isCancellable)
                        {
                            if (!tokenSource.Token.IsCancellationRequested)
                                tokenSource.Cancel(); // cancelling other running tasks
                        }                           

                        
                    }
                    else
                    {
                        // SEE SUCCESS
                        auditLogObj.Status = "SUCCESS";
                        auditLogObj.Output = response.ScriptResponse.FirstOrDefault().SuccessMessage;
                        Audit_Log audit_Log_obj = new Audit_Log();
                        var isSuccess = audit_Log_obj.LogAuditData(auditLogObj);
                    }
                    

                    #region Remediation plan execution entry
                    DE.remediation_plan_execution_actions execActionObj = new DE.remediation_plan_execution_actions();
                    execActionObj.RemediationPlanExecId = remediationPlanExecutions_obj.RemediationPlanExecId;
                    execActionObj.RemediationPlanActionId = action.actionid;
                    execActionObj.CorrelationId = response.ScriptResponse.FirstOrDefault().TransactionId;

                    if (response != null && response.ScriptResponse.FirstOrDefault().IsSuccess)
                    {
                        execActionObj.Output = response.ScriptResponse.FirstOrDefault().SuccessMessage.Length >= 2000 ? response.ScriptResponse.FirstOrDefault().SuccessMessage.Substring(0, 100) : response.ScriptResponse.FirstOrDefault().SuccessMessage;

                    }
                    else
                    {
                        execActionObj.Output = response.ScriptResponse.FirstOrDefault().ErrorMessage.Length >= 2000 ? response.ScriptResponse.FirstOrDefault().ErrorMessage.Substring(0, 100) : response.ScriptResponse.FirstOrDefault().ErrorMessage;
                        //flag = false;
                    }


                    //execActionObj.Status = response.ScriptResponse.FirstOrDefault().Status;
                    if (response.ScriptResponse.FirstOrDefault().LogData == null)
                    {
                        execActionObj.Logdata = "Log Data";
                    }
                    else
                    {
                        execActionObj.Logdata = response.ScriptResponse.FirstOrDefault().LogData;
                    }
                    if (response.ScriptResponse.FirstOrDefault().Status == null)
                    {
                        execActionObj.Status = "status";
                    }
                    else
                    {
                        execActionObj.Status = response.ScriptResponse.FirstOrDefault().Status;
                    }
                    execActionObj.OrchestratorDetails = "SuperBot";
                    execActionObj.CreatedBy = remediationPlanExecutions_obj.CreatedBy;
                    execActionObj.ModifiedBy = remediationPlanExecutions_obj.ModifiedBy;
                    execActionObj.CreateDate = DateTime.UtcNow;
                    execActionObj.ModifiedDate = DateTime.UtcNow;
                    execActionObj.TenantId = remediationPlanExecutions_obj.TenantId;

                    LogHandler.LogDebug("Inserting the data into remediation_plan_execution_actions table.  Data:{0} ",
                        LogHandler.Layer.Business, execActionObj);

                    RemediationPlanExecutionActionsDS remediationPlanExecutionActionsDS = new RemediationPlanExecutionActionsDS();

                    if (remediationPlanExecutionActionsDS.Insert(execActionObj) == null)
                    {
                        LogHandler.LogWarning(String.Format(ErrorMessages.ValuesInsertionUnsuccessful, "remediation_plan_execution_actions"), LogHandler.Layer.Business, null);
                    }

                    #endregion
                }
                else
                {
                    LogHandler.LogError(String.Format(ErrorMessages.SEE_Response_Null, action.actionid, executionReqMsg),
                                    LogHandler.Layer.Business, null);

                    #region LoggingAudit
                    auditLogObj.Status = "FAILED";
                    auditLogObj.Output = "SEE returned NULL";

                    Audit_Log audit_Log_obj = new Audit_Log();
                    var isSuccess = audit_Log_obj.LogAuditData(auditLogObj);
                    #endregion

                    
                    isRemediationFailed = true;
                    if (isCancellable)
                    {
                        if (!tokenSource.Token.IsCancellationRequested)
                            tokenSource.Cancel(); // cancelling other running tasks  
                    }

                    //SuperbotDataItemNotFoundException exception = new SuperbotDataItemNotFoundException(String.Format(ErrorMessages.SEE_Response_Null, action.actionid, executionReqMsg));
                    //List<ValidationError> validationErrors_List = new List<ValidationError>();
                    //ValidationError validationErr = new ValidationError();
                    //validationErr.Code = "1043";
                    //validationErr.Description = ;
                    //validationErrors_List.Add(validationErr);

                    //if (validationErrors_List.Count > 0)
                    //{
                    //    exception.Data.Add("SEEResponseNullError", validationErrors_List);

                        
                    //    throw exception;
                    //}
                }
            }
            catch (Exception ex)
            {
                //stops all the upcoming actions from execution
                isRemediationFailed = true;

                LogHandler.LogError(String.Format(ex.Message),LogHandler.Layer.Business, null);
            }
            
        }

        public bool CheckSEEResponse(InitiateExecutionResMsg response)
        {
            bool status = false;            
            if (response.ScriptResponse.FirstOrDefault().IsSuccess)
            {
                string successmsg = response.ScriptResponse.FirstOrDefault().SuccessMessage.ToLower();
                if (successmsg.Contains("status=success") || successmsg.Contains("error=no error found") || !successmsg.Contains("status=failure"))
                {
                    if (!successmsg.Contains("exception occurred"))
                        status = true;
                    else
                        status = false;
                }
                
                else
                {
                    status = false;
                }
            }
            return status;
        }
        
        
        public string GetDetailJson(string inputJson, BE.Anomaly anomaly)
        {
            string dbString;
            using (StreamReader sr = new StreamReader(AppDomain.CurrentDomain.BaseDirectory + inputJson))
            {
                dbString = sr.ReadToEnd();
            }
            Infrastructure.ServiceClientLibrary.SuperBot resourceHandler = new Infrastructure.ServiceClientLibrary.SuperBot();
            var channel = resourceHandler.ServiceChannel;

            StringBuilder sb = new StringBuilder(dbString);

            var observationDetails = channel.GetObservationsDetails(anomaly.ObservationId, Convert.ToInt32(anomaly.PlatformId), tenantId);
            var resourceDetails = channel.GetResourceDetails(anomaly.ResourceId, tenantId, Convert.ToInt32(anomaly.PlatformId));

            String[] spearator = { "Anomaly found." };
            Int32 count = 2;
            String[] strlist = observationDetails.Description.Split(spearator, count,
                  StringSplitOptions.RemoveEmptyEntries);

            string anomalyReason = "The " + observationDetails.ObservableName + strlist[1];

            string resourceTypeName = resourceDetails.ResourceTypeName;
            string resourceName = resourceDetails.ResourceName;
            string observableName = observationDetails.ObservableName;
            List<string> detailsList = new NotificationBuilder().GetAuditLogOutputAttachments(anomaly.ObservationId);
                       

            string details = string.Join("", detailsList.ToArray());
            
            string returnString = sb.Replace("##ANOMALY_REASON##", anomalyReason)
                                    .Replace("##RESOURCETYPENAME##", resourceTypeName)
                                    .Replace("##RESOURCENAME##", resourceName)
                                    .Replace("##OBSERVABLENAME##", observableName)
                                    .Replace("##DETAILS##", details).ToString();
            
            return returnString;
        }
    }
}
