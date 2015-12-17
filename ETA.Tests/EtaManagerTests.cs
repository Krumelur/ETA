using NUnit.Framework;
using System;
using ETA.Shared;
using System.Threading.Tasks;
using System.Threading;

namespace ETA.Tests
{
	/// <summary>
	/// Tests the ETA manager with a fake IEtaWebApi.
	/// </summary>
	[TestFixture]
	public class EtaManagerTests
	{
		[TestFixtureSetUp]
		public void Setup()
		{
			// TODO: Setup DI container.
			EtaManager.Instance.Config = new EtaConfig("192.168.178.35", 8080);
		}

		[Test]
		public async Task Get_Api_Version_Should_Return_Valid_VersionString()
		{
			var apiVersion = await EtaManager.Instance.GetApiVersionAsync();
			Assert.AreEqual("1.1", apiVersion);
		}
	}
}

