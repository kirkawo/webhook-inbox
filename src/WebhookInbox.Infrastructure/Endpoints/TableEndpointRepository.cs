using Azure;
using Azure.Data.Tables;
using WebhookInbox.Application.Endpoints;
using WebhookInbox.Domain.Endpoints;

namespace WebhookInbox.Infrastructure.Endpoints;

public sealed class TableEndpointRepository : IEndpointRepository
{
    public const string PartitionKey = "default";

    private readonly TableClient _table;
    private bool _tableCreated;

    public TableEndpointRepository(TableClient table)
    {
        _table = table;
    }

    public async Task AddAsync(Endpoint endpoint, CancellationToken cancellationToken = default)
    {
        await EnsureTableAsync(cancellationToken);
        await _table.AddEntityAsync(ToEntity(endpoint), cancellationToken);
    }

    public async Task<Endpoint?> GetAsync(string endpointId, CancellationToken cancellationToken = default)
    {
        await EnsureTableAsync(cancellationToken);
        try
        {
            var entity = await _table.GetEntityAsync<TableEntity>(PartitionKey, endpointId, cancellationToken: cancellationToken);
            return FromEntity(entity);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task<IReadOnlyList<Endpoint>> ListAsync(CancellationToken cancellationToken = default)
    {
        await EnsureTableAsync(cancellationToken);
        var endpoints = new List<Endpoint>();
        await foreach (var entity in _table.QueryAsync<TableEntity>(
            filter: $"PartitionKey eq '{PartitionKey}'",
            cancellationToken: cancellationToken))
        {
            endpoints.Add(FromEntity(entity));
        }

        return endpoints.OrderByDescending(e => e.CreatedAtUtc).ToList();
    }

    public async Task UpdateAsync(Endpoint endpoint, CancellationToken cancellationToken = default)
    {
        await EnsureTableAsync(cancellationToken);
        await _table.UpdateEntityAsync(ToEntity(endpoint), ETag.All, TableUpdateMode.Replace, cancellationToken);
    }

    private async Task EnsureTableAsync(CancellationToken cancellationToken)
    {
        if (_tableCreated)
        {
            return;
        }

        await _table.CreateIfNotExistsAsync(cancellationToken);
        _tableCreated = true;
    }

    private static TableEntity ToEntity(Endpoint endpoint)
    {
        return new TableEntity(PartitionKey, endpoint.EndpointId)
        {
            ["Name"] = endpoint.Name,
            ["PathToken"] = endpoint.PathToken,
            ["IsActive"] = endpoint.IsActive,
            ["CreatedAtUtc"] = endpoint.CreatedAtUtc,
            ["ExpiresAtUtc"] = endpoint.ExpiresAtUtc,
            ["LastReceivedAtUtc"] = endpoint.LastReceivedAtUtc,
            ["EventCount"] = endpoint.EventCount,
        };
    }

    private static Endpoint FromEntity(TableEntity entity)
    {
        return Endpoint.FromStorage(
            entity.RowKey ?? string.Empty,
            entity.GetString("Name") ?? string.Empty,
            entity.GetString("PathToken") ?? string.Empty,
            entity.GetBoolean("IsActive") ?? false,
            entity.GetDateTimeOffset("CreatedAtUtc") ?? DateTimeOffset.MinValue,
            entity.GetDateTimeOffset("ExpiresAtUtc"),
            entity.GetDateTimeOffset("LastReceivedAtUtc"),
            entity.GetInt64("EventCount") ?? 0);
    }
}
