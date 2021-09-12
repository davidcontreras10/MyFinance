using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace MyFinance
{
	public class GlobalResources : INotifyPropertyChanged
	{
		// Singleton
		public static GlobalResources Current = new GlobalResources();

		#region Orientation

		#region Private Properties

		private StackOrientation _deviceOrientation;
		private double _latestWithScreen;
		private double _latestHeightScreen;

		#endregion

		#region Public Properties

		public StackOrientation DeviceOrientation
		{
			get => _deviceOrientation;
			private set
			{
				_deviceOrientation = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Public Methods

		public bool SetOrientation(StackOrientation orientation, double width, double height)
		{
			if (width == 0 || height == 0 || width == _latestWithScreen || height == _latestHeightScreen)
				return false;

			_latestWithScreen = width;
			_latestHeightScreen = height;
			var result = DeviceOrientation == orientation;
			DeviceOrientation = orientation;
			return result;
		}

		#endregion

		#endregion

		#region Property Changed

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}
}
