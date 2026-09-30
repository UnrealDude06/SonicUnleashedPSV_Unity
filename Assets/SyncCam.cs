using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncCam : MonoBehaviour {
	public GameObject mainCamera;
    public GameObject skyCamera;
    public float rotationAmount;

    void LateUpdate()
    {
        if (mainCamera != null && skyCamera != null)
        {
            skyCamera.transform.position = mainCamera.transform.position;
            transform.Rotate(0,rotationAmount * Time.deltaTime,0);
        }
    }
}