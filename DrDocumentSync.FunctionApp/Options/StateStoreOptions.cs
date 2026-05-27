namespace DrDocumentSync.FunctionApp.Options;

public sealed class StateStoreOptions
{
    public string StorageAccountUrl { get; set; } = string.Empty;
    public string TableName { get; set; } = "DrSyncState";
    public string PartitionKey { get; set; } = "DrDocumentSync";
    public string RowKey { get; set; } = "LastRun";
}
