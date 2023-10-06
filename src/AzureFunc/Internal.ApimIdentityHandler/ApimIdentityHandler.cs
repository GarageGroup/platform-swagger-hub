using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.Json;
using Azure.Core;
using Azure.Identity;
using Microsoft.OpenApi.Readers;
using Microsoft.OpenApi.Readers.Interface;

namespace GarageGroup.Platform.Swagger.Hub;

internal sealed partial class ApimIdentityHandler : DelegatingHandler
{
    private const string ApimUriStart = "https://management.azure.com/subscriptions/";

    private const string ScopeRelativeUri = "/.default";

    private const string AuthorizationScheme = "Bearer";

    private static readonly Lazy<AzureCliCredential> CliCredential;

    private static readonly ConcurrentDictionary<string, ManagedIdentityCredential> ManagedIdentityCredentials;

    private static readonly JsonSerializerOptions SerializerOptions;

    private static readonly IOpenApiReader<string, OpenApiDiagnostic> OpenApiReader;

    static ApimIdentityHandler()
    {
        CliCredential = new(CreateCliCredential);
        ManagedIdentityCredentials = new();
        SerializerOptions = new(JsonSerializerDefaults.Web);
        OpenApiReader = new OpenApiStringReader();
    }

    private static AzureCliCredential CreateCliCredential()
        =>
        new();

    private static ManagedIdentityCredential CreateManagedIdentityCredential(string clientId)
        =>
        new(clientId: clientId);

    private readonly string? azureClientId;

    private ApimIdentityHandler(HttpMessageHandler innerHandler, [AllowNull] string azureClientId) : base(innerHandler)
        =>
        this.azureClientId = azureClientId.OrNullIfEmpty();

    private TokenCredential GetTokenCredential()
    {
        if (string.IsNullOrEmpty(azureClientId))
        {
            return CliCredential.Value;
        }

        return ManagedIdentityCredentials.GetOrAdd(azureClientId, CreateManagedIdentityCredential);
    }

    private readonly record struct ApimResponseJson(string? Value);

    private static TokenRequestContext CreateRequestContext(Uri requestUri)
        =>
        new(
            scopes: new[]
            {
                new Uri(requestUri, ScopeRelativeUri).ToString()
            });

    private static bool IsApimRequestUri(Uri requestUri)
        =>
        requestUri.ToString().StartsWith(ApimUriStart, StringComparison.InvariantCultureIgnoreCase);
}