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
    [SerializeField] List<GameObject> _spawnList = new List<GameObject>();

    private void Start()
    {
        SpawnDisks();
    }

    private void SetSpawnSize()
    {
        if (_spawnSize != _spawnList.Count)
        {
            _spawnList.RemoveRange(_spawnSize, _spawnList.Count - _spawnSize);
        }
    }

    public void SpawnDisks()
    {
        SetSpawnSize();
        bool initialSpawn = true;
        float spawnOffset = _spawnOffsetHeight;
        foreach(GameObject disk in _spawnList)
        {
            Debug.Log(disk.name);
            Vector3 spawnPosition = _spawnPosition.position + new Vector3(0f, initialSpawn ? _initialSpawnHeight: spawnOffset, 0f);
            Instantiate(disk, spawnPosition, Quaternion.identity, transform);
            spawnOffset += _spawnOffsetHeight;
            initialSpawn = false;
        }
    }
}
