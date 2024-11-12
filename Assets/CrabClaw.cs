using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabClaw : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "item")
        {
            Debug.Log("The gameObject is: " + gameObject.name);
            if (gameObject.name == "claw_R")
            {
                gameObject.GetComponentInParent<PlayerController>().rightItem = other.gameObject;
                gameObject.GetComponentInParent<PlayerController>().canPickupR = true;
                Debug.Log("item is called " + other.name);
                Debug.Log("rigth can pick up is " + gameObject.GetComponentInParent<PlayerController>().canPickupR);

            }
            else
            {
                Debug.Log("The gameObject is: " + gameObject.name);
                gameObject.GetComponentInParent<PlayerController>().leftItem = other.gameObject;
                gameObject.GetComponentInParent<PlayerController>().canPickupL = true;
                Debug.Log("item is called " + other.name);
                Debug.Log("left can pick up is " + gameObject.GetComponentInParent<PlayerController>().canPickupL);
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        Debug.Log("The gameObject is: " + gameObject.name);
        if (gameObject.name == "claw_R")
        {
            gameObject.GetComponentInParent<PlayerController>().rightItem = null;
            gameObject.GetComponentInParent<PlayerController>().canPickupR = false;
        }
        else
        {
            gameObject.GetComponentInParent<PlayerController>().leftItem = null;
            gameObject.GetComponentInParent<PlayerController>().canPickupL = false;

        }
    }
}