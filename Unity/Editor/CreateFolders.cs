using UnityEngine;
using UnityEditor;
using System.IO;

public class CreateFolders : EditorWindow
{
    string[] folders = { "Assets/Audio", "Assets/Materials", "Assets/Prefabs", "Assets/Scripts", "Assets/VFX" };

    [MenuItem("Tools/Create Folders")]
    static void ShowWindow()
    {
        GetWindow<CreateFolders>("Create Folders");
    }

    void OnGUI()
    {
        // Add 10 pixels of space between the buttons
        GUILayout.Space(10);

        GUILayout.Label(("Create Folder:"), EditorStyles.boldLabel);

        foreach (string folder in folders)
        {
            if (GUILayout.Button(folder))
            {
                CreateFolderIfNotExists(folder);
            }
        }
        
        // Add 20 pixels of space between the buttons
        GUILayout.Space(20);

        if (GUILayout.Button("Create All Folders"))
        {
            CreateAllFolders();
        }

        AssetDatabase.Refresh();
    }

    void CreateFolderIfNotExists(string folder)
    {
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
            Debug.Log("Created folder: " + folder);
        }
    }

    void CreateAllFolders()
    {
        foreach (string folder in folders)
        {
            CreateFolderIfNotExists(folder);
        }
    }
}
