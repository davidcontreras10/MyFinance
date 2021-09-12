using System.Web.Mvc;
using MyFinanceWebApp.Models;

namespace MyFinanceWebApp.Helpers
{
    public static class PageHeaderBuilder
    {
        public enum AppMenuItem
        {
            Unknown = 0,
            Home = 1,
            Account = 2,
            SpendType = 3,
            MyAccount = 4,
            Loan = 5
        }

        public static MainHeaderModel GetHeader(UrlHelper urlHelper, AppMenuItem activeMenuItem)
        {
            var result = new MainHeaderModel
            {
                PageTitle = "My Finance",
                LeftMenuItems = new[]
                {
                    GetHomeMenuItem(activeMenuItem == AppMenuItem.Home, urlHelper),
                    GetAccountMenuItem(activeMenuItem == AppMenuItem.Account, urlHelper),
                    GetSpendTypeMenuItem(activeMenuItem == AppMenuItem.SpendType,urlHelper),
                    GetLoanMenuItem(activeMenuItem == AppMenuItem.Loan,urlHelper)
                },

                RightMenuItems = new []
                {
                    GetMyAccountMenuItem(activeMenuItem == AppMenuItem.MyAccount, urlHelper)
                }
            };

            return result;
        }

        private static MenuItem GetHomeMenuItem(bool active, UrlHelper urlHelper)
        {
            return active
                ? (MenuItem)new DropDownMenuItem
                {
                    Name = "Home",
                    IsActive = true,
                    Items = new MenuItem[]
                    {
                        new JsActionMenuItem{ Name = "Hide pending data", JavascriptCode = "updatePendingDataStatus()", Id = "show-pending-data-button"},
                        new JsActionMenuItem{ Name = "Toggle Summary", JavascriptCode = "showHideSummaryFinanceTable()", Id = "show-hide-summary-button"}
                    }
                }
                : new CustomLinkMenuItem { Name = "Home", Link = urlHelper.Action("Index", "Home") };
        }

        private static MenuItem GetAccountMenuItem(bool active, UrlHelper urlHelper)
        {
            return active
                ? (MenuItem) new DropDownMenuItem
                {
                    Name = "Accounts",
                    IsActive = true,
                    Items = new MenuItem[]
                    {
                        new JsActionMenuItem {Name = "New...", JavascriptCode = "loadAddAccountViewModel()"},
                        new JsActionMenuItem
                        {
                            Name = "Save positions",
                            Id = "account-save-positions-link",
                            JavascriptCode = "submitAccountPositions()"
                        },
                        new JsActionMenuItem
                        {
                            Name = "Restore default positions",
                            Id = "account-restore-positions-link",
                            JavascriptCode = "restoreAccountPositions()"
                        },
                        new JsActionMenuItem
                        {
                            Name = "Manage Account Groups...",
                            Id = "account-group-link",
                            JavascriptCode = "loadAccountGroupList()"
                        }
                    }
                }
                : new CustomLinkMenuItem {Name = "Account", Link = urlHelper.Action("Index", "Account") };
        }

        private static MenuItem GetSpendTypeMenuItem(bool active, UrlHelper urlHelper)
        {
            return active
                ? (MenuItem) new SimpleMenuItem {Name = "Spending Types", IsActive = true}
                : new CustomLinkMenuItem {Name = "Spending Type", Link = urlHelper.Action("Index", "SpendType")};
        }

        private static MenuItem GetLoanMenuItem(bool active, UrlHelper urlHelper)
        {
            return active
                ? (MenuItem)new DropDownMenuItem
                {
                    Name = "Loan",
                    IsActive = true,
                    Items = new MenuItem[]
                    {
                        new JsActionMenuItem{ Name = "New Loan", JavascriptCode = "showLoanDetailModal()", Id = "show-loan-detail-modal"}
                    }
                }
                : new CustomLinkMenuItem { Name = "Loans", Link = urlHelper.Action("Index", "Loan") };
        }

        private static MenuItem GetMyAccountMenuItem(bool active, UrlHelper urlHelper)
        {
            return new DropDownMenuItem
            {
                Name = "My Account",
                IsActive = active,
                Items = new MenuItem[]
                {
                    //new CustomLinkMenuItem {Name = "My Profile", Link = urlHelper.Action("MeUser", "User")},
                    new CustomLinkMenuItem {Name = "Log off", Link = urlHelper.Action("LogOff", "User")}
                }
            };
        }
    }
}