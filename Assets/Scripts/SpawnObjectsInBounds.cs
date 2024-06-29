using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************************************************************************

Author: Cherry Nugget Games - Lex vd Berg
Free to use and adapt as you see fit! Have fun with it. It's not perfect.

*/

public class SpawnObjectsInBounds : MonoBehaviour
{
    [Header("Prefabs to Spawn")]
    [SerializeField] private List<GameObject> prefabs;

    //List of the spawned prefabs so they can be deleted. You CAN do this manual as well
    [SerializeField, HideInInspector] private List<GameObject> spawnedPrefabs;


    [Header("Settings")]
    [SerializeField] private BoxCollider boxCollider;
    private Bounds bounds;
    [SerializeField] private int density = 5; //The number of objects to spawn

    //Used to randomize rotation
    [SerializeField] private bool randomRotationX = true;
    [SerializeField] private bool randomRotationY = true;
    [SerializeField] private bool randomRotationZ = true;
    [SerializeField] private float randomRotationFactor = 360f;

    //Used to randomize scale
    [SerializeField] private bool randomScale = true;
    [SerializeField] private float randomScaleMin = 0.1f;
    [SerializeField] private float randomScaleMax = 1.5f;
   

    //SpawnObject should only be called in the Editor
    public void SpawnObjects()
    {
        if(boxCollider == null)
            boxCollider = GetComponent<BoxCollider>();

        bounds = boxCollider.bounds;
        boxCollider.isTrigger = true;

        for(int i = 0; i < density; i++)
        {
            //Using PrefabUtility.InstantiatePrefab actually spawns a prefab rather then a clone of the prefab object.
            GameObject obj = PrefabUtility.InstantiatePrefab(prefabs[UnityEngine.Random.Range(0, prefabs.Count)], transform) as GameObject; 

            obj.transform.rotation = Quaternion.Euler(randomRotationX ? UnityEngine.Random.rotation.x * randomRotationFactor : obj.transform.rotation.x, 
            randomRotationY ? UnityEngine.Random.rotation.y * randomRotationFactor : obj.transform.rotation.y, 
            randomRotationZ ? UnityEngine.Random.rotation.z * randomRotationFactor : obj.transform.rotation.z);
            
            float randomizedScale = UnityEngine.Random.Range(randomScaleMin, randomScaleMax);
            obj.transform.localScale = new Vector3(randomizedScale, randomizedScale, randomizedScale);

            //Set it to a random position on the bounds;
            obj.transform.position = RandomPointInBounds();

            spawnedPrefabs.Add(obj);
        }
    }

    public void ClearPrefabs()
    {
        //Destorys all the prefabs and clears the list of saved prefabs.
        //Called IN the editor script.
        foreach(GameObject child in spawnedPrefabs)
        {
            DestroyImmediate(child);
        }
       
        spawnedPrefabs.Clear();
    }

    private Vector3 RandomPointInBounds() 
    {
        //Returns a random point WITHIN the bounds
        return new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            transform.position.y,
            Random.Range(bounds.min.z, bounds.max.z)
        );
    }
}
