using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class crabAnimation : MonoBehaviour
{
    private Vector3[] defaultLegPositions;  //array of start positions for the legs
    public GameObject[] legIKtargets; //targets points for the legs, where the toes should move to


    // Start is called before the first frame update
    void Start()
    {
        //for each leg, get it's default position and assign the value in the array
        for(int i = 0; i < legIKtargets.Length; i++)
        {
            defaultLegPositions[i] = legIKtargets[i].transform.position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
