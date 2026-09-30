using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class RingLost : MonoBehaviour {
        [SerializeField]private LostRingPool pool;
	    [SerializeField]public float speed = 5f;

        [SerializeField]public float gravity = 9.81f;

        [SerializeField]public float aliveTime = 7;
        [SerializeField]public float grabTime = 1;

        [SerializeField]private Vector3 velocity = Vector3.zero; // Current velocity of the object

    
    private Dictionary<string, object> initialFieldValues = new Dictionary<string, object>();

    private void Awake() {

        pool = FindObjectOfType<LostRingPool>();
        SaveInitialFieldValues();
    }
 
    private void Start()
    {
        // Get a random rotation around the y-axis
        float randomYRotation = UnityEngine.Random.Range(0, 360);

        // Apply the rotation to the ring
        transform.rotation = Quaternion.Euler(0f, randomYRotation, 0f);
    }

    private void Update()
    {
          aliveTime -= Time.deltaTime;
        // Apply gravity
        velocity.y -= gravity * Time.deltaTime;
        // Check if there's ground beneath the object
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 1f, LayerMask.GetMask("Default"));
        // Adjust velocity.y based on grounding
        velocity.y = isGrounded ? 0f : velocity.y;
        // Move the object based on velocity
        transform.position += velocity * Time.deltaTime;

        // Move the ring forwards
        transform.Translate(transform.forward * speed * Time.deltaTime);

        speed = Mathf.MoveTowards(speed, 0 , 5 * Time.deltaTime);

        // Reduce aliveTime only if the grabTime is zero
        if (aliveTime < 0)
        {
          pool.ReturnRingToPool(this.gameObject);  // Destroy the ring object
            ResetScript();
        }


        // Allows Ring to be grabbed to avoid grab on spawn
        if (grabTime > 0)
        {
            grabTime -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && grabTime < 0)
        {
            if(grabTime < 0.1)
            {
                RingCollectionManager.Instance.CollectRing(1);  // Trigger the ring collection event
                pool.ReturnRingToPool(this.gameObject);  // Destroy the ring object
                ResetScript();

            }
        }
    }

    public void SaveInitialFieldValues()
    {
        // Save initial values of serialized fields
        FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (FieldInfo field in fields)
        {
            if (field.IsDefined(typeof(SerializeField), false))
            {
                initialFieldValues[field.Name] = field.GetValue(this);
            }
        }
    }

    public void ResetScript()
    {
        // Reset all variables to their initial values
        FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (FieldInfo field in fields)
        {
            if (field.IsDefined(typeof(SerializeField), false))
            {
                field.SetValue(this, initialFieldValues[field.Name]);
            }
        }
    }

   



}