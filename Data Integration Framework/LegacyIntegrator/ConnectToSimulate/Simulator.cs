using System;
using System.Collections.Generic;
using System.Text;
using IBM.WMQ;
using System.Configuration;

namespace ConnectToSimulate
{
	class Simulator
	{
		public Simulator()
		{

		}

		public void Test(MQQueueManager mq)
		{
			string QueueManagerName = "";
			string RequestQueueName = "";
			string ResponseQueueName = "";
			MQQueueManager QueueManager;
			MQQueue RequestQueue;
			MQQueue ResponseQueue;
			string msg = "APCA    KICUYG01JZR     0000004/06/01                 L78Z5AWR0001N                                          ";
			msg += Environment.NewLine;
			msg += "CUACCTIL0001S0001009999                   373940005";
			msg += Environment.NewLine;
			
			string qModelName = "SYSTEM.DEFAULT.MODEL.QUEUE";
			QueueManagerName = ConfigurationManager.AppSettings["QueueManager"]; ;// "NU.MQ.SIMULATE";
			ResponseQueueName = ConfigurationManager.AppSettings["ResponseQueue"]; //"RESPONSE"; ;
			RequestQueueName = ConfigurationManager.AppSettings["RequestQueue"]; //"REQUEST";
			string channelName = ConfigurationManager.AppSettings["channelname"]; //"MQ.SIML.SRVR.CHNL";
			string connectname = ConfigurationManager.AppSettings["connectionname"]; //"172.25.77.51(1450)";
			
			try
			{
				QueueManager = new MQQueueManager(QueueManagerName, channelName, connectname);

				RequestQueue = QueueManager.AccessQueue(RequestQueueName, MQC.MQOO_OUTPUT);//+ MQC.MQPER_PERSISTENCE_AS_Q_DEF);
				//ResponseQueue = QueueManager.AccessQueue(qModelName, MQC.MQOO_INPUT_AS_Q_DEF, QueueManagerName, ResponseQueueName + "*", null);
				ResponseQueue = QueueManager.AccessQueue(ResponseQueueName, MQC.MQOO_INPUT_AS_Q_DEF);

				MQMessage PutMessage = new MQMessage();


				PutMessage.WriteString(msg);
				PutMessage.Format = MQC.MQFMT_STRING;
				MQPutMessageOptions PutOptions = new MQPutMessageOptions();
				//PutMessage.CorrelationId = correlationId;  
				PutMessage.ReplyToQueueName = ResponseQueue.Name.Trim();

				try
				{
					for (int i = 0; i < 1000; i++)
						RequestQueue.Put(PutMessage, PutOptions);
					
				}
				catch
				{
					QueueManager = new MQQueueManager(QueueManagerName);
					RequestQueue = QueueManager.AccessQueue(RequestQueueName, MQC.MQOO_OUTPUT);
					RequestQueue.Put(PutMessage, PutOptions);
					//QueueManager.Disconnect();
				}

			}
			catch (MQException ex)
			{
				Console.WriteLine(ex.Message);
			}


		}
		
	}
}
