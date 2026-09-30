using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour {

public static LevelChanger Instance;

    [SerializeField] private GameObject loaderCanvas;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (loaderCanvas != null)
            loaderCanvas.SetActive(false);
    }

    public void LoadScene(string sceneName)
	{
		StartCoroutine(LoadSceneRoutine(sceneName));
	}

	private IEnumerator LoadSceneRoutine(string sceneName)
	{
		loaderCanvas?.SetActive(true);

		AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
		op.allowSceneActivation = false;

		while (op.progress < 0.9f)
			yield return null;

		yield return new WaitForSeconds(0.1f); // Give Vita breathing space
		op.allowSceneActivation = true;
		yield return new WaitForSeconds(1f);
		loaderCanvas?.SetActive(false);
	}
}