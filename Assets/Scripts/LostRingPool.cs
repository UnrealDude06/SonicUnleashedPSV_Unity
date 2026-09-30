using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostRingPool : MonoBehaviour {  
    public GameObject ringPrefab;
    public int numberOfRings = 10;
    public float forceMagnitude = 10f;
    public Transform spawnPoint;

    private List<GameObject> ringPool = new List<GameObject>();
    private List<GameObject> availableRings = new List<GameObject>(); // New list for available rings

    public static LostRingPool instance;

    void Awake()
    {
        instance = this;
    }


    void Start()
    {
        InitializePool();
    }

    void InitializePool()
    {
        for (int i = 0; i < numberOfRings; i++)
        {
            GameObject ring = Instantiate(ringPrefab);
            ring.SetActive(false);
            ringPool.Add(ring);
            availableRings.Add(ring); // Add the ring to the available list
        }
    }

    public void SpawnRings()
    {
        if (availableRings.Count == 0)
        {
            Debug.LogWarning("No available rings in the pool.");
            foreach (GameObject rings in ringPool)
            {
                rings.SetActive(false);
                rings.GetComponent<RingLost>().ResetScript();
  
                // Activate and initialize the ring
                rings.transform.position = spawnPoint.position;
            
                rings.SetActive(true);
                return;
            }
            
        }
        
        // Take the first available ring from the list
        GameObject ring = availableRings[0];
        availableRings.RemoveAt(0); // Remove it from the available list
        
        // Activate and initialize the ring
        ring.transform.position = spawnPoint.position;
       
        ring.SetActive(true);
        
    }

    // Method to return a ring to the available list when deactivated
    public void ReturnRingToPool(GameObject ring)
    {
        ring.SetActive(false);
        ring.GetComponent<RingLost>().ResetScript();
        availableRings.Add(ring);
    }

}