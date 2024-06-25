using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ObjectCreationTool : EditorWindow
{
    bool isEnabled = false;
    Vector3 lastSelectedObjectCoordinates = Vector3.zero;
    Vector3 adjustmentValues = Vector3.zero;
    GameObject lastSelectedObject;

    PrimitiveType selectedPrimitiveType = PrimitiveType.Cube;
    int selectedPrefabIndex = 0;
    int selectedAudioIndex = 0;

    List<GameObject> prefabs = new List<GameObject>();
    List<string> audioClipPaths = new List<string>();
    List<string> objFiles = new List<string>();

    string[] prefabNames;
    string[] audioClipNames;
    string[] objFileNames;    

    // Menu item to show the window
    [MenuItem("Tools/Object Creation Tool")]
    public static void ShowWindow()
    {
        GetWindow<ObjectCreationTool>("Object Creation Tool");
    }

    // Called when the window is enabled
    void OnEnable()
    {
        LoadPrefabs();
        LoadAudioClipNames();
        LoadOBJFiles();

        // Subscribe to project change and assembly reload events
        EditorApplication.projectChanged += OnProjectChange;
        AssemblyReloadEvents.afterAssemblyReload += OnAssemblyReload;
        EditorApplication.update += Update;
    }

    // Called when the window is disabled
    void OnDisable()
    {
        // Unsubscribe from events
        EditorApplication.projectChanged -= OnProjectChange;
        AssemblyReloadEvents.afterAssemblyReload -= OnAssemblyReload;
        EditorApplication.update -= Update;
    }

    // Main GUI for the tool
    void OnGUI()
    {
        GUILayout.Space(5);

        // Enable/Disable button
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button((isEnabled ? "Disable" : "Enable"), GUILayout.Width(150)))
        {
            isEnabled = !isEnabled;
            if (!isEnabled)
            {
                lastSelectedObjectCoordinates = Vector3.zero;
                adjustmentValues = Vector3.zero;
                lastSelectedObject = null;
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        // Display current selection coordinates and adjustment values
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Selected Obj @", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.Label("Create Obj @", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"X: {(isEnabled ? lastSelectedObjectCoordinates.x : 0)}");
        GUILayout.FlexibleSpace();
        GUILayout.Label($"X: {adjustmentValues.x:+#;-#;0}");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"Y: {(isEnabled ? lastSelectedObjectCoordinates.y : 0)}");
        GUILayout.FlexibleSpace();
        GUILayout.Label($"Y: {adjustmentValues.y:+#;-#;0}");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"Z: {(isEnabled ? lastSelectedObjectCoordinates.z : 0)}");
        GUILayout.FlexibleSpace();
        GUILayout.Label($"Z: {adjustmentValues.z:+#;-#;0}");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Adjustment buttons for X, Y, Z coordinates
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

        // Dropdown for primitive types
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        selectedPrimitiveType = (PrimitiveType)EditorGUILayout.EnumPopup("Primitive Objects", selectedPrimitiveType, GUILayout.Width(200));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Button to create selected primitive
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Create Primitive", GUILayout.Width(150)))
        {
            CreatePrimitiveAt(lastSelectedObjectCoordinates + adjustmentValues);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        // Dropdown for prefabs
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        selectedPrefabIndex = EditorGUILayout.Popup("Prefabs", selectedPrefabIndex, prefabNames, GUILayout.Width(200));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Button to create selected prefab
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Create Prefab", GUILayout.Width(150)))
        {
            CreatePrefabAt(lastSelectedObjectCoordinates + adjustmentValues);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        // Dropdown for audio files
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        selectedAudioIndex = EditorGUILayout.Popup("Audio Files", selectedAudioIndex, audioClipNames, GUILayout.Width(200));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Buttons to create audio source or reverb zone
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

        // Button to focus scene view
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Focus Scene View", GUILayout.Width(150)))
        {
            FocusSceneView();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    // Called when selection changes
    void OnSelectionChange()
    {
        if (!isEnabled || Selection.activeTransform == null)
        {
            lastSelectedObjectCoordinates = Vector3.zero;
            lastSelectedObject = null;
            Repaint();
            return;
        }

        lastSelectedObject = Selection.activeTransform.gameObject;
        lastSelectedObjectCoordinates = lastSelectedObject.transform.position;
        adjustmentValues = Vector3.zero;

        Repaint();
    }

    // Called every frame to update the window
    void Update()
    {
        if (lastSelectedObject != null)
        {
            lastSelectedObjectCoordinates = lastSelectedObject.transform.position;
            Repaint();
        }
    }

    // Called when project changes
    void OnProjectChange()
    {
        LoadPrefabs();
        LoadAudioClipNames();
        LoadOBJFiles();
    }

    // Called after assembly reload
    void OnAssemblyReload()
    {
        OnProjectChange();
    }

    // Load all prefabs from the "Assets/Prefabs" folder
    void LoadPrefabs()
    {
        prefabs.Clear();
        var prefabGuids = AssetDatabase.FindAssets("t:GameObject", new[] { "Assets/Prefabs" });
        foreach (var guid in prefabGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            prefabs.Add(prefab);
        }
        prefabNames = prefabs.Select(p => p.name).ToArray();
    }

    // Load all .ogg audio file names from the "Assets/Audio" folder
    void LoadAudioClipNames()
    {
        audioClipPaths.Clear();
        var audioClipGuids = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/Audio" });
        foreach (var guid in audioClipGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            audioClipPaths.Add(path);
        }
        audioClipNames = audioClipPaths.Select(p => Path.GetFileNameWithoutExtension(p)).ToArray();
    }

    // Load all .obj files from the "Assets/Prefabs" folder
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

    // Create a primitive at the specified position
    void CreatePrimitiveAt(Vector3 position)
    {
        var primitive = GameObject.CreatePrimitive(selectedPrimitiveType);
        primitive.transform.position = position;
        Undo.RegisterCreatedObjectUndo(primitive, "Create Primitive");
    }

    // Create a prefab at the specified position
    void CreatePrefabAt(Vector3 position)
    {
        var prefab = prefabs[selectedPrefabIndex];
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.transform.position = position;
        Undo.RegisterCreatedObjectUndo(instance, "Create Prefab");
    }

    // Create an audio source at the specified position
    void CreateAudioSourceAt(Vector3 position)
    {
        var audioClipPath = audioClipPaths[selectedAudioIndex];
        var audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(audioClipPath);

        // Create a new GameObject to hold the AudioSource
        var audioSourcePrefab = new GameObject(audioClip.name);
        var audioSource = audioSourcePrefab.AddComponent<AudioSource>();
        audioSource.clip = audioClip;

        // Place the GameObject in the scene
        audioSourcePrefab.transform.position = position;
        Undo.RegisterCreatedObjectUndo(audioSourcePrefab, "Audio Source");
    }

    // Create a reverb zone at the specified position
    void CreateAudioReverbZoneAt(Vector3 position)
    {
        var reverbZone = new GameObject("Reverb Zone");
        reverbZone.transform.position = position;
        var reverbComponent = reverbZone.AddComponent<AudioReverbZone>();
        reverbComponent.reverbPreset = AudioReverbPreset.Arena;
        Undo.RegisterCreatedObjectUndo(reverbZone, "Reverb Zone");
    }

    // Focus the scene view on the specified position
    void FocusSceneView()
    {
        if (SceneView.lastActiveSceneView != null)
        {
            SceneView.lastActiveSceneView.LookAt(lastSelectedObjectCoordinates + adjustmentValues);
        }
    }
}
