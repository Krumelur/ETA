﻿using System;
using System.Diagnostics;

namespace ETA.Shared
{
	internal class DebugLogger : ILogger
	{
		public void Log(Exception ex)
		{
			Debug.WriteLine($"[ETA] Oops, something went wrong: {ex}");
		}

		public void Log(string s)
		{
			Debug.WriteLine($"[ETA] {s}");
		}
	}
}