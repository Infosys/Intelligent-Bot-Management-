/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infosys.Lif.LegacyCommon;
using Infosys.Lif.LegacyIntegrator;
using Infosys.Solutions.Superbot.Infrastructure.Common;
using Infosys.Solutions.Superbot.Resource.Entity.Queue;
using Infosys.Solutions.Superbot.Resource.IDataAccess;

namespace Infosys.Solutions.Ainauto.Resource.DataAccess.Queue
{
    public class MetricProcessorDS : IQueue<MetricMessage>
    {
        ///<summary>
        /// Send presentation details for triggering presentation encoding process for both HTML as well as Image.
        ///</summary>
        ///<param name="message">Message to be logged of type Queue Presentation Entity</param>
        ///<param name="process">Id of the process to be initiated. 
        ///DEFAULT or a blank field or a null value will invoke the HTML & IMAGE encoding regions on LISettings.config</param>
        ///<returns>Status of operation in the format "HTML:[Post Status],Image:[Post status]</returns>
        public string Send(MetricMessage message, string process)
        {            

            using (LogHandler.TraceOperations("MetricProcessorDS:Send",
                LogHandler.Layer.Resource, Guid.NewGuid(), null))
            {
                AdapterManager adapterManager = new AdapterManager();
                string msgResponse = "";
                //serialize presentation entity to JSON msg
                string serializedPresentationMsg = Utility.SerialiseToJSON(message);

                if (process == "DEFAULT" || process == "" || process == null)
                {
                    //Post message for html encoding
                    msgResponse = adapterManager.Execute(serializedPresentationMsg, "MetricProcessor");
                    LogHandler.ArchiveMessages(serializedPresentationMsg, "MetricProcessor");
                    //LogHandler.LogDebug("MetricProcessorDS: Metric Payload for metric processor successfully posted.",
                    //  Infosys.PPTWare.Infrastructure.ApplicationCore.LogHandler.Layer.Resource, message.CompanyId, message.PresentationId);

                    return msgResponse;
                }
                else
                {

                    //Post message for html encoding
                    msgResponse = adapterManager.Execute(serializedPresentationMsg, process);
                    LogHandler.ArchiveMessages(serializedPresentationMsg, process);
                    //LogHandler.LogDebug("PresentationEncoderDS: " + process + "  Message for presentation {0}.{1} successfully posted.",
                    //    Infosys.PPTWare.Infrastructure.ApplicationCore.LogHandler.Layer.Resource, message.CompanyId, message.PresentationId);
                    //
                    return msgResponse;
                }
            }
        }

        public string Send(Metric message, string process)
        {
            MetricMessage metricMessage = new MetricMessage();
            metricMessage.MetricMessages = new List<Metric>();
            metricMessage.MetricMessages.Add(message);

            using (LogHandler.TraceOperations("MetricProcessorDS:Send",
                LogHandler.Layer.Resource, Guid.NewGuid(), null))
            {
                AdapterManager adapterManager = new AdapterManager();
                string msgResponse = "";
                //serialize presentation entity to JSON msg
                string serializedPresentationMsg = Utility.SerialiseToJSON(metricMessage);

                if (process == "DEFAULT" || process == "" || process == null)
                {
                    //Post message for html encoding
                    msgResponse = adapterManager.Execute(serializedPresentationMsg, "MetricProcessor");
                    LogHandler.ArchiveMessages(serializedPresentationMsg, "MetricProcessor");
                    //LogHandler.LogDebug("MetricProcessorDS: Metric Payload for metric processor successfully posted.",
                    //  Infosys.PPTWare.Infrastructure.ApplicationCore.LogHandler.Layer.Resource, message.CompanyId, message.PresentationId);

                    return msgResponse;
                }
                else
                {

                    //Post message for html encoding
                    msgResponse = adapterManager.Execute(serializedPresentationMsg, process);
                    LogHandler.ArchiveMessages(serializedPresentationMsg, process);
                    //LogHandler.LogDebug("PresentationEncoderDS: " + process + "  Message for presentation {0}.{1} successfully posted.",
                    //    Infosys.PPTWare.Infrastructure.ApplicationCore.LogHandler.Layer.Resource, message.CompanyId, message.PresentationId);
                    //
                    return msgResponse;
                }
            }
        }
    }
}
