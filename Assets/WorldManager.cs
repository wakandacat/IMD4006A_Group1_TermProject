using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//this script will manage setting up the world space
//this includes: spawning items, creatin + resetting terrain, other stuff I can't think of rn
public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;
    private SpawnItems itemSpawnScript;
    private TerrainEditor terrainScript;
    public GameObject outOfBounds;
    public GameObject safetyNet;
    public GameObject homeArea;
    public Vector3 crabStartPos;
    public Vector3 cameraStartPos;
    public GameObject crab;
    public bool enterFlag = true; //flag for checking if the crab only just entered the home area
    public bool toDelete = false;
    public bool gameStart = true; //dont perform some actions at the very beginning of the game

    //may be unnecessary but idk
    private void Awake()
    {
        //if instance doesn't exist, fill with this one else destroy the existing version
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
           // Destroy(gameObject);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        terrainScript = GameObject.FindGameObjectWithTag("TerrManager").GetComponent<TerrainEditor>();

        crab = GameObject.Find("Crab").gameObject;

        //on start of game, spawn items
        itemSpawnScript = GameObject.Find("ItemSpawner").GetComponent<SpawnItems>();
        itemSpawnScript.spawnItemsFunc();

        //start the crab at the default position
        crab.transform.position = crabStartPos;
        //Camera.main.transform.position = cameraStartPos;

        enterFlag = true;
        toDelete = false;
        gameStart = true;

    }

    // Update is called once per frame
    void Update()
    {

        //---------------------------------OUT OF BOUNDS AREA-----------------------------------------
        //always check if the crab goes out of bounds
        //if it does, do a fade to black thing and then move the crab back home
        if (crab.transform.position.x <= outOfBounds.transform.position.x + outOfBounds.transform.localScale.x / 2 && crab.transform.position.x >= outOfBounds.transform.position.x - outOfBounds.transform.localScale.x / 2 && crab.transform.position.z <= outOfBounds.transform.position.z + outOfBounds.transform.localScale.z / 2 && crab.transform.position.z >= outOfBounds.transform.position.z - outOfBounds.transform.localScale.z / 2)
        {
            //Debug.Log("Out of bounds");
            crab.transform.position = crabStartPos;
           // Camera.main.transform.position = cameraStartPos;
        }

        //---------------------------------SAFETY NET AREA-----------------------------------------
        //always check if the crab goes out of bounds
        //if it does, do a fade to black thing and then move the crab back home
        if (crab.transform.position.x <= safetyNet.transform.position.x + safetyNet.transform.localScale.x / 2 && crab.transform.position.x >= safetyNet.transform.position.x - safetyNet.transform.localScale.x / 2 && crab.transform.position.z <= safetyNet.transform.position.z + safetyNet.transform.localScale.z / 2 && crab.transform.position.z >= safetyNet.transform.position.z - safetyNet.transform.localScale.z / 2)
        {
            //Debug.Log("Out of bounds");
            crab.transform.position = crabStartPos;
           // Camera.main.transform.position = cameraStartPos;
        }

        //---------------------------------HOME AREA-----------------------------------------
        //if the crab enters the home area, then destroy all items in playable area and spawn new ones
        if (crab.transform.position.x <= homeArea.transform.position.x + homeArea.transform.localScale.x / 2 && crab.transform.position.x >= homeArea.transform.position.x - homeArea.transform.localScale.x / 2 && crab.transform.position.z <= homeArea.transform.position.z + homeArea.transform.localScale.z / 2 && crab.transform.position.z >= homeArea.transform.position.z - homeArea.transform.localScale.z / 2 && enterFlag == false && gameStart == false)
        {
            //Debug.Log("Entered");
            enterFlag = true;
            toDelete = true;

            // Resetting terrain upon entering the home area.
            terrainScript.resetTerrainHeight();

        }


        //when crab leaves home area, set flags back
        // if (crab.transform.position.x >= homeArea.transform.position.x + homeArea.transform.localScale.x / 2 || crab.transform.position.x <= homeArea.transform.position.x - homeArea.transform.localScale.x / 2 && crab.transform.position.z >= homeArea.transform.position.z + homeArea.transform.localScale.z / 2 || crab.transform.position.z <= homeArea.transform.position.z - homeArea.transform.localScale.z / 2 && enterFlag == true)
        if (crab.transform.position.x >= homeArea.transform.position.x + homeArea.transform.localScale.x / 2 || crab.transform.position.x <= homeArea.transform.position.x - homeArea.transform.localScale.x / 2 && crab.transform.position.z >= homeArea.transform.position.z + homeArea.transform.localScale.z / 2 || crab.transform.position.z <= homeArea.transform.position.z - homeArea.transform.localScale.z / 2)
        {
            //Debug.Log("Left");
            enterFlag = false;
            gameStart = false; //its no longer the beginnning of the game so begin functions as usual

            crab.GetComponent<PlayerController>().gameObject.transform.Find("text").GetChild(0).GetComponent<TextMesh>().text = "";

        }

        //only destroy items once
        if (enterFlag && toDelete && gameStart == false)
        {
            StartCoroutine(CallDeleteDelay());

            toDelete = false;
        }

    }

    public bool getHomeStatus()
    {
        return enterFlag;
    }

    IEnumerator CallDeleteDelay()
    {
        yield return new WaitForSeconds(1.5f); // Wait for a bit second to prevent items being deleted in the players claws
        Delete(); // Call the function
    }

    void Delete()
    {
        Debug.Log("Destroyed");
        //destory current items
        itemSpawnScript.destroyItemsFunc();
        //spawn new items
        //Debug.Log("Created");
        itemSpawnScript.spawnItemsFunc();

    }
}
