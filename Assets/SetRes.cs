using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetRes : MonoBehaviour {
public int ScreenSizeX,ScreenSizeY;


 private void OnEnable()
    {
        StartCoroutine(ApplyResolutionWithDelay());
    }

    private IEnumerator ApplyResolutionWithDelay()
    {
        // Wait one frame to ensure Unity's internal resolution sync completes
        yield return null;
        Screen.SetResolution(ScreenSizeX, ScreenSizeY, FullScreenMode.FullScreenWindow);
    }
	private void Awake() {
        Screen.SetResolution(ScreenSizeX, ScreenSizeY, FullScreenMode.FullScreenWindow);
    }
	
}
