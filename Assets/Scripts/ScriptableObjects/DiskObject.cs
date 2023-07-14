using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskObject : MonoBehaviour
{
    [SerializeField] private DiskData _diskData;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Collider _collider;

    public int DiskValue;

    public void SetDiskValue()
    {
        DiskValue = _diskData.DiskValue;
    }

    public void SetDiskToKinematic()
    {
        _rigidbody.isKinematic = true;
        _collider.enabled = false;
    }

    public void ResetDiskToDynamic()
    {
        _rigidbody.isKinematic = false;
        _collider.enabled = true;
    }

    public void SetDiskTransform(Transform targetDiskHolder)
    {
        transform.SetParent(targetDiskHolder, true);
        transform.position = targetDiskHolder.position;
    }

    public void ResetDiskTransform(Transform topDisk, Transform diskParent)
    {
        transform.SetParent(diskParent, true);

        float heightOffset = 0.5f;
        transform.position = new(topDisk.position.x, topDisk.position.y + heightOffset, topDisk.position.z);
    }
}
