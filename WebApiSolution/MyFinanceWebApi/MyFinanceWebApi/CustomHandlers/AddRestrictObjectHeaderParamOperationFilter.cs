using Swashbuckle.Swagger;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Description;

namespace MyFinanceWebApi.CustomHandlers
{
	public class AddRestrictObjectHeaderParamOperationFilter : IOperationFilter
	{
        public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
        {
            if (!HasRestrictAttribute(apiDescription))
            {
                return;
            }

            if (operation == null)
            {
                return;
            }

            if (operation.parameters == null)
            {
                operation.parameters = new List<Parameter>();
            }

            var parameter = new Parameter
            {
                description = "Restrict response object",
                @in = "header",
                name = "$restrict",
                required = false,
                type = "string"
            };

            operation.parameters.Add(parameter);
        }

        private static bool HasRestrictAttribute(ApiDescription apiDescription)
        {
            var attributes = apiDescription.ActionDescriptor.GetCustomAttributes<IncludeRestrictObjectHeaderAttribute>();
            return attributes.Any();
        }
    }
}