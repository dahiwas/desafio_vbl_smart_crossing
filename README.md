# VBL Smart Crossing — Projeto Completo

Simulador de travessia de pedestres estilo Frogger com trafego dinamico alimentado por uma API em tempo real. O projeto e composto por um **jogo em Unity 6** e um **backend Node.js** que fornece dados de trafego (densidade, velocidade, clima) ao jogo.

---

## Estrutura do Repositorio

```
EnviarProjeto/
├── game_unity/     # Jogo Unity 6 (frontend)
│   ├── Assets/
│   │   ├── Scripts/    # 14 scripts C# (~350 linhas)
│   │   ├── Scenes/     # SampleScene.unity
│   │   ├── Mock/       # JSON de exemplo para modo offline
│   │   └── Prefabs/    # Prefab de veiculo
│   └── README.md       # Documentacao detalhada do jogo
│
└── backend_game/       # API de trafego (backend)
    ├── server.js       # Servidor Express.js
    ├── Dockerfile      # Container Docker
    ├── docker-compose.yml
    └── package.json
```

---

## Scripts do Jogo (`Assets/Scripts/`)

O codigo do jogo esta dividido em **4 pastas** com responsabilidades bem definidas:

```
Assets/Scripts/
├── Core/Services/        # Calculos e formulas de gameplay
├── Data/
│   ├── api/              # Comunicacao com a API de trafego
│   └── dto/              # Objetos de transferencia de dados
├── Gameplay/             # Logica principal do jogo
└── UI/                   # Interface visual (HUD)
```

### Core/Services — Calculos de Gameplay

| Script | O que faz |
|---|---|
| `TrafficMath.cs` | Classe estatica com formulas que convertem dados da API em valores usaveis no jogo. Converte densidade de trafego em intervalo de spawn, velocidade media em velocidade real dos veiculos, e clima em multiplicador de velocidade do jogador (ex: `sunny` = 1.0x, `heavy rain` = 0.4x) |

### Data/api — Providers de Trafego

Usa o padrao **Strategy**: uma interface comum com duas implementacoes intercambiaveis.

| Script | O que faz |
|---|---|
| `ITrafficDataProvider.cs` | Interface que define o contrato `GetTrafficStatusAsync()` — qualquer provider deve implementar este metodo |
| `HttpTrafficDataProvider.cs` | **Implementacao online** — faz `GET` em `localhost:3000/v1/traffic/status` usando `UnityWebRequest` e retorna os dados reais do backend |
| `JsonTrafficDataProvider.cs` | **Implementacao offline** — le um arquivo JSON estatico (`Assets/Mock/`), util para testar sem o backend rodando |

### Data/dto — Data Transfer Objects

| Script | O que faz |
|---|---|
| `TrafficResponseDto.cs` | Define 3 classes que espelham a estrutura JSON da API: `TrafficResponseDto` (container principal), `StatusDto` (densidade, velocidade, clima) e `PredictedStatusDto` (predicao futura com `estimated_time`) |

### Gameplay — Logica Principal

| Script | O que faz |
|---|---|
| `GameManager.cs` | **Orquestrador central** — inicia niveis, busca dados do provider, distribui para TrafficManager e PredictionScheduler, gerencia estados de vitoria/derrota e transicao entre niveis |
| `TrafficManager.cs` | Recebe um `StatusDto` e aplica os valores no jogo: atualiza o spawner de veiculos, o multiplicador do jogador e o HUD, usando `TrafficMath` para as conversoes |
| `PredictionScheduler.cs` | Agenda mudancas futuras de trafego — recebe a lista de `predicted_status` e dispara coroutines que aplicam cada predicao no tempo correto (`estimated_time`) |
| `PlayerController.cs` | Controle do jogador — captura input (WASD / setas), move o personagem com `baseSpeed * weatherMultiplier`, reseta posicao ao colidir com veiculos |
| `VehicleController.cs` | Comportamento individual de cada veiculo — move-se continuamente na direcao atribuida e se destroi ao sair da tela |
| `VehicleSpawner.cs` | Instancia veiculos em pontos predefinidos a cada X segundos — o intervalo e a velocidade sao ajustados dinamicamente pelo `TrafficManager` |
| `GoalZone.cs` | Zona de chegada — detecta colisao com o jogador e notifica o `GameManager` que o nivel foi vencido |
| `TimerController.cs` | Contagem regressiva do nivel — duracao configuravel, dispara callback quando o tempo acaba (game over) |

### UI — Interface Visual

| Script | O que faz |
|---|---|
| `HUDController.cs` | Gerencia todos os elementos visuais: numero do nivel, estatisticas atuais de trafego (densidade/velocidade/clima), timer de contagem regressiva, proxima predicao e mensagens de resultado (vitoria/derrota). Usa TextMeshPro |

---

## Tecnologias

| Componente | Tecnologia | Versao |
|---|---|---|
| Game Engine | Unity 6 | 6000.1.14f1 |
| Linguagem (jogo) | C# | .NET Standard |
| Renderizacao | Universal Render Pipeline (URP) | 17.1.0 |
| Runtime (backend) | Node.js | 20 (Alpine) |
| Framework (backend) | Express.js | 4.21.0 |
| Container | Docker | - |

---

## Como o Backend se Conecta ao Jogo

```
┌──────────────────────────┐         HTTP GET          ┌──────────────────────────┐
│      Unity Game          │ ──────────────────────── │     Backend (Node.js)    │
│                          │    localhost:3000          │                          │
│  HttpTrafficDataProvider │ ◄──── JSON Response ──── │  Express server.js       │
│  faz GET /v1/traffic/    │                           │  responde em GET /       │
│  status                  │                           │  com dados aleatorios    │
└──────────────────────────┘                           └──────────────────────────┘
         │
         ▼
  TrafficResponseDto
         │
    ┌────┴────┐
    ▼         ▼
TrafficManager    PredictionScheduler
(aplica status)   (agenda mudancas futuras)
    │                    │
    ▼                    ▼
VehicleSpawner ← atualiza densidade, velocidade, clima → HUD / Player
```

**Fluxo:**
1. O `GameManager` inicia o nivel e solicita dados ao provider de trafego
2. O `HttpTrafficDataProvider` faz `GET http://localhost:3000/v1/traffic/status`
3. O backend retorna JSON com `current_status` + `predicted_status`
4. O `TrafficManager` aplica as condicoes atuais (spawner, jogador, HUD)
5. O `PredictionScheduler` agenda mudancas futuras com base nos `estimated_time`

---

## Passo a Passo para Executar

### 1. Clonar o Repositorio

```bash
git clone <url-do-repositorio>
cd EnviarProjeto
```

### 2. Subir o Backend

**Opcao A — Com Docker (recomendado):**

```bash
cd backend_game
docker compose up --build
```

**Opcao B — Com Node.js diretamente:**

```bash
cd backend_game
npm install
npm start
```

O servidor estara disponivel em `http://localhost:3000`.

Para testar, acesse no navegador ou execute:

```bash
curl http://localhost:3000
```

Voce devera ver um JSON com `current_status` e `predicted_status`.

### 3. Abrir o Projeto Unity

1. Abra o **Unity Hub**
2. Clique em **Add** e selecione a pasta `My project (1)/`
3. Abra o projeto com **Unity 6** (6000.1.14f1 ou compativel)

### 4. Configurar o Provider HTTP

1. Abra a cena `Assets/Scenes/SampleScene.unity`
2. Selecione o objeto **GameManager** na Hierarchy
3. No Inspector, no campo do provider de trafego, atribua o componente `HttpTrafficDataProvider`
4. Verifique que a URL aponta para `http://localhost:3000/v1/traffic/status`

> **Modo offline:** Para jogar sem o backend, use o `JsonTrafficDataProvider` com o JSON em `Assets/Mock/traffic_status_example.json`

### 5. Jogar

Clique em **Play** no Unity Editor.

| Tecla | Acao |
|---|---|
| `W` / `Seta Cima` | Mover para cima |
| `S` / `Seta Baixo` | Mover para baixo |
| `A` / `Seta Esquerda` | Mover para esquerda |
| `D` / `Seta Direita` | Mover para direita |
| `Espaco` | Reiniciar apos Game Over |

---

## Formato da API

**Endpoint:** `GET /` (porta 3000)

**Resposta:**

```json
{
  "current_status": {
    "vehicleDensity": 0.7,
    "averageSpeed": 62.0,
    "weather": "sunny"
  },
  "predicted_status": [
    {
      "estimated_time": 5000,
      "predictions": {
        "vehicleDensity": 0.8,
        "averageSpeed": 70.0,
        "weather": "clouded"
      }
    }
  ]
}
```

| Campo | Tipo | Faixa | Descricao |
|---|---|---|---|
| `vehicleDensity` | float | 0.1 - 1.0 | Densidade de veiculos na via |
| `averageSpeed` | float | 30 - 90 | Velocidade media em km/h |
| `weather` | string | — | `sunny`, `clouded`, `foggy`, `light rain`, `heavy rain`, `stormy` |
| `estimated_time` | int | — | Milissegundos ate a predicao entrar em vigor |

---