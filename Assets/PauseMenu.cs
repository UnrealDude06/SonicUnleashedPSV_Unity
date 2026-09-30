using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

	public static bool gameIsPaused = false;

	public GameObject pause_ui;


	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Start"))
		{
			if(gameIsPaused)
			{
				Resume();
				gameIsPaused = false;
			}
			else
			{
				Pause();
				gameIsPaused = true;
			}
		}

	}


	public void Resume()
	{
		
		pause_ui.SetActive(false);
		Time.timeScale = 01f;
		gameIsPaused = false;
	}
	void Pause()
	{
		pause_ui.SetActive(true);
		Time.timeScale = 0f;
		gameIsPaused = true;
	}

	public void RestartScene()
    {
        // Get the index of the currently active scene
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Reload the currently active scene by its index
        SceneManager.LoadScene(currentSceneIndex);
		Resume();

    }

	public void QuitGame()
    {
     
        // Reload the currently active scene by its index
        SceneManager.LoadScene(0);
		Resume();

    }
}
