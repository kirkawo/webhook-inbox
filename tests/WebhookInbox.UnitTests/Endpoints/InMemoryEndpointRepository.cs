using WebhookInbox.Application.Endpoints;
using WebhookInbox.Domain.Endpoints;

namespace WebhookInbox.UnitTests.Endpoints;

public sealed class InMemoryEndpointRepository : IEndpointRepository
{
    private readonly Dictionary<string, Endpoint> _endpoints = new();

    public Task AddAsync(Endpoint endpoint, CancellationToken cancellationToken = default)
    {
        _endpoints[endpoint.EndpointId] = endpoint;
        return Task.CompletedTask;
    }

    public Task<Endpoint?> GetAsync(string endpointId, CancellationToken cancellationToken = default)
    {
        _endpoints.TryGetValue(endpointId, out var endpoint);
        return Task.FromResult(endpoint);
    }

    public Task<IReadOnlyList<Endpoint>> ListAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Endpoint> result = _endpoints.Values.ToList();
        return Task.FromResult(result);
    }

    public Task UpdateAsync(Endpoint endpoint, CancellationToken cancellationToken = default)
    {
        _endpoints[endpoint.EndpointId] = endpoint;
        return Task.CompletedTask;
    }
}
