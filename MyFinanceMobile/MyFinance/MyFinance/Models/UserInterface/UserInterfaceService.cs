using System;
using System.Collections.Generic;
using System.Text;

namespace MyFinance.Models.UserInterface
{
	public class UserInterfaceService
	{
		public static IEnumerable<MbMenuItem> GetMainMenuItems()
		{
			var mainSubMenus = new[]
			{
				new MbMenuItem {Name = "Show Pending"},
				new MbMenuItem {Name = "Option 2"},
				new MbMenuItem {Name = "Option 2"}
			};

			var result = new[]
			{
				new MbMenuItem {Name = "Main", SubMenus = mainSubMenus, IsActive = true},
				new MbMenuItem {Name = "Account"},
				new MbMenuItem {Name = "Spending Types"}
			};

			return result;
		}
	}
}
