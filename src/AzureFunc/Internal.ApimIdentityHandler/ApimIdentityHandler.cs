using System;
using System.Net.Http;
using System.Text.Json;
using Azure.Core;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Readers.Interface;

namespace GarageGroup.Platform.Swagger.Hub;

internal sealed partial class ApimIdentityHandler : DelegatingHandler
{
    private const string ApimUriStart = "https://management.azure.com/subscriptions/";

    private const string ScopeRelativeUri = "/.default";

    private const string AuthorizationScheme = "Bearer";

    private static readonly JsonSerializerOptions SerializerOptions;

    private static readonly IOpenApiReader<string, OpenApiDiagnostic> OpenApiReader;

    static ApimIdentityHandler()
    {
        SerializerOptions = new(JsonSerializerDefaults.Web);
        OpenApiReader = new OpenApiStringReader();
    }

    private readonly TokenCredential tokenCredential;

    private ApimIdentityHandler(HttpMessageHandler innerHandler, TokenCredential tokenCredential) : base(innerHandler)
        =>
        this.tokenCredential = tokenCredential;

    private readonly record struct ApimResponseJson(string? Value);

    private static TokenRequestContext CreateRequestContext(Uri requestUri)
        =>
        new(
            scopes:
            [
                new Uri(requestUri, ScopeRelativeUri).ToString()
            ]);

    private static bool IsApimRequestUri(Uri requestUri)
        =>
        requestUri.ToString().StartsWith(ApimUriStart, StringComparison.InvariantCultureIgnoreCase);
}