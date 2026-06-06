# Garimpo Espacial

Este é o monorepo para o projeto Garimpo Espacial, uma plataforma de inteligência de dados para mapeamento de detritos espaciais.

## Estrutura do Projeto

O repositório está organizado da seguinte forma:

-   `/backend`: Contém a API RESTful desenvolvida em ASP.NET Core, seguindo uma arquitetura Hexagonal.
-   `/frontend`: Contém o aplicativo móvel desenvolvido em React Native.
-   `/docker-compose.yml`: Orquestra todos os serviços (frontend, backend, banco de dados) para o ambiente de desenvolvimento local.

## Configuracao (.env) — obrigatorio

Secrets ficam fora do Git (Cybersecurity + Arquitetura):

```bash
cp .env.example .env
# Edite .env: POSTGRES_PASSWORD, Security__ApiKey, EXPO_PUBLIC_API_KEY
```

| Variavel | Precisa de secret? |
| --- | --- |
| `Security__ApiKey` | Sim — protege a nossa API |
| `POSTGRES_PASSWORD` | Sim |
| `ExternalServices__Celestrak__*` | **Nao** — Celestrak e catalogo publico |
| `ExternalServices__SpaceTrack__*` | Sim (opcional) — NORAD oficial |

## Como Rodar

```bash
cp .env.example .env          # primeira vez
docker compose up --build     # raiz: front + back + db
```

Consulte [`backend/README.md`](backend/README.md) e [`backend/docs/CYBERSECURITY.md`](backend/docs/CYBERSECURITY.md) para detalhes.
