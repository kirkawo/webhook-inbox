using WebhookInbox.Domain.Endpoints;

namespace WebhookInbox.Application.Endpoints;

public interface IEndpointRepository
{
    Task AddAsync(Endpoint endpoint, CancellationToken cancellationToken = default);
    Task<Endpoint?> GetAsync(string endpointId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Endpoint>> ListAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(Endpoint endpoint, CancellationToken cancellationToken = default);
}
