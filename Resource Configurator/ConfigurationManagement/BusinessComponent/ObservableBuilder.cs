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
    public class ObservableBuilder
    {
        public BE.observable getObservableDetails(int tenantId)
        {
            ObservableDS obsDS = new ObservableDS();
            try
            {
                List<BE.observableDetails> observabledetails = new List<BE.observableDetails>();
                
                observabledetails = (from a in obsDS.GetAny().ToArray()
                                     where a.TenantId == tenantId &&
                                     ( a.ValidityEnd > DateTime.Today)
                                     select new BE.observableDetails
                                     {
                                         observableid = a.ObservableId ,
                                         observablename = a.ObservableName ,
                                         unitofmeasure = a.UnitOfMeasure ,
                                         datatype = a.DataType , 
                                         ValidityStart = Convert.ToDateTime(a.ValidityStart) ,
                                         ValidityEnd = Convert.ToDateTime(a.ValidityEnd) ,
                                         createdby = a.CreatedBy ,
                                         ModifiedBy = a.ModifiedBy ,
                                         CreateDate = Convert.ToDateTime(a.CreateDate) ,
                                         ModifiedDate = Convert.ToDateTime(a.ModifiedDate),
                                         
                                     }).ToList();
                BE.observable obs = new BE.observable();
                obs.tenantId = tenantId;
                obs.observableDetails = observabledetails;
                return obs;
            }

            catch(Exception ex)
            {
                throw ex;
            }
           
        }

        public string createObservableDetails(BE.observable observable)
        {
            StringBuilder responseMessage = new StringBuilder();
            ObservableDS obsDS = new ObservableDS();
            try
            {
                foreach(BE.observableDetails obsDetails in observable.observableDetails)
                {
                    observable obs = new observable();
                    obs.ObservableId = obsDetails.observableid;
                    obs.ObservableName = obsDetails.observablename;
                    obs.UnitOfMeasure = obsDetails.unitofmeasure;
                    obs.DataType = obsDetails.datatype;
                    obs.ValidityStart = Convert.ToDateTime(obsDetails.ValidityStart);
                    obs.ValidityEnd = Convert.ToDateTime(obsDetails.ValidityEnd);
                    obs.CreatedBy = obsDetails.createdby;
                    obs.CreateDate = Convert.ToDateTime(obsDetails.CreateDate);
                    obs.ModifiedDate = Convert.ToDateTime(obsDetails.ModifiedDate);
                    obs.ModifiedBy = obsDetails.ModifiedBy;
                    obs.TenantId = observable.tenantId;

                    var result = obsDS.Insert(obs);
                    responseMessage.Append(result == null ? "\n Insertion of observable data failed " : "\n Insertion of observable data success ");

                }
            }

            catch(Exception ex)
            {
                throw ex;
            }

            return responseMessage.ToString();
        }

        public string updateObservableDetails(BE.observable observable)
        {
            StringBuilder responseMessage = new StringBuilder();
            ObservableDS obsDS = new ObservableDS();
            IList<observable> observableList = new List<observable>();
            try
            {
                if(observable != null)
                {
                    foreach(BE.observableDetails obs in observable.observableDetails)
                    {
                        observableList.Add(new observable()
                        {
                            ObservableId = obs.observableid,
                            ObservableName = obs.observablename,
                            UnitOfMeasure = obs.unitofmeasure,
                            DataType=obs.datatype,
                            ValidityStart = Convert.ToDateTime(obs.ValidityStart),
                            ValidityEnd = Convert.ToDateTime(obs.ValidityEnd),
                            CreateDate = Convert.ToDateTime(obs.CreateDate),
                            CreatedBy = obs.createdby,
                            ModifiedBy=obs.ModifiedBy,
                            ModifiedDate= Convert.ToDateTime(obs.ModifiedDate),
                            TenantId = obs.tenantid,


                        
                        });

                        var result = obsDS.UpdateBatch(observableList);
                        responseMessage.Append(result == null ? "\n Updation of observable data failed " : "\n Updation of observable data success ");



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
