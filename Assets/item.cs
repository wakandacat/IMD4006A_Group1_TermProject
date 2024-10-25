using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class item : MonoBehaviour
{

    public GameObject itemPrefab;   //attach prefab of item

    //these could all potentially be made private variables
    public string itemName;         //item's name
    public float itemWeight;        //item's weight, will be adding to/affect the crab's movement speed and claw y position (only slightly)
    public int itemPtValue;        //item's point value, will be added and used to determine the house's overall point value
    public int[] spawnAreas;        //array of the indeces of all areas in which it can spawn (ex. can spawn in area1 and area2, then spawnAreas = [1, 2])
    public bool specialItem;        //true if only an NPC given item
    public bool breakable;          //true if it is a breakable item (clamshell)

    //public void positionItem()
    //{
    //    
    //}
}
