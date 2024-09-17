using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //crab object being controlled by inputs
    public GameObject crab;
    public GameObject camera;

    //input action asset that reads controller inputs
    PlayerControls controls;

    //numbers to fiddle with
    public float moveSpeed;
    public float camSpeed;

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

        //looking controls
        Vector2 rightStick = controls.GamePlay.Look.ReadValue<Vector2>();
        Vector3 look = new Vector3(-1 * (rightStick.y), rightStick.x, 0f);
        camera.transform.Rotate(look*camSpeed, Space.Self);

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
        float leftBumper = controls.GamePlay.Grab.ReadValue<float>();
        //Debug.Log(leftBumper);
        //make the crab drop here

    }
}
