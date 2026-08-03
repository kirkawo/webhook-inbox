***

**Project Overview – Webhook Inbox**

This project is a pet project called **Webhook Inbox**.

The goal is to build a small, production-minded developer tool that acts like a minimal RequestBin / webhook inbox and debugger, using a modern .NET 8 stack.

### Communication rules

- The owner of this project will talk to you (the coding agent) **in Russian**.
- You must provide:
  - **all prompts**, implementation plans, and explanations in **English**,
  - **all commit messages** in **English** (short, conventional-commit style),
  - **all code comments** and documentation in **English**.
- When you receive Russian messages, treat them as user instructions and respond with clear, concise English, focusing on implementation details and practical steps.

### High-level goal

Build a minimal, but realistic cloud-native .NET product:

- A developer can create a temporary webhook endpoint.
- They can send HTTP requests/webhooks to that endpoint.
- The system stores request metadata and payload.
- The developer can inspect received events in a simple UI.
- They can replay events to another URL.
- They can deactivate or expire endpoints.

This is a **pet project** but should look and feel like a small, serious SaaS-style tool, suitable for use in a portfolio.

### Mandatory technology choices

You **must** follow these constraints:

- **Backend / Runtime**
  - .NET 8
  - ASP.NET Core MVC for all UI (no Blazor, no Angular, no React)
  - Azure Functions (C# .NET isolated worker) for public webhook ingestion
  - Azure Table Storage as the main persistence store
  - Azurite as local emulator for Azure Storage
- **Architecture**
  - MVC-first UI, standard controllers + Razor views
  - Thin controllers (no business logic inside controllers)
  - Thin Azure Functions (no business logic inside functions)
  - Business logic and orchestration in application services
  - Modular monolith with separate projects for MVC, Functions, Domain, Application, Infrastructure, etc.
- **Local development**
  - Docker-first: local development must run easily via Docker Compose
  - Azurite runs in Docker for local Table Storage emulation
- **Cloud**
  - Azure deployment is a later phase (Phase 2+), not the first deliverable
  - Use Azure Functions Consumption/Flex plan for HTTP-triggered ingest
  - Use Azure Table Storage in Azure (matching local Azurite setup)

### Explicit prohibitions

You **must not**:

- Use Blazor for this project.
- Use Angular or React.
- Introduce unnecessary microservices.
- Add billing, payment flows, or team workspace features in the MVP.
- Add complex OAuth/OpenID auth in the MVP.
- Introduce Cosmos DB or Kubernetes in this project.
- Over-engineer the solution with too many abstractions or patterns.

Focus on a clean, pragmatic MVC + Functions + Table Storage stack.

### Desired solution layout

The solution structure should look like this:

```text
webhook-inbox/
├─ src/
│  ├─ WebhookInbox.Mvc/              # ASP.NET Core MVC UI
│  ├─ WebhookInbox.Functions/        # Azure Functions (isolated worker)
│  ├─ WebhookInbox.Contracts/        # DTOs, shared request/response models
│  ├─ WebhookInbox.Domain/           # core entities, value objects, rules
│  ├─ WebhookInbox.Application/      # use cases, services, orchestration
│  └─ WebhookInbox.Infrastructure/   # Azure Table Storage, HTTP clients, Azurite integration
├─ tests/
│  ├─ WebhookInbox.UnitTests/        # domain/application unit tests
│  └─ WebhookInbox.IntegrationTests/ # storage and function integration tests
├─ infra/
│  ├─ docker/                        # docker-compose, Azurite, app containers
│  ├─ bicep/                         # Azure resource definitions (later phases)
│  └─ scripts/                       # helper scripts for dev/deploy
└─ WebhookInbox.sln                  # main solution file
```

### Core product scope (MVP)

The MVP should cover:

- **Endpoint management**
  - Create endpoints from the MVC UI.
  - Generate unique public webhook URL tokens.
  - List existing endpoints.
  - Show endpoint details.
  - Deactivate endpoints.
  - Set and enforce endpoint expiration.

- **Webhook ingestion**
  - Public HTTP routes (Azure Functions): `POST/PUT/PATCH /in/{token}`.
  - Resolve endpoint by token.
  - Read method, headers, query string, content type, body, timestamp, source IP.
  - Store events in Azure Table Storage.
  - Return `200` or `202` quickly to the caller.

- **Event inspection UI**
  - Show recent events per endpoint.
  - Show event details page (method, headers, query string, raw body).
  - Display payload in a readable way (raw text/JSON block).
  - Provide copy buttons for endpoint URL, replay target, etc.

- **Replay**
  - Allow replay of an event to a target URL.
  - Store replay status and replay count.
  - Show replay results in UI (success/failure).

- **Expiration and cleanup**
  - Do not accept new events for expired or deactivated endpoints.
  - Provide a cleanup mechanism for old events and expired endpoints (can be a simple job in later phase).

### Out of scope for MVP

For the initial implementation, **do not include**:

- Billing or subscription management.
- Team workspaces or multi-tenant UI complexity.
- Complex auth (OAuth, social login, etc.).
- Provider-specific signature verification (Stripe, GitHub, etc.).
- Real-time websockets/SignalR dashboards.
- Cosmos DB.
- Kubernetes.
- Advanced analytics, dashboards, or charts.

Keep the MVP small and focused on ingestion, storage, inspection, replay, and simple endpoint lifecycle.

### Data model guidelines

Azure Table Storage should be designed around query patterns.

Suggested tables:

- **Endpoints table**
  - `PartitionKey = workspaceId` (or a simple single workspace ID for MVP)
  - `RowKey = endpointId`
  - Fields: `EndpointId`, `WorkspaceId`, `Name`, `PathToken`, `IsActive`, `SecretHash`, `CreatedAtUtc`, `ExpiresAtUtc`, `LastReceivedAtUtc`, `EventCount`.

- **Events table**
  - `PartitionKey = endpointId`
  - `RowKey = reverseTicks_eventId` (so newest events can be read first without extra sorting)
  - Fields: `EventId`, `EndpointId`, `ReceivedAtUtc`, `Method`, `ContentType`, `QueryString`, `HeadersJson`, `BodyText`, `BodyTruncated`, `SourceIp`, `ReplayCount`, `DeliveryStatus`.

- **Optional EndpointLookup table**
  - `PartitionKey = tokenPrefix`
  - `RowKey = pathToken`
  - Fields: `EndpointId`, `WorkspaceId`, `IsActive`, `ExpiresAtUtc`.

The agent should adjust details as needed, but keep the general approach: efficient PartitionKey/RowKey design, simple queries, no over-complicated schema.

### Phased implementation plan

The owner prefers **phase-based execution**. You should work in clear phases and wait for confirmation before moving to the next phase.

Recommended phases:

1. **Phase 1 – Repository & Solution Bootstrap**
   - Create solution and projects according to the layout above.
   - Wire project references correctly.
   - Add `.gitignore`, `README.md`, `.editorconfig`, `.env.example`.
   - Add basic MVC shell (`HomeController`, `Home/Index` view).
   - Add Docker Compose with Azurite container and app container.
   - Ensure local environment can start with a single command.

2. **Phase 2 – Endpoint Management**
   - Implement Endpoint entity and related repositories (Azure Table Storage).
   - Implement use cases: create, list, get, deactivate, expire endpoint.
   - Implement MVC views: endpoints list, create form, details view.
   - Add tests for endpoint lifecycle.

3. **Phase 3 – Webhook Ingestion**
   - Implement Azure Function HTTP trigger for `/in/{token}`.
   - Resolve endpoint by token; normalize request data.
   - Persist events in the Events table.
   - Add integration tests that send requests and verify stored events.

4. **Phase 4 – Event Inspection UI**
   - Extend endpoint details page with recent events list.
   - Implement event details page (headers, body, metadata).
   - Add basic convenience UX (copy buttons, collapsible headers/payload).

5. **Phase 5 – Replay & Cleanup**
   - Implement replay service and MVC action.
   - Store replay status and count.
   - Enforce endpoint expiration in both UI and ingest path.
   - Add cleanup job for expired endpoints and/or old events.

6. **Phase 6 – Deployment & CI**
   - Add GitHub Actions workflows for build/test and Azure Functions deployment.
   - Add Bicep/infra scripts for Azure resources.
   - Update README with deployment instructions.

### Agent delivery style

For every phase or task, the coding agent should:

- Provide a **short English commit message suggestion** (conventional-commit style).
- Provide a **clear English implementation plan** (steps, files, and responsibilities).
- Return:
  - updated solution/project tree,
  - explanation of what was implemented vs stubbed,
  - notes about configuration and how to run locally (Docker-first),
  - a short list of follow-up items for the next phase.

The owner will interact in Russian, you respond in English, and all code, commits, and docs stay in English.

***