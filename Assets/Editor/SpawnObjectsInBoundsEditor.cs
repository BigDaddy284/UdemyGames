using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/****************************************************************************

Author: Cherry Nugget Games - Lex vd Berg
Free to use and adapt as you see fit! Have fun with it. It's not perfect.

NOTE: THIS SCRIPT MUST BE PLACED INSIDE A FOLDER CALLED “EDITOR” IN YOUR UNITY PROJECT. Other wise the code will not work.

*/

[CustomEditor(typeof(SpawnObjectsInBounds))]
public class SpawnObjectsInBoundsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SpawnObjectsInBounds soib = (SpawnObjectsInBounds)target;

        //Create two editor buttons to spawn and clear the objects
        if(GUILayout.Button("Spawn Objects"))
        {
            soib.SpawnObjects();
        }

        if(GUILayout.Button("Clear Objects"))
        {
            soib.ClearPrefabs();
        }
    }
}
