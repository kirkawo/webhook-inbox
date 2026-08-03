# AGENT.md — Webhook Inbox

## Purpose

This repository contains **Webhook Inbox**, a .NET 8 pet project for developers.

The goal is to build a minimal webhook debugger and inbox:
- create endpoints,
- receive webhooks,
- inspect payloads,
- replay events,
- manage endpoint deactivation and expiration.

The project should look like a small, production-minded cloud-native .NET product.

## Communication rules

- All prompts, implementation plans, commit messages, code comments, and documentation must be written in **English**.
- Keep answers direct and implementation-focused.

## Working rules

- Work phase by phase, do not jump ahead without confirmation.
- Keep the architecture MVC-first.
- Use Docker-first local development.
- Treat Azure deployment as Phase 2+.
- Prefer small, focused changes that can be reviewed and committed cleanly.
- Do **not** use Blazor.
- Do **not** use Angular or React.
- Do **not** introduce unnecessary microservices or abstractions.

## Required stack

- .NET 8
- ASP.NET Core MVC for UI
- Azure Functions (isolated worker) for public webhook ingestion
- Azure Table Storage for persistence
- Azurite for local storage emulation
- Docker Compose for local development

## Main docs to read first

Read these files before making changes:

1. `docs/agent/project-brief.md`
2. `docs/agent/project-overview.md`
3. `docs/agent/architecture-stack.md`
4. `docs/tasks/phase-plan-tasks.md`

If the content of the docs conflicts with this AGENT file, prefer the docs in `docs/`.

## Execution style

- Work one phase at a time.
- Start with repository bootstrap and solution layout.
- Then implement endpoint management.
- Then webhook ingestion.
- Then event inspection UI.
- Then replay and cleanup.
- Finally add CI/CD and Azure deployment.

Implementation guidelines:
- Keep MVC controllers thin.
- Move business logic into application services.
- Keep Azure Functions thin and focused only on request ingestion and forwarding.
- Prefer clear names and straightforward code over clever abstractions.
- Write tests for core domain and storage-backed flows.

## Phase reminders

### Phase 1 — Bootstrap
- Create the solution and projects.
- Wire project references.
- Add MVC shell (`HomeController`, `Home/Index`).
- Add Docker Compose with Azurite.
- Ensure local environment can start easily.

### Phase 2 — Endpoints
- Implement endpoint entity and Table Storage persistence.
- Implement create/list/details/deactivate/expire flows.
- Add MVC views for endpoint management.

### Phase 3 — Ingestion
- Implement Azure Function HTTP trigger for `/in/{token}`.
- Normalize requests and persist events.
- Add integration tests for ingest flow.

### Phase 4 — Event UI
- Extend endpoint details page with event list.
- Implement event details view (headers, body, metadata).

### Phase 5 — Replay & Cleanup
- Implement replay service and UI.
- Enforce expiration.
- Add cleanup path for old events/expired endpoints.

### Phase 6 — CI/CD & Azure
- Add GitHub Actions workflows.
- Add Azure deployment scripts (Bicep/infra).
- Update README with deploy instructions.

## Output expectations

For every task or phase, provide:
- a short English commit message suggestion,
- a concise implementation plan,
- the files changed,
- what is implemented vs stubbed,
- how to run and verify locally.