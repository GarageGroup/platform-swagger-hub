using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace GarageGroup.Platform.Swagger.Hub;

partial class Function
{
    [Function("GetSwaggerDocument")]
    public static Task<HttpResponseData> GetSwaggerDocumentAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "swagger/swagger.{format}")] HttpRequestData request,
        string? format,
        CancellationToken cancellationToken)
        =>
        PrimaryHandler.UseStandardSocketsHttpHandler()
        .UseLogging("HubSwaggerDocumentProvider")
        .Map(ApimIdentityHandler.Resolve)
        .UsePollyStandard()
        .UseHubSwaggerDocumentProvider(SwaggerSectionKey)
        .GetSwaggerDocumentAsync(request, format, cancellationToken);
}