using GarageGroup.Infra;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace GarageGroup.Platform.Swagger.Hub;

partial class Function
{
    [Function("GetSwaggerUI")]
    public static HttpResponseData GetSwaggerUI([HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "swagger")] HttpRequestData request)
        =>
        request.GetSwaggerUI(SwaggerSectionKey, request.FunctionContext.GetRouteUrl("swagger/swagger.json"));
}