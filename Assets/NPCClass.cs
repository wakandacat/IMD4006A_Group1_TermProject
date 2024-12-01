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
    private Material material;
    private float scale;


    //constructor
    public NPCClass(GameObject npcModel, GameObject item, int ptsToSpawn, Vector3 spawnPos, GameObject text, string textDesc, Material mat, float scale)
    {
        this.npcModel = npcModel;
        this.item = item;
        this.ptsToSpawn = ptsToSpawn;
        this.spawnPos = spawnPos;
        this.text = text;
        this.textDesc = textDesc;
        this.material = mat;
        this.scale = scale;
    }

    public void spawnNPC()
    {
        if (hasSpawned == false)
        {
            this.npcModel = Instantiate(npcModel, spawnPos, Quaternion.identity);
            //change the body and the two claws to have the new colour
            this.npcModel.transform.GetChild(0).GetComponent<MeshRenderer>().material = material;
            this.npcModel.transform.GetChild(1).GetComponent<MeshRenderer>().material = material;
            this.npcModel.transform.GetChild(2).GetComponent<MeshRenderer>().material = material;
            this.npcModel.transform.localScale = new Vector3(scale, scale, scale);

            GameObject newItem = Instantiate(item, new Vector3(spawnPos.x + 1f, spawnPos.y, spawnPos.z + 1f), Quaternion.identity);
            newItem.tag = "specialitem";
            this.text = Instantiate(text, new Vector3(spawnPos.x, spawnPos.y + 1.5f, spawnPos.z), Quaternion.identity);
            this.text.GetComponent<TextMesh>().text = textDesc;
            hasSpawned = true;
        }
    }

    //ensure the NPCs are always looking at the player
    public void updateRotation()
    {
        Transform player = GameObject.Find("Crab").transform;

        if (hasSpawned == true)
        {
            this.text.transform.LookAt(player);
            this.npcModel.transform.LookAt(player);
        }
    }
}
