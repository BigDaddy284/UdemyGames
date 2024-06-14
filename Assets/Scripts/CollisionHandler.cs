using UnityEngine.SceneManagement;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEditor.PackageManager;
using System;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] float loadDelay = 1f;
    [SerializeField] AudioClip crashBang;
    [SerializeField] AudioClip winning;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem crashParticles;
    
    AudioSource audioSource;
    ParticleSystem ps;

    bool isTransitioning = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();                  
    }
    
    void OnCollisionEnter(Collision other) 
    {
        if (isTransitioning) {return;}
        
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
        isTransitioning = true;
        audioSource.Stop();
        GetComponent<Movement>().enabled = false;
        audioSource.PlayOneShot(winning);      
        successParticles.Play();
        Invoke("LoadNextLevel", loadDelay);
    }

    void StartCrashScene()
    {        
        isTransitioning = true;
        audioSource.Stop();
        GetComponent<Movement>().enabled = false;
        audioSource.PlayOneShot(crashBang);
        crashParticles.Play();
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
