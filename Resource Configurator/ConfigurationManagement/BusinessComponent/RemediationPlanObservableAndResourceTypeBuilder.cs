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
    public class RemediationPlanObservableAndResourceTypeBuilder
    {
        public BE.RemediationPlanObservableAndResourceTypeMap getRemediationPlanObservableAndResourceTypeDetails(int tenantId)
        {
            ResourcetypeObservableRemediationPlanMapDS rDS = new ResourcetypeObservableRemediationPlanMapDS();
            ResourceTypeDS rType = new ResourceTypeDS();
            ObservableDS obsDs = new ObservableDS();
            RemediationPlanDS rPlanDs = new RemediationPlanDS();
            try
            {
                var remPlan = (from a in rDS.GetAll().ToArray()
                                   join b in rType.GetAll().ToArray() on a.ResourceTypeId equals b.ResourceTypeId
                                   join c in obsDs.GetAll().ToArray() on a.ObservableId equals c.ObservableId
                                   join d in rPlanDs.GetAll().ToArray() on a.RemediationPlanId equals d.RemediationPlanId
                                   where a.TenantId == b.TenantId &&
                                   a.TenantId == c.TenantId &&
                                   a.TenantId == d.TenantId &&
                                   a.TenantId == tenantId &&
                                  // b.PlatformId == platformId &&
                                   a.ValidityEnd > DateTime.Today

                                   select new BE.RemediationPlanObservableAndResourceType
                                   {
                                       resourcetypename = b.ResourceTypeName ,
                                       resourcetypeid = a.ResourceTypeId ,
                                       ObservableId = a.ObservableId ,
                                       ObservableName = c.ObservableName ,
                                       RemediationPlanId = a.RemediationPlanId ,
                                       RemediationPlanName = d.RemediationPlanName ,
                                       CreateDate = a.CreateDate ,
                                       CreatedBy = a.CreatedBy ,
                                       ModifiedBy = a.ModifiedBy ,
                                       ModifiedDate = Convert.ToDateTime(a.ModifiedDate), 
                                       ValidityStart = Convert.ToString(a.ValidityStart),
                                       ValidityEnd = Convert.ToString(a.ValidityEnd)
                                   }).ToList();

                List<BE.RemediationPlanObservableAndResourceType> remPlanList = new List<BE.RemediationPlanObservableAndResourceType>();
                remPlanList = (from o in remPlan
                               group o by new
                               {
                                   o.resourcetypename,
                                   o.resourcetypeid,
                                   o.ObservableId,
                                   o.ObservableName,
                                   o.RemediationPlanId,
                                   o.RemediationPlanName,
                                   o.CreateDate,
                                   o.CreatedBy,
                                   o.ModifiedBy,
                                   o.ModifiedDate,
                                   o.ValidityEnd,
                                   o.ValidityStart
                               } into g
                               select new BE.RemediationPlanObservableAndResourceType
                               {
                                   resourcetypename = g.Key.resourcetypename,
                                   resourcetypeid = g.Key.resourcetypeid,
                                   ObservableId = g.Key.ObservableId,
                                   ObservableName = g.Key.ObservableName,
                                   RemediationPlanId = g.Key.RemediationPlanId,
                                   RemediationPlanName = g.Key.RemediationPlanName,
                                   CreateDate = g.Key.CreateDate,
                                   CreatedBy = g.Key.CreatedBy,
                                   ModifiedBy = g.Key.ModifiedBy,
                                   ModifiedDate = g.Key.ModifiedDate,
                                   ValidityStart = g.Key.ValidityStart,
                                   ValidityEnd = g.Key.ValidityEnd

                              }).ToList();


                BE.RemediationPlanObservableAndResourceTypeMap rMap = new BE.RemediationPlanObservableAndResourceTypeMap();
                rMap.tenantid = tenantId;
               // rMap.platformid = platformId;
                rMap.RemediationPlanObservableAndResourceTypeList = remPlanList;
                return rMap;
            }
            catch(Exception ex)
            {
                throw ex;
            }


            
        }

        public string insertRemediationPlanObservableAndResourceTypeDetails(BE.RemediationPlanObservableAndResourceTypeMap remPlan)
        {
            StringBuilder responseMessage = new StringBuilder();
            ResourcetypeObservableRemediationPlanMapDS rDS = new ResourcetypeObservableRemediationPlanMapDS();
            ResourceTypeDS rType = new ResourceTypeDS();
            ObservableDS obsDs = new ObservableDS();
            RemediationPlanDS rPlanDs = new RemediationPlanDS();
            try
            {
                
                foreach(BE.RemediationPlanObservableAndResourceType rem in remPlan.RemediationPlanObservableAndResourceTypeList)
                {
                    resourcetype_observable_remediation_plan_map remObsPlan = new resourcetype_observable_remediation_plan_map();
                    remObsPlan.ResourceTypeId = rem.resourcetypeid;
                    remObsPlan.ObservableId = rem.ObservableId;
                    remObsPlan.RemediationPlanId = rem.RemediationPlanId;
                    remObsPlan.CreatedBy = rem.CreatedBy;
                    remObsPlan.CreateDate = rem.CreateDate;
                    remObsPlan.ModifiedBy = rem.ModifiedBy;
                    remObsPlan.ModifiedDate = rem.ModifiedDate;
                    remObsPlan.TenantId = remPlan.tenantid;
                    remObsPlan.ValidityStart = rem.ValidityStart.Contains("India Standard Time") ? DateTime.ParseExact(rem.ValidityStart, "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(India Standard Time)'", System.Globalization.CultureInfo.InvariantCulture) : Convert.ToDateTime(rem.ValidityStart);
                    remObsPlan.ValidityEnd = rem.ValidityEnd.Contains("India Standard Time") ? DateTime.ParseExact(rem.ValidityEnd, "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(India Standard Time)'", System.Globalization.CultureInfo.InvariantCulture) : Convert.ToDateTime(rem.ValidityEnd);
                    var res = rDS.Insert(remObsPlan);
                    responseMessage.Append(res == null ? "\n Insertion of data failed for the RemediationPlanObservableAndResourceType:" + remObsPlan.ResourceTypeId : "\n Insertion of data success for the RemediationPlanObservableAndResourceType:" + remObsPlan.ResourceTypeId);

                }

            }
            catch(Exception ex)
            {
                throw ex;
            }
        
            return responseMessage.ToString();
        }


        public string updateRemediationPlanObservableAndResourceTypeDetails(BE.RemediationPlanObservableAndResourceTypeMap remPlan)
        {
            StringBuilder responseMessage = new StringBuilder();
            ResourcetypeObservableRemediationPlanMapDS rDS = new ResourcetypeObservableRemediationPlanMapDS();
            ResourceTypeDS rType = new ResourceTypeDS();
            ObservableDS obsDs = new ObservableDS();
            RemediationPlanDS rPlanDs = new RemediationPlanDS();
            try
            {
                foreach (BE.RemediationPlanObservableAndResourceType rem in remPlan.RemediationPlanObservableAndResourceTypeList)
                {
                    resourcetype_observable_remediation_plan_map remObsPlan = new resourcetype_observable_remediation_plan_map();
                    remObsPlan.ResourceTypeId = rem.resourcetypeid;
                    remObsPlan.ObservableId = rem.ObservableId;
                    remObsPlan.RemediationPlanId = rem.RemediationPlanId;
                    remObsPlan.CreatedBy = rem.CreatedBy;
                    remObsPlan.CreateDate = rem.CreateDate;
                    remObsPlan.ModifiedBy = rem.ModifiedBy;
                    remObsPlan.ModifiedDate = rem.ModifiedDate;
                    remObsPlan.TenantId = remPlan.tenantid;
                   // remObsPlan.ValidityStart = Convert.ToDateTime(rem.ValidityStart);
                   // remObsPlan.ValidityEnd = Convert.ToDateTime(rem.ValidityEnd);
                    remObsPlan.ValidityStart = rem.ValidityStart.Contains("India Standard Time") ? DateTime.ParseExact(rem.ValidityStart, "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(India Standard Time)'", System.Globalization.CultureInfo.InvariantCulture) : Convert.ToDateTime(rem.ValidityStart);
                    remObsPlan.ValidityEnd = rem.ValidityEnd.Contains("India Standard Time") ? DateTime.ParseExact(rem.ValidityEnd, "ddd MMM dd yyyy HH:mm:ss 'GMT'K '(India Standard Time)'", System.Globalization.CultureInfo.InvariantCulture) : Convert.ToDateTime(rem.ValidityEnd);
                    var res = rDS.Update(remObsPlan);
                    responseMessage.Append(res == null ? "\n Update of data failed for the RemediationPlanObservableAndResourceType:" + remObsPlan.ResourceTypeId : "\n Update of data success for the RemediationPlanObservableAndResourceType:" + remObsPlan.ResourceTypeId);

                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
                return responseMessage.ToString();
        }

        public string deleteRemediationPlanObservableAndResourceTypeDetails(BE.RemediationPlanObservableAndResourceTypeMap remPlan)
        {
            StringBuilder responseMessage = new StringBuilder();
            ResourcetypeObservableRemediationPlanMapDS rDS = new ResourcetypeObservableRemediationPlanMapDS();

            try
            {
                foreach (BE.RemediationPlanObservableAndResourceType rem in remPlan.RemediationPlanObservableAndResourceTypeList)
                {
                    resourcetype_observable_remediation_plan_map remObsPlan = new resourcetype_observable_remediation_plan_map();
                    remObsPlan.ResourceTypeId = rem.resourcetypeid;
                    remObsPlan.ObservableId = rem.ObservableId;
                    remObsPlan.RemediationPlanId = rem.RemediationPlanId;
                    remObsPlan.CreatedBy = rem.CreatedBy;
                    remObsPlan.CreateDate = rem.CreateDate;
                    remObsPlan.ModifiedBy = rem.ModifiedBy;
                    remObsPlan.ModifiedDate = rem.ModifiedDate;
                    remObsPlan.TenantId = remPlan.tenantid;
                    remObsPlan.ValidityStart = Convert.ToDateTime(rem.ValidityStart);
                    remObsPlan.ValidityEnd = DateTime.Now.AddDays(-1);
                    var res = rDS.Delete(remObsPlan);
                    responseMessage.Append(res == null ? "\n Deletion of data failed for the RemediationPlanObservableAndResourceType:" + remObsPlan.ResourceTypeId : "\n Deletion of data success for the RemediationPlanObservableAndResourceType:" + remObsPlan.ResourceTypeId);

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
