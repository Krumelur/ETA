using System;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;

namespace EtaShared
{
	public class AzureDataService
	{
		static Lazy<AzureDataService> azureDataService = new Lazy<AzureDataService> (() => new AzureDataService(), true);
		public static AzureDataService Instance => azureDataService.Value;

		public MobileServiceClient MobileService { get;  } = new MobileServiceClient("https://reneruppert.azurewebsites.net");
		IMobileServiceSyncTable<AzureSupplyData> suppliesTable;

		AzureDataService ()
		{
		}

		bool isInitialized;

		public async Task InitializeAsync()
		{
			if (this.isInitialized)
			{
				return;
			}

			this.isInitialized = true;
			const string path = "syncstore.db";
			var store = new MobileServiceSQLiteStore(path);
			store.DefineTable<AzureSupplyData>();
			await MobileService.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

			//Get our sync table that will call out to azure
			suppliesTable = this.MobileService.GetSyncTable<AzureSupplyData>();
		}

		public async Task AddSupplyAsync(ISupplyData supplyData)
		{
			var azureData = new AzureSupplyData {
				Amount = supplyData.Amount,
				Id = supplyData.Id.ToString(),
				NumericId = supplyData.Id,
				TimeStamp = supplyData.TimeStamp,
				Unit = supplyData.Unit
			};
			if (await this.suppliesTable.LookupAsync (azureData.Id) == null)
			{
				await this.suppliesTable.InsertAsync (azureData);
			}
			await this.SyncSuppliesAsync();
		}

		public async Task SyncSuppliesAsync()
		{
			await this.suppliesTable.PullAsync("allSupplies", suppliesTable.CreateQuery());
			await this.MobileService.SyncContext.PushAsync();
		}
	}
}

