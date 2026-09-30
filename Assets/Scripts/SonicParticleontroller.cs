using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonicParticleontroller : MonoBehaviour {

[SerializeField] private ParticleSystem dust;
[SerializeField] private ParticleSystem dust_burst,water;
[SerializeField] private TrailRenderer windL;
[SerializeField] private TrailRenderer windR;

[SerializeField] private TrailRenderer HomingAttkTrail;
private SonicController sonic;
private SonicObejctInteraction sonic_col;
private cameraShake camshake;
private Camera cam;
	// Use this for initialization
	void Start () {
		sonic = GetComponent<SonicController>();
		sonic_col = GetComponent<SonicObejctInteraction>();
		camshake =Camera.main.GetComponent<cameraShake>();
		cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
		bool shouldShowDust = sonic.isGrounded && sonic.speed > 19;

		dust.emissionRate = shouldShowDust ? Mathf.Lerp(dust.emissionRate, 15f, 10 * Time.deltaTime) : -1f;
		dust_burst.gameObject.SetActive(shouldShowDust);
		dust.gameObject.SetActive(shouldShowDust);

		water.gameObject.SetActive(sonic_col.water);

		if(sonic.boost)
		{
			camshake.startShake = true;
			cam.fieldOfView = Mathf.Lerp(cam.fieldOfView,100, 3 * Time.deltaTime);
			
		}
		else
		{
			cam.fieldOfView = Mathf.Lerp(cam.fieldOfView,63, 3 * Time.deltaTime);
		}
		if(sonic.sonic_action == SonicController.Actions.attack)
		{
			HomingAttkTrail.enabled = true;
			HomingAttkTrail.time = 0.5f;
		}
		else
		{
			HomingAttkTrail.time = 0;
			HomingAttkTrail.enabled  =false;
		}


		
//		windL.time = (sonic.speed >  sonic.maxSpeed_og &&sonic.isGrounded || sonic.sonic_col.current_action != SonicObejctInteraction.action.none) ? 0.19f 	: 	0	;
//		windR.time = (sonic.speed > sonic.maxSpeed_og &&sonic.isGrounded || sonic.sonic_col.current_action != SonicObejctInteraction.action.none) ? 0.19f 	: 	0	;
		

	}
}
