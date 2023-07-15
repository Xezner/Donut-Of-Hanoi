using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BuildSceneManager : MonoBehaviour
{
    public static BuildSceneManager Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadSceneAsync(BuildScene buildScene)
    {
        SceneManager.LoadSceneAsync((int)buildScene);
    }
}

public enum BuildScene
{
    MainMenuScene = 0,
    GameScene = 1,
    FTUEScene = 2,
}
