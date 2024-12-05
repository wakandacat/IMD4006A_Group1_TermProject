using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decorate_Left : MonoBehaviour
{
    private PlayerController pControllerScript;
    //public GameObject decorateItemL;
    public ContactPoint Cpoint;
    public bool castleCollision = false;

    // Start is called before the first frame update
    void Start()
    {
        pControllerScript = GameObject.Find("Crab").GetComponent<PlayerController>();

    }

    // Update is called once per frame
    void Update()
    {

    }

    //get contact point means tha the contact is true
    //assign the contact point coordinates to it

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "castle")
        {
            //Debug.Log("The gameObject is: " + gameObject.name);
            if (gameObject.name == "new_L_claw_locator")
            {
                Debug.Log("item is called " + collision.gameObject.name);
                Debug.Log("Entered this collsionm");
                Cpoint = collision.contacts[0];
                castleCollision = true;
            }

        }

    }
    public void OnCollisionExit(Collision collision)
    {
        castleCollision = false;
    }
}
