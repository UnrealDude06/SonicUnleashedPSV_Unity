using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXmanager : MonoBehaviour {
	SonicController sonic;

    public AudioSource footstepAudioSource, jumpSFXSource,driftAudioSource,boostAudioSource,actionAudioSource,loopAudioSource;
    public List<AudioClip> grassFootstepClip;
    public List<AudioClip>  concreteFootstepClip;
    public List<AudioClip>  dirtFootstepClip;

	 public AudioClip breakSoundClip ,driftSoundClip;
    public AudioClip jumpSoundClip,airDashAudioClip,quickStepAudioClip,boostAudioClip;
	[Header(" sonic actions")]
	public AudioClip dash_ring;
	public AudioClip dashramp,spring,pullley,enemyhit,speedpad,ring;
    [Header(" sliding")]
    public AudioClip slideClip;
    public AudioClip stompClip,stompLandClip;
    [Header("Score SFX")]
    public AudioClip ringUp;
    public AudioClip boostUp,attkUp;

    [Header("Sonic VoiceClips")]
    private AudioClip hurt;
    public AudioClip jump, boost, homingtrick, drift_finish, slide, wooh, hurtVC,collision,found_enemy_attack,dead;


    public float footstepDelay = 0.3f; // Adjust the delay time as needed
    private float nextFootstepTime;

        private bool isPlaying = false; // Flag to check if a ring sound is currently playing
    private int soundsCount = 0; // Counter for the number of overlapping ring sounds
    private int maxSoundCount = 2; // Maximum number of overlapping ring sounds



	void Awake()
	{
		sonic= GetComponent<SonicController>();
		nextFootstepTime = Time.time; // Initialize the next footstep time

       
	}


    // Detect collision with surfaces and play the appropriate footstep sound
    private void Update()
    {
		
        if (sonic.movement.magnitude > 0.2f && Time.time >= nextFootstepTime && sonic.isGrounded)
        {
			if (sonic.hit.collider != null)
			{	string surfaceTag =sonic.hit.collider.tag;	
            PlayFootstepSound(surfaceTag);
			}
            nextFootstepTime = Time.time + footstepDelay; // Set the next footstep time

			float target;
            if(sonic.speed > 7)
            {
                if(sonic.speed > 36 )
                {
                    target = 0.1f;
                }
                else
                {
                    target = 0.25f;
                }
            }
            else
            {
                target = 0.4f;
            }
			footstepDelay = Mathf.MoveTowards(footstepDelay,target, sonic.speed / 2 * Time.deltaTime);
        }
		

		if(sonic.sonic_action == SonicController.Actions.drift)
		{
			PlayDriftSound();
		}
		else if(sonic.sonic_action != SonicController.Actions.drift)
		{
			StopDriftSound();
		}

        if(sonic.sonic_action != SonicController.Actions.slide)
        {
            LoopClipStop();
        }


    }

	    // Method to play the footstep sound
    public void PlayFootstepSound(string surfaceTag)
    {
        AudioClip[] footstepClips = null;

        switch (surfaceTag)
        {
            case "Grass":
                footstepClips = grassFootstepClip.ToArray();
                break;
            case "conc":
                footstepClips = concreteFootstepClip.ToArray();
                break;
            case "dirt":
                footstepClips = dirtFootstepClip.ToArray();
                break;
            case "Untagged":
                footstepClips = dirtFootstepClip.ToArray();
                break;
            // Add more cases for other surface types if needed
        }

        if (footstepClips != null && footstepClips.Length > 0)
        {
            AudioClip randomClip = footstepClips[Random.Range(0, footstepClips.Length)];
            footstepAudioSource.PlayOneShot(randomClip);
        }
    }




	///////////SFXES?////////////////////
	public void PlayDriftSound()
    {
        if (!driftAudioSource.isPlaying)
        {
            driftAudioSource.clip = driftSoundClip;
            driftAudioSource.loop = true;
            driftAudioSource.Play();
        }
    }

    // Method to stop playing the drift sound
    public void StopDriftSound()
    {    
        driftAudioSource.Stop();
        driftAudioSource.loop = false;
    }

    // Method to play the jump sound
    public void PlayJumpSound()
    {
		jumpSFXSource.clip = jumpSoundClip;
        jumpSFXSource.PlayOneShot(jumpSoundClip);
    }
	public void PlayADSound()
    {
        jumpSFXSource.PlayOneShot(airDashAudioClip);
    }
	public void PlayBoostSound()
    {
		boostAudioSource.clip = boostAudioClip;
        boostAudioSource.Play();
    }
	public void PlayQSSound()
    {
        jumpSFXSource.PlayOneShot(quickStepAudioClip);
    }

	public void playActionSound(AudioClip clip)
    {    
        if (!isPlaying)
        {
            isPlaying = true;
            actionAudioSource.PlayOneShot(clip);
            StartCoroutine(ResetRingFlag());
        }
        else if (soundsCount < maxSoundCount)
        {
            actionAudioSource.PlayOneShot(clip);
            soundsCount++;
        } 
    }
    private IEnumerator ResetRingFlag()
    {
        yield return new WaitForSeconds(ring.length); // Wait for the duration of the ring sound
        isPlaying = false;
        soundsCount = 0;
    }
    
    public void LoopClip(AudioClip clip)
    {
        loopAudioSource.enabled = true;
        if (!loopAudioSource.isPlaying)
        {
            loopAudioSource.clip = clip;
            loopAudioSource.Play();
        }
    }
    public void LoopClipStop()
    {
        loopAudioSource.enabled = false;
        loopAudioSource.Stop();
    }


}

