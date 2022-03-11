using System;
using Microsoft.VisualStudio.QualityTools.UnitTesting.Framework;
using Infosys.Lif.PerfTest.DataEntities.Kicinwa;     
using Infosys.Lif.LegacyFacade;
using Infosys.Lif.LegacyParameters;
using Infosys.Lif.LegacyCommon;


namespace LegacyFacadeTest
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]

	public class UnitTest1
	{

		private string s3 = string.Empty;
		private static System.Collections.ArrayList responses = new System.Collections.ArrayList();


		public UnitTest1()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		/// Initialize() is called once during test execution before
		/// test methods in this test class are executed.
		/// </summary>
		[TestInitialize()]
		public void Initialize()
		{
			//  TODO: Add test initialization code
		}

		/// <summary>
		/// Cleanup() is called once during test execution after
		/// test methods in this class have executed unless the
		/// corresponding Initialize() call threw an exception.
		/// </summary>
		[TestCleanup()]
		public void Cleanup()
		{
			//  TODO: Add test cleanup code
		}

		[TestMethod]
		public void TestWaitForAllForMQ()
		{

			Infosys.Lif.LegacyFacade.LegacyEvent myeve = new Infosys.Lif.LegacyFacade.LegacyEvent();
			myeve.NewEvent += new Infosys.Lif.LegacyFacade.RequestDelegate(Event_MQ);
			ServiceManager.BeginBatch();//  s = new Infosys.Lif.LegacyFacade.ServiceManager();
			Kicinwas kinwcas = new Kicinwas();
			kinwcas.Date_Element = "26/12/2004";
			kinwcas.Identi = 1;
			kinwcas.Obj_Code = "SA";
			Kicinwa kinwa = new Kicinwa();
			kinwa.KicinwasCollection.Add(kinwcas);
			RequestParameters parameters = new RequestParameters();

			parameters.RequestCollection.Add("OperationType", "S");
			parameters.RequestCollection.Add("TransactionID", "KICU");
			parameters.RequestCollection.Add("UserID", "YG01CXP");
			parameters.RequestCollection.Add("TerminalID", "YGXXXXXX");
			parameters.RequestCollection.Add("ConfigFlag", "N");
			ServiceManager.Add("GetServiceMQ", kinwa, parameters);

			ServiceManager.ExecuteBatch(myeve, Infosys.Lif.LegacyFacade.ServiceManager.ProcessMode.WaitForAll, "");


		}



		[TestMethod]
		public void TestWaitForAllForMQForBatch()
		{
			Infosys.Lif.LegacyFacade.LegacyEvent myeve = new Infosys.Lif.LegacyFacade.LegacyEvent();
			myeve.NewEvent += new Infosys.Lif.LegacyFacade.RequestDelegate(Event_MQ);
			ServiceManager.BeginBatch();//  s = new Infosys.Lif.LegacyFacade.ServiceManager();
			Kicinwas kinwcas = new Kicinwas();
			kinwcas.Date_Element = "26/12/2004";
			kinwcas.Identi = 1;
			kinwcas.Obj_Code = "SA";
			Kicinwa kinwa = new Kicinwa();
			kinwa.KicinwasCollection.Add(kinwcas);
			RequestParameters parameters = new RequestParameters();

			parameters.RequestCollection.Add("OperationType", "S");
			parameters.RequestCollection.Add("TransactionID", "KICU");
			parameters.RequestCollection.Add("UserID", "YG01CXP");
			parameters.RequestCollection.Add("TerminalID", "YGXXXXXX");
			parameters.RequestCollection.Add("ConfigFlag", "N");
			ServiceManager.BeginBatch();
			ServiceManager.Add("GetServiceMQ", kinwa, parameters);

			ServiceManager.ExecuteBatch(myeve, Infosys.Lif.LegacyFacade.ServiceManager.ProcessMode.WaitForAll, "");
			ServiceManager.BeginBatch();
			ServiceManager.Add("GetServiceMQ", kinwa, parameters);

			ServiceManager.ExecuteBatch(myeve, Infosys.Lif.LegacyFacade.ServiceManager.ProcessMode.WaitForAll, "");

		}

		[TestMethod]
		public void TestWaitForAllForHIS()
		{
			Infosys.Lif.LegacyFacade.LegacyEvent myeve = new Infosys.Lif.LegacyFacade.LegacyEvent();
			myeve.NewEvent += new Infosys.Lif.LegacyFacade.RequestDelegate(Event_MQ);
			ServiceManager.BeginBatch();//  s = new Infosys.Lif.LegacyFacade.ServiceManager();
			Kicinwas kinwcas = new Kicinwas();
			kinwcas.Date_Element = "26/12/2004";
			kinwcas.Identi = 1;
			kinwcas.Obj_Code = "SA";
			Kicinwa kinwa = new Kicinwa();
			kinwa.KicinwasCollection.Add(kinwcas);
			RequestParameters parameters = new RequestParameters();

			parameters.RequestCollection.Add("OperationType", "S");
			parameters.RequestCollection.Add("TransactionID", "KICU");
			parameters.RequestCollection.Add("UserID", "YG01CXP");
			parameters.RequestCollection.Add("TerminalID", "YGXXXXXX");
			parameters.RequestCollection.Add("ConfigFlag", "N");
			ServiceManager.Add("GetServiceHIS", kinwa, parameters);

			ServiceManager.ExecuteBatch(myeve, Infosys.Lif.LegacyFacade.ServiceManager.ProcessMode.WaitForAll, "");

		}


		[TestMethod]
		public void TestWaitForNoneForMQ()
		{
			Infosys.Lif.LegacyFacade.LegacyEvent myeve = new Infosys.Lif.LegacyFacade.LegacyEvent();
			myeve.NewEvent += new Infosys.Lif.LegacyFacade.RequestDelegate(Event_MQ);
			ServiceManager.BeginBatch();//  s = new Infosys.Lif.LegacyFacade.ServiceManager();
			Kicinwas kinwcas = new Kicinwas();
			kinwcas.Date_Element = "26/12/2004";
			kinwcas.Identi = 1;
			kinwcas.Obj_Code = "SA";
			Kicinwa kinwa = new Kicinwa();
			kinwa.KicinwasCollection.Add(kinwcas);
			RequestParameters parameters = new RequestParameters();

			parameters.RequestCollection.Add("OperationType", "S");
			parameters.RequestCollection.Add("TransactionID", "KICU");
			parameters.RequestCollection.Add("UserID", "YG01CXP");
			parameters.RequestCollection.Add("TerminalID", "YGXXXXXX");
			parameters.RequestCollection.Add("ConfigFlag", "N");
			ServiceManager.Add("GetServiceMQ", kinwa, parameters);

			ServiceManager.ExecuteBatch(myeve, Infosys.Lif.LegacyFacade.ServiceManager.ProcessMode.WaitForNone, "");

		}


		[TestMethod]
		public void TestWaitForNoneForHIS()
		{
			Infosys.Lif.LegacyFacade.LegacyEvent myeve = new Infosys.Lif.LegacyFacade.LegacyEvent();
			myeve.NewEvent += new Infosys.Lif.LegacyFacade.RequestDelegate(Event_MQ);
			ServiceManager.BeginBatch();//  s = new Infosys.Lif.LegacyFacade.ServiceManager();
			Kicinwas kinwcas = new Kicinwas();
			kinwcas.Date_Element = "26/12/2004";
			kinwcas.Identi = 1;
			kinwcas.Obj_Code = "SA";
			Kicinwa kinwa = new Kicinwa();
			kinwa.KicinwasCollection.Add(kinwcas);
			RequestParameters parameters = new RequestParameters();

			parameters.RequestCollection.Add("OperationType", "S");
			parameters.RequestCollection.Add("TransactionID", "KICU");
			parameters.RequestCollection.Add("UserID", "YG01CXP");
			parameters.RequestCollection.Add("TerminalID", "YGXXXXXX");
			parameters.RequestCollection.Add("ConfigFlag", "N");
			ServiceManager.Add("GetServiceMQ", kinwa, parameters);

			ServiceManager.ExecuteBatch(myeve, Infosys.Lif.LegacyFacade.ServiceManager.ProcessMode.WaitForNone, "");

		}

		[TestMethod]
		[ExpectedException(typeof(LegacyException))]
		public void TestExceptionNoService()
		{
			Infosys.Lif.LegacyFacade.LegacyEvent myeve = new Infosys.Lif.LegacyFacade.LegacyEvent();
			myeve.NewEvent += new Infosys.Lif.LegacyFacade.RequestDelegate(Event_MQ);
			ServiceManager.BeginBatch();//  s = new Infosys.Lif.LegacyFacade.ServiceManager();
			Kicinwas kinwcas = new Kicinwas();
			kinwcas.Date_Element = "26/12/2004";
			kinwcas.Identi = 1;
			kinwcas.Obj_Code = "SA";
			Kicinwa kinwa = new Kicinwa();
			kinwa.KicinwasCollection.Add(kinwcas);
			RequestParameters parameters = new RequestParameters();

			parameters.RequestCollection.Add("OperationType", "S");
			parameters.RequestCollection.Add("TransactionID", "KICU");
			parameters.RequestCollection.Add("UserID", "YG01CXP");
			parameters.RequestCollection.Add("TerminalID", "YGXXXXXX");
			parameters.RequestCollection.Add("ConfigFlag", "N");
			ServiceManager.Add("ServiceExceptionNoService", kinwa, parameters);

			ServiceManager.ExecuteBatch(myeve, Infosys.Lif.LegacyFacade.ServiceManager.ProcessMode.WaitForAll, "");

		}

		[TestMethod]
		[ExpectedException(typeof(LegacyException))]
		public void TestExceptionNoSerializer()
		{
			Infosys.Lif.LegacyFacade.LegacyEvent myeve = new Infosys.Lif.LegacyFacade.LegacyEvent();
			myeve.NewEvent += new Infosys.Lif.LegacyFacade.RequestDelegate(Event_MQ);
			ServiceManager.BeginBatch();//  s = new Infosys.Lif.LegacyFacade.ServiceManager();
			Kicinwas kinwcas = new Kicinwas();
			kinwcas.Date_Element = "26/12/2004";
			kinwcas.Identi = 1;
			kinwcas.Obj_Code = "SA";
			Kicinwa kinwa = new Kicinwa();
			kinwa.KicinwasCollection.Add(kinwcas);
			RequestParameters parameters = new RequestParameters();

			parameters.RequestCollection.Add("OperationType", "S");
			parameters.RequestCollection.Add("TransactionID", "KICU");
			parameters.RequestCollection.Add("UserID", "YG01CXP");
			parameters.RequestCollection.Add("TerminalID", "YGXXXXXX");
			parameters.RequestCollection.Add("ConfigFlag", "N");
			ServiceManager.Add("ServiceExceptionNoSerializer", kinwa, parameters);

			ServiceManager.ExecuteBatch(myeve, Infosys.Lif.LegacyFacade.ServiceManager.ProcessMode.WaitForAll, "");

		}

		[TestMethod]
		[ExpectedException(typeof(LegacyException))]
		public void TestExceptionNoWrapper()
		{
			Infosys.Lif.LegacyFacade.LegacyEvent myeve = new Infosys.Lif.LegacyFacade.LegacyEvent();
			myeve.NewEvent += new Infosys.Lif.LegacyFacade.RequestDelegate(Event_MQ);
			ServiceManager.BeginBatch();//  s = new Infosys.Lif.LegacyFacade.ServiceManager();
			Kicinwas kinwcas = new Kicinwas();
			kinwcas.Date_Element = "26/12/2004";
			kinwcas.Identi = 1;
			kinwcas.Obj_Code = "SA";
			Kicinwa kinwa = new Kicinwa();
			kinwa.KicinwasCollection.Add(kinwcas);
			RequestParameters parameters = new RequestParameters();

			parameters.RequestCollection.Add("OperationType", "S");
			parameters.RequestCollection.Add("TransactionID", "KICU");
			parameters.RequestCollection.Add("UserID", "YG01CXP");
			parameters.RequestCollection.Add("TerminalID", "YGXXXXXX");
			parameters.RequestCollection.Add("ConfigFlag", "N");
			ServiceManager.Add("ServiceExceptionNoWrapper", kinwa, parameters);

			ServiceManager.ExecuteBatch(myeve, Infosys.Lif.LegacyFacade.ServiceManager.ProcessMode.WaitForAll, "");

		}

		[TestMethod]
		[ExpectedException(typeof(LegacyException))]
		public void TestExceptionNoBeginBatch()
		{
			Infosys.Lif.LegacyFacade.LegacyEvent myeve = new Infosys.Lif.LegacyFacade.LegacyEvent();
			myeve.NewEvent += new Infosys.Lif.LegacyFacade.RequestDelegate(Event_MQ);

			Kicinwas kinwcas = new Kicinwas();
			kinwcas.Date_Element = "26/12/2004";
			kinwcas.Identi = 1;
			kinwcas.Obj_Code = "SA";
			Kicinwa kinwa = new Kicinwa();
			kinwa.KicinwasCollection.Add(kinwcas);
			RequestParameters parameters = new RequestParameters();

			parameters.RequestCollection.Add("OperationType", "S");
			parameters.RequestCollection.Add("TransactionID", "KICU");
			parameters.RequestCollection.Add("UserID", "YG01CXP");
			parameters.RequestCollection.Add("TerminalID", "YGXXXXXX");
			parameters.RequestCollection.Add("ConfigFlag", "N");
			ServiceManager.Add("ServiceExceptionNoBeginBatch", kinwa, parameters);

			ServiceManager.ExecuteBatch(myeve, Infosys.Lif.LegacyFacade.ServiceManager.ProcessMode.WaitForAll, "");

		}

		[TestMethod]
		[ExpectedException(typeof(LegacyException))]
		public void TestExceptionIntegrator()
		{
			Infosys.Lif.LegacyFacade.LegacyEvent myeve = new Infosys.Lif.LegacyFacade.LegacyEvent();
			myeve.NewEvent += new Infosys.Lif.LegacyFacade.RequestDelegate(Event_MQ);
			ServiceManager.BeginBatch();//  s = new Infosys.Lif.LegacyFacade.ServiceManager();
			Kicinwas kinwcas = new Kicinwas();
			kinwcas.Date_Element = "26/12/2004";
			kinwcas.Identi = 1;
			kinwcas.Obj_Code = "SA";
			Kicinwa kinwa = new Kicinwa();
			kinwa.KicinwasCollection.Add(kinwcas);
			RequestParameters parameters = new RequestParameters();

			parameters.RequestCollection.Add("OperationType", "S");
			parameters.RequestCollection.Add("TransactionID", "KICU");
			parameters.RequestCollection.Add("UserID", "YG01CXP");
			parameters.RequestCollection.Add("TerminalID", "YGXXXXXX");
			parameters.RequestCollection.Add("ConfigFlag", "N");
			ServiceManager.Add("Integrator", kinwa, parameters);

			ServiceManager.ExecuteBatch(myeve, Infosys.Lif.LegacyFacade.ServiceManager.ProcessMode.WaitForAll, "");

		}



		void Event_MQ(object sender, RequestDelegateArgs req)
		{
			object[] obj = (object[])req.Response;
			//MessageBox.Show(obj.GetType().ToString() + "  " + obj[0].GetType().ToString());
			Kicinwa kinwa = (Kicinwa)obj[0];


			KicinwasCollection kc = new KicinwasCollection();
			kc = kinwa.KicinwasCollection;
			Kicinwas kinwas = (Kicinwas)kc[0];

			

			//MessageBox.Show("Response Received");
			 
		}

		void Event_HIS(object sender, RequestDelegateArgs req)
		{

		}
	}
}
