using System;
using System.Net.Http;
using Azure.Core;
using Microsoft.Extensions.DependencyInjection;

namespace GarageGroup.Platform.Swagger.Hub;

partial class ApimIdentityHandler
{
    public static HttpMessageHandler Resolve(IServiceProvider serviceProvider, HttpMessageHandler innerHandler)
    {
        ArgumentNullException.ThrowIfNull(serviceProvider);
        ArgumentNullException.ThrowIfNull(innerHandler);

        var tokenCredential = serviceProvider.GetRequiredService<TokenCredential>();
        return new ApimIdentityHandler(innerHandler, tokenCredential);
    }
}