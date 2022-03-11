/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
using Infosys.Lif.LegacyIntegrator;
using Infosys.Lif.LegacyIntegratorService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using Infosys.Lif.MemoryQueue;

namespace MemoryQueueAdapterTester
{
    class Program
    {
        static void Main(string[] args)
        {
            // The code provided will print ‘Hello World’ to the console.
            // Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.
            Console.WriteLine("Hello World!");
            string region1 = "FP_1_MemoryQueue";
            string region2 = "FP_2_MemoryQueue";

            Task.Factory.StartNew(() => ProducerTask(region1));
            Task.Factory.StartNew(() => ProducerTask(region2));
            Task.Factory.StartNew(() => ConsumerTask(region1));
            Task.Factory.StartNew(() => ConsumerTask(region2));

           
            //ProducerTask(region);
            //ConsumerTask(region);
            while (true)
            {
                Thread.Sleep(10000);
                Console.WriteLine("Waiting");
            }
            Console.WriteLine("Completed");

          // MemoryQueueAdapter test = new MemoryQueueAdapter();

            //MemoryQueueAdapter test;
            //Console.ReadKey();


        }

        private static void ConsumerTask(string region)
        {
            // configure adapter manager to receive messages
            
            var am = new AdapterManager();
            am.ResponseReceived += new AdapterManager.AdapterReceiveHandler((ea) => am_ResponseReceived(ea, am, region));
            Console.WriteLine("ConsumerTask :" + Thread.CurrentThread.ManagedThreadId);
            am.Receive(region);
        }

        private static void am_ResponseReceived(ReceiveEventArgs eventArgs, AdapterManager am,string region)
        {
            string messageId = eventArgs.ResponseDetails["MessageIdentifier"] as string;
            string message = eventArgs.ResponseDetails["MessageBody"] as string;
            

            Console.WriteLine("Message Received " + message + ":Time " + DateTime.Now.ToString()+"Thread ID:"+Thread.CurrentThread.ManagedThreadId+
                ": messageId:" + messageId);
           // Thread.Sleep(10000);
            //  Console.WriteLine("Response messageId " + messageId);
            am.Delete(messageId);                      

        }

        /// <summary>
        /// Sends a message per second to queues in round robin fashion
        /// </summary>
        /// <param name="regions"></param>
        private static void ProducerTask(string region)
        {
            for (int i=0; i<10; i++ )
            {
                var am = new AdapterManager();
                //var msg = Guid.NewGuid().ToString();
                var msg = region+"_"+ i;
                Console.WriteLine("Message send " + msg+":Time "+DateTime.Now.ToString());
                
                am.Execute(msg, region);
                //Thread.Sleep(3000);
            }   
           
        }
    }
}
