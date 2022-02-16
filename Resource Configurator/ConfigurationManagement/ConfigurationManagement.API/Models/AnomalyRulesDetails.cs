/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Infosys.Solutions.Ainauto.ConfigurationManagement.API.Models
{
    public class AnomalyRulesDetails
    {
        public int PlatformId { get; set; }
        public int TenantId { get; set; }
        public List<AnomalyDetectionRule> AnomalyDetectionRules { get; set; }
    }

    public class AnomalyDetectionRule
    {
        public string ResourceId { get; set; }
        public string ResourceName { get; set; }
        public int ResourceTypeId { get; set; }
        public string ResourceTypeName { get; set; }
        public int ObservableId { get; set; }
        public string ObservableName { get; set; }
        public int OperatorId { get; set; }
        public string Operator { get; set; }
        public string LowerThreshold { get; set; }
        public string UpperThreshold { get; set; }
        public LogDetail LogDetails { get; set; }
    }
    public class LogDetail
    {
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime CreateDate { get; set; }
        public Nullable<DateTime> ModifiedDate { get; set; }
        public DateTime ValidityStart { get; set; }
        public DateTime ValidityEnd { get; set; }
    }
}