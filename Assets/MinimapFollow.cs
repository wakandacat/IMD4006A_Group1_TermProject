using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    //Minimap Script and how to: https://www.youtube.com/watch?v=lPSj41cNbtU
    // Start is called before the first frame update
    [SerializeField] private Transform player;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
        
    }
}
