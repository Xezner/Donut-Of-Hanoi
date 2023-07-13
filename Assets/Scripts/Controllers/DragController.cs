using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
public class DragController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Camera _camera;

    [Header("RigidBody")]
    [SerializeField] private Rigidbody _rigidbody;

    [Header("Position Values")]
    [SerializeField] private float _maxVerticalHeight = 3.5f;

    [Header("Animation Values")]
    [SerializeField] private float _animationDuration = 0.2f;
    [SerializeField] private float _animationElapsedTime = 0f;

    [SerializeField] private float _bufferDuration = 0.2f;
    [SerializeField] private float _elapsedBufferTime = 0f;
    private GameObject _draggedObject;
    [SerializeField] private Vector3 _initialPosition;
    private void Update()
    {
        DragHandler();
    }

    private void DragHandler()
    {
        OnDragStart();
        OnDragEnd();
        OnDrag();
    }

    //Handles drag start event
    private void OnDragStart()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_draggedObject == null)
            {
                RaycastHit hit = GetRayCastHit();

                if (hit.collider != null && hit.collider.gameObject == this.gameObject)
                {
                    if (!hit.collider.CompareTag("DragObject"))
                    {
                        return;
                    }
                    InitDragValues(hit.collider.gameObject);
                }
            }
        }
    }

    //Handles while dragging event
    private void OnDrag()
    {
        //make sure that there is an object being dragged
        if (_draggedObject != null && Input.GetMouseButton(0))
        {
            bool isDiskLiftAnimationOver = _animationElapsedTime >= _animationDuration;
            bool isBufferTimerOver = _elapsedBufferTime >= _bufferDuration;
            

            if (!isDiskLiftAnimationOver)
            {
                DiskLiftAnimation();
            }
            else if (!isBufferTimerOver)
            {
                var center = _camera.WorldToScreenPoint(new Vector2(_draggedObject.GetComponent<Renderer>().bounds.center.x, _draggedObject.transform.position.y));
                Debug.Log(center);
                Debug.Log(_draggedObject.GetComponent<Renderer>().bounds.center.x);
                Cursor.SetCursor(null, center, CursorMode.Auto);
                _elapsedBufferTime += Time.deltaTime;
            }
            else
            {
                Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _camera.WorldToScreenPoint(_draggedObject.transform.position).z);
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
                _draggedObject.transform.position = new Vector3(worldPosition.x, _maxVerticalHeight, _initialPosition.z);
            }
        }
    }

    //Handles drag end event
    private void OnDragEnd()
    {
        if (Input.GetMouseButtonUp(0))
        {
            ResetDragValues();
        }
    }

    private void InitDragValues(GameObject targetObject)
    {
        _draggedObject = targetObject;
        _initialPosition = _draggedObject.transform.position;
        _animationElapsedTime = 0f;
        _elapsedBufferTime = 0f;
        _rigidbody.isKinematic = true;
        Cursor.visible = false;
    }

    private void ResetDragValues()
    {
        Cursor.lockState = CursorLockMode.None;
        _rigidbody.isKinematic = false;
        _draggedObject = null;
        Cursor.visible = true;
    }

    private void DiskLiftAnimation()
    {
        //Slowly lerp the position for a seamless lift animation
        _animationElapsedTime += Time.deltaTime;
        float time = _animationElapsedTime / _animationDuration;

        Vector3 targetPosition = new Vector3(_initialPosition.x, _maxVerticalHeight, _initialPosition.z);
        Vector3 currentPosition = new Vector3(_draggedObject.transform.position.x, _initialPosition.y, _initialPosition.z);
        _draggedObject.transform.position = Vector3.Lerp(currentPosition, targetPosition, time);

        //Sets the cursor position to the center of the object (disabling movement of cursor)
        var center = _camera.WorldToScreenPoint(new Vector2(_draggedObject.GetComponent<Renderer>().bounds.center.x, _draggedObject.transform.position.y));
        Cursor.SetCursor(null, center, CursorMode.Auto);
    }

    private RaycastHit GetRayCastHit()
    {
        //Get your mouse position on your screen (0,0) is the bottom left corner of the screen
        Vector3 screenMousePositionFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _camera.farClipPlane);
        Vector3 screenMousePositionNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _camera.nearClipPlane);

        //Converts our screen space positions to the world space positions
        Vector3 worldMousePositionFar = _camera.ScreenToWorldPoint(screenMousePositionFar);
        Vector3 worldMousePositionNear = _camera.ScreenToWorldPoint(screenMousePositionNear);

        Physics.Raycast(worldMousePositionNear, worldMousePositionFar - worldMousePositionNear, out RaycastHit hit);

        return hit;
    }
}
