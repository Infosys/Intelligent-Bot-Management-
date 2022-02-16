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
    public class EnvironmentScanReportDetails
    {
        public string ResourceName { get; set; }
        public string ResourceTypeName { get; set; }
        public string OldReportDate { get; set; }
        public string NewReportDate { get; set; }
        public string PlatformName { get; set; }
        public string TenantName { get; set; }
        public List<EnvironmentScanAnomalyDetails> EnvironmentScanAnomalyDetails { get; set; }
    }
    public class EnvironmentScanAnomalyDetails
    {
        public string MetricName { get; set; }
        public int MetricId { get; set; }
        public string MetricKey { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValueNew { get; set; }
        public string AttributeValueOld { get; set; }
        public string AttributeStatus { get; set; }
    }
        //public class EnvironmentScanAnomalyDetails
        //{
        //    public List<ESInstalledSoftware> InstalledSoftware { get; set; }
        //    public ESOSDetails OSDetails { get; set; }
        //    public ESScreenResolution ScreenResolution { get; set; }
        //}
        //public class ESInstalledSoftware
        //{
        //    public string SoftwareNameOld { get; set; }
        //    public string SoftwareVersionOld { get; set; }
        //    public string InstalledDateOld { get; set; }
        //    public string PublisherOld { get; set; }
        //    public string Status { get; set; }
        //    public string SoftwareNameNew { get; set; }
        //    public string SoftwareVersionNew { get; set; }
        //    public string InstalledDateNew { get; set; }
        //    public string PublisherNew { get; set; }

        //}
        //public class ESOSDetails
        //{
        //    public string OSNameOld { get; set; }
        //    public string SystemTypeOld { get; set; }
        //    public string VersionOld { get; set; }
        //    public string BuildNumberOld { get; set; }
        //    public string Status { get; set; }
        //    public string OSNameNew { get; set; }
        //    public string SystemTypeNew { get; set; }
        //    public string VersionNew { get; set; }
        //    public string BuildNumberNew { get; set; }
        //}
        //public class ESScreenResolution
        //{
        //    public string HeightOld { get; set; }
        //    public string WidthOld { get; set; }
        //    public string Status { get; set; }
        //    public string HeightNew { get; set; }
        //    public string WidthNew { get; set; }

        //}
    }
