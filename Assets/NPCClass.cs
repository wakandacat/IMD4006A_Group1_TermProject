using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCClass : MonoBehaviour
{

    private GameObject npcModel;   //attach NPC
    private GameObject item;
    public int ptsToSpawn;
    private bool hasSpawned = false;
    private Vector3 spawnPos;
    private Vector3 spawnRot;
    private GameObject text;
    private string textDesc;


    //constructor
    public NPCClass(GameObject npcModel, GameObject item, int ptsToSpawn, Vector3 spawnPos, Vector3 spawnRot, GameObject text, string textDesc)
    {
        this.npcModel = npcModel;
        this.item = item;
        this.ptsToSpawn = ptsToSpawn;
        this.spawnPos = spawnPos;
        this.spawnRot = spawnRot;
        this.text = text;
        this.textDesc = textDesc;
    }

    public void spawnNPC(float camRotY)
    {
        if (hasSpawned == false)
        {
            this.npcModel = Instantiate(npcModel, spawnPos, Quaternion.Euler(0f, camRotY - 180, 0f));
            GameObject newItem = Instantiate(item, new Vector3(spawnPos.x + 1f, spawnPos.y, spawnPos.z + 1f), Quaternion.identity);
            newItem.tag = "specialitem";
            this.text = Instantiate(text, new Vector3(spawnPos.x, spawnPos.y + 1.5f, spawnPos.z), Quaternion.Euler(0f, camRotY, 0f));
            this.text.GetComponent<TextMesh>().text = textDesc;
            hasSpawned = true;
        }
    }

    public void updateRotation(float camRotY)
    {
        if (hasSpawned == true)
        {
            this.text.transform.rotation = Quaternion.Euler(0f, camRotY, 0f);
            this.npcModel.transform.rotation = Quaternion.Euler(0f, camRotY - 180, 0f);
        }
    }
}
