using System.Collections.Generic;
using MyFinanceModel.ClientViewModel;
using MyFinanceModel.ViewModel;

namespace MyFinanceWebApp.Models
{
    public class BankAccountSummaryViewModel
    {
        public IEnumerable<BankAccountSummary> Summary { get; set; }
        public IEnumerable<int> AccountIds { get; set; }
    }

    public class AccountGroupListViewModel
	{
		public IEnumerable<AccountGroupViewModel> AccountGroupViewModels { get; set; }
	}

    public class AccountGroupDetailViewModel
    {
        public IEnumerable<AccountGroupPosition> AccountGroupPositions { get; set; }
        public MyFinanceModel.ViewModel.AccountGroupDetailViewModel Model { get; set; }
    }

	public class SpendTypesAllUser
	{
		public IEnumerable<SpendTypeViewModel> AllSpendTypes { get; set; }
		public IEnumerable<SpendTypeViewModel> UserSpendTypes { get; set; }
	}

    public class CustomClientNewPassword : ClientNewPasswordRequest
    {
        public string PageTitle { get; set; } = "Reset Password";
        public string PageMessage { get; set; } = "Enter your new password";
        public bool Success { get; set; }
        public bool ReadyToUpdate { get; set; } = true;
    }

    public class ForgottenPasswordModel : ClientResetPasswordEmailRequest
    {
        public string PageTitle { get; set; } = "Forgotten Password";
        public string PageMessage { get; set; } = "Please enter your information";
        public bool Success { get; set; }
        public bool ReadyToUpdate { get; set; } = true;
    }

    public class BasicSelectableItem : IDropDownSelectable
    {
	    public int Id { get; set; }
	    public string Name { get; set; }
	    public bool IsSelected { get; set; }
    }
}