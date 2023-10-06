using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GarageGroup.Platform.Swagger.Hub;

partial class ApimIdentityHandler
{
    private const string AzureClientIdKey = "AZURE_CLIENT_ID";

    public static HttpMessageHandler Resolve(IServiceProvider serviceProvider, HttpMessageHandler innerHandler)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(innerHandler);

        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        return new ApimIdentityHandler(innerHandler, configuration[AzureClientIdKey]);
    }
}