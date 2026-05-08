using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Tool to fix tilemap rendering order so walls and floors render behind everything
/// </summary>
public class FixTilemapDepth : EditorWindow
{
    private int floorSortingOrder = -100;
    private int wallSortingOrder = -90;
    private string sortingLayerName = "Default";

    [MenuItem("Tools/Tilemap/Fix Tilemap Rendering Depth")]
    public static void ShowWindow()
    {
        FixTilemapDepth window = GetWindow<FixTilemapDepth>("Fix Tilemap Depth");
        window.minSize = new Vector2(400, 300);
        window.Show();
    }

    [MenuItem("Tools/Tilemap/Quick Fix Current Scene #&d")]
    public static void QuickFixCurrentScene()
    {
        FixTilemapDepth fixer = CreateInstance<FixTilemapDepth>();
        fixer.FixCurrentScene();
        DestroyImmediate(fixer);
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Fix Tilemap Rendering Depth", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Sets tilemaps to render behind everything while preserving collisions.", MessageType.Info);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Rendering Settings", EditorStyles.boldLabel);
        
        sortingLayerName = EditorGUILayout.TextField("Sorting Layer", sortingLayerName);
        floorSortingOrder = EditorGUILayout.IntField("Floor Order", floorSortingOrder);
        wallSortingOrder = EditorGUILayout.IntField("Wall Order", wallSortingOrder);

        EditorGUILayout.HelpBox("Negative values render behind (e.g., -100). Walls should be higher than floors.", MessageType.None);

        GUILayout.Space(20);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Fix Current Scene", GUILayout.Height(40)))
        {
            FixCurrentScene();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(10);

        if (GUILayout.Button("Fix All TestScenes (1-5)", GUILayout.Height(40)))
        {
            FixAllTestScenes();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Fix All Scenes in Build Settings", GUILayout.Height(30)))
        {
            FixAllBuildScenes();
        }

        GUILayout.Space(20);
        EditorGUILayout.HelpBox("Shortcut: Shift+Alt+D to fix current scene", MessageType.None);
    }

    public void FixCurrentScene()
    {
        Scene currentScene = EditorSceneManager.GetActiveScene();
        Debug.Log($"=== Fixing Tilemap Depth in {currentScene.name} ===");

        int fixedCount = FixSceneTilemaps();

        EditorSceneManager.MarkSceneDirty(currentScene);
        
        Debug.Log($"Fixed {fixedCount} tilemap renderer(s) in {currentScene.name}");
        EditorUtility.DisplayDialog("Complete", 
            $"Fixed {fixedCount} tilemap renderer(s) in {currentScene.name}", 
            "OK");
    }

    private void FixAllTestScenes()
    {
        // Get all scenes in build settings
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
        List<string> testScenePaths = new List<string>();

        foreach (EditorBuildSettingsScene buildScene in buildScenes)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(buildScene.path);
            if (sceneName.ToLower().StartsWith("testscene"))
            {
                // Check if it's TestScene1-5
                if (sceneName.Length > 9)
                {
                    char lastChar = sceneName[sceneName.Length - 1];
                    if (lastChar >= '1' && lastChar <= '5')
                    {
                        testScenePaths.Add(buildScene.path);
                    }
                }
                else if (sceneName.ToLower() == "testscene" || sceneName.ToLower() == "testscene1")
                {
                    testScenePaths.Add(buildScene.path);
                }
            }
        }

        if (testScenePaths.Count == 0)
        {
            // Try to find them in Assets
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene TestScene");
            foreach (string guid in sceneGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(path);
                
                if (sceneName.ToLower().StartsWith("testscene"))
                {
                    if (sceneName.Length > 9)
                    {
                        char lastChar = sceneName[sceneName.Length - 1];
                        if (lastChar >= '1' && lastChar <= '5')
                        {
                            testScenePaths.Add(path);
                        }
                    }
                    else
                    {
                        testScenePaths.Add(path);
                    }
                }
            }
        }

        if (testScenePaths.Count == 0)
        {
            EditorUtility.DisplayDialog("No Scenes Found", 
                "Could not find TestScene1-5 in Build Settings or Assets!", 
                "OK");
            return;
        }

        Scene originalScene = EditorSceneManager.GetActiveScene();
        string originalScenePath = originalScene.path;

        int totalFixed = 0;

        foreach (string scenePath in testScenePaths)
        {
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            Debug.Log($"Opening {sceneName}...");

            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
            int fixedCount = FixSceneTilemaps();
            totalFixed += fixedCount;

            EditorSceneManager.SaveScene(scene);
            Debug.Log($"Fixed {fixedCount} tilemap(s) in {sceneName}");
        }

        // Restore original scene
        if (!string.IsNullOrEmpty(originalScenePath))
        {
            EditorSceneManager.OpenScene(originalScenePath, OpenSceneMode.Single);
        }

        Debug.Log($"=== Completed: Fixed {totalFixed} tilemap(s) across {testScenePaths.Count} scene(s) ===");
        EditorUtility.DisplayDialog("Complete", 
            $"Fixed {totalFixed} tilemap renderer(s) across {testScenePaths.Count} TestScene(s)", 
            "OK");
    }

    private void FixAllBuildScenes()
    {
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
        
        if (buildScenes.Length == 0)
        {
            EditorUtility.DisplayDialog("No Scenes", 
                "No scenes in Build Settings!", 
                "OK");
            return;
        }

        Scene originalScene = EditorSceneManager.GetActiveScene();
        string originalScenePath = originalScene.path;

        int totalFixed = 0;
        int sceneCount = 0;

        foreach (EditorBuildSettingsScene buildScene in buildScenes)
        {
            if (!buildScene.enabled)
                continue;

            string sceneName = System.IO.Path.GetFileNameWithoutExtension(buildScene.path);
            Debug.Log($"Opening {sceneName}...");

            Scene scene = EditorSceneManager.OpenScene(buildScene.path, OpenSceneMode.Single);
            int fixedCount = FixSceneTilemaps();
            totalFixed += fixedCount;
            sceneCount++;

            EditorSceneManager.SaveScene(scene);
            Debug.Log($"Fixed {fixedCount} tilemap(s) in {sceneName}");
        }

        // Restore original scene
        if (!string.IsNullOrEmpty(originalScenePath))
        {
            EditorSceneManager.OpenScene(originalScenePath, OpenSceneMode.Single);
        }

        Debug.Log($"=== Completed: Fixed {totalFixed} tilemap(s) across {sceneCount} scene(s) ===");
        EditorUtility.DisplayDialog("Complete", 
            $"Fixed {totalFixed} tilemap renderer(s) across {sceneCount} scene(s)", 
            "OK");
    }

    private int FixSceneTilemaps()
    {
        int fixedCount = 0;

        // Find all TilemapRenderers
        TilemapRenderer[] renderers = FindObjectsByType<TilemapRenderer>(FindObjectsSortMode.None);

        foreach (TilemapRenderer renderer in renderers)
        {
            string name = renderer.gameObject.name.ToLower();
            
            Undo.RecordObject(renderer, "Fix Tilemap Depth");

            // Determine if it's a floor or wall
            if (name.Contains("ground") || name.Contains("floor") || name.Contains("walkable"))
            {
                renderer.sortingLayerName = sortingLayerName;
                renderer.sortingOrder = floorSortingOrder;
                Debug.Log($"Set {renderer.gameObject.name} to floor depth ({floorSortingOrder})");
                fixedCount++;
            }
            else if (name.Contains("wall") || name.Contains("obstacle") || name.Contains("collision"))
            {
                renderer.sortingLayerName = sortingLayerName;
                renderer.sortingOrder = wallSortingOrder;
                Debug.Log($"Set {renderer.gameObject.name} to wall depth ({wallSortingOrder})");
                fixedCount++;
            }
            else
            {
                // Default to floor depth for unknown tilemaps
                renderer.sortingLayerName = sortingLayerName;
                renderer.sortingOrder = floorSortingOrder;
                Debug.Log($"Set {renderer.gameObject.name} to default floor depth ({floorSortingOrder})");
                fixedCount++;
            }

            EditorUtility.SetDirty(renderer);
        }

        return fixedCount;
    }
}
