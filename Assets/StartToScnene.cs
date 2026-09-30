using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartToScnene : MonoBehaviour {


	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Start"))
		{
			 // Reload the currently active scene by its index
        	LevelChanger.Instance.LoadScene("TitleScreen");

		}
		if(Input.GetButtonDown("Select"))
		{
			// Get the index of the currently active scene
			int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

			// Reload the currently active scene by its index
			SceneManager.LoadScene(currentSceneIndex -1);
		}

	}
}
