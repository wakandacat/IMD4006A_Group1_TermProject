using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;



public class PauseMenuControls : MonoBehaviour
{
    private bool pause = true;
    public GameObject audioManager;
    // Start is called before the first frame update
    public GameObject pauseMenu;

    AudioSource[] audioSources;

    public GameObject ControlsFirstButton, InstructionsFirstButton, GoalFirstButton, ControlsClosedFirst, InstructionsCloseFirst, GoalCloseFirst;
    public GameObject controlMenu, InstructionsMenu, pausedMenu, GoalMenu;

    PlayerControls UIcontrols;
    void Start()
    {
        audioSources = audioManager.GetComponents<AudioSource>();
        Debug.Log(audioSources.Length);

        UIcontrols = new PlayerControls();

        UIcontrols.MenuControls.Enable();

        UIcontrols.MenuControls.Pause.performed += pauseGameInput;


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void pauseGameInput(InputAction.CallbackContext context)
    {
        Debug.Log("Pause" + pause);
        if (pause == false)
        {
            unpauseGame();
        }
        else
        {
            pauseGame();
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
