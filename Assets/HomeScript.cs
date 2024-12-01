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
    public string[] NPCText = new[] { "What a nice home!", "OOOOOO!", "Nice place! Here ya go!", "Found this for you!", "You should be mayor!", "Whoa!" , "What a beautiful home!" , "I love it!", "Amazing place!" };

    //total point value on castle
    int totalPts = 0;

    //npc spawn
    List<NPCClass> npcArr = new List<NPCClass>();


    // Start is called before the first frame update
    void Start()
    {
        //create NPC insatnces to spawn later
        for (int i = 0; i < numNPCs; i++)
        {
            //get a bunch of random values
            int currNPC = UnityEngine.Random.Range(0, npcs.Length);
            int currText = UnityEngine.Random.Range(0, NPCText.Length);
            int currPts = UnityEngine.Random.Range(3, 5);
            int currMat = UnityEngine.Random.Range(0, NPCMats.Length);
            int currItem = UnityEngine.Random.Range(0, items.Length - 1);
            float currX = UnityEngine.Random.Range(4f, 30f);
            float currZ = UnityEngine.Random.Range(50f, 70f);
            float scale = UnityEngine.Random.Range(0.5f, 1.0f);

            if (i == 0) //vary the amount of points needed to spawn the next NPC
            {
                npcArr.Add(new NPCClass(npcs[currNPC], items[currItem], currPts, new Vector3(4f, 3f, 53f), textPrefab, NPCText[currText], NPCMats[currMat], scale));
            }
            else if (i == numNPCs - 1) //the very last NPC -> the very last item
            {
                npcArr.Add(new NPCClass(npcs[currNPC], items[items.Length-1], npcArr[i - 1].ptsToSpawn + currPts, new Vector3(currX, 3f, currZ), textPrefab, NPCText[currText], NPCMats[currMat], scale));
            }
            else //the rest of the npcs
            {
                npcArr.Add(new NPCClass(npcs[currNPC], items[currItem], npcArr[i - 1].ptsToSpawn + currPts, new Vector3(currX, 3f, currZ), textPrefab, NPCText[currText], NPCMats[currMat], scale));
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
