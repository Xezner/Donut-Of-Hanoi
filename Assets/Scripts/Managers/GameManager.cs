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

    [Header("UI MainMenu")]
    [SerializeField] private GameObject _mainMenuUI;

    [Header("UI Prompts")]
    [SerializeField] private GameObject _successUI;
    [SerializeField] private ErrorPromptController _errorPrompt;

    [Header("UI Canvas")]
    [SerializeField] private Canvas _worldSpaceCanvas;

    [Header("UI ZenMode")]
    [SerializeField] private GameObject _zenModePopUp;
    [SerializeField] private TMP_InputField _spawnCountText;

    [Header("Level Manager")]
    [SerializeField] private LevelManager _levelmanager;

    [Header("UI Move and Time")]
    [SerializeField] private GameObject _gameUI;
    [SerializeField] private TextMeshProUGUI _moveText;
    [SerializeField] private TextMeshProUGUI _timerText;
    [SerializeField] private TextMeshProUGUI _levelText;
    //GameMode
    private GameMode _gameMode;

    //Spawn Data
    private int _spawnSize = 3;
    private int _minSpawnSize = 3;
    private int _maxSpawnSize = 9;

    //Countdown Data
    private float _countdownTimer = 3f;
    private float _countdown = 3f;

    //LevelData
    public int Moves;
    public float TimeSpent;
    private int _level;

    //States
    public bool IsCountdownStarted = false;
    public bool IsGamePaused = true;
    public bool IsPlayerControllable = true;
    public bool IsGameStarted = false;

    public static GameManager Instance;
    private void Awake()
    {
        Debug.Log("AWAKE");
        if(Instance != null && Instance !=this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if(ResetFTUE)
        {
            FTUEManager.Instance.ClearPlayerPrefs();
        }
        Debug.Log("START");
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene loadedScene, LoadSceneMode arg1)
    {
        if(loadedScene.name == BuildScene.MainMenuScene.ToString())
        {
            ResetGame();
        }
        else if (loadedScene.name == BuildScene.GameScene.ToString())
        {
            InitGameStart();
        }
        else if (loadedScene.name == BuildScene.FTUEScene.ToString())
        {
            InitFTUEStart();
        }
    }
    private void Update()
    {
        StartCountdown();
        StartTimer();
        ZenModePopUp();
    }

    private void SetGameCountdown()
    {
        _countdown = _countdownTimer;
        IsCountdownStarted = true;
    }

    private void StartCountdown()
    {
        if (IsCountdownStarted && _gameMode != GameMode.FTUE)
        {
            _countdown -= Time.deltaTime;
            if (_countdown <= 0f)
            {
                IsCountdownStarted = false;
                Debug.Log("GAME STARTING");
                DiskSpawnManager.Instance.SpawnDisks(_spawnSize);
                UnPauseGame();
                SetGameCounters();
                SetGameTimer();
                SetLevelText();
            }
        }
    }

    private void StartTimer()
    {
        if (IsGameStarted && !IsGamePaused && IsPlayerControllable)
        {
            TimeSpent += Time.deltaTime;
            SetMoveText();
            SetTimerText();
        }
    }

    private void SetGameCounters()
    {
        _gameUI.SetActive(true);
    }

    private void SetGameTimer()
    {
        TimeSpent = 0;
        IsGameStarted = true;
    }

    private void SetTimerText()
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(TimeSpent);
        string formattedTime = string.Format("{0:mm\\:ss}", timeSpan);
        _timerText.text = $"Time: {formattedTime}";
    }

    private void SetMoveText()
    {
        _moveText.text = $"Moves: {Moves}";
    }

    private void SetLevelText()
    {
        _levelText.text = $"Level {_level}";
    }

    private void InitGameStart()
    {
        Debug.Log("Init game start");
        Moves = 0;
        TimeSpent = 0f;
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
                //if Last level was 0 (no games yet) set to min spawn size
                //if last level was 6 (current level is last level) set to 6 + min spawn size = 9
                _spawnSize = _levelmanager.GetLastLevel() + _minSpawnSize;
                _spawnSize = _spawnSize > _maxSpawnSize ? _maxSpawnSize : _spawnSize;
                _level = _levelmanager.GetLastLevel() + 1;
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

    private void ZenModePopUp()
    {
        if(_gameMode != GameMode.ZenMode)
        {
            return;
        }

        if (_spawnCountText.isActiveAndEnabled)
        {
            bool isNumber = int.TryParse(_spawnCountText.text, out int spawnCount);
            if (spawnCount < _minSpawnSize || !isNumber)
            {
                _spawnCountText.text = _minSpawnSize.ToString();
            }
            else if (spawnCount > _maxSpawnSize)
            {
                _spawnCountText.text = _maxSpawnSize.ToString();
            }
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
        Debug.Log("Pausing Game");
        IsGamePaused = true;
        IsPlayerControllable = false;
    }

    public void UnPauseGame()
    {
        Debug.Log("Unpausing Game");
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
        else if (_gameMode == GameMode.Standard)
        {
            GameOver();
            _levelmanager.SetLevelClearData(Moves, TimeSpent);
            ReturnToMainMenu();
            GoToNextLevel();
        }
    }

    private void GameOver()
    {
        IsGameStarted = false;
        _gameUI.SetActive(false);
    }

    public void GameOverCallback()
    {
        //add the call back on the finished baking popup
    }

    private void ResetGame()
    {
        Debug.Log("Reset");
        _gameUI.SetActive(false);
        _mainMenuUI.SetActive(true);
        _successUI.SetActive(false);
        _errorPrompt.gameObject.SetActive(false);

        PauseGame();
    }

    public void ReturnToMainMenu()
    {
        ResetGame();
        BuildSceneManager.Instance.LoadSceneAsync(BuildScene.MainMenuScene);
    }

    public void GoToNextLevel()
    {
        var maximumLevel = _maxSpawnSize - _minSpawnSize - 1;
        if(_levelmanager.GetLastLevel() < maximumLevel)
        {
            Debug.Log($"Can go to next level: Next level is{ _levelmanager.GetLastLevel()+1}");
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

