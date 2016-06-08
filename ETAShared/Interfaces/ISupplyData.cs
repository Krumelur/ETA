using System;

namespace EtaShared
{
	public interface ISupplyData
	{
		DateTime TimeStamp { get; set; }

		double Amount { get; set; }

		string Unit { get; set; }
	}
}