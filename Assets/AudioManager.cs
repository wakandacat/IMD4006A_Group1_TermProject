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
    public AudioSource armMoveSource; //used exclusively for the arm movement sound
    public AudioSource ambientSource; //used exclusively for the background sounds
    public AudioSource breakBuild; //used exclusively for the breaking build up sound 
    public AudioSource rattleSource; //used exclusively for rattling pearl sound when moving 
    public GameObject heldR;
    public GameObject heldL;
    public bool activeClaw = false;

    private float secondsToWait;


    private void Awake()
    {
        //if instance doesn't exist, fill with this one else destroy the existing version
        if(instance == null)
        {
            instance = this;
           // DontDestroyOnLoad(gameObject);
        }
        else
        {
          //  Destroy(gameObject);
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
            float clawMag = GameObject.Find("Crab").GetComponent<PlayerController>().rightStick.magnitude;
            heldR = GameObject.Find("Crab").GetComponent<PlayerController>().heldRight;
            heldL = GameObject.Find("Crab").GetComponent<PlayerController>().heldLeft;
            activeClaw = GameObject.Find("Crab").GetComponent<PlayerController>().isLeft;
            Debug.Log("activeClaw before if else: " + activeClaw);

            //check for the magnitude applied to joystick, decrase secondsToWait as the magnitude increases
            if (clawMag >= 0.01f)
            {
                //determine if active has a clam
                if (heldR != null && heldR.gameObject.GetComponent<item>().breakable == true && activeClaw == false)
                {
                    Debug.Log("activeClaw in R: " + activeClaw);
                    rattleSource.PlayOneShot(rattleSource.clip);

                }
                else if (heldL != null && heldL.gameObject.GetComponent<item>().breakable == true && activeClaw == true)
                {
                    Debug.Log("activeClaw in L: " + activeClaw);
                    rattleSource.PlayOneShot(rattleSource.clip);
                }

            }
            else if(walkMag >= 0.7f)
            {
                secondsToWait = 0.33f;
                //play a single instance of the sfx
                walkSource.PlayOneShot(walkSource.clip);

                //if held left or right is clam then rattle
                if(heldR != null && heldR.gameObject.GetComponent<item>().breakable == true)
                {
                        Debug.Log("activeClaw in R: " + activeClaw);
                        rattleSource.PlayOneShot(rattleSource.clip);
                    
                }
                else if(heldL != null && heldL.gameObject.GetComponent<item>().breakable == true)
                {
                        Debug.Log("activeClaw in L: " + activeClaw);
                        rattleSource.PlayOneShot(rattleSource.clip);
                }
            }
            else if (walkMag >= 0.5f && walkMag < 0.7f)
            {
                secondsToWait = 0.5f;
                //play a single instance of the sfx
                walkSource.PlayOneShot(walkSource.clip);

                //if held left or right is clam then rattle
                if (heldR != null && heldR.gameObject.GetComponent<item>().breakable == true)
                {
                        rattleSource.PlayOneShot(rattleSource.clip);
                }
                else if (heldL != null && heldL.gameObject.GetComponent<item>().breakable == true)
                {
                        rattleSource.PlayOneShot(rattleSource.clip);
                    
                }
            }
            else if (walkMag >= 0.1f && walkMag < 0.5f)
            {
                secondsToWait = 0.6f;
                //play a single instance of the sfx
                walkSource.PlayOneShot(walkSource.clip);

                //if held left or right is clam then rattle
                if (heldR != null && heldR.gameObject.GetComponent<item>().breakable == true)
                {
                        rattleSource.PlayOneShot(rattleSource.clip);
                    
                }
                else if (heldL != null && heldL.gameObject.GetComponent<item>().breakable == true)
                {
                        rattleSource.PlayOneShot(rattleSource.clip);
                    
                }
            }
            else
            {
                secondsToWait = 0.5f;
                rattleSource.Stop();
            }

            //wait for x seconds before re-entering the loop
            yield return new WaitForSeconds(secondsToWait);

        }
    }

    //adds sound for as long as the player is moving the claws
    public IEnumerator armMoveTimer()
    {

        //for as long as player is using joystick
        while (true)
        {
            //get stickMag
            float stickMag = GameObject.Find("Crab").GetComponent<PlayerController>().rightStick.magnitude;
            //Debug.Log("rightstick mag is: " + stickMag);

            if(stickMag >= 0.1f)
            {
                armMoveSource.PlayOneShot(armMoveSource.clip);

            }
            else
            {
                armMoveSource.Stop();
            }

            //wait for x seconds before re-entering the loop
            yield return new WaitForSeconds(1);

        }
    }

    //adds sound for as long as the player is moving the claws
    public IEnumerator breakBuildTimer()
    {

        //for as long as player is using joystick
        while (true)
        {
            //get break trigger value
            float triggerVal = GameObject.Find("Crab").GetComponent<PlayerController>().leftTrigger;
            bool playBreak = GameObject.Find("Crab").GetComponent<PlayerController>().canBreak;


            if (playBreak == true)
            {
                if (triggerVal >= 0.05f)
                {
                    breakBuild.PlayOneShot(breakBuild.clip);

                }
                else
                {
                    breakBuild.Stop();
                }
            }
            
            //wait for x seconds before re-entering the loop
            yield return new WaitForSeconds(0.5f);

        }
    }

    //adds sound for as long as the player is digging
    public IEnumerator digSoundTimer()
    {

        //for as long as player is pressing on the trigger
        while (true)
        {
            //get trigger value for dig
            float triggerVal = GameObject.Find("Crab").GetComponent<PlayerController>().rightTrigger;
            float seconds = 0.0f;

            //fastest
            if (triggerVal >= 0.7f)
            {
                seconds = 0.4f;
                //play a single instance of the sfx
                digSource.PlayOneShot(digSource.clip);
            }
            else if (triggerVal >= 0.4f && triggerVal < 0.7f)
            {
                seconds = 0.5f;
                //play a single instance of the sfx
                digSource.PlayOneShot(digSource.clip);
            }
            //slowest
            else if (triggerVal >= 0.1f && triggerVal < 0.4f)
            {
                seconds = 0.6f;
                //play a single instance of the sfx
                digSource.PlayOneShot(digSource.clip);
            }
            else
            {
                digSource.Stop();
                seconds = 0.4f;
            }

            //wait for x seconds before re-entering the loop
            yield return new WaitForSeconds(seconds);

        }
    }
}
