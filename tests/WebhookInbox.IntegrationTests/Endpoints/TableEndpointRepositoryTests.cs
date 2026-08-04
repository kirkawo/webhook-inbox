using Azure.Data.Tables;
using WebhookInbox.Application.Endpoints;
using WebhookInbox.Infrastructure.Endpoints;

namespace WebhookInbox.IntegrationTests.Endpoints;

public class TableEndpointRepositoryTests : IAsyncLifetime
{
    private const string ConnectionString =
        "DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";

    private readonly string _tableName = "endpoints" + Guid.NewGuid().ToString("N");
    private TableServiceClient _service = null!;
    private EndpointService _endpoints = null!;

    public async Task InitializeAsync()
    {
        _service = new TableServiceClient(ConnectionString);
        var table = _service.GetTableClient(_tableName);
        await table.CreateIfNotExistsAsync();
        _endpoints = new EndpointService(new TableEndpointRepository(table));
    }

    public async Task DisposeAsync()
    {
        await _service.DeleteTableAsync(_tableName);
    }

    [Fact]
    public async Task Create_Then_Get_ReturnsEndpoint()
    {
        var created = await _endpoints.CreateAsync(new CreateEndpointRequest("Stripe", TimeSpan.FromDays(7)));

        var loaded = await _endpoints.GetAsync(created.EndpointId);

        Assert.NotNull(loaded);
        Assert.Equal(created.EndpointId, loaded!.EndpointId);
        Assert.Equal("Stripe", loaded.Name);
        Assert.True(loaded.IsActive);
        Assert.NotNull(loaded.ExpiresAtUtc);
    }

    [Fact]
    public async Task List_ReturnsCreatedEndpoints()
    {
        await _endpoints.CreateAsync(new CreateEndpointRequest("First"));
        await _endpoints.CreateAsync(new CreateEndpointRequest("Second"));

        var all = await _endpoints.ListAsync();

        Assert.Equal(2, all.Count);
    }

    [Fact]
    public async Task Get_Missing_ReturnsNull()
    {
        Assert.Null(await _endpoints.GetAsync("does-not-exist"));
    }

    [Fact]
    public async Task Deactivate_Persists()
    {
        var created = await _endpoints.CreateAsync(new CreateEndpointRequest("Stripe"));

        var result = await _endpoints.DeactivateAsync(created.EndpointId);

        Assert.True(result);
        var loaded = await _endpoints.GetAsync(created.EndpointId);
        Assert.False(loaded!.IsActive);
    }

    [Fact]
    public async Task Expire_Persists()
    {
        var created = await _endpoints.CreateAsync(new CreateEndpointRequest("Stripe"));

        var result = await _endpoints.ExpireAsync(created.EndpointId);

        Assert.True(result);
        var loaded = await _endpoints.GetAsync(created.EndpointId);
        Assert.NotNull(loaded!.ExpiresAtUtc);
    }
}
