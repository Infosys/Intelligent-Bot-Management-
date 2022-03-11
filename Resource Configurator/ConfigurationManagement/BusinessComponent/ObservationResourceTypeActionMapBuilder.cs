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
   public class ObservationResourceTypeActionMapBuilder
    {
              



        public string createObservableResourceTypeActionMapDetails(BE.ObservableResourceTypeActionMap observableResourceTypeActionMap)
        {
            StringBuilder responseMessage = new StringBuilder();
            ResourcetypeObservableActionMapDS resourceTypeActionMapDS = new ResourcetypeObservableActionMapDS();
            var result2 = "";

            try
            {
                IList<resourcetype_observable_action_map> observable_Action_Map_List = new List<resourcetype_observable_action_map>();

                foreach (BE.ObservableResourceTypeAction observableResourceTypeAct in observableResourceTypeActionMap.observableresourcetypeactions)
                {
                    resourcetype_observable_action_map observable_Action_Map = new resourcetype_observable_action_map();
                    // observable_Action_Map.
                    observable_Action_Map.ActionId = observableResourceTypeAct.actionid;
                    observable_Action_Map.TenantId = observableResourceTypeActionMap.tenantID;
                    observable_Action_Map.ResourceTypeId = observableResourceTypeAct.resourcetypeid;
                    observable_Action_Map.ObservableId = observableResourceTypeAct.observableid;
                    observable_Action_Map.Name = observableResourceTypeAct.name;
                    observable_Action_Map.ValidityStart = Convert.ToDateTime(observableResourceTypeAct.ValidityStart);
                    observable_Action_Map.ValidityEnd = Convert.ToDateTime(observableResourceTypeAct.ValidityEnd);
                    observable_Action_Map.CreatedBy = observableResourceTypeAct.CreatedBy;
                    observable_Action_Map.CreateDate = Convert.ToDateTime(observableResourceTypeAct.CreateDate);
                    observable_Action_Map.ModifiedBy = observableResourceTypeAct.ModifiedBy;
                    observable_Action_Map.ModifiedDate = Convert.ToDateTime(observableResourceTypeAct.ModifiedDate);

                    var result1 = resourceTypeActionMapDS.Insert(observable_Action_Map);

                    //observableResourceTypeActionRecord.CreateDate = observableResourceTypeAct.CreateDate;

                    observable_Action_Map_List.Add(observable_Action_Map);
                    // observableResourceTypeActionMapToAdd.observableresourcetypeactions = observableResourceTypeActionRecord;

                    responseMessage.Append(result1 == null ? "\n Insertion of resourcetype_observable_action_map data failed " : "\n Insertion of resource_dependency_map data success ");

                }
                
                


            }
            catch (Exception ex)
            {
                throw ex;
            }
            return responseMessage.ToString();
        }


        public string updatebservableResourceTypeActionMapDetails(BE.ObservableResourceTypeActionMap observableResourceTypeActionMap)
        {
            StringBuilder responseMessage = new StringBuilder();
            try
            {
                if (observableResourceTypeActionMap != null)
                {
                    ResourcetypeObservableActionMapDS observableActionMapDS = new ResourcetypeObservableActionMapDS();
                    IList<resourcetype_observable_action_map> resource_obs_action = new List<resourcetype_observable_action_map>();

                    foreach (BE.ObservableResourceTypeAction obs in observableResourceTypeActionMap.observableresourcetypeactions)
                    {
                        int id = obs.resourcetypeid;
                        // var entityItem = observableActionMapDSList.Where(c => c.ResourceTypeId == id).FirstOrDefault();
                        
                        //if (entityItem != null)
                        //{
                        //    resource_obs_action.Add(new resourcetype_observable_action_map()
                        //    {
                        //        ActionId = obs.actionid,
                        //        Name = obs.name,
                        //        CreateDate= obs.CreateDate ,
                        //        ModifiedDate = obs.ModifiedDate ,
                        //        ModifiedBy = obs.ModifiedBy ,
                        //        ValidityEnd = obs.ValidityEnd ,
                        //        ValidityStart = obs.ValidityStart ,
                        //        CreatedBy = obs.CreatedBy ,

                        //    });

                        resource_obs_action.Add(new resourcetype_observable_action_map()
                        {
                            ActionId = obs.actionid,
                            Name = obs.name,
                            CreateDate = Convert.ToDateTime(obs.CreateDate),
                            ModifiedDate = Convert.ToDateTime(obs.ModifiedDate),
                            ModifiedBy = obs.ModifiedBy,
                            ValidityEnd = Convert.ToDateTime(obs.ValidityEnd),
                            ValidityStart = Convert.ToDateTime(obs.ValidityStart),
                            CreatedBy = obs.CreatedBy,
                            ObservableId = obs.observableid,
                            ResourceTypeId = obs.resourcetypeid,


                        });



                    }
                    var res = observableActionMapDS.UpdateBatch(resource_obs_action);
                    responseMessage.Append(res == null ? "\n Updation of resourcetype_observable_action_map data failed " : "\n Updation of resource_dependency_map data success ");
                }

                    
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return responseMessage.ToString();
        }

        public string returnDummyVariable()
        {
            dummyDS d = new dummyDS();
            BE.dummy dummy = new BE.dummy();
            dummy.name = "ABC";
            string name = dummy.name;
            return name;
        }

        public BE.ObservableResourceTypeActionMap getObservableResourceTypeActionMapDetails(int tenantID)
        {
            ResourcetypeObservableActionMapDS rtypeobsDS = new ResourcetypeObservableActionMapDS();
            ObservableDS obsDS = new ObservableDS();
            ResourceTypeDS rtypeDS = new ResourceTypeDS();
            ActionDS actionDS = new ActionDS();
            try
            {
                List<BE.ObservableResourceTypeAction> observableresourcetypeactiondetails = new List<BE.ObservableResourceTypeAction>();
                observableresourcetypeactiondetails = (from a in rtypeobsDS.GetAny().ToArray()
                                                       join b in obsDS.GetAny().ToArray() on a.ObservableId equals b.ObservableId
                                                       join c in rtypeDS.GetAny().ToArray() on a.ResourceTypeId equals c.ResourceTypeId
                                                       join d in actionDS.GetAny().ToArray() on a.ActionId equals d.ActionId
                                                       where a.TenantId == b.TenantId &&
                                                             a.TenantId == c.TenantId &&
                                                             a.TenantId == d.TenantId &&
                                                             a.TenantId == tenantID &&
                                                             //c.PlatformId == platformID &&
                                                             ( a.ValidityEnd > DateTime.Today) &&
                                                             ( b.ValidityEnd > DateTime.Today) &&
                                                             ( c.ValidityEnd > DateTime.Today) &&
                                                             ( d.ValidityEnd > DateTime.Today) &&
                                                             d.IsDeleted == false
                                                       select new BE.ObservableResourceTypeAction
                                                       {
                                                           name = a.Name,
                                                           resourcetypename = c.ResourceTypeName,
                                                           resourcetypeid = c.ResourceTypeId,
                                                           observableid = b.ObservableId,
                                                           observablename = b.ObservableName,
                                                           actionid = d.ActionId,
                                                           actionname = d.ActionName,
                                                           CreateDate = Convert.ToDateTime(a.CreateDate),
                                                           ModifiedDate = Convert.ToDateTime(a.ModifiedDate),
                                                           ValidityStart = Convert.ToDateTime(a.ValidityStart),
                                                           ValidityEnd = Convert.ToDateTime(a.ValidityEnd),
                                                           CreatedBy = a.CreatedBy,
                                                           ModifiedBy = a.ModifiedBy

                                                       }).ToList();
                BE.ObservableResourceTypeActionMap observableResourceTypeActionMap = new BE.ObservableResourceTypeActionMap();
                //observableResourceTypeActionMap.PlatformID = platformID;
                observableResourceTypeActionMap.tenantID = tenantID;
                observableResourceTypeActionMap.observableresourcetypeactions = observableresourcetypeactiondetails;
                return observableResourceTypeActionMap;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

    }
}
