using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] Texture2D _cursorTextre;
    private Vector2 _cursorHotSpot = Vector2.zero;

    private CursorManager Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        Cursor.SetCursor(_cursorTextre, _cursorHotSpot, CursorMode.Auto);
    }
}
