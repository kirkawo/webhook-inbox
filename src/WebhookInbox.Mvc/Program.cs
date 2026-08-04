using Azure.Data.Tables;
using WebhookInbox.Application.Endpoints;
using WebhookInbox.Infrastructure.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var connectionString = builder.Configuration["Storage:ConnectionString"];
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Storage:ConnectionString is not configured.");
}

builder.Services.AddSingleton(new TableServiceClient(connectionString));
builder.Services.AddSingleton(sp => sp.GetRequiredService<TableServiceClient>().GetTableClient("Endpoints"));
builder.Services.AddScoped<IEndpointRepository, TableEndpointRepository>();
builder.Services.AddScoped<EndpointService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
