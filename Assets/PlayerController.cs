using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //crab object being controlled by inputs
    public GameObject crab;
    public GameObject clawRight;
    public GameObject clawLeft;

    //input action asset that reads controller inputs
    PlayerControls controls;

    //numbers to fiddle with
    public float moveSpeed = 10f;
    public bool isLeft = false; //right by default
    public float clawSpeed;
    public Vector3 camOffset;
    public float camSmooth = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        //instantiate a new input action object and enable it
        controls = new PlayerControls();
        controls.Enable();

        //setup callback function to switch active claws
        //+= refers to adding a callback function
        controls.GamePlay.Switch.performed += OnSwitch;

        //camera distance from player
        camOffset = Camera.main.transform.position - crab.transform.position;

    }

    // Update is called once per frame
    void Update()
    {

        //grab main camera directions
        Vector3 projectCamFront = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        Quaternion cameraRotate = Quaternion.LookRotation(projectCamFront, Vector3.up);

        //---------------------------------------BASICMOVEMENT-------------------------------------

        //read in the controller inputs
        Vector2 leftStick = controls.GamePlay.Walk.ReadValue<Vector2>();

        //ensure crab does not move when not reading inputs (including rotation)
        if (leftStick.magnitude > 0.1f)
        {
            Vector3 moveDirection = new Vector3(-1 * (leftStick.y), 0f, leftStick.x);

            //rotate the crab with player motion
            Quaternion targetRotate = Quaternion.LookRotation(moveDirection);
            crab.transform.rotation = Quaternion.Slerp(crab.transform.rotation, targetRotate, moveSpeed * Time.deltaTime);

            //move the crab
            Vector3 move = moveDirection * moveSpeed * Time.deltaTime;
            crab.transform.Translate(move, Space.World);
            Debug.Log(move);
        }
        else
        {
            //do nothing
        }

        //camera movement when walking
        Vector3 targetPos = crab.transform.position + camOffset;
        Vector3 smoothPos = Vector3.Lerp(Camera.main.transform.position, targetPos, camSmooth);
        Camera.main.transform.position = smoothPos;

        //---------------------------------------CLAWMOVEMENT-------------------------------------

        //claw movement controls
        Vector2 rightStick = controls.GamePlay.Claw.ReadValue<Vector2>();
        Vector3 look = new Vector3(-1 * (rightStick.y), rightStick.x, 0f);
        //follow video of making them stay close to body

        //---------------------------------------DIGGING-------------------------------------

        //digging controls ---------> only if right claw (!isLeft)
        float leftTrigger = controls.GamePlay.Dig.ReadValue<float>();
        //Debug.Log(leftTrigger);
        //make the crab dig here

        //---------------------------------------BREAKING-------------------------------------

        //break controls ---------> only if right claw (!isLeft)
        float rightTrigger = controls.GamePlay.Break.ReadValue<float>();
        //Debug.Log(rightTrigger);
        //make the crab break here

        //---------------------------------------GRABBING-------------------------------------

        //grab controls ---------> either claw
        float rightBumper = controls.GamePlay.Grab.ReadValue<float>();
        //Debug.Log(rightBumper);
        //make the crab grab here

        //---------------------------------------DROPPING-------------------------------------

        //drop controls ---------> either claw
        float leftBumper = controls.GamePlay.Drop.ReadValue<float>();
        //Debug.Log(leftBumper);
        //make the crab drop here

        //---------------------------------------THROWING-------------------------------------

        //throw controls -----> only if holding item ---------> only if left claw (isLeft)
        float bottomButton = controls.GamePlay.Throw.ReadValue<float>();
        //Debug.Log(bottomButton);
        //make the crab throw item here

    }

    //toggle the active claw
    public void OnSwitch(InputAction.CallbackContext context)
    {
        isLeft = !isLeft;
        //Debug.Log(isLeft);
    }
}
