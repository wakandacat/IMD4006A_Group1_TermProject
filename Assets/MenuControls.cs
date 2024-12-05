using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MenuControls : MonoBehaviour
{
    public GameObject ControlsFirstButton, InstructionsFirstButton, GoalFirstButton, ControlsClosedFirst, InstructionsCloseFirst, GoalCloseFirst;
    public GameObject controlMenu, InstructionsMenu, MainMenu, GoalMenu;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is exiting");
    }
    public void openControls()
    {
        controlMenu.SetActive(true);
        MainMenu.SetActive(false);

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
        MainMenu.SetActive(true);

        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set new selected object
        EventSystem.current.SetSelectedGameObject(ControlsClosedFirst);

    }
    public void closeInstructions()
    {
        InstructionsMenu.SetActive(false);
        MainMenu.SetActive(true);

        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set new selected object
        EventSystem.current.SetSelectedGameObject(InstructionsCloseFirst);
    }
    public void closeGoals()
    {
        GoalMenu.SetActive(false);
        MainMenu.SetActive(true);

        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set new selected object
        EventSystem.current.SetSelectedGameObject(GoalCloseFirst);
    }
}
