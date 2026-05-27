using DrDocumentSync.FunctionApp.Models;

namespace DrDocumentSync.FunctionApp.Services;

public interface ISitecoreApiClient
{
    IAsyncEnumerable<DocumentAsset> GetDocumentsAsync(DateTimeOffset? modifiedSinceUtc, CancellationToken cancellationToken);
    Task<Stream> DownloadFileAsync(Uri downloadUri, CancellationToken cancellationToken);
}
