using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyFinance.Models.UserInterface
{
    public class MainHeaderModel : INotifyPropertyChanged
    {
        public static MainHeaderModel Current = new MainHeaderModel();

        #region Private Properties

        private bool _pageBusy;
        private ObservableCollection<MbMenuItem> _mainMenuItems;
        private bool _showMainMenu;

        #endregion

        #region Public Properties

        public ObservableCollection<MbMenuItem> MainMenuItems => _mainMenuItems ?? (_mainMenuItems =
                                                                     new ObservableCollection<MbMenuItem>(UserInterfaceService.GetMainMenuItems()));

        public bool PageBusy
        {
            get => _pageBusy;
            set
            {
                if (_pageBusy == value)
                {
                    return;
                }

                _pageBusy = value;
                OnPropertyChanged();
            }
        }

        public bool ShowMainMenu
        {
            get => _showMainMenu;
            set
            {
                _showMainMenu = value; 
                OnPropertyChanged();
            }
        }

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