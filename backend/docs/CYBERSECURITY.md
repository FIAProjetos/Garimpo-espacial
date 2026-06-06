# Cybersecurity - Garimpo Espacial Backend

Documento de seguranca da informacao para a Global Solution, alinhado aos entregaveis da
disciplina de Cybersecurity (3ES 2026).

---

## Integrantes

| Nome | RM |
| --- | --- |
| Ricardo Fernandes de Aquino | 554597 |
| Khadija do Rocio Vieira de Lima | 558971 |

---

## 1. Analise de Riscos e Ameacas (Threat Modeling)

### 1.1 Ativos criticos

| Ativo | Criticidade | Descricao |
| --- | --- | --- |
| Catalogo TLE (telemetria orbital) | Alta | Dados de posicao de detritos e satelites; manipulacao pode causar rotas de coleta incorretas |
| Resultados DBSCAN (aglomerados) | Alta | Inteligencia de densidade usada para decisoes de missao |
| API REST | Alta | Ponto de entrada para ingestao e processamento |
| PostgreSQL | Critica | Persistencia de detritos, clusters, alertas e usuarios |
| Credenciais JWT + senhas | Critica | Autenticacao de operadores e analistas |

### 1.2 Vetores de ataque (Red Team)

| # | Vetor | Cenario no Garimpo Espacial | Impacto |
| --- | --- | --- | --- |
| 1 | **Manipulacao de telemetria** | Atacante injeta TLE falsos via ingestao ou MITM na Celestrak | Rotas de interceptacao incorretas; colisao |
| 2 | **DDoS / indisponibilidade** | Flood em `POST /api/ingestion` ou `/api/clusters/run` | Centro de controle sem dados atualizados |
| 3 | **Acesso nao autorizado** | Operacao sem token JWT valido (ingestao, clustering, leitura) | Corrupcao do catalogo ou vazamento de inteligencia |
| 4 | **Roubo de credenciais** | Senha fraca ou vazamento do secret JWT | Impersonacao de operadores |

### 1.3 Controles implementados (Blue Team)

| Vetor | Controle | Implementacao no codigo |
| --- | --- | --- |
| Manipulacao de telemetria | Validacao TLE + alertas de integridade | `TleParser`, `TelemetryIntegrityAlert`, hash SHA-256 em `Debris.ComputeTleIntegrityHash()` |
| DDoS | Rate limiting | `AddRateLimiter` — 60 req/min (leitura), 10 req/min (escrita) |
| Acesso nao autorizado | JWT Bearer + FallbackPolicy global | `JwtBearer`, `[AllowAnonymous]` apenas em `/api/auth/*` |
| Senhas em texto claro | Hash BCrypt | `BcryptPasswordHasher` (work factor padrao BCrypt) |
| Vazamento de dados | DTOs sem TLE bruto nem PasswordHash | `DebrisDto`, `UserDto` expõem apenas metadados |
| Headers inseguros | Security headers | `SecurityHeadersMiddleware` (CSP, X-Frame-Options, nosniff) |
| Auditoria | Logs estruturados | `AuditLoggingMiddleware` para operacoes POST/PUT/DELETE |

---

## 2. Arquitetura de Seguranca (Controles)

### 2.1 Controle de acesso

- **Autenticacao**: registro e login com email/senha; emissao de JWT assinado com HMAC-SHA256.
- **Autorizacao**: **FallbackPolicy** exige usuario autenticado em todos os endpoints por padrao; apenas `/api/auth/register` e `/api/auth/login` sao publicos (`[AllowAnonymous]`).
- **Roles**: `UserRole.Analyst` (padrao) e `UserRole.Admin` (futuro); claim `role` no token.
- **CORS restrito**: origens configuradas em `Security:AllowedOrigins` (nao `AllowAnyOrigin`).

### 2.2 Protecao de dados

| Estado | Medida |
| --- | --- |
| **Em transito** | HTTPS recomendado em producao (Fly.io/Vercel); TLS no PostgreSQL gerenciado |
| **Em repouso** | Senhas com BCrypt; JWT secret via `Security__Jwt__Secret`; sem secrets no codigo |
| **Integridade** | Hash SHA-256 de linhas TLE; alertas de rejeicao na ingestao |
| **Minimizacao (LGPD)** | DTOs nao expõem dados brutos sensiveis; logs de auditoria sem payload TLE completo |

### 2.3 Seeding de usuario de teste

No startup, apos migrations, `DatabaseSeeder` cria idempotentemente:

| Email | Senha | Role |
| --- | --- | --- |
| `fiap@teste.com` | `123456` | Analyst |

Requisito da sprint Mobile (FIAP). O hash e gerado com o mesmo `IPasswordHasher` do fluxo de registro — o login funciona de verdade.

### 2.4 Seguranca da infraestrutura

- **Rede isolada**: PostgreSQL acessivel apenas na rede Docker interna (`db` hostname).
- **Healthcheck**: Postgres com `pg_isready` antes do backend subir.
- **Zero Trust (parcial)**: toda requisicao valida JWT; nenhuma confianca implicita por IP.
- **Monitoramento**: logs de auditoria com actor, IP, status HTTP e duracao.

---

## 3. Governanca e Compliance

### 3.1 ISO 27001 (principios aplicados)

| Principio | Aplicacao no projeto |
| --- | --- |
| Gestao de riscos | Threat modeling documentado (secao 1) |
| Controle de acesso | JWT + roles (`Analyst`, `Admin`) |
| Seguranca em desenvolvimento | Excecoes tipadas, sem stack trace em producao (`ProblemDetails`) |
| Continuidade | Retry de migrations no startup; rate limiting para resiliencia |

### 3.2 LGPD / Privacidade

O Garimpo Espacial armazena **email e nome** de usuarios registrados (autenticacao). Dados orbitais sao **publicos** (Celestrak/NORAD).

- Nenhum dado de localizacao terrestre e armazenado.
- Logs de auditoria registram IP e actor, com retencao configuravel pelo operador.
- DTOs seguem minimizacao de dados (sem TLE bruto nem hash de senha nas respostas).

---

## 4. Plano de Resiliencia e Continuidade

### 4.1 Resposta a incidentes

| Fase | Acao |
| --- | --- |
| **Deteccao** | Alertas `TelemetryIntegrityAlert` (Critical); logs `AUDIT` com status 401/429/500 |
| **Contencao** | Rotacionar `Security:Jwt:Secret` (invalida tokens existentes); reduzir `PermitLimit` do rate limiter; bloquear IP no reverse proxy |
| **Erradicacao** | `DELETE` de debris corrompidos; re-ingestao TLE da fonte oficial; `POST /api/clusters/run` |
| **Recuperacao** | Restore do volume PostgreSQL; `docker compose up`; validar via Swagger |
| **Licoes aprendidas** | Atualizar threat model; revisar limiares de `TelemetryIntegrityAlert` |

### 4.2 Continuidade operacional

- Migrations idempotentes aplicadas no startup com retry (10 tentativas).
- Seeding idempotente de usuario de teste apos migrations.
- Compose com `depends_on: condition: service_healthy` garante ordem de subida.
- Ingestao deduplica por NORAD ID (idempotente).

---

## 5. Configuracao (.env)

Secrets **nunca** ficam em `appsettings.json` versionado. Fluxo obrigatorio:

```bash
cp .env.example .env   # na raiz do mono-repo
# Editar .env com senhas e secret JWT forte (min. 32 caracteres)
```

| Variavel | Segredo? | Observacao |
| --- | --- | --- |
| `Security__Jwt__Secret` | **Sim** | Assina tokens JWT (min. 32 chars) |
| `Security__Jwt__Issuer` | Nao | Emissor do token |
| `Security__Jwt__Audience` | Nao | Audiencia do token |
| `Security__Jwt__ExpirationHours` | Nao | Validade do token (padrao 24h) |
| `POSTGRES_PASSWORD` | **Sim** | Banco de dados |
| `ExternalServices__SpaceTrack__*` | **Sim** (se usar) | Credenciais NORAD opcionais |
| `ExternalServices__Celestrak__*` | Nao | URL publica, sem autenticacao |

- `.env` esta no `.gitignore`
- Docker Compose carrega via `env_file: .env`
- `dotnet run` carrega via `DotNetEnv.TraversePath()` no startup
- Producao: falha se `Security:Jwt:Secret` ou connection string ausentes

### Teste de autenticacao

```bash
# Login (usuario seed)
TOKEN=$(curl -s -X POST http://localhost:8080/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"fiap@teste.com","password":"123456"}' | jq -r .token)

# Sem token -> 401
curl http://localhost:8080/api/clusters

# Com Bearer -> 200
curl http://localhost:8080/api/clusters -H "Authorization: Bearer $TOKEN"
```

---

## 6. Roadmap de seguranca (proximas iteracoes)

- Refresh tokens e revogacao de sessao.
- mTLS entre servicos em producao.
- WAF / Cloudflare na borda.
- Scan de vulnerabilidades no CI (Dependabot, `dotnet list package --vulnerable`).
