using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
public class Pulley : MonoBehaviour {
public float springSpeed,time;
public PathCreator pathCreator;
public EndOfPathInstruction end;
public float player_spd_set;
public float splineSpeedOveride;
public LineRenderer ln;
public Transform rope_start_model,rope_end;
	// Use this for initialization
	void Awake () {
		ln = GetComponentInChildren<LineRenderer>();
        ln.positionCount = 2;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3[] positions = { rope_start_model.transform.position, rope_end.transform.position };
		ln.SetPositions(positions);
	
	}
}
