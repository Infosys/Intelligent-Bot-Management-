/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Linq;
using System.Text;

using System.Messaging;
using System.Threading;
using Infosys.Lif.LegacyCommon;
using Infosys.Lif.LegacyIntegratorService;
using System;
using System.EnterpriseServices;


namespace Infosys.Lif
{
    public partial class MSMQAdapter
    {
        /// <summary>
        /// Method to handle Send, Receive and Peek operations on the MSMQ (transactional)
        /// </summary>
        /// <param name="operation">the type of operation to be done on the MSMQ, it should be either Send or Received as there is no need for transaction in case of Peek</param>
        /// <param name="msMQDetails">the details related to MSMQ to be used by the MSMQAdapter</param>
        /// <param name="message">the message to be dropped to the MSMQ in case of Send operation type,
        /// for other operation type its value is sent as null</param>
        /// <returns></returns>
        private string HandleTransactionalMessage(MSMQOperationType operation, MSMQDetails msMQDetails, string message)
        {
            LifLogHandler.LogDebug("MSMQ Adapter- Handle Message (transactional) called for operation of type- " + operation.ToString(), LifLogHandler.Layer.IntegrationLayer);
            MessageQueue queue = new MessageQueue();
            // Set the queue's MessageReadPropertyFilter property to enable the
            // message's ArrivedTime property.
            queue.MessageReadPropertyFilter.ArrivedTime = true;

            //set the queue to keep the message even after m/c start-up, this will make the msmq more available and reliable
            queue.DefaultPropertiesToSend.Recoverable = true;

            if (msMQDetails.QueueType == MSMQType.Private)
            {
                //queue.Path = "FormatName:Direct=OS:" + msMQDetails.ServerName + @"\Private$\" + msMQDetails.QueueName;
                if (msMQDetails.ServerName.Contains('.') && msMQDetails.ServerName != ".") //i.e. IP address is given for the server
                    queue.Path = "FormatName:Direct=TCP:" + msMQDetails.ServerName + @"\Private$\" + msMQDetails.QueueName;
                else
                    queue.Path = "FormatName:Direct=OS:" + msMQDetails.ServerName + @"\Private$\" + msMQDetails.QueueName;
                //queue.Path = msMQDetails.ServerName + @"\Private$\" + msMQDetails.QueueName;
            }
            else if (msMQDetails.QueueType == MSMQType.Public)
                queue.Path = msMQDetails.ServerName + @"\" + msMQDetails.QueueName;

            //assign the formatter with the target type of the message body
            ((XmlMessageFormatter)queue.Formatter).TargetTypes = new Type[] { typeof(string) };

            try
            {
                //check if the msmq exists
                //if (MessageQueue.Exists(queue.Path)) //commented as the with new format for the queue path, the check for existence doesnt work
                {
                    switch (operation)
                    {
                        case MSMQOperationType.Send:
                            //set the message dequeue count also
                            queue.Send(message, msMQDetails.MessageLabel + "$0", MessageQueueTransactionType.Single);
                            response = SUCCESSFUL_SENT_MESSAGE;
                            queue.Close();
                            break;
                        case MSMQOperationType.Receive:
                            //threadName = Guid.NewGuid().ToString();
                            tempMsMQDetails = msMQDetails;
                            if (msMQDetails.QueueReadingMode == MSMQReadMode.Async)
                            {
                                LifLogHandler.LogDebug("MSMQ Adapter- Receive in ASYNC mode is requested", LifLogHandler.Layer.IntegrationLayer);

                                ////queue.ReceiveCompleted += new ReceiveCompletedEventHandler(queue_ReceiveCompleted);
                                ////Receive is internally calling Peek but the deletion of message is handled by the framework itself.
                                ////this is to avoid missing the message ina any Unfortunate circumstances.
                                queue.PeekCompleted += new PeekCompletedEventHandler(queue_PeekCompletedForTransactionReceive);

                                while (true)
                                {
                                    queue.BeginPeek(new TimeSpan(msMQDetails.QueueReadTimeout));
                                    if (!msMQDetails.ContinueToReceive)
                                        break;
                                    Thread.Sleep(msMQDetails.PollingRestDuration);
                                }
                                //response = SUCCESSFUL_RECEIVE_MESSAGE;
                            }
                            else if (msMQDetails.QueueReadingMode == MSMQReadMode.Sync)
                            {
                                LifLogHandler.LogDebug("MSMQ Adapter- Receive in SYNC mode is requested", LifLogHandler.Layer.IntegrationLayer);
                                while (true)
                                {
                                    MessageQueueTransaction transac = new MessageQueueTransaction();
                                    try
                                    {
                                        LifLogHandler.LogDebug("MSMQ Adapter- starting transaction", LifLogHandler.Layer.IntegrationLayer);
                                        transac.Begin();

                                        responseMessage = queue.Receive(new TimeSpan(msMQDetails.QueueReadTimeout), transac);
                                        response = SUCCESSFUL_RECEIVE_MESSAGE;
                                        bool messageToBeDeleted = false;
                                        if (responseMessage != null)
                                        {
                                            //check if the message is already requested to be deleted
                                            LifLogHandler.LogDebug("MSMQ Adapter- checking if the delete for the received message is requested", LifLogHandler.Layer.IntegrationLayer);
                                            foreach (string msg in messagesToDelete)
                                            {
                                                if (msg == responseMessage.Id)
                                                {
                                                    messagesToDelete.Remove(msg);
                                                    LifLogHandler.LogDebug("MSMQ Adapter- YES, delete for this message is requested and hence deleted", LifLogHandler.Layer.IntegrationLayer);
                                                    messageToBeDeleted = true;
                                                    if (transac.Status == MessageQueueTransactionStatus.Pending)
                                                    {
                                                        LifLogHandler.LogDebug("MSMQ Adapter- commiting transaction as the message is deleted explicitly", LifLogHandler.Layer.IntegrationLayer);
                                                        transac.Commit();
                                                    }
                                                    break;
                                                }
                                            }
                                            if (!messageToBeDeleted)
                                            {
                                                LifLogHandler.LogDebug("MSMQ Adapter- checking if the message is being processed", LifLogHandler.Layer.IntegrationLayer);
                                                //check if the message is being processed
                                                if (IsBeingProcessed(responseMessage))
                                                {
                                                    LifLogHandler.LogDebug("MSMQ Adapter- YES, message is being processed", LifLogHandler.Layer.IntegrationLayer);
                                                    //check if the processing timeout is reached
                                                    LifLogHandler.LogDebug("MSMQ Adapter- checking if the message processing timeout is reached", LifLogHandler.Layer.IntegrationLayer);
                                                    TimeSpan processedFor = DateTime.Now.Subtract(responseMessage.ArrivedTime);
                                                    double timeInMilliSecond = processedFor.TotalMilliseconds;
                                                    if (timeInMilliSecond >= msMQDetails.MessaseInvisibilityTimeout)
                                                    {
                                                        LifLogHandler.LogDebug("MSMQ Adapter- YES, message processing timeout is reached", LifLogHandler.Layer.IntegrationLayer);
                                                        HandleMessageReappearance(responseMessage, msMQDetails);
                                                        if (transac.Status == MessageQueueTransactionStatus.Pending)
                                                        {
                                                            LifLogHandler.LogDebug("MSMQ Adapter- commiting transaction after successful execution of HandleMessageReappearance", LifLogHandler.Layer.IntegrationLayer);
                                                            transac.Commit();
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //make the message visible
                                                        if (transac.Status == MessageQueueTransactionStatus.Pending)
                                                        {
                                                            LifLogHandler.LogDebug("MSMQ Adapter- putting the message back to the queue (by aborting the transaction) as message processing timeout is not reached", LifLogHandler.Layer.IntegrationLayer);
                                                            transac.Abort();
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //i.e. the message is yet to be processed
                                                    LifLogHandler.LogDebug("MSMQ Adapter- NO, message is yet to be processed", LifLogHandler.Layer.IntegrationLayer);
                                                    string newMessageId = HandleMessageProcessing(responseMessage, msMQDetails);
                                                    if (!string.IsNullOrEmpty(newMessageId))
                                                    {
                                                        ReceiveEventArgs args = ConstructResponse(responseMessage, newMessageId);
                                                        if (Received != null)
                                                        {
                                                            Received(args);
                                                        }                                                        
                                                    }
                                                    if (transac.Status == MessageQueueTransactionStatus.Pending)
                                                    {
                                                        LifLogHandler.LogDebug("MSMQ Adapter- commiting transaction as the message is successfully handled by HandleMessageProcessing (either the message is put back or deleted after successful processing)", LifLogHandler.Layer.IntegrationLayer);
                                                        transac.Commit();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (System.Messaging.MessageQueueException ex)
                                    {
                                        if (transac.Status == MessageQueueTransactionStatus.Pending)
                                            transac.Abort();
                                        if (ex.Message.Contains("Timeout for the requested operation has expired."))
                                        {
                                            //do nothing...
                                        }
                                        else
                                        {
                                            //throw ex;
                                            string error = ex.Message;
                                            if (ex.InnerException != null)
                                                error = error + ".Inner Exception- " + ex.InnerException.Message;
                                            //log error
                                            LifLogHandler.LogError("MSMQ Adapter- HandleMessage(inner) FAILED  for- {0}. Reason- {1}", LifLogHandler.Layer.IntegrationLayer, operation.ToString(), error);
                                        }
                                    }
                                    finally
                                    {
                                        queue.Close();
                                    }
                                    if (!msMQDetails.ContinueToReceive)
                                        break;
                                    Thread.Sleep(msMQDetails.PollingRestDuration);

                                }
                            }
                            break;
                        //case MSMQOperationType.Peek:
                        //    if (msMQDetails.QueueReadingMode == MSMQReadMode.Async)
                        //    {
                        //        queue.PeekCompleted += new PeekCompletedEventHandler(queue_PeekCompleted);
                        //        queue.BeginPeek(new TimeSpan(msMQDetails.QueueReadTimeout));
                        //        response = SUCCESSFUL_PEEK_MESSAGE;
                        //    }
                        //    else if (msMQDetails.QueueReadingMode == MSMQReadMode.Sync)
                        //    {
                        //        responseMessage = queue.Peek(new TimeSpan(msMQDetails.QueueReadTimeout));
                        //        queue.Close();
                        //        response = SUCCESSFUL_PEEK_MESSAGE;

                        //        ReceiveEventArgs args = ConstructResponse(responseMessage, responseMessage.Id);
                        //        if (Received != null)
                        //        {
                        //            Received(args);
                        //        }
                        //    }

                        //    break;
                    }
                }
                //else
                //{
                //    response = QUEUE_NOTFOUND;
                //    ReceiveEventArgs args = ConstructResponse(null);
                //    if (Received != null)
                //    {
                //        Received(args);
                //    }
                //}
            }
            catch (Exception ex)
            {
                queue.Close();
                response = ex.Message;
                if (ex.InnerException != null)
                    response = response + ".Inner Exception- " + ex.InnerException.Message;
                //log error
                LifLogHandler.LogError("MSMQ Adapter- HandleMessage(outer) FAILED for- {0}. Reason- {1}", LifLogHandler.Layer.IntegrationLayer, operation.ToString(), response);
                ReceiveEventArgs args = ConstructResponse(null);
                if (Received != null)
                {
                    Received(args);
                }
                //throw ex;
            }
            return response;
        }

        void queue_PeekCompletedForTransactionReceive(object sender, PeekCompletedEventArgs e)
        {
            MessageQueue queue = ((MessageQueue)sender);
            MessageQueueTransaction transac = new MessageQueueTransaction();
            try
            {
                responseMessage = queue.EndPeek(e.AsyncResult);
                LifLogHandler.LogDebug("MSMQ Adapter (async)- starting transaction", LifLogHandler.Layer.IntegrationLayer);
                transac.Begin();
                responseMessage = queue.Receive(new TimeSpan(tempMsMQDetails.QueueReadTimeout), transac);
                //queue.Close();

                if (responseMessage != null)
                {
                    response = SUCCESSFUL_RECEIVE_MESSAGE;
                    bool messageToBeDeleted = false;
                    //check if the message is already requested to be deleted
                    LifLogHandler.LogDebug("MSMQ Adapter (async)- checking if the delete for the received message is requested", LifLogHandler.Layer.IntegrationLayer);
                    foreach (string msg in messagesToDelete)
                    {
                        if (msg == responseMessage.Id)
                        {
                            messagesToDelete.Remove(msg);
                            LifLogHandler.LogDebug("MSMQ Adapter (async)- YES, delete for this message is requested and hence deleted", LifLogHandler.Layer.IntegrationLayer);
                            messageToBeDeleted = true;
                            if (transac.Status == MessageQueueTransactionStatus.Pending)
                            {
                                LifLogHandler.LogDebug("MSMQ Adapter (async)- commiting transaction as the message is deleted explicitly", LifLogHandler.Layer.IntegrationLayer);
                                transac.Commit();
                            }
                            break;
                        }
                    }
                    if (!messageToBeDeleted)
                    {
                        LifLogHandler.LogDebug("MSMQ Adapter (async)- checking if the message is being processed", LifLogHandler.Layer.IntegrationLayer);
                        //check if the message is being processed
                        if (IsBeingProcessed(responseMessage))
                        {
                            LifLogHandler.LogDebug("MSMQ Adapter (async)- YES, message is being processed", LifLogHandler.Layer.IntegrationLayer);
                            //check if the processing timeout is reached
                            LifLogHandler.LogDebug("MSMQ Adapter (async)- checking if the message processing timeout is reached", LifLogHandler.Layer.IntegrationLayer);
                            TimeSpan processedFor = DateTime.Now.Subtract(responseMessage.ArrivedTime);
                            double timeInMilliSecond = processedFor.TotalMilliseconds;
                            if (timeInMilliSecond >= tempMsMQDetails.MessaseInvisibilityTimeout)
                            {
                                LifLogHandler.LogDebug("MSMQ Adapter (async)- YES, message processing timeout is reached", LifLogHandler.Layer.IntegrationLayer);
                                HandleMessageReappearance(responseMessage, tempMsMQDetails);
                                if (transac.Status == MessageQueueTransactionStatus.Pending)
                                {
                                    LifLogHandler.LogDebug("MSMQ Adapter (async)- commiting transaction after successful execution of HandleMessageReappearance", LifLogHandler.Layer.IntegrationLayer);
                                    transac.Commit();
                                }
                            }
                            else
                            {
                                //make the message visible
                                if (transac.Status == MessageQueueTransactionStatus.Pending)
                                {
                                    LifLogHandler.LogDebug("MSMQ Adapter (async)- putting the message back to the queue (by aborting the transaction) as message processing timeout is not reached", LifLogHandler.Layer.IntegrationLayer);
                                    transac.Abort();
                                }
                            }
                        }
                        else
                        {
                            //i.e. the message is yet to be processed
                            LifLogHandler.LogDebug("MSMQ Adapter (async)- NO, message is yet to be processed", LifLogHandler.Layer.IntegrationLayer);
                            string newMessageId = HandleMessageProcessing(responseMessage, tempMsMQDetails);
                            if (!string.IsNullOrEmpty(newMessageId))
                            {
                                ReceiveEventArgs args = ConstructResponse(responseMessage, newMessageId);
                                if (Received != null)
                                {
                                    Received(args);
                                }
                            }
                            if (transac.Status == MessageQueueTransactionStatus.Pending)
                            {
                                LifLogHandler.LogDebug("MSMQ Adapter (async)- commiting transaction as the message is successfully handled by HandleMessageProcessing (either the message is put back or deleted after successful processing)", LifLogHandler.Layer.IntegrationLayer);
                                transac.Commit();
                            }
                        }
                    }
                }
            }
            catch (System.Messaging.MessageQueueException ex)
            {
                if (transac.Status == MessageQueueTransactionStatus.Pending)
                    transac.Abort();
                if (ex.Message.Contains("Timeout for the requested operation has expired."))
                {
                    //do nothing...
                }
                else
                {
                    response = ex.Message;
                    if (ex.InnerException != null)
                        response = response + ".Inner Exception- " + ex.InnerException.Message;
                    //log error
                    LifLogHandler.LogError("MSMQ Adapter (async)- Receive FAILED. Reason- " + response, LifLogHandler.Layer.IntegrationLayer);                    
                }
            }
            catch (Exception ex)
            {
                if (transac.Status == MessageQueueTransactionStatus.Pending)
                    transac.Abort();
                response = ex.Message;
                if (ex.InnerException != null)
                    response = response + ".Inner Exception- " + ex.InnerException.Message;
                //log error
                LifLogHandler.LogError("MSMQ Adapter- Peek FAILED. Reason- " + response, LifLogHandler.Layer.IntegrationLayer);                
            }
            finally
            {
                queue.Close();
            }
        }

        /// <summary>
        /// Gets the next queue to be populated based on the last queue populated in case of Round Robin sending pattern
        /// </summary>
        /// <param name="primaryQueue">primary queue</param>
        /// <param name="lastQueuePopulated">last queue populated</param>
        /// <param name="secondaryQueues">the secondary queues configured, names separated by ';'</param>
        /// <returns>next queue to be populated</returns>
        private string GetNextQueueTobePopulatedForRoundRobin(string primaryQueue, string lastQueuePopulated, string secondaryQueues)
        {
            LifLogHandler.LogDebug("MSMQ Adapter - getting the next queue to be populated for RoundRobin", LifLogHandler.Layer.IntegrationLayer);
            string nextQueue = "";
            if(string.IsNullOrEmpty(lastQueuePopulated))
                nextQueue = primaryQueue;
            else if (string.IsNullOrEmpty(secondaryQueues))
                nextQueue = string.IsNullOrEmpty(lastQueuePopulated)? primaryQueue : lastQueuePopulated;
            else
            {
                string[] alternateQueues = secondaryQueues.Split(';');
                bool lastQueueIsSecondary = false;
                if (alternateQueues.Length > 0)
                {
                    for(int i =0; i< alternateQueues.Length; i++)
                    {
                        if (alternateQueues[i] == lastQueuePopulated)
                        {
                            lastQueueIsSecondary = true;
                            //if the current queue is the last in the list then return the primary queue
                            //otherwise send the next one
                            if (i + 1 == alternateQueues.Length)
                                nextQueue = primaryQueue;
                            else
                                nextQueue = alternateQueues[i + 1];

                            break;
                        }
                    }
                    //if the last queue is primary then return the first secondary queue
                    if (!lastQueueIsSecondary)
                        nextQueue = alternateQueues[0];
                }
            }

            //even if the next queue is blank then assign the primary queue
            if (string.IsNullOrEmpty(nextQueue))
                nextQueue = primaryQueue;

            LifLogHandler.LogDebug("MSMQ Adapter - next queue to be populated for RoundRobin- " + nextQueue, LifLogHandler.Layer.IntegrationLayer);
            return nextQueue;
        }

        /// <summary>
        /// Gets the next queue to be populated based on the last queue populated in case of Queue Load sending pattern
        /// </summary>
        /// <param name="primaryQueue">primary queue</param>
        /// <param name="lastQueuePopulated">last queue populated</param>
        /// <param name="secondaryQueues">the secondary queues configured, names separated by ';'</param>
        /// <param name="messagePerQueueLimit">the maximum number of messages per queue allowed</param>
        /// <param name="queue">the reference to the queue object for checking the current message count</param>
        /// <returns>next queue to be populated</returns>
        private string GetNextQueueTobePopulatedForQueueLoad(string primaryQueue, string lastQueuePopulated, string secondaryQueues, int messagePerQueueLimit, MessageQueue queue)
        {
            LifLogHandler.LogDebug("MSMQ Adapter - getting the next queue to be populated for QueueLoad", LifLogHandler.Layer.IntegrationLayer);
            string nextQueue = "";
            int totalQueueCount = 0;
            if (messagePerQueueLimit == 0)
                nextQueue = primaryQueue;
            else if (string.IsNullOrEmpty(lastQueuePopulated))
                nextQueue = primaryQueue;
            else if (string.IsNullOrEmpty(secondaryQueues))
                nextQueue = string.IsNullOrEmpty(lastQueuePopulated) ? primaryQueue : lastQueuePopulated;
            else
            {
                string[] alternateQueues = secondaryQueues.Split(';');
                bool lastQueueIsSecondary = false;
                if (alternateQueues.Length > 0)
                {
                    totalQueueCount = alternateQueues.Length + 1; //1 more for primary queue
                    for (int i = 0; i < alternateQueues.Length; i++)
                    {
                        if (alternateQueues[i] == lastQueuePopulated)
                        {
                            lastQueueIsSecondary = true;
                            //if the current queue is the last in the list then return the primary queue
                            //otherwise send the next one
                            if (i + 1 == alternateQueues.Length)
                                nextQueue = primaryQueue;
                            else
                                nextQueue = alternateQueues[i + 1];

                            break;
                        }
                    }
                    //if the last queue is primary then return the first secondary queue
                    if (!lastQueueIsSecondary)
                        nextQueue = alternateQueues[0];
                }
            }

            //even if the next queue is blank then assign the primary queue
            if (string.IsNullOrEmpty(nextQueue))
                nextQueue = primaryQueue;

            //check if the identified queue is already full
            if(messagePerQueueLimit > 0 && !string.IsNullOrEmpty(secondaryQueues))
            {
                queue.Path = queue.Path = queue.Path.Substring(0, queue.Path.LastIndexOf('\\') + 1) + nextQueue;
                if (queue.GetAllMessages().Count() >= messagePerQueueLimit)
                {
                    LifLogHandler.LogDebug("MSMQ Adapter - next queue idenfied for QueueLoad- " + nextQueue + " is already full hence finding the next available queue. If all secondary queues are full, then primary queue is populated by default", LifLogHandler.Layer.IntegrationLayer);
                    totalQueuesTraversed++;
                    if(totalQueuesTraversed < totalQueueCount)
                        nextQueue = GetNextQueueTobePopulatedForQueueLoad(primaryQueue, nextQueue, secondaryQueues, messagePerQueueLimit, queue);
                    else //if all the subsequent queues are full, then return the primary queue
                        nextQueue = primaryQueue;
                }
            }
            LifLogHandler.LogDebug("MSMQ Adapter - next queue to be populated for QueueLoad- " + nextQueue, LifLogHandler.Layer.IntegrationLayer);
            return nextQueue;
        }        
    }
}
