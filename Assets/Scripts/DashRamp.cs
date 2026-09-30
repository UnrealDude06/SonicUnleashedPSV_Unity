using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;


public class DashRamp : MonoBehaviour {
public GameObject ramp_launch_pos;
public float ramp_spd,lockTime;
public float player_spd_set = 6;

public PathCreator pathCreator;
public EndOfPathInstruction end;
public float splineSpeedOveride;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
