/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using System;
using System.Threading;
using Infosys.Lif.LegacyCommon;
using Infosys.Lif.LegacyIntegrator;
using Infosys.Lif.LegacyIntegratorService;
using Infosys.Solutions.Superbot.Infrastructure.Common;

namespace Infosys.Solutions.Ainauto.Infrastructure.ProcessScheduler.Framework
{
    public abstract class ProcessHandlerBase<T> : IProcessHandler where T : class
    {
        protected AdapterManager adapterManager = new AdapterManager();
        protected Drive[] drives;
        protected ModeType mode;
        protected string entityName;
        protected string id;
       
        protected int robotId;
        protected int runInstanceId;
        protected int robotTaskMapId;

        public virtual void Start(
            Drive[] drives,
            ModeType mode,
            string entityName,
            string id,
            int robotId, int runInstanceId, int robotTaskMapId)
        {
            // save parameters in protected members for later use
            this.drives = drives;
            this.mode = mode;
            this.entityName = entityName;
            this.id = id;
           
            this.robotId = robotId;
            this.runInstanceId = runInstanceId;
            this.robotTaskMapId = robotTaskMapId;
            
            LogHandler.LogDebug("PPTWare Windows Service {0} Process Started", LogHandler.Layer.Infrastructure, id);
            // call processing function based on mode type
            switch (mode)
            {
                case ModeType.Queue:

                    // initialize adapter manager - entityName is the region name in LISetting.Config file
                    adapterManager.ResponseReceived +=
                        new AdapterManager.AdapterReceiveHandler(adapterManager_ResponseReceived);

                    
                        LogHandler.LogDebug("PPTWare Windows Service {0} Process receive first message", LogHandler.Layer.Infrastructure, id);
                        try
                        {
                            adapterManager.Receive(this.id);
                        }
                        catch (LegacyException integrationException)
                        {
                           //TODO: Handle exception policy
                            LogHandler.LogError(integrationException.Message,
                                LogHandler.Layer.Business);

                        }

                        break;
                    // imp: above while loop is infinite, hence control will not pass to this point
                    // so no need of break statement
                case ModeType.Table:
                   LogHandler.LogDebug("PPTWare Windows Service {0} Process started. Call Process logic.", LogHandler.Layer.Infrastructure, id);
                    Process(null,0,0,0);
                    break;
            }
        }


        void adapterManager_ResponseReceived(ReceiveEventArgs eventArgs)
        {
            string message ="";
            string messageId = "";

            try
            {
                LogHandler.LogDebug("PPTWare Windows Service {0} Process message received", LogHandler.Layer.Infrastructure, id);
                messageId = eventArgs.ResponseDetails["MessageIdentifier"] as string;
                message = eventArgs.ResponseDetails["MessageBody"] as string;


                if (string.IsNullOrWhiteSpace(message) || string.IsNullOrWhiteSpace(messageId))
                {

                    LogHandler.LogWarning("Empty Message was received", LogHandler.Layer.Infrastructure, messageId);
                    return;
                }
                //using (LogHandler.TraceOperations("Message with identifier {0} to be processed", LogHandler.Layer.Infrastructure, new Guid(messageId), null))
                {
                    // try to deserialize the message
                    T qMessage = Utility.DeserializeFromJSON<T>(message);

                    if (qMessage == null)
                    {
                        LogHandler.LogWarning(
                            "Message {0} deserialized was invalid (null)", LogHandler.Layer.Infrastructure, messageId);
                        return;
                    }

                    LogHandler.LogDebug("Message {0} deserialized", LogHandler.Layer.Infrastructure, messageId);
                    // dump the message into logs
                    Dump(qMessage);
                    LogHandler.LogDebug("Message {0} dumped", LogHandler.Layer.Infrastructure, messageId);

                    // try to process the message
                    if (Process(qMessage , robotId, runInstanceId,robotTaskMapId))
                    {
                        LogHandler.LogDebug("Message {0} processed succesfully", LogHandler.Layer.Infrastructure, messageId);
                        // successfully processed the message, hence delete it
                        adapterManager.Delete(messageId);
                        LogHandler.LogDebug("Message {0} deleted succesfully", LogHandler.Layer.Infrastructure, messageId);
                    }
                    else
                    {
                        LogHandler.LogError("Message {0} failed to be processed", LogHandler.Layer.Infrastructure, messageId);
                        // log error - failed to process message
                        // leave the message to reappear in queue
                    }
                }
            }
            catch (Exception difException)
            {
                try
                {
                    Exception ex = new Exception();
                    bool rethrow = ExceptionHandler.HandleException(difException, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY,out ex);

                    if (rethrow)
                    {
                        throw difException;
                    }
                    else
                    {
                        //Set as a succesfull operation as the message was invalid since an equivalent presentation entity was
                        //not found in the database. This could be a rogue transaction.
                        //returning a true since the message has been sent with invalid presentation id and has to be deleted
                        //to avoid further processing
                        adapterManager.Delete(messageId);
                        LogHandler.LogDebug("Message {0} deleted succesfully", LogHandler.Layer.Infrastructure, messageId);
                    }

                }
                catch (Exception)
                {
                    //Any messages which would have to indicate to the worker process that the transaction has failed
                    // and the messaGE should be retried
                    //return false;
                    //TODO: Any strategy to handle critical , warning or error messages
                    LogHandler.LogDebug("Message {0} failed to be processed", LogHandler.Layer.Infrastructure, messageId);
                    LogHandler.LogError("Message {0} failed to be processed", LogHandler.Layer.Infrastructure, messageId);
                }
            }
        }


        /// <summary>
        /// Implement this function to dump message details in logs for debugging purpose
        /// </summary>
        /// <param name="message">Message to be dumped</param>
        public abstract void Dump(T message);


        /// <summary>
        /// Implement this function to process a message received from queue. This function will be called when a
        /// message is received from queue.
        /// </summary>
        /// <param name="message">Message to be processed</param>
        /// <returns>True, if processed successfully; false, otherwise</returns>
        public abstract bool Process(T message , int robotId, int runInstanceId, int robotTaskMapId );


        //public void Start(Drive[] drives, ModeType mode, string entityName, string id,int robotId, int runInstanceId, int robotTaskMapId)
        //{
        //    Start(drives, mode, entityName, id, robotId,  runInstanceId,  robotTaskMapId);
        //}
    }
}
