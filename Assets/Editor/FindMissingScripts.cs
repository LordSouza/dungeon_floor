using UnityEngine;
using UnityEditor;

/// <summary>
/// Ferramenta para encontrar e remover scripts quebrados (Missing Scripts) na cena
/// </summary>
public class FindMissingScripts : EditorWindow
{
    private int missingCount = 0;
    private string resultMessage = "";

    [MenuItem("Tools/Find Missing Scripts")]
    static void ShowWindow()
    {
        GetWindow<FindMissingScripts>("Find Missing Scripts");
    }

    void OnGUI()
    {
        GUILayout.Label("Encontrar Scripts Quebrados", EditorStyles.boldLabel);
        GUILayout.Space(10);

        if (GUILayout.Button("Procurar na Cena Atual", GUILayout.Height(30)))
        {
            FindInCurrentScene();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Procurar em Todas as Cenas", GUILayout.Height(30)))
        {
            FindInAllScenes();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Remover Scripts Quebrados da Cena Atual", GUILayout.Height(30)))
        {
            RemoveMissingScriptsInCurrentScene();
        }

        GUILayout.Space(20);
        
        if (!string.IsNullOrEmpty(resultMessage))
        {
            EditorGUILayout.HelpBox(resultMessage, MessageType.Info);
        }
    }

    void FindInCurrentScene()
    {
        missingCount = 0;
        resultMessage = "Procurando...\n\n";

        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject go in allObjects)
        {
            Component[] components = go.GetComponents<Component>();
            
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    missingCount++;
                    string path = GetGameObjectPath(go);
                    resultMessage += $"❌ Missing script em: {path}\n";
                    Debug.LogError($"Missing script em GameObject: {path}", go);
                }
            }
        }

        if (missingCount == 0)
        {
            resultMessage = "✅ Nenhum script quebrado encontrado!";
            Debug.Log("Nenhum script quebrado encontrado na cena atual.");
        }
        else
        {
            resultMessage += $"\n⚠️ Total: {missingCount} scripts quebrados encontrados!";
            Debug.LogWarning($"Total de {missingCount} scripts quebrados encontrados!");
        }
    }

    void FindInAllScenes()
    {
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;
        string[] scenePaths = System.IO.Directory.GetFiles("Assets/Scenes", "*.unity", System.IO.SearchOption.AllDirectories);
        
        resultMessage = "Procurando em todas as cenas...\n\n";
        int totalMissing = 0;

        foreach (string scenePath in scenePaths)
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);
            
            GameObject[] allObjects = FindObjectsOfType<GameObject>();
            int sceneCount = 0;
            
            foreach (GameObject go in allObjects)
            {
                Component[] components = go.GetComponents<Component>();
                
                for (int i = 0; i < components.Length; i++)
                {
                    if (components[i] == null)
                    {
                        sceneCount++;
                        totalMissing++;
                    }
                }
            }

            if (sceneCount > 0)
            {
                resultMessage += $"⚠️ {System.IO.Path.GetFileName(scenePath)}: {sceneCount} missing\n";
            }
        }

        // Voltar para cena original
        if (!string.IsNullOrEmpty(currentScene))
        {
            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(currentScene);
        }

        if (totalMissing == 0)
        {
            resultMessage = "✅ Nenhum script quebrado encontrado em nenhuma cena!";
        }
        else
        {
            resultMessage += $"\n⚠️ Total geral: {totalMissing} scripts quebrados!";
        }
    }

    void RemoveMissingScriptsInCurrentScene()
    {
        int removedCount = 0;
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        
        foreach (GameObject go in allObjects)
        {
            // Usar SerializedObject para remover componentes nulos
            SerializedObject so = new SerializedObject(go);
            SerializedProperty sp = so.FindProperty("m_Component");

            for (int i = sp.arraySize - 1; i >= 0; i--)
            {
                SerializedProperty component = sp.GetArrayElementAtIndex(i);
                
                if (component.objectReferenceValue == null)
                {
                    sp.DeleteArrayElementAtIndex(i);
                    removedCount++;
                    string path = GetGameObjectPath(go);
                    Debug.Log($"Removido script quebrado de: {path}", go);
                }
            }
            
            so.ApplyModifiedProperties();
        }

        resultMessage = removedCount > 0 
            ? $"✅ {removedCount} scripts quebrados foram removidos!" 
            : "✅ Nenhum script quebrado para remover.";
        
        Debug.Log(resultMessage);
        
        // Marcar cena como modificada
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene()
        );
    }

    string GetGameObjectPath(GameObject obj)
    {
        string path = obj.name;
        Transform parent = obj.transform.parent;
        
        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }
        
        return path;
    }
}
