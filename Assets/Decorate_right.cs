using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decorate_right : MonoBehaviour
{
    private PlayerController pControllerScript;
    public bool castleCollision = false;
    public ContactPoint Cpoint;

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
            if (gameObject.name == "new_R_claw_locator")
            {
                Debug.Log("item is called " + collision.gameObject.name);
                Debug.Log("Entered this collsionm");
                foreach (ContactPoint contact in collision.contacts)
                {
                    Debug.Log("Contact Point right: " + contact.point);                 
                }
                castleCollision = true;
                Cpoint = collision.contacts[0];
            }

        }

    }
    public void OnCollisionExit(Collision collision)
    {
        castleCollision = false;
    }
}
