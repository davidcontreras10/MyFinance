using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyFinance.MyFinanceWebApp.Models;

namespace MyFinance.MyFinanceWebApp.Helpers
{
	public static class MainHeaderModelExtension
	{
		public static string ListClass(this MenuItem menuItem)
		{
			var classes = "nav-item";
			if (menuItem.IsActive)
			{
				classes += " active";
			}

			if (menuItem.MenuType == MenuItemType.DropDown)
			{
				classes += " dropdown";
			}

			return classes;
		}
	}
}
