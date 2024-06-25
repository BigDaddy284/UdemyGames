using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ObjectCreationTool : EditorWindow
{
    bool isEnabled = false;  // Flag to check if the tool is enabled
    Vector3 lastSelectedObjectCoordinates = Vector3.zero;  // Stores the coordinates of the last selected object
    Vector3 adjustmentValues = Vector3.zero;  // Stores the adjustment values for object creation
    GameObject lastSelectedObject;  // Stores the reference to the last selected object

    PrimitiveType selectedPrimitiveType = PrimitiveType.Cube;  // Default primitive type to create
    int selectedPrefabIndex = 0;  // Index of the selected prefab in the dropdown
    int selectedAudioIndex = 0;  // Index of the selected audio prefab in the dropdown
    int selectedVFXIndex = 0;  // Index of the selected VFX prefab in the dropdown
    int selectedEffectIndex = 0;  // Index of the selected effect in the dropdown
    
    List<GameObject> prefabs = new List<GameObject>();  // List to store loaded prefabs
    List<GameObject> audioPrefabs = new List<GameObject>();  // List to store loaded audio prefabs
    List<GameObject> vfxPrefabs = new List<GameObject>();  // List to store loaded VFX prefabs
    List<string> objFiles = new List<string>();  // List to store loaded OBJ files

    string[] prefabNames;  // Array to store prefab names for the dropdown
    string[] audioPrefabNames;  // Array to store audio prefab names for the dropdown
    string[] vfxPrefabNames;  // Array to store VFX prefab names for the dropdown
    string[] objFileNames;  // Array to store OBJ file names for the dropdown
    string[] effectNames = { "Particle System", "Trail Renderer", "Line Renderer" };  // Array for effect names

    [MenuItem("Tools/Object Creation Tool")]
    public static void ShowWindow()
    {
        GetWindow<ObjectCreationTool>("Object Creation Tool");
    }

    void OnEnable()
    {
        LoadPrefabs();
        LoadAudioPrefabs();
        LoadVFXPrefabs();
        LoadOBJFiles();

        // Subscribe to events for project changes, assembly reloads, and selection changes
        EditorApplication.projectChanged += OnProjectChange;
        AssemblyReloadEvents.afterAssemblyReload += OnAssemblyReload;
        Selection.selectionChanged += OnSelectionChange;
    }

    void OnDisable()
    {
        // Unsubscribe from events when the window is disabled
        EditorApplication.projectChanged -= OnProjectChange;
        AssemblyReloadEvents.afterAssemblyReload -= OnAssemblyReload;
        Selection.selectionChanged -= OnSelectionChange;
    }

    void OnGUI()
    {
        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button((isEnabled ? "Disable" : "Enable"), GUILayout.Width(150)))
        {
            isEnabled = !isEnabled;
            if (!isEnabled)
            {
                // Reset coordinates and adjustment values when the tool is disabled
                lastSelectedObjectCoordinates = Vector3.zero;
                adjustmentValues = Vector3.zero;
                lastSelectedObject = null;
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Selected Obj @", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.Label("Create Obj Modifier", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"X: {(isEnabled && lastSelectedObject != null ? lastSelectedObject.transform.position.x : 0)}");  // Update dynamically
        GUILayout.FlexibleSpace();
        GUILayout.Label($"X: {adjustmentValues.x:+#;-#;0}");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"Y: {(isEnabled && lastSelectedObject != null ? lastSelectedObject.transform.position.y : 0)}");  // Update dynamically
        GUILayout.FlexibleSpace();
        GUILayout.Label($"Y: {adjustmentValues.y:+#;-#;0}");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"Z: {(isEnabled && lastSelectedObject != null ? lastSelectedObject.transform.position.z : 0)}");  // Update dynamically
        GUILayout.FlexibleSpace();
        GUILayout.Label($"Z: {adjustmentValues.z:+#;-#;0}");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("X -", GUILayout.Width(50)))
        {
            adjustmentValues.x = Mathf.Max(adjustmentValues.x - 1, -20);
        }
        if (GUILayout.Button("X +", GUILayout.Width(50)))
        {
            adjustmentValues.x = Mathf.Min(adjustmentValues.x + 1, 20);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Y -", GUILayout.Width(50)))
        {
            adjustmentValues.y = Mathf.Max(adjustmentValues.y - 1, -20);
        }
        if (GUILayout.Button("Y +", GUILayout.Width(50)))
        {
            adjustmentValues.y = Mathf.Min(adjustmentValues.y + 1, 20);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Z -", GUILayout.Width(50)))
        {
            adjustmentValues.z = Mathf.Max(adjustmentValues.z - 1, -20);
        }
        if (GUILayout.Button("Z +", GUILayout.Width(50)))
        {
            adjustmentValues.z = Mathf.Min(adjustmentValues.z + 1, 20);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        selectedPrimitiveType = (PrimitiveType)EditorGUILayout.EnumPopup("Primitive Objects", selectedPrimitiveType, GUILayout.Width(200));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Create Primitive", GUILayout.Width(150)))
        {
            CreatePrimitiveAt(lastSelectedObjectCoordinates + adjustmentValues);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        selectedPrefabIndex = EditorGUILayout.Popup("Prefabs", selectedPrefabIndex, prefabNames, GUILayout.Width(200));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Create Prefab", GUILayout.Width(150)))
        {
            CreatePrefabAt(lastSelectedObjectCoordinates + adjustmentValues);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        selectedAudioIndex = EditorGUILayout.Popup("Audio Files", selectedAudioIndex, audioPrefabNames, GUILayout.Width(200));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Audio Source", GUILayout.Width(85)))
        {
            CreateAudioSourceAt(lastSelectedObjectCoordinates + adjustmentValues);
        }
        if (GUILayout.Button("Reverb Zone", GUILayout.Width(85)))
        {
            CreateAudioReverbZoneAt(lastSelectedObjectCoordinates + adjustmentValues);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        selectedVFXIndex = EditorGUILayout.Popup("VFX Files", selectedVFXIndex, vfxPrefabNames, GUILayout.Width(200));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        selectedEffectIndex = EditorGUILayout.Popup("Effects", selectedEffectIndex, effectNames, GUILayout.Width(200));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Create VFX", GUILayout.Width(150)))
        {
            CreateVFXAt(lastSelectedObjectCoordinates + adjustmentValues);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Focus Scene View", GUILayout.Width(150)))
        {
            FocusSceneView();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    void OnSelectionChange()
    {
        if (!isEnabled || Selection.activeTransform == null)
        {
            lastSelectedObjectCoordinates = Vector3.zero;  // Reset coordinates if no object is selected or tool is disabled
            lastSelectedObject = null;
        }
        else
        {
            lastSelectedObject = Selection.activeTransform.gameObject;  // Update last selected object and its coordinates
            lastSelectedObjectCoordinates = lastSelectedObject.transform.position;
        }
        adjustmentValues = Vector3.zero;  // Reset adjustment values
        Repaint();  // Repaint the window to reflect changes
    }

    void OnProjectChange()
    {
        LoadPrefabs();
        LoadAudioPrefabs();
        LoadVFXPrefabs();
        LoadOBJFiles();
    }

    void OnAssemblyReload()
    {
        LoadPrefabs();
        LoadAudioPrefabs();
        LoadVFXPrefabs();
        LoadOBJFiles();
    }

    void LoadPrefabs()
    {
        prefabs.Clear();
        var prefabPaths = Directory.GetFiles("Assets/Prefabs", "*.prefab");
        foreach (var path in prefabPaths)
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            prefabs.Add(prefab);
        }
        prefabNames = prefabs.Select(p => p.name).ToArray();
    }

    void LoadAudioPrefabs()
    {
        audioPrefabs.Clear();
        var audioPaths = Directory.GetFiles("Assets/Prefabs/Audio", "*.prefab");
        foreach (var path in audioPaths)
        {
            var audioPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            audioPrefabs.Add(audioPrefab);
        }
        audioPrefabNames = audioPrefabs.Select(p => p.name).ToArray();
    }

    void LoadVFXPrefabs()
    {
        vfxPrefabs.Clear();
        var vfxPaths = Directory.GetFiles("Assets/Prefabs/VFX", "*.prefab");
        foreach (var path in vfxPaths)
        {
            var vfxPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            vfxPrefabs.Add(vfxPrefab);
        }
        vfxPrefabNames = vfxPrefabs.Select(p => p.name).ToArray();
    }

    void LoadOBJFiles()
    {
        objFiles.Clear();
        var objFilePaths = Directory.GetFiles("Assets/Prefabs", "*.obj");
        foreach (var path in objFilePaths)
        {
            objFiles.Add(Path.GetFileNameWithoutExtension(path));
        }
        objFileNames = objFiles.ToArray();
    }

    void CreatePrimitiveAt(Vector3 position)
    {
        var primitive = GameObject.CreatePrimitive(selectedPrimitiveType);
        primitive.transform.position = position;
        Undo.RegisterCreatedObjectUndo(primitive, "Create Primitive");
        SelectObject(primitive);  // Select the newly created object
    }

    void CreatePrefabAt(Vector3 position)
    {
        var prefab = prefabs[selectedPrefabIndex];
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.transform.position = position;
        Undo.RegisterCreatedObjectUndo(instance, "Create Prefab");
        SelectObject(instance);  // Select the newly created object
    }

    void CreateAudioSourceAt(Vector3 position)
    {
        var audioPrefab = audioPrefabs[selectedAudioIndex];
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(audioPrefab);
        instance.transform.position = position;
        Undo.RegisterCreatedObjectUndo(instance, "Audio Source");
        SelectObject(instance);  // Select the newly created object
    }

    void CreateAudioReverbZoneAt(Vector3 position)
    {
        var reverbZone = new GameObject("Reverb Zone");
        reverbZone.transform.position = position;
        var reverbComponent = reverbZone.AddComponent<AudioReverbZone>();
        reverbComponent.reverbPreset = AudioReverbPreset.Arena;
        Undo.RegisterCreatedObjectUndo(reverbZone, "Reverb Zone");
        SelectObject(reverbZone);  // Select the newly created object
    }

    void CreateVFXAt(Vector3 position)
    {
        if (selectedEffectIndex == 0)
        {
            var vfxPrefab = vfxPrefabs[selectedVFXIndex];
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(vfxPrefab);
            instance.transform.position = position;
            Undo.RegisterCreatedObjectUndo(instance, "Create VFX");
            SelectObject(instance);  // Select the newly created object
        }
        else if (selectedEffectIndex == 1)
        {
            var trailObject = new GameObject("Trail Renderer");
            var trailRenderer = trailObject.AddComponent<TrailRenderer>();
            trailRenderer.time = 2.0f;
            trailObject.transform.position = position;
            Undo.RegisterCreatedObjectUndo(trailObject, "Create Trail Renderer");
            SelectObject(trailObject);  // Select the newly created object
        }
        else if (selectedEffectIndex == 2)
        {
            var lineObject = new GameObject("Line Renderer");
            var lineRenderer = lineObject.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, position);
            lineRenderer.SetPosition(1, position + Vector3.up * 5);
            lineObject.transform.position = position;
            Undo.RegisterCreatedObjectUndo(lineObject, "Create Line Renderer");
            SelectObject(lineObject);  // Select the newly created object
        }
    }

    void FocusSceneView()
    {
        if (SceneView.lastActiveSceneView != null)
        {
            SceneView.lastActiveSceneView.LookAt(lastSelectedObjectCoordinates + adjustmentValues);
        }
    }

    void SelectObject(GameObject obj)
    {
        Selection.activeObject = obj;  // Automatically select the created object
    }
}
