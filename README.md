# Garimpo Espacial

Plataforma de inteligência de dados para mapeamento de detritos orbitais na **Órbita Baixa da Terra (LEO)**. O sistema ingere telemetria **TLE** da Celestrak/NORAD, aplica **DBSCAN** para identificar aglomerados de lixo espacial e gera alertas de risco — com foco na mitigação da **Síndrome de Kessler**.

Mono-repo FIAP (Global Solution · Exploração Espacial): backend **ASP.NET Core 9** (hexagonal) + app **Expo / React Native** (web e mobile) + **PostgreSQL 16**, orquestrados por Docker Compose.

## Documentação por disciplina

| Disciplina | Documento |
| --- | --- |
| **GS Arquitetura de Software** | [backend/docs/ARCHITECTURE.md](backend/docs/ARCHITECTURE.md) |
| **GS Cybersecurity** | [backend/docs/CYBERSECURITY.md](backend/docs/CYBERSECURITY.md) |
| **GS C# / API Backend** | [backend/README.md](backend/README.md) |
| **GS Mobile (React Native / Expo)** | [frontend/README.md](frontend/README.md) |

## Integrantes

| Nome | RM |
| --- | --- |
| Ricardo Fernandes de Aquino | 554597 |
| Khadija do Rocio Vieira de Lima | 558971 |

## Como testar (avaliadores)

1. Clone o repositório e configure o ambiente:

```bash
sh scripts/setup-env.sh
# Edite .env: POSTGRES_PASSWORD, Security__Jwt__Secret (mín. 32 chars) e ConnectionStrings
docker compose up --build
```

2. Acesse o app em **http://localhost:8081** (web) ou escaneie o QR code do Expo Go (mobile).
3. API e Swagger em **http://localhost:8080/swagger**.

**Conta de teste** (criada automaticamente no seed):

| E-mail | Senha |
| --- | --- |
| `fiap@teste.com` | `123456` |

**Fluxo sugerido para validação:**

1. Abrir a landing → **Entrar** ou **Criar conta**
2. No painel do analista → executar **Ingestão** e **Pipeline** (ou aguardar o carregamento inicial)
3. Ajustar **ε** e **minPoints** no painel DBSCAN → **Executar DBSCAN**
4. Conferir gráficos, listas paginadas, aba **Alertas** e **Perfil**
5. Usar **← Site** na navbar do analista para voltar à landing **sem deslogar**

## Estrutura do repositório

```
/
├── backend/          # API REST ASP.NET Core (hexagonal)
├── frontend/         # App Expo (web + mobile)
├── scripts/
│   └── setup-env.sh  # cria .env a partir de .env.example
├── docker-compose.yml
├── .env.example
└── README.md         # este arquivo
```

| Pasta | Conteúdo |
| --- | --- |
| [`backend/`](backend/) | Domain, Application, Infrastructure, Api, migrations EF Core, Swagger |
| [`frontend/`](frontend/) | Landing pública, painel do analista, gráficos, integração JWT |
| [`docker-compose.yml`](docker-compose.yml) | Orquestra PostgreSQL + API + frontend |

Cada aplicação também possui `Dockerfile` e `docker-compose.yml` próprios para rodar isoladamente.

## Configuração (.env)

Secrets ficam fora do Git. Use o script na raiz do repositório e edite os valores obrigatórios:

```bash
sh scripts/setup-env.sh
```

| Variável | Obrigatório | Descrição |
| --- | --- | --- |
| `Security__Jwt__Secret` | Sim | Assina tokens JWT (mín. 32 caracteres) |
| `POSTGRES_PASSWORD` | Sim | Senha do banco PostgreSQL |
| `EXPO_PUBLIC_API_URL` | Sim (frontend) | URL da API — padrão `http://localhost:8080` |
| `ExternalServices__Celestrak__*` | Não | Catálogo TLE público, sem API key |
| `ExternalServices__SpaceTrack__*` | Não | NORAD oficial (opcional, requer cadastro) |

Detalhes de segurança: [backend/docs/CYBERSECURITY.md](backend/docs/CYBERSECURITY.md).

## URLs locais

| Serviço | URL |
| --- | --- |
| App (web) | http://localhost:8081 |
| API | http://localhost:8080 |
| Swagger | http://localhost:8080/swagger |
| PostgreSQL | localhost:5432 |

## Rodar sem Docker (desenvolvimento)

```bash
# Backend — ver backend/README.md
cd backend && dotnet run --project src/Api/Garimpo.Api.csproj

# Frontend — ver frontend/README.md
cd frontend && npm install && npx expo start --web
```
