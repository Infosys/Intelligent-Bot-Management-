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
using Infosys.Solutions.Ainauto.BusinessEntity;
using DE = Infosys.Solutions.Superbot.Resource.Entity;


namespace Infosys.Solutions.Ainauto.Superbot.BusinessComponent.Translator
{
    public class ActionTranslator
    {
        //public static BusinessEntity.RemediationPlan Translator_DE_BE(Resource.DataAccess.Models.RemediationPlan remediationPlan_DE)        
        //{
        //    BusinessEntity.RemediationPlan remediationPlan_BE = new BusinessEntity.RemediationPlan();
        //    remediationPlan_BE.remediationplanid = remediationPlan_DE.remediationplanid;
        //    remediationPlan_BE.remediationplanname = remediationPlan_DE.remediationplanname;
        //    foreach (Resource.DataAccess.Models.Actions actions in remediationPlan_DE.actions)
        //    {
        //        BusinessEntity.Actions action_obj = new BusinessEntity.Actions();
        //        action_obj.remediationplanactionid = actions.remediationplanactionid;
        //        action_obj.actionid = actions.actionid;
        //        action_obj.actionname = actions.actionname;
        //        action_obj.scriptid = actions.scriptid;
        //        action_obj.categoryid = actions.categoryid;
        //        action_obj.actionstageid = actions.actionstageid;

        //        foreach (Resource.DataAccess.Models.Parameters param in actions.parameters)
        //        {
        //            BusinessEntity.Parameters param_obj = new BusinessEntity.Parameters();
        //            param_obj.remediationplanactionid = param.remediationplanactionid;
        //            param_obj.paramid = param.paramid;
        //            param_obj.name = param.name;
        //            param_obj.ismandatory = param.ismandatory;
        //            param_obj.defaultvalue = param.defaultvalue;
        //            param_obj.providedvalue = param.providedvalue;
        //            param_obj.isfield = param.isfield;

        //            action_obj.parameters.Add(param_obj);
        //        }
        //        remediationPlan_BE.actions.Add(action_obj);
        //    }
        //    return remediationPlan_BE;
        //}

        public static RemediationPlan RemediationPlan_DE_BE(DE.remediation_plan remediationPlan_DE)
        {
            RemediationPlan remediationPlan_BE = new RemediationPlan();
            remediationPlan_BE.remediationplanid = remediationPlan_DE.RemediationPlanId;
            remediationPlan_BE.remediationplanname = remediationPlan_DE.RemediationPlanName;
            remediationPlan_BE.CancelIfFailed = Convert.ToBoolean(remediationPlan_DE.CancelIfFailed);
            return remediationPlan_BE;
        }

        public static RPParameters Param_DE_BE(DE.remediation_plan_action_map actionObj_DE, DE.action_params paramObj_DE)
        {
            RPParameters paramObj_BE = new RPParameters();
            paramObj_BE.remediationplanactionid = actionObj_DE.RemediationPlanActionId;
            paramObj_BE.providedvalue = "Provided Value";
            paramObj_BE.isfield = true;
            paramObj_BE.paramid = paramObj_DE.ParamId;
            paramObj_BE.name = paramObj_DE.Name;
            paramObj_BE.ismandatory = paramObj_DE.IsMandatory;
            paramObj_BE.defaultvalue = paramObj_DE.DefaultValue;            
            
            return paramObj_BE;
        }
        
        public static RPActions Actions_DE_BE(DE.remediation_plan_action_map action_DE,DE.action actionObj_DE)
        {
            RPActions actionsObj_BE = new RPActions();

            actionsObj_BE.remediationplanactionid = action_DE.RemediationPlanActionId;
            actionsObj_BE.actionid = action_DE.ActionId;
            actionsObj_BE.actionstageid = action_DE.ActionStageId;
            actionsObj_BE.actionname = actionObj_DE.ActionName;
            actionsObj_BE.scriptid = (int)actionObj_DE.ScriptId;
            actionsObj_BE.categoryid = (int)actionObj_DE.CategoryId;
            actionsObj_BE.ExecutionMode = action_DE.ExecutionMode;
            return actionsObj_BE;
        }
    }
}
