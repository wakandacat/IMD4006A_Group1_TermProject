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
    public AudioSource digSource;   //used exclusively for the digging sound

    public bool isLooping = false;

    private float secondsToWait;

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
        //set sfx
        sfxSource.clip = sfxClips[i];   //load sfx clip based on array index
        //sfxSource.loop = isLooping;     //set whether it needs to loop,, works but don't know how to stop it
        sfxSource.PlayOneShot(sfxClips[i]);               //play clip
    }

    //play walk sound with speed of clip increasing with magnitude on joystick
    public IEnumerator walkTimer()
    {
      
        //for as long as player is using joystick
        while (true)
        {
            //get walkMag
            float walkMag = GameObject.Find("Crab").GetComponent<PlayerController>().leftStick.magnitude;
            Debug.Log("walk mag is: " + walkMag);

            //check for the magnitude applied to joystick, decrase secondsToWait as the magnitude increases
            if(walkMag >= 0.7f)
            {
                secondsToWait = 0.3f;
                //play a single instance of the sfx
                sfxSource.PlayOneShot(sfxClips[5]);
            }
            else if (walkMag >= 0.5f && walkMag < 0.7f)
            {
                secondsToWait = 0.5f;
                //play a single instance of the sfx
                sfxSource.PlayOneShot(sfxClips[5]);
            }
            else if (walkMag >= 0.1f && walkMag < 0.5f)
            {
                secondsToWait = 0.6f;
                //play a single instance of the sfx
                sfxSource.PlayOneShot(sfxClips[5]);
            }
            else
            {
                secondsToWait = 1.0f;
            }

            //wait for x seconds before re-entering the loop
            yield return new WaitForSeconds(secondsToWait);

        }
    }
}
