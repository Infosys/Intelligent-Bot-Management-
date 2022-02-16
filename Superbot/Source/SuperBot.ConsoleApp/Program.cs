/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.Solutions.Ainauto.Superbot.BusinessComponent;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace SuperBot.ConsoleApp
{
    public static class Program
    {
        #region Nested classes to support running as service
        public const string ServiceName = "SuperBotforHealthCheck";

        public static string platfromId=string.Empty;
        public static string tenantId=string.Empty;
        public static string type = string.Empty;
        public static string dependencyResourceID = string.Empty;
        public static string resourceIds = string.Empty;

        public class Service : ServiceBase
        {
            public Service()
            {
                ServiceName = Program.ServiceName;
            }

            protected override void OnStart(string[] args)
            {
                platfromId = Convert.ToString(ConfigurationManager.AppSettings["PlatformID"]);
                tenantId = Convert.ToString(ConfigurationManager.AppSettings["TenantID"]);
                type = Convert.ToString(ConfigurationManager.AppSettings["CheckType"]);
                dependencyResourceID = ConfigurationManager.AppSettings["dependencyResourceID"];
                resourceIds = ConfigurationManager.AppSettings["resourceIds"];
                Program.Start(platfromId,tenantId, Convert.ToInt32(type),dependencyResourceID,resourceIds);
            }

            protected override void OnStop()
            {
                Program.Stop();
            }
        }
        #endregion

        static void Main(string[] args)
        {
            try
            {
                if (args != null && args.Length == 1)
                {
                    if (args[0].Equals("--help", StringComparison.InvariantCultureIgnoreCase))
                    {
                        int inp = 1;
                        while(inp != 0)
                        {
                            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------\nParameters details : \n1.Platform ID \n2. Tenant ID \n3. Dependency Resource ID \n4. Resource Ids to Monitor\n\nEnter the parameter number to get more details about it.\n\nEnter 0 to Exit. ");
                            inp =Convert.ToInt32(Console.ReadLine());
                            switch (inp.ToString())
                            {
                                case "0":
                                    break;
                                case "1":
                                    Console.WriteLine("1. Platform ID :\nPlatform ID for which the resources to be monitored are configured under");
                                    break;
                                case "2":
                                    Console.WriteLine("2. Tenant ID :\nTenant ID for which the resources to be monitored are configured under");
                                    break;
                                case "3":
                                    Console.WriteLine("3. Dependency Resource ID :\nThe Parent Id / Ids (comma seperated) for which the resources to be monitored belongs to.\nFor Example: if the resource Ids to monitor are 1_2_1,1_3_1 then the Dependency Resource Id should be 1_2,1_3 where 1_2 is the parent of 1_2_1 and 1_3 is the parent of 1_3_1.\nWe can also pass the Dependency Resource ID as 1_1 (Control tower) everytime disregarding the resources to monitor as all the resources falls under Control tower.");
                                    break;
                                case "4":
                                    Console.WriteLine("4. Resource Ids: \nThe resources which are to be monitored should be passed here as a comma seperated string. \nFor Example: if we want to monitor resource 1_2 and 1_3 we can pass 1_2,1_3 as resource Ids");
                                    break;
                                default:
                                    Console.WriteLine("Please choose from 1 to 4.\n");
                                    break;
                            }
                        }
                        
                    }
                }
                else
                {
                    if (args != null && args.Length >= 4)
                    {
                        type = args[0];
                        platfromId = args[1];
                        tenantId = args[2];
                        dependencyResourceID = args[3];
                        if(args.Length >= 5)
                            resourceIds = args[4];
                        else
                            resourceIds = Convert.ToString(ConfigurationManager.AppSettings["resourceIds"]);
                    }
                    // running as console app
                    else
                    {
                        string platfromType = Convert.ToString(ConfigurationManager.AppSettings["PlatformType"]);
                        string resourceType = Convert.ToString(ConfigurationManager.AppSettings["ResourceType"]);
                        Monitor monitorObj = new Monitor();
                        var latestPlatformDetails = monitorObj.GetRecentPlatformDetails(platfromType, resourceType);
                        if (latestPlatformDetails != null)
                        {
                            platfromId = latestPlatformDetails.platformID;
                            dependencyResourceID = latestPlatformDetails.resouceID;
                        }

                        platfromId = Convert.ToString(ConfigurationManager.AppSettings["PlatformID"]);
                        tenantId = Convert.ToString(ConfigurationManager.AppSettings["TenantID"]);
                        type = Convert.ToString(ConfigurationManager.AppSettings["CheckType"]);
                        resourceIds = Convert.ToString(ConfigurationManager.AppSettings["resourceIds"]);
                        dependencyResourceID = ConfigurationManager.AppSettings["dependencyResourceID"];
                    }
                    if (string.IsNullOrEmpty(platfromId)) throw new ArgumentNullException("PlatfromId cannot be null.");
                    if (string.IsNullOrEmpty(tenantId)) throw new ArgumentNullException("TenantId cannot be null.");
                    if (string.IsNullOrEmpty(type)) throw new ArgumentNullException("Type cannot be null.");
                    if (string.IsNullOrEmpty(dependencyResourceID)) throw new ArgumentNullException("DependencyResourceID cannot be null.");

                    Start(platfromId, tenantId, Convert.ToInt32(type), dependencyResourceID, resourceIds);
                    // Console.WriteLine("Press any key to stop...");
                    //Console.ReadKey(true);

                    Stop();
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured..." + ex.Message);
            }

            /*if (args.Length >= 2)
           {
               platfromId = args[0];
               tenantId = args[1];

               WriteToFile("platformID " + platfromId + " tenantID = " + tenantId);
               //Monitor monitor = new Monitor();
               //int result = monitor.SystemHealthcheck(Convert.ToInt32(platformVal), Convert.ToInt32(tenantval));
           }*/
        }

        private static void Start(string platformVal,string tenantval,int type,string dependencyResourceID, string resourceIds)
        {
            // onstart code here
            try
            {
                Monitor monitor = new Monitor();
                switch(type)
                {
                    case 1:
                        Console.WriteLine("Calling a System health check for Platform Id:" + platfromId + " & Tenant Id:" + tenantId);
                        Console.WriteLine("Please wait for  system health check to complete");
                        int result = monitor.SystemHealthcheck(Convert.ToInt32(platformVal), Convert.ToInt32(tenantval),dependencyResourceID,resourceIds);
                        Console.WriteLine("System health check completed");
                        break;
                    case 2:
                        Console.WriteLine("Calling a Platform health check for Platform Id:" + platfromId + " & Tenant Id:" + tenantId);
                        Console.WriteLine("Please wait for platform health check to complete");
                        int result1 = monitor.PlatformHealthcheck(Convert.ToInt32(platformVal), Convert.ToInt32(tenantval),dependencyResourceID,resourceIds);
                        Console.WriteLine("Platform health check completed");
                        break;
                    default:
                        Console.WriteLine("Calling a System health check for Platform Id:" + platfromId + " & Tenant Id:" + tenantId);
                        Console.WriteLine("Please wait for  system health check to complete");
                        int res = monitor.SystemHealthcheck(Convert.ToInt32(platformVal), Convert.ToInt32(tenantval),dependencyResourceID,resourceIds);
                        Console.WriteLine("System health check completed");

                        Console.WriteLine("Calling a Platform health check for Platform Id:" + platfromId + " & Tenant Id:" + tenantId);
                        Console.WriteLine("Please wait for platform health check to complete");
                        int res1 = monitor.PlatformHealthcheck(Convert.ToInt32(platformVal), Convert.ToInt32(tenantval),dependencyResourceID,resourceIds);
                        Console.WriteLine("Platform health check completed");

                        break;
                        
                }      
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Exception occured..." + ex.Message);
                throw ex;
            }
        }

        private static void Stop()
        {
            //Console.WriteLine("System & Platform health check completed");
            // onstop code here
        }

        public static void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\superbotLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
