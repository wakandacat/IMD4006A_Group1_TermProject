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
    public float moveSpeed = 10f;
    public float camSmooth = 0.2f;
    public bool isLeft = false; //right by default
    private Vector3 camOffset;
    private Transform camTransform;
    public float camXRot;
    private Vector3 clawLeftStart;
    private Vector3 clawRightStart;
    public float clawLeftBound;
    public float clawRightBound;

    //Booleans
    bool canPickup;
    bool ifpickedUp;

    [SerializeField] public Rigidbody _rb;

    GameObject pickedUpItem;


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
        }

        //---------------------------------------CLAWMOVEMENT-------------------------------------

        //claw movement controls
        Vector2 rightStick = controls.GamePlay.Claw.ReadValue<Vector2>();
        Vector3 clawMovement = new Vector3(-1 * (rightStick.y), 0f, rightStick.x);

        if (isLeft && rightStick.magnitude > 0.1f)
        {
            Vector3 move = clawMovement * moveSpeed * Time.deltaTime;

            if (clawLeft.transform.localPosition.x <= (clawLeftStart.x + clawLeftBound) && clawLeft.transform.localPosition.z <= (clawLeftStart.z + clawLeftBound) && clawLeft.transform.localPosition.x >= (clawLeftStart.x - 0.1f) && clawLeft.transform.localPosition.z >= (clawLeftStart.z - 0.1f))
            {
                
                clawLeft.transform.Translate(move, Space.World);
            }
        }
        else if (!isLeft && rightStick.magnitude > 0.1f)
        {
            Vector3 move = clawMovement * moveSpeed * Time.deltaTime;

            if (clawRight.transform.localPosition.x <= (clawRightStart.x + clawRightBound) && clawRight.transform.localPosition.z >= (clawRightStart.z - clawRightBound) && clawRight.transform.localPosition.x >= (clawRightStart.x - 0.1f) && clawRight.transform.localPosition.z <= (clawRightStart.z + 0.1f))
            {
                Debug.Log(clawRight.transform.localPosition);
                clawRight.transform.Translate(move, Space.World);
            }
        }
        else
        {
            //move with the body when no direct input for claws
            clawLeft.transform.localPosition = Vector3.Lerp(clawLeft.transform.localPosition, clawLeftStart, camSmooth);
            clawRight.transform.localPosition = Vector3.Lerp(clawRight.transform.localPosition, clawRightStart, camSmooth);
        }

        //---------------------------------------DIGGING-------------------------------------

        //digging controls ---------> only if right claw (!isLeft)
        float leftTrigger = controls.GamePlay.Dig.ReadValue<float>();
        //Debug.Log(leftTrigger);
        //make the crab dig here
        if (leftTrigger > 0f)
        {
            digAnimTimer += leftTrigger;
            if (digAnimTimer / 240.0f >= 1.0f)
            {
                digPartSystem.Play();
                terrainScript.digTerrain(crab.gameObject.transform.position, crab.gameObject.transform.rotation, leftTrigger);
                digAnimTimer = 0.0f;
            }
        }
        else
        {
            digAnimTimer = 240.0f;
            digPartSystem.Stop();
        }

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

        if(rightBumper == 1)
        {
            if (canPickup == true)
            {
                if (isLeft)
                {
                    pickedUpItem.transform.parent = clawLeft.transform;

                }
                else
                {

                   pickedUpItem.transform.parent = clawRight.transform;

                }
                ifpickedUp = true;
            }
           if (ifpickedUp == true)
            {
                if (isLeft)
                {
                    //pickedUpItem.transform.position = clawLeft.transform.position;
                    pickedUpItem.transform.position = new Vector3(clawLeft.transform.position.x, clawLeft.transform.position.y, clawLeft.transform.position.z + 0.25f);
                }
                else
                {
                    pickedUpItem.transform.position = new Vector3(clawRight.transform.position.x, clawRight.transform.position.y, clawRight.transform.position.z + 0.25f);
                }
            }
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
                ifpickedUp = false;
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
        if(other.gameObject.tag == "item")
        {
            canPickup = true;
            pickedUpItem = other.gameObject;
            Debug.Log("the item can be picked up:" + canPickup);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        canPickup = false;
    }
}
