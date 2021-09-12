using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyFinance.Models.UserInterface
{
    public class MainMenuModel : INotifyPropertyChanged
    {
        public MainMenuModel()
        {
            CurrentPageNameSelected = MainPageValue;
            IsCurrentPageMenuOpened = false;
            _showPendingChanges = true;
        }

        public static MainMenuModel Current = new MainMenuModel();

        public void SetPage(string pageName)
        {
            CurrentPageNameSelected = pageName;
            UpdateMenuControls?.Invoke(this, new EventArgs());
        }

        public void CloseMenu()
        {
            IsCurrentPageMenuOpened = false;
        }

        public void OpenMenu()
        {
            IsCurrentPageMenuOpened = true;
        }

        public void SwitchMenuVisibility()
        {
            IsCurrentPageMenuOpened = !IsCurrentPageMenuOpened;
        }

        public void SubscribeShowPendingSpends(EventHandler showPendingSpendsHandler)
        {
            UpdateShowPendingSpends = showPendingSpendsHandler;
        }

        public EventHandler UpdateMenuControls;

        public EventHandler UpdateShowPendingSpends;

        #region Backing Fields

        private bool _isCurrentPageMenuOpened;
        private bool _showPendingChanges;

        #endregion

        #region Properties

        public string SelectedMenuItemStyle => "MainMenuItemHeaderSelected";
        public string NonSelectedMenuItemStyle => "MainMenuItemHeaderNonSelected";

        public string MainPageValue => "MainPage";
        public string AccountsValue => "AccountsPage";
        public string SpendingTypesValue => "SpendingTypesPage";
        public string MyAccountValue => "MyAccountPage";
        public string PendingSpendsValue => ShowPendingSpend ? "Hide Pending Spends" : "Show Pending Spends";
        public string CurrentPageNameSelected { get; private set; }

        public bool IsCurrentPageMenuOpened
        {
            get => _isCurrentPageMenuOpened;
            private set
            {
                _isCurrentPageMenuOpened = value;
                OnPropertyChanged();
            }
        }

        public bool ShowPendingSpend
        {
            get => _showPendingChanges;
            set
            {
                _showPendingChanges = value;
                UpdateShowPendingSpends?.Invoke(this, new EventArgs());
                OnPropertyChanged(nameof(PendingSpendsValue));
            }
        }

        #endregion

        #region Notify Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
