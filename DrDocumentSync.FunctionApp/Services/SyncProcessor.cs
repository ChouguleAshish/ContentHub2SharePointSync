using DrDocumentSync.FunctionApp.Models;
using DrDocumentSync.FunctionApp.Options;
using DrDocumentSync.FunctionApp.Persistence;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DrDocumentSync.FunctionApp.Services;

public sealed class SyncProcessor(
    ISitecoreApiClient sitecore,
    IMetadataFilter filter,
    IMetadataTransformer transformer,
    ISharePointService sharePoint,
    ISyncStateStore stateStore,
    IOptions<SyncOptions> options,
    ILogger<SyncProcessor> logger) : ISyncProcessor
{
    public async Task<SyncRunSummary> ExecuteAsync(CancellationToken cancellationToken)
    {
        var summary = new SyncRunSummary();
        var lastRun = await stateStore.GetLastRunUtcAsync(cancellationToken);

        var semaphore = new SemaphoreSlim(options.Value.DegreeOfParallelism);
        var tasks = new List<Task>();

        await foreach (var doc in sitecore.GetDocumentsAsync(lastRun, cancellationToken))
        {
            if (!filter.IsMatch(doc)) continue;
            await semaphore.WaitAsync(cancellationToken);
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    Interlocked.Increment(ref summary.TotalProcessed);
                    var mapped = transformer.Transform(doc);
                    if (!await sharePoint.NeedsUploadAsync(mapped, cancellationToken))
                    {
                        Interlocked.Increment(ref summary.Succeeded);
                        return;
                    }

                    await using var stream = await sitecore.DownloadFileAsync(doc.DownloadUri, cancellationToken);
                    await sharePoint.UpsertDocumentAsync(mapped, stream, cancellationToken);
                    Interlocked.Increment(ref summary.Succeeded);
                }
                catch (Exception ex)
                {
                    Interlocked.Increment(ref summary.Failed);
                    lock (summary.Errors) summary.Errors.Add($"{doc.Id}: {ex.Message}");
                    logger.LogError(ex, "Failed processing document {Id}", doc.Id);
                }
                finally
                {
                    semaphore.Release();
                }
            }, cancellationToken));
        }

        await Task.WhenAll(tasks);
        await stateStore.SetLastRunUtcAsync(DateTimeOffset.UtcNow, cancellationToken);
        logger.LogInformation("DR sync finished. Total={Total} Success={Success} Failed={Failed}", summary.TotalProcessed, summary.Succeeded, summary.Failed);
        return summary;
    }
}
