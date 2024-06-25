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
    int selectedVFXIndex = 0;
    int selectedEffectIndex = 0;
    
    List<GameObject> prefabs = new List<GameObject>();
    List<GameObject> audioPrefabs = new List<GameObject>();
    List<GameObject> vfxPrefabs = new List<GameObject>();
    List<string> objFiles = new List<string>();

    string[] prefabNames;
    string[] audioPrefabNames;
    string[] vfxPrefabNames;
    string[] objFileNames;
    string[] effectNames = { "Particle System", "Trail Renderer", "Line Renderer" };

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

        EditorApplication.projectChanged += OnProjectChange;
        AssemblyReloadEvents.afterAssemblyReload += OnAssemblyReload;
    }

    void OnDisable()
    {
        EditorApplication.projectChanged -= OnProjectChange;
        AssemblyReloadEvents.afterAssemblyReload -= OnAssemblyReload;
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
        if (!isEnabled || Selection.activeTransform == null) return;

        lastSelectedObject = Selection.activeTransform.gameObject;
        lastSelectedObjectCoordinates = lastSelectedObject.transform.position;
        adjustmentValues = Vector3.zero;

        Repaint();
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
        OnProjectChange();
    }

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

    void LoadAudioPrefabs()
    {
        audioPrefabs.Clear();
        var audioPrefabGuids = AssetDatabase.FindAssets("t:GameObject", new[] { "Assets/Audio" });
        foreach (var guid in audioPrefabGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var audioPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            audioPrefabs.Add(audioPrefab);
        }
        audioPrefabNames = audioPrefabs.Select(p => p.name).ToArray();
    }

    void LoadVFXPrefabs()
    {
        vfxPrefabs.Clear();
        var vfxPrefabGuids = AssetDatabase.FindAssets("t:GameObject", new[] { "Assets/VFX" });
        foreach (var guid in vfxPrefabGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
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
    }

    void CreatePrefabAt(Vector3 position)
    {
        var prefab = prefabs[selectedPrefabIndex];
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.transform.position = position;
        Undo.RegisterCreatedObjectUndo(instance, "Create Prefab");
    }

    void CreateAudioSourceAt(Vector3 position)
    {
        var audioPrefab = audioPrefabs[selectedAudioIndex];
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(audioPrefab);
        instance.transform.position = position;
        Undo.RegisterCreatedObjectUndo(instance, "Audio Source");
    }

    void CreateAudioReverbZoneAt(Vector3 position)
    {
        var reverbZone = new GameObject("Reverb Zone");
        reverbZone.transform.position = position;
        var reverbComponent = reverbZone.AddComponent<AudioReverbZone>();
        reverbComponent.reverbPreset = AudioReverbPreset.Arena;
        Undo.RegisterCreatedObjectUndo(reverbZone, "Reverb Zone");
    }

    void CreateVFXAt(Vector3 position)
    {
        if (selectedEffectIndex == 0)
        {
            var vfxPrefab = vfxPrefabs[selectedVFXIndex];
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(vfxPrefab);
            instance.transform.position = position;
            Undo.RegisterCreatedObjectUndo(instance, "Create VFX");
        }
        else if (selectedEffectIndex == 1)
        {
            var trailObject = new GameObject("Trail Renderer");
            var trailRenderer = trailObject.AddComponent<TrailRenderer>();
            trailRenderer.time = 2.0f;
            trailObject.transform.position = position;
            Undo.RegisterCreatedObjectUndo(trailObject, "Create Trail Renderer");
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
        }
    }

    void FocusSceneView()
    {
        if (SceneView.lastActiveSceneView != null)
        {
            SceneView.lastActiveSceneView.LookAt(lastSelectedObjectCoordinates + adjustmentValues);
        }
    }
}
