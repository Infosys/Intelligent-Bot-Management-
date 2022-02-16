/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.Solutions.Ainauto.Superbot.BusinessComponent;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnvironmentScan.ConsoleApp
{
    class Program
    {
        public static string platfromId = string.Empty;
        public static string tenantId = string.Empty;
        static void Main(string[] args)
        {
            try
            {
                if (args != null && args.Length >= 2)
                {                    
                    platfromId = args[0];
                    tenantId = args[1];
                }
                // running as console app
                else
                {
                    platfromId = Convert.ToString(ConfigurationManager.AppSettings["PlatformID"]);
                    tenantId = Convert.ToString(ConfigurationManager.AppSettings["TenantID"]);
                }
                Start(Convert.ToInt32(platfromId), Convert.ToInt32(tenantId));                
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured..." + ex.Message);
            }
        }

        private static void Start(int platformVal, int tenantval)
        {
            // onstart code here
            try
            {
                Monitor monitor = new Monitor();
                Console.WriteLine("Executing a method to get Environment Metric Details");
                monitor.GenerateEnvironmentMetricDetails(platformVal, tenantval);
                // monitor.EnvironmentScanConsolidatedReport(new List<int>() { 6185, 6186, 6187 }, 1, 1);
                //monitor.EnvironmentScanConsolidatedReport(new List<int>() { 6288, 6289 }, 1, 1);
                Console.WriteLine("Fetch Environment Metric Details completed successfully ");
               // Console.ReadLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception occured..." + ex.Message);
            }
        }
    }
}
