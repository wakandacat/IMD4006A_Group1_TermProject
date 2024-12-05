using Cinemachine;
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
    public GameObject clawR_grab;
    public GameObject clawL_grab;

    //public GameObject clawLocator_R;
    //public GameObject clawLocator_L;

    //digging related vars
    private TerrainEditor terrainScript;
    public float rightTrigger = 0.0f;

    //Audio manager to activate sounds
    public GameObject audiomanager;

    //particle systems and particle control variables
    public ParticleSystem movePartSystem;
    public ParticleSystem digPartSystem;
    public ParticleSystem footstepPartSystem;
    ParticleSystem.EmissionModule digEmit;
    ParticleSystem.EmissionModule footstepEmit;
    ParticleSystem.EmissionModule moveEmit;

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
    public bool dirChange = false;

    //vectors used to set the claw targets for active/non-active claws
    private Vector3 defaultClawPos = new Vector3(1.0f, 0.25f, 0.0f);     //resting claw targ should be set to this value
    private Vector3 activeClawPos_L = new Vector3(1.428f, 0.4f, 0.262f);
    private Vector3 activeClawPos_R = new Vector3(1.428f, 0.4f, -0.262f);

    //break vars
    public float shakeAmount = 1f;
    public float shakeSpeed = 5f;
    public item pearl;
    public item shellTop;
    public item shellBottom;
    public float holdLength = 2f; 
    private float currHoldTime = 0f; 

    //for decorate
    private HomeScript homeScript;
    public GameObject droppedItemR;
    public GameObject droppedItemL;
    public GameObject decorateItemR;
    public GameObject decorateItemL;
    public bool firstItem = true;

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

    Decorate_right decorateRight;
    Decorate_Left decorateLeft;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        decorateRight = clawR_grab.GetComponent<Decorate_right>();
        decorateLeft = clawL_grab.GetComponent<Decorate_Left>();

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

        //set claw starting positions
        clawLeftStart = defaultClawPos; //start with left down
        clawRightStart = activeClawPos_R; //start with right active
        //clawRightStart = clawRight.transform.localPosition; //old code

        //locators/grab start position -> these are the gameobjects outside of the crab gameobject
        clawR_grab.transform.position = clawRight.transform.position;
        clawL_grab.transform.position = clawLeft.transform.position;

        currMoveSpeed = baseMoveSpeed;

        // Stopping the particle system by default
        movePartSystem.Stop();
        footstepPartSystem.Stop();
        digEmit = digPartSystem.emission;
        footstepEmit = footstepPartSystem.emission;
        moveEmit = movePartSystem.emission;

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
        UnityEditorInternal.InternalEditorUtility.AddTag("none");

        //text to tell player to place item on castle -> only for the first item
        if (firstItem && GameObject.Find("WorldManager").GetComponent<WorldManager>().enterFlag && (heldLeft != null || heldRight != null))
        {
            this.transform.Find("text").GetChild(0).GetComponent<TextMesh>().text = "Drop item onto sandcastle!";
        } 
        else
        {
            this.transform.Find("text").GetChild(0).GetComponent<TextMesh>().text = "";
        }

        //text for when holding breakable item -> only clam
        if (heldLeft != null)
        {
            if (heldLeft.gameObject.name == "clam")
            {
                this.transform.Find("text").GetChild(0).GetComponent<TextMesh>().text = "Break item!";
            }
        } 
        else if (heldRight != null)
        {
            if (heldRight.gameObject.name == "clam")
            {
                this.transform.Find("text").GetChild(0).GetComponent<TextMesh>().text = "Break item!";
            }
        }
        

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
            if (Mathf.Abs(angleToTarget) > 90)
            {
                if (dirChange == false)
                {
                    crabVel = Vector3.zero; //reset velocity to zero on direction change
                    dirChange = true;
                }

                //if player is not actively turning then swap sides
                if(Mathf.Abs(leftStick.x) > Mathf.Abs(leftStick.y))
                {
                    targetRotate = Quaternion.LookRotation(-moveDirection);
                }


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
            if (crab.transform.position.z < 70)
            {
                if (movePartSystem.isPlaying == false)
                {
                    moveEmit.rateOverTime = leftStick.magnitude * 30;
                    movePartSystem.Play();

                    // Found advice for changing particle emission here:
                    // https://discussions.unity.com/t/how-do-you-change-a-particle-systems-emission-rate-over-time-in-script/775702/2
                    footstepEmit.rateOverTime = 10 - (leftStick.magnitude * 5);
                    footstepPartSystem.Play();
                }
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

        //--------------------ELLENA SPECIAL TOPIC-----------------------------------
        //grab the tiltcam
        GameObject tempCam = GameObject.Find("tiltCam");
        CinemachineVirtualCamera tiltCam = tempCam.GetComponent<CinemachineVirtualCamera>();

        //grab the larger fov cam
        GameObject tempCam2 = GameObject.Find("tiltCam2");
        CinemachineVirtualCamera tiltCam2 = tempCam2.GetComponent<CinemachineVirtualCamera>();

        //grab the dig cam
        GameObject tempCam3 = GameObject.Find("digCam");
        CinemachineVirtualCamera digCam = tempCam3.GetComponent<CinemachineVirtualCamera>();

        //increase camera FOV when going very fast
        if (crabVel.magnitude >= baseMoveSpeed - 1)
        {
            tiltCam2.Priority = tiltCam.Priority + 1;
        } 
        //if crab is digging currently, then change to digcam
        else if (rightTrigger > 0.1f || crab.transform.position.y <= 2.3f)
        {
            if (tiltCam.Priority > tiltCam2.Priority)
            {
                digCam.Priority = tiltCam.Priority + 1;
            }
            else
            {
                digCam.Priority = tiltCam2.Priority + 1;
            }

        }
        else
        {
            if (tiltCam2.Priority > digCam.Priority)
            {
                tiltCam.Priority = tiltCam2.Priority + 1;
            }
            else
            {
                tiltCam.Priority = digCam.Priority + 1;
            }

        }

        //---------------------------------------CLAWMOVEMENT-------------------------------------

        //claw movement controls
        rightStick = controls.GamePlay.Claw.ReadValue<Vector2>();

        //input from controls move the crab in xz plane -> take into account camera rotation here as well
        Vector3 clawMovement = ((camForward * rightStick.y) + (camRight * rightStick.x)) * currMoveSpeed * Time.deltaTime;
        //Vector3 clawMove = clawMovement * baseMoveSpeed * Time.deltaTime;

        // Finding the new claw locator positions
        //clawLeftStart = (clawLeft.transform.position);
        //clawRightStart = (clawRight.transform.position);

        //clawLeft.transform.rotation = clawLocator_L.transform.rotation;
        //clawRight.transform.rotation = clawLocator_R.transform.rotation;

        //update external gameobjects to match the internal claw positions
        clawR_grab.transform.position = clawRight.transform.position;
        clawL_grab.transform.position = clawLeft.transform.position;

        clawR_grab.transform.rotation = clawRight.transform.rotation;
        clawL_grab.transform.rotation = clawLeft.transform.rotation;

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

            //get the forward direction to calculate the angle from -> crab local position forward
            Vector3 forwardDirection = crab.transform.InverseTransformDirection(claw.transform.forward);
            //Vector3 forwardDirection = claw.transform.InverseTransformDirection(claw.transform.forward);

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
                currHoldTime += (Time.deltaTime * leftTrigger); //count the time
               //AudioManager.instance.sfxPlayer(3);
               //add breaking build up noise here? potentially a coroutine

                if (currHoldTime >= holdLength) //broke the object
                {
                    GameObject toBreak = heldLeft.gameObject;
                    reduceWeight(toBreak);

                    //spawn the pearl in the claw
                    //heldLeft = Instantiate(pearl.gameObject, heldLeft.transform.position, Quaternion.identity);
                    heldLeft = Instantiate(pearl.gameObject, GameObject.Find("jnt_L_tip").transform.position, Quaternion.identity);
                    heldLeft.transform.parent = GameObject.Find("jnt_L_tip").transform;

                    addWeight(heldLeft);

                    //delete the clam
                    Destroy(toBreak);

                    //play break nosie
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

                currHoldTime += (Time.deltaTime * leftTrigger); //count the time
                //AudioManager.instance.sfxPlayer(3);

                if (currHoldTime >= holdLength) //broke the object
                {
                    GameObject toBreak = heldRight.gameObject;
                    reduceWeight(toBreak);

                    //spawn the pearl in the claw
                    //heldRight = Instantiate(pearl.gameObject, heldRight.transform.position, Quaternion.identity);
                    heldRight = Instantiate(pearl.gameObject, GameObject.Find("jnt_R_tip").transform.position, Quaternion.identity);
                    heldRight.transform.parent = GameObject.Find("jnt_R_tip").transform;

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
        if (rightTrigger > 0f && crab.transform.position.z < 70)
        {

            if (isLeft)
            {
                terrainScript.digTerrain(clawLeft.gameObject.transform.position, crab.gameObject.transform.rotation, rightTrigger);

                //animate claw movement applied to left
            }
            else
            {
                terrainScript.digTerrain(clawRight.gameObject.transform.position, crab.gameObject.transform.rotation, rightTrigger);

                //animate claw movement applied to right
                Vector3 digPosDown = clawRight.transform.localPosition + new Vector3(0.0f, -0.6f, 0.0f); //should take current claw pos and only make the y value decrease
                Vector3 digPosUp = clawRight.transform.localPosition + new Vector3(0.0f, 0.4f, 0.0f); //should take current claw pos and only make the y value decrease
                clawRight.transform.localPosition = Vector3.Lerp(clawRight.transform.localPosition, digPosDown, Time.deltaTime * 1.0f); //lerp down
                //clawRight.transform.localPosition = Vector3.Lerp(clawRight.transform.localPosition, digPosUp, Time.deltaTime * 5.0f); //lerp back up?
                //Debug.Log("Local Pos: " + clawRight.transform.localPosition + ", newDigPos: " + newDigPos);
            }

            // Found advice for changing particle emission here:
            // https://discussions.unity.com/t/how-do-you-change-a-particle-systems-emission-rate-over-time-in-script/775702/2
            digEmit.rateOverTime = rightTrigger * 30;
            digPartSystem.Play();

        }
        else
        {
            digPartSystem.Stop();
            //may need to reset claw pos here

        }


    }
       
//toggle the active claw
    public void OnSwitchLeft(InputAction.CallbackContext context)
    {

        if (!isLeft && clawRight.transform.position.y > -0.5)
        {
            clawRightStart = defaultClawPos;
            clawLeftStart = activeClawPos_L;
            AudioManager.instance.sfxPlayer(2);
        }

        isLeft = true;

    }

    public void OnSwitchRight(InputAction.CallbackContext context)
    {

        if (isLeft && clawLeft.transform.position.y > -0.5)
        {
            clawRightStart = activeClawPos_R;
            clawLeftStart = defaultClawPos;

            AudioManager.instance.sfxPlayer(2);
        }

        isLeft = false;

    }

    public void OnPickDropControls(InputAction.CallbackContext context)
    {
       // Debug.Log("this is pick up");
        if (!isLeft)
        {
            Debug.Log("inside the R3 first if");
            if (ifpickedUpR == false)
            {
                ifpickedUpR = pickUpItemRight(clawR_grab);
                Debug.Log("inside ifpickedUpR == false");

            }
            else
            {
                dropItemR();
                ifpickedUpR = false;
                Debug.Log("inside first else");

            }

        }
        else
        {
            if (ifpickedUpL == false)
            {
                ifpickedUpL = pickUpItemLeft(clawL_grab);

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

    public bool pickUpItemRight(GameObject clawR_grab)
    {
        if (Rpickedup == false && canPickupR == true)
        {
           // Debug.Log("you are here");
           Vector3 clawRItemPos = GameObject.Find("jnt_R_tip").transform.position;
            rightItem.GetComponent<Collider>().enabled = false;
            //Vector3 itemRHoldPos = new Vector3(clawR_grab.transform.position.x, clawR_grab.transform.position.y + 0.1f, clawR_grab.transform.position.z - 0.2f);
            Vector3 itemRHoldPos = new Vector3(clawRItemPos.x, clawRItemPos.y + 0.2f, clawRItemPos.z - 0.2f); // <----------- Add offset here
            rightItem.transform.position = itemRHoldPos;
            //rightItem.transform.parent = clawR_grab.transform;
            rightItem.transform.parent = GameObject.Find("jnt_R_tip").transform;

            heldRight = rightItem;
            addWeight(heldRight);

            Rpickedup = true;

            //play pick up sound
            AudioManager.instance.sfxPlayer(0);

        }

        return Rpickedup;
    }

    public bool pickUpItemLeft(GameObject clawL_grab)
    {
        if (Lpickedup == false && canPickupL == true)
        {
            // Debug.Log("you are here");
            Vector3 clawLItemPos = GameObject.Find("jnt_L_tip").transform.position;
            leftItem.GetComponent<Collider>().enabled = false;
            //Vector3 itemLHoldPos = new Vector3(clawL_grab.transform.position.x, clawL_grab.transform.position.y + 0.1f, clawL_grab.transform.position.z - 0.2f);
            Vector3 itemLHoldPos = new Vector3(clawLItemPos.x, clawLItemPos.y + 0.2f, clawLItemPos.z - 0.2f);
            leftItem.transform.position = itemLHoldPos;
            //leftItem.transform.position = clawLeft.transform.position;
            //leftItem.transform.parent = clawL_grab.transform;
            leftItem.transform.parent = GameObject.Find("jnt_L_tip").transform;

            //if you are in the home then outline the sandcastle
            if (GameObject.Find("WorldManager").GetComponent<WorldManager>().enterFlag)
            {
                //outline castle to tell player to place item
                var outline = GameObject.Find("newSandCastle").gameObject.GetComponent<Outline>();

                outline.OutlineWidth = 5;
            }


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
        droppedItemR = heldRight;

        //Debug.Log("right item" + heldRight);
        //homeScript.decorateItem(heldRight);
        decorateItemR = droppedItemR;
        Debug.Log("decorate Right" + decorateRight.castleCollision);
        if (decorateRight.castleCollision == true)
        {
            Debug.Log("(decorateRight.castleCollision" + decorateRight.castleCollision);
            Debug.Log("The point is" + decorateRight.Cpoint.point);
            decorateItemR.transform.position = decorateRight.Cpoint.point;
            homeScript.decorateItem(decorateItemR);
            decorateItemR.tag = "none";
            decorateItemR.GetComponent<Collider>().enabled = false;
            decorateItemR.GetComponent<Outline>().enabled = false;

            //turn off first item text
            if (firstItem && heldLeft == null)
            {
                firstItem = false;

            }

        }
        else
        {
            heldRight.transform.position = new Vector3(heldRight.transform.position.x, 2.9f, heldRight.transform.position.z);
    
        }

        Debug.Log("we are here here!");
        heldRight.transform.parent = null;
        heldRight.GetComponent<Collider>().enabled = true;
        Rpickedup = false;
        heldRight = null;
        decorateItemR = null;
        droppedItemR = null;

        //play drop sound
        AudioManager.instance.sfxPlayer(1);

        //Debug.Log("dropped");
    }

    public void dropItemL()
    {
        reduceWeight(heldLeft);
        droppedItemL = heldLeft;

        //Debug.Log("right item" + heldRight);
        //homeScript.decorateItem(heldRight);
        decorateItemL = droppedItemL;
        Debug.Log("decorate Left" + decorateLeft.castleCollision);
        if (decorateLeft.castleCollision == true)
        {
            Debug.Log("(decorateLeft.castleCollision" + decorateLeft.castleCollision);
            Debug.Log("The point is" + decorateLeft.Cpoint.point);
            decorateItemL.transform.position = decorateLeft.Cpoint.point;
            homeScript.decorateItem(decorateItemL);
            decorateItemL.tag = "none";
            decorateItemL.GetComponent<Collider>().enabled = false;
            decorateItemL.GetComponent<Outline>().enabled = false;

            //unoutline the castle
            //if (heldRight == null)
            //{
            //    var outline = GameObject.Find("newSandCastle").gameObject.GetComponent<Outline>();

            //    outline.OutlineWidth = 0;
            //}

            //turn off first item text
            if (firstItem && heldRight == null)
            {
                firstItem = false;

            }

        }
        else
        {
            heldLeft.transform.position = new Vector3(heldLeft.transform.position.x, 2.9f, heldLeft.transform.position.z);

        }

        Debug.Log("we are here here!");
        heldLeft.transform.parent = null;
        heldLeft.GetComponent<Collider>().enabled = true;
        Lpickedup = false;
        heldLeft = null;
        decorateItemL = null;
        droppedItemL = null;

        //play drop sound
        AudioManager.instance.sfxPlayer(1);
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

    public void DecorateRight()
    {
        if(isLeft && Rpickedup == true)
        {

        }
    }


}
