using NUnit.Framework;
using System;
using ETA.Shared;
using System.Threading.Tasks;
using System.Threading;
using Ploeh.AutoFixture;
using Moq;
using Should;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;

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
			// Use AutoFixture to generate values.
			this.fixture = new Fixture();

			// Create mocks for our interfaces.
			this.etaWebApiMock = new Mock<IEtaWebApi>();
			this.loggerMock = new Mock<ILogger>();

			// Register mocks as implementations for the interfaces in DI container.
			SimpleIoc.Default.Register<IEtaWebApi>(() => this.etaWebApiMock.Object);
			SimpleIoc.Default.Register<ILogger>(() => this.loggerMock.Object);
			SimpleIoc.Default.Register<EtaManager>();

			// Create EtaManager through DI container to inject interface implementations.
			this.etaManager = SimpleIoc.Default.GetInstance<EtaManager>();

			this.etaManager.Config = new EtaConfig("192.168.178.35", 8080);
		}

		EtaManager etaManager;
		IFixture fixture;
		Mock<IEtaWebApi> etaWebApiMock;
		Mock<ILogger> loggerMock;

		public IEtaWebApi WebApi => this.etaWebApiMock.Object;

		[Test]
		public async Task GetApiVersionAsync_Should_Return_Valid_Version()
		{
			// Create a random version so we're not always testing agsinst the same hardcoded string.
			// This will generate a random string.
			var expectedApiVersion = this.fixture.Create<string>();

			// Create some valid XML for the EtaManager to parse when calling the IEtaWebApi methods.
			var xml = 
				@"<?xml version=""1.0"" encoding=""utf - 8""?>" +
				@"<eta version=""1.0"" xmlns=""http://www.eta.co.at/rest/v1"">" +
					@"<api version=""" + expectedApiVersion + @"""/>" +
				@"</eta>";

			// Make the mock return the XML from above.
			this.etaWebApiMock.Setup(x => x.GetApiVersionXmlAsync(CancellationToken.None)).ReturnsAsync(xml);

			// Call EtaManager method. EtaManager uses IEtaWebApi (passed in by DI Container) and will receive the XML specified in the mock. Magic.
			var apiVersion = await this.etaManager.GetApiVersionAsync();

			// Whatever the actual version string is - we don't care, but it must match our original version.
			apiVersion.ShouldEqual(expectedApiVersion);
		}

		[Test]
		public async Task GetApiVersionAsync_Should_Return_NULL_for_invalid_XML()
		{
			var expectedApiVersion = "some invalid xml string";
			this.etaWebApiMock.Setup(x => x.GetApiVersionXmlAsync(CancellationToken.None)).ReturnsAsync(expectedApiVersion);

			var apiVersion = await this.etaManager.GetApiVersionAsync();
			apiVersion.ShouldBeNull();
		}
	}
}

