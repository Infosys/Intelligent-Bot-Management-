/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.Lif.LegacyIntegrator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using Infosys.Lif.LegacyIntegratorService;
using System.Threading;
using System.Collections.Specialized;

namespace MemoryDocAdapterTester
{
    class Program
    {
        
        static void Main(string[] args)
        {
            // The code provided will print ‘Hello World’ to the console.
            // Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.
            Console.WriteLine("Hello World!");
            // Console.ReadKey();
            NameValueCollection dictionary = GetDictonary();
            upload(dictionary);
            download(dictionary);

            Console.WriteLine("Completed");

        }


        private static void upload(NameValueCollection dictionary)
        {

           
            AdapterManager adapterManager = new AdapterManager();
            byte[] fileData = File.ReadAllBytes("D:\\tmp\\input\\1.png");
            MemoryStream DataStream = new MemoryStream(fileData);
            string msgResponse = adapterManager.Execute(DataStream,
                        "FrameRepository", dictionary);
            Console.WriteLine("Upload Response:"+ msgResponse);

        }

        private static void download(NameValueCollection dictionary)
        {

            
            AutoResetEvent arEvent = new AutoResetEvent(false);
            AdapterManager adapterManager = new AdapterManager();

            // initialize adapter manager - entityName is the region name in LISetting.Config file
            adapterManager.ResponseReceived +=
                new AdapterManager.AdapterReceiveHandler((ea) => adapterManager_ResponseReceived(ea, arEvent));
            
            adapterManager.Receive("FrameRepository", dictionary);

            // wait till response is received
            arEvent.WaitOne();

        }

        static NameValueCollection GetDictonary()
        {
            NameValueCollection dictionary = new NameValueCollection();
            dictionary.Add("UriScheme", "");
            dictionary.Add("RootDNS", "");
            dictionary.Add("Port", "");
            dictionary.Add("container_name", "test");
            dictionary.Add("file_name", DateTime.UtcNow.Ticks.ToString());

            // pass other parameters
            // mandatory parameters accepted by repository service
            dictionary.Add("device_id", "DeviceId_10");
            //dictionary.Add("workflow_ver#", workflowRequest.WorkflowVer.ToString());
            dictionary.Add("tenant_id", "1");
            return dictionary;
        }

        static void adapterManager_ResponseReceived(ReceiveEventArgs eventArgs, AutoResetEvent arEvent)
        {
            //System.IO.Stream _presentation = null;
            int statusCode = (int)eventArgs.ResponseDetails["StatusCode"];
            string response = response = eventArgs.ResponseDetails["Response"] as string; 
            if (statusCode == 0)
            {

              

                System.IO.Stream dataStream = eventArgs.ResponseDetails["DataStream"] as System.IO.Stream;

                string filePath = "D:\\tmp\\output\\";
                string fileName = eventArgs.ResponseDetails["FileName"] as string;
                fileName = fileName + ".png";
                DirectoryInfo info = new DirectoryInfo(filePath);
                if (!info.Exists)
                {
                    info.Create();
                }
                string path = Path.Combine(filePath, fileName);
               
                using (FileStream outputFileStream = new FileStream(path, FileMode.Create))
                {
                    dataStream.CopyTo(outputFileStream);
                }               

            }
            
            Console.WriteLine("Download Response:"+ response);

            // signal that response has been received
            arEvent.Set();
        }
    }
}
