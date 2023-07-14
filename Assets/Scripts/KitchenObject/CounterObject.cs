using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CounterObject : MonoBehaviour
{
    [SerializeField] DiskHolder _diskHolder;
    [SerializeField] private bool _isDeliveryCounter;

    private void Start()
    {
        if (_isDeliveryCounter)
        {
            InteractionManager.Instance.OnDiskStackChange += Instance_OnDiskStackChange;
        }
    }

    private void Instance_OnDiskStackChange(object sender, InteractionManager.OnDiskStackChangeEventArgs diskStackChangeEvent)
    {
        if(_isDeliveryCounter)
        {
            CheckDiskHolderStack();
        }
    }

    private void CheckDiskHolderStack()
    {
        bool isStackEqual = _diskHolder.GetDiskStack().SequenceEqual(DiskSpawnManager.Instance.GetDiskStack());
        
        foreach(GameObject disk in _diskHolder.GetDiskStack())
        {
            Debug.Log($"This Disk: {disk}");
        }

        foreach(GameObject disk in DiskSpawnManager.Instance.GetDiskStack())
        {
            Debug.Log($"Spawner's Disk: {disk}");
        }

        if (isStackEqual)
        {
            //Add game over logic here
            Debug.Log("TRUE");
        }
        else
        {
            Debug.Log("Stack is not equal. Keep Trying");
        }
    }

    public DiskHolder GetDiskHolder()
    {
        return _diskHolder;
    }
}
