using MyFinanceModel;
using System;
using System.Net;

namespace MyFinanceWebApiCore.Models
{
	[Serializable]
	internal class UnauthorizeAccessException : ServiceException
	{
		public UnauthorizeAccessException(string message = "User is not allowed to perform this action") : base(message,
			1, HttpStatusCode.Unauthorized)
		{
		}
	}
}
