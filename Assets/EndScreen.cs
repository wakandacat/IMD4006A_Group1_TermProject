using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EndScreen : MonoBehaviour
{
    public GameObject EndScreenFirstButton;
    public GameObject EndScreenPanel;
    private int numNPCs = 1;

    public int npcCounter = 0;
    private bool isFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        EndSCreenLogic();
    }
    public void EndSCreenLogic()
    {
        Debug.Log("Array count 1: " + npcCounter);
        if (npcCounter == numNPCs && isFinished == false)
        {
            Debug.Log("Array count: " + npcCounter);
            showScreen();

            isFinished = true;
        }
        //npcCounter = 0;

    }

    public void showScreen()
    {
        Time.timeScale = 0f;
        EndScreenPanel.SetActive(true);

        //clear selected object
        EventSystem.current.SetSelectedGameObject(null);
        //set new selected object
        EventSystem.current.SetSelectedGameObject(EndScreenFirstButton);
    }

    public void continuePlayingButton()
    {
        Time.timeScale = 1f;
        EndScreenPanel.SetActive(false);
    }

    public void returnMainMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }
}
