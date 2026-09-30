using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneChunkLoader : MonoBehaviour {

public Transform player;
    public GameObject sectionToToggle;

    public float activationDistance = 100f;
    public GameObject nextSectionTrigger; // Optional, for unloading

    private bool isLoaded = false;

    public void CheckDistance()
    {
        float dist = Vector3.Distance(player.position, transform.position);

        if (!isLoaded && dist < activationDistance)
        {
            sectionToToggle.SetActive(true);
            isLoaded = true;
        }

        // Optional: Deload if player passed the next section's trigger
        if (isLoaded && nextSectionTrigger != null && 
            Vector3.Distance(player.position, nextSectionTrigger.transform.position) < activationDistance-50)
        {
			
            sectionToToggle.SetActive(false);
            isLoaded = false;
        }
    }
}