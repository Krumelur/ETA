namespace ETA.Shared
{
	/// <summary>
	/// A numeric value which is associated with a unit, for example "5kg".
	/// </summary>
	public struct NumericUnit
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ETA.Shared.NumericUnit"/> struct.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <param name="unit">Unit.</param>
		public NumericUnit (double value, string unit)
		{
			this.Value = value;
			this.Unit = unit;
		}

		/// <summary>
		/// Gets the value.
		/// </summary>
		public double Value
		{
			get;
		}

		/// <summary>
		/// Gets the unit.
		/// </summary>
		public string Unit
		{
			get;
		}

		/// <summary>>
		/// Implicitly cast a numeric unit to a double.
		/// </summary>
		/// <param name="numUnit">Number unit.</param>
		public static implicit operator double(NumericUnit numUnit) => numUnit.Value;

		public override string ToString () => $"[NumericUnit: Value={this.Value}, Unit={this.Unit}]";
	}

}

