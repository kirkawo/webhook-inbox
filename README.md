# Webhook Inbox

Minimalist webhook inbox & debugger for developers.

## Stack
- .NET 8
- ASP.NET Core MVC
- Azure Functions (isolated worker)
- Azure Table Storage / Azurite (local)
- Docker-first local development

## Status
- [x] Phase 1: Solution bootstrap & local sandbox
- [ ] Phase 2: Endpoint management
- [ ] Phase 3: Webhook ingestion
- [ ] Phase 4: Event inspection UI
- [ ] Phase 5: Replay & cleanup
- [ ] Phase 6: Azure deployment & CI/CD

## Local Development
```bash
docker compose -f infra/docker/docker-compose.yml up -d
```