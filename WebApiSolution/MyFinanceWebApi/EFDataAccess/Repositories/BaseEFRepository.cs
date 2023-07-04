using EFDataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
