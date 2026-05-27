using System.Net.Http.Json;
using DrDocumentSync.FunctionApp.Models;
using DrDocumentSync.FunctionApp.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DrDocumentSync.FunctionApp.Services;

public sealed class SitecoreApiClient(HttpClient httpClient, IOptions<SitecoreOptions> options, ILogger<SitecoreApiClient> logger) : ISitecoreApiClient
{
    public async IAsyncEnumerable<DocumentAsset> GetDocumentsAsync(DateTimeOffset? modifiedSinceUtc, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var page = 0;
        while (true)
        {
            var url = $"{options.Value.AssetEndpoint}?page={page}&size={options.Value.PageSize}";
            if (modifiedSinceUtc.HasValue) url += $"&modifiedSince={Uri.EscapeDataString(modifiedSinceUtc.Value.UtcDateTime.ToString("O"))}";

            var response = await httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<SitecorePageResult>(cancellationToken: cancellationToken);

            if (result?.Items is null || result.Items.Count == 0) yield break;
            foreach (var item in result.Items)
            {
                yield return item;
            }

            if (!result.HasMore) yield break;
            page++;
        }
    }

    public Task<Stream> DownloadFileAsync(Uri downloadUri, CancellationToken cancellationToken)
    {
        logger.LogDebug("Downloading {Uri}", downloadUri);
        return httpClient.GetStreamAsync(downloadUri, cancellationToken);
    }

    private sealed class SitecorePageResult
    {
        public List<DocumentAsset> Items { get; set; } = [];
        public bool HasMore { get; set; }
    }
}
