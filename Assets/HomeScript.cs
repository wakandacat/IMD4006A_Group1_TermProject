using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class HomeScript : MonoBehaviour
{
    //an array to store all items within home area
    GameObject[] homeItems;     //array of gameObjects of class 'item', these should be prefabs

    //home area
    public GameObject home;

    //total point value
    int totalPts = 0;

    //an array of decorate positions on the castle
    Vector3[] spawnPoints = new[] { new Vector3(7f, 5f, 68f), new Vector3(5f, 5f, 68f), new Vector3(9f, 5f, 70f), new Vector3(9f, 5f, 70f), new Vector3(6.5f, 7.5f, 67.7f), };

    //crab playercontroller script
    private PlayerController crabControls;

    //NPC spawn value
    public int spawnNPC = 7;
    bool NPCSpawned = false;

    // Start is called before the first frame update
    void Start()
    {
        crabControls = GameObject.Find("Crab").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        //check for items in homeArea that are not being held
       // homeItems = GameObject.FindGameObjectsWithTag("item");

        //cycle through all of them
        //for (int x = 0; x < homeItems.Length; x++)
        //{
        //    //if the item is inside the house area and not being held then add to the array
        //    if (homeItems[x].transform.position.x <= home.transform.position.x + home.transform.localScale.x / 2 && homeItems[x].transform.position.x >= home.transform.position.x - home.transform.localScale.x / 2 && homeItems[x].transform.position.z <= home.transform.position.z + home.transform.localScale.z / 2 && homeItems[x].transform.position.z >= home.transform.position.z - home.transform.localScale.z / 2)
        //    {
        //        //totalPts = totalPts + homeItems[x].GetComponent<item>().itemPtValue;
        //    }
        //}


        //then set its position to one of the decorate positions


        //Debug.Log(totalPts);
        //check if the total point value is enought to spawn NPC
        if (totalPts >= spawnNPC && NPCSpawned == false)
        {
            NPCSpawned = true;
        }

        if (NPCSpawned == true)
        {
            //spawn NPC guy with message "looking good!" or something
            Debug.Log("NPC spawned");
        }
    }
}
