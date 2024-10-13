using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item : MonoBehaviour
{

    public GameObject itemPrefab;   //attach prefab of item

    //these could all potentially be made private variables
    public string itemName;         //item's name
    public float itemWeight;        //item's weight, will be adding to/affect the crab's movement speed and claw y position (only slightly)
    public Vector3 itemPosition;    //item's position in the world space
    private int itemPtValue;        //item's point value, will be added and used to determine the house's overall point value

    public void positionItem()
    {
        itemPrefab.transform.position = itemPosition;   //this code may need to be altered to work
    }
}
