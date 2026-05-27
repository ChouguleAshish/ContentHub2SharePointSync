using DrDocumentSync.FunctionApp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace DrDocumentSync.FunctionApp.Functions;

public sealed class DrDocumentSyncFunction(ISyncProcessor syncProcessor, ILogger<DrDocumentSyncFunction> logger)
{
    [Function("DrDocumentSync")]
    public async Task Run([TimerTrigger("%SyncOptions:Schedule%")]
        TimerInfo timerInfo, CancellationToken cancellationToken)
    {
        logger.LogInformation("DR document sync started at: {Timestamp}", DateTimeOffset.UtcNow);
        var result = await syncProcessor.ExecuteAsync(cancellationToken);
        logger.LogInformation("DR document sync completed: Total={Total} Success={Success} Fail={Fail}", result.TotalProcessed, result.Succeeded, result.Failed);
    }
}
