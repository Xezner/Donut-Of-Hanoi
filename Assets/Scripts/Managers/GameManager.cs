using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    //CHEAT
    public bool ResetFTUE = false;


    [Header("UI Prompts")]
    [SerializeField] private GameObject _successUI;
    [SerializeField] private ErrorPromptController _errorPrompt;

    [Header("UI Canvas")]
    [SerializeField] private Canvas _worldSpaceCanvas;

    [Header("UI ZenMode")]
    [SerializeField] private GameObject _zenModePopUp;
    [SerializeField] private TMP_InputField _spawnCountText;


    private GameMode _gameMode;
    private int _spawnSize = 3;
    private int _minSpawnSize = 3;
    private int _maxSpawnSize = 9;
    private float _countdownTimer = 3f;
    private float _countdown = 3f;

    //States
    public bool IsCountdownStarted = false;
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
        if(ResetFTUE)
        {
            FTUEManager.Instance.ClearPlayerPrefs();
        }

        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void Update()
    {
        if(IsCountdownStarted && _gameMode != GameMode.FTUE)
        {
            _countdown -= Time.deltaTime;
            if(_countdown <= 0f)
            {
                IsCountdownStarted = false;
                Debug.Log("GAME STARTING");
                DiskSpawnManager.Instance.SpawnDisks(_spawnSize);

                UnPauseGame();
            }
        }

        if(_spawnCountText.isActiveAndEnabled)
        {
            Debug.Log("HERE");
            bool isNumber = int.TryParse(_spawnCountText.text, out int spawnCount);
            if(spawnCount < _minSpawnSize|| !isNumber)
            {
                _spawnCountText.text = _minSpawnSize.ToString();
            }
            else if (spawnCount > _maxSpawnSize)
            {
                _spawnCountText.text = _maxSpawnSize.ToString();
            }
        }
    }

    private void SceneManager_activeSceneChanged(Scene previousScene, Scene loadedScene)
    {
        if(loadedScene.name == BuildScene.MainMenuScene.ToString())
        {
            PauseGame();
        }
        if (loadedScene.name == BuildScene.GameScene.ToString())
        {
            InitGameStart();
        }
        if(loadedScene.name == BuildScene.FTUEScene.ToString())
        {
            InitFTUEStart();
        }
    }
    private void InitGameStart()
    {
        _errorPrompt.gameObject.SetActive(true);
        _errorPrompt.GetPlayer();
        _worldSpaceCanvas.worldCamera = Camera.main;

        GetSpawnSizeByGameMode();
    }

    private void GetSpawnSizeByGameMode()
    {
        switch(_gameMode)
        {
            case GameMode.Standard:
            {
                _spawnSize = FTUEManager.Instance.GetInt(FTUE.Level);
                _spawnSize = _spawnSize > _maxSpawnSize ? _maxSpawnSize : _spawnSize;
                SetGameCountdown();
                break;
            }
            case GameMode.ZenMode:
            {
                int.TryParse(_spawnCountText.text, out _spawnSize);
                SetGameCountdown();
                break;
            }
            default: break;
        }
    }

    public void InitZenModeStart()
    {
        _zenModePopUp.SetActive(true);
    }

    public void ZenModeCallback()
    {
        BuildSceneManager.Instance.LoadSceneAsync(BuildScene.GameScene);
    }

    public void SetGameMode(int gameModeIndex)
    {
        _gameMode = (GameMode)gameModeIndex;
    }

    public void InitFTUEStart()
    {
        _errorPrompt.gameObject.SetActive(true);
        _errorPrompt.GetPlayer();
        _worldSpaceCanvas.worldCamera = Camera.main;
        _spawnSize = _minSpawnSize;

        FTUEManager.Instance.ClearPlayerPrefs();
        FTUEManager.Instance.ControlsTutorialStart();
    }

    private void SetGameCountdown()
    {
        _countdown = _countdownTimer;
        IsCountdownStarted = true;
    }

    public void StartGame()
    {
        IsPlayerControllable = false;
        if (!FTUEManager.Instance.GetBool(FTUE.FinishedTutorial))
        {
            BuildSceneManager.Instance.LoadSceneAsync(BuildScene.FTUEScene);
        }
        else
        {
            BuildSceneManager.Instance.LoadSceneAsync(BuildScene.GameScene);
        }
    }

    public void PauseGame()
    {
        IsGamePaused = true;
        IsPlayerControllable = false;
    }

    public void UnPauseGame()
    {
        IsGamePaused = false;
        IsPlayerControllable = true;
    }

    public void PauseGameToggle()
    {
        IsGamePaused = !IsGamePaused;
        IsPlayerControllable = !IsPlayerControllable;
        Debug.Log($"Game Paused: {IsGamePaused}");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GameOverScreen()
    {
        if (!FTUEManager.Instance.GetBool(FTUE.FinishedTutorial) && SceneManager.GetActiveScene().name == BuildScene.FTUEScene.ToString())
        {
            FTUEManager.Instance.FinishedTutorialStart();
        }
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

public enum GameMode
{
    FTUE,
    Standard,
    ZenMode
}

