using WebhookInbox.Application.Endpoints;

namespace WebhookInbox.UnitTests.Endpoints;

public class EndpointServiceTests
{
    private readonly EndpointService _service;

    public EndpointServiceTests()
    {
        _service = new EndpointService(new InMemoryEndpointRepository());
    }

    [Fact]
    public async Task CreateAsync_PersistsActiveEndpoint()
    {
        var created = await _service.CreateAsync(new CreateEndpointRequest("Stripe"));

        Assert.True(created.IsActive);
        Assert.Equal("Stripe", created.Name);

        var loaded = await _service.GetAsync(created.EndpointId);
        Assert.NotNull(loaded);
        Assert.Equal(created.EndpointId, loaded!.EndpointId);
    }

    [Fact]
    public async Task CreateAsync_WithLifetime_SetsExpiration()
    {
        var created = await _service.CreateAsync(new CreateEndpointRequest("Stripe", TimeSpan.FromDays(3)));

        Assert.NotNull(created.ExpiresAtUtc);
        Assert.True(created.ExpiresAtUtc > DateTimeOffset.UtcNow.AddDays(2));
    }

    [Fact]
    public async Task ListAsync_ReturnsAllEndpoints()
    {
        await _service.CreateAsync(new CreateEndpointRequest("First"));
        await _service.CreateAsync(new CreateEndpointRequest("Second"));

        var all = await _service.ListAsync();

        Assert.Equal(2, all.Count);
    }

    [Fact]
    public async Task GetAsync_Missing_ReturnsNull()
    {
        Assert.Null(await _service.GetAsync("does-not-exist"));
    }

    [Fact]
    public async Task DeactivateAsync_DeactivatesEndpoint()
    {
        var created = await _service.CreateAsync(new CreateEndpointRequest("Stripe"));

        var result = await _service.DeactivateAsync(created.EndpointId);

        Assert.True(result);
        var loaded = await _service.GetAsync(created.EndpointId);
        Assert.False(loaded!.IsActive);
    }

    [Fact]
    public async Task DeactivateAsync_Missing_ReturnsFalse()
    {
        Assert.False(await _service.DeactivateAsync("does-not-exist"));
    }

    [Fact]
    public async Task ExpireAsync_SetsExpirationToNow()
    {
        var created = await _service.CreateAsync(new CreateEndpointRequest("Stripe"));
        Assert.Null(created.ExpiresAtUtc);

        var result = await _service.ExpireAsync(created.EndpointId);

        Assert.True(result);
        var loaded = await _service.GetAsync(created.EndpointId);
        Assert.NotNull(loaded!.ExpiresAtUtc);
        Assert.True(loaded.ExpiresAtUtc <= DateTimeOffset.UtcNow.AddSeconds(5));
    }
}
