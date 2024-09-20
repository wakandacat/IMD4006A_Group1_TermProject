using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //crab object being controlled by inputs
    public GameObject crab;
    public GameObject camera;
    public GameObject clawRight;
    public GameObject clawLeft;

    //input action asset that reads controller inputs
    PlayerControls controls;

    //numbers to fiddle with
    public float moveSpeed;
    public float camSpeed;
    public Boolean isLeft = false; //right by default
    public float clawSpeed;

    // Start is called before the first frame update
    void Start()
    {
        //instantiate a new input action object
        controls = new PlayerControls();
        controls.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        //walking controls
        Vector2 leftStick = controls.GamePlay.Walk.ReadValue<Vector2>();
        Vector3 walk = new Vector3(leftStick.x, 0f, leftStick.y);
        crab.transform.Translate(walk*moveSpeed);

        //claw movement controls
        Vector2 rightStick = controls.GamePlay.Claw.ReadValue<Vector2>();
        Vector3 look = new Vector3(-1 * (rightStick.y), rightStick.x, 0f);
        //follow video of making them stay close to body

        //digging controls
        float leftTrigger = controls.GamePlay.Dig.ReadValue<float>();
        //Debug.Log(leftTrigger);
        //make the crab dig here

        //break controls
        float rightTrigger = controls.GamePlay.Break.ReadValue<float>();
        //Debug.Log(rightTrigger);
        //make the crab break here

        //grab controls
        float rightBumper = controls.GamePlay.Grab.ReadValue<float>();
        //Debug.Log(rightBumper);
        //make the crab grab here

        //drop controls
        float leftBumper = controls.GamePlay.Drop.ReadValue<float>();
        //Debug.Log(leftBumper);
        //make the crab drop here

        //throw controls -----> only if holding item
        float bottomButton = controls.GamePlay.Throw.ReadValue<float>();
        //Debug.Log(bottomButton);
        //make the crab throw item here

        //switch claw controls
        float topButton = controls.GamePlay.Switch.ReadValue<float>();
        //Debug.Log(topButton);
        //toggle the active claw
        
        if (topButton > 0f)
        {
            isLeft = true;
        }
        Debug.Log(isLeft);
        //change cinemachine virtual cameras to follow the correct claw

    }
}
