using MyFinanceModel.Utilities;
using MyFinanceModel.ViewModel;

namespace MyFinanceWebApp.Helpers
{
    public static class ModelHtmlExtension
    {
        public static string GetHtmlAmount(string currencySymbol, float amount)
        {
            var result = $"{currencySymbol}{amount}";
            return result;
        }
    }

    public static class SpendHtmlExtension
    {
        public static string GetHtmlString(this SpendViewModel spendViewModel, string currencySymbol)
        {
            //var spanTag = string.Format("<nobr><span class='{0}' id='{1}'>", GetAmountClass(), GetHtmlSpendId());
            //return spanTag + CurrencySymbol + NumUtils.GetCurrencyFormatted(OriginalAmount) +
            //       "</span></nobr>";
            return spendViewModel.GetAmountHtmlString(spendViewModel.OriginalAmount, currencySymbol);
        }

        public static string GetHtmlStringConverted(this SpendViewModel spendViewModel, string currencySymbol)
        {
            //var spanTag = string.Format("<nobr><span class='{0}' id='{1}'>", GetAmountClass(), GetHtmlSpendId());
            //return spanTag + currencySymbol + NumUtils.GetCurrencyFormatted(GetConvertedAmount()) +
            //       "</span></nobr>";
            return spendViewModel.GetAmountHtmlString(spendViewModel.GetConvertedAmount(), currencySymbol);
        }

        public static string GetAmountClass(this SpendViewModel spendViewModel)
        {
            switch (spendViewModel.AmountTypeId)
            {
                case 1:
                    return "spend-amount";
                case 2:
                    return "saving-amount";
                default:
                    return "";
            }
        }

        private static string GetHtmlSpendId(this SpendViewModel spendViewModel)
        {
            return "spend-item-" + spendViewModel.SpendId + "-" + spendViewModel.AccountId;
        }

        private static string GetAmountHtmlString(this SpendViewModel spendViewModel, float amount, string currencySymbol)
        {
            var spanTag = string.Format("<nobr><span onclick='spendAmountClick({3},{2})' class='{0}' id='{1}'>",
                spendViewModel.GetAmountClass(), spendViewModel.GetHtmlSpendId(), spendViewModel.AccountId, spendViewModel.SpendId);
            return spanTag + currencySymbol + NumUtils.GetCurrencyFormatted(amount) +
                   "</span></nobr>";
        }
    }

    public static class LoanHtmlExtension
    {
        public static string GetId(this LoanReportViewModel loanReportViewModel, string objectValue = "")
        {
            var id = $"{objectValue}-{loanReportViewModel.LoanRecordId}";
            return id;
        }
    }
}