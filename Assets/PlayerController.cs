using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public float digAnimTimer = 20.0f;

    //input action asset that reads controller inputs
    PlayerControls controls;

    //global variables

    //basic movement vars
    public float baseMoveSpeed = 7f;
    public float currMoveSpeed;
    public float rotateSpeed = 0.8f;
    public float accelRate = 1.5f;
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

    //break vars
    public float shakeAmount = 1f;
    public float shakeSpeed = 5f;
    public item pearl;
    public item shellTop;
    public item shellBottom;
    public float holdLength = 2f; 
    private float currHoldTime = 0f; 

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
        controls.GamePlay.Switch.performed += OnSwitch;
        controls.GamePlay.PickUpPutDown.performed += OnPickDropControls;

        //camera distance from player
        camOffset = Camera.main.transform.position - crab.transform.position;

        //grab starting positions
        clawLeftStart = clawLeft.transform.localPosition;
        clawRightStart = clawRight.transform.localPosition;

        // Stopping the particle system by default
        movePartSystem.Stop();

        // Reminder on how to do this came from: https://youtu.be/gFwf_T8_8po?si=knchWQ0Sk1b1Lmna
        terrainScript = GameObject.FindGameObjectWithTag("TerrManager").GetComponent<TerrainEditor>();

        //setting up pick up drop


        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        //walking controls

        //---------------------------------------BASICMOVEMENT-------------------------------------

        //read in the controller inputs
        Vector2 leftStick = controls.GamePlay.Walk.ReadValue<Vector2>();

        //make sure to keep control scheme corresponding to camera's rotation
        Vector3 camForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camRight = Vector3.Scale(Camera.main.transform.right, new Vector3(1, 0, 1)).normalized;

        //ensure crab does not move when not reading inputs (including rotation)
        //but allow for some deceleration
        if (leftStick.magnitude > 0.1f || crabVel.magnitude > 3.0f)
        {

            //input from controls move the crab in xz plane
            Vector3 moveDirection = ((camForward * leftStick.y) + (camRight * leftStick.x)).normalized;


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
                //move the crab
                crabVel = Vector3.Lerp(crabVel, moveDirection * currMoveSpeed, accelRate * Time.deltaTime);
                crab.transform.Translate(crabVel * Time.deltaTime, Space.World);
            }

            //rotate the crab but not on deceleration
            if (leftStick.magnitude > 0.1f)
            {
                crab.transform.rotation = Quaternion.Slerp(crab.transform.rotation, targetRotate, rotateSpeed * Time.deltaTime);
            }

            //move the crab and take into account the weight of any held objects
            if (ifpickedUp)
            {
                //item weight affects movement speed of crab
                currMoveSpeed = baseMoveSpeed - pickedUpItem.GetComponent<item>().itemWeight;
            } else
            {
                currMoveSpeed = baseMoveSpeed;
            }

            // Figuring out the crab's Y position, relative to the terrain
            currTerrHeight = terrainScript.getTerrainHeight(crab.gameObject.transform.position);

            if (currTerrHeight != 0.005f)
            {

            }
            else
            {

            }

            //main camera follows behind player when walking
            Vector3 rotatedOffset = crab.transform.rotation * new Vector3(-camOffset.x, camOffset.y, -camOffset.z); //adjust offset direction based on crabs rotation
             Vector3 targetPos = crab.transform.position + rotatedOffset;
            //Vector3 targetPos = crab.transform.position + camOffset;

            Vector3 smoothPos = Vector3.Lerp(Camera.main.transform.position, targetPos, crabSmooth);
            Camera.main.transform.position = smoothPos;

            float smoothRot = Mathf.LerpAngle(Camera.main.transform.eulerAngles.y, crab.transform.eulerAngles.y + 90, camSmooth);
            Camera.main.transform.rotation = Quaternion.Euler(20, smoothRot, 0);


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

        //input from controls move the crab in xz plane -> take into account camera rotation here as well
        Vector3 clawMovement = ((camForward * rightStick.y) + (camRight * rightStick.x)).normalized;
        Vector3 clawMove = clawMovement * baseMoveSpeed * Time.deltaTime;

        //move the active claw
        if (isLeft && rightStick.magnitude > 0.1f)
        {
            clawLeft.transform.Translate(clawMove, Space.World);

            MoveClaw(clawLeft); //clamp into an arc
        }
        else if (!isLeft && rightStick.magnitude > 0.1f)
        {
            clawRight.transform.Translate(clawMove, Space.World);

            MoveClaw(clawRight); //clamp into an arc

        }
        else
        {
            //move with the body when no direct input for claws
            clawLeft.transform.localPosition = Vector3.Lerp(clawLeft.transform.localPosition, clawLeftStart, clawSmooth);
            clawRight.transform.localPosition = Vector3.Lerp(clawRight.transform.localPosition, clawRightStart, clawSmooth);
        }

        // keep claws close to crab body in an arc
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

                Debug.Log(currHoldTime);
                currHoldTime += Time.deltaTime; //count the time

                if (leftTrigger == 1f && currHoldTime >= holdLength) //broke the object
                {
                    GameObject toBreak = heldLeft.gameObject;

                    //spawn the pearl in the claw
                    heldLeft = Instantiate(pearl.gameObject, heldLeft.transform.position, Quaternion.identity);
                    heldLeft.transform.parent = clawLeft.transform;

                    //delete the clam
                    Destroy(toBreak);

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

                Debug.Log(currHoldTime);
                currHoldTime += Time.deltaTime; //count the time

                if (leftTrigger == 1f && currHoldTime >= holdLength) //broke the object
                {
                    GameObject toBreak = heldRight.gameObject;

                    //spawn the pearl in the claw
                    heldRight = Instantiate(pearl.gameObject, heldRight.transform.position, Quaternion.identity);
                    heldRight.transform.parent = clawRight.transform;

                    //delete the clam
                    Destroy(toBreak);

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
        }

        //---------------------------------------DIGGING-------------------------------------

        //digging controls ---------> only if a claw is available
        float rightTrigger = controls.GamePlay.Break.ReadValue<float>();
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
                Debug.Log("Dig Particles Deployed");
                digPartSystem.Play();
                digAnimTimer = 0.0f;
            }
        }
        else
        {
            digAnimTimer = 20.0f;
            digPartSystem.Stop();

            //play digging audio
            AudioManager.instance.digSource.Play();
        }

        //---------------------------------------PICK UP AND DROP-------------------------------------

        //grab controls ---------> either claw
        float rightBumper = controls.GamePlay.PickUpPutDown.ReadValue<float>();
        //Debug.Log(rightBumper);
        //make the crab grab here
     
    }

    //toggle the active claw
    public void OnSwitch(InputAction.CallbackContext context)
    {
        isLeft = !isLeft;
        //Debug.Log(isLeft);
    }

    public void OnPickDropControls(InputAction.CallbackContext context)
    {
        Debug.Log("this is pick up");
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
    public bool pickUpItemRight(GameObject clawRight)
    {
        if (Rpickedup == false && canPickupR == true)
        {
            Debug.Log("you are here");
            rightItem.GetComponent<Collider>().enabled = false;
            rightItem.transform.position = clawRight.transform.position;
            rightItem.transform.parent = clawRight.transform;

            heldRight = rightItem;
            Rpickedup = true;
        }
        Debug.Log("pickedup is" + Rpickedup);
        Debug.Log("canPickupR is " + canPickupR);
        Debug.Log("this is in the pickupdrop script right item is: " + heldRight);
        return Rpickedup;
    }

    public bool pickUpItemLeft(GameObject clawLeft)
    {
        if (Lpickedup == false && canPickupL == true)
        {
            Debug.Log("you are here");
            leftItem.GetComponent<Collider>().enabled = false;
            leftItem.transform.position = clawLeft.transform.position;
            leftItem.transform.parent = clawLeft.transform;

            heldLeft = leftItem;
            Lpickedup = true;
        }
        Debug.Log("canPickup is " + canPickupL);
        return Lpickedup;
    }

    public void dropItemR()
    {
        Debug.Log("right item" + heldRight);
        heldRight.transform.parent = null;

        heldRight.GetComponent<Collider>().enabled = true;
        Rpickedup = false;
        heldRight = null;

        Debug.Log("dropped");
    }

    public void dropItemL()
    {
        Debug.Log("left item" + heldLeft);
        heldLeft.transform.parent = null;

        heldLeft.GetComponent<Collider>().enabled = true;
        Lpickedup = false;
        heldLeft = null;

        Debug.Log("dropped");
    }
}
