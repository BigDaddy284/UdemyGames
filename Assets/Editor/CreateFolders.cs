using UnityEditor;
using UnityEngine;
using System.IO;

public class CreateFolders : EditorWindow
{
    private string[] folders = {"Assets/Audio", "Assets/Prefabs", "Assets/Materials", "Assets/Scenes", "Assets/VFX" };

    [MenuItem("Tools/Create Folders")]
    private static void ShowWindow()
    {
        GetWindow<CreateFolders>("Create Folders");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select folders to create:");

        foreach (string folder in folders)
        {
            if (GUILayout.Button(folder))
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                    Debug.Log("Created folder: " + folder);
                }
            }
        }

        AssetDatabase.Refresh();
    }
}
