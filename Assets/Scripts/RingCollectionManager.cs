using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingCollectionManager : MonoBehaviour {

 public static RingCollectionManager Instance;  // Singleton instance

    public RingCollectionEvent OnRingCollected;  // The ring collection event

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        // Initialize the ring collection event
        OnRingCollected = new RingCollectionEvent();
    }

    public void CollectRing(int ringValue)
    {
        // Trigger the ring collection event and pass the ring value
        OnRingCollected.Invoke(ringValue);
    }
}