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

    public IEnumerator rumble()
    {
        //for as long as player is holding trigger
        while (true)
        {
            //get values from player script
            float triggerVal_L = GameObject.Find("Crab").GetComponent<PlayerController>().leftTrigger;
            float triggerVal_R = GameObject.Find("Crab").GetComponent<PlayerController>().rightTrigger;
            bool playDig = GameObject.Find("Crab").GetComponent<PlayerController>().isDigging; //you are actively digging
            bool playBreak = GameObject.Find("Crab").GetComponent<PlayerController>().canBreak; //object you're holding can break
            bool isBroken = GameObject.Find("Crab").GetComponent<PlayerController>().broken;  //object you were breaking is now broken
            float lowFreqVal = 0.0f;  //speed to set left motor
            float highFreqVal = 0.0f; //speed to set right motor


            //digging rumble
            if (playDig == true)
            {
                if (triggerVal_R >= 0.1f)
                {
                    //fastest speed
                    if (triggerVal_R >= 0.7f)
                    {
                        //lowFreqVal = 0.05f;
                        //highFreqVal = 0.1f;
                        lowFreqVal = 0.12f;
                        highFreqVal = 0.16f;
                    }
                    else if (triggerVal_R >= 0.4f && triggerVal_R < 0.7f)
                    {
                        //lowFreqVal = 0.04f;
                        //highFreqVal = 0.07f;
                        lowFreqVal = 0.06f;
                        highFreqVal = 0.10f;
                    }
                    else
                    {
                        //slowest speed
                        //lowFreqVal = 0.005f;
                        //highFreqVal = 0.01f;
                        lowFreqVal = 0.01f;
                        highFreqVal = 0.04f;
                    }

                    gamepad.SetMotorSpeeds(lowFreqVal, highFreqVal);

                }
            }
            else if (playBreak == true && triggerVal_L >= 0.01f) //holding breakable object and trying to break it
            {
                
                 //Debug.Log("In trigger > 0.1f");
                //fastest speed
                if (triggerVal_L >= 0.7f)
                {
                    lowFreqVal = 0.2f;
                    highFreqVal = 0.18f;
                    //Debug.Log("In trigger > 0.7f");

                }
                else if (triggerVal_L >= 0.4f && triggerVal_L < 0.7f)
                {
                    lowFreqVal = 0.1f;
                    highFreqVal = 0.08f;
                    //Debug.Log("In trigger mid range ");

                }
                else
                {
                    //slowest speed
                    lowFreqVal = 0.01f;
                    highFreqVal = 0.05f;
                    //Debug.Log("In trigger slowest");

                }

                gamepad.SetMotorSpeeds(lowFreqVal, highFreqVal);

            }
            else if (isBroken == true) //object you were breaking is now broken
            {
                //this will kill rumble once item is broken
                gamepad.SetMotorSpeeds(0.0f, 0.0f);
            }
            else //if none of these situations are true, kill rumble
            {
                gamepad.SetMotorSpeeds(0.0f, 0.0f);
            }
          

            //wait for x seconds before re-entering the loop
            yield return new WaitForSeconds(0.3f);

        }
    }

}
