using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverAnimator : MonoBehaviour
{
    public void GameOverCallback()
    {
        GameManager.Instance.GameOverCallback();
    }
}
