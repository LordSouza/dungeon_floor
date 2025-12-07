# FishingScene - Setup Rápido

## 🎣 Problema: Cena carrega mas nada acontece

**Solução:** Adicionar FishingSceneController para iniciar automaticamente!

---

## ✅ Setup na FishingScene (Unity)

### 1️⃣ Adicionar FishingSceneController

**Na FishingScene:**

1. Selecione o GameObject **FishingManager** (ou crie um novo vazio)
2. **Add Component** → **FishingSceneController**
3. No Inspector:
   - **Fishing Minigame**: Arraste o componente FishingMinigame do mesmo GameObject

### 2️⃣ Verificar FishingMinigame está configurado

**No FishingManager:**

Certifique-se que o componente **FishingMinigame** tem todas as referências:

- ✅ **Fishing UI**: GameObject do painel/canvas (deve iniciar DESATIVADO)
- ✅ **Progress Slider**: O slider que mostra o indicador
- ✅ **Success Zone**: A área verde (RectTransform)
- ✅ **Instruction Text**: Texto de instruções
- ✅ **Result Text**: Texto de resultado (sucesso/falha)

**Configurações:**
- **Indicator Speed**: 2 (ajuste a dificuldade)
- **Success Zone Size**: 0.2 (20% da barra)
- **Fish Heal Amount**: 12

### 3️⃣ Estrutura mínima necessária

```
FishingScene (Hierarchy)
├── Camera
├── Background (sprite)
├── FishingManager
│   ├── FishingMinigame (script)
│   └── FishingSceneController (script) ← NOVO!
└── Canvas
    └── FishingPanel (GameObject - inicialmente ATIVO para testes)
        ├── ProgressSlider (Slider)
        │   └── SuccessZone (Image verde)
        ├── InstructionText (TextMeshPro)
        └── ResultText (TextMeshPro)
```

---

## 🔧 Configuração Rápida do Canvas

Se você ainda não tem a UI criada:

### Canvas Simples:

1. **Create → UI → Canvas**
2. Dentro do Canvas, criar:
   - **Panel** (renomear para FishingPanel)
   - Dentro do Panel:
     - **Slider** (renomear para ProgressSlider)
     - **Text - TextMeshPro** (InstructionText)
     - **Text - TextMeshPro** (ResultText)

### Slider:

1. Selecione o Slider
2. Configure:
   - Min Value: 0
   - Max Value: 1
   - Value: 0
   - **Interactable**: DESMARCADO

### Success Zone (zona verde):

1. **Right-click no ProgressSlider** → UI → Image
2. Renomear para **SuccessZone**
3. **Color**: Verde (#00FF00) com 50% alpha
4. **Anchor Presets**: Clique no quadrado de ancoragem
   - Segure **Alt+Shift** e clique no canto inferior esquerdo (stretch)
5. **RectTransform**:
   - Left: 0, Right: 0, Top: 0, Bottom: 0

---

## 🎮 Como Funciona Agora

1. **Player interage com FishingBoat** → Salva posição → Carrega FishingScene
2. **FishingScene carrega** → FishingSceneController.Start() → fishingMinigame.StartFishing()
3. **Minigame inicia automaticamente** → Indicador se move
4. **Player aperta SPACE ou E** → Tenta pegar peixe
5. **Sucesso/Falha** → Mostra resultado por 2 segundos
6. **Automaticamente volta para MapScene** → Restaura posição do player

---

## 🐛 Troubleshooting

### "Nada acontece ao carregar a cena"

✅ **Verifique:**
- FishingSceneController está no GameObject FishingManager?
- Campo "Fishing Minigame" está preenchido?
- FishingMinigame tem todas as referências configuradas?
- FishingUI está inicialmente DESATIVADO no editor?

### "Não vejo o indicador se movendo"

✅ **Verifique:**
- Progress Slider está configurado (Min=0, Max=1)?
- Indicator Speed > 0 no FishingMinigame?
- A UI está visível (não está atrás de outro elemento)?

### "Não volta para o MapScene"

✅ **Verifique:**
- MapScene está no Build Settings (File → Build Settings)?
- Nome da cena é exatamente "MapScene"?

### "Erro NullReferenceException"

✅ **Cause mais comum:**
- Alguma referência não foi atribuída no Inspector
- Abra o Console (Ctrl+Shift+C) para ver qual campo é null

---

## 🎯 Checklist Rápido

**Scripts** (já criados):
- [x] FishingMinigame.cs
- [x] FishingSceneController.cs (NOVO!)
- [x] FishingBoat.cs
- [x] SaveData.cs

**Unity Setup**:
- [ ] FishingScene existe
- [ ] Canvas com FishingPanel criado
- [ ] ProgressSlider configurado
- [ ] SuccessZone (Image verde) criada
- [ ] InstructionText e ResultText criados
- [ ] FishingManager GameObject criado
- [ ] FishingMinigame component adicionado
- [ ] **FishingSceneController component adicionado** ← IMPORTANTE!
- [ ] Todas as referências atribuídas
- [ ] FishingScene está no Build Settings

---

## 🚀 Teste Rápido

1. **Abra FishingScene diretamente no Unity**
2. **Clique Play**
3. **Você deve ver:**
   - UI aparece automaticamente
   - Indicador se movendo no slider
   - Zona verde visível
   - Texto de instrução
4. **Aperte SPACE**
   - Se acertar: "SUCCESS! You caught a fish!"
   - Se errar: "The fish got away..."
5. **Após 2 segundos:** Volta para MapScene

Se funcionar aqui, vai funcionar no fluxo completo! 🎣
