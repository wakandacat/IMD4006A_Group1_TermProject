using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class rumbleBehavior : MonoBehaviour
{
    //this script holds the coroutines related to haptics behaviors
    public static rumbleBehavior instance;
    private Gamepad gamepad;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        gamepad = Gamepad.current;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator digRumble()
    {
        //for as long as player is holding trigger
        while (true)
        {
            //get trigger value
            float triggerVal = GameObject.Find("Crab").GetComponent<PlayerController>().rightTrigger;
            float seconds = 0.0f;
            float lowFreqVal = 0.0f;
            float highFreqVal = 0.0f;
            //float yPos = -0.8f * triggerVal;

            if (triggerVal >= 0.1f)
            {
                //fastest speed
                if (triggerVal >= 0.7f)
                {
                    lowFreqVal = 0.05f;
                    highFreqVal = 0.1f;
                    seconds = 0.4f;
                }
                else if (triggerVal >= 0.4f && triggerVal < 0.7f)
                {
                    lowFreqVal = 0.04f;
                    highFreqVal = 0.07f;
                    seconds = 0.5f;
                }
                else
                {
                    //slowest speed
                    lowFreqVal = 0.005f;
                    highFreqVal = 0.01f;
                    seconds = 0.7f;
                }
              
                gamepad.SetMotorSpeeds(lowFreqVal, highFreqVal);

            }

            //this may be doing nothing
            gamepad.SetMotorSpeeds(0.0f, 0.0f);

            //wait for x seconds before re-entering the loop
            yield return new WaitForSeconds(seconds);

        }
    }
}
