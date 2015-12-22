using NUnit.Framework;
using System;
using ETA.Shared;
using System.Threading.Tasks;
using System.Threading;
using Should;
using System.IO;
using Ploeh.AutoFixture;

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

			this.storage = new Storage(null)
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
	}
}

