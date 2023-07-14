using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DiskData", menuName = "ScriptableObject/DiskData")]
public class DiskData : ScriptableObject
{
    public GameObject DiskPrefab;
    public DiskObject DiskObject;
    public int DiskValue;
}
