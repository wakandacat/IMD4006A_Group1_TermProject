using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

[System.Serializable]
public class SpawnItems : MonoBehaviour
{
    public item[] prefabList;     //array of gameObjects of class 'item', these should be prefabs
    public Vector3[] spawnPoints = new[] { new Vector3(20f, -1f, 20f), new Vector3(35f, 0f, 55f), new Vector3(15f, 0f, 60f) };
    
    //bucket areas
    public GameObject[] areas;
    private int currArea = -1;


    // Start is called before the first frame update
    void Start()
    {

        //cycle through all of the spawn points
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            //check each spawn point against each area collider to determine which it is inside
            for (int x = 0; x < areas.Length; x++)
            {
                //if inside x
                if (spawnPoints[i].x <= areas[x].transform.position.x + areas[x].transform.localScale.x/2 && spawnPoints[i].x >= areas[x].transform.position.x - areas[x].transform.localScale.x / 2)
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

            switch (currArea)
            {
                case 0:
                    Debug.Log("area0");

                    //choose between the possible prefabs for this area
                    //tempNum = Random.Range(0, 1); //will return either 0 or 1
                    tempNum = 0;
                    setPosition(tempNum, spawnPoints[i]);
                    Instantiate(prefabList[tempNum]);     //instantiate instance of item to scene
                    break;
                case 1:
                    Debug.Log("area1");

                    //choose between the possible prefabs for this area
                    tempNum = Random.Range(0, 2); //will return either 0 or 1

                    setPosition(tempNum, spawnPoints[i]);
                    Instantiate(prefabList[tempNum]);     //instantiate instance of item to scene
                    break;
                default:
                    Debug.Log("nahhhhhhhhhhhhhh");
                    break;
            }
        }

    }

    //set the current spawnPosition to be the position of the prefab at the random num index
    public void setPosition(int num, Vector3 pos)
    {
        prefabList[num].transform.position = pos;
    }

}
