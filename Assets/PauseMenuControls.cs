using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuControls : MonoBehaviour
{
    public static bool pause = false;
    // Start is called before the first frame update
    public GameObject pauseMenu;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if(pause == false)
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
        pause = false;
    }
    public void unpauseGame()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        pause = true;

    }
    public void returnToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
