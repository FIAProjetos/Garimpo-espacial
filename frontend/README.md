# Garimpo Espacial — Frontend (Expo / React Native)

App **Expo SDK 52** para web e mobile. Conecta-se à API REST do backend para visualizar detritos orbitais, clusters **DBSCAN**, gráficos altitude × inclinação e alertas de risco orbital.

Documentação geral do mono-repo e links por disciplina: [README.md](../README.md).

## Integrantes

| Nome | RM |
| --- | --- |
| Ricardo Fernandes de Aquino | 554597 |
| Khadija do Rocio Vieira de Lima | 558971 |

## Usuário de teste (exigência da sprint)

| E-mail | Senha |
| --- | --- |
| `fiap@teste.com` | `123456` |

Também é possível **criar conta** pelo modal de registro na landing.

## Pré-requisitos

- Node.js 20+
- Backend rodando em `http://localhost:8080` (ou IP da máquina no dispositivo físico)
- Expo Go no celular, emulador ou navegador

## Configuração

Na raiz do mono-repo, configure o `.env`:

```bash
EXPO_PUBLIC_API_URL=http://localhost:8080
```

Em dispositivo físico, use o IP da sua máquina (ex.: `http://192.168.1.10:8080`).

## Como rodar

### Com Docker (recomendado — stack completa na raiz)

```bash
# Na raiz do mono-repo
sh scripts/setup-env.sh   # criar .env e editar secrets
docker compose up --build
```

App web: **http://localhost:8081**

### Local (apenas frontend)

```bash
cd frontend
npm install
npx expo start
```

| Plataforma | Como abrir |
| --- | --- |
| **Web (navegador)** | `npx expo start --web` ou pressione `w` → `http://localhost:8081` |
| **Mobile (Expo Go)** | Escaneie o QR code no app Expo Go |
| **Android** | Pressione `a` no terminal (emulador) |
| **iOS** | Pressione `i` no terminal (simulador, Mac) |

## Navegação

O app divide-se em duas áreas, acessíveis **sem logout** entre elas:

| Área | Acesso | Conteúdo |
| --- | --- | --- |
| **Site público** | Landing, navbar com Entrar / Criar conta | Home, Planos |
| **Painel do analista** | Após login ou botão Painel (se já logado) | Dashboard, Alertas, Perfil |

- **Login** e **Registro** abrem em modais sobre a landing.
- Na navbar do analista: **Painel**, **Alertas**, **Perfil**, relógio UTC, badge do usuário e **← Site** (volta à landing mantendo a sessão).

## Telas e funcionalidades

### Site público

1. **Home** — landing com hero (slideshow), contexto sobre Síndrome de Kessler, como funciona, features e CTAs.
2. **Planos** — Beta Gratuito (ativo) e Pro (em breve).

### Painel do analista (mission control)

3. **Dashboard**
   - Cabeçalho estilo mission control com ações de **Ingestão** e **Pipeline**
   - Gráfico **visão geral** (altitude × inclinação) com clusters e detritos
   - Painel **DBSCAN** com parâmetros **ε** (epsilon) e **minPoints** + botão **Executar DBSCAN**
   - Gráfico **experimento** (resultado da execução customizada)
   - Listas paginadas de aglomerados e detritos (abas Clusters / Detritos)
4. **Alertas** — monitoramento de risco orbital com resumo por severidade.
5. **Perfil** — dados da conta e logout.

## Integração com a API

| Ação na UI | Endpoint |
| --- | --- |
| Registro / Login | `POST /api/auth/register`, `POST /api/auth/login` |
| Ingestão | `POST /api/ingestion` |
| Pipeline (ingestão + clustering) | ingestão + `POST /api/clusters/run` |
| DBSCAN customizado | `POST /api/clusters/run` com `{ epsilon, minPoints }` |
| Listagens | `GET /api/clusters`, `GET /api/debris` (paginados) |
| Alertas | `GET /api/alerts` |

Autenticação via **JWT Bearer**; token persistido em AsyncStorage.

## Stack

- Expo SDK 52 + React Native Web
- React Navigation (stack raiz + bottom tabs no analista)
- AsyncStorage (sessão JWT)
- react-native-svg (scatter chart com grade e legenda)
- expo-linear-gradient, @expo-google-fonts/exo-2

## Estrutura relevante

```
frontend/src/
├── components/
│   ├── landing/          # Hero, Kessler, Features, Footer...
│   ├── AnalystNavbar.tsx
│   ├── ClusteringPanel.tsx
│   ├── ScatterChart.tsx
│   ├── LoginModal.tsx
│   └── RegisterModal.tsx
├── navigation/
│   ├── RootNavigator.tsx # Public ↔ Analyst
│   └── MainTabs.tsx      # Dashboard, Alertas, Perfil
└── screens/
    ├── HomeScreen.tsx
    ├── PricingScreen.tsx
    ├── DashboardScreen.tsx
    ├── AlertsScreen.tsx
    └── ProfileScreen.tsx
```
