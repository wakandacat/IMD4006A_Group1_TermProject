using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
//using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    //crab object being controlled by inputs
    public GameObject crab;
    public GameObject camera;
    public GameObject clawRight;
    public GameObject clawLeft;

    public GameObject clawLocator_R;
    public GameObject clawLocator_L;

    //digging related vars
    private TerrainEditor terrainScript;
    public float rightTrigger = 0.0f;

    //Audio manager to activate sounds
    public GameObject audiomanager;

    //particle systems and particle control variables
    public ParticleSystem movePartSystem;
    public ParticleSystem digPartSystem;
    public ParticleSystem footstepPartSystem;
    public float digAnimTimer = 20.0f;

    //input action asset that reads controller inputs
    PlayerControls controls;

    //global variables

    //basic movement vars
    public Vector2 leftStick = Vector2.zero;
    public Vector2 rightStick = Vector2.zero;
    public float baseMoveSpeed = 7f;
    public float currMoveSpeed;
    public float rotateSpeed = 0.8f;
    public float accelRate;
    private Vector3 crabVel = Vector3.zero;

    private float currTerrHeight;
    public float crabY;

    public float crabSmooth = 0.2f;
    public float camSmooth = 0.05f;
    public bool isLeft = false; //right by default
    private Vector3 camOffset;
    private Vector3 clawLeftStart;
    private Vector3 clawRightStart;
    public float clawSmooth;
    public float clawAngle;
    public float maxClawDistance;
    private bool dirChange = false;

    private Vector3 clawDrop = new Vector3(0.0f, -0.1f, 0.0f);
    private Vector3 clawRaise = new Vector3(0.0f, 0.1f, 0.0f);

    //break vars
    public float shakeAmount = 1f;
    public float shakeSpeed = 5f;
    public item pearl;
    public item shellTop;
    public item shellBottom;
    public float holdLength = 2f; 
    private float currHoldTime = 0f; 

    //for decorate
    public HomeScript homeScript;

    //Booleans
    bool ifpickedUp;


    [SerializeField] public Rigidbody _rb;

    GameObject pickedUpItem;
    private GameObject currentHoldingClaw;

    //pick up drop specific variables
    public bool canPickupR = false;
    public bool canPickupL = false;

    public bool Rpickedup = false;
    public bool Lpickedup = false;

    bool ifpickedUpR = false;
    bool ifpickedUpL = false;

    public GameObject leftItem;
    public GameObject rightItem;
    public GameObject heldRight;
    public GameObject heldLeft;

    // Start is called before the first frame update
    void Start()
    {
        //instantiate a new input action object and enable it
        controls = new PlayerControls();
        controls.Enable();

        //setup callback function to switch active claws
        //+= refers to adding a callback function
        controls.GamePlay.SwitchToLeft.performed += OnSwitchLeft;
        controls.GamePlay.SwitchToRight.performed += OnSwitchRight;
        controls.GamePlay.GrabDrop.performed += OnPickDropControls;

        //camera distance from player
        camOffset = Camera.main.transform.position - crab.transform.position;

        if (isLeft)
        {
            clawLocator_R.transform.Translate(clawDrop);
        }
        else
        {
            clawLocator_L.transform.Translate(clawDrop);
        }

        //grab starting positions
        clawLeftStart = clawLocator_L.transform.localPosition;
        clawRightStart = clawLocator_R.transform.localPosition;
        //clawLeftStart = clawLeft.transform.localPosition;
        //clawRightStart = clawRight.transform.localPosition;

        currMoveSpeed = baseMoveSpeed;

        // Stopping the particle system by default
        movePartSystem.Stop();
        footstepPartSystem.Stop();

        // Reminder on how to do this came from: https://youtu.be/gFwf_T8_8po?si=knchWQ0Sk1b1Lmna
        terrainScript = GameObject.FindGameObjectWithTag("TerrManager").GetComponent<TerrainEditor>();

        homeScript = GameObject.Find("HomeArea").GetComponent<HomeScript>();

        //setting up pick up drop

        //start sound coroutine for walking and arm movement
        StartCoroutine(audiomanager.GetComponent<AudioManager>().walkTimer());
        StartCoroutine(audiomanager.GetComponent<AudioManager>().armMoveTimer());
        StartCoroutine(audiomanager.GetComponent<AudioManager>().digSoundTimer());
        AudioManager.instance.ambientSource.Play();

        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        //---------------------------------------BASICMOVEMENT-------------------------------------

        //read in the controller inputs
        leftStick = controls.GamePlay.Walk.ReadValue<Vector2>();

        //make sure to keep control scheme corresponding to camera's rotation
        //https://discussions.unity.com/t/what-is-vector3-scale-use-for-in-this-code/632022
        //https://www.youtube.com/watch?v=reWtxGTyN78&ab_channel=TheGameDevCave
        Vector3 camForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = Vector3.Scale(Camera.main.transform.right, new Vector3(1, 0, 1)).normalized;

        //input from controls move the crab in xz plane
        Vector3 moveDirection = ((camForward * leftStick.y) + (camRight * leftStick.x)) * currMoveSpeed;

        //ensure crab does not move when not reading inputs (including rotation)
        //but allow for some deceleration
        if (leftStick.magnitude > 0.1f)
        {

            //rotate the crab with player motion to face the z-direction
            Quaternion targetRotate = Quaternion.LookRotation(moveDirection);

            //get forward direction of crab (positive z-direction)
            Vector3 currentForward = transform.forward;

            //get angle between forward direction and target direction
            float angleToTarget = Vector3.SignedAngle(currentForward, moveDirection, Vector3.up);

            //determine if -z or +z direction is closest to target and make that the front of the crab
            //try to only adjust forward direction when not holding continuously and rotating
            if (Mathf.Abs(angleToTarget) > 90 && leftStick.magnitude > 0.3f)
            {
                if (dirChange == false)
                {
                    crabVel = Vector3.zero; //reset velocity to zero on direction change
                    dirChange = true;
                }
                
                targetRotate = Quaternion.LookRotation(-moveDirection);

            } else
            {
                dirChange = false;
            }

            //for large rotations, rotate first, then move
            if (Mathf.Abs(Mathf.Abs(crab.transform.eulerAngles.y) - Mathf.Abs(targetRotate.eulerAngles.y)) < 40)
            {
                //calculate the velocity
                if (moveDirection != Vector3.zero)
                {
                    crabVel = Vector3.Lerp(crabVel, moveDirection, accelRate * Time.deltaTime);
                } 

            }

            //rotate the crab but not on deceleration
            if (leftStick.magnitude > 0.1f)
            {
                crab.transform.rotation = Quaternion.Slerp(crab.transform.rotation, targetRotate, rotateSpeed * Time.deltaTime);

            }

            // Figuring out the crab's Y position, relative to the terrain
            currTerrHeight = terrainScript.getTerrainHeight(crab.gameObject.transform.position);


            //--------------------CAMERA-----------------------------
            //main camera follows behind player when walking
            Vector3 rotatedOffset = crab.transform.rotation * new Vector3(-camOffset.x, camOffset.y, -camOffset.z); //adjust offset direction based on crabs rotation
            Vector3 targetPos = crab.transform.position + rotatedOffset;

            Vector3 smoothPos = Vector3.Lerp(Camera.main.transform.position, targetPos, crabSmooth);
            Camera.main.transform.position = smoothPos;

            float smoothRot = Mathf.LerpAngle(Camera.main.transform.eulerAngles.y, crab.transform.eulerAngles.y + 90, camSmooth);
            Camera.main.transform.rotation = Quaternion.Euler(20, smoothRot, 0);


            // Playing the particle system
            if (movePartSystem.isPlaying == false)
            {
                movePartSystem.Play();
                footstepPartSystem.Play();
            }
        }
        else
        {

            // Stopping the particle system when the movement stops
            if (movePartSystem.isPlaying == true)
            {
                movePartSystem.Stop();
                footstepPartSystem.Stop();
            }

            //decelerate
            crabVel = Vector3.Lerp(crabVel, Vector3.zero, accelRate * Time.deltaTime);

        }

        //move the crab
        crab.transform.Translate(crabVel * Time.deltaTime, Space.World);

        //---------------------------------------CLAWMOVEMENT-------------------------------------

        //claw movement controls
        rightStick = controls.GamePlay.Claw.ReadValue<Vector2>();

        //input from controls move the crab in xz plane -> take into account camera rotation here as well
        Vector3 clawMovement = ((camForward * rightStick.y) + (camRight * rightStick.x)) * currMoveSpeed * Time.deltaTime;
        //Vector3 clawMove = clawMovement * baseMoveSpeed * Time.deltaTime;

        // Finding the new claw locator positions
        clawLeftStart = (clawLocator_L.transform.position);
        clawRightStart = (clawLocator_R.transform.position);

        clawLeft.transform.rotation = clawLocator_L.transform.rotation;
        clawRight.transform.rotation = clawLocator_R.transform.rotation;

        //moving the left claw, right claw just follows body
        if (isLeft && rightStick.magnitude > 0.1f)
        {
            clawLeft.transform.Translate(clawMovement, Space.World);

            clawRight.transform.localPosition = Vector3.Lerp(clawRight.transform.localPosition, clawRightStart, clawSmooth * Time.deltaTime);

            ClampClaw(clawLeft, clawLeftStart); //clamp into an arc
        }
        else if (!isLeft && rightStick.magnitude > 0.1f) //moving the right claw, left claw just follows body
        {
            clawRight.transform.Translate(clawMovement, Space.World);

            clawLeft.transform.localPosition = Vector3.Lerp(clawLeft.transform.localPosition, clawLeftStart, clawSmooth * Time.deltaTime);

            ClampClaw(clawRight, clawRightStart); //clamp into an arc

        }
        else //move both claws with the body when no direct input
        {
            clawLeft.transform.localPosition = Vector3.Lerp(clawLeft.transform.localPosition, clawLeftStart, clawSmooth * Time.deltaTime);
            clawRight.transform.localPosition = Vector3.Lerp(clawRight.transform.localPosition, clawRightStart, clawSmooth * Time.deltaTime);
        }

        // keep claws close to crab body in an arc
        void ClampClaw(GameObject claw, Vector3 clawStart)
        {
            
            Vector3 directionFromStart = claw.transform.localPosition - clawStart;
            //clamp the claw into a set radius
            //https://stackoverflow.com/questions/70501814/mathf-clamp-inside-of-a-sphere-radius-unity
            directionFromStart = Vector3.ClampMagnitude(directionFromStart, maxClawDistance);

            //get the forward direction to calculate the angle from
            Vector3 forwardDirection = crab.transform.right;
            float angle = Vector3.SignedAngle(forwardDirection, directionFromStart, Vector3.up);

            //stay within a subset of the radius
            if (angle > -clawAngle || angle < clawAngle)
            {
                //remember how to use clamp with some help from https://discussions.unity.com/t/clamping-angle-between-two-values/782349
                float clampedAngle = Mathf.Clamp(angle, -clawAngle, clawAngle);
                Quaternion rotation = Quaternion.Euler(0, clampedAngle, 0);

                directionFromStart = rotation * forwardDirection * directionFromStart.magnitude;

            }

            //move the claw
            claw.transform.localPosition = clawStart + directionFromStart;

        }

        //---------------------------------------BREAKING-------------------------------------

        //breaking controls ---------> only if a claw is available
        float leftTrigger = controls.GamePlay.Dig.ReadValue<float>();

        //if crab is holding something breakable in active claw
        if (leftTrigger > 0.1f)
        {
            //get some random jitter
            float xOffset = UnityEngine.Random.Range(-shakeAmount * leftTrigger, shakeAmount * leftTrigger);
            float yOffset = UnityEngine.Random.Range(-shakeAmount * leftTrigger, shakeAmount * leftTrigger);
            float zOffset = UnityEngine.Random.Range(-shakeAmount * leftTrigger, shakeAmount * leftTrigger);

            if (isLeft && heldLeft != null && heldLeft.gameObject.GetComponent<item>().breakable == true) //breaking the object
            {
                //move the claw
                Vector3 newPos = clawLeftStart + new Vector3(xOffset, yOffset, zOffset);
                clawLeft.transform.localPosition = Vector3.Lerp(clawLeft.transform.localPosition, newPos, Time.deltaTime * shakeSpeed);

               // Debug.Log(currHoldTime);
                currHoldTime += Time.deltaTime; //count the time
               //AudioManager.instance.sfxPlayer(3);

                if (leftTrigger == 1f && currHoldTime >= holdLength) //broke the object
                {
                    GameObject toBreak = heldLeft.gameObject;
                    reduceWeight(toBreak);

                    //spawn the pearl in the claw
                    heldLeft = Instantiate(pearl.gameObject, heldLeft.transform.position, Quaternion.identity);
                    heldLeft.transform.parent = clawLeft.transform;

                    addWeight(heldLeft);

                    //delete the clam
                    Destroy(toBreak);

                    //play break nosie
                    //AudioManager.instance.sfxSource.Stop();
                    AudioManager.instance.sfxPlayer(4);

                    //spawn shell top and bottom beside the crab
                    Instantiate(shellTop.gameObject, new Vector3(clawLeft.transform.position.x - 0.5f, 3f, clawLeft.transform.position.z), Quaternion.identity);
                    Instantiate(shellBottom.gameObject, new Vector3(clawLeft.transform.position.x + 0.5f, 3f, clawLeft.transform.position.z), Quaternion.identity);

                    currHoldTime = 0f; //reset time
                }
            }
            else if (!isLeft && heldRight != null && heldRight.gameObject.GetComponent<item>().breakable == true) //breaking the object
            {
                //move the claw
                Vector3 newPos = clawRightStart + new Vector3(xOffset, yOffset, zOffset);
                clawRight.transform.localPosition = Vector3.Lerp(clawRight.transform.localPosition, newPos, Time.deltaTime * shakeSpeed);

                //Debug.Log(currHoldTime);
                currHoldTime += Time.deltaTime; //count the time
                //AudioManager.instance.sfxPlayer(3);

                if (leftTrigger == 1f && currHoldTime >= holdLength) //broke the object
                {
                    GameObject toBreak = heldRight.gameObject;
                    reduceWeight(toBreak);

                    //spawn the pearl in the claw
                    heldRight = Instantiate(pearl.gameObject, heldRight.transform.position, Quaternion.identity);
                    heldRight.transform.parent = clawLeft.transform;

                    addWeight(heldRight);

                    //delete the clam
                    Destroy(toBreak);

                    //play break nosie
                    //AudioManager.instance.sfxSource.Stop();
                    AudioManager.instance.sfxPlayer(4);

                    //spawn shell top and bottom beside the crab
                    Instantiate(shellTop.gameObject, new Vector3(clawRight.transform.position.x - 0.5f, 3f, clawRight.transform.position.z), Quaternion.identity);
                    Instantiate(shellBottom.gameObject, new Vector3(clawRight.transform.position.x + 0.5f, 3f, clawRight.transform.position.z), Quaternion.identity);

                    currHoldTime = 0f; //reset time
                }
            }

        } 
        else
        {
            currHoldTime = 0f;
            //AudioManager.instance.sfxSource.Stop();
        }

        //---------------------------------------DIGGING-------------------------------------

        //digging controls ---------> only if a claw is available
        rightTrigger = controls.GamePlay.Break.ReadValue<float>();
        //Debug.Log(rightTrigger);
        //make the crab dig here
        if (rightTrigger > 0f)
        {
            digAnimTimer += rightTrigger;

            if (isLeft)
            {
                terrainScript.digTerrain(clawLeft.gameObject.transform.position, crab.gameObject.transform.rotation, rightTrigger);
            }
            else
            {
                terrainScript.digTerrain(clawRight.gameObject.transform.position, crab.gameObject.transform.rotation, rightTrigger);
            }
            

            if (digAnimTimer / 20.0f >= 1.0f)
            {
               // Debug.Log("Dig Particles Deployed");
                digPartSystem.Play();
                digAnimTimer = 0.0f;
            }
        }
        else
        {
            digAnimTimer = 20.0f;
            digPartSystem.Stop();

            //play digging audio --> need to update post alpha
            //AudioManager.instance.digSource.Play();
        }


    }


       
//toggle the active claw
    public void OnSwitchLeft(InputAction.CallbackContext context)
    {

        if (!isLeft && clawLocator_R.transform.position.y > -0.5)
        {
            clawLocator_R.transform.Translate(clawDrop);
            clawLocator_L.transform.Translate(clawRaise);

            AudioManager.instance.sfxPlayer(2);
        }

        isLeft = true;

    }

    public void OnSwitchRight(InputAction.CallbackContext context)
    {

        if (isLeft && clawLocator_L.transform.position.y > -0.5)
        {
            clawLocator_R.transform.Translate(clawRaise);
            clawLocator_L.transform.Translate(clawDrop);


            AudioManager.instance.sfxPlayer(2);
        }

        isLeft = false;

    }

    public void OnPickDropControls(InputAction.CallbackContext context)
    {
       // Debug.Log("this is pick up");
        if (!isLeft)
        {
            if (ifpickedUpR == false)
            {
                ifpickedUpR = pickUpItemRight(clawRight);

            }
            else
            {
                dropItemR();
                ifpickedUpR = false;
               
            }

        }
        else
        {
            if (ifpickedUpL == false)
            {
                ifpickedUpL = pickUpItemLeft(clawLeft);

            }
            else
            {
                dropItemL();
                ifpickedUpL = false;
                
            }
        }
    }

    public void updateRClawStatus(GameObject collItem, bool canPickup)
    {
        rightItem = collItem;
        canPickupR = canPickup;
    }

    public void updateLClawStatus(GameObject collItem, bool canPickup)
    {
        leftItem = collItem;
        canPickupL = canPickup;
    }

    public bool pickUpItemRight(GameObject clawRight)
    {
        if (Rpickedup == false && canPickupR == true)
        {
           // Debug.Log("you are here");
            rightItem.GetComponent<Collider>().enabled = false;
            Vector3 itemRHoldPos = new Vector3(clawRight.transform.position.x, clawRight.transform.position.y + 0.1f, clawRight.transform.position.z - 0.2f);
            rightItem.transform.position = itemRHoldPos;
            rightItem.transform.parent = clawRight.transform;

            heldRight = rightItem;
            addWeight(heldRight);

            Rpickedup = true;

            //play pick up sound
            AudioManager.instance.sfxPlayer(0);
        }
       // Debug.Log("pickedup is" + Rpickedup);
       // Debug.Log("canPickupR is " + canPickupR);
       //Debug.Log("this is in the pickupdrop script right item is: " + heldRight);
        return Rpickedup;
    }

    public bool pickUpItemLeft(GameObject clawLeft)
    {
        if (Lpickedup == false && canPickupL == true)
        {
           // Debug.Log("you are here");
            leftItem.GetComponent<Collider>().enabled = false;
            Vector3 itemLHoldPos = new Vector3(clawLeft.transform.position.x, clawLeft.transform.position.y + 0.1f, clawLeft.transform.position.z - 0.2f);
            leftItem.transform.position = itemLHoldPos;
            //leftItem.transform.position = clawLeft.transform.position;
            leftItem.transform.parent = clawLeft.transform;

            heldLeft = leftItem;
            addWeight(heldLeft);

            Lpickedup = true;

            //play pick up sound
            AudioManager.instance.sfxPlayer(0);
        }
        //Debug.Log("canPickup is " + canPickupL);
        return Lpickedup;
    }

    public void dropItemR()
    {

        reduceWeight(heldRight);
        //Debug.Log("right item" + heldRight);
        homeScript.decorateItem(heldRight);
        heldRight.transform.parent = null;

        heldRight.GetComponent<Collider>().enabled = true;
        Rpickedup = false;
        heldRight = null;
        //play drop sound
        AudioManager.instance.sfxPlayer(1);

        //Debug.Log("dropped");
    }

    public void dropItemL()
    {        
        reduceWeight(heldLeft);
        // Debug.Log("left item" + heldLeft);
        homeScript.decorateItem(heldLeft);
        heldLeft.transform.parent = null;

        heldLeft.GetComponent<Collider>().enabled = true;
        Lpickedup = false;
        heldLeft = null;
        //play drop sound
        AudioManager.instance.sfxPlayer(1);

        //Debug.Log("dropped");
    }

    public void addWeight(GameObject heldItem)
    {
        //move the crab and take into account the weight of any held objects
        if (heldItem != null)
        {          
            //item weight affects movement speed of crab
            currMoveSpeed = currMoveSpeed - heldItem.gameObject.GetComponent<item>().itemWeight;
        }
        else
        {
            currMoveSpeed = baseMoveSpeed;
        }
    }

    public void reduceWeight(GameObject droppedItem)
    {

      currMoveSpeed = currMoveSpeed + droppedItem.gameObject.GetComponent<item>().itemWeight;
        
    }

}
