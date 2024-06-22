using UnityEditor;
using UnityEngine;
using System.IO;

public class CreateFolders : EditorWindow
{
    // Define an array to store folder names
    private string[] folders = { "Assets/Test", "Assets/Audio", "Assets/Prefabs", "Assets/Materials", "Assets/Scenes", "Assets/VFX" };

    [MenuItem("Tools/Create Folders")]
    private static void ShowWindow()
    {
        // Create and show the custom window
        GetWindow<CreateFolders>("Create Folders");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select folders to create:");

        // Iterate through the folders array
        foreach (string folder in folders)
        {
            // Display a toggle (checkbox) for each folder
            bool createFolder = EditorGUILayout.ToggleLeft(folder, false);

            if (createFolder)
            {
                // If the checkbox is checked, create the folder
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                    Debug.Log("Created folder: " + folder);
                }
            }
        }

        // Refresh the Asset Database to reflect the changes
        AssetDatabase.Refresh();
    }
}
