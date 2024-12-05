using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;



public class PauseMenuControls : MonoBehaviour
{
    public static bool pause = false;
    public GameObject audioManager;
    // Start is called before the first frame update
    public GameObject pauseMenu;

    AudioSource[] audioSources;

    public GameObject ControlsFirstButton, InstructionsFirstButton, GoalFirstButton, ControlsClosedFirst, InstructionsCloseFirst, GoalCloseFirst;
    public GameObject controlMenu, InstructionsMenu, pausedMenu, GoalMenu;
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

    public void openControls()
    {
        controlMenu.SetActive(true);
        pausedMenu.SetActive(false);

        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set new selected object
        EventSystem.current.SetSelectedGameObject(ControlsFirstButton);
    }
    public void openInstructions()
    {
        InstructionsMenu.SetActive(true);
        controlMenu.SetActive(false);

        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set new selected object
        EventSystem.current.SetSelectedGameObject(InstructionsFirstButton);
    }
    public void openGoal()
    {
        InstructionsMenu.SetActive(false);
        GoalMenu.SetActive(true);

        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set new selected object
        EventSystem.current.SetSelectedGameObject(GoalFirstButton);

    }
    public void closeControls()
    {
        controlMenu.SetActive(false);
        pausedMenu.SetActive(true);

        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set new selected object
        EventSystem.current.SetSelectedGameObject(ControlsClosedFirst);

    }
    public void closeInstructions()
    {
        InstructionsMenu.SetActive(false);
        pausedMenu.SetActive(true);

        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set new selected object
        EventSystem.current.SetSelectedGameObject(InstructionsCloseFirst);
    }
    public void closeGoals()
    {
        GoalMenu.SetActive(false);
        pausedMenu.SetActive(true);

        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set new selected object
        EventSystem.current.SetSelectedGameObject(GoalCloseFirst);
    }
}
