# AGENTS.md — Webhook Inbox

## Purpose
Minimal webhook inbox/debugger for developers (.NET 8 pet project): create endpoints, receive webhooks at `/in/{token}`, inspect payloads, replay events, deactivate/expire endpoints. Production-minded but MVP-only.

## Repo state
- Pre-bootstrap: `src/`, `tests/`, `infra/docker`, `infra/bicep`, `infra/scripts`, `.github/workflows` are empty. No `.sln`/`.csproj` files exist yet. Next step is Phase 1 (see `docs/tasks/phase-plan-tasks.md`).
- Git history uses conventional commits (`chore(repo):`, `docs(agent):`).

## Communication
- Owner writes prompts in Russian. Respond in English. All prompts, plans, commit messages, code comments, and docs must be in English.

## Stack & hard constraints
- .NET 8, ASP.NET Core MVC only — **no Blazor, Angular, or React**.
- Azure Functions (isolated worker) for public ingestion; Azure Table Storage for persistence; Azurite (Docker) locally.
- Docker-first local dev; Azure deployment is Phase 2+.
- Thin MVC controllers and thin Functions; business logic lives in application services.
- No microservices, no over-engineering, plain Table Storage (no EF/migrations), MVP only: no billing, auth, Cosmos DB, Kubernetes.

## Planned layout (project names are fixed)
- `WebhookInbox.sln` at repo root; projects under `src/` and `tests/`:
  - `src/WebhookInbox.Mvc`, `WebhookInbox.Functions`, `WebhookInbox.Contracts`, `WebhookInbox.Domain`, `WebhookInbox.Application`, `WebhookInbox.Infrastructure`
  - `tests/WebhookInbox.UnitTests`, `WebhookInbox.IntegrationTests`
- Storage design: Endpoints (PK=workspaceId, RK=endpointId), Events (PK=endpointId, RK=reverseTicks_eventId for newest-first reads), optional EndpointLookup.

## Docs of record (read before making changes)
1. `docs/agent/project-brief.md`
2. `docs/agent/project-overview.md`
3. `docs/agent/architecture-stack.md`
4. `docs/tasks/phase-plan-tasks.md`

If docs conflict with this file, prefer `docs/`.

## Phases (work one at a time; wait for confirmation before advancing)
1. Bootstrap: solution + 8 projects, references, MVC shell, docker-compose with Azurite.
2. Endpoints: entity, Table Storage repos, create/list/details/deactivate/expire, MVC views.
3. Ingestion: `/in/{token}` Function trigger, normalize request, persist events.
4. Event UI: recent events on endpoint details, event detail page (headers/body/metadata).
5. Replay & cleanup: replay service, expiration enforcement, cleanup job.
6. CI/CD & Azure: GitHub Actions, Bicep, deploy docs in README.

## Commands
- Local dev: `docker compose -f infra/docker/docker-compose.yml up -d` (compose file not created yet).
- Build/test: `dotnet build WebhookInbox.sln`, `dotnet test WebhookInbox.sln` (once Phase 1 exists).

## Output expectations
For each task/phase deliver: a short conventional-commit message, an implementation plan, files changed, what is implemented vs stubbed, and how to run and verify locally.
