using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

/// <summary>
/// Editor tool for automatically setting up and baking NavMesh in scenes.
/// Access via: Tools > NavMesh > Auto Setup and Bake
/// </summary>
public class NavMeshAutoSetup : EditorWindow
{
    private string groundTag = "";
    private string[] groundKeywords = new string[] { "ground", "floor", "walkable" };
    private string wallTag = "";
    private string[] wallKeywords = new string[] { "wall", "obstacle", "collision" };
    private int agentTypeID = 0;
    private bool autoBake = true;
    private bool autoFillFloor = true;
    private TileBase floorTile = null;
    private Vector2 scrollPosition;

    [MenuItem("Tools/NavMesh/Auto Setup and Bake")]
    public static void ShowWindow()
    {
        NavMeshAutoSetup window = GetWindow<NavMeshAutoSetup>("NavMesh Auto Setup");
        window.minSize = new Vector2(400, 500);
        window.Show();
    }

    [MenuItem("Tools/NavMesh/Quick Setup Current Scene %#n")]
    public static void QuickSetupCurrentScene()
    {
        NavMeshAutoSetup setup = CreateInstance<NavMeshAutoSetup>();
        setup.SetupCurrentScene();
        DestroyImmediate(setup);
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("NavMesh Auto Setup Tool", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Automatically configures NavMesh components on tilemaps and bakes the NavMesh.", MessageType.Info);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Ground Tilemap Detection", EditorStyles.boldLabel);
        groundTag = EditorGUILayout.TextField("Ground Tag (Optional)", groundTag);
        
        EditorGUILayout.LabelField("Ground Keywords:");
        for (int i = 0; i < groundKeywords.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            groundKeywords[i] = EditorGUILayout.TextField(groundKeywords[i]);
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                ArrayUtility.RemoveAt(ref groundKeywords, i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add Ground Keyword"))
        {
            ArrayUtility.Add(ref groundKeywords, "");
        }

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Wall/Obstacle Tilemap Detection", EditorStyles.boldLabel);
        wallTag = EditorGUILayout.TextField("Wall Tag (Optional)", wallTag);
        
        EditorGUILayout.LabelField("Wall Keywords:");
        for (int i = 0; i < wallKeywords.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            wallKeywords[i] = EditorGUILayout.TextField(wallKeywords[i]);
            if (GUILayout.Button("X", GUILayout.Width(25)))
            {
                ArrayUtility.RemoveAt(ref wallKeywords, i);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("Add Wall Keyword"))
        {
            ArrayUtility.Add(ref wallKeywords, "");
        }

        GUILayout.Space(10);
        EditorGUILayout.LabelField("NavMesh Settings", EditorStyles.boldLabel);
        agentTypeID = EditorGUILayout.IntField("Agent Type ID", agentTypeID);
        autoBake = EditorGUILayout.Toggle("Auto-Bake After Setup", autoBake);

        GUILayout.Space(10);
        EditorGUILayout.LabelField("Floor Tile Settings", EditorStyles.boldLabel);
        autoFillFloor = EditorGUILayout.Toggle("Auto-Fill Floor Tiles", autoFillFloor);
        floorTile = (TileBase)EditorGUILayout.ObjectField("Floor Tile", floorTile, typeof(TileBase), false);
        
        if (autoFillFloor && floorTile == null)
        {
            EditorGUILayout.HelpBox("Select a floor tile to auto-fill enclosed areas!", MessageType.Warning);
        }

        GUILayout.Space(20);
        
        // Action buttons
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Setup and Bake Current Scene", GUILayout.Height(40)))
        {
            SetupCurrentScene();
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(10);
        
        if (GUILayout.Button("Setup Only (No Bake)", GUILayout.Height(30)))
        {
            bool previousAutoBake = autoBake;
            autoBake = false;
            SetupCurrentScene();
            autoBake = previousAutoBake;
        }

        GUILayout.Space(5);
        
        if (GUILayout.Button("Bake All NavMeshSurfaces", GUILayout.Height(30)))
        {
            BakeAllNavMeshSurfaces();
        }

        GUILayout.Space(5);
        
        GUI.backgroundColor = new Color(1f, 0.5f, 0.5f);
        if (GUILayout.Button("Clear All NavMesh Data", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Clear NavMesh", 
                "Are you sure you want to clear all NavMesh data in this scene?", 
                "Yes", "Cancel"))
            {
                ClearAllNavMeshData();
            }
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(10);
        
        GUI.backgroundColor = new Color(1f, 0.7f, 0.3f);
        if (GUILayout.Button("Remove All NavMesh Components", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Remove Components", 
                "Are you sure you want to remove all NavMesh components from this scene?", 
                "Yes", "Cancel"))
            {
                RemoveAllNavMeshComponents();
            }
        }
        GUI.backgroundColor = Color.white;

        GUILayout.Space(20);
        EditorGUILayout.HelpBox("Shortcut: Ctrl+Shift+N for quick setup", MessageType.None);

        EditorGUILayout.EndScrollView();
    }

    public void SetupCurrentScene()
    {
        Debug.Log("=== Starting NavMesh Auto Setup ===");

        // Find all tilemaps in the scene
        Tilemap[] allTilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);

        if (allTilemaps.Length == 0)
        {
            EditorUtility.DisplayDialog("No Tilemaps Found", 
                "No Tilemaps found in the current scene! Make sure you have Tilemaps for walls.", 
                "OK");
            Debug.LogWarning("No Tilemaps found in scene!");
            return;
        }

        int groundCount = 0;
        int wallCount = 0;
        List<Tilemap> wallTilemaps = new List<Tilemap>();

        Undo.SetCurrentGroupName("NavMesh Auto Setup");
        int undoGroup = Undo.GetCurrentGroup();

        foreach (Tilemap tilemap in allTilemaps)
        {
            // Check if this is a ground tilemap
            if (IsGroundTilemap(tilemap))
            {
                SetupGroundNavMesh(tilemap);
                groundCount++;
            }
            // Check if this is a wall tilemap
            else if (IsWallTilemap(tilemap))
            {
                SetupWallObstacle(tilemap);
                wallTilemaps.Add(tilemap);
                wallCount++;
            }
        }

        // If no ground tilemaps found, auto-generate a NavMesh surface based on walls
        if (groundCount == 0 && wallCount > 0)
        {
            // Auto-fill floor tiles if enabled and tile is assigned
            if (autoFillFloor && floorTile != null && wallTilemaps.Count > 0)
            {
                Tilemap groundTilemap = FillFloorTiles(wallTilemaps);
                if (groundTilemap != null)
                {
                    SetupGroundNavMesh(groundTilemap);
                    groundCount = 1;
                }
            }
            else
            {
                CreateAutoNavMeshSurface(wallTilemaps);
                groundCount = 1;
            }
        }

        Undo.CollapseUndoOperations(undoGroup);

        Debug.Log($"NavMesh setup complete! Configured {groundCount} ground tilemap(s) and {wallCount} wall tilemap(s)");

        // Mark scene as dirty
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        // Auto-bake
        if (autoBake)
        {
            BakeAllNavMeshSurfaces();
        }

        EditorUtility.DisplayDialog("NavMesh Setup Complete", 
            $"Configured {groundCount} ground tilemap(s) and {wallCount} wall tilemap(s).\n\n" +
            (autoBake ? "NavMesh has been baked." : "Don't forget to bake the NavMesh!"), 
            "OK");
    }

    private bool IsGroundTilemap(Tilemap tilemap)
    {
        // Check by tag first
        if (!string.IsNullOrEmpty(groundTag) && tilemap.CompareTag(groundTag))
            return true;

        // Check by name keywords
        string name = tilemap.name.ToLower();
        foreach (string keyword in groundKeywords)
        {
            if (!string.IsNullOrEmpty(keyword) && name.Contains(keyword.ToLower()))
                return true;
        }

        return false;
    }

    private bool IsWallTilemap(Tilemap tilemap)
    {
        // Check by tag first
        if (!string.IsNullOrEmpty(wallTag) && tilemap.CompareTag(wallTag))
            return true;

        // Check by name keywords
        string name = tilemap.name.ToLower();
        foreach (string keyword in wallKeywords)
        {
            if (!string.IsNullOrEmpty(keyword) && name.Contains(keyword.ToLower()))
                return true;
        }

        return false;
    }

    private void SetupGroundNavMesh(Tilemap tilemap)
    {
        GameObject obj = tilemap.gameObject;

        // Add NavMeshSurface if not present
        NavMeshSurface surface = obj.GetComponent<NavMeshSurface>();
        if (surface == null)
        {
            surface = Undo.AddComponent<NavMeshSurface>(obj);
            Debug.Log($"Added NavMeshSurface to: {obj.name}");
        }
        else
        {
            Undo.RecordObject(surface, "Update NavMeshSurface");
            Debug.Log($"NavMeshSurface already exists on: {obj.name}");
        }

        // Configure NavMeshSurface
        surface.agentTypeID = agentTypeID;
        surface.collectObjects = CollectObjects.Children;
        surface.useGeometry = NavMeshCollectGeometry.RenderMeshes;

        // NOTE: Ground tilemaps should NOT have colliders in top-down games
        // Only the NavMeshSurface is needed for pathfinding
        // The tilemap renderer is enough for NavMesh generation

        EditorUtility.SetDirty(obj);
    }

    private void SetupWallObstacle(Tilemap tilemap)
    {
        GameObject obj = tilemap.gameObject;

        // Add NavMeshModifier to mark as not walkable
        NavMeshModifier modifier = obj.GetComponent<NavMeshModifier>();
        if (modifier == null)
        {
            modifier = Undo.AddComponent<NavMeshModifier>(obj);
            Debug.Log($"Added NavMeshModifier to: {obj.name}");
        }
        else
        {
            Undo.RecordObject(modifier, "Update NavMeshModifier");
            Debug.Log($"NavMeshModifier already exists on: {obj.name}");
        }

        // Configure as not walkable
        modifier.overrideArea = true;
        modifier.area = 1; // 1 = Not Walkable

        // Make sure it has colliders
        TilemapCollider2D tilemapCollider = obj.GetComponent<TilemapCollider2D>();
        if (tilemapCollider == null)
        {
            tilemapCollider = Undo.AddComponent<TilemapCollider2D>(obj);
            Debug.Log($"Added TilemapCollider2D to: {obj.name}");
        }

        // Add composite collider for better performance
        CompositeCollider2D composite = obj.GetComponent<CompositeCollider2D>();
        if (composite == null)
        {
            composite = Undo.AddComponent<CompositeCollider2D>(obj);
            
            // Configure the tilemap collider to use the composite
            if (tilemapCollider != null)
            {
                Undo.RecordObject(tilemapCollider, "Update TilemapCollider2D");
                tilemapCollider.usedByComposite = true;
            }

            // Ensure rigidbody exists and is static
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = Undo.AddComponent<Rigidbody2D>(obj);
            }
            
            Undo.RecordObject(rb, "Update Rigidbody2D");
            rb.bodyType = RigidbodyType2D.Static;

            Debug.Log($"Added CompositeCollider2D to: {obj.name}");
        }

        EditorUtility.SetDirty(obj);
    }

    private void BakeAllNavMeshSurfaces()
    {
        NavMeshSurface[] surfaces = FindObjectsByType<NavMeshSurface>(FindObjectsSortMode.None);

        if (surfaces.Length == 0)
        {
            EditorUtility.DisplayDialog("No NavMeshSurfaces", 
                "No NavMeshSurface components found to bake!", 
                "OK");
            Debug.LogWarning("No NavMeshSurface components found to bake!");
            return;
        }

        EditorUtility.DisplayProgressBar("Baking NavMesh", "Baking NavMeshSurfaces...", 0f);

        for (int i = 0; i < surfaces.Length; i++)
        {
            EditorUtility.DisplayProgressBar("Baking NavMesh", 
                $"Baking {surfaces[i].gameObject.name} ({i + 1}/{surfaces.Length})", 
                (float)i / surfaces.Length);

            surfaces[i].BuildNavMesh();
            Debug.Log($"Baked NavMesh for: {surfaces[i].gameObject.name}");
        }

        EditorUtility.ClearProgressBar();
        Debug.Log($"Successfully baked {surfaces.Length} NavMeshSurface(s)");
    }

    private void ClearAllNavMeshData()
    {
        NavMeshSurface[] surfaces = FindObjectsByType<NavMeshSurface>(FindObjectsSortMode.None);

        if (surfaces.Length == 0)
        {
            EditorUtility.DisplayDialog("No NavMeshSurfaces", 
                "No NavMeshSurface components found!", 
                "OK");
            return;
        }

        foreach (NavMeshSurface surface in surfaces)
        {
            surface.RemoveData();
            Debug.Log($"Cleared NavMesh data for: {surface.gameObject.name}");
        }

        Debug.Log($"Cleared {surfaces.Length} NavMeshSurface(s)");
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
    }

    private void CreateAutoNavMeshSurface(List<Tilemap> wallTilemaps)
    {
        Debug.Log("No ground tilemap found. Creating automatic NavMesh surface based on room bounds...");

        // Calculate the bounds that encompass all wall tilemaps
        Bounds totalBounds = new Bounds();
        bool boundsInitialized = false;

        foreach (Tilemap wallTilemap in wallTilemaps)
        {
            Bounds wallBounds = wallTilemap.localBounds;
            Vector3 worldCenter = wallTilemap.transform.TransformPoint(wallBounds.center);
            Bounds worldBounds = new Bounds(worldCenter, wallBounds.size);

            if (!boundsInitialized)
            {
                totalBounds = worldBounds;
                boundsInitialized = true;
            }
            else
            {
                totalBounds.Encapsulate(worldBounds);
            }
        }

        if (!boundsInitialized)
        {
            Debug.LogWarning("Could not calculate room bounds!");
            return;
        }

        // Expand bounds slightly to ensure coverage
        totalBounds.Expand(2f);

        // Create a new GameObject for the NavMesh surface
        GameObject navMeshObj = new GameObject("NavMesh Surface (Auto-Generated)");
        Undo.RegisterCreatedObjectUndo(navMeshObj, "Create NavMesh Surface");

        // Position it at the center of the room
        navMeshObj.transform.position = new Vector3(totalBounds.center.x, totalBounds.center.y, 0);

        // Add NavMeshSurface component
        NavMeshSurface surface = Undo.AddComponent<NavMeshSurface>(navMeshObj);
        surface.agentTypeID = agentTypeID;
        surface.collectObjects = CollectObjects.All;
        surface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;

        // Add a large box collider to define the walkable area
        BoxCollider2D boxCollider = Undo.AddComponent<BoxCollider2D>(navMeshObj);
        boxCollider.size = new Vector2(totalBounds.size.x, totalBounds.size.y);
        boxCollider.isTrigger = true; // Make it a trigger so it doesn't collide with anything

        // Add a visual indicator (optional, can be removed in play mode)
        SpriteRenderer spriteRenderer = Undo.AddComponent<SpriteRenderer>(navMeshObj);
        spriteRenderer.color = new Color(0f, 1f, 0f, 0.1f); // Very transparent green
        spriteRenderer.sortingOrder = -100; // Render behind everything
        spriteRenderer.drawMode = SpriteDrawMode.Sliced;

        // Create a simple sprite for visualization (1x1 white square)
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        spriteRenderer.sprite = sprite;
        spriteRenderer.size = boxCollider.size;

        EditorUtility.SetDirty(navMeshObj);

        Debug.Log($"Created auto NavMesh surface: {totalBounds.size.x:F1} x {totalBounds.size.y:F1} at {totalBounds.center}");
    }

    private void RemoveAllNavMeshComponents()
    {
        int removedCount = 0;

        // Remove NavMeshSurfaces
        NavMeshSurface[] surfaces = FindObjectsByType<NavMeshSurface>(FindObjectsSortMode.None);
        foreach (NavMeshSurface surface in surfaces)
        {
            Undo.DestroyObjectImmediate(surface);
            removedCount++;
        }

        // Remove NavMeshModifiers
        NavMeshModifier[] modifiers = FindObjectsByType<NavMeshModifier>(FindObjectsSortMode.None);
        foreach (NavMeshModifier modifier in modifiers)
        {
            Undo.DestroyObjectImmediate(modifier);
            removedCount++;
        }

        Debug.Log($"Removed {removedCount} NavMesh component(s)");
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        
        EditorUtility.DisplayDialog("Components Removed", 
            $"Removed {removedCount} NavMesh component(s) from the scene.", 
            "OK");
    }

    private Tilemap FillFloorTiles(List<Tilemap> wallTilemaps)
    {
        Debug.Log("Auto-filling floor tiles within wall boundaries...");

        // Find or create a ground tilemap
        Tilemap groundTilemap = null;
        Tilemap[] allTilemaps = FindObjectsByType<Tilemap>(FindObjectsSortMode.None);
        
        foreach (Tilemap tm in allTilemaps)
        {
            if (IsGroundTilemap(tm))
            {
                groundTilemap = tm;
                break;
            }
        }

        // If no ground tilemap exists, create one
        if (groundTilemap == null)
        {
            // Find the Grid parent
            Grid grid = FindFirstObjectByType<Grid>();
            if (grid == null)
            {
                Debug.LogError("No Grid found in scene! Cannot create ground tilemap.");
                return null;
            }

            GameObject groundObj = new GameObject("Ground");
            Undo.RegisterCreatedObjectUndo(groundObj, "Create Ground Tilemap");
            groundObj.transform.SetParent(grid.transform);
            groundObj.transform.localPosition = Vector3.zero;
            
            groundTilemap = Undo.AddComponent<Tilemap>(groundObj);
            TilemapRenderer renderer = Undo.AddComponent<TilemapRenderer>(groundObj);
            renderer.sortingOrder = -1; // Render below everything else

            Debug.Log("Created new Ground tilemap");
        }

        // Get bounds of all walls combined
        BoundsInt totalBounds = new BoundsInt();
        bool boundsInitialized = false;

        foreach (Tilemap wallTilemap in wallTilemaps)
        {
            wallTilemap.CompressBounds();
            BoundsInt wallBounds = wallTilemap.cellBounds;
            
            if (!boundsInitialized)
            {
                totalBounds = wallBounds;
                boundsInitialized = true;
            }
            else
            {
                totalBounds.xMin = Mathf.Min(totalBounds.xMin, wallBounds.xMin);
                totalBounds.yMin = Mathf.Min(totalBounds.yMin, wallBounds.yMin);
                totalBounds.xMax = Mathf.Max(totalBounds.xMax, wallBounds.xMax);
                totalBounds.yMax = Mathf.Max(totalBounds.yMax, wallBounds.yMax);
            }
        }

        if (!boundsInitialized)
        {
            Debug.LogWarning("Could not calculate wall bounds!");
            return null;
        }

        // Expand bounds slightly for flood fill
        totalBounds.xMin -= 1;
        totalBounds.yMin -= 1;
        totalBounds.xMax += 1;
        totalBounds.yMax += 1;

        // Perform flood fill from the center
        Vector3Int centerCell = new Vector3Int(
            (totalBounds.xMin + totalBounds.xMax) / 2,
            (totalBounds.yMin + totalBounds.yMax) / 2,
            0
        );

        // Make sure center is not a wall
        bool centerIsWall = false;
        foreach (Tilemap wallTilemap in wallTilemaps)
        {
            if (wallTilemap.HasTile(centerCell))
            {
                centerIsWall = true;
                break;
            }
        }

        if (centerIsWall)
        {
            // Find a non-wall cell to start from
            bool foundStart = false;
            for (int y = totalBounds.yMin; y < totalBounds.yMax && !foundStart; y++)
            {
                for (int x = totalBounds.xMin; x < totalBounds.xMax && !foundStart; x++)
                {
                    Vector3Int testCell = new Vector3Int(x, y, 0);
                    bool isWall = false;
                    
                    foreach (Tilemap wallTilemap in wallTilemaps)
                    {
                        if (wallTilemap.HasTile(testCell))
                        {
                            isWall = true;
                            break;
                        }
                    }

                    if (!isWall)
                    {
                        centerCell = testCell;
                        foundStart = true;
                    }
                }
            }

            if (!foundStart)
            {
                Debug.LogWarning("Could not find a starting point for flood fill!");
                return null;
            }
        }

        // Perform flood fill
        HashSet<Vector3Int> filledCells = new HashSet<Vector3Int>();
        Queue<Vector3Int> queue = new Queue<Vector3Int>();
        queue.Enqueue(centerCell);
        filledCells.Add(centerCell);

        int maxCells = 10000; // Safety limit
        int cellCount = 0;

        while (queue.Count > 0 && cellCount < maxCells)
        {
            Vector3Int current = queue.Dequeue();
            cellCount++;

            // Check 4 neighbors
            Vector3Int[] neighbors = new Vector3Int[]
            {
                current + Vector3Int.up,
                current + Vector3Int.down,
                current + Vector3Int.left,
                current + Vector3Int.right
            };

            foreach (Vector3Int neighbor in neighbors)
            {
                // Skip if out of bounds
                if (neighbor.x < totalBounds.xMin || neighbor.x >= totalBounds.xMax ||
                    neighbor.y < totalBounds.yMin || neighbor.y >= totalBounds.yMax)
                    continue;

                // Skip if already filled
                if (filledCells.Contains(neighbor))
                    continue;

                // Skip if it's a wall
                bool isWall = false;
                foreach (Tilemap wallTilemap in wallTilemaps)
                {
                    if (wallTilemap.HasTile(neighbor))
                    {
                        isWall = true;
                        break;
                    }
                }

                if (!isWall)
                {
                    filledCells.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        // Apply the floor tiles
        Undo.RecordObject(groundTilemap, "Fill Floor Tiles");
        
        EditorUtility.DisplayProgressBar("Filling Floor", "Placing floor tiles...", 0f);
        
        int tileIndex = 0;
        foreach (Vector3Int cell in filledCells)
        {
            groundTilemap.SetTile(cell, floorTile);
            
            if (tileIndex % 100 == 0)
            {
                float progress = (float)tileIndex / filledCells.Count;
                EditorUtility.DisplayProgressBar("Filling Floor", $"Placing floor tiles... {tileIndex}/{filledCells.Count}", progress);
            }
            tileIndex++;
        }

        EditorUtility.ClearProgressBar();
        
        EditorUtility.SetDirty(groundTilemap);
        Debug.Log($"Filled {filledCells.Count} floor tiles within wall boundaries");

        return groundTilemap;
    }
}
