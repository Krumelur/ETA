using NUnit.Framework;
using System;
using EtaShared;
using System.Threading.Tasks;
using System.Threading;
using Ploeh.AutoFixture;
using Moq;
using Should;

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
			try
			{
				// Create mocks for our interfaces.
				this.etaWebApiMock = new Mock<IEtaWebApi>();
				this.loggerMock = new Mock<ILogger>();

				this.storageMock = new Mock<IStorage>();

				var fixture = new Fixture();
				this.etaManager = new EtaManager(this.etaWebApiMock.Object, this.loggerMock.Object, storageMock.Object, () => new SupplyData());

				this.etaManager.Config = new EtaConfig("192.168.178.35", 8080);
			}
			catch (Exception ex)
			{
				// Exception in Setup methods are not reported by Nunit and silently swallowed. Can be annoying. So let's log this.
				Console.WriteLine(ex);

			}
		}

		[TestFixtureTearDown]
		public void TearDown()
		{
		}

		EtaManager etaManager;
		Mock<IEtaWebApi> etaWebApiMock;
		Mock<ILogger> loggerMock;
		Mock<IStorage> storageMock;

		public IEtaWebApi WebApi => this.etaWebApiMock.Object;

		[Test]
		public async Task EtaManager_GetApiVersionAsync_Should_Return_Valid_Version()
		{
			// Create a random version so we're not always testing agsinst the same hardcoded string.
			// This will generate a random string.
			var fixture = new Fixture();
			var expectedApiVersion = fixture.Create<string>();

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
		public async Task EtaManager_GetApiVersionAsync_Should_Return_NULL_for_invalid_XML()
		{
			var expectedApiVersion = "some invalid xml string";
			this.etaWebApiMock.Setup(x => x.GetApiVersionXmlAsync(CancellationToken.None)).ReturnsAsync(expectedApiVersion);

			var apiVersion = await this.etaManager.GetApiVersionAsync();
			apiVersion.ShouldBeNull();
		}

		[Test]
		public async Task EtaManager_GetSuppliesAsync_Should_Return_Valid_Amount()
		{
			var fixture = new Fixture();
			// Create a random value so we're not always testing agsinst the same hardcoded value.
			var expectedSuppliesAmount = fixture.Create<NumericUnit>();
			fixture.Customizations.Add(new RandomNumericSequenceGenerator(10, 100));
			var scaleFactor = fixture.Create<int>();
			fixture.Customizations.Clear();

			// Create some valid XML for the EtaManager to parse when calling the IEtaWebApi methods.
			var xml =
				@"<?xml version=""1.0"" encoding=""utf - 8""?>" +
				@"<eta version=""1.0"" xmlns=""http://www.eta.co.at/rest/v1"">" +
					@"<value uri=""/user/var/112/10201/0/0/12015"" strValue=""" + expectedSuppliesAmount.Value + @""" unit=""" + expectedSuppliesAmount.Unit + @""" decPlaces=""0"" scaleFactor=""" + scaleFactor + @""" advTextOffset=""0"">" + expectedSuppliesAmount.Value*scaleFactor + "</value>" +
				@"</eta>";

			// Make the mock return the XML from above.
			this.etaWebApiMock.Setup(x => x.GetSuppliesXmlAsync(CancellationToken.None)).ReturnsAsync(xml);

			// Call EtaManager method. EtaManager uses IEtaWebApi (passed in by DI Container) and will receive the XML specified in the mock. Magic.
			var amount = await this.etaManager.GetSuppliesAsync();

			// Whatever the actual version string is - we don't care, but it must match our original version.
			amount.ShouldEqual(expectedSuppliesAmount);
		}

		[Test]
		public async Task EtaManager_GetSuppliesAsync_Should_save_value_to_storage()
		{
			var fixture = new Fixture();
			// Create a random value so we're not always testing agsinst the same hardcoded value.
			var expectedSuppliesAmount = fixture.Create<NumericUnit>();
			fixture.Customizations.Add(new RandomNumericSequenceGenerator(10, 100));
			var scaleFactor = fixture.Create<int>();
			fixture.Customizations.Clear();

			// Create some valid XML for the EtaManager to parse when calling the IEtaWebApi methods.
			var xml =
				@"<?xml version=""1.0"" encoding=""utf - 8""?>" +
				@"<eta version=""1.0"" xmlns=""http://www.eta.co.at/rest/v1"">" +
					@"<value uri=""/user/var/112/10201/0/0/12015"" strValue=""" + expectedSuppliesAmount.Value + @""" unit=""" + expectedSuppliesAmount.Unit + @""" decPlaces=""0"" scaleFactor=""" + scaleFactor + @""" advTextOffset=""0"">" + expectedSuppliesAmount.Value * scaleFactor + "</value>" +
				@"</eta>";

			// Make the mock return the XML from above.
			this.etaWebApiMock.Setup(x => x.GetSuppliesXmlAsync(CancellationToken.None)).ReturnsAsync(xml);

			// If not reset, SaveSupplyDataAsync() might be called more than once because it is also called as part of other tests.
			this.storageMock.ResetCalls();

			// Call EtaManager method. EtaManager uses IEtaWebApi (passed in by DI Container) and will receive the XML specified in the mock. Magic.
			var amount = await this.etaManager.GetSuppliesAsync();

			// We want to see that the parsed data gets saved to the DB.
			this.storageMock.Verify(x => x.SaveSupplyDataAsync(It.IsNotNull<ISupplyData>()), Times.Once);
		}

		[Test]
		public async Task EtaManager_GetSuppliesAsync_Should_Return_Empty_Amount_for_invalid_XML()
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
		public async Task EtaManager_GetSuppliesWarningLevelAsync_Should_Return_Valid_Amount()
		{
			var fixture = new Fixture();
			// Create a random value so we're not always testing agsinst the same hardcoded value.
			var expectedSuppliesWarningLevelAmount = fixture.Create<NumericUnit>();
			fixture.Customizations.Add(new RandomNumericSequenceGenerator(10, 100));
			var scaleFactor = fixture.Create<int>();
			fixture.Customizations.Clear();

			// Create some valid XML for the EtaManager to parse when calling the IEtaWebApi methods.
			var xml =
				@"<?xml version=""1.0"" encoding=""utf - 8""?>" +
				@"<eta version=""1.0"" xmlns=""http://www.eta.co.at/rest/v1"">" +
					@"<value uri=""/user/var/112/10201/0/0/12042"" strValue=""" + expectedSuppliesWarningLevelAmount.Value + @""" unit=""" + expectedSuppliesWarningLevelAmount.Unit + @""" decPlaces=""0"" scaleFactor=""" + scaleFactor + @""" advTextOffset=""0"">" + expectedSuppliesWarningLevelAmount.Value * scaleFactor + "</value>" +
				@"</eta>";

			// Make the mock return the XML from above.
			this.etaWebApiMock.Setup(x => x.GetSuppliesWarningLevelXml(CancellationToken.None)).ReturnsAsync(xml);

			// Call EtaManager method. EtaManager uses IEtaWebApi (passed in by DI Container) and will receive the XML specified in the mock. Magic.
			var amount = await this.etaManager.GetSuppliesWarningLevelAsync();

			// Whatever the actual version string is - we don't care, but it must match our original version.
			amount.ShouldEqual(expectedSuppliesWarningLevelAmount);
		}

		[Test]
		public async Task EtaManager_GetTotalConsumptionAsync_Should_Return_Valid_Amount()
		{
			var fixture = new Fixture();
			// Create a random value so we're not always testing agsinst the same hardcoded value.
			var expectedConsumption = fixture.Create<NumericUnit>();

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
		public async Task EtaManager_GetErrorsAsync_Should_Return_correct_amount_of_errors()
		{
			// Create some valid XML for the EtaManager to parse when calling the IEtaWebApi methods.
			var xml =
				@"<?xml version=""1.0"" encoding=""utf - 8""?>" +
				@"<eta version=""1.0"" xmlns=""http://www.eta.co.at/rest/v1"">" +
					@"<errors uri=""/user/errors"">" +
						@"<fub uri=""/112/10021"" name=""Kessel"">" +
							@"<error msg=""Flue gas sensor Interrupted"" priority=""Error"" time=""2011-06-29 12:47:50"">Sensor or Cable broken or badly connected</error>" +
							@"<error msg=""Water pressure too low 0,00 bar"" priority=""Error"" time=""2011-06-29 12:48:12"">Top up heating water! If this warning occurs more than once a year, please contact plumber.</error>" +
							@"<error msg=""Erinnerung Aschebox leeren 1000 kg"" priority=""Warnung"" time=""2015-12-21 07:00:00"">Die Verschlüsse an der Aschebox öffnen und diese vom Kessel abziehen und entleeren. Der Zählerstand [Verbrauch seit Aschebox leeren] wird beim Abnehmen der Aschebox automatisch auf Null zurückgesetzt.</error>" +
						@"</fub>" +
						@"<fub uri=""/112/10101"" name=""HK1""/>" +
					@"</errors>" +
				@"</eta>";

			// Make the mock return the XML from above.
			this.etaWebApiMock.Setup(x => x.GetErrorsXmlAsync(CancellationToken.None)).ReturnsAsync(xml);

			// Call EtaManager method. EtaManager uses IEtaWebApi (passed in by DI Container) and will receive the XML specified in the mock. Magic.
			var errors = await this.etaManager.GetErrorsAsync();

			// Expecting three errors from the XML above.
			errors.Count.ShouldEqual(3);
		}

		[Test]
		public async Task EtaManager_GetErrorsAsync_Should_Return_correct_error_types()
		{
			// Create some valid XML for the EtaManager to parse when calling the IEtaWebApi methods.
			var xml =
				@"<?xml version=""1.0"" encoding=""utf - 8""?>" +
				@"<eta version=""1.0"" xmlns=""http://www.eta.co.at/rest/v1"">" +
				@"<errors uri=""/user/errors"">" +
				@"<fub uri=""/112/10021"" name=""Kessel"">" +
				@"<error msg=""Flue gas sensor Interrupted"" priority=""Error"" time=""2011-06-29 12:47:50"">Sensor or Cable broken or badly connected</error>" +
				@"<error msg=""Erinnerung Aschebox leeren 1000 kg"" priority=""Warnung"" time=""2015-12-21 07:00:00"">Die Verschlüsse an der Aschebox öffnen und diese vom Kessel abziehen und entleeren. Der Zählerstand [Verbrauch seit Aschebox leeren] wird beim Abnehmen der Aschebox automatisch auf Null zurückgesetzt.</error>" +
				@"</fub>" +
				@"<fub uri=""/112/10101"" name=""HK1""/>" +
				@"</errors>" +
				@"</eta>";

			// Make the mock return the XML from above.
			this.etaWebApiMock.Setup(x => x.GetErrorsXmlAsync(CancellationToken.None)).ReturnsAsync(xml);

			// Call EtaManager method. EtaManager uses IEtaWebApi (passed in by DI Container) and will receive the XML specified in the mock. Magic.
			var errors = await this.etaManager.GetErrorsAsync();

			// Expecting dates and message to be correct.
			errors[0].ShouldEqual(new EtaError("Flue gas sensor Interrupted", "Sensor or Cable broken or badly connected", new DateTime(2011, 6, 29, 12, 47, 50), EtaError.ERROR_TYPE.Error));
			errors[1].ShouldEqual(new EtaError("Erinnerung Aschebox leeren 1000 kg", "Die Verschlüsse an der Aschebox öffnen und diese vom Kessel abziehen und entleeren. Der Zählerstand [Verbrauch seit Aschebox leeren] wird beim Abnehmen der Aschebox automatisch auf Null zurückgesetzt.", new DateTime(2015, 12, 21, 7, 0, 0), EtaError.ERROR_TYPE.Warning));
		}
	}
}

