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
            //if the player is moving then trigger the walk animation, otherwise trigger the exit walk animation
            if (pControllerScript.leftStick.magnitude > 0.1f)
            {
                //set playback speed for animation
                mAnimator.SetFloat("walkSpeed", pControllerScript.leftStick.magnitude);
                //mAnimator.speed = 

                //moving to the right
                if (pControllerScript.dirChange == true)
                {
                    mAnimator.SetTrigger("walkingRight");
                    //mAnimator.SetBool("directionR", true);
                    //mAnimator.SetBool("directionL", false);
                }
                else //moving to the left
                {
                    mAnimator.SetTrigger("walkingLeft");
                    //mAnimator.SetBool("directionL", true);
                    //mAnimator.SetBool("directionR", false);
                }

            }
            else
            {
                //end walk cycle and set directions back to false
                mAnimator.SetTrigger("ExitWalk");
                //mAnimator.SetBool("directionR", false);
                //mAnimator.SetBool("directionL", false);
            }

        }
    }

}
