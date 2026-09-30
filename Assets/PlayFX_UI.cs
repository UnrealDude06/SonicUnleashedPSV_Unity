using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayFX_UI : MonoBehaviour {
	[SerializeField]private GameObject u1,u2,u3;
	private AudioSource aud;

	[SerializeField]private AudioClip hereWe, go;

	bool play = true;
	
	// Use this for initialization
	void Start () {
		aud = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if(u3.gameObject.active)
		{
			if(play)
			{
				aud.PlayOneShot(hereWe);
			}
			play = false;
		}
		if(u2.gameObject.active)
		{
			play = true;
		}
		if(u1.gameObject.active)
		{
			if(play)
			{
				aud.PlayOneShot(go);
			}
			play = false;
		}
	}
}
