using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace ETA.Shared
{
	/// <summary>
	/// Holds information about errors in the ETA system.
	/// </summary>
	public struct EtaError
	{
		public EtaError (string message, string reason, DateTime occurredAt)
		{
			this.Message = message;
			this.Reason = reason;
			this.OccurredAt = occurredAt;
		}

		/// <summary>
		/// Error message.
		/// </summary>
		public string Message
		{
			get;
		}

		/// <summary>
		/// Possible reason or further explanation.
		/// </summary>
		public string Reason
		{
			get;
		}

		/// <summary>
		/// Point in time of occurrence.
		/// </summary>
		public DateTime OccurredAt
		{
			get;
		}

		public override string ToString ()
		{
			return $"[EtaError: Message={this.Message}, Reason={this.Reason}, OccurredAt={this.OccurredAt}]";
		}
	}

}

