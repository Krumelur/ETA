﻿using NUnit.Framework;
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

		[Test]
		public async Task GetSuppliesXmlAsync_Should_Return_Valid_Amount()
		{
			// Create a random value so we're not always testing agsinst the same hardcoded value.
			var expectedSuppliesAmount = this.fixture.Create<NumericUnit>();

			// Create some valid XML for the EtaManager to parse when calling the IEtaWebApi methods.
			var xml =
				@"<?xml version=""1.0"" encoding=""utf - 8""?>" +
				@"<eta version=""1.0"" xmlns=""http://www.eta.co.at/rest/v1"">" +
					@"<value uri=""/user/var/112/10201/0/0/12015"" strValue=""" + expectedSuppliesAmount.Value + @""" unit=""" + expectedSuppliesAmount.Unit + @""" decPlaces=""0"" scaleFactor=""10"" advTextOffset=""0"">" + expectedSuppliesAmount.Value + "</value>" +
				@"</eta>";

			// Make the mock return the XML from above.
			this.etaWebApiMock.Setup(x => x.GetSuppliesXmlAsync(CancellationToken.None)).ReturnsAsync(xml);

			// Call EtaManager method. EtaManager uses IEtaWebApi (passed in by DI Container) and will receive the XML specified in the mock. Magic.
			var amount = await this.etaManager.GetSuppliesAsync();

			// Whatever the actual version string is - we don't care, but it must match our original version.
			amount.ShouldEqual(expectedSuppliesAmount);
		}

		[Test]
		public async Task GetSuppliesXmlAsync_Should_Return_Empty_Amount_for_invalid_XML()
		{
			// Create some valid XML for the EtaManager to parse when calling the IEtaWebApi methods.
			var xml = "<xml>some invalid xml</xml>";

			// Make the mock return the XML from above.
			this.etaWebApiMock.Setup(x => x.GetSuppliesXmlAsync(CancellationToken.None)).ReturnsAsync(xml);

			// Call EtaManager method. EtaManager uses IEtaWebApi (passed in by DI Container) and will receive the XML specified in the mock. Magic.
			var amount = await this.etaManager.GetSuppliesAsync();

			// Whatever the actual version string is - we don't care, but it must match our original version.
			amount.ShouldEqual(NumericUnit.Empty);
		}

		[Test]
		public async Task GetStockContentWarningLevelAsync_Should_Return_Valid_Amount()
		{
			// Create a random value so we're not always testing agsinst the same hardcoded value.
			var expectedSuppliesWarningLevelAmount = this.fixture.Create<NumericUnit>();

			// Create some valid XML for the EtaManager to parse when calling the IEtaWebApi methods.
			var xml =
				@"<?xml version=""1.0"" encoding=""utf - 8""?>" +
				@"<eta version=""1.0"" xmlns=""http://www.eta.co.at/rest/v1"">" +
					@"<value uri=""/user/var/112/10201/0/0/12042"" strValue=""" + expectedSuppliesWarningLevelAmount.Value + @""" unit=""" + expectedSuppliesWarningLevelAmount.Unit + @""" decPlaces=""0"" scaleFactor=""10"" advTextOffset=""0"">" + expectedSuppliesWarningLevelAmount.Value + "</value>" +
				@"</eta>";

			// Make the mock return the XML from above.
			this.etaWebApiMock.Setup(x => x.GetSuppliesWarningLevelXml(CancellationToken.None)).ReturnsAsync(xml);

			// Call EtaManager method. EtaManager uses IEtaWebApi (passed in by DI Container) and will receive the XML specified in the mock. Magic.
			var amount = await this.etaManager.GetSuppliesWarningLevelAsync();

			// Whatever the actual version string is - we don't care, but it must match our original version.
			amount.ShouldEqual(expectedSuppliesWarningLevelAmount);
		}

		[Test]
		public async Task GetTotalConsumptionAsync_Should_Return_Valid_Amount()
		{
			// Create a random value so we're not always testing agsinst the same hardcoded value.
			var expectedConsumption = this.fixture.Create<NumericUnit>();

			// Create some valid XML for the EtaManager to parse when calling the IEtaWebApi methods.
			var xml =
				@"<?xml version=""1.0"" encoding=""utf - 8""?>" +
				@"<eta version=""1.0"" xmlns=""http://www.eta.co.at/rest/v1"">" +
					@"<value uri=""/user/var/112/10201/0/0/12016"" strValue=""" + expectedConsumption.Value + @""" unit=""" + expectedConsumption.Unit + @""" decPlaces=""0"" scaleFactor=""10"" advTextOffset=""0"">" + expectedConsumption.Value + "</value>" +
				@"</eta>";

			// Make the mock return the XML from above.
			this.etaWebApiMock.Setup(x => x.GetTotalConsumptionXmlAsync(CancellationToken.None)).ReturnsAsync(xml);

			// Call EtaManager method. EtaManager uses IEtaWebApi (passed in by DI Container) and will receive the XML specified in the mock. Magic.
			var amount = await this.etaManager.GetTotalConsumptionAsync();

			// Whatever the actual version string is - we don't care, but it must match our original version.
			amount.ShouldEqual(expectedConsumption);
		}

		[Test]
		public async Task GetErrorsAsync_Should_Return_List_of_errors()
		{
			// Create some valid XML for the EtaManager to parse when calling the IEtaWebApi methods.
			var xml =
				@"<?xml version=""1.0"" encoding=""utf - 8""?>" +
				@"<eta version=""1.0"" xmlns=""http://www.eta.co.at/rest/v1"">" +
					@"<errors uri=""/user/errors"">" +
						@"<fub uri=""/112/10021"" name=""Kessel"">" +
							@"<error msg=""Flue gas sensor Interrupted"" priority=""Error"" time=""2011-06-29 12:47:50"">Sensor or Cable broken or badly connected</error>" +
							@"<error msg=""Water pressure too low 0,00 bar"" priority=""Error"" time=""2011-06-29 12:48:12"">Top up heating water! If this warning occurs more than once a year, please contact plumber.</error>" +
						@"</fub>" +
						@"<fub uri=""/112/10101"" name=""HK1""/>" +
					@"</errors>" +
				@"</eta>";

			// Make the mock return the XML from above.
			this.etaWebApiMock.Setup(x => x.GetErrorsXmlAsync(CancellationToken.None)).ReturnsAsync(xml);

			// Call EtaManager method. EtaManager uses IEtaWebApi (passed in by DI Container) and will receive the XML specified in the mock. Magic.
			var errors = await this.etaManager.GetErrorsAsync();

			// Expecting two errors from the XML above.
			errors.Count.ShouldEqual(2);

			// Expecting dates and message to be correct.
			errors[0].ShouldEqual(new EtaError("Flue gas sensor Interrupted", "Sensor or Cable broken or badly connected", new DateTime(2011, 6, 29, 12, 47, 50)));
		}
	}
}

