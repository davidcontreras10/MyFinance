using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyFinanceWebApp.Models
{
	public class UserSelectAccount
	{
		public int AccountId { get; set; }
		public int AccountPeriodId { get; set; }
		public string AccountName { get; set; }
	}
}