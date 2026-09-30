using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
public class Spring : MonoBehaviour {
public float springSpeed,time;
private Animator anim;
public bool killSpd,spline;
public float player_spd_set = 6;
public PathCreator pathCreator;
public EndOfPathInstruction end;
public float splineSpeedOveride;
    private void Start() {
    anim = GetComponentInChildren<Animator>();    
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Player")
        {
            anim.SetBool("spr_start", true);
            Invoke("stop", 0.1f);
        }
        
    }
    void stop() {
        
        anim.SetBool("spr_start", false);
        
    }
}
