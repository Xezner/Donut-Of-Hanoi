using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiskHolder : MonoBehaviour
{
    [SerializeField]private List<GameObject> _diskList = new();
    private Stack<GameObject> _diskStack = new();

    public void PushDisk(GameObject disk)
    {
        _diskList.Add(disk);
        _diskStack.Push(disk);
    }

    public GameObject PopDisk()
    {
        if (_diskStack.Count > 0)
        {
            GameObject disk = _diskStack.Pop();
            List<GameObject> poppedList = _diskList.ToList();
            poppedList.Remove(disk);
            _diskList = poppedList;
            return disk;
        }
        else
        {
            return null;
        }
    }


    public GameObject GetLastDiskOnStack()
    {
        if (_diskStack != null && _diskStack.Count > 0)
        {
            return _diskStack.Peek();
        }
        return null;
    }

    public GameObject GetPoppedDisk()
    {
        if (_diskStack.Count > 0)
        {
            GameObject disk = _diskStack.Pop();
            _diskList.Remove(disk);
            return disk;
        }
        else
        {
            return null;
        }
    }

    public Stack<GameObject> GetDiskStack()
    {
        return new(_diskStack);
    }

    public void SetDiskStack(Stack<GameObject> diskStack)
    {
        _diskList = new(diskStack);
        _diskStack = diskStack;
    }
}
