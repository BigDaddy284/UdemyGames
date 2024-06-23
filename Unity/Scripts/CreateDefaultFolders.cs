using UnityEditor;
using UnityEngine;
using System.IO;

public class CreateDefaultFolders : Editor
{
    [MenuItem("Tools/Create Default Folders")]
    private static void CreateFolders()
    {
        string[] folders = { "Assets/Audio", "Assets/Prefabs", "Assets/Materials", "Assets/Scenes", "Assets/VFX" };

        foreach (string folder in folders)
        {
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
                Debug.Log("Created folder: " + folder);
            }
        }

        AssetDatabase.Refresh();
    }
}
