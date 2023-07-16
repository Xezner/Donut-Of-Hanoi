using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class FTUEManager : MonoBehaviour
{
    [Header("Controls Tutorial")]
    [SerializeField] private GameObject _controlsTutorial;
    [SerializeField] private GameObject _controlsWayPoint;
    [SerializeField] private FTUEWaypointController _waypointController;

    [Header("Interact Tutorial")]
    [SerializeField] private GameObject _interactTutorial;

    [Header("Pickup Tutorial")]
    [SerializeField] private GameObject _pickupTutorial;

    [Header("Place Tutorial")]
    [SerializeField] private GameObject _placeTutorial;

    [Header("How To Play Tutorial")]
    [SerializeField] private GameObject _howToPlayTutorial;

    [Header("Finished Tutorial")]
    [SerializeField] private GameObject _finishedTutorial;


    private int _minSpawnSize = 3;

    public static FTUEManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Instance_OnInteractedCounterChanged(object sender, PlayerController.OnInteractCounterChangedEventArgs counterChangedEvent)
    {
        if (counterChangedEvent.InteractedCounter != null)
        {
            if (!GetBool(FTUE.InteractTutorial))
            {
                return;
            }

            if (!GetBool(FTUE.PickUpTutorial))
            {
                PickupTutorialStart();
            }
            else if(!GetBool(FTUE.PlaceTutorial) && InteractionManager.Instance.GetPoppedDisk())
            {
                PlaceTutorialStart();
            }
            else if (GetBool(FTUE.PlaceTutorial) && InteractionManager.Instance.GetPoppedDisk() == null)
            {
                HowToPlayTutorialStart();
            }
        }
    }

    private void PauseAndStartTutorial(GameObject tutorial)
    {
        GameManager.Instance.PauseGame();
        tutorial.SetActive(true);
    }

    private void UnpauseAndUpdateFTUE(FTUE ftue)
    {
        GameManager.Instance.UnPauseGame();
        SetBool(ftue, true);
    }

    public void ControlsTutorialStart()
    {
        _controlsTutorial.SetActive(true);
        PlayerController.Instance.OnInteractedCounterChanged += Instance_OnInteractedCounterChanged;
    }

    public void ControlsTutorialCallback()
    {
        _controlsWayPoint.SetActive(true);

        UnpauseAndUpdateFTUE(FTUE.MovementTutorial);
    }

    public void InteractTutorialStart()
    {
        _controlsWayPoint.SetActive(false);

        PauseAndStartTutorial(_interactTutorial);        
    }

    public void InteractTutorialCallback()
    {
        DiskSpawnManager.Instance.SpawnDisks(_minSpawnSize);

        UnpauseAndUpdateFTUE(FTUE.InteractTutorial);
    }

    private void PickupTutorialStart()
    {
        PauseAndStartTutorial(_pickupTutorial);
    }

    public void PickupTutorialCallback()
    {
        UnpauseAndUpdateFTUE(FTUE.PickUpTutorial);
    }

    private void PlaceTutorialStart()
    {
        PauseAndStartTutorial(_placeTutorial);
    }

    public void PlaceTutorialCallback()
    {
        UnpauseAndUpdateFTUE(FTUE.PlaceTutorial);
    }

    private void HowToPlayTutorialStart()
    {
        PauseAndStartTutorial(_howToPlayTutorial);
        PlayerController.Instance.OnInteractedCounterChanged -= Instance_OnInteractedCounterChanged;
    }

    public void HowToPlayCallback()
    {
        UnpauseAndUpdateFTUE(FTUE.PlaceTutorial);
    }

    public void FinishedTutorialStart()
    {
        PauseAndStartTutorial(_finishedTutorial);
    }

    public void FinishedTutorialCallback()
    {
        UnpauseAndUpdateFTUE(FTUE.FinishedTutorial);
        BuildSceneManager.Instance.LoadSceneAsync(BuildScene.MainMenuScene);
    }

    public void SetBool(FTUE ftue, bool value)
    {
        string key = ftue.ToString();
        int intValue = value ? 1 : 0;
        PlayerPrefs.SetInt(key, intValue);
    }

    public bool GetBool(FTUE ftue)
    {
        string key = ftue.ToString();
        int intValue = PlayerPrefs.GetInt(key, 0);
        return intValue != 0;
    }

    public void SetInt(FTUE ftue, int value)
    {
        string key = ftue.ToString();
        PlayerPrefs.SetInt(key, value);
    }

    public int GetInt(FTUE ftue)
    {
        string key = ftue.ToString();
        int intValue = PlayerPrefs.GetInt(key, 1);
        return intValue + 2;
    }

    public void SetString(FTUE ftue, string value)
    {
        string key = ftue.ToString();
        PlayerPrefs.SetString(key, value);
    }

    public string GetString(FTUE ftue)
    {
        string key = ftue.ToString();
        return PlayerPrefs.GetString(key);
    }

    public void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
    }
}

public enum FTUE
{
    FirstStart,
    MovementTutorial,
    InteractTutorial,
    PickUpTutorial,
    PlaceTutorial,
    GameloopTutorial,
    FirstErrorPrompt,
    FinishedTutorial,
    Level,
    LevelData
}
