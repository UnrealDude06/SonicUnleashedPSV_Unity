using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicCDobj : MonoBehaviour {

public Countdown cd;
	public bool go;
	public float speed;
	public Rigidbody rb;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();

	}
	
	// Update is called once per frame
	void Update () {
		if(!go)
		{
			if(Input.GetButtonDown("Square"))
			{
				speed+= 0.5f;
			}
		}

		speed = Mathf.Clamp(speed, 0, 20);

	}
}
