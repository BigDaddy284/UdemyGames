using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

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

    string[] prefabNames;
    string[] audioPrefabNames;
    string[] vfxPrefabNames;
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
        // Add 5 pixels of space between the buttons
        GUILayout.Space(5);
        
        // Centered Enable/Disable button
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button((isEnabled ? "Disable" : "Enable"), GUILayout.Width(150)))
        {
            isEnabled = !isEnabled;
            if (!isEnabled)
            {
                // Reset coordinates when disabled
                lastSelectedObjectCoordinates = Vector3.zero;
                adjustmentValues = Vector3.zero;
                lastSelectedObject = null;
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Add 5 pixels of space between the buttons
        GUILayout.Space(5);

        // Display locked and final coordinates side by side
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
        Vector3 finalCoordinates = (isEnabled ? lastSelectedObjectCoordinates : Vector3.zero) + adjustmentValues;
        GUILayout.Label($"X: {finalCoordinates.x}");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"Y: {(isEnabled ? lastSelectedObjectCoordinates.y : 0)}");
        GUILayout.FlexibleSpace();
        GUILayout.Label($"Y: {finalCoordinates.y}");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"Z: {(isEnabled ? lastSelectedObjectCoordinates.z : 0)}");
        GUILayout.FlexibleSpace();
        GUILayout.Label($"Z: {finalCoordinates.z}");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Adjustment buttons for X
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

        // Adjustment buttons for Y
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

        // Adjustment buttons for Z
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

        // Add 5 pixels of space between the buttons
        GUILayout.Space(5);

        // Dropdown for selecting primitive type
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        selectedPrimitiveType = (PrimitiveType)EditorGUILayout.EnumPopup("Primitive Objects", selectedPrimitiveType, GUILayout.Width(200));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Button to create primitive object at final coordinates centered
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Create Primitive", GUILayout.Width(150)))
        {
            CreatePrimitiveAt(finalCoordinates);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Add 5 pixels of space between the buttons
        GUILayout.Space(5);

        // Dropdown for selecting prefab
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        selectedPrefabIndex = EditorGUILayout.Popup("Prefabs", selectedPrefabIndex, prefabNames, GUILayout.Width(200));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Button to create prefab object at final coordinates centered
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Create Prefab", GUILayout.Width(150)))
        {
            CreatePrefabAt(finalCoordinates);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Add 5 pixels of space between the buttons
        GUILayout.Space(5);

        // Dropdown for selecting audio prefab
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        selectedAudioIndex = EditorGUILayout.Popup("Audio Files", selectedAudioIndex, audioPrefabNames, GUILayout.Width(200));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Buttons to create audio prefab or reverb zone at final coordinates centered and side by side
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Audio Source", GUILayout.Width(100)))
        {
            CreateAudioSourceAt(finalCoordinates);
        }
        if (GUILayout.Button("Reverb Zone", GUILayout.Width(100)))
        {
            CreateAudioReverbZoneAt(finalCoordinates);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Add 5 pixels of space between the buttons
        GUILayout.Space(5);

        // Dropdown for selecting VFX prefab
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        selectedVFXIndex = EditorGUILayout.Popup("VFX Files", selectedVFXIndex, vfxPrefabNames, GUILayout.Width(200));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Dropdown for selecting effect type
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        selectedEffectIndex = EditorGUILayout.Popup("Select Effect", selectedEffectIndex, effectNames, GUILayout.Width(200));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Button to create VFX prefab with selected effect at final coordinates centered
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Create VFX", GUILayout.Width(150)))
        {
            CreateVFXPrefabAt(finalCoordinates);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Add 5 pixels of space between the buttons
        GUILayout.Space(5);

        // Heading for Script creation
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Script Creation", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Button to create a new C# script
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Create Script", GUILayout.Width(150)))
        {
            CreateCSharpScript();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Add 5 pixels of space between the buttons
        GUILayout.Space(5);

        // Heading for Scene creation
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label("Scene Creation", EditorStyles.boldLabel);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        // Button to create a new Scene
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Create Scene", GUILayout.Width(150)))
        {
            CreateNewScene();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
    }

    void OnSelectionChange()
    {
        if (isEnabled && Selection.activeGameObject != null)
        {
            lastSelectedObject = Selection.activeGameObject;
            lastSelectedObjectCoordinates = lastSelectedObject.transform.position;
            Repaint(); // Redraw the window to update the displayed coordinates
        }
    }

    void OnHierarchyChange()
    {
        if (isEnabled && lastSelectedObject != null)
        {
            GameObject[] selectedObjects = Selection.gameObjects;
            if (selectedObjects.Length > 1)
            {
                foreach (var obj in selectedObjects)
                {
                    if (obj != lastSelectedObject && obj.transform.position == lastSelectedObjectCoordinates)
                    {
                        obj.transform.position = lastSelectedObjectCoordinates + adjustmentValues;
                        lastSelectedObject = obj;
                        lastSelectedObjectCoordinates = obj.transform.position;
                        Repaint(); // Redraw the window to update the displayed coordinates
                        break;
                    }
                }
            }
        }
    }

    void OnProjectChange()
    {
        LoadAllAssets();
        Repaint();
    }

    void OnAssemblyReload()
    {
        LoadAllAssets();
        Repaint();
    }

    void CreatePrimitiveAt(Vector3 coordinates)
    {
        GameObject newObject = GameObject.CreatePrimitive(selectedPrimitiveType);
        newObject.transform.position = coordinates;
        SelectNewObject(newObject);
        Undo.RegisterCreatedObjectUndo(newObject, "Create Primitive");
    }

    void CreatePrefabAt(Vector3 coordinates)
    {
        if (prefabs.Count > 0 && selectedPrefabIndex >= 0 && selectedPrefabIndex < prefabs.Count)
        {
            GameObject prefab = prefabs[selectedPrefabIndex];
            GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            newObject.transform.position = coordinates;
            SelectNewObject(newObject);
            Undo.RegisterCreatedObjectUndo(newObject, "Create Prefab");
        }
    }

    void CreateAudioSourceAt(Vector3 coordinates)
    {
        if (audioPrefabs.Count > 0 && selectedAudioIndex >= 0 && selectedAudioIndex < audioPrefabs.Count)
        {
            GameObject audioPrefab = audioPrefabs[selectedAudioIndex];
            GameObject newObject = Instantiate(audioPrefab);
            newObject.transform.position = coordinates;
            SelectNewObject(newObject);
            Undo.RegisterCreatedObjectUndo(newObject, "Create Audio Source");
        }
    }

    void CreateAudioReverbZoneAt(Vector3 coordinates)
    {
        GameObject newObject = new GameObject("Audio Reverb Zone");
        AudioReverbZone reverbZone = newObject.AddComponent<AudioReverbZone>();
        newObject.transform.position = coordinates;
        SelectNewObject(newObject);
        Undo.RegisterCreatedObjectUndo(newObject, "Create Audio Reverb Zone");
    }

    void CreateVFXPrefabAt(Vector3 coordinates)
    {
        if (vfxPrefabs.Count > 0 && selectedVFXIndex >= 0 && selectedVFXIndex < vfxPrefabs.Count)
        {
            GameObject vfxPrefab = vfxPrefabs[selectedVFXIndex];
            GameObject newObject = (GameObject)PrefabUtility.InstantiatePrefab(vfxPrefab);
            newObject.transform.position = coordinates;

            // Add selected effect component
            switch (selectedEffectIndex)
            {
                case 0:
                    newObject.AddComponent<ParticleSystem>();
                    break;
                case 1:
                    newObject.AddComponent<TrailRenderer>();
                    break;
                case 2:
                    newObject.AddComponent<LineRenderer>();
                    break;
            }

            SelectNewObject(newObject);
            Undo.RegisterCreatedObjectUndo(newObject, "Create VFX Prefab");
        }
    }

    void SelectNewObject(GameObject newObject)
    {
        Selection.activeGameObject = newObject;
        lastSelectedObject = newObject;
        lastSelectedObjectCoordinates = newObject.transform.position;
        // Do not reset adjustment values to retain the modifier
    }

    void LoadPrefabs()
    {
        LoadAssets("t:Prefab", "Assets/Prefabs", ref prefabs, ref prefabNames);
    }

    void LoadAudioPrefabs()
    {
        string[] audioPrefabGuids = AssetDatabase.FindAssets("t:AudioClip", new[] { "Assets/Audio" });
        audioPrefabs.Clear();
        audioPrefabNames = new string[audioPrefabGuids.Length];
        for (int i = 0; i < audioPrefabGuids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(audioPrefabGuids[i]);
            AudioClip audioClip = AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            GameObject audioPrefab = new GameObject(audioClip.name);
            audioPrefab.AddComponent<AudioSource>().clip = audioClip;
            audioPrefabs.Add(audioPrefab);
            audioPrefabNames[i] = audioClip.name;
        }
    }

    void LoadVFXPrefabs()
    {
        LoadAssets("t:Prefab", "Assets/VFX", ref vfxPrefabs, ref vfxPrefabNames);
    }

    void LoadAssets(string filter, string folder, ref List<GameObject> list, ref string[] names)
    {
        string[] guids = AssetDatabase.FindAssets(filter, new[] { folder });
        list.Clear();
        names = new string[guids.Length];
        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            list.Add(asset);
            names[i] = asset.name;
        }
    }

    void LoadAllAssets()
    {
        LoadPrefabs();
        LoadAudioPrefabs();
        LoadVFXPrefabs();
    }

    void CreateCSharpScript()
    {
        string path = EditorUtility.SaveFilePanel("Create C# Script", "Assets/Scripts", "NewScript.cs", "cs");
        if (!string.IsNullOrEmpty(path))
        {
            if (!path.StartsWith(Application.dataPath))
            {
                EditorUtility.DisplayDialog("Invalid Path", "The script must be placed within the Assets folder.", "OK");
                return;
            }

            string scriptContent = "using UnityEngine;\n\npublic class " + System.IO.Path.GetFileNameWithoutExtension(path) + " : MonoBehaviour\n{\n\tvoid Start()\n\t{\n\t\t// Start logic here\n\t}\n\n\tvoid Update()\n\t{\n\t\t// Update logic here\n\t}\n}";
            System.IO.File.WriteAllText(path, scriptContent);
            AssetDatabase.Refresh();
        }
    }

    void CreateNewScene()
    {
        string path = EditorUtility.SaveFilePanel("Create New Scene", "Assets/Scenes", "NewScene.unity", "unity");
        if (!string.IsNullOrEmpty(path))
        {
            if (!path.StartsWith(Application.dataPath))
            {
                EditorUtility.DisplayDialog("Invalid Path", "The scene must be placed within the Assets folder.", "OK");
                return;
            }

            UnityEngine.SceneManagement.Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(newScene, path);
            AssetDatabase.Refresh();
        }
    }
}
