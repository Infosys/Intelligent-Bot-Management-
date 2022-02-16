﻿/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Infosys.Solutions.Superbot.Resource.Entity.Queue
{
    public class Notification
    {
        public int ObservationId { get; set; }
        public int PlatformId { get; set; }
        public string ResourceId { get; set; }
        public int ResourceTypeId { get; set; }
        public int ObservableId { get; set; }
        public string ObservableName { get; set; }
        public string ObservationStatus { get; set; }
        public string Value { get; set; }
        public string ThresholdExpression { get; set; }
        public string ServerIp { get; set; }
        public string ObservationTime { get; set; }
        public string Description { get; set; }
        public string EventType { get; set; }
        public string Source { get; set; }
        public int TenantId { get; set; }
        public int Type { get; set; }
        public int Channel { get; set; }
        public string BaseURL { get; set; }

        //newly added
        public string ConfigId { get; set; }
        public string PortfolioId { get; set; }
        public string TransactionId { get; set; }
        public string ApplicationName { get; set; }
    }
}
