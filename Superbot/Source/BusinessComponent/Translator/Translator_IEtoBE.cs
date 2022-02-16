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
using IE = Infosys.Solutions.Ainauto.Services.Superbot.Contracts.Data;
using BE = Infosys.Solutions.Ainauto.BusinessEntity;
using BE2 = Infosys.Solutions.Ainauto.Superbot.BusinessEntity;

namespace Infosys.Solutions.Ainauto.Superbot.BusinessComponent.Translator
{
    public static class Translator_IEtoBE
    {
        public static BE2.EnvironmentScanReportDetails EnvironmentScanAnomalyDetails_BEtoIE(IE.EnvironmentScanReportDetails environmentScanReportDetailsIE)
        {
            BE2.EnvironmentScanReportDetails environmentScanReportDetailsBE = new BE2.EnvironmentScanReportDetails();
            List<BE2.EnvironmentScanAnomalyDetails> environmentScanAnomalyDetailsBE = new List<BE2.EnvironmentScanAnomalyDetails>();

            foreach (var obj in environmentScanReportDetailsIE.EnvironmentScanAnomalyDetails)
            {
                BE2.EnvironmentScanAnomalyDetails environmentScanAnomalyDetailsBEObj = new BE2.EnvironmentScanAnomalyDetails();

                environmentScanAnomalyDetailsBEObj.MetricName = obj.MetricName;
                environmentScanAnomalyDetailsBEObj.MetricId = obj.MetricId;
                environmentScanAnomalyDetailsBEObj.MetricKey = obj.MetricKey;
                environmentScanAnomalyDetailsBEObj.AttributeName = obj.AttributeName;
                environmentScanAnomalyDetailsBEObj.AttributeValueNew = obj.AttributeValueNew;
                environmentScanAnomalyDetailsBEObj.AttributeValueOld = obj.AttributeValueOld;
                environmentScanAnomalyDetailsBEObj.AttributeStatus = obj.AttributeStatus;

                environmentScanAnomalyDetailsBE.Add(environmentScanAnomalyDetailsBEObj);
            }
            environmentScanReportDetailsBE.ResourceName = environmentScanReportDetailsIE.ResourceName;
            environmentScanReportDetailsBE.ResourceTypeName = environmentScanReportDetailsIE.ResourceTypeName;
            environmentScanReportDetailsBE.OldReportDate = environmentScanReportDetailsIE.OldReportDate;
            environmentScanReportDetailsBE.NewReportDate = environmentScanReportDetailsIE.NewReportDate;
            environmentScanReportDetailsBE.PlatformName = environmentScanReportDetailsIE.PlatformName;
            environmentScanReportDetailsBE.TenantName = environmentScanReportDetailsIE.TenantName;
            environmentScanReportDetailsBE.EnvironmentScanAnomalyDetails = environmentScanAnomalyDetailsBE;

            return environmentScanReportDetailsBE;
        }

        public static BE2.DeviceList UiPathDeviceList_DeviceList(BE2.UiPathDeviceList list)
        {
            BE2.DeviceList deviceList = new BE2.DeviceList();
            List<BE2.Lists> lists = new List<BE2.Lists>();
            foreach(BE2.UiPathLists obj in list.value)
            {
                BE2.Lists listobj = new BE2.Lists()
                {
                    id = obj.MachineId,
                    type = obj.MachineName,
                    status=obj.State,
                    fullyQualifiedHostName=obj.MachineName,
                    hostName=obj.HostMachineName
                };
                lists.Add(listobj);
            }
            deviceList.list = lists;
            return deviceList;
        }
    }
}
