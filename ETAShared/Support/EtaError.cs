using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace EtaShared
{
	/// <summary>
	/// Holds information about errors in the ETA system.
	/// </summary>
	public struct EtaError
	{
		/// <summary>
		/// Types of errors
		/// </summary>
		public enum ERROR_TYPE
		{
			/// <summary>
			/// An error which requires immediate action.
			/// </summary>
			Error,
			/// <summary>
			/// A warning message.
			/// </summary>
			Warning
		}

		public EtaError(string message, string reason, DateTime occurredAt, ERROR_TYPE errorType)
		{
			this.Message = message;
			this.Reason = reason;
			this.OccurredAt = occurredAt;
			this.ErrorType = errorType;
		}

		/// <summary>
		/// Gets the type of the error.
		/// </summary>
		public ERROR_TYPE ErrorType
		{
			get;
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

		public override string ToString() => $"[EtaError: Message={this.Message}, Reason={this.Reason}, OccurredAt={this.OccurredAt}, Type={this.ErrorType}]";

		public override bool Equals(object obj)
		{
			return obj is EtaError && this == (EtaError)obj;
		}

		public override int GetHashCode()
		{
			return this.Message.GetHashCode() ^ this.Reason.GetHashCode() ^ this.OccurredAt.GetHashCode() ^ this.ErrorType.GetHashCode();
		}

		public static bool operator ==(EtaError x, EtaError y)
		{
			return x.Message == y.Message && x.Reason == y.Reason && x.OccurredAt == y.OccurredAt && x.ErrorType == y.ErrorType;
		}

		public static bool operator !=(EtaError x, EtaError y)
		{
			return !(x == y);
		}
	}
}

