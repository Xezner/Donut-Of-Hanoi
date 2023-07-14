using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static PlayerController;

public class PlayerController : MonoBehaviour
{
    #region Variables
    [Header("Player Movement")]
    [SerializeField] private float _moveSpeed = 7f;
    [SerializeField] private float _turnSpeed = 10f;

    [Header("Player Data")]
    [SerializeField] private float _playerRadius = 0.7f;
    [SerializeField] private float _playerHeight = 2f;

    [Header("Interaction Data")]
    [SerializeField] private float _interactionDistance = 2f;
    [SerializeField] private LayerMask _interactedLayerMask;


    private Vector3 _lastInteractionDirection;
    private CounterObject _lastInteractedCounter;
    private bool _isMoving;

    public static PlayerController Instance;
    #endregion

    #region Interaction Change Event
    //Event for setting up interaction changes
    public event EventHandler<OnInteractCounterChangedEventArgs> OnInteractedCounterChanged;
    public class OnInteractCounterChangedEventArgs : EventArgs
    {
        public CounterObject InteractedCounter;
    }
    #endregion

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        //Subscribe to the OnInteractAction event
        GameInput.Instance.OnInteractAction += Instance_OnInteractAction;
    }

    private void Update()
    {
        //!!TODO: ADD GAME PAUSE / GAME CONTROLLABLE
        InteractionHandler();
        MovementController();
    }

    private void MovementController()
    {
        Vector3 moveInput = GameInput.Instance.GetMovementInputNormalized();
        Vector3 moveDirection = moveInput;
        float moveDistance = _moveSpeed * Time.deltaTime;

        Vector3 playerHeadPosition = transform.position + Vector3.up * _playerHeight;

        PlayerData playerData = new()
        {
            PlayerPosition = transform.position,
            PlayerHeadPosition = playerHeadPosition,
            MoveDirection = moveInput,
            PlayerRadius = _playerRadius,
            MoveDistance = moveDistance
        };

        //Check if player can move
        bool canMove = CanMove(playerData);

        //Cannove move towards direction
        if(!canMove)
        {
            CheckForCollision(playerData, ref canMove, ref moveInput);
        }

        if (canMove)
        {
            transform.position += moveInput * moveDistance;
        }

        _isMoving = moveInput != Vector3.zero;

        moveDirection = _isMoving ? moveInput : moveDirection;
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, _turnSpeed * Time.deltaTime);

        
    }

    private void CheckForCollision(PlayerData playerData, ref bool canMove, ref Vector3 moveInput)
    {
        //Check if X axis is movable
        Vector3 movementX = new Vector3(moveInput.x, 0, 0).normalized;
        playerData.MoveDirection = movementX;

        canMove = CanMove(playerData);
        if (canMove)
        {
            moveInput = movementX;
        }

        //if X axis is not movable then move towards z axis
        else
        {
            Vector3 movementZ = new Vector3(0, 0, moveInput.z).normalized;
            playerData.MoveDirection = movementZ;

            canMove = CanMove(playerData);
            if (canMove)
            {
                moveInput = movementZ;
            }
        }
    }

    private bool CanMove(PlayerData playerData)
    {
        Vector3 playerPosition = playerData.PlayerPosition;
        Vector3 playerHeadPosition = playerData.PlayerHeadPosition;
        Vector3 moveDirection = playerData.MoveDirection;
        float playerRadius = playerData.PlayerRadius;
        float moveDistance = playerData.MoveDistance;

        //Player position = pivot point
        //Player head position = top position of the object
        //Player radius = size of the object
        return !Physics.CapsuleCast(playerPosition, playerHeadPosition, playerRadius, moveDirection, moveDistance);
    }

    private void InteractionHandler()
    {
        Vector3 moveInput = GameInput.Instance.GetMovementInputNormalized();

        if(moveInput != Vector3.zero)
        {
            _lastInteractionDirection = moveInput;
        }

        if (Physics.Raycast(transform.position, _lastInteractionDirection, out RaycastHit raycastHit, _interactionDistance, _interactedLayerMask))
        {
            if (raycastHit.transform.parent.TryGetComponent(out CounterObject counterObject))
            {
                if (counterObject != _lastInteractedCounter)
                {
                    SetInteractedCounter(counterObject);
                }
            }
            else
            {
                SetInteractedCounter(null);
            }
        }
        else
        {
            SetInteractedCounter(null);
        }
    }

    private void SetInteractedCounter(CounterObject counterObject)
    {
        _lastInteractedCounter = counterObject;

        OnInteractedCounterChanged?.Invoke(this, new OnInteractCounterChangedEventArgs
        {
            InteractedCounter = counterObject
        });
    }

    private void Instance_OnInteractAction(object sender, System.EventArgs e)
    {
        if(_lastInteractedCounter != null)
        {
            InteractionManager.Instance.InteractOnCounter(_lastInteractedCounter);
        }
        
    }

    public bool IsMoving()
    {
        return _isMoving;
    }

    private struct PlayerData
    {
        public Vector3 PlayerPosition;
        public Vector3 PlayerHeadPosition;
        public Vector3 MoveDirection;
        public float PlayerRadius;
        public float MoveDistance;
    }
}
