Phase 1 — Repository & Solution Bootstrap
- Create GitHub repo with folders: src, tests, infra/docker, infra/bicep, infra/scripts, docs/agent, docs/tasks, .github/workflows.
- Add .gitignore, README.md, .editorconfig, .env.example.
- Create WebhookInbox.sln and projects:
  - WebhookInbox.Mvc
  - WebhookInbox.Functions
  - WebhookInbox.Contracts
  - WebhookInbox.Domain
  - WebhookInbox.Application
  - WebhookInbox.Infrastructure
  - WebhookInbox.UnitTests
  - WebhookInbox.IntegrationTests
- Wire up project references.
- Add basic MVC shell (Home/Index).
- Add docker-compose with Azurite.
- Make sure `docker compose up` starts the app.

Phase 2 — Endpoint Management
- Implement Endpoint entity and repositories (Table Storage).
- Implement use cases: CreateEndpoint, GetEndpoint, ListEndpoints, DeactivateEndpoint, ExpireEndpoint.
- Implement MVC pages: endpoints list, create form, details view.
- Add tests for endpoint creation and retrieval.

Phase 3 — Webhook Ingestion
- Implement Azure Function HTTP trigger for /in/{token}.
- Resolve endpoint by token, normalize request (method, headers, query string, body, content type, timestamp, source IP).
- Persist event in the Events table.
- Return 200/202 quickly.
- Add integration tests for the ingest flow.

Phase 4 — Event Inspection UI
- Extend endpoint details page with recent events list.
- Implement event details page with headers/body/replay form.
- Add basic filtering/sorting if needed.

Phase 5 — Replay & Cleanup
- Implement replay service and MVC action.
- Store replay status and count.
- Implement expiration enforcement for endpoints.
- Add cleanup path for expired endpoints or old events.

Phase 6 — Deployment & CI
- Add GitHub Actions workflows for build/test and Azure Functions deployment.
- Add Bicep/scripts for creating Azure resources.
- Document deploy steps in README.