using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonic_enemyExplode_launcher : MonoBehaviour {
public ExplosionManager  explosionManager;

	// Use this for initialization
	void Awake () {
		explosionManager = FindObjectOfType<ExplosionManager>();
	}
	
	// Update is called once per frame
	public void  SpawnExplode (Vector3 position) {
		explosionManager.SpawnExplosion(position);
	}
}
