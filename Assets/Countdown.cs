using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Countdown : MonoBehaviour {

[SerializeField] private Animator animSonic, animCamera,animUI;

public float countdownTime = 4.20f; 	
public bool go;
private GameObject parent;
private Camera cam;
private SonicController sonic;



	// Use this for initialization
	void Start () {
		parent = this.gameObject;
		cam = Camera.main;
		sonic = GetComponentInChildren<SonicController>();

		cam.GetComponent<UnleashedCam>().enabled = false;
		sonic.gameObject.SetActive(false);
		animSonic.gameObject.SetActive(true);

	}
	
	// Update is called once per frame
	void Update () {
		countdownTime -= Time.deltaTime;
		go = countdownTime < 3 ? true : false;
		if(go)
		{
			
			animSonic.SetBool("go",true);
			animCamera.SetBool("go",true);
			animUI.SetBool("go",true);

			if(countdownTime < 0)
			{
				cam.transform.SetParent(null, true);
				cam.GetComponent<UnleashedCam>().enabled = true;
				sonic.gameObject.SetActive(true);
				sonic.transform.SetParent(null, true);

				Destroy(animCamera);
				Destroy(parent);
			}
		}

		if(Input.GetKeyDown(KeyCode.W))
		{
			countdownTime = 0;
			go = true;
		}
	}
}
