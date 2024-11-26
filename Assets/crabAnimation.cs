using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class crabAnimation : MonoBehaviour
{
    //OLD CODE
    //private Vector3[] defaultLegPositions;  //array of start positions for the legs
    //public GameObject[] legIKtargets; //targets for the legs, where the toes should move to
    //private int numberOfLegs = 8;
    //private float cycleLimit = 1f; //time it takes for a cycle of each individual leg to move
    ////private int cycleSpeed;
    //private float stepDistance = 1.1f;
    //private int currCycle = 0;
    //private float currHoldTime = 0.0f;
    //---------------------------------------------------------------------------------------------------

    private Animator mAnimator;
    public bool isMoving = false;
    public bool isDigging = false;


    // Start is called before the first frame update
    void Start()
    {
        //OLD CODE
        //Debug.Log(legIKtargets.Length);
        ////for each leg, get it's default position and assign the value in the array
        //for (int i = 0; i < legIKtargets.Length; i++)
        //{
        //    //defaultLegPositions[i] = legIKtargets[i].transform.localPosition;
        //    //Debug.Log(legIKtargets[i]);
        //}
        //------------------------------------------------------------------------------------------------

        //get animator component on parent aka crab
        mAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //OLD CODE
        ////get left joystick magnitude
        //float walkMag = GameObject.Find("Crab").GetComponent<PlayerController>().leftStick.magnitude;

        ////if player is moving
        //if(walkMag > 0.1f)
        //{
        //    //start timer
        //    currHoldTime += Time.deltaTime * walkMag; //count the time, increments every frame
        //    Debug.Log(currHoldTime);

        //    //if you've hit the time then move the legs
        //    if(currHoldTime > cycleLimit)
        //    {
        //        for (int i = 0; i < legIKtargets.Length; i++)
        //        {
        //            Vector3 newTarg = legIKtargets[i].transform.localPosition;
        //            newTarg.x = legIKtargets[i].transform.localPosition.x + stepDistance;
        //            legIKtargets[i].transform.Translate(newTarg, Space.Self);
        //            //Debug.Log("original pos: " + legIKtargets[i].transform.localPosition.x + " new pos: " + newTarg);
        //        }
        //        //reset timer
        //        currHoldTime = 0.0f;
        //    }

            
        //}
        //----------------------------------------------------------------------------------------------------

        //check that animator was found, if yes then continue
        if(mAnimator != null)
        {
            //if the player is moving then trigger the walk animation, otherwise trigger the exit walk animation
            if (Input.GetKey(KeyCode.L))
            {
                mAnimator.SetTrigger("EnterWalk");
            }

            if (Input.GetKey(KeyCode.S))
            {
                mAnimator.SetTrigger("ExitWalk");
            }
        }
    }

}
