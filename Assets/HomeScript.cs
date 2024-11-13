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

    //total point value
    int totalPts = 0;

    //an array of decorate positions on the castle
    Vector3[] spawnPoints = new[] { new Vector3(4.5f, 5f, 66.2f), new Vector3(4.5f, 6.4f, 66f), new Vector3(6.55f, 5f, 68.2f), new Vector3(6.55f, 6.4f, 68.2f), new Vector3(6.2f, 5f, 67f), new Vector3(6.2f, 6.42f, 67f), new Vector3(6.55f, 7.4f, 68.2f), new Vector3(4.5f, 7.4f, 66f), new Vector3(6.55f, 7.4f, 68.2f) };

    //NPC spawn value
    public int spawnNPC = 7;
    bool NPCSpawned = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log(totalPts);
        //check if the total point value is enought to spawn NPC
        if (totalPts >= spawnNPC && NPCSpawned == false)
        {
            NPCSpawned = true;
        }

        if (NPCSpawned == true)
        {
            //spawn NPC guy with message "looking good!" or something
            //Debug.Log("NPC spawned");
            Instantiate(npc, new Vector3(3.5f, 3.0f, 60.0f), Quaternion.identity);
        }
    }

    public void decorateItem(GameObject droppedItem)
    {
        if (droppedItem.transform.position.x <= home.transform.position.x + home.transform.localScale.x / 2 && droppedItem.transform.position.x >= home.transform.position.x - home.transform.localScale.x / 2 && droppedItem.transform.position.z <= home.transform.position.z + home.transform.localScale.z / 2 && droppedItem.transform.position.z >= home.transform.position.z - home.transform.localScale.z / 2)
        {
            //Debug.Log("dropped inside house");
            //increment total points
            totalPts = totalPts + droppedItem.GetComponent<item>().itemPtValue;

            //add to list
            homeItems.Add(droppedItem);

            //add it to the house by getting the next position from the set array of positions
            droppedItem.transform.position = spawnPoints[homeItems.Count-1];

        }
    }

}
