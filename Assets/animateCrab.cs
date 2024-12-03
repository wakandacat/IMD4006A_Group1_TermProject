using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class animateCrab : MonoBehaviour
{
    private Animator mAnimator;
    public bool isMoving = false;
    public bool isDigging = false;
    private PlayerController pControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        //get player controller script, needed for accessing joystick inputs
        pControllerScript = GameObject.Find("Crab").GetComponent<PlayerController>();

        //get animator component on parent aka crab
        mAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (mAnimator != null)
        {
            //walking/movement animation
            //set playback speed for animation
            mAnimator.SetFloat("walkSpeed", pControllerScript.leftStick.magnitude);

            //if the player is moving then trigger the walk animation, otherwise trigger the exit walk animation
            if (pControllerScript.leftStick.magnitude > 0.1f)
            {
                
                //moving to the right
                if (pControllerScript.dirChange == true)
                {
                    mAnimator.SetTrigger("walkingRight");

                }
                else //moving to the left
                {
                    mAnimator.SetTrigger("walkingLeft");

                }

            }
            else
            {
                //end walk cycle and set directions back to false
                mAnimator.SetTrigger("ExitWalk");

            }


        }
    }

    public IEnumerator digAnim()
    {

        //for as long as player is using joystick
        while (true)
        {
            //get stickMag
            float triggerVal = GameObject.Find("Crab").GetComponent<PlayerController>().rightTrigger;
            GameObject clawStart_R = GameObject.Find("Crab").GetComponent<PlayerController>().clawRight;
            GameObject clawStart_L = GameObject.Find("Crab").GetComponent<PlayerController>().clawLeft;
            bool isLeftt = GameObject.Find("Crab").GetComponent<PlayerController>().isLeft;

            //Debug.Log("rightstick mag is: " + stickMag);

            if (triggerVal >= 0.1f)
            {
                if (isLeftt)
                {
                    Vector3 digPosDown = clawStart_L.transform.localPosition + new Vector3(0.0f, -0.8f, 0.0f);
                    clawStart_L.transform.localPosition = Vector3.Lerp(clawStart_L.transform.localPosition, digPosDown, 0.5f);
                }
                else
                {
                    Vector3 digPosDown = clawStart_R.transform.localPosition + new Vector3(0.0f, -0.8f, 0.0f);
                    clawStart_R.transform.localPosition = Vector3.Lerp(clawStart_R.transform.localPosition, digPosDown, 0.5f);
                }
                //Debug.Log("digging should play sound");
                //digSource.PlayOneShot(digSource.clip);
                //Vector3 digPosDown = clawRight.transform.localPosition + new Vector3(0.0f, -0.6f, 0.0f); //should take current claw pos and only make the y value decrease
                //Vector3 digPosUp = clawRight.transform.localPosition + new Vector3(0.0f, 0.4f, 0.0f); //should take current claw pos and only make the y value decrease
                //clawRight.transform.localPosition = Vector3.Lerp(clawRight.transform.localPosition, digPosDown, Time.deltaTime * 5.0f);

            }
            else
            {
                //Debug.Log("stop digging sound");
                //digSource.Stop();
            }

            //wait for x seconds before re-entering the loop
            yield return new WaitForSeconds(0.5f);

        }
    }
}
