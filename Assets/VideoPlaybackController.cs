using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoPlaybackController : MonoBehaviour {

public Animator logoAnimator;
    public GameObject logoCanvas;      // the canvas to destroy
    public GameObject titleScreen;     // the title screen to enable

    private bool animationFinished = false;

    void Update()
    {
        if (!animationFinished)
        {
            if (logoAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f &&
                !logoAnimator.IsInTransition(0))
            {
                animationFinished = true;
                OnLogoAnimationFinished();
            }
        }
        if(Input.GetButtonDown("Start"))
        {
            OnLogoAnimationFinished();
        }
    }

    void OnLogoAnimationFinished()
    {
        titleScreen.SetActive(true);       // show title screen
        Destroy(logoCanvas);               // remove logo canvas
    }
}