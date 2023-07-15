using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FTUEWaypointController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject);
        if (other.gameObject.GetComponent<PlayerController>())
        {
            //Pause the game
            GameManager.Instance.PauseGame();
            FTUEManager.Instance.InteractTutorialStart();
            gameObject.SetActive(false);
        }
    }
}
