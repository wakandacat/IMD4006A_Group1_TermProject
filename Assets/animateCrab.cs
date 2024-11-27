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
        pControllerScript = GameObject.Find("CrabParentObj").GetComponent<PlayerController>();

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


            //digging animation
            //if player is pressing on the right trigger
            //if(pControllerScript.rightTrigger > 0.1f)
            //{
            //    //play on active claw
            //    if (pControllerScript.isLeft == true)
            //    {
            //        mAnimator.SetTrigger("isDiggingL");
            //    }
            //    else
            //    {
            //        mAnimator.SetTrigger("isDiggingR");
            //    }
                
            //}
            //else
            //{
            //    //end digging animation
            //    mAnimator.SetTrigger("ExitDigging");
            //}

        }
    }

}
