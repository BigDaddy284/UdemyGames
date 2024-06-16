using UnityEngine.SceneManagement;
using UnityEngine;
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
    bool collisionDisabled = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();                  
    }

    void Update()
    {       
        CollisionSwtich();        
        SkipLevel();
                
    }
    
    void OnCollisionEnter(Collision other) 
    {
        if (isTransitioning || collisionDisabled) {return;}  // The '||' means 'or'

        switch (other.gameObject.tag) 
        {
            case "Friendly":
                
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
    
    void SkipLevel()
    {         
        if (Input.GetKey(KeyCode.L))
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

void CollisionSwtich()
    {
        if (Input.GetKey(KeyCode.C))
        {
            collisionDisabled = !collisionDisabled; //This will toggle the bool, but we need to tell the bool what it's controlling
        }
    }
}
