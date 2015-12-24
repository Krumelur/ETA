using System;

namespace ETA.Shared
{
	/// <summary>
	/// Use to preserve classes from being stripped by the linker.
	/// </summary>
	[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	public sealed class PreserveAttribute : Attribute
	{
		// Keep all members
		public bool AllMembers;
		// Keep member ONLY if type itself is kept
		public bool Conditional; 
	}
}