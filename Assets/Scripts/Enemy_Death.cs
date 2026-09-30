using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Death : MonoBehaviour {

private Sonic_enemyExplode_launcher player;
private CustomGravity customGravity;

// Use this for initialization
void Awake() {
    player = FindObjectOfType<Sonic_enemyExplode_launcher>();
    customGravity = GetComponent<CustomGravity>();
}

public void kill_enemy(Vector2 enemy_knock_speed) {
    //currentState = EnemyState.killed;

    // Use custom gravity script to set velocity
	customGravity.isGrounded = false;
    customGravity.SetVelocity(-transform.forward * enemy_knock_speed.x + Vector3.up * enemy_knock_speed.y);
    
    Invoke("kys", 1.2f);
    
    //EGGFIGHTER
    if (GetComponent<EggFighter>() != null) {
        GetComponent<EggFighter>().currentState = EggFighter.EnemyState.killed;
    }
}

void kys() {
    player.GetComponent<Sonic_enemyExplode_launcher>().SpawnExplode(transform.position);
    Destroy(gameObject);
}

}