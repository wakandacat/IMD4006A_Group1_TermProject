using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScript : MonoBehaviour
{
    //an array to store all items within home area
    item[] homeItems;     //array of gameObjects of class 'item', these should be prefabs

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
        //check if item is brought into homeArea and if the crab has dropped it

        //then set its position to one of the decorate positions


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
