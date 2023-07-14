using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CounterObject : MonoBehaviour
{
    [SerializeField] DiskHolder _diskHolder;
    [SerializeField] GameObject _selectedOverlay;
    [SerializeField] private bool _isDeliveryCounter;
    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if(_isDeliveryCounter)
        {
            CheckDiskHolderStack();
        }
    }


    private void CheckDiskHolderStack()
    {
        bool isStackEqual = _diskHolder.GetDiskStack().SequenceEqual(DiskSpawnManager.Instance.GetDiskStack());
        if (isStackEqual)
        {
            Debug.Log("TRUE");
        }
    }

    public void SetSelectedOverlay()
    {
        _selectedOverlay.gameObject.SetActive(true);
    }

    public void ResetSelectedOverlay()
    {
        _selectedOverlay.gameObject.SetActive(false);
    }
}
