using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace GarageGroup.Platform.Swagger.Hub;

static class Program
{
    static Task Main()
        =>
        Host.CreateDefaultBuilder()
        .ConfigureFunctionsWorkerStandard(
            useHostConfiguration: true)
        .Build()
        .RunAsync();
}