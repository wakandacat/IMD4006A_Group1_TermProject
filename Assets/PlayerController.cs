using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    //crab object being controlled by inputs
    public GameObject crab;
    public GameObject camera;
    public GameObject clawRight;
    public GameObject clawLeft;

    private TerrainEditor terrainScript;

    //particle systems and particle control variables
    public ParticleSystem movePartSystem;
    public ParticleSystem digPartSystem;
    private float digAnimTimer = 240.0f;

    //input action asset that reads controller inputs
    PlayerControls controls;

    //global variables
    public float moveSpeed = 7f;
    public float camSmooth = 0.2f;
    public bool isLeft = false; //right by default
    private Vector3 camOffset;
    private Transform camTransform;
    public float camXRot;
    private Vector3 clawLeftStart;
    private Vector3 clawRightStart;
    public float clawSmooth;
    public float clawAngle;
    public float maxClawDistance;

    //Booleans
    bool canPickup = true;
    bool ifpickedUp;
    bool closetoItem = false;

    [SerializeField] public Rigidbody _rb;

    GameObject pickedUpItem;
    private GameObject currentHoldingClaw;


    // Start is called before the first frame update
    void Start()
    {
        //instantiate a new input action object and enable it
        controls = new PlayerControls();
        controls.Enable();

        //setup callback function to switch active claws
        //+= refers to adding a callback function
        controls.GamePlay.Switch.performed += OnSwitch;
        controls.GamePlay.Focus.performed += OnFocus;

        //camera distance from player
        camOffset = Camera.main.transform.position - crab.transform.position;
        camTransform = Camera.main.transform;  // Get the main camera's transform

        //grab starting positions
        clawLeftStart = clawLeft.transform.localPosition;
        clawRightStart = clawRight.transform.localPosition;
        
        // Stopping the particle system by default
        movePartSystem.Stop();

        // Reminder on how to do this came from: https://youtu.be/gFwf_T8_8po?si=knchWQ0Sk1b1Lmna
        terrainScript = GameObject.FindGameObjectWithTag("TerrManager").GetComponent<TerrainEditor>();
        

        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        //walking controls

        //---------------------------------------BASICMOVEMENT-------------------------------------

        //read in the controller inputs
        Vector2 leftStick = controls.GamePlay.Walk.ReadValue<Vector2>();

        //ensure crab does not move when not reading inputs (including rotation)
        if (leftStick.magnitude > 0.1f)
        {
            //input from controls move the crab in xz plane
            Vector3 moveDirection = new Vector3(-1 * (leftStick.y), 0f, leftStick.x);

            //rotate the crab with player motion
            Quaternion targetRotate = Quaternion.LookRotation(moveDirection);
            crab.transform.rotation = Quaternion.Slerp(crab.transform.rotation, targetRotate, moveSpeed * Time.deltaTime);

            //move the crab
            Vector3 move = moveDirection * moveSpeed * Time.deltaTime;
            crab.transform.Translate(move, Space.World);

            //main camera follows behind player when walking
            Vector3 targetPos = crab.transform.position + camOffset;
            Vector3 smoothPos = Vector3.Lerp(Camera.main.transform.position, targetPos, camSmooth);
            Camera.main.transform.position = smoothPos;

            // Playing the particle system
            if (movePartSystem.isPlaying == false)
            {
                movePartSystem.Play();
            }
        }
        else
        {
            // Stopping the particle system when the movement stops
            if (movePartSystem.isPlaying == true)
            {
                movePartSystem.Stop();
            }
            //play walking audio --------- need to figure out why this works against the logic
            AudioManager.instance.walkSource.Play();
        }

        //---------------------------------------CLAWMOVEMENT-------------------------------------

        //claw movement controls
        Vector2 rightStick = controls.GamePlay.Claw.ReadValue<Vector2>();
        Vector3 clawMovement = new Vector3(-1*(rightStick.y), 0f, rightStick.x); //side to side movement
        Vector3 clawMove = clawMovement * moveSpeed * Time.deltaTime;


        if (isLeft && rightStick.magnitude > 0.1f)
        {

            //Debug.Log("left claw move");
            //Debug.Log("y" + -1 * (rightStick.y));
            //Debug.Log("x" + rightStick.x);
            clawLeft.transform.Translate(clawMove, Space.World);

            MoveClaw(clawLeft);
        }
        else if (!isLeft && rightStick.magnitude > 0.1f)
        {

            //Debug.Log("right claw move");
            //Debug.Log("y" + -1 * (rightStick.y));
            //Debug.Log("x" + rightStick.x);
            clawRight.transform.Translate(clawMove, Space.World);

            MoveClaw(clawRight);

        }
        else
        {
            //move with the body when no direct input for claws
            clawLeft.transform.localPosition = Vector3.Lerp(clawLeft.transform.localPosition, clawLeftStart, clawSmooth);
            clawRight.transform.localPosition = Vector3.Lerp(clawRight.transform.localPosition, clawRightStart, clawSmooth);
        }

        // keep claws close to crab body
        void MoveClaw(GameObject claw)
        {
            // Get the crab's position
            Vector3 crabPosition = crab.transform.position;
            Vector3 crabForward = crab.transform.right; //forward in this case being its actual face

            // get distance between the claw and the crab
            float distanceFromCrab = Vector3.Distance(claw.transform.position, crabPosition);

            // clamp claw within max distance
            if (distanceFromCrab > maxClawDistance)
            {
                // direction from the crab to the claw
                Vector3 directionFromCrab = (claw.transform.position - crabPosition).normalized;

                // set the claw's position to the maximum allowed distance from crab if it is further
                //creates a radius around the crab for the claw to move
                claw.transform.position = crabPosition + directionFromCrab * maxClawDistance;
            }

            //get the angle of the claw relative to the crab
            Vector3 directionToClaw = (claw.transform.position - crabPosition).normalized;
            float angle = Vector3.SignedAngle(crabForward, directionToClaw, Vector3.up);

            //stay within an arc
            if (angle > -clawAngle || angle < clawAngle)
            {
                //clamp again into the arc
                float clampedAngle = Mathf.Clamp(angle, -clawAngle, clawAngle);
                Quaternion rotation = Quaternion.AngleAxis(clampedAngle, Vector3.up);

                // set the new position
                claw.transform.position = crabPosition + rotation * (crabForward * maxClawDistance);
            }

        }

        //---------------------------------------BREAKING-------------------------------------

        //breaking controls ---------> only if right claw (!isLeft)
        float leftTrigger = controls.GamePlay.Dig.ReadValue<float>();
        //Debug.Log(leftTrigger);
        //make the crab break here

        //---------------------------------------DIGGING-------------------------------------

        //digging controls ---------> only if right claw (!isLeft)
        float rightTrigger = controls.GamePlay.Break.ReadValue<float>();
        //Debug.Log(rightTrigger);
        //make the crab dig here
        if (rightTrigger > 0f)
        {
            digAnimTimer += rightTrigger;
            terrainScript.digTerrain(crab.gameObject.transform.position, crab.gameObject.transform.rotation, rightTrigger);

            if (digAnimTimer / 240.0f >= 1.0f)
            {
                digPartSystem.Play();
                digAnimTimer = 0.0f;
            }
        }
        else
        {
            digAnimTimer = 240.0f;
            digPartSystem.Stop();

            //play digging audio
            AudioManager.instance.digSource.Play();
        }

        //---------------------------------------GRABBING-------------------------------------

        //grab controls ---------> either claw
        float rightBumper = controls.GamePlay.Grab.ReadValue<float>();
        //Debug.Log(rightBumper);
        //make the crab grab here

        if (rightBumper == 1)
        {
            if (canPickup == true && closetoItem == true)
            {
                if (isLeft)
                {
                    pickedUpItem.transform.parent = clawLeft.transform;
                    currentHoldingClaw = clawLeft;
                    canPickup = false;

                }
                else
                {

                    pickedUpItem.transform.parent = clawRight.transform;
                    //pickedUpItem.transform.parent = clawRight.transform;
                    Debug.Log(pickedUpItem.name);
                    currentHoldingClaw = clawRight;
                    canPickup = false;

                }
                ifpickedUp = true;
                closetoItem = false;
                //item weight affects movement speed of crab
                //moveSpeed = moveSpeed - pickedUpItem.GetComponent<Rigidbody>().mass;

                //play pick up audio
                AudioManager.instance.sfxPlayer(0);
            }

        }
        if (ifpickedUp == true && canPickup == false)
        {
            pickedUpItem.transform.position = new Vector3(currentHoldingClaw.transform.position.x, currentHoldingClaw.transform.position.y, currentHoldingClaw.transform.position.z + 0.25f);
        }

        //---------------------------------------DROPPING-------------------------------------

        //drop controls ---------> either claw
        float leftBumper = controls.GamePlay.Drop.ReadValue<float>();
        //Debug.Log(leftBumper);
        //make the crab drop here

        if (leftBumper == 1)
        {
            if (ifpickedUp == true)
            {
                pickedUpItem.transform.parent = null;
                //pickedUpItem.transform.parent = null;
                ifpickedUp = false;
                canPickup = true;
                //moveSpeed = 0; //HARDCODED FOR NOW

                //play put down audio
                AudioManager.instance.sfxPlayer(1);
            }
        }
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

    //recenter camera
    public void OnFocus(InputAction.CallbackContext context)
    {
        ////DOESNT WORK YET
        //Quaternion targetRotation = Quaternion.Euler(camXRot, crab.transform.eulerAngles.y, 0);
        //Vector3 targetPos = new Vector3(crab.transform.position.x + camOffset.x, crab.transform.position.y + camOffset.y, crab.transform.position.z);
        ////Vector3 targetPosition = crab.transform.rotation * camOffset;
        //camTransform.rotation = targetRotation;
        //camTransform.position = targetPos;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "item" && canPickup == true)
        {

            pickedUpItem = other.gameObject;
            Debug.Log("the item can be picked up:" + canPickup);
            canPickup = true;
            closetoItem = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //canPickup = false;
        //pickedUpItem = null;
        closetoItem = false;
    }
}
