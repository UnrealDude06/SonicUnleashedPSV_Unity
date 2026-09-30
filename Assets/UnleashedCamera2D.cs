using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnleashedCamera2D : MonoBehaviour {
	public Transform player; // Reference to the player
	public float transitionSpeed;
	 public Vector3 offset2D = new Vector3(-9, 2, 2); // Offset for 2D mode

    private Vector3 targetPosition; // Desired position of the camera
    private Quaternion targetRotation; // Desired rotation of the camera

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
		targetPosition = player.position + offset2D;
               // Smoothly rotate the camera to look at the target
        Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);
         transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 12);

			// Smoothly interpolate camera position and rotation
        transform.position = Vector3.Lerp(transform.position, targetPosition, transitionSpeed * Time.deltaTime);
	}
}
