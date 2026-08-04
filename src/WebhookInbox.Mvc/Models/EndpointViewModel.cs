using WebhookInbox.Application.Endpoints;

namespace WebhookInbox.Mvc.Models;

public sealed class EndpointViewModel
{
    public required string EndpointId { get; init; }
    public required string Name { get; init; }
    public required string PathToken { get; init; }
    public required string WebhookUrl { get; init; }
    public bool IsActive { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset? ExpiresAtUtc { get; init; }
    public long EventCount { get; init; }
    public required string StatusText { get; init; }
    public required string StatusClass { get; init; }

    public static EndpointViewModel From(EndpointDto dto, DateTimeOffset now, string webhookUrl)
    {
        var isExpired = dto.ExpiresAtUtc is { } expiresAt && expiresAt <= now;
        var (statusText, statusClass) = !dto.IsActive ? ("[\u2013] inactive", "inactive")
            : isExpired ? ("[x] expired", "expired")
            : ("[+] active", "active");

        return new EndpointViewModel
        {
            EndpointId = dto.EndpointId,
            Name = dto.Name,
            PathToken = dto.PathToken,
            WebhookUrl = webhookUrl,
            IsActive = dto.IsActive,
            CreatedAtUtc = dto.CreatedAtUtc,
            ExpiresAtUtc = dto.ExpiresAtUtc,
            EventCount = dto.EventCount,
            StatusText = statusText,
            StatusClass = statusClass,
        };
    }
}
