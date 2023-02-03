using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FSEDataFeedAPI
{
    // Reference: https://bartwullems.blogspot.com/2021/12/swagger-ui-add-required-header.html

    /// <summary>
    /// a filter that checks for the FSE-Access-Key header.
    /// </summary>
    public class AddHeaderParameter : IOperationFilter
    {
        /// <summary>
        /// Adds a requirement that the FSE-Access-Key header is set in the requests that need the key to query the FSEconomy Data Feed.
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            if (operation.OperationId.Contains("GetCommercialAssignments") ||
                operation.OperationId.Contains("GetBestCommercialAssignment") ||
                operation.OperationId.Contains("GetUSCommercialAssignments"))
            {
                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "fse-access-key",
                    In = ParameterLocation.Header,
                    Required = true,
                    Schema = new OpenApiSchema
                    {
                        Type = "string"
                    },
                    Description = "The user's FSE Access Key from the FSE Game World."
                });
            }
        }
    }
}
