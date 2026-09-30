using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoBehaviour {
    public GameObject explosionPrefab; // Reference to the explosion prefab
    public int maxExplosionCount = 10; // Maximum number of explosion instances to create

    private List<GameObject> explosionPool; // Pool of explosion instances

    void Start()
    {
        // Create the explosion pool
        explosionPool = new List<GameObject>();

        for (int i = 0; i < maxExplosionCount; i++)
        {
            GameObject explosion = Instantiate(explosionPrefab);
            explosion.SetActive(false);
            explosion.transform.parent = transform; // Parent the explosion instance to the ExplosionPool
            explosionPool.Add(explosion);
        }
    }

    public void SpawnExplosion(Vector3 position)
    {
        // Get an available explosion from the pool
        GameObject explosion = GetAvailableExplosion();

        if (explosion != null)
        {
            // Position and enable the explosion
            explosion.transform.position = position;
            explosion.SetActive(true);

            // Start a coroutine to disable the explosion after a certain duration
            StartCoroutine(DisableExplosion(explosion));
        }
    }

    GameObject GetAvailableExplosion()
    {
        // Find an available explosion in the pool
        foreach (GameObject explosion in explosionPool)
        {
            if (!explosion.activeInHierarchy)
            {
                return explosion;
            }
        }

        return null; // Return null if no available explosion found
    }

    IEnumerator DisableExplosion(GameObject explosion)
    {
        yield return new WaitForSeconds(2f); // Replace 2f with your desired explosion duration

        // Disable the explosion and move it back to the pool
        explosion.SetActive(false);
    }
}