using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
        _inputSystem.Player.Pause.performed += Pause_performed;
    }

   
    //Event system for Interact Button (Space)
    private void Interact_performed(InputAction.CallbackContext obj)
    {
        //Only enable this if the game is not paused or the character is controllable
        if(GameManager.Instance.IsGamePaused && GameManager.Instance.IsPlayerControllable)
        {
            return;
        }

        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    //Even system for pause button (ESC)
    private void Pause_performed(InputAction.CallbackContext obj)
    {
        if(SceneManager.GetActiveScene().name == BuildScene.MainMenuScene.ToString())
        {
            return;
        }
        GameManager.Instance.PauseGame();
    }

    //Returns the vector3 for the movement input using the new input system
    public Vector3 GetMovementInputNormalized()
    {
        Vector2 moveInputXY = _inputSystem.Player.Move.ReadValue<Vector2>().normalized;

        Vector3 moveInput = new(moveInputXY.x, 0f, moveInputXY.y);

        return moveInput;
    }


}
