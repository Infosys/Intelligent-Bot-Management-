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

namespace Infosys.Solutions.Ainauto.Superbot.BusinessEntity
{
    public class EnvironmentScanComparisonReport2
    {
        public string metricName { get; set; }
        public List<Details> details { get; set; }

    }

    public class Details
    {
        public string resource { get; set; }
        public string resourcetype { get; set; }
        public string SoftwareName { get; set; }
        public string oldversion { get; set; }
        public string oldinstalleddate { get; set; }
        public string oldpublisher { get; set; }
        public string newversion { get; set; }
        public string newinstalleddate { get; set; }
        public string newpublisher { get; set; }
        public string status { get; set; }

    }

    public class OSDetails
    {
        public string metricName { get; set; }
        public List<OSResoutions> details { get; set; }
    }

    public class OSResoutions
    {
        public string resource { get; set; }
        public string resourcetype { get; set; }
        public string name { get; set; }
        public string oldVersion { get; set; }
        public string oldbuildnumber { get; set; }
        public string newVersion { get; set; }
        public string newbuildnumber { get; set; }
        public string status { get; set; }
    }

    public class ScreenResoution
    {
        public string metricName { get; set; }
        public List<SRDetails> details { get; set; }
    }


    public class SRDetails
    {
        public string resource { get; set; }
        public string resourceyype { get; set; }
        public string oldversion { get; set; }
        public string newversion { get; set; }
        public string status { get; set; }


    }

    public class Report
    {
        public string metricName { get; set; }
        public finalReport metricValue { get; set; }
        
    }

    public class finalReport
    {
        public EnvironmentScanComparisonReport2 softwareReport { get; set; }
        public OSDetails osDetailsReport { get; set; }
        public ScreenResoution srDetails { get; set; }
    }
    
    public class ActiveResource
    {
        public string ResourceId { get; set; }
        public string ResourceName { get; set; }
        public int ResourceTypeId { get; set; }
        public string ResourceTypeName { get; set; }
        


    }

   
    public class EnvironmentScanComparisonReport
    {
        public string PlatformId { get; set; }
        public string TenantId { get; set; }
        public string PlatformName { get; set; }
        public string TenantName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public ESResourceDetails ResourceDetails { get; set; }
    }

    public class ESResourceDetails
    {
        public string ResourceId { get; set; }
        public string ResourceName { get; set; }
        public string ResourceTypeId { get; set; }
        public string ResourceTypeName { get; set; }
        public ESMetricDetails MetricDetails { get; set; }
    }
    public class ESMetricDetails
    {
        public string ObservableId { get; set; }
        public string ObservableName { get; set; }
        public List<ESMetricValues> MetricValue { get; set; }
    }
    public class ESMetricValues
    {
        public string MetricId { get; set; }
        public string MetricName { get; set; }
        public string MetricKey { get; set; }
        public string Status { get; set; }
        public List<ESAttributes> Attributes { get; set; }
    }
    public class ESAttributes
    {
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
        public string DisplayName { get; set; }
    }

   
   

    
}
