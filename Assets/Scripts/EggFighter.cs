using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EggFighter : MonoBehaviour {
    public bool triggerAction;
    public EnemyTrigg trigger;
    public Transform placeTOMoveTrigger;
    private Animator anim;
    public float spherecastRadius = 5f;
    public float attackCooldownDuration = 3f;
    public float distanceToAttk = 5;
    public float windUpTimer= 0.5f;
    public float intervalCheck = 1;
    public CustomGravity physics;

    public enum EnemyState
    {
        Idle,
        Wait,
        Windup,
        Attack,
        AttackCooldown,
        killed
    }

    public EnemyState currentState = EnemyState.Idle;
   [SerializeField] private GameObject player;
    private float attackCooldownTimer = 0f;

    public float speed;
    float windTimer;
    float attackCooldownDurationOG;
Collider[] overlaps = new Collider[0];
Collider hitCollider;
    void Awake()
    {
        if (player == null)
        {player = GameObject.FindGameObjectWithTag("Player");}
        anim = GetComponentInChildren<Animator>();
        physics = GetComponent<CustomGravity>();
        windTimer = windUpTimer;
        attackCooldownDurationOG = attackCooldownDuration;

    }

    void Update()
    {
        StartCoroutine(CheckPlayer(intervalCheck));

        switch (currentState)
        {
            case EnemyState.Idle:
            if(overlaps.Length > 0)
                {
                    anim.SetBool("walk", true);
                    currentState = EnemyState.Wait;
                    
                }
            break;
            
            case EnemyState.Wait:
                Move();

                if(overlaps.Length > 0)
                {
                    if (Vector3.Distance(overlaps[0].transform.position, transform.position) < distanceToAttk)
                    {
                        anim.SetBool("windup", true);
                        currentState = EnemyState.Windup;
                    }
                }
                if(triggerAction)
                {
                    currentState = EnemyState.Idle;
                }
            break;

            case EnemyState.Windup:
                physics.SetVelocity(Vector3.zero);
                windTimer -= Time.deltaTime;
                if(windTimer < 0)
                {
                    anim.SetBool("windup", false);
                    anim.SetBool("attack", true);
                    currentState = EnemyState.Attack;
                    windTimer = windUpTimer;
                }
            break;

            case EnemyState.Attack:
                anim.SetBool("walk", false);
                if(overlaps.Length > 0)
                {
                hitCollider = overlaps[0];
                }

                if (hitCollider.CompareTag("Player") && (Vector3.Distance(overlaps[0].transform.position, transform.position) < distanceToAttk+2.5f))
                {
                    SonicController sonic = player.GetComponent<SonicController>();
                    player.GetComponent<SonicScore>().rings -= 13;
                    sonic.hurt = true;
                    sonic.sfx.playActionSound(sonic.sfx.hurtVC);
                }
                currentState = EnemyState.AttackCooldown;

            break;

            case EnemyState.AttackCooldown:
                anim.SetBool("attack", false);
                attackCooldownTimer -= Time.deltaTime;

                if (attackCooldownTimer <= 0f)
                {
                    currentState = EnemyState.Idle;
                    attackCooldownTimer = attackCooldownDurationOG;
                }
            break;

            case EnemyState.killed:
                anim.SetBool("died", true);
            break;
        }

        if(triggerAction)
        {
            if(trigger.collided)
            {
                anim.SetBool("walk",true);

                // Calculate the direction from the enemy to the player
                Vector3 direction = (new Vector3( placeTOMoveTrigger.position.x,transform.position.y,placeTOMoveTrigger.position.z) - transform.position).normalized;
                //transform.position += direction * 0.7f;
                physics.SetVelocity(direction * speed);
                if((placeTOMoveTrigger.position - transform.position).magnitude < 3f)
                {
                    physics.SetVelocity(Vector3.zero);
                    triggerAction = false;
                    anim.SetBool("walk",false);
                }

            }
        }
        
    }


    void Move()
    {
        if (currentState == EnemyState.Wait)
        {
        // Get the player's position
            Vector3 playerPosition = player.transform.position;

            // Calculate the direction from the enemy to the player
            Vector3 direction = (playerPosition - transform.position).normalized;
            direction.y = 0; // Ignore vertical difference
            Vector3 newVel = direction * speed;
            physics.SetVelocity(new Vector3 (newVel.x, physics.velocity.y, newVel.z));

            // Make the enemy face the player's position
            //transform.LookAt(playerPosition);
        
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

           
        }
    }

    IEnumerator CheckPlayer(float time) {
        yield return new WaitForSeconds(time);
        overlaps = Physics.OverlapSphere(transform.position, spherecastRadius, LayerMask.GetMask("Player"));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spherecastRadius);
    }


} 	