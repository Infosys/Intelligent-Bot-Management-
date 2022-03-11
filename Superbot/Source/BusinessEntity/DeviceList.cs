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
    public class DeviceList
    {
        public Page page { get; set; }
        public List<Lists> list { get; set; }
    }
    public class Page
    {
        public string offset { get; set; }
        public string total { get; set; }
        public string totalFilter { get; set; }
    }
    public class Lists
    {
        public string id { get; set; }
        public string type { get; set; }
        public string hostName { get; set; }
        public string userId { get; set; }
        public string userName { get; set; }
        public string status { get; set; }
        public string poolName { get; set; }
        public string fullyQualifiedHostName { get; set; }
       
    }


    public class UiPathDeviceList
    {
        public string context { get; set; }
        public string count { get; set; }
        public List<UiPathLists> value { get; set; }
    }
    public class UiPathLists
    {
        public string HostMachineName { get; set; }
        public string MachineId { get; set; }
        public string MachineName { get; set; }
        public string State { get; set; }
        public string ReportingTime { get; set; }
        public string Info { get; set; }
        public string IsUnresponsive { get; set; }
        public string LicenseErrorCode { get; set; }
        public string OrganizationUnitId { get; set; }
        public string FolderName { get; set; }
        public string Id { get; set; }

    }


    public class UiPathProcessList
    {
        public string context { get; set; }
        public string count { get; set; }
        public List<ProcessList> value { get; set; }
    }
    public class ProcessList
    {
        public string Key { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string State { get; set; }
        public string JobPriority { get; set; }
        public string Source { get; set; }
        public string SourceType { get; set; }
        public string BatchExecutionKey { get; set; }
        public string Info { get; set; }
        public string CreationTime { get; set; }
        public string StartingScheduleId { get; set; }
        public string ReleaseName { get; set; }
        public string Type { get; set; }
        public string InputArguments { get; set; }
        public string OutputArguments { get; set; }
        public string HostMachineName { get; set; }
        public bool HasMediaRecorded { get; set; }
        public string PersistenceId { get; set; }
        public string ResumeVersion { get; set; }
        public string StopStrategy { get; set; }
        public string RuntimeType { get; set; }
        public bool RequiresUserInteraction { get; set; }
        public string ReleaseVersionId { get; set; }
        public string EntryPointPath { get; set; }
        public int Id { get; set; }

    }
}
