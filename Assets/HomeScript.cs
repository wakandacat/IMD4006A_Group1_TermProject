using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class HomeScript : MonoBehaviour
{
    //an array to store all items within home area
    public List<GameObject> homeItems = new List<GameObject>();     //list of gameObjects of class 'item', these should be prefabs

    //home area
    public GameObject home;
    private int numNPCs = 10; //total amount of NPCs in game
    public GameObject[] npcs;
    public GameObject[] items;
    public Material[] NPCMats;
    public GameObject textPrefab;
    public GameObject camera;
    private string[] NPCText = new[] { "What a nice home!", "Oooooo! Fancy!", "Nice place! Here ya go!", "I found this for you!", "You should be mayor!", "Whoa! Cool house!" , "A housewarming gift!" , "I love your house!", "Welcome to Crab Cove!" };
    private Vector3[] NPCPositions = new Vector3[] { new Vector3(7.08f, 3.58f, 89.3f), new Vector3(12.1f, 3.82f, 94.05f), new Vector3(13f, 3.8f, 82.25f), new Vector3(18f, 3.8f, 88.5f), new Vector3(23.65f, 3.8f, 95f), new Vector3(28f, 3.8f, 86f), new Vector3(33f, 3.8f, 94f), new Vector3(35.75f, 3.8f, 80.5f), new Vector3(40.5f, 3.8f, 96.85f), new Vector3(44.05f, 3.8f, 87.8f) };

    //total point value on castle
    int totalPts = 0;

    //npc spawn
    public List<NPCClass> npcArr = new List<NPCClass>();


    // Start is called before the first frame update
    void Start()
    {
        //create NPC insatnces to spawn later
        for (int i = 0; i < numNPCs; i++)
        {
            //get a bunch of random values
            int currNPC = UnityEngine.Random.Range(0, npcs.Length);
            int currText = UnityEngine.Random.Range(0, NPCText.Length);
            int currPts = UnityEngine.Random.Range(7, 9); //vary the amount of points needed to spawn the next NPC
            int currMat = UnityEngine.Random.Range(0, NPCMats.Length);
            int currItem = UnityEngine.Random.Range(0, items.Length - 1); 
            float scale = UnityEngine.Random.Range(0.5f, 1.0f);

            if (i == 0)  //the very first NPC
            {
                npcArr.Add(new NPCClass(npcs[currNPC], items[currItem], 4, NPCPositions[i], textPrefab, NPCText[currText], NPCMats[currMat], scale));
            }
            else if (i == numNPCs - 1) //the very last NPC -> the very last item
            {
                npcArr.Add(new NPCClass(npcs[currNPC], items[items.Length-1], npcArr[i - 1].ptsToSpawn + currPts, NPCPositions[i], textPrefab, NPCText[currText], NPCMats[currMat], scale));
            }
            else //the rest of the npcs
            {
                npcArr.Add(new NPCClass(npcs[currNPC], items[currItem], npcArr[i - 1].ptsToSpawn + currPts, NPCPositions[i], textPrefab, NPCText[currText], NPCMats[currMat], scale));
            }

        }

    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log(totalPts);
        //check if the total point value is enought to spawn NPC
        for (int i = 0; i < npcArr.Count; i++)
        {
            if (totalPts >= npcArr[i].ptsToSpawn)
            {
                npcArr[i].spawnNPC();
                npcArr[i].updateRotation();
            }
        }

    }

    public void decorateItem(GameObject droppedItem)
    {       
        //increment total points
        totalPts = totalPts + droppedItem.GetComponent<item>().itemPtValue;
        
    }

}
