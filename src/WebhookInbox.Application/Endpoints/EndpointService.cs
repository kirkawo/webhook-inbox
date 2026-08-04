using WebhookInbox.Domain.Endpoints;

namespace WebhookInbox.Application.Endpoints;

public sealed class EndpointService
{
    private readonly IEndpointRepository _repository;

    public EndpointService(IEndpointRepository repository)
    {
        _repository = repository;
    }

    public async Task<EndpointDto> CreateAsync(CreateEndpointRequest request, CancellationToken cancellationToken = default)
    {
        var endpoint = Endpoint.Create(request.Name, DateTimeOffset.UtcNow, request.Lifetime);
        await _repository.AddAsync(endpoint, cancellationToken);
        return ToDto(endpoint);
    }

    public async Task<IReadOnlyList<EndpointDto>> ListAsync(CancellationToken cancellationToken = default)
    {
        var endpoints = await _repository.ListAsync(cancellationToken);
        return endpoints.Select(ToDto).ToList();
    }

    public async Task<EndpointDto?> GetAsync(string endpointId, CancellationToken cancellationToken = default)
    {
        var endpoint = await _repository.GetAsync(endpointId, cancellationToken);
        return endpoint is null ? null : ToDto(endpoint);
    }

    public async Task<bool> DeactivateAsync(string endpointId, CancellationToken cancellationToken = default)
    {
        var endpoint = await _repository.GetAsync(endpointId, cancellationToken);
        if (endpoint is null)
        {
            return false;
        }

        endpoint.Deactivate();
        await _repository.UpdateAsync(endpoint, cancellationToken);
        return true;
    }

    public async Task<bool> ExpireAsync(string endpointId, CancellationToken cancellationToken = default)
    {
        var endpoint = await _repository.GetAsync(endpointId, cancellationToken);
        if (endpoint is null)
        {
            return false;
        }

        endpoint.SetExpiration(DateTimeOffset.UtcNow);
        await _repository.UpdateAsync(endpoint, cancellationToken);
        return true;
    }

    private static EndpointDto ToDto(Endpoint endpoint)
    {
        return new EndpointDto(
            endpoint.EndpointId,
            endpoint.Name,
            endpoint.PathToken,
            endpoint.IsActive,
            endpoint.CreatedAtUtc,
            endpoint.ExpiresAtUtc,
            endpoint.LastReceivedAtUtc,
            endpoint.EventCount);
    }
}
