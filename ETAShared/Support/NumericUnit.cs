namespace EtaShared
{
	/// <summary>
	/// A numeric value which is associated with a unit, for example "5kg".
	/// </summary>
	public struct NumericUnit
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="EtaShared.NumericUnit"/> struct.
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
		public static NumericUnit Empty => new NumericUnit(0f, string.Empty);

		/// <summary>>
		/// Implicitly cast a numeric unit to a double.
		/// </summary>
		/// <param name="numUnit">Number unit.</param>
		public static implicit operator double(NumericUnit numUnit) => numUnit.Value;

		public override string ToString () => $"[NumericUnit: Value={this.Value}, Unit={this.Unit}]";

		public override bool Equals(object obj)
		{
			return obj is NumericUnit && this == (NumericUnit)obj;
		}
		public override int GetHashCode()
		{
			return this.Unit.GetHashCode() ^ this.Value.GetHashCode();
		}
		public static bool operator ==(NumericUnit x, NumericUnit y)
		{
			return x.Value == y.Value && x.Unit == y.Unit;
		}
		public static bool operator !=(NumericUnit x, NumericUnit y)
		{
			return !(x == y);
		}
	}

}

