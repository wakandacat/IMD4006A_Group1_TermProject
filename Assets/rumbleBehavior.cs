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
            bool playDig = GameObject.Find("Crab").GetComponent<PlayerController>().isDigging;

            if (playDig == true)
            {
                if (triggerVal >= 0.1f)
                {
                    //fastest speed
                    if (triggerVal >= 0.7f)
                    {
                        //lowFreqVal = 0.05f;
                        //highFreqVal = 0.1f;
                        lowFreqVal = 0.12f;
                        highFreqVal = 0.16f;
                        seconds = 0.3f;
                    }
                    else if (triggerVal >= 0.4f && triggerVal < 0.7f)
                    {
                        //lowFreqVal = 0.04f;
                        //highFreqVal = 0.07f;
                        lowFreqVal = 0.06f;
                        highFreqVal = 0.10f;
                        seconds = 0.5f;
                    }
                    else
                    {
                        //slowest speed
                        //lowFreqVal = 0.005f;
                        //highFreqVal = 0.01f;
                        lowFreqVal = 0.01f;
                        highFreqVal = 0.04f;
                        seconds = 0.7f;
                    }

                    gamepad.SetMotorSpeeds(lowFreqVal, highFreqVal);

                }
            }
            else if(playDig == false)
            {
                //gamepad.SetMotorSpeeds(0.0f, 0.0f);
            }
            

            //this may be doing nothing
            //gamepad.SetMotorSpeeds(0.0f, 0.0f);

            //wait for x seconds before re-entering the loop
            yield return new WaitForSeconds(seconds);

        }
    }

    public IEnumerator breakRumble()
    {

        while (true)
        {
            //get break trigger value
            float triggerVal = GameObject.Find("Crab").GetComponent<PlayerController>().leftTrigger;
            bool playBreak = GameObject.Find("Crab").GetComponent<PlayerController>().canBreak;
            bool isBroken = GameObject.Find("Crab").GetComponent<PlayerController>().broken;
            float lowFreqVal = 0.0f;
            float highFreqVal = 0.0f;


            if (playBreak == true)
            {
                //trigger is being held
                if (triggerVal >= 0.1f)
                {
                    Debug.Log("In trigger > 0.1f");
                    //fastest speed
                    if (triggerVal >= 0.7f)
                    {
                        lowFreqVal = 0.2f;
                        highFreqVal = 0.18f;
                        Debug.Log("In trigger > 0.7f");

                    }
                    else if (triggerVal >= 0.4f && triggerVal < 0.7f)
                    {
                        lowFreqVal = 0.1f;
                        highFreqVal = 0.08f;
                        Debug.Log("In trigger mid range ");

                    }
                    else
                    {
                        //slowest speed
                        lowFreqVal = 0.01f;
                        highFreqVal = 0.05f;
                        Debug.Log("In trigger slowest");

                    }

                    gamepad.SetMotorSpeeds(lowFreqVal, highFreqVal);

                }
                //this will kill rumble if let go but still holding item
                gamepad.SetMotorSpeeds(0.0f, 0.0f);
            }
            else if(isBroken == true)
            {
                //this will kill rumble once item is broken
                gamepad.SetMotorSpeeds(0.0f, 0.0f);
            }



            //wait for x seconds before re-entering the loop
            yield return new WaitForSeconds(0.3f);

        }
    }
}
