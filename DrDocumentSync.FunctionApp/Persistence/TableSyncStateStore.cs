using Azure;
using Azure.Data.Tables;
using Azure.Identity;
using DrDocumentSync.FunctionApp.Options;
using Microsoft.Extensions.Options;

namespace DrDocumentSync.FunctionApp.Persistence;

public sealed class TableSyncStateStore : ISyncStateStore
{
    private readonly TableClient _table;
    private readonly StateStoreOptions _options;

    public TableSyncStateStore(IOptions<StateStoreOptions> options)
    {
        _options = options.Value;
        _table = new TableClient(new Uri(_options.StorageAccountUrl), _options.TableName, new DefaultAzureCredential());
        _table.CreateIfNotExists();
    }

    public async Task<DateTimeOffset?> GetLastRunUtcAsync(CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _table.GetEntityAsync<TableEntity>(_options.PartitionKey, _options.RowKey, cancellationToken: cancellationToken);
            return entity.Value.GetDateTimeOffset("LastRunUtc");
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public Task SetLastRunUtcAsync(DateTimeOffset value, CancellationToken cancellationToken)
    {
        var entity = new TableEntity(_options.PartitionKey, _options.RowKey)
        {
            ["LastRunUtc"] = value
        };
        return _table.UpsertEntityAsync(entity, TableUpdateMode.Replace, cancellationToken);
    }
}
