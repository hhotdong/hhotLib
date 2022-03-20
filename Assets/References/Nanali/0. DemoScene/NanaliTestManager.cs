using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NanaliTestManager : MonoBehaviour
{
	private static NanaliTestManager _instance;
	public static NanaliTestManager Instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType(typeof(NanaliTestManager)) as NanaliTestManager;
			return _instance;
		}
	}

	public void LoadScene(int sceneIndex)
	{
		StartCoroutine(LoadSceneCoroutine(sceneIndex));
	}

	IEnumerator LoadSceneCoroutine(int sceneIndex)
	{
		AsyncOperation async = SceneManager.LoadSceneAsync(sceneIndex);
		while (!async.isDone) {
			yield return null;
		}
    }
}
