# Garimpo Espacial

Este é o monorepo para o projeto Garimpo Espacial, uma plataforma de inteligência de dados para mapeamento de detritos espaciais.

## Estrutura do Projeto

O repositório está organizado da seguinte forma:

-   `/backend`: Contém a API RESTful desenvolvida em ASP.NET Core, seguindo uma arquitetura Hexagonal.
-   `/frontend`: Contém o aplicativo móvel desenvolvido em React Native.
-   `/docker-compose.yml`: Orquestra todos os serviços (frontend, backend, banco de dados) para o ambiente de desenvolvimento local.

## Como Rodar

Consulte o `README.md` dentro de cada pasta (`backend/` e `frontend/`) para instruções específicas de cada parte do projeto.

Para rodar a aplicação completa, utilize o Docker Compose a partir da raiz do projeto:

```bash
docker-compose up --build
```
