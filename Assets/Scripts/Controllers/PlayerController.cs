using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _turnSpeed = 10f;

    private bool _isMoving;

    private void Update()
    {
        MovementController();
    }

    private void MovementController()
    {
        Vector3 moveInput = GameInput.Instance.GetMovementInputNormalized();


        transform.position += moveInput * _moveSpeed * Time.deltaTime;
        transform.forward = Vector3.Slerp(transform.forward, moveInput, _turnSpeed * Time.deltaTime);

        _isMoving = moveInput != Vector3.zero;
    }

    public bool IsMoving()
    {
        return _isMoving;
    }
}
