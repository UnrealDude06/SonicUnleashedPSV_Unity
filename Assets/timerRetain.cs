using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timerRetain : MonoBehaviour {
public string timerToShow;
	// Use this for initialization
	void Awake () {
		DontDestroyOnLoad(this.gameObject);
	}

}
