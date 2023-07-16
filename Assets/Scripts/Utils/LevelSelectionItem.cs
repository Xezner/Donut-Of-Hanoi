using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _movesText;
    [SerializeField] private TextMeshProUGUI _timeText;
    [SerializeField] private GameObject _lockObject;
    [SerializeField] private Button _levelSelectionButton;

    public void PopulateData(LevelData levelData)
    {
        bool isLocked = levelData.Level == 0;
        string levelText = isLocked ? "???" : levelData.Level.ToString();
        _levelText.text = $"LEVEL {levelText}";

        string moveText = isLocked ? "???" : levelData.Moves.ToString();
        _movesText.text = $"MOVES: {moveText}";

        TimeSpan timeSpan = TimeSpan.FromSeconds(levelData.Time);
        string formattedTime = string.Format("{0:mm\\:ss}", timeSpan);
        string timeText = isLocked ? "???" : formattedTime;
        _timeText.text = $"TIME: {timeText}";

        
        _lockObject.SetActive(isLocked);
        _levelSelectionButton.interactable = !isLocked;

    }
}
