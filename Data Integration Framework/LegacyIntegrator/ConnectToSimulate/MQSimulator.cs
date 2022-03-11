using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Configuration;
using IBM.WMQ;
using System.Threading;

namespace ConnectToSimulate
{
	sealed class MQSimulator
	{
		static decimal[] threadPoolCounters = new decimal[10];
		static Thread[] threads = new Thread[10];
		static MQQueueManager[] queueMngr = new MQQueueManager[10];  
		
		private static string QueueManagerName;

		private string RequestQueueName;
		private string ResponseQueueName;
		private string channelName;
		private string connectionName;
		private string filePath;
		private int mCount;

		public static MQQueueManager QueueManager;
		private MQQueue RequestQueue;
		private MQQueueManager QueueManager1;

		static void Main(string[] args)
		{

			MQSimulator P = new MQSimulator();
			P.StartProcess();
		}

		public static readonly MQSimulator instance = new MQSimulator();

		static MQSimulator()
		{
		}

		public MQSimulator()
		{
		}

		public static MQSimulator Instance
		{
			get
			{
				return instance;
			}
		}

		public void StartProcess()
		{
			try
			{
				QueueManagerName = ConfigurationManager.AppSettings["QueueManager"];
				RequestQueueName = ConfigurationManager.AppSettings["RequestQueue"];
				ResponseQueueName = ConfigurationManager.AppSettings["ResponseQueue"];
				filePath = ConfigurationManager.AppSettings["filepath"];
				channelName = ConfigurationManager.AppSettings["channelname"];
				connectionName = ConfigurationManager.AppSettings["connectionname"];

				if (channelName.Equals(string.Empty) || connectionName.Equals(string.Empty))
				{
					QueueManager1 = new MQQueueManager(QueueManagerName);
				}
				else
				{
					QueueManager1 = new MQQueueManager(QueueManagerName, channelName, connectionName);
				}
				RequestQueue = QueueManager1.AccessQueue(RequestQueueName, MQC.MQOO_INQUIRE +   MQC.MQOO_FAIL_IF_QUIESCING);
				if (QueueManager == null)
					if (connectionName == "")
					{
						QueueManager = new MQQueueManager(QueueManagerName);
					}
					else
					{
						QueueManager = new MQQueueManager(QueueManagerName, channelName, connectionName);
					}
					
				if (RequestQueue == null)
					RequestQueue = QueueManager.AccessQueue(RequestQueueName, MQC.MQOO_INQUIRE);
				while (true)
				{
					if (RequestQueue.CurrentDepth != 0)
					{
						try
						{
							mCount = mCount + 1;
							DateTime startTime = DateTime.Now;
							Console.WriteLine("Message Received : " + mCount);

							int count = RequestQueue.CurrentDepth;
							if (count > 9)
								count = 10;
							for (int threadCounter = 0; threadCounter < count; threadCounter++)
							{
								Process(threadCounter);									

							}
							DateTime endTime = DateTime.Now;
							TimeSpan timeDiff = endTime.Subtract(startTime);
							Console.WriteLine("Message Served" + mCount + " in " + timeDiff.TotalMilliseconds + " msec");
							Console.WriteLine("==============================================================");
						}
						catch (Exception ex)
						{
							Console.WriteLine(ex.Message);
						}

					}
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}

		public void Process(object RequiredData)
		{
			IBMMQ i = new IBMMQ(QueueManagerName, RequestQueueName, ResponseQueueName, filePath, channelName, connectionName);
			i.Receive(QueueManager1);
		}
	}
}
