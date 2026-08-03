Project: Webhook Inbox

Goal:
Build a minimal webhook debugger and inbox for developers.

Core features:
- Create webhook endpoints from the UI.
- Receive incoming HTTP requests (webhooks) on public URLs.
- Store request metadata (method, headers, query string, content type, body, timestamp, source IP).
- Show endpoints and events in a simple MVC UI.
- Replay a stored event to another URL.
- Deactivate and expire endpoints.

Constraints:
- UI stack: ASP.NET Core MVC only (no Blazor, Angular, React).
- Backend: .NET 8, Azure Functions isolated worker for public ingest, Azure Table Storage for persistence, Docker-first local environment.
- Architecture: modular monolith + separate Functions project, controllers must stay thin, Functions must stay thin, business logic in application services.
- MVP only, no billing, no complex auth, no Cosmos DB, no Kubernetes.

Purpose:
Use this project as a portfolio-ready cloud-native .NET pet project and as a playground for MVC + Azure Functions + Docker.