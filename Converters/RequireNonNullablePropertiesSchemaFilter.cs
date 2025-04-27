using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Auth.Converters;

internal sealed class RequireNonNullablePropertiesSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema model, SchemaFilterContext context)
    {
        foreach (var propKey in model.Properties
                     .Where(x => !x.Value.Nullable && !model.Required.Contains(x.Key))
                     .Select(x => x.Key))
        {
            model.Required.Add(propKey);
        }
    }
}