/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IE = Infosys.Solutions.Ainauto.ConfigurationManagement.API.Models;
using BE = Infosys.Solutions.Ainauto.ConfigurationManager.BusinessEntity;

namespace Infosys.Solutions.Ainauto.ConfigurationManagement.API.Translator
{
    public class AnomalyRulesDetailsBE_IE
    {
        public static IE.AnomalyRulesDetails AnomalyRulesDetails_BE_IE(BE.AnomalyRulesDetails anomalyRulesDetailsBE)
        {
            IE.AnomalyRulesDetails anomalyRulesDetailsIE = new IE.AnomalyRulesDetails();
            anomalyRulesDetailsIE.TenantId = anomalyRulesDetailsBE.TenantId;
            anomalyRulesDetailsIE.PlatformId = anomalyRulesDetailsBE.PlatformId;
            List<IE.AnomalyDetectionRule> anomalyDetectionRulesListIE = new List<IE.AnomalyDetectionRule>();

            foreach (BE.AnomalyDetectionRule objBE in anomalyRulesDetailsBE.AnomalyDetectionRules)
            {
                IE.LogDetail logDetails = new IE.LogDetail();
                logDetails.CreatedBy = objBE.LogDetails.CreatedBy;
                logDetails.CreateDate = objBE.LogDetails.CreateDate;
                logDetails.ModifiedBy = objBE.LogDetails.ModifiedBy;
                logDetails.ModifiedDate = objBE.LogDetails.ModifiedDate;
                logDetails.ValidityEnd = objBE.LogDetails.ValidityEnd;
                logDetails.ValidityStart = objBE.LogDetails.ValidityStart;

                IE.AnomalyDetectionRule objIE = new IE.AnomalyDetectionRule();
                objIE.ResourceId = objBE.ResourceId;
                objIE.ResourceName = objBE.ResourceName;
                objIE.ResourceTypeId = objBE.ResourceTypeId;
                objIE.ResourceTypeName = objBE.ResourceTypeName;
                objIE.ObservableId = objBE.ObservableId;
                objIE.ObservableName = objBE.ObservableName;
                objIE.OperatorId = objBE.OperatorId;
                objIE.Operator = objBE.Operator;
                objIE.LowerThreshold = objBE.LowerThreshold;
                objIE.UpperThreshold = objBE.UpperThreshold;
                objIE.LogDetails = logDetails;

                anomalyDetectionRulesListIE.Add(objIE);

            }
            anomalyRulesDetailsIE.AnomalyDetectionRules = anomalyDetectionRulesListIE;
            return anomalyRulesDetailsIE;
        }

        public static BE.AnomalyRulesDetails AnomalyRulesDetails_IE_BE(IE.AnomalyRulesDetails anomalyRulesDetailsIE)
        {
            BE.AnomalyRulesDetails anomalyRulesDetailsBE = new BE.AnomalyRulesDetails();
            anomalyRulesDetailsBE.TenantId = anomalyRulesDetailsIE.TenantId;
            anomalyRulesDetailsBE.PlatformId = anomalyRulesDetailsIE.PlatformId;
            List<BE.AnomalyDetectionRule> anomalyDetectionRulesListBE = new List<BE.AnomalyDetectionRule>();

            foreach (IE.AnomalyDetectionRule objIE in anomalyRulesDetailsIE.AnomalyDetectionRules)
            {
                BE.LogDetail logDetails = new BE.LogDetail();
                logDetails.CreatedBy = objIE.LogDetails.CreatedBy;
                logDetails.CreateDate = objIE.LogDetails.CreateDate;
                logDetails.ModifiedBy = objIE.LogDetails.ModifiedBy;
                logDetails.ModifiedDate = objIE.LogDetails.ModifiedDate;
                logDetails.ValidityEnd = objIE.LogDetails.ValidityEnd;
                logDetails.ValidityStart = objIE.LogDetails.ValidityStart;

                BE.AnomalyDetectionRule objBE = new BE.AnomalyDetectionRule();
                objBE.ResourceId = objIE.ResourceId;
                objBE.ResourceName = objIE.ResourceName;
                objBE.ResourceTypeId = objIE.ResourceTypeId;
                objBE.ResourceTypeName = objIE.ResourceTypeName;
                objBE.ObservableId = objIE.ObservableId;
                objBE.ObservableName = objIE.ObservableName;
                objBE.OperatorId = objIE.OperatorId;
                objBE.Operator = objIE.Operator;
                objBE.LowerThreshold = objIE.LowerThreshold;
                objBE.UpperThreshold = objIE.UpperThreshold;
                objBE.LogDetails = logDetails;

                anomalyDetectionRulesListBE.Add(objBE);

            }
            anomalyRulesDetailsBE.AnomalyDetectionRules = anomalyDetectionRulesListBE;
            return anomalyRulesDetailsBE;
        }
    }
}