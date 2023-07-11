using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
public class DragController : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private float _lerpDuration = 0.2f;
    [SerializeField] private float _elapsedTime = 0f;
    private float _bufferTime = 0f;
    private GameObject _draggedObject;
    public Vector2 _savedMousePosition;
    public Vector3 _initialPosition;
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

    private void OnDrag()
    {
        if (_draggedObject != null && Input.GetMouseButton(0))
        {
            DiskLiftAnimation();

            if (_bufferTime < 0.2f)
            {
                var center = _camera.ScreenToWorldPoint(new Vector2(_draggedObject.GetComponent<Renderer>().bounds.center.x, _draggedObject.transform.position.y));
                Cursor.SetCursor(null, center, CursorMode.Auto);
                _bufferTime += Time.deltaTime;
            }

            if (_elapsedTime >= _lerpDuration)
            {
                Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, _camera.WorldToScreenPoint(_draggedObject.transform.position).z);
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(position);
                _draggedObject.transform.position = new Vector3(worldPosition.x, 2.5f, 0);
            }
        }
    }

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
        _elapsedTime = 0f;
        _bufferTime = 0f;
        _rigidbody.isKinematic = true;
        _savedMousePosition = Input.mousePosition;
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
        if (_elapsedTime < _lerpDuration)
        {
            _elapsedTime += Time.deltaTime;
            float time = _elapsedTime / _lerpDuration;
            Vector3 targetPosition = new Vector3(_initialPosition.x, 2.5f, 0);
            Vector3 currentPosition = new Vector3(_draggedObject.transform.position.x, _initialPosition.y, 0);
            _draggedObject.transform.position = Vector3.Lerp(currentPosition, targetPosition, time);
            var center = _camera.ScreenToWorldPoint(new Vector2(_draggedObject.GetComponent<Renderer>().bounds.center.x, _draggedObject.transform.position.y));
            Cursor.SetCursor(null, center, CursorMode.Auto);
        }
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
