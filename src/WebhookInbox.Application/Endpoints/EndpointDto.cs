namespace WebhookInbox.Application.Endpoints;

public sealed record EndpointDto(
    string EndpointId,
    string Name,
    string PathToken,
    bool IsActive,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? ExpiresAtUtc,
    DateTimeOffset? LastReceivedAtUtc,
    long EventCount);
