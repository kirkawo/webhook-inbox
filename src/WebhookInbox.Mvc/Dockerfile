FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/WebhookInbox.Mvc/WebhookInbox.Mvc.csproj src/WebhookInbox.Mvc/
COPY src/WebhookInbox.Application/WebhookInbox.Application.csproj src/WebhookInbox.Application/
COPY src/WebhookInbox.Domain/WebhookInbox.Domain.csproj src/WebhookInbox.Domain/
COPY src/WebhookInbox.Contracts/WebhookInbox.Contracts.csproj src/WebhookInbox.Contracts/
COPY src/WebhookInbox.Infrastructure/WebhookInbox.Infrastructure.csproj src/WebhookInbox.Infrastructure/
COPY src/WebhookInbox.Functions/WebhookInbox.Functions.csproj src/WebhookInbox.Functions/
RUN dotnet restore src/WebhookInbox.Mvc/WebhookInbox.Mvc.csproj

COPY . .
RUN dotnet publish src/WebhookInbox.Mvc/WebhookInbox.Mvc.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "WebhookInbox.Mvc.dll"]
