using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour {
    public ResoloutionScaler res;
    private Slider fps_slider;
    public TMP_Text fps_slider_text;

    public Toggle enableVsync;

    int enabelvsyns = -1;




    void Start()
    {
        // Find and assign references
        res = FindObjectOfType<ResoloutionScaler>();
        fps_slider = GetComponentInChildren<Slider>();

        fps_slider.value = (int)PlayerPrefs.GetFloat("fps");

        // Update UI text initially
        UpdateFPSUI();
    }

    // Update is called once per frame
    void Update()
    {
        // Update FPS value based on slider
        res.fps = (int)fps_slider.value;
        StartCoroutine(SaveFPS(1f));

        

        // Update UI text
        UpdateFPSUI();
    }

    // Update FPS UI text
    void UpdateFPSUI()
    {
        fps_slider_text.SetText("FPS = " + res.fps);
    }

    // Public method to enable FPS with specified number of frames
    public void EnableFPS(int numOfFrames)
    {
        res.fps = numOfFrames;
    }

    public void EnableVsync()
    {
        enabelvsyns *= -1;
        PlayerPrefs.SetInt("vsync", enabelvsyns == -1 ? 0 : 1);
        if(enabelvsyns > 0)
        {
            PlayerPrefs.SetInt("vsync", 1);
        }
        if(enabelvsyns < 0)
        {
            PlayerPrefs.SetInt("vsync", 0);
        }
       

    }

    IEnumerator SaveFPS(float time)
    {
        yield return new WaitForSeconds(time);
        PlayerPrefs.SetFloat("fps", (int)fps_slider.value);
        res.fps = (int)fps_slider.value;
    }
}