using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskHolder : MonoBehaviour
{
    private Stack<GameObject> _diskStack = new();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }




    public Stack<GameObject> GetDiskStack()
    {
        return _diskStack;
    }
}
