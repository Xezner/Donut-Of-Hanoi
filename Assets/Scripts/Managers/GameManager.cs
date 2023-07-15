using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _successUI;
    [SerializeField] private ErrorPromptController _errorPrompt;
    [SerializeField] private Canvas _worldSpaceCanvas;

    public bool IsGamePaused = true;
    public bool IsPlayerControllable = true;


    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene previousScene, Scene loadedScene)
    {
        Debug.Log(previousScene.name);
        Debug.Log(loadedScene.name);
        if (loadedScene.name == BuildScene.GameScene.ToString())
        {
            InitGameStart();
        }
    }
    public void InitGameStart()
    {
        _errorPrompt.gameObject.SetActive(true);
        _errorPrompt.GetPlayer();

        DiskSpawnManager.Instance.SpawnDisks();

        _worldSpaceCanvas.worldCamera = Camera.main;
    }


    public void StartGame()
    {
        //!!TODO: Add FTUE Checker here
        SceneManager.LoadSceneAsync((int)BuildScene.GameScene);
        IsGamePaused = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    
    public void ErrorPrompt(ErrorType errorType)
    {
        _errorPrompt.ErrorPrompt(errorType);
    }

    public void SuccessPrompt(Vector3 position)
    {
        Vector3 offset = new(0, 3f, 1f);
        _successUI.SetActive(false);
        _successUI.transform.position = position + offset;
        _successUI.SetActive(true);
    }
}

public enum BuildScene
{
    MainMenuScene = 0,
    GameScene = 1,
    FTUEScene = 2,
}