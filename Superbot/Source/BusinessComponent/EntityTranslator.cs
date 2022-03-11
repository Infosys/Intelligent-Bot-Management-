/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using Infosys.Solutions.Superbot.Infrastructure.Common;
using Infosys.Solutions.Superbot.Resource.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infosys.Solutions.Ainauto.Resource.DataAccess;
using DE = Infosys.Solutions.Superbot.Resource.Entity.Queue;
using BE = Infosys.Solutions.Ainauto.BusinessEntity;

namespace Infosys.Solutions.Ainauto.Superbot.BusinessComponent
{
    public class EntityTranslator
    {

        public BE.Metric MetricDEToBE(DE.Metric message)
        {
            BE.Metric observableResourceMap = new BE.Metric();
            try
            {
                if (message != null)
                {
                    observableResourceMap.EventType = message.EventType;
                    observableResourceMap.Application = message.Application;
                    observableResourceMap.MetricTime = message.MetricTime;
                    observableResourceMap.ResourceId = message.ResourceId;
                    observableResourceMap.MetricName = message.MetricName;
                    observableResourceMap.MetricValue = message.MetricValue;
                    observableResourceMap.ServerIp = message.ServerIp;
                    observableResourceMap.Description = message.Description;
                    observableResourceMap.Source = message.Source;
                    observableResourceMap.SequenceNumber = message.SequenceNumber;

                    //new
                    observableResourceMap.ConfigId = message.ConfigId;
                    observableResourceMap.PortfolioId = message.PortfolioId;
                    observableResourceMap.ObservableId = message.ObservableId;
                    observableResourceMap.Count = message.Count;
                    observableResourceMap.ResourceTypeId = message.ResourceTypeId;
                    observableResourceMap.IncidentId = message.IncidentId;
                    observableResourceMap.IncidentTime = message.IncidentTime;
                    observableResourceMap.Serverstate = message.Serverstate;
                    observableResourceMap.TransactionId = message.TransactionId;
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

        public DE.Metric MetricBEToDE(BE.Metric message)
        {
            DE.Metric observableResourceMap = new DE.Metric();

            try
            {
                if (message != null)
                {
                    observableResourceMap.EventType = message.EventType;
                    observableResourceMap.Application = message.Application;
                    observableResourceMap.MetricTime = message.MetricTime;
                    observableResourceMap.ResourceId = message.ResourceId;
                    observableResourceMap.MetricName = message.MetricName;
                    observableResourceMap.MetricValue = message.MetricValue;
                    observableResourceMap.ServerIp = message.ServerIp;
                    observableResourceMap.Description = message.Description;
                    observableResourceMap.Source = message.Source;
                    observableResourceMap.SequenceNumber = message.SequenceNumber;

                    observableResourceMap.ConfigId = message.ConfigId;
                    observableResourceMap.PortfolioId = message.PortfolioId;
                    observableResourceMap.ObservableId = message.ObservableId;
                    observableResourceMap.Count = message.Count;
                    observableResourceMap.ResourceTypeId = message.ResourceTypeId;
                    observableResourceMap.IncidentId = message.IncidentId;
                    observableResourceMap.IncidentTime = message.IncidentTime;
                    observableResourceMap.Serverstate = message.Serverstate;
                    observableResourceMap.TransactionId = message.TransactionId;


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

        public DE.MetricMessage MetricBEToDE(BE.MetricMessage messages)
        {
            DE.MetricMessage retObj = new DE.MetricMessage();
            retObj.MetricMessages = new List<DE.Metric>();
            try
            {
                
                if(messages != null && messages.MetricMessages != null && messages.MetricMessages.Count > 0)
                {
                    foreach (var message in messages.MetricMessages)
                    {
                        DE.Metric observableResourceMap = new DE.Metric();
                        observableResourceMap.EventType = message.EventType;
                        observableResourceMap.Application = message.Application;
                        observableResourceMap.MetricTime = message.MetricTime;
                        observableResourceMap.ResourceId = message.ResourceId;
                        observableResourceMap.MetricName = message.MetricName;
                        observableResourceMap.MetricValue = message.MetricValue;
                        observableResourceMap.ServerIp = message.ServerIp;
                        observableResourceMap.Description = message.Description;
                        observableResourceMap.Source = message.Source;
                        observableResourceMap.SequenceNumber = message.SequenceNumber;

                        observableResourceMap.ConfigId = message.ConfigId;
                        observableResourceMap.PortfolioId = message.PortfolioId;
                        observableResourceMap.ObservableId = message.ObservableId;
                        observableResourceMap.Count = message.Count;
                        observableResourceMap.ResourceTypeId = message.ResourceTypeId;
                        observableResourceMap.IncidentId = message.IncidentId;
                        observableResourceMap.IncidentTime = message.IncidentTime;
                        observableResourceMap.Serverstate = message.Serverstate;
                        observableResourceMap.TransactionId = message.TransactionId;

                        retObj.MetricMessages.Add(observableResourceMap);
                    }
                }

                return retObj;
            }
            catch (Exception superBotException)
            {
                Exception ex = new Exception();
                bool rethrow = ExceptionHandler.HandleException(superBotException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                if (rethrow)
                {
                    throw ex;

                }
                return retObj;
            }
        }
    }
}
