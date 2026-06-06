#!/bin/sh
set -e

# Keep node_modules in sync when package-lock.json changes (anonymous volume persists across rebuilds).
if [ ! -d node_modules ] || [ ! -f node_modules/.package-lock.json ] || ! cmp -s package-lock.json node_modules/.package-lock.json 2>/dev/null; then
  echo "Installing frontend dependencies..."
  npm ci
  cp package-lock.json node_modules/.package-lock.json
fi

exec "$@"
