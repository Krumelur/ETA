using NUnit.Framework;
using System;
using ETA.Shared;
using System.Threading.Tasks;
using System.Threading;

namespace ETA.Tests
{
	/// <summary>
	/// Integration tests for accessing the ETA web api.
	/// </summary>
	[TestFixture]
	public class ProductiveEtaWebApiTests
	{
		[TestFixtureSetUp]
		public void Setup()
		{
			this.webApi = new ProductiveEtaWebApi("http://192.168.178.35:8080", null);
		}

		IEtaWebApi webApi;

		[Test]
		public async Task Cancel_Async_Rest_Method_Should_Not_Crash ()
		{
			var cts = new CancellationTokenSource();
			
			// Cancel. Method should not fail.
			cts.Cancel();
			var apiVersion = await this.webApi.GetApiVersionXmlAsync(cts.Token);

			// Expecting empty value if the method does not succeed.
			Assert.IsNullOrEmpty(apiVersion);
		}

		[Test]
		public async Task Get_Api_Version_Should_Succeed()
		{
			var apiVersion = await this.webApi.GetApiVersionXmlAsync();
			// Expecting a valid value if the call has succeeded.
			Assert.IsNotEmpty(apiVersion);
		}
	}
}

