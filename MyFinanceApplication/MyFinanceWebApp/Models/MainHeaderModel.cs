using System;
using System.Collections.Generic;
using System.Linq;

namespace MyFinanceWebApp.Models
{
    public interface IHtmlHeaderHelper
    {
        string CreateMenuList(IEnumerable<MenuItem> menuItems);
    }

    public class BootstrapHtmlHeaderHelper : IHtmlHeaderHelper
    {
		public string CreateMenuList(IEnumerable<MenuItem> menuItems)
        {
            if (menuItems == null || !menuItems.Any())
            {
                return "";
            }

            var result = menuItems.Aggregate("", (current, menuItem) => current + CreateMenuItem(menuItem));
            return result;
        }

        public string CreateMenuItem(MenuItem menuItem)
        {
            if (menuItem == null)
            {
                return "";
            }

            if (menuItem.MenuType == MenuItemType.Undefined)
            {
                throw new ArgumentException("Invalid menu type");
            }

            if (menuItem.MenuType == MenuItemType.Simple)
            {
	            return
	                $"<li{(menuItem.IsActive ? " class='active'" : "")}><a{menuItem.GetId()} href='#'>{menuItem.Name}</a></li>";
            }

            if (menuItem.MenuType == MenuItemType.DropDown)
            {
                var result = $"<li class='dropdown{(menuItem.IsActive ? " active" : "")}'>";
	            result += "<a href='#' class='dropdown-toggle' data-toggle='dropdown' role='button' " +
	                      $"aria-haspopup='true' aria-expanded='false'>{menuItem.Name}<span class='caret'></span></a>";
                result += "<ul class='dropdown-menu'>";
                var dropDownMenuItem = (DropDownMenuItem) menuItem;
                result += CreateMenuList(dropDownMenuItem.Items);
                result += "</li></a></ul>";
                return result;
            }

	        if (menuItem.MenuType == MenuItemType.CustomLinkAction)
	        {
				return
				    $"<li{(menuItem.IsActive ? " class='active'" : "")}><a{menuItem.GetId()} href='{((CustomLinkMenuItem) menuItem).Link}'>{menuItem.Name}</a></li>";
	        }

			if (menuItem.MenuType == MenuItemType.JsAction)
			{
				return
				    $"<li{(menuItem.IsActive ? " class='active'" : "")}><a{menuItem.GetId()} onclick='{((JsActionMenuItem) menuItem).JavascriptCode}' href='#'>{menuItem.Name}</a></li>";
			}

	        throw new ArgumentException("Invalid menu type");
        }
    }

    public class MainHeaderModel
    {
        public string PageTitle { get; set; }
        public IEnumerable<MenuItem> LeftMenuItems { get; set; }
        public IEnumerable<MenuItem> RightMenuItems { get; set; }
    }

    public abstract class MenuItem
    {
	    public string Id { get; set; }
        public string Name { get; set; }
        public abstract MenuItemType MenuType { get; }
        public bool IsActive { get; set; }

	    public string GetId()
	    {
		    return string.IsNullOrEmpty(Id) ? "" : $" id='{Id}'";
	    }
    }

    public class SimpleMenuItem : MenuItem
    {
        public override MenuItemType MenuType => MenuItemType.Simple;
    }

    public class DropDownMenuItem : MenuItem
    {
        public override MenuItemType MenuType => MenuItemType.DropDown;

        public IEnumerable<MenuItem> Items { get; set; } 
    }

	public class CustomLinkMenuItem : MenuItem
	{
		public override MenuItemType MenuType => MenuItemType.CustomLinkAction;

        public string Link { get; set; }
	}

	public class ActionLinkMenuItem : MenuItem
	{
		public override MenuItemType MenuType => MenuItemType.ActionLink;

        public string Action { get; set; }
		public string Controller { get; set; }
	}

	public class JsActionMenuItem : MenuItem
	{
		public override MenuItemType MenuType => MenuItemType.JsAction;

        public string JavascriptCode { get; set; }
	}

	public enum MenuItemType
	{
		Undefined = 0,
		Simple = 1,
		DropDown = 2,
		CustomLinkAction = 3,
		ActionLink = 4,
		JsAction = 5
	}
}