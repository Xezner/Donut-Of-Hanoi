using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    private InputSystem inputSystem;

    public static GameInput Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        inputSystem = new();
        inputSystem.Player.Enable();
    }

    public Vector3 GetMovementInputNormalized()
    {
        Vector2 moveInputXY = inputSystem.Player.Move.ReadValue<Vector2>().normalized;

        Vector3 moveInput = new(moveInputXY.x, 0f, moveInputXY.y);

        return moveInput;
    }


}
