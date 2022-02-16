/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace Infosys.Solutions.Superbot.Infrastructure.Common
{
    [ConfigurationElementType(typeof(CustomHandlerData))]
    public class HTTPErrorExceptionHandler : IExceptionHandler
    {
        public HTTPErrorExceptionHandler(NameValueCollection ignore)
        {
        }


        public Exception HandleException(Exception exception, Guid handlingInstanceId)
        {
            int statusCode = 500;
            string message = string.Empty;

            //Check if the app has passed custom status code
            if (exception.Data["StatusCode"] != null)
            {
                if (((int)exception.Data["StatusCode"]) < 1000)
                {
                    statusCode = (int)exception.Data["StatusCode"];
                }

                if (!string.IsNullOrWhiteSpace(exception.Data["StatusDescription"] as string))
                {
                    message = exception.Data["StatusDescription"] as string;
                }
                else
                {
                    message = ErrorMessages.ResourceManager.GetString(
                        Enum.GetName(typeof(Errors.ErrorCodes), statusCode));
                }
            }
            else if (!string.IsNullOrWhiteSpace(exception.Message))
            {
                //Incase status code is not set check if an exception messages is passed
                //relevant in cases where an object of type Exception is passed
                message = exception.Message;
            }
            else
            {
                message = ErrorMessages.Standard_Error;
            }

            List<ServiceFaultError> faults = new List<ServiceFaultError>();


            if (exception.GetType() == typeof(SuperbotValidationException))
            {
                CollateFaults(exception, faults);            
            }
            else if (exception.GetType() == typeof(SuperbotSecurityError))
            {
                CollateFaults(exception, faults);
                WebFaultException<List<ServiceFaultError>> unauthorized =
                    new WebFaultException<List<ServiceFaultError>>(faults, System.Net.HttpStatusCode.BadRequest);
                return unauthorized;
            }
            else
            {

                ServiceFaultError fault = new ServiceFaultError
                {
                    Message = message,
                    ErrorCode = statusCode
                };
                faults.Add(fault);
            }


            WebFaultException<List<ServiceFaultError>> webFault = new WebFaultException<List<ServiceFaultError>>(faults, System.Net.HttpStatusCode.InternalServerError);
            return webFault;
        }

        private void CollateFaults(Exception exception, List<ServiceFaultError> faults)
        {
            List<ValidationError> validationErrors = new List<ValidationError>();
            if (exception.Data["ValidationErrors"] != null)
            {
                validationErrors = exception.Data["ValidationErrors"] as List<ValidationError>;

                for (int iCount = 0; iCount < validationErrors.Count; iCount++)
                {
                    ServiceFaultError fault = new ServiceFaultError
                    {
                        Message = validationErrors[iCount].Description,
                        ErrorCode = Convert.ToInt32(validationErrors[iCount].Code)
                    };
                    faults.Add(fault);
                }
            }
        }
    }
    public class ServiceFaultError
    {
        public string Message { get; set; }
        public int ErrorCode { get; set; }

    }
    public class ValidationError
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Key { get; set; }
    }
    public class SuperbotSecurityError
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string Key { get; set; }
    }
}

