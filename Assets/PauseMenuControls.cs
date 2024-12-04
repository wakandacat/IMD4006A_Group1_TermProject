using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenuControls : MonoBehaviour
{
    public static bool pause = false;
    public GameObject audioManager;
    // Start is called before the first frame update
    public GameObject pauseMenu;

    AudioSource[] audioSources;
    void Start()
    {
         audioSources = audioManager.GetComponents<AudioSource>();
        Debug.Log(audioSources.Length);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (pause == false)
            {
                unpauseGame();
            }
            else
            {

                pauseGame();
            }
        }

    }

    public void pauseGame()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        

        
        for(int i=0; i < audioSources.Length; i++)
        {
            audioSources[i].Pause();
            Debug.Log("HELLO");
        }

        pause = false;
    }
    public void unpauseGame()
    {        
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        pause = true;
    }
    public void returnToMainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }
}
