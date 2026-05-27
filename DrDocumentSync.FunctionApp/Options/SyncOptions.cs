namespace DrDocumentSync.FunctionApp.Options;

public sealed class SyncOptions
{
    public string Schedule { get; set; } = "0 0 2 * * 0";
    public int BatchSize { get; set; } = 200;
    public int DegreeOfParallelism { get; set; } = 8;
    public bool UseFileHashForIdempotency { get; set; } = true;
}
