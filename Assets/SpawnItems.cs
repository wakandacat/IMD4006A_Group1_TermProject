using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

[System.Serializable]
public class SpawnItems : MonoBehaviour
{
    public item[] prefabList;     //array of gameObjects of class 'item', these should be prefabs
    public Vector3[] spawnPoints = new[] { new Vector3(20f, 0f, 25f), new Vector3(35f, 0f, 55f), new Vector3(20f, 0f, 25f), new Vector3(51f, 0f, 53f), new Vector3(74f, 0f, 70f), new Vector3(72f, 0f, 68f) };
    
    //bucket areas
    public GameObject[] areas;
    private int currArea = -1;

    //existing items in world
    public GameObject[] itemList;

    //home area
    public GameObject home;

    // Start is called before the first frame update
    void Start()
    {

    }

    //set the current spawnPosition to be the position of the prefab at the random num index
    public void setPosition(int num, Vector3 pos, int areaIndex)
    {
        //vary the height by the point value, so higher point value items are deeper in the terrain
        float yPos = areas[areaIndex].transform.localScale.y - (areas[areaIndex].transform.localScale.y / prefabList[num].itemPtValue);

        prefabList[num].transform.position = new Vector3(pos.x, -yPos, pos.z);
    }

    //get all the items that can spawn in an area, and pick one
    public int getItem(int areaIndex)
    {
        List<int> indecesToPickFrom = new List<int>();
        int chosenItem = -1;
        List<int> prefabIndecesForArea = new List<int>(); //use a list to append the indeces to

        for (int i = 0; i < prefabList.Length; i++)
        {
            // Check if the current prefab's spawnAreas contains the areaIndex
            if (Array.Exists(prefabList[i].spawnAreas, elem => elem == areaIndex))
            {
                prefabIndecesForArea.Add(i); //add the index to the list
            }
        }

        //convert it back to an array
        int[] prefabIndecesArray = prefabIndecesForArea.ToArray();
        //Debug.Log("list of prefabs in this area" + string.Join(", ", prefabIndecesArray));

        //determine weights of items to pick randomly
        //get ptvalue of each item in the area (ex. 5, 3, 1)
        //multiply them all together to get a sum (ex. 5x3x1 = 15)
        //divide the sum by each ptvalue to get the amount of instances of that item in an area to pick randomly from (ex. 15/5 = 3, 15/3 = 5, 15/1 = 15)
        //pick a random nunber from the array (ex. array[5,5,5,3,3,3,3,3,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1])

        int sumPts = 1;
        int tempVal = 0;

        //multiply the point values together
        for (int i = 0; i < prefabIndecesArray.Length; i++)
        {
            sumPts = sumPts * prefabList[prefabIndecesArray[i]].itemPtValue;
        }

        //Debug.Log("sum" + sumPts);

        for (int i = 0; i < prefabIndecesArray.Length; i++)
        {
            tempVal = sumPts / prefabList[prefabIndecesArray[i]].itemPtValue; 
            
            //for the size of tempVal, add that many instances to the array to pick from
            for (int x = 0; x < tempVal; x++)
            {
                //append the index of that item a bunch of times
                indecesToPickFrom.Add(prefabIndecesArray[i]);
            }

        }

        //convert it back to an array
        int[] randomArr = indecesToPickFrom.ToArray();
        //Debug.Log("indeces to pick from" + string.Join(", ", randomArr));

        //pick an item index and return it
        chosenItem = randomArr[UnityEngine.Random.Range(0, randomArr.Length)];
        //Debug.Log("chosen item" + chosenItem);

        return chosenItem;

    }

    public void spawnItemsFunc()
    {
        //cycle through all of the spawn points
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            //check each spawn point against each area collider to determine which it is inside
            for (int x = 0; x < areas.Length; x++)
            {
                //if inside x
                if (spawnPoints[i].x <= areas[x].transform.position.x + areas[x].transform.localScale.x / 2 && spawnPoints[i].x >= areas[x].transform.position.x - areas[x].transform.localScale.x / 2)
                {
                    currArea = -1;
                    //if inside y
                    if (spawnPoints[i].y <= areas[x].transform.position.y + areas[x].transform.localScale.y / 2 && spawnPoints[i].y >= areas[x].transform.position.y - areas[x].transform.localScale.y / 2)
                    {
                        currArea = -1;
                        //if inside z
                        if (spawnPoints[i].z <= areas[x].transform.position.z + areas[x].transform.localScale.z / 2 && spawnPoints[i].z >= areas[x].transform.position.z - areas[x].transform.localScale.z / 2)
                        {
                            //yass gamers we here
                            currArea = x;
                        }
                    }
                }
            }

            int tempNum = -1;

            //catch for any items outside range
            bool itemInArea = false;

            //make a case for every area
            switch (currArea)
            {
                case 0:
                    //Debug.Log("area0");
                    itemInArea = true;
                    //choose between the possible prefabs for this area
                    tempNum = getItem(currArea);
                    break;
                case 1:
                    //Debug.Log("area1");
                    itemInArea = true;
                    //choose between the possible prefabs for this area
                    tempNum = getItem(currArea);
                    break;
                case 2:
                    //Debug.Log("area2");
                    itemInArea = true;
                    //choose between the possible prefabs for this area
                    tempNum = getItem(currArea);
                    break;
                default:
                    //Debug.Log("nah");
                    break;
            }

            if (itemInArea == true)
            {
                setPosition(tempNum, spawnPoints[i], currArea);
                Instantiate(prefabList[tempNum]);     //instantiate instance of item to scene
                itemInArea = false;
            }

        }
    }

    public void destroyItemsFunc()
    {
        //find all the existing items in the playable area
        itemList = GameObject.FindGameObjectsWithTag("item");
        //Debug.Log("Item List: " + string.Join(", ", itemList.Select(item => item.name)));

        //only delete those that are not within the home area
        for (int x = 0; x < itemList.Length; x++)
        {
            //if the item is outside the house area then destroy it
            if (itemList[x].transform.position.x >= home.transform.position.x + home.transform.localScale.x / 2 || itemList[x].transform.position.x <= home.transform.position.x - home.transform.localScale.x / 2 && itemList[x].transform.position.z >= home.transform.position.z + home.transform.localScale.z / 2 || itemList[x].transform.position.z <= home.transform.position.z - home.transform.localScale.z / 2)
            {
                //Debug.Log("Destroyed");
                Destroy(itemList[x]);
            }
        }
    }

}
