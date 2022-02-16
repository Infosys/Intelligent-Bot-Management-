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
using DE = Infosys.Solutions.Superbot.Resource.Entity.Queue;
using BE = Infosys.Solutions.Ainauto.BusinessEntity;
using Infosys.Solutions.Superbot.Infrastructure.Common;

namespace Infosys.Solutions.Ainauto.Superbot.BusinessComponent.Translator
{
    public class AnamolyTranslator
    {
        public BE.Anomaly AnamolyDEToBE(DE.Anomaly message)
        {
            BE.Anomaly observableResourceMap = new BE.Anomaly();


            try
            {
                if (message != null)
                {

                    observableResourceMap.ObservationId = message.ObservationId;
                    observableResourceMap.PlatformId = message.PlatformId;
                    observableResourceMap.ResourceId = message.ResourceId;
                    observableResourceMap.ResourceTypeId = message.ResourceTypeId;
                    observableResourceMap.ObservableId = message.ObservableId;
                    observableResourceMap.ObservableName = message.ObservableName;
                    observableResourceMap.ObservationStatus = message.ObservationStatus;
                    observableResourceMap.Value = message.Value;
                    observableResourceMap.ThresholdExpression = message.ThresholdExpression;
                    observableResourceMap.ServerIp = message.ServerIp;
                    observableResourceMap.ObservationTime = message.ObservationTime;
                    observableResourceMap.Descriptionn = message.Description;
                    observableResourceMap.EventType = message.EventType;
                    observableResourceMap.Source = message.Source;

                }

                return observableResourceMap;
            }
            catch (Exception superBotException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(superBotException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;

                }
                return observableResourceMap;
            }
        }
        public DE.Anomaly AnamolyBEToDE(BE.Anomaly message)
        {
            DE.Anomaly observableResourceMap = new DE.Anomaly();
            

            try
            {
                if (message != null)
                {

                    observableResourceMap.ObservationId = message.ObservationId;
                    observableResourceMap.PlatformId = message.PlatformId;
                    observableResourceMap.ResourceId = message.ResourceId;
                    observableResourceMap.ResourceTypeId = message.ResourceTypeId;
                    observableResourceMap.ObservableId = message.ObservableId;
                    observableResourceMap.ObservableName = message.ObservableName;
                    observableResourceMap.ObservationStatus = message.ObservationStatus;
                    observableResourceMap.Value = message.Value;
                    observableResourceMap.ThresholdExpression = message.ThresholdExpression;
                    observableResourceMap.ServerIp = message.ServerIp;
                    observableResourceMap.ObservationTime = message.ObservationTime;
                    observableResourceMap.Description = message.Descriptionn;
                    observableResourceMap.EventType = message.EventType;                    
                    observableResourceMap.Source = message.Source;
                    
                }

                return observableResourceMap;
            }
            catch (Exception superBotException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(superBotException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;

                }
                return observableResourceMap;
            }
        }
    }
}
