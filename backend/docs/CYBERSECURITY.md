# Cybersecurity - Garimpo Espacial Backend

Documento de seguranca da informacao para a Global Solution, alinhado aos entregaveis da
disciplina de Cybersecurity (3ES 2026).

---

## 1. Analise de Riscos e Ameacas (Threat Modeling)

### 1.1 Ativos criticos

| Ativo | Criticidade | Descricao |
| --- | --- | --- |
| Catalogo TLE (telemetria orbital) | Alta | Dados de posicao de detritos e satelites; manipulacao pode causar rotas de coleta incorretas |
| Resultados DBSCAN (aglomerados) | Alta | Inteligencia de densidade usada para decisoes de missao |
| API REST | Alta | Ponto de entrada para ingestao e processamento |
| PostgreSQL | Critica | Persistencia de detritos, clusters e alertas |
| Chaves de API | Critica | Credenciais de operadores de missao |

### 1.2 Vetores de ataque (Red Team)

| # | Vetor | Cenario no Garimpo Espacial | Impacto |
| --- | --- | --- | --- |
| 1 | **Manipulacao de telemetria** | Atacante injeta TLE falsos via ingestao ou MITM na Celestrak | Rotas de interceptacao incorretas; colisao |
| 2 | **DDoS / indisponibilidade** | Flood em `POST /api/ingestion` ou `/api/clusters/run` | Centro de controle sem dados atualizados |
| 3 | **Acesso nao autorizado** | Operacao de escrita sem credencial (ingestao, clustering) | Corrupcao do catalogo ou reprocessamento malicioso |

### 1.3 Controles implementados (Blue Team)

| Vetor | Controle | Implementacao no codigo |
| --- | --- | --- |
| Manipulacao de telemetria | Validacao TLE + alertas de integridade | `TleParser`, `TelemetryIntegrityAlert`, hash SHA-256 em `Debris.ComputeTleIntegrityHash()` |
| DDoS | Rate limiting | `AddRateLimiter` — 60 req/min (leitura), 10 req/min (escrita) |
| Acesso nao autorizado | API Key + privilegio minimo | `ApiKeyAuthenticationHandler`, `[Authorize]` em POST, leitura publica |
| Vazamento de dados | DTOs sem TLE bruto em listagens | `DebrisDto` expoe metadados, nao `Line1`/`Line2` |
| Headers inseguros | Security headers | `SecurityHeadersMiddleware` (CSP, X-Frame-Options, nosniff) |
| Auditoria | Logs estruturados | `AuditLoggingMiddleware` para operacoes POST/PUT/DELETE |
| Comparacao de chaves | Timing-safe | `CryptographicOperations.FixedTimeEquals` no auth handler |

---

## 2. Arquitetura de Seguranca (Controles)

### 2.1 Controle de acesso

- **Autenticacao**: API Key via header `X-Api-Key` (configuravel por `Security:ApiKey`).
- **Autorizacao**: principio de **privilegio minimo** — endpoints de escrita exigem `[Authorize]`; consultas (GET) sao publicas para dashboards.
- **CORS restrito**: origens configuradas em `Security:AllowedOrigins` (nao `AllowAnyOrigin`).

### 2.2 Protecao de dados

| Estado | Medida |
| --- | --- |
| **Em transito** | HTTPS recomendado em producao (Fly.io/Vercel); TLS no PostgreSQL gerenciado |
| **Em repouso** | Credenciais via variaveis de ambiente (`Security__ApiKey`, `ConnectionStrings__DefaultConnection`); sem secrets no codigo |
| **Integridade** | Hash SHA-256 de linhas TLE; alertas de rejeicao na ingestao |
| **Minimizacao (LGPD)** | DTOs nao expõem dados brutos sensiveis; logs de auditoria sem payload TLE completo |

### 2.3 Seguranca da infraestrutura

- **Rede isolada**: PostgreSQL acessivel apenas na rede Docker interna (`db` hostname).
- **Healthcheck**: Postgres com `pg_isready` antes do backend subir.
- **Zero Trust (parcial)**: toda operacao de escrita valida API Key; nenhuma confianca implicita por IP.
- **Monitoramento**: logs de auditoria com actor, IP, status HTTP e duracao.

---

## 3. Governanca e Compliance

### 3.1 ISO 27001 (principios aplicados)

| Principio | Aplicacao no projeto |
| --- | --- |
| Gestao de riscos | Threat modeling documentado (secao 1) |
| Controle de acesso | API Key + roles (`Operator`) |
| Seguranca em desenvolvimento | Excecoes tipadas, sem stack trace em producao (`ProblemDetails`) |
| Continuidade | Retry de migrations no startup; rate limiting para resiliencia |

### 3.2 LGPD / Privacidade

O Garimpo Espacial **nao coleta dados pessoais** de usuarios finais. Os dados orbitais sao
**publicos** (Celestrak/NORAD). Mesmo assim:

- Nenhum dado de localizacao terrestre e armazenado.
- Logs de auditoria registram IP e actor, com retencao configuravel pelo operador.
- DTOs seguem minimizacao de dados (sem TLE bruto nas respostas de listagem).

---

## 4. Plano de Resiliencia e Continuidade

### 4.1 Resposta a incidentes

| Fase | Acao |
| --- | --- |
| **Deteccao** | Alertas `TelemetryIntegrityAlert` (Critical); logs `AUDIT` com status 401/429/500 |
| **Contencao** | Rotacionar `Security:ApiKey`; reduzir `PermitLimit` do rate limiter; bloquear IP no reverse proxy |
| **Erradicacao** | `DELETE` de debris corrompidos; re-ingestao TLE da fonte oficial; `POST /api/clusters/run` |
| **Recuperacao** | Restore do volume PostgreSQL; `docker compose up`; validar via Swagger |
| **Licoes aprendidas** | Atualizar threat model; revisar limiares de `TelemetryIntegrityAlert` |

### 4.2 Continuidade operacional

- Migrations idempotentes aplicadas no startup com retry (10 tentativas).
- Compose com `depends_on: condition: service_healthy` garante ordem de subida.
- Ingestao deduplica por NORAD ID (idempotente).

---

## 5. Configuracao (.env)

Secrets **nunca** ficam em `appsettings.json` versionado. Fluxo obrigatorio:

```bash
cp .env.example .env   # na raiz do mono-repo
# Editar .env com senhas e chaves fortes
```

| Variavel | Segredo? | Observacao |
| --- | --- | --- |
| `Security__ApiKey` | **Sim** | Protege operacoes de escrita da nossa API |
| `POSTGRES_PASSWORD` | **Sim** | Banco de dados |
| `ExternalServices__SpaceTrack__*` | **Sim** (se usar) | Credenciais NORAD opcionais |
| `ExternalServices__Celestrak__*` | Nao | URL publica, sem autenticacao |

- `.env` esta no `.gitignore`
- Docker Compose carrega via `env_file: .env`
- `dotnet run` carrega via `DotNetEnv.TraversePath()` no startup
- Producao: falha se `Security:ApiKey` ou connection string ausentes

### Teste de autenticacao

```bash
source .env 2>/dev/null || export $(grep -v '^#' .env | xargs)

# Sem chave -> 401
curl -X POST http://localhost:8080/api/ingestion

# Com chave do .env -> 200
curl -X POST http://localhost:8080/api/ingestion \
  -H "X-Api-Key: $Security__ApiKey"
```

---

## 6. Roadmap de seguranca (proximas iteracoes)

- JWT com refresh tokens para operadores humanos (mobile Expo).
- mTLS entre servicos em producao.
- WAF / Cloudflare na borda.
- Scan de vulnerabilidades no CI (Dependabot, `dotnet list package --vulnerable`).
