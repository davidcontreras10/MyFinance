using System;
using System.ComponentModel.DataAnnotations;

namespace MyFinance.MyFinanceWebApp.CustomHandlers
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DateRangeAttribute : RangeAttribute
    {
        public DateRangeAttribute(string minimum, string maximun = null)
            : base(typeof(DateTime), minimum, string.IsNullOrEmpty(maximun) ? 
                  DateTime.Now.ToShortDateString() : maximun) { }
    }
}