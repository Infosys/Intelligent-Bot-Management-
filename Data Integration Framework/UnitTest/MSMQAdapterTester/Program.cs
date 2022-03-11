/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Infosys.Lif.LegacyCommon;
using Infosys.Lif.LegacyIntegrator;
using Infosys.Lif.LegacyIntegratorService;
using System.Diagnostics;

namespace MSMQAdapterTester
{
    class Program
    {
        static void Main(string[] args)
        {
            LifLogHandler.LogDebug("Loading lisettings config file.", LifLogHandler.Layer.IntegrationLayer);
            var lisettingPath = @"Configuration\LiSettings.config";
            // load lisettings as xml document
            var lisetting = new XmlDocument();
            lisetting.Load(lisettingPath);
            LifLogHandler.LogDebug("Successfully loaded config file from {0}.", LifLogHandler.Layer.IntegrationLayer,
                lisettingPath);

            // plumb unhandled exception handler
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            // get names of queues
            var queueNames = lisetting.SelectNodes(@"/LISettings/MSMQ/MSMQDetails/QueueName")
                                        .Cast<XmlNode>()
                                        .Select(x => x.InnerText)
                                        .Distinct()
                                        .ToList();
            LifLogHandler.LogDebug("Retrieved {0} distinct queue names.", LifLogHandler.Layer.IntegrationLayer,
                queueNames.Count);
            // get names of poison queues
            var poisonQueueNames = lisetting.SelectNodes(@"/LISettings/MSMQ/MSMQDetails/PoisonQueueName")
                                        .Cast<XmlNode>()
                                        .Select(x => x.InnerText)
                                        .Distinct()
                                        .ToList();
            LifLogHandler.LogDebug("Retrieved {0} distinct poison queue names.", LifLogHandler.Layer.IntegrationLayer,
                poisonQueueNames.Count);
            // create private queues
            foreach (var q in queueNames)
            {
                // create q if it does not exist, else get existing queue
                var qName = @".\Private$\" + q;
                MessageQueue mq = null;
                if (MessageQueue.Exists(qName))
                    mq = new MessageQueue(qName);
                else
                    mq = MessageQueue.Create(qName);

                if (mq == null)
                    continue;

                
                // revoke permissions for everyone
                mq.SetPermissions(@"Everyone",
                    MessageQueueAccessRights.FullControl, AccessControlEntryType.Revoke);
                // revoke permissions for anonymous
                mq.SetPermissions(@"ANONYMOUS LOGON",
                    MessageQueueAccessRights.FullControl, AccessControlEntryType.Revoke);
                // set permission for pptware_support
                mq.SetPermissions(@"itlinfosys\pptware_support",
                    MessageQueueAccessRights.ReceiveMessage
                    | MessageQueueAccessRights.PeekMessage
                    | MessageQueueAccessRights.GetQueueProperties
                    | MessageQueueAccessRights.SetQueueProperties
                    | MessageQueueAccessRights.GetQueuePermissions
                    | MessageQueueAccessRights.WriteMessage
                    , AccessControlEntryType.Allow);
                // set permission for pptware_support
                mq.SetPermissions(@"itlinfosys\pptware_service",
                    MessageQueueAccessRights.ReceiveMessage
                    | MessageQueueAccessRights.PeekMessage
                    | MessageQueueAccessRights.GetQueueProperties
                    | MessageQueueAccessRights.SetQueueProperties
                    | MessageQueueAccessRights.GetQueuePermissions
                    | MessageQueueAccessRights.WriteMessage
                    , AccessControlEntryType.Allow);

            }
            LifLogHandler.LogDebug("Created private queues.", LifLogHandler.Layer.IntegrationLayer);
            // create private poison queues
            foreach (var q in poisonQueueNames)
            {
                var qName = @".\Private$\" + q;
                if (MessageQueue.Exists(qName))
                    new MessageQueue(qName);
                else
                    MessageQueue.Create(qName);
            }
            LifLogHandler.LogDebug("Created private poison queues.", LifLogHandler.Layer.IntegrationLayer);

            // get names of transports for queues
            var transportNames = lisetting.SelectNodes(@"/LISettings/MSMQ/MSMQDetails/TransportName")
                                        .Cast<XmlNode>()
                                        .Select(x => x.InnerText)
                                        .Distinct()
                                        .ToList();

            // get regions used in lisettings for MSMQ transports only
            var regions = new List<string>();
            foreach (var transName in transportNames)
            {
                var xPath = string.Format(
                    @"/LISettings/HostRegion/Region/Name[../TransportName/text() = '{0}']", transName);
                var regionNode = lisetting.SelectSingleNode(xPath);
                regions.Add(regionNode.InnerText);
            }

            // initiate producer task to populate queues
            LifLogHandler.LogDebug("Starting produce task...", LifLogHandler.Layer.IntegrationLayer);
            var producer = Task.Factory.StartNew(new Action(() => ProducerTask(regions)));

            // initiate one consumer task for each region
            Task[] consumers = new Task[regions.Count];
            for (int i = 0; i < regions.Count; i++)
            {
                var region = regions[i];
                LifLogHandler.LogDebug("Starting consumer task for region {0}",
                    LifLogHandler.Layer.IntegrationLayer, region);
                consumers[i] = Task.Factory.StartNew(new Action(() => ConsumerTask(region)));
            }

            Console.WriteLine("Producer and consumers have been started. Hit control+c to stop the process. Monitor log file for details.");
            // wait for producer and consumers to complete their job
            Task.WaitAll(consumers);
            producer.Wait();
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LifLogHandler.LogError("Unhandled exception occured: {0}", LifLogHandler.Layer.IntegrationLayer,
                e.ExceptionObject);
            if (e.IsTerminating)
                LifLogHandler.LogDebug("CLR is terminating.", LifLogHandler.Layer.IntegrationLayer);
            else
                LifLogHandler.LogDebug("CLR is not terminating.", LifLogHandler.Layer.IntegrationLayer);
        }

        private static void ConsumerTask(string region)
        {
            // configure adapter manager to receive messages
            LifLogHandler.LogDebug("Configuring receive handler for region {0}",
                LifLogHandler.Layer.IntegrationLayer, region);
            var am = new AdapterManager();
            am.ResponseReceived += new AdapterManager.AdapterReceiveHandler((ea) => am_ResponseReceived(ea, region, am));

            LifLogHandler.LogDebug("Requesting message for region {0}",
                LifLogHandler.Layer.IntegrationLayer, region);
            am.Receive(region);
        }

        private static void am_ResponseReceived(ReceiveEventArgs eventArgs, string region, AdapterManager am)
        {
            string messageId = eventArgs.ResponseDetails["MessageIdentifier"] as string;
            string message = eventArgs.ResponseDetails["MessageBody"] as string;
            LifLogHandler.LogDebug("Received message: Region: {0}\tId: {1}\tContent: {2}",
                LifLogHandler.Layer.IntegrationLayer, region, messageId, message);

            LifLogHandler.LogDebug("Response contains {0} fields.", LifLogHandler.Layer.IntegrationLayer,
                eventArgs.ResponseDetails.Keys.Count);

            foreach (var key in eventArgs.ResponseDetails.Keys)
                LifLogHandler.LogDebug("Response[{0}]={1}", LifLogHandler.Layer.IntegrationLayer,
                    key, eventArgs.ResponseDetails[key]);

            if (messageId == null)
            {
                LifLogHandler.LogDebug("No message id to delete", LifLogHandler.Layer.IntegrationLayer);
                return;
            }

            LifLogHandler.LogDebug("Deleting message {0}", LifLogHandler.Layer.IntegrationLayer, messageId);
            am.Delete(messageId);
            LifLogHandler.LogDebug("Deleted message {0}", LifLogHandler.Layer.IntegrationLayer, messageId);
            //// randomly generate exception
            //var test = new Random().Next();
            ////if ((test % 2) == 0)
            //if(region == "test1")
            //    throw new Exception("exception check!");
        }

        /// <summary>
        /// Sends a message per second to queues in round robin fashion
        /// </summary>
        /// <param name="regions"></param>
        private static void ProducerTask(List<string> regions)
        {
            var am = new AdapterManager();
            do
            {
                foreach (var region in regions)
                {
                    var msg = Guid.NewGuid().ToString();
                    LifLogHandler.LogDebug("Sending message {0} to region {1}...",
                        LifLogHandler.Layer.IntegrationLayer, msg, region);
                    am.Execute(msg, region);
                    Thread.Sleep(30000);
                }

                // dump thread count
                LifLogHandler.LogDebug("# thread count: {0}", LifLogHandler.Layer.IntegrationLayer,
                    Process.GetCurrentProcess().Threads.Count);
            } while (true);
        }
    }
}
