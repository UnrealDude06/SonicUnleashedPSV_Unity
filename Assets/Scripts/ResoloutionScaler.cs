using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResoloutionScaler : MonoBehaviour {
	public int fps = 30;
    public bool useVsync = false;
	private static ResoloutionScaler instance;

	void Start()
	{
		 // Singleton pattern
        killMe_if_i_exist();

		DontDestroyOnLoad(this.gameObject);
        if((int)PlayerPrefs.GetFloat("fps") > 0)
        {
            fps = (int)PlayerPrefs.GetFloat("fps");
        }
  
        Application.targetFrameRate = fps;
        QualitySettings.vSyncCount = 0;

        // Optional: adjust physics update rate too
        //Time.fixedDeltaTime = 1f / fps;
		


	}
    private void Awake() {
        //Application.targetFrameRate = (int)fps;
        if((int)PlayerPrefs.GetFloat("fps") > 0)
        {
            fps = (int)PlayerPrefs.GetFloat("fps");
        }

        QualitySettings.vSyncCount = -1; // Make sure vSync isn't fighting us
        Application.targetFrameRate = fps;

        // Optional: adjust physics update rate too
        Time.fixedDeltaTime = 1f / fps;
    }

	void killMe_if_i_exist()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Update is called once per frame
	 void Update()
    {

    }
	
}
