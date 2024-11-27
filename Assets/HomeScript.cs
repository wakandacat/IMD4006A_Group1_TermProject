using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
//using static UnityEditor.Progress;

public class HomeScript : MonoBehaviour
{
    //an array to store all items within home area
    public List<GameObject> homeItems = new List<GameObject>();     //list of gameObjects of class 'item', these should be prefabs

    //home area
    public GameObject home;
    public GameObject npc;
    public GameObject[] items;
    public GameObject textPrefab;
    public GameObject camera;

    //total point value
    int totalPts = 0;

    //an array of decorate positions on the castle
    Vector3[] spawnPoints = new[] { new Vector3(4.5f, 5f, 66.2f), new Vector3(4.5f, 6.4f, 66f), new Vector3(6.55f, 5f, 68.2f), new Vector3(6.55f, 6.4f, 68.2f), new Vector3(6.2f, 5f, 67f), new Vector3(6.2f, 6.42f, 67f), new Vector3(6.55f, 7.4f, 68.2f), new Vector3(4.5f, 7.4f, 66f), new Vector3(6.55f, 7.4f, 68.2f) };

    //npc spawn
    NPCClass[] npcArr;


    // Start is called before the first frame update
    void Start()
    {
        //create NPC instances
        NPCClass npc1 = new NPCClass(npc, items[0], 5, new Vector3(4f, 3f, 53f), new Vector3(0f, 0f, 0f), textPrefab, "What a nice home!");
        NPCClass npc2 = new NPCClass(npc, items[1], 8, new Vector3(4f, 3f, 45f), new Vector3(0f, 25f, 0f), textPrefab, "OOO, fancy!");
        NPCClass npc3 = new NPCClass(npc, items[2], 11, new Vector3(15f, 3f, 70f), new Vector3(0f, -90f, 0f), textPrefab, "Nice place! Here ya go!");
        NPCClass npc4 = new NPCClass(npc, items[3], 15, new Vector3(18f, 3f, 65f), new Vector3(0f, -120f, 0f), textPrefab, "Found this for you!");

        npcArr = new NPCClass[] { npc1, npc2, npc3, npc4 };

    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log(totalPts);
        //check if the total point value is enought to spawn NPC
        for (int i = 0; i < npcArr.Length; i++)
        {
            if (totalPts >= npcArr[i].ptsToSpawn)
            {
                npcArr[i].spawnNPC(camera.transform.eulerAngles.y);
                npcArr[i].updateRotation(camera.transform.eulerAngles.y);
            }
        }

    }

    public void decorateItem(GameObject droppedItem)
    {       //increment total points
            totalPts = totalPts + droppedItem.GetComponent<item>().itemPtValue;
        
    }

}
