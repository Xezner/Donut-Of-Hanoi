using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskSpawnManager : MonoBehaviour
{
    [Header("Target Spawn Point")]
    [SerializeField] private Transform _spawnPosition;

    [Header("Spawn Values")]
    [SerializeField] private float _initialSpawnHeight = 1f;
    [SerializeField] private float _spawnOffsetHeight = 0.5f;
    [SerializeField] private int _spawnSize = 9;

    [Header("Spawn List")]
    [SerializeField] List<GameObject> _spawnList = new();
    [SerializeField] Stack<GameObject> _spawnStack = new();


    public static DiskSpawnManager Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        SpawnDisks();
    }

    private void SetSpawnSize()
    {
        _spawnStack = new(_spawnList);
        foreach(GameObject spawn in _spawnStack)
        {
            Debug.Log(spawn.name);
        }
        if (_spawnSize < _spawnStack.Count)
        {
            int itemsToRemove = _spawnStack.Count - _spawnSize;
            for(int i = 0; i < itemsToRemove; i++)
            {
                _spawnStack.Pop();
            }
            //_spawnList.RemoveRange(_spawnSize, _spawnList.Count - _spawnSize);
        }
    }

    public void SpawnDisks()
    {
        SetSpawnSize();
        bool initialSpawn = true;
        float spawnOffset = _spawnOffsetHeight;
        List<GameObject> spawnList = new(_spawnStack);
        for (int i = spawnList.Count - 1; i >= 0; i--)
        {
            Debug.Log(spawnList[i].name);
            Vector3 spawnPosition = _spawnPosition.position + new Vector3(0f, initialSpawn ? _initialSpawnHeight: spawnOffset, 0f);
            Instantiate(spawnList[i], spawnPosition, Quaternion.identity, transform);
            spawnOffset += _spawnOffsetHeight;
            initialSpawn = false;
        }
    }

    public Stack<GameObject> GetDiskStack()
    {
        return _spawnStack;
    }
}
