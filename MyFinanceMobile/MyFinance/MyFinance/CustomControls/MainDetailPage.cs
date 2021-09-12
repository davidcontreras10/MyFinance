using Xamarin.Forms;

namespace MyFinance.CustomControls
{
    public abstract class MainDetailPage : ContentPage
    {
        #region Backing field

        private ContentView _mainViewLayout;
        private ContentView _detailViewLayout;

        #endregion

        #region Private Attributes

        private readonly ContentView _verticalContent;
        private readonly Grid _horizontalGrid;

        #endregion

        #region Bindable Properties

        public static readonly BindableProperty PageOrientationProperty =
            BindableProperty.Create(
                nameof(PageOrientation), typeof(StackOrientation), typeof(MainDetailPage), StackOrientation.Vertical, propertyChanged: OnOrientationChanged);

        public static readonly BindableProperty IsDetailVisibleProperty =
            BindableProperty.Create(
                nameof(IsDetailVisible), typeof(bool), typeof(MainDetailPage), false,
                propertyChanged: OnIsDetailVisibleChanged);

        #endregion

        #region Constructor

        protected MainDetailPage(StackOrientation pageOrientation = StackOrientation.Vertical, bool detailVisible = false)
        {
            IsDetailVisible = detailVisible;
            _horizontalGrid = CreateGrid();
            _verticalContent = new ContentView();
            PageOrientation = pageOrientation;
        }

        #endregion

        #region Properties

        public new View Content
        {
            get => base.Content;
            private set => base.Content = value;
        }

        public ContentView MainViewLayout
        {
            get => _mainViewLayout;
            set
            {
                _mainViewLayout = value;
                UpdatePages();
            }
        }

        public ContentView DetailViewLayout
        {
            get => _detailViewLayout;
            set
            {
                _detailViewLayout = value;
                UpdatePages();
            }
        }

        public StackOrientation PageOrientation
        {
            get => (StackOrientation)GetValue(PageOrientationProperty);
            set
            {
                SetValue(PageOrientationProperty, value);
                UpdatePages();
            }
        }

        public bool IsDetailVisible
        {
            get => (bool)GetValue(IsDetailVisibleProperty);
            set
            {
                if (PageOrientation != StackOrientation.Vertical)
                    return;
                SetValue(IsDetailVisibleProperty, value);
                UpdatePages();
            }
        }

        #endregion

        #region Methods

        private static void OnIsDetailVisibleChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is MainDetailPage page)
            {
                page.UpdatePages();
            }
        }

        private static void OnOrientationChanged(BindableObject bindable, object oldvalue, object newvalue)
        {
            if (bindable is MainDetailPage page)
            {
                page.UpdatePages();
            }
        }

        private Grid CreateGrid()
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.4, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.6, GridUnitType.Star) });
            return grid;
        }

        private void UpdatePages()
        {
            if (MainViewLayout == null || DetailViewLayout == null)
            {
                Content = null;
                return;
            }

            var isGridVisible = PageOrientation == StackOrientation.Horizontal;
            if (isGridVisible)
            {
                if (Content == _horizontalGrid)
                {
                    return;
                }

                _horizontalGrid.Children.Clear();
                _horizontalGrid.Children.Add(MainViewLayout, 0, 0);
                _horizontalGrid.Children.Add(DetailViewLayout, 1, 0);
                Content = _horizontalGrid;
            }
            else
            {
                if (Content == _verticalContent)
                {
                    return;
                }

                Content = IsDetailVisible ? DetailViewLayout : MainViewLayout;
            }
        }

        //protected override void OnSizeAllocated(double width, double height)
        //{
        //    try
        //    {
        //        base.OnSizeAllocated(width, height);
        //        var orientation = Width > Height ? StackOrientation.Horizontal : StackOrientation.Vertical;
        //        PageOrientation = orientation;
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.WriteLine(e);
        //    }
        //}

        protected override bool OnBackButtonPressed()
        {
            if (IsDetailVisible && PageOrientation == StackOrientation.Vertical)
            {
                IsDetailVisible = false;
                return true;
            }

            base.OnBackButtonPressed();
            return false;
        }

        #endregion
    }
}
