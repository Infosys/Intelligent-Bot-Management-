using Microsoft.VisualStudio.QualityTools.UnitTesting.Framework;
using System.IO;
using Infosys.Lif.LegacyIntegrator;
using Infosys.Lif.LegacyCommon;
using Infosys.Lif.LegacyIntegratorService;
using System.Diagnostics;

namespace Infosys.Lif.LegacyIntegratorUnitTest
{
	/// <summary>
	///This is a test class for Infosys.LIF.LegacyIntegrator.LegacyIntegrator and is intended
	///to contain all Infosys.LIF.LegacyIntegrator.LegacyIntegrator Unit Tests
	///</summary>
	[TestClass()]
	public class LegacyIntegratorTest
	{

		private TestContext testContextInstance;
		private string expected;
		private string input;
		private string sourcePath;
		private string destinationPath;
		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		/// <summary>
		///Initialize() is called once during test execution before
		///test methods in this test class are executed.
		///</summary>
		[TestInitialize()]
		public void Initialize()
		{
			expected = "APCA    KICUYG01CXP     0000                          YGXXXXXX0001N                                               \r\nCYCSCDTL0000L0001400001 0000ENDOFSET 0100MO1901/27/2005\r\n";
			input = "APCA    KICUYG01CXP     0000                          YGXXXXXX0001N                                               \r\nCYCSCDTL0000S000001000014                   SA00126/12/2004";
			sourcePath = @"C:\LIF\Code\Framework\LegacyIntegrator\LegacyIntegratorUnitTest\LegacyIntegratorUnitTest\Configurations";
			destinationPath = @"C:\LIF\Code\Framework\LegacyIntegrator\LegacyIntegratorUnitTest\LegacyIntegratorUnitTest";
		}

		/// <summary>
		///Cleanup() is called once during test execution after
		///test methods in this class have executed unless
		///this test class' Initialize() method throws an exception.
		///</summary>
		[TestCleanup()]
		public void Cleanup()
		{
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileName"></param>
		private void ChangeLISettingsConfigFile(string fileName)
		{
			FileInfo fileInfo = new FileInfo(sourcePath + "\\"+ fileName +".config");
			fileInfo.CopyTo(destinationPath + "\\LISettings.config", true);
		}
		/// <summary>
		/// This method is for testing a postive case. This tests a synchronous messaging using
		/// static queue. ConnectionModel is TLS.
		///</summary>
		[TestMethod()]
		public void CheckSync()
		{
			ChangeLISettingsConfigFile("LISettings1");
			
			AdapterManager target = new AdapterManager();
			string hostRegion ="TEST1"; 
			string actual = target.Execute(input, hostRegion);
			Assert.AreEqual(expected, actual, false,
				"Sync communication using static queue failed");			
		}
		
		/// <summary>
		/// Uses static queue for sending message. Communication type is async.
		///  ConnectionModel is None.
		/// </summary>
		[TestMethod()]
		public void CheckAsync()
		{
			ChangeLISettingsConfigFile("LISettings1");

			string expected = "";
			AdapterManager target = new AdapterManager();
			string hostRegion = "TEST2";
			string actual = target.Execute(input, hostRegion);
			Assert.AreEqual(expected, actual, false,
				"Async communication using static queue failed");			
		}

		/// <summary>
		/// Uses dynamic queue for sending message. Communication type is sync.
		///  ConnectionModel is None.
		/// </summary>
		[TestMethod()]
		public void CheckDynamicQueue()
		{
			ChangeLISettingsConfigFile("LISettings1");
			AdapterManager target = new AdapterManager();
			string hostRegion = "TEST4";
			string actual = target.Execute(input, hostRegion);
			Assert.AreEqual(expected, actual, false,
				"Sync communication using dynamic queue failed");
		}

		/// <summary>
		/// Checks communication when TLS is used for one of the region.
		/// </summary>
		[TestMethod()]
		public void CheckCommunicationUsingTLS()
		{
			ChangeLISettingsConfigFile("LISettings1");
			AdapterManager target = new AdapterManager();
			string hostRegion = "TEST3";
			string actual = target.Execute(input, hostRegion);
			Assert.AreEqual(expected, actual, false,
				"Sync communication using TLS as connection model failed");	
		}

		/// <summary>
		/// Checks communication using No connection pooling. i.e creating connection for 
		/// each request.
		/// </summary>
		[TestMethod()]
		public void CheckCommunicationUsingNoPooling()
		{
			ChangeLISettingsConfigFile("LISettings1");

			AdapterManager target = new AdapterManager();
			string hostRegion = "TEST1";
			string actual = target.Execute(input, hostRegion);
			Assert.AreEqual(expected, actual, false,
				"Sync communication using no pooling as connection model failed");
		}

		/// <summary>
		/// Checks communication using connection pooling. Its uses LISettings2.config.		
		/// </summary>
		[TestMethod()]
		public void CheckCommunicationUsingPooling()
		{
			ChangeLISettingsConfigFile("LISettings2");

			AdapterManager target = new AdapterManager();
			string hostRegion = "TEST1";
			string actual = target.Execute(input, hostRegion);
			Assert.AreEqual(expected, actual, false,
			"1 -Sync communication using ConnectionPool as connection model failed");
			
			// Wait for 30 sec so MQPoolManager pools and disconnects object which are 
			// inactive for more than ActiveTimeForConnection value specified in config.
			// In this case ActiveTimeForConnection value is 20 sec and time interval for
			// pooling is 10 sec.
			System.Threading.Thread.Sleep(30000);

			hostRegion = "TEST1";
			actual = target.Execute(input, hostRegion);
			Assert.AreEqual(expected, actual, false,
			"2 -Sync communication using ConnectionPool as connection model failed");
			
			// Make request to TEST2
			hostRegion = "TEST2";
			string expected1 = string.Empty;
			actual = target.Execute(input, hostRegion);
			Assert.AreEqual(expected1, actual, false,
			"3 -Async communication using ConnectionPool as connection model failed");
		}

		/// <summary>
		/// TimeOut is set to 1 millisec. If message wont come within this time then
		/// it will come with result.
		/// </summary>
		[TestMethod()]
		public void CheckTimeOut()
		{
			ChangeLISettingsConfigFile("LISettings2");
			string expected = "";
			AdapterManager target = new AdapterManager();
			string hostRegion = "TEST3";
			string actual = target.Execute(input, hostRegion);
			Assert.AreEqual(expected, actual, false,
			 "Sync communication using no pooling as connection model failed");
		}

		/// <summary>
		/// Invalid region is passed to execute method.
		/// Legacy exception should be thrown for this case.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(LegacyException))]
		public void CheckInValidHostRegion()
		{
			ChangeLISettingsConfigFile("LISettings2");
			AdapterManager target = new AdapterManager();
			// TEST99 does not exist in LISettings2.config.
			string hostRegion = "TEST99";
			string actual = target.Execute(input, hostRegion);			
		}
			
		/// <summary>
		/// Test a case where ConnectionModel in transport details is invalid.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(LegacyException))]
		public void CheckInValidConnectionModel()
		{
			ChangeLISettingsConfigFile("LISettings3");
			AdapterManager target = new AdapterManager();
			string hostRegion = "TEST11";
			string actual = target.Execute(input, hostRegion);			
		}

		/// <summary>
		/// Transport medium for the specified region is empty.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(LegacyException))]
		public void CheckEmptyTransportMedium()
		{
			ChangeLISettingsConfigFile("LISettings2");
			AdapterManager target = new AdapterManager();
			string hostRegion = "TEST4";
			string actual = target.Execute(input, hostRegion);
		}

		/// <summary>
		/// Transport name for the specified region is empty.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(LegacyException))]
		public void CheckEmptyTransportName()
		{
			ChangeLISettingsConfigFile("LISettings2");
			AdapterManager target = new AdapterManager();
			string hostRegion = "TEST5";
			string actual = target.Execute(input, hostRegion);
		}

		/// <summary>
		/// Transport medium for the specified region is invalid. 
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(LegacyException))]
		public void CheckInvalidTransportName()
		{
			ChangeLISettingsConfigFile("LISettings2");
			AdapterManager target = new AdapterManager();
			string hostRegion = "TEST6";
			string actual = target.Execute(input, hostRegion);
		}

		/// <summary>
		/// TransportName specified in Region does not exist in IBMMQDetails section.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(LegacyException))]
		public void CheckEmptyTransportNameInIBMMQDetailsSection()
		{
			ChangeLISettingsConfigFile("LISettings2");
			AdapterManager target = new AdapterManager();
			string hostRegion = "TEST7";
			string actual = target.Execute(input, hostRegion);
		}

		/// <summary>
		/// Communication type is not specified for MQ adapter.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(LegacyException))]
		public void CheckEmptyCommunicationType()
		{
			ChangeLISettingsConfigFile("LISettings2");
			AdapterManager target = new AdapterManager();
			string hostRegion = "TEST8";
			string actual = target.Execute(input, hostRegion);
		}

		/// <summary>
		/// Communication type specified is neither Sync nor Async.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(LegacyException))]
		public void CheckInvalidCommunicationType()
		{
			ChangeLISettingsConfigFile("LISettings2");
			AdapterManager target = new AdapterManager();
			string hostRegion = "TEST9";
			string actual = target.Execute(input, hostRegion);
		}

		/// <summary>
		/// QueueType specified is neither Static nor Dynamic.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(LegacyException))]
		public void CheckInvalidQueueType()
		{
			ChangeLISettingsConfigFile("LISettings2");
			AdapterManager target = new AdapterManager();
			string hostRegion = "TEST10";
			string actual = target.Execute(input, hostRegion);
		}

		/// <summary>
		/// Checking number of connection and number of pool.
		/// </summary>
		[TestMethod]
		public void CheckNumberOfConnections()
		{
			ChangeLISettingsConfigFile("LISettings2");

			AdapterManager target = new AdapterManager();
			string hostRegion = "TEST1";
			string actual = target.Execute(input, hostRegion);
			Assert.AreEqual(expected, actual, false,
			"Sync communication using ConnectionPool as connection model failed");

			const string CATEGORY = "LegacyIntegrator";
			const string TOTAL_ACTIVE_CONNECTIONS = "MQ - TotalActiveConnections";
			const string ACTIVE_CONNECTIONS_PER_POOL = "MQ - ActiveConnectionsPerPool";

			//Create performance counter for total number of connectiona
			PerformanceCounter totalConnection = new PerformanceCounter();
			totalConnection.CategoryName = CATEGORY;
			totalConnection.CounterName = TOTAL_ACTIVE_CONNECTIONS;
			totalConnection.ReadOnly = false;

			//Create performance counter for total number of connections per pool
			PerformanceCounter connectionPerPool = new PerformanceCounter();
			connectionPerPool.CategoryName = CATEGORY;
			connectionPerPool.CounterName = ACTIVE_CONNECTIONS_PER_POOL;
			connectionPerPool.ReadOnly = false;

			//At this point number of connections should be 2 and number of connection per pool 
			//should be 1. Since first time when a request is made to TEST2, it creates 2 connections 
			// to TEST2 region and no connections are created for TEST3 region which also uses pool.
			long totalConnections = 2;
			long connectionsPerPool = 1;
			Assert.AreEqual(totalConnections, totalConnection.RawValue);
			Assert.AreEqual(connectionsPerPool, connectionPerPool.RawValue);

			// Wait for 30 sec so MQPoolManager pools and disconnects object which are 
			// inactive for more than ActiveTimeForConnection value specified in config.
			// In this case ActiveTimeForConnection value is 20 sec and time interval for
			// pooling is 10 sec.
			System.Threading.Thread.Sleep(30000);
			
			//At this point number of connections should be 0 and number of connection per pool 
			//should be 0. Zero means ActiveTimeForConnection TimeIntervalForPool are fine.
			totalConnections = 0;
			connectionsPerPool = 0;
			Assert.AreEqual(totalConnections, totalConnection.RawValue);
			Assert.AreEqual(connectionsPerPool, connectionPerPool.RawValue);
		}

		/// <summary>
		/// Connection model is specified as TLS but DefaultTLSTransport is empty.
		/// </summary>
		[TestMethod]
		[ExpectedException(typeof(LegacyException))]
		public void CheckTLSWhenDefaultTLSisEmpty()
		{
			ChangeLISettingsConfigFile("LISettings3");
			AdapterManager target = new AdapterManager();
			string hostRegion = "TEST1";
			string actual = target.Execute(input, hostRegion);
		}

		/// <summary>
		/// Connection model is specified as TLS but DefaultTLSTransport is empty.
		/// </summary>
		[TestMethod]
		public void TestErrorCondition()
		{
			ChangeLISettingsConfigFile("LISettings4");
			AdapterManager target = new AdapterManager();
			string hostRegion = "TEST";
			string actual = target.Execute(input, hostRegion);
			Assert.AreEqual(expected, actual, false,
				"Fine");			
		}

	}
}
