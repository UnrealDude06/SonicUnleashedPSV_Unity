using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnleashedCam : MonoBehaviour {

    public UnleashedCamera2D cam2d;
    public UnleashedCamera3D cam3d;

    SonicController sonic;
	private void Start() {
		cam2d = GetComponent<UnleashedCamera2D>();
        cam3d = GetComponent<UnleashedCamera3D>();
        sonic = cam3d.target.GetComponentInParent<SonicController>();
	}
    private void LateUpdate()
    {
        if(sonic.twoDmode)
        {
            cam2d.enabled = (true);
            cam3d.enabled = (false);
        }
        else
        {
            cam2d.enabled = (false);
            cam3d.enabled = (true);
        }
    }

    
}