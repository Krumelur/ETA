using GalaSoft.MvvmLight;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace ETA.Shared
{
	/// <summary>
	/// View model to show information about pellets supplies.
	/// </summary>
	public class SuppliesViewModel : BaseViewModel
	{
		public SuppliesViewModel (EtaManager manager) : base ()
		{
			this.manager = manager;
		}

		EtaManager manager;

		public ICommand UpdateSuppliesInfoCommand
		{
			get
			{
				return new RelayCommand(async () => {
					this.supplies = await this.manager.GetSuppliesAsync();
					this.RaisePropertyChanged(nameof(SuppliesDisplayValue));
				}, () => true);
			}
		}

		NumericUnit supplies;

		/// <summary>
		/// Defines the filling of the supplies storage in percent. Changing this value will also change SuppliesFillAbsoluteValue.
		/// </summary>
		public double SuppliesFillPercentage
		{
			get {
				return this.suppliesFillPercentage;
			}
			set {
				if (value != this.suppliesFillPercentage)
				{
					this.suppliesFillPercentage = value;
					this.RaisePropertyChanged ();
					this.RaisePropertyChanged(nameof(SuppliesFillAbsoluteValue));
				}
			}
		}
		double suppliesFillPercentage;

		/// <summary>
		/// Used by SuppliesFillAbsoluteValue in combination with SuppliesFillPercentage to calculate an absolute value of the supplies which can be used to display some sort of graphical representation in the UI.
		/// Changing this value will also change SuppliesFillAbsoluteValue.
		/// </summary>
		public double SuppliesFillReferenceValue
		{
			get {
				return this.suppliesFillReferenceValue;
			}
			set {
				if (value != this.SuppliesFillReferenceValue)
				{
					this.suppliesFillReferenceValue = value;
					this.RaisePropertyChanged ();
					this.RaisePropertyChanged(nameof(SuppliesFillAbsoluteValue));
				}
			}
		}
		double suppliesFillReferenceValue;

		/// <summary>
		/// Readonly value which represents the absolute amount of supplies based on a maximum of SuppliesFillReferenceValue.
		/// </summary>
		public double SuppliesFillAbsoluteValue
		{
			get {
				return this.SuppliesFillPercentage * this.SuppliesFillReferenceValue;
			}
		}

		public string SuppliesDisplayValue
		{
			get
			{
				return $"{this.supplies.Value}{this.supplies.Unit}";
			}
		}
	}
}