using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioManager : MonoBehaviour
{
    //array to hold all the sound effects
    public AudioClip[] sfxClips;

    //available Audio Sources
    public AudioSource sfxSource;   //used for sfx sounds
    public AudioSource walkSource;  //used exclusively for the walking sound
    //public AudioSource digSource; //used exclusively for the digging sound


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //loads audio source based on the index and plays
    public void sfxPlayer(int i)
    {
        if(i == 2)
        {
            //set walking
            print("play digging sound");
        }
        else
        {
            //set normal sfx
            sfxSource.clip = sfxClips[i];   //load sfx clip based on array index
            sfxSource.Play();               //play clip
        }
    }
}
