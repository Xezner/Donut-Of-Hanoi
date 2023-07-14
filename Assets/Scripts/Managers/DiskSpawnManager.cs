using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskSpawnManager : MonoBehaviour
{
    [Header("Target Spawn Point")]
    [SerializeField] private Transform _spawnPosition;
    [SerializeField] private DiskHolder _spawnDiskHolder;

    [Header("Spawn Values")]
    [SerializeField] private float _initialSpawnHeight = 1f;
    [SerializeField] private float _spawnOffsetHeight = 0.5f;
    [SerializeField] private int _spawnSize = 9;

    [Header("Spawn List")]
    [SerializeField] List<DiskData> _diskDataList = new();
    List<GameObject> _spawnList = new();
    Stack<GameObject> _spawnStack = new();


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
        //!!TODO: MOVE SPAWN DISKS TO GAME MANAGER
        SpawnDisks();
    }

    private void InitSpawnList()
    {
        foreach(DiskData diskData in _diskDataList)
        {
            diskData.DiskObject.SetDiskValue();
            _spawnList.Add(diskData.DiskPrefab);
        }
        _spawnStack = new(_spawnList);
    }

    private void SetSpawnSize(int spawnSize)
    {
        InitSpawnList();
        if (spawnSize < _spawnStack.Count)
        {
            int itemsToRemove = _spawnStack.Count - spawnSize;
            for(int i = 0; i < itemsToRemove; i++)
            {
                _spawnStack.Pop();
            }
        }  
    }

    public void SpawnDisks()
    {
        SetSpawnSize(_spawnSize);

        bool initialSpawn = true;
        float spawnOffset = _spawnOffsetHeight;

        List<GameObject> spawnList = new(_spawnStack);
        Stack<GameObject> spawnStack = new();

        for (int i = spawnList.Count - 1; i >= 0; i--)
        {
            Vector3 spawnOffsetPosition = new(0f, initialSpawn ? _initialSpawnHeight : spawnOffset, 0f);
            Vector3 spawnPosition = _spawnPosition.position + spawnOffsetPosition;

            GameObject spawn = Instantiate(spawnList[i], spawnPosition, Quaternion.identity, _spawnPosition.transform);

            spawnStack.Push(spawn);

            spawnOffset += _spawnOffsetHeight;

            initialSpawn = false;
        }
        _spawnStack = new(spawnStack);
        _spawnDiskHolder.SetDiskStack(spawnStack);
    }

    public Stack<GameObject> GetDiskStack()
    {
        return _spawnStack;
    }
}
