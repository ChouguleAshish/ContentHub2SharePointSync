using DrDocumentSync.FunctionApp.Models;

namespace DrDocumentSync.FunctionApp.Services;

public interface ISyncProcessor
{
    Task<SyncRunSummary> ExecuteAsync(CancellationToken cancellationToken);
}
