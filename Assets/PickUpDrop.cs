using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpDrop : MonoBehaviour
{

    //PlayerControls controls;

    GameObject itemToPickup;
    GameObject storeScale;

    bool canPickup = false;
    bool empty = true;
    bool Rpickedup = false;
    bool Lpickedup = false;

    public float currMoveSpeed;
    public float baseMoveSpeed = 7f;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool pickUpItemRight(GameObject clawRight)
    {
        if (empty == true && canPickup == true)
        {
            Debug.Log("you are here");
            itemToPickup.GetComponent<Collider>().enabled = false;
            itemToPickup.transform.position = clawRight.transform.position;
            itemToPickup.transform.parent = clawRight.transform;

            empty = false;
            Rpickedup = true;

        }
        Debug.Log("pickedup is" + Rpickedup);
        Debug.Log("Empty is" + empty);
        Debug.Log("canPickup is " + canPickup);
        return Rpickedup;
    }

    public bool pickUpItemLeft(GameObject clawLeft)
    {
        if (empty == true && canPickup == true)
        {
            Debug.Log("you are here");
            itemToPickup.GetComponent<Collider>().enabled = false;
            itemToPickup.transform.position = clawLeft.transform.position;
            itemToPickup.transform.parent = clawLeft.transform;

            empty = false;
            Lpickedup = true;
        }
        Debug.Log("Empty is" + empty);
        Debug.Log("canPickup is " + canPickup);
        return Lpickedup;
    }



    public void dropItemL()
    {
        if (empty == false)
        {
            itemToPickup.transform.parent = null;
            empty = true;
            itemToPickup = null;
            Lpickedup = false;
            Rpickedup = false;
            Debug.Log("dropped");
        }
    }

    public void changeMovementSpeed()
    {
        if (Rpickedup == true || Lpickedup == true)
        {
            currMoveSpeed = baseMoveSpeed;// - itemToPickup.GetComponent<Rigidbody>().mass;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "item")
        {
            itemToPickup = other.gameObject;
            Debug.Log(itemToPickup.name);
            canPickup = true;
            Debug.Log("canPickup is set as" + canPickup);
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject == itemToPickup)
        {
            Debug.Log(itemToPickup.name);
            canPickup = false;
            Debug.Log("canPickup is set as" + canPickup);
            itemToPickup = null;
            empty = true;
        }
    }
}