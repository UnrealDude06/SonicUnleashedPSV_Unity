using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class menuController : MonoBehaviour {
public Image[] menuOptions;
    private int selectedOption = 0;

    public Color color,color_select;

    public string optionsSceneIndex, levelSceneIndex, aridSandsLevel, testScene;

    // Variables for cooldown mechanism
public float selectionCooldownTime = 0.5f; // Time between each selection change
private float lastSelectionTime = 0f;

    void Start()
    {
        // Select the first option by default
        SelectOption(selectedOption);
    }

    void Update()
    {
          Debug.Log("Selected Option: " + menuOptions[selectedOption].gameObject.name);
        float verticalInput = Input.GetAxis("Vertical");
        if (verticalInput != 0)
        {
            // Only change selection if there's no cooldown in effect
            if (verticalInput > 0 && Time.time - lastSelectionTime >= selectionCooldownTime)
            {
                lastSelectionTime = Time.time;
                ChangeSelectedOption(-1); // Move selection up
            }
            else if (verticalInput < 0 && Time.time - lastSelectionTime >= selectionCooldownTime)
            {
                lastSelectionTime = Time.time;
                ChangeSelectedOption(1); // Move selection down
            }
        }

        // Execute option when X button is pressed
        if (Input.GetButtonDown("X"))
        {
            ExecuteOption();
        }
    }

    void ChangeSelectedOption(int direction)
    {
        // Deselect the current option
        menuOptions[selectedOption].GetComponent<Image>().color = (color);

        // Update the selected option index
        selectedOption += direction;

        // Ensure the selected option index stays within bounds
        if (selectedOption < 0)
            selectedOption = menuOptions.Length - 1;
        else if (selectedOption >= menuOptions.Length)
            selectedOption = 0;

        // Select the new option
        SelectOption(selectedOption);
    }

    void SelectOption(int optionIndex)
    {
        // Select the specified option
        menuOptions[selectedOption].GetComponent<Image>().color = (color_select);
    }

    void ExecuteOption()
    {
        // Perform the action of the selected option here
        Debug.Log("Selected Option: " + menuOptions[selectedOption].gameObject.name);

        // Execute the action based on the selected option
        switch (selectedOption)
        {
            case 0:
                OnSelectPlay();
                break;
            case 1:
                OnSelectSettings();
                break;
            case 2:
                AridSands();
                break;
            // Add more cases for additional options
            case 3:
                LoadingScene(testScene);
            break;
        }
    }

    public void OnSelectPlay()
    {
        Debug.Log("Selected Option: " + gameObject.name);
        LevelChanger.Instance.LoadScene(levelSceneIndex);

    }

    public void OnSelectSettings()
    {
        Debug.Log("Selected Option: " + gameObject.name);
        LevelChanger.Instance.LoadScene(optionsSceneIndex);

    }
    public void AridSands()
    {
        Debug.Log("Selected Option: " + gameObject.name);
        LevelChanger.Instance.LoadScene(aridSandsLevel);
    }

    public void LoadingScene(String name)
    {
        Debug.Log("Selected Option: " + gameObject.name);
        LevelChanger.Instance.LoadScene(name);
    }


}
