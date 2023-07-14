using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelecetedObjectToggler : MonoBehaviour
{
    [SerializeField] private CounterObject _counterObject;
    [SerializeField] private GameObject _selectedOverlay;
    void Start()
    {
        //Subscribe to the interacted counter changed event
        PlayerController.Instance.OnInteractedCounterChanged += Instance_OnInteractedCounterChanged;
    }

    //Shows a display overlay if interacted counter is the same as this object, hides if not
    private void Instance_OnInteractedCounterChanged(object sender, PlayerController.OnInteractCounterChangedEventArgs counterChangedEvent)
    {
        if(counterChangedEvent.InteractedCounter == _counterObject)
        {
            ShowSelectedOverlay();
        }
        else
        {
            HideSelectedOverlay();
        }
    }

    public void ShowSelectedOverlay()
    {
        _selectedOverlay.gameObject.SetActive(true);
    }

    public void HideSelectedOverlay()
    {
        _selectedOverlay.gameObject.SetActive(false);
    }
}
