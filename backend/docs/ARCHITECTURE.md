# Arquitetura - Garimpo Espacial Backend

## Visao geral

Mono-repo com backend em **ASP.NET Core 9**, **PostgreSQL 16**, **EF Core** e arquitetura **hexagonal**
(clean architecture). O dominio permanece isolado de frameworks; adapters na infraestrutura implementam
as portas definidas na aplicacao.

## Requisitos atendidos (disciplina Arquitetura)

| Requisito | Implementacao |
| --- | --- |
| Mono-repo + Docker Compose raiz | [`docker-compose.yml`](../../docker-compose.yml) orquestra db + backend + frontend |
| Compose por aplicacao | [`backend/docker-compose.yml`](../docker-compose.yml), [`frontend/docker-compose.yml`](../../frontend/docker-compose.yml) |
| Dockerfile por app | [`backend/Dockerfile`](../Dockerfile), [`frontend/Dockerfile`](../../frontend/Dockerfile) |
| Arquitetura hexagonal | 4 camadas: Domain, Application, Infrastructure, Api |
| Swagger | `/swagger` com documentacao OpenAPI |
| Heranca e polimorfismo | Hierarquia `SpaceAsset` → `OrbitalAsset` → `Satellite` / `DebrisAsset`; `SensorAsset`; `Alert` abstrato com subclasses |
| Classes abstratas | `SpaceAsset`, `OrbitalAsset`, `Alert` |
| Interfaces + DI | Portas `I*Repository`, `ITleProvider`, `ISensor`, `IClusteringService` |
| VO / DTO | `OrbitalElements`, `OrbitalCoordinate` (struct), DTOs na Application |
| Struct + Partial | `OrbitalCoordinate` (struct parcial), `Debris` (partial class para TLE) |
| WebService + Banco | API REST + PostgreSQL via EF Core |
| Tratamento de excecoes | Middleware `ExceptionHandlingMiddleware` → `ProblemDetails` |
| Evidencias de execucao | Secao [Evidencias de execucao](#evidencias-de-execucao) |

## Hierarquia de dominio (OOP)

```mermaid
classDiagram
    class SpaceAsset {
        <<abstract>>
        +Guid Id
        +string Name
        +GetSummary()*
        +GetTrackingId()*
    }
    class OrbitalAsset {
        <<abstract>>
        +int NoradId
        +double AltitudeKm
        +CalculateRiskScore()*
    }
    class Satellite
    class DebrisAsset
    class SensorAsset
    class Alert {
        <<abstract>>
        +BuildMessage()*
        +RequiresImmediateAction()*
    }
    class HighDensityClusterAlert
    class TelemetryIntegrityAlert

    SpaceAsset <|-- OrbitalAsset
    OrbitalAsset <|-- Satellite
    OrbitalAsset <|-- DebrisAsset
    SpaceAsset <|-- SensorAsset
    Alert <|-- HighDensityClusterAlert
    Alert <|-- TelemetryIntegrityAlert
```

## Camadas e dependencias

```mermaid
flowchart TB
    subgraph api [Api]
        Controllers
        Middleware
        Security
    end
    subgraph app [Application]
        UseCases
        Ports
        DTOs
    end
    subgraph domain [Domain]
        Entities
        Services
        ValueObjects
    end
    subgraph infra [Infrastructure]
        EFCore
        Celestrak
        Adapters
    end
    api --> app
    api --> infra
    infra --> app
    app --> domain
    infra -.-> Ports
```

## Fluxo de dados

```mermaid
sequenceDiagram
    participant Client
    participant API
    participant UseCase
    participant Domain
    participant Celestrak
    participant DB

    Client->>API: POST /api/auth/login
    API-->>Client: JWT Bearer token

    Client->>API: POST /api/ingestion (Authorization Bearer)
    API->>UseCase: IngestTleUseCase
    UseCase->>Celestrak: Fetch TLE
    Celestrak-->>UseCase: Raw telemetry
    UseCase->>Domain: Parse + classify
    UseCase->>DB: Persist debris
    UseCase->>Domain: Evaluate telemetry alerts
    UseCase-->>Client: IngestionResult

    Client->>API: POST /api/clusters/run (Authorization Bearer)
    API->>UseCase: RunClusteringUseCase
    UseCase->>Domain: DBSCAN
    UseCase->>DB: Persist clusters
    UseCase->>Domain: Generate density alerts
    UseCase-->>Client: ClusteringResult + alerts
```

## Justificativas SOLID

- **SRP**: cada use case encapsula um fluxo (ingestao, clusterizacao, alertas).
- **OCP**: novos tipos de alerta via subclasses de `Alert` sem alterar `AlertEvaluationService`.
- **LSP**: `Satellite` e `DebrisAsset` substituem `OrbitalAsset` em calculos de risco.
- **ISP**: portas granulares (`IDebrisRepository`, `IAlertRepository`) em vez de repositorio monolitico.
- **DIP**: Application depende de abstracoes; Infrastructure implementa adapters.

## Endpoints

| Metodo | Rota | Auth | Descricao |
| --- | --- | --- | --- |
| POST | `/api/auth/register` | Publico | Registrar usuario |
| POST | `/api/auth/login` | Publico | Login e emissao JWT |
| POST | `/api/ingestion` | Bearer | Ingestao TLE |
| POST | `/api/clusters/run` | Bearer | DBSCAN |
| GET | `/api/clusters` | Bearer | Listar aglomerados |
| GET | `/api/debris` | Bearer | Listar detritos |
| GET | `/api/alerts` | Bearer | Listar alertas |
| POST | `/api/alerts/evaluate` | Bearer | Reavaliar alertas |
| POST | `/api/alerts/{id}/acknowledge` | Bearer | Reconhecer alerta |

## Evidencias de execucao

Ambiente validado em `2026-06-06` com `docker compose up --build` na raiz do mono-repo.

### Como reproduzir

```bash
sh scripts/setup-env.sh   # na raiz do mono-repo; depois edite os secrets no .env
docker compose up --build
```

- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`
- App (frontend): `http://localhost:8081`

### Build e startup

```
dotnet build Garimpo.Backend.sln  -> Build succeeded. 0 Error(s)

Applying migration '20260606011510_InitialCreate'.
Migrations aplicadas com sucesso.
Now listening on: http://[::]:8080
```

### Swagger

```
GET /swagger/index.html -> 200
```

### Fluxo completo (API)

```
POST /api/ingestion?group=cosmos-2251-debris
-> {"fetched":588,"imported":588,"skipped":0}

POST /api/clusters/run {"epsilon":0.3,"minPoints":5}
-> {"processedDebris":588,"clustersFound":8,"noiseCount":77}

GET /api/clusters -> 8 aglomerados (maior densidade: ~4.3 em LEO ~760 km)
GET /api/debris    -> 588 detritos catalogados
GET /api/alerts    -> 6 alertas (2 Critical, demais Warning)
```

### Autenticacao JWT

```
POST /api/auth/login (fiap@teste.com / 123456) -> token JWT
GET /api/clusters (sem token) -> 401 Unauthorized
GET /api/clusters (com Bearer) -> 200 OK
Seed: usuario fiap@teste.com criado no startup
```

### Frontend integrado

Com a stack Docker ativa, o app em `http://localhost:8081` autentica via JWT, executa ingestao/pipeline no painel do analista, exibe graficos de clusters/detritos e lista alertas — consumindo os mesmos endpoints documentados acima.
