using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EtaShared
{
	/// <summary>
	/// Implementation of ISupplyData.
	/// </summary>
	public class SupplyData : ISupplyData
	{
		public virtual double Amount { get; set; }
		
		public virtual int Id { get; set; }
		
		public virtual DateTime TimeStamp { get; set; }

		public virtual string Unit { get; set; }

		public override string ToString() => $"[SupplyData]: Id={this.Id}; Amount={this.Amount}; TimeStamp={this.TimeStamp}; Unit={this.Unit}";
	}

	[Microsoft.WindowsAzure.MobileServices.DataTable("Pellets")]
	public class AzureSupplyData : SupplyData
	{
		[Microsoft.WindowsAzure.MobileServices.Version]
		public string AzureVersion { get; set; }

		new public string Id {
			get;
			set;
		}

		public int NumericId {
			get;
			set;
		}



		public override string ToString() => $"[SupplyData]: Id={this.Id}; Amount={this.Amount}; TimeStamp={this.TimeStamp}; Unit={this.Unit}";
	}
}
