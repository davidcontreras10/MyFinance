using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using MyFinance.Models.UserInterface;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyFinance.CustomControls
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainMenuPage
	{
		#region Constructor

		public MainMenuPage()
		{
			InitializeComponent();
			BindingContext = this;

			var app = (App)Application.Current;
			app.MainMenuPageInstance = this;
			MainMenuModel.Current.UpdateMenuControls = null;
			MainMenuModel.Current.UpdateMenuControls += UpdateMenuControls;
		}

		#endregion

		#region Methods

		private void UpdateMenuControls(object sender, EventArgs eventArgs)
		{
			foreach (var child in MainGrid.Children)
			{
				var menuItem = (StackLayout)child;
				var labelsStackLayout = (StackLayout)menuItem.Children.First(c => c is StackLayout);
				var expandLabel = (Label)labelsStackLayout.Children[1];
				var currentMenuName = GetMenuPageName(menuItem);
				var grid = (Grid)menuItem.Children.FirstOrDefault(c => c is Grid);
				var isCurrentPageSelected = currentMenuName == MainMenuModel.Current.CurrentPageNameSelected;
				if (grid != null)
				{
					grid.IsVisible = isCurrentPageSelected && MainMenuModel.Current.IsCurrentPageMenuOpened;
				}

				expandLabel.Text = GetExpandLabelText(grid, isCurrentPageSelected,
					MainMenuModel.Current.IsCurrentPageMenuOpened);
				var menuStyleName = currentMenuName + "ItemStyle";
				var newResource = isCurrentPageSelected
					? MainMenuModel.Current.SelectedMenuItemStyle
					: MainMenuModel.Current.NonSelectedMenuItemStyle;
				UpdateMenuPageStyleResource(menuStyleName, newResource);
			}
		}

		private void UpdateMenuPageStyleResource(string dynamicResource, string newResourceName)
		{
			var resources = Application.Current.Resources;
			resources[dynamicResource] = resources[newResourceName];
		}

		private static string GetExpandLabelText(Grid grid, bool isCurrentPageSelected, bool isOpen)
		{
			if (isCurrentPageSelected)
			{
				if (grid == null)
				{
					return "";
				}

				return isOpen ? "--" : "+";
			}

			return "X";
		}

		private static string GetMenuPageName(StackLayout menuItem)
		{
			var labelsStackLayout = (StackLayout)menuItem.Children.First(c => c is StackLayout);
			var gestureRecognizer = labelsStackLayout.GestureRecognizers.First(gr => gr is GestureRecognizer);
			if (gestureRecognizer is TapGestureRecognizer tabGestureRecognizer)
			{
				return tabGestureRecognizer.CommandParameter?.ToString();
			}
			return "";
		}

		private async Task<bool> OpenWindowAsync(string requestedWindow)
		{
			if (string.IsNullOrEmpty(requestedWindow))
			{
				throw new ArgumentNullException(nameof(requestedWindow));
			}

			if (requestedWindow == MainMenuModel.Current.MainPageValue)
			{
				MainHeaderModel.Current.ShowMainMenu = false;
				//await OpenWindowAsync(ViewsContainer.Get.MainPageInstnace);
				throw new NotImplementedException();
				MainMenuModel.Current.SetPage(MainMenuModel.Current.MainPageValue);
			}

			if (requestedWindow == MainMenuModel.Current.AccountsValue)
			{
				MainHeaderModel.Current.ShowMainMenu = false;
				//await OpenWindowAsync(ViewsContainer.Get.AccountsPageInstance);
				throw new NotImplementedException();
				MainMenuModel.Current.SetPage(MainMenuModel.Current.AccountsValue);
			}

			if (requestedWindow == MainMenuModel.Current.MyAccountValue)
			{
				MainHeaderModel.Current.ShowMainMenu = false;
				//await AuthManager.SignoutAsync();
				throw new NotImplementedException();
			}

			return true;
		}

		private async Task<bool> OpenWindowAsync(Page page)
		{
			if (Navigation.NavigationStack.Any(p => p == page))
			{
				Navigation.RemovePage(page);
			}

			await Navigation.PushAsync(page);
			return true;
		}

		#endregion

		#region Notify Property

		protected bool SetValue<T>(ref T backingField, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(backingField, value))
				return false;

			backingField = value;
			OnPropertyChanged(propertyName);
			return true;
		}

		#endregion

		#region Backing Field

		private string _mainPageExpandIcon;
		private bool _mainPageExpanded;

		#endregion

		#region Properties

		public bool MainPageExpanded
		{
			get => _mainPageExpanded;
			set => SetValue(ref _mainPageExpanded, value);
		}

		public string MainPageExpandIcon
		{
			get => _mainPageExpandIcon;
			set => SetValue(ref _mainPageExpandIcon, value);
		}

		#endregion

		#region Events

		private async void ExpandItem_OnTapped(object sender, EventArgs e)
		{
			var parameter = ((TappedEventArgs)e).Parameter.ToString();
			var isCurrent = parameter == MainMenuModel.Current.CurrentPageNameSelected;
			if (isCurrent)
			{
				Debug.WriteLine("IsCurrent true");
				MainMenuModel.Current.SwitchMenuVisibility();
			}
			else
			{
				Debug.WriteLine("IsCurrent false");
				var result = await OpenWindowAsync(parameter);
				if (result)
				{
					MainMenuModel.Current.CloseMenu();
				}
			}
		}

		#endregion
	}
}