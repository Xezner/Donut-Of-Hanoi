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
        //Only subscribe to the event if the counter object is the delivery counter
        if (_isDeliveryCounter)
        {
            InteractionManager.Instance.OnDiskStackChange += Instance_OnDiskStackChange;
        }
    }

    //If disk stack change for the delivery counter, check if the solution is correct
    private void Instance_OnDiskStackChange(object sender, InteractionManager.OnDiskStackChangeEventArgs diskStackChangeEvent)
    {
        if(_isDeliveryCounter)
        {
            CheckDiskHolderStack();
        }
    }

    private void CheckDiskHolderStack()
    {
        Stack<GameObject> deliveryCounterStack = new(_diskHolder.GetDiskStack());
        Stack<GameObject> targetStack = new(DiskSpawnManager.Instance.GetDiskStack());

        bool isStackEqual = deliveryCounterStack.SequenceEqual(targetStack);

        if (isStackEqual)
        {
            Debug.Log("Delivery Success! Good job!");
            GameManager.Instance.SuccessPrompt(this.transform.position);

            GameManager.Instance.GameOverScreen();
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
