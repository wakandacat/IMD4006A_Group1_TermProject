using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//this script will manage setting up the world space
//this includes: spawning items, creatin + resetting terrain, other stuff I can't think of rn
public class WorldManager : MonoBehaviour
{
    public static WorldManager instance;

    //may be unnecessary but idk
    private void Awake()
    {
        //if instance doesn't exist, fill with this one else destroy the existing version
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
