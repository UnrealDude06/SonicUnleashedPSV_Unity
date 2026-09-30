using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerPoint : MonoBehaviour {
[SerializeField] private Transform follow_pos;
[SerializeField] private Transform lookAt;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.position = follow_pos.position;

		transform.rotation =  Quaternion.LookRotation(lookAt.position - transform.position);

	}
}
