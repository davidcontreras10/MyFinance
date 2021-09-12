using System;
using MyFinance.CustomControls;
using MyFinance.Models.UserInterface;
using MyFinance.Views;
using Xamarin.Forms;
using Xamarin.Forms.DataGrid;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace MyFinance
{
	public partial class App : Application
	{
		#region Attributes

		private bool _forceReloadFromProperties;
		public MainMenuPage MainMenuPageInstance { get; set; }

		#endregion

		#region Constructor

		public App ()
		{
			DataGridComponent.Init();
			InitializeComponent();
			InitializeMainMenuStyles();
			MainPage = new MbNavigationPage(new MainPage());
		}

		#endregion

		#region Protected

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}

		#endregion

		#region Privates

		private void InitializeMainMenuStyles()
		{
			Resources["MainPageItemStyle"] = Resources[MainMenuModel.Current.SelectedMenuItemStyle];
			Resources["AccountsPageItemStyle"] = Resources[MainMenuModel.Current.NonSelectedMenuItemStyle];
			Resources["SpendingTypesPageItemStyle"] = Resources[MainMenuModel.Current.NonSelectedMenuItemStyle];
			Resources["MyAccountPageItemStyle"] = Resources[MainMenuModel.Current.NonSelectedMenuItemStyle];
		}

		private void ReloadAuthProperties()
		{
			//if ((AuthManager.Token != null && !_forceReloadFromProperties) || !ApplicationProperties.ContainsKey("AuthToken"))
			//{
			//	if (_forceReloadFromProperties)
			//	{
			//		_forceReloadFromProperties = false;
			//	}

			//	return;
			//}

			//AuthManager.SaveReloadedToken(ApplicationProperties.GetValue<AuthToken>("AuthToken"));
		}

		private void MenuButton_OnClicked(object sender, EventArgs e)
		{
			var willShow = !MainHeaderModel.Current.ShowMainMenu;
			MainHeaderModel.Current.ShowMainMenu = willShow;
		}

		#endregion
	}
}
