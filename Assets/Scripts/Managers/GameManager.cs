using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject _successUI;
    [SerializeField] private ErrorPromptController _errorPrompt;


    public bool IsGamePaused = true;
    public bool IsPlayerControllable = true;


    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void StartGame()
    {
        //!!TODO: Add more logic for starting the game here
        IsGamePaused = false;
        DiskSpawnManager.Instance.SpawnDisks();
    }

    public void ErrorPrompt(ErrorType errorType)
    {
        _errorPrompt.ErrorPrompt(errorType);
    }

    public void SuccessPrompt()
    {
        _successUI.SetActive(false);
        _successUI.SetActive(true);
    }
}
