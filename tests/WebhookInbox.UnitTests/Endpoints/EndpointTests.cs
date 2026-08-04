using WebhookInbox.Domain.Endpoints;

namespace WebhookInbox.UnitTests.Endpoints;

public class EndpointTests
{
    private static readonly DateTimeOffset Now = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Create_ReturnsActiveEndpointWithIds()
    {
        var endpoint = Endpoint.Create("Stripe", Now);

        Assert.True(endpoint.IsActive);
        Assert.Equal("Stripe", endpoint.Name);
        Assert.NotEmpty(endpoint.EndpointId);
        Assert.NotEmpty(endpoint.PathToken);
        Assert.Null(endpoint.ExpiresAtUtc);
        Assert.True(endpoint.IsUsableAt(Now));
    }

    [Fact]
    public void Create_WithLifetime_SetsExpiration()
    {
        var endpoint = Endpoint.Create("Stripe", Now, TimeSpan.FromDays(7));

        Assert.Equal(Now.AddDays(7), endpoint.ExpiresAtUtc);
        Assert.True(endpoint.IsUsableAt(Now.AddDays(6)));
        Assert.False(endpoint.IsUsableAt(Now.AddDays(7)));
    }

    [Fact]
    public void Create_WithBlankName_Throws()
    {
        Assert.Throws<ArgumentException>(() => Endpoint.Create("  ", Now));
    }

    [Fact]
    public void Create_WithNonPositiveLifetime_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Endpoint.Create("Stripe", Now, TimeSpan.Zero));
    }

    [Fact]
    public void Deactivate_SetsEndpointInactive()
    {
        var endpoint = Endpoint.Create("Stripe", Now);

        endpoint.Deactivate();

        Assert.False(endpoint.IsActive);
        Assert.False(endpoint.IsUsableAt(Now));
    }

    [Fact]
    public void SetExpiration_InPast_MakesEndpointUnusable()
    {
        var endpoint = Endpoint.Create("Stripe", Now);

        endpoint.SetExpiration(Now.AddMinutes(-1));

        Assert.False(endpoint.IsUsableAt(Now));
    }

    [Fact]
    public void SetExpiration_InFuture_KeepsEndpointUsable()
    {
        var endpoint = Endpoint.Create("Stripe", Now);

        endpoint.SetExpiration(Now.AddDays(1));

        Assert.True(endpoint.IsUsableAt(Now));
    }

    [Fact]
    public void IsUsableAt_ActiveWithNoExpiration_IsUsable()
    {
        var endpoint = Endpoint.Create("Stripe", Now);

        Assert.True(endpoint.IsUsableAt(Now.AddYears(1)));
    }
}
