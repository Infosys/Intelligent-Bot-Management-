/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using System;
using Infosys.Lif.LegacyIntegrator;
using Infosys.Solutions.Superbot.Resource.IDataAccess;
using Infosys.Solutions.Superbot.Infrastructure.Common;
using Infosys.Lif.LegacyIntegratorService;

namespace Infosys.Solutions.Ainauto.Resource.DataAccess.Queue
{
    public class QueueDS<T> : IQueue<T> where T : class
    {
        private string targetName;
        private AdapterManager adapterManager;

        public QueueDS(string targetName)
        {
            // check if type T is serializable
            if (!Attribute.IsDefined(typeof(T), typeof(SerializableAttribute)))
            {
                throw new ArgumentException("Type is not serializable!", (typeof(T)).ToString());
            }

            // note queue name
            this.targetName = targetName;
            adapterManager = new AdapterManager();
        }

        public string Send(T message, string Region = null)
        {
            // validate input parameter
            if (null == message)
            {
                LogHandler.LogError("Message passed is null!", LogHandler.Layer.Resource);
                throw new SuperbotException("Cannot enqueue null message.");
            }

            // check if region exists
            if (string.IsNullOrWhiteSpace(targetName) && string.IsNullOrWhiteSpace(Region))
            {
                LogHandler.LogError("Host region not mentioned!", LogHandler.Layer.Resource);
                throw new SuperbotException("Host region must be passed either in constructor or in this method!");
            }

            // serialize message
            var serializedMessage = Utility.SerialiseToJSON(message);
            var region = Region ?? targetName;

            // send message to queue
            string response = new AdapterManager().Execute(serializedMessage, region);

            // archive and log message sending operation
            LogHandler.ArchiveMessages(serializedMessage, region);
            LogHandler.LogDebug("Successfully posted message to queue", LogHandler.Layer.Resource, serializedMessage);

            // return response of message send operation
            return response;
        }
    }
}
