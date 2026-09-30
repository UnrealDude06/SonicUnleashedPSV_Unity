using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuOption : MonoBehaviour {

    public int sceneIndex; // Index of the scene to load when this option is selected

    public void OnSelect()
    {
        Debug.Log("Selected Option: " + gameObject.name);

        // Load the specified scene by index when this option is selected
        SceneManager.LoadScene(sceneIndex);
    }

    

}