using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxPlayer : MonoBehaviour {
	[SerializeField] private Transform target;
	SonicController sonic;
	// Use this for initialization
	void Start () {
		sonic= target.GetComponent<SonicController>();
	}
	
	// Update is called once per frame
	void Update () {
		 // Get the target's position
		 if(!sonic.jumped){
            Vector3 targetPos = target.position;

            // Update the follower's position to match the target's X and Z positions
            transform.position = new Vector3(targetPos.x, transform.position.y, targetPos.z);
		 }
		 else
		 {
			Vector3 targetPos = target.position;
			 transform.position = new Vector3(targetPos.x, targetPos.y, targetPos.z);
		 }
	}
}
