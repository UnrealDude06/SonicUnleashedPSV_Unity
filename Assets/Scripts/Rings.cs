using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rings : MonoBehaviour {

    public int ringValue = 10;  // The value associated with the ring
    public float rotationSpeed = 30f;  // Speed of rotation in degrees per second
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RingCollectionManager.Instance.CollectRing(ringValue);  // Trigger the ring collection event
            Destroy(gameObject);  // Destroy the ring object
        }
    }

}