Architecture:

- WebhookInbox.Mvc
  - ASP.NET Core MVC UI.
  - EndpointsController, EventsController, ReplayController.
  - Razor views for endpoints list, create, details, event details.
  - No Blazor, Angular, or React.

- WebhookInbox.Functions
  - Azure Functions (.NET isolated worker).
  - HTTP trigger for /in/{token} (POST/PUT/PATCH).
  - Thin ingestion layer: resolve token, normalize request, forward into application layer to persist event.

- WebhookInbox.Domain / Application / Infrastructure
  - Domain: core entities (Endpoint, Event), value objects, rules.
  - Application: use cases (CreateEndpoint, IngestEvent, ListEvents, GetEvent, ReplayEvent, PurgeExpired).
  - Infrastructure: Azure Table Storage repositories, Azurite integration, HTTP replay client, clock abstraction, secret hashing.

Storage:

- Azure Table Storage (with Azurite for local dev).
- Tables:
  - Endpoints: PartitionKey = workspaceId, RowKey = endpointId.
  - Events: PartitionKey = endpointId, RowKey = reverseTicks_eventId.
  - Optional EndpointLookup: PartitionKey = tokenPrefix, RowKey = pathToken.

Local environment:

- Docker-first: docker-compose with:
  - MVC app container.
  - Azurite container.
- One command startup: `docker compose -f infra/docker/docker-compose.yml up -d`.

Cloud:

- Azure Functions Consumption/Flex plan for ingest.
- Azure Table Storage for persistence.
- Later phases: Azure App Service or Static Web Apps for UI, GitHub Actions for CI/CD.