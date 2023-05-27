using EFDataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace EFDataAccess.Repositories
{
	public class BaseEFRepository
	{
		protected MyFinanceContext Context { get; }

		protected BaseEFRepository(MyFinanceContext context)
		{
			Context = context;
		}
	}
}
