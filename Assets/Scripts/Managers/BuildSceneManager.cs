using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject _transitionPanel;
    public static BuildSceneManager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadSceneAsync(BuildScene buildScene)
    {
        GameManager.Instance.PauseGame();
        _transitionPanel.SetActive(false);
        Debug.Log("LOADING SCENE");
        StartCoroutine(LoadAsync(buildScene));
        //_transitionPanel.SetActive(true);
        //SceneManager.LoadSceneAsync((int)buildScene, LoadSceneMode.Single);
    }

    private IEnumerator LoadAsync(BuildScene buildScene)
    {
        Debug.Log("Loading scene");
        _transitionPanel.SetActive(true);
        float elapsedTime = 0f;
        while(elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SceneManager.LoadSceneAsync((int)buildScene, LoadSceneMode.Single);
    }
}

public enum BuildScene
{
    MainMenuScene = 0,
    GameScene = 1,
    FTUEScene = 2,
}
