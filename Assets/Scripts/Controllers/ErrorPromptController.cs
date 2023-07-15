using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ErrorPromptController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] PlayerController _player;

    [Header("Error Objects")]
    [SerializeField] GameObject _errorPrompt;
    [SerializeField] ErrorMessage _errorMessage;
    [SerializeField] TextMeshProUGUI _errorText;
    [Header("Follow Data")]
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

    public void GetPlayer()
    {
        _player = PlayerController.Instance;
    }

    public void ErrorPrompt(ErrorType errorType)
    {
        _errorPrompt.SetActive(false);
        _errorPrompt.SetActive(true);
        _errorText.text = _errorMessage.ErrorMessageDictionary[errorType];
    }
}
