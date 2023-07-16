using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private List<LevelData> _levelDataList = new();
    private int _currentLevel = 0;

    private LevelManager Instance;
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

    public void SetLevelData(List<LevelData> levelData)
    {
        string levelDataJSON = JsonConvert.SerializeObject(levelData);
        Debug.Log($"Setting Level Data List: {levelDataJSON}");
        FTUEManager.Instance.SetString(FTUE.LevelData, levelDataJSON);
    }

    public List<LevelData> GetLevelDataList()
    {
        string levelDataJSON = FTUEManager.Instance.GetString(FTUE.LevelData);
        Debug.Log($"Getting level Data List: {levelDataJSON}");

        List<LevelData> levelData = JsonConvert.DeserializeObject<List<LevelData>>(levelDataJSON);
        if (levelData != null && levelData.Count > 0)
        {
            foreach (LevelData level in levelData)
            {
                Debug.Log(JsonConvert.SerializeObject(level));
            }
            _levelDataList = levelData;
            return levelData;
        }
        _levelDataList = new();
        return levelData;
    }

    public int GetLastLevel()
    {
        List<LevelData> levelData = GetLevelDataList();
        if (levelData != null && levelData.Count > 0)
        {
            return  levelData.Last().Level;
        }
        return 0;
    }

    public LevelData GetLastLevelData()
    {
        List<LevelData> levelData = GetLevelDataList();
        if(levelData != null && levelData.Count > 0)
        {
            return levelData.Last();
        }

        return new()
        {
            Level = 0,
            Moves = 0,
            Time = 0
        };
    }

    public void SetLevelClearData(int level, int moves, float time)
    {
        LevelData levelCleared = new()
        {
            Level = level,
            Moves = moves,
            Time = time
        };
        bool isDataExisting = false;

        //Replace existing data
        if(_levelDataList != null && _levelDataList.Count > 0)
        {
            foreach(LevelData levelData in _levelDataList)
            {
                if(levelData.Level == level)
                {
                    Debug.Log($"Replacing Data: {JsonConvert.SerializeObject(levelData)}");
                    levelData.Level = level;
                    levelData.Moves = levelData.Moves < moves ? levelData.Moves : moves;
                    levelData.Time = levelData.Time < time? levelData.Time : time;
                    isDataExisting = true;
                    break;
                }
            }
        }

        //Adds new data
        if(!isDataExisting)
        {
            Debug.Log($"Adding: {JsonConvert.SerializeObject(levelCleared)}");
            _levelDataList.Add(levelCleared);
        }

        Debug.Log($"Newly modifiedt list: {JsonConvert.SerializeObject(_levelDataList)}");

        SetLevelData(_levelDataList);
    }

    public void SetLastLevelClearData(int moves, float time)
    {
        _currentLevel = GetLastLevel() + 1;
        LevelData levelCleared = new()
        {
            Level = _currentLevel,
            Moves = moves,
            Time = time
        };
        Debug.Log(JsonConvert.SerializeObject(levelCleared));
        _levelDataList.Add(levelCleared);

        SetLevelData(_levelDataList);
    }

    public LevelData GetLevelData(int level)
    {
        if (_levelDataList != null && _levelDataList.Count > 0)
        { 
            foreach (LevelData levelData in _levelDataList)
            {
                Debug.Log($"GameOver Getting All Level Data: {JsonConvert.SerializeObject(levelData)}");
                if(levelData.Level == level)
                {
                    Debug.Log($"Data get, returning existing data");
                    return levelData;
                }
            }
        }

        Debug.Log($"No Data get, returning default data 0,0,0");
        return new() 
        { 
            Level = 0,
            Moves = 0,
            Time = 0
        };
    }

    public void ClearLevelData()
    {
        SetLevelData(new());
    }


    public void DisplayStuff()
    {
        Debug.Log(GetLastLevel());
    }

}
[Serializable]
public class LevelData
{
    public int Level { get; set; }
    public int Moves { get; set; }
    public float Time { get; set; }
}
