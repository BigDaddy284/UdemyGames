using UnityEngine.SceneManagement;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEditor.PackageManager;
using System;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] float loadDelay = 1f;
    
    AudioSource boost;

    void Start()
    {
        boost = GetComponent<AudioSource>();
    }
    void OnCollisionEnter(Collision other) 
    {
        switch (other.gameObject.tag)
        {
            case "Friendly":
                Debug.Log("Friendly");
                break;
            
            case "Finish":
                StartSuccessSequence();
                break;

            default:
                StartCrashScene();                
                break;
        }        
    }

     void StartSuccessSequence()
    {
        GetComponent<Movement>().enabled = false;
        boost.enabled = false;
        Invoke("LoadNextLevel", loadDelay);
    }

    void StartCrashScene()
    {        
        GetComponent<Movement>().enabled = false;
        boost.enabled = false;
        Invoke("ReloadLevel", loadDelay);
    }
    void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    void LoadNextLevel()
    {
       int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
       int nextSceneIndex = currentSceneIndex +1;
       if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
       {
            nextSceneIndex = 0;
       }
       SceneManager.LoadScene(nextSceneIndex);
    }
}
