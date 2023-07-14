using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class GameInput : MonoBehaviour
{
    private InputSystem _inputSystem;

    public event EventHandler OnInteractAction;

    public static GameInput Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        //enables the player input system
        _inputSystem = new();
        _inputSystem.Player.Enable();

        //setting up the event system for the interact button
        _inputSystem.Player.Interact.performed += Interact_performed; ;
    }

    //Event system for Interact Button (Space)
    private void Interact_performed(InputAction.CallbackContext obj)
    {
        //!!TODO: ADD GAME PAUSE / GAME CONTROLLABLE
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    //Returns the vector3 for the movement input using the new input system
    public Vector3 GetMovementInputNormalized()
    {
        Vector2 moveInputXY = _inputSystem.Player.Move.ReadValue<Vector2>().normalized;

        Vector3 moveInput = new(moveInputXY.x, 0f, moveInputXY.y);

        return moveInput;
    }


}
