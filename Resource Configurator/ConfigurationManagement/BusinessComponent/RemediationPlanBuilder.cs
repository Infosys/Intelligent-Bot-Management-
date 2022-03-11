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
    public class RemediationPlanBuilder
    {


        public BE.RemediationPlanDetails getRemediationPlanDetails(int TenentID)
        {
            RemediationPlanDS remediationPlanDS = new RemediationPlanDS();
            RemediationPlanActionMapDS remediationPlanActionMapDS = new RemediationPlanActionMapDS();
            ActionDS action = new ActionDS();

            try
            {
                var RemediationPlanList = (from a in remediationPlanDS.GetAll().ToArray()
                                           join b in remediationPlanActionMapDS.GetAll().ToArray() on a.RemediationPlanId equals b.RemediationPlanId
                                           join c in action.GetAll().ToArray() on b.ActionId equals c.ActionId
                                           where b.TenantId == c.TenantId
                                           && a.isDeleted == false && c.IsDeleted == false
                                          && a.TenantId == TenentID

                                           select new
                                           {
                                               a.RemediationPlanId,
                                               a.RemediationPlanName,
                                               a.RemediationPlanDescription,
                                               a.IsUserDefined,
                                               b.ActionId,
                                               c.ActionName,
                                               b.ActionSequence,
                                               b.ActionStageId,
                                               b.RemediationPlanActionId

                                           }).ToList();

                List<BE.RemediationPlan> remediationPlanDetailsList = new List<BE.RemediationPlan>();
                remediationPlanDetailsList = (from o in RemediationPlanList
                                              group o by new { o.RemediationPlanId, o.RemediationPlanName, o.RemediationPlanDescription, o.IsUserDefined } into g
                                              select new BE.RemediationPlan
                                              {
                                                  remediationId = g.Key.RemediationPlanId,
                                                  remediationPlanName = g.Key.RemediationPlanName,
                                                  RemediationPlanDescription = g.Key.RemediationPlanDescription,

                                                  IsUserDefined = g.Key.IsUserDefined,
                                                  ActionDetails = (from actDetails in g
                                                                   orderby actDetails.ActionSequence
                                                                   select new BE.ActionRemdiation
                                                                   {
                                                                       ActionId = actDetails.ActionId,
                                                                       ActionStageId = actDetails.ActionStageId,
                                                                       ActionSequence = actDetails.ActionSequence,
                                                                       ActionName = actDetails.ActionName,
                                                                       RemediationPlanActionId = actDetails.RemediationPlanActionId
                                                                        


                                                                   } ).ToList()


                                              }).ToList();

                BE.RemediationPlanDetails rem = new BE.RemediationPlanDetails();
                rem.tenantId = TenentID;
                rem.RemediationPlans = remediationPlanDetailsList;
                return rem;


            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    

    public string insertRemediationPlanDetails(BE.RemediationPlanDetails remediationPlanDetails)
        {
            StringBuilder responseMessage = new StringBuilder();
            RemediationPlanDS remediationPlanDS = new RemediationPlanDS();
            RemediationPlanActionMapDS remediationPlanActionMapDS = new RemediationPlanActionMapDS();
            ActionDS action = new ActionDS();
            try
            {
                foreach (BE.RemediationPlan remPlan in remediationPlanDetails.RemediationPlans)
                {
                    remediation_plan rem = new remediation_plan();
                    rem.RemediationPlanId = remPlan.remediationId;
                    rem.RemediationPlanName = remPlan.remediationPlanName;
                    rem.RemediationPlanDescription = remPlan.RemediationPlanDescription;
                    rem.IsUserDefined = remPlan.IsUserDefined;
                    rem.TenantId = remediationPlanDetails.tenantId;
                    rem.isDeleted = false;
                    var res1 = remediationPlanDS.Insert(rem);
                    responseMessage.Append(res1 == null ? "\n Insertion of RemediationPlan data failed " : "\n Insertion of RemediationPlan data success ");
                    if (remPlan.ActionDetails != null)
                    {
                        foreach (BE.ActionRemdiation actRemedation in remPlan.ActionDetails)
                        {

                            remediation_plan_action_map remActionMap = new remediation_plan_action_map();
                            remActionMap.ActionId = actRemedation.ActionId;
                            remActionMap.ActionSequence = actRemedation.ActionSequence;
                            remActionMap.ActionStageId = actRemedation.ActionStageId;
                            remActionMap.RemediationPlanId = res1.RemediationPlanId;
                            remActionMap.RemediationPlanActionId = actRemedation.RemediationPlanActionId;
                            remActionMap.TenantId = remediationPlanDetails.tenantId;
                            var res2 = remediationPlanActionMapDS.Insert(remActionMap);
                            responseMessage.Append(res2 == null ? "\n Insertion of RemediationPlanActionMap data failed " : "\n Insertion of RemediationPlanActionMap data success ");
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


        public string updateRemediationPlanDetails(BE.RemediationPlanDetails remediationPlanDetails)
        {
            StringBuilder responseMessage = new StringBuilder();
            RemediationPlanDS remediationPlanDS = new RemediationPlanDS();
            RemediationPlanActionMapDS remediationPlanActionMapDS = new RemediationPlanActionMapDS();
            ActionDS action = new ActionDS();
            IList<remediation_plan> remPlanList = new List<remediation_plan>();
            IList<remediation_plan_action_map> remPlanActionList = new List<remediation_plan_action_map>();
            try
            {
                if(remediationPlanDetails != null) { 
                foreach (BE.RemediationPlan remPlan in remediationPlanDetails.RemediationPlans)
                {
                    remediation_plan rem = new remediation_plan();
                    rem.RemediationPlanId = remPlan.remediationId;
                    rem.RemediationPlanName = remPlan.remediationPlanName;
                    rem.RemediationPlanDescription = remPlan.RemediationPlanDescription;
                    rem.IsUserDefined = remPlan.IsUserDefined;
                    var res = remediationPlanDS.Update(rem);
                    responseMessage.Append(res == null ? "\n Updation of RemediationPlan data failed " : "\n Updation of RemediationPlan data success ");

                    
                    // var res1 = remediationPlanDS.Insert(rem);
                    foreach (BE.ActionRemdiation actRemedation in remPlan.ActionDetails)
                    {
                        var remPlanActionId = (from o in remediationPlanActionMapDS.GetAll().ToArray()
                                               where o.RemediationPlanId == rem.RemediationPlanId && o.ActionId == actRemedation.ActionId
                                              select o.RemediationPlanActionId).FirstOrDefault();
                        if(remPlanActionId == 0 )
                            {
                                remediation_plan_action_map remActionMap = new remediation_plan_action_map();
                                remActionMap.ActionId = actRemedation.ActionId;
                                remActionMap.ActionSequence = actRemedation.ActionSequence;
                                remActionMap.ActionStageId = actRemedation.ActionStageId;
                                remActionMap.RemediationPlanId = res.RemediationPlanId;
                                remActionMap.RemediationPlanActionId = remPlanActionId;
                                remActionMap.TenantId = remediationPlanDetails.tenantId;

                                var res3 = remediationPlanActionMapDS.Insert(remActionMap);
                                responseMessage.Append(res3 == null ? "\n Insertion of RemediationPlanActionMap data failed " : "\n Insertion of RemediationPlanActionMap data success ");
                            }
                            else
                            {
                                remediation_plan_action_map remActionMap = new remediation_plan_action_map();
                                remActionMap.ActionId = actRemedation.ActionId;
                                remActionMap.ActionSequence = actRemedation.ActionSequence;
                                remActionMap.ActionStageId = actRemedation.ActionStageId;
                                remActionMap.RemediationPlanId = res.RemediationPlanId;
                                remActionMap.RemediationPlanActionId = remPlanActionId;
                                
                                //var res2 = remediationPlanActionMapDS.Insert(remActionMap);
                                //responseMessage.Append(res2 == null ? "\n Insertion of RemediationPlanActionMap data failed " : "\n Insertion of RemediationPlanActionMap data success ");
                                remPlanActionList.Add(remActionMap);

                                if(actRemedation.isDeleted == true)
                                {
                                    var res3 = remediationPlanActionMapDS.Delete(remActionMap);
                                }
                            }
                        
                        

                    }

                    var res2 = remediationPlanActionMapDS.UpdateBatch(remPlanActionList);
                    responseMessage.Append(res2 == null ? "\n Updation of RemediationPlanActionMap data failed " : "\n Updation of RemediationPlanActionMap data success ");

                }

                }



            }
            catch(Exception ex)
            {
                throw ex;
            }



            return responseMessage.ToString();

        }



        public string deleteRemediationPlan(BE.RemediationPlanDetails remediationPlanDetails)
        {
            StringBuilder responseMessage = new StringBuilder();
            RemediationPlanDS remediationPlanDS = new RemediationPlanDS();
            RemediationPlanActionMapDS remediationPlanActionMapDS = new RemediationPlanActionMapDS();
            ActionDS action = new ActionDS();
           
            try
            {
                if(remediationPlanDetails != null)
                {
                    IList<remediation_plan> remPlanList = new List<remediation_plan>();
                    IList<remediation_plan_action_map> remPlanActionList = new List<remediation_plan_action_map>();
                    foreach (BE.RemediationPlan remPlan in remediationPlanDetails.RemediationPlans)
                    {  
                        
                        remediation_plan rem = new remediation_plan();
                        rem.RemediationPlanId = remPlan.remediationId;
                        rem.RemediationPlanName = remPlan.remediationPlanName;
                        rem.RemediationPlanDescription = remPlan.RemediationPlanDescription;
                        rem.IsUserDefined = remPlan.IsUserDefined;
                        rem.isDeleted = true;
                        rem.TenantId = remediationPlanDetails.tenantId;
                        var res = remediationPlanDS.Update(rem);
                        responseMessage.Append(res == null ? "\n Updation of RemediationPlan data failed " : "\n Updation of RemediationPlan data success ");

                        foreach(BE.ActionRemdiation actRemedation in remPlan.ActionDetails)
                        {
                            var remPlanActionId = (from o in remediationPlanActionMapDS.GetAll().ToArray()
                                                   where o.RemediationPlanId == rem.RemediationPlanId && o.ActionId == actRemedation.ActionId
                                                   select o.RemediationPlanActionId).FirstOrDefault();
                            if (actRemedation.isDeleted == true)
                            {
                                remediation_plan_action_map remActionMap = new remediation_plan_action_map();
                                remActionMap.ActionId = actRemedation.ActionId;
                                remActionMap.ActionSequence = actRemedation.ActionSequence;
                                remActionMap.ActionStageId = actRemedation.ActionStageId;
                                remActionMap.RemediationPlanId = res.RemediationPlanId;
                                remActionMap.RemediationPlanActionId = remPlanActionId;
                                remActionMap.TenantId = remediationPlanDetails.tenantId;
                                var res3 = remediationPlanActionMapDS.Delete(remActionMap);
                            }
                        }
                    }
                    }

            }
            catch(Exception ex)
            {

                throw ex;
            }
            return responseMessage.ToString();
        }


        





    }
}
