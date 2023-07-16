using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private List<LevelData> _levelDataList = new();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetLevelData(List<LevelData> levelData)
    {
        //levelData.Add(new()
        //{
        //    Level = 1,
        //    Moves = 10,
        //    Time = 10.5f
        //});

        //levelData.Add(new()
        //{
        //    Level = 2,
        //    Moves = 20,
        //    Time = 11.5f
        //});

        //levelData.Add(new()
        //{
        //    Level = 3,
        //    Moves = 50,
        //    Time = 23.5f
        //});

        //levelData.Clear();
        string levelDataJSON = JsonConvert.SerializeObject(levelData);
        Debug.Log(levelDataJSON);
        FTUEManager.Instance.SetString(FTUE.LevelData, levelDataJSON);
    }

    public List<LevelData> GetLevelDataList()
    {
        string levelDataJSON = FTUEManager.Instance.GetString(FTUE.LevelData);
        Debug.Log($"{levelDataJSON}");

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

    public void DisplayStuff()
    {
        Debug.Log(GetLastLevel());
    }

    public void SetLevelClearData(int moves, float time)
    {
        LevelData levelCleared = new()
        {
            Level = GetLastLevel() + 1,
            Moves = moves,
            Time = time
        };
        Debug.Log(JsonConvert.SerializeObject(levelCleared));
        _levelDataList.Add(levelCleared);

        SetLevelData(_levelDataList);
    }

    public void ClearLevelData()
    {
        SetLevelData(new());
    }
}
[Serializable]
public class LevelData
{
    public int Level { get; set; }
    public int Moves { get; set; }
    public float Time { get; set; }
}
