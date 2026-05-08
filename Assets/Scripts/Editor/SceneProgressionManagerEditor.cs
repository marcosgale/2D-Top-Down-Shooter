using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Editor tool to automatically sync SceneProgressionManager with Build Settings
/// </summary>
[CustomEditor(typeof(SceneProgressionManager))]
public class SceneProgressionManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SceneProgressionManager manager = (SceneProgressionManager)target;

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("Scene progression is managed here. The order matches your Build Settings.", MessageType.Info);

        GUILayout.Space(5);

        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("Sync with Build Settings", GUILayout.Height(35)))
        {
            SyncWithBuildSettings(manager);
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(5);

        if (GUILayout.Button("Open Build Settings", GUILayout.Height(30)))
        {
            EditorWindow.GetWindow(System.Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
        }

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Current Progression Info:", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Current Scene: {manager.CurrentSceneName}");
        EditorGUILayout.LabelField($"Next Scene: {manager.NextSceneName}");
        EditorGUILayout.LabelField($"Is Last Scene: {manager.IsLastScene}");
    }

    private void SyncWithBuildSettings(SceneProgressionManager manager)
    {
        List<string> sceneNames = new List<string>();

        // Get all scenes from Build Settings
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;

        if (buildScenes.Length == 0)
        {
            EditorUtility.DisplayDialog("No Scenes in Build Settings",
                "No scenes found in Build Settings! Add your scenes via File > Build Settings.",
                "OK");
            return;
        }

        foreach (EditorBuildSettingsScene scene in buildScenes)
        {
            if (scene.enabled)
            {
                // Extract scene name from path
                string scenePath = scene.path;
                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
                sceneNames.Add(sceneName);
            }
        }

        if (sceneNames.Count == 0)
        {
            EditorUtility.DisplayDialog("No Enabled Scenes",
                "No enabled scenes found in Build Settings!",
                "OK");
            return;
        }

        // Update the SceneProgressionManager's scene progression array
        SerializedObject so = new SerializedObject(manager);
        SerializedProperty sceneProgressionProp = so.FindProperty("_sceneProgression");

        if (sceneProgressionProp != null)
        {
            sceneProgressionProp.ClearArray();
            
            for (int i = 0; i < sceneNames.Count; i++)
            {
                sceneProgressionProp.InsertArrayElementAtIndex(i);
                sceneProgressionProp.GetArrayElementAtIndex(i).stringValue = sceneNames[i];
            }

            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(manager);

            Debug.Log($"SceneProgressionManager synced with Build Settings: {string.Join(" → ", sceneNames)}");
            
            EditorUtility.DisplayDialog("Sync Complete",
                $"Synced {sceneNames.Count} scene(s) from Build Settings:\n\n{string.Join("\n", sceneNames)}",
                "OK");
        }
        else
        {
            Debug.LogError("Could not find _sceneProgression field!");
        }
    }
}

/// <summary>
/// Menu item to quickly create SceneProgressionManager if it doesn't exist
/// </summary>
public class SceneProgressionHelper
{
    [MenuItem("Tools/Scene Management/Create Scene Progression Manager")]
    public static void CreateSceneProgressionManager()
    {
        SceneProgressionManager existing = Object.FindFirstObjectByType<SceneProgressionManager>();
        
        if (existing != null)
        {
            EditorUtility.DisplayDialog("Already Exists",
                "SceneProgressionManager already exists in this scene!",
                "OK");
            Selection.activeGameObject = existing.gameObject;
            return;
        }

        GameObject managerObj = new GameObject("SceneProgressionManager");
        managerObj.AddComponent<SceneProgressionManager>();
        
        Undo.RegisterCreatedObjectUndo(managerObj, "Create SceneProgressionManager");
        Selection.activeGameObject = managerObj;

        EditorUtility.DisplayDialog("Created",
            "SceneProgressionManager created! Don't forget to sync it with Build Settings.",
            "OK");
    }

    [MenuItem("Tools/Scene Management/Add Transition Trigger to Selected")]
    public static void AddTransitionTriggerToSelected()
    {
        if (Selection.activeGameObject == null)
        {
            EditorUtility.DisplayDialog("No Selection",
                "Please select a GameObject first!",
                "OK");
            return;
        }

        GameObject selected = Selection.activeGameObject;
        
        if (selected.GetComponent<SceneTransitionTrigger>() != null)
        {
            EditorUtility.DisplayDialog("Already Exists",
                "This GameObject already has a SceneTransitionTrigger!",
                "OK");
            return;
        }

        // Add the component
        Undo.AddComponent<SceneTransitionTrigger>(selected);
        
        // Make sure it has a collider
        if (selected.GetComponent<Collider2D>() == null)
        {
            BoxCollider2D collider = Undo.AddComponent<BoxCollider2D>(selected);
            collider.isTrigger = true;
        }

        EditorUtility.DisplayDialog("Added",
            "SceneTransitionTrigger added to " + selected.name + "!\n\n" +
            "It's already configured to use Scene Progression by default.",
            "OK");
    }
}
