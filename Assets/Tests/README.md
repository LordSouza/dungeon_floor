# Unit Tests - Dungeon Floor

## 📋 Visão Geral

Implementação completa de unit tests para os sistemas principais do jogo:
- Sistema de Level Up e XP
- Cálculos de recompensa de XP
- Sistema de Save/Load
- GameManager

## 🧪 Testes Implementados

### 1. LevelingSystemTests.cs (11 testes)
- ✅ Cálculo de XP necessário para diferentes níveis
- ✅ Ganho de XP e level up
- ✅ Level up com múltiplos níveis de uma vez
- ✅ Bônus de milestone a cada 5 níveis
- ✅ Full heal ao subir de nível
- ✅ Inicialização de inimigos com scaling correto
- ✅ Sistema de dano e morte
- ✅ Sistema de cura com limite de HP máximo

### 2. SaveDataTests.cs (6 testes)
- ✅ Inicialização de EnemyDeathRecord
- ✅ Inicialização de EnemySpawnPoint
- ✅ Valores padrão do SaveData
- ✅ Adicionar e buscar registros de morte
- ✅ Gerenciamento de spawn points

### 3. XPCalculationTests.cs (5 testes)
- ✅ Cálculo de XP para inimigo de mesmo nível (100%)
- ✅ Bônus de XP para inimigos mais fortes (120-200%+)
- ✅ Redução de XP para inimigos mais fracos (50-80%)
- ✅ XP mínimo para inimigos muito mais fracos
- ✅ Validação de XP sempre positivo

### 4. GameManagerTests.cs (4 testes)
- ✅ Reset do save com valores padrão
- ✅ Cálculo de XP consistente com Unit.cs
- ✅ Sistema de marcação de inimigos mortos
- ✅ Prevenção de duplicatas na lista de mortos

## 🎮 Como Executar os Testes

### No Unity Editor:

1. **Abrir Test Runner**
   - Menu: `Window → General → Test Runner`
   - Atalho: `Ctrl+Alt+T`

2. **Visualizar Testes**
   - Clique na aba **EditMode**
   - Você verá todos os 26 testes organizados por classe

3. **Executar Testes**
   - **Todos**: Clique em "Run All"
   - **Por classe**: Clique no nome da classe e "Run Selected"
   - **Individual**: Clique no teste específico e "Run Selected"

4. **Interpretar Resultados**
   - ✅ Verde = Passou
   - ❌ Vermelho = Falhou (clique para ver detalhes)
   - ⏱️ Tempo de execução exibido ao lado

### Via Linha de Comando:

```bash
# Windows
Unity.exe -runTests -batchmode -projectPath "C:\Users\lucas\Dungeon Floor" -testResults results.xml

# Verificar resultados
type results.xml
```

## 📊 Cobertura de Testes

| Sistema | Cobertura | Testes |
|---------|-----------|--------|
| Unit.cs (Leveling) | ~90% | 11 testes |
| SaveData.cs | ~80% | 6 testes |
| BattleSystem.cs (XP) | ~70% | 5 testes |
| GameManager.cs | ~60% | 4 testes |

## 🔍 Exemplos de Testes

### Teste de Level Up com Milestone
```csharp
[Test]
public void LevelUp_AtLevel5_AppliesMilestoneBonus()
{
    // Arrange
    Unit unit = CreateUnit();
    unit.unitLevel = 4;
    unit.xpToNextLevel = 52;
    
    // Act
    unit.GainXP(52);
    
    // Assert
    Assert.AreEqual(5, unit.unitLevel);
    Assert.AreEqual(hpBefore + 16, unit.MaxHp); // +5 base +1 scaling +10 milestone
}
```

### Teste de Cálculo de XP Dinâmico
```csharp
[Test]
public void CalculateXPReward_EnemyHigherLevel_GivesBonusXP()
{
    // Player level 5, Enemy level 8 (+3 difference)
    int xp = battleSystem.CalculateXPReward(5, 8);
    
    // Expected: 40 * 1.8 = 72 (base 40, multiplier 1.8)
    Assert.AreEqual(72, xp);
}
```

## 🐛 Debugging Testes

Se um teste falhar:

1. **Verificar mensagem de erro** no Test Runner
2. **Executar apenas esse teste** para isolar o problema
3. **Usar Debug.Log** dentro do teste para investigar
4. **Verificar valores esperados** vs valores reais no Assert

## ⚙️ Configuração Automática

Os testes usam:
- **NUnit Framework** (incluído no Unity)
- **Reflection** para testar métodos privados
- **Setup/Teardown** para limpeza automática
- **DestroyImmediate** para cleanup de GameObjects

## 📝 Adicionar Novos Testes

1. Crie arquivo em `Assets/Tests/Editor/`
2. Use namespace `Tests`
3. Adicione `[Test]` antes de cada método de teste
4. Use padrão **Arrange-Act-Assert**:

```csharp
[Test]
public void MeuNovoTeste()
{
    // Arrange - Preparar dados
    var data = new SaveData();
    
    // Act - Executar ação
    data.fishCount = 5;
    
    // Assert - Verificar resultado
    Assert.AreEqual(5, data.fishCount);
}
```

## 🎯 Boas Práticas

✅ **Testes independentes** - Não dependem de ordem de execução
✅ **Cleanup automático** - DestroyImmediate de GameObjects
✅ **Nomes descritivos** - `NomeDoMetodo_Condicao_ResultadoEsperado`
✅ **One Assert per Test** - Foco em um comportamento específico
✅ **Fast Tests** - Evitar delays e operações lentas

## 📚 Referências

- [Unity Test Framework](https://docs.unity3d.com/Packages/com.unity.test-framework@latest)
- [NUnit Documentation](https://docs.nunit.org/)
- [Test-Driven Development](https://unity.com/how-to/unity-test-framework-tdd)
