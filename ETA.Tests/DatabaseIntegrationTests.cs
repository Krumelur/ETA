using NUnit.Framework;
using System;
using EtaShared;
using System.Threading.Tasks;
using System.Threading;
using Should;
using System.IO;
using Ploeh.AutoFixture;
using Moq;

namespace ETA.Tests
{
	/// <summary>
	/// Integration tests for accessing the Sqlite database.
	/// </summary>
	[TestFixture]
	public class DatabaseIntegrationTests
	{
		[TestFixtureSetUp]
		public void Setup()
		{
			try
			{
				File.Delete("test.sqlite");
			}
			catch
			{
				// File did not exist.
			}

			this.storage = new DatabaseStorage(null)
			{
				DatabasePath = "test.sqlite"
			};

			this.fixture = new Fixture();
		}

		IStorage storage;
		IFixture fixture;

		[Test]
		public async Task Saved_config_value_should_be_retrievable ()
		{

			var key = this.fixture.Create<string>();
			var inputValue  = this.fixture.Create<string>();

			await this.storage.SetConfigValueAsync(key, inputValue);
			var resultValue = await this.storage.GetConfigValueAsync(key, "defaultValue");

			resultValue.ShouldEqual(inputValue);
		}

		[Test]
		public async Task Not_existing_config_key_should_return_default()
		{

			var key = this.fixture.Create<string>();
			var defaultValue = this.fixture.Create<string>();
			var resultValue = await this.storage.GetConfigValueAsync(key, defaultValue);

			resultValue.ShouldEqual(defaultValue);
		}

		[Test]
		public async Task Saving_null_value_should_delete_config_entry()
		{

			var key = this.fixture.Create<string>();
			var inputValue = this.fixture.Create<string>();
			var defaultValue = this.fixture.Create<string>();

			// Store a value.
			await this.storage.SetConfigValueAsync(key, inputValue);
			// Delete same value.
			await this.storage.SetConfigValueAsync(key, null);
			// Retrieve value should deliver default.
			var resultValue = await this.storage.GetConfigValueAsync(key, defaultValue);

			resultValue.ShouldEqual(defaultValue);
		}

		[Test]
		public async Task Saved_new_supply_data_should_be_retrievable()
		{

			// We need an implementation of ISupplyData but without relying on a specific class.
			// Mocking this is not as easy as it seems: AutoFixture does not know how to create instances of interfaces, it 
			// requires an implementing class.
			// Moq can help here, however by default it does not support getters and setters.
			var supplyDataMock = new Mock<ISupplyData>();
			// This enabled properties to work as expected in the Mock object. Only when this is called, the properties can be assigned to.
			supplyDataMock.SetupAllProperties();

			var supplyData = supplyDataMock.Object;

			// To create test values for the properties we can sue AutoFixture.
			supplyData.Amount = this.fixture.Create<double>();
			supplyData.Unit = this.fixture.Create<string>();
			supplyData.TimeStamp = this.fixture.Create<DateTime>();

			// Store data.
			supplyData = await this.storage.SaveSupplyDataAsync(supplyData);

			// Retrieve value again.
			var result = await this.storage.GetSupplyDataAsync(supplyData.Id);

			result.Id.ShouldBeGreaterThan(0);
			result.Amount.ShouldEqual(supplyData.Amount);
			result.Unit.ShouldEqual(supplyData.Unit);
			result.TimeStamp.ShouldEqual(supplyData.TimeStamp);
		}

		[Test]
		public async Task Get_filtered_supply_data_should_succeed()
		{
			for (int i = 0; i < 100; i++)
			{
				var supplyDataMock = new Mock<ISupplyData>();
				// This enabled properties to work as expected in the Mock object. Only when this is called, the properties can be assigned to.
				supplyDataMock.SetupAllProperties();

				var supplyData = supplyDataMock.Object;

				// To create test values for the properties we can sue AutoFixture.
				supplyData.Amount = i;
				supplyData.Unit = this.fixture.Create<string>();
				supplyData.TimeStamp = this.fixture.Create<DateTime>();

				// Store data.
				supplyData = await this.storage.SaveSupplyDataAsync(supplyData);
			}

			// Get data where amount < 50.
			var result = await this.storage.GetSupplyDataAsync(data => data.Amount < 50);

			result.Count.ShouldEqual(50);
		}
	}
}

