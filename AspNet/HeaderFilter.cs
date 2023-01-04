using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace LogsAndAuth
{
    public class HeaderFilter : IOperationFilter
    {

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "Token",
                In = ParameterLocation.Header,
                Required = true // set to false if this is optional
            });
            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "Tool",
                In = ParameterLocation.Header,
                Required = true // set to false if this is optional
            });
            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "Table",
                In = ParameterLocation.Header,
                Required = true // set to false if this is optional
            });
            operation.Parameters.Add(new OpenApiParameter()
            {
                Name = "Method",
                In = ParameterLocation.Header,
                Required = true // set to false if this is optional
            });
        }
    }
}
