using NUnit.Framework;
using System;
using ETA.Shared;
using System.Threading.Tasks;
using System.Threading;
using Should;

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
			this.webApi = new EtaWebApi(null);
			this.webApi.SetHostUrl("http://192.168.178.35:8080");
		}

		IEtaWebApi webApi;

		[Test]
		public async Task EtaWebService_Cancel_Async_Rest_Method_Should_Not_Crash ()
		{
			var cts = new CancellationTokenSource();
			
			// Cancel. Method should not fail.
			cts.Cancel();
			var resultXml = await this.webApi.GetApiVersionXmlAsync(cts.Token);

			// Expecting empty value if the method does not succeed.
			resultXml.ShouldBeNull();
		}

		[Test]
		public async Task EtaWebService_GetApiVersionXml_Should_Succeed()
		{
			var resultXml = await this.webApi.GetApiVersionXmlAsync();
			// Expecting a valid value if the call has succeeded.
			resultXml.ShouldContain("xml", StringComparison.OrdinalIgnoreCase);
		}

		[Test]
		public async Task EtaWebService_GetErrorsXml_Should_Succeed()
		{
			var resultXml = await this.webApi.GetErrorsXmlAsync();
			// Expecting a valid value if the call has succeeded.
			resultXml.ShouldContain("xml", StringComparison.OrdinalIgnoreCase);
		}

		[Test]
		public async Task EtaWebService_GetSuppliesXmlWarningLevelXml_Should_Succeed()
		{
			var resultXml = await this.webApi.GetSuppliesWarningLevelXml();
			// Expecting a valid value if the call has succeeded.
			resultXml.ShouldContain("xml", StringComparison.OrdinalIgnoreCase);
		}

		[Test]
		public async Task EtaWebService_GetSuppliesXml_Should_Succeed()
		{
			var resultXml = await this.webApi.GetSuppliesXmlAsync();
			// Expecting a valid value if the call has succeeded.
			resultXml.ShouldContain("xml", StringComparison.OrdinalIgnoreCase);
		}

		[Test]
		public async Task EtaWebService_GetTotalConsumptionXml_Should_Succeed()
		{
			var resultXml = await this.webApi.GetTotalConsumptionXmlAsync();
			// Expecting a valid value if the call has succeeded.
			resultXml.ShouldContain("xml", StringComparison.OrdinalIgnoreCase);
		}
	}
}

