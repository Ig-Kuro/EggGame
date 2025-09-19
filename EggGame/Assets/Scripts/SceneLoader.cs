using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class SceneLoader : MonoBehaviour
{
    public string mainSceneName;

    [System.Serializable]
    public class SceneData
    {
        public string sceneName;
        public float yOffset;
    }

    public SceneData[] additiveScenes;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == mainSceneName)
        {
            StartCoroutine(LoadAllScenes());
        }
    }

    IEnumerator LoadAllScenes()
    {
        foreach (var sceneData in additiveScenes)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneData.sceneName, LoadSceneMode.Additive);

            while (!op.isDone)
            {
                yield return null;
            }

            Scene loadedScene = SceneManager.GetSceneByName(sceneData.sceneName);
            if (loadedScene.IsValid())
            {
                GameObject[] rootObjects = loadedScene.GetRootGameObjects();
                foreach (var obj in rootObjects)
                {
                    obj.transform.position += new Vector3(0, sceneData.yOffset, 0);
                }
            }
        }
    }
}