using System;

namespace ETA.Shared
{
	/// <summary>
	/// Interface for logging purposes.
	/// </summary>
	public interface ILogger
	{
		/// <summary>
		/// Logs a string.
		/// </summary>
		/// <param name="s">message to log</param>
		void Log(string s);

		/// <summary>
		/// Logs an exception.
		/// </summary>
		/// <param name="ex">Exception to log</param>
		void Log(Exception ex);
	}

}

