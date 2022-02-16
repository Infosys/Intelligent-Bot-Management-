/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.Lif.LegacyCommon;
using Infosys.Lif.LegacyIntegratorService;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
//using System.Threading.Tasks;

namespace Infosys.Lif
{
    public class MemoryQueueAdapter : IAdapter
    
    {


        private const string REGION = "Region";
        private const string TRANSPORT_SECTION = "TransportSection";
        private const string SUCCESSFUL_SENT_MESSAGE = "Message successfully sent to the MSMQ.";
        private const string SUCCESSFUL_RECEIVE_MESSAGE = "Message successfully received from MSMQ.";
        private const string SUCCESSFUL_PEEK_MESSAGE = "Message successfully peeked from MSMQ.";
        private const string PROCESSING_INCOMPLETE = "Processing is incomplete.";
        private const string QUEUE_NOTFOUND = "Queue not found.";
        private const int SUCCESSFUL_STATUS_CODE = 0;
        private const int UNSUCCESSFUL_STATUS_CODE = 1000;
        private const string dateFormat = "dd-MMM-yyyy-HH:mm:ss.fffff";
        private const string MESSAGE_TO_BE_DELETED = "MessageToBeDeleted";
        private const string LI_CONFIGURATION = "LISettings";
        private const string ZERO = "$0";


        private string response = PROCESSING_INCOMPLETE;

        public event ReceiveHandler Received;

        static private Dictionary<string, ConcurrentQueue<MemoryQueueMessage>> queueDetails = new Dictionary<string, ConcurrentQueue<MemoryQueueMessage>>();
        static private Dictionary<string, string> messagesToDelete = new Dictionary<string, string>();
        private readonly object messagesToDeleteForLock = new object();

        /// <summary>
        /// Static constructor is used to initialize inMemory Queue Details 
        /// In Memory Queue construction based on LISetting that needs to be performed only once.
        /// It is called automatically before the first instance is created or any static members are referenced.
        /// </summary>        
        static MemoryQueueAdapter()
        {
            LifLogHandler.LogDebug("MemoryQueue Adapter- MemoryQueueAdapter static constructor executed", LifLogHandler.Layer.IntegrationLayer);
           // Console.WriteLine("MemoryQueueAdapter static constructor executed");
            // Read all config data into LISetttings object.
            LISettings liSettings = (LISettings)ConfigurationManager.GetSection(LI_CONFIGURATION);
            // Find the MemoryQueueDetails
            MemoryQueue queueConfiguration = liSettings.MemoryQueue;
            if (queueConfiguration != null)
            {
                foreach (MemoryQueueDetails details in queueConfiguration.MemoryQueueDetailsCollection)
                {
                    string queueName = details.QueueName;
                    if (!string.IsNullOrEmpty(queueName))
                    {
                        if (!queueDetails.ContainsKey(queueName))
                        {
                            queueDetails.Add(queueName, new ConcurrentQueue<MemoryQueueMessage>());
                        }
                        
                    }
                    if (!string.IsNullOrEmpty(details.SecondaryQueues))
                    {
                        string[] alternateQueues = details.SecondaryQueues.Split(';');
                        for (int i = 0; i < alternateQueues.Length; i++)
                        {
                            string secondaryQueueName = alternateQueues[i];
                            if (!queueDetails.ContainsKey(secondaryQueueName))
                            {
                                queueDetails.Add(secondaryQueueName, new ConcurrentQueue<MemoryQueueMessage>());
                            }                            
                        }
                    }
                }
            }
        }
        enum MemoryQueueOperationType
        {
            Send, Receive, Peek
        }

        public bool Delete(ListDictionary messageDetails)
        {
            bool response = true;
            string messageId = messageDetails["MessageIdentifier"].ToString();
            LifLogHandler.LogDebug("MemoryQueue Adapter- Delete called for message with Id {0}",
                   LifLogHandler.Layer.IntegrationLayer, messageId);
            lock (messagesToDeleteForLock)
            {
                messagesToDelete.Add(messageId, MESSAGE_TO_BE_DELETED);
            }
           // Console.WriteLine("Message to delete:"+ messageId);
            return response;
        }

        public void Receive(ListDictionary adapterDetails)
        {
            LifLogHandler.LogDebug("MemoryQueue Adapter- Receive called", LifLogHandler.Layer.IntegrationLayer);

            Infosys.Lif.LegacyIntegratorService.MemoryQueue transportSection = null;
            Infosys.Lif.LegacyIntegratorService.Region regionToBeUsed = null;
            string response = string.Empty;
            try
            {                
                foreach (DictionaryEntry items in adapterDetails)
                {
                    if (items.Key.ToString() == REGION)
                    {
                        regionToBeUsed = items.Value as Region;
                    }
                    else if (items.Key.ToString() == TRANSPORT_SECTION)
                    {
                        transportSection = items.Value as Infosys.Lif.LegacyIntegratorService.MemoryQueue;
                    }
                }

                // Validates whether TransportName specified in the region, exists in MemoryQueueDetails section.
                MemoryQueueDetails memoryQueueDetails = ValidateTransportName(transportSection, regionToBeUsed.TransportName);
                string queueName = memoryQueueDetails.QueueName;
                ConcurrentQueue<MemoryQueueMessage> queue = getQueueDetails(queueName);
                if (queue != null)
                {
                    if (memoryQueueDetails.QueueReadingType == MemoryQueueReadType.Receive)
                    {
                        Thread receiveOphandler = new Thread((ThreadStart)delegate { HandleMessage(MemoryQueueOperationType.Receive, memoryQueueDetails, null, queue); });
                        receiveOphandler.Start();
                    }                        
                }
                else
                {
                    LifLogHandler.LogError("MemoryQueue Adapter- Receive FAILED, because inMemoryQueue does not exit for {0}",
                   LifLogHandler.Layer.IntegrationLayer, queueName);
                }

            }
            catch (LegacyException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            
        }

        public string Send(ListDictionary adapterDetails, string message)
        {
            LifLogHandler.LogDebug("MemoryQueue Adapter- Send called", LifLogHandler.Layer.IntegrationLayer);

            Infosys.Lif.LegacyIntegratorService.MemoryQueue transportSection = null;
            Infosys.Lif.LegacyIntegratorService.Region regionToBeUsed = null;
            string response = string.Empty;
            try
            {

                if (string.IsNullOrEmpty(message))
                    throw new ArgumentException("Message parameter cannot be Empty", "message");
                foreach (DictionaryEntry items in adapterDetails)
                {
                    if (items.Key.ToString() == REGION)
                    {
                        regionToBeUsed = items.Value as Region;
                    }
                    else if (items.Key.ToString() == TRANSPORT_SECTION)
                    {
                        transportSection = items.Value as Infosys.Lif.LegacyIntegratorService.MemoryQueue;
                    }
                }

                // Validates whether TransportName specified in the region, exists in MemoryQueueDetails section.
                MemoryQueueDetails memoryQueueDetails = ValidateTransportName(transportSection, regionToBeUsed.TransportName);
                string queueName = memoryQueueDetails.QueueName;
                string label = memoryQueueDetails.MessageLabel + ZERO;
                ConcurrentQueue<MemoryQueueMessage> queue = getQueueDetails(queueName);
                if (queue != null && message !=null)
                {
                    MemoryQueueMessage queueMessage = ConstructMessage(message, label);
                    response = HandleMessage(MemoryQueueOperationType.Send, memoryQueueDetails, queueMessage, queue);
                   // Console.WriteLine("QueueName:"+ queueName+":" +queue.Count);
                } else
                {
                    LifLogHandler.LogError("MemoryQueue Adapter- Send FAILED, because inMemoryQueue does not exit for {0}",
                   LifLogHandler.Layer.IntegrationLayer, queueName);                   
                }
                
            }
            catch (LegacyException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return response;
        }

        private MemoryQueueMessage ConstructMessage(string msg,string label)
        {
            MemoryQueueMessage message = new MemoryQueueMessage();
            message.Body = msg;
            message.Id = Guid.NewGuid().ToString();
            message.Label = label;
           return message;
        }

        /// <summary>
        /// Get the inMemoryQueue object based on the QueueName. 
        /// If the QueueName does not exist in the queueDetails object, it throws the LegacyException
        /// </summary>
        /// <param name="transportSection">MemoryQueue section</param>
        /// <param name="transportName">name of the transport</param>

        private ConcurrentQueue<MemoryQueueMessage> getQueueDetails(string queueName)
        {
            ConcurrentQueue<MemoryQueueMessage> queue = null;
            if (queueDetails.ContainsKey(queueName))
            {
                queue = (ConcurrentQueue<MemoryQueueMessage>)queueDetails[queueName];
            }else
            {
                throw new LegacyException(queueName + " Either Queue is not defined in MemoryQueueDetails section or not created in Memory");
            }            
            return queue;

        }

        /// <summary>
        /// Validates whether TransportName specified in the region, exists in MemoryQueueDetails
        /// section. If it found, it returns corresponding MemoryQueueDetails object.
        /// </summary>
        /// <param name="transportSection">MemoryQueue section</param>
        /// <param name="transportName">name of the transport</param>
        private MemoryQueueDetails ValidateTransportName(Infosys.Lif.LegacyIntegratorService.MemoryQueue transportSection, string transportName)
        {
            MemoryQueueDetails memoryQueueDetails = null;
            bool isTransportNameExists = false;
            // Find the MemoryQueue region to which it should connect for sending message.
            for (int count = 0; count < transportSection.MemoryQueueDetailsCollection.Count; count++)
            {
                memoryQueueDetails = transportSection.MemoryQueueDetailsCollection[count] as MemoryQueueDetails;
                if (memoryQueueDetails.TransportName == transportName)
                {
                    isTransportNameExists = true;
                    break;
                }
            }
            // If MemoryQueue region is not set in the config then throw the exception
            if (!isTransportNameExists)
            {
                throw new LegacyException(transportName + " is not defined in MemoryQueueDetails section");
            }
            return memoryQueueDetails;
        }

        /// <summary>
        /// Constructs the response to be sent back to the client calling the Receive operation
        /// </summary>
        /// <param name="msg">the old message read from the queue</param>
        /// <param name="messageId">the Id of the newly added message</param>
        /// <returns>response to be sent back</returns>
        private ReceiveEventArgs ConstructResponse(string msg,string newMessageId)
        {
            ReceiveEventArgs args = new ReceiveEventArgs();
            args.ResponseDetails = new ListDictionary();
            if (msg != null)
            {                
                args.ResponseDetails.Add("MessageBody", msg);
            }
            args.ResponseDetails.Add("MessageIdentifier", newMessageId);
            args.ResponseDetails.Add("Status", response);
            //assign the Status Code based on the "response"
            if (response == SUCCESSFUL_PEEK_MESSAGE || response == SUCCESSFUL_RECEIVE_MESSAGE)
                args.ResponseDetails.Add("StatusCode", SUCCESSFUL_STATUS_CODE);
            else
                args.ResponseDetails.Add("StatusCode", UNSUCCESSFUL_STATUS_CODE);
            return args;
        }

        private string HandleMessage(MemoryQueueOperationType operation, MemoryQueueDetails memoryQueueDetails, MemoryQueueMessage message,
            ConcurrentQueue<MemoryQueueMessage> queue)
        {
            LifLogHandler.LogDebug("MemoryQueue Adapter- Handle Message called for operation of type- " + operation.ToString(), LifLogHandler.Layer.IntegrationLayer);

            try
            {
                switch (operation)
                {
                    case MemoryQueueOperationType.Send:
                        //check the MemoryQueue send pattern configured
                        switch (memoryQueueDetails.SendPattern)
                        {
                            case MemoryQueueSendPattern.None:
                                LifLogHandler.LogDebug("MemoryQueue Adapter (transport- {0})- Send pattern configured- None", 
                                    LifLogHandler.Layer.IntegrationLayer, memoryQueueDetails.TransportName);
                                queue.Enqueue(message);                               
                                response = SUCCESSFUL_SENT_MESSAGE;
                                break;
                            case MemoryQueueSendPattern.RoundRobin:
                                LifLogHandler.LogDebug("MemoryQueue Adapter (RoundRobin, transport- {0})- Send pattern configured- RoundRobin",
                                    LifLogHandler.Layer.IntegrationLayer, memoryQueueDetails.TransportName);
                                queue.Enqueue(message);
                                response = SUCCESSFUL_SENT_MESSAGE;
                                break;
                            case MemoryQueueSendPattern.QueueLoad:
                                LifLogHandler.LogDebug("MemoryQueue Adapter (QueueLoad, transport- {0})- Send pattern configured- QueueLoad", 
                                    LifLogHandler.Layer.IntegrationLayer, memoryQueueDetails.TransportName);
                                queue.Enqueue(message);
                                response = SUCCESSFUL_SENT_MESSAGE;
                                break;
                            case MemoryQueueSendPattern.BroadCast:
                                LifLogHandler.LogDebug("MemoryQueue Adapter (transport- {0})- Send pattern configured- BroadCast", 
                                    LifLogHandler.Layer.IntegrationLayer, memoryQueueDetails.TransportName);
                                queue.Enqueue(message);
                                BroadCastMessageToSecondaryQueues(memoryQueueDetails, message);
                                response = SUCCESSFUL_SENT_MESSAGE;
                                break;
                            default:
                                LifLogHandler.LogDebug("MemoryQueue Adapter- Right Send pattern configuration is missing, hence considering - None",
                                    LifLogHandler.Layer.IntegrationLayer);
                                queue.Enqueue(message);
                                response = SUCCESSFUL_SENT_MESSAGE;
                                break;
                        }
                        break;
                    case MemoryQueueOperationType.Receive:
                        if (memoryQueueDetails.QueueReadingMode == MemoryQueueReadMode.Async)
                        {
                            try
                            {
                                LifLogHandler.LogDebug("MemoryQueue Adapter- Receive in ASYNC mode is requested", LifLogHandler.Layer.IntegrationLayer);
                                string queueName = memoryQueueDetails.QueueName;
                                while (true)
                                {
                                    MemoryQueueMessage messageDetails = null;
                                    bool hasMessage = queue.TryDequeue(out messageDetails);                                    
                                    if (hasMessage && messageDetails != null)
                                    {
                                        response = SUCCESSFUL_RECEIVE_MESSAGE;
                                        ReceiveMessageForAsync(messageDetails, memoryQueueDetails, queue);
                                    }
                                    if (!memoryQueueDetails.ContinueToReceive)
                                        break;
                                    Thread.Sleep(memoryQueueDetails.PollingRestDuration);

                                }
                                //LifLogHandler.LogDebug("MemoryQueue Adapter- Receive in ASYNC mode after while loop,  Queue Name {0} ",
                                //    LifLogHandler.Layer.IntegrationLayer,  memoryQueueDetails.QueueName);
                            }
                            catch (Exception ex)
                            {
                                LifLogHandler.LogError("MemoryQueue Adapter- Receive in ASYNC mode, unexpected Exception occured. " +
                                    " Queue Name {0} , Exception Message: {1} and Exception StackTrace: {2}",
                                    LifLogHandler.Layer.IntegrationLayer,  memoryQueueDetails.QueueName, ex.Message, ex.StackTrace);
                                throw ex;
                            }

                        }
                        else if (memoryQueueDetails.QueueReadingMode == MemoryQueueReadMode.Sync)
                        {
                            LifLogHandler.LogDebug("MemoryQueue Adapter- Receive in SYNC mode is requested", LifLogHandler.Layer.IntegrationLayer);
                            string queueName = memoryQueueDetails.QueueName;
                            while (true)
                            {
                                    MemoryQueueMessage messageDetails = null;                               
                                    bool hasMessage = queue.TryDequeue(out messageDetails);                                    
                                    if (hasMessage && messageDetails != null)
                                    {
                                        response = SUCCESSFUL_RECEIVE_MESSAGE;
                                        ProcessReceivedMessage(messageDetails, memoryQueueDetails, queue);
                                    }

                                if (!memoryQueueDetails.ContinueToReceive)
                                    break;
                                Thread.Sleep(memoryQueueDetails.PollingRestDuration);

                            }

                        }
                        break;
                    case MemoryQueueOperationType.Peek:
                        if (memoryQueueDetails.QueueReadingMode == MemoryQueueReadMode.Async)
                        {
                            response = SUCCESSFUL_PEEK_MESSAGE;

                        }
                        else if (memoryQueueDetails.QueueReadingMode == MemoryQueueReadMode.Sync)
                        {
                            response = SUCCESSFUL_PEEK_MESSAGE;
                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                response = ex.Message;
                if (ex.InnerException != null)
                    response = response + ".Inner Exception- " + ex.InnerException.Message;
                LifLogHandler.LogError("MemoryQueue Adapter- HandleMessage(outer) FAILED for- {0}. Reason- {1}",
                    LifLogHandler.Layer.IntegrationLayer, operation.ToString(), response);
            }
            return response;
        }

        /// <summary>
        /// Process the Received message. 
        /// Check if the message id exists in the messagesToDelete. If Yes remove the message id from the messagesToDelete
        /// Becasue the message is already processed successfully, so no need to process it again
        /// If the messgae id does not exit in the messagesToDelete, delete operation did not call, so we need to process the message again.
        /// Need to consutruct the message and send it back to Queue
        /// Incase of exeception we need to consutruct the message and send it back to Queue
        /// </summary>
        /// <param name="queueMessage">the message received</param>
        /// <param name="memoryQueueDetails">the MemoryQueue details</param>   
        /// <param name="queue">queue details</param>  
        private void ProcessReceivedMessage(MemoryQueueMessage queueMessage, MemoryQueueDetails memoryQueueDetails, ConcurrentQueue<MemoryQueueMessage> queue)
        {
            bool isMessageToBeResend = false;
            string queueName = memoryQueueDetails.QueueName;
            try
            {
                ReceiveEventArgs args = ConstructResponse(queueMessage.Body, queueMessage.Id);
                if (Received != null)
                {
                    Received(args);
                }
            }
            catch (Exception ex)
            {
                isMessageToBeResend = true;
                //do nothing          
                LifLogHandler.LogError("MemoryQueue Adapter ProcessReceivedMessage method FAILED for " +
                    " QueueName {0}. Exception Message: {1} and Exception StackTrace: {2}",
                LifLogHandler.Layer.IntegrationLayer, queueName, ex.Message, ex.StackTrace);

            }

            // Check if the message id exists in the messagesToDelete. If Yes remove the message id from the messagesToDelete
            // Becasue the message is already processed successfully, so no need to process it again
            // If the messgae id does not exit in the messagesToDelete, delete operation did not call, so we need to process the message again.
            // Need to consutruct the message and send it to Queue
            if (messagesToDelete.ContainsKey(queueMessage.Id))
            {
                LifLogHandler.LogDebug("MemoryQueue Adapter ProcessReceivedMessage- Message remvoed from messagesToDelete" +
                    " QueueName {0},Message Id {1}",
                    LifLogHandler.Layer.IntegrationLayer, queueName, queueMessage.Id);
                lock (messagesToDeleteForLock)
                {
                    messagesToDelete.Remove(queueMessage.Id);
                }                    
               // Console.WriteLine("Message remvoed from messagesToDelete :" + queueMessage.Id);
            }
            else
            {
                LifLogHandler.LogDebug("MemoryQueue Adapter ProcessReceivedMessage- Message not exist in messagesToDelete" +
                    " QueueName {0},Message Id {1}",
                    LifLogHandler.Layer.IntegrationLayer, queueName, queueMessage.Id);
               // Console.WriteLine("Message not exist in messagesToDelete :" + queueMessage.Id);
                isMessageToBeResend = true;
            }

            if (isMessageToBeResend && memoryQueueDetails.MessageProcessingMaxCount > 0)
            {              
                LifLogHandler.LogDebug("MemoryQueue Adapter ProcessReceivedMessage- Resending the message, QueueName {0},Message Id {1}",
                    LifLogHandler.Layer.IntegrationLayer, queueName, queueMessage.Id);
                // We need to increament the DeQueue count to next value 
                HandleMessageReappearance(queueMessage, memoryQueueDetails, queue);
            }

        }

        /// <summary>
        /// Process the Received message in Asynchronously.        
        /// </summary>
        /// <param name="queueMessage">the message received</param>
        /// <param name="memoryQueueDetails">the MemoryQueue details</param>   
        /// <param name="queue">queue details</param>   
        private void ReceiveMessageForAsync(MemoryQueueMessage message, MemoryQueueDetails memoryQueueDetails, 
            ConcurrentQueue<MemoryQueueMessage> queue)
        {
            // Async Process
            Task.Factory.StartNew(() => {
                ProcessReceivedMessage(message, memoryQueueDetails, queue);
            });            
           
        }

        /// <summary>
        /// Handles the re-appearance of the message.
        /// Also takes care of moving the message to the poison queue if the maximum number of allowed read has happened.
        /// </summary>
        /// <param name="queueMessage">the message peeked/received</param>
        /// <param name="memoryQueueDetails">the MemoryQueue details</param>   
        /// <param name="queue">queue details</param>   
        private void HandleMessageReappearance(MemoryQueueMessage queueMessage, MemoryQueueDetails memoryQueueDetails, ConcurrentQueue<MemoryQueueMessage> queue)
        {
            LifLogHandler.LogDebug("MemoryQueue Adapter- HandleMessageReappearance is called", LifLogHandler.Layer.IntegrationLayer);
            try
            {
                //Console.WriteLine("messageLabel" + queueMessage.Label);
                string[] labelParts = queueMessage.Label.Split('$');
                if (labelParts.Length >= 2)
                {
                    int dequeueCount = int.Parse(labelParts[1]);
                    LifLogHandler.LogDebug("MemoryQueue Adapter- HandleMessageReappearance and dequeueCount {0}",
                          LifLogHandler.Layer.IntegrationLayer, dequeueCount);
                    dequeueCount++;
                    //check if the maximum dequeue count has reached    
                   if (dequeueCount >= memoryQueueDetails.MessageProcessingMaxCount)
                    {
                        LifLogHandler.LogDebug("MemoryQueue Adapter- HandleMessageReappearance - the maximum dequeue count has reached, " +
                            "so the message will be moved to the poison/dead-letter queue", LifLogHandler.Layer.IntegrationLayer);
                        //create a queue to the poison queue
                    } else
                    {
                        string messageLabel = labelParts[0] + "$" + (dequeueCount).ToString();
                        MemoryQueueMessage newMesaage = ConstructMessage(queueMessage.Body, messageLabel);
                       // Console.WriteLine("New messageLabel" + messageLabel);
                        // Resend to Queue
                        queue.Enqueue(newMesaage);
                    }

                    
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                if (ex.InnerException != null)
                    error = error + ".Inner Exception- " + ex.InnerException.Message;
                LifLogHandler.LogError("MemoryQueue Adapter- HandleMessageReappearance FAILED. Reason- {0}", LifLogHandler.Layer.IntegrationLayer, error);
                throw ex;
            }

        }

        /// <summary>
        /// BroadCast the messge to secondary queues.
        /// It will send same message into multiple queues
        /// </summary>
        /// <param name="memoryQueueDetails">MemoryQueue Details</param>
        /// <param name="message">Message to be send</param>
        private void BroadCastMessageToSecondaryQueues(MemoryQueueDetails memoryQueueDetails, MemoryQueueMessage message)
        {
            LifLogHandler.LogDebug("MemoryQueue Adapter -  Broadcast the message to secondary queues ", LifLogHandler.Layer.IntegrationLayer);            
            try
            {
                if (!string.IsNullOrEmpty(memoryQueueDetails.SecondaryQueues))
                {
                    string[] alternateQueues = memoryQueueDetails.SecondaryQueues.Split(';');
                    for (int i = 0; i < alternateQueues.Length; i++)
                    {
                        string secondaryQueueName = alternateQueues[i];
                        MemoryQueueMessage newMessage = ConstructMessage(message.Body, message.Label);
                        ConcurrentQueue<MemoryQueueMessage> secondaryQueue = getQueueDetails(secondaryQueueName);
                        secondaryQueue.Enqueue(newMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                if (ex.InnerException != null)
                    error = error + ".Inner Exception- " + ex.InnerException.Message;
                LifLogHandler.LogError("MemoryQueue Adapter- BroadCastMessageToSecondaryQueues FAILED. Reason- {0}", LifLogHandler.Layer.IntegrationLayer, error);
                throw ex;
            }
        }



    }
}
