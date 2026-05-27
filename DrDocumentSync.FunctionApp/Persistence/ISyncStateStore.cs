namespace DrDocumentSync.FunctionApp.Persistence;

public interface ISyncStateStore
{
    Task<DateTimeOffset?> GetLastRunUtcAsync(CancellationToken cancellationToken);
    Task SetLastRunUtcAsync(DateTimeOffset value, CancellationToken cancellationToken);
}
