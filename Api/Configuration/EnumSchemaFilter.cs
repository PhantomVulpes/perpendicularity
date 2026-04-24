using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Runtime.Serialization;

namespace Vulpes.Perpendicularity.Api.Configuration;

public class EnumSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum.Clear();
            schema.Type = "string";
            schema.Format = null;

            foreach (var enumValue in Enum.GetValues(context.Type))
            {
                var memberInfo = context.Type.GetMember(enumValue.ToString()!).FirstOrDefault();
                var enumMemberAttribute = memberInfo?.GetCustomAttributes(typeof(EnumMemberAttribute), false)
                    .Cast<EnumMemberAttribute>()
                    .FirstOrDefault();

                var label = enumMemberAttribute?.Value ?? enumValue.ToString();
                schema.Enum.Add(new OpenApiString(label));
            }
        }
    }
}
