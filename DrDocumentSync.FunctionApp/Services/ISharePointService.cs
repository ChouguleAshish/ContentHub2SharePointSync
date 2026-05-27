using DrDocumentSync.FunctionApp.Models;

namespace DrDocumentSync.FunctionApp.Services;

public interface ISharePointService
{
    Task<bool> NeedsUploadAsync(SharePointDocument document, CancellationToken cancellationToken);
    Task UpsertDocumentAsync(SharePointDocument metadata, Stream content, CancellationToken cancellationToken);
}
