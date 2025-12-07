# 🚤 Setup do Barco de Pesca - Guia Completo

## 📋 Passo a Passo

### 1. Configurar o Barco no MapScene

#### A. Criar o GameObject do Barco
1. Abra **MapScene** no Unity
2. Localize o GameObject **"Boat"** na Hierarchy
3. Se não existir, crie: Right-click → 2D Object → Sprite
4. Renomeie para **"FishingBoat"**

#### B. Adicionar Componentes ao Barco
1. Selecione o GameObject **FishingBoat**
2. **Add Component → Box Collider 2D**
   - ✅ Marque **"Is Trigger"**
   - Ajuste o **Size** para cobrir a área de interação (ex: X: 3, Y: 2)
   - Ajuste o **Offset** se necessário

3. **Add Component → FishingBoat** (script criado)

#### C. Configurar o Script FishingBoat
No Inspector, com FishingBoat selecionado:

**UI Settings:**
- `Prompt Panel`: (configurar no passo 2)
- `Prompt Text`: (configurar no passo 2)
- `Prompt Message`: "Pressione F para pescar" (ou customize)

**Scene Settings:**
- `Fishing Scene Name`: "FishingScene" (ou o nome da sua cena de pesca)

---

### 2. Criar UI de Prompt (Texto "Pressione F")

#### Opção A: UI no Canvas Principal (Recomendado)

1. **Encontre ou crie um Canvas:**
   - Se já existe Canvas no MapScene, use-o
   - Se não: Right-click → UI → Canvas

2. **Criar o Painel de Prompt:**
   ```
   Canvas
   └── FishingPromptPanel (Panel)
       └── PromptText (TextMeshPro - Text)
   ```

3. **Configurar FishingPromptPanel:**
   - Add Component → Panel (ou Image)
   - Color: Preto com Alpha ~150 (semi-transparente)
   - Rect Transform:
     - Anchor: Bottom-Center
     - Pos Y: 150
     - Width: 400
     - Height: 80

4. **Configurar PromptText:**
   - Component: TextMeshPro - Text (UI)
   - Text: "Pressione F para pescar"
   - Font Size: 24
   - Alignment: Center + Middle
   - Color: Branco
   - Rect Transform: Stretch to fill parent

5. **Desativar o painel:**
   - No Inspector, **desmarque** o checkbox ao lado de "FishingPromptPanel"
   - Ou via script: Panel está desativado por padrão

#### Opção B: World Space Canvas (Sobre o Barco)

1. **Criar Canvas acima do barco:**
   - Right-click FishingBoat → UI → Canvas
   - Renomeie para "FishingPromptCanvas"

2. **Configurar Canvas:**
   - Render Mode: **World Space**
   - Rect Transform:
     - Width: 200
     - Height: 50
     - Pos Y: 1.5 (acima do barco)
   - Scale: X: 0.01, Y: 0.01, Z: 0.01

3. **Adicionar texto:**
   - Add Child → UI → Text - TextMeshPro
   - Configurar texto como acima

---

### 3. Linkar UI ao Script

1. Selecione **FishingBoat** na Hierarchy
2. No Inspector, encontre o componente **FishingBoat (Script)**
3. Arraste e solte:
   - `Prompt Panel` ← FishingPromptPanel
   - `Prompt Text` ← PromptText (dentro do panel)

---

### 4. Criar/Configurar a Cena de Pesca

#### Opção A: Criar Nova Cena

1. **File → New Scene**
2. Salve como **"FishingScene"** em `Assets/Scenes/`
3. Configure a cena de pesca:
   - Adicione GameManager (vazio ou prefab)
   - Adicione FishingMinigame (vazio com o script)
   - Adicione UI de pesca
   - Adicione botão "Voltar ao Mapa"

#### Opção B: Usar Cena Existente

1. Se já tem uma cena de pesca, anote o nome exato
2. No script FishingBoat, configure:
   - `Fishing Scene Name` = nome da sua cena

---

### 5. Adicionar Cena ao Build Settings

⚠️ **IMPORTANTE!** A cena precisa estar no Build Settings:

1. **File → Build Settings**
2. **Add Open Scenes** ou arraste `FishingScene` para a lista
3. Verifique que **MapScene** também está na lista
4. **Close**

---

### 6. Testar no Unity

1. **Play no MapScene**
2. **Mova o player até o barco**
3. **Verifique:**
   - ✅ Prompt "Pressione F" aparece?
   - ✅ Ao pressionar F, muda para FishingScene?
   - ✅ Posição do player é salva?

#### Debug:
- Abra **Console** (Ctrl+Shift+C)
- Veja mensagens:
  - "Player entrou na área do barco de pesca"
  - "Iniciando pesca - mudando para FishingScene"

---

## 🎨 Customizações Opcionais

### Mudar a Tecla de Interação

No script `FishingBoat.cs`, linha 31:
```csharp
if (playerInRange && !isInteracting && Input.GetKeyDown(KeyCode.F))
```
Troque `KeyCode.F` por:
- `KeyCode.E` → Tecla E
- `KeyCode.Space` → Barra de espaço
- `KeyCode.Return` → Enter

### Adicionar Som

1. Adicione `AudioSource` ao GameObject FishingBoat
2. No script, adicione:
```csharp
[SerializeField] private AudioClip interactSound;
private AudioSource audioSource;

void Start() {
    audioSource = GetComponent<AudioSource>();
}

void StartFishing() {
    if (audioSource != null && interactSound != null)
        audioSource.PlayOneShot(interactSound);
    // ... resto do código
}
```

### Animação do Prompt

Adicione um Animator ao PromptPanel com animação de fade in/out.

---

## ⚠️ Troubleshooting

### Prompt não aparece?
- ✅ Verifique se Player tem **Tag "Player"**
- ✅ BoxCollider2D está marcado como **Is Trigger**
- ✅ UI está linkada no Inspector
- ✅ Panel não está sendo ocultado por outro objeto

### Não muda de cena?
- ✅ Cena está no **Build Settings**?
- ✅ Nome da cena está correto (case-sensitive)?
- ✅ Veja Console para erros

### Player volta na posição errada?
- ✅ FishingScene precisa ter MapSceneLoader ou similar
- ✅ Verificar se `hasSpawnedOnce` está true

---

## 📝 Checklist Final

- [ ] GameObject Boat tem BoxCollider2D (Is Trigger)
- [ ] Script FishingBoat adicionado ao Boat
- [ ] UI de Prompt criada e linkada
- [ ] FishingScene existe e está no Build Settings
- [ ] Player tem tag "Player"
- [ ] Testado no Play Mode

---

## 🎮 Resultado Esperado

1. Player se aproxima do barco
2. Aparece texto: "Pressione F para pescar"
3. Player pressiona F
4. Salva posição automaticamente
5. Muda para cena de pesca
6. (Na cena de pesca, botão para voltar ao mapa)

**Pronto! Sistema de interação com barco funcionando!** 🚤✨
