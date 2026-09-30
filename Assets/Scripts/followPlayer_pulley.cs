using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followPlayer_pulley : MonoBehaviour {
public GameObject plyer;
public bool pull;
Vector3 startPos;
	// Use this for initialization
	void Awake () {
		startPos = transform.position;
		plyer = FindObjectOfType<SonicController>().gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		if(pull){
		transform.position = plyer.transform.position;
		}
		else
		{
			transform.position = Vector3.Lerp(transform.position,startPos,19 * Time.deltaTime);
		}
	}
}
