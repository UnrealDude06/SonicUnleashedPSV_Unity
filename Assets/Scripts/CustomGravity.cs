using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGravity : MonoBehaviour {

// casting Rigidbodies is expensive, this allows the enemy to
// align themselves with the ground without a rigidbody.

    public LayerMask layerMask;
    public float alignmentInterval = 1.0f;
    public float groundCheckDistance = 0.1f;
    [SerializeField]private float gravity;

    private Transform enemyTransform;
    private CapsuleCollider capsuleCollider;
    public Vector3 velocity;
    public bool isGrounded = false;

    private void Awake()
    {
        enemyTransform = GetComponent<Transform>();
        capsuleCollider = GetComponent<CapsuleCollider>();

        AlignWithGround();
    }

	 private void Update() {
		{
            AlignWithGround();
            CollisionChecks();
			
            StartCoroutine(GroundAlign(0.2f));

            enemyTransform.position += velocity * Time.deltaTime;
		}
	}
    IEnumerator GroundAlign(float time)
    {
        yield return new WaitForSeconds(time);
        AlignWithGround();
    }
    private void AlignWithGround()
    {
		Vector3 bottomPoint = enemyTransform.position - new Vector3(0, capsuleCollider.height / 2 - capsuleCollider.radius, 0);
        RaycastHit hit;
        if (Physics.Raycast(bottomPoint, -enemyTransform.up, out hit, groundCheckDistance, layerMask))
        {
             if (hit.collider != gameObject)
                {
                    velocity.y = 0f;
                    isGrounded = true;
                    
                    //Rotate the player to align with the surface normal
                    enemyTransform.rotation = Quaternion.FromToRotation(enemyTransform.up, hit.normal) * enemyTransform.rotation;
                }
        }
        else
        {
            isGrounded = false;
        }

        Debug.DrawRay(bottomPoint, -enemyTransform.up *groundCheckDistance, Color.green);
    }

    private void CollisionChecks()
    {
        if (isGrounded)
            {
                velocity.y = 0f;
            }
            else
            {
                velocity += Vector3.down * gravity * Time.deltaTime;
            }

        if (Physics.Raycast(transform.position, transform.forward, 2, layerMask))
        {
            Vector3 forwardDir = transform.forward.normalized;
            velocity -= Vector3.Project(velocity, forwardDir);

        }

        
    }

    public void SetVelocity(Vector3 newVelocity)
    {
        velocity.x = newVelocity.x;
        velocity.z = newVelocity.z;

        if (!isGrounded)
        {
            velocity.y += -gravity * Time.deltaTime;
        }

        
    }
}