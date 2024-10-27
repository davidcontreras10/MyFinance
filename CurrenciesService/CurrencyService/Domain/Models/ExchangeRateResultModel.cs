using System;
using System.Collections.Generic;

namespace Domain.Models
{
	public class ExchangeRateResultModel
	{
		public IEnumerable<MethodParam> MethodIds { get; set; }
		public DateTime DateTime { get; set; }

		public class MethodParam
		{
			public int Id { get; set; }
			public bool IsPurchase { get; set; }
			public DateTime? DateTime { get; set; }
		}
	}
}
