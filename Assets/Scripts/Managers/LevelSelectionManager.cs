using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectionManager : MonoBehaviour
{
    [SerializeField] LevelManager _levelmanager;
    [SerializeField] List<LevelSelectionItem> _levelSelectionItem;
    

    public void PopulateLevelSelectionItems()
    {
        List<LevelData> levelDataList = _levelmanager.GetLevelDataList();

        int lastLevel = _levelmanager.GetLastLevel();
        //lastLevel = lastLevel == 0 ? 0 : lastLevel + 1;
        Debug.Log($"LastLevel {lastLevel}");
        for(int i = 0; i < _levelSelectionItem.Count; i++)
        {
            if (levelDataList != null && i < levelDataList.Count)
            {
                _levelSelectionItem[i].PopulateData(levelDataList[i]);
            }
            else
            {
                _levelSelectionItem[i].PopulateData(new()
                {
                    Level = 
                    i == lastLevel ? i == 0 ? 1 : lastLevel + 1 : 0,
                    Moves = 0,
                    Time = 0,
                });
            }
        }
    }

    private void OnDisable()
    {
        Debug.Log("Disabled");
    }
}
