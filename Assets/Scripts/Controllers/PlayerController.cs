using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 7f;


    private bool _isMoving;

    private void Update()
    {
        MovementController();
    }

    private void MovementController()
    {
        Vector3 moveInput = Vector3.zero;

        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            moveInput.z = 1;
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            moveInput.z = -1;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            moveInput.x = -1;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            moveInput.x = 1;
        }
 
        transform.position += moveInput.normalized * _moveSpeed * Time.deltaTime;
        transform.forward = Vector3.Slerp(transform.forward, moveInput, Time.deltaTime * 10f);

        _isMoving = moveInput != Vector3.zero;
    }

    public bool IsMoving()
    {
        return _isMoving;
    }
}
