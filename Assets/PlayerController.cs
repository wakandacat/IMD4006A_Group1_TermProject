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

    //Booleans
    bool canPickup = true;
    bool ifpickedUp;
    bool ifpickedUpR = false;
    bool ifpickedUpL = false;

    [SerializeField] public Rigidbody _rb;

    GameObject pickedUpItem;
    private GameObject currentHoldingClaw;

    PickUpDrop pick_drop;
    PickUpDrop pick_drop1;


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

        //grab starting positions
        clawLeftStart = clawLeft.transform.localPosition;
        clawRightStart = clawRight.transform.localPosition;

        // Stopping the particle system by default
        movePartSystem.Stop();

        // Reminder on how to do this came from: https://youtu.be/gFwf_T8_8po?si=knchWQ0Sk1b1Lmna
        terrainScript = GameObject.FindGameObjectWithTag("TerrManager").GetComponent<TerrainEditor>();

        //setting up pick up drop
        pick_drop = clawRight.AddComponent<PickUpDrop>();
        pick_drop1 = clawLeft.AddComponent<PickUpDrop>();

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
        //but allow for some deceleration
        if (leftStick.magnitude > 0.1f || crabVel.magnitude > 3.0f)
        {

            //make sure to keep control scheme corresponding to camera's rotation
            Vector3 camForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 camRight = Vector3.Scale(Camera.main.transform.right, new Vector3(1, 0, 1)).normalized;

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
                currMoveSpeed = baseMoveSpeed - pickedUpItem.GetComponent<Rigidbody>().mass;
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
            Camera.main.transform.rotation = Quaternion.Euler(15, smoothRot, 0);



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
        Vector3 clawMove = clawMovement * baseMoveSpeed * Time.deltaTime;


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
            if (isLeft)
            {
                Vector3 newPos = clawLeftStart + new Vector3(xOffset, yOffset, zOffset);
                clawLeft.transform.localPosition = Vector3.Lerp(clawLeft.transform.localPosition, newPos, Time.deltaTime * shakeSpeed);
            }
            else
            {
                Vector3 newPos = clawRightStart + new Vector3(xOffset, yOffset, zOffset);
                clawRight.transform.localPosition = Vector3.Lerp(clawRight.transform.localPosition, newPos, Time.deltaTime * shakeSpeed);
            }
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

        if (rightBumper == 1 && ifpickedUp == true && canPickup == false)
        {
            if (!isLeft)
            {
                ifpickedUpR = pick_drop.pickUpItemRight(clawRight);
            }
            else
            {
                ifpickedUpL = pick_drop1.pickUpItemLeft(clawLeft);
            }
        }
        else if(rightBumper == 1 && ifpickedUp == false && canPickup == true)
        {

                //play pick up audio
                AudioManager.instance.sfxPlayer(0);
        }

        

    }

    //toggle the active claw
    public void OnSwitch(InputAction.CallbackContext context)
    {
        isLeft = !isLeft;
        //Debug.Log(isLeft);
    }
}
