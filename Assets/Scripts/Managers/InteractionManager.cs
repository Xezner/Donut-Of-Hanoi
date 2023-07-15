using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlayerController;

public class InteractionManager : MonoBehaviour
{
    [Header("Player Disk")]
    [SerializeField] private Transform _playerDiskHolder;

    private GameObject _poppedDisk;
    private DiskObject _poppedDiskObject;

    public event EventHandler<OnDiskStackChangeEventArgs> OnDiskStackChange;
    public class OnDiskStackChangeEventArgs : EventArgs
    {
        public DiskObject DiskObject;
    }

    public static InteractionManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public GameObject GetPoppedDisk()
    {
        return _poppedDisk;
    }

    //Handles interaction state if take(pickup) or place
    public void InteractOnCounter(CounterObject counterObject)
    {
        if(_poppedDisk == null)
        {
            TakeDiskOnCounter(counterObject);
        }
        else
        {
            PlaceDiskOnCounter(counterObject);
        }
    }

    //Takes the disk on the counter if no disk is being held
    public void TakeDiskOnCounter(CounterObject counterObject)
    {
        _poppedDisk = counterObject.GetDiskHolder().GetPoppedDisk();
        if(_poppedDisk == null)
        {
            GameManager.Instance.ErrorPrompt(ErrorType.EmptyCounter);
            return;
        }
        _poppedDiskObject = _poppedDisk.GetComponent<DiskObject>();
        SetDiskToPlayer();
    }

    //Places the disk back on the counter if a disk is being held
    public void PlaceDiskOnCounter(CounterObject counterObject)
    {
        DiskHolder diskHolder = counterObject.GetDiskHolder();
        GameObject topDisk = diskHolder.GetLastDiskOnStack();

        if (topDisk && _poppedDiskObject.DiskValue > topDisk.GetComponent<DiskObject>().DiskValue)
        {
            GameManager.Instance.ErrorPrompt(ErrorType.CantPlaceHere);
            Debug.Log("Cannot Place This Object on a smaller disk");
            return;
        }

        Transform topDiskPosition = topDisk ? topDisk.transform : diskHolder.transform;
        SetDiskToCounter(topDiskPosition, diskHolder.transform);

        diskHolder.PushDisk(_poppedDisk);
        _poppedDisk = null;

        InvokeEvent();
    }

    //Moves the disk from the counter to the player
    private void SetDiskToPlayer()
    {
        if (_poppedDisk)
        {
            _poppedDiskObject.SetDiskToKinematic();
            _poppedDiskObject.SetDiskTransform(_playerDiskHolder);
        }
    }

    //Moves the disk from the player back to the counter
    private void SetDiskToCounter(Transform topDiskPosition, Transform diskHolder)
    {
        _poppedDiskObject.ResetDiskTransform(topDiskPosition, diskHolder);
        _poppedDiskObject.ResetDiskToDynamic();
    }

    //Invoke the on disk stack change event
    private void InvokeEvent()
    {
        OnDiskStackChange?.Invoke(this, new OnDiskStackChangeEventArgs
        {
            DiskObject = _poppedDiskObject
        });
    }
}
