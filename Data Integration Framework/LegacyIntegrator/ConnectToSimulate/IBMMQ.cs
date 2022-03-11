using System;
using System.Collections.Generic;
using System.Text;
using IBM.WMQ;
using System.IO;
using System.Threading;
using System.Configuration;
using System.Collections;

namespace ConnectToSimulate
{
	class IBMMQ
	{
		private static string QueueManagerName;
		private string RequestQueueName;
		private string ResponseQueueName;
		private string channelName;
		private string connectionName;
		private string filePath;
		private static Hashtable dapContents = new Hashtable();

		private byte[] correlationId;
		public IBMMQ(string queueManagerName, string requestQueueName, string responseQueueName, string FilePath, string ChannelName, string ConnectionName)
		{
			//dapContents = new Hashtable();
			QueueManagerName = queueManagerName; //System.Configuration.ConfigurationManager.AppSettings["QueueManager"];
			RequestQueueName = requestQueueName; //ConfigurationManager.AppSettings["RequestQueue"];
			ResponseQueueName = responseQueueName;// ConfigurationManager.AppSettings["ResponseQueue"];
			filePath = Path.GetFullPath(FilePath);// ConfigurationManager.AppSettings["filepath"];
			channelName = ChannelName;// ConfigurationManager.AppSettings["channelname"];
			connectionName = ConnectionName;// ConfigurationManager.AppSettings["connectionname"];			
		}

		/// <summary>
		/// Send the message to IBM WebSphere MQ Queue.
		/// </summary>
		/// <param name="msg">Message to be sent</param>
		/// <returns>Message ID of the message</returns>
		public string SendAsync(string msg, MQQueue q, MQQueueManager qm)
		{


			MQMessage PutMessage = new MQMessage();


			try
			{
				PutMessage.WriteString(msg);
				PutMessage.Format = MQC.MQFMT_STRING;
				MQPutMessageOptions PutOptions = new MQPutMessageOptions();

				try
				{
					PutMessage.CorrelationId = correlationId;
					q.Put(PutMessage, PutOptions);

					if (qm != null)
						qm.Disconnect();
				}
				catch
				{

					MQQueueManager qm1 = new MQQueueManager(QueueManagerName);
					MQQueue RequestQueue3 = qm1.AccessQueue("TIME", MQC.MQOO_OUTPUT);
					RequestQueue3.Put(PutMessage, PutOptions);
					RequestQueue3.Close();
				}
				finally
				{

				}

			}
			catch (MQException mqe)
			{
				Console.WriteLine(mqe.Message + " from IBMMQ");
			}
			return "";
		}

		public void Receive(MQQueueManager mq)
		{
			MQQueueManager QueueManager2 = null;
			QueueManager2 = mq;
			if (QueueManager2 == null)
			{
				try
				{
					QueueManager2 = new MQQueueManager(QueueManagerName, channelName, connectionName);
				}
				catch
				{
					QueueManager2 = new MQQueueManager(QueueManagerName, channelName, connectionName);
				}
                QueueManager2.Begin();
            }
			MQQueue newQ = null;
			try
			{
				newQ = QueueManager2.AccessQueue(RequestQueueName , MQC.MQOO_INPUT_AS_Q_DEF);// + MQC.MQOO_FAIL_IF_QUIESCING);
			}
			catch
			{

				try
				{
					QueueManager2 = new MQQueueManager(QueueManagerName, channelName, connectionName);
					newQ = QueueManager2.AccessQueue(RequestQueueName, MQC.MQOO_INPUT_AS_Q_DEF);// + MQC.MQOO_FAIL_IF_QUIESCING);
				}
				catch(MQException exq)
				{
					throw exq;
				}
			}

			if (newQ == null)
			{
				return;
			}
            ////if (newQ.CurrentDepth == 0)
            ////{
            ////    return;
            ////}
			
			MQQueue newRespQ = null;
			MQMessage GetMessage = new MQMessage();
			string strMsg = "";
			MQGetMessageOptions GetOptions = new MQGetMessageOptions();
			newQ.Get(GetMessage, GetOptions);
			string responseQName = GetMessage.ReplyToQueueName.Trim();
			if (responseQName == "" || responseQName == ResponseQueueName)
			{
				newRespQ = QueueManager2.AccessQueue(ResponseQueueName, MQC.MQOO_OUTPUT);
			}
			else
			{
				newRespQ = QueueManager2.AccessQueue(responseQName, MQC.MQOO_OUTPUT);
			}
			
			correlationId = GetMessage.MessageId;
			if (GetMessage.Format.CompareTo(MQC.MQFMT_STRING) == 0)
			{
				strMsg = GetMessage.ReadString(GetMessage.MessageLength);
			}
			Console.WriteLine("Input = " + strMsg);
			string response = ParseMessage(strMsg);
			MQMessage PutMessage = new MQMessage();
			PutMessage.WriteString(response);
			PutMessage.Format = MQC.MQFMT_STRING;
			MQPutMessageOptions PutOptions = new MQPutMessageOptions();
			PutMessage.CorrelationId = GetMessage.MessageId;
			//QueueManager2.Begin();
			newRespQ.Put(PutMessage, PutOptions);
			Console.WriteLine("Output = " + response);
			QueueManager2.Commit();
			newQ.Close();
			
		}

		private string ParseMessage(string msg)
		{
			//string request = msg;
			string delimiter = "\r\n";
			string apcaHeader = string.Empty;
			apcaHeader = msg.Substring(0, 109);
			string[] response = msg.Split(delimiter.ToCharArray(),
				System.StringSplitOptions.RemoveEmptyEntries);
			StringBuilder dapStrngBldr = new StringBuilder();
			dapStrngBldr.Append(response[0]);
			for (int i = 1; i < response.Length; i++)
			{
				string dataHeader = response[i].Substring(0, 8).Trim();
				string dapOperation = response[i].Substring(12, 1);

				if (dapContents.ContainsKey(dataHeader))
				{
					if (dapOperation.Equals("S") || dapOperation.Equals("L") || dapOperation.Equals("N"))
					{
						dapStrngBldr.Append(Environment.NewLine);
						string dapCon = dapContents[dataHeader].ToString();
						dapCon = dapCon.Remove(8, 4).ToString();
						dapCon = dapCon.Insert(8, response[i].Substring(8, 4).Trim());
						dapStrngBldr.Append(dapCon);

					}
					else if ((dapOperation.Equals("I") || dapOperation.Equals("U")))
					{

						StreamWriter strmRdr = new StreamWriter(filePath + "\\" + dataHeader + ".txt", true);
						string input = response[i];
						strmRdr.Close();
						StreamReader strmRdrDataHeader = new StreamReader(filePath + "\\" + dataHeader + ".txt");
						input = strmRdrDataHeader.ReadToEnd();
						string dapConIU = input;
						dapConIU = dapConIU.Remove(8, 4).ToString();
						dapConIU = dapConIU.Insert(8, response[i].Substring(8, 4).Trim());

						dapContents[dataHeader] = dapConIU;
						dapStrngBldr.Append(Environment.NewLine);
						dapStrngBldr.Append(dapContents[dataHeader].ToString());

					}
					else if (dapOperation.Equals("D"))
					{
						string input = response[i];
						dapContents[dataHeader] = input;
						dapStrngBldr.Append(Environment.NewLine);
						dapStrngBldr.Append(dapContents[dataHeader].ToString());
					}

				}
				else
				{
					if (dapOperation.Equals("S") || dapOperation.Equals("L") || dapOperation.Equals("N"))
					{
						StreamReader strmRdr = new StreamReader(filePath + "\\" + dataHeader + ".txt");
						string input = strmRdr.ReadToEnd();
						dapContents.Add(dataHeader, input);
						strmRdr.Close();
						string dapCon = dapContents[dataHeader].ToString();
						dapCon = dapCon.Remove(8, 4).ToString();
						dapCon = dapCon.Insert(8, response[i].Substring(8, 4).Trim());

						dapStrngBldr.Append(Environment.NewLine);
						dapStrngBldr.Append(dapCon);
					}
					else if ((dapOperation.Equals("I") || dapOperation.Equals("U")))
					{
						StreamWriter strmRdr = new StreamWriter(filePath + "\\" + dataHeader + ".txt", true);
						string input = response[i];
						strmRdr.Close();
						StreamReader strmRdrDataHeader2 = new StreamReader(filePath + "\\" + dataHeader + ".txt");
						input = strmRdrDataHeader2.ReadToEnd();

						string dapConIUCa = input;
						dapConIUCa = dapConIUCa.Remove(8, 4).ToString();
						dapConIUCa = dapConIUCa.Insert(8, response[i].Substring(8, 4).Trim());

						dapContents.Add(dataHeader, dapConIUCa);
						dapStrngBldr.Append(Environment.NewLine);
						dapStrngBldr.Append(dapContents[dataHeader].ToString());
					}

					else if (dapOperation.Equals("D"))
					{
						string input = response[i];
						dapContents[dataHeader] = input;
						dapStrngBldr.Append(Environment.NewLine);
						dapStrngBldr.Append(dapContents[dataHeader].ToString());
					}
				}
			}
			dapStrngBldr.Append(Environment.NewLine);

			return dapStrngBldr.ToString();
		}
	}
}
