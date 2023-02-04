using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingManager : MonoBehaviour
{
	private static SceneLoadingManager _instance;

	private void Awake()
	{
		if(_instance == null) _instance = this;
		else
		{
			Destroy(this.gameObject);
			return;
		}
		DontDestroyOnLoad(this.gameObject);
	}

	public static void LoadLevel(string levelName)
	{
		_instance.StartCoroutine(AsyncSceneLoad(levelName));
	}
	private static IEnumerator AsyncSceneLoad(string levelName)
	{
		AsyncOperation operation = SceneManager.LoadSceneAsync(levelName);
		while(!operation.isDone)
		{
			yield return null;
		}
		operation.allowSceneActivation = true;
	}
}
