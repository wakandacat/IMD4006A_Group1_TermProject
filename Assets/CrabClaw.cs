using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrabClaw : MonoBehaviour
{
    private PlayerController pControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        pControllerScript = GameObject.Find("Crab").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "item" || other.gameObject.tag == "specialitem")
        {
            //Debug.Log("The gameObject is: " + gameObject.name);
            if (gameObject.name == "new_R_claw_locator")
            {
                //gameObject.GetComponentInParent<PlayerController>().rightItem = other.gameObject;
                //gameObject.GetComponentInParent<PlayerController>().canPickupR = true;
                pControllerScript.updateRClawStatus(other.gameObject, true);
                //Debug.Log("item is called " + other.name);
                //Debug.Log("rigth can pick up is " + gameObject.GetComponentInParent<PlayerController>().canPickupR);

            }
            else
            {
                //Debug.Log("The gameObject is: " + gameObject.name);
                //gameObject.GetComponentInParent<PlayerController>().leftItem = other.gameObject;
                //gameObject.GetComponentInParent<PlayerController>().canPickupL = true;
                pControllerScript.updateLClawStatus(other.gameObject, true);
                //Debug.Log("item is called " + other.name);
                //Debug.Log("left can pick up is " + gameObject.GetComponentInParent<PlayerController>().canPickupL);
            }

            if (other.tag == "item" || other.gameObject.tag == "specialitem")
            {
                //indicate the object can be picked up by changing its outline colour
                var outline = other.gameObject.GetComponent<Outline>();

                outline.OutlineColor = Color.green;
            }

        }
    }
    public void OnTriggerExit(Collider other)
    {
        //Debug.Log("The gameObject is: " + gameObject.name);
        if (gameObject.name == "new_R_claw_locator")
        {
            //gameObject.GetComponentInParent<PlayerController>().rightItem = null;
            //gameObject.GetComponentInParent<PlayerController>().canPickupR = false;
            pControllerScript.updateRClawStatus(null, false);
        }
        else
        {
            //gameObject.GetComponentInParent<PlayerController>().leftItem = null;
            //gameObject.GetComponentInParent<PlayerController>().canPickupL = false;
            pControllerScript.updateLClawStatus(null, false);

        }

        if (other.tag == "item" || other.gameObject.tag == "specialitem")
        {
            //indicate the object is no longer in range by changing its outline colour back to its original colour
            var outline = other.gameObject.GetComponent<Outline>();

            outline.OutlineColor = other.gameObject.GetComponent<item>().outlineColor;
        }

    }
}