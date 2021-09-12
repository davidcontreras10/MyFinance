using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyFinanceWebApi.QueryHeaders
{
	public static class FilterQueryProcessor
	{
		private const string FULL_FILTER_QUERY_REGEX = @"^(?<init_path>[a-zA-Z0-9_]+(\.[a-zA-Z0-9_]+)*)(?<extension>=>)(?<predicate>[a-zA-Z0-9_]+(\.[a-zA-Z0-9_]+)*)(?<operator>==|=>|=<|<>|<|>)(?<value>.+)$";
	}
}