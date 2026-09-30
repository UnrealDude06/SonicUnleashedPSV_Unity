using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultAudioController : MonoBehaviour {

public ResultsUI_Set resultsUI;
public AudioSource win_audioSource,lose_audioSource;

public AudioClip normal, e_rank;

	// Use this for initialization
	void Start () {

	}

	void Update()
{


		Debug.Log(resultsUI.rank.ToString());
		 
}

}
