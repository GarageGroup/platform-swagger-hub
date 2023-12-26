using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GarageGroup.Platform.Swagger.Hub;

partial class Function
{
    [Function("GetData")]
    public static async Task<HttpResponseData> GetDataAsync(
        [HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "data")] HttpRequestData request)
    {
        var data = new
        {
            Environment = Environment.GetEnvironmentVariable("ConnectionStrings:AppConfig", EnvironmentVariableTarget.Process),
            Configuration = request.FunctionContext.InstanceServices.GetRequiredService<IConfiguration>().GetConnectionString("AppConfig")
        };

        var response = request.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(data);

        return response;
    }
}