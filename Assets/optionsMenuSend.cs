using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class optionsMenuSend : MonoBehaviour {

	public int optionsSceneIndex;
	 public void optionsSelcet()
    {
        Debug.Log("Selected Option: " + gameObject.name);

        // Load the specified scene by index when this option is selected
        SceneManager.LoadScene(optionsSceneIndex);
    }
}
