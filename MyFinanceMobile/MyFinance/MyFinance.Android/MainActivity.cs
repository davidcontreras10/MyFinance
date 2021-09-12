using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace MyFinance.Droid
{
    [Activity(Label = "MyFinance", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
	        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }

	    private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
	    {
		    if (e.ExceptionObject is Exception exception)
		    {
			    var stackTrace = exception.ToString();
		    }
	    }
	}
}

