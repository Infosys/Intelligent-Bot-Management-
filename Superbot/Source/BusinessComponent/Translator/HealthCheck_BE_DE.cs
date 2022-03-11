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
using DE = Infosys.Solutions.Superbot.Resource.Entity;

namespace Infosys.Solutions.Ainauto.Superbot.BusinessComponent.Translator
{
    public class HealthCheck_BE_DE
    {
        public static DE.healthcheck_iteration_tracker HealthCheckTrackerBEtoDE(int platformId, int platformResourceModelVersion, string healthcheckSource, string ipIaddress)
        {
            DE.healthcheck_iteration_tracker healthcheckTracker = new DE.healthcheck_iteration_tracker();
            //healthcheckTracker.TrackingId = 1;
            healthcheckTracker.Status = (int)HealthcheckStatus.initiated;//Enum.GetName(typeof(HealthcheckStatus), "Initiated");
            healthcheckTracker.StartTime = DateTime.UtcNow;
            healthcheckTracker.PlatformId  = platformId;
            healthcheckTracker.PlatformModelVersion = platformResourceModelVersion;
            healthcheckTracker.HealthcheckSource = healthcheckSource;
            healthcheckTracker.IpAddress = ipIaddress;
            return healthcheckTracker;
        }

        public static DE.healthcheck_iteration_tracker_details ObservableHealthCheckBEtoDE(string healtcheckTrackingId, string serverid, string observableid, string scriptexecutedtransactionid, string status, string healthCheckType,string error)
        {
            DE.healthcheck_iteration_tracker_details healthcheckTrackerDetails = new DE.healthcheck_iteration_tracker_details();
            healthcheckTrackerDetails.HealthcheckTrackingId = healtcheckTrackingId;
            healthcheckTrackerDetails.Status = (int)Enum.Parse(typeof(HealthcheckStatus), status);
            healthcheckTrackerDetails.StartTime = DateTime.UtcNow;
            healthcheckTrackerDetails.ResourceId = serverid;
            healthcheckTrackerDetails.ObservableId =Convert.ToInt32(observableid);
            healthcheckTrackerDetails.SeeTransactionId = Guid.Parse(scriptexecutedtransactionid);
            healthcheckTrackerDetails.HealthcheckSource = healthCheckType;
            healthcheckTrackerDetails.Error = error;

            return healthcheckTrackerDetails;
        }
    }
}
