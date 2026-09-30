using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkTriggerLoad : MonoBehaviour {
    public GameObject chunkToLoad; // Object to load when the player enters the trigger
    public GameObject chunkToUnload; // Object to unload when the player exits the trigger
    
    private bool playerInsideTrigger = false; // Flag to check if the player is inside the trigger
    public bool deleteFromRAM;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LoadChunk();
            UnloadChunk();
            playerInsideTrigger = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UnloadChunk();
            LoadChunk();
            playerInsideTrigger = false;
        }
    }

    private void Update()
    {
        if (playerInsideTrigger)
        {
            LoadChunk();
            UnloadChunk();
        }
    }

    private void LoadChunk()
    {
        if (chunkToLoad != null)
        {
            chunkToLoad.SetActive(true);
        }
    }

    private void UnloadChunk()
    {
        if (chunkToUnload != null)
        {
            chunkToUnload.SetActive(false);
            if(deleteFromRAM)
            {
                Destroy(chunkToUnload.gameObject);
                
            }
        }
    }
}