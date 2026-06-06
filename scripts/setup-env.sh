#!/bin/sh
# Cria .env a partir de .env.example na raiz do mono-repo.
# Uso: sh scripts/setup-env.sh   (a partir da raiz do repositorio)

set -eu

ROOT="$(cd "$(dirname "$0")/.." && pwd)"
ENV_FILE="$ROOT/.env"
EXAMPLE="$ROOT/.env.example"

if [ ! -f "$EXAMPLE" ]; then
  echo "Erro: .env.example nao encontrado em $ROOT" >&2
  exit 1
fi

if [ -f "$ENV_FILE" ]; then
  echo "Arquivo .env ja existe em $ROOT"
  echo "Edite-o manualmente ou remova-o e rode o script de novo."
  exit 0
fi

cp "$EXAMPLE" "$ENV_FILE"

echo "Criado: $ENV_FILE"
echo ""
echo "Edite o .env com seus proprios valores antes de subir os containers:"
echo "  1. POSTGRES_PASSWORD"
echo "  2. Security__Jwt__Secret (minimo 32 caracteres)"
echo "  3. ConnectionStrings__DefaultConnection (mesma senha do POSTGRES_PASSWORD)"
echo ""
echo "Opcional:"
echo "  - EXPO_PUBLIC_API_URL (IP da maquina, se testar no celular)"
echo "  - ExternalServices__SpaceTrack__* (somente se usar Space-Track)"
echo ""
echo "Depois:"
echo "  docker compose up --build"
