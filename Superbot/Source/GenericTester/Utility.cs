
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
using Infosys.Solutions.Ainauto.Resource.DataAccess;
using BE = Infosys.Solutions.Ainauto.BusinessEntity;
using DE = Infosys.Solutions.Superbot.Resource.Entity;
using ES = Infosys.Solutions.Ainauto.Resource.DataAccess.ElasticSearch;
using Infosys.Solutions.Superbot.Infrastructure.Common;

namespace GenericTester
{
    public class Utility
    {
        public bool PushDataToElasticSearch(int platformId,string indexName)
        {
            bool status = true;
            ES.ElasticSearch es = new ES.ElasticSearch();
            ObservationsDS observationsDS = new ObservationsDS();
            ResourceDS resourceDS = new ResourceDS();
            ResourceTypeDS resourceTypeDS = new ResourceTypeDS();
            ObservableResourceMapDS observableResourceMapDS = new ObservableResourceMapDS();
            try
            {
                var observationObjList = observationsDS.GetAll(new DE.observations() { PlatformId = platformId });
                var resultdata = (from s in observationObjList
                                  join r in resourceDS.GetAll()
                                  on s.ResourceId equals r.ResourceId
                                  join rt in resourceTypeDS.GetAll()
                                  on s.ResourceTypeId equals rt.ResourceTypeId
                                  join orm in observableResourceMapDS.GetAll()
                                  on new { s.ResourceId, s.ObservableId } equals new { orm.ResourceId, orm.ObservableId }
                                  select new ES.Model.ElasticSearchInput
                                  {
                                      ConfigId = s.ConfigId,
                                      PortfolioId = s.PortfolioId,
                                      PortfolioName = resourceDS.GetOne(new DE.resource() { ResourceId = s.PortfolioId }).ResourceName,
                                      ResourceId = s.ResourceId,
                                      ResourceName = r.ResourceName,
                                      ObservabeId = s.ObservableId.ToString(),
                                      ObservableName = s.ObservableName,
                                      MetricValue = s.Value,
                                      MetricValueinNumeric =s.Value.ConvertToDouble(),
                                      MetricTime = Convert.ToDateTime(s.ObservationTime),
                                      MetricTimeString = s.ObservationTime.ToString(),
                                      ResourceTypeId = s.ResourceTypeId.ToString(),
                                      ResourceTypeName = rt.ResourceTypeName,
                                      IncidentId = s.IncidentId,
                                      IncidentCreateTime =Convert.ToDateTime(s.ObservationTime),
                                      ServerState = s.State,
                                      UpperThreshold = orm.UpperThreshold,
                                      LowerThreshold = orm.LowerThreshold,
                                      UpperThresholdinNumeric = orm.UpperThreshold.ConvertToDouble(),
                                      LowerThresholdinNumeric = orm.LowerThreshold.ConvertToDouble(),
                                      Count = s.ObservationSequence.ToString()
                                  }).ToList();

                DE.resource resource = new DE.resource();

                foreach (var obj in resultdata)
                {
                    //resource.ResourceId = obj.PortfolioId;
                    //obj.PortfolioName = resourceDS.GetOne(resource).ResourceName;
                    //obj.Count = resultdata.Count.ToString();
                    obj.IsHealthy = 0;
                    obj.IsWarning = 0;
                    obj.IsCritical = 0;
                    switch (obj.ServerState)
                    {
                        case "Healthy":
                            obj.IsHealthy = 1;
                            break;
                        case "Warning":
                            obj.IsWarning = 1;
                            break;
                        case "Critical":
                            obj.IsCritical = 1;
                            break;
                        default:
                            break;
                    }

                    if(!es.Insert(obj, indexName))
                    {
                        LogHandler.LogError(String.Format("Failed to Insert into Elastic Search for Config ID : {0}",obj.ConfigId), LogHandler.Layer.Business, null);
                    }
                }
                //ES.Model.ElasticSearchInput inputObj = new ES.Model.ElasticSearchInput();

                //foreach (var obj in observationObjList)
                //{
                //    inputObj.ConfigId = obj.ConfigId;
                //    inputObj.PortfolioId = obj.PortfolioId;
                //    inputObj.PortfolioName = "";//n
                //    inputObj.ResourceId = "";
                //    inputObj.ResourceName = "";
                //    inputObj.ObservabeId = "";
                //    inputObj.ObservableName = "";
                //    inputObj.Count = "";//n
                //    inputObj.MetricValue = "";
                //    inputObj.MetricTime = "";
                //    inputObj.MetricTimeString = "";
                //    inputObj.ResourceTypeId = "";
                //    inputObj.ResourceTypeName = "";
                //    inputObj.IncidentId = "";
                //    inputObj.IncidentCreateTime = "";
                //    inputObj.ServerState = "";
                //    //inputObj.IsCritical = "";
                //    //inputObj.IsHealthy = "";
                //    //inputObj.IsWarning = "";
                //    inputObj.UpperThreshold = "";
                //    inputObj.LowerThreshold = "";

                //    es.Insert(inputObj, "");
                //}
            }
            catch (Exception ex)
            {
                status = false;
            }

            return status;
        }


    }
}
