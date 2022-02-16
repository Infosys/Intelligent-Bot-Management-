/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;

using Infosys.Solutions.Ainauto.Infrastructure.ProcessScheduler.Framework;
//Uncomment whend deploying to azure *Start
//using Microsoft.WindowsAzure.ServiceRuntime;
//Uncomment whend deploying to azure *end

namespace Infosys.Solutions.Ainauto.Infrastructure.ProcessScheduler
{
    public class Tasks
    {
        public void InitialiseComponent(int robotId , int runInstanceId , int robotTaskMapId)
        {
            try
            {
                const string PROCESS_STARTMETHOD = "Start";

                string directory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                //string directory = Environment.CurrentDirectory;
                Trace.WriteLine("CodeGenWorker- starting the worker role", "Information");
                string xmlstring = File.ReadAllText(directory + "\\Configuration\\Processes.config");
                Trace.WriteLine("CodeGenWorker- read the Processes.config", "Information");
                Trace.TraceInformation("Contents of file Process.Config ...\n" + xmlstring);

                XmlSerializer xs = new XmlSerializer(typeof(Processes));

                MemoryStream memoryStream = new MemoryStream(SerializationOfProcess.StringToUTF8ByteArray(xmlstring));
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                //XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, new UTF8Encoding());
                Processes processes = (Processes) xs.Deserialize(memoryStream);

                Trace.WriteLine("parsed the processes from xml. Process count: " +
                                    processes.Processes2Execute.Count().ToString(), "Information");

                string driveLetter = "";

                //foreach (ExecutionProcess process in processes.Processes2Execute) {
                for (int procIndex = 0; procIndex < processes.Processes2Execute.Length; procIndex++)
                {
                    var process = processes.Processes2Execute[procIndex];
                    //spawn a new thread for each configured process
                    Trace.WriteLine("Try to search for path : ", Path.Combine(directory, process.Dll));
                    Assembly assembly = Assembly.LoadFile(Path.Combine(directory, process.Dll));

                    Trace.WriteLine(
                        String.Format(
                            "Trying to create object of class '{0}' from assembly '{1}'...",
                            process.FullClassName, process.Dll),
                        "Verbose");

                    object obj = assembly.CreateInstance(process.FullClassName);

                    if (obj == null)
                    {
                        Trace.WriteLine(String.Format(
                            "Type name '{0}' not found in assemble '{1}'.", process.FullClassName, process.Dll),
                            "Error");
                        return;
                    }

                    Trace.WriteLine("Created object of class " + process.FullClassName);
                    // mount drive(s) if Azure runtime is available and drive information is present
                    //Uncomment whend deploying to azure *Start
                    //if (RoleEnvironment.IsAvailable && process.Drives != null)
                    //{
                    //    Trace.WriteLine("Mounting drives for the class...", "Verbose");
                    //    for (int i = 0; i < process.Drives.Count(); i++)
                    //    {
                    //        CloudDriveHandler driveHandler = new CloudDriveHandler();
                    //        //to assign same drive to all the processes running though the same worker role
                    //        if (driveLetter == "" || process.Id.ToLower() == ProcessID.mergeandcompress.ToString())
                    //        {
                    //            driveLetter = driveHandler.GetCloudDrive(process.Drives[i].TypeOfDrive.ToString(), process.Drives[i].Vhd);
                    //            //add the extra "\" after the drive if not present, needed in case the processing is done thru dev-fabric
                    //            //uncomment this while running locally thru dev-fabric
                    //            if (string.IsNullOrEmpty(driveLetter))
                    //            {
                    //                // failed to retrieve drive letter
                    //                continue;
                    //            }

                    //            string partDrive = driveLetter.Substring(driveLetter.Length - 1, 1);
                    //            if (partDrive != "\\")
                    //                driveLetter = driveLetter + "\\";
                    //        }
                    //        process.Drives[i].Letter = driveLetter;
                    //        Trace.WriteLine("CodeGenWorker- drive mounted-" + driveLetter + ", blob-" + process.Drives[i].Vhd + ", process type-" + process.Id, "Information");
                    //    }
                    //}
                    //else
                    //{
                    //    Trace.WriteLine("No drives to mount.", "Information");
                    //}
                    //Uncomment whend deploying to azure *end

                    MethodInfo method = obj.GetType().GetMethod(PROCESS_STARTMETHOD,
                        new Type[] {
                            typeof(Drive[]), typeof(ModeType), typeof(string), typeof(string), 
                             typeof(int) , typeof(int),typeof(int) });
                    if (method != null)
                    {
                        ThreadStart startMethod = delegate { method.Invoke(obj,
                            new object[] {
                                process.Drives, process.Mode, process.EntityToBeWatched, process.Id, robotId , runInstanceId , robotTaskMapId}); };
                        new Thread(startMethod).Start();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("CodeGenWorker- Exception occured\n" + ex);
            }
        }
    }
}
