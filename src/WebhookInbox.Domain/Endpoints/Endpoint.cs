namespace WebhookInbox.Domain.Endpoints;

public sealed class Endpoint
{
    public string EndpointId { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string PathToken { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTimeOffset CreatedAtUtc { get; private set; }
    public DateTimeOffset? ExpiresAtUtc { get; private set; }
    public DateTimeOffset? LastReceivedAtUtc { get; private set; }
    public long EventCount { get; private set; }

    private Endpoint()
    {
    }

    public static Endpoint Create(string name, DateTimeOffset now, TimeSpan? lifetime = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (lifetime is { } t && t <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(lifetime), "Lifetime must be positive.");
        }

        return new Endpoint
        {
            EndpointId = Guid.NewGuid().ToString("N"),
            Name = name.Trim(),
            PathToken = Guid.NewGuid().ToString("N"),
            IsActive = true,
            CreatedAtUtc = now,
            ExpiresAtUtc = lifetime is null ? null : now + lifetime.Value,
        };
    }

    public static Endpoint FromStorage(
        string endpointId,
        string name,
        string pathToken,
        bool isActive,
        DateTimeOffset createdAtUtc,
        DateTimeOffset? expiresAtUtc,
        DateTimeOffset? lastReceivedAtUtc,
        long eventCount)
    {
        return new Endpoint
        {
            EndpointId = endpointId,
            Name = name,
            PathToken = pathToken,
            IsActive = isActive,
            CreatedAtUtc = createdAtUtc,
            ExpiresAtUtc = expiresAtUtc,
            LastReceivedAtUtc = lastReceivedAtUtc,
            EventCount = eventCount,
        };
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void SetExpiration(DateTimeOffset expiresAtUtc)
    {
        ExpiresAtUtc = expiresAtUtc;
    }

    public bool IsUsableAt(DateTimeOffset now)
    {
        return IsActive && (ExpiresAtUtc is null || ExpiresAtUtc > now);
    }
}
