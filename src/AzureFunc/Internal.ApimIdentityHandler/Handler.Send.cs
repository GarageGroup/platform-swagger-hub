using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.Models;

namespace GarageGroup.Platform.Swagger.Hub;

partial class ApimIdentityHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri is null || IsApimRequestUri(request.RequestUri) is false)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var context = CreateRequestContext(request.RequestUri);
        var token = await GetTokenCredential().GetTokenAsync(context, cancellationToken);

        request.Headers.Authorization = new(AuthorizationScheme, token.Token);
        var response = await base.SendAsync(request, cancellationToken);

        if (response.IsSuccessStatusCode is false)
        {
            return response;
        }

        try
        {
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            var sourceDocument = JsonSerializer.Deserialize<ApimResponseJson>(responseContent, SerializerOptions);
            var document = ProcessSwaggerDocument(sourceDocument.Value.OrEmpty());

            return new(response.StatusCode)
            {
                Content = new StringContent(document)
            };
        }
        finally
        {
            response.Dispose();
        }
    }

    private static string ProcessSwaggerDocument(string sourceDocument)
    {
        var document = OpenApiReader.Read(sourceDocument, out var _);

        document.SecurityRequirements = GetSecurityRequirements(document).ToList();
        document.Components.SecuritySchemes = GetSecuritySchemes(document);
        document.Servers.Clear();

        return document.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0);
    }

    private static IEnumerable<OpenApiSecurityRequirement> GetSecurityRequirements(OpenApiDocument document)
    {
        foreach (var securityRequirement in document.SecurityRequirements)
        {
            var requirement = new OpenApiSecurityRequirement();

            foreach (var security in securityRequirement)
            {
                if (security.Key.In is ParameterLocation.Query && security.Key.Type is SecuritySchemeType.ApiKey)
                {
                    continue;
                }

                requirement.Add(security.Key, security.Value);
            }

            if (requirement.Any())
            {
                yield return requirement;
            }
        }
    }

    private static IDictionary<string, OpenApiSecurityScheme> GetSecuritySchemes(OpenApiDocument document)
    {
        var schemes = new Dictionary<string, OpenApiSecurityScheme>(document.Components.SecuritySchemes.Count);

        foreach (var scheme in document.Components.SecuritySchemes)
        {
            if (scheme.Value.In is ParameterLocation.Query && scheme.Value.Type is SecuritySchemeType.ApiKey)
            {
                continue;
            }

            schemes.Add(scheme.Key, scheme.Value);
        }

        return schemes;
    }
}