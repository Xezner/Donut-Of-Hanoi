using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] PlayerController _player;
    [SerializeField] Vector3 _offset;
    [SerializeField] float _followSpeed;
    private void LateUpdate()
    {
        if(GameManager.Instance.IsGamePaused)
        {
            return;
        }

        Vector3 targetPosition = _player.transform.position + _offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _followSpeed);
    }
}
