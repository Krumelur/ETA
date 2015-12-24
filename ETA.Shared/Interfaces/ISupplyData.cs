﻿using System;

namespace ETA.Shared
{
	public interface ISupplyData
	{
		int Id { get; set; }

		DateTime TimeStamp { get; set; }

		double Amount { get; set; }

		string Unit { get; set; }
	}
}