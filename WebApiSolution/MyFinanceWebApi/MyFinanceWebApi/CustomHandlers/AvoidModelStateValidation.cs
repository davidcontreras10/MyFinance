using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;

namespace MyFinanceWebApi.CustomHandlers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AvoidModelStateValidationAttribute : Attribute
    {
    }
}