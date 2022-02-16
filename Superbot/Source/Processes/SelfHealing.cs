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
using Infosys.Solutions.Ainauto.Infrastructure.ProcessScheduler.Framework;
using QueueEntity = Infosys.Solutions.Superbot.Resource.Entity.Queue;
using Infosys.Solutions.Superbot.Infrastructure.Common;
using Infosys.Solutions.Ainauto.Superbot.BusinessComponent;
using Infosys.Solutions.Ainauto.Superbot.BusinessComponent.Translator;
using BE = Infosys.Solutions.Ainauto.BusinessEntity;
using DE = Infosys.Solutions.Superbot.Resource.Entity;
using Infosys.Solutions.Ainauto.Resource.DataAccess;
using DE1 = Infosys.Solutions.Superbot.Resource.Entity.Queue;
using Infosys.Solutions.Ainauto.Resource.DataAccess.Queue;
using System.Reflection;

namespace Infosys.Solutions.Ainauto.Processes
{
    public class SelfHealing : ProcessHandlerBase<QueueEntity.Anomaly>
    {
        public override void Dump(QueueEntity.Anomaly anomaly)
        {
           
            //var formatString = string.Join("\n", new string[]{
            //    "Request to assemble presentation with following details:",
            //    "CompanyId : {0}",
            //    "Presentation Id: {1}",
            //    "PartitionKey: {2}",
            //    "Row Key: {3}",
            //    "Presentation File Name: {4}",
            //    "Action: {5}",
            //    "MyStuffRowKey: {6}",
            //    "Site: {7}"
            //});

            //LogHandler.LogDebug(formatString, LogHandler.Layer.WorkerHost, message.CompanyId, message.PresentationId,
            //    message.PartitionKey, message.RowKey, message.PresentationName, message.Action, message.MyStuffRowKey,
            //    message.Site);
        }

        public override bool Process(QueueEntity.Anomaly message, int robotId, int runInstanceId, int robotTaskMapId)
        {
            LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_Start,"Process","SelfHealing"),LogHandler.Layer.Business,null);
            LogHandler.LogDebug("Executing the Process method of SelfHealing class with parameters: Anamoly message={0}; robotId={1} ; runInstanceId={2} ; robotTaskMapId={3}",
                            LogHandler.Layer.Business, message,robotId,runInstanceId,robotTaskMapId);

            try
            {

                using (LogHandler.TraceOperations("SelfHealing:Process", LogHandler.Layer.Business, Guid.NewGuid(), null))
                {
                    LogHandler.LogDebug("iterating through all the properties of anomaly message to check for any null or empty values",
                            LogHandler.Layer.Business, null);
                    foreach (PropertyInfo pi in message.GetType().GetProperties())
                    {
                        LogHandler.LogDebug("checking null or empty values for the property: {0}",
                            LogHandler.Layer.Business, pi.Name);
                        if (pi.PropertyType == typeof(string))
                        {
                            string value = (string)pi.GetValue(message);
                            if (string.IsNullOrEmpty(value) && (pi.Name != "Description"))
                            {
                                LogHandler.LogDebug("the value for the property :{0} is null or empty. value: {1}",
                                    LogHandler.Layer.Business, pi.Name,value);
                                LogHandler.LogError(String.Format(ErrorMessages.Value_NullOrEmpty_Error, pi.Name), LogHandler.Layer.Business, null);

                                //throwing exception for null or empty values in anamoly message                                
                                SuperbotValidationException exception = new SuperbotValidationException(String.Format(ErrorMessages.Value_NullOrEmpty_Error, pi.Name));
                                List<ValidationError> validationErrors_List = new List<ValidationError>();
                                ValidationError validationErr = new ValidationError();
                                validationErr.Code = "1044";
                                validationErr.Description = string.Format(ErrorMessages.Value_NullOrEmpty_Error, pi.Name);
                                validationErrors_List.Add(validationErr);

                                if (validationErrors_List.Count > 0)
                                {
                                    exception.Data.Add("ValueNullorEmptyErrors", validationErrors_List);
                                    throw exception;
                                }                                                          
                                
                            }
                        }
                    }


                    //converting Anamoly object from DE to BE                   
                    BE.Anomaly receivedMessage = new BE.Anomaly();
                    AnamolyTranslator anamolyTranslator = new AnamolyTranslator();
                    receivedMessage = anamolyTranslator.AnamolyDEToBE(message);

                    BE.RemediationPlan remediationPlan = new BE.RemediationPlan();
                    Superbot.BusinessComponent.Action action = new Superbot.BusinessComponent.Action();
                    
                    remediationPlan = action.GetRemediationPlan(receivedMessage);
                    if (remediationPlan != null)
                    {
                        LogHandler.LogDebug("The GetRemediationPlan method of action class returned RemediationPlan= {0} ; for the inputs: anamoly:{1}",
                            LogHandler.Layer.Business,remediationPlan, receivedMessage);

                        bool status = action.RemediateIssue(remediationPlan, receivedMessage);
                        LogHandler.LogDebug("The RemediateIssue method of action class returned {0} ; for the inputs: RemediationPlan:{1}; anamoly:{2}",
                            LogHandler.Layer.Business,status.ToString(), remediationPlan, receivedMessage);
                    }
                    else
                    {                        
                        LogHandler.LogDebug("The GetRemediationPlan method of action class returned null value for the inputs: anamoly:{0}",LogHandler.Layer.Business,receivedMessage);
                        LogHandler.LogError(String.Format(ErrorMessages.Method_Returned_Null, "GetRemediationPlan", "Action","Anamoly Message: "+receivedMessage),
                            LogHandler.Layer.Business, null);

                        //throwing exception for null or empty values in anamoly message                                
                        SuperbotException exception = new SuperbotException(String.Format(ErrorMessages.Method_Returned_Null, "GetRemediationPlan", "Action", "Anamoly Message: " + receivedMessage));
                        List<ValidationError> validationErrors_List = new List<ValidationError>();
                        ValidationError validationErr = new ValidationError();
                        validationErr.Code = "1045";
                        validationErr.Description = string.Format(ErrorMessages.Method_Returned_Null, "GetRemediationPlan", "Action", "Anamoly Message: " + receivedMessage);
                        validationErrors_List.Add(validationErr);

                        if (validationErrors_List.Count > 0)
                        {
                            exception.Data.Add("ValueNullorEmptyErrors", validationErrors_List);
                            throw exception;
                        }
                    }

                    //logging the execution completion
                    LogHandler.LogInfo(String.Format(InfoMessages.Method_Execution_End,"Process","SelfHealing"), LogHandler.Layer.Business, null);               

                    return true;

                }
            }
            catch (Exception exMP)
            {
                LogHandler.LogError("Exception occured in Process method of SelfHealing class Exception : {0} and StackTrace {1} ",
                    LogHandler.Layer.Business, exMP.Message, exMP.StackTrace);
                bool failureLogged = false;

                try
                {
                    Exception ex = new Exception();
                    bool rethrow = ExceptionHandler.HandleException(exMP, ApplicationConstants.SERVICE_EXCEPTIONHANDLING_POLICY, out ex);

                    failureLogged = true;
                    if (rethrow)
                    {                        
                        throw ex;
                    }
                    else
                    {                        
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    LogHandler.LogError("Exception Occured while handling an exception, Exception : {0} and StackTrace {1} ",
                        LogHandler.Layer.Business, ex.Message, ex.StackTrace);

                    if (!failureLogged)
                    {
                        LogHandler.LogDebug(String.Format("Exception Occured while handling an exception. error message: {0}", ex.Message), LogHandler.Layer.Business, null);                        
                    }

                    return false;
                }
            }
        }


    }
}
