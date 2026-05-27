using DrDocumentSync.FunctionApp.Models;
using DrDocumentSync.FunctionApp.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace DrDocumentSync.FunctionApp.Services;

public sealed class SharePointService(GraphServiceClient graph, IOptions<SharePointOptions> options, ILogger<SharePointService> logger) : ISharePointService
{
    public async Task<bool> NeedsUploadAsync(SharePointDocument document, CancellationToken cancellationToken)
    {
        try
        {
            var item = await graph.Drives[options.Value.DriveId].Root
                .ItemWithPath($"{options.Value.RootFolder}/{document.FileName}")
                .GetAsync(cancellationToken: cancellationToken);

            var lastWrite = item?.FileSystemInfo?.LastModifiedDateTime;
            var existingHash = item?.AdditionalData?.TryGetValue("fileHash", out var hashObj) == true ? hashObj?.ToString() : null;

            return lastWrite is null || lastWrite < document.LastModifiedUtc || (!string.IsNullOrWhiteSpace(document.FileHash) && !string.Equals(existingHash, document.FileHash, StringComparison.OrdinalIgnoreCase));
        }
        catch (ODataError)
        {
            return true;
        }
    }

    public async Task UpsertDocumentAsync(SharePointDocument metadata, Stream content, CancellationToken cancellationToken)
    {
        _ = await graph.Drives[options.Value.DriveId].Root
            .ItemWithPath($"{options.Value.RootFolder}/{metadata.FileName}")
            .Content.PutAsync(content, cancellationToken: cancellationToken);

        var fields = new FieldValueSet
        {
            AdditionalData = new Dictionary<string, object>
            {
                ["Title"] = metadata.Title,
                ["Description"] = metadata.Description,
                ["Product"] = metadata.Product,
                ["ProductFamily"] = metadata.ProductFamily,
                ["ProductCategory"] = metadata.ProductCategory
            }
        };

        logger.LogInformation("Uploaded/updated {FileName}", metadata.FileName);
    }
}
