using MyFinanceModel;
using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;

namespace MyFinanceWebApi.CustomHandlers
{
	public class AddObjectHeaderParamOperationFilter : IOperationFilter
	{
		public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
		{
			var requiredHeader = GetServiceAppHeader(apiDescription);
			if(requiredHeader == null)
			{
				return;
			}

			if (operation.parameters == null)
			{
				operation.parameters = new List<Parameter>();
			}

			var parameter = new Parameter
			{
				description = requiredHeader.Description,
				@in = "header",
				name = requiredHeader.Name,
				required = requiredHeader.IsRequired,
				type = requiredHeader.ValueType
			};

			if(requiredHeader.Enums != null && requiredHeader.Enums.Any())
			{
				parameter.@enum = requiredHeader.Enums;
			}

			operation.parameters.Add(parameter);
		}

		private static ServiceAppHeader GetServiceAppHeader(ApiDescription apiDescription)
		{
			var attributes = apiDescription.ActionDescriptor.GetCustomAttributes<RequiresHeaderFilter>();
			if (!attributes.Any())
			{
				return null;
			}

			var attribute = attributes.First();
			var requiredHeader = attribute.RequiredServiceAppHeader;
			return requiredHeader;
		}

	}
}