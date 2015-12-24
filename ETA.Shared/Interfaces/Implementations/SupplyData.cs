using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETA.Shared
{
	/// <summary>
	/// Implementation of ISupplyData.
	/// </summary>
	public class SupplyData : ISupplyData
	{
		public double Amount { get; set; }
		
		public int Id { get; set; }
		
		public DateTime TimeStamp { get; set; }

		public string Unit { get; set; }

		public override string ToString() => $"[SupplyData]: Id={this.Id}; Amount={this.Amount}; TimeStamp={this.TimeStamp}; Unit={this.Unit}";
	}
}
