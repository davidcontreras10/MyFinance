using System;
using System.Diagnostics;
using Xamarin.Forms;

namespace MyFinance.Views
{
	public class MbNavigationPage : NavigationPage
	{
		public MbNavigationPage(Page root) : base(root)
		{
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			try
			{
				base.OnSizeAllocated(width, height);
				var orientation = Width > Height ? StackOrientation.Horizontal : StackOrientation.Vertical;
				var result = GlobalResources.Current.SetOrientation(orientation, width, height);
				if (result)
				{
					Debug.WriteLine($"Orientation changed Date: {DateTime.Now:T}. W: {width} - H: {height}");
				}
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
			}
		}
	}
}
