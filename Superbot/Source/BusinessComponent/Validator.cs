/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using Infosys.Solutions.Superbot.Infrastructure.Common;
using DE = Infosys.Solutions.Superbot.Resource.Entity;
using BE = Infosys.Solutions.Ainauto.BusinessEntity;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Infosys.Solutions.Ainauto.Superbot.BusinessComponent
{
    public class Validator
    {
        public string CheckCriteriaVarchar(BE.Metric message, DE.observable_resource_map resultObservableResourceMapDS, DE.@operator operatorObj, string unit, out string state)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "CheckCriteriaVarchar", "Monitor"), LogHandler.Layer.Business, null);
            string criteria = string.Empty;
            state = string.Empty;
            string operator1 = operatorObj.Operator1.ToLower().Trim();
            var queueMetricValue = message.MetricValue;
            var upperThresholdValue = resultObservableResourceMapDS.UpperThreshold;
            String[] upperThresholdValueArr = upperThresholdValue.ToString().ToLower().Split(',');

            switch (operator1)
            {
                case "=":
                    if ((queueMetricValue.Equals(upperThresholdValue, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        //criteria = "Anomaly found. Value " + queueMetricValue + " ("+unit+") not equal to threshold value " + upperThresholdValue + " (" + unit + ")";
                        criteria = String.Format(operatorObj.Rule,queueMetricValue, upperThresholdValue, unit);
                        state = "Critical";
                    }
                    else
                        state = "Healthy";
                    break;
                case "!=":
                    if (!queueMetricValue.Equals(upperThresholdValue, StringComparison.InvariantCultureIgnoreCase))
                    {
                        //criteria = "Anomaly found. Value " + queueMetricValue + " (" + unit + ") equal to threshold value " + upperThresholdValue + " (" + unit + ")";
                        criteria = String.Format(operatorObj.Rule, queueMetricValue, upperThresholdValue, unit);
                        state = "Critical";
                    }
                    else
                        state = "Healthy";
                    break;
                case "in":
                    int pos = Array.IndexOf(upperThresholdValueArr, queueMetricValue.ToLower());
                    if(pos > -1)
                    {
                        //criteria = "Anomaly found. Value " + queueMetricValue + " (" + unit + ") equal to threshold value " + upperThresholdValue + " (" + unit + ")";
                        criteria = String.Format(operatorObj.Rule, queueMetricValue, upperThresholdValue, unit);
                        state = "Healthy";
                    }
                    else
                        state = "Critical";
                    break;
                case "not in":
                    int pos2 = Array.IndexOf(upperThresholdValueArr, queueMetricValue.ToLower());
                    if (pos2 == -1)
                    {
                        //criteria = "Anomaly found. Value " + queueMetricValue + " (" + unit + ") equal to threshold value " + upperThresholdValue + " (" + unit + ")";
                        criteria = String.Format(operatorObj.Rule, queueMetricValue, upperThresholdValue, unit);
                        state = "Critical";
                    }
                    else
                        state = "Healthy";
                    break;

                default:
                    LogHandler.LogWarning(String.Format(ErrorMessages.RemediatioPlan_NotFound, "Operator1", "Operator", "OperatorId=" + operatorObj.OperatorId),
                            LogHandler.Layer.Business, null);
                    break;

                

            }
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "CheckCriteriaVarchar", "Monitor"), LogHandler.Layer.Business, null);
            return criteria;
        }


        


          public string CheckCriteriaInt(BE.Metric message, DE.observable_resource_map resultObservableResourceMapDS, DE.@operator operatorObj, string unit,out string state)
          {
              LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "CheckCriteriaInt", "Monitor"), LogHandler.Layer.Business, null);
              string criteria = string.Empty;
              state= string.Empty;
              var queueMetricValue=0;
              string operator1 = operatorObj.Operator1.ToLower().Trim();
              if (message.MetricValue.Contains("%"))
              {
                  string[] splitval = message.MetricValue.Split('%');
                  var dmetricvalue = Convert.ToDouble(splitval[0]);
                  queueMetricValue = Convert.ToInt32(Math.Round(dmetricvalue));
              }

              else
              {
                  queueMetricValue = Convert.ToInt32(message.MetricValue);
              }
              var lowerThresholdValue = Helper.ConvertToInt(resultObservableResourceMapDS.LowerThreshold);
              var upperThresholdValue = Helper.ConvertToInt(resultObservableResourceMapDS.UpperThreshold);
              switch (operator1)
              {
                  case "=":
                      if (queueMetricValue == upperThresholdValue)
                      {
                          //criteria = "Anomaly found. value " + queueMetricValue + " (" + unit + ") is not equal to the threshold value " + upperThresholdValue +" ("+unit+")";
                          criteria = String.Format(operatorObj.Rule, queueMetricValue, upperThresholdValue, unit);
                          state = "Critical";
                      }
                      else
                          state = "Healthy";
                      break;
                  case "!=":
                      if (queueMetricValue != upperThresholdValue)
                      {
                          //criteria = "Anomaly found. value " + queueMetricValue + " (" + unit + ") is equal to the threshold value " + upperThresholdValue + " (" + unit + ")";
                          criteria = String.Format(operatorObj.Rule, queueMetricValue, upperThresholdValue, unit);
                          state = "Critical";
                      }
                      else
                          state = "Healthy";
                      break;
                  case "<":
                      if (queueMetricValue < lowerThresholdValue)
                      {
                          //criteria = "Anomaly found. Lower threshold breached " + queueMetricValue + " less than threshold value " + lowerThresholdValue;
                          //criteria = "Anomaly found. value " + queueMetricValue + " (" + unit + ") is less than the lower threshold value " + lowerThresholdValue + " (" + unit + ")";
                          criteria = String.Format(operatorObj.Rule, queueMetricValue, lowerThresholdValue, unit);
                          state = "Critical";
                      }
                      else if (queueMetricValue<upperThresholdValue)
                          state = "Warning";
                      else
                          state = "Healthy";
                      break;
                  case "<=":
                      if (queueMetricValue <= lowerThresholdValue)
                      {
                          //criteria = "Anomaly found. Lower threshold breached " + queueMetricValue + " less than or equal to threshold value " + lowerThresholdValue;
                          //criteria = "Anomaly found. value " + queueMetricValue + " (" + unit + ") is less than or equal to the lower threshold value " + lowerThresholdValue + " (" + unit + ")";
                          criteria = String.Format(operatorObj.Rule, queueMetricValue, lowerThresholdValue, unit);
                          state = "Critical";
                      }
                      else if (queueMetricValue <= upperThresholdValue)
                          state = "Warning";
                      else
                          state = "Healthy";
                      break;
                  case ">":
                      if (queueMetricValue > upperThresholdValue)
                      {
                          //criteria = "Anomaly found. Upper threshold breached. " + queueMetricValue + " greater than threshold value " + upperThresholdValue;
                          //criteria = "Anomaly found. value " + queueMetricValue + " (" + unit + ") is greater than the upper threshold value " + upperThresholdValue + " (" + unit + ")";
                          criteria = String.Format(operatorObj.Rule, queueMetricValue, upperThresholdValue, unit);
                          state = "Critical";
                      }
                      else if (queueMetricValue > lowerThresholdValue)
                          state = "Warning";
                      else
                          state = "Healthy";
                      break;
                  case ">=":
                      if (queueMetricValue >= upperThresholdValue)
                      {
                          //criteria = "Anomaly found. Upper threshold breached. " + queueMetricValue + " greater than or Equal to threshold value " + upperThresholdValue;
                          //criteria = "Anomaly found. value " + queueMetricValue + " (" + unit + ") is greater than or Equal to the upper threshold value " + upperThresholdValue + " (" + unit + ")";
                          criteria = String.Format(operatorObj.Rule, queueMetricValue, upperThresholdValue, unit);
                          state = "Critical";
                      }
                      else if (queueMetricValue >= lowerThresholdValue)
                          state = "Warning";
                      else
                          state = "Healthy";
                      break;
                  case "between":
                      if (queueMetricValue >= lowerThresholdValue && queueMetricValue <= upperThresholdValue)
                      {
                          //criteria = "Anomaly found. value " + queueMetricValue + " (" + unit + ") does not lie between lower threshold value " + lowerThresholdValue + " (" + unit + ") and upper threshold value " + upperThresholdValue + " (" + unit + ")";
                          criteria = String.Format(operatorObj.Rule, queueMetricValue, lowerThresholdValue, upperThresholdValue, unit);
                          state = "Critical";
                      }
                      else
                          state = "Healthy";
                      break;
                  default:
                      LogHandler.LogWarning(String.Format(ErrorMessages.RemediatioPlan_NotFound, "Operator1", "Operator", "OperatorId=" + operatorObj.OperatorId),
                              LogHandler.Layer.Business, null);
                      break;
              }
              LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "CheckCriteriaInt", "Monitor"), LogHandler.Layer.Business, null);
              return criteria;
          }
          
        public string CheckCriteriaDateTime(BE.Metric message, DE.observable_resource_map resultObservableResourceMapDS, DE.@operator operatorObj, string unit,out string state)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "CheckCriteriaDateTime", "Monitor"), LogHandler.Layer.Business, null);
            string criteria = string.Empty;
            state = string.Empty;
            DateTime queueMetricValue;
            if (DateTime.TryParse(message.MetricValue, out queueMetricValue))
            {
                DateTime defaultDateTime = new DateTime();
                DateTime upperThresholdDate, lowerThresholdDate;
                if (DateTime.TryParse(resultObservableResourceMapDS.UpperThreshold, out upperThresholdDate))
                {
                    //the threshold values are in DateTime format
                    if (!DateTime.TryParse(resultObservableResourceMapDS.LowerThreshold, out lowerThresholdDate))
                    {
                        //the operator must be =
                    }


                    if (upperThresholdDate.Date == defaultDateTime.Date)
                    {
                        //only Time comparison needs to be done
                        queueMetricValue = defaultDateTime.Date + queueMetricValue.TimeOfDay;
                        criteria = DateTimeComparison(queueMetricValue, lowerThresholdDate, upperThresholdDate, operatorObj,unit,out state);
                    }
                    else if (upperThresholdDate.TimeOfDay == defaultDateTime.TimeOfDay)
                    {
                        // only Date comparison needs to be done
                        queueMetricValue = queueMetricValue.Date + defaultDateTime.TimeOfDay;
                        criteria = DateTimeComparison(queueMetricValue, lowerThresholdDate, upperThresholdDate, operatorObj, unit, out state);
                    }
                    else
                    {
                        //datetime comparison
                        criteria = DateTimeComparison(queueMetricValue, lowerThresholdDate, upperThresholdDate, operatorObj, unit, out state);
                    }
                }
                else
                {
                    //the threshold values are in Segmented format
                    upperThresholdDate = ExtractThresholdValue(resultObservableResourceMapDS.UpperThreshold, queueMetricValue);
                    lowerThresholdDate = ExtractThresholdValue(resultObservableResourceMapDS.LowerThreshold, queueMetricValue);
                    if (upperThresholdDate.TimeOfDay == defaultDateTime.TimeOfDay && lowerThresholdDate.TimeOfDay == defaultDateTime.TimeOfDay)
                        criteria = DateTimeComparison(DateTime.UtcNow.Date, lowerThresholdDate, upperThresholdDate, operatorObj, unit, out state);
                    else
                        criteria = DateTimeComparison(DateTime.UtcNow, lowerThresholdDate, upperThresholdDate, operatorObj, unit, out state);
                }
            }
            else
            {
                LogHandler.LogError(String.Format(ErrorMessages.Data_Format_Error),
                                   LogHandler.Layer.Business, null);
                SuperbotValidationException exception = new SuperbotValidationException(String.Format(ErrorMessages.Data_Format_Error));
                List<ValidationError> validationErrors_List = new List<ValidationError>();
                ValidationError validationErr = new ValidationError();
                validationErr.Code = "1000";
                validationErr.Description = String.Format(ErrorMessages.Data_Format_Error);
                validationErrors_List.Add(validationErr);

                if (validationErrors_List.Count > 0)
                {
                    exception.Data.Add("ValidationErrors", validationErrors_List);
                    throw exception;
                }
            }
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "CheckCriteriaDateTime", "Monitor"), LogHandler.Layer.Business, null);
            return criteria;
        }

        //to extract the segmented threshold values
        public DateTime ExtractThresholdValue(string thresholdValue, DateTime queueMetricValue)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "ExtractThresholdValue", "Monitor"), LogHandler.Layer.Business, null);
            if (thresholdValue != null)
            {
                int mm, yr, dd, hr, min, sec;
                var expression = new Regex(
                @"((?<dd>[0-9]+)dd)?" +
                @"((?<mm>[0-9]+)mm)?" +
                @"((?<yr>[0-9]+)yr)?" +
                @"((?<hr>[0-9]+)hr)?" +
                @"((?<min>[0-9]+)min)?" +
                @"((?<sec>[0-9]+)sec)?"
                );                

                var match = expression.Match(thresholdValue);
                dd = (match.Groups["dd"].ToString()!="")?Convert.ToInt32(match.Groups["dd"].ToString()):0;
                mm = (match.Groups["mm"].ToString() != "") ? Convert.ToInt32(match.Groups["mm"].ToString()) : 0;
                yr = (match.Groups["yr"].ToString() != "") ? Convert.ToInt32(match.Groups["yr"].ToString()) : 0;
                hr = (match.Groups["hr"].ToString() != "") ? Convert.ToInt32(match.Groups["hr"].ToString()) : 0;
                min = (match.Groups["min"].ToString() != "") ? Convert.ToInt32(match.Groups["min"].ToString()) : 0;
                sec = (match.Groups["sec"].ToString() != "") ? Convert.ToInt32(match.Groups["sec"].ToString()) : 0;

                queueMetricValue = queueMetricValue.AddSeconds(sec).AddMinutes(min).AddHours(hr).AddDays(dd).AddMonths(mm).AddYears(yr);
                if (hr == 0 && min == 0 && sec == 0)
                {
                    queueMetricValue = queueMetricValue.Date;
                }
            }
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "ExtractThresholdValue", "Monitor"), LogHandler.Layer.Business, null);
            return queueMetricValue;
        }
        public string DateTimeComparison(DateTime queueMetricValue, DateTime lowerThresholdValue, DateTime upperThresholdValue, DE.@operator operatorObj, string unit,out string state)
        {
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_Start, "DateTimeComparison", "Monitor"), LogHandler.Layer.Business, null);
            string criteria = string.Empty;
            state= string.Empty;
            string operator1 = operatorObj.Operator1.ToLower().Trim();
            switch (operator1)
            {
                case "=":
                    if (queueMetricValue == upperThresholdValue)
                    {
                        //criteria = "Anomaly found. value " + queueMetricValue +" ("+ unit+") is not equal to the threshold value " + upperThresholdValue+" ("+unit+")";
                        criteria = String.Format(operatorObj.Rule, queueMetricValue, upperThresholdValue, unit);
                        state = "Critical";
                    }
                    else
                        state = "Healthy";
                    break;
                case "!=":
                    if (queueMetricValue != upperThresholdValue)
                    {
                        //criteria = "Anomaly found. value " + queueMetricValue + " (" + unit + ") is equal to the threshold value " + upperThresholdValue + " (" + unit + ")";
                        criteria = String.Format(operatorObj.Rule, queueMetricValue, upperThresholdValue, unit);
                        state = "Critical";
                    }
                    else
                        state = "Healthy";
                    break;
                case "<":
                    if (queueMetricValue < lowerThresholdValue)
                    {
                        //criteria = "Anomaly found. Lower threshold breached " + queueMetricValue + " less than threshold value " + lowerThresholdValue;
                        //criteria = "Anomaly found. value " + queueMetricValue + " (" + unit + ") is less than the lower threshold value " + lowerThresholdValue + " (" + unit + ")";
                        criteria = String.Format(operatorObj.Rule, queueMetricValue, lowerThresholdValue, unit);
                        state = "Critical";
                    }
                    else if (queueMetricValue < upperThresholdValue)
                        state = "Warning";
                    else
                        state = "Healthy";
                    break;
                case "<=":
                    if (queueMetricValue <= lowerThresholdValue)
                    {
                        //criteria = "Anomaly found. Lower threshold breached " + queueMetricValue + " less than or equal to threshold value " + lowerThresholdValue;
                        //criteria = "Anomaly found. value " + queueMetricValue + " (" + unit + ") is less than or equal to the lower threshold value " + lowerThresholdValue + " (" + unit + ")";
                        criteria = String.Format(operatorObj.Rule, queueMetricValue, lowerThresholdValue, unit);
                        state = "Critical";
                    }
                    else if (queueMetricValue <= upperThresholdValue)
                        state = "Warning";
                    else
                        state = "Healthy";
                    break;
                case ">":
                    if (queueMetricValue > upperThresholdValue)
                    {
                        //criteria = "Anomaly found. Upper threshold breached. " + queueMetricValue + " greater than threshold value " + upperThresholdValue;
                        //criteria = "Anomaly found. value " + queueMetricValue + " (" + unit + ") is greater than the upper threshold value " + upperThresholdValue + " (" + unit + ")";
                        criteria = String.Format(operatorObj.Rule, queueMetricValue, upperThresholdValue, unit);
                        state = "Critical";
                    }
                    else if (queueMetricValue > lowerThresholdValue)
                        state = "Warning";
                    else
                        state = "Healthy";
                    break;
                case ">=":
                    if (queueMetricValue >= upperThresholdValue)
                    {
                        //criteria = "Anomaly found. Upper threshold breached. " + queueMetricValue + " greater than or Equal to threshold value " + upperThresholdValue;
                        //criteria = "Anomaly found. value " + queueMetricValue + " (" + unit + ") is greater than or Equal to the upper threshold value " + upperThresholdValue + " (" + unit + ")";
                        criteria = String.Format(operatorObj.Rule, queueMetricValue, upperThresholdValue, unit);
                        state = "Critical";
                    }
                    else if (queueMetricValue >= lowerThresholdValue)
                        state = "Warning";
                    else
                        state = "Healthy";
                    break;
                case "between":
                    if (queueMetricValue >= lowerThresholdValue && queueMetricValue < upperThresholdValue)
                    {
                        //criteria = "Anomaly found. value " + queueMetricValue + " (" + unit + ") does not lie between lower threshold value " + lowerThresholdValue + " (" + unit + ") and upper threshold value " + upperThresholdValue + " (" + unit + ")";
                        criteria = String.Format(operatorObj.Rule, queueMetricValue, lowerThresholdValue, upperThresholdValue, unit);
                        state = "Critical";
                    }
                    else
                        state = "Healthy";
                    break;
                default:
                    LogHandler.LogWarning(String.Format(ErrorMessages.RemediatioPlan_NotFound, "Operator1", "Operator", "OperatorId=" + operatorObj.OperatorId),
                            LogHandler.Layer.Business, null);
                    break;
            }
            LogHandler.LogInfo(string.Format(InfoMessages.Method_Execution_End, "DateTimeComparison", "Monitor"), LogHandler.Layer.Business, null);
            return criteria;
        }
    }
}
