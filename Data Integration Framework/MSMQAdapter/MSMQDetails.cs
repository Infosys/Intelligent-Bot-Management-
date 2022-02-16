/****************************************************************
Copyright 2021 Infosys Ltd. 
Use of this source code is governed by Apache License Version 2.0 that can be found in the LICENSE file or at 
http://www.apache.org/licenses/
 ***************************************************************/
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace Infosys.Lif.LegacyIntegratorService
//{
//    public class MSMQDetail
//    {
//        public string TransportName { get; set; }
//        public string ServerName { get; set; }
//        public string QueueName { get; set; }
//        public string PoisonQueueName { get; set; }
//        public string MessageLabel { get; set; }
//        public MSMQType QueueType { get; set; }
//        public MSMQReadType QueueReadingType { get; set; }
//        public MSMQReadMode QueueReadingMode { get; set; }

//        int retryCount = 3;
//        public int MessageProcessingMaxCount
//        {
//            get { return retryCount; }
//            set { retryCount = value; }
//        }

//        bool readAndDelete = true;
//        public bool ReadAndDelete
//        {
//            get { return readAndDelete; }
//            set { readAndDelete = value; }
//        }

//        long messaseInvisibilityTimeout = 60000;
//        public long MessaseInvisibilityTimeout
//        {
//            get { return messaseInvisibilityTimeout; }
//            set { messaseInvisibilityTimeout = value; }
//        }

//        long queueReadTimeout = 1000;
//        public long QueueReadTimeout
//        {
//            get { return queueReadTimeout; }
//            set { queueReadTimeout = value; }
//        }

//        int pollingRestDuration = 1000;
//        public int PollingRestDuration
//        {
//            get { return pollingRestDuration; }
//            set { pollingRestDuration = value; }
//        }
//    }

//    public class MSMQ
//    {
//        public string DllPath { get; set; }
//        public string TypeName { get; set; }
//        public List<MSMQDetail> MSMQDetails { get; set; }
//    }

//    public enum MSMQType
//    {
//        /// <summary>
//        /// Accessible across the corpnet. Registered in the corpnet Active directory.
//        /// </summary>
//        Public, 
//        /// <summary>
//        /// Accessible only in the current machine.
//        /// </summary>
//        Private
//    }

//    public enum MSMQReadType
//    {
//        /// <summary>
//        /// Read and then hide/delete the message from the queue
//        /// </summary>
//        Receive, 
//        /// <summary>
//        /// Only read i.e. don't delete message from the queue 
//        /// </summary>
//        Peek
//    }

//    public enum MSMQReadMode
//    {
//        Async, Sync
//    }
//}
