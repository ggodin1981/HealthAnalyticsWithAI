# CareLytix Health Analytics – Enterprise Sample (C#, ASP.NET Core, PostgreSQL, AI Assistant)

This repository is an **enterprise-style reference implementation** designed to demonstrate experience as a **Lead C#/.NET Developer** for data-driven healthcare systems.

It showcases:

- C#, ASP.NET Core 8, RESTful APIs
- PostgreSQL with EF Core and proper schema design
- Clean, layered architecture (Domain, Application, Infrastructure, API)
- **JWT / OAuth2-style authentication** with role-based authorization
- **API-first design with Swagger + Bearer token flow**
- **AI-powered analytics assistant** (LLM-backed cohort / patient insights)
- Docker + Docker Compose
- Kubernetes-ready deployments
- Front-end integration using **Next.js, React, TypeScript, Tailwind CSS**

## AI Assistant Overview

- Backend service: `LlmAnalyticsAssistantService` (infrastructure) implements `IAnalyticsAssistantService`.
- It combines:
  - Patient + encounter data (PostgreSQL via EF Core)
  - Population statistics (counts, average risk score)
  - A configurable LLM endpoint (`Ai` section in `appsettings`).
- Exposed via secured endpoints:
  - `POST /api/ai/analyze` – ask a question with optional `patientMrn`
  - `POST /api/ai/chat` – chat-style co-pilot for analytics / architecture
- Frontend page: `/ai-assistant` in the Next.js app, which calls `/api/ai/analyze`.

> Note: The AI endpoint is **generic**. Configure it to point to your preferred provider (e.g., Azure OpenAI, etc.) via:
>
> ```json
> "Ai": {
>   "Endpoint": "https://YOUR-AI-ENDPOINT",
>   "Path": "/v1/chat/completions",
>   "ApiKey": "SET_ME_IN_USER_SECRETS_OR_ENV",
>   "Model": "gpt-4.1-mini"
> }
> ```

## Quick Start (Docker Compose)

```bash
cd deploy/docker
docker compose up --build
```

- API: http://localhost:5000 (Swagger: `/swagger`)
- Web: http://localhost:3000
- PostgreSQL: localhost:5432 (carelytix / changeme)

## Auth & Roles

- Seeded demo user:

  - **Username:** `admin`
  - **Password:** `Admin123!`
  - **Role:** `Admin`

- `POST /api/auth/login` → JWT access token.
- Use the token as `Authorization: Bearer {token}`.
- `PatientsController` and `AiController` are protected with role-based authorization.

## Frontend Usage

```bash
cd frontend/carelytix-web
npm install
echo "NEXT_PUBLIC_API_BASE_URL=http://localhost:5000" > .env.local
npm run dev
```

- `/patients`:
  - Click **Demo Login** to obtain a JWT.
  - Lists patients via secured API.
- `/ai-assistant`:
  - Type a question and optional MRN.
  - The API joins DB data + LLM response to produce narrative insights.
