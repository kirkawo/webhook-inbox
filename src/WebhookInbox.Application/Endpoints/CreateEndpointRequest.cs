namespace WebhookInbox.Application.Endpoints;

public sealed record CreateEndpointRequest(string Name, TimeSpan? Lifetime = null);
