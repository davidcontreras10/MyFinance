using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyFinance
{
	public partial class MainPage : ContentPage
	{
		private bool _onAppearingLocked;
		private bool _hasInitialized;

		public MainPage()
		{
			InitializeComponent();
			NavigationPage.SetHasNavigationBar(this, false);
		}
	}
}
