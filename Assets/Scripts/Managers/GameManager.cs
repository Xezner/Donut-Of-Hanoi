using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    //CHEAT
    public bool ResetFTUE = false;

    [Header("UI MainMenu")]
    [SerializeField] private GameObject _mainMenuUI;
    [SerializeField] private GameObject _tutorialButton;
    [SerializeField] private GameObject _startButton;
    [SerializeField] private GameObject _zenModeButton;

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

    [Header("UI Game Over")]
    [SerializeField] private GameObject _gameOverUI;
    [SerializeField] private TextMeshProUGUI _gameOverMoveText;
    [SerializeField] private GameObject _movePersonalBest;
    [SerializeField] private TextMeshProUGUI _gameOverTimeText;
    [SerializeField] private GameObject _timePersonalBest;
    [SerializeField] private TextMeshProUGUI _gameOverLevelText;
    [SerializeField] private GameObject _nextLevelButton;

    [Header("UI Pause")]
    [SerializeField] private GameObject _pauseUI;
    [SerializeField] private GameObject _pauseButton;
    [SerializeField] private GameObject _resumeButton;

    [Header("UI Countdown")]
    [SerializeField] private GameObject _countDownUI;
    [SerializeField] private TextMeshProUGUI _countDownText;

    //GameMode
    private GameMode _gameMode;
    public int _currentLevel = 0;

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
    public bool IsPausedByUI = false;

    //Singleton
    public static GameManager Instance;
    private void Awake()
    {
        Debug.Log("AWAKE");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (ResetFTUE)
        {
            FTUEManager.Instance.ClearPlayerPrefs();
        }
        InitMenuButtons();
        

        //Subscribe to scene loaded events
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }



    //Event handler
    private void SceneManager_sceneLoaded(Scene loadedScene, LoadSceneMode arg1)
    {
        if (loadedScene.name == BuildScene.MainMenuScene.ToString())
        {
            InitMenuButtons();
            ResetGame();
        }
        else if (loadedScene.name == BuildScene.GameScene.ToString())
        {
            _levelmanager.GetLevelDataList();
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

  
    private void StartCountdown()
    {
        if (IsCountdownStarted && _gameMode != GameMode.FTUE)
        {
            SetCountDownText(true);
            _countdown -= Time.deltaTime;
            if (_countdown <= 0f)
            {
                IsCountdownStarted = false;
                Debug.Log("GAME STARTING");
                DiskSpawnManager.Instance.SpawnDisks(_spawnSize);
                UnPauseGame();
                SetCountDownText(false);
                SetGameCounters();
                SetGameTimer();
                SetLevelText();
            }
        }
    }

    private void SetCountDownText(bool isActive)
    {
        _countDownUI.SetActive(isActive);
        float count = _countdown + 1;
        _countDownText.text = count >= _countdownTimer ? "3": count.ToString("0");
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

    private void InitMenuButtons()
    {
        bool isTutorialFinished = FTUEManager.Instance.GetBool(FTUE.FinishedTutorial);
        _tutorialButton.SetActive(!isTutorialFinished);
        _startButton.SetActive(isTutorialFinished);
        _zenModeButton.SetActive(isTutorialFinished);
    }

    private void SetGameCountdown()
    {
        PauseGame();
        _countdown = _countdownTimer;
        IsCountdownStarted = true;
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
        bool isZenMode = _gameMode == GameMode.ZenMode;
        string levelString = isZenMode ? "ZEN" : _level.ToString();
        _levelText.text = $"Level {levelString}";
    }

    private void InitGameStart()
    {
        Debug.Log("Init game start");
        PauseGame();
        Moves = 0;
        TimeSpent = 0f;
        _errorPrompt.gameObject.SetActive(true);
        _errorPrompt.GetPlayer();
        _worldSpaceCanvas.worldCamera = Camera.main;

        GetSpawnSizeByGameMode();
    }

    private void GetSpawnSizeByGameMode()
    {
        switch (_gameMode)
        {
            case GameMode.Standard:
            {
                SetStandardSpawnSize();
                break;
            }
            case GameMode.ZenMode:
            {
                SetZenModeSpawnSize();
                break;
            }
            default: break;
        }
    }

    private void SetStandardSpawnSize()
    {
        //spawnSize = currentLevel + Minspawnsize (3) - 1 to get minimum of 3 level at level 1
        _spawnSize = _currentLevel + _minSpawnSize - 1;
        _spawnSize = _spawnSize > _maxSpawnSize ? _maxSpawnSize : _spawnSize;
        _level = _currentLevel;
        SetGameCountdown();
    }

    private void SetZenModeSpawnSize()
    {
        bool isNumber = int.TryParse(_spawnCountText.text, out _spawnSize);
        if (!isNumber)
        {
            _spawnSize = _minSpawnSize;
        }

        SetGameCountdown();
    }

    private void ZenModePopUp()
    {
        if (_gameMode != GameMode.ZenMode)
        {
            return;
        }

        if (_spawnCountText.isActiveAndEnabled)
        {
            if(string.IsNullOrWhiteSpace(_spawnCountText.text))
            {
                return;
            }

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
            _gameMode = GameMode.FTUE;
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
        IsPausedByUI = true;
    }

    public void UnPauseGame()
    {
        Debug.Log("Unpausing Game");
        IsGamePaused = false;
        IsPlayerControllable = true;
        IsPausedByUI = false;
    }

    public void PauseGameToggle()
    {
        IsGamePaused = !IsGamePaused;
        IsPlayerControllable = !IsPlayerControllable;

        _pauseButton.SetActive(!IsGamePaused);
        _pauseUI.SetActive(IsGamePaused);
        _resumeButton.SetActive(IsGamePaused);

        Debug.Log($"Game Paused: {IsGamePaused}");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void GameOverScreen()
    {
        PauseGame();
        if (!FTUEManager.Instance.GetBool(FTUE.FinishedTutorial) && SceneManager.GetActiveScene().name == BuildScene.FTUEScene.ToString())
        {
            FTUEManager.Instance.FinishedTutorialStart();
        }
        else if (_gameMode == GameMode.Standard)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        IsGameStarted = false;
        _gameUI.SetActive(false);
    }
    public void GameOverCallback()
    {
        GameOver();
        Debug.Log($"GameOverCallback: {_gameMode}");
        if(_gameMode == GameMode.FTUE)
        {
            return;
        }

        _gameOverUI.SetActive(true);

        bool isZenMode = _gameMode == GameMode.ZenMode;
        SetGameOverLevelUI(_levelmanager.GetLevelData(_currentLevel), isZenMode);
        _levelText.gameObject.SetActive(!isZenMode);
        _nextLevelButton.SetActive(!isZenMode);

        if (_gameMode == GameMode.Standard)
        {
            _levelmanager.SetLevelClearData(_currentLevel, Moves, TimeSpent);
            CheckForNextLevel();
        }
    }

    private void SetGameOverLevelUI(LevelData currentLevelData, bool isZenMode)
    {
        Debug.Log("GameOver Current Level: " + JsonConvert.SerializeObject(currentLevelData));
        TimeSpan timeSpan = TimeSpan.FromSeconds(TimeSpent);
        string formattedTime = string.Format("{0:mm\\:ss}", timeSpan);
        _gameOverTimeText.text = $"Time: {formattedTime}";
        
        _gameOverMoveText.text = $"Moves: {Moves}";

        _gameOverLevelText.gameObject.SetActive(!isZenMode);
        _gameOverLevelText.text = $"Level {_currentLevel}";

        if (!isZenMode)
        {
            bool isPersonalBestMove = currentLevelData.Moves == 0 || Moves < currentLevelData.Moves;
            _movePersonalBest.SetActive(isPersonalBestMove);

            bool isPersonalBestTime = currentLevelData.Time == 0 || TimeSpent < currentLevelData.Time;
            _timePersonalBest.SetActive(isPersonalBestTime);
        }
    }
    public void ResetGame(bool isNextLevel = true)
    {
        Debug.Log("Reset");
        _gameUI.SetActive(false);
        _mainMenuUI.SetActive(isNextLevel);
        _successUI.SetActive(false);
        _errorPrompt.gameObject.SetActive(false);

        PauseGame();
    }

    public void ReturnToMainMenu()
    {
        BuildSceneManager.Instance.LoadSceneAsync(BuildScene.MainMenuScene);
    }

    private void CheckForNextLevel()
    {
        var maximumLevel = 7;
        _nextLevelButton.SetActive(_levelmanager.GetLastLevel() < maximumLevel);
        Debug.Log($"Can go to next level: Next level is {_levelmanager.GetLastLevel() + 1}");
    }

    public void GoToNextLevel()
    {
        _currentLevel++;
        ResetGame(false);
        StartGame();
            
    }

    public void SetCurrentLevel(int level)
    {
        Debug.Log($"Current Level: {level}");
        _currentLevel = level;
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

