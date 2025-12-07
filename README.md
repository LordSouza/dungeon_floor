# Dungeon Floor

Um jogo 2D de plataforma estilo dungeon crawler desenvolvido em Unity, com sistema de combate por turnos, progressão de nível e minigame de pesca.

## 🎮 Sobre o Jogo

Dungeon Floor é um jogo de ação e aventura onde o jogador explora masmorras, enfrenta inimigos em combates estratégicos por turnos e coleta recursos através de minigames. O jogo apresenta um sistema de progressão baseado em experiência, com inimigos que reaparecem dinamicamente.

### Principais Funcionalidades

- **Exploração 2D**: Movimentação de plataforma com double jump
- **Combate por Turnos**: Sistema de batalha com QTE (Quick Time Events)
- **Sistema de Progressão**: Ganho de XP e level up com milestones
- **Minigame de Pesca**: Pesque para obter itens de cura
- **Respawn de Inimigos**: Sistema dinâmico de reaparecimento
- **Persistência de Dados**: Save/load automático usando JSON

## 🚀 Como Rodar o Projeto

### Requisitos

- **Unity**: Versão 2022.3 ou superior (testado com Unity 6000.0.27f1)
- **Sistema Operacional**: Windows, macOS ou Linux
- **Espaço em Disco**: ~2GB para o projeto completo

### Passos para Instalação

1. **Clone o Repositório**
   ```bash
   git clone https://github.com/LordSouza/dungeon_floor.git
   cd dungeon_floor
   ```

2. **Abra no Unity Hub**
   - Abra o Unity Hub
   - Clique em "Add" → "Add project from disk"
   - Selecione a pasta `Dungeon Floor`
   - Aguarde a indexação dos assets

3. **Configure o Projeto**
   - O Unity irá importar automaticamente todos os pacotes necessários
   - Aguarde a compilação dos scripts (pode levar alguns minutos)

4. **Execute o Jogo**
   - Abra a cena `Assets/Scenes/Init.unity`
   - Pressione o botão **Play** (▶) no Unity Editor
   - Ou use `Ctrl + P` (Windows/Linux) / `Cmd + P` (macOS)

### Build do Jogo

1. Vá em **File → Build Settings**
2. Selecione a plataforma desejada (Windows, macOS, Linux)
3. Clique em **Build** e escolha a pasta de destino
4. Execute o arquivo gerado (.exe no Windows)

## 📁 Estrutura do Projeto

```
Dungeon Floor/
├── Assets/
│   ├── Scenes/              # Cenas do jogo
│   │   ├── Init.unity       # Cena de inicialização (boot)
│   │   ├── MainMenuScene.unity  # Menu principal
│   │   ├── MapScene.unity   # Mapa de exploração
│   │   ├── GameScene.unity  # Cena de batalha
│   │   ├── FishingScene.unity   # Minigame de pesca
│   │   └── DefeatScene.unity    # Tela de derrota
│   │
│   ├── Scripts/             # Código C# do jogo
│   │   ├── GameManager.cs   # Singleton de gerenciamento
│   │   ├── SaveData.cs      # Estrutura de dados persistentes
│   │   ├── Player.cs        # Controle do jogador
│   │   ├── Enemy.cs         # Comportamento dos inimigos
│   │   ├── BattleSystem.cs  # Lógica de combate
│   │   ├── Unit.cs          # Stats de personagens
│   │   ├── FishingMinigame.cs   # Minigame de pesca
│   │   ├── FishingBoat.cs   # Interação com barco
│   │   ├── MapSceneLoader.cs    # Carregamento do mapa
│   │   └── QteController.cs # Sistema QTE
│   │
│   ├── Prefabs/             # Prefabs reutilizáveis
│   │   ├── Player.prefab    # Prefab do jogador
│   │   ├── Mobs.prefab      # Inimigos variados
│   │   └── BattleHUD.prefab # HUD de batalha
│   │
│   ├── Sprites/             # Texturas e sprites
│   ├── Animations/          # Animações do Animator
│   ├── Audio/               # Sons e música
│   └── Tests/               # Testes unitários (138 testes)
│
├── ProjectSettings/         # Configurações do Unity
├── Packages/                # Pacotes do Unity
└── README.md                # Este arquivo
```

## 🎯 Fluxo de Cenas

```
Init.unity
    ↓
MainMenuScene.unity (Menu)
    ↓
MapScene.unity (Exploração)
    ↔ GameScene.unity (Batalha)
    ↔ FishingScene.unity (Pesca)
    ↓
DefeatScene.unity (Game Over)
```

## 🛠️ Arquitetura do Código

### GameManager (Singleton)

**Arquivo**: `Assets/Scripts/GameManager.cs`

- **Responsabilidade**: Gerenciamento central do jogo
- **Características**: 
  - `DontDestroyOnLoad` - persiste entre cenas
  - Salva/carrega dados em JSON (`Application.persistentDataPath + "/save.json"`)
  - Gerencia XP, level up e sistema de respawn

**Principais Métodos**:
```csharp
GameManager.Instance.Save()           // Salva o jogo
GameManager.Instance.Load()           // Carrega o jogo
GameManager.Instance.ResetSave()      // Reseta o progresso
```

### SaveData (Serialização)

**Arquivo**: `Assets/Scripts/SaveData.cs`

Estrutura de dados serializável que contém:
- **Stats do Player**: level, XP, HP, dano, posição
- **Inventário**: fishCount (número de peixes)
- **Sistema de Respawn**: enemyDeathRecords, totalSceneLoads
- **Flags**: justReturnedFromBattle (imunidade pós-batalha)

### Sistema de Combate

**Arquivo**: `Assets/Scripts/BattleSystem.cs`

- **Estados**: START → PLAYERTURN → ENEMYTURN → (WON|LOST)
- **Ações do Player**: Attack, Heal, Use Item (peixe)
- **IA do Inimigo**: 10% cura, 5% buff, 85% ataque
- **QTE**: Sistema de sequência de teclas para bônus

**Fórmula de XP**:
```csharp
XP necessário = 10 × level^1.2
XP ganho = enemyLevel × 5 × (multiplicador de diferença de nível)
```

### Sistema de Respawn

**Arquivo**: `Assets/Scripts/MapSceneLoader.cs`

- Inimigos reaparecem após `respawnAfterSceneLoads` transições (padrão: 1)
- Spawn em posições aleatórias do pool de spawn points
- 50% de chance de flip de direção
- Rastreamento por `enemyId` único

### Minigame de Pesca

**Arquivos**: 
- `Assets/Scripts/FishingMinigame.cs` - Lógica do minigame
- `Assets/Scripts/FishingBoat.cs` - Interação com barco
- `Assets/Scripts/FishingSceneController.cs` - Controlador da cena

**Mecânica**:
1. Player se aproxima do barco e pressiona `F`
2. Indicador se move na barra (0-1)
3. Zona verde randomizada (20% da barra)
4. Pressione `SPACE` ou `E` quando na zona verde
5. Sucesso = +1 peixe no inventário
6. Pesca contínua até pressionar `ESC` ou botão Exit

### Sistema de Imunidade

Após vencer uma batalha, o player fica imune a colisões com inimigos por `battleImmunityDuration` segundos (padrão: 2s).

**Fluxo**:
```
Vence batalha → justReturnedFromBattle = true
Volta ao mapa → Player.Start() ativa imunidade
2 segundos depois → imunidade desativa
```

## 🎨 Controles

### Exploração (MapScene)
- **Movimento**: Setas / A-D / Analógico esquerdo
- **Pulo**: Espaço / Botão Sul do controle
- **Double Jump**: Pressione pulo novamente no ar
- **Pescar**: F (próximo ao barco)

### Combate (GameScene)
- **Attack**: Clique no botão ou QTE
- **Heal**: Clique no botão ou QTE
- **Use Fish**: Clique no botão (cura 100% HP, não consome turno)

### Minigame de Pesca
- **Pescar**: Espaço / E (quando indicador na zona verde)
- **Sair**: ESC ou botão Exit

## 📊 Sistema de Progressão

### Leveling (v2.1 - Fast Progression)

```
Level 1: 10 XP (10 × 1^1.2)
Level 2: 23 XP (10 × 2^1.2)
Level 3: 36 XP (10 × 3^1.2)
Level 5: 61 XP (10 × 5^1.2)
Level 10: 158 XP (10 × 10^1.2)
```

### Ganhos por Level Up
- **HP**: +5 base + bônus de scaling
- **Dano**: +2 base + bônus de scaling
- **Milestones**: A cada 5 níveis (+10 HP, +3 dano)
- **Full Heal**: HP restaurado ao máximo

### XP Dinâmico
- Inimigos mais fortes: 120-200% XP
- Mesmo nível: 100% XP
- Inimigos mais fracos: 50-90% XP

## 🧪 Testes

O projeto inclui **138 testes unitários** divididos em:

```
Assets/Tests/Editor/
├── LevelingSystemTests.cs       (11 testes)
├── SaveDataTests.cs             (6 testes)
├── XPCalculationTests.cs        (5 testes)
├── GameManagerTests.cs          (4 testes)
├── PlayerPositionTests.cs       (6 testes)
├── UserCaseTests.cs             (27 testes)
├── FishingSystemTests.cs        (17 testes)
├── EnemyRespawnTests.cs         (16 testes)
├── BattleItemTests.cs           (15 testes)
└── IntegrationTests.cs          (11 testes)
```

**Rodar Testes**:
1. Window → General → Test Runner
2. Clique em "Run All" na aba EditMode

## 🐛 Troubleshooting

### Cena não carrega
- Verifique se todas as cenas estão em **Build Settings** (File → Build Settings)
- Ordem correta: Init, MainMenuScene, MapScene, GameScene, FishingScene, DefeatScene

### Erros de compilação
- Reimporte os assets: Right-click na pasta Assets → Reimport All
- Delete a pasta `Library/` e reabra o projeto

### Save não funciona
- Verifique permissões de escrita em `Application.persistentDataPath`
- Windows: `C:\Users\[user]\AppData\LocalLow\DefaultCompany\Dungeon Floor\`

### Player atravessa inimigos
- Verifique se o sistema de imunidade não está sempre ativo
- Confira o valor de `battleImmunityDuration` no Inspector do Player

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Add: MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto é um projeto educacional e está disponível para estudo e aprendizado.

## 🙏 Agradecimentos

- Assets de sprites e animações da comunidade Unity
