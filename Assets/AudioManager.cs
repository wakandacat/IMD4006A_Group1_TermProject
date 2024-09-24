using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AudioManager : MonoBehaviour
{
    //create instance of AudioManager for easier access
    public static AudioManager instance;

    //array to hold all the sound effects
    public AudioClip[] sfxClips;

    //available Audio Sources
    public AudioSource sfxSource;   //used for sfx sounds
    public AudioSource walkSource;  //used exclusively for the walking sound
    //public AudioSource digSource; //used exclusively for the digging sound

    public bool isLooping = false;

    private void Awake()
    {
        //if instance doesn't exist, fill with this one else destroy the existing version
        if(instance == null)
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

    //loads audio source based on the index and plays
    public void sfxPlayer(int i)
    {
        ////determine if the clip should loop
        //if (i == 2)
        //{
        //    isLooping = true;   //digging and walking should loop
        //}
        //else
        //{
        //    isLooping = false;  //pick up and put down should play once
        //}

        //set sfx
        sfxSource.clip = sfxClips[i];   //load sfx clip based on array index
        //sfxSource.loop = isLooping;     //set whether it needs to loop,, works but don't know how to stop it
        sfxSource.Play();               //play clip
    }
}
