# Garimpo Espacial — Mobile

Aplicativo React Native (Expo) para análise de detritos orbitais. Conecta-se à API REST do backend para visualizar clusters DBSCAN, gráficos altitude × inclinação e alertas de risco.

## Integrantes

| Nome | RM |
| --- | --- |
| Ricardo Fernandes de Aquino | 554597 |
| Khadija do Rocio Vieira de Lima | 558971 |

## Usuário de teste (exigencia da sprint)

| E-mail | Senha |
| --- | --- |
| `fiap@teste.com` | `123456` |

## Pré-requisitos

- Node.js 20+
- Backend rodando em `http://localhost:8080` (ou IP da máquina no dispositivo físico)
- Expo Go no celular ou emulador Android/iOS

## Configuração

Na raiz do mono-repo, configure o `.env`:

```bash
EXPO_PUBLIC_API_URL=http://localhost:8080
```

Em dispositivo físico, use o IP da sua máquina (ex.: `http://192.168.1.10:8080`).

## Como rodar

```bash
cd frontend
npm install
npx expo start
```

| Plataforma | Como abrir |
| --- | --- |
| **Web (navegador)** | `npx expo start --web` ou pressione `w` no terminal → `http://localhost:8081` |
| **Mobile (Expo Go)** | Escaneie o QR code no app Expo Go |
| **Android** | Pressione `a` no terminal (emulador) |
| **iOS** | Pressione `i` no terminal (simulador, Mac) |

## Telas

1. **Home** — objetivos do projeto e CTAs
2. **Planos** — Beta Gratuito (ativo) + Pro (em breve)
3. **Dashboard** — painel do analista (gráficos + listas paginadas de clusters e detritos)
4. **Alertas** — monitoramento de risco orbital
5. **Perfil** — conta e logout

O login abre em modal; após autenticação, o app vai direto para o Dashboard.

## Stack

- Expo SDK 52
- React Navigation (stack + bottom tabs)
- AsyncStorage (sessão JWT)
- react-native-svg (scatter chart)
