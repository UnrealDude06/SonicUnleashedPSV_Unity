using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraShake : MonoBehaviour {

   public float shakeDuration = 0.5f; // The duration of the shake in seconds.
    public float shakeAmount = 0.1f; // The intensity of the shake.
    public float decreaseFactor = 1.0f; // The rate at which the shake decreases.

    private Vector3 originalPosition;
	public bool startShake = false;
	float normalShakeDuration;

    private void Start()
    {
        normalShakeDuration  =shakeDuration;
    }

    void Update()
    {
		if(startShake)
		{
			originalPosition = transform.localPosition;
			if (shakeDuration > 0)
			{
				transform.localPosition = originalPosition + Random.insideUnitSphere * shakeAmount;
				shakeDuration -= Time.deltaTime * decreaseFactor;
			}
			else
			{
				shakeDuration = normalShakeDuration;
				startShake = false;
				transform.localPosition = originalPosition;
			}
		}
    }
	
}