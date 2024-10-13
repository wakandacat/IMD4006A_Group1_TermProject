using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnItems : MonoBehaviour
{
    public item[] prefabList;     //array of gameObjects of class 'item',, these should be prefabs

    // Start is called before the first frame update
    void Start()
    {
        prefabList[0].positionItem();   //call function to update the item's position

        Instantiate(prefabList[0]);     //instantiate instance of item to scene
    }

}
